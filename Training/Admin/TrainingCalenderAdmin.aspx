<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="TrainingCalenderAdmin.aspx.cs" Inherits="Training.Admin.TrainingCalenderAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px
        }

        .dashboard-card {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px #d9d9d9;
            padding: 20px;
            margin-bottom: 20px
        }

        .calendar-table {
            width: 100%;
            border-collapse: collapse
        }

            .calendar-table th {
                background: #198754;
                color: white;
                padding: 10px;
                text-align: center
            }

            .calendar-table td {
                height: 90px;
                vertical-align: top;
                padding: 8px;
                border: 1px solid #ddd;
                text-align: center;
            }

            .calendar-table .day-number {
                font-weight: 600;
                font-size: 14px;
                text-align: left;
                margin-bottom: 6px;
            }

            .calendar-table .count-badge {
                display: inline-block;
                min-width: 40px;
                padding: 4px 10px;
                border-radius: 5px;
                font-size: 14px;
                font-weight: 600;
                color: #fff;
                text-decoration: none;
                cursor: pointer;
            }

                .calendar-table .count-badge:hover {
                    opacity: .85;
                    color: #fff;
                }

            .calendar-table .today {
                background: #eaf3ff
            }

        .year-table th {
            background: #198754;
            color: #fff;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Training Calendar</div>

        <div class="dashboard-card">
            <div class="row g-3">
                <div class="col-md-3">
                    <label>Select Year</label>
                    <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed" />
                </div>
                <div class="col-md-3">
                    <label>Select Month</label>
                    <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed" />
                </div>
                <div class="col-md-3">
                    <label>Select Location</label>
                    <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed" />
                </div>
                <div class="col-md-3">
                    <label>Select Organizer</label>
                    <asp:DropDownList ID="ddlOrganizer" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed" />
                </div>
            </div>
        </div>

        <div class="dashboard-card">
            <h5 class="mb-3"><asp:Label ID="lblMonthYear" runat="server" /></h5>
            <asp:Table ID="tblCalendar" runat="server" CssClass="calendar-table" />
            <div class="mt-3">
                <h6>Legend:</h6>
                <span class="badge bg-info">Upcoming</span><span class="badge bg-warning text-dark ms-2">Ongoing</span><span class="badge bg-success ms-2">Completed</span>
            </div>
        </div>

        <div class="dashboard-card">
            <h5 class="mb-3">Trainings for <asp:Label ID="lblYearTableCaption" runat="server" /></h5>
            <div class="table-responsive">
                <asp:GridView
                    ID="gvYearList"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="table table-bordered table-sm table-hover year-table"
                    EmptyDataText="No trainings found for the selected filters."
                    OnRowCommand="gvYearList_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="S.No">
                            <ItemTemplate>
                                <%# Container.DataItemIndex+1 %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
                        <asp:BoundField DataField="CourseName" HeaderText="Course" />
                        <asp:BoundField DataField="TrainingType" HeaderText="Type" />
                        <asp:BoundField DataField="TrainingOrganizer" HeaderText="Organizer" />
                        <asp:BoundField DataField="TrainingLocation" HeaderText="Location" />
                        <asp:BoundField DataField="Batch" HeaderText="Batch" />
                        <asp:BoundField DataField="DateFrom" HeaderText="Start Date" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="DateTo" HeaderText="End Date" DataFormatString="{0:dd-MMM-yyyy}" />
                        <asp:BoundField DataField="BatchStrength" HeaderText="Strength" />
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:LinkButton
                                    runat="server"
                                    CssClass="btn btn-sm btn-outline-primary"
                                    CommandName="ViewTraining"
                                    CommandArgument='<%# Eval("TrainingID") %>'>
                                    Open
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>