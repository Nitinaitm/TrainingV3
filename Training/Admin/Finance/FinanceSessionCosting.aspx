<%@ Page Title="Finance - Session Costing" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceSessionCosting.aspx.cs" Inherits="Training.Admin.FinanceSessionCosting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
body { background:#f5f5f5; }
.main-card { background:#fff; padding:25px; border-radius:12px; box-shadow:0 0 10px #d9d9d9; margin-top:20px; margin-bottom:20px; }
.page-heading { font-size:28px; font-weight:bold; color:darkcyan; margin-bottom:20px; }
.form-select { height:38px !important; }
.table-responsive { margin-top:25px; }
.finance-grid { margin-bottom:0 !important; }
.finance-grid th { background:darkcyan !important; color:white !important; font-weight:600; white-space:nowrap; }
.finance-grid td { vertical-align:middle; }
.summary-label { font-weight:500; margin-bottom:6px; display:block; color:#343a40; }
.summary-value { min-height:38px; padding:7px 12px; background:#f8f9fa; border:1px solid #ced4da; border-radius:4px; }
.total-value { font-weight:bold; color:darkcyan; font-size:18px; }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="main-card">
<div class="page-heading">Finance - Session Costing</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row">
<div class="col-lg-5 col-md-6 mb-3"><label class="form-label">Training / Batch</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-5 col-md-6 mb-3"><label class="form-label">Session</label><asp:DropDownList ID="ddlSession" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlSession_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-2 col-md-12 mb-3"><label class="summary-label">Session Cost</label><div class="summary-value total-value"><asp:Label ID="lblTotal" runat="server">₹0.00</asp:Label></div></div>
</div>
<div class="table-responsive">
<asp:GridView ID="gvCosting" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False" DataKeyNames="CostingDetailID" OnRowCommand="gvCosting_RowCommand">
<Columns>
<asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" />
<asp:BoundField DataField="UnitType" HeaderText="Unit" />
<asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" />
<asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" />
<asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" />
<asp:BoundField DataField="OverrideAmount" HeaderText="Override" DataFormatString="{0:N2}" />
<asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" />
<asp:TemplateField HeaderText="Manual Override"><ItemTemplate><asp:TextBox ID="txtOverride" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("OverrideAmount") %>'></asp:TextBox><asp:Button ID="btnOverride" runat="server" Text="Save" CssClass="btn btn-sm btn-primary mt-1" CommandName="SaveOverride" CommandArgument="<%# Container.DataItemIndex %>" /></ItemTemplate></asp:TemplateField>
</Columns>
</asp:GridView>
</div>
</div>
</div>
</asp:Content>