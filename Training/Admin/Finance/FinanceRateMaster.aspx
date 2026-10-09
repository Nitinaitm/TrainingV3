<%@ Page Title="Finance - Rate Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceRateMaster.aspx.cs" Inherits="Training.Admin.FinanceRateMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
body{background:#f5f5f5}.main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin:20px 0}.page-heading{font-size:28px;font-weight:bold;color:darkcyan;margin-bottom:4px}.page-subheading{color:#6c757d;display:block;margin-bottom:22px}.finance-section{border-top:1px solid #e5e5e5;padding-top:20px;margin-top:20px}.finance-section-title{font-size:20px;font-weight:bold;color:darkcyan;margin-bottom:18px}.form-label{font-weight:500;margin-bottom:6px;color:#343a40}.form-control,.form-select{height:38px;border:1px solid #ced4da;border-radius:4px}.btn-save{background:darkcyan;color:#fff;border:none}.btn-save:hover{background:teal;color:#fff}.finance-actions{margin-top:10px}.finance-table{margin-bottom:0!important}.finance-table th{background:darkcyan!important;color:#fff!important;white-space:nowrap;font-weight:600;padding:10px 12px}.finance-table td{vertical-align:middle;padding:9px 12px}.finance-table tbody tr:hover{background:#f8fbfb}.table-wrap{overflow-x:auto;border:1px solid #dee2e6;border-radius:6px}.message{display:block;margin-bottom:10px}@media(max-width:767px){.main-card{padding:18px}.page-heading{font-size:24px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card">
<div class="page-heading">Finance - Rate Master</div><span class="page-subheading">Maintain effective rates for each cost head</span>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold message"></asp:Label>
<div class="row">
<div class="col-lg-4 col-md-6 mb-3"><label class="form-label">Cost Head</label><asp:DropDownList ID="ddlCostHead" runat="server" CssClass="form-select"></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Rate</label><asp:TextBox ID="txtRate" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Effective From</label><asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Effective To</label><asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Remarks</label><asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" placeholder="Optional"></asp:TextBox></div>
</div>
<div class="finance-actions"><asp:Button ID="btnSave" runat="server" Text="Save Rate" CssClass="btn btn-save mr-2" OnClick="btnSave_Click" /><asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" /></div>
<div class="finance-section"><div class="finance-section-title"><i class="fas fa-history mr-2"></i>Rate History</div>
<div class="table-wrap"><asp:GridView ID="gvRates" runat="server" CssClass="table table-bordered table-hover mb-0 finance-table" AutoGenerateColumns="False">
<Columns><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="Rate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="EffectiveFrom" HeaderText="From" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="EffectiveTo" HeaderText="To" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="Active" HeaderText="Active" /></Columns>
</asp:GridView></div></div>
</div></div></asp:Content>