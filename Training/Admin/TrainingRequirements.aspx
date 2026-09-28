<%@ page title="Training Requirements" language="C#" masterpagefile="~/AdminMaster.Master" autoeventwireup="true" codebehind="TrainingRequirements.aspx.cs" inherits="Training.Admin.TrainingRequirements" MaintainScrollPositionOnPostback="true" %>
<%@ Register Src="~/Admin/TrainingSummary.ascx" TagPrefix="uc" TagName="TrainingSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .main-card { background: linear-gradient(180deg,#f8fbff 0%,#ffffff 28%); padding: 25px; border-radius: 16px; box-shadow: 0 8px 28px rgba(25,60,100,.12); margin-top: 20px; border: 1px solid #e3eaf3 }

        .heading { font-size: 26px; font-weight: 700; margin-bottom: 20px; color: #174a7e; padding: 14px 18px; border-radius: 12px; background: linear-gradient(90deg,#e8f3ff,#f3f9ff); border-left: 6px solid #0d6efd }

        .req-card { border: 1px solid #dbe5ef; border-radius: 14px; padding: 20px; margin-bottom: 20px; background: #ffffff; box-shadow: 0 4px 16px rgba(25,60,100,.07) }
        .req-card h5 { color: #174a7e; font-weight: 700; margin-bottom: 6px }
        .req-card > p { font-size: 13px }

        .req-box { border: 1px solid #dfe7f0; border-radius: 12px; padding: 16px; height: 100%; background: linear-gradient(145deg,#ffffff,#f7fbff); box-shadow: 0 3px 10px rgba(25,60,100,.05); transition: transform .15s ease,box-shadow .15s ease }
        .req-box:hover { transform: translateY(-2px); box-shadow: 0 6px 18px rgba(25,60,100,.10) }
        .req-box > b { color: #263b53; font-size: 16px }

        .req-status {
            display: inline-block;
            margin: 6px 0 10px
        }

        .reason {
            max-width: 500px
        }

        .table td, .table th {
            vertical-align: middle
        }

        .action-btn { margin: 2px 4px 2px 0; border-radius: 7px; font-weight: 600 }
        .table { border-radius: 10px; overflow: hidden; border-color: #d9e3ee !important }
        .table thead th { background: linear-gradient(90deg,#174a7e,#2468a3); color: #fff; border-color: #174a7e; font-weight: 600 }
        .table tbody tr:nth-child(even) { background: #f7fbff }
        .table tbody tr:hover { background: #eef6ff }
        .table td { padding: 10px; }
        .req-status { padding: 6px 10px; border-radius: 20px; font-weight: 600 }
    </style>
</asp:Content>
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card">
<div class="heading">Training Requirements</div>
        <uc:TrainingSummary ID="TrainingSummary1" runat="server" />

<asp:Label ID="lblTraining" runat="server" CssClass="fw-bold" Visible="false" />

<div class="req-card mt-3"><h5>Batch-wise Requirements</h5><p class="text-muted">Admin can change any requirement from Required to Not Required or from Not Required to Required after the batch is created.</p>
<div class="row g-3">
<div class="col-md-6 col-lg-4"><div class="req-box"><b>Attendance</b><br /><asp:Label ID="lblAttendanceStatus" runat="server" CssClass="badge bg-secondary req-status" /><br /><asp:Button ID="btnAttendanceRequired" runat="server" Text="Make Required" CssClass="btn btn-sm btn-outline-success action-btn" OnClick="btnAttendanceRequired_Click" /><asp:Button ID="btnAttendanceNotRequired" runat="server" Text="Make Not Required" CssClass="btn btn-sm btn-outline-secondary action-btn" OnClick="btnAttendanceNotRequired_Click" /></div></div>
<div class="col-md-6 col-lg-4"><div class="req-box"><b>Pre-Test</b><br /><asp:Label ID="lblPreStatus" runat="server" CssClass="badge bg-secondary req-status" /><br /><asp:Button ID="btnPreRequired" runat="server" Text="Make Required" CssClass="btn btn-sm btn-outline-success action-btn" OnClick="btnPreRequired_Click" /><asp:Button ID="btnPreNotRequired" runat="server" Text="Make Not Required" CssClass="btn btn-sm btn-outline-secondary action-btn" OnClick="btnPreNotRequired_Click" /></div></div>
<div class="col-md-6 col-lg-4"><div class="req-box"><b>Post-Test</b><br /><asp:Label ID="lblPostStatus" runat="server" CssClass="badge bg-secondary req-status" /><br /><asp:Button ID="btnPostRequired" runat="server" Text="Make Required" CssClass="btn btn-sm btn-outline-success action-btn" OnClick="btnPostRequired_Click" /><asp:Button ID="btnPostNotRequired" runat="server" Text="Make Not Required" CssClass="btn btn-sm btn-outline-secondary action-btn" OnClick="btnPostNotRequired_Click" /></div></div>
<div class="col-md-6 col-lg-6"><div class="req-box"><b>Feedback</b><br /><asp:Label ID="lblFeedbackStatus" runat="server" CssClass="badge bg-secondary req-status" /><br /><asp:Button ID="btnFeedbackRequired" runat="server" Text="Make Required" CssClass="btn btn-sm btn-outline-success action-btn" OnClick="btnFeedbackRequired_Click" /><asp:Button ID="btnFeedbackNotRequired" runat="server" Text="Make Not Required" CssClass="btn btn-sm btn-outline-secondary action-btn" OnClick="btnFeedbackNotRequired_Click" /><asp:TextBox ID="txtFeedbackReason" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control reason mt-2" placeholder="Reason required when skipping Feedback" /><asp:Button ID="btnFeedback" runat="server" CssClass="btn btn-outline-danger mt-2 action-btn" OnClick="btnFeedback_Click" /></div></div>
<div class="col-md-6 col-lg-6"><div class="req-box"><b>Certificate</b><br /><asp:Label ID="lblCertificateStatus" runat="server" CssClass="badge bg-secondary req-status" /><br /><asp:Button ID="btnCertificateRequired" runat="server" Text="Make Required" CssClass="btn btn-sm btn-outline-success action-btn" OnClick="btnCertificateRequired_Click" /><asp:Button ID="btnCertificateNotRequired" runat="server" Text="Make Not Required" CssClass="btn btn-sm btn-outline-secondary action-btn" OnClick="btnCertificateNotRequired_Click" /><br /><asp:TextBox ID="txtCertificateReason" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control reason mt-2" placeholder="Reason required when skipping Certificate" /><asp:Button ID="btnCertificate" runat="server" CssClass="btn btn-outline-danger mt-2 action-btn" OnClick="btnCertificate_Click" /></div></div>
</div></div>

<div class="req-card"><h5>Session-wise Skip / Unskip</h5><p class="text-muted">Trainer/Admin can skip a session requirement. Admin can always unskip it again. Skipped sessions do not block the workflow.</p>
<asp:GridView ID="gvSessions" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped" DataKeyNames="SessionID" OnRowCommand="gvSessions_RowCommand"><Columns>
<asp:BoundField DataField="SessionID" HeaderText="Session" />
<asp:TemplateField HeaderText="Attendance"><ItemTemplate><asp:Label runat="server" Text='<%# Convert.ToBoolean(Eval("AttendanceSkipped")) ? "Skipped" : (Convert.ToBoolean(Eval("AttendanceRequired")) ? "Required" : "Not Required") %>' CssClass='<%# Convert.ToBoolean(Eval("AttendanceSkipped")) ? "badge bg-secondary" : (Convert.ToBoolean(Eval("AttendanceRequired")) ? "badge bg-success" : "badge bg-light text-dark") %>' /><br /><asp:TextBox ID="txtAttendanceReason" runat="server" CssClass="form-control mt-1" placeholder="Skip reason" /><asp:Button ID="btnAttendanceReq" runat="server" Text='<%# Convert.ToBoolean(Eval("AttendanceSkipped")) ? "Unskip" : "Skip" %>' CommandName="Attendance" CommandArgument='<%# Eval("SessionID") %>' CssClass="btn btn-sm btn-outline-danger mt-1" /></ItemTemplate></asp:TemplateField>
<asp:TemplateField HeaderText="Pre-Test"><ItemTemplate><asp:Label runat="server" Text='<%# Convert.ToBoolean(Eval("PreAssessmentSkipped")) ? "Skipped" : (Convert.ToBoolean(Eval("InitialAssessmentRequired")) ? "Required" : "Not Required") %>' CssClass='<%# Convert.ToBoolean(Eval("PreAssessmentSkipped")) ? "badge bg-secondary" : (Convert.ToBoolean(Eval("InitialAssessmentRequired")) ? "badge bg-success" : "badge bg-light text-dark") %>' /><br /><asp:TextBox ID="txtPreReason" runat="server" CssClass="form-control mt-1" placeholder="Skip reason" /><asp:Button ID="btnPreReq" runat="server" Text='<%# Convert.ToBoolean(Eval("PreAssessmentSkipped")) ? "Unskip" : "Skip" %>' CommandName="Pre" CommandArgument='<%# Eval("SessionID") %>' CssClass="btn btn-sm btn-outline-danger mt-1" /></ItemTemplate></asp:TemplateField>
<asp:TemplateField HeaderText="Post-Test"><ItemTemplate><asp:Label runat="server" Text='<%# Convert.ToBoolean(Eval("PostAssessmentSkipped")) ? "Skipped" : (Convert.ToBoolean(Eval("FinalAssessmentRequired")) ? "Required" : "Not Required") %>' CssClass='<%# Convert.ToBoolean(Eval("PostAssessmentSkipped")) ? "badge bg-secondary" : (Convert.ToBoolean(Eval("FinalAssessmentRequired")) ? "badge bg-success" : "badge bg-light text-dark") %>' /><br /><asp:TextBox ID="txtPostReason" runat="server" CssClass="form-control mt-1" placeholder="Skip reason" /><asp:Button ID="btnPostReq" runat="server" Text='<%# Convert.ToBoolean(Eval("PostAssessmentSkipped")) ? "Unskip" : "Skip" %>' CommandName="Post" CommandArgument='<%# Eval("SessionID") %>' CssClass="btn btn-sm btn-outline-danger mt-1" /></ItemTemplate></asp:TemplateField>
</Columns></asp:GridView></div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true" />
</div></div>
</asp:content>
