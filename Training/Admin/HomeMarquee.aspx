<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="HomeMarquee.aspx.cs" Inherits="Training.Admin.HomeMarquee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4 class="mb-3">Scrolling Notice Marquee</h4>
    <a href="HomePageManagement.aspx" class="btn btn-outline-secondary btn-sm mb-3">&larr; Back to Home Page Management</a>

    <div class="card mb-4">
        <div class="card-body">

            <asp:HiddenField ID="hfID" runat="server" Value="0" />

            <div class="row g-3">

                <div class="col-md-8">
                    <label>Notice Text</label>
                    <asp:TextBox ID="txtText" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-2">
                    <label>Display Order</label>
                    <asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="form-control" Text="1" />
                </div>

                <div class="col-md-2 d-flex align-items-end">
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

    <div class="card">
        <div class="card-body table-responsive">
            <asp:GridView
                ID="gvMarquee"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm table-hover align-middle"
                DataKeyNames="ID"
                EmptyDataText="No marquee notices added yet."
                OnRowCommand="gvMarquee_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Text" HeaderText="Notice Text" />
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
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="EditNotice" CommandArgument='<%# Eval("ID") %>'>Edit</asp:LinkButton>
                            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="DeleteNotice" CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('Delete this notice?');">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>