<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WhSpace.About" Theme="Skins" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit.HTMLEditor" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link href="css/styles.css" rel="stylesheet" type="text/css" />

    <link rel="icon" href="images/favicon.ico" type="image/x-icon" /> 
    <link rel="shortcut icon" href="images/favicon.ico" type="image/x-icon" />

</head>
<body class="aboutPageBody">
    <form id="form1" runat="server">
    <div style="text-align:center;">
    <div style="width:900px;margin:0 auto; text-align:left;">
    
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    
        
        <asp:Panel ID="pnlLinksS" runat="server" style="margin:10px 0px;">

         <asp:HyperLink ID="hlIndexS" runat="server" NavigateUrl="~/Index.aspx" CssClass="indexLinks" >
            <div class="clearfix2 indexLinkImgOnly" style="">
                <asp:Image ID="Image7" runat="server" ImageUrl="~/images/index.png" ImageAlign="Left" BorderWidth="0"/> 
            </div>
          </asp:HyperLink>

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

            <asp:HyperLink ID="hlCorporationS" runat="server" NavigateUrl="~/Main.aspx" CssClass="indexLinks" >
            <div class="clearfix2 indexLinkImgOnly" style="">
                <asp:Image ID="Image8" runat="server" ImageUrl="~/images/corporation.png" ImageAlign="Left" BorderWidth="0"/> 
            </div>
            </asp:HyperLink>

            <asp:HyperLink ID="hlGAdminS" runat="server" NavigateUrl="~/GAdmin.aspx" CssClass="indexLinks" >
            <div class="clearfix2 indexLinkImgOnly" style="">
                <asp:Image ID="Image10" runat="server" ImageUrl="~/images/gadmin.png" ImageAlign="Left" BorderWidth="0"/> 
            </div>
            </asp:HyperLink>

            <asp:LinkButton ID="lbLogOut" runat="server" CssClass="indexLinks" 
                onclick="lbLogOut_Click"></asp:LinkButton>
          <asp:Panel ID="pnlLogOut" runat="server" CssClass="clearfix2 indexLinkImgOnly" Visible="false">
            <asp:Image ID="Image9" runat="server" ImageUrl="~/images/log out.png" ImageAlign="Left" BorderWidth="0"/> 
            </asp:Panel>

              <asp:LinkButton ID="lbEditAdminText" runat="server" CssClass="indexLinks" onclick="lbEditAdminText_Click"></asp:LinkButton>
                  <asp:Panel ID="pnlEditAdminTextLink" runat="server" CssClass="clearfix2 indexLinkImgOnly" Visible="false">
            <asp:Image ID="Image11" runat="server" ImageUrl="~/images/editL.png" ImageAlign="Left" BorderWidth="0"/> 
            </asp:Panel>


            </asp:Panel>
        

        <br />


        <asp:Panel ID="pnlEditAdminAboutText" runat="server" Visible="False" CssClass="editFeedBackPnl"
            DefaultButton="btnEditAdminText">
        
        <div style="margin:5px 0px 10px 0px; text-align:center;">
            <cc1:Editor ID="edEditAdminText" runat="server" BackColor="White" style="display:inline-block; margin:0px 5px;"
                Height="200px" NoScript="True" />
                </div>
          
            <asp:Button ID="btnEditAdminText" runat="server" style="margin-left:5px;"
                Text="Edit" SkinID="editBtn" onclick="btnEditAdminText_Click" />
            &nbsp;<asp:Button ID="btnCancelEditAdminText" runat="server" 
                Text="Cancel" SkinID="cancelBtn" onclick="btnCancelEditAdminText_Click" />
               &nbsp;<asp:Label ID="lblStaticEditAdminTextError" runat="server" Text="error" 
                Visible="False" CssClass="errors"></asp:Label>
        </asp:Panel>




        <div class="clearfix2 aboutContPnl" style="width:880px;">
        <div style="float:left;width:640px;">
        
        <div class="aboutText">
            <asp:Label ID="lblAboutText" runat="server" Text="text"></asp:Label>
            <asp:Panel ID="pnlAdminTextLastModified" runat="server" Visible="False" CssClass="adminFeedBackLastModifPnl">
            <asp:Label ID="lblStaticAdminLastModified" runat="server" Text="Last Modified"></asp:Label>
                <asp:Label ID="lblStaticAdminLastModifiedDate" runat="server" Text="Date"></asp:Label>
                <asp:Label ID="lblStaticAdminLastModifiedBy" runat="server" Text="by"></asp:Label>
                <asp:Label ID="lblStaticAdminLastModifiedByUser" runat="server" Text="User" CssClass="users"></asp:Label>
            </asp:Panel>
        </div>

        </div>
        <div style="margin-left:650px;">
        <div class="aboutImages">
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/images/screens/screen1l.png" CssClass="indexLinks" style="" Target="_blank">
            <asp:Image ID="Image1" runat="server"  ImageUrl="~/images/screens/screen1s.png" Width="220px" style="margin:0px 0px 10px 0px; border-radius:10px; -webkit-border-radius: 10px;" />
            </asp:HyperLink>

            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/images/screens/screen2l.png" CssClass="indexLinks" Target="_blank">
            <asp:Image ID="Image2" runat="server"  ImageUrl="~/images/screens/screen2s.png" Width="220px" style="margin:10px 0px; border-radius:10px; -webkit-border-radius: 10px;" />
            </asp:HyperLink>

            <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/images/screens/screen3l.png" CssClass="indexLinks" Target="_blank">
            <asp:Image ID="Image3" runat="server"  ImageUrl="~/images/screens/screen3s.png" Width="220px" style="margin:10px 0px; border-radius:10px; -webkit-border-radius: 10px;" />
            </asp:HyperLink>

            <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/images/screens/screen4l.png" CssClass="indexLinks" Target="_blank">
            <asp:Image ID="Image4" runat="server"  ImageUrl="~/images/screens/screen4s.png" Width="220px" style="margin:10px 0px; border-radius:10px; -webkit-border-radius: 10px;" />
            </asp:HyperLink>



        </div>
        </div>
        </div>

    
    </div>
    </div>
    </form>
</body>
</html>
