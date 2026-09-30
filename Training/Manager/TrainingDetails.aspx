<%@ Page Title="Training Details" Language="C#" MasterPageFile="~/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="TrainingDetails.aspx.cs" Inherits="Training.Manager.TrainingDetails" MaintainScrollPositionOnPostback="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.card{background:#fff;border-radius:12px;box-shadow:0 2px 10px rgba(0,0,0,.08);padding:20px;margin-bottom:20px}
.page-title{font-size:28px;font-weight:600;color:#1e293b;margin-bottom:20px}
.info-label{font-size:13px;color:#64748b;font-weight:600}.info-value{font-size:16px;color:#1e293b;font-weight:600}
.gridview th{background:#198754;color:#fff;white-space:nowrap}.gridview td{vertical-align:middle;white-space:nowrap}.action-btn{min-width:160px;margin:5px}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid py-4">
<div class="page-title">Training Details</div>
<asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" Font-Bold="true" />
<div class="card">
<h5 class="mb-3">Training Summary</h5>
<div class="row">
<div class="col-md-3 mb-3"><div class="info-label">Training ID</div><div class="info-value"><asp:Label ID="lblTrainingID" runat="server" /></div></div>
<div class="col-md-3 mb-3"><div class="info-label">Training Type</div><div class="info-value"><asp:Label ID="lblTrainingType" runat="server" /></div></div>
<div class="col-md-3 mb-3"><div class="info-label">Organizer</div><div class="info-value"><asp:Label ID="lblOrganizer" runat="server" /></div></div>
<div class="col-md-3 mb-3"><div class="info-label">Training Location</div><div class="info-value"><asp:Label ID="lblTrainingLocation" runat="server" /></div></div>
<div class="col-md-3 mb-3"><div class="info-label">Batch</div><div class="info-value"><asp:Label ID="lblBatch" runat="server" /></div></div>
<div class="col-md-3 mb-3"><div class="info-label">From</div><div class="info-value"><asp:Label ID="lblDateFrom" runat="server" /></div></div>
<div class="col-md-3 mb-3"><div class="info-label">To</div><div class="info-value"><asp:Label ID="lblDateTo" runat="server" /></div></div>
<div class="col-md-3 mb-3"><div class="info-label">Status</div><div class="info-value"><asp:Label ID="lblStatus" runat="server" /></div></div>
</div>
</div>
<div class="card">
<h5 class="mb-3">Sessions</h5>
<div class="table-responsive">
<asp:GridView ID="gvSession" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Session Found" ShowHeaderWhenEmpty="true" DataKeyNames="SessionID">
<Columns>
<asp:TemplateField HeaderText="Sl No"><ItemTemplate><%# Container.DataItemIndex+1 %></ItemTemplate></asp:TemplateField>
<asp:BoundField DataField="SessionID" HeaderText="Session ID" />
<asp:BoundField DataField="SessionNo" HeaderText="Session No" />
<asp:BoundField DataField="SessionName" HeaderText="Session Name" />
<asp:BoundField DataField="SessionDate" HeaderText="Session Date" />
<asp:BoundField DataField="TrainerName" HeaderText="Trainer" />
<asp:TemplateField HeaderText="Select"><ItemTemplate><asp:RadioButton ID="rbSession" runat="server" GroupName="ManagerSession" /></ItemTemplate></asp:TemplateField>
</Columns>
</asp:GridView>
</div>
<div class="mt-3">
<asp:Button ID="btnMaterial" runat="server" Text="Upload Materials" CssClass="btn btn-success action-btn" OnClick="btnMaterial_Click" />
<asp:Button ID="btnAttendance" runat="server" Text="Attendance" CssClass="btn btn-info action-btn" OnClick="btnAttendance_Click" />
<asp:Button ID="btnPreTest" runat="server" Text="Pre Training Test" CssClass="btn btn-warning action-btn" OnClick="btnPreTest_Click" />
<asp:Button ID="btnPostTest" runat="server" Text="Post Training Test" CssClass="btn btn-dark action-btn" OnClick="btnPostTest_Click" />
<asp:Button ID="btnResult" runat="server" Text="Test Result" CssClass="btn btn-primary action-btn" OnClick="btnResult_Click" />
</div>
</div>
</div>
</asp:Content>