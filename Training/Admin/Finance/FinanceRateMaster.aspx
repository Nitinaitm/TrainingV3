<%@ Page Title="Finance - Rate Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceRateMaster.aspx.cs" Inherits="Training.Admin.FinanceRateMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
        .main-card { background:#fff; padding:25px; border-radius:12px; box-shadow:0 0 10px #d9d9d9; margin-top:20px; margin-bottom:20px; }

        .page-heading { font-size:28px; font-weight:bold; color:#198754; margin-bottom:20px; }

        .form-control,.form-select { height:38px !important; border:1px solid #ced4da; border-radius:4px; }
        .btn-save { background:#198754; color:#fff; border:none; }
        .btn-save:hover { background:#157347; color:#fff; }
        .summary-label { font-weight:500; margin-bottom:6px; display:block; color:#343a40; }
        .summary-value { min-height:38px; padding:7px 12px; background:#f8f9fa; border:1px solid #ced4da; border-radius:4px; display:block; }
        .total-value { font-weight:bold; color:#198754; font-size:18px; }
        .table-responsive { margin-top:25px; }
        .finance-grid { margin-bottom:0 !important; }
        .finance-grid th { background:#198754 !important; color:#fff !important; font-weight:600; white-space:nowrap; }
        .finance-grid td { vertical-align:middle; }
        @media(max-width:768px) { .main-card { padding:15px; } }
    
.finance-grid .btn { white-space:nowrap; min-width:78px; padding:5px 10px; font-size:13px; display:inline-block; }
.finance-grid td:last-child { white-space:nowrap; }
.action-button { white-space:nowrap; min-width:110px; }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="main-card">
<div class="page-heading">Finance - Rate Master</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row">
<div class="col-lg-4 col-md-6 mb-3"><label class="form-label">Cost Head</label><asp:DropDownList ID="ddlCostHead" runat="server" CssClass="form-select"></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Rate</label><asp:TextBox ID="txtRate" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Effective From</label><asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Effective To</label><asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Remarks</label><asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" placeholder="Optional"></asp:TextBox></div>
</div>
<div class="mt-2 text-center">
<asp:Button ID="btnSave" runat="server" Text="Save Rate" CssClass="btn btn-save" OnClick="btnSave_Click" />
&nbsp;
<asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
</div>
<div class="table-responsive">
<asp:GridView ID="gvRates" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField>
<asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" />
<asp:BoundField DataField="Rate" HeaderText="Rate" DataFormatString="{0:N2}" />
<asp:BoundField DataField="EffectiveFrom" HeaderText="From" DataFormatString="{0:dd-MM-yyyy}" />
<asp:BoundField DataField="EffectiveTo" HeaderText="To" DataFormatString="{0:dd-MM-yyyy}" />
<asp:BoundField DataField="Active" HeaderText="Active" />
</Columns>
</asp:GridView>
</div>
</div>
</div>
</asp:Content>