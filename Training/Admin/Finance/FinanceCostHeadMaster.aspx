<%@ page title="Finance - Cost Head Master" language="C#" masterpagefile="~/AdminMaster.Master" autoeventwireup="true" codebehind="FinanceCostHeadMaster.aspx.cs" inherits="Training.Admin.FinanceCostHeadMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <style>
</asp:Content>
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">
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
