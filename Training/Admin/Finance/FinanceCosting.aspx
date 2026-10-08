<%@ Page Title="Finance - Training Costing" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceCosting.aspx.cs" Inherits="Training.Admin.FinanceCosting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>.finance-page .page-title{font-weight:600;color:#343a40}.finance-page .card{border:0;box-shadow:0 2px 10px rgba(0,0,0,.08)}.finance-page .card-header{font-weight:600;background:#f8f9fa;border-bottom:1px solid #e9ecef}.finance-page .form-group label,.finance-page .info-label{font-weight:600;color:#495057;font-size:14px}.finance-page .total-box{font-size:20px;font-weight:700;color:#0d6efd;background:#f8f9fa;border:1px solid #dee2e6;border-radius:.25rem;padding:.375rem .75rem;min-height:38px}.finance-page .table th{background:#343a40;color:#fff;white-space:nowrap}.finance-page .table td{vertical-align:middle}.finance-page .table tbody tr:hover{background:#f8f9fa}.finance-page .message{display:block;margin-bottom:15px}</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid finance-page">
<div class="mb-3"><h3 class="page-title mb-1">Finance - Training Costing</h3><small class="text-muted">Calculate and review batch-wise training cost</small></div>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold message"></asp:Label>
<div class="card shadow-sm mb-4"><div class="card-header"><i class="fas fa-calculator mr-2"></i>Training / Batch Selection</div><div class="card-body"><div class="row">
<div class="col-lg-5 col-md-12 form-group"><label>Training / Batch</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-2 col-md-4 col-6 form-group"><label>Trainees</label><asp:Label ID="lblTrainees" runat="server" CssClass="form-control bg-light"></asp:Label></div>
<div class="col-lg-2 col-md-4 col-6 form-group"><label>Training Days</label><asp:Label ID="lblDays" runat="server" CssClass="form-control bg-light"></asp:Label></div>
<div class="col-lg-3 col-md-4 form-group"><label>Total Cost</label><div class="total-box"><asp:Label ID="lblTotal" runat="server" Text="₹0.00"></asp:Label></div></div>
</div></div></div>
<div class="card shadow-sm"><div class="card-header"><i class="fas fa-file-invoice-dollar mr-2"></i>Batch Costing Details</div><div class="card-body p-0"><div class="table-responsive">
<asp:GridView ID="gvCosting" runat="server" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="False" DataKeyNames="CostingDetailID" OnRowCommand="gvCosting_RowCommand">
<Columns><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="CostLevel" HeaderText="Level" /><asp:BoundField DataField="CalculationMode" HeaderText="Mode" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" /><asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" /><asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" /><asp:BoundField DataField="OverrideAmount" HeaderText="Override" DataFormatString="{0:N2}" /><asp:TemplateField HeaderText="Manual Override"><ItemTemplate><asp:TextBox ID="txtOverride" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("OverrideAmount") %>'></asp:TextBox><asp:Button ID="btnOverride" runat="server" Text="Save" CssClass="btn btn-sm btn-primary mt-1" CommandName="SaveOverride" CommandArgument="<%# Container.DataItemIndex %>" /></ItemTemplate></asp:TemplateField></Columns>
</asp:GridView>
</div></div></div>
<div class="card shadow-sm mt-4"><div class="card-header"><i class="fas fa-chart-pie mr-2"></i>Course-wise Summary</div><div class="card-body p-0"><div class="table-responsive">
<asp:GridView ID="gvCourse" runat="server" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="False"><Columns><asp:BoundField DataField="CourseName" HeaderText="Course" /><asp:BoundField DataField="BatchCount" HeaderText="Batches" /><asp:BoundField DataField="CourseCost" HeaderText="Course Cost" DataFormatString="{0:N2}" /><asp:BoundField DataField="AverageBatchCost" HeaderText="Avg Batch Cost" DataFormatString="{0:N2}" /></Columns></asp:GridView>
</div></div></div>
</div>
</asp:Content>