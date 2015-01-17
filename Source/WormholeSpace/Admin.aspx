<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.Master" CodeBehind="Admin.aspx.cs" Inherits="WhSpace.Admin" Theme="Skins"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="padding-top:10px;">
      
       

        <asp:Panel ID="pnlStatus" CssClass="statusPnl" style="width:840px;" 
            runat="server" Direction="NotSet" Visible="False">
        
            <asp:Label ID="lblStaticStatus" runat="server" Text="Статус :" style="color:Black;"></asp:Label>
         <asp:Label ID="lblStatus" runat="server"></asp:Label>

        </asp:Panel>

        <div class="clearfix2">
        <div style="float:left;width:485px; padding-left:15px;">
        

         <div class="clearfix2">
        <div style="float:left;width:120px; text-align:right;">
            <asp:Label ID="lblStaticUsers" runat="server" Text="Потребители >>"></asp:Label>
        </div>
        <div style="margin-left:125px;">
        <asp:PlaceHolder ID="phUsers" runat="server"></asp:PlaceHolder>
        </div>
        </div>


        
        
        </div>
        <div style="margin-left:510px;">
        
        <asp:Panel ID="pnlAddUser" runat="server" DefaultButton="btnAddUser" 
            Width="400px" CssClass="panel">

            <div style="text-align:center;  margin-bottom:10px;" class="headerText">
                <asp:Label ID="lblStaticAddUser" runat="server" Text="Добавяне на потребител"></asp:Label>
            </div> 


            <div class="clearfix2">
        <div style="float:left;width:150px; text-align:right;">
            <asp:Label ID="lblStaticAddUserName" runat="server" Text="Потребителско име :"></asp:Label>
        <div style="margin:5px 0px;">
            <asp:Label ID="lblStaticAddUserPassword" runat="server" Text="Парола :"></asp:Label></div>
        </div>
        <div style="margin-left:155px;">
         <asp:TextBox ID="tbAddUsername" runat="server" Width="150px" SkinID="required"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                ControlToValidate="tbAddUsername" ErrorMessage="*" 
                ValidationGroup="vgAddUser" CssClass="errors"></asp:RequiredFieldValidator>
            <br />
            <asp:TextBox ID="tbAddPassword" runat="server" TextMode="Password" 
                style="margin: 3px 0px;" Width="150px" SkinID="required"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                ControlToValidate="tbAddPassword" ErrorMessage="*" 
                ValidationGroup="vgAddUser" CssClass="errors"></asp:RequiredFieldValidator>
        </div>
        </div>

            &nbsp;&nbsp;

            <asp:RadioButtonList ID="rblAddUser" runat="server" 
                RepeatDirection="Horizontal" RepeatLayout="Flow" Width="200px">
                <asp:ListItem>админ</asp:ListItem>
                <asp:ListItem>мембер</asp:ListItem>
                <asp:ListItem Selected="True">гост</asp:ListItem>
            </asp:RadioButtonList>
            <asp:Button ID="btnAddUser" runat="server" onclick="btnAddUser_Click" 
                Text="Добави" ValidationGroup="vgAddUser" />
            <br />
            &nbsp;&nbsp;&nbsp; <asp:Label ID="lblAddUserError" runat="server" 
                Visible="False" CssClass="errors"></asp:Label>
            <br />
        </asp:Panel>
           
         

        <asp:Panel ID="pnlModifyUser" runat="server" Width="400px" CssClass="panel" style="margin-top:7px;">
        <div style="text-align:center;  margin-bottom:10px;" class="headerText">
            <asp:Label ID="lblStaticEditUser" runat="server" Text="Промяна на потребител"></asp:Label>
        </div>
      
        <div class="clearfix2" style="margin-bottom:3px;">
        <div style="float:left;width:150px; text-align:right;">
        <asp:Label ID="lblStaticEditUserName" runat="server" Text="Потребител :"></asp:Label>
        </div>
        <div style="margin-left:155px;">
        <asp:DropDownList ID="ddlEditUser" runat="server" AutoPostBack="True" 
                onselectedindexchanged="ddlEditUser_SelectedIndexChanged" CssClass="users" 
                SkinID="required">
            </asp:DropDownList>
        </div>
        </div>
        
           
            &nbsp;&nbsp;&nbsp;&nbsp;<asp:RadioButtonList ID="rblEditUser" runat="server" 
                RepeatDirection="Horizontal" RepeatLayout="Flow" Width="200px">
                <asp:ListItem>админ</asp:ListItem>
                <asp:ListItem>мембер</asp:ListItem>
                <asp:ListItem Selected="True">гост</asp:ListItem>
            </asp:RadioButtonList>
            &nbsp;<asp:Button ID="btnEditUser" runat="server" onclick="btnEditUser_Click" 
                Text="Промени" SkinID="editBtn" />
            &nbsp;<asp:Button ID="btnDelUser" runat="server" Text="Изтрий" 
                SkinID="delBtn" onclick="btnDelUser_Click" />
            &nbsp;<br /> &nbsp;&nbsp;<asp:Button ID="btnMakeCorpAdmin" runat="server" 
                onclick="btnMakeCorpAdmin_Click" Text="Корп. админ" Visible="False" />
            &nbsp;&nbsp; <asp:Label ID="lblEditUserError" runat="server" 
                Visible="False" CssClass="errors"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnlEditCorporation" runat="server" Width="400px" CssClass="panel" style="margin-top:7px;">

            <div style="text-align:center;  margin-bottom:10px;" class="headerText">
            <asp:Label ID="lblStaticEditCorporation" runat="server" Text="Промяна по корпорацията"></asp:Label>
        </div>
    
        <div class="clearfix2">
        <div style="float:left;width:150px; text-align:right;">
            <asp:Label ID="lblStaticEditCorporationName" runat="server" Text="New name:"></asp:Label>
            <div style="margin:5px 0px;">
                <asp:Label ID="lblStaticEditCorporationLogsLanguage" runat="server" Text="Label"></asp:Label>
            </div>
        </div>
        <div style="margin-left:155px;">
            <asp:TextBox ID="tbCorpNewName" runat="server" SkinID="required"></asp:TextBox>
            <asp:Button ID="btnChangeCorprationName" runat="server" Text="Change" 
                onclick="btnChangeCorprationName_Click" ValidationGroup="corp" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                ControlToValidate="tbCorpNewName" ValidationGroup="corp"></asp:RequiredFieldValidator>
            <br />
            <asp:DropDownList ID="ddlCorpLogsLanguages" runat="server" style="margin: 3px 0px;" AutoPostBack="True" 
                onselectedindexchanged="ddlCorpLogsLanguages_SelectedIndexChanged">
            </asp:DropDownList>
        </div>
        </div>
        &nbsp;&nbsp;&nbsp;&nbsp; <asp:Label ID="lblEditCorporationError" runat="server" 
                Visible="False" CssClass="errors"></asp:Label>
            </asp:Panel>
           

        </div>
        </div>


         
        
        <div class="clearfix2">
        <div style="float:left;width:500px;">
        
        </div>
        <div style="margin-left:510px;">
        
        </div>
        </div>



    
    </div>
    </asp:Content>
