<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="Training.Admin.Default"  MaintainScrollPositionOnPostback="true" %>

<asp:Content
    ID="Content1" 
    ContentPlaceHolderID="head"
    runat="server">

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" /> 

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

    <style type="text/css">

        * {
            box-sizing: border-box;
        }

        html,
        body {
            width: 100%;
            max-width: 100%;
        }

        body {
            overflow-x: hidden;
        }

        .main-container {
            width: 100%;
            max-width: 100%;
            min-width: 0;
            padding: 20px;
            min-height: 700px;
        }

        .search-card,
        .grid-card {
            width: 100%;
            max-width: 100%;
            min-width: 0;
            background: #ffffff;
            border-radius: 12px;
            padding: 25px;
            margin-bottom: 25px;
            box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
        }

        .page-title {
            font-size: 28px;
            font-weight: 600;
            margin-bottom: 25px;
            color: #1e293b;
        }

        .search-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 18px;
            align-items: start;
        }

        .form-group {
            min-width: 0;
            width: 100%;
            position: relative;
            display: flex;
            flex-direction: column;
        }

        .form-group label {
            display: block;
            margin-bottom: 7px;
            font-weight: 600;
            color: #334155;
            font-size: 14px;
        }

        .textbox {
            display: block;
            width: 100%;
            height: 40px;
            padding: 8px 11px;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            background: #ffffff;
            color: #334155;
            font-size: 14px;
            outline: none;
        }

        .textbox:focus {
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.10);
        }

        .select2-container {
            width: 100% !important;
        }

        .select2-container--default
        .select2-selection--multiple {
            width: 100% !important;
            min-height: 40px !important;
            border: 1px solid #cbd5e1 !important;
            border-radius: 6px !important;
            background: #ffffff !important;
            padding: 2px 5px !important;
        }

        .select2-container--default.select2-container--focus
        .select2-selection--multiple {
            border-color: #2563eb !important;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.10);
        }

        .select2-container--default
        .select2-selection--multiple
        .select2-selection__rendered {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            padding: 0 !important;
            margin: 0 !important;
        }

        .select2-container--default
        .select2-selection--multiple
        .select2-selection__choice {
            margin-top: 4px !important;
            margin-right: 5px !important;
            padding: 2px 7px 2px 20px !important;
            border: 1px solid #bfdbfe !important;
            border-radius: 4px !important;
            background: #eff6ff !important;
            color: #1e40af !important;
            font-size: 12px !important;
        }

        .select2-container--default
        .select2-selection--multiple
        .select2-selection__choice__remove {
            border-right: 1px solid #bfdbfe !important;
            color: #1e40af !important;
        }

        .select2-container--default
        .select2-search--inline
        .select2-search__field {
            height: 27px !important;
            margin-top: 4px !important;
            font-size: 13px !important;
            min-width: 100px !important;
        }

        .select2-dropdown {
            border: 1px solid #cbd5e1 !important;
            border-radius: 6px !important;
            box-shadow: 0 5px 15px rgba(0,0,0,.12);
            z-index: 99999 !important;
        }

        .select2-search--dropdown {
            padding: 8px !important;
        }

        .select2-search--dropdown
        .select2-search__field {
            width: 100% !important;
            height: 36px !important;
            padding: 6px 9px !important;
            border: 1px solid #cbd5e1 !important;
            border-radius: 5px !important;
            outline: none;
        }

        .select2-results__option {
            padding: 7px 10px !important;
            font-size: 13px !important;
        }

        /* =====================================================
           POSTING DETAILS
        ===================================================== */

        .posting-details-card {
            grid-column: 1 / -1;
            border: 1px solid #e2e8f0;
            border-radius: 10px;
            padding: 20px;
            background: #f8fafc;
            margin-top: 5px;
        }

        .posting-details-title {
            font-size: 17px;
            font-weight: 700;
            color: #2563eb;
            margin-bottom: 18px;
        }

        .posting-details-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 18px;
            align-items: start;
        }

        .posting-radio-container {
            min-height: 40px;
            display: flex;
            align-items: center;
        }

        .posting-radio {
            margin-top: 3px;
        }

        .posting-radio td {
            padding-right: 25px;
            white-space: nowrap;
        }

        .posting-radio input[type="radio"] {
            margin-right: 6px;
        }

        .posting-radio label {
            display: inline-block;
            margin-bottom: 0;
            font-weight: 500;
            color: #334155;
            cursor: pointer;
        }

        /* =====================================================
           BUTTONS
        ===================================================== */

        .button-container {
            margin-top: 25px;
            display: flex;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
        }

        .custom-btn {
            min-width: 110px;
            padding: 9px 22px;
            border: none;
            border-radius: 6px;
            color: #ffffff;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
        }

        .btn-search {
            background: #2563eb;
        }

        .btn-search:hover {
            background: #1d4ed8;
        }

        .btn-reset {
            background: #64748b;
        }

        .btn-reset:hover {
            background: #475569;
        }

        /* =====================================================
           GRID OUTER CARD
        ===================================================== */

        .grid-card {
            overflow: hidden;
        }

        /* =====================================================
           TOP HORIZONTAL SCROLLBAR
        ===================================================== */

        .grid-toolbar {
            width: 100%;
            display: flex;
            justify-content: flex-end;
            align-items: center;
            margin-bottom: 10px;
        }

        .grid-scroll-top {
            display: block !important;
            width: 100%;
            height: 22px;
            overflow-x: scroll;
            overflow-y: hidden;
            margin-bottom: 8px;
            -webkit-overflow-scrolling: touch;
        }

        .grid-scroll-top-inner {
            display: block;
            height: 1px;
            width: 100%;
            min-width: 1px;
        }

        .grid-scroll-top::-webkit-scrollbar {
            height: 14px;
        }

        .grid-scroll-top::-webkit-scrollbar-track {
            background: #e2e8f0;
        }

        .grid-scroll-top::-webkit-scrollbar-thumb {
            background: #64748b;
            border-radius: 7px;
        }

        /* =====================================================
           GRID BOTTOM HORIZONTAL SCROLLBAR
        ===================================================== */

        .grid-scroll {
            width: 100%;
            max-width: 100%;
            min-width: 0;
            overflow-x: auto;
            overflow-y: hidden;
            -webkit-overflow-scrolling: touch;
        }

        .gridview {
            width: max-content;
            min-width: 100%;
            border-collapse: collapse;
            margin: 0;
        }

        .gridview th {
            padding: 12px;
            background: #2563eb;
            color: #ffffff;
            text-align: left;
            font-size: 13px;
            font-weight: 600;
            white-space: nowrap;
        }

        .gridview td {
            padding: 11px 12px;
            border-bottom: 1px solid #e2e8f0;
            color: #334155;
            font-size: 13px;
            white-space: nowrap;
        }

        .gridview tr:nth-child(even) {
            background: #f8fafc;
        }

        .gridview tr:hover {
            background: #eef4ff;
        }

        /* =====================================================
           ACTION / EDIT BUTTON
        ===================================================== */

        .action-button {
            display: inline-block;
            min-width: 58px;
            padding: 6px 12px;
            background: #2563eb;
            color: #ffffff !important;
            border-radius: 5px;
            text-decoration: none !important;
            font-size: 12px;
            font-weight: 600;
            text-align: center;
            cursor: pointer;
        }

        .action-button:hover {
            background: #1d4ed8;
            color: #ffffff !important;
        }

        /* =====================================================
           LARGE EDIT EMPLOYEE POPUP
        ===================================================== */

        .employee-edit-modal {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            width: 100vw;
            height: 100vh;
            background: rgba(15, 23, 42, 0.72);
            z-index: 100000;
            align-items: center;
            justify-content: center;
            padding: 2vh 2vw;
        }

        .employee-edit-modal.show {
            display: flex;
        }

        .employee-edit-dialog {
            position: relative;
            width: 96vw;
            max-width: 1600px;
            height: 94vh;
            max-height: 94vh;
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.35);
            overflow: hidden;
        }

        .employee-edit-frame {
            display: block;
            width: 100%;
            height: 100%;
            border: 0;
            background: #ffffff;
        }

     

        /* =====================================================
           LARGE / MEDIUM SCREEN
        ===================================================== */

        @media screen and (max-width: 1200px) {

            .search-grid {
                grid-template-columns: repeat(3, 1fr);
            }

            .posting-details-grid {
                grid-template-columns: repeat(3, 1fr);
            }

        }

        /* =====================================================
           TABLET
        ===================================================== */

        @media screen and (max-width: 991px) {

            .main-container {
                padding: 15px;
            }

            .search-grid {
                grid-template-columns: repeat(2, 1fr);
            }

            .posting-details-grid {
                grid-template-columns: repeat(2, 1fr);
            }

            .search-card,
            .grid-card {
                padding: 20px;
            }

            .employee-edit-dialog {
                width: 97vw;
                height: 95vh;
                max-height: 95vh;
            }

        }

        /* =====================================================
           MOBILE
        ===================================================== */

        @media screen and (max-width: 576px) {

            .main-container {
                padding: 10px;
            }

            .search-card,
            .grid-card {
                padding: 15px;
                border-radius: 8px;
            }

            .page-title {
                font-size: 21px;
                margin-bottom: 18px;
            }

            .search-grid {
                grid-template-columns: 1fr;
                gap: 14px;
            }

            .posting-details-grid {
                grid-template-columns: 1fr;
                gap: 14px;
            }

            .posting-details-card {
                padding: 15px;
            }

            .button-container {
                flex-direction: column;
                width: 100%;
            }

            .custom-btn {
                width: 100%;
            }

            .gridview {
                min-width: 1200px;
            }

            .employee-edit-modal {
                padding: 1vh 1vw;
            }

            .employee-edit-dialog {
                width: 98vw;
                height: 96vh;
                max-height: 96vh;
                border-radius: 7px;
            }

            .employee-edit-close {
                top: 5px;
                right: 5px;
                width: 34px;
                height: 34px;
                line-height: 32px;
                font-size: 24px;
            }

        }

    </style>

    <script type="text/javascript">

        function LoadSearchableDropdowns() {

            var company = $('#<%= lstCompany.ClientID %>');
            var designation = $('#<%= lstDesignation.ClientID %>');
            var postingPlace = $('#<%= lstPostingPlace.ClientID %>');
            var department = $('#<%= lstPostingDepartment.ClientID %>');
            var zone = $('#<%= lstAreaBoardZone.ClientID %>');
            var circle = $('#<%= lstCircle.ClientID %>');
            var division = $('#<%= lstDivision.ClientID %>');
            var subdivision = $('#<%= lstSubdivision.ClientID %>');
            var section = $('#<%= lstSection.ClientID %>');

            if (company.length > 0) {

                if (company.hasClass('select2-hidden-accessible')) {
                    company.select2('destroy');
                }

                company.select2({
                    width: '100%',
                    placeholder: 'Search / Select Company',
                    closeOnSelect: false
                });
            }

            if (designation.length > 0) {

                if (designation.hasClass('select2-hidden-accessible')) {
                    designation.select2('destroy');
                }

                designation.select2({
                    width: '100%',
                    placeholder: 'Search / Select Designation',
                    closeOnSelect: false
                });
            }

            if (postingPlace.length > 0) {

                if (postingPlace.hasClass('select2-hidden-accessible')) {
                    postingPlace.select2('destroy');
                }

                postingPlace.select2({
                    width: '100%',
                    placeholder: 'Search / Select HRMS Posting Place',
                    closeOnSelect: false
                });
            }

            if (department.length > 0) {

                if (department.hasClass('select2-hidden-accessible')) {
                    department.select2('destroy');
                }

                department.select2({
                    width: '100%',
                    placeholder: 'Search / Select Department / Office / Cell',
                    closeOnSelect: false
                });
            }

            if (zone.length > 0) {

                if (zone.hasClass('select2-hidden-accessible')) {
                    zone.select2('destroy');
                }

                zone.select2({
                    width: '100%',
                    placeholder: 'Search / Select Area Board / Zone',
                    closeOnSelect: false
                });
            }

            if (circle.length > 0) {

                if (circle.hasClass('select2-hidden-accessible')) {
                    circle.select2('destroy');
                }

                circle.select2({
                    width: '100%',
                    placeholder: 'Search / Select Circle',
                    closeOnSelect: false
                });
            }

            if (division.length > 0) {

                if (division.hasClass('select2-hidden-accessible')) {
                    division.select2('destroy');
                }

                division.select2({
                    width: '100%',
                    placeholder: 'Search / Select Division',
                    closeOnSelect: false
                });
            }

            if (subdivision.length > 0) {

                if (subdivision.hasClass('select2-hidden-accessible')) {
                    subdivision.select2('destroy');
                }

                subdivision.select2({
                    width: '100%',
                    placeholder: 'Search / Select Subdivision',
                    closeOnSelect: false
                });
            }

            if (section.length > 0) {

                if (section.hasClass('select2-hidden-accessible')) {
                    section.select2('destroy');
                }

                section.select2({
                    width: '100%',
                    placeholder: 'Search / Select Section',
                    closeOnSelect: false
                });
            }
        }

        /* =====================================================
           EDIT EMPLOYEE POPUP
        ===================================================== */

        function openEmployeeEdit(empID) {

            if (!empID) {
                return false;
            }

            var modal = document.getElementById('employeeEditModal');
            var frame = document.getElementById('employeeEditFrame');

            if (!modal || !frame) {
                return false;
            }

            frame.src =
                'EditEmployee.aspx?EmpID=' +
                encodeURIComponent(empID);

            modal.classList.add('show');

            document.body.style.overflow = 'hidden';

            return false;
        }

      function closeEmployeeEdit() {

    var modal =
        document.getElementById('employeeEditModal');

    var frame =
        document.getElementById('employeeEditFrame');

    if (frame) {
        frame.src = 'about:blank';
    }

    if (modal) {
        modal.classList.remove('show');
    }

    document.body.style.overflow = '';

    return false;
}


