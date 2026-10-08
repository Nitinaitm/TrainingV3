<%@ Page Title="Finance - Training Costing" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceCosting.aspx.cs" Inherits="Training.Admin.FinanceCosting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"><style>.finance-kpi{border-radius:10px;padding:16px;background:#fff;box-shadow:0 2px 8px rgba(0,0,0,.08)}.money{font-weight:700;font-size:22px}</style></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<h3 class="mb-3">Finance - Training Costing</h3>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold"></asp:Label>
<div class="card mb-3"><div class="card-body"><div class="row">
<div class="col-md-5"><label>Training / Batch</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-md-2"><label>Trainees</label><asp:Label ID="lblTrainees" runat="server" CssClass="form-control bg-light"></asp:Label></div>
<div class="col-md-2"><label>Training Days</label><asp:Label ID="lblDays" runat="server" CssClass="form-control bg-light"></asp:Label></div>
<div class="col-md-3"><label>Total Cost</label><div class="finance-kpi money"><asp:Label ID="lblTotal" runat="server" Text="₹0.00"></asp:Label></div></div>
</div></div></div>
<div class="card"><div class="card-header font-weight-bold">Batch Costing</div><div class="card-body">
<div class="table-responsive"><asp:GridView ID="gvCosting" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False">
<Columns><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="CostLevel" HeaderText="Level" /><asp:BoundField DataField="CalculationMode" HeaderText="Mode" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" /><asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" /><asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" /><asp:BoundField DataField="OverrideAmount" HeaderText="Override" DataFormatString="{0:N2}" /><asp:TemplateField HeaderText="Manual Override"><ItemTemplate><asp:TextBox ID="txtOverride" runat="server" CssClass="form-control form-control-sm" Text="<%# Eval("OverrideAmount") %>"></asp:TextBox><asp:Button ID="btnOverride" runat="server" Text="Save" CssClass="btn btn-sm btn-primary mt-1" CommandName="SaveOverride" CommandArgument="<%# Container.DataItemIndex %>" /></ItemTemplate></asp:TemplateField></Columns>
</asp:GridView></div>
</div></div>
<div class="card mt-3"><div class="card-header font-weight-bold">Course-wise Summary</div><div class="card-body"><div class="table-responsive">
<asp:GridView ID="gvCourse" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False"><Columns><asp:BoundField DataField="CourseName" HeaderText="Course" /><asp:BoundField DataField="BatchCount" HeaderText="Batches" /><asp:BoundField DataField="CourseCost" HeaderText="Course Cost" DataFormatString="{0:N2}" /><asp:BoundField DataField="AverageBatchCost" HeaderText="Avg Batch Cost" DataFormatString="{0:N2}" /></Columns></asp:GridView>
</div></div></div>
</div>
</asp:Content>