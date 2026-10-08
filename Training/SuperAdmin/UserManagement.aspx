<%@ Page Title="Super Admin - User Management" Language="C#" MasterPageFile="~/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="Training.SuperAdmin.UserManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.sa-page{padding:22px}.sa-card{background:#fff;border-radius:14px;padding:22px;box-shadow:0 3px 16px rgba(0,0,0,.07);margin-bottom:18px}.sa-title{font-size:25px;font-weight:700;color:#172033}.sa-sub{color:#6b7280;margin:5px 0 20px}.filter-row{display:grid;grid-template-columns:260px 1fr auto;gap:12px;align-items:end}.sa-label{font-weight:700;display:block;margin-bottom:6px}.sa-input{width:100%;padding:10px;border:1px solid #d1d5db;border-radius:8px}.sa-btn{border:0;border-radius:8px;padding:10px 16px;font-weight:700;color:#fff;cursor:pointer}.blue{background:#2563eb}.gray{background:#6b7280}.green{background:#198754}.red{background:#dc3545}.orange{background:#f59e0b}.msg{display:block;margin:12px 0;font-weight:700}.grid-wrap{overflow:auto}.grid{width:100%;border-collapse:collapse;min-width:820px}.grid th{background:#1f2937;color:#fff;padding:11px;text-align:left;white-space:nowrap}.grid td{padding:9px;border-bottom:1px solid #e5e7eb;vertical-align:middle}.grid tr:hover td{background:#f8fafc}.action{display:inline-block;padding:6px 10px;border-radius:6px;color:#fff!important;text-decoration:none;border:0;margin:2px;font-weight:600}.status-active{color:#198754;font-weight:700}.status-inactive{color:#dc3545;font-weight:700}.empty{padding:25px;text-align:center;color:#6b7280;background:#f8fafc;border-radius:8px}.note{font-size:13px;color:#6b7280;margin-top:10px}
@media(max-width:800px){.filter-row{grid-template-columns:1fr}.sa-page{padding:10px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="sa-page">
<div class="sa-card">
<div class="sa-title"><i class="fas fa-users-cog mr-2"></i>User Management</div>
<div class="sa-sub">Manage existing Login accounts. Select a role before loading users.</div>
<asp:Label ID="lblMessage" runat="server" CssClass="msg"></asp:Label>
<div class="filter-row">
<div><span class="sa-label">Role</span><asp:DropDownList ID="ddlRoleFilter" runat="server" CssClass="sa-input"></asp:DropDownList></div>
<div><span class="sa-label">Search Login ID / Corresponding ID</span><asp:TextBox ID="txtSearch" runat="server" CssClass="sa-input"></asp:TextBox></div>
<div><asp:Button ID="btnLoadUsers" runat="server" Text="Load Users" CssClass="sa-btn blue" OnClick="btnLoadUsers_Click" /></div>
</div>
<div class="note">Users are not loaded automatically. Only the selected role is fetched.</div>
</div>

<div class="sa-card">
<div class="sa-title" style="font-size:19px">Existing Users</div>
<div class="grid-wrap">
<asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" CssClass="grid" DataKeyNames="LoginIDUserID" EmptyDataText="No users found for the selected role." OnRowCommand="gvUsers_RowCommand">
<Columns>
<asp:BoundField DataField="LoginIDUserID" HeaderText="Login ID" />
<asp:BoundField DataField="Role" HeaderText="Role" />
<asp:BoundField DataField="CorrespondingEmpID" HeaderText="Corresponding ID" />
<asp:TemplateField HeaderText="Status"><ItemTemplate><asp:Label ID="lblStatus" runat="server" Text='<%# Convert.ToString(Eval("Active"))=="Y" ? "Active" : "Inactive" %>' CssClass='<%# Convert.ToString(Eval("Active"))=="Y" ? "status-active" : "status-inactive" %>' /></ItemTemplate></asp:TemplateField>
<asp:BoundField DataField="LastLogin" HeaderText="Last Login" DataFormatString="{0:dd-MM-yyyy HH:mm}" />
<asp:TemplateField HeaderText="Actions"><ItemTemplate>
<asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditUser" CommandArgument='<%# Eval("LoginIDUserID") %>' CssClass="action blue">Edit</asp:LinkButton>
<asp:LinkButton ID="lnkPassword" runat="server" CommandName="ChangePassword" CommandArgument='<%# Eval("LoginIDUserID") %>' CssClass="action green">Change Password</asp:LinkButton>
<asp:LinkButton ID="lnkStatus" runat="server" CommandName="ToggleStatus" CommandArgument='<%# Eval("LoginIDUserID") %>' CssClass="action orange"><%# Convert.ToString(Eval("Active"))=="Y" ? "Inactive" : "Activate" %></asp:LinkButton>
<asp:LinkButton ID="lnkDelete" runat="server" CommandName="DeleteUser" CommandArgument='<%# Eval("LoginIDUserID") %>' CssClass="action red" OnClientClick="return confirm('Delete this user account?');">Delete</asp:LinkButton>
</ItemTemplate></asp:TemplateField>
</Columns>
</asp:GridView>
</div>
</div>

<div class="sa-card" id="editCard" runat="server" visible="false">
<div class="sa-title" style="font-size:19px">Edit Existing User</div>
<div class="filter-row" style="grid-template-columns:1fr 1fr auto">
<div><span class="sa-label">Login ID</span><asp:TextBox ID="txtLoginID" runat="server" CssClass="sa-input" ReadOnly="true"></asp:TextBox></div>
<div><span class="sa-label">Role</span><asp:DropDownList ID="ddlRole" runat="server" CssClass="sa-input"></asp:DropDownList></div>
<div><span class="sa-label">Corresponding ID</span><asp:TextBox ID="txtCorrespondingID" runat="server" CssClass="sa-input"></asp:TextBox></div>
</div>
<br />
<div>
<span class="sa-label">Account Status</span>
<asp:DropDownList ID="ddlActive" runat="server" CssClass="sa-input" style="max-width:260px"><asp:ListItem Value="Y">Active</asp:ListItem><asp:ListItem Value="N">Inactive</asp:ListItem></asp:DropDownList>
</div><br />
<asp:Button ID="btnUpdate" runat="server" Text="Save Changes" CssClass="sa-btn blue" OnClick="btnUpdate_Click" />
<asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="sa-btn gray" OnClick="btnCancelEdit_Click" />
</div>
</div>
</asp:Content>