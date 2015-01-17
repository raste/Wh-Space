﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WhSpace
{
    public class BUser
    {
        public void AddUser(Entities objectContext, User currUser, Corporation currCorporation, string username, string password, bool admin, bool canEdit)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("password");
            }

            User dbuser = objectContext.UserSet.FirstOrDefault(usr => usr.name == username);
            if (dbuser == null)
            {
                User newUser = new User();

                newUser.visible = true;
                newUser.globalAdmin = false;
                newUser.name = username;
                newUser.password = GetHashed(password);
                newUser.admin = admin;
                newUser.canEdit = canEdit;
                newUser.RCorporation = currCorporation;

                objectContext.AddToUserSet(newUser);
                Tools.Save(objectContext);
            }
            else
            {
                if (dbuser.visible == true)
                {
                    return;
                }

                dbuser.visible = true;

                dbuser.name = username;
                dbuser.password = GetHashed(password);
                dbuser.admin = admin;
                dbuser.canEdit = canEdit;
                dbuser.RCorporation = currCorporation;

                Tools.Save(objectContext);
            }

            string log = string.Format("{0} {1}.", Tools.GetLogResource("memberAddNew", currCorporation), username);
            BLog.Add(objectContext, currCorporation, currUser, log);

        }

        public void EditUserType(Entities objectContext, Corporation currCorporation, User currUser, User selectedUser, bool admin, bool canEdit)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            if (selectedUser == null)
            {
                throw new ArgumentNullException("selectedUser");
            }

            selectedUser.admin = admin;
            selectedUser.canEdit = canEdit;

            Tools.Save(objectContext);

            string log = string.Format("{0} {1}.", Tools.GetLogResource("memberEditType", currCorporation), selectedUser.name);
            BLog.Add(objectContext, currCorporation, currUser, log);
        }

        public void ChangeUserPassword(Entities objectContext, User currUser, string currPass, string newPass)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (string.IsNullOrEmpty(currPass))
            {
                throw new ArgumentException("currPass is null or empty");
            }
            if (string.IsNullOrEmpty(newPass))
            {
                throw new ArgumentException("newPass is null or empty");
            }

            if (currPass == newPass)
            {
                throw new InvalidOperationException("currPass == newPass");
            }

            if (currUser.password != GetHashed(currPass))
            {
                throw new InvalidOperationException("currPass is not actual");
            }

            currUser.password = GetHashed(newPass);
            Tools.Save(objectContext);

            ResetUserLogInHash(objectContext, currUser);
        }

        public void ChangeCorporationAdmin(Entities objectContext, Corporation currCorporation, User currUser, User selectedUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            if (selectedUser == null)
            {
                throw new ArgumentNullException("selectedUser");
            }

            if (currUser.globalAdmin == false)
            {
                return;
            }

            if (selectedUser.globalAdmin == true)
            {
                return;
            }

            if (selectedUser.ID == currUser.ID)
            {
                return;
            }

            if (!selectedUser.RCorporationReference.IsLoaded)
            {
                selectedUser.RCorporationReference.Load();
            }

            if (selectedUser.RCorporation == null)
            {
                throw new InvalidOperationException("selected user's corporation is null");
            }

            User corpAdmin = GetCorporationMainAdmin(currCorporation);
            if (corpAdmin.ID == selectedUser.ID)
            {
                return;
            }

            currCorporation.RAddedBy = selectedUser;
            selectedUser.canEdit = true;
            Tools.Save(objectContext);

            string msg = string.Format("{0} {1} {2} {3}.", Tools.GetLogResource("corporationAdminChange", currCorporation)
                , selectedUser.name, Tools.GetLogResource("corporationAdminChange1", currCorporation), corpAdmin.name);

            BLog.Add(objectContext, currCorporation, currUser, msg);

        }

        public void DeleteUser(Entities objectContext, Corporation currCorporation, User currUser, User selectedUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            if (selectedUser == null)
            {
                throw new ArgumentNullException("selectedUser");
            }

            if (selectedUser.globalAdmin == true)
            {
                return;
            }

            if (selectedUser.ID == currUser.ID)
            {
                return;
            }

            BFeedback bFeedback = new BFeedback();
            bFeedback.DeleteUserFeedback(objectContext, selectedUser);

            selectedUser.visible = false;
            Tools.Save(objectContext);

            string log = string.Format("{0} {1}.", Tools.GetLogResource("memberDelete", currCorporation), selectedUser.name);
            BLog.Add(objectContext, currCorporation, currUser, log);
        }


        public User Get(Entities objectContext, string username, string password)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("password");
            }

            string hashedPass = GetHashed(password);

            User user = objectContext.UserSet.FirstOrDefault(usr => usr.name == username && usr.password == hashedPass && usr.visible == true);

            return user;
        }

        public User Get(Entities objectContext, long id, string hash)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (string.IsNullOrEmpty(hash))
            {
                throw new ArgumentException("hash");
            }

            User user = objectContext.UserSet.FirstOrDefault(usr => usr.ID == id && usr.logInHash == hash && usr.visible == true);

            return user;
        }

        public User Get(Entities objectContext, string username)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username");
            }

            User user = objectContext.UserSet.FirstOrDefault(usr => usr.name == username && usr.visible == true);

            return user;
        }

        public User Get(Entities objectContext, long id, bool onlyVisible, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            User user = null;

            if (onlyVisible == true)
            {
                user = objectContext.UserSet.FirstOrDefault(usr => usr.ID == id && usr.visible == true);
            }
            else
            {
                user = objectContext.UserSet.FirstOrDefault(usr => usr.ID == id);
            }

            if (throwExcIfNull == true && user == null)
            {
                throw new InvalidOperationException(string.Format("no user with id = {0}", id));
            }

            return user;
        }

        public List<User> GetVisibleUsers(Entities objectContext, Corporation currCorporation)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            List<User> users = objectContext.UserSet.Where(usr => usr.visible == true && usr.RCorporation.ID == currCorporation.ID).ToList();
            if (users != null && users.Count > 0)
            {
                IEnumerable<User> ieUsers = users.OrderBy(usr => usr.name);
                users = ieUsers.ToList();
            }

            return users;
        }

        public List<User> GetUsersWhichCanBeEdited(Entities objectContext, User currUser, Corporation currCorporation)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            List<User> users = new List<User>();

            if (currUser.globalAdmin == true)
            {
                users = objectContext.UserSet.Where(usr => usr.RCorporation.ID == currCorporation.ID && usr.visible == true
                    && usr.globalAdmin == false).ToList();
            }
            else
            {
                User corpMainAdmin = GetCorporationMainAdmin(currCorporation);

                users = objectContext.UserSet.Where(usr => usr.RCorporation.ID == currCorporation.ID && usr.visible == true
                    && usr.globalAdmin == false && usr.ID != currUser.ID && usr.ID != corpMainAdmin.ID).ToList();
            }

            if (users != null && users.Count > 1)
            {
                users = users.OrderBy(usr => usr.name).ToList();
            }

            return users;
        }

        public bool AreThereUsersWhichCanBeEdited(Entities objectContext, User currUser, Corporation currCorporation)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            User user = null;

            if (currUser.globalAdmin == true)
            {
                user = objectContext.UserSet.FirstOrDefault(usr => usr.RCorporation.ID == currCorporation.ID && usr.visible == true
                    && usr.globalAdmin == false);
            }
            else
            {
                User corpMainAdmin = GetCorporationMainAdmin(currCorporation);

                user = objectContext.UserSet.FirstOrDefault(usr => usr.RCorporation.ID == currCorporation.ID && usr.visible == true
                    && usr.globalAdmin == false && usr.ID != currUser.ID && usr.ID != corpMainAdmin.ID);
            }

            

            bool result = false;

            if (user != null)
            {
                result = true;
            }

            return result;
        }


        public void CreateUserLogInHash(Entities objectContext, User user, bool onlyIfThereIsntOne)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if ((onlyIfThereIsntOne == true && user.logInHash == null) || onlyIfThereIsntOne == false)
            {
                string hashStr;
                using (System.Security.Cryptography.RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider())
                {
                    byte[] bhash = new byte[50];
                    csp.GetBytes(bhash);

                    hashStr = Convert.ToBase64String(bhash);
                }

                user.logInHash = hashStr;
                Tools.Save(objectContext);
            }
        }

        public void ResetUserLogInHash(Entities objectContext, User user)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            DeleteUserLogInHash(objectContext, user);
            CreateUserLogInHash(objectContext, user, false);
        }

        public void DeleteUserLogInHash(Entities objectContext, User user)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.logInHash = null;
            Tools.Save(objectContext);
        }

        public string GetUserHashedLogIn(User user, bool throwExcIfNull)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            string hash = string.Empty;

            if(!string.IsNullOrEmpty(user.logInHash))
            {
                hash = user.logInHash;
            }

            if(string.IsNullOrEmpty(hash) && throwExcIfNull == true)
            {
                 new ArgumentException(string.Format("User ID : {0} doesn't have saved log in hash.", user.ID));
            }

            return hash;
        }

        public User GetCorporationMainAdmin(Corporation corporation)
        {
            if (corporation == null)
            {
                throw new ArgumentNullException("corporation");
            }

            if (!corporation.RAddedByReference.IsLoaded)
            {
                corporation.RAddedByReference.Load();
            }

            return corporation.RAddedBy;
        }

        public bool IsUserCorpMainAdmin(User user, Corporation corporation)
        {
            if (corporation == null)
            {
                throw new ArgumentNullException("corporation");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            bool result = false;

            if (!corporation.RAddedByReference.IsLoaded)
            {
                corporation.RAddedByReference.Load();
            }

            if (corporation.RAddedBy.ID == user.ID)
            {
                result = true;
            }

            return result;
        }

        public static bool CanUserOperateWithCorporationItems(Corporation corporation, User currUser)
        {
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            bool result = false;

            if (currUser.globalAdmin == true)
            {
                result = true;
            }
            else
            {
                if (corporation == null)
                {
                    throw new ArgumentNullException("corporation");
                }

                if (!currUser.RCorporationReference.IsLoaded)
                {
                    currUser.RCorporationReference.Load();
                }

                if (currUser.RCorporation != null && currUser.RCorporation.ID == corporation.ID && (currUser.canEdit == true || currUser.admin == true))
                {
                    result = true;
                }
            }

            return result;
        }



        public bool IsThereUserWithName(Entities objectContext, string name, bool onlyVisibleTrue)
        {
            Tools.AssertObjectContextExists(objectContext);

            bool result = false;

            User user = null;

            if (onlyVisibleTrue == true)
            {
                user = objectContext.UserSet.FirstOrDefault(usr => usr.name == name && usr.visible == true);
            }
            else
            {
                user = objectContext.UserSet.FirstOrDefault(usr => usr.name == name);
            }

            if (user != null)
            {
                result = true;
            }

            return result;
        }

        public string GetHashed(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("password is null or empty");
            }

            UTF8Encoding encoder = new UTF8Encoding();
            SHA256CryptoServiceProvider sha256hasher = new SHA256CryptoServiceProvider();
            byte[] hashed256bytes = sha256hasher.ComputeHash(encoder.GetBytes(password));

            StringBuilder output = new StringBuilder();
            for (int i = 0; i < hashed256bytes.Length; i++)
            {
                output.Append(hashed256bytes[i].ToString("X2"));
            }

            return output.ToString();
        }

        public int UserCount(Entities objectContext, bool onlyVisible)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (onlyVisible == true)
            {
                return objectContext.UserSet.Count(usr => usr.visible == true);
            }
            else
            {
                return objectContext.UserSet.Count();
            }
        }

        public int CountCorporationMembers(Corporation corporation)
        {
            if (corporation == null)
            {
                throw new ArgumentNullException("corporation");
            }

            corporation.Users.Load();

            return corporation.Users.Count;
        }

        public User GetGuest(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            User guest = objectContext.UserSet.FirstOrDefault(u => u.name == "guest");
            if (guest == null)
            {
                throw new ArgumentNullException("guest");
            }

            return guest;
        }


    }
}