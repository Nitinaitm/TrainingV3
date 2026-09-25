<%@ Page Title="External Trainee Report" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ExternalTraineeReport.aspx.cs" Inherits="Training.Admin.ExternalTraineeReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .etr-container{width:100%;padding:20px}
        .etr-card{background:#fff;border-radius:12px;padding:25px;margin-bottom:20px;box-shadow:0 2px 12px rgba(0,0,0,.08)}
        .etr-title{font-size:26px;font-weight:700;color:#1e293b;margin-bottom:20px}
        .etr-grid{display:grid;grid-template-columns:repeat(4,1fr);gap:16px}
        .etr-group label{display:block;font-weight:600;color:#334155;font-size:14px;margin-bottom:7px}
        .etr-input{width:100%;height:40px;padding:8px 11px;border:1px solid #cbd5e1;border-radius:6px}
        .etr-buttons{margin-top:20px;display:flex;gap:10px;flex-wrap:wrap}
        .etr-btn{border:0;border-radius:6px;padding:9px 20px;color:#fff;font-weight:600;cursor:pointer}
        .etr-search{background:#2563eb}.etr-reset{background:#64748b}.etr-export{background:#198754}
        .etr-scroll{width:100%;overflow-x:auto}
        .etr-gridview{width:max-content;min-width:100%;border-collapse:collapse}
        .etr-gridview th{padding:11px 12px;background:#2563eb;color:#fff;white-space:nowrap;font-size:13px}
        .etr-gridview td{padding:10px 12px;border-bottom:1px solid #e2e8f0;white-space:nowrap;font-size:13px;color:#334155}
        .etr-gridview tr:nth-child(even){background:#f8fafc}
        @media(max-width:900px){.etr-grid{grid-template-columns:repeat(2,1fr)}}
        @media(max-width:576px){.etr-container{padding:10px}.etr-card{padding:15px}.etr-grid{grid-template-columns:1fr}}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="etr-container">
        <div class="etr-card">
            <div class="etr-title"><i class="fas fa-user-friends"></i> External Trainee Report</div>
            <div class="etr-grid">
                <div class="etr-group">
                    <label>Employee ID</label>
                    <asp:TextBox ID="txtEmpID" runat="server" CssClass="etr-input" placeholder="Enter Employee ID"></asp:TextBox>
                </div>
                <div class="etr-group">
                    <label>Name</label>
                    <asp:TextBox ID="txtEmpName" runat="server" CssClass="etr-input" placeholder="Enter Name"></asp:TextBox>
                </div>
                <div class="etr-group">
                    <label>Organization / Company</label>
                    <asp:TextBox ID="txtCompany" runat="server" CssClass="etr-input" placeholder="Enter Organization"></asp:TextBox>
                </div>
                <div class="etr-group">
                    <label>Designation</label>
                    <asp:TextBox ID="txtDesignation" runat="server" CssClass="etr-input" placeholder="Enter Designation"></asp:TextBox>
                </div>
            </div>
            <div class="etr-buttons">
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="etr-btn etr-search" OnClick="btnSearch_Click" />
                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="etr-btn etr-reset" OnClick="btnReset_Click" />
            </div>
        </div>

        <div class="etr-card">
            <div class="etr-buttons" style="margin-top:0;margin-bottom:15px">
                <asp:Button ID="btnExportExcel" runat="server" Text="Download Excel" CssClass="etr-btn etr-export" OnClick="btnExportExcel_Click" />
            </div>
            <div class="etr-scroll">
                <asp:GridView ID="gvExternalTrainee" runat="server" AutoGenerateColumns="False" CssClass="etr-gridview" GridLines="None" EmptyDataText="No External Trainee Found" OnRowCommand="gvExternalTrainee_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="Sl No">
                            <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="EmpID" HeaderText="Employee ID" />
                        <asp:BoundField DataField="EmpName" HeaderText="Name" />
                        <asp:BoundField DataField="MobileNo" HeaderText="Mobile No" />
                        <asp:BoundField DataField="EmailId" HeaderText="Email ID" />
                        <asp:BoundField DataField="EmpCompany" HeaderText="Organization / Company" />
                        <asp:BoundField DataField="EmpDesignation" HeaderText="Designation" />
                        <asp:BoundField DataField="CreatedOn" HeaderText="Created On" DataFormatString="{0:dd-MM-yyyy}" />
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnEditExternal" runat="server" Text="Edit" CommandName="EditExternal" CommandArgument='<%# Eval("EmpID") %>' CssClass="etr-btn etr-search" Style="padding:6px 12px;text-decoration:none;display:inline-block;" />
                                <asp:LinkButton ID="btnDeleteExternal" runat="server" Text="Delete" CommandName="DeleteExternal" CommandArgument='<%# Eval("EmpID") %>' CssClass="etr-btn" Style="padding:6px 12px;background:#dc3545;text-decoration:none;display:inline-block;margin-left:6px;" OnClientClick="return confirm('Are you sure you want to delete this external trainee?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>