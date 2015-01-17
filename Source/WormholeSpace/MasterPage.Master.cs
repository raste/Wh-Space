﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;

namespace WhSpace
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        Entities objectContext = new Entities();
        User currUser = null;
        BUser bUser = new BUser();

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckUser();
            ShowLinks();
            SetLocalText();
        }

        private void SetLocalText()
        {
            lblStaticAdmin.Text = GetLocalResourceObject("pageAdmin").ToString();
            lblStaticLogs.Text = GetLocalResourceObject("pageLogs").ToString();
            lblStaticMap.Text = GetLocalResourceObject("pageMain").ToString();
            lblStaticOperations.Text = GetLocalResourceObject("pageOperations").ToString();
            lblStaticProfile.Text = GetLocalResourceObject("pageProfile").ToString();

            hlFeedBackS.ToolTip = GetGlobalResourceObject("Resources", "pageFeedback").ToString();
            hlAboutS.ToolTip = GetGlobalResourceObject("Resources", "pageAbout").ToString();
            lbLogOut.ToolTip = GetGlobalResourceObject("Resources", "btnLogOut").ToString();
            hlGAdminS.ToolTip = GetGlobalResourceObject("Resources", "pageSiteAdmin").ToString();
        }

        private void ShowLinks()
        {
            if (currUser.admin == true)
            {
                pnlAdminLinks.Visible = true;
            }
            else
            {
                pnlAdminLinks.Visible = false;
            }

            if (currUser.globalAdmin == true)
            {
                hlGAdminS.Visible = true;
            }
            else
            {
                hlGAdminS.Visible = false;
            }

            lbLogOut.Visible = true;
            pnlLogOut.Visible = true;
            lbLogOut.Controls.Add(pnlLogOut);
        }

        private void CheckUser()
        {

            object objUserId = Session[UiSessionParams.SessionCurrentUserID];
            if (objUserId == null)
            {
                Response.Redirect("Index.aspx");
            }

            string strId = objUserId.ToString();
            long id = 0;

            if (long.TryParse(strId, out id) == false)
            {
                Session[UiSessionParams.SessionCurrentUserID] = null;
                Response.Redirect("Index.aspx");
            }

            currUser = bUser.Get(objectContext, id, true, false);

            if (currUser == null)
            {
                Session[UiSessionParams.SessionCurrentUserID] = null;
                Response.Redirect("Index.aspx");
            }

        }

        protected void lbLogOut_Click(object sender, EventArgs e)
        {

            // zapisvane cookie settings nanovo
            if (Session[UiSessionParams.SessionStayLoggedIn] != null)
            {
                bUser.DeleteUserLogInHash(objectContext, currUser);
                Language currLang = UiCookie.GetStringAsLanguage(Session[UiSessionParams.SessionLangParam].ToString());
                UiCookie.SaveSettings(Session, Response, Request, currLang, null);
            }

            Session[UiSessionParams.SessionCurrentUserID] = null;
            Session[UiSessionParams.SessionCurrCorporationID] = null;
            
            Response.Redirect("Index.aspx");
            
        }




        //public void SetPageDivWidth(int levelX)
        //{
        //    if (levelX > 5)
        //    {
        //        pageDiv.Attributes.Clear();
        //        int width = 1100 + (levelX - 5) * 200;
        //        pageDiv.Attributes.CssStyle.Add(HtmlTextWriterStyle.Width, width.ToString() + "px");

        //    }
        //}


    }
}