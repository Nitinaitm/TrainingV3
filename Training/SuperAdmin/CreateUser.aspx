<%@ Page Title="Super Admin - Create User" Language="C#" MasterPageFile="~/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="CreateUser.aspx.cs" Inherits="Training.SuperAdmin.CreateUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.sa-page{padding:22px}.sa-card{background:#fff;border-radius:14px;padding:24px;box-shadow:0 3px 16px rgba(0,0,0,.07);max-width:1000px}.sa-title{font-size:25px;font-weight:700;color:#172033}.sa-sub{color:#6b7280;margin:5px 0 22px}.grid{display:grid;grid-template-columns:repeat(2,1fr);gap:18px}.label{display:block;font-weight:700;margin-bottom:6px}.input{width:100%;padding:10px;border:1px solid #d1d5db;border-radius:8px}.msg{display:block;margin-bottom:15px;font-weight:700}.btn{border:0;border-radius:8px;padding:10px 18px;color:#fff;font-weight:700;margin-right:7px}.blue{background:#2563eb}.gray{background:#6b7280}.note{margin-top:18px;padding:12px;background:#f3f4f6;border-radius:8px;color:#4b5563;font-size:13px;line-height:1.6}@media(max-width:700px){.grid{grid-template-columns:1fr}.sa-page{padding:10px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="sa-page"><div class="sa-card">
<div class="sa-title"><i class="fas fa-user-plus mr-2"></i>Create User</div>
<div class="sa-sub">Create a new Login account and associate it with the appropriate system identity.</div>
<asp:Label ID="lblMessage" runat="server" CssClass="msg"></asp:Label>
<div class="grid">
<div><span class="label">Login ID</span><asp:TextBox ID="txtLoginID" runat="server" CssClass="input"></asp:TextBox></div>
<div><span class="label">Role</span><asp:DropDownList ID="ddlRole" runat="server" CssClass="input" AutoPostBack="false"></asp:DropDownList></div>
<div><span class="label">Corresponding ID</span><asp:TextBox ID="txtCorrespondingID" runat="server" CssClass="input"></asp:TextBox></div>
<div><span class="label">Password</span><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input"></asp:TextBox></div>
<div><span class="label">Account Status</span><asp:DropDownList ID="ddlActive" runat="server" CssClass="input"><asp:ListItem Value="Y">Active</asp:ListItem><asp:ListItem Value="N">Inactive</asp:ListItem></asp:DropDownList></div>
</div><br />
<asp:Button ID="btnCreate" runat="server" Text="Create User" CssClass="btn blue" OnClick="btnCreate_Click" />
<asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn gray" OnClick="btnClear_Click" />
<div class="note"><b>Note:</b> The Corresponding ID must already exist in the relevant master. A new user's first login will require password reset.</div>
</div></div>
</asp:Content>