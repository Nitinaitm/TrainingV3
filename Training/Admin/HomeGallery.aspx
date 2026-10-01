<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="HomeGallery.aspx.cs" Inherits="Training.Admin.HomeGallery" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4 class="mb-3">Media Gallery</h4>
    <a href="HomePageManagement.aspx" class="btn btn-outline-secondary btn-sm mb-3">&larr; Back to Home Page Management</a>

    <!-- ================= ADD / EDIT FORM ================= -->
    <div class="card mb-4">
        <div class="card-body">

            <asp:HiddenField ID="hfID" runat="server" Value="0" />
            <asp:HiddenField ID="hfExistingThumbnailPath" runat="server" Value="" />

            <div class="row g-3">

                <div class="col-md-3">
                    <label>Media Type</label>
                    <asp:DropDownList ID="ddlMediaType" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Photo" Value="photo"></asp:ListItem>
                        <asp:ListItem Text="Video" Value="video"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-md-6">
                    <label>Caption</label>
                    <asp:TextBox ID="txtCaption" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-3">
                    <label>Display Order</label>
                    <asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="form-control" Text="1" />
                </div>

                <div class="col-md-6">
                    <label>Video URL <span class="text-muted small">(only needed if Media Type is Video)</span></label>
                    <asp:TextBox ID="txtVideoUrl" runat="server" CssClass="form-control" placeholder="https://youtube.com/..." />
                </div>

                <div class="col-md-3 d-flex align-items-end">
                    <div class="form-check">
                        <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" CssClass="form-check-input" />
                        <label class="form-check-label">Active</label>
                    </div>
                </div>

                <div class="col-md-6">
                    <label>Thumbnail Image</label>
                    <asp:FileUpload ID="fuThumbnail" runat="server" CssClass="form-control" />
                    <div class="small text-muted mt-1">JPG or PNG, max 2 MB. Required for both Photo and Video entries (the video thumbnail is just a still image). Leave blank when editing to keep the existing thumbnail.</div>
                </div>

                <div class="col-md-3">
                    <label class="d-block">Current Thumbnail</label>
                    <asp:Image ID="imgCurrentThumbnail" runat="server" Width="90" Height="60" CssClass="border rounded" />
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
    <div class="card">
        <div class="card-body table-responsive">
            <asp:GridView
                ID="gvGallery"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm table-hover align-middle"
                DataKeyNames="ID"
                EmptyDataText="No media added yet."
                OnRowCommand="gvGallery_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Thumbnail">
                        <ItemTemplate>
                            <asp:Image runat="server" Width="70" Height="45" CssClass="border rounded" ImageUrl='<%# string.IsNullOrEmpty(Eval("ThumbnailPath").ToString()) ? "" : "~/" + Eval("ThumbnailPath") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Type">
                        <ItemTemplate>
                            <span class='<%# Eval("MediaType").ToString()=="photo" ? "badge bg-info text-dark" : "badge bg-danger" %>'>
                                <%# Eval("MediaType") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Caption" HeaderText="Caption" />
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
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="EditMedia" CommandArgument='<%# Eval("ID") %>'>Edit</asp:LinkButton>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="DeleteMedia" CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('Delete this media item?');">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>