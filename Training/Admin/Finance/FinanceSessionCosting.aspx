<%@ Page Title="Finance - Session Costing" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceSessionCosting.aspx.cs" Inherits="Training.Admin.FinanceSessionCosting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

<style>
<script>$(document).ready(function(){if($("#ddlTraining").length){if($("#ddlTraining").hasClass("select2-hidden-accessible"))$("#ddlTraining").select2("destroy");$("#ddlTraining").select2({width:"100%",placeholder:"Search Training ID / Course / Batch / Location",allowClear:true});}});</script>
<script>$(document).ready(function(){if($("#ddlTraining").length){if($("#ddlTraining").hasClass("select2-hidden-accessible"))$("#ddlTraining").select2("destroy");$("#ddlTraining").select2({width:"100%",placeholder:"Search / Select",allowClear:true});}if($("#ddlSession").length){if($("#ddlSession").hasClass("select2-hidden-accessible"))$("#ddlSession").select2("destroy");$("#ddlSession").select2({width:"100%",placeholder:"Search / Select",allowClear:true});}});</script></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="main-card">
<div class="page-heading">Finance - Session Costing</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row">
<div class="col-lg-5 col-md-6 mb-3"><label class="form-label">Training / Batch / Location</label><asp:DropDownList ID="ddlTraining" runat="server" ClientIDMode="Static" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-5 col-md-6 mb-3"><label class="form-label">Session</label><asp:DropDownList ID="ddlSession" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlSession_SelectedIndexChanged"></asp:DropDownList></div>
<div class="col-lg-2 col-md-12 mb-3"><label class="summary-label">Session Cost</label><div class="summary-value total-value"><asp:Label ID="lblTotal" runat="server">Rs. 0.00</asp:Label></div></div>
</div>
<div class="table-responsive">
<asp:GridView ID="gvCosting" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False" DataKeyNames="CostingDetailID" OnRowCommand="gvCosting_RowCommand">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField>
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