window.addEventListener('message', function (event) {

    if (!event.data) {
        return;
    }

    if (event.data.type === 'closeEmployeeEdit') {
        closeEmployeeEdit();
    }

});

        /* =====================================================
           GRID TOP / BOTTOM SCROLLBAR
        ===================================================== */

        function getEmployeeGridElements() {

            var top = document.getElementById('<%= gridScrollTop.ClientID %>');
            var inner = document.getElementById('gridScrollTopInner');
            var bottom = document.getElementById('gridScroll');

            if (!top || !inner || !bottom) {
                return null;
            }

            var table = bottom.querySelector('table.gridview');

            if (!table) {
                return null;
            }

            return {
                top: top,
                inner: inner,
                bottom: bottom,
                table: table
            };
        }

        function syncEmployeeGridScrollbars() {

            var elements = getEmployeeGridElements();

            if (!elements) {
                return;
            }

            var top = elements.top;
            var inner = elements.inner;
            var bottom = elements.bottom;
            var table = elements.table;

            var tableWidth = Math.ceil(table.getBoundingClientRect().width);
            var bottomWidth = bottom.scrollWidth;

            if (bottomWidth > tableWidth) {
                tableWidth = bottomWidth;
            }

            if (tableWidth <= bottom.clientWidth) {
                tableWidth = bottom.clientWidth + 1;
            }

            inner.style.width = tableWidth + 'px';
            inner.style.minWidth = tableWidth + 'px';

            if (top.scrollLeft !== bottom.scrollLeft) {
                top.scrollLeft = bottom.scrollLeft;
            }
        }

        function initializeEmployeeGridScrollbars() {

            var elements = getEmployeeGridElements();

            if (!elements) {
                return;
            }

            var top = elements.top;
            var bottom = elements.bottom;

            if (top.getAttribute('data-scroll-bound') !== '1') {

                top.setAttribute('data-scroll-bound', '1');

                top.addEventListener('scroll', function () {
                    bottom.scrollLeft = top.scrollLeft;
                });

                bottom.addEventListener('scroll', function () {
                    top.scrollLeft = bottom.scrollLeft;
                });
            }

            syncEmployeeGridScrollbars();
        }

        function refreshEmployeeGridScrollbars() {

            initializeEmployeeGridScrollbars();

            setTimeout(function () {
                syncEmployeeGridScrollbars();
            }, 50);

            setTimeout(function () {
                syncEmployeeGridScrollbars();
            }, 250);

            setTimeout(function () {
                syncEmployeeGridScrollbars();
            }, 750);
        }

        $(document).ready(function () {

            LoadSearchableDropdowns();
            refreshEmployeeGridScrollbars();

            $(window).on('resize', function () {
                refreshEmployeeGridScrollbars();
            });

        });

    </script>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="main-container">

        <!-- =====================================================
             SEARCH CARD
        ====================================================== -->

        <div class="search-card">

            <div class="page-title">

                <i class="fa fa-search"></i>

                Employee Search

            </div>


            <div class="search-grid">

                <!-- =================================================
                     EMPLOYEE ID
                ================================================== -->

                <div class="form-group">

                    <label>
                        Employee ID
                    </label>

                    <asp:TextBox
                        ID="txtEmpID"
                        runat="server"
                        CssClass="textbox"
                        placeholder="Enter Employee ID">
                    </asp:TextBox>

                </div>


                <!-- =================================================
                     EMPLOYEE NAME
                ================================================== -->

                <div class="form-group">

                    <label>
                        Employee Name
                    </label>

                    <asp:TextBox
                        ID="txtEmpName"
                        runat="server"
                        CssClass="textbox"
                        placeholder="Enter Employee Name">
                    </asp:TextBox>

                </div>


                <!-- =================================================
                     MOBILE
                ================================================== -->

                <div class="form-group">

                    <label>
                        Mobile No
                    </label>

                    <asp:TextBox
                        ID="txtMobile"
                        runat="server"
                        CssClass="textbox"
                        MaxLength="10"
                        placeholder="Enter Mobile No">
                    </asp:TextBox>

                </div>


                <!-- =================================================
                     EMAIL
                ================================================== -->

                <div class="form-group">

                    <label>
                        Email ID
                    </label>

                    <asp:TextBox
                        ID="txtEmail"
                        runat="server"
                        CssClass="textbox"
                        placeholder="Enter Email ID">
                    </asp:TextBox>

                </div>


                <!-- =================================================
                     COMPANY
                ================================================== -->

                <div class="form-group">

                    <label>
                        Company
                    </label>

                    <asp:ListBox
                        ID="lstCompany"
                        runat="server"
                        SelectionMode="Multiple"
                        CssClass="form-control"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="lstCompany_SelectedIndexChanged">
                    </asp:ListBox>

                </div>


                <!-- =================================================
                     DESIGNATION
                ================================================== -->

                <div class="form-group">

                    <label>
                        Designation
                    </label>

                    <asp:ListBox
                        ID="lstDesignation"
                        runat="server"
                        SelectionMode="Multiple"
                        CssClass="form-control">
                    </asp:ListBox>

                </div>


                <!-- =================================================
                     HRMS POSTING PLACE
                ================================================== -->

                <div class="form-group">

                    <label>
                        Posting Place (HRMS)
                    </label>

                    <asp:ListBox
                        ID="lstPostingPlace"
                        runat="server"
                        SelectionMode="Multiple"
                        CssClass="form-control">
                    </asp:ListBox>

                </div>


                <!-- =================================================
                     POSTING DETAILS CARD
                ================================================== -->

                <div
                    id="grpPostingDetails"
                    runat="server"
                    class="posting-details-card"
                    visible="false">

                    <div class="posting-details-title">
                        Posting Details
                    </div>


                    <div class="posting-details-grid">

                        <!-- =========================================
                             POSTING DETAILS PLACE
                        ========================================== -->

                        <div
                            id="grpPostingPlace"
                            runat="server"
                            class="form-group"
                            visible="false">

                            <label>
                                Posting Details - Place
                            </label>

                            <div class="posting-radio-container">

                                <asp:RadioButtonList
                                    ID="rblPostingPlace"
                                    runat="server"
                                    RepeatDirection="Horizontal"
                                    RepeatLayout="Table"
                                    AutoPostBack="true"
                                    CssClass="posting-radio"
                                    OnSelectedIndexChanged="rblPostingPlace_SelectedIndexChanged">

                                    <asp:ListItem
                                        Text="HQ"
                                        Value="HQ">
                                    </asp:ListItem>

                                    <asp:ListItem
                                        Text="Field Office"
                                        Value="Field">
                                    </asp:ListItem>

                                </asp:RadioButtonList>

                            </div>

                        </div>


                        <!-- =========================================
                             DEPARTMENT / OFFICE / CELL
                        ========================================== -->

                        <div
                            id="grpPostingDepartment"
                            runat="server"
                            class="form-group"
                            visible="false">

                            <label>
                                Department / Office / Cell
                            </label>

                            <asp:ListBox
                                ID="lstPostingDepartment"
                                runat="server"
                                SelectionMode="Multiple"
                                CssClass="form-control">
                            </asp:ListBox>

                        </div>


                        <!-- =========================================
                             AREA BOARD / ZONE
                        ========================================== -->

                        <div
                            id="grpAreaBoardZone"
                            runat="server"
                            class="form-group"
                            visible="false">

                            <label>
                                Area Board / Zone
                            </label>

                            <asp:ListBox
                                ID="lstAreaBoardZone"
                                runat="server"
                                SelectionMode="Multiple"
                                CssClass="form-control"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="lstAreaBoardZone_SelectedIndexChanged">
                            </asp:ListBox>

                        </div>


                        <!-- =========================================
                             CIRCLE
                        ========================================== -->

                        <div
                            id="grpCircle"
                            runat="server"
                            class="form-group"
                            visible="false">

                            <label>
                                Circle
                            </label>

                            <asp:ListBox
                                ID="lstCircle"
                                runat="server"
                                SelectionMode="Multiple"
                                CssClass="form-control"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="lstCircle_SelectedIndexChanged">
                            </asp:ListBox>

                        </div>


                        <!-- =========================================
                             DIVISION
                        ========================================== -->

                        <div
                            id="grpDivision"
                            runat="server"
                            class="form-group"
                            visible="false">

                            <label>
                                Division
                            </label>

                            <asp:ListBox
                                ID="lstDivision"
                                runat="server"
                                SelectionMode="Multiple"
                                CssClass="form-control"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="lstDivision_SelectedIndexChanged">
                            </asp:ListBox>

                        </div>


                        <!-- =========================================
                             SUBDIVISION
                        ========================================== -->

                        <div
                            id="grpSubdivision"
                            runat="server"
                            class="form-group"
                            visible="false">

                            <label>
                                Subdivision
                            </label>

                            <asp:ListBox
                                ID="lstSubdivision"
                                runat="server"
                                SelectionMode="Multiple"
                                CssClass="form-control"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="lstSubdivision_SelectedIndexChanged">
                            </asp:ListBox>

                        </div>


                        <!-- =========================================
                             SECTION
                        ========================================== -->

                        <div
                            id="grpSection"
                            runat="server"
                            class="form-group"
                            visible="false">

                            <label>
                                Section
                            </label>

                            <asp:ListBox
                                ID="lstSection"
                                runat="server"
                                SelectionMode="Multiple"
                                CssClass="form-control">
                            </asp:ListBox>

                        </div>

                    </div>

                </div>

            </div>


            <!-- =====================================================
                 BUTTONS
            ====================================================== -->

            <div class="button-container">

                <asp:Button
                    ID="btnSearch"
                    runat="server"
                    Text="Search"
                    CssClass="custom-btn btn-search"
                    OnClick="btnSearch_Click" />

                <asp:Button
                    ID="btnReset"
                    runat="server"
                    Text="Reset"
                    CssClass="custom-btn btn-reset"
                    OnClick="btnReset_Click" />

            </div>

        </div>


        <!-- =====================================================
             EMPLOYEE GRID
        ====================================================== -->

        <div class="grid-card">

            <div class="grid-toolbar">

                <asp:Button
                    ID="btnExportExcel"
                    runat="server"
                    Text="Download Excel"
                    CssClass="custom-btn btn-search"
                    OnClick="btnExportExcel_Click" />

            </div>

            <!-- TOP HORIZONTAL SCROLLBAR -->

            <div
                id="gridScrollTop"
                runat="server"
                class="grid-scroll-top"
                visible="false">

                <div
                    id="gridScrollTopInner"
                    class="grid-scroll-top-inner">
                </div>

            </div>


            <!-- GRID + BOTTOM SCROLLBAR -->

            <div
                id="gridScroll"
                class="grid-scroll">

                <asp:GridView
                    ID="gvEmployee"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="gridview"
                    EmptyDataText="No Record Found"
                    GridLines="None">

                    <Columns>

                      

                        <asp:TemplateField
                            HeaderText="Action">

                            <ItemStyle
                                HorizontalAlign="Center" />

                            <HeaderStyle
                                HorizontalAlign="Center" />

                            <ItemTemplate>

                                <a
                                    href="javascript:void(0);"
                                    class="action-button"
                                    onclick="return openEmployeeEdit('<%# Eval("EmpID") %>');">

                                    Edit

                                </a>

                            </ItemTemplate>

                        </asp:TemplateField>


                   

                        <asp:TemplateField
                            HeaderText="Sl No">

                            <ItemTemplate>

                                <%# Container.DataItemIndex + 1 %>

                            </ItemTemplate>

                        </asp:TemplateField>


           

                        <asp:BoundField
                            DataField="EmpID"
                            HeaderText="Emp ID" />


                    
                        <asp:BoundField
                            DataField="EmpName"
                            HeaderText="Employee Name" />


                       

                        <asp:BoundField
                            DataField="MobileNo"
                            HeaderText="Mobile No" />


                        

                        <asp:BoundField
                            DataField="EmailId"
                            HeaderText="Email ID" />


                       

                        <asp:BoundField
                            DataField="EmpCompany"
                            HeaderText="Company" />


                     

                        <asp:BoundField
                            DataField="EmpDesignation"
                            HeaderText="Designation" />


                       

                        <asp:BoundField
                            DataField="EmpPostingPlace"
                            HeaderText="Posting Place (HRMS)" />


                    

                        <asp:BoundField
                            DataField="DetailPostingPlace"
                            HeaderText="Posting Details" />


                    

                        <asp:BoundField
                            DataField="EmpPostingDepartment"
                            HeaderText="Department / Office / Cell" />


                      
                        <asp:BoundField
                            DataField="AreaBoardZone"
                            HeaderText="Area Board / Zone" />


                       

                        <asp:BoundField
                            DataField="Circle"
                            HeaderText="Circle" />


                       

                        <asp:BoundField
                            DataField="Division"
                            HeaderText="Division" />


                      

                        <asp:BoundField
                            DataField="Subdivision"
                            HeaderText="Subdivision" />


                       
                        <asp:BoundField
                            DataField="Section"
                            HeaderText="Section" />

                    </Columns>


                    <EmptyDataTemplate>

                        <div
                            style="
                                padding:25px;
                                text-align:center;
                                color:#64748b;
                                font-weight:600;">

                            No Employee Record Found

                        </div>

                    </EmptyDataTemplate>

                </asp:GridView>

            </div>

        </div>


    </div>


    <!-- =========================================================
         EDIT EMPLOYEE POPUP
    ========================================================== -->

    <div
        id="employeeEditModal"
        class="employee-edit-modal">

        <div
            class="employee-edit-dialog">

            <!-- TOP RIGHT X -->

          <%--  <button
                type="button"
                class="employee-edit-close"
                aria-label="Close"
                title="Close"
                onclick="return closeEmployeeEdit();">

                &times;

            </button>--%>


            <!-- EDIT EMPLOYEE PAGE -->

            <iframe
                id="employeeEditFrame"
                class="employee-edit-frame"
                title="Edit Employee">
            </iframe>

        </div>

    </div>


</asp:Content>
