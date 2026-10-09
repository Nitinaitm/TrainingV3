<%@ Page Title="Finance - Training Costing" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceCosting.aspx.cs" Inherits="Training.Admin.FinanceCosting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<style>
body{background:#f5f5f5;}
.main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin-top:20px;margin-bottom:20px;}
.page-heading{font-size:28px;font-weight:bold;color:darkcyan;margin-bottom:20px;}
.form-control,.form-select{height:38px!important;border:1px solid #ced4da!important;border-radius:4px;}
.btn-save{background:darkcyan!important;color:#fff!important;border:1px solid darkcyan!important;border-radius:4px;padding:7px 18px;font-weight:600;}
.btn-save:hover{background:teal!important;border-color:teal!important;color:#fff!important;}
.finance-grid .btn{white-space:nowrap;min-width:78px;padding:5px 10px;font-size:13px;display:inline-block;}
.finance-grid td:last-child{white-space:nowrap;}
.select2-container{width:100%!important;}
.select2-container--default .select2-selection--single{height:38px!important;border:1px solid #ced4da!important;border-radius:4px!important;}
.select2-selection__rendered{line-height:36px!important;}
.select2-selection__arrow{height:36px!important;}
.summary-label{font-weight:600;margin-bottom:5px;display:block;color:#343a40;}
.summary-value{min-height:38px;height:38px;padding:7px 12px;background:#f8f9fa;border:1px solid #ced4da;border-radius:4px;display:block;}
.total-value{font-weight:700;color:#198754;font-size:18px;}
.report-button{min-width:130px;white-space:nowrap;}
.detail-actions .btn{min-width:130px;white-space:nowrap;}
@media(max-width:768px){.main-card{padding:15px;}.page-heading{font-size:24px;}}
</style>

</asp:Content><asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="main-card">
<div class="page-heading">Finance - Training Costing</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row g-2 align-items-end">
<div class="col-lg-6 col-md-12 mb-2"><label class="form-label">Training / Batch / Location</label><asp:DropDownList ID="ddlTraining" runat="server" ClientIDMode="Static" CssClass="form-select select2-searchable" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-2 col-md-4 col-6 mb-2"><label class="summary-label">Trainees</label><asp:Label ID="lblTrainees" runat="server" CssClass="summary-value"></asp:Label></div>
<div class="col-lg-2 col-md-4 col-6 mb-2"><label class="summary-label">Training Days</label><asp:Label ID="lblDays" runat="server" CssClass="summary-value"></asp:Label></div>
<div class="col-lg-2 col-md-4 col-12 mb-2"><label class="summary-label">Total Cost</label><div class="summary-value total-value"><asp:Label ID="lblTotal" runat="server" Text="Rs. 0.00"></asp:Label></div></div>
</div>
<div class="table-responsive">
<asp:GridView ID="gvCosting" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False" DataKeyNames="CostingDetailID" OnRowCommand="gvCosting_RowCommand">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField>
<asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" />
<asp:BoundField DataField="CostLevel" HeaderText="Level" />
<asp:BoundField DataField="CalculationMode" HeaderText="Mode" />
<asp:BoundField DataField="UnitType" HeaderText="Unit" />
<asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" />
<asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" />
<asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" />
<asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" />
<asp:BoundField DataField="OverrideAmount" HeaderText="Override" DataFormatString="{0:N2}" />
<asp:TemplateField HeaderText="Manual Override"><ItemTemplate><asp:TextBox ID="txtOverride" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("OverrideAmount") %>'></asp:TextBox><asp:Button ID="btnOverride" runat="server" Text="Save" CssClass="btn btn-sm btn-primary mt-1" CommandName="SaveOverride" CommandArgument="<%# Container.DataItemIndex %>" /></ItemTemplate></asp:TemplateField>
</Columns>
</asp:GridView>
</div>
<div class="row mt-4">
<div class="col-12"><div class="table-responsive">
<asp:GridView ID="gvCourse" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="CourseName" HeaderText="Course" /><asp:BoundField DataField="BatchCount" HeaderText="Batches" /><asp:BoundField DataField="CourseCost" HeaderText="Course Cost" DataFormatString="{0:N2}" /><asp:BoundField DataField="AverageBatchCost" HeaderText="Avg Batch Cost" DataFormatString="{0:N2}" /></Columns>
</asp:GridView>
</div></div>
</div>
</div>
</div>
</asp:Content>