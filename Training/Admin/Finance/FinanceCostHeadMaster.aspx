<%@ Page Title="Finance - Cost Head Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceCostHeadMaster.aspx.cs" Inherits="Training.Admin.FinanceCostHeadMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
body { background:#f5f5f5; }
.main-card { background:#fff; padding:25px; border-radius:12px; box-shadow:0 0 10px #d9d9d9; margin-top:20px; margin-bottom:20px; }
.page-heading { font-size:28px; font-weight:bold; color:darkcyan; margin-bottom:20px; }
.validation { color:red; font-size:13px; }
.btn-save { background:darkcyan; color:white; border:none; }
.btn-save:hover { background:teal; color:white; }
.select2-container { width:100% !important; }
.form-select { height:38px !important; }
.form-control { min-height:38px; }
.finance-section-title { font-size:20px; font-weight:bold; color:darkcyan; margin-bottom:15px; }
.finance-table th { background:darkcyan; color:white; white-space:nowrap; }
.finance-table td { vertical-align:middle; }
.message { display:block; margin-bottom:15px; }
.total-box { font-size:20px; font-weight:bold; color:darkcyan; background:#f8f9fa; border:1px solid #ced4da; border-radius:4px; padding:6px 12px; min-height:38px; }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="d-flex justify-content-between align-items-center mb-3"><div><h3 class="page-heading">Finance - Cost Head Master</h3><small class="text-muted">Configure cost heads and calculation rules</small></div></div>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold message"></asp:Label>
<div class="main-card"><div class="finance-section-title"><i class="fas fa-sliders-h mr-2"></i>Cost Head Details</div><div class="card-body">
<div class="row">
<div class="col-lg-3 col-md-6 form-group"><label>Code <span class="text-danger">*</span></label><asp:TextBox ID="txtCode" runat="server" CssClass="form-control" placeholder="TRAINER_TEACH"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 form-group"><label>Cost Head <span class="text-danger">*</span></label><asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Trainer Teaching"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 form-group"><label>Level</label><asp:DropDownList ID="ddlLevel" runat="server" CssClass="form-control"><asp:ListItem>Course</asp:ListItem><asp:ListItem>Batch</asp:ListItem><asp:ListItem>Session</asp:ListItem><asp:ListItem>Person</asp:ListItem></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 form-group"><label>Calculation</label><asp:DropDownList ID="ddlMode" runat="server" CssClass="form-control"><asp:ListItem>Automatic</asp:ListItem><asp:ListItem>Manual</asp:ListItem><asp:ListItem>Automatic + Override</asp:ListItem></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 form-group"><label>Unit <span class="text-danger">*</span></label><asp:TextBox ID="txtUnit" runat="server" CssClass="form-control" placeholder="Trainee-Day"></asp:TextBox></div>
</div>
<asp:Button ID="btnSave" runat="server" Text="Save Cost Head" CssClass="btn btn-primary mr-2" OnClick="btnSave_Click" /><asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
</div></div>
<div class="main-card"><div class="finance-section-title"><i class="fas fa-list mr-2"></i>Configured Cost Heads</div><div class="card-body p-0"><div class="table-responsive">
<asp:GridView ID="gvHeads" runat="server" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="False" DataKeyNames="CostHeadID" OnRowCommand="gvHeads_RowCommand">
<Columns><asp:BoundField DataField="CostHeadCode" HeaderText="Code" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="CostLevel" HeaderText="Level" /><asp:BoundField DataField="CalculationMode" HeaderText="Calculation" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Active" HeaderText="Active" /><asp:ButtonField CommandName="Toggle" Text="Activate / Deactivate" ButtonType="Button" ControlStyle-CssClass="btn btn-sm btn-outline-primary" /></Columns>
</asp:GridView>
</div></div></div>
</div>
</asp:Content>