<%@ Page Title="Finance - Rate Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceRateMaster.aspx.cs" Inherits="Training.Admin.FinanceRateMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
body { background:#f5f5f5; }
.main-card { background:#fff; padding:25px; border-radius:12px; box-shadow:0 0 10px #d9d9d9; margin-top:20px; margin-bottom:20px; }
.page-heading { font-size:28px; font-weight:bold; color:darkcyan; margin-bottom:20px; }
.validation { color:red; font-size:13px; }
.btn-save { background:darkcyan; color:white; border:none; }
.btn-save:hover { background:teal; color:white; }
.select2-container { width:100% !important; }
.form-select { height:38px !important; }
.form-control { min-height:38px; }
.finance-section-title { font-size:20px; font-weight:bold; color:darkcyan; margin-bottom:15px; }
.finance-table th { background:darkcyan; color:white; white-space:nowrap; }
.finance-table td { vertical-align:middle; }
.message { display:block; margin-bottom:15px; }
.total-box { font-size:20px; font-weight:bold; color:darkcyan; background:#f8f9fa; border:1px solid #ced4da; border-radius:4px; padding:6px 12px; min-height:38px; }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="mb-3"><h3 class="page-heading">Finance - Rate Master</h3><small class="text-muted">Maintain effective rates for each cost head</small></div>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold message"></asp:Label>
<div class="main-card"><div class="finance-section-title"><i class="fas fa-money-bill-wave mr-2"></i>Rate Details</div><div class="card-body">
<div class="row">
<div class="col-lg-4 col-md-6 form-group"><label>Cost Head</label><asp:DropDownList ID="ddlCostHead" runat="server" CssClass="form-control"></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 form-group"><label>Rate</label><asp:TextBox ID="txtRate" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 form-group"><label>Effective From</label><asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 form-group"><label>Effective To</label><asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 form-group"><label>Remarks</label><asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" placeholder="Optional"></asp:TextBox></div>
</div>
<asp:Button ID="btnSave" runat="server" Text="Save Rate" CssClass="btn btn-primary mr-2" OnClick="btnSave_Click" /><asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
</div></div>
<div class="main-card"><div class="finance-section-title"><i class="fas fa-history mr-2"></i>Rate History</div><div class="card-body p-0"><div class="table-responsive">
<asp:GridView ID="gvRates" runat="server" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="False">
<Columns><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="Rate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="EffectiveFrom" HeaderText="From" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="EffectiveTo" HeaderText="To" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="Active" HeaderText="Active" /></Columns>
</asp:GridView>
</div></div></div>
</div>
</asp:Content>