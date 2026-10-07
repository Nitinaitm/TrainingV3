<%@ Page Title="Dashboard Details" Language="C#" MasterPageFile="~/TraineeMaster.Master" AutoEventWireup="true" CodeBehind="DashboardDetails.aspx.cs" Inherits="Training.Trainee.DashboardDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"><style>.page-title{font-size:24px;font-weight:700;color:#0d6efd;margin-bottom:18px}.card-box{background:#fff;border-radius:10px;box-shadow:0 2px 8px rgba(0,0,0,.08);margin-bottom:20px;padding:20px}.grid th{background:#0d6efd;color:#fff;text-align:center}.grid td{vertical-align:middle}</style></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="d-flex justify-content-between align-items-center mb-3"><div class="page-title mb-0"><asp:Label ID="lblTitle" runat="server" /></div><asp:Button ID="btnBack" runat="server" Text="Back to Dashboard" CssClass="btn btn-secondary" CausesValidation="false" PostBackUrl="~/Trainee/Default.aspx" /></div>
<div class="card-box"><div class="mb-3"><asp:Label ID="lblSummary" runat="server" CssClass="fw-bold" /></div>
<asp:GridView ID="gvDetails" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-hover grid" GridLines="None" EmptyDataText="No matching records found." OnRowCommand="gvDetails_RowCommand">
<Columns>
<asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
<asp:BoundField DataField="CourseName" HeaderText="Course" />
<asp:BoundField DataField="TrainingType" HeaderText="Training Type" />
<asp:BoundField DataField="TrainingOrganizer" HeaderText="Organizer" />
<asp:BoundField DataField="Batch" HeaderText="Batch" />
<asp:BoundField DataField="DateFrom" HeaderText="From" DataFormatString="{0:dd-MM-yyyy}" />
<asp:BoundField DataField="DateTo" HeaderText="To" DataFormatString="{0:dd-MM-yyyy}" />
<asp:BoundField DataField="TrainingStatus" HeaderText="Status" />
<asp:TemplateField HeaderText="Action"><ItemTemplate><asp:LinkButton ID="btnDownload" runat="server" CommandName="DownloadCertificate" CommandArgument='<%# Eval("CertificateID") %>' CssClass="btn btn-sm btn-success" CausesValidation="false" Visible='<%# Eval("CertificateID") != null && Eval("CertificateID").ToString() != "" %>'><i class="fa fa-download"></i> Download PDF</asp:LinkButton></ItemTemplate></asp:TemplateField>
</Columns>
</asp:GridView></div></div>
</asp:Content>