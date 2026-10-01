<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="CloneTest.aspx.cs"
    Inherits="Training.Admin.CloneTest" %>

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
    </style>
</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">
    <div class="container-fluid">
        <div class="main-card">
            <div class="page-heading">
                Clone Test To Another Training
            </div>

            <div class="row mb-3">
                <div class="col-md-8">
                    <label>Existing Published Test *</label>
                    <asp:DropDownList
                        ID="ddlSourceTest"
                        runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlSourceTest_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
            </div>

            <asp:Panel ID="pnlSourceDetails" runat="server" Visible="false" CssClass="card mb-3">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-3"><b>Type:</b> <asp:Label ID="lblSrcType" runat="server" /></div>
                        <div class="col-md-3"><b>Duration (min):</b> <asp:Label ID="lblSrcDuration" runat="server" /></div>
                        <div class="col-md-3"><b>Total Questions:</b> <asp:Label ID="lblSrcTotalQuestions" runat="server" /></div>
                        <div class="col-md-3"><b>Total Marks:</b> <asp:Label ID="lblSrcTotalMarks" runat="server" /></div>
                    </div>
                </div>
            </asp:Panel>

            <hr />

            <div class="row mb-3">
                <div class="col-md-4">
                    <label>Target Training *</label>
                    <asp:DropDownList
                        ID="ddlTargetTraining"
                        runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlTargetTraining_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div class="col-md-4">
                    <label>Target Session *</label>
                    <asp:DropDownList
                        ID="ddlTargetSession"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>
                </div>
                <div class="col-md-4 d-flex align-items-end">
                    <asp:Button
                        ID="btnClone"
                        runat="server"
                        Text="Clone &amp; Publish"
                        CssClass="btn btn-success"
                        OnClick="btnClone_Click"
                        OnClientClick="return confirm('Clone this test and publish it for the selected session?');" />
                </div>
            </div>

            <div class="mt-3">
                <asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>