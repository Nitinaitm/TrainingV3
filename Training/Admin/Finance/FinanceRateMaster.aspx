<%@ Page Title="Finance - Rate Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceRateMaster.aspx.cs" Inherits="Training.Admin.FinanceRateMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.finance-page{padding:8px 0 28px}.finance-title{font-weight:700;color:#263238}.finance-subtitle{color:#78909c;font-size:13px}.finance-card{border:0;border-radius:12px;box-shadow:0 4px 16px rgba(0,0,0,.08);overflow:hidden}.finance-card .card-header{background:#f8fafc;border-bottom:1px solid #e9ecef;font-weight:600;color:#37474f}.finance-label{font-size:13px;font-weight:600;color:#546e7a;margin-bottom:6px}.finance-btn{min-width:105px}.finance-message{display:block;margin-bottom:12px}.finance-table{margin-bottom:0}.finance-table th{background:#37474f;color:#fff;border-color:#455a64!important;font-size:13px;white-space:nowrap}.finance-table td{vertical-align:middle;font-size:13px}.finance-table tbody tr:hover{background:#f5f9fc}.finance-actions{padding-top:8px}@media(max-width:767px){.finance-title{font-size:20px}.finance-actions .btn{width:100%;margin:4px 0!important}.finance-table{min-width:700px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid finance-page">
<div class="mb-3"><h3 class="finance-title mb-1">Finance - Rate Master</h3><div class="finance-subtitle">Maintain effective finance rates for each cost head</div></div>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold finance-message"></asp:Label>
<div class="card finance-card mb-4"><div class="card-header">Rate Details</div><div class="card-body">
<div class="row">
<div class="col-lg-4 col-md-6 mb-3"><label class="finance-label">Cost Head</label><asp:DropDownList ID="ddlCostHead" runat="server" CssClass="form-control"></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="finance-label">Rate</label><asp:TextBox ID="txtRate" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="finance-label">Effective From</label><asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="finance-label">Effective To</label><asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="finance-label">Remarks</label><asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" placeholder="Optional"></asp:TextBox></div>
</div>
<div class="finance-actions"><asp:Button ID="btnSave" runat="server" Text="Save Rate" CssClass="btn btn-primary finance-btn mr-2" OnClick="btnSave_Click" /><asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-outline-secondary finance-btn" OnClick="btnClear_Click" /></div>
</div></div>
<div class="card finance-card"><div class="card-header">Rate History</div><div class="card-body p-0"><div class="table-responsive">
<asp:GridView ID="gvRates" runat="server" CssClass="table table-bordered table-hover finance-table" AutoGenerateColumns="False">
<Columns><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="Rate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="EffectiveFrom" HeaderText="From" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="EffectiveTo" HeaderText="To" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="Active" HeaderText="Active" /></Columns>
</asp:GridView>
</div></div></div>
</div>
</asp:Content>