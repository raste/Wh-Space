<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="WhSpace.Profile" Theme="Skins"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="padding:10px;">

 <asp:Panel ID="pnlStatus" CssClass="statusPnl" style="margin-bottom:7px;" 
            runat="server" Direction="NotSet" Visible="False">
        
         <asp:Label ID="lblStaticStatus" style="color:Black;" runat="server" Text="Статус :"></asp:Label>
         <asp:Label ID="lblStatus" runat="server"></asp:Label>

        </asp:Panel>



    <asp:Panel ID="pnlChangePass" runat="server" Width="400px" CssClass="panel">

    <div style="text-align:center;" class="headerText">
                    <asp:Label ID="lblStaticChangePassword" runat="server" Text="Смяна на парола"></asp:Label>
                </div>
                    <br />

                <div class="clearfix2">
                <div style="float:left; width:150px; text-align:right; padding-top:0px;">

                    <asp:Label ID="lblStaticOldPassword" runat="server" Text="Stara parola:"></asp:Label>
                    <div style="margin:5px 0px;">
                    <asp:Label ID="lblStaticNewPassword" runat="server" Text="Nova parola:"></asp:Label>
                    </div>

                </div>
                 <div style="margin-left:155px;">
                     <asp:TextBox ID="tbOldPassword" runat="server" TextMode="Password"></asp:TextBox>
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                         ControlToValidate="tbNewPassword" CssClass="errors" ErrorMessage="*" 
                         ValidationGroup="pass"></asp:RequiredFieldValidator>
                     <br />
                     <asp:TextBox ID="tbNewPassword" style="margin-top:3px;" runat="server" 
                         TextMode="Password"></asp:TextBox>
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                         ControlToValidate="tbNewPassword" CssClass="errors" ErrorMessage="*" 
                         ValidationGroup="pass"></asp:RequiredFieldValidator>
                 </div>
                 </div>
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnChangePass" runat="server" Text="Change" 
            onclick="btnChangePass_Click" ValidationGroup="pass" />
        &nbsp;<asp:Label ID="lblChangePassError" CssClass="errors" runat="server" 
            Text="Error" Visible="False"></asp:Label>

    </asp:Panel>



    <br />
    <br />
    <br />
    <br />
    <br />


</div>
</asp:Content>
