<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="HomePageManagement.aspx.cs" Inherits="Training.Admin.HomePageManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
    <style>
        .home-mgmt-tile {
            height: 100%;
        }
        .home-mgmt-tile .stat-value {
            font-size: 1.4rem;
            font-weight: 700;
            color: #198754;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4 class="mb-3">Home Page Management</h4>
    <p class="text-muted mb-4">Manage the content shown on the public homepage (Home.aspx) from here.</p>

    <div class="row g-3">

        <div class="col-md-4">
            <div class="card home-mgmt-tile">
                <div class="card-body">
                    <h5><i class="fa-solid fa-user-tie me-2"></i>Director General's Message</h5>
                    <p class="text-muted small mb-2">Photo, quote, and full message.</p>
                    <div class="stat-value">Last updated: <asp:Label ID="lblDgLastUpdated" runat="server" /></div>
                    <a href="HomeDGMessage.aspx" class="btn btn-primary btn-sm mt-2">Manage</a>
                </div>
            </div>
        </div>

        <div class="col-md-4">
            <div class="card home-mgmt-tile">
                <div class="card-body">
                    <h5><i class="fa-solid fa-users me-2"></i>Honorable Members &amp; Leadership</h5>
                    <p class="text-muted small mb-2">Profile cards shown on the homepage.</p>
                    <div class="stat-value"><asp:Label ID="lblLeadershipCount" runat="server" /> members</div>
                    <a href="HomeLeadership.aspx" class="btn btn-primary btn-sm mt-2">Manage</a>
                </div>
            </div>
        </div>

        <div class="col-md-4">
            <div class="card home-mgmt-tile">
                <div class="card-body">
                    <h5><i class="fa-solid fa-comment-dots me-2"></i>What Our Trainees Say</h5>
                    <p class="text-muted small mb-2">Testimonial quotes and names.</p>
                    <div class="stat-value"><asp:Label ID="lblTestimonialCount" runat="server" /> testimonials</div>
                    <a href="HomeTestimonials.aspx" class="btn btn-primary btn-sm mt-2">Manage</a>
                </div>
            </div>
        </div>

        <div class="col-md-4">
            <div class="card home-mgmt-tile">
                <div class="card-body">
                    <h5><i class="fa-solid fa-clipboard-list me-2"></i>Online Nominations</h5>
                    <p class="text-muted small mb-2">Nominations submitted through the public form.</p>
                    <div class="stat-value"><asp:Label ID="lblPendingNominations" runat="server" /> pending</div>
                    <a href="AdminOnlineNominations.aspx" class="btn btn-primary btn-sm mt-2">Review</a>                
                </div>
            </div>
        </div>

         <div class="col-md-4">
            <div class="card home-mgmt-tile">
                <div class="card-body">
                    <h5><i class="fa-solid fa-clipboard-list me-2"></i>Announcements</h5>
                    <p class="text-muted small mb-2">Announcements submitted through the Admin.</p>
                    <div class="stat-value"><asp:Label ID="Label1" runat="server" /> pending</div>
                    <a href="HomeAnnouncements.aspx" class="btn btn-primary btn-sm mt-2">Review</a>
                </div>
            </div>
        </div>

        <div class="col-md-4">
            <div class="card home-mgmt-tile">
                <div class="card-body">
                    <h5><i class="fa-solid fa-clipboard-list me-2"></i>Scrolling Notice Marquee</h5>
                    <p class="text-muted small mb-2">Scrolling Notice in Marquee submitted through the Admin.</p>
                    <div class="stat-value"><asp:Label ID="Label2" runat="server" /> pending</div>
                    <a href="HomeMarquee.aspx" class="btn btn-primary btn-sm mt-2">Review</a>
                </div>
            </div>
        </div>

        <div class="col-md-4">
            <div class="card home-mgmt-tile">
                <div class="card-body">
                    <h5><i class="fa-solid fa-images me-2"></i>Media Gallery</h5>
                    <p class="text-muted small mb-2">Photos and videos shown on the homepage.</p>
                    <div class="stat-value">
                        <asp:Label ID="lblGalleryCount" runat="server" />
                        items</div>
                    <a href="HomeGallery.aspx" class="btn btn-primary btn-sm mt-2">Manage</a>
                </div>
            </div>
        </div>

        <div class="col-md-4">
            <div class="card home-mgmt-tile">
                <div class="card-body">
                    <h5><i class="fa-solid fa-blog me-2"></i>Blogs &amp; Knowledge Repository</h5>
                    <p class="text-muted small mb-2">Articles shown on the homepage.</p>
                    <div class="stat-value">
                        <asp:Label ID="lblPendingBlogs" runat="server" />
                        pending</div>
                    <a href="HomeBlogs.aspx" class="btn btn-primary btn-sm mt-2">Manage</a>
                </div>
            </div>
        </div>

    </div>

</asp:Content>