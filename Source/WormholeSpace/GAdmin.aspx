<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GAdmin.aspx.cs" Inherits="WhSpace.GAdmin" Theme="Skins" %>

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

            </asp:Panel>

    
        <br />
        <asp:Label ID="lblStaticCorporations"  runat="server" Text="Corporations:"></asp:Label>
        <asp:PlaceHolder ID="phCorporations" runat="server"></asp:PlaceHolder>



    
    </div>
    </div>
    </form>
</body>
</html>
