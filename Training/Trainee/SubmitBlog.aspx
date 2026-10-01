<%@ Page Title="Submit a Blog Post" Language="C#" MasterPageFile="~/TraineeMaster.Master" AutoEventWireup="true" CodeBehind="SubmitBlog.aspx.cs" Inherits="Training.Trainee.SubmitBlog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <h3 class="page-title mb-3">Submit a Blog Post</h3>
        <p class="text-muted">Submitting as <asp:Label ID="lblSubmittingAs" runat="server" CssClass="fw-semibold" /> - your post will be reviewed by the Admin before it appears on the homepage.</p>

        <div class="card">
            <div class="card-body">

                <div class="row g-3">

                    <div class="col-md-4">
                        <label class="form-label fw-semibold">Category <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtCategory" runat="server" CssClass="form-control" placeholder="e.g. Safety, Technology, Policy" />
                        <asp:RequiredFieldValidator ID="rfvCategory" runat="server" ControlToValidate="txtCategory" ErrorMessage="Category is required." CssClass="text-danger small" Display="Dynamic" />
                    </div>

                    <div class="col-md-8">
                        <label class="form-label fw-semibold">Title <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle" ErrorMessage="Title is required." CssClass="text-danger small" Display="Dynamic" />
                    </div>

                    <div class="col-12">
                        <label class="form-label fw-semibold">Excerpt <span class="text-danger">*</span></label>
                        <asp:TextBox ID="txtExcerpt" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" placeholder="A short one or two line summary shown on the homepage card." />
                        <asp:RequiredFieldValidator ID="rfvExcerpt" runat="server" ControlToValidate="txtExcerpt" ErrorMessage="Excerpt is required." CssClass="text-danger small" Display="Dynamic" />
                    </div>

                    <div class="col-12">
                        <label class="form-label fw-semibold">Full Content (optional)</label>
                        <asp:TextBox ID="txtFullContent" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6" placeholder="The full article, if you want it shown when someone clicks Read More." />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label fw-semibold">External Link (optional)</label>
                        <asp:TextBox ID="txtUrl" runat="server" CssClass="form-control" placeholder="If your post links elsewhere instead of Full Content above" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label fw-semibold">Image (optional)</label>
                        <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" />
                        <div class="small text-muted mt-1">JPG or PNG, max 2 MB.</div>
                    </div>

                </div>

                <div class="mt-4">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit Blog Post" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                    <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-3" />
                </div>

            </div>
        </div>

    </div>

</asp:Content>