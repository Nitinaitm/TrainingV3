<%@ Page Title="Learning Resource Library" Language="C#" AutoEventWireup="true" CodeBehind="LMSBrowse.aspx.cs" Inherits="Training.LMS.LMSBrowse" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Learning Resource Library</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
    <style>
        body {
            background: #f4f7fb;
        }

        .lms-header {
            background: linear-gradient(135deg, #2563eb 0%, #4f46e5 48%, #7c3aed 100%);
            color: #fff;
            padding: 24px 0;
            margin-bottom: 24px;
        }

        .material-card {
            border: 1px solid #e2e8f0;
            border-radius: 12px;
            background: #fff;
            padding: 18px;
            height: 100%;
            transition: all .2s ease;
        }

        .material-card:hover {
            box-shadow: 0 8px 20px rgba(15,23,42,.08);
            transform: translateY(-2px);
        }

        .material-type-badge {
            font-size: 11px;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: .5px;
        }

        .material-title {
            font-weight: 700;
            font-size: 16px;
            margin: 10px 0 4px;
            color: #1e293b;
        }

        .material-meta {
            font-size: 12px;
            color: #64748b;
            margin-bottom: 8px;
        }

        .material-desc {
            font-size: 13px;
            color: #475569;
            margin-bottom: 12px;
        }

        .filter-card {
            border-radius: 12px;
            margin-bottom: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div class="lms-header">
            <div class="container d-flex justify-content-between align-items-center">
                <div>
                    <h3 class="mb-0">Learning Resource Library</h3>
                    <div class="small" style="opacity:.85;">Manuals, PPTs, documents and videos for trainees, trainers and staff</div>
                </div>
                <asp:HyperLink ID="lnkDashboard" runat="server" CssClass="btn btn-light btn-sm">Back to Dashboard</asp:HyperLink>
            </div>
        </div>

        <div class="container">

            <div class="card filter-card">
                <div class="card-body">
                    <div class="row g-2">
                        <div class="col-md-3">
                            <label class="small text-muted">Type</label>
                            <asp:DropDownList ID="ddlFilterType" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                                <asp:ListItem Text="All Types" Value=""></asp:ListItem>
                                <asp:ListItem Text="Manual" Value="Manual"></asp:ListItem>
                                <asp:ListItem Text="PPT" Value="PPT"></asp:ListItem>
                                <asp:ListItem Text="Document" Value="Document"></asp:ListItem>
                                <asp:ListItem Text="Video" Value="Video"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <label class="small text-muted">Category</label>
                            <asp:DropDownList ID="ddlFilterCategory" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <label class="small text-muted">Course</label>
                            <asp:DropDownList ID="ddlFilterCourse" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-3">
                            <label class="small text-muted">Search</label>
                            <div class="input-group">
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Title or description" />
                                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="Filter_Changed" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <asp:Repeater ID="rptMaterials" runat="server">
                <HeaderTemplate>
                    <div class="row g-3">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="col-md-4">
                        <div class="material-card">
                            <span class="badge bg-primary material-type-badge"><%# Eval("MaterialType") %></span>
                            <div class="material-title"><%# Eval("Title") %></div>
                            <div class="material-meta">
                                <%# string.IsNullOrEmpty(Eval("CourseName").ToString()) ? "" : "Course: " + Eval("CourseName") + " | " %>
                                <%# string.IsNullOrEmpty(Eval("Category").ToString()) ? "" : "Category: " + Eval("Category") %>
                            </div>
                            <div class="material-desc"><%# Eval("Description") %></div>
                            <asp:Literal ID="litOpenButton" runat="server" Text='<%# GetOpenButtonHtml(Eval("MaterialType"), Eval("FilePath"), Eval("VideoUrl")) %>' />
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>

            <asp:Label ID="lblEmpty" runat="server" CssClass="d-block text-center text-muted my-5" Visible="false" Text="No materials found matching your filters." />

        </div>
                <div class="modal fade" id="videoModal" tabindex="-1">
            <div class="modal-dialog modal-xl modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Video Player</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body p-0 text-center">
                        <div id="videoContainer"></div>
                    </div>
                </div>
            </div>
        </div>

                <script>
                    function playVideo(el) {
                        var embedHtml = el.getAttribute('data-embed');
                        document.getElementById('videoContainer').innerHTML = embedHtml;
                        var modal = new bootstrap.Modal(document.getElementById('videoModal'));
                        modal.show();
                    }

                    document.getElementById('videoModal').addEventListener('hidden.bs.modal', function () {
                        document.getElementById('videoContainer').innerHTML = '';
                    });
                </script>

    </form>
    
</body>
</html>