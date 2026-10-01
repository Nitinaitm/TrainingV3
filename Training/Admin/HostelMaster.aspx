<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="HostelMaster.aspx.cs"
    Inherits="Training.Admin.HostelMaster" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />



    <style>
        .main-card {
            background: #fff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 0 10px #d9d9d9;
            margin-top: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #0d6efd;
            margin-bottom: 20px;
        }

        .validation {
            color: red;
            font-size: 13px;
        }

        .gridview th {
            background: #0d6efd;
            color: white;
            text-align: center;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">
    <div class="container-fluid">
        <div class="main-card">
            <div class="page-heading">
                Hostel Master Entry
            </div>

            <asp:HiddenField ID="hfHostelID" runat="server" />



            <div class="row">
                <div class="col-md-4 mb-3">
                    <label>Hostel Name *</label>
                    <asp:TextBox
                        ID="txtHostelName"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="100">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvHostelName"
                        runat="server"
                        ControlToValidate="txtHostelName"
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Enter Hostel Name">
                    </asp:RequiredFieldValidator>
                </div>

                <div class="col-md-4 mb-3">
                    <label>Location *</label>
                    <asp:TextBox
                        ID="txtLocation"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="100">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvLocation"
                        runat="server"
                        ControlToValidate="txtLocation"
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Enter Location">
                    </asp:RequiredFieldValidator>
                </div>

                <div class="col-md-4 mb-3">
                    <label>Status *</label>
                    <asp:DropDownList
                        ID="ddlIsActive"
                        runat="server"
                        CssClass="form-select">

                        <asp:ListItem Text="Active" Value="Y"></asp:ListItem>
                        <asp:ListItem Text="Inactive" Value="N"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-12 mb-3">
                    <label>Description</label>

                    <asp:TextBox
                        ID="txtDescription"
                        runat="server"
                        CssClass="form-control"
                        TextMode="MultiLine"
                        Rows="3"
                        MaxLength="200">
                    </asp:TextBox>
                </div>
                <div class="col-md-12">


                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Save Hostel"
                        CssClass="btn btn-primary"
                        ValidationGroup="SaveGroup"
                        OnClick="btnSave_Click" />
                    <asp:Button
                        ID="btnReset"
                        runat="server"
                        Text="Reset"
                        CssClass="btn btn-secondary ms-2"
                        CausesValidation="false"
                        OnClick="btnReset_Click" />
                </div>
                <div class="col-md-12 mt-3">
                    <asp:Label
                        ID="lblMessage"
                        runat="server"
                        Font-Bold="true">
                    </asp:Label>
                </div>
            </div>


            <hr />
            <asp:GridView
                ID="gvHostel"
                runat="server"
                AutoGenerateColumns="False"
                OnRowCommand="gvHostel_RowCommand"
                CssClass="table table-bordered table-striped gridview">
                <Columns>
                    <asp:TemplateField HeaderText="S.No">
                         <ItemTemplate>
                             <%# Container.DataItemIndex + 1 %>
                         </ItemTemplate>
                     </asp:TemplateField>

                    <asp:BoundField
                        DataField="HostelID"
                        HeaderText="Hostel ID" />
                    <asp:BoundField
                        DataField="HostelName"
                        HeaderText="Hostel Name" />
                    <asp:BoundField
                        DataField="Location"
                        HeaderText="Location" />
                    <asp:BoundField
                        DataField="Description"
                        HeaderText="Description" />
                    <asp:BoundField
                        DataField="IsActive"
                        HeaderText="Status" />

                    <asp:BoundField
                        DataField="CreatedOn"
                        HeaderText="Created On"
                        DataFormatString="{0:dd-MM-yyyy}" />

                    <asp:TemplateField HeaderText="Action">

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="lnkEdit"
                                runat="server"
                                Text="Edit"
                                CssClass="btn btn-warning btn-sm"
                                CommandName="EditHostel"
                                CommandArgument='<%# Eval("ID") %>' />

                            &nbsp;

                            <asp:LinkButton
                                ID="lnkDelete"
                                runat="server"
                                Text="Delete"
                                CssClass="btn btn-danger btn-sm"
                                CommandName="DeleteHostel"
                                CommandArgument='<%# Eval("ID") %>'
                                OnClientClick="return confirm('Delete Hostel?');" />

                        </ItemTemplate>

                    </asp:TemplateField>
                </Columns>

            </asp:GridView>
        </div>
    </div>
</asp:Content>
