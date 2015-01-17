﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WhSpace
{
    public partial class Index : BasePage
    {
        protected object reggingCorp = new object();

        Entities objectContext = new Entities();

        User currUser = null;
        
        BUser bUser = new BUser();
        BCorporation bCorporation = new BCorporation();
        BStatistics bStatistics = new BStatistics();

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckUser();

            ShowOptions();
            FillDdlCorpLogsLanguage();

            SetLocalText();
            HideStatusPanel();
            ShowStatistics();
        }

        private void ShowStatistics()
        {
            lblVisitsCount.Text = bStatistics.getTodaysVisits(objectContext).ToString();

            lblCorporationsCount.Text = bCorporation.CountCorporations(objectContext).ToString();
            lblUsersCount.Text = bUser.UserCount(objectContext, true).ToString();
        }

        private void FillDdlCorpLogsLanguage()
        {
            if (IsPostBack == false)
            {
                ddlCorpLogsLanguage.Items.Clear();

                ListItem firstItem = new ListItem();
                firstItem.Text = GetGlobalResourceObject("Resources", "ddlChoose").ToString();
                firstItem.Value = "0";
                ddlCorpLogsLanguage.Items.Add(firstItem);

                ddlCorpLogsLanguage.SelectedIndex = 0;

                Array languages = Enum.GetValues(typeof(Language));
                foreach (Language lang in languages)
                {
                    ListItem langItem = new ListItem();
                    langItem.Text = UiCookie.GetLanguageFullName(lang.ToString());
                    langItem.Value = lang.ToString();
                    ddlCorpLogsLanguage.Items.Add(langItem);
                }
            }
           
        }


        private void ShowOptions()
        {

#if DEBUG
            btnDebug.Visible = true;
#endif

            if (currUser != null)
            {
                pnlLogIn.Visible = false;
                pnlRegCorporation.Visible = false;

                pnlLinks.Visible = true;
                pnlLinksS.Visible = false;

                lbRegCorp.Visible = false;

                lbGuestLogIn.Visible = false;
                pnlGuestLogIn.Visible = false;

                lbLogOut.Visible = true;
                pnlLogOut.Visible = true;
                lbLogOut.Controls.Add(pnlLogOut);

                if (GetUserCorporation(objectContext, currUser, false) != null)
                {
                    hlCorporation.Visible = true;
                }
                else
                {
                    hlCorporation.Visible = false;
                }

                if (currUser.globalAdmin == true)
                {
                    hlGAdmin.Visible = true;
                }
                else
                {
                    hlGAdmin.Visible = false;
                }

            }
            else
            {
                pnlLogIn.Visible = true;

                hlCorporation.Visible = false;
                hlGAdmin.Visible = false;
                lbLogOut.Visible = false;

                lbGuestLogIn.Visible = true;
                pnlGuestLogIn.Visible = true;
                lbGuestLogIn.Controls.Add(pnlGuestLogIn);

                lbRegCorp.Visible = true;
                pnlRegCorpLink.Visible = true;
                lbRegCorp.Controls.Add(pnlRegCorpLink);
            }

        }

        private void SetLocalText()
        {
            Title = string.Format("{0} : {1}", GetGlobalResourceObject("Resources", "projectName"), GetLocalResourceObject("title"));

            lblUsername.Text = GetLocalResourceObject("logInUsername").ToString();
            lblPassword.Text = GetLocalResourceObject("logInPassword").ToString();
            btnLogIn.Text = GetLocalResourceObject("logInLogIn").ToString();
            cbStayLogged.Text = GetLocalResourceObject("stayLogged").ToString();

            lblStaticIntefaceLang.Text = GetGlobalResourceObject("Resources", "staticInterfaceLang").ToString();
            ibLangBulgarian.ToolTip = GetGlobalResourceObject("Resources", "langBulgarian").ToString();
            ibLangEnglish.ToolTip = GetGlobalResourceObject("Resources", "langEnglish").ToString();

            lblStaticCorporations.Text = GetLocalResourceObject("staticCorporations").ToString();
            lblStaticVisits.Text = GetLocalResourceObject("staticVisits").ToString();
            lblStaticUsers.Text = GetLocalResourceObject("staticUsers").ToString();

            lblStaticLogOut.Text = GetGlobalResourceObject("Resources", "btnLogOut").ToString();
            lblStaticAboutPage.Text = GetGlobalResourceObject("Resources", "pageAbout").ToString();
            lblStaticCorporationPage.Text = GetGlobalResourceObject("Resources", "pageCorporation").ToString();
            lblStaticFeedBackPage.Text = GetGlobalResourceObject("Resources", "pageFeedback").ToString();
            lblStaticRegCorpLink.Text = GetLocalResourceObject("staticCorporationRegLink").ToString();
            lblStaticGlobalAdminPage.Text = GetGlobalResourceObject("Resources", "pageSiteAdmin").ToString();
            lblStaticGuestLogIn.Text = GetLocalResourceObject("staticGuestLogIn").ToString();

            hlAboutS.ToolTip = GetGlobalResourceObject("Resources", "pageAbout").ToString();
            hlFeedBackS.ToolTip = GetGlobalResourceObject("Resources", "pageFeedback").ToString();

            ///
            lblStaticRegCorporation.Text = GetLocalResourceObject("staticCorporationReg").ToString();
            lblStaticCorporationName.Text = GetLocalResourceObject("staticCorporationName").ToString();
            lblStaticLogsLanguage.Text = GetLocalResourceObject("staticCorporationLogsLang").ToString();
            lblStaticNameRootSystem.Text = GetLocalResourceObject("staticCorporationRootSys").ToString();
            lblStaticNewUsername.Text = GetLocalResourceObject("staticNewUsername").ToString();
            lblStaticNewUserPass.Text = GetLocalResourceObject("staticNewUserPassword").ToString();

            btnCancelReg.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            btnRegCorpAndUser.Text =  GetLocalResourceObject("registerCorpAndUser").ToString();
            ///

            lblStaticStatus.Text = GetGlobalResourceObject("Resources", "StatusStatic").ToString();

        }

        private void HideStatusPanel()
        {
            pnlStatus.Visible = false;
            lblStatus.Text = string.Empty;
        }

        private void ShowStatusPanel(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                throw new ArgumentException("msg is null or empty");
            }

            pnlStatus.Visible = true;
            lblStatus.Text = msg;
        }

        private void CheckUser()
        {

            currUser = GetCurrUser(objectContext, false);

            if (currUser != null)
            {
                if (Request["action"] != null)
                {
                    // log out
                    LogOut(objectContext, true);
                }
                else
                {
                    ShowOptions();
                    // log in
                }
            }


        }

        protected void btnLogIn_Click(object sender, EventArgs e)
        {

            bool errorOccured = false;
            string error = string.Empty;

            if (string.IsNullOrEmpty(tbUsername.Text) || string.IsNullOrEmpty(tbPassword.Text))
            {
                error = GetLocalResourceObject("errEnterNamePass").ToString();
                errorOccured = true;
            }
            else
            {

                User currUser = bUser.Get(objectContext, tbUsername.Text, tbPassword.Text);
                if (currUser != null)
                {

                    // zapisvane cookie settings nanovo
                    if (cbStayLogged.Checked == true)
                    {
                        Session[UiSessionParams.SessionStayLoggedIn] = "true";
                        bUser.CreateUserLogInHash(objectContext, currUser, true);
                        Language currLang = UiCookie.GetStringAsLanguage(Session[UiSessionParams.SessionLangParam].ToString());
                        SaveSettings(currLang, currUser, false, false);
                    }
                   
                    Session[UiSessionParams.SessionCurrentUserID] = currUser.ID;

                    Corporation userCorporation = BCorporation.GetUserCorporation(currUser, false);
                    if (userCorporation != null)
                    {
                        Session[UiSessionParams.SessionCurrCorporationID] = userCorporation.ID.ToString();

                        Response.Redirect("Main.aspx");
                    }

                    //Response.Redirect("Main.aspx");
                    CheckUser();
                    ShowOptions();
                }
                else
                {
                    error = GetLocalResourceObject("errNameOrPass").ToString();
                    errorOccured = true;
                }

                
            }

            if (errorOccured == true)
            {
                lblError.Visible = true;
                lblError.Text = error;
            }
        }

        protected void btnDebug_Click(object sender, EventArgs e)
        {
            ///POINT to the ID's of the user and corporation with which you want to be logged in when you click on that button
            Session[UiSessionParams.SessionCurrentUserID] = 15;
            Session[UiSessionParams.SessionCurrCorporationID] = 2;
            Response.Redirect("Main.aspx");
        }

        protected void ibLangEnglish_Click(object sender, ImageClickEventArgs e)
        {
            SaveSettings(Language.EN, currUser, true, false);
        }

        protected void ibLangBulgarian_Click(object sender, ImageClickEventArgs e)
        {
            SaveSettings(Language.BG, currUser, true, false);
        }

        protected void lbLogOut_Click(object sender, EventArgs e)
        {
            LogOut(objectContext, true);
        }

        protected void btnRegCorpAndUser_Click(object sender, EventArgs e)
        {
            lock (reggingCorp)
            {

                bool errorsOccured = false;
                StringBuilder errors = new StringBuilder();

                string corpName = tbCorpName.Text;
                Language chosenLanguage;
                string username = tbNewUsername.Text;
                string password = tbNewUserPassword.Text;
                string sysName = tbRootSysName.Text;

                CheckRegCorporationParams(out chosenLanguage, ref errors, out errorsOccured);

                if (errorsOccured == false)
                {
                    lblRegCorpErrors.Visible = false;

                    // reg corp and log in
                    bCorporation.Add(objectContext, corpName, chosenLanguage, sysName, username, password);
                    currUser = bUser.Get(objectContext, username, password);
                    
                    if (currUser == null)
                    {
                        throw new ArgumentNullException("currUser");
                    }
                    Corporation currCorporation = BCorporation.GetUserCorporation(currUser, true);

                    Session[UiSessionParams.SessionCurrCorporationID] = currCorporation.ID;
                    Session[UiSessionParams.SessionCurrentUserID] = currUser.ID;

                    bStatistics.CorpRegistered(objectContext);
                    bStatistics.UserRegistered(objectContext);
                    ShowStatistics();

                    // show status panel
                    ShowStatusPanel(GetLocalResourceObject("statusRegCorporation").ToString());

                    pnlRegCorporation.Visible = true;
                    pnlLinks.Visible = true;
                    pnlLinksS.Visible = false;

                    CheckUser();
                    ShowOptions();
                }
                else
                {
                    errors.Replace("<br/>", "<br/> &nbsp;&nbsp;&nbsp;");
                    errors.Insert(0, GetLocalResourceObject("errErrors"));

                    lblRegCorpErrors.Visible = true;
                    lblRegCorpErrors.Text = errors.ToString();
                }
            }
        }

        private void CheckRegCorporationParams(out Language chosenLang, ref StringBuilder errors, out bool errorsOccured)
        {
            errorsOccured = false;
            errors = new StringBuilder();

            string corpName = tbCorpName.Text;
            chosenLang = Language.BG;
            string username = tbNewUsername.Text;
            string password = tbNewUserPassword.Text;
            string sysName = tbRootSysName.Text;

            if (!string.IsNullOrEmpty(corpName))
            {
                if (corpName.Length > Configuration.MaxCorporationNameLength)
                {
                    errorsOccured = true;
                    errors.Append(string.Format("<br/>{0} {1}.", GetLocalResourceObject("errCorporationNameMore")
                        , Configuration.MaxCorporationNameLength));
                }
                else
                {
                    if (bCorporation.IsThereCorporationWithName(objectContext, corpName) == false)
                    {
                        // corp name ok
                    }
                    else
                    {
                        errorsOccured = true;
                        errors.Append(string.Format("<br/>{0}", GetLocalResourceObject("errCorporationExist")));
                    }
                }
            }
            else
            {
                errorsOccured = true;
                errors.Append(string.Format("<br/>{0}", GetLocalResourceObject("errCorporationNameType")));
            }

            if (ddlCorpLogsLanguage.SelectedIndex > 0)
            {
                // ok
                chosenLang = UiCookie.GetStringAsLanguage(ddlCorpLogsLanguage.SelectedValue);
            }
            else
            {
                errorsOccured = true;
                errors.Append(string.Format("<br/>{0}", GetLocalResourceObject("errCorpLogsLanguageChoose")));
            }

            if (!string.IsNullOrEmpty(username))
            {
                if (username.Length > Configuration.MaxUsernameLength)
                {
                    errorsOccured = true;
                    errors.Append(string.Format("<br/>{0} {1}.", GetLocalResourceObject("errUserNameMore")
                        , Configuration.MaxUsernameLength));
                }
                else
                {
                    User user = bUser.Get(objectContext, username);
                    if (user == null)
                    {
                        // ok
                    }
                    else
                    {
                        errorsOccured = true;
                        errors.Append(string.Format("<br/>{0}", GetLocalResourceObject("errUserExist")));
                    }
                }
            }
            else
            {
                errorsOccured = true;
                errors.Append(string.Format("<br/>{0}", GetLocalResourceObject("errUserNameType")));
            }

            if (!string.IsNullOrEmpty(password))
            {
                if (password.Length > Configuration.MaxPasswordLength)
                {
                    errorsOccured = true;
                    errors.Append(string.Format("<br/>{0} {1}.", GetLocalResourceObject("errUserPassMore")
                        , Configuration.MaxPasswordLength));
                }
                else
                {
                    //ok
                }
            }
            else
            {
                errorsOccured = true;
                errors.Append(string.Format("<br/>{0}", GetLocalResourceObject("errUserPassType")));
            }


            if (!string.IsNullOrEmpty(sysName))
            {
                if (sysName.Length > Configuration.MaxSystemNameLength)
                {
                    errorsOccured = true;
                    errors.Append(string.Format("<br/>{0} {1}.", GetLocalResourceObject("errSystemNameMore")
                        , Configuration.MaxSystemNameLength));
                }
                else
                {
                    // ok
                }
            }
            else
            {
                errorsOccured = true;
                errors.Append(string.Format("<br/>{0}", GetLocalResourceObject("errSystemType")));
            }
        }

        protected void btnCancelReg_Click(object sender, EventArgs e)
        {
            pnlRegCorporation.Visible = false;

            pnlLinks.Visible = true;
            pnlLinksS.Visible = false;
        }

        protected void lbRegCorp_Click(object sender, EventArgs e)
        {
            if (currUser == null)
            {
                pnlRegCorporation.Visible = true;

                pnlLinks.Visible = false;
                pnlLinksS.Visible = true;
            }
            else
            {
                LogOut(objectContext, true);
            }
        }

        protected void lbGuestLogIn_Click(object sender, EventArgs e)
        {
            User currUser = bUser.GetGuest(objectContext);
          
            Session[UiSessionParams.SessionCurrentUserID] = currUser.ID;

            Corporation userCorporation = BCorporation.GetUserCorporation(currUser, false);
            if (userCorporation != null)
            {
                Session[UiSessionParams.SessionCurrCorporationID] = userCorporation.ID.ToString();

                Response.Redirect("Main.aspx");
            }

            //Response.Redirect("Main.aspx");
            CheckUser();
            ShowOptions();
           
        }



    }
}