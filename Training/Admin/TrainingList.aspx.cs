using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;


namespace Training.Admin
{
    public partial class TrainingList :
        System.Web.UI.Page
    {
        clsDataAccess obj =
            new clsDataAccess();

        protected void Page_Load(
    object sender,
    EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Remove("TrainingID");

                BindGrid();
            }
        }

        private void BindGrid()
        {
            string query = "SELECT TD.TrainingID,TM.CourseName,TD.TrainingCategory,TD.TrainingType,TD.TrainingLocation,TD.Batch,TD.DateFrom,TD.DateTo,CASE WHEN ISNULL(TD.TrainingStatus,'Draft')='Completed' OR ISNULL(TD.WorkflowStatus,'')='ABCDEFGHIJ' THEN 'Completed' WHEN ISNULL(TD.WorkflowStatus,'') LIKE '%E%' THEN 'InProgress' WHEN EXISTS (SELECT 1 FROM TrainingAssignment TA WHERE TA.TrainingID=TD.TrainingID AND ISNULL(TA.AssignmentStatus,'Assigned')='Assigned') THEN 'TraineeAssigned' WHEN EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TD.TrainingID) THEN 'SessionAssigned' ELSE 'Draft' END AS TrainingStatus FROM TrainingDetails TD INNER JOIN CourseMaster TM ON TD.CourseID=TM.CourseID WHERE 1=1";
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(txtTrainingID.Text))
            {
                query += " AND TD.TrainingID LIKE @TrainingID";
                parameters.Add(new SqlParameter("@TrainingID", "%" + txtTrainingID.Text.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(txtCourse.Text))
            {
                query += " AND TM.CourseName LIKE @CourseName";
                parameters.Add(new SqlParameter("@CourseName", "%" + txtCourse.Text.Trim() + "%"));
            }
            if (!string.IsNullOrWhiteSpace(ddlStatus.SelectedValue))
            {
                string status = ddlStatus.SelectedValue;
                if (status == "Completed")
                {
                    query += " AND (ISNULL(TD.TrainingStatus,'Draft')='Completed' OR ISNULL(TD.WorkflowStatus,'')='ABCDEFGHIJ')";
                }
                else if (status == "InProgress")
                {
                    query += " AND ISNULL(TD.WorkflowStatus,'') LIKE '%E%' AND ISNULL(TD.TrainingStatus,'Draft')<>'Completed' AND ISNULL(TD.WorkflowStatus,'')<>'ABCDEFGHIJ'";
                }
                else if (status == "TraineeAssigned")
                {
                    query += " AND EXISTS (SELECT 1 FROM TrainingAssignment TA WHERE TA.TrainingID=TD.TrainingID AND ISNULL(TA.AssignmentStatus,'Assigned')='Assigned') AND NOT EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TD.TrainingID) AND ISNULL(TD.WorkflowStatus,'') NOT LIKE '%E%'";
                }
                else if (status == "SessionAssigned")
                {
                    query += " AND EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TD.TrainingID) AND NOT EXISTS (SELECT 1 FROM TrainingAssignment TA WHERE TA.TrainingID=TD.TrainingID AND ISNULL(TA.AssignmentStatus,'Assigned')='Assigned') AND ISNULL(TD.WorkflowStatus,'') NOT LIKE '%E%'";
                }
                else if (status == "Draft")
                {
                    query += " AND NOT EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TD.TrainingID) AND NOT EXISTS (SELECT 1 FROM TrainingAssignment TA WHERE TA.TrainingID=TD.TrainingID AND ISNULL(TA.AssignmentStatus,'Assigned')='Assigned') AND ISNULL(TD.WorkflowStatus,'') NOT LIKE '%E%' AND ISNULL(TD.TrainingStatus,'Draft')<>'Completed'";
                }
            }
            query += " ORDER BY TD.ID DESC";
            gvTraining.DataSource = obj.GetDataTable(query, parameters.ToArray());
            gvTraining.DataBind();
        }

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            BindGrid();
        }

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            txtTrainingID.Text = "";

            txtCourse.Text = "";

            ddlStatus.SelectedIndex = 0;

            BindGrid();
        }

        protected void gvTraining_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Manage")
            {
                Session["TrainingID"] =
                    e.CommandArgument
                    .ToString();

                Response.Redirect(
                    "ManageTraining.aspx");
            }
        }
    }
}