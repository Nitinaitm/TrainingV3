<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="HostelRoomMaster.aspx.cs"
    Inherits="Training.Admin.HostelRoomMaster" %>

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
                Hostel Room Master Entry
            </div>
             <asp:HiddenField ID="hfRoomID" runat="server" />
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
                    <!-- No RequiredFieldValidator here on purpose -- Flat
                         is genuinely optional. Some blocks don't have any
                         flats at all, in which case this dropdown is just
                         disabled with an explanatory placeholder. -->
                    <label>Flat (optional)</label>
                    <asp:DropDownList
                        ID="ddlFlat"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>
                </div>


                <div class="col-md-3 mb-3">
                    <label>Room No *</label>
                    <asp:TextBox
                        ID="txtRoomNo"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="20">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvRoomNo"
                        runat="server"
                        ControlToValidate="txtRoomNo"
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Enter Room No">
                    </asp:RequiredFieldValidator>
                </div>

                <div class="col-md-3 mb-3">
                    <label>Floor No *</label>
                    <asp:TextBox
                        ID="txtFloorNo"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="3">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvFloorNo"
                        runat="server"
                        ControlToValidate="txtFloorNo"
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Enter Floor No">
                    </asp:RequiredFieldValidator>

                    <asp:CompareValidator
                        ID="cvFloorNo"
                        runat="server"
                        ControlToValidate="txtFloorNo"
                        ValidationGroup="SaveGroup"
                        Operator="DataTypeCheck"
                        Type="Integer"
                        CssClass="validation"
                        ErrorMessage="Floor No must be a number">
                    </asp:CompareValidator>
                </div>

                <div class="col-md-3 mb-3">
                    <label>Room Type *</label>
                    <asp:DropDownList
                        ID="ddlRoomType"
                        runat="server"
                        CssClass="form-select">
                        <asp:ListItem Text="Single" Value="Single"></asp:ListItem>
                        <asp:ListItem Text="Double" Value="Double"></asp:ListItem>
                        <asp:ListItem Text="Triple" Value="Triple"></asp:ListItem>
                        <asp:ListItem Text="Dormitory" Value="Dormitory"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-md-3 mb-3">
                    <label>Capacity *</label>
                    <asp:TextBox
                        ID="txtCapacity"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="3">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvCapacity"
                        runat="server"
                        ControlToValidate="txtCapacity"
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Enter Capacity">
                    </asp:RequiredFieldValidator>

                    <asp:CompareValidator
                        ID="cvCapacity"
                        runat="server"
                        ControlToValidate="txtCapacity"
                        ValidationGroup="SaveGroup"
                        Operator="DataTypeCheck"
                        Type="Integer"
                        CssClass="validation"
                        ErrorMessage="Capacity must be a number">
                    </asp:CompareValidator>
                </div>

                <div class="col-md-3 mb-3">
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
                        Text="Save Room"
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
                        ID="lblRoomMessage"
                        runat="server"
                        Font-Bold="true">
                    </asp:Label>
                </div>
            </div>

            <hr />

            <asp:GridView
                ID="gvRoom"
                runat="server"
                AutoGenerateColumns="False"
                OnRowCommand="gvRoom_RowCommand"
                CssClass="table table-bordered table-striped gridview">
               <Columns>
                    <asp:BoundField DataField="RoomID" HeaderText="Room ID" />
                    <asp:BoundField DataField="HostelName" HeaderText="Hostel" />
                    <asp:BoundField DataField="BlockName" HeaderText="Block" />
                    <asp:BoundField DataField="FlatName" HeaderText="Flat" />
                    <asp:BoundField DataField="RoomNo" HeaderText="Room No" />
                    <asp:BoundField DataField="FloorNo" HeaderText="Floor" />
                    <asp:BoundField DataField="RoomType" HeaderText="Type" />
                    <asp:BoundField DataField="Capacity" HeaderText="Capacity" />
                    <asp:BoundField DataField="IsActive" HeaderText="Status" />
                    <asp:BoundField DataField="CreatedOn" HeaderText="Created On" DataFormatString="{0:dd-MM-yyyy}" />
                    <asp:TemplateField HeaderText="Action">

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="lnkEditRoom"
                                runat="server"
                                Text="Edit"
                                CssClass="btn btn-warning btn-sm"
                                CommandName="EditRoom"
                                CommandArgument='<%# Eval("ID") %>' />

                            &nbsp;

                            <asp:LinkButton
                                ID="lnkDeleteRoom"
                                runat="server"
                                Text="Delete"
                                CssClass="btn btn-danger btn-sm"
                                CommandName="DeleteRoom"
                                CommandArgument='<%# Eval("ID") %>'
                                OnClientClick="return confirm('Delete Room?');" />

                        </ItemTemplate>

                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
