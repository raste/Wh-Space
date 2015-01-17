<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WhSpace.Index" Theme="Skins"%>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link href="css/styles.css" rel="stylesheet" type="text/css" />

    <link rel="icon" href="images/favicon.ico" type="image/x-icon" /> 
    <link rel="shortcut icon" href="images/favicon.ico" type="image/x-icon" />

    </head>

<body class="logInBgr">
    <form id="form1" runat="server">
    <div style="text-align:center;">
    <div style="width:1000px;margin:0 auto; text-align:left;">
    


        <br />
        <br />
        <img src="images/projectName.png" style="margin-left:20px;" />
         <br />
        <br />

       <asp:Panel ID="pnlStatus" CssClass="indexStatusPnl"
            runat="server" Direction="NotSet" Visible="False">
        
         <asp:Label ID="lblStaticStatus" style="color:#C8C8C8;" runat="server" Text="Статус :"></asp:Label>
         <asp:Label ID="lblStatus" runat="server"></asp:Label>

        </asp:Panel>

    <div class="clearfix2" style="min-height:450px;">
    <div style="float:left; width:440px;">
    
            <asp:Panel ID="pnlLogIn" runat="server" DefaultButton="btnLogIn" Width="430px" 
             CssClass="logInPnl clearfix2">
            <asp:Button ID="btnDebug" runat="server" onclick="btnDebug_Click" 
                Text="debug login" Visible="False" style="margin-left:50px;"/>
            <br />
            <div class="clearfix2" style="margin-bottom:3px;">
                <div style="float:left;width:180px; text-align:right;">
                    <asp:Label ID="lblUsername" runat="server" Text="Потребителско име :"></asp:Label>
                 <div style="margin:5px 0px;">
                     <asp:Label ID="lblPassword" runat="server" Text="Парола :"></asp:Label>
                 </div>
                </div>
                <div style="margin-left:185px;">

                <div style="float:right; padding-right:45px;">
                 <asp:Button ID="btnLogIn" runat="server" onclick="btnLogIn_Click" Text="Вход" 
                ValidationGroup="vgLogIn" Height="45px" Width="60px" SkinID="whBtn" />
                </div>

                <asp:TextBox ID="tbUsername" runat="server" Width="120px" SkinID="required"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                ControlToValidate="tbUsername" ErrorMessage="*" ValidationGroup="vgLogIn" 
                CssClass="errors"></asp:RequiredFieldValidator>

                <br />
                    <asp:TextBox ID="tbPassword" runat="server" TextMode="Password" 
                        style="margin: 3px 0px 0px 0px;" Width="120px" SkinID="required"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                ControlToValidate="tbPassword" ErrorMessage="*" ValidationGroup="vgLogIn" 
                CssClass="errors"></asp:RequiredFieldValidator>

                </div>
            </div>
            <asp:Label ID="lblError" runat="server" Text="Error" Visible="False" style="margin-left:30px;"
                CssClass="errors"></asp:Label>
                <asp:CheckBox ID="cbStayLogged" runat="server" Text="Stay logged" 
                    style="float:right; margin-right:45px;" BorderColor="Yellow" />
            <br />
        </asp:Panel>
        
    </div>
    <div style="margin-left:450px;">
    
        <asp:Panel ID="pnlLinks" runat="server" >

         <asp:HyperLink ID="hlGAdmin" runat="server" NavigateUrl="~/GAdmin.aspx" CssClass="indexLinks" >
            <div class="clearfix2 indexLinkPnl" style="">
                <asp:Image ID="Image8" runat="server" ImageUrl="~/images/gadmin.png" ImageAlign="Left" style="margin:5px 0px;" BorderWidth="0"/> 
                
                <div class="linkTextDiv">
                    <asp:Label ID="lblStaticGlobalAdminPage" runat="server" Text="Site adminstration"></asp:Label>
                </div>
            </div>
            </asp:HyperLink>

        
            <asp:HyperLink ID="hlCorporation" runat="server" NavigateUrl="~/Main.aspx" CssClass="indexLinks" >
            <div class="clearfix2 indexLinkPnl" style="">
                <asp:Image ID="Image3" runat="server" ImageUrl="~/images/corporation.png" ImageAlign="Left" style="margin:5px 0px;" BorderWidth="0"/> 
                
                <div class="linkTextDiv">
                    <asp:Label ID="lblStaticCorporationPage" runat="server" Text="Corporation"></asp:Label>
                </div>
            </div>
            </asp:HyperLink>

            <asp:HyperLink ID="hlFeedBack" runat="server" NavigateUrl="~/FeedBack.aspx" CssClass="indexLinks" >
            <div class="clearfix2 indexLinkPnl" style="">
                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/feedback.png" ImageAlign="Left" style="margin:5px 0px;" BorderWidth="0"/> 
                
                <div class="linkTextDiv">
                    <asp:Label ID="lblStaticFeedBackPage" runat="server" Text="Feedback"></asp:Label>
                </div>
            </div>
            </asp:HyperLink>

               <asp:HyperLink ID="hlAbout" runat="server" NavigateUrl="~/About.aspx" CssClass="indexLinks" >
            <div class="clearfix2 indexLinkPnl" style="">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/about.png" ImageAlign="Left" style="margin:5px 0px;" BorderWidth="0" /> 
                
                <div class="linkTextDiv">
                    <asp:Label ID="lblStaticAboutPage" runat="server" Text="About"></asp:Label>
                </div>
            </div>
            </asp:HyperLink>

            <asp:LinkButton ID="lbLogOut" CssClass="indexLinks" runat="server" Visible="True" onclick="lbLogOut_Click"> </asp:LinkButton>
             <asp:LinkButton ID="lbRegCorp" CssClass="indexLinks" Visible="True" runat="server" onclick="lbRegCorp_Click"></asp:LinkButton>
              <asp:LinkButton ID="lbGuestLogIn" CssClass="indexLinks" Visible="True" runat="server" onclick="lbGuestLogIn_Click"></asp:LinkButton>
              

        </asp:Panel>


        <asp:Panel ID="pnlRegCorporation" runat="server" CssClass="regCorporationPnl" 
            DefaultButton="btnRegCorpAndUser" Visible="False">
            
             <div class="headerBlackText" style="margin:5px 0px 15px 0px;">
            <asp:Label ID="lblStaticRegCorporation" runat="server" Text="Reg Corporation"></asp:Label>
            </div>
            
             <div class="clearfix2" style="margin-bottom:10px;">
                <div style="float:left;width:220px; text-align:right;">

                    <asp:Label ID="lblStaticCorporationName" runat="server" Text="Name:"></asp:Label>
                   
                   <div style="margin:5px 0px;">
                    <asp:Label ID="lblStaticLogsLanguage" runat="server" Text="Logs language:"></asp:Label>
                     </div>
                    <br />
                    <div style="margin-top:3px; margin-bottom:2px;">
                    <asp:Label ID="lblStaticNameRootSystem" runat="server" Text="Root name:"></asp:Label>
                          </div>
                         <br />
                    <asp:Label ID="lblStaticNewUsername" runat="server" Text="Username:"></asp:Label>
                     <div style="margin-top:5px;">
                    <asp:Label ID="lblStaticNewUserPass" runat="server" Text="Password:"></asp:Label>
                    </div>
                </div>
                 <div style="margin-left:225px;">

                 <asp:TextBox ID="tbCorpName" runat="server" SkinID="required" Width="200px"></asp:TextBox>
                 <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                ControlToValidate="tbCorpName" ErrorMessage="*" ValidationGroup="reg" 
                         CssClass="errors"></asp:RequiredFieldValidator>
                 <br />
            <asp:DropDownList ID="ddlCorpLogsLanguage" runat="server" style="margin:3px 0px;" 
                         SkinID="required">
            </asp:DropDownList>
             <br />
            <br />
              <asp:TextBox ID="tbRootSysName" runat="server" SkinID="required" Width="150px"></asp:TextBox>
              <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                ControlToValidate="tbRootSysName" ErrorMessage="*" ValidationGroup="reg" 
                         CssClass="errors"></asp:RequiredFieldValidator>
                 <br />
            <br />
              <asp:TextBox ID="tbNewUsername" runat="server" SkinID="required" Width="150px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                ControlToValidate="tbNewUsername" ErrorMessage="*" ValidationGroup="reg" 
                         CssClass="errors"></asp:RequiredFieldValidator>
            <br />
              <asp:TextBox ID="tbNewUserPassword" runat="server" SkinID="required" 
                         style="margin:3px 0px;" TextMode="Password" Width="150px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                ControlToValidate="tbNewUserPassword" ErrorMessage="*" ValidationGroup="reg" 
                         CssClass="errors"></asp:RequiredFieldValidator>
                 </div>
                </div>

        
           
            <asp:Button ID="btnRegCorpAndUser" runat="server" style="margin-left:50px;"
                onclick="btnRegCorpAndUser_Click" Text="Register" ValidationGroup="reg" />
            &nbsp;<asp:Button ID="btnCancelReg" runat="server" onclick="btnCancelReg_Click" 
                Text="Cancel" SkinID="cancelBtn" />
            <asp:Label ID="lblRegCorpErrors" runat="server" Text="Label" Visible="False" 
                 CssClass="errors"></asp:Label>
            <br />
        </asp:Panel>

        <asp:Panel ID="pnlLinksS" runat="server" Visible="False" style="padding-left:8px;">

             <asp:HyperLink ID="hlFeedBackS" runat="server" NavigateUrl="~/FeedBack.aspx" CssClass="indexLinks" >
            <div class="clearfix2 indexLinkImgOnly" style="">
                <asp:Image ID="Image5" runat="server" ImageUrl="~/images/feedback.png" ImageAlign="Left" BorderWidth="0"/> 
            </div>
            </asp:HyperLink>

             <asp:HyperLink ID="hlAboutS" runat="server" NavigateUrl="~/About.aspx" CssClass="indexLinks" >
            <div class="clearfix2 indexLinkImgOnly" style="">
                <asp:Image ID="Image6" runat="server" ImageUrl="~/images/about.png" ImageAlign="Left" BorderWidth="0"/> 
            </div>
            </asp:HyperLink>

            </asp:Panel>



    </div>
    </div>

       <div class="statisticsPnl">
            <asp:Label ID="lblStaticIntefaceLang" runat="server" Text="Интерфейс език:"></asp:Label>
            &nbsp;<asp:ImageButton ID="ibLangEnglish" runat="server" 
                ImageUrl="~/images/english.gif" onclick="ibLangEnglish_Click" />
