<%@ Page Title="Finance - Training Costing" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceCosting.aspx.cs" Inherits="Training.Admin.FinanceCosting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.finance-page{padding:8px 0 28px}.finance-title{font-weight:700;color:#263238}.finance-subtitle{color:#78909c;font-size:13px}.finance-card{border:0;border-radius:12px;box-shadow:0 4px 16px rgba(0,0,0,.08);overflow:hidden}.finance-card .card-header{background:#f8fafc;border-bottom:1px solid #e9ecef;font-weight:600;color:#37474f}.finance-label{font-size:13px;font-weight:600;color:#546e7a;margin-bottom:6px}.finance-message{display:block;margin-bottom:12px}.finance-table{margin-bottom:0}.finance-table th{background:#37474f;color:#fff;border-color:#455a64!important;font-size:13px;white-space:nowrap}.finance-table td{vertical-align:middle;font-size:13px}.finance-table tbody tr:hover{background:#f5f9fc}.finance-kpi{border-radius:10px;padding:12px 16px;background:#f8fafc;border:1px solid #e9ecef}.money{font-weight:700;font-size:22px;color:#1565c0;min-height:38px;display:flex;align-items:center}.finance-select{height:38px}@media(max-width:767px){.finance-title{font-size:20px}.finance-table{min-width:1050px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid finance-page">
<div class="mb-3"><h3 class="finance-title mb-1">Finance - Training Costing</h3><div class="finance-subtitle">Batch-wise automatic costing with manual override</div></div>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold finance-message"></asp:Label>
<div class="card finance-card mb-4"><div class="card-header">Training Selection</div><div class="card-body"><div class="row">
<div class="col-lg-5 col-md-12 mb-3"><label class="finance-label">Training / Batch</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-control finance-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-2 col-md-4 col-6 mb-3"><label class="finance-label">Trainees</label><asp:Label ID="lblTrainees" runat="server" CssClass="form-control bg-light"></asp:Label></div>
<div class="col-lg-2 col-md-4 col-6 mb-3"><label class="finance-label">Training Days</label><asp:Label ID="lblDays" runat="server" CssClass="form-control bg-light"></asp:Label></div>
<div class="col-lg-3 col-md-4 mb-3"><label class="finance-label">Total Cost</label><div class="finance-kpi money"><asp:Label ID="lblTotal" runat="server" Text="₹0.00"></asp:Label></div></div>
</div></div></div>
<div class="card finance-card"><div class="card-header">Batch Costing Details</div><div class="card-body p-0"><div class="table-responsive">
<asp:GridView ID="gvCosting" runat="server" CssClass="table table-bordered table-hover finance-table" AutoGenerateColumns="False" DataKeyNames="CostingDetailID">
<Columns><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="CostLevel" HeaderText="Level" /><asp:BoundField DataField="CalculationMode" HeaderText="Mode" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" /><asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" /><asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" /><asp:BoundField DataField="OverrideAmount" HeaderText="Override" DataFormatString="{0:N2}" /><asp:TemplateField HeaderText="Manual Override"><ItemTemplate><asp:TextBox ID="txtOverride" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("OverrideAmount") %>'></asp:TextBox><asp:Button ID="btnOverride" runat="server" Text="Save" CssClass="btn btn-sm btn-primary mt-1" CommandName="SaveOverride" CommandArgument="<%# Container.DataItemIndex %>" /></ItemTemplate></asp:TemplateField></Columns>
</asp:GridView>
</div></div></div>
<div class="card finance-card mt-4"><div class="card-header">Course-wise Summary</div><div class="card-body p-0"><div class="table-responsive">
<asp:GridView ID="gvCourse" runat="server" CssClass="table table-bordered table-hover finance-table" AutoGenerateColumns="False"><Columns><asp:BoundField DataField="CourseName" HeaderText="Course" /><asp:BoundField DataField="BatchCount" HeaderText="Batches" /><asp:BoundField DataField="CourseCost" HeaderText="Course Cost" DataFormatString="{0:N2}" /><asp:BoundField DataField="AverageBatchCost" HeaderText="Avg Batch Cost" DataFormatString="{0:N2}" /></Columns></asp:GridView>
</div></div></div>
</div>
</asp:Content>