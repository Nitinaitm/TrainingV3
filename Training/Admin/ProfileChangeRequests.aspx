<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="ProfileChangeRequests.aspx.cs"
    Inherits="Training.Admin.ProfileChangeRequests"
    ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-3">
        <h4 class="mb-0">Profile Change Requests</h4>
        <a href="Dashboard.aspx" class="btn btn-sm btn-outline-secondary">&larr; Back to Dashboard</a>
    </div>

<asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3" />

    <div class="card mb-3">
        <div class="card-body">
            <div class="row">
                <div class="col-md-3">
                    <label>Status</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                        <asp:ListItem Text="Pending" Value="Pending" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Approved" Value="Approved"></asp:ListItem>
                        <asp:ListItem Text="Rejected" Value="Rejected"></asp:ListItem>
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>
    </div>

    <div class="card">
        <div class="card-body table-responsive">
            <asp:GridView
                ID="gvRequests"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm table-hover"
                EmptyDataText="No requests found."
                OnRowCommand="gvRequests_RowCommand">
                <Columns>
                    <asp:BoundField DataField="RequestID" HeaderText="Request ID" />
                    <asp:BoundField DataField="EmpID" HeaderText="Emp ID" />
                    <asp:BoundField DataField="EmpName" HeaderText="Name" />
                    <asp:BoundField DataField="FieldLabel" HeaderText="Field" />
                    <asp:BoundField DataField="CurrentValue" HeaderText="Current" />
                    <asp:BoundField DataField="RequestedValue" HeaderText="Requested" />
                    <asp:BoundField DataField="Remarks" HeaderText="Remarks" />
                    <asp:BoundField DataField="RequestedOn" HeaderText="Requested On" DataFormatString="{0:dd-MMM-yyyy}" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# Eval("Status").ToString()=="Pending" ? "badge bg-warning text-dark" : (Eval("Status").ToString()=="Approved" ? "badge bg-success" : "badge bg-danger") %>'>
                                <%# Eval("Status") %>
                            </span>
                            <div class="small text-muted">
                                <%# Eval("Status").ToString()!="Pending" ? "by " + Eval("ReviewedBy") + " on " + (Eval("ReviewedOn")==DBNull.Value ? "" : Convert.ToDateTime(Eval("ReviewedOn")).ToString("dd-MMM-yyyy")) : "" %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:Panel runat="server" CssClass="btn-group" Visible='<%# Eval("Status").ToString()=="Pending" %>'>
                                <asp:LinkButton
                                    runat="server"
                                    CssClass="btn btn-success btn-sm"
                                    CommandName="ApproveRequest"
                                    CommandArgument='<%# Eval("RequestID") %>'
                                    OnClientClick="return confirm('Approve this correction? This will update the employee record.');">
                                    Approve
                                </asp:LinkButton>
                                <asp:LinkButton
                                    runat="server"
                                    CssClass="btn btn-danger btn-sm"
                                    CommandName="RejectRequest"
                                    CommandArgument='<%# Eval("RequestID") %>'
                                    OnClientClick="return confirm('Reject this correction request?');">
                                    Reject
                                </asp:LinkButton>
                            </asp:Panel>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>