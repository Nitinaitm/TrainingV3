<%@ Page Title="Super Admin - User Management" Language="C#" MasterPageFile="~/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="Training.SuperAdmin.UserManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.page-wrap{padding:20px}.card-box{background:#fff;border-radius:12px;padding:20px;margin-bottom:20px;box-shadow:0 2px 12px rgba(0,0,0,.08)}.title{font-size:24px;font-weight:600;margin-bottom:15px}.form-grid{display:grid;grid-template-columns:repeat(4,1fr);gap:12px}.form-grid label{font-weight:600;margin-bottom:5px}.form-grid .full{grid-column:1/-1}.form-control,.form-select{width:100%;padding:9px;border:1px solid #ced4da;border-radius:6px}.btn{padding:8px 16px;border:0;border-radius:6px;margin-right:6px}.btn-primary{background:#0d6efd;color:#fff}.btn-success{background:#198754;color:#fff}.btn-danger{background:#dc3545;color:#fff}.btn-secondary{background:#6c757d;color:#fff}.msg{display:block;margin:10px 0;font-weight:600}.grid{width:100%;border-collapse:collapse}.grid th{background:#0d6efd;color:#fff;padding:9px}.grid td{padding:8px;border-bottom:1px solid #ddd}.section-title{font-size:18px;font-weight:600;margin-bottom:12px}.table-wrap{overflow:auto}
@media(max-width:900px){.form-grid{grid-template-columns:repeat(2,1fr)}}@media(max-width:600px){.form-grid{grid-template-columns:1fr}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="page-wrap">
<div class="card-box">
<div class="title">Super Admin - User Management</div>
<asp:Label ID="lblMessage" runat="server" CssClass="msg"></asp:Label>
<div class="form-grid">
<div><label>Login ID</label><asp:TextBox ID="txtLoginID" runat="server" CssClass="form-control"></asp:TextBox></div>
<div><label>Role</label><asp:DropDownList ID="ddlRole" runat="server" CssClass="form-select"></asp:DropDownList></div>
<div><label>Corresponding ID</label><asp:TextBox ID="txtCorrespondingID" runat="server" CssClass="form-control"></asp:TextBox></div>
<div><label>Password</label><asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox></div>
<div><label>Active</label><asp:DropDownList ID="ddlActive" runat="server" CssClass="form-select"><asp:ListItem Value="Y">Yes</asp:ListItem><asp:ListItem Value="N">No</asp:ListItem></asp:DropDownList></div>
<div class="full">
<asp:Button ID="btnSave" runat="server" Text="Create / Update User" CssClass="btn btn-primary" OnClick="btnSave_Click" />
<asp:Button ID="btnPassword" runat="server" Text="Change Password" CssClass="btn btn-success" OnClick="btnPassword_Click" />
<asp:Button ID="btnDelete" runat="server" Text="Delete / Disable User" CssClass="btn btn-danger" OnClick="btnDelete_Click" />
<asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
</div>
</div>
</div>

<div class="card-box">
<div class="section-title">Users</div>
<div class="table-wrap">
<asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" CssClass="grid" DataKeyNames="LoginIDUserID" OnRowCommand="gvUsers_RowCommand">
<Columns>
<asp:BoundField DataField="LoginIDUserID" HeaderText="Login ID" />
<asp:BoundField DataField="Role" HeaderText="Role" />
<asp:BoundField DataField="CorrespondingEmpID" HeaderText="Corresponding ID" />
<asp:BoundField DataField="Active" HeaderText="Active" />
<asp:BoundField DataField="LastLogin" HeaderText="Last Login" />
<asp:TemplateField HeaderText="Action"><ItemTemplate><asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditUser" CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-primary">Edit</asp:LinkButton></ItemTemplate></asp:TemplateField>
</Columns>
</asp:GridView>
</div>
</div>

<div class="card-box">
<div class="section-title">Login History</div>
<div class="table-wrap">
<asp:GridView ID="gvLoginHistory" runat="server" AutoGenerateColumns="True" CssClass="grid"></asp:GridView>
</div>
</div>

<div class="card-box">
<div class="section-title">User Activity</div>
<div class="table-wrap">
<asp:GridView ID="gvActivity" runat="server" AutoGenerateColumns="True" CssClass="grid"></asp:GridView>
</div>
</div>
</div>
</asp:Content>