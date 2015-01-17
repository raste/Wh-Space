﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace WhSpace
{
    public partial class Admin : BasePage 
    {
        
        protected object editingUsers = new object();
        protected object editingCorporation = new object();

        Entities objectContext = new Entities();
        User currUser = null;
        Corporation currCorporation = null;

        BUser bUser = new BUser();
        BCorporation bCorporation = new BCorporation();

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckUserAndCorp();

            ShowUsers();
            ShowEditInfo();
            
            HideStatusPanel();

            SetLocalText();
        }

        private void ShowEditInfo()
        {
            ShowEditUserPnl(true);

            if (currUser.globalAdmin == true || bUser.IsUserCorpMainAdmin(currUser, currCorporation) == true)
            {
                pnlEditCorporation.Visible = true;
                FillDDlCorpLanguages(true);
            }
            else
            {
                pnlEditCorporation.Visible = false;
            }
        }

        private void FillDDlCorpLanguages(bool checkForPostBack)
        {
            if (IsPostBack == true && checkForPostBack == true)
            {
                return;
            }

            ddlCorpLogsLanguages.Items.Clear();

            Array languages = Enum.GetValues(typeof(Language));
            foreach (Language lang in languages)
            {
                ListItem langItem = new ListItem();
                langItem.Text = UiCookie.GetLanguageFullName(lang.ToString());
                langItem.Value = lang.ToString();
                ddlCorpLogsLanguages.Items.Add(langItem);

                if (lang == UiCookie.GetStringAsLanguage(currCorporation.logsLanguage))
                {
                    langItem.Selected = true;
                }
            }
        }


        private void SetLocalText()
        {
            Title = string.Format("{0} : {1}", currCorporation.name, GetLocalResourceObject("title"));

            lblStaticStatus.Text = GetGlobalResourceObject("Resources", "StatusStatic").ToString();

            lblStaticUsers.Text = GetLocalResourceObject("staticUsers").ToString();

            ///
            lblStaticAddUser.Text = GetLocalResourceObject("staticAddUser").ToString();
            lblStaticAddUserName.Text = GetLocalResourceObject("staticAddUserName").ToString();
            lblStaticAddUserPassword.Text = GetLocalResourceObject("staticAddUserPassword").ToString();

            rblAddUser.Items[0].Text = GetLocalResourceObject("staticUserTypeAdmin").ToString();
            rblAddUser.Items[1].Text = GetLocalResourceObject("staticUserTypeMember").ToString();
            rblAddUser.Items[2].Text = GetLocalResourceObject("staticUserTypeGuest").ToString();

            btnAddUser.Text = GetGlobalResourceObject("Resources", "btnAdd").ToString();
            ///

            ///
            lblStaticEditUser.Text = GetLocalResourceObject("staticEditUser").ToString();
            lblStaticEditUserName.Text = GetLocalResourceObject("staticEditUserName").ToString();

            rblEditUser.Items[0].Text = GetLocalResourceObject("staticUserTypeAdmin").ToString();
            rblEditUser.Items[1].Text = GetLocalResourceObject("staticUserTypeMember").ToString();
            rblEditUser.Items[2].Text = GetLocalResourceObject("staticUserTypeGuest").ToString();

            btnEditUser.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnDelUser.Text = GetGlobalResourceObject("Resources", "btnDelete").ToString();
            ///

            ///
            lblStaticEditCorporation.Text = GetLocalResourceObject("staticEditCorporation").ToString();
            lblStaticEditCorporationLogsLanguage.Text = GetLocalResourceObject("staticEditCorporationLogsLanguage").ToString();
            lblStaticEditCorporationName.Text = GetLocalResourceObject("staticEditCorporationName").ToString();

            btnChangeCorprationName.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            ///
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


        private void CheckUserRights()
        {
            if (currUser == null)
            {
                Session[UiSessionParams.SessionCurrentUserID] = null;
                Response.Redirect("index.aspx");
            }


            if (currUser.globalAdmin == false && currUser.admin == false)
            {
                Response.Redirect("index.aspx");
            }
            

            
        }

        private void ShowEditUserPnl(bool checkForPostBack)
        {
            if (checkForPostBack == true && IsPostBack == true)
            {
                return;
            }

            if (bUser.AreThereUsersWhichCanBeEdited(objectContext, currUser, currCorporation) == true)
            {
                pnlModifyUser.Visible = true;

                lblEditUserError.Visible = false;

                rblEditUser.SelectedIndex = 2;
                rblEditUser.Enabled = false;

                btnEditUser.Enabled = false;
                btnDelUser.Enabled = false;

                FillEditUserDDl();
            }
            else
            {
                pnlModifyUser.Visible = false;
            }
            
        }

        private void FillEditUserDDl()
        {
            ddlEditUser.Items.Clear();

            ListItem mainItem = new ListItem();
            ddlEditUser.Items.Add(mainItem);
            mainItem.Text = GetGlobalResourceObject("Resources", "ddlChoose").ToString(); ;
            mainItem.Value = "0";

            ddlEditUser.SelectedIndex = 0;

            List<User> users = bUser.GetUsersWhichCanBeEdited(objectContext, currUser, currCorporation);

            if (users != null && users.Count > 0)
            {
                foreach (User user in users)
                {
                    ListItem item = new ListItem();
                    ddlEditUser.Items.Add(item);
                    item.Text = user.name;
                    item.Value = user.ID.ToString();
                }
            }
            else
            {
                pnlModifyUser.Visible = false;
            }
        }

        private void ShowUsers()
        {
            phUsers.Controls.Clear();

            List<User> users = bUser.GetVisibleUsers(objectContext, currCorporation);
            if (users == null || users.Count == 0)
            {
                throw new InvalidOperationException("No visible users in database.");
            }

            foreach (User user in users)
            {

                Panel newPanel = new Panel();
                phUsers.Controls.Add(newPanel);

                //if (user.ID != currUser.ID && user.main == false)
                //{
                //    ImageButton btnDeleteUser = new ImageButton();
                //    newPanel.Controls.Add(btnDeleteUser);

                //    btnDeleteUser.ID = string.Format("delUsr{0}", user.ID);
                //    btnDeleteUser.ImageUrl = "~\\images\\remove.png";
                //    btnDeleteUser.Attributes.Add("usrID", user.ID.ToString());
                //    btnDeleteUser.Click += new ImageClickEventHandler(btnDeleteUser_Click);
                //}

                Panel pnlType = new Panel();
                newPanel.Controls.Add(pnlType);
                pnlType.CssClass = "userTypePnle";

                Label lblType = new Label();
                pnlType.Controls.Add(lblType);
                lblType.CssClass = "userType";

                Label lblUser = new Label();
                newPanel.Controls.Add(lblUser);
                lblUser.Text = user.name;
                lblUser.CssClass = "users";


                if (user.admin == true)
                {
                    lblType.Text = string.Format(" {0} ", GetLocalResourceObject("userTypeAdmin"));
                }
                else if (user.canEdit == true)
                {
                    lblType.Text = string.Format(" {0} ", GetLocalResourceObject("userTypeMember"));
                }
                else
                {
                    lblType.Text = string.Format(" {0} ", GetLocalResourceObject("userTypeGuest"));
                }

            }
        }


        private void CheckUserAndCorp()
        {
            currUser = GetCurrUser(objectContext, true);

            if (currUser.admin == false && currUser.globalAdmin)
            {
                Response.Redirect("Main.aspx");
            }

            currCorporation = GetUserCorporation(objectContext, currUser, true);
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            CheckUserRights();

            lock (editingUsers)
            {
                bool errorOccured = false;
                string error = string.Empty;

                if (string.IsNullOrEmpty(tbAddUsername.Text) || string.IsNullOrEmpty(tbAddPassword.Text))
                {
                    errorOccured = true;
                    error = GetLocalResourceObject("errEnterUserAndPass").ToString();
                }

                if (errorOccured == false)
                {
                    User usr = bUser.Get(objectContext, tbAddUsername.Text);
                    if (usr != null)
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errNameTaken").ToString();
                    }
                }

                bool admin = false;
                bool canedit = false;

                switch (rblAddUser.SelectedIndex)
                {
                    case 0:
                        admin = true;
                        canedit = true;
                        break;
                    case 1:
                        admin = false;
                        canedit = true;
                        break;
                    case 2:
                        admin = false;
                        canedit = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("rblAddUser.SelectedIndex");
                }

                if (errorOccured == false)
                {
                    lblAddUserError.Visible = false;

                    bUser.AddUser(objectContext, currUser, currCorporation, tbAddUsername.Text, tbAddPassword.Text, admin, canedit);

                    BStatistics bStatistics = new BStatistics();
                    bStatistics.UserRegistered(objectContext);

                    ShowStatusPanel(GetLocalResourceObject("statusUserAdded").ToString());
                    tbAddPassword.Text = string.Empty;
                    tbAddUsername.Text = string.Empty;
                    rblAddUser.SelectedIndex = 2;

                    ShowUsers();
                    ShowEditUserPnl(false);

                }
                else
                {
                    lblAddUserError.Visible = true;
                    lblAddUserError.Text = error;
                }
            }

        }

        protected void ddlEditUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEditUser.SelectedIndex > 0)
            {
                string strUsrId = ddlEditUser.SelectedValue;
                if (string.IsNullOrEmpty(strUsrId))
                {
                    throw new ArgumentException("ddlEditUser.SelectedValue is null or empty");
                }

                long usrId = 0;
                if (long.TryParse(strUsrId, out usrId) == false)
                {
                    throw new FormatException(string.Format("Couldnt parse ddlEditUser.SelectedValue (\"{0}\") to long", strUsrId));
                }

                User selectedUser = bUser.Get(objectContext, usrId, true, false);
                if (selectedUser == null)
                {
                    ShowEditUserPnl(false);
                    return;
                }
                if (selectedUser.ID == currUser.ID || selectedUser.globalAdmin == true)
                {
                    ShowEditUserPnl(false);
                    return;
                }

                rblEditUser.Enabled = true;

                if (selectedUser.admin == true)
                {
                    rblEditUser.SelectedIndex = 0;
                }
                else if (selectedUser.canEdit == true)
                {
                    rblEditUser.SelectedIndex = 1;
                }
                else
                {
                    rblEditUser.SelectedIndex = 2;
                }

                User corpAdmin = bUser.GetCorporationMainAdmin(currCorporation);

                if (currUser.globalAdmin == true && selectedUser.ID != corpAdmin.ID)
                {
                    btnMakeCorpAdmin.Visible = true;
                }
                else
                {
                    btnMakeCorpAdmin.Visible = false;
                }

                if (selectedUser.ID == corpAdmin.ID)
                {
                    btnEditUser.Enabled = false;
                    btnDelUser.Enabled = false;
                }
                else
                {
                    btnEditUser.Enabled = true;
                    btnDelUser.Enabled = true;
                }

                

            }
            else
            {
                lblEditUserError.Visible = false;

                rblEditUser.SelectedIndex = 2;
                rblEditUser.Enabled = false;

                btnEditUser.Enabled = false;
                btnDelUser.Enabled = false;
            }
        }

        protected void btnEditUser_Click(object sender, EventArgs e)
        {
            CheckUserRights();

            string strUsrId = ddlEditUser.SelectedValue;
            if (string.IsNullOrEmpty(strUsrId))
            {
                throw new ArgumentException("ddlEditUser.SelectedValue is null or empty");
            }

            long usrId = 0;
            if (long.TryParse(strUsrId, out usrId) == false)
            {
                throw new FormatException(string.Format("Couldnt parse ddlEditUser.SelectedValue (\"{0}\") to long", strUsrId));
            }

            User selectedUser = bUser.Get(objectContext, usrId, true, false);
            if (selectedUser == null)
            {
                ShowEditUserPnl(false);
                return;
            }
            if (selectedUser.ID == currUser.ID || selectedUser.globalAdmin == true)
            {
                ShowEditUserPnl(false);
                return;
            }

            if (!selectedUser.RCorporationReference.IsLoaded)
            {
                selectedUser.RCorporationReference.Load();
            }

            if (selectedUser.RCorporation.ID != currCorporation.ID)
            {
                ShowEditUserPnl(false);
                return;
            }

            lock (editingUsers)
            {

                bool errorOccured = false;
                bool changeOccured = false;
                string error = string.Empty;

                bool admin = false;
                bool canEdit = false;

                switch (rblEditUser.SelectedIndex)
                {
                    case 0:
                        admin = true;
                        canEdit = true;
                        break;
                    case 1:
                        admin = false;
                        canEdit = true;
                        break;
                    case 2:
                        admin = false;
                        canEdit = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("rblEditUser.SelectedIndex");
                }

                if (selectedUser.admin != admin)
                {
                    changeOccured = true;
                }
                if (selectedUser.canEdit != canEdit)
                {
                    changeOccured = true;
                }

                if (changeOccured == false)
                {
                    errorOccured = true;
                    error = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                }

                if (errorOccured == false)
                {
                    lblEditUserError.Visible = false;

                    bUser.EditUserType(objectContext, currCorporation, currUser, selectedUser, admin, canEdit);

                    ShowStatusPanel(GetLocalResourceObject("statusUserUpdated").ToString());

                    ShowUsers();
                    ShowEditUserPnl(false);
                }
                else
                {
                    lblEditUserError.Visible = true;
                    lblEditUserError.Text = error;
                }
            }

        }

        protected void btnDelUser_Click(object sender, EventArgs e)
        {
            CheckUserRights();

            string strUsrId = ddlEditUser.SelectedValue;
            if (string.IsNullOrEmpty(strUsrId))
            {
                throw new ArgumentException("ddlEditUser.SelectedValue is null or empty");
            }

            long usrId = 0;
            if (long.TryParse(strUsrId, out usrId) == false)
            {
                throw new FormatException(string.Format("Couldnt parse ddlEditUser.SelectedValue (\"{0}\") to long", strUsrId));
            }

            User selectedUser = bUser.Get(objectContext, usrId, true, false);
            if (selectedUser == null)
            {
                ShowEditUserPnl(false);
                return;
            }
            if (selectedUser.ID == currUser.ID || selectedUser.globalAdmin == true)
            {
                ShowEditUserPnl(false);
                return;
            }
            if (!selectedUser.RCorporationReference.IsLoaded)
            {
                selectedUser.RCorporationReference.Load();
            }

            if (selectedUser.RCorporation.ID != currCorporation.ID)
            {
                ShowEditUserPnl(false);
                return;
            }

            User corpAdmin = bUser.GetCorporationMainAdmin(currCorporation);
            if (selectedUser.ID == corpAdmin.ID)
            {
                ShowEditUserPnl(false);
                return;
            }

            lock (editingUsers)
            {

                lblEditUserError.Visible = false;

                bUser.DeleteUser(objectContext, currCorporation, currUser, selectedUser);

                ShowStatusPanel(GetLocalResourceObject("statusUserDeleted").ToString());

                ShowUsers();
                ShowEditUserPnl(false);

            }
        }

        protected void btnChangeCorprationName_Click(object sender, EventArgs e)
        {
            lock (editingCorporation)
            {

                if (currUser.globalAdmin == true || bUser.IsUserCorpMainAdmin(currUser, currCorporation) == true)
                {

                    bool errorOccured = false;
                    string error = string.Empty;

                    string newName = tbCorpNewName.Text;

                    if (string.IsNullOrEmpty(newName))
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errCorporationNameType").ToString();
                    }
                    else
                    {
                        if (newName.Length > Configuration.MaxCorporationNameLength)
                        {
                            errorOccured = true;
                            error = string.Format("{0} {1}.", GetLocalResourceObject("errCorporationNameMore"), Configuration.MaxCorporationNameLength);
                        }
                        else
                        {
                            if (bCorporation.IsThereCorporationWithName(objectContext, newName) == true)
                            {
                                errorOccured = true;
                                error = GetLocalResourceObject("errCorporationExist").ToString();
                            }
                            else
                            {
                                // ok
                            }
                        }

                    }

                    if (errorOccured == false)
                    {
                        lblEditCorporationError.Visible = false;

                        bCorporation.ChangeCorporationName(objectContext, currCorporation, currUser, newName);

                        ShowStatusPanel(GetLocalResourceObject("statusCorporationNameChanged").ToString());
                        SetLocalText();
                        tbCorpNewName.Text = string.Empty;
                    }
                    else
                    {
                        lblEditCorporationError.Visible = true;
                        lblEditCorporationError.Text = error;
                    }

                    
                }
                else
                {
                    Response.Redirect("Admin.aspx");
                }
            }
        }

        protected void ddlCorpLogsLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (editingCorporation)
            {
                if (currUser.globalAdmin == true || bUser.IsUserCorpMainAdmin(currUser, currCorporation) == true)
                {

                    if (ddlCorpLogsLanguages.SelectedValue.ToLowerInvariant() != currCorporation.logsLanguage.ToLowerInvariant())
                    {
                        Language newLang = UiCookie.GetStringAsLanguage(ddlCorpLogsLanguages.SelectedValue);

                        bCorporation.ChangeCorporationLogsLanguage(objectContext, currCorporation, currUser, newLang);

                        ShowStatusPanel(GetLocalResourceObject("statusCorporationLogsLanguageChanged").ToString());

                        FillDDlCorpLanguages(false);
                        lblEditCorporationError.Visible = false;
                    }
                    else
                    {
                        FillDDlCorpLanguages(false);
                        lblEditCorporationError.Visible = false;
                    }
                }
                else
                {
                    Response.Redirect("Admin.aspx");
                }
            }
        }

        protected void btnMakeCorpAdmin_Click(object sender, EventArgs e)
        {
            CheckUserRights();

            btnMakeCorpAdmin.Visible = false;

            string strUsrId = ddlEditUser.SelectedValue;
            if (string.IsNullOrEmpty(strUsrId))
            {
                throw new ArgumentException("ddlEditUser.SelectedValue is null or empty");
            }

            long usrId = 0;
            if (long.TryParse(strUsrId, out usrId) == false)
            {
                throw new FormatException(string.Format("Couldnt parse ddlEditUser.SelectedValue (\"{0}\") to long", strUsrId));
            }

            User selectedUser = bUser.Get(objectContext, usrId, true, false);
            if (selectedUser == null)
            {
                ShowEditUserPnl(false);
                return;
            }

            User corpAdmin = bUser.GetCorporationMainAdmin(currCorporation);

            if (selectedUser.ID == currUser.ID || selectedUser.globalAdmin == true || selectedUser.ID == corpAdmin.ID)
            {
                ShowEditUserPnl(false);
                return;
            }

            if (!selectedUser.RCorporationReference.IsLoaded)
            {
                selectedUser.RCorporationReference.Load();
            }

            if(selectedUser.RCorporation.ID != currCorporation.ID)
            {
                 ShowEditUserPnl(false);
                return;
            }


            bUser.ChangeCorporationAdmin(objectContext, currCorporation, currUser, selectedUser);
            ShowStatusPanel(GetLocalResourceObject("statusCorpAdminChanged").ToString());
            ShowEditUserPnl(false);
            lblEditUserError.Visible = false;
        }



         











    }
}