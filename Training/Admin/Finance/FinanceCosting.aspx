<%@ Page Title="Finance - Training Costing" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceCosting.aspx.cs" Inherits="Training.Admin.FinanceCosting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"><style>
body{background:#f5f5f5}
.main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin-top:20px;margin-bottom:20px}
.page-heading{font-size:28px;font-weight:bold;color:darkcyan;margin-bottom:20px}
.page-subheading{display:block;color:#6c757d;margin-top:-12px;margin-bottom:22px}
.form-label{font-weight:500;margin-bottom:6px;color:#343a40}
.form-control,.form-select{height:38px!important;border:1px solid #ced4da!important;border-radius:4px!important}
.validation{color:red;font-size:13px}
.btn-save{background:darkcyan;color:white;border:none}
.btn-save:hover{background:teal;color:white}
.section-heading{font-size:20px;font-weight:bold;color:darkcyan;border-bottom:2px solid #e5e5e5;padding-bottom:10px;margin-top:18px;margin-bottom:18px}
.table-box{width:100%;overflow-x:auto;border:1px solid #dee2e6;border-radius:8px}
.finance-table{margin-bottom:0!important;min-width:700px}
.finance-table th{background:darkcyan!important;color:#fff!important;white-space:nowrap;font-weight:600;padding:11px 12px;border-color:#0f7f80!important}
.finance-table td{vertical-align:middle;padding:10px 12px}
.finance-table tbody tr:hover{background:#f4fbfb}
.action-row{text-align:right;margin-top:8px}
.total-card{height:38px;border:1px solid #ced4da;border-radius:4px;background:#f8f9fa;display:flex;align-items:center;padding:0 12px;color:darkcyan;font-size:18px;font-weight:bold}
.info-box{background:#f8f9fa;border:1px solid #e5e5e5;border-radius:8px;padding:12px}
.message{display:block;margin-bottom:8px}
@media(max-width:767px){.main-card{padding:18px}.page-heading{font-size:24px}.action-row{text-align:center}.finance-table{min-width:850px}}
</style></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"><div class="container-fluid"><div class="main-card">
<div class="page-heading">Finance - Training Costing</div><span class="page-subheading">Calculate and review batch-wise training cost</span>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold message"></asp:Label>
<div class="row">
<div class="col-lg-5 col-md-12 mb-3"><label class="form-label">Training / Batch</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-2 col-md-4 col-6 mb-3"><label class="form-label">Trainees</label><asp:Label ID="lblTrainees" runat="server" CssClass="form-control bg-light"></asp:Label></div>
<div class="col-lg-2 col-md-4 col-6 mb-3"><label class="form-label">Training Days</label><asp:Label ID="lblDays" runat="server" CssClass="form-control bg-light"></asp:Label></div>
<div class="col-lg-3 col-md-4 mb-3"><label class="form-label">Total Cost</label><div class="total-card"><asp:Label ID="lblTotal" runat="server" Text="₹0.00"></asp:Label></div></div>
</div>
<div class="section-heading">Batch Costing Details</div>
<div class="table-box"><asp:GridView ID="gvCosting" runat="server" CssClass="table table-bordered table-hover mb-0 finance-table" AutoGenerateColumns="False" DataKeyNames="CostingDetailID" OnRowCommand="gvCosting_RowCommand">
<Columns><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="CostLevel" HeaderText="Level" /><asp:BoundField DataField="CalculationMode" HeaderText="Mode" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" /><asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" /><asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" /><asp:BoundField DataField="OverrideAmount" HeaderText="Override" DataFormatString="{0:N2}" /><asp:TemplateField HeaderText="Manual Override"><ItemTemplate><asp:TextBox ID="txtOverride" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("OverrideAmount") %>'></asp:TextBox><asp:Button ID="btnOverride" runat="server" Text="Save" CssClass="btn btn-sm btn-primary mt-1" CommandName="SaveOverride" CommandArgument="<%# Container.DataItemIndex %>" /></ItemTemplate></asp:TemplateField></Columns></asp:GridView></div>
<div class="section-heading">Course-wise Summary</div>
<div class="table-box"><asp:GridView ID="gvCourse" runat="server" CssClass="table table-bordered table-hover mb-0 finance-table" AutoGenerateColumns="False"><Columns><asp:BoundField DataField="CourseName" HeaderText="Course" /><asp:BoundField DataField="BatchCount" HeaderText="Batches" /><asp:BoundField DataField="CourseCost" HeaderText="Course Cost" DataFormatString="{0:N2}" /><asp:BoundField DataField="AverageBatchCost" HeaderText="Avg Batch Cost" DataFormatString="{0:N2}" /></Columns></asp:GridView></div>
</div></div></asp:Content>