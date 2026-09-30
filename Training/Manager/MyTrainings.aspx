<%@ Page Title="My Trainings" Language="C#" MasterPageFile="~/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="MyTrainings.aspx.cs" Inherits="Training.Manager.MyTrainings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.card{background:#fff;border-radius:12px;box-shadow:0 2px 10px rgba(0,0,0,.08);padding:20px;margin-bottom:20px}
.page-title{font-size:28px;font-weight:600;color:#1e293b;margin-bottom:20px}
.gridview th{background:#198754;color:#fff;white-space:nowrap}.gridview td{vertical-align:middle;white-space:nowrap}
 .filter-label{font-weight:600;color:#475569;margin-bottom:5px}
.training-id-column{width:110px;max-width:110px;white-space:nowrap;}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid py-4">
<div class="page-title">My Trainings</div>
<div class="card">
<div class="row g-3">
<div class="col-md-3"><label class="filter-label">Training ID</label><asp:TextBox ID="txtTrainingID" runat="server" CssClass="form-control" /></div>
<div class="col-md-3"><label class="filter-label">Training Type</label><asp:TextBox ID="txtTrainingType" runat="server" CssClass="form-control" /></div>
<div class="col-md-3"><label class="filter-label">Batch</label><asp:TextBox ID="txtBatch" runat="server" CssClass="form-control" /></div>
<div class="col-md-3"><label class="filter-label">Status</label><asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select"><asp:ListItem Text="All" Value="" /><asp:ListItem Text="Planned" Value="Planned" /><asp:ListItem Text="InProgress" Value="InProgress" /><asp:ListItem Text="AttendanceCompleted" Value="AttendanceCompleted" /><asp:ListItem Text="TrainingCompleted" Value="TrainingCompleted" /><asp:ListItem Text="Completed" Value="Completed" /></asp:DropDownList></div>
<div class="col-md-3"><label class="filter-label">From Date</label><asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy" /></div>
<div class="col-md-3"><label class="filter-label">To Date</label><asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy" /></div>
<div class="col-md-6 d-flex align-items-end"><asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary me-2" OnClick="btnSearch_Click" /><asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary" CausesValidation="false" OnClick="btnReset_Click" /></div>
</div><asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-3" Font-Bold="true" />
</div>
<div class="card">
<h5>Training List</h5>
<div class="table-responsive">
<asp:GridView ID="gvTraining" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Training Found" ShowHeaderWhenEmpty="true" DataKeyNames="TrainingID" OnRowCommand="gvTraining_RowCommand">
<Columns>
<asp:TemplateField HeaderText="Sl No"><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate></asp:TemplateField>
<asp:BoundField DataField="TrainingID" HeaderText="Training ID"><HeaderStyle CssClass="training-id-column" /><ItemStyle CssClass="training-id-column" /></asp:BoundField>
<asp:BoundField DataField="TrainingType" HeaderText="Training Type" />
<asp:BoundField DataField="TrainingOrganizer" HeaderText="Organizer" />
<asp:BoundField DataField="TrainingLocation" HeaderText="Training Location" />
<asp:BoundField DataField="Batch" HeaderText="Batch" />
<asp:BoundField DataField="DateFrom" HeaderText="From" />
<asp:BoundField DataField="DateTo" HeaderText="To" />
<asp:BoundField DataField="TrainingStatus" HeaderText="Status" />
<asp:TemplateField HeaderText="Action"><ItemTemplate><asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-primary btn-sm" CommandName="ViewTraining" CommandArgument='<%# Eval("TrainingID") %>' CausesValidation="false" /></ItemTemplate></asp:TemplateField>
</Columns>
</asp:GridView>
</div>
</div>
</div>
</asp:Content>