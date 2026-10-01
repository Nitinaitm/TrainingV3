<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="LMSMaterials.aspx.cs" Inherits="Training.Admin.LMSMaterials" ValidateRequest="false"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
    <script>
        function toggleMaterialInput() {
            var type = document.getElementById('<%= ddlMaterialType.ClientID %>').value;
            var fileDiv = document.getElementById('<%= divFileUpload.ClientID %>');
            var videoDiv = document.getElementById('<%= divVideoUrl.ClientID %>');

            if (type === 'Video') {
                fileDiv.style.display = 'none';
                videoDiv.style.display = 'block';
            }
            else {
                fileDiv.style.display = 'block';
                videoDiv.style.display = 'none';
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4 class="mb-3">Learning Resource Library - Manage Materials</h4>

    <div class="card mb-3">
        <div class="card-body">

            <asp:HiddenField ID="hfID" runat="server" Value="0" />
            <asp:HiddenField ID="hfExistingFilePath" runat="server" Value="" />

            <div class="row">
                <div class="col-md-6">
                    <label>Title <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-3">
                    <label>Material Type <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlMaterialType" runat="server" CssClass="form-select" onchange="toggleMaterialInput()">
                        <asp:ListItem Text="Manual" Value="Manual"></asp:ListItem>
                        <asp:ListItem Text="PPT" Value="PPT"></asp:ListItem>
                        <asp:ListItem Text="Document" Value="Document"></asp:ListItem>
                        <asp:ListItem Text="Video" Value="Video"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <label>Category</label>
                    <asp:TextBox ID="txtCategory" runat="server" CssClass="form-control" placeholder="e.g. Safety, Technical" />
                </div>
            </div>

            <div class="row mt-3">
                <div class="col-md-6">
                    <label>Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                </div>
                <div class="col-md-3">
                    <label>Course (optional)</label>
                    <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-select">
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <label>Display Order</label>
                    <asp:TextBox ID="txtDisplayOrder" runat="server" CssClass="form-control" Text="1" />
                </div>
            </div>

            <div class="row mt-3">
                <div class="col-md-6" id="divFileUpload" runat="server">
                    <label>File (PDF / PPT / PPTX / DOC / DOCX, max 20 MB)</label>
                    <asp:FileUpload ID="fuFile" runat="server" CssClass="form-control" />
                    <div class="small text-muted mt-1">
                        <asp:Label ID="lblExistingFile" runat="server" />
                    </div>
                </div>
                <div class="col-md-6" id="divVideoUrl" runat="server" style="display: none;">
                    <label>Video URL or Embed Code</label>
                    <asp:TextBox ID="txtVideoUrl" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"
                        placeholder="Paste a plain embed URL (https://www.youtube.com/embed/...), OR paste YouTube's full Share &gt; Embed &lt;iframe&gt; code to control the exact size yourself." />
                </div>
            </div>

            <div class="row mt-3">
                <div class="col-md-3">
                    <div class="form-check mt-4">
                        <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" CssClass="form-check-input" />
                        <label class="form-check-label">Active</label>
                    </div>
                </div>
            </div>

            <div class="mt-3">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancelEdit_Click" />
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-3" />

        </div>
    </div>

    <div class="card">
        <div class="card-body table-responsive">
            <asp:GridView
                ID="gvMaterials"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-bordered table-sm table-hover"
                EmptyDataText="No materials added yet."
                OnRowCommand="gvMaterials_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" />
                    <asp:BoundField DataField="Title" HeaderText="Title" />
                    <asp:BoundField DataField="MaterialType" HeaderText="Type" />
                    <asp:BoundField DataField="CourseName" HeaderText="Course" />
                    <asp:BoundField DataField="Category" HeaderText="Category" />
                    <asp:BoundField DataField="DisplayOrder" HeaderText="Order" />
                    <asp:TemplateField HeaderText="Active">
                        <ItemTemplate>
                            <span class='<%# Convert.ToBoolean(Eval("IsActive")) ? "badge bg-success" : "badge bg-secondary" %>'>
                                <%# Convert.ToBoolean(Eval("IsActive")) ? "Yes" : "No" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="View">
                        <ItemTemplate>
                            <a href='<%# GetViewUrl(Eval("FilePath"), Eval("VideoUrl")) %>' target="_blank" class="btn btn-sm btn-outline-secondary">Open</a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CssClass="btn btn-primary btn-sm" CommandName="EditMaterial" CommandArgument='<%# Eval("ID") %>'>Edit</asp:LinkButton>
                            <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm" CommandName="DeleteMaterial" CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('Delete this material?');">Delete</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>