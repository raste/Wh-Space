﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;

namespace WhSpace
{
    public partial class BasePage : System.Web.UI.Page
    {
        public BasePage()
        {
            this.PreInit += new EventHandler(BasePage_PreInit);
            this.Unload += new EventHandler(BasePage_Unload);
            this.PreRenderComplete += new EventHandler(BasePage_PreRenderComplete);
            this.Load += new EventHandler(BasePage_Load);
            this.PreRender += new EventHandler(BasePage_PreRender);
        }

        private void BasePage_PreInit(object sender, EventArgs e)
        {

        }

        void BasePage_Unload(object sender, EventArgs e)
        {
        }

        void BasePage_PreRenderComplete(object sender, EventArgs e)
        {
        }

        void BasePage_PreRender(object sender, EventArgs e)
        {
        }

        protected void BasePage_Load(object sender, EventArgs e)
        {
            if (Session[UiSessionParams.SessionVisitAdded] == null)
            {
                Session[UiSessionParams.SessionVisitAdded] = true;

                Entities objectContext = new Entities();
                BStatistics bStatistics = new BStatistics();
                bStatistics.NewVisit(objectContext);
            }
        }

        protected void SaveSettings(Language lang, User currUser, bool redirrect, bool onlyIfStayLoggedIn)
        {
            if ((onlyIfStayLoggedIn == true && Session[UiSessionParams.SessionStayLoggedIn] != null) || onlyIfStayLoggedIn == false)
            {
                UiCookie.SaveSettings(Session, Response, Request, lang, currUser);

                if (redirrect == true)
                {
                    Response.Redirect(Request.Url.ToString());
                }
            }
        }


        protected override void InitializeCulture()
        {
            base.InitializeCulture();

            Entities objectContext = new Entities();
            Tools.ChangeUiCultureFromSessionAndCookie(objectContext);
        }

        protected Corporation GetUserCorporation(Entities objectContext ,User user, bool redirrectIfNull)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            Corporation userCorp = null;

            if (user.globalAdmin == true)
            {
                if (Session[UiSessionParams.SessionCurrCorporationID] != null)
                {
                    long id = 0;
                    if (long.TryParse(Session[UiSessionParams.SessionCurrCorporationID].ToString(), out id) == true)
                    {
                        Tools.AssertObjectContextExists(objectContext);

                        BCorporation bCorp = new BCorporation();
                        userCorp = bCorp.Get(objectContext, id, false);
                    }
                }
            }
            else
            {
                if (!user.RCorporationReference.IsLoaded)
                {
                    user.RCorporationReference.Load();
                }

                userCorp = user.RCorporation;
            }

            if (userCorp == null && redirrectIfNull)
            {
                Session[UiSessionParams.SessionCurrentUserID] = null;
                Session[UiSessionParams.SessionCurrCorporationID] = null;
                Response.Redirect("Index.aspx");
            }

            return userCorp;

        }

        protected User GetCurrUser(Entities objectContext, bool redirrectIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            User currUser = null;

            object objUserId = Session[UiSessionParams.SessionCurrentUserID];
            if (objUserId != null)
            {
                string strId = objUserId.ToString();
                long id = 0;

                if (long.TryParse(strId, out id) == true)
                {
                    BUser bUser = new BUser();
                    currUser = bUser.Get(objectContext, id, true, false);
                }
            }
            else
            {
                currUser = UiCookie.GetUser(objectContext, Session);

                if (currUser != null)
                {
                    Session[UiSessionParams.SessionCurrentUserID] = currUser.ID;
                }
            }

            if (redirrectIfNull == true && currUser == null)
            {
                Session[UiSessionParams.SessionCurrentUserID] = null;
                Session[UiSessionParams.SessionCurrCorporationID] = null;
                Response.Redirect("Index.aspx");
            }

            return currUser;
        }

        public void LogOut(Entities objectContext, bool redirrectToIndex)
        {
            Tools.AssertObjectContextExists(objectContext);

            BUser bUser = new BUser();
            bUser.DeleteUserLogInHash(objectContext, GetCurrUser(objectContext, true));

            // zapisvane cookie settings nanovo
            Language currLang = UiCookie.GetStringAsLanguage(Session[UiSessionParams.SessionLangParam].ToString());
            SaveSettings(currLang, null, false, true);

            Session[UiSessionParams.SessionCurrentUserID] = null;
            Session[UiSessionParams.SessionCurrCorporationID] = null;

            if (redirrectToIndex == true)
            {
                Response.Redirect("Index.aspx");
            }

            

        }

    }
}