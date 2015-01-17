﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;

namespace WhSpace
{
    public partial class Profile : BasePage
    {
        Entities objectContext = new Entities();
        User currUser = null;

        BUser bUser = new BUser();

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckUser();

            SetLocalText();
            HideStatusPanel();
        }

        private void SetLocalText()
        {
            Title = string.Format(GetLocalResourceObject("title").ToString());

            lblStaticChangePassword.Text = GetLocalResourceObject("staticChangePassword").ToString();
            lblStaticNewPassword.Text = GetLocalResourceObject("staticNewPassword").ToString();
            lblStaticOldPassword.Text = GetLocalResourceObject("staticOdPassword").ToString();
            lblStaticStatus.Text = GetGlobalResourceObject("Resources", "StatusStatic").ToString();

            btnChangePass.Text = GetGlobalResourceObject("Resources", "btnChange").ToString();
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
            currUser = GetCurrUser(objectContext, true);
        }

        protected void btnChangePass_Click(object sender, EventArgs e)
        {
            string currPass = tbOldPassword.Text;
            string newPass = tbNewPassword.Text;

            string error = string.Empty;
            bool errorOccured = false;

            if (currUser.name.ToLowerInvariant() == "guest")
            {
                lblChangePassError.Visible = true;
                lblChangePassError.Text = GetLocalResourceObject("errGuestCantChangePass").ToString();
                return;
            }

            if (!string.IsNullOrEmpty(currPass))
            {
               if(currUser.password != bUser.GetHashed(currPass))
               {
                   errorOccured = true;
                   error = GetLocalResourceObject("errPassCurrDifferent").ToString();
               }
               else
               {
                   // ok
               }
            }
            else
            {
                errorOccured = true;
                error = GetLocalResourceObject("errPassCurrType").ToString();
            }

            if (errorOccured == false)
            {
                if (!string.IsNullOrEmpty(newPass))
                {
                    if (newPass.Length > Configuration.MaxPasswordLength)
                    {
                        errorOccured = true;
                        error = string.Format("{0} {1}.", GetLocalResourceObject("errPassNewLength"), Configuration.MaxPasswordLength);
                    }
                    else
                    {
                        if (currPass != newPass)
                        {
                            // ok
                        }
                        else
                        {
                            errorOccured = true;
                            error = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                        }
                    }
                }
                else
                {
                    errorOccured = true;
                    error = GetLocalResourceObject("errPassNewType").ToString();
                }
            }

            if (errorOccured == false)
            {
                lblChangePassError.Visible = false;

                bUser.ChangeUserPassword(objectContext, currUser, currPass, newPass);

                // zapisvane cookie settings nanovo
                Language currLang = UiCookie.GetStringAsLanguage(Session[UiSessionParams.SessionLangParam].ToString());
                SaveSettings(currLang, currUser, false, true);
                
                tbNewPassword.Text = string.Empty;
                tbOldPassword.Text = string.Empty;

                // show status
                ShowStatusPanel(GetLocalResourceObject("statusPassChanged").ToString());
            }
            else
            {
                lblChangePassError.Visible = true;
                lblChangePassError.Text = error;
            }

        }





    }
}