<%@ Page Title="Role Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="RoleMaster.aspx.cs" Inherits="Training.Admin.RoleMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>.role-card{background:#fff;padding:24px;border-radius:10px;box-shadow:0 2px 12px rgba(0,0,0,.08);margin:22px auto;max-width:1050px}.role-title{font-size:24px;font-weight:700;margin-bottom:18px;color:#0f766e}.role-grid{width:100%;border-collapse:collapse}.role-grid th{background:#0f766e;color:#fff;padding:10px;text-align:left}.role-grid td{padding:10px;border-bottom:1px solid #e5e7eb}.msg{display:block;margin-top:10px;font-weight:600}</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container"><div class="role-card">
        <div class="role-title">Employee Role Master</div>
        <div class="row g-2 align-items-end">
            <div class="col-md-8"><label class="form-label">Role name</label><asp:TextBox ID="txtRoleName" runat="server" CssClass="form-control" MaxLength="150" /></div>
            <div class="col-md-4"><asp:Button ID="btnSave" runat="server" Text="Add Role" CssClass="btn btn-success w-100" OnClick="btnSave_Click" /></div>
        </div>
        <asp:Label ID="lblMessage" runat="server" CssClass="msg" />
        <div class="table-responsive mt-4"><asp:GridView ID="gvRoles" runat="server" AutoGenerateColumns="False" CssClass="role-grid" DataKeyNames="RoleID" OnRowCommand="gvRoles_RowCommand">
            <Columns>
                <asp:BoundField DataField="RoleID" HeaderText="ID" />
                <asp:BoundField DataField="RoleName" HeaderText="Role" />
                <asp:TemplateField HeaderText="Status"><ItemTemplate><%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %></ItemTemplate></asp:TemplateField>
                <asp:TemplateField HeaderText="Action"><ItemTemplate><asp:LinkButton ID="btnToggle" runat="server" CommandName="ToggleRole" CommandArgument='<%# Eval("RoleID") %>' Text='<%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>' OnClientClick="return confirm('Change this role status?');" /></ItemTemplate></asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>No roles have been added.</EmptyDataTemplate>
        </asp:GridView></div>
        <div class="text-muted mt-2">Inactive roles remain attached to existing employees and historical snapshots but are not offered for new selections.</div>
    </div></div>
</asp:Content>