<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Operations.aspx.cs" Inherits="WhSpace.Operations" Theme="Skins"%>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1
        {
            width: 100%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="text-align:left; padding-top:10px;">
    
    
        <asp:HiddenField ID="hfOpEdited" runat="server" />
        <asp:HiddenField ID="hfUserEdited" runat="server" />
        <asp:HiddenField ID="hfLootEdited" runat="server" />
    
        <asp:Panel ID="pnlStatus" CssClass="statusPnl marginTB10" runat="server" Visible="False">
            <asp:Label ID="lblStaticStatus" runat="server" Text="Статус :" style="color:Black;"></asp:Label>
         <asp:Label ID="lblStatus" runat="server"></asp:Label>
        </asp:Panel>

    
  
        <asp:Button ID="btnShowAddOperation" runat="server" 
            onclick="btnShowAddOperation_Click" Text="Добави операция" 
            Visible="False" />
       

        <asp:Panel ID="pnlAddOperation" runat="server" Visible="False" 
            ViewStateMode="Enabled" CssClass="pnlAddOperation marginTB10" 
            DefaultButton="btnAdd">

            <div style="text-align:center; margin-bottom:10px;" class="headerText">
                <asp:Label ID="lblStaticAddOperation" runat="server" Text="Добавяне на операция"></asp:Label>
                </div>

            <asp:UpdatePanel ID="upAddOpertaion" runat="server" ViewStateMode="Enabled">
                <ContentTemplate>
                <div class="clearfix2">
                    <asp:Label ID="lblStaticOperationType" runat="server" Text="Операция тип:"></asp:Label>
            <asp:DropDownList ID="ddlOperationType" runat="server" AutoPostBack="True" 
                        onselectedindexchanged="ddlOperationType_SelectedIndexChanged" 
                        SkinID="required">
                <asp:ListItem Value="1">бойна</asp:ListItem>
                <asp:ListItem Value="2">събиране газ</asp:ListItem>
            </asp:DropDownList>
                    , <asp:Label ID="lblStaticOperationBasedOn" runat="server" Text="базирана на:"></asp:Label>
                    <asp:DropDownList ID="ddlOpBasedType" runat="server" AutoPostBack="True" 
                        onselectedindexchanged="ddlOpBasedType_SelectedIndexChanged" 
                        SkinID="required">
                        <asp:ListItem Value="1">плексове</asp:ListItem>
                        <asp:ListItem Value="2">време</asp:ListItem>
                    </asp:DropDownList>
                    , <asp:Label ID="lblStaticOperationLength" runat="server" Text="продължителност:"></asp:Label>
                    <asp:TextBox ID="tbOpLength" runat="server" Width="50px" SkinID="required"></asp:TextBox>
                    , <asp:Label ID="lblStaticOperationSystem" runat="server" Text="система:"></asp:Label>
                    <asp:TextBox ID="tbSystem" runat="server"></asp:TextBox>
                    , <asp:CheckBox ID="cbAddAllRows" runat="server" Text="Add all rows" 
                            AutoPostBack="True" oncheckedchanged="cbAddAllRows_CheckedChanged" />
               
                    </div>

                    <div class="clearfix2" style="margin:5px 0px;">
                    <asp:Panel ID="pnlOpParticipants" runat="server" Width="400px" CssClass="pnlLootUsers" style="float:left;width:490px;">

                    <div style="text-align:center; margin:5px 0px 10px 0px;">
                       <asp:Label ID="lblStaticParticipantsInfo" runat="server" Text="Участвали: име - време - доп. инфо"></asp:Label>
                        </div>

                        <asp:PlaceHolder ID="phOpUsers" runat="server" ViewStateMode="Enabled"></asp:PlaceHolder>
                        
                        <asp:Button ID="btnAddOpUser" runat="server" Text="Добави мембер" style="margin:10px 0px 5px 0px;" 
                            onclick="btnAddOpUser_Click" />
                        
                    </asp:Panel>


                        <asp:Panel ID="pnlLoot" runat="server" CssClass="pnlLootUsers" style="margin-left:510px;">

                             <div style="text-align:center; margin:5px 0px 10px 0px;">
                            <asp:Label ID="lblStaticOperationLootInfo" runat="server" Text="Лоот: име - количество - цена бройка"></asp:Label>
                            </div>

                            <asp:PlaceHolder ID="phOpLoot" runat="server"></asp:PlaceHolder>
                           
                            <asp:Button ID="btnAddOpLoot" runat="server" Text="Добави лоот" style="margin:10px 0px 5px 0px;"
                                onclick="btnAddOpLoot_Click" />
                            
                     
                                <asp:CheckBox ID="cbFillLootPriceOnSelect" runat="server" 
                                    Text="Fill loot price on select" />
                     

                        </asp:Panel>
                    </div>

                    <div class="clearfix2" style="margin-bottom:5px;">

                    <div style="float:left;width:490px;" class="pnlLootUsers">
                        <asp:Label ID="lblStaticOperationTax" runat="server" Text="Такса:"></asp:Label>
                    <asp:RadioButtonList ID="rblIskCutType" runat="server" 
                        RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Selected="True" Value="1">%</asp:ListItem>
                        <asp:ListItem Value="2">isk</asp:ListItem>
                    </asp:RadioButtonList>
                    &nbsp;<asp:TextBox ID="tbIskCut" runat="server" onkeyup="insertCommas(this);" Width="150px"></asp:TextBox>
                    <br />
                    <asp:TextBox ID="tbIskCutInfo" runat="server" TextMode="MultiLine" Rows="3" 
                            Width="475px" style="margin:5px;" Height="50px"></asp:TextBox>
                        <cc1:TextBoxWatermarkExtender ID="tbIskCutInfo_TextBoxWatermarkExtender" 
                            runat="server" TargetControlID="tbIskCutInfo" 
                            WatermarkText="Допълнително инфо за таксата.">
                        </cc1:TextBoxWatermarkExtender>
                    </div>
                    <div style="margin-left:510px; text-align:center;" class="pnlLootUsers">
                    
                        <asp:TextBox ID="tbOpInfo" runat="server" Height="80px" TextMode="MultiLine" 
                            Width="450px"></asp:TextBox>
                    
                        <cc1:TextBoxWatermarkExtender ID="tbOpInfo_TextBoxWatermarkExtender" 
                            runat="server" TargetControlID="tbOpInfo" 
                            WatermarkText="Допълнително инфо за оп-а.">
                        </cc1:TextBoxWatermarkExtender>
                    
                    </div>

                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>

            
            <asp:Button ID="btnAdd" runat="server" Text="Добави" onclick="btnAdd_Click" />
            
            &nbsp;<asp:Button ID="btnCancelAdd" runat="server" onclick="btnCancelAdd_Click" 
                Text="Затвори" SkinID="cancelBtn" />
            &nbsp;<asp:Label ID="lblErrorAddOp" runat="server" Text="Error" Visible="False" 
                CssClass="errors"></asp:Label>
            
        </asp:Panel>
      




        <asp:Panel ID="pnlEditOpGeneral" runat="server" Visible="False" 
            CssClass="panel marginTB10">
            <div style="text-align:center; margin-bottom:10px;" class="headerText">
                <asp:Label ID="lblStaticEditOperation" runat="server" Text="Промяна главна информация за операция ID:"></asp:Label>
                <asp:Label ID="lblEditOpGeneralID" runat="server" Text="ID"></asp:Label>
                </div>
                
            <table class="style1">
                <tr>
                    <td style="width:200px; text-align:right;">
                        <asp:Label ID="lblStaticEditOperationLength" runat="server" Text="Продължителност:"></asp:Label>
                        </td>
                    <td>
                        <asp:TextBox ID="tbEditOpLength" runat="server"></asp:TextBox>
                        <asp:Button ID="btnEditOpLength" runat="server" onclick="btnEditOpLength_Click" 
                            SkinID="editBtn" Text="Промени" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align:right;">
                        <asp:Label ID="lblStaticEditOperationSystem" runat="server" Text="Система:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbEditOpSys" runat="server"></asp:TextBox>
                        <asp:Button ID="btnEditOpSystem" runat="server" onclick="btnEditOpSystem_Click" 
                            SkinID="editBtn" Text="Промени" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align:right;">
                        <asp:Label ID="lblstaticEditOperationInfo" runat="server" Text="Доп. инфо:"></asp:Label>
                        </td>
                    <td>
                        <asp:TextBox ID="tbEditOpInfo" runat="server" Rows="3" TextMode="MultiLine" 
                            Height="60px" Width="300px"></asp:TextBox>
                        
                        <asp:Button ID="btnEditOpInfo" runat="server" onclick="btnEditOpInfo_Click" 
                            SkinID="editBtn" Text="Промени" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align:right;">
                        &nbsp;</td>
                    <td>
                        &nbsp; &nbsp;</td>
                </tr>
                <tr>
                    <td style="text-align:right;">
                        <asp:Label ID="lblStaticEditOperationTax" runat="server" Text="Такса:"></asp:Label>
                        </td>
                    <td>
                        <asp:RadioButtonList ID="rblEditIskCutType" runat="server" 
                            RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Selected="True" Value="1">%</asp:ListItem>
                            <asp:ListItem Value="2">isk</asp:ListItem>
                        </asp:RadioButtonList>
                        <asp:TextBox ID="tbEditOpIskCut" onkeyup="insertCommas(this);" runat="server"></asp:TextBox>
                        <asp:Button ID="btnEditOpIskCut" runat="server" onclick="btnEditOpIskCut_Click" 
                            SkinID="editBtn" Text="Промени" />
                    </td>
                </tr>
                <tr>
                    <td style="text-align:right;">
                        <asp:Label ID="lblStaticEditOperationTaxInfo" runat="server" Text="Доп. инфо за таксата:"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="tbEditOpIskCutInfo" runat="server" Rows="3" 
                            TextMode="MultiLine" Height="60px" Width="300px"></asp:TextBox>
                        <asp:Button ID="btnEditIskCutInfo" runat="server" 
                            onclick="btnEditIskCutInfo_Click" SkinID="editBtn" Text="Промени" />
                    </td>
                </tr>
            </table>
          
          <div style="margin-top:5px; padding-left:205px;">  
<asp:Button ID="btnCancelEditOp" runat="server" onclick="btnCancelEditOp_Click" 
                SkinID="cancelBtn" Text="Затвори"  />
            &nbsp;<asp:Label ID="lblEditOpError" runat="server" Text="Label" Visible="False" 
                CssClass="errors"></asp:Label>
            </div>
        </asp:Panel>
      



        <asp:Panel ID="pnlEditOpUser" runat="server" Visible="False" CssClass="panel marginTB10" style="padding:10px;">

         <div style="text-align:center; margin-bottom:10px;" class="headerText">
             <asp:Label ID="lblStaticEditMember" runat="server" Text="Промяна мембер участвал в операция ID:"></asp:Label>
                <asp:Label ID="lblEditOpUserID" runat="server" Text="ID"></asp:Label>
                </div>
                

            <asp:DropDownList ID="ddlEditOpUser" runat="server" Width="200px" 
                SkinID="required">
            </asp:DropDownList>
            &nbsp;, <asp:Label ID="lblStaticEditMemberLength" runat="server" Text="Продължителност:"></asp:Label>
            <asp:TextBox ID="tbEditUserTime" runat="server" SkinID="required"></asp:TextBox>
            , <asp:Label ID="lblStaticEditMemberInfo" runat="server" Text="Доп. инфо:"></asp:Label>
            <asp:TextBox ID="tbEditUserInfo" runat="server" Rows="3" Width="200px"></asp:TextBox>
            &nbsp;<asp:Button ID="btnEditUser" runat="server" onclick="btnEditUser_Click" 
                SkinID="editBtn" Text="Промени" />
            &nbsp;<asp:Button ID="btnCancelEditUser" runat="server" Text="Затвори" 
                onclick="btnCancelEditUser_Click" SkinID="cancelBtn" />
            <div style="margin-top:3px;">
            &nbsp;<asp:Label ID="lblEditUserError" runat="server" Text="Label" Visible="False" 
                CssClass="errors"></asp:Label>
                </div>
        </asp:Panel>
 



        <asp:Panel ID="pnlEditLoot" runat="server" Visible="False" CssClass="panel marginTB10" style="padding:10px;">

         <div style="text-align:center; margin-bottom:10px;" class="headerText">
             <asp:Label ID="lblStaticEditOperationLoot" runat="server" Text="Промяна лоот паднал по-време на операция ID:"></asp:Label>
                 <asp:Label ID="lblEditLootForOpID" runat="server" Text="ID"></asp:Label>
                </div>
               

            <asp:DropDownList ID="ddlEditLoot" runat="server" Width="250px" 
                SkinID="required">
            </asp:DropDownList>
            , <asp:Label ID="lblStaticEditLootQuantity" runat="server" Text="Количество:"></asp:Label>
            <asp:TextBox ID="tbEditLootQuantity" runat="server" SkinID="required"></asp:TextBox>
            , <asp:Label ID="lblStaticEditLootPriceEach" runat="server" Text="Цена бройка:"></asp:Label>
            <asp:TextBox ID="tbEditLootPrice" runat="server" SkinID="required" onkeyup="insertCommas(this);"></asp:TextBox>
            &nbsp;<asp:Button ID="btnEditLoot" runat="server" Text="Промени" 
                onclick="btnEditLoot_Click" SkinID="editBtn" />
            &nbsp;<asp:Button ID="btnCancelEditLoot" runat="server" Text="Затвори" 
                onclick="btnCancelEditLoot_Click" SkinID="cancelBtn" />
            <div style="margin-top:3px;">
            &nbsp;<asp:Label ID="lblEditLootError" runat="server" Text="Label" Visible="False" 
                CssClass="errors"></asp:Label>
                </div>
        </asp:Panel>
      



        <asp:Panel ID="pnlAddUser" runat="server" Visible="False" CssClass="panel marginTB10" style="padding:10px;">
         <div style="text-align:center; margin-bottom:10px;" class="headerText">
             <asp:Label ID="lblStaticAddMemberToOperation" runat="server" Text="Добавяне на мембер към операция ID:"></asp:Label>
                 <asp:Label ID="lblAddUserToOpID" runat="server" Text="ID"></asp:Label>
                </div>
                
        <asp:DropDownList ID="ddlAddUser" runat="server" Width="200px" SkinID="required">
            </asp:DropDownList>
            &nbsp;, <asp:Label ID="lblStaticAddUserLength" runat="server" Text="Продължителност:"></asp:Label>
            <asp:TextBox ID="tbAddUserTime" runat="server" SkinID="required"></asp:TextBox>
            , <asp:Label ID="lblStaticAddUserInfo" runat="server" Text="Доп. инфо:"></asp:Label>
            <asp:TextBox ID="tbAddUserInfo" runat="server" Rows="3" Width="200px"></asp:TextBox>
            &nbsp;<asp:Button ID="btnAddUser" runat="server" onclick="btnAddUser_Click" 
                Text="Добави" />
            &nbsp;<asp:Button ID="btnCancelAddUser" runat="server" Text="Затвори" 
                onclick="btnCancelAddUser_Click" SkinID="cancelBtn" />
            <div style="margin-top:3px;">
            &nbsp;<asp:Label ID="lblAddUserError" runat="server" Text="Label" 
                Visible="False" CssClass="errors"></asp:Label>
                </div>
        </asp:Panel>
      



        <asp:Panel ID="pnlAddLoot" runat="server" Visible="False" CssClass="panel marginTB10" style="padding:10px;">
         <div style="text-align:center; margin-bottom:10px" class="headerText">
             <asp:Label ID="lblStaticAddLootToOperation" runat="server" Text="Добавяне лоот към операция ID:"></asp:Label>
                 <asp:Label ID="lblAddLootToOpId" runat="server" Text="ID"></asp:Label>
                </div>
             
            <asp:DropDownList ID="ddlAddLoot" runat="server" Width="250px" 
                SkinID="required">
            </asp:DropDownList>
            , <asp:Label ID="lblStaticAddLootQuantity" runat="server" Text="Количество:"></asp:Label>
            <asp:TextBox ID="tbAddLootQuantity" runat="server" SkinID="required"></asp:TextBox>
            , <asp:Label ID="lblStaticAddLootPriceEach" runat="server" Text="Цена бройка:"></asp:Label>
            <asp:TextBox ID="tbAddLootPrice" runat="server" SkinID="required" onkeyup="insertCommas(this);"></asp:TextBox>
            &nbsp;<asp:Button ID="btnAddLoot" runat="server" Text="Добави" 
                onclick="btnAddLoot_Click" />
            &nbsp;<asp:Button ID="btnCancelAddLoot" runat="server" Text="Затвори" 
                onclick="btnCancelAddLoot_Click" SkinID="cancelBtn" />
            <div style="margin-top:3px;">
            &nbsp;<asp:Label ID="lblAddLootError" runat="server" Text="Label" Visible="False" 
                CssClass="errors"></asp:Label>
                </div>
        </asp:Panel>

        <asp:PlaceHolder ID="phPagesTop" runat="server"></asp:PlaceHolder>
        <asp:PlaceHolder ID="phOperations" runat="server"></asp:PlaceHolder>
        <asp:PlaceHolder ID="phPagesBottom" runat="server"></asp:PlaceHolder>
    
     <asp:Panel ID="pnlSiteErrors" runat="server" style="margin:10px 10px 0px 10px;" Visible="False">
        <asp:Label ID="lblStaticSiteErrors" CssClass="errors" runat="server" Text="* Операции / участници / лоот не могат да се трият от играта."></asp:Label>
        </asp:Panel>

    </div>
</asp:Content>
