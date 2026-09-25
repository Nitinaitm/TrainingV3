<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="CertificateTemplateMaster.aspx.cs"
    Inherits="Training.Admin.CertificateTemplateMaster"
    MasterPageFile="~/AdminMaster.Master" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" />

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

    <style>
        body {
            background: #f5f5f5;
        }

        .main-card {
            background: #ffffff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0px 0px 10px #d9d9d9;
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: darkcyan;
            margin-bottom: 20px;
        }

        .validation {
            color: red;
            font-size: 13px;
        }

        .btn-save {
            background: darkcyan;
            color: white;
            border: none;
        }

            .btn-save:hover {
                background: teal;
                color: white;
            }

        .select2-container {
            width: 100% !important;
        }

        .select2-container--default
        .select2-selection--single {
            height: 38px !important;
            border: 1px solid #ced4da !important;
        }

        .select2-selection__rendered {
            line-height: 36px !important;
        }

        .select2-selection__arrow {
            height: 36px !important;
        }
    </style>

</asp:Content>


<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <!-- ===================================================== -->
        <!-- PAGE HEADER -->
        <!-- ===================================================== -->

        <div class="card">

            <div class="card-header bg-primary text-white">

                <h4 class="mb-0">
                    Certificate Template Master
                </h4>

            </div>

        </div>


        <!-- ===================================================== -->
        <!-- SEARCH -->
        <!-- ===================================================== -->

        <div class="card">

            <div class="card-header bg-dark text-white">

                Search Templates

            </div>

            <div class="card-body">

                <div class="row">

                    <!-- Template Name -->

                    <div class="col-md-4">

                        <label>
                            Template Name
                        </label>

                        <asp:TextBox
                            ID="txtSearchTemplate"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Status -->

                    <div class="col-md-3">

                        <label>
                            Status
                        </label>

                        <asp:DropDownList
                            ID="ddlSearchStatus"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem
                                Value="">
                                All
                            </asp:ListItem>

                            <asp:ListItem
                                Value="1">
                                Active
                            </asp:ListItem>

                            <asp:ListItem
                                Value="0">
                                Inactive
                            </asp:ListItem>

                        </asp:DropDownList>

                    </div>


                    <!-- Buttons -->

                    <div class="col-md-5 pt-4">

                        <asp:Button
                            ID="btnSearch"
                            runat="server"
                            Text="Search"
                            CssClass="btn btn-primary"
                            OnClick="btnSearch_Click" />

                        <asp:Button
                            ID="btnResetSearch"
                            runat="server"
                            Text="Reset"
                            CssClass="btn btn-secondary"
                            OnClick="btnResetSearch_Click" />

                    </div>

                </div>

            </div>

        </div>


        <!-- ===================================================== -->
        <!-- TEMPLATE ENTRY -->
        <!-- ===================================================== -->

        <div class="card">

            <div class="card-header bg-success text-white">

               Create Template Details

            </div>


            <div class="card-body">


                <!-- Hidden ID -->

                <asp:HiddenField
                    ID="hfID"
                    runat="server" />


                <!-- ================================================= -->
                <!-- BASIC DETAILS -->
                <!-- ================================================= -->

                <div class="section-title">

                    Basic Details

                </div>


                <div class="row">


                    <!-- Template Name -->

                    <div class="col-md-6 mb-3">

                        <label>
                            Template Name

                            <span class="required">
                                *
                            </span>

                        </label>

                        <asp:TextBox
                            ID="txtTemplateName"
                            runat="server"
                            MaxLength="200"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Description -->

                    <div class="col-md-6 mb-3">

                        <label>
                            Description
                        </label>

                        <asp:TextBox
                            ID="txtDescription"
                            runat="server"
                            MaxLength="500"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                </div>


                <!-- ================================================= -->
                <!-- PAGE SETTINGS -->
                <!-- ================================================= -->

                <div class="section-title mt-3">

                    Page Settings

                </div>


                <div class="row">


                    <!-- Display Order -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Display Order
                        </label>

                        <asp:TextBox
                            ID="txtDisplayOrder"
                            runat="server"
                            Text="1"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Orientation -->

                    <div class="col-md-3 mb-3">

                        <label>

                            Orientation

                            <span class="required">
                                *
                            </span>

                        </label>

                        <asp:DropDownList
                            ID="ddlOrientation"
                            runat="server"
                            CssClass="form-control"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlOrientation_SelectedIndexChanged">

                            <asp:ListItem
                                Value="">
                                Select
                            </asp:ListItem>

                            <asp:ListItem
                                Value="Landscape">
                                Landscape
                            </asp:ListItem>

                            <asp:ListItem
                                Value="Portrait">
                                Portrait
                            </asp:ListItem>

                        </asp:DropDownList>

                    </div>


                    <!-- Paper Size -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Paper Size
                        </label>

                        <asp:DropDownList
                            ID="ddlPaperSize"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem
                                Value="">
                                --Select--
                            </asp:ListItem>

                            <asp:ListItem
                                Value="A4">
                                A4
                            </asp:ListItem>

                            <asp:ListItem
                                Value="Letter">
                                Letter
                            </asp:ListItem>

                        </asp:DropDownList>

                    </div>


                    <!-- Status -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Status
                        </label>

                        <div class="form-control">

                            <asp:CheckBox
                                ID="chkActive"
                                runat="server"
                                Checked="true"
                                Text=" Active" />

                        </div>

                    </div>


                </div>


                <!-- Page Dimensions -->

                <div class="row">


                    <div class="col-md-3 mb-3">

                        <label>
                            Page Width
                        </label>

                        <asp:TextBox
                            ID="txtPageWidth"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                        <div class="position-help">
                            Coordinate / page width
                        </div>

                    </div>


                    <div class="col-md-3 mb-3">

                        <label>
                            Page Height
                        </label>

                        <asp:TextBox
                            ID="txtPageHeight"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                        <div class="position-help">
                            Coordinate / page height
                        </div>

                    </div>


                </div>


                <!-- ================================================= -->
                <!-- HEADER / FOOTER -->
                <!-- ================================================= -->

                <div class="section-title mt-3">

                    Header & Footer

                </div>


                <div class="row">


                    <!-- Header -->

                    <div class="col-md-6 mb-3">

                        <label>
                            Header Text
                        </label>

                        <asp:TextBox
                            ID="txtHeader"
                            runat="server"
                            Rows="4"
                            TextMode="MultiLine"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Footer -->

                    <div class="col-md-6 mb-3">

                        <label>
                            Footer Text
                        </label>

                        <asp:TextBox
                            ID="txtFooter"
                            runat="server"
                            Rows="4"
                            TextMode="MultiLine"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                </div>


                <!-- ================================================= -->
                <!-- FONT SETTINGS -->
                <!-- ================================================= -->

                <div class="section-title mt-3">

                    Font Settings

                </div>


                <div class="row">


                    <!-- Course Title -->

                    <div class="col-md-2 mb-3">

                        <label>
                            Course Title Font
                        </label>

                        <asp:TextBox
                            ID="txtCourseTitleFont"
                            runat="server"
                            Text="26"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Header -->

                    <div class="col-md-2 mb-3">

                        <label>
                            Header Font
                        </label>

                        <asp:TextBox
                            ID="txtHeaderFont"
                            runat="server"
                            Text="18"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Footer -->

                    <div class="col-md-2 mb-3">

                        <label>
                            Footer Font
                        </label>

                        <asp:TextBox
                            ID="txtFooterFont"
                            runat="server"
                            Text="12"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Body -->

                    <div class="col-md-2 mb-3">

                        <label>
                            Body Font
                        </label>

                        <asp:TextBox
                            ID="txtBodyFont"
                            runat="server"
                            Text="16"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Name -->

                    <div class="col-md-2 mb-3">

                        <label>
                            Name Font
                        </label>

                        <asp:TextBox
                            ID="txtNameFont"
                            runat="server"
                            Text="28"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                </div>


                <!-- ================================================= -->
                <!-- POSITION SETTINGS -->
                <!-- ================================================= -->

                <div class="section-title mt-3">

                    Certificate Position Settings

                </div>


                <div class="alert alert-secondary">

                    <strong>
                        Note:
                    </strong>

                    Position values are used by the PDF generator.
                    Increase / decrease X and Y values to move
                    certificate elements.

                </div>


                <!-- Row 1 -->

                <div class="row">


                    <!-- Logo X -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Logo X
                        </label>

                        <asp:TextBox
                            ID="txtLogoX"
                            runat="server"
                            Text="50"
                            CssClass="form-control">
                        </asp:TextBox>

                        <div class="position-help">
                            Left / Right position
                        </div>

                    </div>


                    <!-- Logo Y -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Logo Y
                        </label>

                        <asp:TextBox
                            ID="txtLogoY"
                            runat="server"
                            Text="700"
                            CssClass="form-control">
                        </asp:TextBox>

                        <div class="position-help">
                            Up / Down position
                        </div>

                    </div>


                    <!-- Header Y -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Header Y
                        </label>

                        <asp:TextBox
                            ID="txtHeaderY"
                            runat="server"
                            Text="730"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Title Y -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Title Y
                        </label>

                        <asp:TextBox
                            ID="txtTitleY"
                            runat="server"
                            Text="650"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                </div>


                <!-- Row 2 -->

                <div class="row">


                    <!-- Body Y -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Body Y
                        </label>

                        <asp:TextBox
                            ID="txtBodyY"
                            runat="server"
                            Text="520"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Left Signature X -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Left Signature X
                        </label>

                        <asp:TextBox
                            ID="txtLeftSignatureX"
                            runat="server"
                            Text="180"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Right Signature X -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Right Signature X
                        </label>

                        <asp:TextBox
                            ID="txtRightSignatureX"
                            runat="server"
                            Text="650"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                    <!-- Signature Y -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Signature Y
                        </label>

                        <asp:TextBox
                            ID="txtSignatureY"
                            runat="server"
                            Text="150"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                </div>


                <!-- Row 3 -->

                <div class="row">


                    <!-- Footer Y -->

                    <div class="col-md-3 mb-3">

                        <label>
                            Footer Y
                        </label>

                        <asp:TextBox
                            ID="txtFooterY"
                            runat="server"
                            Text="50"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>


                </div>


                <!-- ================================================= -->
                <!-- IMAGES -->
                <!-- ================================================= -->

                <div class="section-title mt-3">

                    Certificate Images

                </div>


                <div class="row">


                    <!-- Background -->

                    <div class="col-md-6 mb-3">

                        <label>
                            Background Image
                        </label>

                        <asp:FileUpload
                            ID="fuBackground"
                            runat="server"
                            CssClass="form-control" />

                        <div class="mt-2">

                            <asp:Image
                                ID="imgBackground"
                                runat="server"
                                CssClass="preview-img" />

                        </div>

                        <div class="position-help">

                            JPG / JPEG / PNG &nbsp; | &nbsp;
                            Maximum 2 MB

                        </div>

                    </div>


                    <!-- Logo -->

                    <div class="col-md-6 mb-3">

                        <label>
                            Logo Image
                        </label>

                        <asp:FileUpload
                            ID="fuLogo"
                            runat="server"
                            CssClass="form-control" />

                        <div class="mt-2">

                            <asp:Image
                                ID="imgLogo"
                                runat="server"
                                CssClass="preview-img" />

                        </div>

                        <div class="position-help">

                            JPG / JPEG / PNG &nbsp; | &nbsp;
                            Maximum 2 MB

                        </div>

                    </div>


                </div>


                <!-- ================================================= -->
                <!-- ACTION BUTTONS -->
                <!-- ================================================= -->

                <div class="text-center mt-4">


                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Save"
                        CssClass="btn btn-success px-4"
                        OnClick="btnSave_Click" />


                    <asp:Button
                        ID="btnReset"
                        runat="server"
                        Text="Reset"
                        CssClass="btn btn-secondary px-4"
                        OnClick="btnReset_Click" />


                    <asp:Button
                        ID="btnPreview"
                        runat="server"
                        Text="Preview"
                        CssClass="btn btn-info px-4"
                        OnClick="btnPreview_Click" />


                </div>


                <!-- Message -->

                <div class="mt-3 text-center">

                    <asp:Label
                        ID="lblMessage"
                        runat="server">
                    </asp:Label>

                </div>


            </div>

        </div>


        <!-- ===================================================== -->
        <!-- TEMPLATE GRID -->
        <!-- ===================================================== -->

        <div class="card">

            <div class="card-header bg-info text-white">

                Template List

            </div>


            <div class="card-body">


                <div style="overflow-x:auto;">

                    <asp:GridView
                        ID="gvTemplate"
                        runat="server"
                        AutoGenerateColumns="False"
                        CssClass="table table-bordered table-hover"
                        DataKeyNames="TemplateID"
                        OnRowCommand="gvTemplate_RowCommand">

                        <Columns>


                           

                            <asp:TemplateField
                                HeaderText="Action">

                                <ItemTemplate>


                                    <asp:LinkButton
                                        ID="lnkEdit"
                                        runat="server"
                                        CommandName="EditRow"
                                        CommandArgument='<%# Eval("TemplateID") %>'
                                        Text="Edit"
                                        CssClass="btn btn-sm btn-primary">
                                    </asp:LinkButton>


                                    &nbsp;


                                    <asp:LinkButton
                                        ID="lnkPreview"
                                        runat="server"
                                        CommandName="PreviewTemplate"
                                        CommandArgument='<%# Eval("TemplateID") %>'
                                        Text="Preview"
                                        CssClass="btn btn-sm btn-info">
                                    </asp:LinkButton>


                                    &nbsp;


                                    <asp:LinkButton
                                        ID="lnkStatus"
                                        runat="server"
                                        CommandName="ChangeStatus"
                                        CommandArgument='<%# Eval("TemplateID") %>'
                                        Text='<%# Convert.ToBoolean(Eval("Active")) ? "Deactivate" : "Activate" %>'
                                        CssClass="btn btn-sm btn-warning">
                                    </asp:LinkButton>


                                </ItemTemplate>

                            </asp:TemplateField>


                           

                            <asp:BoundField
                                DataField="TemplateName"
                                HeaderText="Template Name" />


                          

                            <asp:BoundField
                                DataField="Orientation"
                                HeaderText="Orientation" />


                           

                            <asp:BoundField
                                DataField="PaperSize"
                                HeaderText="Paper Size" />


                           

                            <asp:BoundField
                                DataField="PageWidth"
                                HeaderText="Width" />


                        

                            <asp:BoundField
                                DataField="PageHeight"
                                HeaderText="Height" />


                            

                            <asp:BoundField
                                DataField="DisplayOrder"
                                HeaderText="Order" />


                            

                            <asp:CheckBoxField
                                DataField="Active"
                                HeaderText="Active" />


                            

                            <asp:BoundField
                                DataField="CreatedOn"
                                HeaderText="Created On"
                                DataFormatString="{0:dd-MM-yyyy}" />


                        </Columns>

                    </asp:GridView>

                </div>


            </div>

        </div>


    </div>

</asp:Content>
