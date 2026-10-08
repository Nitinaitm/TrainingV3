<%@ Page Title="Super Admin - Password Management" Language="C#" MasterPageFile="~/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="PasswordManagement.aspx.cs" Inherits="Training.SuperAdmin.PasswordManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.sa-page{padding:24px}.sa-card{background:#fff;border-radius:14px;padding:24px;box-shadow:0 3px 15px rgba(0,0,0,.07)}.sa-title{font-size:24px;font-weight:700;color:#1f2937}.sa-sub{color:#6b7280;margin:5px 0 20px}.sa-grid{display:grid;grid-template-columns:repeat(2,1fr);gap:18px;max-width:850px}.sa-label{font-weight:700;display:block;margin-bottom:6px}.sa-input{width:100%;padding:11px;border:1px solid #d1d5db;border-radius:8px}.sa-btn{border:0;border-radius:8px;padding:10px 20px;background:#0d6efd;color:#fff;font-weight:600}.sa-msg{display:block;margin-bottom:15px;font-weight:600}@media(max-width:700px){.sa-grid{grid-template-columns:1fr}.sa-page{padding:12px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="sa-page"><div class="sa-card">
<div class="sa-title"><i class="fas fa-key mr-2"></i>Password Management</div>
<div class="sa-sub">Change or reset the password of any system user.</div>
<asp:Label ID="lblMessage" runat="server" CssClass="sa-msg"></asp:Label>
<div class="sa-grid">
<div><span class="sa-label">Login ID</span><asp:TextBox ID="txtLoginID" runat="server" CssClass="sa-input"></asp:TextBox></div>
<div><span class="sa-label">New Password</span><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="sa-input"></asp:TextBox></div>
<div><span class="sa-label">Confirm Password</span><asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="sa-input"></asp:TextBox></div>
</div><br />
<asp:Button ID="btnChange" runat="server" Text="Change Password" CssClass="sa-btn" OnClick="btnChange_Click" />
</div></div>
</asp:Content>