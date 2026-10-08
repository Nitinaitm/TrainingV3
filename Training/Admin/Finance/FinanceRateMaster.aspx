<%@ Page Title="Finance - Rate Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceRateMaster.aspx.cs" Inherits="Training.Admin.FinanceRateMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>.finance-page .page-title{font-weight:600;color:#343a40}.finance-page .card{border:0;box-shadow:0 2px 10px rgba(0,0,0,.08)}.finance-page .card-header{font-weight:600;background:#f8f9fa;border-bottom:1px solid #e9ecef}.finance-page .form-group label{font-weight:600;color:#495057;font-size:14px}.finance-page .table th{background:#343a40;color:#fff;white-space:nowrap}.finance-page .table td{vertical-align:middle}.finance-page .table tbody tr:hover{background:#f8f9fa}.finance-page .message{display:block;margin-bottom:15px}</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid finance-page">
<div class="mb-3"><h3 class="page-title mb-1">Finance - Rate Master</h3><small class="text-muted">Maintain effective rates for each cost head</small></div>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold message"></asp:Label>
<div class="card shadow-sm mb-4"><div class="card-header"><i class="fas fa-money-bill-wave mr-2"></i>Rate Details</div><div class="card-body">
<div class="row">
<div class="col-lg-4 col-md-6 form-group"><label>Cost Head</label><asp:DropDownList ID="ddlCostHead" runat="server" CssClass="form-control"></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 form-group"><label>Rate</label><asp:TextBox ID="txtRate" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 form-group"><label>Effective From</label><asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 form-group"><label>Effective To</label><asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 form-group"><label>Remarks</label><asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" placeholder="Optional"></asp:TextBox></div>
</div>
<asp:Button ID="btnSave" runat="server" Text="Save Rate" CssClass="btn btn-primary mr-2" OnClick="btnSave_Click" /><asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
</div></div>
<div class="card shadow-sm"><div class="card-header"><i class="fas fa-history mr-2"></i>Rate History</div><div class="card-body p-0"><div class="table-responsive">
<asp:GridView ID="gvRates" runat="server" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="False">
<Columns><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="Rate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="EffectiveFrom" HeaderText="From" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="EffectiveTo" HeaderText="To" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="Active" HeaderText="Active" /></Columns>
</asp:GridView>
</div></div></div>
</div>
</asp:Content>