<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="TrainingAllotmentDetails.aspx.cs"
    Inherits="Training.Admin.TrainingAllotmentDetails" %>

<%@ Register Src="~/Admin/HostelStatusDashboard.ascx" TagPrefix="uc" TagName="HostelStatusDashboard" %>

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
                Training Allotment Details
            </div>

            <uc:HostelStatusDashboard ID="HostelStatusDashboard1" runat="server" />

            <div class="row mb-3">
                <div class="col-md-6">
                    <label>Training *</label>
                    <asp:DropDownList
                        ID="ddlReportTraining"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>
                </div>
                <div class="col-md-3 d-flex align-items-end">
                    <asp:Button
                        ID="btnShowReport"
                        runat="server"
                        Text="Show Details"
                        CssClass="btn btn-primary"
                        OnClick="btnShowReport_Click" />
                </div>
            </div>

            <asp:Panel ID="pnlReportTrainingDetails" runat="server" Visible="false" CssClass="card mb-3">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-3"><b>Training ID:</b> <asp:Label ID="lblRptTrainingID" runat="server" /></div>
                        <div class="col-md-3"><b>Type:</b> <asp:Label ID="lblRptTrainingType" runat="server" /></div>
                        <div class="col-md-3"><b>Batch:</b> <asp:Label ID="lblRptBatch" runat="server" /></div>
                        <div class="col-md-3"><b>Dates:</b> <asp:Label ID="lblRptDates" runat="server" /></div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-3"><b>Location:</b> <asp:Label ID="lblRptLocation" runat="server" /></div>
                        <div class="col-md-3"><b>Organizer:</b> <asp:Label ID="lblRptOrganizer" runat="server" /></div>
                    </div>
                </div>
            </asp:Panel>

            <div class="mb-2">
                <asp:Label ID="lblReportMessage" runat="server" Font-Bold="true"></asp:Label>
            </div>

            <asp:GridView
                ID="gvReport"
                runat="server"
                AutoGenerateColumns="false"
                DataKeyNames="EmpID,AllotmentPK,BedPK"
                CssClass="table table-bordered table-striped gridview"
                OnRowCommand="gvReport_RowCommand"
                OnRowDataBound="gvReport_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="S.No">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Emp ID">
                        <ItemTemplate>
                            <asp:Label ID="lblempID" runat="server" Text='<%# Eval("EmpID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Emp Name">
                        <ItemTemplate>
                            <asp:Label ID="lblempName" runat="server" Text='<%# Eval("EmpName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Gender">
                        <ItemTemplate>
                            <asp:Label ID="lblGender" runat="server" Text='<%# Eval("gender") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Hostel Name">
                        <ItemTemplate>
                            <asp:Label ID="lblhostelName" runat="server" Text='<%# Eval("HostelName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Block Name">
                        <ItemTemplate>
                            <asp:Label ID="lblblockName" runat="server" Text='<%# Eval("BlockName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Flat Name">
                        <ItemTemplate>
                            <asp:Label ID="lblflatName" runat="server" Text='<%# Eval("FlatName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Room No.">
                        <ItemTemplate>
                            <asp:Label ID="lblrmNo" runat="server" Text='<%# Eval("RoomNo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Bed No">
                        <ItemTemplate>
                            <asp:Label ID="lblbedNo" runat="server" Text='<%# Eval("BedNo") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:LinkButton
                                ID="btnVacate"
                                runat="server"
                                CommandName="Vacate"
                                Text="Vacate"
                                CommandArgument='<%# Eval("AllotmentPK") %>'
                                OnClientClick="return confirm('Are you sure you want to vacate this allotment?');">
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>