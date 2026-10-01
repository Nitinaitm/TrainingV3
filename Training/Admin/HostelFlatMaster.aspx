<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="HostelFlatMaster.aspx.cs"
    Inherits="Training.Admin.HostelFlatMaster" %>

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
                Hostel Flat Master Entry
            </div>
            <asp:HiddenField ID="hfFlatID" runat="server" />
            <div class="row">
                 <div class="col-md-4 mb-3">
                    <label>Hostel *</label>
                    <asp:DropDownList
                        ID="ddlHostel"
                        runat="server"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlHostel_SelectedIndexChanged"
                        CausesValidation="false"
                        CssClass="form-select">
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator
                        ID="rfvHostel"
                        runat="server"
                        ControlToValidate="ddlHostel"
                        InitialValue=""
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Select a Hostel">
                    </asp:RequiredFieldValidator>
                </div>
                
                
                <div class="col-md-4 mb-3">
                    <label>Block *</label>
                    <asp:DropDownList
                        ID="ddlBlock"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator
                        ID="rfvBlock"
                        runat="server"
                        ControlToValidate="ddlBlock"
                        InitialValue=""
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Select a Block">
                    </asp:RequiredFieldValidator>
                </div>

                <div class="col-md-4 mb-3">
                    <label>Flat Name *</label>
                    <asp:TextBox
                        ID="txtFlatName"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="50">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvFlatName"
                        runat="server"
                        ControlToValidate="txtFlatName"
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Enter Flat Name">
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

                <div class="col-md-12">
                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Save Flat"
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
                        ID="lblFlatMessage"
                        runat="server"
                        Font-Bold="true">
                    </asp:Label>
                </div>
            </div>

            <hr />

            

            <asp:GridView
                ID="gvFlat"
                runat="server"
                AutoGenerateColumns="False"
                OnRowCommand="gvFlat_RowCommand"
                CssClass="table table-bordered table-striped gridview">
                <Columns>
                    <asp:BoundField DataField="FlatID" HeaderText="Flat ID" />
                    <asp:BoundField DataField="HostelName" HeaderText="Hostel" />
                    <asp:BoundField DataField="BlockName" HeaderText="Block" />
                    <asp:BoundField DataField="FlatName" HeaderText="Flat Name" />
                    <asp:BoundField DataField="IsActive" HeaderText="Status" />
                    <asp:BoundField DataField="CreatedOn" HeaderText="Created On" DataFormatString="{0:dd-MM-yyyy}" />
                     <asp:TemplateField HeaderText="Action">

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="lnkEditFlat"
                                runat="server"
                                Text="Edit"
                                CssClass="btn btn-warning btn-sm"
                                CommandName="EditFlat"
                                CommandArgument='<%# Eval("ID") %>' />

                            &nbsp;

                            <asp:LinkButton
                                ID="lnkDeleteFlat"
                                runat="server"
                                Text="Delete"
                                CssClass="btn btn-danger btn-sm"
                                CommandName="DeleteFlat"
                                CommandArgument='<%# Eval("ID") %>'
                                OnClientClick="return confirm('Delete Flat?');" />

                        </ItemTemplate>

                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>