<%@ Page Title="Share Your Feedback" Language="C#" MasterPageFile="~/TraineeMaster.Master" AutoEventWireup="true" CodeBehind="SubmitTestimonial.aspx.cs" Inherits="Training.Trainee.SubmitTestimonial" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <h3 class="page-title mb-3">Share Your Feedback</h3>
        <p class="text-muted">Your feedback will be reviewed by the Admin before it appears on the homepage.</p>

        <div class="card">
            <div class="card-body">

                <div class="row g-3">
                    <div class="col-md-6 mb-2"><strong>Name</strong><br /><asp:Label ID="lblName" runat="server" /></div>
                    <div class="col-md-6 mb-2"><strong>Designation</strong><br /><asp:Label ID="lblRole" runat="server" /></div>

                    <div class="col-12">
                        <label class="form-label fw-semibold">Your Feedback <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtQuote" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" placeholder="Tell us about your training experience..." />
                        <asp:RequiredFieldValidator
                            ID="rfvQuote"
                            runat="server"
                            ControlToValidate="txtQuote"
                            ErrorMessage="Please share your feedback."
                            CssClass="text-danger small"
                            Display="Dynamic" />
                    </div>
                </div>

                <div class="mt-4">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit Feedback" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                    <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-3" />
                </div>

            </div>
        </div>

    </div>

</asp:Content>