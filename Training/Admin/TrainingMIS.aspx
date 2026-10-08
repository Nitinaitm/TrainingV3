<%@ Page Title="Training MIS" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="TrainingMIS.aspx.cs" Inherits="Training.Admin.TrainingMIS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
*{box-sizing:border-box}html,body{width:100%;max-width:100%;margin:0;padding:0}body{overflow-x:hidden}

.mis-wrap{padding:22px;background:#f5f7fb}.mis-card{background:#fff;border-radius:14px;padding:20px;margin-bottom:20px;box-shadow:0 3px 14px rgba(15,23,42,.08)}.mis-title{font-size:25px;font-weight:700;color:#17365d;margin-bottom:18px}.filter-grid{display:grid;grid-template-columns:repeat(4,1fr);gap:14px}.mis-wrap .field label{display:block;font-weight:600;color:#334155;margin-bottom:6px}.mis-wrap .input{width:100%;padding:9px 11px;border:1px solid #cbd5e1;border-radius:7px}.mis-wrap .actions{display:flex;gap:10px;flex-wrap:wrap;margin-top:16px}.btnx{border:0;border-radius:7px;padding:9px 16px;font-weight:600;color:#fff;cursor:pointer}.blue{background:#2563eb}.gray{background:#64748b}.green{background:#198754}.kpi-grid{display:grid;grid-template-columns:repeat(5,1fr);gap:14px}.kpi{border-radius:12px;padding:0;color:#fff;min-height:105px}.kpi-link{display:block;padding:18px;color:#inherit;text-decoration:none;height:100%}.kpi-link:hover{color:#fff;text-decoration:none;filter:brightness(1.05)}.kpi-label{font-size:13px;font-weight:600;opacity:.9}.kpi-value{font-size:30px;font-weight:800;margin-top:8px}.k1{background:#2563eb}.k2{background:#198754}.k3{background:#f59e0b}.k4{background:#7c3aed}.k5{background:#0f766e}.k6{background:#dc2626}.k7{background:#0891b2}.k8{background:#475569}.summary-grid{display:grid;grid-template-columns:repeat(2,1fr);gap:20px}.mis-table{width:100%;border-collapse:collapse}.mis-table th{background:#17365d;color:#fff;padding:10px;text-align:left}.mis-table td{padding:9px;border:1px solid #e2e8f0}.mis-table tr:nth-child(even){background:#f8fafc}.message{font-weight:600;color:#475569;margin:10px 0}.kpi-modal{position:fixed;z-index:99999;inset:0;background:rgba(15,23,42,.6);display:flex;align-items:flex-start;justify-content:center;padding:20px;overflow:auto}.kpi-modal-box{background:#fff;width:min(1600px,99vw);max-height:none;margin:auto;border-radius:12px;box-shadow:0 20px 60px rgba(0,0,0,.25);overflow:hidden}.kpi-modal-head{display:flex;align-items:center;justify-content:space-between;padding:14px 18px;background:#17365d;color:#fff;font-size:18px;font-weight:700}.kpi-close{background:none;border:0;color:#fff;font-size:28px;line-height:1;cursor:pointer}.kpi-modal-body{padding:15px;overflow:auto;max-height:calc(100vh - 170px)}.kpi-modal-actions{display:flex;justify-content:flex-end;gap:8px;padding:10px 15px;border-top:1px solid #e2e8f0}.kpi-export{background:#198754;color:#fff;border:0;border-radius:6px;padding:8px 14px;font-weight:600;cursor:pointer}.kpi-modal-body .mis-table{min-width:850px}@media(max-width:1100px){.kpi-grid{grid-template-columns:repeat(3,1fr)}.filter-grid{grid-template-columns:repeat(2,1fr)}}@media(max-width:600px){.kpi-grid,.filter-grid,.summary-grid{grid-template-columns:1fr}.mis-wrap{padding:10px}}

        form > hr + .container-fluid.text-white.py-4{width:100%!important;max-width:100%!important;min-width:0!important;box-sizing:border-box!important;clear:both!important;overflow:hidden!important;margin-left:0!important;margin-right:0!important}
        form > hr + .container-fluid.text-white.py-4 .text-center{width:100%!important;max-width:100%!important;box-sizing:border-box!important;text-align:center!important}
        form > hr + .container-fluid.text-white.py-4 img{max-width:100%!important;height:auto!important;vertical-align:middle!important}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="mis-wrap">
<div class="mis-card">
<div class="mis-title">Training MIS / Management Summary</div>
<div class="filter-grid">
<div class="field"><label>From Date</label><asp:TextBox ID="txtFromDate" runat="server" CssClass="input" placeholder="dd-mm-yyyy" /></div>
<div class="field"><label>To Date</label><asp:TextBox ID="txtToDate" runat="server" CssClass="input" placeholder="dd-mm-yyyy" /></div>
<div class="field"><label>Training Status</label><asp:DropDownList ID="ddlStatus" runat="server" CssClass="input" /></div>
</div>
<div class="actions"><asp:Button ID="btnSearch" runat="server" Text="Generate MIS" CssClass="btnx blue" OnClick="btnSearch_Click" /><asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btnx gray" OnClick="btnReset_Click" /><asp:Button ID="btnExport" runat="server" Text="Export MIS" CssClass="btnx green" OnClick="btnExport_Click" /></div>
</div>
<div class="mis-card"><div class="mis-title">Key Performance Indicators</div>
<div class="kpi-grid">
<div class="kpi k1"><asp:LinkButton ID="lnkTotalTrainings" runat="server" CommandArgument="TotalTrainings" OnClick="kpiCard_Click" CssClass="kpi-link"><div class="kpi-label">Total Trainings</div><asp:Label ID="lblTotalTrainings" runat="server" CssClass="kpi-value" /></asp:LinkButton></div>
<div class="kpi k2"><asp:LinkButton ID="lnkCompletedTrainings" runat="server" CommandArgument="CompletedTrainings" OnClick="kpiCard_Click" CssClass="kpi-link"><div class="kpi-label">Completed Trainings</div><asp:Label ID="lblCompletedTrainings" runat="server" CssClass="kpi-value" /></asp:LinkButton></div>
<div class="kpi k3"><asp:LinkButton ID="lnkOngoingTrainings" runat="server" CommandArgument="OngoingTrainings" CssClass="kpi-link" OnClick="kpiCard_Click"><div class="kpi-label">Ongoing Trainings</div><asp:Label ID="lblOngoingTrainings" runat="server" CssClass="kpi-value" /></asp:LinkButton></div>
<div class="kpi k4"><asp:LinkButton ID="lnkFutureTrainings" runat="server" CommandArgument="FutureTrainings" CssClass="kpi-link" OnClick="kpiCard_Click"><div class="kpi-label">Future Trainings</div><asp:Label ID="lblFutureTrainings" runat="server" CssClass="kpi-value" /></asp:LinkButton></div>
<div class="kpi k5"><asp:LinkButton ID="lnkTotalTrainees" runat="server" CommandArgument="TotalTrainees" CssClass="kpi-link" OnClick="kpiCard_Click"><div class="kpi-label">Total Trainees Trained</div><asp:Label ID="lblTotalTrainees" runat="server" CssClass="kpi-value" /></asp:LinkButton></div>
<div class="kpi k6"><asp:LinkButton ID="lnkTotalSessions" runat="server" CommandArgument="TotalSessions" CssClass="kpi-link" OnClick="kpiCard_Click"><div class="kpi-label">Total Sessions</div><asp:Label ID="lblTotalSessions" runat="server" CssClass="kpi-value" /></asp:LinkButton></div>
<div class="kpi k7"><asp:LinkButton ID="lnkAttendance" runat="server" CommandArgument="Attendance" CssClass="kpi-link" OnClick="kpiCard_Click"><div class="kpi-label">Attendance %</div><asp:Label ID="lblAttendance" runat="server" CssClass="kpi-value" /></asp:LinkButton></div>
<div class="kpi k8"><asp:LinkButton ID="lnkCertificates" runat="server" CommandArgument="Certificates" CssClass="kpi-link" OnClick="kpiCard_Click"><div class="kpi-label">Certificates Generated</div><asp:Label ID="lblCertificates" runat="server" CssClass="kpi-value" /></asp:LinkButton></div>
<div class="kpi k1"><asp:LinkButton ID="lnkFeedback" runat="server" CommandArgument="Feedback" CssClass="kpi-link" OnClick="kpiCard_Click"><div class="kpi-label">Feedback Submitted</div><asp:Label ID="lblFeedback" runat="server" CssClass="kpi-value" /></asp:LinkButton></div>
<div class="kpi k2"><asp:LinkButton ID="lnkPostTest" runat="server" CommandArgument="PostTest" CssClass="kpi-link" OnClick="kpiCard_Click"><div class="kpi-label">Avg Post Test %</div><asp:Label ID="lblPostTest" runat="server" CssClass="kpi-value" /></asp:LinkButton></div><div class="kpi k4"><asp:LinkButton ID="lnkScheduledManHours" runat="server" CommandArgument="ScheduledManHours" CssClass="kpi-link" OnClick="kpiCard_Click"><div class="kpi-label">Man Hours - Training Scheduled</div><asp:Label ID="lblScheduledManHours" runat="server" CssClass="kpi-value" /></asp:LinkButton></div><div class="kpi k5"><asp:LinkButton ID="lnkCompletedManHours" runat="server" CommandArgument="CompletedManHours" CssClass="kpi-link" OnClick="kpiCard_Click"><div class="kpi-label">Man Hours - Training Completed</div><asp:Label ID="lblCompletedManHours" runat="server" CssClass="kpi-value" /></asp:LinkButton></div><div class="kpi k4"><asp:LinkButton ID="lnkTotalManHours" runat="server" CommandArgument="TotalManHours" CssClass="kpi-link" OnClick="kpiCard_Click"><div class="kpi-label">Total Man Hours</div><asp:Label ID="lblTotalManHours" runat="server" CssClass="kpi-value" /></asp:LinkButton></div>
</div></div>
<div class="summary-grid">
<div class="mis-card"><div class="mis-title">Training Status Summary</div><asp:GridView ID="gvStatus" runat="server" AutoGenerateColumns="False" CssClass="mis-table" OnRowCommand="gvSummary_RowCommand">
<Columns>
<asp:BoundField DataField="TrainingStatus" HeaderText="Training Status" />
<asp:TemplateField HeaderText="Count"><ItemTemplate><asp:LinkButton ID="lnkStatusCount" runat="server" Text='<%# Eval("TrainingCount") %>' CommandName="StatusSummary" CommandArgument='<%# Eval("TrainingStatus") %>' CssClass="summary-link" /></ItemTemplate></asp:TemplateField>
</Columns></asp:GridView></div>
<div class="mis-card"><div class="mis-title">Training Type Summary</div><asp:GridView ID="gvType" runat="server" AutoGenerateColumns="False" CssClass="mis-table" OnRowCommand="gvSummary_RowCommand">
<Columns>
<asp:BoundField DataField="TrainingType" HeaderText="Training Type" />
<asp:TemplateField HeaderText="Count"><ItemTemplate><asp:LinkButton ID="lnkTypeCount" runat="server" Text='<%# Eval("TrainingCount") %>' CommandName="TypeSummary" CommandArgument='<%# Eval("TrainingType") %>' CssClass="summary-link" /></ItemTemplate></asp:TemplateField>
</Columns></asp:GridView></div>
</div>
<div class="mis-card"><div class="mis-title">Course-wise Summary</div><asp:GridView ID="gvCourse" runat="server" AutoGenerateColumns="False" CssClass="mis-table" OnRowCommand="gvSummary_RowCommand">
<Columns>
<asp:BoundField DataField="CourseName" HeaderText="Course" />
<asp:TemplateField HeaderText="Count"><ItemTemplate><asp:LinkButton ID="lnkCourseCount" runat="server" Text='<%# Eval("TrainingCount") %>' CommandName="CourseSummary" CommandArgument='<%# Eval("CourseName") %>' CssClass="summary-link" /></ItemTemplate></asp:TemplateField>
<asp:BoundField DataField="TotalTrainingDays" HeaderText="Training Days" />
<asp:BoundField DataField="CompletedTrainingDays" HeaderText="Completed Days" />
<asp:BoundField DataField="ScheduledManHours" HeaderText="Scheduled Man Hours" />
<asp:BoundField DataField="CompletedManHours" HeaderText="Completed Man Hours" />
</Columns>
</asp:GridView></div>
<asp:Panel ID="pnlKpiDetails" runat="server" ClientIDMode="Static" CssClass="kpi-modal" style="display:none;">
<div class="kpi-modal-box">
<div class="kpi-modal-head"><asp:Label ID="lblKpiDetailsTitle" runat="server" /><button type="button" class="kpi-close" onclick="closeKpiDetails();return false;">&times;</button></div>
<div class="kpi-modal-actions"><asp:Button ID="btnExportKpiDetails" runat="server" Text="Export Excel" CssClass="kpi-export" OnClick="btnExportKpiDetails_Click" /></div>
<div class="kpi-modal-body"><asp:GridView ID="gvKpiDetails" runat="server" AutoGenerateColumns="true" CssClass="mis-table" EmptyDataText="No records found." OnRowDataBound="gvKpiDetails_RowDataBound" /></div>
</div>
</asp:Panel>
<script>
function showKpiDetails(){var e=document.getElementById("pnlKpiDetails");if(e)e.style.display="flex";}
function closeKpiDetails(){var e=document.getElementById("pnlKpiDetails");if(e)e.style.display="none";}
document.addEventListener("click",function(e){var m=document.getElementById("pnlKpiDetails");if(m&&e.target===m)closeKpiDetails();});
</script>
<script>document.addEventListener("DOMContentLoaded",function(){if(typeof flatpickr!=="undefined"){flatpickr("#<%= txtFromDate.ClientID %>",{dateFormat:"d-m-Y",allowInput:true});flatpickr("#<%= txtToDate.ClientID %>",{dateFormat:"d-m-Y",allowInput:true});}});</script>
</asp:Content>