<%@ Page Title="Finance - Rate Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceRateMaster.aspx.cs" Inherits="Training.Admin.FinanceRateMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<h3 class="mb-3">Finance - Rate Master</h3>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold"></asp:Label>
<div class="card mb-3"><div class="card-body"><div class="row">
<div class="col-md-4"><label>Cost Head</label><asp:DropDownList ID="ddlCostHead" runat="server" CssClass="form-control"></asp:DropDownList></div>
<div class="col-md-2"><label>Rate</label><asp:TextBox ID="txtRate" runat="server" CssClass="form-control"></asp:TextBox></div>
<div class="col-md-2"><label>Effective From</label><asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-md-2"><label>Effective To</label><asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox></div>
<div class="col-md-2"><label>Remarks</label><asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox></div>
</div><div class="mt-3"><asp:Button ID="btnSave" runat="server" Text="Save Rate" CssClass="btn btn-primary mr-2" OnClick="btnSave_Click" /><asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" /></div></div></div>
<div class="table-responsive"><asp:GridView ID="gvRates" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False">
<Columns><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="Rate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="EffectiveFrom" HeaderText="From" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="EffectiveTo" HeaderText="To" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="Active" HeaderText="Active" /></Columns></asp:GridView></div>
</div>
</asp:Content>