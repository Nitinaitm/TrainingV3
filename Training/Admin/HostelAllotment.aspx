<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="HostelAllotment.aspx.cs"
    Inherits="Training.Admin.HostelAllotment" %>

<%@ Register Src="~/Admin/HostelStatusDashboard.ascx" TagPrefix="uc" TagName="HostelStatusDashboard" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <style>
        .main-card {
            background: #fff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 0 10px #d9d9d9;
            margin-top: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #0d6efd;
            margin-bottom: 20px;
        }

        .gridview th {
            background: #0d6efd;
            color: white;
            text-align: center;
            vertical-align: middle;
        }

        .gridview td {
            vertical-align: middle;
        }

        .row-select {
            min-width: 130px;
        }
    </style>

    <script type="text/javascript">
        // No server round-trip needed here -- this only flips checkbox
        // states already sitting in the browser. The server only reads
        // whichever boxes end up checked when Save / Apply Bulk is clicked.
        function ToggleSelectAllRows(headerCheckbox) {
            //alert("hi");
            var rowCheckboxes = document.querySelectorAll(".rowCheckbox input[type='checkbox']");
            //alert(rowCheckboxes.length);
            for (var i = 0; i < rowCheckboxes.length; i++) {
                rowCheckboxes[i].checked = headerCheckbox.checked;
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">
    <div class="container-fluid">
        <div class="main-card">
            <div class="page-heading">
                Hostel Allotment
            </div>
            

            <uc:HostelStatusDashboard ID="HostelStatusDashboard1" runat="server" />

            <br />       
            <br />
           
    <div class="mb-4">

        <asp:Button ID="btnManualAllot"
            runat="server"
            Text="Manual Allotment"
            CausesValidation="false"
            CssClass="btn btn-primary btn-switch" OnClick="btnManualAllot_Click"
            />

        <asp:Button ID="btnBulkAllot"
            runat="server"
            Text="Bulk Auto Allotment"
            CausesValidation="false"
            CssClass="btn btn-secondary btn-switch ms-md-2" OnClick="btnBulkAllot_Click"
             />

        <asp:Button ID="btnViewAllot"
            runat="server"
            Text="Allotment Summary"
            CausesValidation="false"
            CssClass="btn btn-primary btn-switch ms-md-3" OnClick="btnViewAllot_Click"
            />
    </div>

             

            
     <div>
         <asp:MultiView ID="mvAllotment" runat="server">
             <asp:View ID="vwManual" runat="server">
                 <br /><br />
<%--Replaced this div due to hiding dropdownlist of Training, and fetching training ID from session for Hostel allotment --%>
          <%--  <div class="row">
                <div class="col-md-6 mb-3">
                    <label>Training *</label>
                    <asp:DropDownList
                        ID="ddlTraining"
                        runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        CausesValidation="false"
                        OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
            </div>--%>


                 <asp:Panel ID="pnlTrainingPicker" runat="server">
                     <div class="row">
                         <div class="col-md-6 mb-3">
                             <label>Training *</label>
                             <asp:DropDownList
                                 ID="ddlTraining"
                                 runat="server"
                                 CssClass="form-select"
                                 AutoPostBack="true"
                                 CausesValidation="false"
                                 OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged">
                             </asp:DropDownList>
                         </div>
                     </div>
                 </asp:Panel>

            <asp:Panel ID="pnlTrainingDetails" runat="server" Visible="false" CssClass="card mb-3">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-3"><b>Training ID:</b> <asp:Label ID="lblSelectedTrainingID" runat="server" /></div>
                        <div class="col-md-3"><b>Type:</b> <asp:Label ID="lblSelectedTrainingType" runat="server" /></div>
                        <div class="col-md-3"><b>Batch:</b> <asp:Label ID="lblSelectedBatch" runat="server" /></div>
                        <div class="col-md-3"><b>Dates:</b> <asp:Label ID="lblSelectedDates" runat="server" /></div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-3"><b>Location:</b> <asp:Label ID="lblSelectedLocation" runat="server" /></div>
                        <div class="col-md-3"><b>Organizer:</b> <asp:Label ID="lblSelectedOrganizer" runat="server" /></div>
                    </div>
                </div>
            </asp:Panel>

            <div class="row align-items-end mb-3">
                <div class="col-md-4">
                    <label>Bulk Apply Hostel (to checked rows)</label>
                    <asp:DropDownList
                        ID="ddlBulkHostel"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>
                </div>
                <div class="col-md-3">
                    <asp:Button
                        ID="btnApplyBulk"
                        runat="server"
                        Text="Apply To Checked"
                        CssClass="btn btn-outline-primary"
                        CausesValidation="false"
                        OnClick="btnApplyBulk_Click" />
                </div>
                 <asp:Label ID="lblAutSuccessMessage" runat="server" Font-Bold="true"></asp:Label>
            </div>

            <asp:GridView
                ID="gvParticipants"
                runat="server"
                AutoGenerateColumns="False"
                DataKeyNames="EmpID,Gender"
                CssClass="table table-bordered table-striped gridview"
                OnRowDataBound="gvParticipants_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="S.No">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                                                         
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkSelectAll"  runat="server" onclick="ToggleSelectAllRows(this);" />
                            <asp:Label runat="server"  Text=" All" Font-Bold="true">
                            </asp:Label>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSelect" runat="server" CssClass="rowCheckbox" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Emp ID">
                        <ItemTemplate>
                            <asp:Label
                                ID="lblEmpID"
                                runat="server"
                                Text='<%# Eval("EmpID") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:Label
                                ID="lblEmpName"
                                runat="server"
                                Text='<%# Eval("EmpName") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Gender" HeaderText="Gender" />

                    

                    <asp:BoundField
                        DataField="EmpDesignation"
                        HeaderText="Designation" />

                    <asp:BoundField
                        DataField="EmpCompany"
                        HeaderText="Company" />

                    <asp:TemplateField HeaderText="Hostel">
                        <ItemTemplate>
                            <asp:DropDownList
                                ID="ddlHostelRow"
                                runat="server"
                                CssClass="form-select row-select"
                                AutoPostBack="true"
                                CausesValidation="false"
                                OnSelectedIndexChanged="ddlHostelRow_SelectedIndexChanged">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Block">
                        <ItemTemplate>
                            <asp:DropDownList
                                ID="ddlBlockRow"
                                runat="server"
                                CssClass="form-select row-select"
                                AutoPostBack="true"
                                CausesValidation="false"
                                OnSelectedIndexChanged="ddlBlockRow_SelectedIndexChanged">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Room">
                        <ItemTemplate>
                            <asp:DropDownList
                                ID="ddlRoomRow"
                                runat="server"
                                CssClass="form-select row-select"
                                AutoPostBack="true"
                                CausesValidation="false"
                                OnSelectedIndexChanged="ddlRoomRow_SelectedIndexChanged">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>

                     

                    <asp:TemplateField HeaderText="Bed">
                        <ItemTemplate>
                            <asp:DropDownList
                                ID="ddlBedRow"
                                runat="server"
                                CssClass="form-select row-select">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <div class="mt-3">
                <asp:Button
                    ID="btnSaveAllotment"
                    runat="server"
                    Text="Save Allotment"
                    CssClass="btn btn-primary"
                    OnClick="btnSaveAllotment_Click" />
            </div>

            <div class="mt-3">
                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    Font-Bold="true">
                </asp:Label>
            </div>

             </asp:View>
             <asp:View ID="vwBulk" runat="server">
                 <div class="row mb-3">
                     <div class="col-md-4">
                         <label>Training</label>
                         <div class="form-control-plaintext fw-bold">
                             <asp:Label ID="lblBulkTrainingInfo" runat="server" />
                         </div>
                     </div>
                     <div class="col-md-4">
                         <label>Hostel *</label>
                         <asp:DropDownList ID="ddlAutoHostel" runat="server" CssClass="form-select"></asp:DropDownList>
                     </div>
                     <div class="col-md-4 d-flex align-items-end">
                         <asp:Button ID="btnRunAutoAllot" runat="server" Text="Auto Allot"
                             CssClass="btn btn-primary" OnClick="btnRunAutoAllot_Click" />
                     </div>
                 </div>

                 <asp:Label ID="lblAutoMessage" runat="server" Font-Bold="true"></asp:Label>
             </asp:View>


             <asp:View ID="vwSummary" runat="server">
                 <div class="row mb-3">
                     <%--<div class="col-md-4">
                         <label>Training *</label>
                         <asp:DropDownList ID="ddlSummaryTraining" runat="server" CssClass="form-select"></asp:DropDownList>
                     </div>--%>

                     <div class="col-md-4">
                         <label>Training</label>
                         <div class="form-control-plaintext fw-bold">
                             <asp:Label ID="lblSummaryTrainingInfo" runat="server" />
                         </div>
                     </div>
                                         
                     <br /><br />
                     <br />
                     <div>
                         <asp:Button ID="btnLoadSummary" runat="server" Text="Show Summary" OnClick="btnLoadSummary_Click" />

                     </div>
                    <br />
                     <div>

                         <asp:Label ID="lblMessageOnSummary" runat="server" Text=""></asp:Label>
                     </div>
                     
                     <div>
                     <asp:GridView ID="gvSummary" 
                          runat="server" 
                          AutoGenerateColumns="false"
                          DataKeyNames="EmpID,AllotmentPK,BedPK"                          
                          CssClass="table table-bordered table-striped gridview"
                          OnRowCommand="gvSummary_RowCommand"
                          OnRowDataBound ="gvSummary_RowDataBound">
                
                 <Columns>
                     <asp:TemplateField HeaderText="S.No">
                         <ItemTemplate>
                             <%# Container.DataItemIndex + 1 %>
                         </ItemTemplate>
                     </asp:TemplateField>

                     <asp:TemplateField HeaderText="Emp ID">
                        <ItemTemplate>
                            <asp:Label
                                ID="lblempID"
                                runat="server"
                                Text='<%# Eval("EmpID") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Emp Name">
                        <ItemTemplate>
                            <asp:Label
                                ID="lblempName"
                                runat="server"
                                Text='<%# Eval("EmpName") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                      <asp:TemplateField HeaderText="Gender">
                        <ItemTemplate>
                            <asp:Label
                                ID="lblGender"
                                runat="server"
                                Text='<%# Eval("gender") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>


                             <asp:TemplateField HeaderText="Hostel Name">
                        <ItemTemplate>
                            <asp:Label
                                ID="lblhostelName"
                                runat="server"
                                Text='<%# Eval("HostelName") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Block Name">
                        <ItemTemplate>
                            <asp:Label
                                ID="lblblockName"
                                runat="server"
                                Text='<%# Eval("BlockName") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                   <asp:TemplateField HeaderText="Flat Name">
                        <ItemTemplate>
                            <asp:Label
                                ID="lblflatName"
                                runat="server"
                                Text='<%# Eval("FlatName") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>


                             <asp:TemplateField HeaderText="Room No.">
                        <ItemTemplate>
                            <asp:Label
                                ID="lblrmNo"
                                runat="server"
                                Text='<%# Eval("RoomNo") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Bed No">
                        <ItemTemplate>
                            <asp:Label
                                ID="lblbedNo"
                                runat="server"
                                Text='<%# Eval("BedNo") %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>


                   <asp:TemplateField HeaderText="Edit Action">
                        <ItemTemplate>
                            <asp:LinkButton
                                ID="btnEditSummary"
                                runat="server"
                                CommandName="Vacate"
                                Text="Vacate"
                                CommandArgument='<%# Eval("AllotmentPK") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

               </Columns>
                     
                     </asp:GridView>
                    </div>
                 </div>
             </asp:View>

         </asp:MultiView>
     </div>
 

     
        </div>
    </div>
</asp:Content>
