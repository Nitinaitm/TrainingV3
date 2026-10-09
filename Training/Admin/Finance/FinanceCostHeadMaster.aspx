<%@ page title="Finance - Cost Head Master" language="C#" masterpagefile="~/AdminMaster.Master" autoeventwireup="true" codebehind="FinanceCostHeadMaster.aspx.cs" inherits="Training.Admin.FinanceCostHeadMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .main-card {
            background: #fff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 0 10px #d9d9d9;
            margin-top: 20px
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px
        }

        .info-box {
            margin-bottom: 12px
        }

        .action-card {
            margin-top: 20px;
            background: #fff;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            padding: 20px
        }

        .btn-action {
            min-width: 180px;
            margin-right: 10px;
            margin-bottom: 10px
        }

        .status-badge {
            font-size: 16px;
            padding: 8px 15px
        }

        .lifecycle-section {
            margin-top: 16px;
            padding: 14px;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            background: #fff
        }

        .session-cycle {
            background: #fbfbfb
        }

        .lifecycle-section + .lifecycle-section {
            margin-top: 12px
        }

        .lifecycle {
            margin: 20px 0 25px;
            padding: 20px;
            border: 1px solid #dee2e6;
            border-radius: 12px;
            background: #f8f9fa
        }

        .lifecycle-title {
            font-size: 20px;
            font-weight: 700;
            margin-bottom: 18px;
            text-align: center
        }

        .stage-scroll {
            overflow-x: auto;
            padding: 8px 0 12px
        }

        .stage-line {
            display: flex;
            align-items: flex-start;
            min-width: 1100px
        }

        .stage-item {
            flex: 1;
            position: relative;
            text-align: center
        }

            .stage-item:not(:last-child):after {
                content: "";
                position: absolute;
                top: 17px;
                left: 50%;
                width: 100%;
                height: 4px;
                background: #dc3545;
                z-index: 0
            }

            .stage-item.done:not(:last-child):after {
                background: #198754
            }

        .stage-bubble {
            position: relative;
            z-index: 1;
            width: 52px;
            height: 52px;
            line-height: 46px;
            border-radius: 50%;
            margin: 0 auto 8px;
            background: #dc3545;
            color: #fff;
            font-weight: 700;
            font-size: 13px;
            border: 3px solid #fff;
            box-shadow: 0 0 0 1px #dc3545;
            cursor: help
        }

        .stage-item.done .stage-bubble {
            background: #198754;
            box-shadow: 0 0 0 1px #198754
        }

        .stage-item.na .stage-bubble, .stage-item.skipped .stage-bubble {
            background: #adb5bd;
            box-shadow: 0 0 0 1px #adb5bd
        }

        .stage-item.partial .stage-bubble {
            box-shadow: 0 0 0 1px #198754
        }

        .stage-label {
            font-size: 12px;
            font-weight: 600;
            line-height: 1.25;
            padding: 0 4px
        }

        .stage-state {
            font-size: 10px;
            margin-top: 3px;
            color: #dc3545
        }

        .stage-item.done .stage-state {
            color: #198754
        }

        .stage-item.na .stage-state, .stage-item.skipped .stage-state {
            color: #6c757d
        }

        .stage-bubble[data-tooltip] {
            position: relative
        }

            .stage-bubble[data-tooltip]:hover:after {
                content: attr(data-tooltip);
                position: absolute;
                left: 50%;
                bottom: calc(100% + 10px);
                transform: translateX(-50%);
                background: #212529;
                color: #fff;
                padding: 7px 10px;
                border-radius: 6px;
                font-size: 12px;
                font-weight: 600;
                line-height: 1.2;
                white-space: nowrap;
                z-index: 1000;
                box-shadow: 0 3px 10px rgba(0,0,0,.25)
            }

            .stage-bubble[data-tooltip]:hover:before {
                content: "";
                position: absolute;
                left: 50%;
                bottom: calc(100% + 4px);
                transform: translateX(-50%);
                border: 6px solid transparent;
                border-top-color: #212529;
                z-index: 1001
            }

        @media(max-width:768px) {
            .main-card {
                padding: 15px
            }

            .stage-scroll {
                margin-left: -5px;
                margin-right: -5px
            }
        }
    </style>
</asp:Content>
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="main-card">
<div class="page-heading">Finance - Cost Head Master</div>
<asp:Label ID="lblMessage" runat="server" Font-Bold="true"></asp:Label>
<div class="row">
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Code *</label><asp:TextBox ID="txtCode" runat="server" CssClass="form-control" placeholder="TRAINER_TEACH"></asp:TextBox></div>
<div class="col-lg-3 col-md-6 mb-3"><label class="form-label">Cost Head *</label><asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Trainer Teaching"></asp:TextBox></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Level</label><asp:DropDownList ID="ddlLevel" runat="server" CssClass="form-select"><asp:ListItem>Course</asp:ListItem><asp:ListItem>Batch</asp:ListItem><asp:ListItem>Session</asp:ListItem><asp:ListItem>Person</asp:ListItem></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Calculation</label><asp:DropDownList ID="ddlMode" runat="server" CssClass="form-select"><asp:ListItem>Automatic</asp:ListItem><asp:ListItem>Manual</asp:ListItem><asp:ListItem>Automatic + Override</asp:ListItem></asp:DropDownList></div>
<div class="col-lg-2 col-md-6 mb-3"><label class="form-label">Unit *</label><asp:TextBox ID="txtUnit" runat="server" CssClass="form-control" placeholder="Trainee-Day"></asp:TextBox></div>
</div>
<div class="mt-2 text-center">
<asp:Button ID="btnSave" runat="server" Text="Save Cost Head" CssClass="btn btn-save" OnClick="btnSave_Click" />
&nbsp;
<asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
</div>
<div class="table-responsive">
<asp:GridView ID="gvHeads" runat="server" CssClass="table table-bordered table-hover finance-grid" AutoGenerateColumns="False" DataKeyNames="CostHeadID" OnRowCommand="gvHeads_RowCommand">
<Columns>
<asp:TemplateField HeaderText="S.No."><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><HeaderStyle Width="55px" /><ItemStyle HorizontalAlign="Center" /></asp:TemplateField>
<asp:BoundField DataField="CostHeadCode" HeaderText="Code" />
<asp:BoundField DataField="CostHeadName" HeaderText="Cost Head" />
<asp:BoundField DataField="CostLevel" HeaderText="Level" />
<asp:BoundField DataField="CalculationMode" HeaderText="Calculation" />
<asp:BoundField DataField="UnitType" HeaderText="Unit" />
<asp:BoundField DataField="Active" HeaderText="Active" />
<asp:ButtonField CommandName="Toggle" Text="Activate / Deactivate" ButtonType="Button" ControlStyle-CssClass="btn btn-sm btn-outline-primary" />
</Columns>
</asp:GridView>
</div>
</div>
</div>
</asp:content>
