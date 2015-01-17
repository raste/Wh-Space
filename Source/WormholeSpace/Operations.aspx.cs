﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using AjaxControlToolkit;

namespace WhSpace
{
    public partial class Operations : BasePage
    {
        protected object addingOperation = new object();
        protected object modifyingOperation = new object();

        Entities objectContext = new Entities();
        User currUser = null;
        Corporation currCorporation = null;

        int newUserRowNumOnAdd = -1;
        int newLootRowNumOnAdd = -1;

        List<int> opUserRowsNumbers = new List<int>();
        List<int> opLootRowsNumbers = new List<int>();

        BSystem bSystem = new BSystem();
        BUser bUser = new BUser();
        BLoot bLoot = new BLoot();
        BOperation bOperation = new BOperation();

        protected long OperationsNumber = 0;  // Numbers of operations
        protected long PageNum = 1;           // Number of current page
        protected long OperationsOnPage = 4;  // Number of operations to show on page 

        private string contentPhId = "ctl00$ContentPlaceHolder1$";
        private string contentPhId2 = "ContentPlaceHolder1_";

        private string opUserIdDdl = "phUserDdl";
        private string opUserTbTime = "phUserTbTime";
        private string opUserTbInfo = "phUserTbInfo";

        private string opLootIdDdl = "phLootDdl";
        private string opLootQuantity = "phLootTbQuantity";
        private string opLootPrice = "phLootTbPrice";

        protected void Page_Init(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// sled dobavqne/iztrivane na operaciq : update : check page params, show pages links, i nakraq show operations - TEST
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckUserAndCorp();          

            CheckPageParams();
            ShowPagesLinks();

            ShowOperations();
            FillDynamicPlaceHolders();
            HideStatusPanel();

            if (currUser.admin == true || currUser.canEdit == true)
            {
                btnShowAddOperation.Visible = true;
                //pnlSiteErrors.Visible = true;
            }
            //else
            //{
            //    pnlSiteErrors.Visible = false;
            //}

            SetLocalText();

            if (IsPostBack == false)
            {
                ResetAddOperationPanel();
            }
        }

        private void SetLocalText()
        {
            Title = string.Format("{0} : {1}", currCorporation.name, GetLocalResourceObject("title"));
            lblStaticStatus.Text = GetGlobalResourceObject("Resources", "StatusStatic").ToString();

            btnShowAddOperation.Text = GetLocalResourceObject("btnAddOperation").ToString();

            ///
            lblStaticAddOperation.Text = GetLocalResourceObject("staticAddOperation").ToString();
            lblStaticOperationBasedOn.Text = GetLocalResourceObject("staticOperationBasedOn").ToString();

            if (IsPostBack == false)
            {
                lblStaticOperationLength.Text = BOperation.GetOperationLengthType(1,false) + ":";
                //lblStaticOperationLength.Text = GetLocalResourceObject("staticOperationLength").ToString();
            }

            lblStaticOperationLootInfo.Text = GetLocalResourceObject("staticOperationLootInfo").ToString();
            lblStaticOperationSystem.Text = GetLocalResourceObject("staticOperationSystem").ToString();
            lblStaticOperationTax.Text = GetLocalResourceObject("staticOperationTax").ToString();
            lblStaticOperationType.Text = GetLocalResourceObject("staticOperationType").ToString();
            lblStaticParticipantsInfo.Text = GetLocalResourceObject("staticParticipantsInfo").ToString();
            cbAddAllRows.Text = GetLocalResourceObject("staticAllLootTypes").ToString();
            cbFillLootPriceOnSelect.Text = GetLocalResourceObject("staticCbFillLootLastPriceOnSelect").ToString();

            ddlOpBasedType.Items[0].Text = GetLocalResourceObject("staticOperationBasedOnPlexes").ToString();
            ddlOpBasedType.Items[1].Text = GetLocalResourceObject("staticOperationBasedOnTime").ToString();

            ddlOperationType.Items[0].Text = GetLocalResourceObject("staticOperationTypeCombat").ToString();
            ddlOperationType.Items[1].Text = GetLocalResourceObject("staticOperationTypeGas").ToString();

            btnAddOpUser.Text = GetLocalResourceObject("btnAddMember").ToString();
            btnAddOpLoot.Text = GetLocalResourceObject("btnAddLoot").ToString();

            rblIskCutType.Items[1].Text = GetLocalResourceObject("staticOperationTaxTypeIsk").ToString();

            tbIskCutInfo_TextBoxWatermarkExtender.WatermarkText = GetLocalResourceObject("staticOperationTaxInfoWatermark").ToString();
            tbOpInfo_TextBoxWatermarkExtender.WatermarkText = GetLocalResourceObject("staticOperationInfoWatermark").ToString();

            btnAdd.Text = GetGlobalResourceObject("Resources", "btnAdd").ToString();
            btnCancelAdd.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///

            ///
            lblStaticEditOperation.Text = GetLocalResourceObject("staticEditOperation").ToString();
            //lblStaticEditOperationLength.Text = GetLocalResourceObject("staticEditOperationLength").ToString();
            lblStaticEditOperationSystem.Text = GetLocalResourceObject("staticEditOperationSystem").ToString();
            lblstaticEditOperationInfo.Text = GetLocalResourceObject("staticEditOperationInfo").ToString();
            lblStaticEditOperationTax.Text = GetLocalResourceObject("staticEditOperationTax").ToString();
            lblStaticEditOperationTaxInfo.Text = GetLocalResourceObject("staticEditOperationTaxInfo").ToString();

            rblEditIskCutType.Items[1].Text = GetLocalResourceObject("staticOperationTaxTypeIsk").ToString();

            btnEditIskCutInfo.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnEditOpInfo.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnEditOpIskCut.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnEditOpLength.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnEditOpSystem.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnCancelEditOp.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///

            ///
            lblStaticEditMember.Text = GetLocalResourceObject("staticEditMember").ToString();
            lblStaticEditMemberInfo.Text = GetLocalResourceObject("staticEditMemberInfo").ToString();
            lblStaticEditMemberLength.Text = GetLocalResourceObject("staticEditMemberLength").ToString();

            btnEditUser.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnCancelEditUser.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///

            ///
            lblStaticEditOperationLoot.Text = GetLocalResourceObject("staticEditOperationLoot").ToString();
            lblStaticEditLootPriceEach.Text = GetLocalResourceObject("staticEditLootPriceEach").ToString();
            lblStaticEditLootQuantity.Text = GetLocalResourceObject("staticEditLootQuantity").ToString();

            btnEditLoot.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnCancelEditLoot.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();

            ///
            lblStaticAddMemberToOperation.Text = GetLocalResourceObject("staticAddMemberToOperation").ToString();
            lblStaticAddUserInfo.Text = GetLocalResourceObject("staticAddUserInfo").ToString();
            lblStaticAddUserLength.Text = GetLocalResourceObject("staticAddUserLength").ToString();

            btnAddUser.Text = GetGlobalResourceObject("Resources", "btnAdd").ToString();
            btnCancelAddUser.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///


            ///
            lblStaticAddLootToOperation.Text = GetLocalResourceObject("staticAddLootToOperation").ToString();
            lblStaticAddLootQuantity.Text = GetLocalResourceObject("staticAddLootQuantity").ToString();
            lblStaticAddLootPriceEach.Text = GetLocalResourceObject("staticAddLootPriceEach").ToString();

            btnAddLoot.Text = GetGlobalResourceObject("Resources", "btnAdd").ToString();
            btnCancelAddLoot.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///

            lblStaticSiteErrors.Text = GetLocalResourceObject("staticSiteErrors").ToString();
        }

        private void ResetAddOperationPanel()
        {

            tbOpLength.Text = string.Empty;
            tbSystem.Text = string.Empty;
            tbIskCut.Text = string.Empty;
            tbIskCutInfo.Text = string.Empty;
            tbOpInfo.Text = string.Empty;
            lblErrorAddOp.Visible = false;

            if (bOperation.CountOperations(objectContext, currCorporation) > 0)
            {
                cbFillLootPriceOnSelect.Visible = true;
            }
            else
            {
                cbFillLootPriceOnSelect.Visible = false;
            }

        }

