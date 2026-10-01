<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AdminOnlineNominations.aspx.cs" Inherits="Training.Admin.AdminOnlineNominations" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4 class="mb-3">Online Nominations</h4>

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
                ID="gvNominations"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm table-hover"
                EmptyDataText="No nominations found."
                OnRowCommand="gvNominations_RowCommand">
                <Columns>
                    <asp:BoundField DataField="NominationID" HeaderText="Ref No." />
                    <asp:BoundField DataField="Name" HeaderText="Name" />
                    <asp:BoundField DataField="Designation" HeaderText="Designation" />
                    <asp:BoundField DataField="EmpID" HeaderText="Emp ID" />
                    <asp:BoundField DataField="Organization" HeaderText="Organization" />
                    <asp:BoundField DataField="MobileNo" HeaderText="Mobile" />
                    <asp:BoundField DataField="CourseName" HeaderText="Training" />
                    <asp:BoundField DataField="SubmittedOn" HeaderText="Submitted On" DataFormatString="{0:dd-MMM-yyyy}" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# Eval("Status").ToString()=="Pending" ? "badge bg-warning text-dark" : (Eval("Status").ToString()=="Approved" ? "badge bg-success" : "badge bg-danger") %>'>
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:Panel runat="server" CssClass="btn-group" Visible='<%# Eval("Status").ToString()=="Pending" %>'>
                                <asp:LinkButton
                                    runat="server"
                                    CssClass="btn btn-success btn-sm"
                                    CommandName="ApproveNomination"
                                    CommandArgument='<%# Eval("NominationID") %>'
                                    OnClientClick="return confirm('Approve this nomination?');">
                                    Approve
                                </asp:LinkButton>
                                <asp:LinkButton
                                    runat="server"
                                    CssClass="btn btn-danger btn-sm"
                                    CommandName="RejectNomination"
                                    CommandArgument='<%# Eval("NominationID") %>'
                                    OnClientClick="return confirm('Reject this nomination?');">
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