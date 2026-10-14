using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace Training.Admin
{
    public partial class FinanceRateMaster : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCostHeads();
                BindCourses();
                BindTrainingBatches();
                BindLocations();
                BindTrainers();
                txtFrom.Text = DateTime.Today.ToString("yyyy-MM-dd");
                BindGrid();
            }
        }

        private void BindCostHeads()
        {
            DataTable dt = objDB.GetDataTable("SELECT CostHeadID,CostHeadName FROM FinanceCostHeadMaster WHERE Active='Y' ORDER BY CostHeadName");
            ddlCostHead.DataSource = dt;
            ddlCostHead.DataTextField = "CostHeadName";
            ddlCostHead.DataValueField = "CostHeadID";
            ddlCostHead.DataBind();
            ddlCostHead.Items.Insert(0,new System.Web.UI.WebControls.ListItem("Select Cost Head",""));
        }

        private void BindCourses()
        {
            DataTable dt = objDB.GetDataTable("SELECT CourseID,ISNULL(CourseName,'')+' ('+CourseID+')' AS CourseDisplay FROM CourseMaster ORDER BY CourseName");
            ddlCourse.DataSource = dt;
            ddlCourse.DataTextField = "CourseDisplay";
            ddlCourse.DataValueField = "CourseID";
            ddlCourse.DataBind();
            ddlCourse.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Courses", ""));
        }

        private void BindTrainingBatches()
        {
            DataTable dt = objDB.GetDataTable("SELECT TD.TrainingID,TD.TrainingID+' - '+ISNULL(C.CourseName,'')+' - '+ISNULL(TD.Batch,'')+' - '+ISNULL(TD.TrainingLocation,'') AS TrainingDisplay FROM TrainingDetails TD LEFT JOIN CourseMaster C ON C.CourseID=TD.CourseID ORDER BY TD.TrainingID DESC");
            ddlTraining.DataSource = dt;
            ddlTraining.DataTextField = "TrainingDisplay";
            ddlTraining.DataValueField = "TrainingID";
            ddlTraining.DataBind();
            ddlTraining.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Batches", ""));
        }

        private void BindLocations()
        {
            DataTable dt = objDB.GetDataTable("SELECT DISTINCT LTRIM(RTRIM(TrainingLocation)) AS TrainingLocation FROM TrainingDetails WHERE ISNULL(LTRIM(RTRIM(TrainingLocation)),'')<>'' ORDER BY TrainingLocation");
            ddlLocation.DataSource = dt;
            ddlLocation.DataTextField = "TrainingLocation";
            ddlLocation.DataValueField = "TrainingLocation";
            ddlLocation.DataBind();
            ddlLocation.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Locations", ""));
        }

        private void BindTrainers()
        {
            DataTable dt = objDB.GetDataTable("SELECT TM.TrainerID,CASE WHEN TM.TrainerType='Internal' THEN ISNULL(E.EmpName,'') ELSE ISNULL(TM.NameExternal,'') END+' ('+TM.TrainerID+')' AS TrainerDisplay FROM TrainerMaster TM LEFT JOIN EmpBasicMaster E ON E.EmpID=TM.EmpID WHERE ISNULL(TM.ActiveStatus,'Active') NOT IN ('N','No','Inactive','Disabled') ORDER BY TrainerDisplay");
            ddlTrainer.DataSource = dt;
            ddlTrainer.DataTextField = "TrainerDisplay";
            ddlTrainer.DataValueField = "TrainerID";
            ddlTrainer.DataBind();
            ddlTrainer.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All Trainers", ""));
        }

        private void BindGrid()
        {
            gvRates.DataSource = objDB.GetDataTable("SELECT R.RateID,H.CostHeadName,R.Rate,R.EffectiveFrom,R.EffectiveTo,ISNULL(R.CourseID,'All') AS CourseScope,ISNULL(R.TrainingID,'All') AS BatchScope,ISNULL(R.TrainingLocation,'All') AS LocationScope,ISNULL(R.ResidentialType,'All') AS ResidentialScope,ISNULL(R.TrainerID,'All') AS TrainerScope,R.Active FROM FinanceRateMaster R INNER JOIN FinanceCostHeadMaster H ON R.CostHeadID=H.CostHeadID ORDER BY H.CostHeadName,R.EffectiveFrom DESC,R.RateID DESC");
            gvRates.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            decimal rate;
            DateTime fromDate;
            DateTime toDate;
            if (ddlCostHead.SelectedValue == "" || !Decimal.TryParse(txtRate.Text.Trim(),out rate) || rate < 0 || !DateTime.TryParse(txtFrom.Text,out fromDate))
            {
                ShowMessage("Cost Head, valid Rate and Effective From are required.",Color.Red);
                return;
            }
            DateTime? to = null;
            if (txtTo.Text.Trim() != "")
            {
                if (!DateTime.TryParse(txtTo.Text,out toDate))
                {
                    ShowMessage("Invalid Effective To date.",Color.Red);
                    return;
                }
                to = toDate.Date;
                if (to.Value.Date < fromDate.Date)
                {
                    ShowMessage("Effective To cannot be earlier than Effective From.", Color.Red);
                    return;
                }
            }
            string courseID = ddlCourse.SelectedValue;
            string trainingID = ddlTraining.SelectedValue;
            string location = ddlLocation.SelectedValue;
            string trainerID = ddlTrainer.SelectedValue;
            object residentialType = ddlResidentialType.SelectedValue == "" ? (object)DBNull.Value : ddlResidentialType.SelectedValue;
            DataTable overlap = objDB.GetDataTable("SELECT TOP 1 RateID FROM FinanceRateMaster WHERE CostHeadID=@CostHeadID AND ISNULL(CourseID,'')=@CourseID AND ISNULL(TrainingID,'')=@TrainingID AND ISNULL(TrainingLocation,'')=@Location AND ISNULL(ResidentialType,'')=@ResidentialType AND ISNULL(TrainerID,'')=@TrainerID AND Active='Y' AND EffectiveFrom<=ISNULL(@To,CONVERT(date,'99991231')) AND ISNULL(EffectiveTo,CONVERT(date,'99991231'))>=@From", new SqlParameter[] { new SqlParameter("@CostHeadID", Convert.ToInt32(ddlCostHead.SelectedValue)), new SqlParameter("@CourseID", courseID), new SqlParameter("@TrainingID", trainingID), new SqlParameter("@Location", location), new SqlParameter("@ResidentialType", residentialType == DBNull.Value ? "" : residentialType), new SqlParameter("@TrainerID", trainerID), new SqlParameter("@From", fromDate.Date), new SqlParameter("@To", to.HasValue ? (object)to.Value : DBNull.Value) });
            if (overlap.Rows.Count > 0)
            {
                ShowMessage("An active rate already exists for the same scope and overlapping effective dates. Close the earlier rate first.", Color.Red);
                return;
            }
            int result = objDB.ExecuteSql("INSERT INTO FinanceRateMaster(CostHeadID,Rate,EffectiveFrom,EffectiveTo,CourseID,TrainingID,TrainingLocation,ResidentialType,TrainerID,Remarks,CreatedBy) VALUES(@CostHeadID,@Rate,@From,@To,NULLIF(@CourseID,''),NULLIF(@TrainingID,''),NULLIF(@Location,''),NULLIF(@ResidentialType,''),NULLIF(@TrainerID,''),@Remarks,@CreatedBy)", new SqlParameter[] { new SqlParameter("@CostHeadID",Convert.ToInt32(ddlCostHead.SelectedValue)),new SqlParameter("@Rate",rate),new SqlParameter("@From",fromDate.Date),new SqlParameter("@To",to.HasValue?(object)to.Value:(object)DBNull.Value),new SqlParameter("@CourseID",courseID),new SqlParameter("@TrainingID",trainingID),new SqlParameter("@Location",location),new SqlParameter("@ResidentialType",residentialType == DBNull.Value ? "" : residentialType),new SqlParameter("@TrainerID",trainerID),new SqlParameter("@Remarks",txtRemarks.Text.Trim()),new SqlParameter("@CreatedBy",Convert.ToString(Session["UserID"])) });
            if(result>0){ShowMessage("Rate saved successfully.",Color.Green);ClearForm();BindGrid();}
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm(){txtRate.Text="";txtFrom.Text=DateTime.Today.ToString("yyyy-MM-dd");txtTo.Text="";txtRemarks.Text="";ddlCourse.SelectedIndex=0;ddlTraining.SelectedIndex=0;ddlLocation.SelectedIndex=0;ddlTrainer.SelectedIndex=0;ddlResidentialType.SelectedIndex=0;}
        private void ShowMessage(string text,Color color){lblMessage.Text=text;lblMessage.ForeColor=color;}
    }
}