﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WhSpace
{
    public partial class Logs : BasePage
    {
        Entities objectContext = new Entities();

        User currUser = null;
        Corporation currCorporation = null;

        BUser bUser = new BUser();
        BLog bLog = new BLog();

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckUserAndCorp();

            ShowLogsPanel();
            FillLogs(null, 50);

            SetLocalText();
        }

        private void SetLocalText()
        {
            Title = string.Format("{0} : {1}", currCorporation.name, GetLocalResourceObject("title"));

            lblStaticLastLogsFor.Text = GetLocalResourceObject("staticLastLogsFor").ToString();

            btnShowLogs.Text = GetGlobalResourceObject("Resources", "btnShow").ToString();

            tbLogsCount_TextBoxWatermarkExtender.WatermarkText = GetLocalResourceObject("count").ToString();
        }

        private void ShowLogsPanel()
        {
            if (bLog.CountLogs(objectContext, currCorporation) > 10)
            {
                pnlShowLogs.Visible = true;

                if (IsPostBack == false)
                {
                    ddlUsers.Controls.Clear();

                    ListItem first = new ListItem();
                    first.Text = GetLocalResourceObject("ddlShowLogsForAll").ToString();
                    first.Value = "0";
                    ddlUsers.Items.Add(first);

                    ddlUsers.SelectedIndex = 0;

                    List<User> users = bUser.GetVisibleUsers(objectContext, currCorporation);
                    foreach (User user in users)
                    {
                        ListItem item = new ListItem();
                        item.Text = user.name;
                        item.Value = user.ID.ToString();
                        ddlUsers.Items.Add(item);
                    }
                }
            }
            else
            {
                pnlShowLogs.Visible = false;
            }
        }

        private void FillLogs(User forUser, int count)
        {
            tblLogs.Rows.Clear();

            List<Log> logs = new List<Log>();

            if (forUser == null)
            {
                logs = bLog.GetLastLogs(objectContext, currCorporation, count);
            }
            else
            {
                logs = bLog.GetLastLogsFor(objectContext, currCorporation, forUser, count);
            }
            

            if (logs != null && logs.Count > 0)
            {
                lblNoLogs.Visible = false;

                TableRow zeroRow = new TableRow();
                tblLogs.Rows.Add(zeroRow);

                TableCell zeroCell = new TableCell();
                zeroRow.Cells.Add(zeroCell);

                if (forUser == null)
                {
                    zeroCell.Text = string.Format("{0} {1} {2} ", GetLocalResourceObject("lastLogs")
                        , logs.Count, GetLocalResourceObject("lastLogs2"));
                }
                else
                {
                    zeroCell.Text = string.Format("{0} {1} {2} {3} : ", GetLocalResourceObject("lastLogs")
                        , logs.Count, GetLocalResourceObject("lastLogsFor"), forUser.name);
                }
                
                zeroCell.HorizontalAlign = HorizontalAlign.Center;
                zeroCell.ColumnSpan = 3;
                zeroCell.CssClass = "headerText";
                zeroCell.Attributes.CssStyle.Add(HtmlTextWriterStyle.PaddingBottom, "10px");

                TableRow firstRow = new TableRow();
                tblLogs.Rows.Add(firstRow);

                TableCell fdateCell = new TableCell();
                firstRow.Cells.Add(fdateCell);
                fdateCell.Text = GetLocalResourceObject("logsDate").ToString();
                fdateCell.Width = Unit.Pixel(170);
                fdateCell.HorizontalAlign = HorizontalAlign.Center;

                TableCell fuserCell = new TableCell();
                firstRow.Cells.Add(fuserCell);
                fuserCell.Text = GetLocalResourceObject("logsUser").ToString();
                fuserCell.Width = Unit.Pixel(200);
                fuserCell.CssClass = "users";
                fuserCell.HorizontalAlign = HorizontalAlign.Center;

                TableCell finfoCell = new TableCell();
                firstRow.Cells.Add(finfoCell);
                finfoCell.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                    + GetLocalResourceObject("logsDescription").ToString();
                


                foreach (Log log in logs)
                {
                    if (!log.UsersReference.IsLoaded)
                    {
                        log.UsersReference.Load();
                    }

                    TableRow newRow = new TableRow();
                    tblLogs.Rows.Add(newRow);

                    TableCell dateCell = new TableCell();
                    newRow.Cells.Add(dateCell);
                    dateCell.Text = Tools.GetDateTimeInLocalFormat(log.dateAdded);
                    dateCell.CssClass = "operationUserLootCells";

                    TableCell userCell = new TableCell();
                    newRow.Cells.Add(userCell);
                    userCell.Text = log.Users.name;
                    userCell.CssClass = "users operationUserLootCells";

                    TableCell infoCell = new TableCell();
                    newRow.Cells.Add(infoCell);
                    infoCell.Text = log.description;
                    infoCell.CssClass = "operationUserLootCells";
                }

            }
            else
            {
                lblNoLogs.Visible = true;

                if (forUser == null)
                {
                    lblNoLogs.Text = GetLocalResourceObject("noLogs").ToString();
                }
                else
                {
                    lblNoLogs.Text = string.Format("{0} {1}.", GetLocalResourceObject("noLogsFor").ToString(), forUser.name);
                }
                
            }

        }





        private void CheckUserAndCorp()
        {
            currUser = GetCurrUser(objectContext, true);
            currCorporation = GetUserCorporation(objectContext, currUser, true);
        }

        protected void btnShowLogs_Click(object sender, EventArgs e)
        {
            string error = string.Empty;
            bool errorOccured = false;

            User selectedUser = null;
            if (ddlUsers.SelectedIndex > 0)
            {
                long id = 0;
                if (long.TryParse(ddlUsers.SelectedValue, out id) == false)
                {
                    throw new InvalidCastException("Couldnt parse ddlUsers.SelectedValue to long");
                }

                selectedUser = bUser.Get(objectContext, id, true, false);
                if (selectedUser == null)
                {
                    Response.Redirect("Logs.aspx");
                }
            }

            int count = 0;

            if (int.TryParse(tbLogsCount.Text, out count) == false)
            {
                error = GetLocalResourceObject("errInvalidLogsFormat").ToString();
                errorOccured = true;
            }
            else if (count < 1)
            {
                error = GetLocalResourceObject("errInvalidLogsCount").ToString();
                errorOccured = true;
            }
            else if (count > 1000)
            {
                error = GetLocalResourceObject("errInvalidLogsCount2").ToString();
                errorOccured = true;
            }

            if (errorOccured == false)
            {
                lblShowLogsError.Visible = false;

                FillLogs(selectedUser, count);
            }
            else
            {
                lblShowLogsError.Visible = true;
                lblShowLogsError.Text = error;
            }

        }





    }
}