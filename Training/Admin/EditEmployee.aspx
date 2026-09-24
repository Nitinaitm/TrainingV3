<%@ Page Title="Edit Employee"
    Language="C#"
    AutoEventWireup="true"
    CodeBehind="EditEmployee.aspx.cs"
    Inherits="Training.Admin.EditEmployee" %>

<!DOCTYPE html>

<html>
<head runat="server">

    <meta charset="utf-8" />

    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <title>Edit Employee</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
          rel="stylesheet" />

    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
          rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

    <style type="text/css">

        html,
        body {
            width: 100%;
            height: 100%;
            margin: 0;
            padding: 0;
            background: #f5f7fb;
            overflow: auto;
            font-family: Arial, Helvetica, sans-serif;
        }

        .container-fluid {
            width: 100%;
            padding: 0;
        }

        .edit-card {
            width: 100%;
            max-width: 1400px;
            background: #ffffff;
            border-radius: 12px;
            box-shadow: 0 2px 14px rgba(0,0,0,.10);
            padding: 22px 26px;
            margin: 0 auto;
            min-height: 100vh;
        }

        .edit-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            border-bottom: 1px solid #e5e7eb;
            padding-bottom: 14px;
            margin-bottom: 18px;
        }

        .heading {
            font-size: 24px;
            font-weight: 700;
            color: #198754;
            margin: 0;
        }

        .popup-close {
            width: 38px;
            height: 38px;
            border: 0;
            border-radius: 7px;
            background: #dc3545;
            color: #ffffff;
            font-size: 24px;
            line-height: 36px;
            text-align: center;
            cursor: pointer;
            padding: 0;
        }

        .popup-close:hover {
            background: #bb2d3b;
        }

        .readonly {
            background: #eef3f8 !important;
        }

        .form-label {
            font-weight: 600;
            margin-bottom: 6px;
            color: #334155;
        }

        .form-control,
        .form-select {
            min-height: 40px;
        }

        .select2-container {
            width: 100% !important;
        }

        .select2-selection--single {
            height: 40px !important;
            padding-top: 5px;
            border: 1px solid #ced4da !important;
        }

        .select2-selection__rendered {
            line-height: 28px !important;
        }

        .posting-box {
            border: 1px solid #dee2e6;
            border-radius: 10px;
            padding: 18px;
            margin-top: 8px;
        }

        .action-row {
            display: flex;
            gap: 10px;
            align-items: center;
            flex-wrap: wrap;
            margin-top: 18px;
        }

        .message-row {
            margin-top: 12px;
            min-height: 24px;
        }

        @media(max-width:768px) {

            .edit-card {
                padding: 16px;
                border-radius: 0;
            }

            .heading {
                font-size: 20px;
            }

            .popup-close {
                width: 34px;
                height: 34px;
                font-size: 21px;
            }

            .action-row .btn {
                width: 100%;
            }

        }

    </style>

    <script type="text/javascript">

        function closeEditEmployee() {

            try {

                if (window.parent && window.parent !== window) {

                    window.parent.postMessage(
                        {
                            type: 'closeEmployeeEdit'
                        },
                        '*'
                    );

                    return;
                }

            }
            catch (e) {
            }

            try {

                if (window.opener && !window.opener.closed) {

                    window.opener.location.reload();

                }

            }
            catch (e) {
            }

            try {

                window.close();

            }
            catch (e) {
            }
        }


        function LoadSearchableDropdown() {

            $('[id*=ddlCompany]').select2({
                placeholder: 'Search Company',
                allowClear: true,
                width: '100%'
            });

            $('[id*=ddlDesignation]').select2({
                placeholder: 'Search Designation',
                allowClear: true,
                width: '100%'
            });

            $('[id*=ddlPostingPlace]').select2({
                placeholder: 'Search HRMS Posting Place',
                allowClear: true,
                width: '100%'
            });

            $('[id*=ddlPostingDetailPlace]').select2({
                placeholder: 'Search Posting Place',
                allowClear: true,
                width: '100%'
            });

            $('[id*=ddlPostingDepartment]').select2({
                placeholder: 'Search Department / Office / Cell',
                allowClear: true,
                width: '100%'
            });

            $('[id*=ddlAreaBoardZone]').select2({
                placeholder: 'Search Area Board / Zone',
                allowClear: true,
                width: '100%'
            });

            $('[id*=ddlCircle]').select2({
                placeholder: 'Search Circle',
                allowClear: true,
                width: '100%'
            });

            $('[id*=ddlDivision]').select2({
                placeholder: 'Search Division',
                allowClear: true,
                width: '100%'
            });

            $('[id*=ddlSubdivision]').select2({
                placeholder: 'Search Subdivision',
                allowClear: true,
                width: '100%'
            });

            $('[id*=ddlSection]').select2({
                placeholder: 'Search Section',
                allowClear: true,
                width: '100%'
            });
        }


        $(document).ready(function () {

            LoadSearchableDropdown();

        });

    </script>

