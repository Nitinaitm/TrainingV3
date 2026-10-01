<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="HostelStayHistoryReport.aspx.cs"
    Inherits="Training.Admin.HostelStayHistoryReport" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <style>
        .main-card {
            background: #fff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 0 10px #d9d9d9;
            margin-top: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #0d6efd;
            margin-bottom: 20px;
        }

        .gridview th {
            background: #0d6efd;
            color: white;
            text-align: center;
            vertical-align: middle;
        }

        .gridview td {
            vertical-align: middle;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">
    <div class="container-fluid">
        <div class="main-card">
            <div class="page-heading">
                Hostel Stay History Report
            </div>

            <div class="row mb-3">
                <div class="col-md-3">
                    <label>Training</label>
                    <asp:DropDownList ID="ddlTrainingFilter" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <label>Hostel</label>
                    <asp:DropDownList ID="ddlHostelFilter" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <label>Status</label>
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-select">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                        <asp:ListItem Text="Currently Staying (Allotted)" Value="Allotted"></asp:ListItem>
                        <asp:ListItem Text="Vacated" Value="Vacated"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-2">
                    <label>Employee (ID or Name)</label>
                    <asp:TextBox ID="txtEmpSearch" runat="server" CssClass="form-control" placeholder="Search..." />
                </div>
                <div class="col-md-3 mt-2">
                    <label>Allotment Date From</label>
                    <asp:TextBox ID="txtDateFrom" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy" />
                </div>
                <div class="col-md-3 mt-2">
                    <label>Allotment Date To</label>
                    <asp:TextBox ID="txtDateTo" runat="server" CssClass="form-control" placeholder="dd-MM-yyyy" />
                </div>
                <div class="col-md-6 mt-2 d-flex align-items-end">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary me-2" OnClick="btnSearch_Click" />
                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-outline-secondary" OnClick="btnReset_Click" CausesValidation="false" />
                </div>
            </div>

            <div class="mb-2">
                <asp:Label ID="lblSummary" runat="server" Font-Bold="true"></asp:Label>
            </div>

            <div class="table-responsive">
                <asp:GridView
                    ID="gvHistory"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="table table-bordered table-striped gridview"
                    EmptyDataText="No hostel stay records found for the selected filters."
                    OnRowDataBound="gvHistory_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="S.No">
                            <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="EmpID" HeaderText="Emp ID" />
                        <asp:BoundField DataField="EmpName" HeaderText="Emp Name" />
                        <asp:BoundField DataField="Gender" HeaderText="Gender" />
                        <asp:BoundField DataField="EmpDesignation" HeaderText="Designation" />
                        <asp:BoundField DataField="EmpCompany" HeaderText="Company" />
                        <asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
                        <asp:BoundField DataField="TrainingType" HeaderText="Training Type" />
                        <asp:BoundField DataField="Batch" HeaderText="Batch" />
                        <asp:BoundField DataField="HostelName" HeaderText="Hostel" />
                        <asp:BoundField DataField="BlockName" HeaderText="Block" />
                        <asp:BoundField DataField="FlatName" HeaderText="Flat" />
                        <asp:BoundField DataField="RoomNo" HeaderText="Room" />
                        <asp:BoundField DataField="BedNo" HeaderText="Bed" />
                        <asp:BoundField DataField="AllotmentDate" HeaderText="Allotment Date" DataFormatString="{0:dd-MM-yyyy}" />
                        <asp:BoundField DataField="VacateDate" HeaderText="Vacate Date" DataFormatString="{0:dd-MM-yyyy}" />
                        <asp:BoundField DataField="StayDays" HeaderText="Days Stayed" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>