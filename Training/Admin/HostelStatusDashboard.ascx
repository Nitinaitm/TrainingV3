<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HostelStatusDashboard.ascx.cs" Inherits="Training.Admin.HostelStatusDashboard" %>

<div class="card mb-4">
    <div class="card-header bg-primary text-white">
        <b>Hostel Status</b> - Vacant Seats Overview
    </div>
    <div class="card-body p-2">
        <asp:GridView
            ID="gvHostelStatus"
            runat="server"
            AutoGenerateColumns="false"
            CssClass="table table-bordered table-sm mb-0"
            EmptyDataText="No active hostels found.">
            <Columns>
                <asp:BoundField DataField="HostelName" HeaderText="Hostel" />
                <asp:BoundField DataField="BlockName" HeaderText="Block" />
                <asp:BoundField DataField="TotalBeds" HeaderText="Total Beds" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="OccupiedBeds" HeaderText="Occupied" ItemStyle-HorizontalAlign="Center" />
                <asp:TemplateField HeaderText="Vacant" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <span class='<%# Convert.ToInt32(Eval("VacantBeds")) > 0 ? "badge bg-success" : "badge bg-danger" %>'>
                            <%# Eval("VacantBeds") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

        </asp:GridView>
    </div>
</div>