&nbsp;<asp:ImageButton ID="ibLangBulgarian" runat="server" ImageUrl="~/images/bulgaria.gif" 
                onclick="ibLangBulgarian_Click" />&nbsp;&nbsp;
            <asp:Label ID="lblStaticVisits" runat="server" Text="Visits"></asp:Label>
            <asp:Label ID="lblVisitsCount" runat="server" Text="0"></asp:Label>
&nbsp;&nbsp;
            <asp:Label ID="lblStaticCorporations" runat="server" Text="Corporations"></asp:Label>
            <asp:Label ID="lblCorporationsCount" runat="server" Text="0"></asp:Label>
&nbsp;&nbsp;
            <asp:Label ID="lblStaticUsers" runat="server" Text="Users"></asp:Label>
            <asp:Label ID="lblUsersCount" runat="server" Text="0"></asp:Label>
        </div> 


          <asp:Panel ID="pnlLogOut" runat="server" Visible="False" CssClass="clearfix2 indexLinkPnl">
            <asp:Image ID="Image7" runat="server" ImageUrl="~/images/log out.png" ImageAlign="Left" style="margin:5px 0px;" BorderWidth="0" />  
            <div class="linkTextDiv">
                    <asp:Label ID="lblStaticLogOut" runat="server" Text="Log out"></asp:Label>
            </div>
         </asp:Panel>

        <asp:Panel ID="pnlRegCorpLink" runat="server" Visible="False" CssClass="clearfix2 indexLinkPnl">
            <asp:Image ID="Image4" runat="server" ImageUrl="~/images/regcorp.png" ImageAlign="Left" style="margin:5px 0px;" BorderWidth="0" />  
            <div class="linkTextDiv">
                    <asp:Label ID="lblStaticRegCorpLink" runat="server" Text="Reg corp"></asp:Label>
            </div>
        </asp:Panel>

         <asp:Panel ID="pnlGuestLogIn" runat="server" Visible="False" CssClass="clearfix2 indexLinkPnl">
            <asp:Image ID="Image9" runat="server" ImageUrl="~/images/guest.png" ImageAlign="Left" style="margin:5px 0px;" BorderWidth="0" />  
            <div class="linkTextDiv">
                    <asp:Label ID="lblStaticGuestLogIn" runat="server" Text="Guest log in"></asp:Label>
            </div>
        </asp:Panel>

    
    </div>
    </div>
    </form>
</body>
</html>
