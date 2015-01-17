﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

namespace WhSpace
{
    public class BCorporation
    {

        public void Add(Entities objectContext, string corpName, Language logsLanguade, string systemName, string username, string password)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(corpName))
            {
                throw new ArgumentException("corpname is null or empty");
            }
            if (string.IsNullOrEmpty(systemName))
            {
                throw new ArgumentException("systemName is null or empty");
            }
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username is null or empty");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("password is null or empty");
            }

            if (IsThereCorporationWithName(objectContext, corpName) == true)
            {
                throw new InvalidOperationException("there is already corp with same name.");
            }

            string strLang = UiCookie.GetLanguageAsString(logsLanguade);

            BUser bUser = new BUser();
            User user = objectContext.UserSet.FirstOrDefault(usr => usr.name == username);
            if (user != null && user.visible == true)
            {
                throw new InvalidOperationException("there is already user with same name");
            }

            // MAIN CORPORATION Admin
            if (user == null)
            {
                user = new User();

                user.admin = true;
                user.canEdit = true;
                user.RCorporation = null;
                user.globalAdmin = false;
                user.name = username;
                user.password = bUser.GetHashed(password);
                user.visible = true;

                objectContext.AddToUserSet(user);
                Tools.Save(objectContext);
            }
            else
            {
                user.admin = true;
                user.canEdit = true;
                user.RCorporation = null;
                user.globalAdmin = false;
                user.password = bUser.GetHashed(password);
                user.visible = true;

                Tools.Save(objectContext);
            }

            // Corporation
            Corporation newCorporation = new Corporation();

            newCorporation.name = corpName;
            newCorporation.RAddedBy = user;
            newCorporation.dateAdded = DateTime.UtcNow;
            newCorporation.logsLanguage = strLang;

            objectContext.AddToCorporationSet(newCorporation);
            Tools.Save(objectContext);

            //
            user.RCorporation = newCorporation;
            Tools.Save(objectContext);
            //

            BSystem bSystem = new BSystem();

            // ROOT system
            Systems newSystem = new Systems();

            newSystem.RCorporation = newCorporation;
            newSystem.Users = user;
            newSystem.dateAdded = DateTime.UtcNow;
            newSystem.info = string.Empty;
            newSystem.name = systemName;
            newSystem.occupied = string.Empty;
            newSystem.root = true;
            newSystem.sysClass = null;
            newSystem.sysEffect = null;

            objectContext.AddToSystemsSet(newSystem);
            Tools.Save(objectContext);

            string msg = string.Format("{0} {1} {2}", Tools.GetLogResource("corporationRegister", newCorporation)
                , newCorporation.name, Tools.GetLogResource("corporationRegister2", newCorporation));
            BLog.Add(objectContext, newCorporation, user, msg);

        }

        public void ChangeCorporationName(Entities objectContext, Corporation currCorporation, User currUser, string newname)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
            if (string.IsNullOrEmpty(newname))
            {
                throw new ArgumentException("newname is null or empty");
            }

            if(BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("user cannot edit corporation.");
            }

            if (IsThereCorporationWithName(objectContext, newname) == true)
            {
                throw new InvalidOperationException("name already taken");
            }

            string oldName = currCorporation.name;

            currCorporation.name = newname;
            Tools.Save(objectContext);

            string msg = string.Format("{0} {1}.", Tools.GetLogResource("corporationChangeName", currCorporation), oldName);
            BLog.Add(objectContext, currCorporation, currUser, msg);
        }

        public void ChangeCorporationLogsLanguage(Entities objectContext, Corporation currCorporation, User currUser, Language newLanguage)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
           

            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("user cannot edit corporation.");
            }

            string strNewLang = UiCookie.GetLanguageAsString(newLanguage);
            if (strNewLang.ToLowerInvariant() == currCorporation.logsLanguage.ToLowerInvariant())
            {
                throw new InvalidOperationException("new language is same as old");
            }

            string oldLang = UiCookie.GetLanguageFullName(currCorporation.logsLanguage);

            currCorporation.logsLanguage = strNewLang;
            Tools.Save(objectContext);

            string msg = string.Format("{0} {1}.", Tools.GetLogResource("corporationChangeLogsLang", currCorporation), oldLang);
            BLog.Add(objectContext, currCorporation, currUser, msg);
        }

        public static Corporation GetUserCorporation(User user, bool throwExcIfCorpIsNull)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (!user.RCorporationReference.IsLoaded)
            {
                user.RCorporationReference.Load();
            }

            if (user.RCorporation == null && throwExcIfCorpIsNull == true)
            {
                throw new InvalidOperationException(string.Format("User ID: {0}, dont have corporation reference", user.ID));
            }

            return user.RCorporation;
        }

        public Corporation Get(Entities objectContext, long id, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            Corporation corporation = objectContext.CorporationSet.FirstOrDefault(corp => corp.ID == id);

            if (corporation == null && throwExcIfNull == true)
            {
                throw new ArgumentException(string.Format("no corporation with ID: {0}", id));
            }

            return corporation;
        }

        public List<Corporation> GetCorporations(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<Corporation> corporations = objectContext.CorporationSet.OrderBy(corp => corp.name).ToList();

            return corporations;
        }

        public bool IsThereCorporationWithName(Entities objectContext, string name)
        {
            Tools.AssertObjectContextExists(objectContext);

            bool result = false;

            Corporation corp = objectContext.CorporationSet.FirstOrDefault(cp => cp.name == name);
            if (corp != null)
            {
                result = true;
            }

            return result;
        }

        public int CountCorporations(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            return objectContext.CorporationSet.Count();
        }


    }
}