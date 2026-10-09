<%@ Page Title="Finance - Cost Head Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceCostHeadMaster.aspx.cs" Inherits="Training.Admin.FinanceCostHeadMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
body{background:#f5f5f5}.main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin:20px 0}.page-heading{font-size:28px;font-weight:bold;color:darkcyan;margin-bottom:4px}.page-subheading{color:#6c757d;display:block;margin-bottom:22px}.finance-section{border-top:1px solid #e5e5e5;padding-top:20px;margin-top:20px}.finance-section-title{font-size:20px;font-weight:bold;color:darkcyan;margin-bottom:18px}.form-label{font-weight:500;margin-bottom:6px;color:#343a40}.form-control,.form-select{height:38px;border:1px solid #ced4da;border-radius:4px}.btn-save{background:darkcyan;color:#fff;border:none}.btn-save:hover{background:teal;color:#fff}.finance-actions{margin-top:10px}.finance-table{margin-bottom:0!important}.finance-table th{background:darkcyan!important;color:#fff!important;white-space:nowrap;font-weight:600;padding:10px 12px}.finance-table td{vertical-align:middle;padding:9px 12px}.finance-table tbody tr:hover{background:#f8fbfb}.table-wrap{overflow-x:auto;border:1px solid #dee2e6;border-radius:6px}.message{display:block;margin-bottom:10px}.validation{color:red;font-size:13px}@media(max-width:767px){.main-card{padding:18px}.page-heading{font-size:24px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="main-card">
<div class="page-heading">Finance - Cost Head Master</div><span class="page-subheading">Configure cost heads and calculation rules</span>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold message"></asp:Label>
<div class="row">
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Code <span class="text-danger">*</span></label><asp:TextBox ID="txtCode" runat="server" CssClass="form-control" placeholder="TRAINER_TEACH"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Cost Head <span class="text-danger">*</span></label><asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Trainer Teaching"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Level</label><asp:DropDownList ID="ddlLevel" runat="server" CssClass="form-select"><asp:ListItem>Course</asp:ListItem><asp:ListItem>Batch</asp:ListItem><asp:ListItem>Session</asp:ListItem><asp:ListItem>Person</asp:ListItem></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Calculation</label><asp:DropDownList ID="ddlMode" runat="server" CssClass="form-select"><asp:ListItem>Automatic</asp:ListItem><asp:ListItem>Manual</asp:ListItem><asp:ListItem>Automatic + Override</asp:ListItem></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Unit <span class="text-danger">*</span></label><asp:TextBox ID="txtUnit" runat="server" CssClass="form-control" placeholder="Trainee-Day"></asp:TextBox></div>
</div>
<div class="finance-actions"><asp:Button ID="btnSave" runat="server" Text="Save Cost Head" CssClass="btn btn-save mr-2" OnClick="btnSave_Click" /><asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" /></div>
<div class="finance-section">
<div class="finance-section-title"><i class="fas fa-list mr-2"></i>Configured Cost Heads</div>
<div class="table-wrap"><asp:GridView ID="gvHeads" runat="server" CssClass="table table-bordered table-hover mb-0 finance-table" AutoGenerateColumns="False" DataKeyNames="CostHeadID" OnRowCommand="gvHeads_RowCommand">
<Columns><asp:BoundField DataField="CostHeadCode" HeaderText="Code" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="CostLevel" HeaderText="Level" /><asp:BoundField DataField="CalculationMode" HeaderText="Calculation" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Active" HeaderText="Active" /><asp:ButtonField CommandName="Toggle" Text="Activate / Deactivate" ButtonType="Button" ControlStyle-CssClass="btn btn-sm btn-outline-primary" /></Columns>
</asp:GridView></div>
</div>
</div></div>
</asp:Content>