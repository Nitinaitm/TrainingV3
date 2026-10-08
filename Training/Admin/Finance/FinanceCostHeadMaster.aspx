<%@ Page Title="Finance - Cost Head Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceCostHeadMaster.aspx.cs" Inherits="Training.Admin.FinanceCostHeadMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.finance-page{padding:8px 0 28px}.finance-title{font-weight:700;color:#263238}.finance-subtitle{color:#78909c;font-size:13px}.finance-card{border:0;border-radius:12px;box-shadow:0 4px 16px rgba(0,0,0,.08);overflow:hidden}.finance-card .card-header{background:#f8fafc;border-bottom:1px solid #e9ecef;font-weight:600;color:#37474f}.finance-label{font-size:13px;font-weight:600;color:#546e7a;margin-bottom:6px}.finance-btn{min-width:105px}.finance-message{display:block;margin-bottom:12px}.finance-table{margin-bottom:0}.finance-table th{background:#37474f;color:#fff;border-color:#455a64!important;font-size:13px;white-space:nowrap}.finance-table td{vertical-align:middle;font-size:13px}.finance-table tbody tr:hover{background:#f5f9fc}.finance-table .btn{font-size:12px}.finance-form{background:#fff}.finance-required{color:#dc3545}.finance-actions{padding-top:8px}@media(max-width:767px){.finance-title{font-size:20px}.finance-card .card-body{padding:15px}.finance-actions .btn{width:100%;margin:4px 0!important}.finance-table{min-width:700px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid finance-page">
<div class="mb-3"><h3 class="finance-title mb-1">Finance - Cost Head Master</h3><div class="finance-subtitle">Configure finance cost heads and calculation rules</div></div>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold finance-message"></asp:Label>
<div class="card finance-card mb-4">
<div class="card-header"><i class="fa fa-sliders mr-2"></i>Cost Head Details</div>
<div class="card-body finance-form">
<div class="row">
<div class="col-lg-3 col-md-6 mb-3"><label class="finance-label">Code <span class="finance-required">*</span></label><asp:TextBox ID="txtCode" runat="server" CssClass="form-control" placeholder="e.g. TRAINER_TEACH"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="finance-label">Cost Head <span class="finance-required">*</span></label><asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="e.g. Trainer Teaching"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="finance-label">Level</label><asp:DropDownList ID="ddlLevel" runat="server" CssClass="form-control"><asp:ListItem>Course</asp:ListItem><asp:ListItem>Batch</asp:ListItem><asp:ListItem>Session</asp:ListItem><asp:ListItem>Person</asp:ListItem></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="finance-label">Calculation</label><asp:DropDownList ID="ddlMode" runat="server" CssClass="form-control"><asp:ListItem>Automatic</asp:ListItem><asp:ListItem>Manual</asp:ListItem><asp:ListItem>Automatic + Override</asp:ListItem></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="finance-label">Unit <span class="finance-required">*</span></label><asp:TextBox ID="txtUnit" runat="server" CssClass="form-control" placeholder="Trainee-Day"></asp:TextBox></div>
</div>
<div class="finance-actions"><asp:Button ID="btnSave" runat="server" Text="Save Cost Head" CssClass="btn btn-primary finance-btn mr-2" OnClick="btnSave_Click" /><asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-outline-secondary finance-btn" OnClick="btnClear_Click" /></div>
</div></div>
<div class="card finance-card">
<div class="card-header">Configured Cost Heads</div><div class="card-body p-0"><div class="table-responsive">
<asp:GridView ID="gvHeads" runat="server" CssClass="table table-bordered table-hover finance-table" AutoGenerateColumns="False" DataKeyNames="CostHeadID" OnRowCommand="gvHeads_RowCommand">
<Columns><asp:BoundField DataField="CostHeadCode" HeaderText="Code" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="CostLevel" HeaderText="Level" /><asp:BoundField DataField="CalculationMode" HeaderText="Calculation" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Active" HeaderText="Active" /><asp:ButtonField CommandName="Toggle" Text="Activate / Deactivate" ButtonType="Button" ControlStyle-CssClass="btn btn-sm btn-outline-primary" /></Columns>
</asp:GridView>
</div></div></div>
</div>
</asp:Content>