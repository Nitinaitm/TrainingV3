<%@ Page Title="Role Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="RoleMaster.aspx.cs" Inherits="Training.Admin.RoleMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>.role-wrap{padding:24px}.role-card{background:#fff;border-radius:12px;padding:22px;box-shadow:0 2px 10px #ddd;margin-bottom:20px}.role-grid{width:100%;border-collapse:collapse}.role-grid th,.role-grid td{padding:9px;border:1px solid #ddd}.role-grid th{background:#0f766e;color:#fff}</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="role-wrap">
<div class="role-card">
<h3>Role Master</h3>
<p>Add roles here. Deactivating a role preserves existing employee mappings and historical training snapshots.</p>
<div class="row g-3 align-items-end">
<div class="col-md-6"><label for="txtRoleName">Role Name</label><asp:TextBox ID="txtRoleName" runat="server" CssClass="form-control" MaxLength="150" /></div>
<div class="col-md-3"><asp:Button ID="btnSave" runat="server" Text="Add Role" CssClass="btn btn-success" OnClick="btnSave_Click" /></div>
</div>
<asp:Label ID="lblMessage" runat="server" />
</div>
<div class="role-card">
<asp:GridView ID="gvRoles" runat="server" AutoGenerateColumns="false" CssClass="role-grid" DataKeyNames="RoleID" OnRowCommand="gvRoles_RowCommand" EmptyDataText="No roles found.">
<Columns>
<asp:BoundField DataField="RoleID" HeaderText="ID" />
<asp:BoundField DataField="RoleName" HeaderText="Role" />
<asp:TemplateField HeaderText="Status"><ItemTemplate><%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %></ItemTemplate></asp:TemplateField>
<asp:TemplateField HeaderText="Action"><ItemTemplate><asp:LinkButton ID="lnkToggle" runat="server" CommandName="ToggleRole" CommandArgument='<%# Eval("RoleID") %>' Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>' OnClientClick="return confirm('Change this role status?');" /></ItemTemplate></asp:TemplateField>
</Columns>
</asp:GridView>
</div>
</div>
</asp:Content>