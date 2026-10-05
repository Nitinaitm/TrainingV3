<%@ Page Title="Session Report" Language="C#" MasterPageFile="~/ManagerMaster.Master" AutoEventWireup="true" CodeBehind="SessionReport.aspx.cs" Inherits="Training.Manager.SessionReport" %>

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
        <div class="page-title">Session Report</div>
        <div class="row mb-3">
            <div class="col-md-3">
                <label>Course</label>
                <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label>Batch</label>
                <asp:TextBox ID="txtBatch" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <label>Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <label>Session Date From</label>
                <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy" />
            </div>
            <div class="col-md-2">
                <label>Session Date To</label>
                <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy" />
            </div>
        </div>
        <div class="mb-3">
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary ml-2" CausesValidation="false" OnClick="btnReset_Click" />
        </div>
        <asp:Label ID="lblMessage" runat="server" />
        <div class="table-responsive">
            <asp:GridView ID="gvSession" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Sessions Found" DataKeyNames="SessionID,TrainingID" OnRowCommand="gvSession_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Sl No"><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate></asp:TemplateField>
                    <asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
                    <asp:BoundField DataField="Batch" HeaderText="Batch" />
                    <asp:BoundField DataField="SessionNo" HeaderText="Session" />
                    <asp:BoundField DataField="SessionName" HeaderText="Session Name" />
                    <asp:BoundField DataField="SessionDate" HeaderText="Session Date" />
                    <asp:BoundField DataField="TrainerID" HeaderText="Trainer ID" />
                    <asp:BoundField DataField="AttendanceStatus" HeaderText="Status" />
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-primary btn-sm" CommandName="ViewSession" CommandArgument='<%# Eval("SessionID") %>' CausesValidation="false" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>
</asp:Content>