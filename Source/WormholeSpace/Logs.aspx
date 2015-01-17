<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.Master" CodeBehind="Logs.aspx.cs" Inherits="WhSpace.Logs" Theme="Skins"%>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="">
    <br />
        <div style="padding-left:20px; margin-top:10px;">

            <asp:Panel ID="pnlShowLogs" runat="server" style="margin:10px 0px;">

                <asp:Label ID="lblStaticLastLogsFor" runat="server" Text="Последни логове за:"></asp:Label>&nbsp; 
            <asp:DropDownList ID="ddlUsers" runat="server" Width="200px">
            </asp:DropDownList>
                <asp:TextBox ID="tbLogsCount" runat="server" Width="50px" SkinID="required"></asp:TextBox>
                <cc1:FilteredTextBoxExtender ID="tbLogsCount_FilteredTextBoxExtender" 
                runat="server" FilterType="Numbers" TargetControlID="tbLogsCount">
            </cc1:FilteredTextBoxExtender>
            <cc1:TextBoxWatermarkExtender ID="tbLogsCount_TextBoxWatermarkExtender" 
                runat="server" TargetControlID="tbLogsCount" WatermarkText="бройка">
            </cc1:TextBoxWatermarkExtender>
            <asp:Button ID="btnShowLogs" runat="server" Text="Покажи" 
                onclick="btnShowLogs_Click" ValidationGroup="1" />

                &nbsp;<asp:Label ID="lblShowLogsError" runat="server" CssClass="errors" Text="Error" 
                    Visible="False"></asp:Label>

                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    ControlToValidate="tbLogsCount" ValidationGroup="1"></asp:RequiredFieldValidator>

            </asp:Panel>

           
     <asp:Table ID="tblLogs" runat="server" CellPadding="2" CellSpacing="2">
        </asp:Table>
        <asp:Label ID="lblNoLogs" runat="server" CssClass="errors" Text="Label" 
            Visible="False"></asp:Label>

    </div>


    </div>
</asp:Content>
