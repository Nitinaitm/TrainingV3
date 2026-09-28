<%@ Page Title="Session Report" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="SessionReport.aspx.cs" Inherits="Training.Trainer.SessionReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.sr-card{background:#fff;border-radius:12px;box-shadow:0 2px 10px rgba(0,0,0,.08);margin:20px 0;overflow:hidden}.sr-title{font-size:24px;font-weight:700;color:#0d6efd}.sr-table th{background:#0d6efd;color:#fff;white-space:nowrap;text-align:center}.sr-table td{vertical-align:middle;white-space:nowrap}.sr-filter{background:#f8f9fa;border-bottom:1px solid #dee2e6;padding:15px}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="sr-card">
<div class="p-3"><span class="sr-title">Session Report</span><div class="text-muted mt-1">View session-wise reports for trainings assigned to you.</div></div>
<div class="sr-filter"><div class="row">
<div class="col-md-3"><label>Course</label><asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-control" /></div>
<div class="col-md-3"><label>Batch</label><asp:TextBox ID="txtBatch" runat="server" CssClass="form-control" /></div>
<div class="col-md-3"><label>Session Date From</label><asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy" /></div>
<div class="col-md-3"><label>Session Date To</label><asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy" /></div>
</div><div class="mt-3"><asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" /><asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary ml-2" CausesValidation="false" OnClick="btnReset_Click" /></div></div>
<div class="p-3"><asp:Label ID="lblMessage" runat="server" /><div class="table-responsive"><asp:GridView ID="gvSession" runat="server" AutoGenerateColumns="false" DataKeyNames="SessionID,TrainingID" CssClass="table table-bordered table-hover sr-table" OnRowCommand="gvSession_RowCommand">
<Columns>
<asp:TemplateField HeaderText="Sl. No."><ItemTemplate><%# Container.DataItemIndex+1 %></ItemTemplate><ItemStyle HorizontalAlign="Center" /></asp:TemplateField>
<asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
<asp:BoundField DataField="CourseName" HeaderText="Course" />
<asp:BoundField DataField="Batch" HeaderText="Batch" />
<asp:BoundField DataField="SessionNo" HeaderText="Session No" />
<asp:BoundField DataField="SessionName" HeaderText="Session Name" />
<asp:BoundField DataField="TopicName" HeaderText="Topic" />
<asp:BoundField DataField="SessionDate" HeaderText="Session Date" />
<asp:BoundField DataField="StartTime" HeaderText="Start Time" />
<asp:BoundField DataField="EndTime" HeaderText="End Time" />
<asp:TemplateField HeaderText="Action"><ItemTemplate><asp:LinkButton ID="btnView" runat="server" Text="View Reports" CssClass="btn btn-sm btn-primary" CommandName="View" CausesValidation="false" /></ItemTemplate><ItemStyle HorizontalAlign="Center" /></asp:TemplateField>
</Columns>
<EmptyDataTemplate><div class="text-center p-4 text-muted">No sessions found.</div></EmptyDataTemplate>
</asp:GridView></div></div></div></div>
</asp:Content>