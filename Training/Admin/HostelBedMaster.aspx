<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="HostelBedMaster.aspx.cs"
    Inherits="Training.Admin.HostelBedMaster" %>

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

        .badge-vacant {
            background: #198754;
            color: #fff;
            padding: 3px 10px;
            border-radius: 10px;
        }

        .badge-occupied {
            background: #dc3545;
            color: #fff;
            padding: 3px 10px;
            border-radius: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">
    <div class="container-fluid">
        <div class="main-card">
            <div class="page-heading">
                Hostel Bed Master Entry
            </div>
            <asp:HiddenField ID="hfBedID" runat="server" />
            <div class="row">
                <div class="col-md-3 mb-3">
                    <label>Hostel *</label>
                    <asp:DropDownList
                        ID="ddlHostel"
                        runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        CausesValidation="false"
                        OnSelectedIndexChanged="ddlHostel_SelectedIndexChanged">
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

                <div class="col-md-3 mb-3">
                    <label>Block *</label>
                    <asp:DropDownList
                        ID="ddlBlock"
                        runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        CausesValidation="false"
                        OnSelectedIndexChanged="ddlBlock_SelectedIndexChanged">
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

                <div class="col-md-3 mb-3">
                    <label>Room *</label>
                    <asp:DropDownList
                        ID="ddlRoom"
                        runat="server"
                        AutoPostBack="true"
                        CssClass="form-select"
                        OnSelectedIndexChanged="ddlRoom_SelectedIndexChanged">
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator
                        ID="rfvRoom"
                        runat="server"
                        ControlToValidate="ddlRoom"
                        InitialValue=""
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Select a Room">
                    </asp:RequiredFieldValidator>
                </div>

                <div class="col-md-3 mb-3">
                    <label>Bed No *</label>
                    <asp:DropDownList
                        ID="ddlBedNo"
                        runat="server"
                        CssClass="form-control">
                         
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator
                        ID="rfvBedNo"
                        runat="server"
                        ControlToValidate="ddlBedNo"
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Select Bed No">
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
                        Text="Save Bed"
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
                        ID="lblBedMessage"
                        runat="server"
                        Font-Bold="true">
                    </asp:Label>
                </div>
            </div>

            <hr />

            <asp:GridView
                ID="gvBed"
                runat="server"
                AutoGenerateColumns="False"
                OnRowCommand="gvBed_RowCommand"
                CssClass="table table-bordered table-striped gridview">
                <Columns>
                    <asp:BoundField  DataField="BedID" HeaderText="Bed ID" />
                    <asp:BoundField  DataField="HostelName" HeaderText="Hostel" />
                    <asp:BoundField DataField="BlockName" HeaderText="Block" />
                    <asp:BoundField DataField="FlatName" HeaderText="Flat" />
                    <asp:BoundField DataField="RoomNo" HeaderText="Room" />
                    <asp:BoundField DataField="BedNo" HeaderText="Bed No" />
                    <asp:TemplateField HeaderText="Availability">
                        <ItemTemplate>
                            <span class='<%# Eval("Status").ToString() == "Vacant" ? "badge-vacant" : "badge-occupied" %>'>
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="IsActive" HeaderText="Active/Inactive" />
                    <asp:BoundField DataField="CreatedOn" HeaderText="Created On"  DataFormatString="{0:dd-MM-yyyy}" />

                     <asp:TemplateField HeaderText="Action">

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="lnkEditBed"
                                runat="server"
                                Text="Edit"
                                CssClass="btn btn-warning btn-sm"
                                CommandName="EditBed"
                                CommandArgument='<%# Eval("ID") %>' />

                            &nbsp;

                           <asp:LinkButton
                               ID="lnkDeleteBed"
                               runat="server"
                               Text="Delete"
                               CssClass="btn btn-danger btn-sm"
                               CommandName="DeleteBed"
                               CommandArgument='<%# Eval("ID") %>'
                               OnClientClick="return confirm('Are you sure you want to delete this bed? If it has allotment history, it will be marked Inactive instead of deleted.');" />
                        </ItemTemplate>

                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