</head>

<body>

    <form id="form1" runat="server">

        <div class="container-fluid">

            <div class="edit-card">

                <div class="edit-header">

                    <div class="heading">
                        Edit Employee
                    </div>

                    <button type="button"
                            class="popup-close"
                            title="Close"
                            onclick="closeEditEmployee();">

                        &times;

                    </button>

                </div>


                <asp:HiddenField
                    ID="hfEmpID"
                    runat="server" />


                <div class="row">


                    <div class="col-md-6 mb-3">

                        <label class="form-label">
                            Employee ID
                        </label>

                        <asp:TextBox
                            ID="txtEmpID"
                            runat="server"
                            CssClass="form-control readonly"
                            ReadOnly="true" />

                    </div>


                    <div class="col-md-6 mb-3">

                        <label class="form-label">
                            Employee Name
                        </label>

                        <asp:TextBox
                            ID="txtEmpName"
                            runat="server"
                            CssClass="form-control readonly"
                            ReadOnly="true" />

                    </div>


                    <div class="col-md-6 mb-3">

                        <label class="form-label">
                            Mobile No
                        </label>

                        <asp:TextBox
                            ID="txtMobile"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="10" />

                    </div>


                    <div class="col-md-6 mb-3">

                        <label class="form-label">
                            Email ID
                        </label>

                        <asp:TextBox
                            ID="txtEmail"
                            runat="server"
                            CssClass="form-control" />

                    </div>


                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Company
                        </label>

                        <asp:DropDownList
                            ID="ddlCompany"
                            runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged" />

                    </div>


                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Designation
                        </label>

                        <asp:DropDownList
                            ID="ddlDesignation"
                            runat="server"
                            CssClass="form-select" />

                    </div>


                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Posting Place (HRMS)
                        </label>

                        <asp:DropDownList
                            ID="ddlPostingPlace"
                            runat="server"
                            CssClass="form-select" />

                    </div>


                    <div class="col-12 mt-3">

                        <div class="posting-box">

                            <h5 class="text-primary mb-3">
                                Posting Details
                            </h5>


                            <div class="row">


                                <div class="col-md-4 mb-3"
                                     id="divPostingPlace"
                                     runat="server">

                                    <label class="form-label">
                                        Posting Place
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlPostingDetailPlace"
                                        runat="server"
                                        CssClass="form-select"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlPostingDetailPlace_SelectedIndexChanged" />

                                </div>


                                <div class="col-md-8 mb-3"
                                     id="divPostingDepartment"
                                     runat="server">

                                    <label class="form-label">
                                        Department / Office / Cell
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlPostingDepartment"
                                        runat="server"
                                        CssClass="form-select" />

                                </div>


                                <div class="col-md-4 mb-3"
                                     id="divAreaBoardZone"
                                     runat="server">

                                    <label class="form-label">
                                        Area Board / Zone
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlAreaBoardZone"
                                        runat="server"
                                        CssClass="form-select"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlAreaBoardZone_SelectedIndexChanged" />

                                </div>


                                <div class="col-md-4 mb-3"
                                     id="divCircle"
                                     runat="server">

                                    <label class="form-label">
                                        Circle
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlCircle"
                                        runat="server"
                                        CssClass="form-select"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlCircle_SelectedIndexChanged" />

                                </div>


                                <div class="col-md-4 mb-3"
                                     id="divDivision"
                                     runat="server">

                                    <label class="form-label">
                                        Division
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlDivision"
                                        runat="server"
                                        CssClass="form-select"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged" />

                                </div>


                                <div class="col-md-4 mb-3"
                                     id="divSubdivision"
                                     runat="server">

                                    <label class="form-label">
                                        Subdivision
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlSubdivision"
                                        runat="server"
                                        CssClass="form-select"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlSubdivision_SelectedIndexChanged" />

                                </div>


                                <div class="col-md-4 mb-3"
                                     id="divSection"
                                     runat="server">

                                    <label class="form-label">
                                        Section
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlSection"
                                        runat="server"
                                        CssClass="form-select" />

                                </div>


                            </div>

                        </div>

                    </div>


                    <div class="col-12">

                        <div class="action-row">

                            <asp:Button
                                ID="btnUpdate"
                                runat="server"
                                Text="Update Employee"
                                CssClass="btn btn-success"
                                OnClick="btnUpdate_Click" />

                            <button type="button"
                                    class="btn btn-secondary"
                                    onclick="closeEditEmployee();">

                                Close

                            </button>

                        </div>

                    </div>


                    <div class="col-12 mt-3">

                        <asp:Label
                            ID="lblMessage"
                            runat="server"
                            Font-Bold="true" />

                    </div>


                </div>

            </div>

        </div>

    </form>

</body>

</html>