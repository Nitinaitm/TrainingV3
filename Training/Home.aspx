<%@ Page Title="Bihar Power Training Management Society" Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Training.Home" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Bihar Power Training Management Society</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />
    <link href="https://fonts.googleapis.com/css2?family=Merriweather:wght@700;900&family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet" />

    <!--<link href="css/HomePortal.css" rel="stylesheet" />-->
    <link href="css/HomePortal.css?v=2" rel="stylesheet" />
</head>
<body>
    <form id="frmHome" runat="server">

        <a href="#mainContent" class="skip-link">Skip to main content</a>

        <!-- ================= ACCESSIBILITY BAR ================= -->
        <div class="access-bar">
            <div class="container d-flex flex-wrap justify-content-between align-items-center py-1">
                <div class="access-left d-flex align-items-center flex-wrap">
                    <span class="access-item"><i class="fa-solid fa-universal-access me-1"></i>Screen Reader Access</span>
                    <span class="access-item font-adjust">
                        Text Size:
                        <a href="javascript:void(0);" onclick="adjustFont(-1);" title="Decrease font size">A-</a>
                        <a href="javascript:void(0);" onclick="adjustFont(0);" title="Reset font size">A</a>
                        <a href="javascript:void(0);" onclick="adjustFont(1);" title="Increase font size">A+</a>
                    </span>
                    <span class="access-item">
                        <select id="ddlLanguage" class="lang-select" aria-label="Select language">
                            <option value="en">English</option>
                            <option value="hi">हिन्दी</option>
                        </select>
                    </span>
                </div>
                <div class="access-right d-flex align-items-center">
                    <a href="#footerQuickLinks" class="access-item">Quick Links</a>
                    <a href="Default.aspx" class="btn-portal-cta">
                        <i class="fa-solid fa-right-to-bracket me-1"></i>Training Portal (BPTMS)
                    </a>
                </div>
            </div>
        </div>

        <!-- ================= HEADER ================= -->
        <header class="site-header">
            <div class="container d-flex flex-wrap align-items-center justify-content-between py-3">
                <div class="brand d-flex align-items-center">
                    <img src="img/emblem.png" alt="Government of Bihar Emblem" class="brand-emblem" onerror="this.style.display='none';" />
                    <div class="brand-text">
                        <div class="brand-title">Bihar Power Training Management Society  </div>
                        <div class="brand-subtitle">Registered under Society Registration Act-21, 1860 &nbsp;|&nbsp; BSPHCL</div>
                    </div>
                </div>
                <div class="brand-search">
                    <asp:TextBox ID="txtSiteSearch" runat="server" CssClass="form-control" placeholder="Search the site..." />
                    <asp:Button ID="btnSiteSearch" runat="server" CssClass="btn btn-search" Text="Search" OnClick="btnSiteSearch_Click" />
                </div>
            </div>

            <nav class="navbar navbar-expand-lg main-nav">
                <div class="container">
                    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#mainNavCollapse">
                        <span class="navbar-toggler-icon"></span>
                    </button>
                    <div class="collapse navbar-collapse" id="mainNavCollapse">
                        <ul class="navbar-nav">
                            <li class="nav-item"><a class="nav-link active" href="#mainContent">Home</a></li>
                            <li class="nav-item"><a class="nav-link" href="#aboutSection">About Us</a></li>
                            <li class="nav-item"><a class="nav-link" href="#leadershipSection">Honorable Members</a></li>
                            <li class="nav-item"><a class="nav-link" href="#trainingHubSection">Training</a></li>
                            <li class="nav-item"><a class="nav-link" href="#mediaSection">Media</a></li>
                            <li class="nav-item"><a class="nav-link" href="#noticesSection">Notices</a></li>
                            <li class="nav-item"><a class="nav-link" href="#footerSection">Contact</a></li>
                        </ul>
                    </div>
                </div>
            </nav>
        </header>

        <main id="mainContent">

            <!-- ================= HERO ================= -->
            <section class="hero-section">
                <div class="container">
                    <div class="row align-items-center gy-4">
                        <div class="col-lg-6">
                            <p class="hero-eyebrow">Empowering Bihar's Energy Workforce</p>
                            <h1 class="hero-title">Building Skilled Hands<br />for a Powered Bihar</h1>
                            <p class="hero-subtext">
                                BPTMS delivers structured technical and management training to power-sector
                                officers and engineers across the state - from classroom sessions to
                                on-site line construction and safety practicals.
                            </p>
                            <div class="hero-actions">
                                <a href="#trainingHubSection" class="btn btn-gold-lg">
                                    <i class="fa-solid fa-calendar-check me-2"></i>View Training Calendar
                                </a>
                            <a href="Default.aspx" class="btn btn-outline-navy-lg">
                                Apply for Nomination
                                </a>
                            </div>
                        </div>
                        <div class="col-lg-6">
                            <div class="hero-visual">
                                <div class="hero-stat-card">
                                    <i class="fa-solid fa-bolt"></i>
                                    <div>
                                        <div class="stat-number">200+</div>
                                        <div class="stat-label">Trainings Published</div>
                                    </div>
                                </div>
                                <%--<div class="hero-illustration">
                                    <i class="fa-solid fa-landmark-dome"></i>
                                </div>--%>

                                <div class="hero-illustration">
                                     
                                    <img src="img/bpti_1.jpg" alt="Hostel Building" class="img-fluid" onerror="this.style.display='none';" />
                                </div>

    <div class="hero-stat-card hero-stat-card-alt">
                                    <i class="fa-solid fa-users"></i>
                                    <div>
                                        <div class="stat-number">1500+</div>
                                        <div class="stat-label">Officers Trained</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- ================= CAROUSEL + DG MESSAGE ================= -->
            <section class="section" id="aboutSection">
                <div class="container">
                    <div class="row g-4">
                        <div class="col-lg-8">
                            <div id="campusCarousel" class="carousel slide campus-carousel" data-bs-ride="carousel">
                                <div class="carousel-indicators">
                                    <asp:Repeater ID="rptCarouselIndicators" runat="server">
                                        <ItemTemplate>
                                            <button type="button" data-bs-target="#campusCarousel" data-bs-slide-to='<%# Container.ItemIndex %>' class='<%# Container.ItemIndex == 0 ? "active" : "" %>'></button>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                                <div class="carousel-inner">
                                    <asp:Repeater ID="rptCarousel" runat="server">
                                        <ItemTemplate>
                                            <div class='<%# "carousel-item" + (Container.ItemIndex == 0 ? " active" : "") %>'>
                                                <div class="carousel-slide-photo">
                                                    <img src='<%# Eval("ThumbnailPath") %>' alt='<%# Eval("Caption") %>' onerror="this.parentNode.classList.add('carousel-slide-photo-fallback');" />
                                                    <span class="carousel-caption-text"><%# Eval("Caption") %></span>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                                <button class="carousel-control-prev" type="button" data-bs-target="#campusCarousel" data-bs-slide="prev">
                                    <span class="carousel-control-prev-icon"></span>
                                </button>
                                <button class="carousel-control-next" type="button" data-bs-target="#campusCarousel" data-bs-slide="next">
                                    <span class="carousel-control-next-icon"></span>
                                </button>
                            </div>
                      </div>
                        <div class="col-lg-4">
                        <div class="dg-message-card">
                            <img id="imgDgPhoto" runat="server" alt="Director General" class="dg-photo" onerror="this.src='';this.classList.add('dg-photo-fallback');" />
                            <div class="dg-name">
                                <asp:Label ID="lblDgName" runat="server" />
                            </div>
                            <div class="dg-title">
                                <asp:Label ID="lblDgDesignation" runat="server" />
                            </div>
                            <p class="dg-quote">
                                &ldquo;<asp:Label ID="lblDgQuote" runat="server" />&rdquo;
                            </p>
                            <button type="button" class="btn btn-outline-navy w-100" data-bs-toggle="modal" data-bs-target="#dgMessageModal">
                                Read Full Message
                            </button>
                            </div>
                        </div>
                        </div>
                    </div>
               
                
            </section>

            <!-- ================= LEADERSHIP ================= -->
            <section class="section section-alt" id="leadershipSection">
                <div class="container">
                    <h2 class="section-heading">Honorable Members &amp; Leadership</h2>
                    <p class="section-subheading">Guiding the Society's training mission and governance.</p>

                    <div class="row g-4">
                        <asp:Repeater ID="rptLeadership" runat="server">
                            <ItemTemplate>
                                <div class="col-md-3 col-sm-6">
                                    <div class="member-card">
                                        <img src='<%# Eval("PhotoUrl") %>' alt='<%# Eval("Name") %>' class="member-photo" onerror="this.classList.add('member-photo-fallback');" />
                                        <div class="member-name"><%# Eval("Name") %></div>
                                        <div class="member-designation"><%# Eval("Designation") %></div>
                                        <asp:HyperLink runat="server" CssClass="member-link" NavigateUrl='<%# Eval("ProfileUrl") %>'>View Profile <i class="fa-solid fa-arrow-right"></i></asp:HyperLink>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </section>

            <!-- ================= TRAINING HUB ================= -->
            <section class="section" id="trainingHubSection">
                <div class="container">
                    <h2 class="section-heading">Training Hub</h2>
                    <p class="section-subheading">Upcoming courses, ongoing programs, and how to apply.</p>

                    <ul class="nav nav-pills hub-tabs" id="hubTabs" role="tablist">
                        <li class="nav-item" role="presentation">
                            <button class="nav-link active" data-bs-toggle="pill" data-bs-target="#tabCalendar" type="button">Training Calendar</button>
                        </li>
                        <li class="nav-item" role="presentation">
                            <button class="nav-link" data-bs-toggle="pill" data-bs-target="#tabActivities" type="button">Training Activities</button>
                        </li>
                    </ul>

                    <div class="tab-content hub-tab-content">

                        <div class="tab-pane fade show active" id="tabCalendar">
                            <div class="table-responsive">
                                <table class="table calendar-table-hub align-middle">
                                    <thead>
                                        <tr>
                                            <th>Course</th>
                                            <th>Target Audience</th>
                                            <th>Start Date</th>
                                            <th>End Date</th>
                                            <th>Location</th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptTrainingCalendar" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><strong><%# Eval("CourseName") %></strong><div class="small-muted"><%# Eval("TrainingType") %></div></td>
                                                    <td><%# Eval("TrainingCategory") %></td>
                                                    <td><%# Eval("DateFrom") %></td>
                                                    <td><%# Eval("DateTo") %></td>
                                                    <td><%# Eval("TrainingLocation") %></td>
                                                    <td>
                                                        <asp:HyperLink runat="server" CssClass="btn btn-gold-sm" NavigateUrl="Default.aspx">Apply</asp:HyperLink>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                                <asp:Label ID="lblNoTrainings" runat="server" CssClass="text-muted d-block text-center py-3" Text="No upcoming trainings published at the moment." Visible="false" />
                            </div>
                        </div>

                        <div class="tab-pane fade" id="tabActivities">
                            <div class="row g-4">
                                <asp:Repeater ID="rptTrainingActivities" runat="server">
                                    <ItemTemplate>
                                        <div class="col-md-4">
                                            <div class="activity-card">
                                                <i class='<%# Eval("IconClass") %>'></i>
                                                <div class="activity-title"><%# Eval("Title") %></div>
                                                <p class="activity-desc"><%# Eval("Description") %></p>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                    </div>

                    <div class="nomination-cta">
                        <div class="row align-items-center g-3">
                            <div class="col-md-8">
                                <h3>Online Nomination Now Open</h3>
                                <p>Nominate officers for the Calendar Training Program directly through the portal.</p>
                            </div>
                            <div class="col-md-4 text-md-end">
                                <asp:HyperLink runat="server" CssClass="btn btn-gold-lg" NavigateUrl="Default.aspx">
                                    Nominate Now <i class="fa-solid fa-arrow-right ms-2"></i>
                                </asp:HyperLink>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- ================= NEWS / NOTICES ================= -->
            <section class="section section-alt" id="noticesSection">
                <div class="container">
                    <h2 class="section-heading">Updates, News &amp; Announcements</h2>

                    <div class="notice-marquee-wrap">
                        <i class="fa-solid fa-bullhorn"></i>
                        <div class="notice-marquee" id="noticeMarquee">
                            <asp:Repeater ID="rptMarquee" runat="server">
                                <ItemTemplate>
                                    <span class="marquee-item"><%# Eval("Text") %></span>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <div class="row g-4 mt-2">
                        <div class="col-md-6">
                            <div class="news-panel">
                                <h4><i class="fa-solid fa-newspaper me-2"></i>Latest News &amp; Tenders</h4>
                                <ul class="news-list">
                                    <asp:Repeater ID="rptNews" runat="server">
                                        <ItemTemplate>
                                            <li>
                                                <span class="news-date"><%# Eval("Date") %></span>
                                                <span class="news-text"><%# Eval("Title") %></span>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="news-panel">
                                <h4><i class="fa-solid fa-file-circle-check me-2"></i>Official Circulars</h4>
                                <ul class="news-list">
                                    <asp:Repeater ID="rptCirculars" runat="server">
                                        <ItemTemplate>
                                            <li>
                                                <span class="news-date"><%# Eval("Date") %></span>
                                                <span class="news-text"><%# Eval("Title") %></span>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- ================= MEDIA GALLERY & BLOGS ================= -->
            <section class="section" id="mediaSection">
                <div class="container">
                    <h2 class="section-heading">Media Gallery</h2>

                    <div class="gallery-filters">
                        <button class="filter-btn active" data-filter="all">All</button>
                        <button class="filter-btn" data-filter="photo">Photos</button>
                        <button class="filter-btn" data-filter="video">Videos</button>
                    </div>

                    <div class="row g-3 gallery-grid" id="galleryGrid">
                        <asp:Repeater ID="rptGallery" runat="server">
                            <ItemTemplate>
                                <div class='col-md-3 col-sm-6 gallery-item' data-type='<%# Eval("MediaType") %>'>
                                    <div class="gallery-tile">
                                        <img src='<%# Eval("ThumbnailUrl") %>' alt='<%# Eval("Caption") %>' onerror="this.parentNode.classList.add('gallery-tile-fallback');" />
                                        <span class="gallery-caption"><%# Eval("Caption") %></span>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <h2 class="section-heading mt-5">Blogs &amp; Knowledge Repository</h2>
                    <div class="row g-4">
                        <asp:Repeater ID="rptBlogs" runat="server">
                            <ItemTemplate>
                                <div class="col-md-4">
                                    <div class="blog-card">
                                        <div class="blog-tag"><%# Eval("Category") %></div>
                                        <div class="blog-title"><%# Eval("Title") %></div>
                                        <p class="blog-excerpt"><%# Eval("Excerpt") %></p>
                                        <asp:HyperLink runat="server" CssClass="blog-readmore" NavigateUrl='<%# Eval("Url") %>'>Read More <i class="fa-solid fa-arrow-right"></i></asp:HyperLink>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </section>

            <!-- ================= IT SERVICES & EXTERNAL LINKS ================= -->
            <section class="section section-alt">
                <div class="container">
                    <h2 class="section-heading">IT Services</h2>
                    <div class="row g-3 text-center">
                        <div class="col-md-3 col-6">
                            <a href="Default.aspx" class="it-service-tile">
                                <i class="fa-solid fa-envelope-open-text"></i><span>e-Office</span>
                            </a>
                        </div>
                        <div class="col-md-3 col-6">
                            <a href="Default.aspx" class="it-service-tile">
                                <i class="fa-solid fa-id-badge"></i><span>Employee Self-Service</span>
                            </a>
                        </div>
                        <div class="col-md-3 col-6">
                            <a href="Default.aspx" class="it-service-tile">
                                <i class="fa-solid fa-graduation-cap"></i><span>LMS</span>
                            </a>
                        </div>
                        <div class="col-md-3 col-6">
                            <a href="Default.aspx" class="it-service-tile">
                                <i class="fa-solid fa-certificate"></i><span>Certificate Verification</span>
                            </a>
                        </div>
                    </div>

                    <h2 class="section-heading mt-5">External Links</h2>
                    <div class="row g-3 align-items-center text-center external-links-row">
                        <div class="col-md-3 col-6"><div class="ext-link-tile">DoPT</div></div>
                        <div class="col-md-3 col-6"><div class="ext-link-tile">Bihar State Portal</div></div>
                        <div class="col-md-3 col-6"><div class="ext-link-tile">Digital India</div></div>
                        <div class="col-md-3 col-6"><div class="ext-link-tile">Dept. of Energy, Bihar</div></div>
                    </div>
                </div>
            </section>

            <!-- ================= SOCIAL MEDIA ================= -->
            <section class="section">
                <div class="container">
                    <h2 class="section-heading">Connect With Us</h2>
                    <div class="row g-4">
                        <div class="col-md-4">
                            <div class="social-card social-x">
                                <i class="fa-brands fa-x-twitter"></i>
                                <h5>Latest on X</h5>
                                <p>Follow @Bihar State Power Holding Company Limited for real-time updates.</p>
                                <a href="#" target="_blank">Visit Page <i class="fa-solid fa-arrow-up-right-from-square"></i></a>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="social-card social-yt">
                                <i class="fa-brands fa-youtube"></i>
                                <h5>Video Highlights</h5>
                                <p>Watch training sessions and campus events.</p>
                                <a href="#" target="_blank">Visit Channel <i class="fa-solid fa-arrow-up-right-from-square"></i></a>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="social-card social-fb">
                                <i class="fa-brands fa-facebook"></i>
                                <h5>Facebook Updates</h5>
                                <p>Announcements, photos, and community posts.</p>
                                <a href="https://www.facebook.com/BiharEnergy" target="_blank">Visit Page <i class="fa-solid fa-arrow-up-right-from-square"></i></a>
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <!-- ================= TESTIMONIALS ================= -->
            <section class="section section-alt">
                <div class="container">
                    <h2 class="section-heading">What Our Trainees Say</h2>

                    <div id="testimonialCarousel" class="carousel slide" data-bs-ride="carousel">
                        <div class="carousel-inner">
                            <asp:Repeater ID="rptTestimonials" runat="server" OnItemDataBound="rptTestimonials_ItemDataBound">
                                <ItemTemplate>
                                    <div class='carousel-item testimonial-item' runat="server" id="testimonialItem">
                                        <div class="testimonial-card mx-auto">
                                            <i class="fa-solid fa-quote-left"></i>
                                            <p><%# Eval("Quote") %></p>
                                            <div class="testimonial-name"><%# Eval("Name") %></div>
                                            <div class="testimonial-role"><%# Eval("Role") %></div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <button class="carousel-control-prev" type="button" data-bs-target="#testimonialCarousel" data-bs-slide="prev">
                            <span class="carousel-control-prev-icon" style="filter:invert(1);"></span>
                        </button>
                        <button class="carousel-control-next" type="button" data-bs-target="#testimonialCarousel" data-bs-slide="next">
                            <span class="carousel-control-next-icon" style="filter:invert(1);"></span>
                        </button>
                    </div>

    <div class="text-center mt-4">
        <a href="Default.aspx" class="btn btn-outline-navy">Share Your Feedback</a>
    </div>
    </div>
    </section>

        </main>

        <!-- ================= FOOTER ================= -->
        <footer class="site-footer" id="footerSection">
            <div class="container">
                <div class="row g-4">
                    <div class="col-md-3">
                        <h5>BPTMS</h5>
                        <p class="footer-about">Bihar Power Training Management Society, Registered under Society Registration Act-21, 1860
