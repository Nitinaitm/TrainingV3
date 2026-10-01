<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="HomeLeadership.aspx.cs" Inherits="Training.Admin.HomeLeadership" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4 class="mb-3">Honorable Members &amp; Leadership</h4>
    <a href="HomePageManagement.aspx" class="btn btn-outline-secondary btn-sm mb-3">&larr; Back to Home Page Management</a>

    <!-- ================= ADD / EDIT FORM ================= -->
    <div class="card mb-4">
        <div class="card-body">

            <asp:HiddenField ID="hfID" runat="server" Value="0" />
            <asp:HiddenField ID="hfExistingPhotoPath" runat="server" Value="" />

            <div class="row g-3">

                <div class="col-md-6">
                    <label>Name</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-6">
                    <label>Designation</label>
                    <asp:TextBox ID="txtDesignation" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-6">
                    <label>Profile URL (optional)</label>
                    <asp:TextBox ID="txtProfileUrl" runat="server" CssClass="form-control" placeholder="#" />
                </div>

                <div class="col-md-3">
                    <label>Display Order</label>
                    <asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="form-control" Text="1" />
                </div>

                <div class="col-md-3 d-flex align-items-end">
                    <div class="form-check">
                        <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" CssClass="form-check-input" />
                        <label class="form-check-label">Active (show on homepage)</label>
                    </div>
                </div>

                <div class="col-md-6">
                    <label>Photo</label>
                    <asp:FileUpload ID="fuPhoto" runat="server" CssClass="form-control" />
                    <div class="small text-muted mt-1">JPG or PNG, max 2 MB. Leave blank to keep the existing photo when editing.</div>
                </div>

                <div class="col-md-6">
                    <label class="d-block">Current Photo</label>
                    <asp:Image ID="imgCurrentPhoto" runat="server" Width="70" Height="70" CssClass="rounded-circle border" />
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
                ID="gvLeadership"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm table-hover align-middle"
                DataKeyNames="ID"
                EmptyDataText="No leadership members added yet."
                OnRowCommand="gvLeadership_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Photo">
                        <ItemTemplate>
                            <asp:Image runat="server" Width="45" Height="45" CssClass="rounded-circle" ImageUrl='<%# string.IsNullOrEmpty(Eval("PhotoPath").ToString()) ? "" : "~/" + Eval("PhotoPath") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Name" />
                    <asp:BoundField DataField="Designation" HeaderText="Designation" />
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
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="EditMember" CommandArgument='<%# Eval("ID") %>'>Edit</asp:LinkButton>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="DeleteMember" CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('Delete this member?');">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>