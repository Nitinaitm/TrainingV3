<%@ Page Title="Finance - Training Costing" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceCosting.aspx.cs" Inherits="Training.Admin.FinanceCosting" %>
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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="main-card">
<div class="page-heading">Finance - Training Costing</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row">
<div class="col-lg-5 col-md-12 mb-3"><label class="form-label">Training / Batch</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-2 col-md-4 col-6 mb-3"><label class="summary-label">Trainees</label><asp:Label ID="lblTrainees" runat="server" CssClass="summary-value"></asp:Label></div>
<div class="col-lg-2 col-md-4 col-6 mb-3"><label class="summary-label">Training Days</label><asp:Label ID="lblDays" runat="server" CssClass="summary-value"></asp:Label></div>
<div class="col-lg-3 col-md-4 mb-3"><label class="summary-label">Total Cost</label><div class="summary-value total-value"><asp:Label ID="lblTotal" runat="server" Text="₹0.00"></asp:Label></div></div>
</div>
<div class="table-responsive">
<asp:GridView ID="gvCosting" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False" DataKeyNames="CostingDetailID" OnRowCommand="gvCosting_RowCommand">
<Columns>
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
<Columns><asp:BoundField DataField="CourseName" HeaderText="Course" /><asp:BoundField DataField="BatchCount" HeaderText="Batches" /><asp:BoundField DataField="CourseCost" HeaderText="Course Cost" DataFormatString="{0:N2}" /><asp:BoundField DataField="AverageBatchCost" HeaderText="Avg Batch Cost" DataFormatString="{0:N2}" /></Columns>
</asp:GridView>
</div></div>
</div>
</div>
</div>
</asp:Content>