        private void ShowPagesLinks()
        {
            string url = "Operations.aspx";

            phPagesTop.Controls.Clear();
            phPagesBottom.Controls.Clear();

            phPagesTop.Controls.Add(Pages.GetPagesPlaceHolder(OperationsNumber, OperationsOnPage, PageNum, url));
            phPagesBottom.Controls.Add(Pages.GetPagesPlaceHolder(OperationsNumber, OperationsOnPage, PageNum, url));
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
                        Response.Redirect(string.Format("Operations.aspx"));
                    }
                }
            }


            OperationsNumber = bOperation.CountOperations(objectContext, currCorporation);

            if (Pages.CheckPageParameters(OperationsNumber, PageNum, OperationsOnPage) == false)
            {
                Response.Redirect(string.Format("Operations.aspx"));
            }

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

        private void FillDynamicPlaceHolders()
        {

            List<string> ieDropdowns = Request.Params.AllKeys.Where(k => k != null && k.Contains(opUserIdDdl)).ToList();
            if (ieDropdowns != null && ieDropdowns.Count<string>() > 0)
            {
                string controlGenId = string.Format("{0}{1}", contentPhId, opUserIdDdl);
                List<int> rowNums = new List<int>();
                int rowNum = 0;
                string strRowNum = string.Empty;

                for (int c = 0; c < ieDropdowns.Count; c++)
                {
                    strRowNum = ieDropdowns[c].Replace(controlGenId, "");
                    if (int.TryParse(strRowNum, out rowNum) == false)
                    {
                        throw new InvalidCastException("Couldnt parse param ddl number to int.");
                    }
                    rowNums.Add(rowNum);
                }
                foreach (int num in rowNums)
                {
                    opUserRowsNumbers.Add(num);

                    if (num > newUserRowNumOnAdd)
                    {
                        newUserRowNumOnAdd = num;
                    }

                    AddOpUserPanelToPh(num);
                }

                newUserRowNumOnAdd++;
                
            }
            ///////////////////


            ieDropdowns = Request.Params.AllKeys.Where(k => k != null && k.Contains(opLootIdDdl)).ToList();
            if (ieDropdowns != null && ieDropdowns.Count<string>() > 0)
            {
                string controlGenId = string.Format("{0}{1}", contentPhId, opLootIdDdl);
                List<int> rowNums = new List<int>();
                int rowNum = 0;
                string strRowNum = string.Empty;

                for (int c = 0; c < ieDropdowns.Count; c++)
                {
                    strRowNum = ieDropdowns[c].Replace(controlGenId, "");
                    if (int.TryParse(strRowNum, out rowNum) == false)
                    {
                        throw new InvalidCastException("Couldnt parse param ddl number to int.");
                    }
                    rowNums.Add(rowNum);
                }
                foreach (int num in rowNums)
                {
                    opLootRowsNumbers.Add(num);

                    if (num > newLootRowNumOnAdd)
                    {
                        newLootRowNumOnAdd = num;
                    }

                    AddOpLootPanelToPh(num);
                }

                newLootRowNumOnAdd++;
                
            }
            

        }

        private void ClearHiddenFields()
        {
            hfLootEdited.Value = null;
            hfOpEdited.Value = null;
            hfUserEdited.Value = null;
        }

        private void HideAddEditPanels()
        {
            ClearHiddenFields();

            pnlAddOperation.Visible = false;
            pnlAddLoot.Visible = false;
            pnlAddUser.Visible = false;

            pnlEditLoot.Visible = false;
            pnlEditOpGeneral.Visible = false;
            pnlEditOpUser.Visible = false;
        }

        private bool CanUserEditOp(Operation op, bool deleteOp)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            bool result = false;

            if (currUser != null && (currUser.admin == true || currUser.canEdit == true))
            {
                if (!op.RAddedByReference.IsLoaded)
                {
                    op.RAddedByReference.Load();
                }

                bool userOk = false;

                if (op.RAddedBy.ID != currUser.ID)
                {
                    if (currUser.admin == true)
                    {
                        userOk = true;
                    }
                }
                else
                {
                    userOk = true;
                }

                if (userOk == true)
                {
                    if (deleteOp == false)
                    {
                        DateTime now = DateTime.UtcNow;

                        TimeSpan span = now - op.dateAdded;
                        if (span.TotalDays < 1)
                        {
                            result = true;
                        }

                    }
                    else
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        private void ShowOperations()
        {

            long from = 0;
            long to = 0;
            Pages.GetFromItemNumberToItemNumber(PageNum, OperationsOnPage, out from, out to);

            List<Operation> operations = bOperation.GetOperations(objectContext, currCorporation, (int)OperationsOnPage, (int)from, (int)to);

            phOperations.Controls.Clear();
            phOperations.Visible = true;

            if (operations == null || operations.Count < 1)
            {
                Label lblNoOps = new Label();
                phOperations.Controls.Add(lblNoOps);
                lblNoOps.CssClass = "errors";
                lblNoOps.Text = GetLocalResourceObject("noAddedOperations").ToString();

                return;
            }

            bool canEdit = false;

           

            foreach (Operation op in operations)
            {
                canEdit = CanUserEditOp(op, false);

                op.RAddedByReference.Load();
                op.RLastModifiedByReference.Load();

                Panel newPanel = new Panel();
                phOperations.Controls.Add(newPanel);
                newPanel.CssClass = "pnlOperation";

                Panel pnlInfo = new Panel();
                newPanel.Controls.Add(pnlInfo);
                pnlInfo.CssClass = "clearfix2";

                Panel pnlLeft = new Panel();
                pnlInfo.Controls.Add(pnlLeft);
                pnlLeft.Attributes.Add("style", "float:left;width:500px;");

                Panel pnlRight = new Panel();
                pnlInfo.Controls.Add(pnlRight);
                pnlRight.Attributes.Add("style", "margin-left:510px;");

                if (canEdit == true)
                {
                    ImageButton ibEditOp = new ImageButton();
                    pnlLeft.Controls.Add(ibEditOp);
                    ibEditOp.ID = string.Format("EditOp{0}", op.ID);
                    ibEditOp.Attributes.Add("opID", op.ID.ToString());
                    ibEditOp.Click += new ImageClickEventHandler(ibEditOp_Click);
                    ibEditOp.ImageUrl = "~\\images\\edit.png";
                    ibEditOp.Height = Unit.Pixel(15);
                    ibEditOp.ToolTip = GetLocalResourceObject("tooltipEditOperation").ToString();
                    ibEditOp.CssClass = "marginLR2";
                }

                if (currUser.admin == true)
                {
                    ImageButton ibDeleteOp = new ImageButton();
                    pnlLeft.Controls.Add(ibDeleteOp);
                    ibDeleteOp.ID = string.Format("DeleteOp{0}", op.ID);
                    ibDeleteOp.Attributes.Add("opID", op.ID.ToString());
                    ibDeleteOp.Click += new ImageClickEventHandler(ibDeleteOp_Click);
                    ibDeleteOp.ImageUrl = "~\\images\\remove.png";
                    ibDeleteOp.Height = Unit.Pixel(15);
                    ibDeleteOp.ToolTip = GetLocalResourceObject("tooltipDeleteOperation").ToString();
                    ibDeleteOp.CssClass = "marginLR2";
                    

                    //AjaxControlToolkit.ConfirmButtonExtender cbeDelete = new ConfirmButtonExtender();
                    //cbeDelete.ConfirmText = GetLocalResourceObject("alertSureToDeleteOperation").ToString();
                    //cbeDelete.TargetControlID = ibDeleteOp.ID;
                    //pnlLeft.Controls.Add(cbeDelete);
                }

                Label lblId = new Label();
                pnlLeft.Controls.Add(lblId);
                lblId.Text = string.Format(" {0} {1} ", GetLocalResourceObject("infoOperationID"), op.ID);

                pnlLeft.Controls.Add(Tools.GetLabel(" , "));

                Label lblOp = new Label();
                pnlLeft.Controls.Add(lblOp);
                lblOp.Text = string.Format("{0} {1}", GetLocalResourceObject("infoOperationType"), BOperation.GetOperationType(op));

                pnlLeft.Controls.Add(Tools.GetLabel("<br/>"));

                Label lblLengthType = new Label();
                pnlLeft.Controls.Add(lblLengthType);
                lblLengthType.Text = string.Format("{0} {1} ", GetLocalResourceObject("infoOperationBasedOn"), BOperation.GetOperationBasedOn(op));

                pnlLeft.Controls.Add(Tools.GetLabel(" , "));

                

                //pnlLeft.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoOperationLength").ToString() + " "));
                string length = string.Format("{0} : ", BOperation.GetOperationLengthType(op, false));
                pnlLeft.Controls.Add(Tools.GetLabel(length));

                Label lblLength = new Label();
                pnlLeft.Controls.Add(lblLength);
                lblLength.Text = op.opLength.ToString();
                lblLength.CssClass = "wormwholes";

                pnlLeft.Controls.Add(Tools.GetLabel("<br/>"));

                if (!string.IsNullOrEmpty(op.systemName))
                {
                    pnlLeft.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoOperationSystem").ToString() + " "));

                    HyperLink hlSystem = new HyperLink();
                    pnlLeft.Controls.Add(hlSystem);
                    hlSystem.Text = op.systemName;
                    hlSystem.NavigateUrl = Tools.GetSystemNameRedirrectUrl(op.systemName);

                    pnlLeft.Controls.Add(Tools.GetLabel(" , "));
                }


                pnlLeft.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoOperationTotalIsk").ToString() + " "));

                Label lblIskMade = new Label();
                pnlLeft.Controls.Add(lblIskMade);
                lblIskMade.Text = string.Format("{0}", Tools.BreakDoubleNumber(op.iskMade, true));
                lblIskMade.CssClass = "sigRadarMagn";


                //newPanel.Controls.Add(Tools.GetLabel("<hr/>"));
                /////////////////////////////
                if (op.iskCut.HasValue == true)
                {
                    pnlRight.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoOperationTax").ToString() + " "));


                    Label lblIskCut = new Label();
                    pnlRight.Controls.Add(lblIskCut);

                    string strIskCut = Tools.BreakDoubleNumber(op.iskCut.Value, true);
                    if (op.iskCutType < 2)
                    {
                        lblIskCut.Text = string.Format("{0}{1} = {2} {3}", Tools.BreakDoubleNumber(op.iskCut.Value, false)
                            , BOperation.GetOperationIskCutType(op), Tools.BreakDoubleNumber(op.iskMade - op.iskMadeWithCut, true)
                            , GetLocalResourceObject("staticOperationTaxTypeIsk"));
                    }
                    else
                    {
                        lblIskCut.Text = string.Format("{0} {1}", Tools.BreakDoubleNumber(op.iskCut.Value, false)
                            , BOperation.GetOperationIskCutType(op));
                    }

                   
                    
                    lblIskCut.CssClass = "sigRadarMagn";

                    pnlRight.Controls.Add(Tools.GetLabel("<br/>"));
                }

                if (!string.IsNullOrEmpty(op.iskCutInfo))
                {
                    pnlRight.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoOperationTaxInfo").ToString() + " "));

                    Label lblIskCutInfo = new Label();
                    pnlRight.Controls.Add(lblIskCutInfo);
                    lblIskCutInfo.Text = string.Format("{0}", Tools.GetFormattedTextFromDB(op.iskCutInfo));
                    lblIskCutInfo.CssClass = "info";

                    pnlRight.Controls.Add(Tools.GetLabel("<br/>"));
                }

                if (!string.IsNullOrEmpty(op.opInfo))
                {
                    pnlRight.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoOperationInfo").ToString() + " "));

                    Label lblinfo = new Label();
                    pnlRight.Controls.Add(lblinfo);
                    lblinfo.Text = string.Format("{0}", Tools.GetFormattedTextFromDB(op.opInfo));
                    lblinfo.CssClass = "info";
                }

                //newPanel.Controls.Add(Tools.GetLabel("<hr/>"));

                Panel dynPnl = new Panel();
                newPanel.Controls.Add(dynPnl);
                dynPnl.CssClass = "clearfix2";

                Panel pnlUsers = new Panel();
                dynPnl.Controls.Add(pnlUsers);
                pnlUsers.Attributes.Add("style", "float:left;width:490px;");
                pnlUsers.CssClass = "pnlLootUsers";

                Panel pnlUsersInfo = new Panel();
                pnlUsers.Controls.Add(pnlUsersInfo);
                pnlUsersInfo.HorizontalAlign = HorizontalAlign.Center;
                pnlUsersInfo.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin,"5px 0px");

                List<OperationUser> participants = bOperation.GetOperationUsers(op);
                int participantsCount = participants.Count;
                int allUsersCount = bUser.GetVisibleUsers(objectContext, currCorporation).Count();

                if (canEdit == true)
                {
                    if (allUsersCount > participantsCount)
                    {
                        // add
                        ImageButton ibAddUser = new ImageButton();
                        pnlUsersInfo.Controls.Add(ibAddUser);
                        ibAddUser.ID = string.Format("AddUserToOp{0}", op.ID);
                        ibAddUser.Attributes.Add("opID", op.ID.ToString());
                        ibAddUser.Click += new ImageClickEventHandler(ibAddUser_Click);
                        ibAddUser.ImageUrl = "~\\images\\add.png";
                        ibAddUser.Height = Unit.Pixel(15);
                        ibAddUser.ToolTip = GetLocalResourceObject("tooltipAddMemberToOp").ToString();
                        ibAddUser.CssClass = "marginLR2";
                    }
                }

                Label lblParticipants = new Label();
                pnlUsersInfo.Controls.Add(lblParticipants);
                lblParticipants.Text = " " + GetLocalResourceObject("infoOperationParticipantsInfo").ToString();

                Table tblUsers = new Table();
                pnlUsers.Controls.Add(tblUsers);
                tblUsers.Width = Unit.Percentage(100);

                foreach (OperationUser user in participants)
                {
                    user.UsersReference.Load();

                    TableRow newRow = new TableRow();
                    tblUsers.Rows.Add(newRow);

                    TableCell nameCell = new TableCell();
                    newRow.Cells.Add(nameCell);
                    nameCell.Width = Unit.Pixel(170);
                    nameCell.CssClass = "operationUserLootCells";

                    TableCell lengthCell = new TableCell();
                    newRow.Cells.Add(lengthCell);
                    lengthCell.Width = Unit.Pixel(30);
                    lengthCell.HorizontalAlign = HorizontalAlign.Right;
                    lengthCell.CssClass = "operationUserLootCells";

                    TableCell iskMadeCell = new TableCell();
                    newRow.Cells.Add(iskMadeCell);
                    iskMadeCell.Width = Unit.Pixel(120);
                    iskMadeCell.HorizontalAlign = HorizontalAlign.Right;
                    iskMadeCell.CssClass = "operationUserLootCells";

                    TableCell infoCell = new TableCell();
                    newRow.Cells.Add(infoCell);


                    //Panel usrPanel = new Panel();
                    //pnlUsers.Controls.Add(usrPanel);
                    //usrPanel.CssClass = "pnlRowOperation";

                    if (canEdit == true)
                    {

                        if (user.Users.visible == true)
                        {
                            // edit
                            ImageButton ibEditUser = new ImageButton();
                            nameCell.Controls.Add(ibEditUser);
                            ibEditUser.ID = string.Format("EditOpUser{0}", user.ID);
                            ibEditUser.Attributes.Add("opUserID", user.ID.ToString());
                            ibEditUser.Click += new ImageClickEventHandler(ibEditUser_Click);
                            ibEditUser.ImageUrl = "~\\images\\edit.png";
                            ibEditUser.Height = Unit.Pixel(15);
                            ibEditUser.ToolTip = GetLocalResourceObject("tooltipEditMember").ToString();
                            ibEditUser.CssClass = "marginLR2";
                        }

                        if (participantsCount > 1)
                        {
                            // remove
                            ImageButton ibDeleteUser = new ImageButton();
                            nameCell.Controls.Add(ibDeleteUser);
                            ibDeleteUser.ID = string.Format("DeleteOpUser{0}", user.ID);
                            ibDeleteUser.Attributes.Add("opUserID", user.ID.ToString());
                            ibDeleteUser.Click += new ImageClickEventHandler(ibDeleteUser_Click);
                            ibDeleteUser.ImageUrl = "~\\images\\remove.png";
                            ibDeleteUser.Height = Unit.Pixel(15);
                            ibDeleteUser.ToolTip = GetLocalResourceObject("tooltipRemoveMember").ToString();
                            ibDeleteUser.CssClass = "marginLR2";

                            AjaxControlToolkit.ConfirmButtonExtender cbeDelUsr = new ConfirmButtonExtender();
                            cbeDelUsr.ConfirmText = GetLocalResourceObject("alertSureToRemoveMember").ToString();
                            cbeDelUsr.TargetControlID = ibDeleteUser.ID;
                            nameCell.Controls.Add(cbeDelUsr);
                        }
                    }

                    

                    Label lblUser = new Label();
                    nameCell.Controls.Add(lblUser);
                    lblUser.Text = " " + user.Users.name;
                    lblUser.CssClass = "users";

                    Label lblUserLength = new Label();
                    lengthCell.Controls.Add(lblUserLength);
                    lblUserLength.Text = user.participatingLength.ToString();
                    lblUserLength.CssClass = "wormwholes";

                    Label lblUserIskMade = new Label();
                    iskMadeCell.Controls.Add(lblUserIskMade);
                    lblUserIskMade.Text = Tools.BreakDoubleNumber(user.iskMade, true).ToString();
                    lblUserIskMade.CssClass = "sigRadarMagn";

                    Label lblUserInfo = new Label();
                    infoCell.Controls.Add(lblUserInfo);
                    lblUserInfo.Text = user.info;
                    lblUserInfo.CssClass = "info";
                    lblUserInfo.Font.Size = 10;
                    
                }

                Panel pnlLoot = new Panel();
                dynPnl.Controls.Add(pnlLoot);
                pnlLoot.Attributes.Add("style", "margin-left:510px;");
                pnlLoot.CssClass = "pnlLootUsers";

                Panel pnlLootInfo = new Panel();
                pnlLoot.Controls.Add(pnlLootInfo);
                pnlLootInfo.HorizontalAlign = HorizontalAlign.Center;
                pnlLootInfo.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "5px 0px");

                List<OperationLoot> loots = bOperation.GetOperationLoot(op);
                int lootCount = loots.Count;
                int allLootCount = bLoot.GetLootWithType(objectContext, op.opType).Count;

                if (canEdit == true)
                {
                    if (allLootCount > lootCount)
                    {
                        // add
                        ImageButton ibAddLoot = new ImageButton();
                        pnlLootInfo.Controls.Add(ibAddLoot);
                        ibAddLoot.ID = string.Format("AddLootToOp{0}", op.ID);
                        ibAddLoot.Attributes.Add("opID", op.ID.ToString());
                        ibAddLoot.Click += new ImageClickEventHandler(ibAddLoot_Click);
                        ibAddLoot.ImageUrl = "~\\images\\add.png";
                        ibAddLoot.Height = Unit.Pixel(15);
                        ibAddLoot.ToolTip = GetLocalResourceObject("tooltipAddLootToOp").ToString();
                        ibAddLoot.CssClass = "marginLR2";
                    }
                }

                Label lblLoot = new Label();
                pnlLootInfo.Controls.Add(lblLoot);
                lblLoot.Text = " " + GetLocalResourceObject("infoOperationLootInfo").ToString();

                Table tblLoot = new Table();
                pnlLoot.Controls.Add(tblLoot);
                tblLoot.Width = Unit.Percentage(100);

                foreach (OperationLoot loot in loots)
                {

                    TableRow newRow = new TableRow();
                    tblLoot.Rows.Add(newRow);

                    TableCell nameCell = new TableCell();
                    newRow.Cells.Add(nameCell);
                    nameCell.CssClass = "operationUserLootCells";

                    TableCell quantCell = new TableCell();
                    newRow.Cells.Add(quantCell);
                    quantCell.HorizontalAlign = HorizontalAlign.Right;
                    quantCell.Width = Unit.Pixel(40);
                    quantCell.CssClass = "operationUserLootCells";

                    TableCell priceEaCell = new TableCell();
                    newRow.Cells.Add(priceEaCell);
                    priceEaCell.HorizontalAlign = HorizontalAlign.Right;
                    priceEaCell.Width = Unit.Pixel(85);
                    priceEaCell.CssClass = "operationUserLootCells";

                    TableCell priceAllCell = new TableCell();
                    newRow.Cells.Add(priceAllCell);
                    priceAllCell.HorizontalAlign = HorizontalAlign.Right;
                    priceAllCell.Width = Unit.Pixel(85);
                    priceAllCell.CssClass = "operationUserLootCells";

                    if (canEdit == true)
                    {
                        // edit
                        ImageButton ibEditLoot = new ImageButton();
                        nameCell.Controls.Add(ibEditLoot);
                        ibEditLoot.ID = string.Format("EditOpLoot{0}", loot.ID);
                        ibEditLoot.Attributes.Add("opLootID", loot.ID.ToString());
                        ibEditLoot.Click += new ImageClickEventHandler(ibEditLoot_Click);
                        ibEditLoot.ImageUrl = "~\\images\\edit.png";
                        ibEditLoot.Height = Unit.Pixel(15);
                        ibEditLoot.ToolTip = GetLocalResourceObject("tooltipEditOpLoot").ToString();
                        ibEditLoot.CssClass = "marginLR2";

                        if (lootCount > 1)
                        {
                            // remove
                            ImageButton ibDeleteLoot = new ImageButton();
                            nameCell.Controls.Add(ibDeleteLoot);
                            ibDeleteLoot.ID = string.Format("DeleteOpLoot{0}", loot.ID);
                            ibDeleteLoot.Attributes.Add("opLootID", loot.ID.ToString());
                            ibDeleteLoot.Click += new ImageClickEventHandler(ibDeleteLoot_Click);
                            ibDeleteLoot.ImageUrl = "~\\images\\remove.png";
                            ibDeleteLoot.Height = Unit.Pixel(15);
                            ibDeleteLoot.ToolTip = GetLocalResourceObject("tooltipRemoveOpLoot").ToString();
                            ibDeleteLoot.CssClass = "marginLR2";

                            AjaxControlToolkit.ConfirmButtonExtender cbeDelLoot = new ConfirmButtonExtender();
                            cbeDelLoot.ConfirmText = GetLocalResourceObject("alertSureToRemoveLoot").ToString();
                            cbeDelLoot.TargetControlID = ibDeleteLoot.ID;
                            nameCell.Controls.Add(cbeDelLoot);
                        }
                    }

                    loot.Loot1Reference.Load();

                    Label lbllot = new Label();
                    nameCell.Controls.Add(lbllot);
                    lbllot.Text = " " + loot.Loot1.name;
                    lbllot.CssClass = "users";

                    //pnlLoot.Controls.Add(Tools.GetLabel(" , "));

                    Label lblQuant = new Label();
                    quantCell.Controls.Add(lblQuant);
                    lblQuant.Text = loot.quantity.ToString();
                    lblQuant.CssClass = "wormwholes";

                    //pnlLoot.Controls.Add(Tools.GetLabel(" , "));

                    Label lblPriceEach = new Label();
                    priceEaCell.Controls.Add(lblPriceEach);
                    lblPriceEach.Text = Tools.BreakDoubleNumber(loot.pricePerOne, false);
                    //lblPriceEach.CssClass = "";

                    //pnlLoot.Controls.Add(Tools.GetLabel(" , "));

                    Label lblPriceTotal = new Label();
                    priceAllCell.Controls.Add(lblPriceTotal);
                    lblPriceTotal.Text = Tools.BreakDoubleNumber(loot.quantity * loot.pricePerOne, false);
                    lblPriceTotal.CssClass = "sigRadarMagn";

                    //pnlLoot.Controls.Add(Tools.GetLabel("<br/>"));
                }


                newPanel.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoOperationAddedBy").ToString() + " "));

                Label lblAddedBy = new Label();
                newPanel.Controls.Add(lblAddedBy);
                lblAddedBy.Text = op.RAddedBy.name;
                lblAddedBy.CssClass = "users";

                newPanel.Controls.Add(Tools.GetLabel(string.Format(" {0} ", GetLocalResourceObject("infoOperationDateAdded"))));

                Label lblAddedDate = new Label();
                newPanel.Controls.Add(lblAddedDate);
                lblAddedDate.Text = Tools.GetDateTimeInLocalFormat(op.dateAdded);
                lblAddedDate.CssClass = "wormwholes";

                if (op.RLastModifiedBy != null)
                {
                    newPanel.Controls.Add(Tools.GetLabel(string.Format(" , {0} ", GetLocalResourceObject("infoOperationLastModifiedBy"))));

                    Label lblLastModifiedBy = new Label();
                    newPanel.Controls.Add(lblLastModifiedBy);
                    lblLastModifiedBy.Text = op.RLastModifiedBy.name;
                    lblLastModifiedBy.CssClass = "users";

                    newPanel.Controls.Add(Tools.GetLabel(string.Format(" {0} ", GetLocalResourceObject("infoOperationDateLastModified"))));

                    Label lblLastModifiedOn = new Label();
                    newPanel.Controls.Add(lblLastModifiedOn);
                    lblLastModifiedOn.Text = Tools.GetDateTimeInLocalFormat(op.dateLastModified.Value);
                    lblLastModifiedOn.CssClass = "wormwholes";
                }
            }
        }

        void ibDeleteLoot_Click(object sender, ImageClickEventArgs e)
        {
            lock (modifyingOperation)
            {
                HideAddEditPanels();

                ImageButton ib = sender as ImageButton;
                string strID = ib.Attributes["opLootID"];

                long id = 0;
                if (long.TryParse(strID, out id) == false)
                {
                    throw new InvalidCastException("Couldn't parse ImageButton[opLootID] to long.");
                }
                OperationLoot opLoot = bOperation.GetOperationLoot(objectContext, id, false);
                if (opLoot == null)
                {
                    Response.Redirect("Operations.aspx");
                }

                if (!opLoot.OperationsReference.IsLoaded)
                {
                    opLoot.OperationsReference.Load();
                }

                Operation currOp = opLoot.Operations;
                CheckIfUserCanEditOp(currOp, false);

                if (bOperation.CountOperationLoot(currOp) < 2)
                {
                    Response.Redirect("Operations.aspx");
                }

                bOperation.DeleteOperationLoot(objectContext, currCorporation, opLoot, currUser, true, true);
                ShowStatusPanel(GetLocalResourceObject("statusLootRemoved").ToString());
                ShowOperations();
            }
        }

        void ibEditLoot_Click(object sender, ImageClickEventArgs e)
        {
            HideAddEditPanels();

            ImageButton ib = sender as ImageButton;
            string strID = ib.Attributes["opLootID"];

            long id = 0;
            if (long.TryParse(strID, out id) == false)
            {
                throw new InvalidCastException("Couldn't parse ImageButton[opLootID] to long.");
            }
            OperationLoot opLoot = bOperation.GetOperationLoot(objectContext, id, false);
            if (opLoot == null)
            {
                Response.Redirect("Operations.aspx");
            }

            if (!opLoot.OperationsReference.IsLoaded)
            {
                opLoot.OperationsReference.Load();
            }

            Operation currOp = opLoot.Operations;
            CheckIfUserCanEditOp(currOp, false);

            ///
            hfLootEdited.Value = opLoot.ID.ToString();
            ///

            ShowEditLootPanel(opLoot, currOp);
        }

        private void ShowEditLootPanel(OperationLoot currLoot, Operation op)
        {
            if (currLoot == null)
            {
                throw new ArgumentNullException("currLoot");
            }

            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            if (!currLoot.Loot1Reference.IsLoaded)
            {
                currLoot.Loot1Reference.Load();
            }

            List<Loot> allLoot = bLoot.GetLootWithType(objectContext, op.opType);
            List<Loot> opLoots = bOperation.GetDroppedLoodInOperation(op);

            pnlEditLoot.Visible = true;

            lblEditLootForOpID.Text = op.ID.ToString();
            ddlEditLoot.Items.Clear();

            if (allLoot.Count == opLoots.Count)
            {
                ddlEditLoot.Enabled = false;

                ListItem item = new ListItem();
                item.Text = currLoot.Loot1.name;
                item.Value = currLoot.Loot1.ID.ToString();
                ddlEditOpUser.Items.Add(item);

                ddlEditLoot.SelectedIndex = 0;
            }
            else
            {
                ddlEditLoot.Enabled = true;

                foreach (Loot opLoot in opLoots)
                {
                    if (currLoot.Loot1.ID != opLoot.ID && allLoot.Contains(opLoot) == true)
                    {
                        allLoot.Remove(opLoot);
                    }
                }

                foreach (Loot loot in allLoot)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = loot.name;
                    newItem.Value = loot.ID.ToString();
                    ddlEditLoot.Items.Add(newItem);

                    if (loot.ID == currLoot.Loot1.ID)
                    {
                        newItem.Selected = true;
                    }
                }

            }

            tbEditLootPrice.Text = currLoot.pricePerOne.ToString();
            tbEditLootQuantity.Text = currLoot.quantity.ToString();

            lblEditLootError.Visible = false;



        }

        void ibAddLoot_Click(object sender, ImageClickEventArgs e)
        {
            HideAddEditPanels();

            ImageButton ib = sender as ImageButton;
            string strID = ib.Attributes["opID"];

            Operation currOp = GetCurrEditedOperation(strID);
            CheckIfUserCanEditOp(currOp, false);

            ///
            hfOpEdited.Value = currOp.ID.ToString();
            ///

            ShowAddLootPanel(currOp);
        }

        private void ShowAddLootPanel(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            List<Loot> allLoot = bLoot.GetLootWithType(objectContext, op.opType);
            List<Loot> opLoots = bOperation.GetDroppedLoodInOperation(op);


            if (allLoot.Count == opLoots.Count)
            {
                Response.Redirect("Operations.aspx");
            }

            foreach (Loot opLoot in opLoots)
            {
                if (allLoot.Contains(opLoot) == true)
                {
                    allLoot.Remove(opLoot);
                }
            }


            ///////
            pnlAddLoot.Visible = true;

            lblAddLootToOpId.Text = op.ID.ToString();

            ddlAddLoot.Items.Clear();

            ListItem item = new ListItem();
            item.Text = GetGlobalResourceObject("Resources", "ddlChoose").ToString();
            item.Value = "0";
            ddlAddLoot.Items.Add(item);

            ddlAddLoot.SelectedIndex = 0;

            foreach (Loot loot in allLoot)
            {
                ListItem newItem = new ListItem();
                newItem.Text = loot.name;
                newItem.Value = loot.ID.ToString();
                ddlAddLoot.Items.Add(newItem);
            }

            tbAddLootPrice.Text = string.Empty;
            tbAddLootQuantity.Text = string.Empty;

            lblAddLootError.Visible = false;
        }


        void ibDeleteUser_Click(object sender, ImageClickEventArgs e)
        {
            lock (modifyingOperation)
            {
                HideAddEditPanels();

                ImageButton ib = sender as ImageButton;
                string strID = ib.Attributes["opUserID"];

                long id = 0;
                if (long.TryParse(strID, out id) == false)
                {
                    throw new InvalidCastException("Couldn't parse ImageButton[opUserID] to long.");
                }
                OperationUser opUser = bOperation.GetOperationUser(objectContext, id, false);
                if (opUser == null)
                {
                    Response.Redirect("Operations.aspx");
                }

                if (!opUser.OperationsReference.IsLoaded)
                {
                    opUser.OperationsReference.Load();
                }

                Operation currOp = opUser.Operations;
                CheckIfUserCanEditOp(currOp, false);

                if (bOperation.CountOperationUsers(currOp) < 2)
                {
                    Response.Redirect("Operations.aspx");
                }

                bOperation.DeleteOperationUser(objectContext, currCorporation, opUser, currUser, true, true);
                ShowStatusPanel(GetLocalResourceObject("statusMemberRemoved").ToString());
                ShowOperations();
            }
        }

        void ibEditUser_Click(object sender, ImageClickEventArgs e)
        {
            HideAddEditPanels();

            ImageButton ib = sender as ImageButton;
            string strID = ib.Attributes["opUserID"];

            long id = 0;
            if (long.TryParse(strID, out id) == false)
            {
                throw new InvalidCastException("Couldn't parse ImageButton[opUserID] to long.");
            }
            OperationUser opUser = bOperation.GetOperationUser(objectContext, id, false);
            if (opUser == null)
            {
                Response.Redirect("Operations.aspx");
            }

            if (!opUser.OperationsReference.IsLoaded)
            {
                opUser.OperationsReference.Load();
            }

            if (!opUser.UsersReference.IsLoaded)
            {
                opUser.UsersReference.Load();
            }

            if (opUser.Users.visible == false)
            {
                ShowOperations();
                return;
            }

            Operation currOp = opUser.Operations;
            CheckIfUserCanEditOp(currOp, false);

            ///
            hfUserEdited.Value = opUser.ID.ToString();
            ///

            ShowEditUserPanel(opUser, currOp);
        }

        private void ShowEditUserPanel(OperationUser currUser, Operation op)
        {
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            if (!currUser.UsersReference.IsLoaded)
            {
                currUser.UsersReference.Load();
            }

            List<User> users = bUser.GetVisibleUsers(objectContext, currCorporation);
            List<User> opUsers = bOperation.GetUsersParticipatingInOperation(op);

            pnlEditOpUser.Visible = true;

            lblEditOpUserID.Text = op.ID.ToString();
            ddlEditOpUser.Items.Clear();

            if (users.Count == opUsers.Count)
            {
                ddlEditOpUser.Enabled = false;

                ListItem item = new ListItem();
                item.Text = currUser.Users.name;
                item.Value = currUser.Users.ID.ToString();
                ddlEditOpUser.Items.Add(item);

                ddlEditOpUser.SelectedIndex = 0;
            }
            else
            {
                ddlEditOpUser.Enabled = true;

                foreach (User opUser in opUsers)
                {
                    if (opUser.ID != currUser.Users.ID && users.Contains(opUser) == true)
                    {
                        users.Remove(opUser);
                    }
                }

                foreach (User user in users)
                {
                    ListItem newItem = new ListItem();
                    newItem.Text = user.name;
                    newItem.Value = user.ID.ToString();
                    ddlEditOpUser.Items.Add(newItem);

                    if (user.ID == currUser.Users.ID)
                    {
                        newItem.Selected = true;
                    }
                }

            }

            tbEditUserInfo.Text = currUser.info;
            tbEditUserTime.Text = currUser.participatingLength.ToString();

            lblEditUserError.Visible = false;

        }

        void ibAddUser_Click(object sender, ImageClickEventArgs e)
        {
            HideAddEditPanels();

            ImageButton ib = sender as ImageButton;
            string strID = ib.Attributes["opID"];

            Operation currOp = GetCurrEditedOperation(strID);
            CheckIfUserCanEditOp(currOp, false);

            ///
            hfOpEdited.Value = currOp.ID.ToString();
            ///

            ShowAddUserPanel(currOp);
        }

        private void ShowAddUserPanel(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            List<User> users = bUser.GetVisibleUsers(objectContext, currCorporation);
            List<User> opUsers = bOperation.GetUsersParticipatingInOperation(op);

            if (users.Count == opUsers.Count)
            {
                Response.Redirect("Operations.aspx");
            }

            foreach (User opUser in opUsers)
            {
                if (users.Contains(opUser) == true)
                {
                    users.Remove(opUser);
                }
            }

            pnlAddUser.Visible = true;

            lblAddUserToOpID.Text = op.ID.ToString();
            ddlAddUser.Items.Clear();

            ListItem item = new ListItem();
            item.Text = GetGlobalResourceObject("Resources", "ddlChoose").ToString();
            item.Value = "0";
            ddlAddUser.Items.Add(item);

            ddlAddUser.SelectedIndex = 0;

            foreach (User user in users)
            {
                ListItem newItem = new ListItem();
                newItem.Text = user.name;
                newItem.Value = user.ID.ToString();
                ddlAddUser.Items.Add(newItem);
            }

            tbAddUserInfo.Text = string.Empty;
            tbAddUserTime.Text = string.Empty;

            lblAddUserError.Visible = false;
        }

        void ibDeleteOp_Click(object sender, ImageClickEventArgs e)
        {
            lock (modifyingOperation)
            {
                HideAddEditPanels();

                ImageButton ib = sender as ImageButton;
                string strID = ib.Attributes["opID"];

                Operation currOp = GetCurrEditedOperation(strID);
                CheckIfUserCanEditOp(currOp, true);

                bOperation.DeleteOperation(objectContext, currCorporation, currOp, currUser);

                ShowStatusPanel(GetLocalResourceObject("statusOperationDeleted").ToString());

                CheckPageParams();
                ShowPagesLinks();
                ShowOperations();
                ResetAddOperationPanel();
            }
        }

        void ibEditOp_Click(object sender, ImageClickEventArgs e)
        {
            HideAddEditPanels();
           
            ImageButton ib = sender as ImageButton;
            string strID = ib.Attributes["opID"];

            Operation currOp = GetCurrEditedOperation(strID);
            CheckIfUserCanEditOp(currOp, false);

            ///
            hfOpEdited.Value = currOp.ID.ToString();
            ///

            ShowEditOpPnl(currOp);
        }

        private void ShowEditOpPnl(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            pnlEditOpGeneral.Visible = true;

            lblEditOpGeneralID.Text = op.ID.ToString();
            lblStaticEditOperationLength.Text = string.Format("{0} :",BOperation.GetOperationLengthType(op, true));
            tbEditOpLength.Text = op.opLength.ToString();
            tbEditOpSys.Text = op.systemName;
            tbEditOpInfo.Text = op.opInfo;
            rblEditIskCutType.SelectedValue = op.iskCutType.ToString();
            if (op.iskCut.HasValue == true)
            {
                tbEditOpIskCut.Text = op.iskCut.ToString();
            }
            else
            {
                tbEditOpIskCut.Text = string.Empty;
            }
            tbEditOpIskCutInfo.Text = op.iskCutInfo;

            lblEditOpError.Visible = false;
        }

        private Operation GetCurrEditedOperation(string strID)
        {
            Operation currOp = null;
            if (!string.IsNullOrEmpty(strID))
            {
                long id = 0;
                if (long.TryParse(strID, out id) == false)
                {
                    throw new InvalidCastException("Couldn`t parse ImageButton[opID] to long.");
                }

                currOp = bOperation.Get(objectContext, currCorporation, id, false);
                if (currOp == null)
                {
                    Response.Redirect("Operations.aspx");
                }

            }
            else
            {
                Response.Redirect("Operations.aspx");
            }

            return currOp;
        }

        private void FillAddOperation()
        {
            //if (checkForPostBack == true && IsPostBack == true)
            //{
            //    return;
            //}

            ddlOperationType.SelectedIndex = 0;
            ddlOpBasedType.SelectedIndex = 0;
            tbOpLength.Text = string.Empty;
            tbSystem.Text = string.Empty;

            // fill users
            phOpUsers.Controls.Clear();

            List<User> users = bUser.GetVisibleUsers(objectContext, currCorporation);
            int addUsers = users.Count;
            if (addUsers > 5)
            {
                addUsers = 5;
            }
            for (int i = 0; i < addUsers; i++)
            {
                AddOpUserPanelToPh(-1);
            }

            // fill loot
            if (cbAddAllRows.Checked == true)
            {
                AddAllLootRowsBasedOnOperationType();
            }
            else
            {
                AddDefaultLootRowsBasedOnOperationType();
            }
            
            //phOpLoot.Controls.Clear();
            //AddOpLootPanelToPh(-1);

            rblIskCutType.SelectedIndex = 0;
            tbIskCut.Text = string.Empty;
            tbIskCutInfo.Text = string.Empty;
            tbOpInfo.Text = string.Empty;

            lblErrorAddOp.Visible = false;
 
        }

        private void AddOpUserPanelToPh(int rowNum)
        {
            int pnlNum = phOpUsers.Controls.Count;
            if (rowNum >= 0)
            {
                pnlNum = rowNum;
            }

            Panel newPanel = new Panel();
            phOpUsers.Controls.Add(newPanel);
            newPanel.ID = string.Format("phUserPnl{0}", pnlNum);
            newPanel.CssClass = "pnlRowOperation";

            DropDownList ddlUsers = new DropDownList();
            ddlUsers.SkinID = "required";
            newPanel.Controls.Add(ddlUsers);
            ddlUsers.Items.Clear();
            ddlUsers.ID = string.Format("{0}{1}", opUserIdDdl, pnlNum);
            ddlUsers.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");
            ddlUsers.Width = Unit.Pixel(190);


            ListItem first = new ListItem();
            first.Text = GetGlobalResourceObject("Resources", "ddlChoose").ToString();
            first.Value = "0";
            ddlUsers.Items.Add(first);

            ddlUsers.SelectedIndex = 0;

            ddlUsers.AutoPostBack = true;
            ddlUsers.SelectedIndexChanged += new EventHandler(ddlUsers_SelectedIndexChanged);

            List<User> users = bUser.GetVisibleUsers(objectContext, currCorporation);
            if (users.Count < 2)
            {
                btnAddOpUser.Enabled = false;
            }
            else
            {
                btnAddOpUser.Enabled = true;
            }

            foreach (User user in users)
            {
                ListItem item = new ListItem();
                ddlUsers.Items.Add(item);
                item.Text = user.name;
                item.Value = user.ID.ToString();
            }

            TextBox tbTime = new TextBox();
            tbTime.SkinID = "required";
            newPanel.Controls.Add(tbTime);
            tbTime.ID = string.Format("{0}{1}", opUserTbTime, pnlNum);
            tbTime.Width = Unit.Pixel(50);
            tbTime.Enabled = false;
            tbTime.Text = string.Empty;
            tbTime.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");

            TextBox tbUserInfo = new TextBox();
            newPanel.Controls.Add(tbUserInfo);
            tbUserInfo.ID = string.Format("{0}{1}", opUserTbInfo, pnlNum);
            tbUserInfo.Enabled = false;
            tbUserInfo.Text = string.Empty;
            tbUserInfo.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");

            if (phOpUsers.Controls.Count > 1)
            {
                Button btnRemoveUserRow = new Button();
                btnRemoveUserRow.SkinID = "delBtn";
                newPanel.Controls.Add(btnRemoveUserRow);
                btnRemoveUserRow.ID = string.Format("phUserRemoveRow{0}", pnlNum);
                btnRemoveUserRow.Text = GetGlobalResourceObject("Resources", "btnRemove").ToString();
                btnRemoveUserRow.Click += new EventHandler(btnRemoveUserRow_Click);
                btnRemoveUserRow.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");
               
            }

            if (users.Count <= phOpUsers.Controls.Count)
            {
                btnAddOpUser.Enabled = false;
            }

        }

        void ddlUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = sender as DropDownList;
            if (ddl == null)
            {
                throw new ArgumentException("couldnt get event ddl");
            }

            Panel pnl = ddl.Parent as Panel;
            if (pnl == null)
            {
                throw new ArgumentException("couldnt get button parent pnl");
            }

            bool enable = true;
            if (ddl.SelectedIndex == 0)
            {
                enable = false;
            }

       

            ControlCollection controls = pnl.Controls;
            foreach (Control control in controls)
            {
                TextBox box = new TextBox();

                if (control.GetType().Equals(box.GetType()) == true)
                {
                    box = control as TextBox;

                    if (enable == true)
                    {
                        box.Enabled = true;
                    }
                    else
                    {
                        box.Text = string.Empty;
                        box.Enabled = false;
                    }
                }
            }

        }

      


        private void AddOpLootPanelToPh(int rowNum)
        {
            int pnlNum = phOpLoot.Controls.Count;
            if (rowNum >= 0)
            {
                pnlNum = rowNum;
            }

            Panel newPanel = new Panel();
            phOpLoot.Controls.Add(newPanel);
            newPanel.ID = string.Format("phLootPnl{0}", pnlNum);
            newPanel.CssClass = "pnlRowOperation";

            DropDownList ddlLoot = new DropDownList();
            ddlLoot.SkinID = "required";
            newPanel.Controls.Add(ddlLoot);
            ddlLoot.Items.Clear();
            ddlLoot.ID = string.Format("{0}{1}", opLootIdDdl, pnlNum);
            ddlLoot.Width = Unit.Pixel(220);
            ddlLoot.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");

            ListItem first = new ListItem();
            first.Text = GetGlobalResourceObject("Resources", "ddlChoose").ToString();
            first.Value = "0";
            ddlLoot.Items.Add(first);

            ddlLoot.SelectedIndex = 0;

            ddlLoot.AutoPostBack = true;
            ddlLoot.SelectedIndexChanged += new EventHandler(ddlLoot_SelectedIndexChanged);

            int selectedValue = Tools.ParseStringToInt(ddlOperationType.SelectedValue);
            List<Loot> loot = bLoot.GetLootWithType(objectContext, selectedValue);
            if (loot.Count < 2)
            {
                btnAddOpLoot.Enabled = false;
            }
            else
            {
                btnAddOpLoot.Enabled = true;
            }

            foreach (Loot lot in loot)
            {
                ListItem item = new ListItem();
                ddlLoot.Items.Add(item);
                item.Text = lot.name;
                item.Value = lot.ID.ToString();
            }

            TextBox tbQuant = new TextBox();
            tbQuant.SkinID = "required";
            newPanel.Controls.Add(tbQuant);
            tbQuant.ID = string.Format("{0}{1}", opLootQuantity, pnlNum);
            tbQuant.Width = Unit.Pixel(50);
            tbQuant.Text = string.Empty;
            tbQuant.Enabled = false;
            tbQuant.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");
            //tbQuant.Attributes.Add("onkeyup", string.Format("ChangeFocus('{0}{1}{2}','{0}{1}{3}','','{0}{4}{5}',event.keyCode)"
            //   , contentPhId2, opLootQuantity, pnlNum - 1, pnlNum + 1, opLootPrice, pnlNum));
            tbQuant.Attributes.Add("onkeyup", string.Format("ChangeFocus('{0}{1}','{2}','','{0}{3}{2}',event.keyCode)"
               , contentPhId2, opLootQuantity, pnlNum, opLootPrice));

            tbQuant.Attributes.Add("autocomplete", "off");


            TextBox tbPrice = new TextBox();
            tbPrice.SkinID = "required";
            newPanel.Controls.Add(tbPrice);
            tbPrice.ID = string.Format("{0}{1}", opLootPrice, pnlNum);
            tbPrice.Width = Unit.Pixel(90);
            tbPrice.Text = string.Empty;
            tbPrice.Enabled = false;
            tbPrice.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");
            //tbPrice.Attributes.Add("onkeyup", "insertCommas(this)");
            //tbPrice.Attributes.Add("onkeyup", string.Format("insertCommas(this);ChangeFocus('{0}{1}{2}','{0}{1}{3}','{0}{4}{5}','',event.keyCode);"
            //    , contentPhId2, opLootPrice, pnlNum - 1, pnlNum + 1, opLootQuantity, pnlNum));

            tbPrice.Attributes.Add("onkeyup", string.Format("insertCommas(this);ChangeFocus('{0}{1}','{2}','{0}{3}{2}','',event.keyCode)"
                , contentPhId2, opLootPrice, pnlNum, opLootQuantity));
            tbPrice.Attributes.Add("autocomplete", "off");

            if (phOpLoot.Controls.Count > 1)
            {
                Button btnRemoveLootRow = new Button();
                btnRemoveLootRow.SkinID = "delBtn";
                newPanel.Controls.Add(btnRemoveLootRow);
                btnRemoveLootRow.ID = string.Format("phLootRemoveRow{0}", pnlNum);
                btnRemoveLootRow.Text = GetGlobalResourceObject("Resources", "btnRemove").ToString();
                btnRemoveLootRow.Click += new EventHandler(btnRemoveLootRow_Click);
                btnRemoveLootRow.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");
            }

            if (loot.Count <= phOpLoot.Controls.Count)
            {
                btnAddOpLoot.Enabled = false;
            }
        }

        private void AddOpLootPanelToPh(int rowNum, Loot loot, List<Loot> loots)
        {
            int pnlNum = phOpLoot.Controls.Count;
            if (rowNum >= 0)
            {
                pnlNum = rowNum;
            }

            Panel newPanel = new Panel();
            phOpLoot.Controls.Add(newPanel);
            newPanel.ID = string.Format("phLootPnl{0}", pnlNum);
            newPanel.CssClass = "pnlRowOperation";
            
            /////////////////////////////////////
            DropDownList ddlLoot = new DropDownList();
            ddlLoot.SkinID = "required";
            newPanel.Controls.Add(ddlLoot);
            ddlLoot.Items.Clear();
            ddlLoot.ID = string.Format("{0}{1}", opLootIdDdl, pnlNum);
            ddlLoot.Width = Unit.Pixel(220);
            ddlLoot.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");

            ListItem first = new ListItem();
            first.Text = GetGlobalResourceObject("Resources", "ddlChoose").ToString();
            first.Value = "0";
            ddlLoot.Items.Add(first);

            ddlLoot.AutoPostBack = true;
            ddlLoot.SelectedIndexChanged += new EventHandler(ddlLoot_SelectedIndexChanged);

            if (loots.Count < 2)
            {
                btnAddOpLoot.Enabled = false;
            }
            else
            {
                btnAddOpLoot.Enabled = true;
            }

            foreach (Loot lot in loots)
            {
                ListItem item = new ListItem();
                ddlLoot.Items.Add(item);
                item.Text = lot.name;
                item.Value = lot.ID.ToString();
            }

            ddlLoot.SelectedValue = loot.ID.ToString();
            ///////////////////////////////////////

            TextBox tbQuant = new TextBox();
            tbQuant.SkinID = "required";
            newPanel.Controls.Add(tbQuant);
            tbQuant.ID = string.Format("{0}{1}", opLootQuantity, pnlNum);
            tbQuant.Width = Unit.Pixel(50);
            tbQuant.Text = string.Empty;
            //tbQuant.Enabled = false;
            tbQuant.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");
            //tbQuant.Attributes.Add("onkeyup", string.Format("ChangeFocus('{0}{1}{2}','{0}{1}{3}','','{0}{4}{5}',event.keyCode)"
            //    , contentPhId2, opLootQuantity, pnlNum - 1, pnlNum + 1, opLootPrice, pnlNum));

            tbQuant.Attributes.Add("onkeyup", string.Format("ChangeFocus('{0}{1}','{2}','','{0}{3}{2}',event.keyCode)"
                , contentPhId2, opLootQuantity, pnlNum, opLootPrice));

            tbQuant.Attributes.Add("autocomplete","off");


            TextBox tbPrice = new TextBox();
            tbPrice.SkinID = "required";
            newPanel.Controls.Add(tbPrice);
            tbPrice.ID = string.Format("{0}{1}", opLootPrice, pnlNum);
            tbPrice.Width = Unit.Pixel(90);
            //tbPrice.Text = string.Empty;
            //tbPrice.Enabled = false;
            tbPrice.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");
            //tbPrice.Attributes.Add("onkeyup", "insertCommas(this)");
            //tbPrice.Attributes.Add("onkeyup", string.Format("insertCommas(this);ChangeFocus('{0}{1}{2}','{0}{1}{3}','{0}{4}{5}','',event.keyCode);"
            //    , contentPhId2, opLootPrice, pnlNum - 1, pnlNum + 1, opLootQuantity, pnlNum));

            tbPrice.Attributes.Add("onkeyup", string.Format("insertCommas(this);ChangeFocus('{0}{1}','{2}','{0}{3}{2}','',event.keyCode)"
                , contentPhId2, opLootPrice, pnlNum, opLootQuantity));

            tbPrice.Attributes.Add("autocomplete", "off");

            if (cbFillLootPriceOnSelect.Checked == true)
            {
                double price = bOperation.GetPriceForLootFromLastOperation(objectContext, currCorporation, loot.ID);
                if (price > 0)
                {
                    tbPrice.Text = price.ToString();
                }
            }

            if (phOpLoot.Controls.Count > 1)
            {
                Button btnRemoveLootRow = new Button();
                btnRemoveLootRow.SkinID = "delBtn";
                newPanel.Controls.Add(btnRemoveLootRow);
                btnRemoveLootRow.ID = string.Format("phLootRemoveRow{0}", pnlNum);
                btnRemoveLootRow.Text = GetGlobalResourceObject("Resources", "btnRemove").ToString();
                btnRemoveLootRow.Click += new EventHandler(btnRemoveLootRow_Click);
                btnRemoveLootRow.Attributes.CssStyle.Add(HtmlTextWriterStyle.Margin, "0px 1px");
            }

            if (loots.Count <= phOpLoot.Controls.Count)
            {
                btnAddOpLoot.Enabled = false;
            }
        }

        void ddlLoot_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = sender as DropDownList;
            if (ddl == null)
            {
                throw new ArgumentException("couldnt get event ddl");
            }

            Panel pnl = ddl.Parent as Panel;
            if (pnl == null)
            {
                throw new ArgumentException("couldnt get button parent pnl");
            }

            bool enable = true;
            if (ddl.SelectedIndex == 0)
            {
                enable = false;
            }

            ControlCollection controls = pnl.Controls;
            for(int i=0 ; i<controls.Count; i++)
            {
                TextBox box = new TextBox();

                if (controls[i].GetType().Equals(box.GetType()) == true)
                {
                    box = controls[i] as TextBox;

                    if (enable == true)
                    {
                        box.Enabled = true;
                        if (cbFillLootPriceOnSelect.Visible == true && cbFillLootPriceOnSelect.Checked == true
                            && string.IsNullOrEmpty(box.Text) && i > 1)
                        {
                            long id = 0;
                            if (long.TryParse(ddl.SelectedValue, out id) == false)
                            {
                                Response.Redirect("Operations.aspx");
                            }

                            double price = bOperation.GetPriceForLootFromLastOperation(objectContext, currCorporation, id);
                            if (price > 0)
                            {
                                box.Text = price.ToString();
                            }
                        }
                    }
                    else
                    {
                        box.Text = string.Empty;
                        box.Enabled = false;
                    }
                }
            }
        }

        void btnRemoveLootRow_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
            {
                throw new ArgumentException("couldnt get event button");
            }

            Panel pnl = btn.Parent as Panel;
            if (pnl == null)
            {
                throw new ArgumentException("couldnt get button parent pnl");
            }

            phOpLoot.Controls.Remove(pnl);

            if (btnAddOpLoot.Enabled == false)
            {
                btnAddOpLoot.Enabled = true;
            }
        }

        void btnRemoveUserRow_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null)
            {
                throw new ArgumentException("couldnt get event button");
            }

            Panel pnl = btn.Parent as Panel;
            if (pnl == null)
            {
                throw new ArgumentException("couldnt get button parent pnl");
            }

            phOpUsers.Controls.Remove(pnl);

            if (btnAddOpUser.Enabled == false)
            {
                btnAddOpUser.Enabled = true;
            }
        }

        private void CheckUserAndCorp()
        {
            currUser = GetCurrUser(objectContext, true);
            currCorporation = GetUserCorporation(objectContext, currUser, true);
        }

        protected void btnAddOpUser_Click(object sender, EventArgs e)
        {
            AddOpUserPanelToPh(newUserRowNumOnAdd);
        }

        protected void btnAddOpLoot_Click(object sender, EventArgs e)
        {
            AddOpLootPanelToPh(newLootRowNumOnAdd);
        }

       

        protected void ddlOperationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAddAllRows.Checked == true)
            {
                AddAllLootRowsBasedOnOperationType();
            }
            else
            {
                AddDefaultLootRowsBasedOnOperationType();
            }
        }

        private void AddDefaultLootRowsBasedOnOperationType()
        {
            switch (ddlOperationType.SelectedIndex)
            {
                case 0:
                    // add 10 loot rows

                    phOpLoot.Controls.Clear();
                    for (int i = 0; i < 10; i++)
                    {
                        AddOpLootPanelToPh(i);
                    }

                    break;
                case 1:
                    // add 1 loot rows

                    phOpLoot.Controls.Clear();
                    AddOpLootPanelToPh(-1);


                    break;
                default:
                    throw new ArgumentOutOfRangeException("ddlOperationType.SelectedIndex is more than 1");
            }
        }

        private void AddAllLootRowsBasedOnOperationType()
        {
            phOpLoot.Controls.Clear();

            List<Loot> loots = new List<Loot>();
            int selectedValue = 0;
            int.TryParse(ddlOperationType.SelectedValue, out selectedValue);

            if (selectedValue != 1 && selectedValue != 2)
            {
                 throw new ArgumentOutOfRangeException("ddlOperationType.SelectedValue is not 1 or 2");
            }

            loots = bLoot.GetLootWithType(objectContext, selectedValue);

            for (int i = 0; i < loots.Count;i++ )
            {
                AddOpLootPanelToPh(i, loots[i], loots);
            }
        }

        protected void ddlOpBasedType_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            lblStaticOperationLength.Text = BOperation.GetOperationLengthType(ddlOpBasedType.SelectedIndex + 1, false) + ":";
        }



        protected void btnAdd_Click(object sender, EventArgs e)
        {
            CheckUserRoles();

            lock (addingOperation)
            {
                List<OperationUserRow> opUsers = new List<OperationUserRow>();
                List<OperationLootRow> opLoot = new List<OperationLootRow>();

                GetNewOperationUsersData(ref opUsers);
                GetNewOperationLootData(ref opLoot);

                double totalISK = 0;
                int totalLenght = 0;

                if (CheckNewOperationForErrors(opUsers, opLoot, ref totalISK, ref totalLenght) == false)
                {
                    int operationType = Tools.ParseStringToInt(ddlOperationType.SelectedValue);
                    int operationBased = Tools.ParseStringToInt(ddlOpBasedType.SelectedValue);
                    int opLength = Tools.ParseStringToInt(tbOpLength.Text);
                    string systemOp = tbSystem.Text;

                    int iskCut = 0;
                    if (!string.IsNullOrEmpty(tbIskCut.Text))
                    {
                        string strIskCut = tbIskCut.Text.Replace(",", "");

                        iskCut = Tools.ParseStringToInt(strIskCut);
                    }
                    string iskCutInfo = tbIskCutInfo.Text;
                    int iskCutType = Tools.ParseStringToInt(rblIskCutType.SelectedValue);
                    string opInfo = tbOpInfo.Text;

                    double totalIskWithCut = totalISK;
                    if (iskCut > 0)
                    {
                        switch (iskCutType)
                        {
                            case 1:
                                totalIskWithCut -= (totalIskWithCut / 100) * iskCut;
                                break;
                            case 2:
                                totalIskWithCut -= iskCut;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("rblIskCutType.SelectedValue is not 1 or 2");
                        }
                    }

                    double iskPerOneLength = totalIskWithCut / totalLenght;

                    // ADD operation
                    bOperation.Add(objectContext, currCorporation, operationType, operationBased, opLength, systemOp, iskCutType, iskCut, iskCutInfo, opInfo
                        , totalISK, totalIskWithCut, iskPerOneLength, opUsers, opLoot, currUser);

                    lblErrorAddOp.Visible = false;

                    ShowStatusPanel(GetLocalResourceObject("statusOperationAdded").ToString());
                    pnlAddOperation.Visible = false;

                    CheckPageParams();
                    ShowPagesLinks();
                    ShowOperations();
                    ResetAddOperationPanel();
                }
            }
        }

        

        private bool CheckNewOperationForErrors(List<OperationUserRow> opUsers, List<OperationLootRow> opLoot
            , ref double totalISK, ref int totalLength)
        {
            bool errorsOccured = false;
            StringBuilder errors = new StringBuilder();

            // operation type
            if (ddlOperationType.SelectedValue != "1" && ddlOperationType.SelectedValue != "2")
            {
                throw new ArgumentOutOfRangeException("ddlOperationType.SelectedValue is not 1 or 2");
            }

            // operation based on
            if (ddlOpBasedType.SelectedValue != "1" && ddlOpBasedType.SelectedValue != "2")
            {
                throw new ArgumentOutOfRangeException("ddlOpBasedType.SelectedValue is not 1 or 2");
            }

            // operation length
            int opLength = 0;
            if (!string.IsNullOrEmpty(tbOpLength.Text))
            {
                if (int.TryParse(tbOpLength.Text, out opLength) == false)
                {
                    errorsOccured = true;
                    errors.Append("<br/>" + GetLocalResourceObject("errOperationLengthFormat").ToString());
                }
                else if (opLength < 1)
                {
                    errorsOccured = true;
                    errors.Append("<br/>" + GetLocalResourceObject("errOperationLengthLess").ToString());
                }
            }
            else
            {
                errorsOccured = true;
                errors.Append("<br/>" + GetLocalResourceObject("errOperationLengthType").ToString());
            }
           
            // system
            if (tbSystem.Text.Length > 100)
            {
                errorsOccured = true;
                errors.Append("<br/>" + GetLocalResourceObject("errOperationSystemLengthMore").ToString());
            }


            // users
            if (opUsers != null && opUsers.Count > 0)
            {

                List<long> userIDs = new List<long>();

                bool errorInTime = false;
                bool errorInUsers = false;
                User user = null;
                long id = 0;
                int length = 0;

                foreach (OperationUserRow opUser in opUsers)
                {
                    if (errorInUsers == false)
                    {
                        if (long.TryParse(opUser.strID, out id) == false)
                        {
                            throw new InvalidCastException("Coudlnt parse opUser.strID to int.");
                        }

                        user = bUser.Get(objectContext, id, true, false);
                        if (user == null)
                        {
                            Response.Redirect("Operation.aspx");
                        }

                        opUser.user = user;

                        if (userIDs.Contains(id) == false)
                        {
                            userIDs.Add(id);
                        }
                        else
                        {
                            errorInUsers = true;
                            errorsOccured = true;
                            errors.Append("<br/>" + GetLocalResourceObject("errOperationMemberDuplicate").ToString());
                        }
                    }

                    if (errorInTime == false)
                    {
                        if (!string.IsNullOrEmpty(opUser.strTime))
                        {
                            if (int.TryParse(opUser.strTime, out length) == false)
                            {
                                errorInTime = true;
                                errorsOccured = true;
                                errors.Append("<br/>" + GetLocalResourceObject("errOperationMemberLengthFormat").ToString());
                            }
                            else
                            {
                                if (length < 0)
                                {
                                    errorInTime = true;
                                    errorsOccured = true;
                                    errors.Append("<br/>" + GetLocalResourceObject("errOperationMemberLengthLess").ToString());
                                }
                                else if (opLength > 0 && length > opLength)
                                {
                                    errorInTime = true;
                                    errorsOccured = true;
                                    errors.Append("<br/>" + GetLocalResourceObject("errOperationMemberLengthMore").ToString());
                                }
                                else
                                {
                                    opUser.Time = length;
                                    totalLength += length;
                                }
                            }
                        }
                        else
                        {
                            errorInTime = true;
                            errorsOccured = true;
                            errors.Append("<br/>" + GetLocalResourceObject("errOperationMemberLengthType").ToString());
                        }
                    }
                }




            }
            else
            {
                errorsOccured = true;
                errors.Append("<br/>" + GetLocalResourceObject("errOperationMemberEnter").ToString());
            }

            // loot
            if (opLoot != null && opLoot.Count > 0)
            {
                ////

                List<long> lootIDs = new List<long>();

                bool errorInLoot = false;
                bool errorInPrice = false;
                bool errorInQuantity = false;
                Loot loot = null;
                long id = 0;
                float price = 0;
                int quantity = 0;

                foreach (OperationLootRow currLoot in opLoot)
                {
                    if (errorInLoot == false)
                    {
                        if (long.TryParse(currLoot.strID, out id) == false)
                        {
                            throw new InvalidCastException("Coudlnt parse currLoot.strID to int.");
                        }

                        loot = bLoot.Get(objectContext, id, false);
                        if (loot == null)
                        {
                            Response.Redirect("Operation.aspx");
                        }

                        currLoot.loot = loot;

                        if (lootIDs.Contains(id) == false)
                        {
                            lootIDs.Add(id);
                        }
                        else
                        {
                            errorInLoot = true;
                            errorsOccured = true;
                            errors.Append("<br/>" + GetLocalResourceObject("errOperationLootDuplicate").ToString());
                        }
                    }

                    if (errorInQuantity == false)
                    {
                        if (!string.IsNullOrEmpty(currLoot.strQuantity))
                        {
                            if (int.TryParse(currLoot.strQuantity, out quantity) == false)
                            {
                                errorInQuantity = true;
                                errorsOccured = true;
                                errors.Append("<br/>" + GetLocalResourceObject("errOperationLootQuantityFormat").ToString());
                            }
                            else if (quantity < 1)
                            {
                                errorInQuantity = true;
                                errorsOccured = true;
                                errors.Append("<br/>" + GetLocalResourceObject("errOperationLootQuantityLess").ToString());
                            }
                            else
                            {
                                currLoot.Quantity = quantity;
                            }
                        }
                        else
                        {
                            errorInQuantity = true;
                            errorsOccured = true;
                            errors.Append("<br/>" + GetLocalResourceObject("errOperationLootQuantityType").ToString());
                        }
                    }

                    if (errorInPrice == false)
                    {
                        if (!string.IsNullOrEmpty(currLoot.strPrice))
                        {
                            if (float.TryParse(currLoot.strPrice, out price) == false)
                            {
                                errorInPrice = true;
                                errorsOccured = true;
                                errors.Append("<br/>" + GetLocalResourceObject("errOperationLootPriceFormat").ToString());
                            }
                            else if (price < 0)
                            {
                                errorInPrice = true;
                                errorsOccured = true;
                                errors.Append("<br/>" + GetLocalResourceObject("errOperationLootPriceLess").ToString());
                            }
                            else
                            {
                                currLoot.Price = price;
                            }
                        }
                        else
                        {
                            errorInPrice = true;
                            errorsOccured = true;
                            errors.Append("<br/>" + GetLocalResourceObject("errOperationLootPriceEnter").ToString());
                        }
                    }

                    if (errorInQuantity == false && errorInPrice == false)
                    {
                        totalISK += (quantity * price);
                    }
                }

            }
            else
            {
                errorsOccured = true;
                errors.Append("<br/>" + GetLocalResourceObject("errOperationLootEnter").ToString());
            }

            if (errorsOccured == false)
            {
                // rbl isk cut
                string iskCutType = rblIskCutType.SelectedValue;
                if (iskCutType != "1" && iskCutType != "2")
                {
                    throw new ArgumentOutOfRangeException("rblIskCutType.SelectedValue is not 1 or 2");
                }

                // isk CUt
                if (!string.IsNullOrEmpty(tbIskCut.Text))
                {

                    string strIskCut = tbIskCut.Text.Replace(",", "");

                    int iskCut = 0;
                    if (int.TryParse(strIskCut, out iskCut) == false)
                    {
                        errorsOccured = true;
                        errors.Append("<br/>" + GetLocalResourceObject("errOperationTaxFormat").ToString());
                    }
                    else if (iskCut < 1)
                    {
                        errorsOccured = true;
                        errors.Append("<br/>" + GetLocalResourceObject("errOperationTaxLess").ToString());
                    }
                    else
                    {
                        switch (iskCutType)
                        {
                            case "1": // &
                                if (iskCut > 100)
                                {
                                    errorsOccured = true;
                                    errors.Append("<br/>" + GetLocalResourceObject("errOperationTaxMorePerc").ToString());
                                }
                                break;
                            case "2": // mil
                                if (iskCut > totalISK)
                                {
                                    errorsOccured = true;
                                    errors.Append("<br/>" + GetLocalResourceObject("errOperationTaxMoreIsk").ToString());
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("rblIskCutType.SelectedValue is not 1 or 2");
                        }
                    }
                }
            }

            if (errorsOccured == true)
            {
                errors.Replace("<br/>", "<br/>&nbsp;&nbsp;&nbsp;");
                errors.Insert(0, GetLocalResourceObject("errOperationErrors"));
                
                lblErrorAddOp.Visible = true;
                lblErrorAddOp.Text = errors.ToString();
            }

            return errorsOccured;
        }

        private void GetNewOperationUsersData(ref List<OperationUserRow> opUsers)
        {
            if (opUsers == null)
            {
                throw new ArgumentNullException("opUsers");
            }

            if (opUserRowsNumbers.Count < 1)
            {
                return;
            }

            string ddl = contentPhId + opUserIdDdl;
            string time = contentPhId + opUserTbTime;
            string info = contentPhId + opUserTbInfo;

            OperationUserRow newUser = new OperationUserRow();

            string userId = string.Empty;
            string userTime = string.Empty;
            string userInfo = string.Empty;

            foreach (int rowNum in opUserRowsNumbers)
            {
                userId = Request.Params.Get(string.Format("{0}{1}", ddl, rowNum));

                if (userId != "0")
                {
                    userTime = Request.Params.Get(string.Format("{0}{1}", time, rowNum));
                    userInfo = Request.Params.Get(string.Format("{0}{1}", info, rowNum));

                    newUser = new OperationUserRow();

                    newUser.strID = userId;
                    newUser.strTime = userTime;
                    newUser.Info = userInfo;

                    opUsers.Add(newUser);
                }

               
            }

        }

        private void GetNewOperationLootData(ref List<OperationLootRow> opLoot)
        {
            if (opLoot == null)
            {
                throw new ArgumentNullException("opLoot");
            }

            if (opLootRowsNumbers.Count < 1)
            {
                return;
            }

            string ddl = contentPhId + opLootIdDdl;
            string quant = contentPhId + opLootQuantity;
            string price = contentPhId + opLootPrice;

            OperationLootRow newLoot = new OperationLootRow();

            string lootId = string.Empty;
            string lootQuant = string.Empty;
            string lootProce = string.Empty;

            foreach (int rowNum in opLootRowsNumbers)
            {
                newLoot = new OperationLootRow();

                lootId = Request.Params.Get(string.Format("{0}{1}", ddl, rowNum));
               
                if (lootId != "0")
                {
                    lootQuant = Request.Params.Get(string.Format("{0}{1}", quant, rowNum));

                    if (!string.IsNullOrEmpty(lootQuant))
                    {
                        lootProce = Request.Params.Get(string.Format("{0}{1}", price, rowNum));

                        newLoot.strID = lootId;
                        newLoot.strQuantity = lootQuant;
                        newLoot.strPrice = lootProce;

                        if (!string.IsNullOrEmpty(newLoot.strPrice))
                        {
                            newLoot.strPrice = newLoot.strPrice.Replace(",", "");
                        }

                        opLoot.Add(newLoot);
                    }
                }
            }

        }

        private void CheckUserRoles()
        {
            if (currUser == null)
            {
                Response.Redirect("Index.aspx");
            }

            if (currUser.admin == false && currUser.canEdit == false)
            {
                Response.Redirect("Operations.aspx");
            }
        }

        private void CheckIfUserCanEditOp(Operation op, bool deleteOp)
        {
            if (op == null)
            {
                Response.Redirect("Operations.aspx");
            }

            CheckUserRoles();

            if (CanUserEditOp(op, deleteOp) == false)
            {
                Response.Redirect("Operations.aspx");
            }
        }

        protected void btnShowAddOperation_Click(object sender, EventArgs e)
        {
            CheckUserRoles();

            pnlAddOperation.Visible = true;
            FillAddOperation();
        }

        protected void btnCancelAdd_Click(object sender, EventArgs e)
        {
            HideAddEditPanels();
            ResetAddOperationPanel();
        }

        protected void btnCancelEditOp_Click(object sender, EventArgs e)
        {
            HideAddEditPanels();
        }

        protected void btnCancelEditUser_Click(object sender, EventArgs e)
        {
            HideAddEditPanels();
        }

        protected void btnCancelEditLoot_Click(object sender, EventArgs e)
        {
            HideAddEditPanels();
        }

        protected void btnCancelAddUser_Click(object sender, EventArgs e)
        {
            HideAddEditPanels();
        }

        protected void btnCancelAddLoot_Click(object sender, EventArgs e)
        {
            HideAddEditPanels();
        }

        protected void btnEditOpLength_Click(object sender, EventArgs e)
        {
            lock (modifyingOperation)
            {
                string strID = hfOpEdited.Value;
                Operation op = GetCurrEditedOperation(strID);

                CheckIfUserCanEditOp(op, false);

                bool errorOccured = false;
                string error = string.Empty;
                int newLength = 0;

                if (string.IsNullOrEmpty(tbEditOpLength.Text))
                {
                    errorOccured = true;
                    error = GetLocalResourceObject("errOperationLengthCantBeEmpty").ToString();
                }
                else if (tbEditOpLength.Text == op.opLength.ToString())
                {
                    errorOccured = true;
                    error = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                }
                else
                {
                    if (int.TryParse(tbEditOpLength.Text, out newLength) == false)
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationLengthFormat").ToString();
                    }
                    else if (newLength < 1)
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationLengthLess").ToString();
                    }
                }

                if (errorOccured == false)
                {
                    if (bOperation.CheckIfNewOperationLengthIsValid(op, newLength) == false)
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationLengthNew").ToString();
                    }
                }

                if (errorOccured == false)
                {
                    bOperation.ChangeOperationLenght(objectContext, currCorporation, op, newLength, currUser);

                    ShowEditOpPnl(op);
                    ShowStatusPanel(GetLocalResourceObject("statusOperationLengthUpdated").ToString());
                    ShowOperations();
                }
                else
                {
                    lblEditOpError.Text = error;
                    lblEditOpError.Visible = true;
                }
            }
        }

        protected void btnEditOpSystem_Click(object sender, EventArgs e)
        {
            lock (modifyingOperation)
            {
                string strID = hfOpEdited.Value;
                Operation op = GetCurrEditedOperation(strID);

                CheckIfUserCanEditOp(op, false);

                string newSystem = tbEditOpSys.Text;

                if (newSystem != op.systemName)
                {
                    bOperation.ChangeOperationSystem(objectContext, currCorporation, op, newSystem, currUser);

                    ShowEditOpPnl(op);
                    ShowStatusPanel(GetLocalResourceObject("statusOperationSystemUpdated").ToString());
                    ShowOperations();
                }
                else
                {
                    lblEditOpError.Text = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                    lblEditOpError.Visible = true;
                }


            }
        }

        protected void btnEditOpInfo_Click(object sender, EventArgs e)
        {
            lock (modifyingOperation)
            {
                string strID = hfOpEdited.Value;
                Operation op = GetCurrEditedOperation(strID);

                CheckIfUserCanEditOp(op, false);

                string newInfo = tbEditOpInfo.Text;

                if (newInfo != op.opInfo)
                {
                    bOperation.ChangeOperationInfo(objectContext, currCorporation, op, newInfo, currUser);

                    ShowEditOpPnl(op);
                    ShowStatusPanel(GetLocalResourceObject("statusOperationInfoUpdated").ToString());
                    ShowOperations();
                }
                else
                {
                    lblEditOpError.Text = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                    lblEditOpError.Visible = true;
                }


            }
        }

        protected void btnEditOpIskCut_Click(object sender, EventArgs e)
        {
            lock (modifyingOperation)
            {
                string strID = hfOpEdited.Value;
                Operation op = GetCurrEditedOperation(strID);

                CheckIfUserCanEditOp(op, false);

                bool errorOccured = false;
                string error = string.Empty;

                string strIskCutType = rblEditIskCutType.SelectedValue;
                int iskCutType = 0;
                if (int.TryParse(strIskCutType, out iskCutType) == false)
                {
                    throw new InvalidCastException("Couldnt parse rblEditIskCutType.SelectedValue to int.");
                }
                else if (iskCutType != 1 && iskCutType != 2)
                {
                    throw new ArgumentOutOfRangeException("rblEditIskCutType.SelectedValue is not 1 or 2");
                }

                string strIskCut = tbEditOpIskCut.Text;
                int iskCut = 0;

                if (!string.IsNullOrEmpty(strIskCut))
                {

                    strIskCut = strIskCut.Replace(",", "");

                    if (int.TryParse(strIskCut, out iskCut) == false)
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationTaxFormat").ToString();
                    }
                    else if (iskCut < 0)
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationTaxLessZero").ToString();
                    }
                }

                if (op.iskCut.HasValue == true)
                {
                    if (op.iskCut == iskCut && iskCutType == op.iskCutType)
                    {
                        errorOccured = true;
                        error = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                    }
                }
                else
                {
                    if (iskCut == 0 && iskCutType == op.iskCutType)
                    {
                        errorOccured = true;
                        error = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                    }
                }

                if (errorOccured == false && iskCut > 0)
                {
                    switch (iskCutType)
                    {
                        case 1:
                            if (iskCut > 100)
                            {
                                errorOccured = true;
                                error = GetLocalResourceObject("errOperationTaxMorePerc").ToString();
                            }
                            break;
                        case 2:
                            if (iskCut > op.iskMade)
                            {
                                errorOccured = true;
                                error = GetLocalResourceObject("errOperationTaxMoreIsk").ToString();
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("rblEditIskCutType.SelectedValue is not 1 or 2");
                    }
                }

                if (errorOccured == false)
                {
                    bOperation.ChangeOperationIskCut(objectContext, currCorporation, op, iskCutType, iskCut, currUser);

                    ShowEditOpPnl(op);
                    ShowStatusPanel(GetLocalResourceObject("statusOperationTaxUpdated").ToString());
                    ShowOperations();

                }
                else
                {
                    lblEditOpError.Text = error;
                    lblEditOpError.Visible = true;
                }

            }
        }

        protected void btnEditIskCutInfo_Click(object sender, EventArgs e)
        {
            lock (modifyingOperation)
            {
                string strID = hfOpEdited.Value;
                Operation op = GetCurrEditedOperation(strID);

                CheckIfUserCanEditOp(op, false);

                string newInfo = tbEditOpIskCutInfo.Text;

                if (newInfo != op.iskCutInfo)
                {
                    bOperation.ChangeOperationIskCutInfo(objectContext, currCorporation, op, newInfo, currUser);

                    ShowEditOpPnl(op);
                    ShowStatusPanel(GetLocalResourceObject("statusOperationTaxInfoUpdated").ToString());
                    ShowOperations();
                }
                else
                {
                    lblEditOpError.Text = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                    lblEditOpError.Visible = true;
                }
            }
        }

        protected void btnEditUser_Click(object sender, EventArgs e)
        {
            lock (modifyingOperation)
            {
                string strID = hfUserEdited.Value;
                OperationUser opUserEdited = null;
                Operation op = null;
                User newUser = null;
                int newLength = 0;
                string newInfo = tbEditUserInfo.Text;

                bool changeMade = false;
                bool errorOccured = false;
                string error = string.Empty;

                if (!string.IsNullOrEmpty(strID))
                {
                    long id = 0;
                    if (long.TryParse(strID, out id) == false)
                    {
                        throw new InvalidCastException("Couldn`t parse hfUserEdited.Value to long.");
                    }

                    opUserEdited = bOperation.GetOperationUser(objectContext, id, false);
                    if (opUserEdited == null)
                    {
                        Response.Redirect("Operations.aspx");
                    }

                }
                else
                {
                    Response.Redirect("Operations.aspx");
                }

                if (!opUserEdited.UsersReference.IsLoaded)
                {
                    opUserEdited.UsersReference.Load();
                }
                if (!opUserEdited.OperationsReference.IsLoaded)
                {
                    opUserEdited.OperationsReference.Load();
                }

                op = opUserEdited.Operations;
                CheckIfUserCanEditOp(op, false);
                
                if (opUserEdited.Users.ID.ToString() != ddlEditOpUser.SelectedValue)
                {
                    long id = 0;
                    if (long.TryParse(ddlEditOpUser.SelectedValue, out id) == false)
                    {
                        throw new InvalidCastException("Couldn`t parse ddlEditOpUser.SelectedValue to long.");
                    }

                    newUser = bUser.Get(objectContext, id, true, false);
                    if (newUser == null)
                    {
                        Response.Redirect("Operations.aspx");
                    }

                    if (bOperation.CheckIfUserIsParticipatingInOperation(op, newUser) == true)
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationChooseOtherMember").ToString();
                    }
                    else
                    {
                        changeMade = true;
                    }
                }

                if (opUserEdited.participatingLength.ToString() != tbEditUserTime.Text)
                {
                    if (!string.IsNullOrEmpty(tbEditUserTime.Text))
                    {
                        if (int.TryParse(tbEditUserTime.Text, out newLength) == false)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationMemberLengthFormat").ToString();
                        }
                        else if (newLength < 0)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationMemberLengthLess").ToString();
                        }
                        else if (newLength > op.opLength)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationMemberLengthMore").ToString();
                        }
                        else
                        {
                            changeMade = true;
                        }
                    }
                    else
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationMemberLengthType").ToString();
                    }

                }
                else
                {
                    newLength = opUserEdited.participatingLength;
                }


                if (newInfo != opUserEdited.info)
                {
                    changeMade = true;
                }

                if (errorOccured == false && changeMade == true)
                {
                    bOperation.ChangeOperationUser(objectContext, currCorporation, opUserEdited, newUser, newLength, newInfo, currUser);

                    HideAddEditPanels();
                    ShowStatusPanel(GetLocalResourceObject("statusOperationMemberUpdated").ToString());
                    ShowOperations();
                }
                else
                {
                    lblEditUserError.Visible = true;

                    if (errorOccured == true)
                    {
                        lblEditUserError.Text = error;
                    }
                    else if (changeMade == false)
                    {
                        lblEditUserError.Text = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                    } 
                }


                
            }
        }

        protected void btnEditLoot_Click(object sender, EventArgs e)
        {
            lock (modifyingOperation)
            {
                string strID = hfLootEdited.Value;
                OperationLoot opLootEdited = null;
                Operation op = null;
                Loot newLoot = null;
                int newQuantity = 0;
                double newPrice = 0;

                bool changeMade = false;
                bool errorOccured = false;
                string error = string.Empty;

                if (!string.IsNullOrEmpty(strID))
                {
                    long id = 0;
                    if (long.TryParse(strID, out id) == false)
                    {
                        throw new InvalidCastException("Couldn`t parse hfLootEdited.Value to long.");
                    }

                    opLootEdited = bOperation.GetOperationLoot(objectContext, id, false);
                    if (opLootEdited == null)
                    {
                        Response.Redirect("Operations.aspx");
                    }

                }
                else
                {
                    Response.Redirect("Operations.aspx");
                }

                if (!opLootEdited.Loot1Reference.IsLoaded)
                {
                    opLootEdited.Loot1Reference.Load();
                }
                if (!opLootEdited.OperationsReference.IsLoaded)
                {
                    opLootEdited.OperationsReference.Load();
                }

                op = opLootEdited.Operations;
                CheckIfUserCanEditOp(op, false);

                if (opLootEdited.Loot1.ID.ToString() != ddlEditLoot.SelectedValue)
                {
                    long id = 0;
                    if (long.TryParse(ddlEditLoot.SelectedValue, out id) == false)
                    {
                        throw new InvalidCastException("Couldn`t parse ddlEditLoot.SelectedValue to long.");
                    }

                    newLoot = bLoot.Get(objectContext, id, false);
                    if (newLoot == null)
                    {
                        Response.Redirect("Operations.aspx");
                    }

                    if (bOperation.CheckIfLootIsDroppedInOperation(op, newLoot) == true)
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationChooseOtherLoot").ToString();
                    }
                    else
                    {
                        changeMade = true;
                    }
                }

                if (opLootEdited.pricePerOne.ToString() != tbEditLootPrice.Text)
                {
                    if (!string.IsNullOrEmpty(tbEditLootPrice.Text))
                    {

                        tbEditLootPrice.Text = tbEditLootPrice.Text.Replace(",", "");
                        
                        if (double.TryParse(tbEditLootPrice.Text, out newPrice) == false)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationLootPriceFormat").ToString();
                        }
                        else if (newPrice < 0)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationLootPriceLess").ToString();
                        }
                        else
                        {
                            changeMade = true;
                        }
                    }
                    else
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationLootPriceEnter").ToString();
                    }

                }
                else
                {
                    newPrice = opLootEdited.pricePerOne;
                }

                if (opLootEdited.quantity.ToString() != tbEditLootQuantity.Text)
                {
                    if (!string.IsNullOrEmpty(tbEditLootQuantity.Text))
                    {
                        if (int.TryParse(tbEditLootQuantity.Text, out newQuantity) == false)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationLootQuantityFormat").ToString();
                        }
                        else if (newQuantity < 1)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationLootQuantityLess").ToString();
                        }
                        else
                        {
                            changeMade = true;
                        }
                    }
                    else
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationLootQuantityType").ToString();
                    }

                }
                else
                {
                    newQuantity = opLootEdited.quantity;
                }
                

                if (errorOccured == false && changeMade == true)
                {
                    bOperation.ChangeOperationLoot(objectContext, currCorporation, opLootEdited, newLoot, newPrice, newQuantity, currUser);

                    HideAddEditPanels();
                    ShowStatusPanel(GetLocalResourceObject("statusOperationLootUpdated").ToString());
                    ShowOperations();
                }
                else
                {
                    lblEditLootError.Visible = true;

                    if (errorOccured == true)
                    {
                        lblEditLootError.Text = error;
                    }
                    else if (changeMade == false)
                    {
                        lblEditLootError.Text = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                    }
                }



            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {


            lock (modifyingOperation)
            {
                string strID = hfOpEdited.Value;
                Operation op = GetCurrEditedOperation(strID);

                CheckIfUserCanEditOp(op, false);

                User newUser = null;
                int time = 0;
                string info = tbAddUserInfo.Text;

                bool errorOccured = false;
                string error = string.Empty;

                if (ddlAddUser.SelectedIndex > 0)
                {
                    long id = 0;
                    if (long.TryParse(ddlAddUser.SelectedValue, out id) == false)
                    {
                        throw new InvalidCastException("Couldnt parse ddlAddUser.SelectedValue to long.");
                    }

                    newUser = bUser.Get(objectContext, id, true, false);
                    if (newUser == null)
                    {
                        Response.Redirect("Operations.aspx");
                    }

                    if (bOperation.CheckIfUserIsParticipatingInOperation(op, newUser) == true)
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationChooseOtherMember").ToString();
                    }
                }
                else
                {
                    errorOccured = true;
                    error = GetLocalResourceObject("errOperationMemberChoose").ToString();
                }

                if (errorOccured == false)
                {
                    if (!string.IsNullOrEmpty(tbAddUserTime.Text))
                    {
                        if (int.TryParse(tbAddUserTime.Text, out time) == false)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationMemberLengthFormat").ToString();
                        }
                        else if (time < 0)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationMemberLengthLess").ToString();
                        }
                        else if (time > op.opLength)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationMemberLengthMore").ToString();
                        }
                    }
                    else
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationMemberLengthType").ToString();
                    }
                }

                if (errorOccured == false)
                {
                    bOperation.AddOperationUser(objectContext, currCorporation, op, newUser, time, info, currUser);

                    HideAddEditPanels();
                    ShowStatusPanel(GetLocalResourceObject("statusOperationMemberAdded").ToString());
                    ShowOperations();
                }
                else
                {
                    lblAddUserError.Text = error;
                    lblAddUserError.Visible = true;
                }
               
            }
        }

        protected void btnAddLoot_Click(object sender, EventArgs e)
        {
            lock (modifyingOperation)
            {
                string strID = hfOpEdited.Value;
                Operation op = GetCurrEditedOperation(strID);

                CheckIfUserCanEditOp(op, false);

                Loot newLoot = null;
                double price = 0;
                int quantity = 0;

                bool errorOccured = false;
                string error = string.Empty;

                if (ddlAddLoot.SelectedIndex > 0)
                {
                    long id = 0;
                    if (long.TryParse(ddlAddLoot.SelectedValue, out id) == false)
                    {
                        throw new InvalidCastException("Couldnt parse ddlAddLoot.SelectedValue to long.");
                    }

                    newLoot = bLoot.Get(objectContext, id, false);
                    if (newLoot == null)
                    {
                        Response.Redirect("Operations.aspx");
                    }

                    if (bOperation.CheckIfLootIsDroppedInOperation(op, newLoot) == true)
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationChooseOtherLoot").ToString();
                    }
                }
                else
                {
                    errorOccured = true;
                    error = GetLocalResourceObject("errOperationLootChoose").ToString();
                }

                if (errorOccured == false)
                {
                    if (!string.IsNullOrEmpty(tbAddLootPrice.Text))
                    {

                        tbAddLootPrice.Text = tbAddLootPrice.Text.Replace(",", "");

                        if (double.TryParse(tbAddLootPrice.Text, out price) == false)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationLootPriceFormat").ToString();
                        }
                        else if (price < 0)
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errOperationLootPriceLess").ToString();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(tbAddLootQuantity.Text))
                            {
                                if (int.TryParse(tbAddLootQuantity.Text, out quantity) == false)
                                {
                                    errorOccured = true;
                                    error = GetLocalResourceObject("errOperationLootQuantityFormat").ToString();
                                }
                                else if (quantity < 1)
                                {
                                    errorOccured = true;
                                    error = GetLocalResourceObject("errOperationLootQuantityLess").ToString();
                                }
                            }
                            else
                            {
                                errorOccured = true;
                                error = GetLocalResourceObject("errOperationLootQuantityType").ToString();
                            }
                        }

                    }
                    else
                    {
                        errorOccured = true;
                        error = GetLocalResourceObject("errOperationLootPriceEnter").ToString();
                    }
                }

                if (errorOccured == false)
                {
                    bOperation.AddOperationLoot(objectContext, currCorporation, op, newLoot, price, quantity, currUser);

                    HideAddEditPanels();
                    ShowStatusPanel(GetLocalResourceObject("statusOperationLootAdded").ToString());
                    ShowOperations();
                }
                else
                {
                    lblAddLootError.Text = error;
                    lblAddLootError.Visible = true;
                }

            }
        }

        protected void cbAddAllRows_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAddAllRows.Checked == true)
            {
                AddAllLootRowsBasedOnOperationType();
            }
        }



        


    }
}