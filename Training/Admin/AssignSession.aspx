<%@ Page Title="Session Management" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AssignSession.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="Training.Admin.AssignSession" ClientIDMode="Static" %>
<%@ Register Src="~/Admin/TrainingSummary.ascx" TagPrefix="uc" TagName="TrainingSummary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet" /><script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
<style>body{background:#f5f5f5}.main-card{background:#fff;padding:25px;border-radius:12px;box-shadow:0 0 10px #d9d9d9;margin:20px 0}.page-heading{font-size:28px;font-weight:bold;color:darkcyan;margin-bottom:20px}.validation{color:red;font-size:13px}.readonly-box{background:#eef3f8!important;font-weight:bold}.select2-container{width:100%!important}.select2-container .select2-selection--single{height:38px!important;border:1px solid #ced4da!important}.select2-selection__rendered{line-height:36px!important}.select2-selection__arrow{height:36px!important}.gridview th{background:#0d6efd;color:#fff;font-weight:bold;text-align:center}.gridview td{vertical-align:middle}</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="main-card"><div class="page-heading">Session Management</div><uc:TrainingSummary ID="TrainingSummary1" runat="server" /><asp:Label ID="lblTrainingID" runat="server" Visible="false" /><asp:Label ID="lblTrainingHours" runat="server" Visible="false" />
<div class="card shadow-sm border-0"><div class="card-header bg-success text-white fw-bold">Session Details</div><div class="card-body"><div class="row"><div class="col-lg-3 mb-3"><label class="form-label fw-bold">Session ID</label><asp:TextBox ID="txtSessionID" runat="server" CssClass="form-control readonly-box" ReadOnly="true" /></div><div class="col-lg-2 mb-3"><label class="form-label fw-bold">Session No.</label><asp:TextBox ID="txtSessionNo" runat="server" CssClass="form-control readonly-box" ReadOnly="true" /></div><div class="col-lg-7 mb-3"><label class="form-label fw-bold">Session Name</label><asp:TextBox ID="txtSessionName" runat="server" CssClass="form-control" /><asp:RequiredFieldValidator ID="rfvSessionName" runat="server" ControlToValidate="txtSessionName" ValidationGroup="SaveGroup" CssClass="validation" ErrorMessage="Required" /></div><div class="col-lg-3 mb-3"><label class="form-label fw-bold">Session Date</label><asp:TextBox ID="txtSessionDate" runat="server" CssClass="form-control flatpickr" placeholder="dd-mm-yyyy" autocomplete="off" onkeydown="return false;" /></div><div class="col-lg-3 mb-3"><label class="form-label fw-bold">Start Time</label><asp:DropDownList ID="ddlStartTime" runat="server" CssClass="form-select" /></div><div class="col-lg-3 mb-3"><label class="form-label fw-bold">End Time</label><asp:DropDownList ID="ddlEndTime" runat="server" CssClass="form-select" /></div><div class="col-lg-3 mb-3"><label class="form-label fw-bold">Session Hours</label><asp:TextBox ID="txtTotalHours" runat="server" CssClass="form-control readonly-box" ReadOnly="true" /><asp:HiddenField ID="hfTotalHours" runat="server" /></div><div class="col-lg-6 mb-3"><label class="form-label fw-bold">Topic</label><asp:DropDownList ID="ddlTopic" runat="server" CssClass="form-select" /><asp:RequiredFieldValidator ID="rfvTopic" runat="server" ControlToValidate="ddlTopic" InitialValue="" ErrorMessage="Select Topic" CssClass="validation" ValidationGroup="SaveGroup" /></div><div class="col-lg-6 mb-3"><label class="form-label fw-bold">Trainer</label><asp:DropDownList ID="ddlTrainer" runat="server" CssClass="form-select" /><asp:RequiredFieldValidator ID="rfvTrainer" runat="server" ControlToValidate="ddlTrainer" InitialValue="" ErrorMessage="Select Trainer" CssClass="validation" ValidationGroup="SaveGroup" /></div><div class="col-lg-12 mb-3"><label class="form-label fw-bold">Area of Expertise</label><asp:Label ID="lblTrainerExpertise" runat="server" CssClass="form-control readonly-box" /></div><div class="col-lg-12 mb-3"><label class="form-label fw-bold">Remarks</label><asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control" Rows="3" TextMode="MultiLine" /></div><div class="col-12 text-center"><asp:Button ID="btnSave" runat="server" CssClass="btn btn-success" Text="Add Session" ValidationGroup="SaveGroup" OnClick="btnSave_Click" /><asp:Button ID="btnUpdate" runat="server" CssClass="btn btn-warning" Text="Update" Visible="false" ValidationGroup="SaveGroup" OnClick="btnUpdate_Click" /><asp:Button ID="btnDelete" runat="server" CssClass="btn btn-danger" Text="Delete" Visible="false" CausesValidation="false" OnClick="btnDelete_Click" /><asp:Button ID="btnClear" runat="server" CssClass="btn btn-secondary" Text="Clear" CausesValidation="false" OnClick="btnClear_Click" /></div><div class="col-12 mt-3"><asp:Label ID="lblMessage" runat="server" Font-Bold="true" /></div></div></div></div>
<div class="card shadow-sm border-0 mt-4"><div class="card-header bg-info text-white fw-bold">Session List</div><div class="card-body p-0 table-responsive"><asp:GridView ID="gvSession" runat="server" CssClass="table table-bordered table-hover table-striped mb-0 gridview" AutoGenerateColumns="False" DataKeyNames="SessionID" OnRowCommand="gvSession_RowCommand" OnRowDataBound="gvSession_RowDataBound"><Columns><asp:TemplateField HeaderText="#"><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><ItemStyle Width="50px" HorizontalAlign="Center" /></asp:TemplateField><asp:TemplateField HeaderText="Session"><ItemTemplate><span class="badge bg-dark">S-<%# Eval("SessionNo") %></span></ItemTemplate></asp:TemplateField><asp:BoundField DataField="SessionName" HeaderText="Session Name" /><asp:TemplateField HeaderText="Schedule"><ItemTemplate><div><%# Eval("SessionDate") %></div><small class="text-muted"><%# Eval("StartTime") %> - <%# Eval("EndTime") %></small></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Hours"><ItemTemplate><span class="badge bg-warning text-dark"><%# Eval("TotalHours") %> Hr</span></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Topic"><ItemTemplate><span class="badge bg-primary"><%# Eval("TopicName") %></span></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Trainer"><ItemTemplate><div style="font-weight:bold;color:#0d6efd;">Trainer ID: <%# Eval("TrainerID") %></div><div>Employee ID: <%# Eval("DisplayTrainerID") %></div><div><%# Eval("TrainerName") %></div><small class="text-muted"><%# Eval("Designation") %></small><br/><span class='<%# Eval("TrainerType").ToString()=="Internal" ? "badge bg-success" : "badge bg-warning text-dark" %>'><%# Eval("TrainerType") %></span><div class="small mt-1">Mobile: <asp:Label ID="lblTrainerMobile" runat="server" /></div><div class="small">Email: <asp:Label ID="lblTrainerEmail" runat="server" /></div></ItemTemplate><ItemStyle Width="280px" /></asp:TemplateField><asp:TemplateField HeaderText="Edit"><ItemTemplate><asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-sm btn-primary" CommandName="EditRecord" CommandArgument='<%# Eval("SessionID") %>'><i class="fa fa-edit"></i></asp:LinkButton></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Delete"><ItemTemplate><asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-sm btn-danger" CommandName="DeleteRecord" CommandArgument='<%# Eval("SessionID") %>' OnClientClick="return confirm('Delete this Session ?');"><i class="fa fa-trash"></i></asp:LinkButton></ItemTemplate></asp:TemplateField></Columns><HeaderStyle BackColor="#0d6efd" ForeColor="White" Font-Bold="true" /><EmptyDataTemplate><div class="text-center p-4 text-danger fw-bold">No Session Created Yet.</div></EmptyDataTemplate></asp:GridView></div></div>
<div class="row mt-4"><div class="col-lg-4"><div class="card border-0 shadow-sm text-center"><div class="card-body"><h6>Total Sessions</h6><h2 class="text-primary fw-bold"><asp:Label ID="lblTotalSessions" runat="server" Text="0" /></h2></div></div></div><div class="col-lg-4"><div class="card border-0 shadow-sm text-center"><div class="card-body"><h6>Total Session Hours</h6><h2 class="text-success fw-bold"><asp:Label ID="lblUsedHours" runat="server" Text="0" /></h2></div></div></div><div class="col-lg-4"><div class="card border-0 shadow-sm text-center"><div class="card-body"><h6>Remaining Hours</h6><h2 class="text-danger fw-bold"><asp:Label ID="lblRemainingHours" runat="server" Text="0" /></h2></div></div></div></div><hr/><div class="text-center"><asp:Button ID="btnUpdateBatch" runat="server" Text="Update Batch" CssClass="btn btn-secondary" OnClick="btnUpdateBatch_Click" /><asp:Button ID="btnUpdateTrainee" runat="server" Text="Update Trainee" CssClass="btn btn-secondary" OnClick="btnUpdateTrainee_Click" /><asp:Button ID="btnFinishSession" runat="server" Text="Finish Session & Trainer Assignment" CssClass="btn btn-success" OnClick="btnFinishSession_Click" /></div></div></div>
<script>
function findControl(id) { return $('[id$="' + id + '"]'); }
function initControls() {
    $('.flatpickr').each(function () { if (this._flatpickr) this._flatpickr.destroy(); });
    if (typeof flatpickr !== 'undefined') flatpickr('.flatpickr', { dateFormat: 'd-m-Y', allowInput: false, clickOpens: true });
    ['ddlStartTime','ddlEndTime','ddlTopic','ddlTrainer'].forEach(function(id) {
        var el = findControl(id);
        if (!el.length) return;
        if (el.hasClass('select2-hidden-accessible')) el.select2('destroy');
        var options = { width: '100%' };
        if (id === 'ddlTopic') options.placeholder = 'Select Topic';
        if (id === 'ddlTrainer') { options.placeholder = 'Search Trainer...'; options.allowClear = true; }
        el.select2(options);
    });
    calculateHours();
}
function convertToMinutes(time) {
    if (!time) return 0;
    var p=time.split(' '), hm=p[0].split(':'), h=parseInt(hm[0],10), m=parseInt(hm[1],10);
    if (p[1]==='PM' && h!==12) h+=12;
    if (p[1]==='AM' && h===12) h=0;
    return h*60+m;
}
function calculateHours() {
    var s=findControl('ddlStartTime').val(), e=findControl('ddlEndTime').val();
    var total=findControl('txtTotalHours'), hidden=findControl('hfTotalHours');
    if (!s || !e) { total.val(''); hidden.val(''); return; }
    var start=convertToMinutes(s), end=convertToMinutes(e);
    if (end<=start) { total.val(''); hidden.val(''); return; }
    var hrs=((end-start)/60).toFixed(2);
    total.val(hrs); hidden.val(hrs);
}
function loadTrainerExpertise() {
    var trainerID=findControl('ddlTrainer').val();
    var expertise=findControl('lblTrainerExpertise');
    if (!trainerID) { expertise.text(''); return; }
    $.ajax({
        type:'POST',
        url:'<%= ResolveUrl("~/Admin/AssignSession.aspx/GetTrainerExpertise") %>',
        contentType:'application/json; charset=utf-8',
        dataType:'json',
        data:JSON.stringify({trainerID:trainerID}),
        success:function(r){ expertise.text(r.d || ''); },
        error:function(){ expertise.text(''); }
    });
}
$(document).on('change', '[id$="ddlTrainer"]', loadTrainerExpertise);
$(document).on('change', '[id$="ddlStartTime"],[id$="ddlEndTime"]', calculateHours);
window.addEventListener('load', initControls);
if (typeof(Sys) !== 'undefined') Sys.Application.add_load(initControls);
</script>
<script runat="server">
protected void gvSession_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
{
    if (e.Row.RowType != System.Web.UI.WebControls.DataControlRowType.DataRow) return;
    string trainerID = System.Web.UI.DataBinder.Eval(e.Row.DataItem, "TrainerID").ToString();
    string trainerType = System.Web.UI.DataBinder.Eval(e.Row.DataItem, "TrainerType").ToString();
    string mobile = "";
    string email = "";
    string constr = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
    using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(constr))
    {
        string sql = trainerType == "Internal"
            ? "SELECT ISNULL(MobileNo,'') AS MobileNo, ISNULL(EmailId,'') AS EmailId FROM EmpBasicMaster WHERE EmpID=(SELECT EmpID FROM TrainerMaster WHERE TrainerID=@TrainerID)"
            : "SELECT ISNULL(MobileNo,'') AS MobileNo, ISNULL(EmailID,'') AS EmailId FROM TrainerMaster WHERE TrainerID=@TrainerID";
        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, con))
        {
            cmd.Parameters.AddWithValue("@TrainerID", trainerID);
            con.Open();
            using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
            {
                if (dr.Read()) { mobile = dr["MobileNo"].ToString(); email = dr["EmailId"].ToString(); }
            }
        }
    }
    System.Web.UI.WebControls.Label lblMobile = e.Row.FindControl("lblTrainerMobile") as System.Web.UI.WebControls.Label;
    System.Web.UI.WebControls.Label lblEmail = e.Row.FindControl("lblTrainerEmail") as System.Web.UI.WebControls.Label;
    if (lblMobile != null) lblMobile.Text = mobile;
    if (lblEmail != null) lblEmail.Text = email;
}
</script>
</asp:Content>