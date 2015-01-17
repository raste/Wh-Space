<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.Master" CodeBehind="Main.aspx.cs" Inherits="WhSpace.Main" Theme="Skins"%>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <%
     Response.Write("<script type=\"text/javascript\">");
     Response.Write(Environment.NewLine);
     Response.Write("var stellarSystems = ");
     Response.Write(StellarSystemTreeJson);
     Response.Write(";");
     Response.Write(Environment.NewLine);
     Response.Write("</script>");
     Response.Write(Environment.NewLine);
%>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:HiddenField ID="hfSelectedSys" runat="server" />
    <asp:HiddenField ID="hfSystemEdited" runat="server" />
    <div class="" style="margin-top:10px;">
    

       
   


      <div id="systemsHolder" style="text-align:left;"></div> 
          

        <asp:Panel ID="pnlStatus" CssClass="statusPnl" style="margin-bottom:7px;" 
            runat="server" Direction="NotSet" Visible="False">
        
         <asp:Label ID="lblStaticStatus" style="color:Black;" runat="server" Text="Статус :"></asp:Label>
         <asp:Label ID="lblStatus" runat="server"></asp:Label>

        </asp:Panel>


        <div class="clearfix2" >
        
        <div class="left">
        
          <asp:Panel ID="pnlSystemInfo" runat="server" Visible="False" 
                CssClass="panel trans" style="padding-left:7px; padding-right:7px; width:480px;">
          <div style="text-align:center;  margin-bottom:10px;" class="headerText">
              <asp:ImageButton ID="ibEditSystem" runat="server" ImageUrl="~/images/edit.png" 
                  onclick="ibEditSystem_Click" Height="15px" ToolTip="промени" />
              <asp:ImageButton ID="ibDeleteSystem" runat="server" Height="15px" 
                  ImageUrl="~/images/remove.png" onclick="ibDeleteSystem_Click" 
                  ToolTip="изтрий" />
              <cc1:ConfirmButtonExtender ID="ibDeleteSystem_ConfirmButtonExtender" 
                  runat="server" ConfirmText="Сигурен ли си, че искаш да изтриеш системата?" 
                  TargetControlID="ibDeleteSystem">
              </cc1:ConfirmButtonExtender>
              <asp:Label ID="lblStaticInfoForSys" runat="server" Text="Информация за"></asp:Label>
              <asp:HyperLink ID="hlSysName" runat="server" Target="_blank">System</asp:HyperLink>
              &nbsp; &nbsp;<asp:Panel ID="pnlSysClassAndEffect" runat="server">
              <asp:Label ID="lblSysClass" runat="server" style="font-size:medium;"></asp:Label>
              <asp:Label ID="lblSysEffect" runat="server" style="margin-left:10px; font-size:medium;"></asp:Label>
              </asp:Panel>

            </div>
              


              <div class="clearfix2">
              
              <div style="float:left; width:110px; text-align:right;">
                  <asp:Label ID="lblStaticSysConnectedWith" runat="server" Text="Свързана :"></asp:Label>
              </div>
              <div style="margin-left:115px;">
              <asp:PlaceHolder ID="phConnToOtherSystems" runat="server"></asp:PlaceHolder>
              
              </div>
              </div>

              <br />

              <div class="clearfix2">
              
              <div style="float:left; width:110px; text-align:right;">
                  <asp:Label ID="lblStaticSysSignatures" runat="server" Text="Сигнатури :"></asp:Label>
              </div>
              <div style="margin-left:115px;">
              <asp:PlaceHolder ID="phSignatures" runat="server"></asp:PlaceHolder>
              
              </div>
              </div>
            
              <asp:Panel ID="pnlSysInfoOccupied" runat="server" Visible="False" CssClass="clearfix2">
                  <br />
                  <div style="float:left; width:110px; text-align:right;">
                      <asp:Label ID="lblStaticSysOccupied" runat="server" Text="Окупирана :"></asp:Label>
                  </div>
                  <div style="margin-left:115px;">
                  <asp:Label ID="lblSysOccupied" runat="server" Text="окупирана" 
                      CssClass="occupation"></asp:Label>
                  </div>
              </asp:Panel>
              <asp:Panel ID="pnlSysInfo" runat="server" Visible="False" CssClass="clearfix2">
                  <br />
                  <div style="float:left; width:110px; text-align:right;">
                      <asp:Label ID="lblStaticSysInfo" runat="server" Text="Доп. инфо :"></asp:Label>
                  </div>
                  <div style="margin-left:115px;">
                  <asp:Label ID="lblSysInfo" runat="server" Text="друго инфо" CssClass="info"></asp:Label>
                  </div>
              </asp:Panel>
              <asp:Panel ID="pnlSysInfoAdded" runat="server" Visible="False">
                  <br />
                  <div style="float:left; width:110px; text-align:right;">
                      <asp:Label ID="lblStaticSysAdded" runat="server" Text="Добавена :"></asp:Label>
                  </div>
                 <div style="margin-left:115px;">
                  <asp:Label ID="lblSysDateAdded" runat="server" Text="Дата" CssClass="dates"></asp:Label>
                  &nbsp;<asp:Label ID="lblStaticSysAddedBy" runat="server" Text="от"></asp:Label>
                  <asp:Label ID="lblSysAddedBy" runat="server" Text="Label" CssClass="users"></asp:Label>
                 </div>
                  
              </asp:Panel>
              <asp:Panel ID="pnlSysApiInfo" runat="server"  Visible="False">
              <br />
              <div style="float:left; width:110px; text-align:right;">
              <asp:Label ID="lblStaticSysApiKills" runat="server" Text="Убийства (1 час) :"></asp:Label>
              <br />
              <asp:Label ID="lblStaticSysApiJumps" runat="server" Text="Скокове (1 час) :"></asp:Label>
              </div>
              <div style="margin-left:115px;">
              <asp:Label ID="lblSysApiKills" runat="server" Text="Kills"></asp:Label>
              <br />
              <asp:Label ID="lblSysApiJumps" runat="server" Text="Kills"></asp:Label>
               </div>

              </asp:Panel>

              <asp:Panel ID="pnlSysFirstDiscovered" runat="server"  Visible="False">
              <br />
              <div style="float:left; width:130px; text-align:right;">
              <asp:Label ID="lblStaticSysFirstDiscovered" runat="server" Text="Първо-открита :"></asp:Label>
              </div>
              <div style="margin-left:135px;">
              <asp:Label ID="lblSysFirstDiscovered" runat="server" Text="Date" CssClass="info"></asp:Label>
              </div>
              </asp:Panel>

              <asp:Panel ID="pnlSysLastDiscovered" runat="server"  Visible="False">
              <div style="float:left; width:130px; text-align:right;">
              <asp:Label ID="lblStaticSysLastDiscovered" runat="server" Text="Последно-открита :"></asp:Label>
              </div>
              <div style="margin-left:135px;">
              <asp:Label ID="lblSysLastDiscovered" runat="server" Text="Date" CssClass="info"></asp:Label>
              </div>
              </asp:Panel>


              
        </asp:Panel>
          
          


        
        </div>
        
        <div class="center">
        
        
        <asp:Panel ID="pnlEditWormwholes" runat="server" DefaultButton="btnChangeWhID" 
            Visible="False" Width="500px" CssClass="panel" style="margin-bottom:8px;">
            <div style="text-align:center;  margin-bottom:10px;" class="headerText">
                <asp:Label ID="lblStaticEditWormhole" runat="server" Text="Промяна на дупка"></asp:Label>
            <asp:Label ID="lblEditWhId" runat="server" Text="Label" CssClass="wormwholes"></asp:Label>
                <asp:Label ID="lblStaticEditWormholeFrom" runat="server" Text="от"></asp:Label>
            
            <asp:Label ID="lblEditWhFromSys" runat="server" Text="Label" CssClass="systems"></asp:Label>
            &nbsp;<asp:Label ID="lblStaticEditWormholeTo" runat="server" Text="към"></asp:Label>
            <asp:Label ID="lblEditWhToSys" runat="server" Text="Label" CssClass="systems"></asp:Label>
            </div>
            
            <div style="padding-left:30px;">
                <asp:Label ID="lblStaticEditWormholeNewId" runat="server" Text="Ново ID :"></asp:Label>
            <asp:TextBox ID="tbEditWhId" runat="server"></asp:TextBox>
            &nbsp;<asp:Button ID="btnChangeWhID" runat="server" onclick="btnChangeWhID_Click" 
                Text="Промяна" SkinID="editBtn" />
            &nbsp;<asp:Button ID="btnCancelEditWh" runat="server" onclick="btnCancelEditWh_Click" 
                Text="Отказ" SkinID="cancelBtn" />
            <br />
            <asp:Label ID="lblEditWhIdError" runat="server" Visible="False" CssClass="errors"></asp:Label>
            <asp:HiddenField ID="hfEditWhId" runat="server" />
            </div>
        </asp:Panel>
           



        <asp:Panel ID="pnlEditSignature" runat="server" Width="500px" Visible="False" 
            DefaultButton="btnEditSignature" CssClass="panel" style="margin-bottom:8px;">
            <div style="text-align:center;  margin-bottom:10px;" class="headerText">
                <asp:Label ID="lblStaticEditSignature" runat="server" Text="Промяна на сигнатура :"></asp:Label>
            <asp:Label ID="lblEditSignatureName" runat="server" Text="Label" 
                    CssClass="signatures"></asp:Label>
            </div>
            
            
            <div class="clearfix2">
                <div style="float:left; width:230px; ">

                <div class="clearfix2">
                <div style="float:left; width:70px; text-align:right;">

                <asp:Label ID="lblStaticEditSignatureNewName" runat="server" Text="Име :"></asp:Label>
                <div style="margin:7px 0px 3px 0px;">     
                <asp:Label ID="lblStaticEditSignatureNewType" runat="server" Text="Тип :"></asp:Label>
                </div>

                </div>
                <div style="margin-left:75px">

                <asp:TextBox ID="tbEditSigName" runat="server" SkinID="required" Width="120px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                ControlToValidate="tbEditSigName" ErrorMessage="*" ValidationGroup="vgEditSig" 
                        CssClass="errors"></asp:RequiredFieldValidator>

            <div style="margin:3px 0px 3px 0px;">      
            <asp:DropDownList ID="ddlEditSignature" runat="server" CssClass="sigType" Width="120px">
                <asp:ListItem Value="6">не е сканирана</asp:ListItem>
                <asp:ListItem Value="5">дупка</asp:ListItem>
                <asp:ListItem Value="1">magnetometric</asp:ListItem>
                <asp:ListItem Value="2">gravimetric</asp:ListItem>
                <asp:ListItem Value="3">radar</asp:ListItem>
                <asp:ListItem Value="4">ladar</asp:ListItem>
            </asp:DropDownList>
                   </div>

                </div>
                </div>
                    
            
            
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnEditSignature" runat="server" 
                        onclick="btnEditSignature_Click1" Text="Промени" 
                        ValidationGroup="vgEditSig" SkinID="editBtn" />
                    &nbsp;<asp:Button ID="bntCancelEditSignature" runat="server" 
                        onclick="bntCancelEditSignature_Click" Text="Отказ" SkinID="cancelBtn" />
            <br />

                 </div>
                <div style="margin-left:235px;">



                    <asp:TextBox ID="tbEditSignatureInfo" runat="server" Rows="4" 
                        TextMode="MultiLine" Width="250px"></asp:TextBox>



                    <cc1:TextBoxWatermarkExtender ID="tbEditSignatureInfo_TextBoxWatermarkExtender" 
                        runat="server" TargetControlID="tbEditSignatureInfo" 
                        WatermarkText="Допълнителна информация"></cc1:TextBoxWatermarkExtender>



                </div>
            </div>

            &nbsp;&nbsp;&nbsp;&nbsp;

            <asp:Label ID="lblEditSignatureError" runat="server" Visible="False" 
                CssClass="errors"></asp:Label>
            <asp:HiddenField ID="hfEditSignatureId" runat="server" />
        </asp:Panel>

        




        <asp:Panel ID="pnlEditSystem" runat="server" Visible="False" Width="500px" 
            CssClass="panel" style="margin-bottom:8px;">
                <div style="text-align:center;  margin-bottom:10px;" class="headerText">
                    <asp:Label ID="lblStaticEditSystem" runat="server" Text="Промяна система :"></asp:Label>
                <asp:Label ID="lblEditSystemName" runat="server" Text="Name" CssClass="systems"></asp:Label>
                </div>
                
               

              

              <div class="clearfix2">
                <div style="float:left; width:130px; text-align:right; padding-top:0px;">
                 <asp:Label ID="lblStaticEditSysNewName" runat="server" Text="Ново име :"></asp:Label>
                 <div style="margin:5px 0px;"><asp:Label ID="lblStaticEditSysNewClass" runat="server" Text="Смяна клас :"></asp:Label></div>
                    <asp:Label ID="lblStaticEditSysNewEffect" runat="server" Text="Смяна ефект :"></asp:Label>
                </div>
                <div style="margin-left:135px;">
                 <asp:TextBox ID="tbEditSysName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                    ControlToValidate="tbEditSysName" ErrorMessage="*" 
                    ValidationGroup="vgEditSysName" CssClass="errors"></asp:RequiredFieldValidator>
                    <asp:Button ID="btnChangeSysName" runat="server" 
                        onclick="btnChangeSysName_Click" SkinID="editBtn" Text="Промени" 
                        ValidationGroup="vgEditSysName" />
                    <br />
                <asp:DropDownList ID="ddlEditSysClass" runat="server" style="margin:3px 0px;" 
                        AutoPostBack="True" 
                        onselectedindexchanged="ddlEditSysClass_SelectedIndexChanged">
                <asp:ListItem Value="0">неизвестен</asp:ListItem>
                <asp:ListItem Value="1">дупка</asp:ListItem>
                <asp:ListItem Value="2">C1</asp:ListItem>
                <asp:ListItem Value="3">C2</asp:ListItem>
                <asp:ListItem Value="4">C3</asp:ListItem>
                <asp:ListItem Value="5">C4</asp:ListItem>
                <asp:ListItem Value="6">C5</asp:ListItem>
                <asp:ListItem Value="7">C6</asp:ListItem>
                <asp:ListItem Value="8">0.0</asp:ListItem>
                <asp:ListItem Value="9">low sec</asp:ListItem>
                <asp:ListItem Value="10">high sec</asp:ListItem>
               </asp:DropDownList>

               <br />

               <asp:DropDownList ID="ddlEditSysEffect" runat="server" AutoPostBack="True" 
                        onselectedindexchanged="ddlEditSysEffect_SelectedIndexChanged">
                <asp:ListItem Value="0">неизвестен</asp:ListItem>
                <asp:ListItem Value="1">няма ефект</asp:ListItem>
                <asp:ListItem Value="2">Black Hole</asp:ListItem>
                <asp:ListItem Value="3">Cataclysmic Variable</asp:ListItem>
                <asp:ListItem Value="4">Magnetar</asp:ListItem>
                <asp:ListItem Value="5">Pulsar</asp:ListItem>
                <asp:ListItem Value="6">Red Giant</asp:ListItem>
                <asp:ListItem Value="7">Wolf-Rayet</asp:ListItem>
               </asp:DropDownList>
                </div>
                </div>

                <br />
                <div class="clearfix2">
                <div style="float:left; width:130px; text-align:right;">
                    <asp:Label ID="lblStaticEditSystemOccupied" runat="server" Text="Окупирана :"></asp:Label>
                 <br />
                    <asp:Button ID="btnChangeSysOccupied" runat="server" style="margin-top:3px;"
                        onclick="btnChangeSysOccupied_Click" Text="Промени" SkinID="editBtn" />
                    &nbsp;</div>
                <div style="margin-left:135px;">
                <asp:TextBox ID="tbEditSysOccupied" runat="server" Rows="3" 
                    TextMode="MultiLine" Width="350px"></asp:TextBox>
                </div>
                </div>

                 <br />
                <div class="clearfix2">
                <div style="float:left; width:130px; text-align:right;">
                    <asp:Label ID="lblStaticEditSystemInfo" runat="server" Text="Доп. инфо :"></asp:Label>
               <br />
                    <asp:Button ID="btnChangeSysInfo" runat="server" style="margin-top:3px;"
                        onclick="btnChangeSysInfo_Click" Text="Промени" SkinID="editBtn" />
                    &nbsp;<br />

                </div>
                <div style="margin-left:135px;">
                
                    <asp:TextBox ID="tbEditSysInfo" runat="server" Rows="3" TextMode="MultiLine" 
                        Width="350px"></asp:TextBox>
                
                </div>
                </div>

               <br />
                <asp:Button ID="btnCancelEditSystem" runat="server" 
                    onclick="btnCancelEditSystem_Click" Text="Затвори" style="margin-left:53px;" 
                    SkinID="cancelBtn"/>
                <asp:Label ID="lblEditSystemError" runat="server" Visible="False" 
                    CssClass="errors"></asp:Label>
                <br />
            </asp:Panel>


        <asp:Panel ID="pnlAddSignature" runat="server" DefaultButton="btnAddSignature" 
            Visible="False" Width="500px" CssClass="panel trans">
            <div style="text-align:center;  margin-bottom:10px;" class="headerText">
                <asp:Label ID="lblStaticAddSignature" runat="server" Text="Добавяне на сигнатура в :"></asp:Label>
            &nbsp;<asp:Label ID="lblSignatureToSystem" runat="server" 
                    Text="Label" CssClass="systems"></asp:Label>
            </div>
            
            <div class="clearfix2">
            <div style="float:left; width:230px; padding-right:5px;">
            
            
             <div class="clearfix2">
                <div style="float:left; width:70px; text-align:right;">
                <asp:Label ID="lblStaticAddSignatureName" runat="server" Text="Име :"></asp:Label>

                <div style="margin:7px 0px 3px 0px;">
                <asp:Label ID="lblStaticAddSignatureType" runat="server" Text="Тип :"></asp:Label>
                </div>

                </div>
                <div style="margin-left:75px;">
                 <asp:TextBox ID="tbNewSigName" runat="server" style="margin-bottom: 0px" 
                    SkinID="required" Width="120px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                ControlToValidate="tbNewSigName" ErrorMessage="*" ValidationGroup="vgAddSig" 
                    CssClass="errors"></asp:RequiredFieldValidator>

                    <div style="margin:3px 0px 3px 0px;">
                    <asp:DropDownList ID="ddlNewSigType" runat="server"
                    CssClass="sigType">
                <asp:ListItem Value="6">не е сканирана</asp:ListItem>
                <asp:ListItem Value="5">дупка</asp:ListItem>
                <asp:ListItem Value="1">magnetometric</asp:ListItem>
                <asp:ListItem Value="2">gravimetric</asp:ListItem>
                <asp:ListItem Value="3">radar</asp:ListItem>
                <asp:ListItem Value="4">ladar</asp:ListItem>
            </asp:DropDownList>
            </div>

                </div>
                </div>
            
            
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnAddSignature" runat="server" onclick="btnAddSignature_Click" 
                    Text="Добави" ValidationGroup="vgAddSig"  />
            </div>
            <div style="margin-left:235px;">
            <asp:TextBox ID="tbNewSignatureInfo" runat="server" Rows="4" 
                TextMode="MultiLine" Width="250px"></asp:TextBox>
                <cc1:TextBoxWatermarkExtender ID="tbNewSignatureInfo_TextBoxWatermarkExtender" 
                    runat="server" TargetControlID="tbNewSignatureInfo" 
                    WatermarkText="Допълнителна информация"></cc1:TextBoxWatermarkExtender>
            </div>
            </div>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="lblAddSignatureError" runat="server" Visible="False" 
                CssClass="errors"></asp:Label>
        </asp:Panel>
        
            
            



        <asp:Panel ID="pnlAddSystem" runat="server" Width="500px" style="margin-top:8px;"
            DefaultButton="btnAddSystem" Visible="False" CssClass="panel trans">
            <div style="text-align:center; margin-bottom:10px;" class="headerText">
                <asp:Label ID="lblStaticAddNewSystem" runat="server" Text="Добави система до която се стига от :"></asp:Label>
            <asp:Label ID="lblFromSystem" runat="server" CssClass="systems"></asp:Label>
            </div>
          
           <div class="clearfix2">
           <div style="float:left; width:270px; text-align:right;">
            
               <asp:Label ID="lblStaticAddSystemName" runat="server" Text="Име на системата :"></asp:Label>
           <br />
           <div style="margin:6px 0px;"><asp:Label ID="lblStaticAddSystemClass" runat="server" Text="Клас :"></asp:Label></div>
           <asp:Label ID="lblStaticAddSystemEffect" runat="server" Text="Ефект :"></asp:Label><br />
           
           <div style="margin:6px 0px;"><asp:Label ID="lblStaticAddSystemWormIDToIt" runat="server" Text="ID на дупката водеща към нея :"></asp:Label></div>
           <asp:Label ID="lblStaticAddSystemWormIDFromIt" runat="server" Text="ID на дупката от обратната страна :"></asp:Label>
           </div>
           <div style="margin-left:275px;">
           
               <asp:TextBox ID="tbNewSysName" runat="server" style="" SkinID="required"></asp:TextBox>
               <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                   ControlToValidate="tbNewSysName" ErrorMessage="*" 
                   ValidationGroup="vgAddSystem" CssClass="errors"></asp:RequiredFieldValidator>
               <br />

               <asp:DropDownList ID="ddlNewSysClass" runat="server" style="margin:3px 0px;">
                <asp:ListItem Value="0">...избери</asp:ListItem>
                <asp:ListItem Value="1">дупка</asp:ListItem>
                <asp:ListItem Value="2">C1</asp:ListItem>
                <asp:ListItem Value="3">C2</asp:ListItem>
                <asp:ListItem Value="4">C3</asp:ListItem>
                <asp:ListItem Value="5">C4</asp:ListItem>
                <asp:ListItem Value="6">C5</asp:ListItem>
                <asp:ListItem Value="7">C6</asp:ListItem>
                <asp:ListItem Value="8">0.0</asp:ListItem>
                <asp:ListItem Value="9">low sec</asp:ListItem>
                <asp:ListItem Value="10">high sec</asp:ListItem>
               </asp:DropDownList>

               <br />

               <asp:DropDownList ID="ddlNewSysEffect" runat="server">
                <asp:ListItem Value="0">...избери</asp:ListItem>
                <asp:ListItem Value="1">няма ефект</asp:ListItem>
                <asp:ListItem Value="2">Black Hole</asp:ListItem>
                <asp:ListItem Value="3">Cataclysmic Variable</asp:ListItem>
                <asp:ListItem Value="4">Magnetar</asp:ListItem>
                <asp:ListItem Value="5">Pulsar</asp:ListItem>
                <asp:ListItem Value="6">Red Giant</asp:ListItem>
                <asp:ListItem Value="7">Wolf-Rayet</asp:ListItem>
               </asp:DropDownList>

               <br />
               <asp:TextBox ID="tbFromSysWhID" runat="server" style="margin:3px 0px;"></asp:TextBox>
               <br />
               <asp:TextBox ID="tbToSysWhID" runat="server"></asp:TextBox>
          
           </div>
           </div>

            <div class="clearfix2" style="margin-top:5px;">
                <div style="float:left; width:130px; text-align:right;">
                <br />
                    <asp:Label ID="lblStaticAddSystemOccupied" runat="server" Text="Окупирана :"></asp:Label>
                </div>
                <div style="margin-left:135px;">
                <asp:TextBox ID="tbNewSysOccupied" runat="server" Rows="3" 
                    TextMode="MultiLine" Width="350px"></asp:TextBox>
                </div>
           </div>

                

           <div class="clearfix2" style="margin-top:5px; margin-bottom:3px;">
                <div style="float:left; width:130px; text-align:right;">
                <br />
                    <asp:Label ID="lblStaticAddSystemInfo" runat="server" Text="Доп. инфо :"></asp:Label>
                </div>
                <div style="margin-left:135px;">
                
                    <asp:TextBox ID="tbNewSysInfo" runat="server" Rows="3" TextMode="MultiLine" 
                        Width="350px"></asp:TextBox>
                
                </div>
           </div>

             
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnAddSystem" runat="server" Text="Добави" 
                onclick="btnAddSystem_Click" ValidationGroup="vgAddSystem" />
            &nbsp;
            <asp:Label ID="lblAddSystemError" runat="server" Visible="False" 
                CssClass="errors"></asp:Label>
        </asp:Panel>
           
         <asp:Label ID="lblUpdatesSinceLoad" runat="server"></asp:Label>
        
        </div>

        </div>

        <asp:Panel ID="pnlSiteErrors" runat="server" style="margin:10px 10px 0px 10px;" Visible="False">
        <asp:Label ID="lblStaticSiteErrors" CssClass="errors" runat="server" Text="* Системи не могат да се трият от играта."></asp:Label>
        </asp:Panel>

        <asp:HiddenField ID="hfPageLoaded" runat="server" />
        
        <asp:PlaceHolder ID="phWhInfoPanels" runat="server"></asp:PlaceHolder>
    
        <asp:PlaceHolder ID="phSystemInfoPanels" runat="server"></asp:PlaceHolder>
    
        <asp:Panel ID="pnlUpdatesSinceLoad" runat="server" style="visibility:hidden">
        </asp:Panel>


    </div>
    </asp:Content>
