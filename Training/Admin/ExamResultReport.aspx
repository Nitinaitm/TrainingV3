<%@ Page Title="Exam Result Report"
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="ExamResultReport.aspx.cs"
    Inherits="Training.Admin.ExamResultReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .exam-report-page{padding:22px;background:#f5f7fb;min-height:calc(100vh - 120px);box-sizing:border-box;color:#1e293b}
        .exam-report-page .exam-header{display:flex;justify-content:space-between;align-items:center;gap:16px;flex-wrap:wrap;margin-bottom:18px}
        .exam-report-page .page-title{font-size:26px;font-weight:700;color:#17365d;display:block}
        .exam-report-page .page-subtitle{font-size:13px;color:#64748b;margin-top:4px}
        .exam-report-page .report-card{background:#fff;border:1px solid #e2e8f0;border-radius:14px;box-shadow:0 4px 16px rgba(15,23,42,.07);margin-bottom:18px;overflow:hidden}
        .exam-report-page .report-card-header{padding:14px 18px;background:linear-gradient(135deg,#17365d,#2563eb);color:#fff;display:flex;justify-content:space-between;align-items:center;gap:12px;flex-wrap:wrap}
        .exam-report-page .comparison-header{background:linear-gradient(135deg,#0f766e,#0891b2)}
        .exam-report-page .section-title{font-size:18px;font-weight:700}
        .exam-report-page .card-body{padding:18px}
        .exam-report-page .filter-label{display:block;font-size:13px;font-weight:700;color:#334155;margin-bottom:6px}
        .exam-report-page .form-control{height:40px;border:1px solid #cbd5e1;border-radius:7px;box-shadow:none}
        .exam-report-page .form-control:focus{border-color:#2563eb;box-shadow:0 0 0 3px rgba(37,99,235,.12)}
        .exam-report-page .filter-actions{display:flex;align-items:flex-end;gap:8px;height:100%}
        .exam-report-page .summary-grid{display:grid;grid-template-columns:repeat(4,1fr);gap:14px;margin-bottom:18px}
        .exam-report-page .summary-card-wrap{min-width:0}.exam-report-page .summary-box{background:#fff;border:1px solid #e2e8f0;border-radius:12px;padding:18px;text-align:left;min-height:105px;box-shadow:0 3px 12px rgba(15,23,42,.05);position:relative;overflow:hidden}
        .exam-report-page .summary-box:before{content:"";position:absolute;left:0;top:0;bottom:0;width:4px;background:#2563eb}
        .exam-report-page .summary-box.passed:before{background:#16a34a}
        .exam-report-page .summary-box.failed:before{background:#dc2626}
        .exam-report-page .summary-box.average:before{background:#7c3aed}
        .exam-report-page .summary-title{display:block;font-size:12px;text-transform:uppercase;letter-spacing:.04em;font-weight:700;color:#64748b;margin-bottom:9px}
        .exam-report-page .summary-value{display:block;font-size:28px;line-height:1.1;font-weight:800;color:#2563eb}
        .exam-report-page .passed-value{color:#16a34a}.exam-report-page .failed-value{color:#dc2626}.exam-report-page .average-value{color:#7c3aed}
        .exam-report-page .message-area{display:block;margin:0 0 12px;font-weight:600;color:#475569}
        .exam-report-page .table-wrap{width:100%;overflow-x:auto;overflow-y:visible;border:1px solid #e2e8f0;border-radius:8px}
        .exam-report-page .report-table{width:100%;min-width:1850px;margin:0;border-collapse:separate;border-spacing:0}
        .exam-report-page .report-table th{background:#17365d;color:#fff;text-align:center;vertical-align:middle;white-space:nowrap;font-weight:700;padding:11px 9px;border:0;border-right:1px solid rgba(255,255,255,.16)}
        .exam-report-page .report-table td{vertical-align:middle;padding:9px;border:0;border-bottom:1px solid #e2e8f0;white-space:nowrap;background:#fff}
        .exam-report-page .report-table tr:hover td{background:#f8fafc}
        .exam-report-page .comparison-table{min-width:900px}
        .exam-report-page .empty-data{text-align:center;padding:28px;color:#64748b;font-weight:600}
        .exam-report-page .result-pass{color:#15803d;font-weight:800;background:#dcfce7;padding:4px 9px;border-radius:20px;display:inline-block}
        .exam-report-page .result-fail{color:#b91c1c;font-weight:800;background:#fee2e2;padding:4px 9px;border-radius:20px;display:inline-block}
        .exam-report-page .percentage-text{font-weight:800;color:#1d4ed8}
        .exam-report-page .improvement-positive{color:#15803d;font-weight:800}.exam-report-page .improvement-negative{color:#b91c1c;font-weight:800}
        .exam-report-page .export-btn{white-space:nowrap}
        @media(max-width:900px){.exam-report-page{padding:12px}.exam-report-page .summary-grid{grid-template-columns:repeat(2,1fr)}.exam-report-page .card-body{padding:14px}}
        @media(max-width:575px){.exam-report-page .summary-grid{grid-template-columns:1fr}.exam-report-page .page-title{font-size:22px}.exam-report-page .filter-actions{align-items:stretch}.exam-report-page .filter-actions .btn{flex:1}}
        .exam-report-page .filter-hint{font-size:12px;opacity:.88;margin-top:3px}
        .exam-report-page .filter-toolbar{display:flex;gap:8px;align-items:center}
        .exam-report-page .filter-body{padding:20px}
        .exam-report-page .filter-grid{display:grid;grid-template-columns:repeat(4,minmax(0,1fr));gap:16px}
        .exam-report-page .filter-item{min-width:0}
        .exam-report-page .filter-wide{grid-column:span 1}
        .exam-report-page .exam-date{background:#fff url('data:image/svg+xml,%3Csvg xmlns=%22http://www.w3.org/2000/svg%22 width=%2216%22 height=%2216%22 fill=%22%2364758b%22 viewBox=%220 0 16 16%3E%3Cpath d=%22M3.5 0a.5.5 0 0 1 .5.5V1h8V.5a.5.5 0 0 1 1 0V1h.5A1.5 1.5 0 0 1 15 2.5v12a1.5 1.5 0 0 1-1.5 1.5h-11A1.5 1.5 0 0 1 1 14.5v-12A1.5 1.5 0 0 1 2.5 1H3V.5a.5.5 0 0 1 .5-.5zM2 5v9.5a.5.5 0 0 0 .5.5h11a.5.5 0 0 0 .5-.5V5H2z%22/%3E%3C/svg%3E') no-repeat right 12px center;background-size:16px;padding-right:38px}
        .exam-report-page .result-toolbar{display:flex;align-items:center;justify-content:space-between;gap:12px}
        .exam-report-page .result-count{font-size:12px;opacity:.9}
        .exam-report-page .report-table th{position:sticky;top:0;z-index:2}
        @media(max-width:1100px){.exam-report-page .filter-grid{grid-template-columns:repeat(3,minmax(0,1fr))}}
        @media(max-width:760px){.exam-report-page .filter-grid{grid-template-columns:repeat(2,minmax(0,1fr))}.exam-report-page .filter-wide{grid-column:span 1}}
        @media(max-width:520px){.exam-report-page .filter-grid{grid-template-columns:1fr}.exam-report-page .filter-toolbar{width:100%}.exam-report-page .filter-toolbar .btn{flex:1}}
    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="exam-report-page">

        <!-- ================================================= -->
        <!-- PAGE TITLE                                        -->
        <!-- ================================================= -->

        <div class="exam-header">

            <div>

                <span class="page-title">Exam Result Report
                </span>

            </div>

        </div>


        <asp:label
            id="lblMessage"
            runat="server"
            cssclass="message-area">
        </asp:label>


        <!-- ================================================= -->
        <!-- FILTERS -->
        <div class="card report-card">
            <div class="report-card-header">
                <div>
                    <span class="section-title">Exam Result Filters</span>
                    <div class="filter-hint">Select the required filters and click Search. Filters are applied together.</div>
                </div>
                <div class="filter-toolbar">
                    <asp:Button ID="btnSearch" runat="server" Text="Search Results" CssClass="btn btn-light btn-sm" OnClick="btnSearch_Click" />
                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-outline-light btn-sm" CausesValidation="false" OnClick="btnReset_Click" />
                </div>
            </div>
            <div class="card-body filter-body">
                <div class="filter-grid">
                    <div class="filter-item filter-wide">
                        <label class="filter-label">Training</label>
                        <asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="filter-item">
                        <label class="filter-label">Course</label>
                        <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="filter-item">
                        <label class="filter-label">Batch</label>
                        <asp:TextBox ID="txtBatch" runat="server" CssClass="form-control" MaxLength="100" placeholder="Batch"></asp:TextBox>
                    </div>
                    <div class="filter-item">
                        <label class="filter-label">Exam Type</label>
                        <asp:DropDownList ID="ddlTestType" runat="server" CssClass="form-control">
                            <asp:ListItem Text="All Exam Types" Value=""></asp:ListItem>
                            <asp:ListItem Text="Pre Training Exam" Value="Pre"></asp:ListItem>
                            <asp:ListItem Text="Post Training Exam" Value="Post"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="filter-item filter-wide">
                        <label class="filter-label">Test</label>
                        <asp:DropDownList ID="ddlTest" runat="server" CssClass="form-control"></asp:DropDownList>
                    </div>
                    <div class="filter-item filter-wide">
                        <label class="filter-label">Trainee ID / Name</label>
                        <asp:TextBox ID="txtTrainee" runat="server" CssClass="form-control" MaxLength="150" placeholder="Enter ID or name"></asp:TextBox>
                    </div>
                    <div class="filter-item">
                        <label class="filter-label">Result</label>
                        <asp:DropDownList ID="ddlResultStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Text="All Results" Value=""></asp:ListItem>
                            <asp:ListItem Text="Pass" Value="PASS"></asp:ListItem>
                            <asp:ListItem Text="Fail" Value="FAIL"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="filter-item">
                        <label class="filter-label">Attempt</label>
                        <asp:DropDownList ID="ddlAttempt" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Final Attempt" Value="Final" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="All Attempts" Value="All"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="filter-item">
                        <label class="filter-label">Submitted From</label>
                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control exam-date" MaxLength="10" placeholder="dd-MM-yyyy"></asp:TextBox>
                    </div>
                    <div class="filter-item">
                        <label class="filter-label">Submitted To</label>
                        <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control exam-date" MaxLength="10" placeholder="dd-MM-yyyy"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>

        <!-- SUMMARY                                           -->
        <!-- ================================================= -->

        <asp:panel
            id="pnlSummary"
            runat="server"
            visible="false">

            <div class="summary-grid">

                <!-- Appeared -->

                <div class="summary-card-wrap">

                    <div class="summary-box">

                        <span class="summary-title">
                            Total Results
                        </span>

                        <asp:Label
                            ID="lblTotalResults"
                            runat="server"
                            Text="0"
                            CssClass="summary-value">
                        </asp:Label>

                    </div>

                </div>


                <!-- Passed -->

                <div class="summary-card-wrap">

                    <div class="summary-box passed">

                        <span class="summary-title">
                            Passed
                        </span>

                        <asp:Label
                            ID="lblPassed"
                            runat="server"
                            Text="0"
                            CssClass="summary-value passed-value">
                        </asp:Label>

                    </div>

                </div>


                <!-- Failed -->

                <div class="summary-card-wrap">

                    <div class="summary-box failed">

                        <span class="summary-title">
                            Failed
                        </span>

                        <asp:Label
                            ID="lblFailed"
                            runat="server"
                            Text="0"
                            CssClass="summary-value failed-value">
                        </asp:Label>

                    </div>

                </div>


                <!-- Average -->

                <div class="summary-card-wrap">

                    <div class="summary-box average">

                        <span class="summary-title">
                            Average Percentage
                        </span>

                        <asp:Label
                            ID="lblAveragePercentage"
                            runat="server"
                            Text="0.00 %"
                            CssClass="summary-value average-value">
                        </asp:Label>

                    </div>

                </div>

            </div>

        </asp:panel>


        <!-- ================================================= -->
        <!-- RESULT GRID                                       -->
        <!-- ================================================= -->

        <div class="card report-card">

            <div class="report-card-header">

                <div class="row">

                    <div class="col-md-8">

                        <span class="section-title">Trainee Exam Results
                        </span>

                    </div>


                    <div class="col-md-4 text-right">

                        <asp:button
                            id="btnExportResult"
                            runat="server"
                            text="Export Excel"
                            cssclass="btn btn-light btn-sm export-btn"
                            causesvalidation="false"
                            onclick="btnExportResult_Click" />

                    </div>

                </div>

            </div>


            <div class="card-body">

                <div class="table-wrap">

                    <asp:gridview
                        id="gvResult"
                        runat="server"
                        autogeneratecolumns="false"
                        cssclass="table table-bordered table-hover report-table"
                        gridlines="None"
                        onrowdatabound="gvResult_RowDataBound">

                        <Columns>


                            <asp:TemplateField
                                HeaderText="Sl. No.">

                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center"
                                    Width="65px" />

                            </asp:TemplateField>



                            <asp:BoundField
                                DataField="TrainingID"
                                HeaderText="Training ID" />



                            <asp:BoundField
                                DataField="CourseName"
                                HeaderText="Course" />



                            <asp:BoundField
                                DataField="Batch"
                                HeaderText="Batch" />



                            <asp:TemplateField
                                HeaderText="Exam">

                                <ItemTemplate>

                                    <%#
                                        Eval("TestType").ToString() == "Pre"
                                        ? "Pre Training"
                                        :
                                        Eval("TestType").ToString() == "Post"
                                        ? "Post Training"
                                        : Eval("TestType").ToString()
                                    %>

                                </ItemTemplate>

                            </asp:TemplateField>



                            <asp:BoundField
                                DataField="TestTitle"
                                HeaderText="Test Title" />



                            <asp:BoundField
                                DataField="EmpID"
                                HeaderText="Trainee ID" />



                            <asp:BoundField
                                DataField="TraineeName"
                                HeaderText="Trainee Name" />



                            <asp:BoundField
                                DataField="AttemptNo"
                                HeaderText="Attempt">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:BoundField
                                DataField="TotalQuestions"
                                HeaderText="Questions">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="AttemptedQuestions"
                                HeaderText="Attempted">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="CorrectAnswers"
                                HeaderText="Correct">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="WrongAnswers"
                                HeaderText="Wrong">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:BoundField
                                DataField="TotalMarks"
                                HeaderText="Total Marks">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>


                            <asp:BoundField
                                DataField="ObtainedMarks"
                                HeaderText="Obtained">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:TemplateField
                                HeaderText="Percentage">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblPercentage"
                                        runat="server"
                                        CssClass="percentage-text"
                                        Text='<%# Eval("Percentage", "{0:0.00}") + " %" %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>



                            <asp:TemplateField
                                HeaderText="Result">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblResult"
                                        runat="server"
                                        Text='<%# Eval("ResultStatus") %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>



                            <asp:BoundField
                                DataField="RankNo"
                                HeaderText="Rank">

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:BoundField>



                            <asp:TemplateField
                                HeaderText="Time Taken">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblTimeTaken"
                                        runat="server"
                                        Text='<%# FormatTimeTaken(Eval("TimeTaken")) %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center"
                                    Wrap="false" />

                            </asp:TemplateField>



                            <asp:BoundField
                                DataField="SubmittedOn"
                                HeaderText="Submitted On"
                                DataFormatString="{0:dd-MM-yyyy hh:mm tt}">

                                <ItemStyle
                                    HorizontalAlign="Center"
                                    Wrap="false" />

                            </asp:BoundField>



                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:HyperLink ID="lnkViewAnswers" runat="server" Text="View Answers" CssClass="btn btn-sm btn-outline-primary" NavigateUrl='<%# "AnswerDetails.aspx?ResultID=" + Eval("ResultID") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Final">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblFinalAttempt"
                                        runat="server"
                                        Text='<%# Convert.ToBoolean(Eval("IsFinalAttempt")) ? "Yes" : "No" %>'>
                                    </asp:Label>

                                </ItemTemplate>

                                <ItemStyle
                                    HorizontalAlign="Center" />

                            </asp:TemplateField>

                        </Columns>


                        <EmptyDataTemplate>

                            <div class="empty-data">
                                No exam result found.
                            </div>

                        </EmptyDataTemplate>

                    </asp:gridview>

                </div>

            </div>

        </div>


        <!-- ================================================= -->
        <!-- PRE VS POST COMPARISON                            -->
        <!-- ================================================= -->

        <div class="card report-card">

            <div class="report-card-header comparison-header">

                <div class="row">

                    <div class="col-md-8">

                        <span class="section-title">Pre vs Post Training Comparison
                        </span>

                    </div>


                    <div class="col-md-4 text-right">

                        <asp:button
                            id="btnExportComparison"
                            runat="server"
                            text="Export Excel"
                            cssclass="btn btn-light btn-sm"
                            causesvalidation="false"
                            onclick="btnExportComparison_Click" />

                    </div>

                </div>

            </div>


            <div class="card-body">

                <div class="table-wrap">

                    <asp:gridview
                        id="gvComparison"
                        runat="server"
                        autogeneratecolumns="false"
                        cssclass="table table-bordered table-hover report-table"
                        gridlines="None"
                        onrowdatabound="gvComparison_RowDataBound">

    <Columns>

        <asp:TemplateField HeaderText="Sl. No.">

            <ItemTemplate>
                <%# Container.DataItemIndex + 1 %>
            </ItemTemplate>

            <ItemStyle
                HorizontalAlign="Center"
                Width="65px" />

        </asp:TemplateField>


        <asp:BoundField
            DataField="TrainingID"
            HeaderText="Training ID" />


        <asp:BoundField
            DataField="CourseName"
            HeaderText="Course" />


        <asp:BoundField
            DataField="Batch"
            HeaderText="Batch" />


        <asp:BoundField
            DataField="EmpID"
            HeaderText="Trainee ID" />


        <asp:BoundField
            DataField="TraineeName"
            HeaderText="Trainee Name" />


        <asp:TemplateField HeaderText="Pre %">

            <ItemTemplate>

                <asp:Label
                    ID="lblPre"
                    runat="server"
                    Text='<%# FormatPercentage(Eval("PrePercentage")) %>'>
                </asp:Label>

            </ItemTemplate>

            <ItemStyle
                HorizontalAlign="Center" />

        </asp:TemplateField>


        <asp:TemplateField HeaderText="Post %">

            <ItemTemplate>

                <asp:Label
                    ID="lblPost"
                    runat="server"
                    Text='<%# FormatPercentage(Eval("PostPercentage")) %>'>
                </asp:Label>

            </ItemTemplate>

            <ItemStyle
                HorizontalAlign="Center" />

        </asp:TemplateField>


        <asp:TemplateField HeaderText="Improvement">

            <ItemTemplate>

                <asp:Label
                    ID="lblImprovement"
                    runat="server"
                    Text='<%# FormatImprovement(Eval("Improvement")) %>'>
                </asp:Label>

            </ItemTemplate>

            <ItemStyle
                HorizontalAlign="Center" />

        </asp:TemplateField>

    </Columns>


    <EmptyDataTemplate>

        <div class="empty-data">

            No Pre/Post comparison data found.

        </div>

    </EmptyDataTemplate>

</asp:gridview>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
