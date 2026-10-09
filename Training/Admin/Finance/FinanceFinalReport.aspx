<%@ Page Title="Finance - Final Report" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceFinalReport.aspx.cs" Inherits="Training.Admin.FinanceFinalReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin:20px 0}.page-heading{font-size:28px;font-weight:bold;color:#198754;margin-bottom:20px}.summary-value{min-height:42px;padding:9px 12px;background:#f8f9fa;border:1px solid #ced4da;border-radius:4px;display:block;font-weight:600}.total-value{font-size:20px;color:#198754}.finance-grid th{background:#198754!important;color:#fff!important;white-space:nowrap}.finance-grid td{vertical-align:middle}.print-only{display:none}@media print{.no-print,.sidebar,.navbar{display:none!important}.main-card{box-shadow:none;margin:0;padding:0}.print-only{display:block}}
@media(max-width:768px){.main-card{padding:15px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card">
<div class="page-heading">Finance - Final Report</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row no-print">
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Report Type *</label><asp:DropDownList ID="ddlReportType" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlReportType_SelectedIndexChanged"><asp:ListItem Text="Batch Wise" Value="Batch"></asp:ListItem><asp:ListItem Text="Course Wise" Value="Course"></asp:ListItem></asp:DropDownList></div>
<div class="col-lg-5 col-md-6 mb-3" id="pnlBatchFilter" runat="server"><label class="form-label">Training / Batch</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-select"></asp:DropDownList></div>
<div class="col-lg-5 col-md-6 mb-3" id="pnlCourseFilter" runat="server"><label class="form-label">Course</label><asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-select"></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3 d-flex align-items-end"><asp:Button ID="btnShow" runat="server" Text="Show Report" CssClass="btn btn-success w-100" OnClick="btnShow_Click" /></div>
</div>
<div class="row mb-3">
<div class="col-md-3 mb-2"><span class="summary-value">Final Cost: <asp:Label ID="lblFinalCost" runat="server" Text="₹0.00"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Actual Paid: <asp:Label ID="lblActual" runat="server" Text="₹0.00"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Balance: <asp:Label ID="lblBalance" runat="server" Text="₹0.00"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value total-value">Report: <asp:Label ID="lblReportName" runat="server" Text="Batch Wise"></asp:Label></span></div>
</div>
<div class="text-end no-print mb-3"><asp:Button ID="btnPrint" runat="server" Text="Print Final Report" CssClass="btn btn-primary" OnClientClick="window.print();return false;" /></div>
<div class="table-responsive"><asp:GridView ID="gvReport" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False" OnRowCommand="gvReport_RowCommand">
<Columns>
<asp:BoundField DataField="CourseName" HeaderText="Course" />
<asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
<asp:BoundField DataField="Batch" HeaderText="Batch" />
<asp:BoundField DataField="BatchCount" HeaderText="Batches" />
<asp:BoundField DataField="SessionCost" HeaderText="Session Cost" DataFormatString="{0:N2}" />
<asp:BoundField DataField="BatchCost" HeaderText="Batch Cost" DataFormatString="{0:N2}" />
<asp:BoundField DataField="FinalCost" HeaderText="Final Cost" DataFormatString="{0:N2}" />
<asp:BoundField DataField="ActualPaid" HeaderText="Actual Paid" DataFormatString="{0:N2}" />
<asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="{0:N2}" />
<asp:TemplateField HeaderText="Details"><ItemTemplate><asp:Button ID="btnDetails" runat="server" Text="View Details" CssClass="btn btn-sm btn-outline-success" CommandName="Details" CommandArgument='<%# Eval("TrainingID") %>' /></ItemTemplate></asp:TemplateField>
</Columns></asp:GridView></div>
</div></div>
</asp:Content>