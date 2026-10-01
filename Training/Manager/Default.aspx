<%@ Page Title="Manager Dashboard" Language="C#" MasterPageFile="~/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Training.Manager.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .dashboard-link{display:block;text-decoration:none;color:inherit}.dashboard-link:hover{text-decoration:none;color:inherit}
        .dashboard-card{background:linear-gradient(135deg,#ffffff,#f8fafc);border-radius:16px;padding:24px;margin-bottom:20px;box-shadow:0 4px 18px rgba(15,23,42,.10);border:1px solid #e2e8f0}\n        .profile-card{background:linear-gradient(135deg,#eef6ff 0%,#ffffff 55%,#f0fdf4 100%);border:1px solid #dbeafe}\n        .profile-badge{display:inline-block;padding:6px 12px;border-radius:20px;background:#dbeafe;color:#1d4ed8;font-size:12px;font-weight:700;margin-bottom:12px}\n        .profile-item{background:rgba(255,255,255,.72);border:1px solid rgba(226,232,240,.9);border-radius:10px;padding:12px 14px;height:100%}
        .page-title{font-size:28px;font-weight:600;color:#1e293b;margin-bottom:20px}
        .info-label{font-size:13px;color:#64748b;font-weight:600;margin-bottom:4px}
        .info-value{font-size:16px;color:#1e2937;font-weight:500}
        .kpi-card{border-radius:12px;padding:20px;color:#fff;min-height:125px;box-shadow:0 2px 10px rgba(0,0,0,.08)}
        .kpi-label{font-size:14px;opacity:.9}.kpi-value{font-size:30px;font-weight:700;margin-top:8px}
        .section-title{font-size:18px;font-weight:600;color:#1e293b;margin-bottom:16px}
        .gridview th{background:#198754;color:#fff;white-space:nowrap}.gridview td{vertical-align:middle;white-space:nowrap}
    .profile-card{background:linear-gradient(135deg,#eef6ff 0%,#ffffff 52%,#f0fdf4 100%);border:1px solid #cfe2ff}.profile-badge{display:inline-block;padding:6px 12px;border-radius:20px;background:#dbeafe;color:#1d4ed8;font-size:12px;font-weight:700;margin-bottom:12px}.profile-item{background:rgba(255,255,255,.78);border:1px solid #dbeafe;border-radius:10px;padding:12px 14px;height:100%}.upcoming-card{background:linear-gradient(135deg,#eff6ff,#f8fafc 55%,#ecfeff);border-color:#bfdbfe}.status-card{background:linear-gradient(135deg,#fff7ed,#fffbeb 50%,#f0fdf4);border-color:#fed7aa}.section-link{display:flex;justify-content:space-between;align-items:center;text-decoration:none}.section-link:hover{text-decoration:none}.view-all{font-size:13px;font-weight:600;color:#2563eb}.status-item{border-radius:12px;padding:16px;background:rgba(255,255,255,.72);border:1px solid rgba(226,232,240,.9);text-align:center}.status-item .info-value{font-size:24px;font-weight:700} 
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid py-4">
    <div class="page-title">Manager Dashboard</div>

    <div class="dashboard-card profile-card">
        <div class="profile-badge">MANAGER PROFILE</div>
        <div class="section-title">My Details</div>
        <div class="row">
            <div class="col-lg-3 col-md-4 col-sm-6 mb-4"><div class="profile-item"><div class="info-label">Manager ID</div><div class="info-value"><asp:Label ID="lblManagerID" runat="server" /></div></div></div>
            <div class="col-lg-3 col-md-4 col-sm-6 mb-4"><div class="profile-item"><div class="info-label">Employee ID</div><div class="info-value"><asp:Label ID="lblEmpID" runat="server" /></div></div></div>
            <div class="col-lg-3 col-md-4 col-sm-6 mb-4"><div class="profile-item"><div class="info-label">Name</div><div class="info-value"><asp:Label ID="lblName" runat="server" /></div></div></div>
            <div class="col-lg-3 col-md-4 col-sm-6 mb-4"><div class="profile-item"><div class="info-label">Designation</div><div class="info-value"><asp:Label ID="lblDesignation" runat="server" /></div></div></div>
            <div class="col-lg-3 col-md-4 col-sm-6 mb-4"><div class="profile-item"><div class="info-label">Posting</div><div class="info-value"><asp:Label ID="lblPosting" runat="server" /></div></div></div>
            <div class="col-lg-3 col-md-4 col-sm-6 mb-4"><div class="profile-item"><div class="info-label">Mapped Location</div><div class="info-value"><asp:Label ID="lblMapForLocation" runat="server" /></div></div></div>
        </div>
    </div>

    <div class="row g-3 mb-1">
        <div class="col-xl-3 col-md-6"><asp:LinkButton ID="lnkTotalTrainings" runat="server" CommandName="TotalTrainings" OnCommand="DashboardLink_Command" CssClass="dashboard-link"><div class="kpi-card bg-primary"><div class="kpi-label">Total Trainings</div><div class="kpi-value"><asp:Label ID="lblTotalTrainings" runat="server" Text="0" /></div></div></asp:LinkButton></div>
        <div class="col-xl-3 col-md-6"><asp:LinkButton ID="lnkActiveTrainings" runat="server" CommandName="ActiveTrainings" OnCommand="DashboardLink_Command" CssClass="dashboard-link"><div class="kpi-card bg-success"><div class="kpi-label">Active Trainings</div><div class="kpi-value"><asp:Label ID="lblActiveTrainings" runat="server" Text="0" /></div></div></asp:LinkButton></div>
        <div class="col-xl-3 col-md-6"><asp:LinkButton ID="lnkCompletedTrainings" runat="server" CommandName="CompletedTrainings" OnCommand="DashboardLink_Command" CssClass="dashboard-link"><div class="kpi-card bg-warning text-dark"><div class="kpi-label">Completed Trainings</div><div class="kpi-value"><asp:Label ID="lblCompletedTrainings" runat="server" Text="0" /></div></div></asp:LinkButton></div>
        <div class="col-xl-3 col-md-6"><asp:LinkButton ID="lnkTotalSessions" runat="server" CommandName="TotalSessions" OnCommand="DashboardLink_Command" CssClass="dashboard-link"><div class="kpi-card bg-info"><div class="kpi-label">Total Sessions</div><div class="kpi-value"><asp:Label ID="lblTotalSessions" runat="server" Text="0" /></div></div></asp:LinkButton></div>
        <div class="col-xl-3 col-md-6"><asp:LinkButton ID="lnkUpcomingSessions" runat="server" CommandName="UpcomingSessions" OnCommand="DashboardLink_Command" CssClass="dashboard-link"><div class="kpi-card bg-secondary"><div class="kpi-label">Upcoming Sessions</div><div class="kpi-value"><asp:Label ID="lblUpcomingSessions" runat="server" Text="0" /></div></div></asp:LinkButton></div>
        <div class="col-xl-3 col-md-6"><asp:LinkButton ID="lnkAttendancePending" runat="server" CommandName="AttendancePending" OnCommand="DashboardLink_Command" CssClass="dashboard-link"><div class="kpi-card bg-danger"><div class="kpi-label">Attendance Pending</div><div class="kpi-value"><asp:Label ID="lblAttendancePending" runat="server" Text="0" /></div></div></asp:LinkButton></div>
        <div class="col-xl-3 col-md-6"><asp:LinkButton ID="lnkSessionsCompleted" runat="server" CommandName="SessionsCompleted" OnCommand="DashboardLink_Command" CssClass="dashboard-link"><div class="kpi-card bg-dark"><div class="kpi-label">Sessions Completed</div><div class="kpi-value"><asp:Label ID="lblSessionsCompleted" runat="server" Text="0" /></div></div></asp:LinkButton></div>
        
    </div>

    <div class="dashboard-card upcoming-card mt-4">
        <a href="MySessions.aspx" class="section-link"><span class="section-title">Upcoming Sessions</span><span class="view-all">View All Sessions &rarr;</span></a>
        <div class="table-responsive">
            <asp:GridView ID="gvUpcomingSessions" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Upcoming Sessions">
                <Columns>
                    <asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
                    <asp:BoundField DataField="Batch" HeaderText="Batch" />
                    <asp:BoundField DataField="SessionNo" HeaderText="Session" />
                    <asp:BoundField DataField="SessionName" HeaderText="Session Name" />
                    <asp:BoundField DataField="SessionDate" HeaderText="Session Date" />
                    <asp:BoundField DataField="AttendanceStatus" HeaderText="Attendance" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <div class="dashboard-card status-card">
        <div class="section-title">Training Status Summary</div>
        <div class="row">
            <div class="col-md-4 mb-3"><asp:LinkButton ID="lnkStatusPlanned" runat="server" CommandName="Planned" OnCommand="DashboardLink_Command" CssClass="dashboard-link"><div class="status-item"><div class="info-label">Planned</div><div class="info-value"><asp:Label ID="lblPlanned" runat="server" Text="0" /></div></div></asp:LinkButton></div>
            <div class="col-md-4 mb-3"><asp:LinkButton ID="lnkStatusInProgress" runat="server" CommandName="InProgress" OnCommand="DashboardLink_Command" CssClass="dashboard-link"><div class="status-item"><div class="info-label">In Progress</div><div class="info-value"><asp:Label ID="lblInProgress" runat="server" Text="0" /></div></div></asp:LinkButton></div>
            <div class="col-md-4 mb-3"><asp:LinkButton ID="lnkStatusCompleted" runat="server" CommandName="Completed" OnCommand="DashboardLink_Command" CssClass="dashboard-link"><div class="status-item"><div class="info-label">Completed - Training Completed</div><div class="info-value"><asp:Label ID="lblCompletedSummary" runat="server" Text="0" /></div></div></asp:LinkButton></div>
        </div>
    </div>
</div>
</asp:Content>