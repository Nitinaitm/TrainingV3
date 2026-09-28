<%@ page language="C#" autoeventwireup="true"
    codebehind="MySessions.aspx.cs"
    inherits="Training.Trainee.MySessions"
    masterpagefile="~/TraineeMaster.Master" %>

<%@ register
    src="~/Trainee/SessionSummary.ascx"
    tagprefix="uc1"
    tagname="SessionSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .card-header {
            font-weight: 600;
        }

        .info-label {
            font-weight: bold;
            color: #495057;
        }

        .status-badge {
            font-size: 13px;
            padding: 6px 12px;
        }
    </style>

</asp:Content>

<asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">

<div class="container-fluid">
      <uc1:SessionSummary
            ID="SessionSummary1"
            runat="server" />
<div class="row" >

<div class="col-md-12">

<div class="card shadow-sm">




<div class="card-body">



<hr />
    <div class="row">

    <div class="col-md-6">

        <div class="card border-success mb-3">

            <div class="card-header bg-success text-white">

                <i class="fa fa-pencil-square-o"></i>
                Pre Test

            </div>

            <div class="card-body">

                <div class="row">

                    <div class="col-md-12 text-center">

                        <asp:Label
                            ID="lblPreStatus"
                            runat="server"
                            CssClass="badge badge-secondary status-badge"
                            Text="Not Published">
                        </asp:Label>

                    </div>

                </div>

                <hr />

                <div class="text-center">

                    <asp:Button
                        ID="btnPreTest"
                        runat="server"
                        Text="Start Pre Test"
                        CssClass="btn btn-success"
                        OnClick="btnPreTest_Click" />

                </div>

            </div>

        </div>

    </div>

    <div class="col-md-6">

        <div class="card border-primary mb-3">

            <div class="card-header bg-primary text-white">

                <i class="fa fa-pencil"></i>
                Post Test

            </div>

            <div class="card-body">

                <div class="row">

                    <div class="col-md-12 text-center">

                        <asp:Label
                            ID="lblPostStatus"
                            runat="server"
                            CssClass="badge badge-secondary status-badge"
                            Text="Not Published">
                        </asp:Label>

                    </div>

                </div>

                <hr />

                <div class="text-center">

                    <asp:Button
                        ID="btnPostTest"
                        runat="server"
                        Text="Start Post Test"
                        CssClass="btn btn-primary"
                        OnClick="btnPostTest_Click" />

                </div>

            </div>

        </div>

    </div>

</div>

<hr />

<div class="row">

    <div class="col-md-12 text-center">

        <asp:Button
            ID="btnBack"
            runat="server"
            Text="Back"
            CssClass="btn btn-secondary"
            CausesValidation="false"
            OnClick="btnBack_Click" />

        <asp:Button
            ID="btnExam"
            runat="server"
            Text="My Exams"
            CssClass="btn btn-primary"
            CausesValidation="false"
            OnClick="btnExam_Click" />

    </div>

</div>

</div>

</div>

</div>

</div>

</div>

</asp:content>
