<%@ Page Title="Super Admin - Activity Log" Language="C#" MasterPageFile="~/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="ActivityLog.aspx.cs" Inherits="Training.SuperAdmin.ActivityLog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"><style>.sa-page{padding:24px}.sa-card{background:#fff;border-radius:14px;padding:24px;box-shadow:0 3px 15px rgba(0,0,0,.07)}.sa-title{font-size:24px;font-weight:700}.sa-sub{color:#6b7280;margin:5px 0 18px}.sa-wrap{overflow:auto}.sa-table{width:100%;border-collapse:collapse}.sa-table th{background:#1f2937;color:#fff;padding:11px;white-space:nowrap}.sa-table td{padding:9px;border-bottom:1px solid #e5e7eb;white-space:nowrap}@media(max-width:700px){.sa-page{padding:12px}} .filter-row{display:grid;grid-template-columns:1fr 1fr auto;gap:12px;align-items:end}.sa-label{font-weight:700;display:block;margin-bottom:6px}.sa-input{width:100%;padding:10px;border:1px solid #d1d5db;border-radius:8px}.sa-btn{border:0;border-radius:8px;padding:10px 16px;font-weight:700;color:#fff;cursor:pointer}.blue{background:#2563eb}.msg{display:block;margin:12px 0;font-weight:700}@media(max-width:700px){.sa-page{padding:12px}.filter-row{grid-template-columns:1fr}} </style></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"><div class="sa-page"><div class="sa-card"><div class="sa-title"><i class="fas fa-history mr-2"></i>User Activity Log</div><div class="sa-sub">Select a date range to load activity records.</div><div class="filter-row">
<div><span class="sa-label">From Date</span><asp:TextBox ID="txtFromDate" runat="server" CssClass="sa-input" TextMode="Date"></asp:TextBox></div>
<div><span class="sa-label">To Date</span><asp:TextBox ID="txtToDate" runat="server" CssClass="sa-input" TextMode="Date"></asp:TextBox></div>
<div><asp:Button ID="btnFilter" runat="server" Text="Search" CssClass="sa-btn blue" OnClick="btnFilter_Click" /></div>
</div>
<asp:Label ID="lblMessage" runat="server" CssClass="msg"></asp:Label><div class="sa-wrap"><asp:GridView ID="gvActivity" runat="server" AutoGenerateColumns="False" CssClass="sa-table" EmptyDataText="No activity found for the selected date range.">
<Columns>
<asp:BoundField DataField="ActivityTime" HeaderText="Date / Time" DataFormatString="{0:dd-MM-yyyy HH:mm:ss}" />
<asp:BoundField DataField="UserID" HeaderText="User" />
<asp:BoundField DataField="UserRole" HeaderText="Role" />
<asp:BoundField DataField="ActionType" HeaderText="Action" />
<asp:BoundField DataField="Module" HeaderText="Module" />
<asp:BoundField DataField="PageName" HeaderText="Page" />
<asp:BoundField DataField="RecordType" HeaderText="Record Type" />
<asp:BoundField DataField="RecordID" HeaderText="Record ID" />
<asp:BoundField DataField="Description" HeaderText="Details / Changed Values" />
<asp:BoundField DataField="IPAddress" HeaderText="IP Address" />
<asp:BoundField DataField="SessionID" HeaderText="Session ID" />
</Columns>
</asp:GridView></div></div></div></asp:Content>