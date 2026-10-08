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
        .exam-report-page .summary-box{background:#fff;border:1px solid #e2e8f0;border-radius:12px;padding:18px;text-align:left;min-height:105px;box-shadow:0 3px 12px rgba(15,23,42,.05);position:relative;overflow:hidden}
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
    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="exam-report-page">

        <!-- ================================================= -->
        <!-- PAGE TITLE                                        -->
        <!-- ================================================= -->

        <div class="exam-header">

            <div class="col-md-12">

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
        <!-- FILTERS                                           -->
        <!-- ================================================= -->

        <div class="card report-card">

            <div class="report-card-header">

                <b>Search Result
                </b>

            </div>

            <div class="card-body">

                <div class="row">

                    <!-- Training -->

                    <div class="col-md-4 mb-3">

                        <label class="filter-label">
                            Training
                        </label>

                        <asp:dropdownlist
                            id="ddlTraining"
                            runat="server"
                            cssclass="form-control"
                            autopostback="true"
                            onselectedindexchanged="ddlTraining_SelectedIndexChanged">
                        </asp:dropdownlist>

                    </div>


                    <!-- Course -->

                    <div class="col-md-4 mb-3">

                        <label class="filter-label">
                            Course
                        </label>

                        <asp:dropdownlist
                            id="ddlCourse"
                            runat="server"
                            cssclass="form-control">
                        </asp:dropdownlist>

                    </div>


                    <!-- Batch -->

                    <div class="col-md-4 mb-3">

                        <label class="filter-label">
                            Batch
                        </label>

                        <asp:textbox
                            id="txtBatch"
                            runat="server"
                            cssclass="form-control"
                            maxlength="100"
                            placeholder="Batch">
                        </asp:textbox>

                    </div>

                </div>


                <div class="row">

                    <!-- Test Type -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Test Type
                        </label>

                        <asp:dropdownlist
                            id="ddlTestType"
                            runat="server"
                            cssclass="form-control"
                            autopostback="true"
                            onselectedindexchanged="ddlTestType_SelectedIndexChanged">

                            <asp:ListItem
                                Text="-- All Test Types --"
                                Value="">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Pre Training Exam"
                                Value="Pre">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Post Training Exam"
                                Value="Post">
                            </asp:ListItem>

                        </asp:dropdownlist>

                    </div>


                    <!-- Test -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Test
                        </label>

                        <asp:dropdownlist
                            id="ddlTest"
                            runat="server"
                            cssclass="form-control">
                        </asp:dropdownlist>

                    </div>


                    <!-- Trainee -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Trainee ID / Name
                        </label>

                        <asp:textbox
                            id="txtTrainee"
                            runat="server"
                            cssclass="form-control"
                            maxlength="150"
                            placeholder="Search trainee">
                        </asp:textbox>

                    </div>


                    <!-- Result -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Result Status
                        </label>

                        <asp:dropdownlist
                            id="ddlResultStatus"
                            runat="server"
                            cssclass="form-control">

                            <asp:ListItem
                                Text="-- All Results --"
                                Value="">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Pass"
                                Value="PASS">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Fail"
                                Value="FAIL">
                            </asp:ListItem>

                        </asp:dropdownlist>

                    </div>

                </div>


                <div class="row">

                    <!-- Attempt -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Attempt
                        </label>

                        <asp:dropdownlist
                            id="ddlAttempt"
                            runat="server"
                            cssclass="form-control">

                            <asp:ListItem
                                Text="Final Attempt"
                                Value="Final"
                                Selected="True">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="All Attempts"
                                Value="All">
                            </asp:ListItem>

                        </asp:dropdownlist>

                    </div>


                    <!-- From Date -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Submitted From
                        </label>

                        <asp:textbox
                            id="txtFromDate"
                            runat="server"
                            cssclass="form-control"
                            maxlength="10"
                            placeholder="dd-MM-yyyy">
                        </asp:textbox>

                    </div>


                    <!-- To Date -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            Submitted To
                        </label>

                        <asp:textbox
                            id="txtToDate"
                            runat="server"
                            cssclass="form-control"
                            maxlength="10"
                            placeholder="dd-MM-yyyy">
                        </asp:textbox>

                    </div>


                    <!-- Buttons -->

                    <div class="col-md-3 mb-3">

                        <label class="filter-label">
                            &nbsp;
                        </label>

                        <asp:button
                            id="btnSearch"
                            runat="server"
                            text="Search"
                            cssclass="btn btn-primary"
                            onclick="btnSearch_Click" />

                        <asp:button
                            id="btnReset"
                            runat="server"
                            text="Reset"
                            cssclass="btn btn-secondary"
                            causesvalidation="false"
                            onclick="btnReset_Click" />

                    </div>

                </div>

            </div>

        </div>


        <!-- ================================================= -->
        <!-- SUMMARY                                           -->
        <!-- ================================================= -->

        <asp:panel
            id="pnlSummary"
            runat="server"
            visible="false">

            <div class="summary-grid">

                <!-- Appeared -->

                <div class="col-md-3">

                    <div class="summary-box passed">

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

                <div class="col-md-3">

                    <div class="summary-box failed">

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

                <div class="col-md-3">

                    <div class="summary-box average">

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

                <div class="col-md-3">

                    <div class="summary-box">

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