(Registration No-S000722 Year 2017-18) निबंधित कार्यालय : बिहार पावर ट्रेनिंग इन्स्टीच्यूट, बी0एस0पी0टी0सी0एल0, गौरीचक, ग्रिड सब स्टेशन, सम्पतचक, पटना-803201
GST No. 10AAPAB6368N1ZB Email. ID-bptms.bsphcl@gmail.com
.</p>
                    </div>
                    <div class="col-md-3" id="footerQuickLinks">
                        <h5>Quick Links</h5>
                        <ul class="footer-links">
                            <li><a href="#aboutSection">About Us</a></li>
                            <li><a href="#trainingHubSection">Training Calendar</a></li>
                            <li><a href="Default.aspx">Online Nomination</a></li>
                            <li><a href="Default.aspx">Training Portal Login</a></li>
                        </ul>
                    </div>
                    <div class="col-md-3">
                        <h5>Compliance</h5>
                        <ul class="footer-links">
                            <li><a href="#">Terms of Service</a></li>
                            <li><a href="#">Privacy Policy</a></li>
                            <li><a href="#">Accessibility Statement</a></li>
                        </ul>
                    </div>
                    <div class="col-md-3">
                        <h5>Helpdesk</h5>
                        <ul class="footer-links">
                            <li><i class="fa-solid fa-phone me-2"></i>0612-XXXXXXX</li>
                            <li><i class="fa-solid fa-envelope me-2"></i>helpdesk@bptms.bihar.gov.in</li>
                        </ul>
                    </div>
                </div>

                <hr class="footer-rule" />

                <div class="d-flex flex-wrap justify-content-between align-items-center footer-bottom">
                    <span>&copy; <%= DateTime.Now.Year %> Bihar Power Training Management Society. All Rights Reserved.</span>
                    <span>
                        <i class="fa-solid fa-eye me-1"></i>Visitors: <asp:Label ID="lblVisitorCount" runat="server" />
                        &nbsp;|&nbsp; Last Updated: <asp:Label ID="lblLastUpdated" runat="server" />
                    </span>
                </div>
            </div>
        </footer>

        <!-- DG Message Modal -->
        <div class="modal fade" id="dgMessageModal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Director General's Message</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="lblDgFullMessage" runat="server" />
                    </div>
                </div>
            </div>
        </div>

    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
    <script>
        function adjustFont(step) {
            var body = document.body;
            var current = parseFloat(getComputedStyle(body).getPropertyValue('--font-scale')) || 1;
            var next = step === 0 ? 1 : Math.min(1.3, Math.max(0.85, current + (step * 0.1)));
            body.style.setProperty('--font-scale', next);
        }

        (function () {
            var wrap = document.querySelector('.notice-marquee-wrap');
            var track = document.getElementById('noticeMarquee');
            if (wrap && track) {
                wrap.addEventListener('mouseenter', function () { track.style.animationPlayState = 'paused'; });
                wrap.addEventListener('mouseleave', function () { track.style.animationPlayState = 'running'; });
            }
        })();

        (function () {
            var buttons = document.querySelectorAll('.filter-btn');
            var items = document.querySelectorAll('.gallery-item');
            buttons.forEach(function (btn) {
                btn.addEventListener('click', function () {
                    buttons.forEach(function (b) { b.classList.remove('active'); });
                    btn.classList.add('active');
                    var filter = btn.getAttribute('data-filter');
                    items.forEach(function (item) {
                        var type = item.getAttribute('data-type');
                        item.style.display = (filter === 'all' || filter === type) ? '' : 'none';
                    });
                });
            });
        })();
    </script>
</body>
</html>
