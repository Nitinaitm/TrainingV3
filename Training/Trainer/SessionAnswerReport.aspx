<%@ page title="Session Answer Report" language="C#" masterpagefile="~/TrainerMaster.Master" autoeventwireup="true" codebehind="SessionAnswerReport.aspx.cs" inherits="Training.Trainer.SessionAnswerReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .sa-card {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 2px 10px rgba(0,0,0,.08);
            margin: 20px 0
        }

        .sa-title {
            font-size: 24px;
            font-weight: 700;
            color: #0d6efd
        }

        .sa-table th {
            background: #0d6efd;
            color: #fff;
            text-align: center;
            white-space: nowrap
        }

        .sa-table td {
            vertical-align: middle
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="sa-card p-3">
            <div class="sa-title">Session Answer Details Report</div>
            <asp:label id="lblSession" runat="server" cssclass="text-muted" />
            <div class="table-responsive mt-3">
                <asp:gridview id="gvAnswers" runat="server" autogeneratecolumns="false" cssclass="table table-bordered table-hover sa-table" datakeynames="ResultID" onrowcommand="gvAnswers_RowCommand"><Columns><asp:TemplateField HeaderText="#"><ItemTemplate><%# Container.DataItemIndex+1 %></ItemTemplate></asp:TemplateField><asp:BoundField DataField="EmpID" HeaderText="Trainee ID" /><asp:BoundField DataField="TraineeName" HeaderText="Trainee Name" /><asp:BoundField DataField="TestType" HeaderText="Exam" /><asp:BoundField DataField="TestTitle" HeaderText="Test" /><asp:BoundField DataField="AttemptNo" HeaderText="Attempt" /><asp:BoundField DataField="Percentage" HeaderText="Percentage" DataFormatString="{0:0.00}%" /><asp:BoundField DataField="ResultStatus" HeaderText="Result" /><asp:TemplateField HeaderText="Action"><ItemTemplate><asp:Button ID="btnView" runat="server" Text="View Answers" CssClass="btn btn-sm btn-outline-primary" CommandName="ViewAnswers" CommandArgument='<%# Container.DataItemIndex %>' CausesValidation="false" /></ItemTemplate></asp:TemplateField></Columns><EmptyDataTemplate><div class="text-center p-3 text-muted">No answer details found for this session.</div></EmptyDataTemplate></asp:gridview>
            </div>
            <asp:button id="btnBack" runat="server" text="Back" cssclass="btn btn-secondary" onclick="btnBack_Click" />
        </div>
    </div>
</asp:Content>
