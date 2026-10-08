<%@ Page Title="Finance - Session Costing" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceSessionCosting.aspx.cs" Inherits="Training.Admin.FinanceSessionCosting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.finance-page{padding:8px 0 28px}.finance-title{font-weight:700;color:#263238}.finance-subtitle{color:#78909c;font-size:13px}.finance-card{border:0;border-radius:12px;box-shadow:0 4px 16px rgba(0,0,0,.08);overflow:hidden}.finance-card .card-header{background:#f8fafc;border-bottom:1px solid #e9ecef;font-weight:600;color:#37474f}.finance-label{font-size:13px;font-weight:600;color:#546e7a;margin-bottom:6px}.finance-message{display:block;margin-bottom:12px}.finance-table{margin-bottom:0}.finance-table th{background:#37474f;color:#fff;border-color:#455a64!important;font-size:13px;white-space:nowrap}.finance-table td{vertical-align:middle;font-size:13px}.finance-table tbody tr:hover{background:#f5f9fc}.finance-total{font-size:18px;font-weight:700;color:#1565c0;background:#f8fafc!important;min-height:38px;display:flex;align-items:center}@media(max-width:767px){.finance-title{font-size:20px}.finance-table{min-width:900px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid finance-page">
<div class="mb-3"><h3 class="finance-title mb-1">Finance - Session Costing</h3><div class="finance-subtitle">Session-wise trainer and other operational costing</div></div>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold finance-message"></asp:Label>
<div class="card finance-card mb-4"><div class="card-header">Session Selection</div><div class="card-body"><div class="row">
<div class="col-lg-5 col-md-6 mb-3"><label class="finance-label">Training / Batch</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-5 col-md-6 mb-3"><label class="finance-label">Session</label><asp:DropDownList ID="ddlSession" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSession_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-2 col-md-12 mb-3"><label class="finance-label">Session Cost</label><asp:Label ID="lblTotal" runat="server" CssClass="form-control finance-total">₹0.00</asp:Label></div>
</div></div></div>
<div class="card finance-card"><div class="card-header">Session Costing Details</div><div class="card-body p-0"><div class="table-responsive">
<asp:GridView ID="gvCosting" runat="server" CssClass="table table-bordered table-hover finance-table" AutoGenerateColumns="False" DataKeyNames="CostingDetailID" OnRowCommand="gvCosting_RowCommand">
<Columns><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" /><asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" /><asp:BoundField DataField="OverrideAmount" HeaderText="Override" DataFormatString="{0:N2}" /><asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" /><asp:TemplateField HeaderText="Manual Override"><ItemTemplate><asp:TextBox ID="txtOverride" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("OverrideAmount") %>'></asp:TextBox><asp:Button ID="btnOverride" runat="server" Text="Save" CssClass="btn btn-sm btn-primary mt-1" CommandName="SaveOverride" CommandArgument="<%# Container.DataItemIndex %>" /></ItemTemplate></asp:TemplateField></Columns>
</asp:GridView>
</div></div></div>
</div>
</asp:Content>