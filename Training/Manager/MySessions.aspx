<%@ Page Title="My Sessions" Language="C#" MasterPageFile="~/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="MySessions.aspx.cs" Inherits="Training.Manager.MySessions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .page-card{background:linear-gradient(135deg,#eff6ff,#ffffff 55%,#ecfeff);border:1px solid #bfdbfe;border-radius:16px;padding:24px;box-shadow:0 4px 18px rgba(15,23,42,.10)}
        .page-title{font-size:26px;font-weight:600;color:#1e293b;margin-bottom:18px}
        .gridview th{background:#0d6efd;color:#fff;white-space:nowrap}.gridview td{vertical-align:middle;white-space:nowrap}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid py-4">
    <div class="page-card">
        <div class="page-title">My Sessions</div>
        <div class="table-responsive">
            <asp:GridView ID="gvSessions" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Sessions Found">
                <Columns>
                    <asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
                    <asp:BoundField DataField="Batch" HeaderText="Batch" />
                    <asp:BoundField DataField="SessionNo" HeaderText="Session" />
                    <asp:BoundField DataField="SessionName" HeaderText="Session Name" />
                    <asp:BoundField DataField="SessionDate" HeaderText="Session Date" />
                    <asp:BoundField DataField="TrainerID" HeaderText="Trainer ID" />
                    <asp:BoundField DataField="AttendanceStatus" HeaderText="Attendance" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>
</asp:Content>