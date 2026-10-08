<%@ Page Title="Finance - Cost Head Master" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceCostHeadMaster.aspx.cs" Inherits="Training.Admin.FinanceCostHeadMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<h3 class="mb-3">Finance - Cost Head Master</h3>
<asp:Label ID="lblMessage" runat="server" CssClass="font-weight-bold"></asp:Label>
<div class="card mb-3"><div class="card-body">
<div class="row">
<div class="col-md-3"><label>Code</label><asp:TextBox ID="txtCode" runat="server" CssClass="form-control"></asp:TextBox></div>
<div class="col-md-3"><label>Cost Head</label><asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox></div>
<div class="col-md-2"><label>Level</label><asp:DropDownList ID="ddlLevel" runat="server" CssClass="form-control"><asp:ListItem>Course</asp:ListItem><asp:ListItem>Batch</asp:ListItem><asp:ListItem>Session</asp:ListItem><asp:ListItem>Person</asp:ListItem></asp:DropDownList></div>
<div class="col-md-2"><label>Calculation</label><asp:DropDownList ID="ddlMode" runat="server" CssClass="form-control"><asp:ListItem>Automatic</asp:ListItem><asp:ListItem>Manual</asp:ListItem><asp:ListItem>Automatic + Override</asp:ListItem></asp:DropDownList></div>
<div class="col-md-2"><label>Unit</label><asp:TextBox ID="txtUnit" runat="server" CssClass="form-control" placeholder="Trainee-Day"></asp:TextBox></div>
</div>
<div class="mt-3"><asp:Button ID="btnSave" runat="server" Text="Save Cost Head" CssClass="btn btn-primary mr-2" OnClick="btnSave_Click" /><asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" /></div>
</div></div>
<div class="table-responsive"><asp:GridView ID="gvHeads" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False" DataKeyNames="CostHeadID" OnRowCommand="gvHeads_RowCommand">
<Columns><asp:BoundField DataField="CostHeadCode" HeaderText="Code" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="CostLevel" HeaderText="Level" /><asp:BoundField DataField="CalculationMode" HeaderText="Calculation" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Active" HeaderText="Active" /><asp:ButtonField CommandName="Toggle" Text="Activate / Deactivate" /></Columns>
</asp:GridView></div>
</div>
</asp:Content>