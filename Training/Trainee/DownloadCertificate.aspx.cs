using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace Training.Trainee
{
    public partial class DownloadCertificate : System.Web.UI.Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null || string.IsNullOrWhiteSpace(Session["EmpID"].ToString()) || Session["Role"] == null || !string.Equals(Session["Role"].ToString(), "Trainee", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            string certificateID=Request.QueryString["CertificateID"];
            string empID=Session["EmpID"].ToString().Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(certificateID))
            {
                Response.StatusCode=400;
                Response.End();
                return;
            }

            DataTable dt=objDB.GetDataTable("SELECT PDFPath,PDFName FROM TrainingCertificate WHERE CertificateID=@CertificateID AND EmpID=@EmpID AND CertificateStatus='A'",new SqlParameter[] { new SqlParameter("@CertificateID",certificateID),new SqlParameter("@EmpID",empID) });

            if (dt.Rows.Count==0)
            {
                Response.StatusCode=404;
                Response.End();
                return;
            }

            string pdfPath=Convert.ToString(dt.Rows[0]["PDFPath"]);
            if (string.IsNullOrWhiteSpace(pdfPath))
            {
                Response.StatusCode=404;
                Response.End();
                return;
            }

            string physicalPath=Server.MapPath(pdfPath);
            if (!File.Exists(physicalPath))
            {
                Response.StatusCode=404;
                Response.End();
                return;
            }

            string pdfName=Convert.ToString(dt.Rows[0]["PDFName"]);
            if (string.IsNullOrWhiteSpace(pdfName)) pdfName=Path.GetFileName(physicalPath);

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType="application/pdf";
            Response.AddHeader("Content-Disposition","attachment; filename=""+pdfName+""");
            Response.AddHeader("Content-Length",new FileInfo(physicalPath).Length.ToString());
            Response.TransmitFile(physicalPath);
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}