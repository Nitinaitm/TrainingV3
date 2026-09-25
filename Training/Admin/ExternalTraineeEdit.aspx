<%@ Page Title="Edit External Trainee" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ExternalTraineeEdit.aspx.cs" Inherits="Training.Admin.ExternalTraineeEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
.ete-container{width:100%;padding:20px}.ete-card{background:#fff;border-radius:12px;padding:25px;max-width:900px;margin:0 auto;box-shadow:0 2px 12px rgba(0,0,0,.08)}.ete-title{font-size:26px;font-weight:700;color:#1e293b;margin-bottom:20px}.ete-grid{display:grid;grid-template-columns:repeat(2,1fr);gap:16px}.ete-group label{display:block;font-weight:600;color:#334155;font-size:14px;margin-bottom:7px}.ete-input{width:100%;height:40px;padding:8px 11px;border:1px solid #cbd5e1;border-radius:6px}.ete-readonly{background:#f1f5f9}.ete-buttons{margin-top:20px;display:flex;gap:10px}.ete-btn{border:0;border-radius:6px;padding:9px 20px;color:#fff;font-weight:600;cursor:pointer}.ete-save{background:#198754}.ete-cancel{background:#64748b}.ete-message{display:block;margin-top:15px;font-weight:600}@media(max-width:700px){.ete-grid{grid-template-columns:1fr}.ete-container{padding:10px}.ete-card{padding:15px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="ete-container"><div class="ete-card">
<div class="ete-title"><i class="fas fa-user-edit"></i> Edit External Trainee</div>
<div class="ete-grid">
<div class="ete-group"><label>Employee ID</label><asp:TextBox ID="txtEmpID" runat="server" CssClass="ete-input ete-readonly" ReadOnly="true"></asp:TextBox></div>
<div class="ete-group"><label>Name</label><asp:TextBox ID="txtEmpName" runat="server" CssClass="ete-input"></asp:TextBox></div>
<div class="ete-group"><label>Mobile No</label><asp:TextBox ID="txtMobileNo" runat="server" CssClass="ete-input"></asp:TextBox></div>
<div class="ete-group"><label>Email ID</label><asp:TextBox ID="txtEmailId" runat="server" CssClass="ete-input"></asp:TextBox></div>
<div class="ete-group"><label>Organization / Company</label><asp:TextBox ID="txtOrganization" runat="server" CssClass="ete-input"></asp:TextBox></div>
<div class="ete-group"><label>Designation</label><asp:TextBox ID="txtDesignation" runat="server" CssClass="ete-input"></asp:TextBox></div>
</div>
<div class="ete-buttons"><asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="ete-btn ete-save" OnClick="btnSave_Click" /><asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="ete-btn ete-cancel" OnClick="btnCancel_Click" /></div>
<asp:Label ID="lblMessage" runat="server" CssClass="ete-message"></asp:Label>
</div></div>
</asp:Content>