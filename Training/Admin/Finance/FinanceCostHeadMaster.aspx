<%@ Page Title="Finance - Cost Head Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceCostHeadMaster.aspx.cs" Inherits="Training.Admin.FinanceCostHeadMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
body { background:#f5f5f5; }
.main-card { background:#fff; padding:25px; border-radius:12px; box-shadow:0 0 10px #d9d9d9; margin-top:20px; margin-bottom:20px; }
.page-heading { font-size:28px; font-weight:bold; color:darkcyan; margin-bottom:20px; }
.validation { color:red; font-size:13px; }
.btn-save { background:darkcyan; color:white; border:none; }
.btn-save:hover { background:teal; color:white; }
.form-select { height:38px !important; }
.table-responsive { margin-top:25px; }
.finance-grid { margin-bottom:0 !important; }
.finance-grid th { background:darkcyan !important; color:white !important; font-weight:600; white-space:nowrap; }
.finance-grid td { vertical-align:middle; }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="main-card">
<div class="page-heading">Finance - Cost Head Master</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row">
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Code *</label><asp:TextBox ID="txtCode" runat="server" CssClass="form-control" placeholder="TRAINER_TEACH"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Cost Head *</label><asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Trainer Teaching"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Level</label><asp:DropDownList ID="ddlLevel" runat="server" CssClass="form-select"><asp:ListItem>Course</asp:ListItem><asp:ListItem>Batch</asp:ListItem><asp:ListItem>Session</asp:ListItem><asp:ListItem>Person</asp:ListItem></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Calculation</label><asp:DropDownList ID="ddlMode" runat="server" CssClass="form-select"><asp:ListItem>Automatic</asp:ListItem><asp:ListItem>Manual</asp:ListItem><asp:ListItem>Automatic + Override</asp:ListItem></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Unit *</label><asp:TextBox ID="txtUnit" runat="server" CssClass="form-control" placeholder="Trainee-Day"></asp:TextBox></div>
</div>
<div class="mt-2 text-center">
<asp:Button ID="btnSave" runat="server" Text="Save Cost Head" CssClass="btn btn-save" OnClick="btnSave_Click" />
&nbsp;
<asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
</div>
<div class="table-responsive">
<asp:GridView ID="gvHeads" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False" DataKeyNames="CostHeadID" OnRowCommand="gvHeads_RowCommand">
<Columns>
<asp:BoundField DataField="CostHeadCode" HeaderText="Code" />
<asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" />
<asp:BoundField DataField="CostLevel" HeaderText="Level" />
<asp:BoundField DataField="CalculationMode" HeaderText="Calculation" />
<asp:BoundField DataField="UnitType" HeaderText="Unit" />
<asp:BoundField DataField="Active" HeaderText="Active" />
<asp:ButtonField CommandName="Toggle" Text="Activate / Deactivate" ButtonType="Button" ControlStyle-CssClass="btn btn-sm btn-outline-primary" />
</Columns>
</asp:GridView>
</div>
</div>
</div>
</asp:Content>