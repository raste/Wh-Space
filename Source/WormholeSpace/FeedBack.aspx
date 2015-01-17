<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FeedBack.aspx.cs" Inherits="WhSpace.FeedBackPage" Theme="Skins" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit.HTMLEditor" tagprefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
    <link href="css/styles.css" rel="stylesheet" type="text/css" />

    <link rel="icon" href="images/favicon.ico" type="image/x-icon" /> 
    <link rel="shortcut icon" href="images/favicon.ico" type="image/x-icon" />

</head>
<body class="feedBackLogIn">
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
            <asp:Image ID="Image1" runat="server" ImageUrl="~/images/editL.png" ImageAlign="Left" BorderWidth="0"/> 
            </asp:Panel>


            </asp:Panel>


        <asp:Panel ID="pnlAdminFeedback" runat="server" CssClass="adminFeedbackpnl">
         
            <asp:Label ID="lblStaticAdminFeedback" runat="server" Text="Label"></asp:Label>

            <asp:Panel ID="pnlAdminFeedLastModified" runat="server" Visible="False" CssClass="adminFeedBackLastModifPnl">
            <asp:Label ID="lblStaticAdminLastModified" runat="server" Text="Last Modified"></asp:Label>
                <asp:Label ID="lblStaticAdminLastModifiedDate" runat="server" Text="Date"></asp:Label>
                <asp:Label ID="lblStaticAdminLastModifiedBy" runat="server" Text="by"></asp:Label>
                <asp:Label ID="lblStaticAdminLastModifiedByUser" runat="server" Text="User" CssClass="users"></asp:Label>
            </asp:Panel>
            
        </asp:Panel>

        <asp:Panel ID="pnlStatus" CssClass="feedbackStatusPnl"
            runat="server" Direction="NotSet" Visible="False">
        
         <asp:Label ID="lblStaticStatus" style="color:#C8C8C8;" runat="server" Text="Статус :"></asp:Label>
         <asp:Label ID="lblStatus" runat="server"></asp:Label>

        </asp:Panel>

       

          <br />
          <div style="text-align:right;">

              <asp:Button ID="btnShowAddFeedback" runat="server" Text="Add" 
                  onclick="btnShowAddFeedback_Click" Visible="False" />&nbsp;<asp:Button ID="btnShowEditFeedback" runat="server" Text="Edit" 
            Visible="False" onclick="btnShowEditFeedback_Click" SkinID="editBtn" />
        &nbsp;<asp:Button ID="btnDeleteFeedback" runat="server" 
            onclick="btnDeleteFeedback_Click" Text="Delete" SkinID="delBtn" />
            </div>

    

           <asp:Panel ID="pnlEditAdminFeedback" runat="server" Visible="False" CssClass="editFeedBackPnl"
            DefaultButton="btnEditAdminFeedback">
        
        <div style="margin:5px 0px 10px 0px; text-align:center;">
            <cc1:Editor ID="edEditAdminFeedback" runat="server" BackColor="White" style="display:inline-block; margin:0px 5px;"
                Height="200px" NoScript="True" />
                </div>
          
            <asp:Button ID="btnEditAdminFeedback" runat="server" style="margin-left:5px;"
                onclick="btnEditAdminFeedback_Click" Text="Edit" SkinID="editBtn" />
            &nbsp;<asp:Button ID="btnCancelEditAdminFeedback" runat="server" 
                onclick="btnCancelEditAdminFeedback_Click" Text="Cancel" SkinID="cancelBtn" />
               &nbsp;<asp:Label ID="lblStaticEditAdminFeedbackError" runat="server" Text="error" 
                Visible="False" CssClass="errors"></asp:Label>
        </asp:Panel>

      

        <asp:Panel ID="pnlAddFeedback" runat="server" Visible="False" CssClass="editFeedBackPnl"
            DefaultButton="btnAddFeedback">
            
           <div class="headerBlackText" style="margin:5px 0px 10px 0px;">
            <asp:Label ID="lblStaticAddFeedback" runat="server" Text="Add feedback"></asp:Label>
            </div>
           
            <div class="clearfix2" style="">
                <div style="float:left;width:180px; text-align:right;">

                <asp:Label ID="lblStaticAddFeedbackName" runat="server" Text="Name"></asp:Label>
                <br />
                <br />
                <br />
                <asp:Label ID="lblStaticAddFeedbackDescr" runat="server" Text="Description"></asp:Label>

                </div>
                <div style="margin-left:185px;">

                <asp:TextBox ID="tbAddFeedbackName" runat="server" Width="300px"></asp:TextBox>
                <br />
                 <asp:TextBox ID="tbAddFeedbackDescr" runat="server" TextMode="MultiLine" Height="80px" Width="500px" style="margin:5px 0px 10px 0px;"
                SkinID="required"></asp:TextBox>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                        ControlToValidate="tbAddFeedbackDescr" ValidationGroup="add"></asp:RequiredFieldValidator>

                </div>
            </div>

            <asp:Button ID="btnAddFeedback" runat="server" Text="Add" style="margin-left:185px;"
                onclick="btnAddFeedback_Click" ValidationGroup="add" />
            &nbsp;<asp:Button ID="btnCancelAddFeedback" runat="server" 
                onclick="btnCancelAddFeedback_Click" SkinID="cancelBtn" Text="Cancel" />
            &nbsp;<asp:Label ID="lblStaticAddFeedbackError" runat="server" Text="Label" 
                Visible="False" CssClass="errors"></asp:Label>
            <br />
        </asp:Panel>
        


        <asp:Panel ID="pnlEditFeedback" runat="server" Visible="False" CssClass="editFeedBackPnl"
            DefaultButton="btnEditFeedback">
          
          <div class="headerBlackText"  style="margin:5px 0px 10px 0px;">
            <asp:Label ID="lblStaticEditFeedback" runat="server" Text="Edit feedback"></asp:Label>
            </div>

            <div class="clearfix2">
                <div style="float:left;width:180px; text-align:right;">

                <asp:Label ID="lblStaticEditFeedbackName" runat="server" Text="Name"></asp:Label>
                <br />
                <br />
                <br />
                <asp:Label ID="lblStaticEditFeedbackDescr" runat="server" Text="Description"></asp:Label>

                </div>
                <div style="margin-left:185px;">

                <asp:TextBox ID="tbEditFeedbackName" runat="server" Width="300px"></asp:TextBox>
                <br />
                <asp:TextBox ID="tbEditFeedbackDescr" runat="server" TextMode="MultiLine" style="margin:5px 0px 10px 0px;"
                SkinID="required" Height="80px" Width="500px"></asp:TextBox>

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        ControlToValidate="tbEditFeedbackDescr" ValidationGroup="edit"></asp:RequiredFieldValidator>

                </div>
            </div>

            <asp:Button ID="btnEditFeedback" runat="server" Text="Edit" style="margin-left:185px;"
                onclick="btnEditFeedback_Click" SkinID="editBtn" ValidationGroup="edit" />
            &nbsp;<asp:Button ID="btnCancelEditFeedback" runat="server" 
                onclick="btnCancelEditFeedback_Click" Text="Cancel" SkinID="cancelBtn" />
            <asp:Label ID="lblStaticEditFeedbackError" runat="server" Text="Label" 
                Visible="False" CssClass="errors"></asp:Label>
          
        </asp:Panel>
        <br />

        <div class="userFeedbackContPnl">
        <asp:PlaceHolder ID="phPagesTop" runat="server"></asp:PlaceHolder>
        <asp:PlaceHolder ID="phUserFeedback" runat="server"></asp:PlaceHolder>
        <asp:PlaceHolder ID="phPagesBottom" runat="server"></asp:PlaceHolder>

   

    </div>
    
    </div>
    </div>
    </form>
</body>
</html>
