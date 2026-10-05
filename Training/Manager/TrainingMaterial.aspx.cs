using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

namespace Training.Manager
{
    public partial class TrainingMaterial : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();
        private string ManagerID { get { return Session["ManagerID"] == null ? "" : Session["ManagerID"].ToString().Trim(); } }
        private string SelectedSessionID { get { return Session["SessionID"] == null ? "" : Session["SessionID"].ToString().Trim(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ManagerID))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (Session["TrainingID"] == null || string.IsNullOrWhiteSpace(Session["TrainingID"].ToString()) || Session["SessionID"] == null || string.IsNullOrWhiteSpace(Session["SessionID"].ToString()))
            {
                Response.Redirect("~/Manager/MyTrainings.aspx");
                return;
            }

            if (!HasSessionAccess(SelectedSessionID))
            {
                Response.Redirect("~/Manager/MyTrainings.aspx");
                return;
            }

            if (!IsPostBack)
            {
                pnlMaterial.Visible = true;
                BindMaterials();
            }
        }

        private string GetLocationID()
        {
            object v = obj.ExecuteScalar("SELECT TOP 1 TrainingLocationID FROM ManagerMaster WHERE ManagerID=@ManagerID AND ISNULL(ActiveStatus,'Y')='Y'", new SqlParameter[] { new SqlParameter("@ManagerID",ManagerID) });
            return v == null || v == DBNull.Value ? "" : v.ToString().Trim();
        }

        private bool HasSessionAccess(string sessionID)
        {
            object v = obj.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID INNER JOIN TrainingDetails TD ON TD.TrainingLocation=L.TrainingLocation INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND SM.SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@ManagerID",ManagerID), new SqlParameter("@SessionID",sessionID) });
            return v != null && v != DBNull.Value && Convert.ToInt32(v) > 0;
        }

