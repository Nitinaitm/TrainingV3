<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="HomeTestimonials.aspx.cs" Inherits="Training.Admin.HomeTestimonials" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4 class="mb-3">What Our Trainees Say</h4>
    <a href="HomePageManagement.aspx" class="btn btn-outline-secondary btn-sm mb-3">&larr; Back to Home Page Management</a>

    <!-- ================= ADD / EDIT FORM ================= -->
    <div class="card mb-4">
        <div class="card-body">

            <asp:HiddenField ID="hfID" runat="server" Value="0" />

            <div class="row g-3">

                <div class="col-12">
                    <label>Quote</label>
                    <asp:TextBox ID="txtQuote" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                </div>

                <div class="col-md-4">
                    <label>Name</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-4">
                    <label>Role / Designation</label>
                    <asp:TextBox ID="txtRole" runat="server" CssClass="form-control" />
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

    <!-- ================= FILTER ================= -->
    <div class="card mb-3">
        <div class="card-body">
            <label>Filter by Status</label>
            <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged" style="max-width:250px;">
                <asp:ListItem Text="Pending" Value="Pending" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Approved" Value="Approved"></asp:ListItem>
                <asp:ListItem Text="Rejected" Value="Rejected"></asp:ListItem>
                <asp:ListItem Text="All" Value=""></asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <!-- ================= LIST ================= -->
    <div class="card">
        <div class="card-body table-responsive">
            <asp:GridView
                ID="gvTestimonials"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm table-hover align-middle"
                DataKeyNames="ID"
                EmptyDataText="No testimonials found for the selected filter."
                OnRowCommand="gvTestimonials_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Quote" HeaderText="Quote" />
                    <asp:BoundField DataField="Name" HeaderText="Name" />
                    <asp:BoundField DataField="Role" HeaderText="Role" />
                    <asp:BoundField DataField="SubmittedBy" HeaderText="Submitted By" />
                    <asp:BoundField DataField="DisplayOrder" HeaderText="Order" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# Eval("Status").ToString()=="Pending" ? "badge bg-warning text-dark" : (Eval("Status").ToString()=="Approved" ? "badge bg-success" : "badge bg-danger") %>'>
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active">
                        <ItemTemplate>
                            <span class='<%# Convert.ToBoolean(Eval("IsActive")) ? "badge bg-success" : "badge bg-secondary" %>'>
                                <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <div class="btn-group mb-1">
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-primary" CommandName="EditTestimonial" CommandArgument='<%# Eval("ID") %>'>Edit</asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger" CommandName="DeleteTestimonial" CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('Delete this testimonial?');">Delete</asp:LinkButton>
                            </div>
                            <asp:Panel runat="server" CssClass="btn-group" Visible='<%# Eval("Status").ToString()=="Pending" %>'>
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-success" CommandName="ApproveTestimonial" CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('Approve and publish this testimonial?');">Approve</asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="btn btn-sm btn-danger" CommandName="RejectTestimonial" CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('Reject this testimonial?');">Reject</asp:LinkButton>
                            </asp:Panel>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>