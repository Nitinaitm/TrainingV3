<%@ Page Title="Finance - Final Report" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceFinalReport.aspx.cs" Inherits="Training.Admin.FinanceFinalReport" %>
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

</asp:Content><asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card">
<div class="page-heading">Finance - Final Report</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row no-print">
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Report Type *</label><asp:DropDownList ID="ddlReportType" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlReportType_SelectedIndexChanged"><asp:ListItem Text="Batch Wise" Value="Batch"></asp:ListItem><asp:ListItem Text="Course Wise" Value="Course"></asp:ListItem></asp:DropDownList></div>
<div class="col-lg-5 col-md-6 mb-3" id="pnlBatchFilter" runat="server"><label class="form-label">Training / Batch</label><asp:DropDownList ID="ddlTraining" runat="server" ClientIDMode="Static" CssClass="form-select select2-searchable"></asp:DropDownList></div>
<div class="col-lg-5 col-md-6 mb-3" id="pnlCourseFilter" runat="server"><label class="form-label">Course</label><asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-select select2-searchable"></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3 d-flex align-items-end"><asp:Button ID="btnShow" runat="server" Text="Show Report" CssClass="btn btn-success w-100 report-button" OnClick="btnShow_Click" /></div>
</div>
<div class="row mb-3">
<div class="col-md-3 mb-2"><span class="summary-value">Final Cost: <asp:Label ID="lblFinalCost" runat="server" Text="Rs. 0.00"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Actual Paid: <asp:Label ID="lblActual" runat="server" Text="Rs. 0.00"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Balance: <asp:Label ID="lblBalance" runat="server" Text="Rs. 0.00"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value total-value">Report: <asp:Label ID="lblReportName" runat="server" Text="Batch Wise"></asp:Label></span></div>
</div>
<div class="text-end no-print mb-3"><asp:Button ID="btnPrint" runat="server" Text="Print Final Report" CssClass="btn btn-primary report-button" OnClientClick="window.print();return false;" /></div>
<div class="table-responsive"><asp:GridView ID="gvReport" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False" OnRowCommand="gvReport_RowCommand">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField>
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