<%@ page title="My Trainings" language="C#" masterpagefile="~/TraineeMaster.Master" autoeventwireup="true" codebehind="MyTrainings.aspx.cs" inherits="Training.Trainee.MyTrainings" maintainscrollpositiononpostback="true" %>

<%@ register src="~/Trainee/TraineeTrainingSummary.ascx" tagprefix="uc1" tagname="TraineeTrainingSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .search-card,
        .grid-card {
            border: 0;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,.08);
        }

        .gridview th {
            background: #198754;
            color: white;
            text-align: center;
            vertical-align: middle;
            white-space: nowrap;
        }

        .gridview td,
        .table td,
        .table th {
            vertical-align: middle !important;
        }

        .badge-status {
            font-size: 13px;
            padding: 6px 10px;
            min-width: 110px;
            display: inline-block;
            text-align: center;
            color: #000 !important;
        }

        .page-title {
            font-size: 24px;
            font-weight: 600;
        }

        .btn-group .btn {
            margin-right: 4px;
        }

        .btn-group {
            display: flex;
            flex-wrap: wrap;
            gap: 4px;
        }

        .progress {
            min-width: 120px;
            height: 20px;
        }

        .btn[disabled],
        .btn.disabled {
            cursor: not-allowed;
            opacity: .55;
        }

        :root {
            --tm-primary: #2563eb;
            --tm-primary-dark: #1d4ed8;
            --tm-secondary: #64748b;
            --tm-success: #16a34a;
            --tm-warning: #f59e0b;
            --tm-info: #0891b2;
            --tm-danger: #dc2626;
            --tm-purple: #7c3aed;
            --tm-dark: #1e293b;
            --tm-muted: #64748b;
            --tm-light: #f8fafc;
            --tm-border: #e2e8f0;
            --tm-white: #ffffff;
            --tm-radius: 14px;
        }

        .modern-card {
            border: 1px solid rgba(226,232,240,.9);
            border-radius: var(--tm-radius);
            background: var(--tm-white);
            box-shadow: 0 5px 20px rgba(15,23,42,.055);
            overflow: hidden;
            transition: all .25s ease;
        }

            .modern-card:hover {
                box-shadow: 0 8px 28px rgba(15,23,42,.08);
            }

        .modern-card-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 16px 20px;
            border-bottom: 1px solid var(--tm-border);
            background: linear-gradient(to right, #ffffff, #f8fafc);
        }

        .card-heading {
            display: flex;
            align-items: center;
            margin: 0;
            color: var(--tm-dark);
            font-size: 17px;
            font-weight: 700;
        }

        .card-heading-icon {
            width: 36px;
            height: 36px;
            margin-right: 10px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            border-radius: 10px;
            background: #eff6ff;
            color: var(--tm-primary);
        }

        .details-card {
            border-top: 3px solid var(--tm-primary);
        }

        .details-body {
            padding: 20px;
        }

        .profile-status {
            font-size: 13px;
            font-weight: 600;
        }

        .info-item {
            height: 100%;
            padding: 12px 14px;
            margin-bottom: 12px;
            border: 1px solid #e8edf4;
            border-radius: 10px;
            background: #fbfdff;
            transition: all .2s ease;
        }

            .info-item:hover {
                border-color: #bfdbfe;
                background: #f8fbff;
                transform: translateY(-1px);
            }

        .info-label {
            display: block;
            margin-bottom: 4px;
            color: #64748b;
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: .5px;
        }

        .info-value {
            display: block;
            min-height: 20px;
            color: #1e293b;
            font-size: 14px;
            font-weight: 600;
            word-break: break-word;
        }

        .profile-actions {
            padding-top: 8px;
            border-top: 1px dashed #e2e8f0;
            margin-top: 4px;
        }

        .myCustomButton {
            color: #fff !important;
            border: none !important;
            background: linear-gradient(135deg, #7c3aed, #6d28d9) !important;
            border-radius: 8px !important;
            box-shadow: 0 3px 8px rgba(124,58,237,.22);
            transition: all .2s ease;
        }

            .myCustomButton:hover {
                transform: translateY(-1px);
                box-shadow: 0 5px 12px rgba(124,58,237,.28);
            }

        .modern-btn {
            border-radius: 8px !important;
            font-weight: 600 !important;
            transition: all .2s ease !important;
        }

            .modern-btn:hover {
                transform: translateY(-1px);
            }

        .search-card {
            margin-top: 20px;
        }

        .search-body {
            padding: 20px;
        }

        .search-label {
            display: block;
            margin-bottom: 7px;
            color: #475569;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: .35px;
        }

        .search-control {
            height: 40px !important;
            border: 1px solid #dbe2ea !important;
            border-radius: 9px !important;
            background: #fff !important;
            box-shadow: none !important;
            transition: all .2s ease;
        }

            .search-control:focus {
                border-color: #60a5fa !important;
                box-shadow: 0 0 0 3px rgba(37,99,235,.10) !important;
            }

        .search-buttons {
            padding-top: 25px;
        }

            .search-buttons .btn {
                min-width: 82px;
                margin-right: 5px;
                border-radius: 8px;
                font-weight: 600;
            }

        .grid-card {
            margin-top: 20px;
        }

        .grid-action {
            border: none !important;
            border-radius: 7px !important;
            font-size: 11px !important;
            font-weight: 600 !important;
            padding: 6px 9px !important;
            box-shadow: none !important;
            transition: all .18s ease !important;
        }

            .grid-action:hover {
                transform: translateY(-1px);
                box-shadow: 0 3px 7px rgba(15,23,42,.12) !important;
            }

        .confirmation-badge {
            display: inline-block;
            padding: 6px 10px;
            border-radius: 20px;
            background: #dcfce7;
            color: #166534;
            font-size: 11px;
            font-weight: 700;
            white-space: nowrap;
        }

        #correctionModal .modal-dialog {
            max-width: 90%;
        }

        #correctionModal .modal-content {
            border: none !important;
            border-radius: 16px !important;
            overflow: hidden;
            box-shadow: 0 20px 60px rgba(15,23,42,.25);
        }

        #correctionModal .modal-header {
            padding: 17px 20px !important;
            border: none !important;
            color: #fff;
            background: linear-gradient(135deg,#2563eb,#4f46e5);
        }

        #correctionModal .modal-title {
            font-weight: 700;
        }

        #correctionModal .modal-header .close {
            color: #fff;
            opacity: .9;
            text-shadow: none;
        }

        #correctionModal .modal-body {
            max-height: 90vh;
            overflow-y: auto;
            padding: 22px !important;
        }

            #correctionModal .modal-body label {
                color: #475569;
                font-size: 12px;
                font-weight: 700;
            }

            #correctionModal .modal-body .form-control {
                border-radius: 9px;
                border: 1px solid #dbe2ea;
            }

                #correctionModal .modal-body .form-control:focus {
                    border-color: #60a5fa;
                    box-shadow: 0 0 0 3px rgba(37,99,235,.10);
                }

        #correctionModal .modal-footer {
            border-top: 1px solid #eef2f7 !important;
            background: #f8fafc;
        }

        .correction-section-title {
            display: flex;
            align-items: center;
            margin: 10px 0 6px;
            color: var(--tm-dark);
            font-size: 13px;
            font-weight: 700;
        }

            .correction-section-title .section-icon {
                width: 24px;
                height: 24px;
                margin-right: 8px;
                display: inline-flex;
                align-items: center;
                justify-content: center;
                border-radius: 7px;
                background: #eff6ff;
                color: var(--tm-primary);
                font-size: 11px;
            }

        .correction-section-hint {
            font-size: 12px;
            font-weight: 400;
            color: #94a3b8;
            margin-left: 8px;
        }

        .correction-field {
            padding: 6px 10px;
            margin-bottom: 8px;
            border: 1px solid #e8edf4;
            border-left: 3px solid var(--tm-primary);
            border-radius: 8px;
            background: #fbfdff;
            transition: all .2s ease;
        }

            .correction-field:hover {
                border-color: #bfdbfe;
                background: #f8fbff;
            }

            .correction-field label {
                display: block;
                margin-bottom: 2px;
                color: #64748b;
                font-size: 10px;
                font-weight: 700;
                text-transform: uppercase;
                letter-spacing: .5px;
            }

            .correction-field .form-control {
                border: 1px solid #dbe2ea;
                border-radius: 6px;
                padding: 3px 10px;
                height: calc(1.5em + 8px);
                font-size: 13px;
            }

                .correction-field .form-control:focus {
                    border-color: var(--tm-primary);
                    box-shadow: 0 0 0 3px rgba(37,99,235,.12);
                }

        .correction-current-posting {
            padding: 6px 12px;
            margin-bottom: 8px;
            border: 1px dashed #cbd5e1;
            border-radius: 8px;
            background: #f8fafc;
        }

            .correction-current-posting .label {
                font-size: 11px;
                font-weight: 700;
                text-transform: uppercase;
                letter-spacing: .5px;
                color: #64748b;
                display: block;
                margin-bottom: 4px;
            }

        .posting-type-radio td {
            padding-right: 22px;
        }

        .posting-type-radio input[type="radio"] {
            margin-right: 6px;
            vertical-align: middle;
        }

        .posting-type-radio label {
            font-weight: 400;
            text-transform: none;
            letter-spacing: normal;
            display: inline;
            color: #334155;
            font-size: 14px;
            vertical-align: middle;
        }
    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <div class="row mb-3">
            <div class="col-md-12">
                <h3 class="page-title">My Trainings</h3>
            </div>
        </div>


        <!-- =========================================================
             MY DETAILS
             ========================================================= -->

        <div class="modern-card details-card mb-3">

            <div class="modern-card-header">

                <h5 class="card-heading">

                    <span class="card-heading-icon">
                        <i class="fa fa-user"></i>
                    </span>

                    My Details

                </h5>

                <asp:Label
                    ID="lblConfirmStatus"
                    runat="server"
                    CssClass="profile-status" />

            </div>


            <div class="details-body">

                <div class="row">

                    <div class="col-md-3 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-id-card-o"></i>
                                Employee ID
                            </span>

                            <asp:Label
                                ID="lblEmpID"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>


                    <div class="col-md-3 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-user"></i>
                                Name
                            </span>

                            <asp:Label
                                ID="lblName"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>


                    <div class="col-md-2 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-venus-mars"></i>
                                Gender
                            </span>

                            <asp:Label
                                ID="lblGender"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>


                    <div class="col-md-2 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-briefcase"></i>
                                Designation
                            </span>

                            <asp:Label
                                ID="lblDesignation"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>


                    <div class="col-md-2 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-building-o"></i>
                                Company
                            </span>

                            <asp:Label
                                ID="lblCompany"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>

                </div>


                <div class="row">

                    <div class="col-md-3 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-map-marker"></i>
                                Current Posting Place
                            </span>

                            <asp:Label
                                ID="lblPostingPlace"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>


                    <div class="col-md-3 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-mobile"></i>
                                Mobile
                            </span>

                            <asp:Label
                                ID="lblMobile"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>


                    <div class="col-md-4 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-envelope-o"></i>
                                Email
                            </span>

                            <asp:Label
                                ID="lblEmail"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>

                </div>


                <div class="row">

                    <div class="col-md-2 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-sitemap"></i>
                                Zone
                            </span>

                            <asp:Label
                                ID="lblZone"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>


                    <div class="col-md-2 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-sitemap"></i>
                                Circle
                            </span>

                            <asp:Label
                                ID="lblCircle"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>


                    <div class="col-md-2 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-sitemap"></i>
                                Division
                            </span>

                            <asp:Label
                                ID="lblDivision"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>


                    <div class="col-md-3 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-sitemap"></i>
                                Sub-Division
                            </span>

                            <asp:Label
                                ID="lblSubDivision"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>


                    <div class="col-md-3 mb-2">
                        <div class="info-item">

                            <span class="info-label">
                                <i class="fa fa-sitemap"></i>
                                Section
                            </span>

                            <asp:Label
                                ID="lblSection"
                                runat="server"
                                CssClass="info-value" />

                        </div>
                    </div>

                </div>


                <div class="profile-actions">

                    <asp:Button
                        ID="btnConfirmProfile"
                        runat="server"
                        Text="Confirm My Details Are Correct"
                        CssClass="btn myCustomButton btn-sm modern-btn"
                        OnClick="btnConfirmProfile_Click" />


                    <button
                        type="button"
                        class="btn btn-info btn-sm modern-btn"
                        data-toggle="modal"
                        data-target="#correctionModal">

                        <i class="fa fa-pencil"></i>
                        Request a Correction

                    </button>

                </div>


                <asp:Label
                    ID="lblConfirmMessage"
                    runat="server" />

            </div>

        </div>


        <!-- =========================================================
             CORRECTION REQUEST MODAL
             ========================================================= -->

        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div
            class="modal fade"
            id="correctionModal"
            tabindex="-1"
            role="dialog"
            aria-labelledby="correctionModalLabel"
            aria-hidden="true">

            <div
                class="modal-dialog modal-lg modal-dialog-centered"
                role="document">

                <div class="modal-content">


                    <div class="modal-header">

                        <h5
                            class="modal-title"
                            id="correctionModalLabel">

                            <i class="fa fa-pencil-square-o"></i>
                            &nbsp;Request a Correction

                        </h5>


                        <button
                            type="button"
                            class="close"
                            data-dismiss="modal"
                            aria-label="Close">

                            <span aria-hidden="true">&times;
                            </span>

                        </button>

                    </div>


                    <asp:UpdatePanel
                        ID="UpdatePanelCorrection"
                        runat="server"
                        UpdateMode="Conditional">

                        <ContentTemplate>


                            <div class="modal-body">


                                <!-- PERSONAL DETAILS -->

                                <div class="correction-section-title">

                                    <span class="section-icon">
                                        <i class="fa fa-id-card"></i>
                                    </span>

                                    Personal &amp; Contact Details

                                    <span class="correction-section-hint">Edit only what needs correcting
                                    </span>

                                </div>


                                <div class="row">


                                    <div class="col-md-6">

                                        <div class="correction-field">

                                            <label>
                                                Gender
                                            </label>

                                            <asp:DropDownList
                                                ID="ddlRequestedGender"
                                                runat="server"
                                                CssClass="form-control">
                                            </asp:DropDownList>

                                        </div>

                                    </div>


                                    <div class="col-md-6">

                                        <div class="correction-field">

                                            <label>
                                                Designation
                                            </label>

                                            <asp:DropDownList
                                                ID="ddlRequestedDesignation"
                                                runat="server"
                                                CssClass="form-control">
                                            </asp:DropDownList>

                                        </div>

                                    </div>


                                    <div class="col-md-6">

                                        <div class="correction-field">

                                            <label>
                                                Company
                                            </label>

                                            <asp:TextBox
                                                ID="txtCompany"
                                                runat="server"
                                                CssClass="form-control" />

                                        </div>

                                    </div>


                                    <div class="col-md-6">

                                        <div class="correction-field">

                                            <label>
                                                Mobile
                                            </label>

                                            <asp:TextBox
                                                ID="txtMobile"
                                                runat="server"
                                                CssClass="form-control" />

                                        </div>

                                    </div>


                                    <div class="col-md-6">

                                        <div class="correction-field">

                                            <label>
                                                Email
                                            </label>

                                            <asp:TextBox
                                                ID="txtEmail"
                                                runat="server"
                                                CssClass="form-control" />

                                        </div>

                                    </div>

                                </div>


                                <!-- POSTING DETAILS -->

                                <div class="correction-section-title">

                                    <span class="section-icon">
                                        <i class="fa fa-map-marker"></i>
                                    </span>

                                    Posting Details

                                    <span class="correction-section-hint">Optional - leave Company blank if not correcting your posting
                                    </span>

                                </div>


                                <div class="correction-current-posting">

                                    <span class="label">Current Posting on Record
                                    </span>

                                    <asp:Label
                                        ID="lblCurrentPosting"
                                        runat="server"
                                        CssClass="d-block font-weight-bold" />

                                </div>


                                <div class="row">


                                    <div class="col-md-6">

                                        <div class="correction-field">

                                            <label>
                                                Company
                                            </label>

                                            <asp:DropDownList
                                                ID="ddlPostingCompany"
                                                runat="server"
                                                CssClass="form-control"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="ddlPostingCompany_SelectedIndexChanged">
                                            </asp:DropDownList>

                                        </div>

                                    </div>


                                    <div
                                        class="col-md-6"
                                        id="divPostingTypeChoice"
                                        runat="server"
                                        visible="false">

                                        <div class="correction-field">

                                            <label>
                                                Posting Place
                                            </label>

                                            <asp:RadioButtonList
                                                ID="rblPostingType"
                                                runat="server"
                                                RepeatDirection="Horizontal"
                                                CssClass="posting-type-radio"
                                                AutoPostBack="true"
                                                OnSelectedIndexChanged="rblPostingType_SelectedIndexChanged">

                                                <asp:ListItem
                                                    Text="HQ"
                                                    Value="HQ"
                                                    Selected="True" />

                                                <asp:ListItem
                                                    Text="Field Office"
                                                    Value="Field" />

                                            </asp:RadioButtonList>

                                        </div>

                                    </div>


                                    <div
                                        class="col-md-6"
                                        id="dvHQ"
                                        runat="server"
                                        visible="false">

                                        <div class="correction-field">

                                            <label>
                                                Dept. / Office / Cell
                                            </label>

                                            <asp:DropDownList
                                                ID="ddlDepartment"
                                                runat="server"
                                                CssClass="form-control">
                                            </asp:DropDownList>

                                        </div>

                                    </div>

                                </div>


                                <!-- FIELD OFFICE HIERARCHY -->

                                <div
                                    id="dvField"
                                    runat="server"
                                    visible="false">

                                    <div class="row">


                                        <div class="col-md-6">

                                            <div class="correction-field">

                                                <label>
                                                    AreaBoard / Zone
                                                </label>

                                                <asp:DropDownList
                                                    ID="ddlZone"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlZone_SelectedIndexChanged">
                                                </asp:DropDownList>

                                            </div>

                                        </div>


                                        <div class="col-md-6">

                                            <div class="correction-field">

                                                <label>
                                                    Circle Name
                                                </label>

                                                <asp:DropDownList
                                                    ID="ddlCircle"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlCircle_SelectedIndexChanged">
                                                </asp:DropDownList>

                                            </div>

                                        </div>


                                        <div class="col-md-6">

                                            <div class="correction-field">

                                                <label>
                                                    Division Name
                                                </label>

                                                <asp:DropDownList
                                                    ID="ddlDivision"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged">
                                                </asp:DropDownList>

                                            </div>

                                        </div>


                                        <div class="col-md-6">

                                            <div class="correction-field">

                                                <label>
                                                    Sub-Division
                                                </label>

                                                <asp:DropDownList
                                                    ID="ddlSubDivision"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddlSubDivision_SelectedIndexChanged">
                                                </asp:DropDownList>

                                            </div>

                                        </div>


                                        <div class="col-md-6">

                                            <div class="correction-field">

                                                <label>
                                                    Section Name
                                                </label>

                                                <asp:DropDownList
                                                    ID="ddlSection"
                                                    runat="server"
                                                    CssClass="form-control">
                                                </asp:DropDownList>

                                            </div>

                                        </div>

                                    </div>

                                </div>


                                <!-- REMARKS -->

                                <div class="correction-field">

                                    <label>
                                        Remarks (optional)
                                    </label>

                                    <asp:TextBox
                                        ID="txtRemarks"
                                        runat="server"
                                        CssClass="form-control"
                                        TextMode="MultiLine"
                                        Rows="2" />

                                </div>


                                <asp:Label
                                    ID="lblCorrectionMessage"
                                    runat="server"
                                    CssClass="d-block mb-2" />


                            </div>


                            <div class="modal-footer">

                                <button
                                    type="button"
                                    class="btn btn-secondary modern-btn"
                                    data-dismiss="modal">
                                    Cancel

                                </button>
                                <asp:Button
                                    ID="btnSubmitCorrection"
                                    runat="server"
                                    Text="Submit Request"
                                    CssClass="btn btn-warning modern-btn"
                                    CausesValidation="false"
                                    OnClick="btnSubmitCorrection_Click" />



                            </div>


                        </ContentTemplate>

                    </asp:UpdatePanel>

                </div>

            </div>

        </div>


        <!-- =========================================================
             CORRECTION MODAL SCRIPT
             ========================================================= -->

        <script type="text/javascript">

            window.addEventListener('load', function () {

                $('#correctionModal').on('show.bs.modal', function () {

                    $('#<%= lblCorrectionMessage.ClientID %>').text('');

                });

            });

        </script>


        <!-- =========================================================
             TRAINING SEARCH
             ========================================================= -->

        <div class="card search-card mb-3">

            <div class="card-body">

                          <div class="row">


                              <div class="col-md-3 mb-2">

                                  <label>
                                      Training ID
                        </label>

                                  <asp:TextBox
                            ID="txtTrainingID"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Training ID">
                        </asp:TextBox>

                    </div>


                    <div class="col-md-3 mb-2">

                        <label>
                            Course
                        </label>

                        <asp:DropDownList
                            ID="ddlCourse"
                            runat="server"
                            CssClass="form-control">
                        </asp:DropDownList>

                    </div>


                    <div class="col-md-3 mb-2">

                        <label>
                            Status
                        </label>

                        <asp:DropDownList
                            ID="ddlStatus"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem
                                Text="All"
                                Value="">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Pending"
                                Value="P">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="In Progress"
                                Value="I">
                            </asp:ListItem>

                            <asp:ListItem
                                Text="Completed"
                                Value="C">
                            </asp:ListItem>

                        </asp:DropDownList>

                    </div>


                    <div class="col-md-3">

                        <label>
                            &nbsp;
                        </label>

                        <div>

                            <asp:Button
                                ID="btnSearch"
                                runat="server"
                                Text="Search"
                                CssClass="btn btn-primary"
                                OnClick="btnSearch_Click" />

                            <asp:Button
                                ID="btnReset"
                                runat="server"
                                Text="Reset"
                                CssClass="btn btn-secondary"
                                OnClick="btnReset_Click" />

                        </div>

                    </div>

                </div>

            </div>

        </div>


        <!-- =========================================================
             TRAINING GRID
             ========================================================= -->

        <div class="card grid-card">

            <div class="card-body table-responsive">

                <asp:GridView
                    ID="gvTraining"
                    runat="server"
                    CssClass="table table-bordered table-hover gridview"
                    AutoGenerateColumns="False"
                    AllowPaging="True"
                    AllowSorting="True"
                    PageSize="10"
                    DataKeyNames="TrainingID"
                    EmptyDataText="No Training Assigned."
                    OnPageIndexChanging="gvTraining_PageIndexChanging"
                    OnSorting="gvTraining_Sorting"
                    OnRowCommand="gvTraining_RowCommand"
                    OnRowDataBound="gvTraining_RowDataBound">

                    <Columns>


                        <asp:BoundField
                            HeaderText="Training ID"
                            DataField="TrainingID" />


                        <asp:BoundField
                            HeaderText="Course"
                            DataField="CourseName" />


                        <asp:BoundField
                            HeaderText="Training Type"
                            DataField="TrainingType" />


                        <asp:BoundField
                            HeaderText="Organizer"
                            DataField="TrainingOrganizer" />


                        <asp:BoundField
                            HeaderText="Batch"
                            DataField="Batch" />


                        <asp:BoundField
                            HeaderText="From"
                            DataField="DateFrom"
                            DataFormatString="{0:dd-MMM-yyyy}" />


                        <asp:BoundField
                            HeaderText="To"
                            DataField="DateTo"
                            DataFormatString="{0:dd-MMM-yyyy}" />


                        <asp:TemplateField
                            HeaderText="Status">

                            <ItemTemplate>

                                <asp:Label
                                    ID="lblStatus"
                                    runat="server"
                                    Text='<%# Eval("StatusText") %>'
                                    CssClass='<%# Eval("StatusClass") %>'>
                                </asp:Label>

                            </ItemTemplate>

                        </asp:TemplateField>


                        <asp:TemplateField
                            HeaderText="Action">

                            <ItemTemplate>

                                <div class="btn-group">


                                    <!-- VIEW -->

                                    <asp:LinkButton
                                        ID="lnkView"
                                        runat="server"
                                        CssClass="btn btn-success btn-sm"
                                        CommandName="ViewTraining"
                                        CommandArgument='<%# Eval("TrainingID") %>'>

                                        <i class="fa fa-eye"></i>
                                        View

                                    </asp:LinkButton>


                                    <!-- ATTENDANCE -->

                                    <asp:LinkButton
                                        ID="lnkAttendance"
                                        runat="server"
                                        CssClass="btn btn-primary btn-sm"
                                        CommandName="Attendance"
                                        CommandArgument='<%# Eval("TrainingID") %>'
                                        Visible='<%# Convert.ToBoolean(Eval("AttendanceRequired")) %>'
                                        Enabled="true">

                                        <i class="fa fa-calendar-check-o"></i>
                                        Attendance

                                    </asp:LinkButton>


                                    <!-- FEEDBACK -->

                                    <asp:LinkButton
                                        ID="lnkFeedback"
                                        runat="server"
                                        CssClass="btn btn-warning btn-sm"
                                        CommandName="BatchFeedback"
                                        CommandArgument='<%# Eval("TrainingID") %>'
                                        Visible='<%# Convert.ToBoolean(Eval("FeedbackRequired")) && !Convert.ToBoolean(Eval("FeedbackSkipped")) %>'
                                        Enabled="false">

                                        <i class="fa fa-comments"></i>
                                        Feedback

                                    </asp:LinkButton>


                                    <!-- CERTIFICATE -->

                                    <asp:LinkButton
                                        ID="lnkCertificate"
                                        runat="server"
                                        CssClass="btn btn-info btn-sm"
                                        CommandName="Certificate"
                                        CommandArgument='<%# Eval("TrainingID") %>'
                                        Visible='<%# Convert.ToBoolean(Eval("CertificateRequired")) && !Convert.ToBoolean(Eval("CertificateSkipped")) %>'
                                        Enabled="false">

                                        <i class="fa fa-certificate"></i>
                                        Certificate

                                    </asp:LinkButton>


                                    <!-- CONFIRM PARTICIPATION -->

                                    <asp:LinkButton
                                        ID="lnkConfirmAttendance"
                                        runat="server"
                                        CssClass="btn btn-secondary btn-sm grid-action"
                                        CommandName="ConfirmAttendance"
                                        CommandArgument='<%# Eval("TrainingID") %>'
                                        Enabled='<%# Convert.ToBoolean(Eval("CanConfirmAttendance")) %>'
                                        OnClientClick="return confirm('Confirm that you will participate in this training?');">

                                        <i class="fa fa-check"></i>
                                        Confirm Participation

                                    </asp:LinkButton>


                                    <!-- ALREADY CONFIRMED -->

                                    <asp:Label
                                        ID="lblConfirmed"
                                        runat="server"
                                        CssClass="confirmation-badge"
                                        Text="Participation Confirmed"
                                        Visible="false">
                                    </asp:Label>


                                </div>

                            </ItemTemplate>

                        </asp:TemplateField>


                    </Columns>


                    <PagerStyle CssClass="pagination-ys" />

                </asp:GridView>

            </div>

        </div>

    </div>

</asp:Content>
