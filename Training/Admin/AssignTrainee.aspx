<%@ Page Title="Assign Trainee" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AssignTrainee.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="Training.Admin.AssignTrainee" ClientIDMode="Static" %>
<%@ Register Src="~/Admin/TrainingSummary.ascx" TagPrefix="uc" TagName="TrainingSummary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<style>
body{background:#f5f5f5}
.main-card{display:block;width:100%;box-sizing:border-box;background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin:20px 0}
.page-heading{font-size:28px;font-weight:bold;color:#198754;margin-bottom:20px}
.section-title{font-size:20px;font-weight:bold;color:#198754;margin:18px 0 15px}
.main-card .row{--bs-gutter-x:1.5rem;--bs-gutter-y:0}
.grid-wrapper{width:100%;max-width:100%;overflow-x:auto;margin-top:12px}
.gridview{width:100%;margin-bottom:0}
.gridview th{background:#198754;color:#fff;text-align:center;white-space:nowrap;padding:10px}
.gridview td{vertical-align:middle;padding:9px}
.select2-container{width:100%!important}
.select2-container .select2-selection--multiple{min-height:38px;border:1px solid #ced4da}
.main-card .form-control,.main-card .form-select{max-width:100%}
.main-card .btn{margin:2px}
@media(max-width:767.98px){
 .main-card{padding:15px;margin:12px 0}
 .page-heading{font-size:23px}
 .section-title{font-size:18px}
 .main-card .row>[class*="col-"]{margin-bottom:12px}
}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card"><div class="page-heading">Assign Trainee</div><uc:TrainingSummary ID="TrainingSummary1" runat="server"/>
<div class="mb-4"><asp:Button ID="btnEmpWise" runat="server" Text="EmpID Wise" CssClass="btn btn-primary" OnClick="btnEmpWise_Click"/> <asp:Button ID="btnCompanyWise" runat="server" Text="Company Wise" CssClass="btn btn-warning" OnClick="btnCompanyWise_Click"/> <asp:Button ID="btnBulkWise" runat="server" Text="Bulk Upload" CssClass="btn btn-success" OnClick="btnBulkWise_Click"/></div>
<asp:Panel ID="pnlEmpWise" runat="server"><div class="section-title">Employee Wise Assignment</div><div class="row"><div class="col-md-4"><label class="form-label">Employee ID</label><asp:TextBox ID="txtEmpID" runat="server" CssClass="form-control" placeholder="Enter Employee ID"/></div><div class="col-md-2"><br/><asp:Button ID="btnAddEmployee" runat="server" Text="Add Employee" CssClass="btn btn-success" OnClick="btnAddEmployee_Click"/></div><div class="col-md-6"><br/><asp:Label ID="lblEmpMessage" runat="server" Font-Bold="true"/></div></div><asp:Panel ID="pnlRepeatCourseConfirm" runat="server" Visible="false" CssClass="alert alert-warning mt-3"><asp:Label ID="lblRepeatCourseMessage" runat="server" Text="This employee has already been assigned to the same course in another batch. Do you want to assign anyway?"/><div class="mt-2"><asp:Button ID="btnAllowRepeatCourse" runat="server" Text="Allow &amp; Assign Anyway" CssClass="btn btn-warning" CausesValidation="false" OnClick="btnAllowRepeatCourse_Click"/><asp:Button ID="btnCancelRepeatCourse" runat="server" Text="Cancel" CssClass="btn btn-outline-secondary" CausesValidation="false" OnClick="btnCancelRepeatCourse_Click"/></div></asp:Panel><hr/></asp:Panel>
<asp:Panel ID="pnlBulk" runat="server" Visible="false"><div class="section-title">Bulk Upload Assignment</div><asp:Button ID="btnDownloadFormat" runat="server" Text="Download Sample Format" CssClass="btn btn-info" OnClick="btnDownloadFormat_Click"/><div class="row mt-3"><div class="col-md-5"><asp:FileUpload ID="fuExcel" runat="server" CssClass="form-control"/></div><div class="col-md-2"><asp:Button ID="btnUploadExcel" runat="server" Text="Upload & Assign" CssClass="btn btn-success" OnClick="btnUploadExcel_Click"/></div><div class="col-md-5"><asp:Label ID="lblBulkMessage" runat="server" Font-Bold="true"/></div></div></asp:Panel>
<asp:Panel ID="pnlCompany" runat="server" Visible="false"><div class="section-title">Company Wise Assignment</div><div class="row"><div class="col-md-4"><label>Company</label><asp:ListBox ID="lstCompany" runat="server" SelectionMode="Multiple" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="lstCompany_SelectedIndexChanged"/></div></div><hr/><div class="row"><div class="col-md-6"><label>Designation</label><asp:ListBox ID="lstDesignation" runat="server" SelectionMode="Multiple" CssClass="form-control" Style="height:300px"/></div><div class="col-md-6"><label>Posting Place</label><asp:ListBox ID="lstPostingPlace" runat="server" SelectionMode="Multiple" CssClass="form-control" Style="height:300px"/></div></div><hr/><div class="text-center"><asp:Button ID="btnLoadEmployee" runat="server" Text="Load Employees" CssClass="btn btn-primary" OnClick="btnLoadEmployee_Click"/> <asp:Button ID="btnSelectAll" runat="server" Text="Select All" CssClass="btn btn-info" OnClientClick="SelectAllEmployee();return false;"/> <asp:Button ID="btnClearSelection" runat="server" Text="Clear Selection" CssClass="btn btn-warning" OnClientClick="ClearEmployeeSelection();return false;"/> <asp:Button ID="btnAssignSelected" runat="server" Text="Assign Selected" CssClass="btn btn-success" OnClick="btnAssignSelected_Click"/></div><br/><asp:Label ID="lblMessage" runat="server" Font-Bold="true" ForeColor="Green"/>
<div class="grid-wrapper"><asp:GridView ID="gvCompanyEmployee" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped gridview" EmptyDataText="No Employee Found"><Columns><asp:TemplateField HeaderText="Select"><HeaderTemplate><input type="checkbox" onclick="ToggleAll(this);"/></HeaderTemplate><ItemTemplate><asp:CheckBox ID="chkSelect" runat="server"/></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Sl No"><ItemTemplate><%# Container.DataItemIndex+1 %></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Emp ID"><ItemTemplate><asp:Label ID="lblEmpID" runat="server" Text='<%# Eval("EmpID") %>'/></ItemTemplate></asp:TemplateField><asp:BoundField DataField="EmpName" HeaderText="Employee Name"/><asp:BoundField DataField="EmpDesignation" HeaderText="Designation"/><asp:BoundField DataField="EmpCompany" HeaderText="Company"/><asp:BoundField DataField="EmpPostingPlace" HeaderText="Posting Place"/><asp:TemplateField HeaderText="Mobile No"><ItemTemplate><asp:Label ID="lblCompanyMobile" runat="server" Text="-"/></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Email ID"><ItemTemplate><asp:Label ID="lblCompanyEmail" runat="server" Text="-"/></ItemTemplate></asp:TemplateField></Columns></asp:GridView></div></asp:Panel>
<hr/><div class="section-title" runat="server" id="messageTotalAssigned">Total Trainee Assigned</div><div class="grid-wrapper"><asp:GridView ID="gvAssignedEmployee" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped gridview" EmptyDataText="No Employee Assigned" OnRowCommand="gvAssignedEmployee_RowCommand"><Columns><asp:TemplateField HeaderText="Sl No"><ItemTemplate><%# Container.DataItemIndex+1 %></ItemTemplate></asp:TemplateField><asp:BoundField DataField="EmpID" HeaderText="Employee ID"/><asp:BoundField DataField="EmpName" HeaderText="Employee Name"/><asp:BoundField DataField="EmpDesignation" HeaderText="Designation"/><asp:BoundField DataField="EmpCompany" HeaderText="Company"/><asp:BoundField DataField="EmpPostingPlace" HeaderText="Posting Place"/><asp:TemplateField HeaderText="Mobile No"><ItemTemplate><asp:Label ID="lblAssignedMobile" runat="server" Text="-"/></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Email ID"><ItemTemplate><asp:Label ID="lblAssignedEmail" runat="server" Text="-"/></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Assignment Status"><ItemTemplate><asp:Label ID="lblStatus" runat="server" Text='<%# Eval("AssignmentStatus") %>' CssClass="badge bg-success"/></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Action"><ItemTemplate><asp:LinkButton ID="lnkRemove" runat="server" Text="Remove" CssClass="btn btn-danger btn-sm" CommandName="RemoveEmployee" CommandArgument='<%# Eval("AssignmentID") %>' OnClientClick="return confirm('Are you sure you want to remove this employee?');"/></ItemTemplate></asp:TemplateField></Columns></asp:GridView></div>
<div class="text-center mt-3"><asp:Button ID="btnUpdateBatch" runat="server" Text="Update Batch" CssClass="btn btn-secondary" OnClick="btnUpdateBatch_Click"/> <asp:Button ID="btnPrevious" runat="server" Text="Update Sessions & Trainers" CssClass="btn btn-secondary" OnClick="btnPrevious_Click"/> <asp:Button ID="btnFinish" runat="server" Text="Finish Trainee Assignment" CssClass="btn btn-success" OnClick="btnFinish_Click"/></div>
</div></div>
<script>
function initAssignTraineeSelect2() {
    ['lstCompany','lstDesignation','lstPostingPlace'].forEach(function(id) {
        var el = $('[id$="' + id + '"]');
        if (!el.length) return;
        if (el.hasClass('select2-hidden-accessible')) el.select2('destroy');
        el.select2({ width: '100%', closeOnSelect: false });
    });
}
window.addEventListener('load', initAssignTraineeSelect2);
if (typeof(Sys) !== 'undefined') {
    Sys.Application.add_load(initAssignTraineeSelect2);
}
function ToggleAll(cb) { $('[id$="gvCompanyEmployee"] input[type=checkbox]').prop('checked', cb.checked); }
function SelectAllEmployee() { $('[id$="gvCompanyEmployee"] input[type=checkbox]').prop('checked', true); }
function ClearEmployeeSelection() { $('[id$="gvCompanyEmployee"] input[type=checkbox]').prop('checked', false); }
</script>
</asp:Content>