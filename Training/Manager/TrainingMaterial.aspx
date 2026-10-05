<%@ page title="Training Materials" language="C#" masterpagefile="~/ManagerMaster.Master" autoeventwireup="true" codebehind="TrainingMaterial.aspx.cs" inherits="Training.Manager.TrainingMaterial" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .material-card {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 2px 10px rgba(0,0,0,.08);
            margin-bottom: 20px;
            padding: 20px
        }

        .page-title {
            font-size: 24px;
            font-weight: 700;
            color: #0d6efd
        }

        .section-title {
            font-size: 18px;
            font-weight: 700;
            color: #198754;
            margin-bottom: 15px
        }

        .gridview th {
            background: #198754;
            color: #fff;
            text-align: center
        }

        .gridview td {
            vertical-align: middle
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="material-card">
            <div class="page-title">Training Material Management</div>
            <div class="text-muted mt-1">Upload and manage materials for sessions available to your mapped training location.</div>
        </div>
                <asp:Panel ID="pnlMaterial" runat="server" Visible="false">
            <div class="material-card">
                <div class="section-title">Upload Training Material</div>
                <div class="row">
                    <div class="col-md-4">
                        <label>Title *</label><asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" /></div>
                    <div class="col-md-3">
                        <label>Material Type *</label><asp:DropDownList ID="ddlType" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Select Type</asp:ListItem>
                            <asp:ListItem Value="PDF">PDF</asp:ListItem>
                            <asp:ListItem Value="PPT">PPT</asp:ListItem>
                            <asp:ListItem Value="Document">Document</asp:ListItem>
                            <asp:ListItem Value="Video">Video</asp:ListItem>
                            <asp:ListItem Value="Other">Other</asp:ListItem>
                        </asp:DropDownList></div>
                    <div class="col-md-3">
                        <label>Select File *</label><asp:FileUpload ID="fuMaterial" runat="server" CssClass="form-control" /></div>
                    <div class="col-md-2">
                        <label>&nbsp;</label><asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btn btn-success w-100" OnClick="btnUpload_Click" /></div>
                </div>
                <div class="mt-3">
                    <label>Description</label><asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" /></div>
                <div class="row mt-3">
                    <div class="col-md-3">
                        <asp:CheckBox ID="chkVisibleToTrainee" runat="server" Text="Visible To Trainee" Checked="true" /></div>
                    <div class="col-md-3">
                        <asp:CheckBox ID="chkDownloadAllowed" runat="server" Text="Download Allowed" Checked="true" /></div>
                </div>
                <asp:Label ID="lblMessage" runat="server" Font-Bold="true" /></div>
            <div class="material-card">
                <div class="section-title">Uploaded Training Materials</div>
                <asp:GridView ID="gvMaterial" runat="server" AutoGenerateColumns="false" DataKeyNames="MaterialID" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Training Material Uploaded." OnRowCommand="gvMaterial_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="Sl No">
                            <ItemTemplate><%# Container.DataItemIndex+1 %></ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Title" HeaderText="Title" />
                        <asp:BoundField DataField="MaterialType" HeaderText="Type" />
                        <asp:BoundField DataField="FileName" HeaderText="File Name" />
                        <asp:BoundField DataField="Description" HeaderText="Description" />
                        <asp:BoundField DataField="CreatedOn" HeaderText="Uploaded On" DataFormatString="{0:dd-MM-yyyy HH:mm}" />
                        <asp:TemplateField HeaderText="Visible">
                            <ItemTemplate><%# Convert.ToBoolean(Eval("VisibleToTrainee")) ? "Yes" : "No" %></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Download">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkDownload" runat="server" Text="Download" CommandName="DownloadMaterial" CommandArgument='<%# Eval("MaterialID") %>' CausesValidation="false" /></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Delete">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" CommandName="DeleteMaterial" CommandArgument='<%# Eval("MaterialID") %>' CausesValidation="false" OnClientClick="return confirm('Are you sure?');" /></ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
