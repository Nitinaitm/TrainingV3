<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="Training.Admin.Dashboard"
    ClientIDMode="Static" %>

<%--<%@ Register Src="~/Admin/HostelStatusDashboard.ascx" TagPrefix="uc" TagName="HostelStatusDashboard" %>--%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.4/dist/chart.umd.min.js"></script>

    <style>
    .kpi-card .card-body {
        min-height: 130px;
        padding: 1.25rem;
    }

    .kpi-card .kpi-value {
        font-size: 2.1rem;
        font-weight: 700;
        line-height: 1.1;
    }

    .kpi-card .kpi-label {
        font-size: .85rem;
        opacity: .85;
    }

    .kpi-card i {
        opacity: .45;
    }

    .hostel-card .card-body {
        min-height: 130px;
        display: flex;
        flex-direction: column;
        justify-content: center;
        padding: 1.25rem;
    }
</style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4 class="mb-3">Dashboard</h4>

    <!-- ================= KPI CARDS ================= -->

    <div class="row g-3 mb-4">

            <div class="col-xl-2 col-md-4 col-6">
                <asp:LinkButton ID="lnkActiveTrainings" runat="server" CssClass="text-decoration-none d-block" CommandName="DrillDown" CommandArgument="ActiveTrainings" OnCommand="Tile_Command">
                    <div class="card kpi-card text-white bg-primary shadow-sm h-100">
                        <div class="card-body d-flex justify-content-between align-items-center">
                            <div>
                                <div class="kpi-label">Active Trainings</div>
                                <asp:Label ID="lblActiveTrainings" runat="server" CssClass="kpi-value" Text="0" />
                            </div>
                            <i class="fas fa-chalkboard-teacher fa-2x"></i>
                        </div>
                    </div>
                </asp:LinkButton>
            </div>

    <div class="col-xl-2 col-md-4 col-6">
        <asp:LinkButton ID="lnkStartingSoon" runat="server" CssClass="text-decoration-none d-block" CommandName="DrillDown" CommandArgument="StartingSoon" OnCommand="Tile_Command">
            <div class="card kpi-card text-white bg-info shadow-sm h-100">
                <div class="card-body d-flex justify-content-between align-items-center">
                    <div>
                        <div class="kpi-label">Starting in 7 Days</div>
                        <asp:Label ID="lblStartingSoon" runat="server" CssClass="kpi-value" Text="0" />
                    </div>
                    <i class="fas fa-calendar-day fa-2x"></i>
                </div>
            </div>
        </asp:LinkButton>
    </div>

        <div class="col-xl-2 col-md-4 col-6">
            <asp:LinkButton ID="lnkPendingConfirmation" runat="server" CssClass="text-decoration-none d-block" CommandName="DrillDown" CommandArgument="Confirmation" OnCommand="Tile_Command">
                <div class="card kpi-card text-white bg-warning shadow-sm h-100">
                    <div class="card-body d-flex justify-content-between align-items-center">
                        <div>
                            <div class="kpi-label">Attendance Confirmation Pending from Trainees</div>
                            <asp:Label ID="lblPendingConfirmation" runat="server" CssClass="kpi-value" Text="0" />
                        </div>
                        <i class="fas fa-user-clock fa-2x"></i>
                    </div>
                </div>
            </asp:LinkButton>
        </div>

        <div class="col-xl-2 col-md-4 col-6">
            <a href="ProfileChangeRequests.aspx" class="text-decoration-none">
                <div class="card kpi-card text-white bg-info shadow-sm h-100">
                    <div class="card-body d-flex justify-content-between align-items-center">
                        <div>
                            <div class="kpi-label">Profile Corrections Requests Pending for Approval</div>
                            <asp:Label ID="lblPendingProfileCorrections" runat="server" CssClass="kpi-value" Text="0" />
                        </div>
                        <i class="fas fa-user-edit fa-2x"></i>
                    </div>
                </div>
            </a>
        </div>

        <div class="col-xl-2 col-md-4 col-6">
            <a href="QuestionApproval.aspx" class="text-decoration-none">
                <div class="card kpi-card text-white bg-danger shadow-sm h-100">
                    <div class="card-body d-flex justify-content-between align-items-center">
                        <div>
                            <div class="kpi-label">Questions Pending for Approval</div>
                            <asp:Label ID="lblPendingApproval" runat="server" CssClass="kpi-value" Text="0" />
                        </div>
                        <i class="fas fa-question-circle fa-2x"></i>
                    </div>
                </div>
            </a>
        </div>

        <div class="col-xl-2 col-md-4 col-6">
            <asp:LinkButton ID="lnkPendingCertificates" runat="server" CssClass="text-decoration-none " CommandName="DrillDown" CommandArgument="Certificates" OnCommand="Tile_Command">
                <div class="card kpi-card text-white bg-success shadow-sm h-100">
                    <div class="card-body d-flex justify-content-between align-items-center">
                        <div>
                            <div class="kpi-label">Certificates Pending</div>
                            <asp:Label ID="lblPendingCertificates" runat="server" CssClass="kpi-value" Text="0" />
                        </div>
                        <i class="fas fa-certificate fa-2x"></i>
                    </div>
                </div>
            </asp:LinkButton>
        </div>

    <div class="col-xl-2 col-md-4 col-6">
        <asp:LinkButton ID="lnkTrainingsThisMonth" runat="server" CssClass="text-decoration-none d-block" CommandName="DrillDown" CommandArgument="ThisMonth" OnCommand="Tile_Command">
            <div class="card kpi-card text-white bg-secondary shadow-sm h-100">
                <div class="card-body d-flex justify-content-between align-items-center">
                    <div>
                        <div class="kpi-label">Trainings This Month</div>
                        <asp:Label ID="lblTrainingsThisMonth" runat="server" CssClass="kpi-value" Text="0" />
                    </div>
                    <i class="fas fa-layer-group fa-2x"></i>T
                </div>
            </div>
        </asp:LinkButton>
    </div>

        <div class="col-xl-2 col-md-4 col-6" runat="server" visible="false">
            <div class="card kpi-card text-white bg-dark shadow-sm h-100">
                <div class="card-body d-flex justify-content-between align-items-center">
                    <div>
                        <div class="kpi-label">Total Trainings Conducted</div>
                        <asp:Label ID="lblTotalConducted" runat="server" CssClass="kpi-value" Text="0" />
                    </div>
                    <i class="fas fa-graduation-cap fa-2x"></i>
                </div>
            </div>
        </div>

        <div class="col-xl-2 col-md-4 col-6"  runat="server" visible="false">
            <div class="card kpi-card text-white bg-primary shadow-sm h-100">
                <div class="card-body d-flex justify-content-between align-items-center">
                    <div>
                        <div class="kpi-label">Total Employees Trained</div>
                        <asp:Label ID="lblTotalTrained" runat="server" CssClass="kpi-value" Text="0" />
                    </div>
                    <i class="fas fa-users fa-2x"></i>
                </div>
            </div>
        </div>

    </div>

    <div class="modal fade" id="drillDownModal" tabindex="-1" role="dialog">
        <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><asp:Label ID="lblDrillDownTitle" runat="server" /></h5>
                    <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
                </div>
                <div class="modal-body">
                    <asp:GridView ID="gvDrillDown" runat="server" CssClass="table table-bordered table-sm" AutoGenerateColumns="true" EmptyDataText="No records found." />
                </div>
            </div>
        </div>
    </div>

    <!-- ================= NEEDS ATTENTION ================= -->

    <div class="card mb-4">
        <div class="card-header bg-dark text-white">
            <b>Needs Attention</b> - Ongoing/upcoming trainings with an incomplete step
        </div>
        <div class="card-body p-2">
            <asp:GridView
                ID="gvNeedsAttention"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm table-hover mb-0"
                EmptyDataText="Nothing outstanding - every ongoing or upcoming training is fully set up."
                OnRowCommand="gvNeedsAttention_RowCommand">
                <Columns>
                    <asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
                    <asp:BoundField DataField="CourseName" HeaderText="Course" />
                    <asp:BoundField DataField="Batch" HeaderText="Batch" />
                    <asp:BoundField DataField="DateFrom" HeaderText="Starts" DataFormatString="{0:dd-MMM-yyyy}" />
                    <asp:TemplateField HeaderText="Outstanding">
                        <ItemTemplate>
                            <span class="badge bg-warning text-dark"><%# Eval("Issues") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <asp:LinkButton
                                ID="lnkView"
                                runat="server"
                                CssClass="btn btn-sm btn-outline-primary"
                                CommandName="ViewTraining"
                                CommandArgument='<%# Eval("TrainingID") %>'>Open</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- ================= HOSTEL STATUS ================= -->

    <div class="card mb-4">
        <div class="card-header bg-primary text-white">
            <b>Hostel Status</b> - Vacant Seats Overview
        </div>
        <div class="card-body">
            <div class="row g-3">
                <asp:Repeater ID="rptHostelStatus" runat="server" OnItemCommand="rptHostelStatus_ItemCommand">
                    <ItemTemplate>
                        <div class="col-xl-3 col-md-4 col-6">
                            <div class='<%# "card kpi-card hostel-card h-100 shadow-sm text-white " + (Convert.ToInt32(Eval("VacantBeds")) > 0 ? "bg-success" : "bg-danger") %>'>
                                <div class="card-body">
                                    <div class="kpi-label"><%# Eval("HostelName") %> - <%# Eval("BlockName") %></div>
                                    <asp:LinkButton runat="server" CssClass="kpi-value text-white text-decoration-none d-block" CommandName="DrillDownHostel" CommandArgument='<%# Eval("BlockID") %>' Text='<%# Eval("VacantBeds") %>' />
                                    <div class="kpi-label">vacant of <%# Eval("TotalBeds") %> total (<%# Eval("OccupiedBeds") %> occupied)</div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <asp:Label ID="lblNoHostel" runat="server" CssClass="text-muted" Text="No active hostels found." Visible="false" />
        </div>
    </div>

    <!-- ================= ANALYTICS ================= -->

    <div class="row g-3 mb-4">
        <div class="col-lg-4">
            <div class="card h-100">
                <div class="card-header bg-dark text-white"><b>Company-wise Employees Trained</b></div>
                <div class="card-body">
                    <canvas id="companyChart"></canvas>
                </div>
            </div>
        </div>
        <div class="col-lg-4">
            <div class="card h-100">
                <div class="card-header bg-dark text-white"><b>Top 10 Courses by Employees Trained</b></div>
                <div class="card-body">
                    <canvas id="courseChart"></canvas>
                </div>
            </div>
        </div>
        <div class="col-lg-4">
            <div class="card h-100">
                <div class="card-header bg-dark text-white"><b>Year-wise Employees Trained</b></div>
                <div class="card-body">
                    <canvas id="yearChart"></canvas>
                </div>
            </div>
        </div>
    </div>

        <script>
            var companyChartInstance = null;
            var courseChartInstance = null;
            var yearChartInstance = null;

            function initDashboardCharts() {
                if (companyChartInstance) { companyChartInstance.destroy(); }
                if (courseChartInstance) { courseChartInstance.destroy(); }
                if (yearChartInstance) { yearChartInstance.destroy(); }

                companyChartInstance = new Chart(document.getElementById('companyChart'), {
                    type: 'pie',
                    data: {
                        labels: <%= CompanyChartLabels %>,
                    datasets: [{
                        data: <%= CompanyChartValues %>,
                        backgroundColor: ['#2563eb', '#7c3aed', '#f59e0b', '#16a34a', '#dc2626', '#0891b2']
                    }]
                }
            });

            courseChartInstance = new Chart(document.getElementById('courseChart'), {
                type: 'bar',
                data: {
                    labels: <%= CourseChartLabels %>,
                    datasets: [{
                        label: 'Employees Trained',
                        data: <%= CourseChartValues %>,
                        backgroundColor: '#2563eb'
                    }]
                },
                options: {
                    indexAxis: 'y',
                    scales: { x: { beginAtZero: true } },
                    plugins: { legend: { display: false } }
                }
            });

            yearChartInstance = new Chart(document.getElementById('yearChart'), {
                type: 'bar',
                data: {
                    labels: <%= YearChartLabels %>,
                    datasets: [{
                        label: 'Employees Trained',
                        data: <%= YearChartValues %>,
                        backgroundColor: '#7c3aed'
                    }]
                },
                options: {
                    scales: { y: { beginAtZero: true } },
                    plugins: { legend: { display: false } }
                }
            });
            }

            initDashboardCharts();
    </script>

        <script>
            window.addEventListener('pageshow', function (event) {
                if (event.persisted) {
                    if (typeof $ !== 'undefined') {
                        $('#drillDownModal').modal('hide');
                    }
                    if (typeof initDashboardCharts === 'function') {
                        initDashboardCharts();
                    }
                }
            });
        </script>

    <!-- ================= UPCOMING TRAININGS ================= -->

    <div class="card mb-4">
        <div class="card-header bg-primary text-white">
            <b>Upcoming Trainings</b> - Next 14 days
        </div>
        <div class="card-body p-2">
            <asp:GridView
                ID="gvUpcoming"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm mb-0"
                EmptyDataText="No trainings scheduled in the next 14 days.">
                <Columns>
                    <asp:BoundField DataField="TrainingID" HeaderText="Training ID" />
                    <asp:BoundField DataField="CourseName" HeaderText="Course" />
                    <asp:BoundField DataField="Batch" HeaderText="Batch" />
                    <asp:BoundField DataField="DateFrom" HeaderText="From" DataFormatString="{0:dd-MMM-yyyy}" />
                    <asp:BoundField DataField="DateTo" HeaderText="To" DataFormatString="{0:dd-MMM-yyyy}" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <%# Eval("WorkflowStatus").ToString().Contains("D") ? "<span class='badge bg-success'>Trainees Assigned</span>"
                                : (Eval("WorkflowStatus").ToString().Contains("C") ? "<span class='badge bg-info'>Sessions Assigned</span>"
                                : "<span class='badge bg-secondary'>Batch Created</span>") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>
