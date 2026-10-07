<%@ Page Title="Employee Training History" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="TrainingSearch.aspx.cs" Inherits="Training.Admin.TrainingSearch" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" />
<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
<style>
.emp-report{padding:22px;background:#f5f7fb}.card{background:#fff;border-radius:14px;box-shadow:0 3px 14px rgba(15,23,42,.08);padding:22px;margin-bottom:20px}.title{font-size:25px;font-weight:700;color:#17365d;margin-bottom:18px}.filter-grid{display:grid;grid-template-columns:repeat(4,minmax(190px,1fr));gap:15px}.field label{display:block;font-weight:600;color:#334155;margin-bottom:6px;font-size:14px}.textbox{width:100%;height:40px;padding:8px 11px;border:1px solid #cbd5e1;border-radius:6px;box-sizing:border-box}.multi{width:100%;min-height:40px}.select2-container{width:100%!important}.select2-container--default .select2-selection--multiple{min-height:40px!important;border:1px solid #cbd5e1!important;border-radius:6px!important;padding:2px 5px!important}.actions{display:flex;gap:10px;flex-wrap:wrap;margin-top:18px}.btnx{border:0;border-radius:7px;padding:9px 16px;font-weight:600}.blue{background:#2563eb;color:#fff}.gray{background:#64748b;color:#fff}.green{background:#198754;color:#fff}.table-wrap{overflow:auto}.grid{width:100%;min-width:1900px;border-collapse:collapse}.grid th{background:#17365d;color:#fff;padding:10px;white-space:nowrap;text-align:left}.grid td{padding:9px;border:1px solid #e2e8f0;white-space:nowrap}.grid tr:nth-child(even){background:#f8fafc}.count{font-weight:600;color:#475569;margin-bottom:10px}@media(max-width:1100px){.filter-grid{grid-template-columns:repeat(2,1fr)}}@media(max-width:650px){.filter-grid{grid-template-columns:1fr}.emp-report{padding:10px}.card{padding:14px}}
</style>
<script>
$(function(){
    $('.multi').select2({placeholder:'Select / search',allowClear:true,closeOnSelect:false});
});
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="emp-report">
<div class="card">
<div class="title">Employee-wise Training Report</div>
<div class="filter-grid">
<div class="field"><label>Emp ID</label><asp:TextBox ID="txtEmpID" runat="server" CssClass="textbox" /></div>
<div class="field"><label>Employee Name</label><asp:TextBox ID="txtEmpName" runat="server" CssClass="textbox" /></div>
<div class="field"><label>Mobile No</label><asp:TextBox ID="txtMobile" runat="server" CssClass="textbox" /></div>
<div class="field"><label>Email ID</label><asp:TextBox ID="txtEmail" runat="server" CssClass="textbox" /></div>
<div class="field"><label>Designation</label><asp:ListBox ID="lstDesignation" runat="server" CssClass="multi" SelectionMode="Multiple" /></div>
<div class="field"><label>Company</label><asp:ListBox ID="lstCompany" runat="server" CssClass="multi" SelectionMode="Multiple" /></div>
<div class="field"><label>Posting Place (HRMS)</label><asp:ListBox ID="lstPostingPlace" runat="server" CssClass="multi" SelectionMode="Multiple" /></div>
<div class="field"><label>Posting Details</label><asp:ListBox ID="lstPostingDetails" runat="server" CssClass="multi" SelectionMode="Multiple" /></div>
<div class="field"><label>Department / Office / Cell</label><asp:ListBox ID="lstDepartment" runat="server" CssClass="multi" SelectionMode="Multiple" /></div>
<div class="field"><label>Area Board / Zone</label><asp:ListBox ID="lstZone" runat="server" CssClass="multi" SelectionMode="Multiple" /></div>
<div class="field"><label>Circle</label><asp:ListBox ID="lstCircle" runat="server" CssClass="multi" SelectionMode="Multiple" /></div>
<div class="field"><label>Division</label><asp:ListBox ID="lstDivision" runat="server" CssClass="multi" SelectionMode="Multiple" /></div>
<div class="field"><label>Subdivision</label><asp:ListBox ID="lstSubdivision" runat="server" CssClass="multi" SelectionMode="Multiple" /></div>
<div class="field"><label>Section</label><asp:ListBox ID="lstSection" runat="server" CssClass="multi" SelectionMode="Multiple" /></div>
</div>
<div class="actions">
<asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btnx blue" OnClick="btnSearch_Click" />
<asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btnx gray" OnClick="btnReset_Click" />
<asp:Button ID="btnExport" runat="server" Text="Export Excel" CssClass="btnx green" OnClick="btnExport_Click" />
</div>
</div>
<div class="card">
<div class="count"><asp:Label ID="lblResultCount" runat="server" /></div>
<div class="table-wrap"><asp:GridView ID="gvTraining" runat="server" AutoGenerateColumns="True" CssClass="grid"><EmptyDataTemplate>No trainee training record found.</EmptyDataTemplate></asp:GridView></div>
</div>
</div>
</asp:Content>