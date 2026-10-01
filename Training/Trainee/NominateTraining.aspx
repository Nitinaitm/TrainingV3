<%@ Page Title="Nominate for Training" Language="C#" MasterPageFile="~/TraineeMaster.Master" AutoEventWireup="true" CodeBehind="NominateTraining.aspx.cs" Inherits="Training.Trainee.NominateTraining" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <h3 class="page-title mb-3">Nominate for Training</h3>

        <!-- ================= MY DETAILS (READ-ONLY) ================= -->
        <div class="card mb-3 shadow-sm" style="border-left:4px solid #198754;">
            <div class="card-body">
                <h5 class="mb-3">Your Details</h5>
                <div class="row">
                    <div class="col-md-3 mb-2"><strong>Employee ID</strong><br /><asp:Label ID="lblEmpID" runat="server" /></div>
                    <div class="col-md-3 mb-2"><strong>Name</strong><br /><asp:Label ID="lblName" runat="server" /></div>
                    <div class="col-md-3 mb-2"><strong>Designation</strong><br /><asp:Label ID="lblDesignation" runat="server" /></div>
                    <div class="col-md-3 mb-2"><strong>Organization</strong><br /><asp:Label ID="lblOrganization" runat="server" /></div>
                </div>
                <div class="row">
                    <div class="col-md-4 mb-2"><strong>Current Posting</strong><br /><asp:Label ID="lblCurrentPosting" runat="server" /></div>
                    <div class="col-md-4 mb-2"><strong>Mobile</strong><br /><asp:Label ID="lblMobile" runat="server" /></div>
                    <div class="col-md-4 mb-2"><strong>Email</strong><br /><asp:Label ID="lblEmail" runat="server" /></div>
                </div>
                <p class="small text-muted mb-0">These details come from your employee record. If anything here is wrong, use "Request a Correction" on My Trainings first.</p>
            </div>
        </div>

        <!-- ================= NOMINATION FORM ================= -->
        <div class="card">
            <div class="card-body">

                <div class="row g-3">
                    <div class="col-md-8">
                        <label class="form-label fw-semibold">Select Training <span class="text-danger">*</span></label>
                        <asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-select" AppendDataBoundItems="True">
                            <asp:ListItem Text="-- Select Training --" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator
                            ID="rfvTraining"
                            runat="server"
                            ControlToValidate="ddlTraining"
                            InitialValue=""
                            ErrorMessage="Please select a training."
                            CssClass="text-danger small"
                            Display="Dynamic" />
                    </div>

                    <div class="col-12">
                        <label class="form-label fw-semibold">Remarks (optional)</label>
                        <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                    </div>
                </div>

                <div class="mt-4">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit Nomination" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                    <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-3" />
                </div>

            </div>
        </div>

    </div>

</asp:Content>