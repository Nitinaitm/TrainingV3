<%@ Page Title="Finance - Rate Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceRateMaster.aspx.cs" Inherits="Training.Admin.FinanceRateMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
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