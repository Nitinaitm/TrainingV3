<%@ Page Title="Finance - Training Detail" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceTrainingDetail.aspx.cs" Inherits="Training.Admin.FinanceTrainingDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body { background:#f5f7fb; }

        .main-card {
            background:#fff;
            padding:25px;
            border-radius:14px;
            box-shadow:0 5px 20px rgba(13,110,253,.08);
            margin-top:20px;
            margin-bottom:20px;
            border:1px solid #e3e8ef;
        }

        .page-heading {
            font-size:28px;
            font-weight:700;
            color:#0d6efd;
            margin-bottom:20px;
            padding:13px 18px;
            border-radius:10px;
            background:linear-gradient(135deg,#eaf3ff,#f5f9ff);
            border-left:5px solid #0d6efd;
            box-shadow:0 3px 10px rgba(13,110,253,.08);
        }

        .finance-summary {
            margin-top:10px;
            padding:16px 12px 8px;
            border-radius:13px;
            background:linear-gradient(135deg,#eef7ff 0%,#f4fff8 100%);
            border:1px solid #d7e5f2;
            box-shadow:0 4px 12px rgba(0,0,0,.05);
        }

        .summary-value {
            display:block;
            min-height:48px;
            padding:12px 13px;
            background:#fff;
            border:1px solid #e1e7ee;
            border-left:4px solid #198754;
            border-radius:8px;
            box-shadow:0 2px 7px rgba(0,0,0,.04);
            font-weight:600;
            color:#495057;
        }

        .finance-summary .col-lg-3:nth-child(2) .summary-value,
        .finance-summary .col-md-3:nth-child(2) .summary-value { border-left-color:#0d6efd; }

        .finance-summary .col-lg-3:nth-child(3) .summary-value,
        .finance-summary .col-md-3:nth-child(3) .summary-value { border-left-color:#fd7e14; }

        .finance-summary .col-lg-3:nth-child(4) .summary-value,
        .finance-summary .col-md-3:nth-child(4) .summary-value { border-left-color:#6f42c1; }

        .summary-value label { color:#212529; font-weight:700; }
        .total-value { color:#198754 !important; font-size:17px; }

        .detail-actions {
            padding:12px 15px;
            margin:16px 0 18px;
            background:linear-gradient(135deg,#f8f9fa,#eef5ff);
            border:1px solid #dbe3ec;
            border-radius:10px;
        }

        .detail-actions .btn {
            min-width:140px;
            white-space:nowrap;
            margin:3px;
            font-weight:600;
        }

        .section-title {
            margin:22px 0 10px;
            padding:10px 14px;
            border-radius:8px;
            background:linear-gradient(90deg,#0d6efd,#4dabf7);
            color:#fff;
            font-size:17px;
            font-weight:700;
            box-shadow:0 3px 8px rgba(13,110,253,.14);
        }

        .table-responsive {
            border-radius:8px;
            overflow-x:auto;
            box-shadow:0 2px 8px rgba(0,0,0,.04);
            margin-bottom:18px;
        }

        .finance-grid {
            margin-bottom:0 !important;
            background:#fff;
            border-color:#dee6ef;
        }

        .finance-grid thead th {
            background:#eaf3ff;
            color:#164a7b;
            font-weight:700;
            border-color:#cddbea;
            white-space:nowrap;
            vertical-align:middle;
        }

        .finance-grid tbody td {
            vertical-align:middle;
            border-color:#e4e9ef;
            white-space:nowrap;
        }

        .finance-grid tbody tr:nth-child(even) td { background:#f8fbff; }
        .finance-grid tbody tr:hover td { background:#fff8e8; }

        .finance-grid .btn {
            white-space:nowrap;
            min-width:78px;
            padding:5px 10px;
        }

        @media(max-width:768px) {
            .main-card { padding:15px; }
            .page-heading { font-size:22px; }
            .summary-value { min-height:auto; }
            .detail-actions .btn { width:100%; margin:4px 0; }
            .section-title { font-size:15px; }
        }

        @media print {
            body { background:#fff; }
            .main-card { box-shadow:none; border:0; margin:0; }
            .page-heading { box-shadow:none; }
            .detail-actions { display:none !important; }
            .section-title { break-after:avoid; }
            .table-responsive { overflow:visible; }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card">
<div class="page-heading">Finance - Training Detail</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row finance-summary">
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Training ID: <asp:Label ID="lblTrainingID" runat="server"></asp:Label></span></div>
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Course: <asp:Label ID="lblCourse" runat="server"></asp:Label></span></div>
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Batch: <asp:Label ID="lblBatch" runat="server"></asp:Label></span></div>
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Duration: <asp:Label ID="lblDuration" runat="server"></asp:Label></span></div>
</div>
<div class="row mt-2 finance-summary">
<div class="col-md-3 mb-2"><span class="summary-value">Trainees: <asp:Label ID="lblTrainees" runat="server"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Trainers: <asp:Label ID="lblTrainers" runat="server"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Final Cost: <asp:Label ID="lblFinalCost" runat="server" CssClass="total-value"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Actual Paid: <asp:Label ID="lblActual" runat="server"></asp:Label></span></div>
</div>
<div class="no-print detail-actions"><asp:Button ID="btnPrint" runat="server" Text="Print Detail" CssClass="btn btn-primary" OnClientClick="window.print();return false;" />&nbsp;<asp:Button ID="btnBack" runat="server" Text="Back to Final Report" CssClass="btn btn-secondary" OnClick="btnBack_Click" /></div>
<div class="section-title">1. Session-wise Cost Head</div>
<div class="table-responsive"><asp:GridView ID="gvSession" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="SessionName" HeaderText="Session" /><asp:BoundField DataField="SessionDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" /><asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" /><asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" /></Columns></asp:GridView></div>
<div class="section-title">2. Batch-wise Cost Head</div>
<div class="table-responsive"><asp:GridView ID="gvBatch" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="UnitType" HeaderText="Unit" /><asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N2}" /><asp:BoundField DataField="AppliedRate" HeaderText="Rate" DataFormatString="{0:N2}" /><asp:BoundField DataField="CalculatedAmount" HeaderText="Calculated" DataFormatString="{0:N2}" /><asp:BoundField DataField="FinalAmount" HeaderText="Final" DataFormatString="{0:N2}" /></Columns></asp:GridView></div>
<div class="section-title">3. Trainer-wise Head Cost</div>
<div class="table-responsive"><asp:GridView ID="gvTrainer" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="TrainerID" HeaderText="Trainer ID" /><asp:BoundField DataField="TrainerName" HeaderText="Trainer" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="FinalAmount" HeaderText="Allocated Cost" DataFormatString="{0:N2}" /></Columns></asp:GridView></div>
<div class="section-title">4. Trainee-wise Head Cost</div>
<div class="table-responsive"><asp:GridView ID="gvTrainee" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="EmpID" HeaderText="Emp ID" /><asp:BoundField DataField="EmpName" HeaderText="Trainee" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="FinalAmount" HeaderText="Allocated Cost" DataFormatString="{0:N2}" /></Columns></asp:GridView></div>
<div class="section-title">5. Actual Expenditure / Payment</div>
<div class="table-responsive"><asp:GridView ID="gvActual" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField><asp:BoundField DataField="ExpenditureDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" /><asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" /><asp:BoundField DataField="SessionName" HeaderText="Session" /><asp:BoundField DataField="VendorName" HeaderText="Vendor / Payee" /><asp:BoundField DataField="BillReference" HeaderText="Bill / Ref." /><asp:BoundField DataField="Amount" HeaderText="Paid" DataFormatString="{0:N2}" /></Columns></asp:GridView></div>
</div></div>
</asp:Content>