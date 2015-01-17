﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace WhSpace
{
    public partial class Main : BasePage 
    {
        protected object addingSystem = new object();
        protected object addingSignature = new object();
        protected object modifyingSystem = new object();
        protected object modifyingSignature = new object();
        protected object modifyingWh = new object();

        protected string StellarSystemTreeJson { get; set; }

        Entities objectContext = new Entities();
        User currUser = null;
        Corporation currCorporation = null;
        Systems selectedSystem = null;

        BSystem bSystem = new BSystem();
        BSignature bSignature = new BSignature();
        BUser bUser = new BUser();
        BWormwhole bWormwhole = new BWormwhole();
        BStaticSystem bStaticSystem = new BStaticSystem();

        protected void Page_PreRender(object sender, EventArgs e)
        {
            hfPageLoaded.Value = DateTime.UtcNow.ToString("O");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckUserAndCorp();

            bStaticSystem.UpdateApiSystemsData(objectContext);

            GetSelectedSystem();
            CheckSelectedSystemAndEditedSystem();

            FillSystemsForJs();
            FillShowSystemDataPanel();
            HideStatusPanel();

            //if (currUser.admin == false && currUser.canEdit == false)
            //{
            //    pnlSiteErrors.Visible = false;
            //}
            //else
            //{
            //    pnlSiteErrors.Visible = true;
            //}

            SetLocalText();
        }

        private void SetLocalText()
        {
            Title = string.Format("{0} : {1}", currCorporation.name, GetLocalResourceObject("title").ToString());

            lblStaticStatus.Text = GetGlobalResourceObject("Resources", "StatusStatic").ToString();
            lblStaticInfoForSys.Text = GetLocalResourceObject("staticInfoForSys").ToString();
            lblStaticSysConnectedWith.Text = GetLocalResourceObject("staticSysConnectedWith").ToString();
            lblStaticSysSignatures.Text = GetLocalResourceObject("staticSysSignatures").ToString();
            lblStaticSysOccupied.Text = GetLocalResourceObject("staticSysOccupied").ToString();
            lblStaticSysInfo.Text = GetLocalResourceObject("staticSysInfo").ToString();
            lblStaticSysAddedBy.Text = GetLocalResourceObject("staticSysAddedBy").ToString();
            lblStaticSysAdded.Text = GetLocalResourceObject("staticSysAdded").ToString();
            ibDeleteSystem_ConfirmButtonExtender.ConfirmText = GetLocalResourceObject("staticSystemDeleteConfirm").ToString();
            lblStaticSysApiKills.Text = GetLocalResourceObject("infoPnlApiKills").ToString();
            lblStaticSysApiJumps.Text = GetLocalResourceObject("infoPnlApiJumps").ToString();
            lblStaticSysFirstDiscovered.Text = GetLocalResourceObject("staticSystemFirstDiscovery").ToString();
            lblStaticSysLastDiscovered.Text = GetLocalResourceObject("staticSystemLastDiscovery").ToString();

            ///
            lblStaticEditWormhole.Text = GetLocalResourceObject("staticEditWormhole").ToString();
            lblStaticEditWormholeFrom.Text = GetLocalResourceObject("staticEditWormholeFrom").ToString();
            lblStaticEditWormholeTo.Text = GetLocalResourceObject("staticEditWormholeTo").ToString();
            lblStaticEditWormholeNewId.Text = GetLocalResourceObject("staticWormholeNewId").ToString();

            btnChangeWhID.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnCancelEditWh.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///
            
            ///
            lblStaticEditSignature.Text = GetLocalResourceObject("staticEditSignature").ToString();
            lblStaticEditSignatureNewName.Text = GetLocalResourceObject("staticEditSignatureNewName").ToString();
            lblStaticEditSignatureNewType.Text = GetLocalResourceObject("staticEditSignatureNewType").ToString();

            ddlEditSignature.Items[0].Text = GetLocalResourceObject("staticSignatureTypeNotScanned").ToString();
            ddlEditSignature.Items[1].Text = GetLocalResourceObject("staticSignatureTypeWormwhole").ToString();

            btnEditSignature.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            bntCancelEditSignature.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///
            
            ///
            lblStaticEditSystem.Text = GetLocalResourceObject("staticEditSystem").ToString();
            lblStaticEditSysNewName.Text = GetLocalResourceObject("staticEditSysNewName").ToString();
            lblStaticEditSysNewClass.Text = GetLocalResourceObject("staticEditSysNewClass").ToString();
            lblStaticEditSysNewEffect.Text = GetLocalResourceObject("staticEditSysNewEffect").ToString();

            ddlEditSysClass.Items[0].Text = GetLocalResourceObject("staticSystemClassUnknown").ToString();
            ddlEditSysClass.Items[1].Text = GetLocalResourceObject("staticSystemClassWormhole").ToString();

            ddlEditSysEffect.Items[0].Text = GetLocalResourceObject("staticSystemEffectUnknown").ToString();
            ddlEditSysEffect.Items[1].Text = GetLocalResourceObject("staticSystemEffectNoEffect").ToString();

            lblStaticEditSystemOccupied.Text = GetLocalResourceObject("staticEditSystemOccupied").ToString();
            lblStaticEditSystemInfo.Text = GetLocalResourceObject("staticEditSystemInfo").ToString();

            btnChangeSysName.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnChangeSysOccupied.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnChangeSysInfo.Text = GetGlobalResourceObject("Resources", "btnEdit").ToString();
            btnCancelEditSystem.Text = GetGlobalResourceObject("Resources", "btnClose").ToString();
            ///

            ///
            lblStaticAddSignature.Text = GetLocalResourceObject("staticAddSignature").ToString();
            lblStaticAddSignatureName.Text = GetLocalResourceObject("staticAddSignatureName").ToString();
            lblStaticAddSignatureType.Text = GetLocalResourceObject("staticAddSignatureType").ToString();

            ddlNewSigType.Items[0].Text = GetLocalResourceObject("staticSignatureTypeNotScanned").ToString();
            ddlNewSigType.Items[1].Text = GetLocalResourceObject("staticSignatureTypeWormwhole").ToString();

            tbNewSignatureInfo_TextBoxWatermarkExtender.WatermarkText = GetLocalResourceObject("staticNewSigInfoWatermark").ToString();

            btnAddSignature.Text = GetGlobalResourceObject("Resources", "btnAdd").ToString();
            ///

            ///
            lblStaticAddNewSystem.Text = GetLocalResourceObject("staticAddNewSystem").ToString();
            lblStaticAddSystemName.Text = GetLocalResourceObject("staticAddSystemName").ToString();
            lblStaticAddSystemClass.Text = GetLocalResourceObject("staticAddSystemClass").ToString();
            lblStaticAddSystemEffect.Text = GetLocalResourceObject("staticAddSystemEffect").ToString();
            lblStaticAddSystemWormIDToIt.Text = GetLocalResourceObject("staticAddSystemWormIDToIt").ToString();
            lblStaticAddSystemWormIDFromIt.Text = GetLocalResourceObject("staticAddSystemWormIDFromIt").ToString();
            lblStaticAddSystemOccupied.Text = GetLocalResourceObject("staticEditSystemOccupied").ToString();
            lblStaticAddSystemInfo.Text = GetLocalResourceObject("staticEditSystemInfo").ToString();

            ddlNewSysClass.Items[0].Text = GetGlobalResourceObject("Resources", "ddlAutomatic").ToString();
            ddlNewSysClass.Items[1].Text = GetLocalResourceObject("staticSystemClassWormhole").ToString();

            ddlNewSysEffect.Items[0].Text = GetGlobalResourceObject("Resources", "ddlChoose").ToString();
            ddlNewSysEffect.Items[1].Text = GetLocalResourceObject("staticSystemEffectNoEffect").ToString();

            btnAddSystem.Text = GetGlobalResourceObject("Resources", "btnAdd").ToString();
            ///

            lblStaticSiteErrors.Text = GetLocalResourceObject("staticSiteErrors").ToString();
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

        ///////////////////////////////////////////////////////////

        private void CheckSelectedSystemAndEditedSystem()
        {
            if (selectedSystem != null)
            {
                if (selectedSystem.ID.ToString() != hfSystemEdited.Value)
                {
                    SetSelectedSystemAsEdited();

                    if (currUser.admin == true || currUser.canEdit == true)
                    {
                        FillAddSystemPanel();
                        FillAddSignaturePanel();
                    }
                    else
                    {
                        pnlAddSystem.Visible = false;
                        pnlAddSignature.Visible = false;
                    }

                    pnlEditSignature.Visible = false;
                    hfEditSignatureId.Value = null;

                    pnlEditSystem.Visible = false;

                    pnlEditWormwholes.Visible = false;
                    hfEditWhId.Value = null;
                }
            }
            else
            {
                hfSystemEdited.Value = null;
                HidePanels();
            }
            
        }

        private void FillSystemsForJs()
        {
            List<StellarSystemData> stellarSystemData = LoadSystems();
            StellarSystemTreeJson = StellarSystemsJson(stellarSystemData);
        }

        private List<StellarSystemData> LoadSystems()
        {
            List<StellarSystemData> stellarSystemDataList = new List<StellarSystemData>();

            Systems root = bSystem.GetRoot(objectContext, currCorporation);
            StellarSystemData rootData = new StellarSystemData();
            int levelX = 0;
            int levelY = 0;

            rootData.ID = root.ID;
            rootData.LevelX = levelX;
            rootData.LevelY = levelY;
            rootData.Name = root.name;
            rootData.SysClass = bSystem.GetSystemClass(root);

            rootData.ParentID = -1;
            rootData.WhFromParent = string.Empty;
            rootData.WhToParent = string.Empty;

            stellarSystemDataList.Add(rootData);

            List<Systems> childSystems = bSystem.GetChildSystems(root);
            List<string> whIds = new List<string>();
            List<Systems> currSystems = new List<Systems>();
            currSystems.Add(root);

            AddChildSystemNodes(stellarSystemDataList, childSystems, root, levelX + 1, ref levelY, ref whIds, ref currSystems);

            FillWhInfoPanels(whIds);
            FillSystemInfoPanels(currSystems);

            //MasterPage master = this.Master as MasterPage;
            //master.SetPageDivWidth(levelX);

            return stellarSystemDataList;

            
        }

        private void AddChildSystemNodes(List<StellarSystemData> stellarSystemsList, List<Systems> systems, Systems parent,
            int levelX, ref int levelY, ref List<string> whIds, ref List<Systems> currSystems)
        {
            if (stellarSystemsList == null)
            {
                throw new ArgumentNullException("stellarSystemsList");
            }
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            if (systems != null && systems.Count > 0)
            {

                List<Wormwhole> sysWhs = new List<Wormwhole>();

                //foreach (Systems system in systems)
                for (int i = 0; i < systems.Count; i++)
                {
                    Systems system = systems[i];
                    currSystems.Add(systems[i]);

                    StellarSystemData nodeData = new StellarSystemData();
                    
                    if (i > 0)
                    {
                        levelY++;
                    }
                    nodeData.ID = system.ID;
                    nodeData.LevelX = levelX;
                    nodeData.LevelY = levelY;
                    nodeData.Name = system.name;
                    nodeData.SysClass = bSystem.GetSystemClass(system);
                    nodeData.ParentID = parent.ID;

                    bWormwhole.GetWhToSystem(system, ref sysWhs);
                    if (sysWhs != null && sysWhs.Count > 0)
                    {
                        if (!sysWhs[0].FromSystemsReference.IsLoaded)
                        {
                            sysWhs[0].FromSystemsReference.Load();
                        }
                        if (sysWhs[0].FromSystems.ID != parent.ID)
                        {
                            throw new InvalidOperationException("system parent is not the same as in system's wormhole data");
                        }

                        nodeData.WhFromParent = sysWhs[0].fromSysWormwholeID;
                        nodeData.WhToParent = sysWhs[0].toSysWormwholeID;

                        if (!whIds.Contains(nodeData.WhFromParent))
                        {
                            whIds.Add(nodeData.WhFromParent);
                        }
                        if (!whIds.Contains(nodeData.WhToParent))
                        {
                            whIds.Add(nodeData.WhToParent);
                        }

                    }

                    stellarSystemsList.Add(nodeData);

                    List<Systems> childSystems = bSystem.GetChildSystems(system);
                    AddChildSystemNodes(stellarSystemsList, childSystems, system, levelX + 1, ref levelY, ref whIds, ref currSystems);

                }
            }

        }

        private void FillWhInfoPanels(List<string> whIds)
        {
            phWhInfoPanels.Controls.Clear();

            if (whIds.Count < 1)
            {
                return;
            }

            Panel noInfoPanel = new Panel();
            noInfoPanel.ID = "whNoInfo";
            phWhInfoPanels.Controls.Add(noInfoPanel);
            noInfoPanel.CssClass = "whInfoPanel";

            noInfoPanel.HorizontalAlign = HorizontalAlign.Center;
            Label noInfoLbl = new Label();
            noInfoPanel.Controls.Add(noInfoLbl);
            noInfoLbl.Text = GetLocalResourceObject("infoPnlNoInfo").ToString();

            WormwholeIdentification wh = null;

            foreach (string whId in whIds)
            {
                if (!string.IsNullOrEmpty(whId))
                {
                    wh = bWormwhole.GetWormwholeIdentification(objectContext, whId, false);

                    if (wh != null)
                    {
                        Panel newPanel = new Panel();
                        newPanel.ID = string.Format("wh{0}info", wh.ID);
                        phWhInfoPanels.Controls.Add(newPanel);
                        newPanel.CssClass = "whInfoPanel";

                        Panel mainPnl = new Panel();
                        newPanel.Controls.Add(mainPnl);
                        mainPnl.CssClass = "clearfix2";

                        Panel leftPnl = new Panel();
                        mainPnl.Controls.Add(leftPnl);
                        leftPnl.CssClass = "leftPnl";

                        Panel centerPnl = new Panel();
                        mainPnl.Controls.Add(centerPnl);
                        centerPnl.CssClass = "centerPnl";

                        if (wh.target != null)
                        {
                            leftPnl.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoPnlTarget").ToString(), true));
                            centerPnl.Controls.Add(Tools.GetLabel(wh.target, true));
                        }
                        if (wh.lifetime != null)
                        {
                            leftPnl.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoPnlLifetime").ToString(), true));
                            centerPnl.Controls.Add(Tools.GetLabel(string.Format("{0} {1}", wh.lifetime, GetLocalResourceObject("infoPnlLifetimeHours")), true));
                        }
                        if (wh.maxMass != null)
                        {
                            leftPnl.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoPnlMaxMass").ToString(),true));

                            centerPnl.Controls.Add(Tools.GetLabel(
                                string.Format("{0} {1}", Tools.BreakLongNumber(wh.maxMass.Value), GetLocalResourceObject("infoPnlKilograms")), true));
                        }
                        if (wh.totalMass != null)
                        {
                            leftPnl.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoPnlTotalMass").ToString(), true));

                            centerPnl.Controls.Add(Tools.GetLabel(
                                string.Format("{0} {1}", Tools.BreakLongNumber(wh.totalMass.Value), GetLocalResourceObject("infoPnlKilograms")), false));
                        }

                    }
                }

            }
        }

        private void FillSystemInfoPanels(List<Systems> systems)
        {
            phSystemInfoPanels.Controls.Clear();

            if (systems.Count < 1)
            {
                return;
            }

            StaticSystemData ssd = null;

            foreach (Systems system in systems)
            {
                system.Signatures.Load();
                   
                Panel newPanel = new Panel();
                newPanel.ID = string.Format("sys{0}info", system.ID);
                phSystemInfoPanels.Controls.Add(newPanel);
                newPanel.CssClass = "whInfoPanel";

                string text = string.Empty;

                if (system.sysClass.HasValue == false || system.sysClass < 8 || system.sysClass > 10) // not low/high/zero sec system
                {

                    Panel mainPnl = new Panel();
                    newPanel.Controls.Add(mainPnl);
                    mainPnl.CssClass = "clearfix2";

                    Panel leftPnl = new Panel();
                    mainPnl.Controls.Add(leftPnl);
                    leftPnl.CssClass = "leftPnl";

                    Panel centerPnl = new Panel();
                    mainPnl.Controls.Add(centerPnl);
                    centerPnl.CssClass = "centerPnl";

                    if (system.sysEffect.HasValue == true)
                    {
                        leftPnl.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoPnlEffect").ToString(), true)); 
                        centerPnl.Controls.Add(Tools.GetLabel(bSystem.GetSystemEffect(system), true));
                    }

                    leftPnl.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoPnlSignatures").ToString(), true));
                    centerPnl.Controls.Add(Tools.GetLabel(system.Signatures.Count.ToString(), true));
                }

                if (!string.IsNullOrEmpty(system.occupied))
                {
                    Panel mainOcc = new Panel();
                    newPanel.Controls.Add(mainOcc);
                    mainOcc.CssClass = "clearfix2";

                    Panel leftOccPnl = new Panel();
                    mainOcc.Controls.Add(leftOccPnl);
                    leftOccPnl.CssClass = "leftPnl";

                    Panel centerOccPnl = new Panel();
                    mainOcc.Controls.Add(centerOccPnl);
                    centerOccPnl.CssClass = "centerPnl";

                    leftOccPnl.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoPnlOccupied").ToString(), true));

                    text = Tools.GetFormattedTextFromDB(system.occupied);
                    text = Tools.TrimString(text, 300, false, true);

                    Label lblOccupied = Tools.GetLabel(text, true);
                    centerOccPnl.Controls.Add(lblOccupied);
                    lblOccupied.CssClass = "occupation";
                }

                if (!string.IsNullOrEmpty(system.info))
                {

                    Panel mainInfo = new Panel();
                    newPanel.Controls.Add(mainInfo);
                    mainInfo.CssClass = "clearfix2";

                    Panel leftInfoPnl = new Panel();
                    mainInfo.Controls.Add(leftInfoPnl);
                    leftInfoPnl.CssClass = "leftPnl";

                    Panel centerInfoPnl = new Panel();
                    mainInfo.Controls.Add(centerInfoPnl);
                    centerInfoPnl.CssClass = "centerPnl";

                    leftInfoPnl.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoPnlInfo").ToString(), true));

                    text = Tools.GetFormattedTextFromDB(system.info);
                    text = Tools.TrimString(text, 300, false, true);

                    Label lblSysInfo = Tools.GetLabel(text, false);
                    centerInfoPnl.Controls.Add(lblSysInfo);
                    lblSysInfo.CssClass = "info";
                }

                ssd = bStaticSystem.GetStaticSystem(objectContext, system.name, false);
                if (ssd != null)
                {
                    Panel apiKillsInfo = new Panel();
                    newPanel.Controls.Add(apiKillsInfo);
                    apiKillsInfo.CssClass = "clearfix2";

                    Panel leftApiKillsPnl = new Panel();
                    apiKillsInfo.Controls.Add(leftApiKillsPnl);
                    leftApiKillsPnl.CssClass = "leftPnl";

                    Panel centerApiKillsPnl = new Panel();
                    apiKillsInfo.Controls.Add(centerApiKillsPnl);
                    centerApiKillsPnl.CssClass = "centerPnl";

                    leftApiKillsPnl.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoPnlApiKills").ToString(), true));
                    centerApiKillsPnl.Controls.Add(Tools.GetLabel(string.Format("{0}:{1}, {2}:{3}, {4}:{5}"
                        , GetLocalResourceObject("infoApiShipKills"), ssd.shipKills, GetLocalResourceObject("infoApiPodKills"), ssd.podKills
                        , GetLocalResourceObject("infoApiNpcKills"), ssd.npcKills), true));

                    Panel apiJumpsInfo = new Panel();
                    newPanel.Controls.Add(apiJumpsInfo);
                    apiJumpsInfo.CssClass = "clearfix2";

                    Panel leftApiJumpsPnl = new Panel();
                    apiJumpsInfo.Controls.Add(leftApiJumpsPnl);
                    leftApiJumpsPnl.CssClass = "leftPnl";

                    Panel centerApiJumpsPnl = new Panel();
                    apiJumpsInfo.Controls.Add(centerApiJumpsPnl);
                    centerApiJumpsPnl.CssClass = "centerPnl";

                    leftApiJumpsPnl.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoPnlApiJumps").ToString(), true));
                    centerApiJumpsPnl.Controls.Add(Tools.GetLabel(string.Format("{0}", ssd.jumps), true));
                }

                if (newPanel.Controls.Count < 1)
                {
                    newPanel.HorizontalAlign = HorizontalAlign.Center;
                    newPanel.Controls.Add(Tools.GetLabel(GetLocalResourceObject("infoPnlNoInfo").ToString(), false));
                }

                    
             }

            
        }

        private string StellarSystemsJson(List<StellarSystemData> stellarSystems)
        {
            if (stellarSystems == null)
            {
                throw new ArgumentNullException("stellarSystems");
            }

            StringBuilder stellarSystemsJson = new StringBuilder();
            //stellarSystemsJson.Append("{");
            //stellarSystemsJson.Append("\"stellarSystemsList\": ");
            //stellarSystemsJson.AppendLine();
            stellarSystemsJson.Append("[");
            stellarSystemsJson.AppendLine();
            for (int i = 0; i < stellarSystems.Count; i++)
            {
                StellarSystemData stellarSystemData = stellarSystems[i];
                string singleSystemJson = stellarSystemData.ToJsonString();
                stellarSystemsJson.Append(singleSystemJson);
                if (i < (stellarSystems.Count - 1))
                {
                    stellarSystemsJson.Append(",");
                }
                stellarSystemsJson.AppendLine();
            }
            stellarSystemsJson.Append("]");
            //stellarSystemsJson.AppendLine();
            //stellarSystemsJson.Append("}");

            string result = stellarSystemsJson.ToString();
            return result;
        }

        private class StellarSystemData
        {
            public long ID { get; set; }
            public string Name { get; set; }

            public int LevelX { get; set; }
            public int LevelY { get; set; }

            public long ParentID { get; set; }
            public string WhToParent { get; set; }
            public string WhFromParent { get; set; }

            public string SysClass { get; set; }

            public string ToJsonString()
            {
                string result =
                    string.Format("{{ \"ID\": {0}, \"Name\": \"{1}\", \"LevelX\": {2}, \"LevelY\": {3}, \"ParentID\": {4}, \"WhToParent\": \"{5}\", \"WhFromParent\": \"{6}\", \"SysClass\": \"{7}\" }}",
                    ID, Name, LevelX, LevelY, ParentID, WhToParent, WhFromParent, SysClass);
                return result;
            }
        }

     
        ///////////////////////////////////////////////////////////



        private void GetSelectedSystem()
        {

            if (!string.IsNullOrEmpty(hfSelectedSys.Value))
            {
                long id = 0;

                if (long.TryParse(hfSelectedSys.Value, out id) == false)
                {
                    throw new ArgumentException("couldnt parse hfSelectedSys.Value to long");
                }

                selectedSystem = bSystem.Get(objectContext, currCorporation, id, false);

            }
            
        }

        private void CheckUserAndCorp()
        {
            currUser = GetCurrUser(objectContext, true);
            currCorporation = GetUserCorporation(objectContext, currUser, true);
        }

        private void FillAddSignaturePanel()
        {
            CheckSystemAndUser();

            if (currUser.canEdit == true || currUser.admin == true)
            {
                pnlAddSignature.Visible = true;
                lblAddSignatureError.Visible = false;

                lblSignatureToSystem.Text = selectedSystem.name;
                tbNewSigName.Text = string.Empty;
                ddlNewSigType.SelectedIndex = 0;
                tbNewSignatureInfo.Text = string.Empty;
            }
            else
            {
                pnlAddSignature.Visible = false;
            }
        }

        private void FillAddSystemPanel()
        {
            CheckSystemAndUser();

            if (currUser.canEdit == true || currUser.admin == true)
            {
                pnlAddSystem.Visible = true;
                lblAddSystemError.Visible = false;

                lblFromSystem.Text = selectedSystem.name;

                tbFromSysWhID.Text = string.Empty;
                tbNewSysName.Text = string.Empty;
                tbToSysWhID.Text = string.Empty;
                tbNewSysOccupied.Text = string.Empty;
                tbNewSysInfo.Text = string.Empty;

                ddlNewSysClass.SelectedIndex = 0;
                ddlNewSysEffect.SelectedIndex = 0;
            }
            else
            {
                pnlAddSystem.Visible = false;
            }

        }

        private void CheckSystemAndUser()
        {
            if (selectedSystem == null)
            {
                //throw new InvalidOperationException("selectedSystem is null");
                Response.Redirect("Main.aspx");
            }

            if (currUser == null)
            {
                //throw new InvalidOperationException("currUser is null");
                Response.Redirect("Index.aspx");
            }
        }

        private void FillShowSystemDataPanel()
        {
            if (selectedSystem != null)
            {
                pnlSystemInfo.Visible = true;

                hlSysName.Text = selectedSystem.name;
                hlSysName.NavigateUrl = Tools.GetSystemNameRedirrectUrl(selectedSystem.name);

                if (selectedSystem.sysClass == null && selectedSystem.sysEffect == null)
                {
                    pnlSysClassAndEffect.Visible = false;
                }
                else
                {
                    pnlSysClassAndEffect.Visible = true;

                    lblSysClass.Text = bSystem.GetSystemClass(selectedSystem);
                    lblSysEffect.Text = bSystem.GetSystemEffect(selectedSystem);
                }
                
                FillSystemConnections();
                FillSystemSignatures();

                if (!string.IsNullOrEmpty(selectedSystem.occupied))
                {
                    pnlSysInfoOccupied.Visible = true;
                    lblSysOccupied.Text = Tools.GetFormattedTextFromDB(selectedSystem.occupied);
                }
                else
                {
                    pnlSysInfoOccupied.Visible = false;
                }

                if (!string.IsNullOrEmpty(selectedSystem.info))
                {
                    pnlSysInfo.Visible = true;
                    lblSysInfo.Text = Tools.GetFormattedTextFromDB(selectedSystem.info);
                }
                else
                {
                    pnlSysInfo.Visible = false;
                }

                StaticSystemData ssd = bStaticSystem.GetStaticSystem(objectContext, selectedSystem.name, false);
                if (ssd != null)
                {
                    pnlSysApiInfo.Visible = true;

                    lblSysApiKills.Text = string.Format("{0}:{1}, {2}:{3}, {4}:{5}"
                        , GetLocalResourceObject("infoApiShipKills"), ssd.shipKills, GetLocalResourceObject("infoApiPodKills"), ssd.podKills
                        , GetLocalResourceObject("infoApiNpcKills"), ssd.npcKills);

                    lblSysApiJumps.Text = ssd.jumps.ToString();

                    StaticCorporationSystem scs = ssd.StaticCorporationsSystemsData.FirstOrDefault(sc => sc.RCorporation.ID == currCorporation.ID);
                    if (scs != null)
                    {
                        if (scs.dateFirstMet != selectedSystem.dateAdded)
                        {
                            pnlSysFirstDiscovered.Visible = true;
                            lblSysFirstDiscovered.Text = Tools.GetDateTimeInLocalFormat(scs.dateFirstMet);
                        }
                        else
                        {
                            pnlSysFirstDiscovered.Visible = false;
                        }

                        if (scs.datePreviousMet != scs.dateFirstMet)
                        {
                            pnlSysLastDiscovered.Visible = true;
                            lblSysLastDiscovered.Text = Tools.GetDateTimeInLocalFormat(scs.datePreviousMet);
                        }
                        else
                        {
                            pnlSysLastDiscovered.Visible = false;
                        }
                    }
                    else
                    {
                        pnlSysFirstDiscovered.Visible = false;
                        pnlSysLastDiscovered.Visible = false;
                    }
                }
                else
                {
                    pnlSysApiInfo.Visible = false;
                    pnlSysFirstDiscovered.Visible = false;
                    pnlSysLastDiscovered.Visible = false;
                }

                if (selectedSystem.root == false)
                {
                    pnlSysInfoAdded.Visible = true;
                    lblSysDateAdded.Text = Tools.GetDateTimeInLocalFormat(selectedSystem.dateAdded);

                    if (!selectedSystem.UsersReference.IsLoaded)
                    {
                        selectedSystem.UsersReference.Load();
                    }
                    lblSysAddedBy.Text = selectedSystem.Users.name;
                }
                else
                {
                    pnlSysInfoAdded.Visible = false;
                }


                if (selectedSystem.root == false)
                {
                    if (currUser.canEdit == true)
                    {
                        ibDeleteSystem.Visible = true;
                        ibDeleteSystem_ConfirmButtonExtender.Enabled = false;
                    }
                    else
                    {
                        ibDeleteSystem.Visible = false;
                    }
                }
                else
                {
                    ibDeleteSystem.Visible = false;
                }
                
                if (currUser.admin == true || currUser.canEdit == true)
                {
                    ibEditSystem.Visible = true;

                    pnlAddSignature.Visible = true;
                    pnlAddSystem.Visible = true;
                }
                else
                {
                    ibEditSystem.Visible = false;
                }

                
            }
            else
            {
                HidePanels();
            }
        }

        private void FillSystemSignatures()
        {
            phSignatures.Controls.Clear();

            List<Signature> signatures = bSignature.GetSystemSignatures(selectedSystem);

            if (signatures != null && signatures.Count > 0)
            {
                string sigType = string.Empty;

                foreach (Signature signature in signatures)
                {
                    signature.UsersReference.Load();
                    sigType = bSignature.GetSignatureType(signature);

                    Panel panel = new Panel();
                    phSignatures.Controls.Add(panel);

                    if (currUser.admin == true || currUser.canEdit == true)
                    {
                        ImageButton btnEditSignature = new ImageButton();
                        panel.Controls.Add(btnEditSignature);

                        btnEditSignature.ID = string.Format("editSig{0}", signature.ID);
                        btnEditSignature.ImageUrl = "~\\images\\edit.png";
                        btnEditSignature.Height = Unit.Pixel(15);
                        btnEditSignature.CssClass = "imgBtn";
                        btnEditSignature.ToolTip = GetGlobalResourceObject("Resources","edit").ToString();
                        btnEditSignature.Attributes.Add("sigID", signature.ID.ToString());
                        btnEditSignature.Click += new ImageClickEventHandler(btnEditSignature_Click);

                        ImageButton btnRemoveSignature = new ImageButton();
                        panel.Controls.Add(btnRemoveSignature);

                        btnRemoveSignature.ID = string.Format("delSig{0}", signature.ID);
                        btnRemoveSignature.ImageUrl = "~\\images\\remove.png";
                        btnRemoveSignature.Height = Unit.Pixel(15);
                        btnRemoveSignature.CssClass = "imgBtn";
                        btnRemoveSignature.ToolTip = GetGlobalResourceObject("Resources", "delete").ToString();
                        btnRemoveSignature.Attributes.Add("sigID", signature.ID.ToString());
                        btnRemoveSignature.Click += new ImageClickEventHandler(btnRemoveSignature_Click);
                    }
                   
                    Label lblsigName = new Label();
                    panel.Controls.Add(lblsigName);
                    lblsigName.CssClass = "signatures";

                    Label lblsigType = new Label();
                    panel.Controls.Add(lblsigType);
                    //lblsigType.CssClass = "sigType";

                    Label lbladdInfo = new Label();
                    panel.Controls.Add(lbladdInfo);

                    lblsigName.Text = string.Format("{0} ", signature.name);
                    lblsigType.Text = bSignature.GetSignatureType(signature);

                    switch (signature.type)
                    {
                        case 1:
                            lblsigType.CssClass = "sigRadarMagn";
                            break;
                        case 2:
                            lblsigType.CssClass = "sigGravLadar";
                            break;
                        case 3:
                            lblsigType.CssClass = "sigRadarMagn";
                            break;
                        case 4:
                            lblsigType.CssClass = "sigGravLadar";
                            break;
                        case 5:
                            lblsigType.CssClass = "wormwholes";
                            break;
                        default:
                            lblsigType.CssClass = string.Empty;
                            break;
                    }

                    //lbladdInfo.Text = string.Format(", доб. {0} от {1}", signature.dateAdded.ToLocalTime(), signature.Users.name);
                    if (!string.IsNullOrEmpty(signature.info))
                    {
                        Label lblInfo = new Label();
                        panel.Controls.Add(lblInfo);
                        lblInfo.CssClass = "info";

                        lblInfo.Text = string.Format(", {0}", signature.info);
                    }
                }
            }
            else
            {
                Label label = new Label();
                phSignatures.Controls.Add(label);
                label.Text = GetLocalResourceObject("noAddedSignatures").ToString();
            }

        }

        void btnRemoveSignature_Click(object sender, ImageClickEventArgs e)
        {
            CheckSystemAndUser();

            ImageButton remImgBtn = sender as ImageButton;
            if (remImgBtn != null)
            {
                string strSigId = remImgBtn.Attributes["sigID"];
                if (string.IsNullOrEmpty(strSigId))
                {
                    throw new ArgumentException("btnRemoveSignature.Attributes['sigID'] is null or empty");
                }

                long sigId = 0;
                if (long.TryParse(strSigId, out sigId) == false)
                {
                    throw new ArgumentException("Couldnt parse btnRemoveSignature.Attributes['sigID'] to long");
                }

                Signature currSig = bSignature.Get(objectContext, sigId, false);
                if (currSig == null)
                {
                    FillShowSystemDataPanel();
                    return;
                }

                lock (modifyingSignature)
                {
                    bSignature.DeleteSignature(objectContext, currCorporation, currUser, currSig, true);

                    ShowStatusPanel(GetLocalResourceObject("statusSigDeleted").ToString());

                    hfEditSignatureId.Value = null;
                    pnlEditSignature.Visible = false;
                }

                FillShowSystemDataPanel();
            }
            else
            {
                throw new ArgumentException("Couldnt get Image button");
            }
        }

        void btnEditSignature_Click(object sender, ImageClickEventArgs e)
        {
            CheckSystemAndUser();

            ImageButton imgBtn = sender as ImageButton;
            if (imgBtn != null)
            {
                string strSigId = imgBtn.Attributes["sigID"];
                if (string.IsNullOrEmpty(strSigId))
                {
                    throw new ArgumentException("btnEditSignature.Attributes['sigID'] is null or empty");
                }

                hfEditSignatureId.Value = strSigId;
                FillEditSignatureData();
            }
            else
            {
                throw new ArgumentException("Couldnt get Image button");
            }

        }

        private void FillEditSignatureData()
        {
            CheckSystemAndUser();

            pnlEditSystem.Visible = false;
            pnlEditWormwholes.Visible = false;

            string strSigId = hfEditSignatureId.Value;
            if (!string.IsNullOrEmpty(strSigId))
            {
                long sigId = 0;
                if (long.TryParse(strSigId, out sigId) == false)
                {
                    throw new ArgumentException("Couldnt parse btnEditSignature.Attributes['sigID'] to long");
                }

                Signature currSig = bSignature.Get(objectContext, sigId, false);
                if (currSig == null)
                {
                    FillShowSystemDataPanel();
                    pnlEditSignature.Visible = false;
                    return;
                }

                pnlEditSignature.Visible = true;

                lblEditSignatureError.Visible = false;
                lblEditSignatureName.Text = currSig.name;

                tbEditSigName.Text = currSig.name;
                tbEditSignatureInfo.Text = currSig.info;
                ddlEditSignature.SelectedValue = currSig.type.ToString();


            }
            else
            {
                pnlEditSignature.Visible = false;
            }


            
        }

        private void SetSelectedSystemAsEdited()
        {
            hfSystemEdited.Value = selectedSystem.ID.ToString();
        }

        private void FillSystemConnections()
        {
            phConnToOtherSystems.Controls.Clear();

            List<Wormwhole> whFromSystem = new List<Wormwhole>();
            List<Wormwhole> whToSystem = new List<Wormwhole>();

            bSystem.GetSystemConnections(selectedSystem, ref whToSystem, ref whFromSystem);

            if (whFromSystem.Count > 0 || whToSystem.Count > 0)
            {
                if (whToSystem.Count > 0)
                {
                    foreach (Wormwhole worm in whToSystem)
                    {
                        worm.FromSystemsReference.Load();

                        Panel panel = new Panel();
                        phConnToOtherSystems.Controls.Add(panel);

                        if (currUser.admin == true || currUser.canEdit == true)
                        {
                            ImageButton btnEditWormWhole = new ImageButton();
                            panel.Controls.Add(btnEditWormWhole);

                            btnEditWormWhole.ID = string.Format("editWhT{0}", worm.ID);
                            btnEditWormWhole.ImageUrl = "~\\images\\edit.png";
                            btnEditWormWhole.Height = Unit.Pixel(15);
                            btnEditWormWhole.CssClass = "imgBtn";
                            btnEditWormWhole.ToolTip = GetGlobalResourceObject("Resources","edit").ToString();
                            btnEditWormWhole.Attributes.Add("whID", worm.ID.ToString());
                            btnEditWormWhole.Click += new ImageClickEventHandler(btnEditWormWhole_Click);
                        }

                        
                        if (!string.IsNullOrEmpty(worm.toSysWormwholeID))
                        {
                            Label lblInfo = new Label();
                            panel.Controls.Add(lblInfo);

                            Label lblWhID = new Label();
                            panel.Controls.Add(lblWhID);
                            lblWhID.CssClass = "wormwholes pointer";

                            Label lblInfo2 = new Label();
                            panel.Controls.Add(lblInfo2);

                            HyperLink hlSys = new HyperLink();
                            panel.Controls.Add(hlSys);
                            hlSys.CssClass = "systems";
                            hlSys.Target = "_blank";
                            hlSys.NavigateUrl = Tools.GetSystemNameRedirrectUrl(worm.FromSystems.name);

                            //lblInfo.Text = string.Format("през {0} към {1}", worm.toSysWormwholeID, worm.FromSystems.name);
                            lblInfo.Text = GetLocalResourceObject("systemInfoFromWh").ToString() + " ";
                            lblWhID.Text = worm.toSysWormwholeID;
                            lblInfo2.Text = string.Format(" {0} ", GetLocalResourceObject("systemInfoToSys"));
                            hlSys.Text = worm.FromSystems.name;

                            AssignEventsToWhLbl(worm.toSysWormwholeID, lblWhID);
                        }
                        else
                        {
                            Label lblInfo = new Label();
                            panel.Controls.Add(lblInfo);

                            HyperLink hlSys = new HyperLink();
                            panel.Controls.Add(hlSys);
                            hlSys.CssClass = "systems";
                            hlSys.Target = "_blank";
                            hlSys.NavigateUrl = Tools.GetSystemNameRedirrectUrl(worm.FromSystems.name);

                            //lblInfo.Text = string.Format("към {0}", worm.FromSystems.name);
                            lblInfo.Text = GetLocalResourceObject("systemInfoConnectedWith").ToString() + " ";
                            hlSys.Text = worm.FromSystems.name;
                        }
                    }
                }

                if (whFromSystem.Count > 0)
                {
                    foreach (Wormwhole worm in whFromSystem)
                    {
                        worm.ToSystemsReference.Load();

                        Panel panel = new Panel();
                        phConnToOtherSystems.Controls.Add(panel);

                        if (currUser.admin == true || currUser.canEdit == true)
                        {
                            ImageButton btnEditWormWhole = new ImageButton();
                            panel.Controls.Add(btnEditWormWhole);

                            btnEditWormWhole.ID = string.Format("editWhF{0}", worm.ID);
                            btnEditWormWhole.ImageUrl = "~\\images\\edit.png";
                            btnEditWormWhole.Height = Unit.Pixel(15);
                            btnEditWormWhole.CssClass = "imgBtn";
                            btnEditWormWhole.ToolTip = GetGlobalResourceObject("Resources", "edit").ToString();
                            btnEditWormWhole.Attributes.Add("whID", worm.ID.ToString());
                            btnEditWormWhole.Click += new ImageClickEventHandler(btnEditWormWhole_Click);
                        }

                

                        if (!string.IsNullOrEmpty(worm.fromSysWormwholeID))
                        {
                            Label lblInfo = new Label();
                            panel.Controls.Add(lblInfo);

                            Label lblWhID = new Label();
                            panel.Controls.Add(lblWhID);
                            lblWhID.CssClass = "wormwholes pointer";

                            Label lblInfo2 = new Label();
                            panel.Controls.Add(lblInfo2);

                            HyperLink hlSys = new HyperLink();
                            panel.Controls.Add(hlSys);
                            hlSys.CssClass = "systems";
                            hlSys.Target = "_blank";
                            hlSys.NavigateUrl = Tools.GetSystemNameRedirrectUrl(worm.ToSystems.name);

                            //lblInfo.Text = string.Format("през {0} към {1}", worm.fromSysWormwholeID, worm.ToSystems.name);
                            lblInfo.Text = GetLocalResourceObject("systemInfoFromWh").ToString() + " ";
                            lblWhID.Text = worm.fromSysWormwholeID;
                            lblInfo2.Text = string.Format(" {0} ", GetLocalResourceObject("systemInfoToSys"));
                            hlSys.Text = worm.ToSystems.name;

                            AssignEventsToWhLbl(worm.fromSysWormwholeID, lblWhID);
                        }
                        else
                        {
                            Label lblInfo = new Label();
                            panel.Controls.Add(lblInfo);

                            HyperLink hlSys = new HyperLink();
                            panel.Controls.Add(hlSys);
                            hlSys.CssClass = "systems";
                            hlSys.Target = "_blank";
                            hlSys.NavigateUrl = Tools.GetSystemNameRedirrectUrl(worm.ToSystems.name);

                            //lblInfo.Text = string.Format("към {0}", worm.ToSystems.name);
                            lblInfo.Text = GetLocalResourceObject("systemInfoConnectedWith").ToString() + " ";
                            hlSys.Text = worm.ToSystems.name;
                            
                        }
                    }
                }


            }
            else
            {
                Label label = new Label();
                phConnToOtherSystems.Controls.Add(label);
                label.Text = GetLocalResourceObject("systemInfoNoConnections").ToString();
            }

        }

        void btnEditWormWhole_Click(object sender, ImageClickEventArgs e)
        {
            pnlEditSystem.Visible = false;
            pnlEditSignature.Visible = false;

            ImageButton editWHBtn = sender as ImageButton;
            if (editWHBtn != null)
            {
                string strSigId = editWHBtn.Attributes["whID"];
                if (string.IsNullOrEmpty(strSigId))
                {
                    throw new ArgumentException("btnEditWormWhole.Attributes['whID'] is null or empty");
                }

                hfEditWhId.Value = strSigId;
                FillEditWhData();
            }
            else
            {
                throw new ArgumentException("Couldnt get btnEditWormWhole");
            }
        }

        private void FillEditWhData()
        {
            CheckSystemAndUser();

            pnlEditSystem.Visible = false;
            pnlEditSignature.Visible = false;

            string strWhId = hfEditWhId.Value;
            if (!string.IsNullOrEmpty(strWhId))
            {
                long whId = 0;
                if (long.TryParse(strWhId, out whId) == false)
                {
                    throw new ArgumentException("Couldnt parse btnEditWormWhole.Attributes['whID'] to long");
                }

                Wormwhole currWH = bWormwhole.GetWormwhole(objectContext, whId, false);
                if (currWH == null)
                {
                    FillShowSystemDataPanel();
                    pnlEditWormwholes.Visible = false;
                    hfEditWhId.Value = null;
                    return;
                }

                currWH.FromSystemsReference.Load();
                currWH.ToSystemsReference.Load();

                if (currWH.FromSystems.ID != selectedSystem.ID && currWH.ToSystems.ID != selectedSystem.ID)
                {
                    FillShowSystemDataPanel();
                    pnlEditWormwholes.Visible = false;
                    return;
                }

                pnlEditWormwholes.Visible = true;

                lblEditWhIdError.Visible = false;

                if (currWH.FromSystems.ID == selectedSystem.ID)
                {
                    lblEditWhId.Visible = true;
                    lblEditWhId.Text = string.Format(": {0} ", currWH.fromSysWormwholeID);

                    lblEditWhFromSys.Text = currWH.FromSystems.name;
                    lblEditWhToSys.Text = currWH.ToSystems.name;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currWH.toSysWormwholeID))
                    {
                        lblEditWhId.Visible = true;
                        lblEditWhId.Text = string.Format(": {0} ", currWH.toSysWormwholeID);
                    }
                    else
                    {
                        lblEditWhId.Visible = false;
                    }

                    lblEditWhFromSys.Text = currWH.ToSystems.name;
                    lblEditWhToSys.Text = currWH.FromSystems.name;
                }

            }
            else
            {
                pnlEditWormwholes.Visible = false;
            }
        }

        protected void btnAddSystem_Click(object sender, EventArgs e)
        {
            CheckSystemAndUser();

            bool errorOccured = false;
            string error = string.Empty;

            if (!string.IsNullOrEmpty(tbNewSysName.Text))
            {
                if (bSystem.CheckIfSystemIsAlreadyAdded(objectContext, currCorporation, tbNewSysName.Text) == false)
                {
                    lock (addingSystem)
                    {

                        int type = 0;
                        if (int.TryParse(ddlNewSysClass.SelectedValue, out type) == false)
                        {
                            throw new FormatException("Couldnt parse ddlNewSysClass.SelectedValue to int");
                        }

                        int effect = 0;
                        if (int.TryParse(ddlNewSysEffect.SelectedValue, out effect) == false)
                        {
                            throw new FormatException("Couldnt parse ddlNewSysEffect.SelectedValue to int");
                        }

                        bSystem.Add(objectContext, currCorporation, bWormwhole, selectedSystem, currUser, tbNewSysName.Text, type, effect
                            , tbFromSysWhID.Text, tbToSysWhID.Text, tbNewSysInfo.Text, tbNewSysOccupied.Text);
                    }

                    ShowStatusPanel(GetLocalResourceObject("statusSystemAdded").ToString());

                    FillSystemsForJs();
                    FillAddSystemPanel();
                    FillShowSystemDataPanel();
                }
                else
                {
                    errorOccured = true;
                    error = GetLocalResourceObject("errSystemNameTaken").ToString();
                }
            }
            else
            {
                errorOccured = true;
                error = GetLocalResourceObject("errEnterSystemName").ToString();
            }

            if (errorOccured == true)
            {
                lblAddSystemError.Visible = true;
                lblAddSystemError.Text = error;
            }

        }

        private void HidePanels()
        {
            pnlAddSignature.Visible = false;
            pnlAddSystem.Visible = false;
            pnlSystemInfo.Visible = false;

            hfEditSignatureId.Value = null;
            pnlEditSignature.Visible = false;

            hfEditWhId.Value = null;
            pnlEditWormwholes.Visible = false;

            pnlEditSystem.Visible = false;
        }

        protected void btnAddSignature_Click(object sender, EventArgs e)
        {
            CheckSystemAndUser();

            bool errorOccured = false;
            string error = string.Empty;

            if (!string.IsNullOrEmpty(tbNewSigName.Text))
            {
                //if (ddlNewSigType.SelectedIndex > 0)
                //{
                    lock (addingSignature)
                    {

                        if (bSignature.CheckIfThereIsAlreadySignatureForSys(objectContext, selectedSystem, tbNewSigName.Text) == false)
                        {

                            int sigType = 0;
                            if (int.TryParse(ddlNewSigType.SelectedValue, out sigType) == false)
                            {
                                throw new InvalidOperationException("Couldnt parse ddlNewSigType.SelectedValue to int.");
                            }

                            bSignature.Add(objectContext, currCorporation, selectedSystem, currUser, tbNewSigName.Text, sigType, tbNewSignatureInfo.Text);

                            FillAddSignaturePanel();
                            FillShowSystemDataPanel();

                            ShowStatusPanel(GetLocalResourceObject("statusSignatureAdded").ToString());

                        }
                        else
                        {
                            errorOccured = true;
                            error = GetLocalResourceObject("errSignatureNameTaken").ToString();
                        }

                    }
                //}
                //else
                //{
                //    errorOccured = true;
                //    error = "Избери тип на сигнатурата!";
                //}
            }
            else
            {
                errorOccured = true;
                error = GetLocalResourceObject("errEnterSignatureName").ToString();
            }

            if (errorOccured == true)
            {
                lblAddSignatureError.Visible = true;
                lblAddSignatureError.Text = error;
            }
        }

        protected void btnEditSignature_Click1(object sender, EventArgs e)
        {
            CheckSystemAndUser();

            if (currUser.admin == false && currUser.canEdit == false)
            {
                return;
            }

            string strSigId = hfEditSignatureId.Value;
            if (!string.IsNullOrEmpty(strSigId))
            {
                long sigId = 0;
                if (long.TryParse(strSigId, out sigId) == false)
                {
                    throw new ArgumentException("Couldnt parse hfEditSignatureId.Value to long");
                }

                Signature currSig = bSignature.Get(objectContext, sigId, false);
                if (currSig == null)
                {
                    FillShowSystemDataPanel();
                    pnlEditSignature.Visible = false;
                    return;
                }

                string error = string.Empty;
                bool errorOccured = false;
                bool sigUpdated = false;

                if (string.IsNullOrEmpty(tbEditSigName.Text))
                {
                    error = GetLocalResourceObject("errEnterSignatureName").ToString();
                    errorOccured = true;
                }
                else if (tbEditSigName.Text != currSig.name)
                {
                    if (bSignature.CheckIfThereIsAlreadySignatureForSys(objectContext, selectedSystem, tbEditSigName.Text) == true)
                    {
                        error = GetLocalResourceObject("errSignatureNameTaken").ToString();
                        errorOccured = true;
                    }
                    else
                    {
                        sigUpdated = true;
                    }
                }

                int type = 0;
                if(int.TryParse(ddlEditSignature.SelectedValue, out type) == false)
                {
                    throw new ArgumentException("Couldnt parse ddlEditSignature.SelectedValue to int");
                }

                if (currSig.type.ToString() != ddlEditSignature.SelectedValue)
                {
                    sigUpdated = true;
                }

                if (currSig.info != tbEditSignatureInfo.Text)
                {
                    sigUpdated = true;
                }

                if (errorOccured == false)
                {
                    if (sigUpdated == true)
                    {
                        lock (modifyingSignature)
                        {
                            bSignature.UpdateSignature(objectContext, currCorporation, currUser, currSig, tbEditSigName.Text, type, tbEditSignatureInfo.Text);
                            pnlEditSignature.Visible = false;
                            hfEditSignatureId.Value = null;
                            ShowStatusPanel(GetLocalResourceObject("statusSignatureUpdated").ToString());
                            FillShowSystemDataPanel();
                        }
                    }
                    else
                    {
                        lblEditSignatureError.Visible = true;
                        lblEditSignatureError.Text = GetGlobalResourceObject("Resources","errNoChanges").ToString();
                    } 
                }
                else
                {
                    lblEditSignatureError.Visible = true;
                    lblEditSignatureError.Text = error;
                }

            }
            else
            {
                pnlEditSignature.Visible = false;
            }


        }

        protected void bntCancelEditSignature_Click(object sender, EventArgs e)
        {
            hfEditSignatureId.Value = null;
            pnlEditSignature.Visible = false;
        }

        protected void btnChangeSysName_Click(object sender, EventArgs e)
        {
            CheckSystemAndUser();

            if (string.IsNullOrEmpty(tbEditSysName.Text))
            {
                lblEditSystemError.Visible = true;
                lblEditSystemError.Text = GetLocalResourceObject("errEnterNewSystemName").ToString();
            }
            else if(selectedSystem.name == tbEditSysName.Text)
            {
                lblEditSystemError.Visible = true;
                lblEditSystemError.Text = GetLocalResourceObject("errEnterNewSystemName").ToString();
            }
            else
            {

                lock (modifyingSystem)
                {
                    if (bSystem.CheckIfSystemIsAlreadyAdded(objectContext, currCorporation, tbEditSysName.Text) == false)
                    {
                        lblEditSystemError.Visible = false;
                        bSystem.ChangeSystemName(objectContext, currCorporation, selectedSystem, currUser, tbEditSysName.Text);

                        FillSystemsForJs();
                        FillShowSystemDataPanel();
                        FillEditSystemPanel();

                        FillAddSystemPanel();
                        FillAddSignaturePanel();

                        ShowStatusPanel(GetLocalResourceObject("statusSystemNameChanged").ToString());
                    }
                    else
                    {
                        lblEditSystemError.Visible = true;
                        lblEditSystemError.Text = GetLocalResourceObject("errSystemNameTaken").ToString();
                    }
                }
            }

        }

        protected void btnChangeSysOccupied_Click(object sender, EventArgs e)
        {
            CheckSystemAndUser();

            if (tbEditSysOccupied.Text == selectedSystem.occupied)
            {
                lblEditSystemError.Visible = true;
                lblEditSystemError.Text = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
            }
            else
            {
                lblEditSystemError.Visible = false;

                bSystem.ChangeSystemOccupied(objectContext, currCorporation, selectedSystem, currUser, tbEditSysOccupied.Text);

                FillShowSystemDataPanel();
                FillEditSystemPanel();

                ShowStatusPanel(GetLocalResourceObject("statusSystemOccupationUpdated").ToString());
            }

        }

        protected void btnChangeSysInfo_Click(object sender, EventArgs e)
        {
            CheckSystemAndUser();

            if (tbEditSysInfo.Text == selectedSystem.info)
            {
                lblEditSystemError.Visible = true;
                lblEditSystemError.Text = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
            }
            else
            {
                lblEditSystemError.Visible = false;

                bSystem.ChangeSystemInfo(objectContext, currCorporation, selectedSystem, currUser, tbEditSysInfo.Text);

                FillShowSystemDataPanel();
                FillEditSystemPanel();

                ShowStatusPanel(GetLocalResourceObject("statusSystemInfoUpdated").ToString());
            }

        }

        protected void ibEditSystem_Click(object sender, ImageClickEventArgs e)
        {
            FillEditSystemPanel();
        }

        private void FillEditSystemPanel()
        {
            CheckSystemAndUser();

            pnlEditSignature.Visible = false;
            pnlEditWormwholes.Visible = false;

            pnlEditSystem.Visible = true;
            lblEditSystemError.Visible = false;

            lblEditSystemName.Text = selectedSystem.name;

            if (selectedSystem.root == true)
            {
                if (currUser.admin == true)
                {
                    tbEditSysName.Enabled = true;
                    btnChangeSysName.Enabled = true;

                    ddlEditSysClass.Enabled = true;
                    ddlEditSysEffect.Enabled = true;
                }
                else
                {
                    tbEditSysName.Enabled = false;
                    btnChangeSysName.Enabled = false;

                    ddlEditSysClass.Enabled = false;
                    ddlEditSysEffect.Enabled = false;
                }
                
            }
            else
            {
                tbEditSysName.Enabled = true;
                btnChangeSysName.Enabled = true;

                ddlEditSysClass.Enabled = true;
                ddlEditSysEffect.Enabled = true;
            }

            if (selectedSystem.sysClass != null)
            {
                ddlEditSysClass.SelectedValue = selectedSystem.sysClass.Value.ToString();
            }
            else
            {
                ddlEditSysClass.SelectedIndex = 0;
            }

            if (selectedSystem.sysEffect != null)
            {
                ddlEditSysEffect.SelectedValue = selectedSystem.sysEffect.Value.ToString();
            }
            else
            {
                ddlEditSysEffect.SelectedIndex = 0;
            }

            tbEditSysName.Text = selectedSystem.name;
            tbEditSysInfo.Text = selectedSystem.info;
            tbEditSysOccupied.Text = selectedSystem.occupied;
        }

        protected void btnCancelEditSystem_Click(object sender, EventArgs e)
        {
            pnlEditSystem.Visible = false;
        }

        protected void ibDeleteSystem_Click(object sender, ImageClickEventArgs e)
        {
            CheckSystemAndUser();

            lock (modifyingSystem)
            {
                bSystem.DeleteSystem(objectContext, currCorporation, selectedSystem, currUser);

                ShowStatusPanel(GetLocalResourceObject("statusSystemDeleted").ToString());

                selectedSystem = null;
                HidePanels();

                FillSystemsForJs();
            }
        }

        protected void btnCancelEditWh_Click(object sender, EventArgs e)
        {
            hfEditWhId.Value = null;
            pnlEditWormwholes.Visible = false;
        }

        protected void btnChangeWhID_Click(object sender, EventArgs e)
        {
            CheckSystemAndUser();

            string strWhId = hfEditWhId.Value;
            if (!string.IsNullOrEmpty(strWhId))
            {
                long whId = 0;
                if (long.TryParse(strWhId, out whId) == false)
                {
                    throw new ArgumentException("Couldnt parse hfEditWhId.Value to long");
                }

                Wormwhole currWH = bWormwhole.GetWormwhole(objectContext, whId, false);
                if (currWH == null)
                {
                    FillShowSystemDataPanel();
                    pnlEditWormwholes.Visible = false;
                    hfEditWhId.Value = null;
                    return;
                }

                currWH.FromSystemsReference.Load();
                currWH.ToSystemsReference.Load();

                if (currWH.FromSystems.ID != selectedSystem.ID && currWH.ToSystems.ID != selectedSystem.ID)
                {
                    FillShowSystemDataPanel();
                    pnlEditWormwholes.Visible = false;
                    hfEditWhId.Value = null;
                    return;
                }

                lock (modifyingWh)
                {
                    bool errorOccured = false;
                    string error = string.Empty;

                    if (currWH.FromSystems.ID == selectedSystem.ID)
                    {
                        if (tbEditWhId.Text == currWH.fromSysWormwholeID)
                        {
                            errorOccured = true;
                            error = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                        }
                        //else if (string.IsNullOrEmpty(tbEditWhId.Text))
                        //{
                        //    errorOccured = true;
                        //    error = "Въведи ново ID на дупката!";
                        //}
                    }
                    else
                    {
                        if (tbEditWhId.Text == currWH.toSysWormwholeID)
                        {
                            errorOccured = true;
                            error = GetGlobalResourceObject("Resources", "errNoChanges").ToString();
                        }
                    }

                    if (errorOccured == false)
                    {
                        lblEditWhIdError.Visible = false;

                        bWormwhole.UpdateWormwhole(objectContext, currCorporation, currUser, currWH, selectedSystem, tbEditWhId.Text);

                        FillSystemsForJs();
                        FillShowSystemDataPanel();

                        tbEditWhId.Text = string.Empty;
                        hfEditWhId.Value = null;
                        pnlEditWormwholes.Visible = false;

                        ShowStatusPanel(GetLocalResourceObject("statusWhIdUpdated").ToString());
                    }
                    else
                    {
                        lblEditWhIdError.Visible = true;
                        lblEditWhIdError.Text = error;
                    }
                }

            }
            else
            {
                pnlEditWormwholes.Visible = false;
            }
        }


        [WebMethod]
        public static string WmUpdatesSincePageLoad(string dateLoaded)
        {
            return WebMethods.WmUpdatesSincePageLoad(dateLoaded);
        }

        protected void ddlEditSysClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSystemAndUser();

            if (string.IsNullOrEmpty(ddlEditSysClass.SelectedValue))
            {
                throw new ArgumentNullException("ddlEditSysClass.SelectedValue");
            }

            int selectedType = 0;
            if (int.TryParse(ddlEditSysClass.SelectedValue, out selectedType) == false)
            {
                throw new FormatException("Couldnt parse ddlEditSysClass.SelectedValue to int");
            }

            if ((selectedType == 0 && selectedSystem.sysClass == null) || (selectedType == selectedSystem.sysClass))
            {
                FillEditSystemPanel();
                return;
            }

            lock (modifyingSystem)
            {
                bSystem.ChangeSystemClass(objectContext, currCorporation, selectedSystem, currUser, selectedType);
            }

            lblEditSystemError.Visible = false;

            FillSystemsForJs();
            FillShowSystemDataPanel();
            FillEditSystemPanel();

            ShowStatusPanel(GetLocalResourceObject("statusSystemClassUpdated").ToString());

        }

        protected void ddlEditSysEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSystemAndUser();

            if (string.IsNullOrEmpty(ddlEditSysEffect.SelectedValue))
            {
                throw new ArgumentNullException("ddlEditSysEffect.SelectedValue");
            }

            int selectedType = 0;
            if (int.TryParse(ddlEditSysEffect.SelectedValue, out selectedType) == false)
            {
                throw new FormatException("Couldnt parse ddlEditSysEffect.SelectedValue to int");
            }

            if ((selectedType == 0 && selectedSystem.sysEffect == null) || (selectedType == selectedSystem.sysEffect))
            {
                FillEditSystemPanel();
                return;
            }

            lock (modifyingSystem)
            {
                bSystem.ChangeSystemEffect(objectContext, currCorporation, selectedSystem, currUser, selectedType);
            }

            lblEditSystemError.Visible = false;

            FillShowSystemDataPanel();
            FillEditSystemPanel();

            ShowStatusPanel(GetLocalResourceObject("statusSystemEffectUpdated").ToString());
        }

        private void AssignEventsToWhLbl(string wormwholeId, Label eventToLabel)
        {
            if (string.IsNullOrEmpty(wormwholeId))
            {
                throw new ArgumentException("wormwholeId is null or empty");
            }
            if (eventToLabel == null)
            {
                throw new ArgumentNullException("eventToLabel");
            }

            //string whInfoPanelId = string.Format("wh{0}info", wormwholeId);

            eventToLabel.Attributes.Add("onmouseover", string.Format("ShowWhInfo('{0}','{1}')", wormwholeId, eventToLabel.ClientID));
            eventToLabel.Attributes.Add("onmouseout", string.Format("HideWhInfo('{0}')", wormwholeId));


           
        }


        
     

    }
}