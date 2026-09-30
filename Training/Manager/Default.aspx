<%@ Page Title="Manager Dashboard" Language="C#" MasterPageFile="~/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Training.Manager.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .dashboard-card { background:#fff; border-radius:12px; padding:24px; margin-bottom:20px; box-shadow:0 2px 12px rgba(0,0,0,.08); }
        .page-title { font-size:28px; font-weight:600; color:#1e293b; margin-bottom:20px; }
        .info-label { font-size:13px; color:#64748b; font-weight:600; margin-bottom:4px; }
        .info-value { font-size:16px; color:#1e293b; font-weight:500; }
        .profile-row { padding:4px 0; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid py-4">
        <div class="page-title">Manager Dashboard</div>

        <div class="dashboard-card">
            <h5 class="mb-4">My Details</h5>
            <div class="row">
                <div class="col-md-4 col-sm-6 mb-4 profile-row">
                    <div class="info-label">Manager ID</div>
                    <div class="info-value"><asp:Label ID="lblManagerID" runat="server"></asp:Label></div>
                </div>
                <div class="col-md-4 col-sm-6 mb-4 profile-row">
                    <div class="info-label">Employee ID</div>
                    <div class="info-value"><asp:Label ID="lblEmpID" runat="server"></asp:Label></div>
                </div>
                <div class="col-md-4 col-sm-6 mb-4 profile-row">
                    <div class="info-label">Name</div>
                    <div class="info-value"><asp:Label ID="lblName" runat="server"></asp:Label></div>
                </div>
                <div class="col-md-4 col-sm-6 mb-4 profile-row">
                    <div class="info-label">Designation</div>
                    <div class="info-value"><asp:Label ID="lblDesignation" runat="server"></asp:Label></div>
                </div>
                <div class="col-md-4 col-sm-6 mb-4 profile-row">
                    <div class="info-label">Posting</div>
                    <div class="info-value"><asp:Label ID="lblPosting" runat="server"></asp:Label></div>
                </div>
                <div class="col-md-4 col-sm-6 mb-4 profile-row">
                    <div class="info-label">Mapped Location</div>
                    <div class="info-value"><asp:Label ID="lblMapForLocation" runat="server"></asp:Label></div>
                </div>
                <div class="col-md-4 col-sm-6 mb-4 profile-row">
                    <div class="info-label">Training Location</div>
                    <div class="info-value"><asp:Label ID="lblTrainingLocation" runat="server"></asp:Label></div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
