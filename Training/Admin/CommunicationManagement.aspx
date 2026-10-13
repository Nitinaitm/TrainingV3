<%@ Page Title="Announcements & Notifications" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="CommunicationManagement.aspx.cs" Inherits="Training.Admin.CommunicationManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<style>
.communication-card{background:#fff;border-radius:12px;padding:22px;box-shadow:0 2px 12px rgba(15,23,42,.08);margin-bottom:20px}
.communication-title{font-size:26px;font-weight:700;color:#0d6efd;margin-bottom:18px}
.communication-grid{min-width:1000px}
.communication-grid th{white-space:nowrap;background:#1f2937;color:#fff}
.communication-grid td{vertical-align:middle}
.message-cell{min-width:240px;max-width:360px;white-space:normal;overflow-wrap:anywhere}
@media(max-width:768px){.communication-card{padding:14px}.communication-title{font-size:22px}}
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
 <div class="communication-title"><i class="fas fa-bell me-2"></i>Announcements &amp; Notifications</div>
 <div class="communication-card">
  <div class="row g-3 align-items-end">
   <div class="col-md-4"><label class="form-label">Show</label><asp:DropDownList ID="ddlType" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged"><asp:ListItem Text="All Announcements & Notifications" Value="All"/><asp:ListItem Text="Announcements" Value="Announcement"/><asp:ListItem Text="Notifications" Value="Notification"/></asp:DropDownList></div>
   <div class="col-md-8"><asp:Label ID="lblMessage" runat="server" CssClass="fw-semibold"/></div>
  </div>
 </div>
 <asp:Panel ID="pnlEdit" runat="server" Visible="false" CssClass="communication-card">
  <h5 class="mb-3">Edit communication</h5>
  <asp:HiddenField ID="hfRecordID" runat="server"/><asp:HiddenField ID="hfRecordType" runat="server"/>
  <div class="row g-3">
   <div class="col-md-4"><label class="form-label">Type</label><asp:TextBox ID="txtEditType" runat="server" CssClass="form-control" ReadOnly="true"/></div>
   <div class="col-md-8"><label class="form-label">Title (announcements)</label><asp:TextBox ID="txtEditTitle" runat="server" CssClass="form-control" MaxLength="250"/></div>
   <div class="col-12"><label class="form-label">Full details / Message</label><asp:TextBox ID="txtEditMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5"/></div>
   <div class="col-md-4"><label class="form-label">Effective From</label><asp:TextBox ID="txtEffectiveFrom" runat="server" CssClass="form-control" TextMode="Date"/></div>
   <div class="col-md-4"><label class="form-label">Effective To</label><asp:TextBox ID="txtEffectiveTo" runat="server" CssClass="form-control" TextMode="Date"/></div>
   <div class="col-md-4"><label class="form-label">Availability</label><asp:DropDownList ID="ddlAvailabilityMode" runat="server" CssClass="form-select"><asp:ListItem Text="Complete batch duration" Value="Batch"/><asp:ListItem Text="This session only" Value="Session"/><asp:ListItem Text="Custom dates" Value="Custom"/></asp:DropDownList></div>
   <div class="col-md-6"><label class="form-label">Target Batch / Training ID</label><asp:TextBox ID="txtEditTrainingID" runat="server" CssClass="form-control" MaxLength="50"/></div><div class="col-md-6"><label class="form-label">Target Session ID</label><asp:TextBox ID="txtEditSessionID" runat="server" CssClass="form-control" MaxLength="50"/></div><div class="col-12"><asp:Label ID="lblEditTarget" runat="server" CssClass="text-secondary small"/></div>
   <div class="col-12"><asp:Button ID="btnSaveEdit" runat="server" Text="Save Changes" CssClass="btn btn-primary" OnClick="btnSaveEdit_Click"/><asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn btn-outline-secondary ms-2" CausesValidation="false" OnClick="btnCancelEdit_Click"/></div>
  </div>
 </asp:Panel>
 <div class="communication-card">
  <div class="table-responsive">
   <asp:GridView ID="gvCommunications" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered table-hover communication-grid" DataKeyNames="RecordID,RecordType" EmptyDataText="No announcements or notifications found." OnRowCommand="gvCommunications_RowCommand">
    <Columns>
     <asp:BoundField DataField="RecordType" HeaderText="Type"/>
     <asp:BoundField DataField="Title" HeaderText="Title"/>
     <asp:TemplateField HeaderText="Details"><ItemTemplate><div class="message-cell"><%#: Eval("Message") %></div></ItemTemplate></asp:TemplateField>
     <asp:BoundField DataField="SenderID" HeaderText="Posted By (ID)"/>
     <asp:BoundField DataField="CreatedOn" HeaderText="Posted On" DataFormatString="{0:dd-MMM-yyyy HH:mm}"/>
     <asp:BoundField DataField="AvailabilityMode" HeaderText="Available For"/>
     <asp:BoundField DataField="EffectiveFrom" HeaderText="From" DataFormatString="{0:dd-MMM-yyyy}"/>
     <asp:BoundField DataField="EffectiveTo" HeaderText="To" DataFormatString="{0:dd-MMM-yyyy}"/>
     <asp:TemplateField HeaderText="Target"><ItemTemplate><%# Eval("TargetTrainingID") %><%# String.IsNullOrEmpty(Convert.ToString(Eval("TargetSessionID"))) ? "" : " / Session " + Eval("TargetSessionID") %></ItemTemplate></asp:TemplateField>
     <asp:TemplateField HeaderText="Actions"><ItemTemplate><asp:LinkButton ID="lnkDetails" runat="server" Text="Details / Edit" CssClass="btn btn-sm btn-outline-primary mb-1" CommandName="EditRecord" CommandArgument='<%# Container.DataItemIndex %>'/><asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" CssClass="btn btn-sm btn-outline-danger" CommandName="DeleteRecord" CommandArgument='<%# Container.DataItemIndex %>' OnClientClick="return confirm('Permanently delete this communication?');"/></ItemTemplate></asp:TemplateField>
    </Columns>
   </asp:GridView>
  </div>
 </div>
</div>
</asp:Content>