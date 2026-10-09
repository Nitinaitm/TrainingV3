<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />

<style>
body{background:#f5f5f5;}
.main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin-top:20px;margin-bottom:20px;}
.page-heading{font-size:28px;font-weight:bold;color:darkcyan;margin-bottom:20px;}
.form-control,.form-select{height:38px!important;border:1px solid #ced4da!important;border-radius:4px;}
.btn-save{background:darkcyan!important;color:#fff!important;border:1px solid darkcyan!important;border-radius:4px;padding:7px 18px;font-weight:600;}
.btn-save:hover{background:teal!important;border-color:teal!important;color:#fff!important;}
.finance-grid .btn{white-space:nowrap;min-width:78px;padding:5px 10px;font-size:13px;display:inline-block;}
.finance-grid td:last-child{white-space:nowrap;}
.select2-container{width:100%!important;}
.select2-container--default .select2-selection--single{height:38px!important;border:1px solid #ced4da!important;border-radius:4px!important;}
.select2-selection__rendered{line-height:36px!important;}
.select2-selection__arrow{height:36px!important;}
.summary-label{font-weight:600;margin-bottom:5px;display:block;color:#343a40;}
.summary-value{min-height:38px;height:38px;padding:7px 12px;background:#f8f9fa;border:1px solid #ced4da;border-radius:4px;display:block;}
.total-value{font-weight:700;color:#198754;font-size:18px;}
.report-button{min-width:130px;white-space:nowrap;}
.detail-actions .btn{min-width:130px;white-space:nowrap;}
@media(max-width:768px){.main-card{padding:15px;}.page-heading{font-size:24px;}}
</style>

</asp:Content><asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">
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
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField>
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
</asp:content>
