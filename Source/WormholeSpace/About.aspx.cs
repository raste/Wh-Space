﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;

namespace WhSpace
{
    public partial class About : BasePage
    {

        Entities objectContext = new Entities();
        User currUser = null;
        Text adminText = null;

        BUser bUser = new BUser();
        BTexts bText = new BTexts();
        bool isAdmin = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            SetPublicProperties();

            ShowLinks();
            ShowText();
            SetLocalText();
        }

        private void ShowLinks()
        {
            if (currUser == null)
            {
                hlCorporationS.Visible = false;

                lbLogOut.Visible = false;
                pnlLogOut.Visible = false;
                hlGAdminS.Visible = false;

                lbEditAdminText.Visible = false;
                pnlEditAdminTextLink.Visible = false;
                pnlEditAdminAboutText.Visible = false;
            }
            else
            {
                if (GetUserCorporation(objectContext, currUser, false) != null)
                {
                    hlCorporationS.Visible = true;
                }
                else
                {
                    hlCorporationS.Visible = false;
                }

                if (currUser.globalAdmin == true)
                {
                    hlGAdminS.Visible = true;

                    lbEditAdminText.Visible = true;
                    pnlEditAdminTextLink.Visible = true;
                    lbEditAdminText.Controls.Add(pnlEditAdminTextLink);
                }
                else
                {
                    hlGAdminS.Visible = false;

                    lbEditAdminText.Visible = false;
                    pnlEditAdminAboutText.Visible = false;
                    pnlEditAdminTextLink.Visible = false;
                }

                lbLogOut.Visible = true;
                pnlLogOut.Visible = true;
                lbLogOut.Controls.Add(pnlLogOut);

            }
        }

        private void ShowText()
        {
            lblAboutText.Text = adminText.description;

            if (adminText.lastModified.HasValue == true)
            {
                pnlAdminTextLastModified.Visible = true;

                if (!adminText.RLastModifiedByReference.IsLoaded)
                {
                    adminText.RLastModifiedByReference.Load();
                }

                lblStaticAdminLastModified.Text = GetLocalResourceObject("staticAdminLastModified").ToString();
                lblStaticAdminLastModifiedDate.Text = Tools.GetDateTimeInLocalFormat(adminText.lastModified.Value);
                lblStaticAdminLastModifiedBy.Text = string.Format("{0}", GetLocalResourceObject("staticAdminLastModifiedBy"));
                lblStaticAdminLastModifiedByUser.Text = adminText.RLastModifiedBy.name;
            }
            else
            {
                pnlAdminTextLastModified.Visible = false;
            }
        }

        private void SetLocalText()
        {
            Title = string.Format("{0} : {1}", GetGlobalResourceObject("Resources", "projectName"), GetLocalResourceObject("title"));

            lbEditAdminText.ToolTip = GetLocalResourceObject("adminEditText").ToString();
            lbLogOut.ToolTip = GetGlobalResourceObject("Resources", "btnLogOut").ToString();
            hlAboutS.ToolTip = GetGlobalResourceObject("Resources", "pageAbout").ToString();
            hlCorporationS.ToolTip = GetGlobalResourceObject("Resources", "pageCorporation").ToString();
            hlFeedBackS.ToolTip = GetGlobalResourceObject("Resources", "pageFeedback").ToString();
            hlIndexS.ToolTip = GetGlobalResourceObject("Resources", "pageIndex").ToString();
            hlGAdminS.ToolTip = GetGlobalResourceObject("Resources", "pageSiteAdmin").ToString();

            ///
            btnEditAdminText.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnCancelEditAdminText.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///
        }

        private void SetPublicProperties()
        {
            currUser = GetCurrUser(objectContext, false);
            if (currUser != null)
            {
                isAdmin = currUser.globalAdmin;
            }
            else
            {
                isAdmin = false;
            }

            adminText = bText.Get(objectContext, Configuration.TextAboutAdmin);
        }

        protected void btnEditAdminText_Click(object sender, EventArgs e)
        {
            if (currUser == null || isAdmin == false)
            {
                Response.Redirect("About.aspx");
            }

            string description = edEditAdminText.Content;

            if (description != adminText.description)
            {
                if (!string.IsNullOrEmpty(description))
                {
                    lblStaticEditAdminTextError.Visible = false;

                    bText.ChangeText(objectContext, currUser, adminText, string.Empty, description);

                    ShowText();
                }
                else
                {
                    lblStaticEditAdminTextError.Visible = true;
                    lblStaticEditAdminTextError.Text = GetLocalResourceObject("errAdminFeedbackEmpty").ToString();
                }
            }
            else
            {
                lblStaticEditAdminTextError.Visible = true;
                lblStaticEditAdminTextError.Text = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
            }
        }

        protected void btnCancelEditAdminText_Click(object sender, EventArgs e)
        {
            pnlEditAdminAboutText.Visible = false;
        }


        protected void lbLogOut_Click(object sender, EventArgs e)
        {
            LogOut(objectContext, true);

            SetPublicProperties();
            ShowLinks();
        }

        protected void lbEditAdminText_Click(object sender, EventArgs e)
        {
            if (currUser == null)
            {
                Response.Redirect("About.aspx");
            }
            if (isAdmin == false)
            {
                Response.Redirect("About.aspx");
            }

            pnlEditAdminAboutText.Visible = true;

            edEditAdminText.Content = adminText.description;
            lblStaticEditAdminTextError.Visible = false;
        }







    }
}