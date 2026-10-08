<%@ Page Title="Finance - Session Costing" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceSessionCosting.aspx.cs" Inherits="Training.Admin.FinanceSessionCosting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<h3 class="mb-3">Finance - Session Costing</h3>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold"></asp:Label>
<div class="card mb-3"><div class="card-body"><div class="row">
<div class="col-md-5"><label>Training / Batch</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-md-5"><label>Session</label><asp:DropDownList ID="ddlSession" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSession_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-md-2"><label>Session Cost</label><asp:Label ID="lblTotal" runat="server" CssClass="form-control bg-light font-weight-bold">₹0.00</asp:Label></div>
</div></div></div>
<div class="card"><div class="card-body"><div class="table-responsive">
<asp:GridView ID="gvCosting" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False" DataKeyNames="CostingDetailID" OnRowCommand="gvCosting_RowCommand">
<Columns><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" /><asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" /><asp:BoundField DataField="OverrideAmount" HeaderText="Override" DataFormatString="{0:N2}" /><asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" /><asp:TemplateField HeaderText="Manual Override"><ItemTemplate><asp:TextBox ID="txtOverride" runat="server" CssClass="form-control form-control-sm" Text='<%# Eval("OverrideAmount") %>'></asp:TextBox><asp:Button ID="btnOverride" runat="server" Text="Save" CssClass="btn btn-sm btn-primary mt-1" CommandName="SaveOverride" CommandArgument="<%# Container.DataItemIndex %>" /></ItemTemplate></asp:TemplateField></Columns>
</asp:GridView>
</div></div></div>
</div>
</asp:Content>