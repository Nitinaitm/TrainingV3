<%@ Page Title="Finance - Actual Expenditure" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceActualExpenditure.aspx.cs" Inherits="Training.Admin.FinanceActualExpenditure" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
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
<script>$(document).ready(function(){if($("#ddlTraining").length){if($("#ddlTraining").hasClass("select2-hidden-accessible"))$("#ddlTraining").select2("destroy");$("#ddlTraining").select2({width:"100%",placeholder:"Search Training ID / Course / Batch / Location",allowClear:true});}if($("#ddlSession").length){if($("#ddlSession").hasClass("select2-hidden-accessible"))$("#ddlSession").select2("destroy");$("#ddlSession").select2({width:"100%",placeholder:"Search Session",allowClear:true});}if($("#ddlCostHead").length){if($("#ddlCostHead").hasClass("select2-hidden-accessible"))$("#ddlCostHead").select2("destroy");$("#ddlCostHead").select2({width:"100%",placeholder:"Search Cost Head",allowClear:true});}});</script>
</asp:Content><asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card">
<div class="page-heading">Finance - Actual Expenditure</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row">
<div class="col-lg-4 col-md-6 mb-3"><label class="form-label">Training / Batch / Location *</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Session</label><asp:DropDownList ID="ddlSession" runat="server" CssClass="form-select"></asp:DropDownList></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Cost Head</label><asp:DropDownList ID="ddlCostHead" runat="server" CssClass="form-select"></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Date *</label><asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Amount *</label><asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Vendor / Payee</label><asp:TextBox ID="txtVendor" runat="server" CssClass="form-control"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Bill / Reference No.</label><asp:TextBox ID="txtBill" runat="server" CssClass="form-control"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Remarks</label><asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox></div>
</div>
<div class="text-center mb-3"><asp:Button ID="btnSave" runat="server" Text="Save Expenditure" CssClass="btn btn-success" OnClick="btnSave_Click" />&nbsp;<asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" /></div>
<div class="row mb-3"><div class="col-md-4"><span class="summary-value">Final Cost: <asp:Label ID="lblFinalCost" runat="server" Text="Rs. 0.00"></asp:Label></span></div><div class="col-md-4"><span class="summary-value">Actual Paid: <asp:Label ID="lblActual" runat="server" Text="Rs. 0.00"></asp:Label></span></div><div class="col-md-4"><span class="summary-value">Balance: <asp:Label ID="lblBalance" runat="server" Text="Rs. 0.00"></asp:Label></span></div></div>
<div class="table-responsive"><asp:GridView ID="gvExpenditure" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False" DataKeyNames="ExpenditureID" OnRowCommand="gvExpenditure_RowCommand">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="ExpenditureDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="SessionName" HeaderText="Session" /><asp:BoundField DataField="VendorName" HeaderText="Vendor / Payee" /><asp:BoundField DataField="BillReference" HeaderText="Bill / Ref." /><asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" /><asp:BoundField DataField="Remarks" HeaderText="Remarks" /><asp:ButtonField CommandName="DeleteExpenditure" Text="Delete" ButtonType="Button" ControlStyle-CssClass="btn btn-sm btn-outline-danger" /></Columns>
</asp:GridView></div>
</div></div>
</asp:Content>