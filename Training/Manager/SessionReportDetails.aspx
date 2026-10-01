<%@ Page Title="Session Reports" Language="C#" MasterPageFile="~/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="SessionReportDetails.aspx.cs" Inherits="Training.Manager.SessionReportDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.rd-card{background:#fff;border-radius:12px;box-shadow:0 2px 10px rgba(0,0,0,.08);margin:20px 0;overflow:hidden}.rd-head{background:#0d6efd;color:#fff;padding:18px}.rd-title{font-size:24px;font-weight:700}.rd-info{background:#f8f9fa;border:1px solid #e9ecef;border-radius:8px;padding:14px;height:100%}.rd-label{display:block;font-size:12px;color:#6c757d;font-weight:600}.rd-value{font-weight:700;color:#212529}.rd-btn{min-width:190px;margin:5px}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="rd-card"><div class="rd-head"><div class="rd-title">Session Reports</div><div>Report view only — no training data will be changed.</div></div>
<div class="p-3"><div class="row">
<div class="col-md-3"><div class="rd-info"><span class="rd-label">Training ID</span><asp:Label ID="lblTrainingID" runat="server" CssClass="rd-value" /></div></div>
<div class="col-md-3"><div class="rd-info"><span class="rd-label">Course</span><asp:Label ID="lblCourse" runat="server" CssClass="rd-value" /></div></div>
<div class="col-md-3"><div class="rd-info"><span class="rd-label">Batch</span><asp:Label ID="lblBatch" runat="server" CssClass="rd-value" /></div></div>
<div class="col-md-3"><div class="rd-info"><span class="rd-label">Session</span><asp:Label ID="lblSession" runat="server" CssClass="rd-value" /></div></div>
</div><div class="row mt-3">
<div class="col-md-3"><div class="rd-info"><span class="rd-label">Topic</span><asp:Label ID="lblTopic" runat="server" CssClass="rd-value" /></div></div>
<div class="col-md-3"><div class="rd-info"><span class="rd-label">Session Date</span><asp:Label ID="lblDate" runat="server" CssClass="rd-value" /></div></div>
<div class="col-md-3"><div class="rd-info"><span class="rd-label">Start Time</span><asp:Label ID="lblStart" runat="server" CssClass="rd-value" /></div></div>
<div class="col-md-3"><div class="rd-info"><span class="rd-label">End Time</span><asp:Label ID="lblEnd" runat="server" CssClass="rd-value" /></div></div>
</div></div>
<div class="p-3 border-top"><h5>Session Reports</h5>
<asp:Button ID="btnAttendance" runat="server" Text="Attendance Report" CssClass="btn btn-primary rd-btn" OnClick="btnAttendance_Click" />
<asp:Button ID="btnPreTest" runat="server" Text="Pre-Test Report" CssClass="btn btn-warning rd-btn" OnClick="btnPreTest_Click" />
<asp:Button ID="btnPostTest" runat="server" Text="Post-Test Report" CssClass="btn btn-dark rd-btn" OnClick="btnPostTest_Click" />
<asp:Button ID="btnTestResult" runat="server" Text="Test Result Report" CssClass="btn btn-success rd-btn" OnClick="btnTestResult_Click" />
<asp:Button ID="btnAnswers" runat="server" Text="Answer Details Report" CssClass="btn btn-info rd-btn" OnClick="btnAnswers_Click" />
<asp:Button ID="btnFeedback" runat="server" Text="Feedback Report" CssClass="btn btn-secondary rd-btn" OnClick="btnFeedback_Click" />
<asp:Button ID="btnCertificate" runat="server" Text="Certificate Report" CssClass="btn btn-primary rd-btn" OnClick="btnCertificate_Click" />
<asp:Button ID="btnBack" runat="server" Text="Back to Session Report" CssClass="btn btn-outline-secondary rd-btn" OnClick="btnBack_Click" />
</div></div></div>
</asp:Content>