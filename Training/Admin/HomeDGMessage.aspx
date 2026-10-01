<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="HomeDGMessage.aspx.cs" Inherits="Training.Admin.HomeDGMessage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4 class="mb-3">Director General's Message</h4>
    <a href="HomePageManagement.aspx" class="btn btn-outline-secondary btn-sm mb-3">&larr; Back to Home Page Management</a>

    <div class="card">
        <div class="card-body">

            <div class="row g-3">

                <div class="col-md-6">
                    <label>Name</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-6">
                    <label>Designation</label>
                    <asp:TextBox ID="txtDesignation" runat="server" CssClass="form-control" />
                </div>

                <div class="col-12">
                    <label>Short Quote (shown on the homepage card)</label>
                    <asp:TextBox ID="txtShortQuote" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                </div>

                <div class="col-12">
                    <label>Full Message (shown in the "Read Full Message" popup)</label>
                    <asp:TextBox ID="txtFullMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6" />
                </div>

                <div class="col-md-6">
                    <label>Photo</label>
                    <asp:FileUpload ID="fuPhoto" runat="server" CssClass="form-control" />
                    <div class="small text-muted mt-1">JPG or PNG, max 2 MB. Leave blank to keep the current photo.</div>
                </div>

                <div class="col-md-6">
                    <label class="d-block">Current Photo</label>
                    <asp:Image ID="imgCurrentPhoto" runat="server" Width="100" Height="100" CssClass="rounded-circle border" />
                </div>

            </div>

            <div class="mt-4">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-3" />
            </div>

        </div>
    </div>

</asp:Content>