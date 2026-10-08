<%@ Page Title="Training Report" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="TrainingDetailsReport.aspx.cs" Inherits="Training.Admin.TrainingDetailsReport" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />
<script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
<style>
.report-wrap{padding:22px;background:#f5f7fb}.report-card{background:#fff;border-radius:14px;box-shadow:0 3px 14px rgba(15,23,42,.08);padding:22px;margin-bottom:20px}.report-title{font-size:25px;font-weight:700;color:#17365d;margin-bottom:18px}.filter-grid{display:grid;grid-template-columns:repeat(4,minmax(180px,1fr));gap:14px}.report-wrap .field label{display:block;font-weight:600;color:#334155;margin-bottom:6px}.report-wrap .input{width:100%;padding:9px 11px;border:1px solid #cbd5e1;border-radius:7px;box-sizing:border-box}.report-wrap .actions{display:flex;gap:10px;flex-wrap:wrap;margin-top:16px}.btnx{border:0;border-radius:7px;padding:9px 16px;font-weight:600;cursor:pointer}.blue{background:#2563eb;color:#fff}.gray{background:#64748b;color:#fff}.green{background:#198754;color:#fff}.orange{background:#f59e0b;color:#fff}.purple{background:#7c3aed;color:#fff}.teal{background:#0f766e;color:#fff}.red{background:#dc2626;color:#fff}.grid-wrap{overflow:auto}.grid{width:100%;min-width:1050px;border-collapse:collapse}.grid th{background:#17365d;color:#fff;padding:10px;text-align:left;white-space:nowrap}.training-link{color:#2563eb;font-weight:700;text-decoration:none}.training-popup{position:fixed;inset:0;display:none;align-items:center;justify-content:center;background:rgba(15,23,42,.55);z-index:99999;padding:20px}.training-popup-box{width:100%;max-width:560px;background:#fff;border-radius:12px;box-shadow:0 15px 45px rgba(0,0,0,.25);overflow:hidden}.training-popup-header{display:flex;align-items:center;justify-content:space-between;padding:15px 18px;background:#17365d;color:#fff}.training-popup-title{font-size:18px;font-weight:700}.training-popup-close{border:0;background:transparent;color:#fff;font-size:24px;line-height:1;cursor:pointer}.training-popup-body{padding:18px}.info-table{width:100%;border-collapse:collapse}.info-table th{background:#f8fafc;width:35%;padding:10px;text-align:left;border-bottom:1px solid #e2e8f0}.info-table td{padding:10px;border-bottom:1px solid #e2e8f0}.training-link{color:#2563eb;font-weight:700;text-decoration:none}.grid td{padding:9px;border:1px solid #e2e8f0;white-space:nowrap}.grid tr:nth-child(even){background:#f8fafc}.detail-head{display:flex;justify-content:space-between;align-items:center;gap:12px;flex-wrap:wrap}.training-info{display:grid;grid-template-columns:repeat(4,1fr);gap:10px}.info{background:#f8fafc;border:1px solid #e2e8f0;border-radius:8px;padding:10px}.info b{display:block;color:#64748b;font-size:12px}.info span{font-weight:600;color:#1e293b}.report-tabs{display:flex;gap:8px;flex-wrap:wrap;margin:15px 0}.session-action{min-width:85px}.report-wrap .message{font-weight:600;color:#475569;margin:10px 0}@media(max-width:1000px){.filter-grid{grid-template-columns:repeat(2,1fr)}.training-info{grid-template-columns:repeat(2,1fr)}}@media(max-width:600px){.filter-grid,.training-info{grid-template-columns:1fr}.report-wrap{padding:10px}.report-card{padding:14px}}
</style>
<script>document.addEventListener("DOMContentLoaded",function(){flatpickr(".datepicker",{dateFormat:"d-m-Y",allowInput:true});});</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="report-wrap">
<div class="report-card">
<div class="report-title">Training Report</div>
<div class="filter-grid">
<div class="field"><label>Training Status</label><asp:DropDownList ID="ddlStatus" runat="server" CssClass="input" /></div>
<div class="field"><label>Course</label><asp:DropDownList ID="ddlCourse" runat="server" CssClass="input" /></div>
<div class="field"><label>Training ID</label><asp:TextBox ID="txtTrainingID" runat="server" CssClass="input" /></div>
<div class="field"><label>Batch</label><asp:TextBox ID="txtBatch" runat="server" CssClass="input" /></div>
<div class="field"><label>From Date</label><asp:TextBox ID="txtDateFrom" runat="server" CssClass="input datepicker" placeholder="dd-mm-yyyy" /></div>
<div class="field"><label>To Date</label><asp:TextBox ID="txtDateTo" runat="server" CssClass="input datepicker" placeholder="dd-mm-yyyy" /></div>
</div>
<div class="actions">
<asp:Button ID="btnSearch" runat="server" Text="Search Training" CssClass="btnx blue" OnClick="btnSearch_Click" />
<asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btnx gray" OnClick="btnReset_Click" />
<asp:Button ID="btnExport" runat="server" Text="Export Training List" CssClass="btnx green" OnClick="btnExport_Click" />
</div>
</div>

<asp:Panel ID="pnlTrainingList" runat="server" CssClass="report-card">
<div class="detail-head"><div class="report-title">Training List</div><asp:Label ID="lblTrainingCount" runat="server" CssClass="message" /></div>
<div class="grid-wrap">
<asp:GridView ID="gvTraining" runat="server" AutoGenerateColumns="False" CssClass="grid" OnRowCommand="gvTraining_RowCommand">
<Columns>
<asp:TemplateField HeaderText="Sl No"><ItemTemplate><%# Container.DataItemIndex+1 %></ItemTemplate></asp:TemplateField>
<asp:TemplateField HeaderText="Training ID"><ItemTemplate><asp:LinkButton ID="lnkTrainingID" runat="server" Text='<%# Eval("TrainingID") %>' CssClass="training-link" CommandName="TrainingInfo" CommandArgument='<%# Eval("TrainingID") %>' /></ItemTemplate></asp:TemplateField>
<asp:BoundField DataField="CourseName" HeaderText="Course" />

<asp:BoundField DataField="NoOfDays" HeaderText="Days" />
<asp:BoundField DataField="TrainingStatus" HeaderText="Status" />
<asp:TemplateField HeaderText="Action"><ItemTemplate><asp:LinkButton ID="btnView" runat="server" Text="View" CssClass="btnx blue" CommandName="ViewTraining" CommandArgument='<%# Eval("TrainingID") %>' /></ItemTemplate></asp:TemplateField>
</Columns>
<EmptyDataTemplate><div class="message">No training found for selected filters.</div></EmptyDataTemplate>
</asp:GridView>
</div>
</asp:Panel>

<asp:Panel ID="pnlDetails" runat="server" CssClass="report-card" Visible="false">
<div class="detail-head"><div class="report-title">Training Details</div><asp:Button ID="btnBack" runat="server" Text="Back to Training List" CssClass="btnx gray" OnClick="btnBack_Click" /></div>
<div class="training-info">
<div class="info"><b>Training ID</b><span><asp:Label ID="lblDetailTrainingID" runat="server" /></span></div>
<div class="info"><b>Course</b><span><asp:Label ID="lblCourse" runat="server" /></span></div>
<div class="info"><b>Batch</b><span><asp:Label ID="lblBatch" runat="server" /></span></div>
<div class="info"><b>Status</b><span><asp:Label ID="lblStatus" runat="server" /></span></div>
<div class="info"><b>Type</b><span><asp:Label ID="lblType" runat="server" /></span></div>
<div class="info"><b>Organizer</b><span><asp:Label ID="lblOrganizer" runat="server" /></span></div>
<div class="info"><b>Location</b><span><asp:Label ID="lblLocation" runat="server" /></span></div>
<div class="info"><b>From Date</b><span><asp:Label ID="lblDateFrom" runat="server" /></span></div>
<div class="info"><b>To Date</b><span><asp:Label ID="lblDateTo" runat="server" /></span></div>
<div class="info"><b>Duration</b><span><asp:Label ID="lblDuration" runat="server" /></span></div>
</div>

<div class="report-title" style="font-size:20px;margin-top:22px">Session List</div>
<div class="grid-wrap">
<asp:GridView ID="gvSessions" runat="server" AutoGenerateColumns="False" CssClass="grid" OnRowCommand="gvSessions_RowCommand">
<Columns>
<asp:TemplateField HeaderText="Sl No"><ItemTemplate><%# Container.DataItemIndex+1 %></ItemTemplate></asp:TemplateField>
<asp:BoundField DataField="SessionID" HeaderText="Session ID" />
<asp:BoundField DataField="SessionNo" HeaderText="Session No" />
<asp:BoundField DataField="SessionName" HeaderText="Session" />
<asp:BoundField DataField="TopicName" HeaderText="Topic" />
<asp:BoundField DataField="SessionDate" HeaderText="Date" />
<asp:BoundField DataField="StartTime" HeaderText="Start" />
<asp:BoundField DataField="EndTime" HeaderText="End" />
<asp:BoundField DataField="TrainerName" HeaderText="Trainer" />
<asp:BoundField DataField="SessionStatus" HeaderText="Session Status" />
<asp:TemplateField HeaderText="Attendance"><ItemTemplate><asp:LinkButton ID="btnAttendance" runat="server" Text="Attendance" CssClass="btnx teal session-action" CommandName="Attendance" CommandArgument='<%# Eval("SessionID") %>' /></ItemTemplate></asp:TemplateField>
<asp:TemplateField HeaderText="Pre Test"><ItemTemplate><asp:LinkButton ID="btnPre" runat="server" Text="Pre Test" CssClass="btnx purple session-action" CommandName="PreTest" CommandArgument='<%# Eval("SessionID") %>' /></ItemTemplate></asp:TemplateField>
<asp:TemplateField HeaderText="Post Test"><ItemTemplate><asp:LinkButton ID="btnPost" runat="server" Text="Post Test" CssClass="btnx orange session-action" CommandName="PostTest" CommandArgument='<%# Eval("SessionID") %>' /></ItemTemplate></asp:TemplateField>
</Columns>
<EmptyDataTemplate><div class="message">No sessions found for this training.</div></EmptyDataTemplate>
</asp:GridView>
</div>

<div class="report-title" style="font-size:20px;margin-top:22px">Training Level Reports</div>
<div class="report-tabs">
<asp:Button ID="btnTrainees" runat="server" Text="List of Trainees" CssClass="btnx blue" OnClick="btnTrainees_Click" />
<asp:Button ID="btnTrainers" runat="server" Text="Assigned Trainers" CssClass="btnx teal" OnClick="btnTrainers_Click" />
<asp:Button ID="btnFeedback" runat="server" Text="Feedback" CssClass="btnx purple" OnClick="btnFeedback_Click" />
<asp:Button ID="btnCertificates" runat="server" Text="Certificates" CssClass="btnx green" OnClick="btnCertificates_Click" />
<asp:Button ID="btnHostel" runat="server" Text="Hostel for Trainees" CssClass="btnx orange" OnClick="btnHostel_Click" />
<asp:Button ID="btnExportReport" runat="server" Text="Export Current Report" CssClass="btnx green" OnClick="btnExportReport_Click" />
</div>
<asp:Label ID="lblDetailMessage" runat="server" CssClass="message" />
<div class="grid-wrap"><asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="True" CssClass="grid"><EmptyDataTemplate><div class="message">No records found.</div></EmptyDataTemplate></asp:GridView></div>
</asp:Panel>
</div>
<asp:Panel ID="pnlTrainingInfoModal" runat="server" ClientIDMode="Static" CssClass="training-popup" style="display:none;"><div class="training-popup-box"><div class="training-popup-header"><div class="training-popup-title">Training Information</div><button type="button" class="training-popup-close" onclick="closeTrainingInfo();return false;">&times;</button></div><div class="training-popup-body"><table class="info-table"><tr><th>Training ID</th><td><asp:Label ID="lblPopupTrainingID" runat="server" /></td></tr><tr><th>Type</th><td><asp:Label ID="lblPopupType" runat="server" /></td></tr><tr><th>Organizer</th><td><asp:Label ID="lblPopupOrganizer" runat="server" /></td></tr><tr><th>Location</th><td><asp:Label ID="lblPopupLocation" runat="server" /></td></tr><tr><th>Batch</th><td><asp:Label ID="lblPopupBatch" runat="server" /></td></tr><tr><th>From</th><td><asp:Label ID="lblPopupFrom" runat="server" /></td></tr><tr><th>To</th><td><asp:Label ID="lblPopupTo" runat="server" /></td></tr></table></div></div></asp:Panel><script>function showTrainingInfo(){var e=document.getElementById("pnlTrainingInfoModal");if(e)e.style.display="flex";}function closeTrainingInfo(){var e=document.getElementById("pnlTrainingInfoModal");if(e)e.style.display="none";}</script></asp:Content>