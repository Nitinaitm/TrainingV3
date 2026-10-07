<%@ Page Title="Employee Training History" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="TrainingSearch.aspx.cs" Inherits="Training.Admin.TrainingSearch" EnableEventValidation="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.emp-wrap{padding:22px;background:#f5f7fb}.cardx{background:#fff;border-radius:14px;box-shadow:0 3px 14px rgba(15,23,42,.08);padding:22px;margin-bottom:20px}.title{font-size:25px;font-weight:700;color:#17365d;margin-bottom:18px}.filter-grid{display:grid;grid-template-columns:repeat(4,minmax(180px,1fr));gap:14px}.field label{display:block;font-weight:600;color:#334155;margin-bottom:6px}.input{width:100%;padding:9px 11px;border:1px solid #cbd5e1;border-radius:7px;box-sizing:border-box}.actions{display:flex;gap:10px;flex-wrap:wrap;margin-top:16px}.btnx{border:0;border-radius:7px;padding:9px 16px;font-weight:600;cursor:pointer}.blue{background:#2563eb;color:#fff}.gray{background:#64748b;color:#fff}.green{background:#198754;color:#fff}.table-wrap{overflow:auto}.grid{width:100%;min-width:1700px;border-collapse:collapse}.grid th{background:#17365d;color:#fff;padding:10px;text-align:left;white-space:nowrap}.grid td{padding:9px;border:1px solid #e2e8f0;white-space:nowrap}.grid tr:nth-child(even){background:#f8fafc}.hint{color:#64748b;font-weight:600;margin:8px 0}@media(max-width:1000px){.filter-grid{grid-template-columns:repeat(2,1fr)}}@media(max-width:600px){.filter-grid{grid-template-columns:1fr}.emp-wrap{padding:10px}.cardx{padding:14px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="emp-wrap">
<div class="cardx">
<div class="title">Employee-wise Training Report</div>
<div class="filter-grid">
<div class="field"><label>Emp ID</label><asp:TextBox ID="txtEmpID" runat="server" CssClass="input" /></div>
<div class="field"><label>Employee Name</label><asp:TextBox ID="txtEmpName" runat="server" CssClass="input" /></div>
<div class="field"><label>Designation</label><asp:TextBox ID="txtDesignation" runat="server" CssClass="input" /></div>
<div class="field"><label>Company</label><asp:TextBox ID="txtCompany" runat="server" CssClass="input" /></div>
<div class="field"><label>Posting Place (HRMS)</label><asp:TextBox ID="txtPostingPlace" runat="server" CssClass="input" /></div>
<div class="field"><label>Mobile No</label><asp:TextBox ID="txtMobile" runat="server" CssClass="input" /></div>
<div class="field"><label>Email ID</label><asp:TextBox ID="txtEmail" runat="server" CssClass="input" /></div>
<div class="field"><label>Posting Details</label><asp:TextBox ID="txtDetailPostingPlace" runat="server" CssClass="input" /></div>
<div class="field"><label>Department / Office / Cell</label><asp:TextBox ID="txtDepartment" runat="server" CssClass="input" /></div>
<div class="field"><label>Area Board / Zone</label><asp:TextBox ID="txtZone" runat="server" CssClass="input" /></div>
<div class="field"><label>Circle</label><asp:TextBox ID="txtCircle" runat="server" CssClass="input" /></div>
<div class="field"><label>Division</label><asp:TextBox ID="txtDivision" runat="server" CssClass="input" /></div>
<div class="field"><label>Subdivision</label><asp:TextBox ID="txtSubdivision" runat="server" CssClass="input" /></div>
<div class="field"><label>Section</label><asp:TextBox ID="txtSection" runat="server" CssClass="input" /></div>
</div>
<div class="actions">
<asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btnx blue" OnClick="btnSearch_Click" />
<asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btnx gray" OnClick="btnReset_Click" />
<asp:Button ID="btnExport" runat="server" Text="Export Excel" CssClass="btnx green" OnClick="btnExport_Click" />
</div>
<div class="hint"><asp:Label ID="lblResultCount" runat="server" /></div>
</div>
<div class="cardx">
<div class="table-wrap">
<asp:GridView ID="gvTraining" runat="server" AutoGenerateColumns="True" CssClass="grid"><EmptyDataTemplate><div class="hint">No trainee training record found for the selected filters.</div></EmptyDataTemplate></asp:GridView>
</div>
</div>
</div>
</asp:Content>