﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace WhSpace
{
    public partial class GAdmin : BasePage
    {
        Entities objectContext = new Entities();
        User currUser = null;
        Corporation currCorporation = null;

        BUser bUser = new BUser();
        BCorporation bCorporation = new BCorporation();

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckUserAndCorp();

            ShowLinks();
            SetLocalText();
            ShowCorporations();
        }

        private void SetLocalText()
        {
            Title = string.Format("{0} : {1}", GetGlobalResourceObject("Resources", "projectName"), GetLocalResourceObject("title"));

            lbLogOut.ToolTip = GetGlobalResourceObject("Resources", "btnLogOut").ToString();
            hlAboutS.ToolTip = GetGlobalResourceObject("Resources", "pageAbout").ToString();
            hlCorporationS.ToolTip = GetGlobalResourceObject("Resources", "pageCorporation").ToString();
            hlFeedBackS.ToolTip = GetGlobalResourceObject("Resources", "pageFeedback").ToString();
            hlIndexS.ToolTip = GetGlobalResourceObject("Resources", "pageIndex").ToString();
            hlGAdminS.ToolTip = GetGlobalResourceObject("Resources", "pageSiteAdmin").ToString();

            lblStaticCorporations.Text = GetLocalResourceObject("staticCorporations").ToString();
        }

        private void ShowLinks()
        {
            if (currCorporation != null)
            {
                hlCorporationS.Visible = true;
            }
            else
            {
                hlCorporationS.Visible = false;
            }

            lbLogOut.Visible = true;
            pnlLogOut.Visible = true;
            lbLogOut.Controls.Add(pnlLogOut);
        }

        private void CheckUserAndCorp()
        {
            currUser = GetCurrUser(objectContext, true);

            if (currUser.globalAdmin == false)
            {
                Response.Redirect("Index.aspx");
            }

            currCorporation = GetUserCorporation(objectContext, currUser, false);
        }

        private void ShowCorporations()
        {
            phCorporations.Controls.Clear();

            List<Corporation> corporations = bCorporation.GetCorporations(objectContext);

            if (corporations != null || corporations.Count > 0)
            {
                foreach (Corporation corporation in corporations)
                {
                    LinkButton lbCorporation = new LinkButton();
                    phCorporations.Controls.Add(lbCorporation);

                    lbCorporation.ID = string.Format("corp{0}", corporation.ID);
                    lbCorporation.Attributes.Add("corpID", corporation.ID.ToString());
                    lbCorporation.Click += new EventHandler(lbCorporation_Click);
                    lbCorporation.CssClass = "indexLinks";

                    Panel pnl = new Panel();
                    lbCorporation.Controls.Add(pnl);
                    pnl.CssClass = "corporationPnlLink";

                    Panel pnlMembersCount = new Panel();
                    pnl.Controls.Add(pnlMembersCount);
                    pnlMembersCount.CssClass = "membersCountPnl";

                    Label lblUsers = new Label();
                    pnlMembersCount.Controls.Add(lblUsers);
                    lblUsers.Text = bUser.CountCorporationMembers(corporation).ToString();
                    //lblUsers.CssClass = "corporationNameText";

                    Label lblCorp = new Label();
                    pnl.Controls.Add(lblCorp);
                    lblCorp.Text = corporation.name;
                    lblCorp.CssClass = "corporationNameText";

                   
                }
            }
            else
            {
                Label lblNoCorporations = new Label();
                phCorporations.Controls.Add(lblNoCorporations);
                lblNoCorporations.Text = GetLocalResourceObject("noCorporations").ToString();
            }

        }

        void lbCorporation_Click(object sender, EventArgs e)
        {
            LinkButton lb = sender as LinkButton;
            if (lb == null)
            {
                throw new ArgumentNullException("lb");
            }

            string strCorpId = lb.Attributes["corpID"];

            if (string.IsNullOrEmpty(strCorpId))
            {
                throw new ArgumentException("lbCorporation.Attributes['corpID'] is null or empty");
            }

            long id = 0;
            if (long.TryParse(strCorpId, out id) == false)
            {
                throw new ArgumentException("Couldn't parse lbCorporation.Attributes['corpID'] to long");
            }

            currCorporation = bCorporation.Get(objectContext, id, true);

            Session[UiSessionParams.SessionCurrCorporationID] = currCorporation.ID;
            Response.Redirect("Main.aspx");

        }

        protected void lbLogOut_Click(object sender, EventArgs e)
        {
            LogOut(objectContext, true);
        }


















    }
}