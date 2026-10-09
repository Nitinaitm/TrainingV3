<%@ Page Title="Finance - Training Detail" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="FinanceTrainingDetail.aspx.cs" Inherits="Training.Admin.FinanceTrainingDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .main-card {
            background: #fff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 0 10px #d9d9d9;
            margin-top: 20px
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px
        }

        .info-box {
            margin-bottom: 12px
        }

        .action-card {
            margin-top: 20px;
            background: #fff;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            padding: 20px
        }

        .btn-action {
            min-width: 180px;
            margin-right: 10px;
            margin-bottom: 10px
        }

        .status-badge {
            font-size: 16px;
            padding: 8px 15px
        }

        .lifecycle-section {
            margin-top: 16px;
            padding: 14px;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            background: #fff
        }

        .session-cycle {
            background: #fbfbfb
        }

        .lifecycle-section + .lifecycle-section {
            margin-top: 12px
        }

        .lifecycle {
            margin: 20px 0 25px;
            padding: 20px;
            border: 1px solid #dee2e6;
            border-radius: 12px;
            background: #f8f9fa
        }

        .lifecycle-title {
            font-size: 20px;
            font-weight: 700;
            margin-bottom: 18px;
            text-align: center
        }

        .stage-scroll {
            overflow-x: auto;
            padding: 8px 0 12px
        }

        .stage-line {
            display: flex;
            align-items: flex-start;
            min-width: 1100px
        }

        .stage-item {
            flex: 1;
            position: relative;
            text-align: center
        }

            .stage-item:not(:last-child):after {
                content: "";
                position: absolute;
                top: 17px;
                left: 50%;
                width: 100%;
                height: 4px;
                background: #dc3545;
                z-index: 0
            }

            .stage-item.done:not(:last-child):after {
                background: #198754
            }

        .stage-bubble {
            position: relative;
            z-index: 1;
            width: 52px;
            height: 52px;
            line-height: 46px;
            border-radius: 50%;
            margin: 0 auto 8px;
            background: #dc3545;
            color: #fff;
            font-weight: 700;
            font-size: 13px;
            border: 3px solid #fff;
            box-shadow: 0 0 0 1px #dc3545;
            cursor: help
        }

        .stage-item.done .stage-bubble {
            background: #198754;
            box-shadow: 0 0 0 1px #198754
        }

        .stage-item.na .stage-bubble, .stage-item.skipped .stage-bubble {
            background: #adb5bd;
            box-shadow: 0 0 0 1px #adb5bd
        }

        .stage-item.partial .stage-bubble {
            box-shadow: 0 0 0 1px #198754
        }

        .stage-label {
            font-size: 12px;
            font-weight: 600;
            line-height: 1.25;
            padding: 0 4px
        }

        .stage-state {
            font-size: 10px;
            margin-top: 3px;
            color: #dc3545
        }

        .stage-item.done .stage-state {
            color: #198754
        }

        .stage-item.na .stage-state, .stage-item.skipped .stage-state {
            color: #6c757d
        }

        .stage-bubble[data-tooltip] {
            position: relative
        }

            .stage-bubble[data-tooltip]:hover:after {
                content: attr(data-tooltip);
                position: absolute;
                left: 50%;
                bottom: calc(100% + 10px);
                transform: translateX(-50%);
                background: #212529;
                color: #fff;
                padding: 7px 10px;
                border-radius: 6px;
                font-size: 12px;
                font-weight: 600;
                line-height: 1.2;
                white-space: nowrap;
                z-index: 1000;
                box-shadow: 0 3px 10px rgba(0,0,0,.25)
            }

            .stage-bubble[data-tooltip]:hover:before {
                content: "";
                position: absolute;
                left: 50%;
                bottom: calc(100% + 4px);
                transform: translateX(-50%);
                border: 6px solid transparent;
                border-top-color: #212529;
                z-index: 1001
            }

        @media(max-width:768px) {
            .main-card {
                padding: 15px
            }

            .stage-scroll {
                margin-left: -5px;
                margin-right: -5px
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card">
<div class="page-heading">Finance - Training Detail</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row">
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Training ID: <asp:Label ID="lblTrainingID" runat="server"></asp:Label></span></div>
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Course: <asp:Label ID="lblCourse" runat="server"></asp:Label></span></div>
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Batch: <asp:Label ID="lblBatch" runat="server"></asp:Label></span></div>
<div class="col-lg-3 col-md-6 mb-2"><span class="summary-value">Duration: <asp:Label ID="lblDuration" runat="server"></asp:Label></span></div>
</div>
<div class="row mt-2">
<div class="col-md-3 mb-2"><span class="summary-value">Trainees: <asp:Label ID="lblTrainees" runat="server"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Trainers: <asp:Label ID="lblTrainers" runat="server"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Final Cost: <asp:Label ID="lblFinalCost" runat="server" CssClass="total-value"></asp:Label></span></div>
<div class="col-md-3 mb-2"><span class="summary-value">Actual Paid: <asp:Label ID="lblActual" runat="server"></asp:Label></span></div>
</div>
<div class="no-print"><asp:Button ID="btnPrint" runat="server" Text="Print Detail" CssClass="btn btn-primary" OnClientClick="window.print();return false;" />&nbsp;<asp:Button ID="btnBack" runat="server" Text="Back to Final Report" CssClass="btn btn-secondary" OnClick="btnBack_Click" /></div>
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