        private void BindMaterials()
        {
            if (string.IsNullOrWhiteSpace(SelectedSessionID) || !HasSessionAccess(SelectedSessionID))
            {
                gvMaterial.DataSource = null;
                gvMaterial.DataBind();
                return;
            }
            DataTable dt = obj.GetDataTable("SELECT MaterialID,Title,MaterialType,FileName,Description,VisibleToTrainee,DownloadAllowed,CreatedOn FROM TrainingMaterial WHERE SessionID=@SessionID ORDER BY CreatedOn DESC", new SqlParameter[] { new SqlParameter("@SessionID",SelectedSessionID) });
            gvMaterial.DataSource = dt;
            gvMaterial.DataBind();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            if (!HasSessionAccess(SelectedSessionID)) { lblMessage.Text="Invalid session."; return; }
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(ddlType.SelectedValue) || !fuMaterial.HasFile) { lblMessage.Text="Please enter title, select material type and select file."; return; }
            string extension = Path.GetExtension(fuMaterial.FileName).ToLowerInvariant();
            if (fuMaterial.PostedFile.ContentLength > 104857600) { lblMessage.Text="Maximum file size is 100 MB."; return; }
            if (!ValidateMaterialType(ddlType.SelectedValue,extension)) { lblMessage.Text="Selected file does not match Material Type."; return; }
            int duplicate = Convert.ToInt32(obj.ExecuteScalar("SELECT COUNT(*) FROM TrainingMaterial WHERE SessionID=@SessionID AND UPPER(Title)=UPPER(@Title)", new SqlParameter[] { new SqlParameter("@SessionID",SelectedSessionID),new SqlParameter("@Title",txtTitle.Text.Trim()) }));
            if (duplicate > 0) { lblMessage.Text="Material Title already exists."; return; }
            object trainingID = obj.ExecuteScalar("SELECT TrainingID FROM SessionMaster WHERE SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID",SelectedSessionID) });
            object topicID = obj.ExecuteScalar("SELECT TopicID FROM SessionMaster WHERE SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID",SelectedSessionID) });
            string materialID = GenerateMaterialID();
            string folder = Server.MapPath("~/Uploads/TrainingMaterial/");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string fileName = materialID + extension;
            fuMaterial.SaveAs(Path.Combine(folder,fileName));
            string query = "INSERT INTO TrainingMaterial(MaterialID,TrainingID,SessionID,TopicID,TrainerID,Title,Description,MaterialType,FileName,FilePath,VideoURL,VisibleToTrainee,DownloadAllowed,CreatedOn,CreatedBy) VALUES(@MaterialID,@TrainingID,@SessionID,@TopicID,@TrainerID,@Title,@Description,@MaterialType,@FileName,@FilePath,@VideoURL,@VisibleToTrainee,@DownloadAllowed,GETDATE(),@CreatedBy)";
            obj.ExecuteSql(query,new SqlParameter[] { new SqlParameter("@MaterialID",materialID),new SqlParameter("@TrainingID",trainingID ?? DBNull.Value),new SqlParameter("@SessionID",SelectedSessionID),new SqlParameter("@TopicID",topicID ?? DBNull.Value),new SqlParameter("@TrainerID","MANAGER:"+ManagerID),new SqlParameter("@Title",txtTitle.Text.Trim()),new SqlParameter("@Description",txtDescription.Text.Trim()),new SqlParameter("@MaterialType",ddlType.SelectedValue),new SqlParameter("@FileName",fuMaterial.FileName),new SqlParameter("@FilePath","~/Uploads/TrainingMaterial/"+fileName),new SqlParameter("@VideoURL",DBNull.Value),new SqlParameter("@VisibleToTrainee",chkVisibleToTrainee.Checked),new SqlParameter("@DownloadAllowed",chkDownloadAllowed.Checked),new SqlParameter("@CreatedBy",ManagerID) });
            lblMessage.ForeColor=System.Drawing.Color.Green;
            lblMessage.Text="Training Material uploaded successfully.";
            txtTitle.Text=""; txtDescription.Text=""; ddlType.SelectedIndex=0; chkVisibleToTrainee.Checked=true; chkDownloadAllowed.Checked=true;
            BindMaterials();
        }

        private string GenerateMaterialID()
        {
            int next = Convert.ToInt32(obj.ExecuteScalar("SELECT ISNULL(MAX(ID),0)+1 FROM TrainingMaterial",null));
            return "MAT"+new Random().Next(1000,9999).ToString()+next.ToString("000000");
        }

        private bool ValidateMaterialType(string type,string extension)
        {
            type=type.Trim().ToUpperInvariant();
            if(type=="PDF") return extension==".pdf";
            if(type=="PPT") return extension==".ppt" || extension==".pptx";
            if(type=="DOCUMENT") return extension==".doc" || extension==".docx" || extension==".xls" || extension==".xlsx";
            if(type=="VIDEO") return extension==".mp4";
            if(type=="OTHER") return true;
            return false;
        }

        protected void gvMaterial_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if(!HasSessionAccess(SelectedSessionID)) return;
            string materialID=Convert.ToString(e.CommandArgument);
            if(e.CommandName=="DownloadMaterial") DownloadMaterial(materialID);
            if(e.CommandName=="DeleteMaterial") DeleteMaterial(materialID);
        }

        private void DownloadMaterial(string materialID)
        {
            DataTable dt=obj.GetDataTable("SELECT FileName,FilePath,DownloadAllowed FROM TrainingMaterial WHERE MaterialID=@MaterialID AND SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@MaterialID",materialID),new SqlParameter("@SessionID",SelectedSessionID)});
            if(dt.Rows.Count==0){lblMessage.Text="Material not found.";return;}
            if(!Convert.ToBoolean(dt.Rows[0]["DownloadAllowed"])){lblMessage.Text="Download is not allowed.";return;}
            string path=Server.MapPath(dt.Rows[0]["FilePath"].ToString());
            if(!File.Exists(path)){lblMessage.Text="Physical file not found.";return;}
            Response.Clear();
            Response.ContentType=MimeType(Path.GetExtension(path));
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + dt.Rows[0]["FileName"].ToString());
            Response.TransmitFile(path);
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private string MimeType(string ext)
        {
            switch(ext.ToLowerInvariant()){case ".pdf":return "application/pdf";case ".ppt":return "application/vnd.ms-powerpoint";case ".pptx":return "application/vnd.openxmlformats-officedocument.presentationml.presentation";case ".doc":return "application/msword";case ".docx":return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";case ".xls":return "application/vnd.ms-excel";case ".xlsx":return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";case ".mp4":return "video/mp4";case ".jpg":case ".jpeg":return "image/jpeg";case ".png":return "image/png";case ".zip":return "application/zip";case ".rar":return "application/x-rar-compressed";default:return "application/octet-stream";}
        }

        private void DeleteMaterial(string materialID)
        {
            DataTable dt=obj.GetDataTable("SELECT FilePath FROM TrainingMaterial WHERE MaterialID=@MaterialID AND SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@MaterialID",materialID),new SqlParameter("@SessionID",SelectedSessionID)});
            if(dt.Rows.Count==0)return;
            obj.ExecuteSql("DELETE FROM TrainingMaterial WHERE MaterialID=@MaterialID AND SessionID=@SessionID",new SqlParameter[]{new SqlParameter("@MaterialID",materialID),new SqlParameter("@SessionID",SelectedSessionID)});
            string path=Server.MapPath(dt.Rows[0]["FilePath"].ToString());
            if(File.Exists(path)) File.Delete(path);
            BindMaterials();
            lblMessage.ForeColor=System.Drawing.Color.Green;
            lblMessage.Text="Material deleted successfully.";
        }
    }
}