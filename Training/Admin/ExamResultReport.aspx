<%@ Page Title="Exam Result Report"
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="ExamResultReport.aspx.cs"
    Inherits="Training.Admin.ExamResultReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .exam-report-page{padding:22px;background:#f5f7fb;min-height:calc(100vh - 120px);box-sizing:border-box;color:#1e293b}
        .exam-report-page .exam-header{display:flex;justify-content:space-between;align-items:center;gap:16px;flex-wrap:wrap;margin-bottom:18px}
        .exam-report-page .page-title{font-size:26px;font-weight:700;color:#17365d;display:block}
        .exam-report-page .page-subtitle{font-size:13px;color:#64748b;margin-top:4px}
        .exam-report-page .report-card{background:#fff;border:1px solid #e2e8f0;border-radius:14px;box-shadow:0 4px 16px rgba(15,23,42,.07);margin-bottom:18px;overflow:hidden}
        .exam-report-page .report-card-header{padding:14px 18px;background:linear-gradient(135deg,#17365d,#2563eb);color:#fff;display:flex;justify-content:space-between;align-items:center;gap:12px;flex-wrap:wrap}
        .exam-report-page .comparison-header{background:linear-gradient(135deg,#0f766e,#0891b2)}
        .exam-report-page .section-title{font-size:18px;font-weight:700}
        .exam-report-page .card-body{padding:18px}
        .exam-report-page .filter-label{display:block;font-size:13px;font-weight:700;color:#334155;margin-bottom:6px}
        .exam-report-page .form-control{height:40px;border:1px solid #cbd5e1;border-radius:7px;box-shadow:none}
        .exam-report-page .form-control:focus{border-color:#2563eb;box-shadow:0 0 0 3px rgba(37,99,235,.12)}
        .exam-report-page .filter-actions{display:flex;align-items:flex-end;gap:8px;height:100%}
        .exam-report-page .summary-grid{display:grid;grid-template-columns:repeat(4,1fr);gap:14px;margin-bottom:18px}
        .exam-report-page .summary-card-wrap{min-width:0}.exam-report-page .summary-box{background:#fff;border:1px solid #e2e8f0;border-radius:12px;padding:18px;text-align:left;min-height:105px;box-shadow:0 3px 12px rgba(15,23,42,.05);position:relative;overflow:hidden}
        .exam-report-page .summary-box:before{content:"";position:absolute;left:0;top:0;bottom:0;width:4px;background:#2563eb}
        .exam-report-page .summary-box.passed:before{background:#16a34a}
        .exam-report-page .summary-box.failed:before{background:#dc2626}
        .exam-report-page .summary-box.average:before{background:#7c3aed}
        .exam-report-page .summary-title{display:block;font-size:12px;text-transform:uppercase;letter-spacing:.04em;font-weight:700;color:#64748b;margin-bottom:9px}
        .exam-report-page .summary-value{display:block;font-size:28px;line-height:1.1;font-weight:800;color:#2563eb}
        .exam-report-page .passed-value{color:#16a34a}.exam-report-page .failed-value{color:#dc2626}.exam-report-page .average-value{color:#7c3aed}
        .exam-report-page .message-area{display:block;margin:0 0 12px;font-weight:600;color:#475569}
        .exam-report-page .table-wrap{width:100%;overflow-x:auto;overflow-y:visible;border:1px solid #e2e8f0;border-radius:8px}
        .exam-report-page .report-table{width:100%;min-width:1850px;margin:0;border-collapse:separate;border-spacing:0}
        .exam-report-page .report-table th{background:#17365d;color:#fff;text-align:center;vertical-align:middle;white-space:nowrap;font-weight:700;padding:11px 9px;border:0;border-right:1px solid rgba(255,255,255,.16)}
        .exam-report-page .report-table td{vertical-align:middle;padding:9px;border:0;border-bottom:1px solid #e2e8f0;white-space:nowrap;background:#fff}
        .exam-report-page .report-table tr:hover td{background:#f8fafc}
        .exam-report-page .comparison-table{min-width:900px}
        .exam-report-page .empty-data{text-align:center;padding:28px;color:#64748b;font-weight:600}
        .exam-report-page .result-pass{color:#15803d;font-weight:800;background:#dcfce7;padding:4px 9px;border-radius:20px;display:inline-block}
        .exam-report-page .result-fail{color:#b91c1c;font-weight:800;background:#fee2e2;padding:4px 9px;border-radius:20px;display:inline-block}
        .exam-report-page .percentage-text{font-weight:800;color:#1d4ed8}
        .exam-report-page .improvement-positive{color:#15803d;font-weight:800}.exam-report-page .improvement-negative{color:#b91c1c;font-weight:800}
        .exam-report-page .export-btn{white-space:nowrap}
        @media(max-width:900px){.exam-report-page{padding:12px}.exam-report-page .summary-grid{grid-template-columns:repeat(2,1fr)}.exam-report-page .card-body{padding:14px}}
        @media(max-width:575px){.exam-report-page .summary-grid{grid-template-columns:1fr}.exam-report-page .page-title{font-size:22px}.exam-report-page .filter-actions{align-items:stretch}.exam-report-page .filter-actions .btn{flex:1}}
        .exam-report-page .filter-hint{font-size:12px;opacity:.88;margin-top:3px}
        .exam-report-page .filter-toolbar{display:flex;gap:8px;align-items:center}
        .exam-report-page .filter-body{padding:20px}
        .exam-report-page .filter-grid{display:grid;grid-template-columns:repeat(4,minmax(0,1fr));gap:16px}
        .exam-report-page .filter-item{min-width:0}
        .exam-report-page .filter-wide{grid-column:span 1}
        .exam-report-page .exam-date{background:#fff url('data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%2216%22 height=%2216%22 fill=%22%2364758b%22 viewBox=%220 0 16 16%3E%3Cpath d=%22M3.5 0a.5.5 0 0 1 .5.5V1h8V.5a.5.5 0 0 1 1 0V1h.5A1.5 1.5 0 0 1 15 2.5v12a1.5 1.5 0 0 1-1.5 1.5h-11A1.5 1.5 0 0 1 1 14.5v-12A1.5 1.5 0 0 1 2.5 1H3V.5a.5.5 0 0 1 .5-.5zM2 5v9.5a.5.5 0 0 0 .5.5h11a.5.5 0 0 0 .5-.5V5H2z%22/%3E%3C/svg%3E') no-repeat right 12px center;background-size:16px;padding-right:38px}
        .exam-report-page .result-toolbar{display:flex;align-items:center;justify-content:space-between;gap:12px}
        .exam-report-page .result-count{font-size:12px;opacity:.9}
        .exam-report-page .report-table th{position:sticky;top:0;z-index:2}
        @media(max-width:1100px){.exam-report-page .filter-grid{grid-template-columns:repeat(3,minmax(0,1fr))}}
        @media(max-width:760px){.exam-report-page .filter-grid{grid-template-columns:repeat(2,minmax(0,1fr))}.exam-report-page .filter-wide{grid-column:span 1}}
        @media(max-width:520px){.exam-report-page .filter-grid{grid-template-columns:1fr}.exam-report-page .filter-toolbar{width:100%}.exam-report-page .filter-toolbar .btn{flex:1}}
        .exam-report-page .metric-main{font-weight:700;color:#1e293b}.exam-report-page .metric-sub{font-size:11px;color:#64748b;margin-top:2px}.exam-report-page .score-badge{display:inline-block;background:#eff6ff;color:#1d4ed8;border:1px solid #bfdbfe;border-radius:6px;padding:4px 8px;font-weight:700}.exam-report-page .answer-btn{white-space:nowrap}
            .exam-report-page .compact-filters{grid-template-columns:1.4fr 1fr 1.4fr 1fr;align-items:end}
        .exam-report-page .filter-toolbar{display:flex;gap:8px;align-items:end}
        .exam-report-page .result-count{margin:12px 0;font-size:13px;font-weight:700;color:#475569}
        .exam-report-page .breadcrumb-bar{display:flex;align-items:center;gap:10px;flex-wrap:wrap;margin:0 0 14px;padding:12px 16px;background:#fff;border:1px solid #e2e8f0;border-radius:10px;box-shadow:0 3px 10px rgba(15,23,42,.05)}
        .exam-report-page .back-link{font-weight:700;color:#2563eb;text-decoration:none}
        .exam-report-page .report-table{width:100%;min-width:1150px;border-collapse:separate;border-spacing:0}
        .exam-report-page .report-table th{background:#17365d;color:#fff;text-align:center;white-space:nowrap;padding:11px 9px}
        .exam-report-page .report-table td{padding:9px;border-bottom:1px solid #e2e8f0;white-space:nowrap;background:#fff}
        .exam-report-page .report-table tr:hover td{background:#f8fafc}
        @media(max-width:900px){.exam-report-page .compact-filters{grid-template-columns:repeat(2,minmax(0,1fr))}}
        @media(max-width:575px){.exam-report-page .compact-filters{grid-template-columns:1fr}}
.exam-report-page .training-link{color:#2563eb;font-weight:700;text-decoration:none}.exam-report-page .training-info-modal{position:fixed;inset:0;display:none;align-items:center;justify-content:center;background:rgba(15,23,42,.58);z-index:99999;padding:20px}.exam-report-page .training-info-box{width:100%;max-width:650px;background:#fff;border-radius:12px;box-shadow:0 20px 60px rgba(0,0,0,.28);overflow:hidden}.exam-report-page .training-info-header{display:flex;align-items:center;justify-content:space-between;padding:14px 18px;background:#17365d;color:#fff;font-size:18px;font-weight:700}.exam-report-page .training-info-close{border:0;background:transparent;color:#fff;font-size:28px;line-height:1;cursor:pointer}.exam-report-page .training-info-body{padding:18px;max-height:calc(100vh - 180px);overflow:auto}.exam-report-page .training-info-table{width:100%;border-collapse:collapse}.exam-report-page .training-info-table th{width:38%;padding:10px;background:#f8fafc;text-align:left;border-bottom:1px solid #e2e8f0}.exam-report-page .training-info-table td{padding:10px;border-bottom:1px solid #e2e8f0;color:#334155}</style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="exam-report-page">
    <div class="exam-header">
        <div>
            <span class="page-title">Exam Result Report</span>
            <div class="page-subtitle">Training → Session → Exam Result</div>
        </div>
        <asp:Label ID="lblMessage" runat="server" CssClass="message-area" />
    </div>

    <asp:Panel ID="pnlTrainingList" runat="server">
        <div class="report-card">
            <div class="report-card-header">
                <div>
                    <span class="section-title">Training List</span>
                    <div class="filter-hint">Select a training to view its sessions and exam results.</div>
                </div>
                <asp:Button ID="btnExportTraining" runat="server" Text="Export Training List" CssClass="btn btn-light btn-sm" OnClick="btnExportTraining_Click" />
            </div>
            <div class="card-body">
                <div class="filter-grid compact-filters">
                    <div class="filter-item">
                        <label class="filter-label">Course</label>
                        <asp:DropDownList ID="ddlCourseFilter" runat="server" CssClass="form-control" />
                    </div>
                    <div class="filter-item">
                        <label class="filter-label">Training Status</label>
                        <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control" />
                    </div>
                    <div class="filter-item">
                        <label class="filter-label">Training ID / Batch</label>
                        <asp:TextBox ID="txtTrainingSearch" runat="server" CssClass="form-control" placeholder="Search Training ID or Batch" />
                    </div>
                    <div class="filter-toolbar filter-item">
                        <asp:Button ID="btnSearchTraining" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearchTraining_Click" />
                        <asp:Button ID="btnResetTraining" runat="server" Text="Reset" CssClass="btn btn-outline-secondary" CausesValidation="false" OnClick="btnResetTraining_Click" />
                    </div>
                </div>
                <div class="result-count"><asp:Label ID="lblTrainingCount" runat="server" /></div>
                <div class="table-wrap">
                    <asp:GridView ID="gvTrainingList" runat="server" AutoGenerateColumns="False" CssClass="report-table" OnRowCommand="gvTrainingList_RowCommand" EmptyDataText="No training found.">
                        <Columns>
                            <asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate></asp:TemplateField>
                            <asp:TemplateField HeaderText="Training ID"><ItemTemplate><asp:LinkButton ID="lnkTrainingInfo" runat="server" Text="<%# Eval(&quot;TrainingID&quot;) %>" CssClass="training-link" CommandName="TrainingInfo" CommandArgument="<%# Eval(&quot;TrainingID&quot;) %>" /></ItemTemplate></asp:TemplateField>
                            <asp:BoundField DataField="CourseName" HeaderText="Course" />
                            <asp:BoundField DataField="TrainingStatus" HeaderText="Status" />
                            <asp:BoundField DataField="SessionCount" HeaderText="Sessions" />
                            <asp:BoundField DataField="TraineeCount" HeaderText="Trainees" />
                            <asp:BoundField DataField="PreResultCount" HeaderText="Pre Results" />
                            <asp:BoundField DataField="PostResultCount" HeaderText="Post Results" />
                            <asp:TemplateField HeaderText="Action"><ItemTemplate><asp:LinkButton ID="btnViewTraining" runat="server" Text="View Sessions" CssClass="btn btn-sm btn-primary" CommandName="ViewTraining" CommandArgument='<%# Eval("TrainingID") %>' /></ItemTemplate></asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </asp:Panel>

    
<asp:Panel ID="pnlTrainingInfoModal" runat="server" ClientIDMode="Static" CssClass="training-info-modal" style="display:none;">
<div class="training-info-box">
<div class="training-info-header"><asp:Label ID="lblTrainingInfoTitle" runat="server" Text="Training Information" /><button type="button" class="training-info-close" onclick="closeTrainingInfo();return false;">&times;</button></div>
<div class="training-info-body">
<table class="training-info-table">
<tr><th>Training ID</th><td><asp:Label ID="lblInfoTrainingID" runat="server" /></td></tr>
<tr><th>Course</th><td><asp:Label ID="lblInfoCourse" runat="server" /></td></tr>
<tr><th>Batch</th><td><asp:Label ID="lblInfoBatch" runat="server" /></td></tr>
<tr><th>Training Type</th><td><asp:Label ID="lblInfoType" runat="server" /></td></tr>
<tr><th>Training Organizer</th><td><asp:Label ID="lblInfoOrganizer" runat="server" /></td></tr>
<tr><th>Training Location</th><td><asp:Label ID="lblInfoLocation" runat="server" /></td></tr>
<tr><th>From Date</th><td><asp:Label ID="lblInfoFrom" runat="server" /></td></tr>
<tr><th>To Date</th><td><asp:Label ID="lblInfoTo" runat="server" /></td></tr>
<tr><th>No. of Days</th><td><asp:Label ID="lblInfoDays" runat="server" /></td></tr>
<tr><th>Status</th><td><asp:Label ID="lblInfoStatus" runat="server" /></td></tr>
</table></div></div></asp:Panel>
<script>function showTrainingInfo(){var e=document.getElementById("pnlTrainingInfoModal");if(e)e.style.display="flex";}function closeTrainingInfo(){var e=document.getElementById("pnlTrainingInfoModal");if(e)e.style.display="none";}</script>
<asp:Panel ID="pnlSessionList" runat="server" Visible="false">
        <div class="breadcrumb-bar">
            <asp:LinkButton ID="btnBackTraining" runat="server" Text="← Training List" CssClass="back-link" OnClick="btnBackTraining_Click" />
            <span>/</span>
            <strong><asp:Label ID="lblSelectedTraining" runat="server" /></strong>
        </div>
        <div class="report-card">
            <div class="report-card-header">
                <div><span class="section-title">Session List</span><div class="filter-hint">Click View Results to see the exam result of that session.</div></div>
                <asp:Button ID="btnExportSessions" runat="server" Text="Export Sessions" CssClass="btn btn-light btn-sm" OnClick="btnExportSessions_Click" />
            </div>
            <div class="card-body">
                <div class="table-wrap">
                    <asp:GridView ID="gvSessionList" runat="server" AutoGenerateColumns="False" CssClass="report-table" OnRowCommand="gvSessionList_RowCommand" EmptyDataText="No sessions found.">
                        <Columns>
                            <asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate></asp:TemplateField>
                            <asp:BoundField DataField="SessionID" HeaderText="Session ID" />
                            <asp:BoundField DataField="SessionNo" HeaderText="Session No" />
                            <asp:BoundField DataField="SessionName" HeaderText="Session" />
                            <asp:BoundField DataField="TopicName" HeaderText="Topic" />
                            <asp:BoundField DataField="SessionDate" HeaderText="Date" />
                            <asp:BoundField DataField="StartTime" HeaderText="Start" />
                            <asp:BoundField DataField="EndTime" HeaderText="End" />
                            <asp:BoundField DataField="TrainerName" HeaderText="Trainer" />
                            <asp:BoundField DataField="SessionStatus" HeaderText="Status" />
                            <asp:BoundField DataField="PreResults" HeaderText="Pre Results" />
                            <asp:BoundField DataField="PostResults" HeaderText="Post Results" />
                            <asp:TemplateField HeaderText="Action"><ItemTemplate><asp:LinkButton ID="btnViewResults" runat="server" Text="View Results" CssClass="btn btn-sm btn-success" CommandName="ViewResults" CommandArgument='<%# Eval("SessionID") %>' /></ItemTemplate></asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlResultList" runat="server" Visible="false">
        <div class="breadcrumb-bar">
            <asp:LinkButton ID="btnBackSessions" runat="server" Text="← Session List" CssClass="back-link" OnClick="btnBackSessions_Click" />
            <span>/</span><strong><asp:Label ID="lblSelectedSession" runat="server" /></strong>
        </div>
        <div class="summary-grid">
            <div class="summary-card-wrap"><div class="summary-box"><span class="summary-title">Results</span><asp:Label ID="lblResultCount" runat="server" CssClass="summary-value" /></div></div>
            <div class="summary-card-wrap"><div class="summary-box passed"><span class="summary-title">Passed</span><asp:Label ID="lblPassed" runat="server" CssClass="summary-value passed-value" /></div></div>
            <div class="summary-card-wrap"><div class="summary-box failed"><span class="summary-title">Failed</span><asp:Label ID="lblFailed" runat="server" CssClass="summary-value failed-value" /></div></div>
            <div class="summary-card-wrap"><div class="summary-box average"><span class="summary-title">Average %</span><asp:Label ID="lblAveragePercentage" runat="server" CssClass="summary-value average-value" /></div></div>
        </div>
        <div class="report-card">
            <div class="report-card-header">
                <div><span class="section-title">Exam Results</span><div class="filter-hint">Trainee-wise result for this session</div></div>
                <asp:Button ID="btnExportResults" runat="server" Text="Export Results" CssClass="btn btn-light btn-sm" OnClick="btnExportResults_Click" />
            </div>
            <div class="card-body">
                <div class="table-wrap">
                    <asp:GridView ID="gvResultList" runat="server" AutoGenerateColumns="False" CssClass="report-table" EmptyDataText="No exam result found for this session." OnRowDataBound="gvResultList_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate></asp:TemplateField>
                            <asp:BoundField DataField="TestType" HeaderText="Exam" />
                            <asp:BoundField DataField="TestTitle" HeaderText="Test" />
                            <asp:BoundField DataField="EmpID" HeaderText="Trainee ID" />
                            <asp:BoundField DataField="TraineeName" HeaderText="Trainee Name" />
                            <asp:BoundField DataField="AttemptNo" HeaderText="Attempt" />
                            <asp:BoundField DataField="TotalQuestions" HeaderText="Total Q." />
                            <asp:BoundField DataField="AttemptedQuestions" HeaderText="Attempted" />
                            <asp:BoundField DataField="CorrectAnswers" HeaderText="Correct" />
                            <asp:BoundField DataField="WrongAnswers" HeaderText="Wrong" />
                            <asp:BoundField DataField="TotalMarks" HeaderText="Total Marks" />
                            <asp:BoundField DataField="ObtainedMarks" HeaderText="Obtained" />
                            <asp:BoundField DataField="Percentage" HeaderText="Percentage" />
                            <asp:BoundField DataField="ResultStatus" HeaderText="Result" />
                            <asp:BoundField DataField="RankNo" HeaderText="Rank" />
                            <asp:BoundField DataField="SubmittedOn" HeaderText="Submitted On" />
                            <asp:TemplateField HeaderText="Action"><ItemTemplate><asp:HyperLink ID="lnkAnswers" runat="server" Text="View Answers" CssClass="btn btn-sm btn-outline-primary" NavigateUrl='<%# "AnswerDetails.aspx?ResultID=" + Eval("ResultID") %>' /></ItemTemplate></asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </asp:Panel>
</div>
</asp:Content>