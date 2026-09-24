<%@ page title=""
    language="C#"
    masterpagefile="~/AdminMaster.Master"
    autoeventwireup="true"
    codebehind="TopicEntry.aspx.cs"
    inherits="Training.Admin.TopicEntry" %>

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

        .validation {
            color: red;
            font-size: 13px;
        }

        .gridview th {
            background: #0d6efd;
            color: white;
            text-align: center;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                Topic Master Entry
            </div>

            <div class="row">

                <div class="col-md-4 mb-3">

                    <label>Topic Name *</label>

                    <asp:textbox
                        id="txtTopicName"
                        runat="server"
                        cssclass="form-control">
</asp:textbox>

                    <asp:requiredfieldvalidator
                        id="rfvTopic"
                        runat="server"
                        controltovalidate="txtTopicName"
                        validationgroup="SaveGroup"
                        cssclass="validation"
                        errormessage="Enter Topic Name">
</asp:requiredfieldvalidator>

                </div>

                <div class="col-md-4 mb-3">

                   
                <!-- Topic Category -->

                <div class="col-lg-6 mb-3">

                    <label class="form-label">
                        Topic Category *

                    </label>

                    <asp:DropDownList
                        ID="ddlCourseCategory"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator
                        ID="rfvCategory"
                        runat="server"
                        ControlToValidate="ddlCourseCategory"
                        InitialValue=""
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Select Topic Category">
                    </asp:RequiredFieldValidator>

                </div>

                </div>

                <div class="col-md-12 mb-3">

                    <label>Description</label>

                    <asp:textbox
                        id="txtDescription"
                        runat="server"
                        cssclass="form-control"
                        textmode="MultiLine"
                        rows="4">
</asp:textbox>

                </div>

                <div class="col-md-12">

                    <asp:button
                        id="btnSave"
                        runat="server"
                        text="Save Topic"
                        cssclass="btn btn-primary"
                        validationgroup="SaveGroup"
                        onclick="btnSave_Click" />

                </div>

                <div class="col-md-12 mt-3">

                    <asp:label
                        id="lblMessage"
                        runat="server"
                        font-bold="true">
</asp:label>

                </div>

            </div>

            <hr />

            <asp:gridview
                id="gvTopic"
                runat="server"
                autogeneratecolumns="False"
                cssclass="table table-bordered table-striped gridview">

<Columns>

<asp:BoundField
DataField="TopicID"
HeaderText="Topic ID" />

<asp:BoundField
DataField="TopicName"
HeaderText="Topic Name" />

<asp:BoundField
DataField="Category"
HeaderText="Category" />

<asp:BoundField
DataField="Description"
HeaderText="Description" />

<asp:BoundField
DataField="CreatedOn"
HeaderText="Created On"
DataFormatString="{0:dd-MM-yyyy}" />

</Columns>

</asp:gridview>

        </div>

    </div>

</asp:Content>
