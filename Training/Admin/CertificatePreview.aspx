<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CertificatePreview.aspx.cs" Inherits="Training.Admin.CertificatePreview" MasterPageFile="~/AdminMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
body{background:#f3f5f7}
.cp-preview-page{padding:10px 0 30px}
.toolbar{text-align:center;margin-bottom:14px}
.certificate-wrap{width:100%;overflow:auto;padding:10px 10px 20px}
.certificate{position:relative;width:1123px;height:794px;margin:0 auto;background:#fff;background-repeat:no-repeat;background-position:center;background-size:100% 100%;border:1px solid #b8b8b8;box-shadow:0 4px 18px rgba(0,0,0,.18);overflow:hidden;box-sizing:border-box}
.cp-logo,.cp-header,.cp-title,.cp-body,.cp-footer,.cp-signature{position:absolute;box-sizing:border-box}
.cp-logo{width:90px;height:90px;object-fit:contain}
.cp-header{left:80px;right:80px;text-align:center;font-weight:bold;line-height:1.25;white-space:normal}
.cp-title{left:60px;right:60px;text-align:center;font-weight:bold;line-height:1.2}
.cp-body{left:90px;right:90px;text-align:center;line-height:1.25}
.cp-body-intro,.cp-body-completion,.cp-body-duration{display:block}
.cp-trainee-name{font-size:30px;font-weight:bold;display:block;margin:16px 0 16px;line-height:1.15}
.cp-course-title{font-size:24px;font-weight:bold;display:block;margin:16px 0 12px;line-height:1.2}
.cp-duration{display:block;margin-top:10px}
.cp-signature{top:auto;width:220px;text-align:center}
.cp-signature img{width:180px;height:70px;object-fit:contain;display:block;margin:0 auto}
.cp-sign-name{font-weight:bold;margin-top:5px;line-height:1.2}
.cp-sign-designation{font-size:15px;line-height:1.2;margin-top:3px}
.cp-footer{left:60px;right:60px;text-align:center;line-height:1.2;white-space:normal}
@media(max-width:1200px){
.certificate{transform-origin:top center}
}
@media print{
body{background:#fff}
.cp-preview-page{padding:0}
.toolbar{display:none}
.certificate-wrap{padding:0;overflow:visible}
.certificate{border:none;box-shadow:none;margin:0;width:1123px;height:794px}
}
</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid preview-page">
    <div class="cp-toolbar cp-toolbar-bottom">
        <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn btn-secondary" OnClick="btnNext_Click" />
    </div>

    <div class="cp-certificate-wrap">
        <div id="divCertificate" runat="server" class="cp-certificate">
            <asp:Image ID="imgLogo" runat="server" CssClass="logo" />

            <asp:Label ID="lblHeader" runat="server" CssClass="header" />

            <asp:Label ID="lblTitle" runat="server" CssClass="title" />

            <div id="divBody" runat="server" class="cp-body">
                <asp:Label ID="lblBodyIntro" runat="server" CssClass="body-intro">
                    This Certificate is proudly presented to
                </asp:Label>

                <asp:Label ID="lblEmployee" runat="server" CssClass="trainee-name" />

                <asp:Label ID="lblBodyCompletion" runat="server" CssClass="body-completion">
                    for successfully completing
                </asp:Label>

                <asp:Label ID="lblCourse" runat="server" CssClass="course-title" />

                <asp:Label ID="lblDuration" runat="server" CssClass="duration" />
            </div>

            <div id="divLeftSignature" runat="server" class="cp-signature">
                <asp:Image ID="imgLeftSignature" runat="server" />
                <div class="cp-sign-name">
                    <asp:Label ID="lblLeftName" runat="server" />
                </div>
                <div class="cp-sign-designation">
                    <asp:Label ID="lblLeftDesignation" runat="server" />
                </div>
            </div>

            <div id="divRightSignature" runat="server" class="cp-signature">
                <asp:Image ID="imgRightSignature" runat="server" />
                <div class="cp-sign-name">
                    <asp:Label ID="lblRightName" runat="server" />
                </div>
                <div class="cp-sign-designation">
                    <asp:Label ID="lblRightDesignation" runat="server" />
                </div>
            </div>

            <asp:Label ID="lblFooter" runat="server" CssClass="footer" />
        </div>
    </div>
</div>
</asp:Content>
