﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WhSpace
{
    public partial class FeedBackPage : BasePage
    {
        Entities objectContext = new Entities();
        User currUser = null;
        Corporation currCorporation = null;
        Text adminText = null;

        BUser bUser = new BUser();
        BFeedback bFeedBack = new BFeedback();
        BTexts bText = new BTexts();
        bool isAdmin = false;

        protected long UserFeedBackNumber = 0;   // Number of user feedback
        protected long PageNum = 1;              // Number of current page
        protected long UserFeedBackOnPage = 10;   // Number of user feedback to show on page 

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckPageParams();
            SetPublicProperties();

            ShowPagesLinks();
            ShowLinks();
            ShowEditInfo();
            SetLocalText();
            ShowFeedback();
            HideStatusPanel();
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
            }
            else
            {
                if (currCorporation != null)
                {
                    hlCorporationS.Visible = true;
                }
                else
                {
                    hlCorporationS.Visible = false;
                }

                if (isAdmin == true)
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
                    pnlEditAdminTextLink.Visible = false;
                }

                lbLogOut.Visible = true;
                pnlLogOut.Visible = true;
                lbLogOut.Controls.Add(pnlLogOut);
            }
        }

        private void SetPublicProperties()
        {
            currUser = GetCurrUser(objectContext, false);
            if (currUser != null)
            {
                currCorporation = GetUserCorporation(objectContext, currUser, false);
                isAdmin = currUser.globalAdmin;
            }

            adminText = bText.Get(objectContext, Configuration.TextFeedBackAdmin);
        }

        private void SetLocalText()
        {
            Title = string.Format("{0} : {1}", GetGlobalResourceObject("Resources", "projectName"), GetLocalResourceObject("title"));

            lbLogOut.ToolTip = GetGlobalResourceObject("Resources", "btnLogOut").ToString();
            lbEditAdminText.ToolTip = GetLocalResourceObject("adminEditText").ToString();
            hlAboutS.ToolTip = GetGlobalResourceObject("Resources", "pageAbout").ToString();
            hlCorporationS.ToolTip = GetGlobalResourceObject("Resources", "pageCorporation").ToString();
            hlFeedBackS.ToolTip = GetGlobalResourceObject("Resources", "pageFeedback").ToString();
            hlIndexS.ToolTip = GetGlobalResourceObject("Resources", "pageIndex").ToString();
            hlGAdminS.ToolTip = GetGlobalResourceObject("Resources", "pageSiteAdmin").ToString();
            ///
            btnShowAddFeedback.Text = GetLocalResourceObject("staticFeedBackAddBtn").ToString();

            lblStaticAddFeedback.Text = GetLocalResourceObject("staticFeedBackAdd").ToString();
            lblStaticAddFeedbackDescr.Text = GetLocalResourceObject("staticFeedBackDescription").ToString();
            lblStaticAddFeedbackName.Text = GetLocalResourceObject("staticFeedBackName").ToString();
            btnAddFeedback.Text = GetGlobalResourceObject("Resources", "btnAdd").ToString();
            btnCancelAddFeedback.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///

            ///
            btnEditAdminFeedback.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnCancelEditAdminFeedback.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///

            ///
            lblStaticEditFeedback.Text = GetLocalResourceObject("staticFeedBackEdit").ToString();
            lblStaticEditFeedbackDescr.Text = GetLocalResourceObject("staticFeedBackDescription").ToString();
            lblStaticEditFeedbackName.Text = GetLocalResourceObject("staticFeedBackName").ToString();
            btnEditFeedback.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnCancelEditFeedback.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///

            ///
            btnShowEditFeedback.Text = GetLocalResourceObject("staticShowEditFeedback").ToString();
            btnDeleteFeedback.Text = GetLocalResourceObject("staticFeedBackDeleteBtn").ToString();
            ///

            lblStaticStatus.Text = GetGlobalResourceObject("Resources", "StatusStatic").ToString();
        }

        private void ShowEditInfo()
        {
            User guest = bUser.GetGuest(objectContext);

            if (currUser != null && currUser.ID != guest.ID)
            {
                if (isAdmin == true)
                {
                    btnShowEditFeedback.Visible = false;
                    btnShowAddFeedback.Visible = false;

                    pnlAddFeedback.Visible = false;
                    pnlEditFeedback.Visible = false;
                    btnAddFeedback.Visible = false;
                    btnDeleteFeedback.Visible = false;
                }
                else
                {
                    pnlEditAdminFeedback.Visible = false;

                    if (bFeedBack.HaveUserAddedFeedback(currUser) == true)
                    {
                        btnShowAddFeedback.Visible = false;
                        pnlAddFeedback.Visible = false;

                        btnShowEditFeedback.Visible = true;
                        btnDeleteFeedback.Visible = true;
                    }
                    else
                    {
                        btnShowEditFeedback.Visible = false;
                        pnlEditFeedback.Visible = false;
                        btnDeleteFeedback.Visible = false;

                        btnShowAddFeedback.Visible = true;
                    }
                }
            }
            else
            {
                pnlAddFeedback.Visible = false;
                pnlEditAdminFeedback.Visible = false;
                pnlEditFeedback.Visible = false;
                btnShowEditFeedback.Visible = false;
                btnShowAddFeedback.Visible = false;
                btnDeleteFeedback.Visible = false;
               
            }
        }

        private void CheckPageParams()
        {
            String strPage = Request.Params["page"];
            if (!string.IsNullOrEmpty(strPage))
            {
                if (!long.TryParse(strPage, out PageNum))
                {
                    if (PageNum < 1)
                    {
                        Response.Redirect(string.Format("FeedBack.aspx"));
                    }
                }
            }

            UserFeedBackNumber = bFeedBack.CountFeedback(objectContext);

            if (Pages.CheckPageParameters(UserFeedBackNumber, PageNum, UserFeedBackOnPage) == false)
            {
                Response.Redirect(string.Format("FeedBack.aspx"));
            }
        }

        private void ShowPagesLinks()
        {
            string url = "FeedBack.aspx";

            phPagesTop.Controls.Clear();
            phPagesBottom.Controls.Clear();

            phPagesTop.Controls.Add(Pages.GetPagesPlaceHolder(UserFeedBackNumber, UserFeedBackOnPage, PageNum, url));
            phPagesBottom.Controls.Add(Pages.GetPagesPlaceHolder(UserFeedBackNumber, UserFeedBackOnPage, PageNum, url));
        }

        private void ShowFeedback()
        {
            // show admin feedback
            lblStaticAdminFeedback.Text = adminText.description;

            if (adminText.lastModified.HasValue == true)
            {
                pnlAdminFeedLastModified.Visible = true;

                if (!adminText.RLastModifiedByReference.IsLoaded)
                {
                    adminText.RLastModifiedByReference.Load();
                }

                lblStaticAdminLastModified.Text = GetLocalResourceObject("staticAdminLastModified").ToString();
                lblStaticAdminLastModifiedDate.Text = Tools.GetDateTimeInLocalFormat(adminText.lastModified.Value);
                lblStaticAdminLastModifiedBy.Text = GetLocalResourceObject("staticAdminLastModifiedBy").ToString();
                lblStaticAdminLastModifiedByUser.Text = adminText.RLastModifiedBy.name;
            }
            else
            {
                pnlAdminFeedLastModified.Visible = false;
            }


            long from = 0;
            long to = 0;
            Pages.GetFromItemNumberToItemNumber(PageNum, UserFeedBackOnPage, out from, out to);

            List<FeedBack> feedback = bFeedBack.Get(objectContext, (int)UserFeedBackOnPage, (int)from, (int)to);

            phUserFeedback.Controls.Clear();
            phUserFeedback.Visible = true;

            if (feedback == null || feedback.Count < 1)
            {
                Label lblNoFeed = new Label();
                phUserFeedback.Controls.Add(lblNoFeed);
                lblNoFeed.CssClass = "errors";
                lblNoFeed.Text = GetLocalResourceObject("noAddedFeedback").ToString();

                return;
            }

            foreach (FeedBack feed in feedback)
            {
                if (!feed.RUserReference.IsLoaded)
                {
                    feed.RUserReference.Load();
                }
                if (!feed.RUser.RCorporationReference.IsLoaded)
                {
                    feed.RUser.RCorporationReference.Load();
                }


                Panel pnl = new Panel();
                phUserFeedback.Controls.Add(pnl);
                pnl.CssClass = "userFeedBackPnl";

                if (isAdmin == true)
                {
                    ImageButton ibDelete = new ImageButton();
                    pnl.Controls.Add(ibDelete);

                    ibDelete.ID = string.Format("DeleteFeed{0}", feed.ID);
                    ibDelete.Attributes.Add("feedID", feed.ID.ToString());
                    ibDelete.Click += new ImageClickEventHandler(ibDelete_Click);
                    ibDelete.ImageUrl = "~\\images\\remove.png";
                    ibDelete.Height = Unit.Pixel(15);
                    ibDelete.ToolTip = GetLocalResourceObject("tooltipDeleteFeedBack").ToString();
                    ibDelete.CssClass = "marginLR2";
                }

                Panel pnlUserName = new Panel();
                pnl.Controls.Add(pnlUserName);
                pnlUserName.CssClass = "userNameFeedBack";

                Label lblUser = new Label();
                pnlUserName.Controls.Add(lblUser);
                lblUser.Text = feed.RUser.name;

                Panel pnlUserCorp = new Panel();
                pnl.Controls.Add(pnlUserCorp);
                pnlUserCorp.CssClass = "userCorpFeedBack";

                Label lblUserCorp = new Label();
                pnlUserCorp.Controls.Add(lblUserCorp);
                lblUserCorp.Text = feed.RUser.RCorporation.name;

                Panel pnlDateAdded = new Panel();
                pnl.Controls.Add(pnlDateAdded);
                pnlDateAdded.CssClass = "userFeedBackDate";

                Label lblDateAdded = new Label();
                pnlDateAdded.Controls.Add(lblDateAdded);
                lblDateAdded.Text = Tools.GetDateTimeInLocalFormat(feed.dateAdded);

                if (!string.IsNullOrEmpty(feed.name))
                {
                    Panel pnlName = new Panel();
                    pnl.Controls.Add(pnlName);
                    pnlName.CssClass = "userFeedBackNamePnl";

                    Label lblName = new Label();
                    pnlName.Controls.Add(lblName);
                    lblName.Text = feed.name;
                    lblName.CssClass = "";
                }

                Panel pnlDescr = new Panel();
                pnl.Controls.Add(pnlDescr);
                pnlDescr.CssClass = "userFeedBackDescrPnl";

                Label lblDescr = new Label();
                pnlDescr.Controls.Add(lblDescr);
                lblDescr.Text = Tools.GetFormattedTextFromDB(feed.description);
                lblDescr.CssClass = "";

                if (feed.lastModified > feed.dateAdded)
                {
                    Panel pnlFeedLastModif = new Panel();
                    pnl.Controls.Add(pnlFeedLastModif);
                    pnlFeedLastModif.CssClass = "userFeedBackLastModifPnl";

                    Label lblLastModif = new Label();
                    pnlFeedLastModif.Controls.Add(lblLastModif);
                    lblLastModif.Text = string.Format("{0} : {1}", GetLocalResourceObject("userFeedLastModified"), Tools.GetDateTimeInLocalFormat(feed.lastModified));
                }

            }
        }

        void ibDelete_Click(object sender, ImageClickEventArgs e)
        {
            if (isAdmin == false || currUser == null)
            {
                Response.Redirect("FeedBack.aspx");
            }

            User guest = bUser.GetGuest(objectContext);
            if (currUser.ID == guest.ID)
            {
                Response.Redirect("FeedBack.aspx");
            }

            ImageButton ib = sender as ImageButton;
            string strID = ib.Attributes["feedID"];

            long id = 0;
            if (long.TryParse(strID, out id) == false)
            {
                throw new InvalidCastException("Couldn't parse ImageButton[feedID] to long.");
            }
            FeedBack feed = bFeedBack.Get(objectContext, id, false);
            if (feed == null)
            {
                Response.Redirect("FeedBack.aspx");
            }

            bFeedBack.DeleteFeedBack(objectContext, currUser, feed);

            ShowStatusPanel(GetLocalResourceObject("statusFeedBackDeleted").ToString());

            ShowEditInfo();
            CheckPageParams();
            ShowPagesLinks();
            ShowFeedback();
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

        protected void btnAddFeedback_Click(object sender, EventArgs e)
        {
            if (currUser == null)
            {
                Response.Redirect("FeedBack.aspx");
            }
            if (currUser.globalAdmin == true)
            {
                throw new InvalidOperationException("Global admins cant add feedback");
            }
            if (bFeedBack.HaveUserAddedFeedback(currUser) == true)
            {
                Response.Redirect("FeedBack.aspx");
            }

            bool errorOccured = false;
            string error = string.Empty;

            string name = tbAddFeedbackName.Text;
            string description = tbAddFeedbackDescr.Text;

            if (!string.IsNullOrEmpty(name))
            {
                if (name.Length > Configuration.MaxFeedBackNameLength)
                {
                    errorOccured = true;
                    error = string.Format("{0} {1}.", GetLocalResourceObject("errFeedbackNameMore"), Configuration.MaxFeedBackNameLength);
                }
            }

            if (string.IsNullOrEmpty(description))
            {
                errorOccured = true;
                error = GetLocalResourceObject("errFeedbackAddType").ToString();
            }

            if (errorOccured == false)
            {
                lblStaticAddFeedbackError.Visible = false;

                // add feedback
                bFeedBack.Add(objectContext, currUser, name, description);

                ShowStatusPanel(GetLocalResourceObject("statusFeedbackAdded").ToString());

                pnlAddFeedback.Visible = false;
                btnShowAddFeedback.Visible = false;
                btnEditFeedback.Visible = true;
                btnDeleteFeedback.Visible = true;

                ShowEditInfo();

                CheckPageParams();
                ShowPagesLinks();
                ShowFeedback();
            }
            else
            {
                lblStaticAddFeedbackError.Visible = true;
                lblStaticAddFeedbackError.Text = error;
            }

        }

        protected void btnEditFeedback_Click(object sender, EventArgs e)
        {
            if (currUser == null)
            {
                Response.Redirect("FeedBack.aspx");
            }
            if (currUser.globalAdmin == true)
            {
                throw new InvalidOperationException("Global admins cant add feedback");
            }
            if (bFeedBack.HaveUserAddedFeedback(currUser) == false)
            {
                Response.Redirect("FeedBack.aspx");
            }

            FeedBack userFeed = bFeedBack.GetUserFeedback(currUser);
            if (userFeed == null)
            {
                Response.Redirect("FeedBack.aspx");
            }

            bool errorOccured = false;
            bool changeMade = false;
            string error = string.Empty;

            string name = tbEditFeedbackName.Text;
            string description = tbEditFeedbackDescr.Text;

            if (!string.IsNullOrEmpty(name))
            {
                if (name.Length > Configuration.MaxFeedBackNameLength)
                {
                    errorOccured = true;
                    error = string.Format("{0} {1}.", GetLocalResourceObject("errFeedbackNameMore"), Configuration.MaxFeedBackNameLength);
                }
                else if (name != userFeed.name)
                {
                    changeMade = true;
                }
            }

            if (string.IsNullOrEmpty(description))
            {
                errorOccured = true;
                error = GetLocalResourceObject("errFeedbackEditType").ToString();
            }
            else if (description != userFeed.description)
            {
                changeMade = true;
            }

            if (errorOccured == false && changeMade == true)
            {
                lblStaticEditFeedbackError.Visible = false;

                bFeedBack.EditFeedBack(objectContext, currUser, userFeed, name, description);

                ShowStatusPanel(GetLocalResourceObject("statusFeedbackUpdated").ToString());

                pnlEditFeedback.Visible = false;

                ShowEditInfo();
                ShowFeedback();
            }
            else
            {
                if (errorOccured == false)
                {
                    error = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                }

                lblStaticEditFeedbackError.Visible = true;
                lblStaticEditFeedbackError.Text = error;
            }
        }

        protected void btnShowEditFeedback_Click(object sender, EventArgs e)
        {
            if (currUser == null)
            {
                Response.Redirect("FeedBack.aspx");
            }
            if (isAdmin == true)
            {
                Response.Redirect("FeedBack.aspx");
            }
            else
            {
                if (bFeedBack.HaveUserAddedFeedback(currUser) == true)
                {
                    pnlEditFeedback.Visible = true;

                    FeedBack userFeed = bFeedBack.GetUserFeedback(currUser);

                    tbEditFeedbackDescr.Text = userFeed.description;
                    tbEditFeedbackName.Text = userFeed.name;

                    lblStaticEditFeedbackError.Visible = false;
                }
                else
                {
                    Response.Redirect("FeedBack.aspx");
                }
            }

        }

        protected void btnCancelEditFeedback_Click(object sender, EventArgs e)
        {
            pnlEditFeedback.Visible = false;
        }

        protected void btnCancelEditAdminFeedback_Click(object sender, EventArgs e)
        {
            pnlEditAdminFeedback.Visible = false;
        }

        protected void btnEditAdminFeedback_Click(object sender, EventArgs e)
        {

            if (currUser == null || isAdmin == false)
            {
                Response.Redirect("FeedBack.aspx");
            }

            string description = edEditAdminFeedback.Content;

            if (description != adminText.description)
            {
                if (!string.IsNullOrEmpty(description))
                {
                    lblStaticEditAdminFeedbackError.Visible = false;

                    bText.ChangeText(objectContext, currUser, adminText, string.Empty, description);

                    ShowStatusPanel(GetLocalResourceObject("statusAdminFeedBackUpdated").ToString());

                    //pnlEditAdminFeedback.Visible = false;

                    ShowEditInfo();
                    ShowFeedback();
                }
                else
                {
                    lblStaticEditAdminFeedbackError.Visible = true;
                    lblStaticEditAdminFeedbackError.Text = GetLocalResourceObject("errAdminFeedbackEmpty").ToString();
                }
            }
            else
            {
                lblStaticEditAdminFeedbackError.Visible = true;
                lblStaticEditAdminFeedbackError.Text = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
            }
            

            
        }

        protected void btnDeleteFeedback_Click(object sender, EventArgs e)
        {
            if (currUser == null || isAdmin == true)
            {
                Response.Redirect("FeedBack.aspx");
            }

            User guest = bUser.GetGuest(objectContext);
            if (currUser.ID == guest.ID)
            {
                Response.Redirect("FeedBack.aspx");
            }

            FeedBack userFeed = bFeedBack.GetUserFeedback(currUser);
            if (userFeed == null)
            {
                Response.Redirect("FeedBack.aspx");
            }

            bFeedBack.DeleteFeedBack(objectContext, currUser, userFeed);

            ShowStatusPanel(GetLocalResourceObject("statusFeedBackDeleted").ToString());

            ShowEditInfo();

            CheckPageParams();
            ShowPagesLinks();
            ShowFeedback();
        }

        protected void btnShowAddFeedback_Click(object sender, EventArgs e)
        {
            if (currUser == null)
            {
                Response.Redirect("FeedBack.aspx");
            }
            if (isAdmin == true)
            {
                Response.Redirect("FeedBack.aspx");
            }
            if (bFeedBack.HaveUserAddedFeedback(currUser) == true)
            {
                Response.Redirect("FeedBack.aspx");
            }

            User guest = bUser.GetGuest(objectContext);
            if (currUser.ID == guest.ID)
            {
                Response.Redirect("FeedBack.aspx");
            }

            pnlAddFeedback.Visible = true;
        }

        protected void btnCancelAddFeedback_Click(object sender, EventArgs e)
        {
            pnlAddFeedback.Visible = false;
        }

        protected void lbLogOut_Click(object sender, EventArgs e)
        {
            LogOut(objectContext, true);

            SetPublicProperties();
            ShowLinks();
            ShowEditInfo();
        }

        protected void lbEditAdminText_Click(object sender, EventArgs e)
        {
            if (currUser == null)
            {
                Response.Redirect("FeedBack.aspx");
            }
            if (isAdmin == true)
            {
                pnlEditAdminFeedback.Visible = true;

                edEditAdminFeedback.Content = adminText.description;
                lblStaticEditAdminFeedbackError.Visible = false;
            }
            else
            {
                Response.Redirect("FeedBack.aspx");
            }
        }





    }
}