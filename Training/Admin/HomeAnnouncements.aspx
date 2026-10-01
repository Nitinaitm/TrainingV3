<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="HomeAnnouncements.aspx.cs" Inherits="Training.Admin.HomeAnnouncements" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4 class="mb-3">Updates, News &amp; Announcements</h4>
    <a href="HomePageManagement.aspx" class="btn btn-outline-secondary btn-sm mb-3">&larr; Back to Home Page Management</a>

    <!-- ================= ADD / EDIT FORM ================= -->
    <div class="card mb-4">
        <div class="card-body">

            <asp:HiddenField ID="hfID" runat="server" Value="0" />

            <div class="row g-3">

                <div class="col-md-3">
                    <label>Category</label>
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select">
                        <asp:ListItem Text="News & Tenders" Value="News"></asp:ListItem>
                        <asp:ListItem Text="Official Circular" Value="Circular"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-md-6">
                    <label>Title</label>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-3">
                    <label>Display Order</label>
                    <asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="form-control" Text="1" />
                </div>

                <div class="col-md-6">
                    <label>Link URL (optional)</label>
                    <asp:TextBox ID="txtLinkUrl" runat="server" CssClass="form-control" placeholder="#" />
                </div>

                <div class="col-md-3">
                    <label>Published Date</label>
                    <asp:TextBox ID="txtPublishedDate" runat="server" CssClass="form-control" TextMode="Date" />
                </div>

                <div class="col-md-3 d-flex align-items-end">
                    <div class="form-check">
                        <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" CssClass="form-check-input" />
                        <label class="form-check-label">Active</label>
                    </div>
                </div>

            </div>

            <div class="mt-3">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel Edit" CssClass="btn btn-secondary" OnClick="btnCancelEdit_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-3" />
            </div>

        </div>
    </div>

    <!-- ================= LIST ================= -->
    <div class="card mb-3">
        <div class="card-body">
            <label>Filter by Category</label>
            <asp:DropDownList ID="ddlFilterCategory" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFilterCategory_SelectedIndexChanged" style="max-width:250px;">
                <asp:ListItem Text="All" Value=""></asp:ListItem>
                <asp:ListItem Text="News & Tenders" Value="News"></asp:ListItem>
                <asp:ListItem Text="Official Circular" Value="Circular"></asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <div class="card">
        <div class="card-body table-responsive">
            <asp:GridView
                ID="gvAnnouncements"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm table-hover align-middle"
                DataKeyNames="ID"
                EmptyDataText="No announcements added yet."
                OnRowCommand="gvAnnouncements_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Category">
                        <ItemTemplate>
                            <span class='<%# Eval("Category").ToString()=="News" ? "badge bg-info text-dark" : "badge bg-warning text-dark" %>'>
                                <%# Eval("Category") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Title" HeaderText="Title" />
                    <asp:BoundField DataField="PublishedDate" HeaderText="Published" DataFormatString="{0:dd-MMM-yyyy}" />
                    <asp:BoundField DataField="DisplayOrder" HeaderText="Order" />
                    <asp:TemplateField HeaderText="Active">
                        <ItemTemplate>
                            <span class='<%# Convert.ToBoolean(Eval("IsActive")) ? "badge bg-success" : "badge bg-secondary" %>'>
                                <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="EditAnnouncement" CommandArgument='<%# Eval("ID") %>'>Edit</asp:LinkButton>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="DeleteAnnouncement" CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('Delete this announcement?');">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>