<%@ Page Title="Finance - Actual Expenditure" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceActualExpenditure.aspx.cs" Inherits="Training.Admin.FinanceActualExpenditure" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin:20px 0}.page-heading{font-size:28px;font-weight:bold;color:#198754;margin-bottom:20px}.summary-value{min-height:38px;padding:7px 12px;background:#f8f9fa;border:1px solid #ced4da;border-radius:4px;display:block;font-weight:600}.finance-grid th{background:#198754!important;color:#fff!important;white-space:nowrap}.finance-grid td{vertical-align:middle}@media(max-width:768px){.main-card{padding:15px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card">
<div class="page-heading">Finance - Actual Expenditure</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row">
<div class="col-lg-4 col-md-6 mb-3"><label class="form-label">Training / Batch *</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Session</label><asp:DropDownList ID="ddlSession" runat="server" CssClass="form-select"></asp:DropDownList></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Cost Head</label><asp:DropDownList ID="ddlCostHead" runat="server" CssClass="form-select"></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Date *</label><asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Amount *</label><asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Vendor / Payee</label><asp:TextBox ID="txtVendor" runat="server" CssClass="form-control"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Bill / Reference No.</label><asp:TextBox ID="txtBill" runat="server" CssClass="form-control"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Remarks</label><asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox></div>
</div>
<div class="text-center mb-3"><asp:Button ID="btnSave" runat="server" Text="Save Expenditure" CssClass="btn btn-success" OnClick="btnSave_Click" />&nbsp;<asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" /></div>
<div class="row mb-3"><div class="col-md-4"><span class="summary-value">Final Cost: <asp:Label ID="lblFinalCost" runat="server" Text="₹0.00"></asp:Label></span></div><div class="col-md-4"><span class="summary-value">Actual Paid: <asp:Label ID="lblActual" runat="server" Text="₹0.00"></asp:Label></span></div><div class="col-md-4"><span class="summary-value">Balance: <asp:Label ID="lblBalance" runat="server" Text="₹0.00"></asp:Label></span></div></div>
<div class="table-responsive"><asp:GridView ID="gvExpenditure" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False" DataKeyNames="ExpenditureID" OnRowCommand="gvExpenditure_RowCommand">
<Columns><asp:BoundField DataField="ExpenditureDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="SessionName" HeaderText="Session" /><asp:BoundField DataField="VendorName" HeaderText="Vendor / Payee" /><asp:BoundField DataField="BillReference" HeaderText="Bill / Ref." /><asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" /><asp:BoundField DataField="Remarks" HeaderText="Remarks" /><asp:ButtonField CommandName="DeleteExpenditure" Text="Delete" ButtonType="Button" ControlStyle-CssClass="btn btn-sm btn-outline-danger" /></Columns>
</asp:GridView></div>
</div></div>
</asp:Content>