<%@ Page Title="Finance - Training Detail" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceTrainingDetail.aspx.cs" Inherits="Training.Admin.FinanceTrainingDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin:20px 0}.page-heading{font-size:28px;font-weight:bold;color:#198754;margin-bottom:20px}.summary-value{min-height:40px;padding:8px 12px;background:#f8f9fa;border:1px solid #ced4da;border-radius:4px;display:block;font-weight:600}.total-value{font-size:20px;color:#198754}.section-title{font-size:20px;font-weight:bold;color:#198754;margin:25px 0 12px}.finance-grid th{background:#198754!important;color:#fff!important;white-space:nowrap}.finance-grid td{vertical-align:middle}.no-print{text-align:right;margin-bottom:15px}@media print{.no-print{display:none}.main-card{box-shadow:none;margin:0;padding:0}}@media(max-width:768px){.main-card{padding:15px}}

.finance-grid .btn { white-space:nowrap; min-width:78px; padding:5px 10px; font-size:13px; display:inline-block; }
.finance-grid td:last-child { white-space:nowrap; }
.action-button { white-space:nowrap; min-width:110px; }
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card">
<div class="page-heading">Finance - Training Detail</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row">
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Training ID: <asp:Label ID="lblTrainingID" runat="server"></asp:Label></span></div>
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Course: <asp:Label ID="lblCourse" runat="server"></asp:Label></span></div>
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Batch: <asp:Label ID="lblBatch" runat="server"></asp:Label></span></div>
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Duration: <asp:Label ID="lblDuration" runat="server"></asp:Label></span></div>
</div>
<div class="row mt-2">
<div class="col-md-3 mb-2"><span class="summary-value">Trainees: <asp:Label ID="lblTrainees" runat="server"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Trainers: <asp:Label ID="lblTrainers" runat="server"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Final Cost: <asp:Label ID="lblFinalCost" runat="server" CssClass="total-value"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Actual Paid: <asp:Label ID="lblActual" runat="server"></asp:Label></span></div>
</div>
<div class="no-print"><asp:Button ID="btnPrint" runat="server" Text="Print Detail" CssClass="btn btn-primary" OnClientClick="window.print();return false;" />&nbsp;<asp:Button ID="btnBack" runat="server" Text="Back to Final Report" CssClass="btn btn-secondary" OnClick="btnBack_Click" /></div>
<div class="section-title">1. Session-wise Cost Head</div>
<div class="table-responsive"><asp:GridView ID="gvSession" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="SessionName" HeaderText="Session" /><asp:BoundField DataField="SessionDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" /><asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" /><asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" /></Columns></asp:GridView></div>
<div class="section-title">2. Batch-wise Cost Head</div>
<div class="table-responsive"><asp:GridView ID="gvBatch" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" /><asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" /><asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" /></Columns></asp:GridView></div>
<div class="section-title">3. Trainer-wise Head Cost</div>
<div class="table-responsive"><asp:GridView ID="gvTrainer" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="TrainerID" HeaderText="Trainer ID" /><asp:BoundField DataField="TrainerName" HeaderText="Trainer" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="FinalAmount" HeaderText="Allocated Cost" DataFormatString="{0:N2}" /></Columns></asp:GridView></div>
<div class="section-title">4. Trainee-wise Head Cost</div>
<div class="table-responsive"><asp:GridView ID="gvTrainee" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="EmpID" HeaderText="Emp ID" /><asp:BoundField DataField="EmpName" HeaderText="Trainee" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="FinalAmount" HeaderText="Allocated Cost" DataFormatString="{0:N2}" /></Columns></asp:GridView></div>
<div class="section-title">5. Actual Expenditure / Payment</div>
<div class="table-responsive"><asp:GridView ID="gvActual" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="ExpenditureDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="SessionName" HeaderText="Session" /><asp:BoundField DataField="VendorName" HeaderText="Vendor / Payee" /><asp:BoundField DataField="BillReference" HeaderText="Bill / Ref." /><asp:BoundField DataField="Amount" HeaderText="Paid" DataFormatString="{0:N2}" /></Columns></asp:GridView></div>
</div></div>
</asp:Content>