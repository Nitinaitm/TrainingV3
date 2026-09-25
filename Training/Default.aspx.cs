using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training
{
    public partial class Default :
        System.Web.UI.Page
    {
        clsDataAccess cls =
            new clsDataAccess();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Remove("dt_Login");
                Session.Remove("dt_Mobile");
            }
        }
        private void LoadLoginCache()
        {
            string query = @"SELECT LoginIDUserID,Password,Role,CorrespondingEmpID,Active,re FROM Login ORDER BY LoginIDUserID";

            DataTable dt =
                cls.GetDataTable(
                query);

            Session["dt_Login"] =
                dt;
        }
        private void LoadMobileCache()
        {
            string query = @"

SELECT

L.LoginIDUserID AS LoginID,

L.CorrespondingEmpID,

E.MobileNo,

L.Role,

'Internal' AS UserType

FROM Login L

INNER JOIN EmpBasicMaster E

ON L.CorrespondingEmpID=E.EmpID

WHERE
L.Role IN
(
'Admin',
'SuperAdmin',
'Nodal',
'Trainee'
)

UNION ALL

SELECT

TM.TrainerID AS LoginID,

TM.EmpID AS CorrespondingEmpID,

E.MobileNo,

'Trainer' AS Role,

'Internal' AS UserType

FROM TrainerMaster TM

INNER JOIN EmpBasicMaster E

ON TM.EmpID=E.EmpID

WHERE TM.TrainerType='Internal'

UNION ALL

SELECT

TrainerID AS LoginID,

TrainerID AS CorrespondingEmpID,

MobileNo,

'Trainer' AS Role,

'External' AS UserType

FROM TrainerMaster

WHERE TrainerType='External'

UNION ALL

SELECT

M.ManagerID AS LoginID,

M.EmpID AS CorrespondingEmpID,

E.MobileNo,

'Manager' AS Role,

'Internal' AS UserType

FROM ManagerMaster M

INNER JOIN EmpBasicMaster E ON M.EmpID=E.EmpID

WHERE ISNULL(M.ActiveStatus,'Y')='Y'

UNION ALL

SELECT

TraineeID AS LoginID,

TraineeID AS CorrespondingEmpID,

MobileNo,

'Trainee' AS Role,

'External' AS UserType

FROM TraineeMasterExternal

WHERE ISNULL(MobileNo,'')<>''

ORDER BY LoginID";

            DataTable dt =
                cls.GetDataTable(
                query);

            Session["dt_Mobile"] =
                dt;
        }
        private bool LoadEmployeeProfile(
    string empID)
        {
            string query = @"

SELECT

EmpID,

EmpName,

EmpCompany,

EmpDesignation,

EmpPostingPlace,

MobileNo,

EmailId,

EmpType

FROM EmpBasicMaster

WHERE EmpID=@EmpID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@EmpID",
            empID)
    };

            DataTable dt =
                cls.GetDataTable(
                query,
                param);

            if (dt.Rows.Count == 0)
            {
                return false;
            }


            Session["EmpID"] =
                dt.Rows[0]["EmpID"].ToString();

            Session["name"] =
                dt.Rows[0]["EmpName"].ToString();

            Session["company"] =
                dt.Rows[0]["EmpCompany"].ToString();

            Session["designation"] =
                dt.Rows[0]["EmpDesignation"].ToString();

            Session["posting"] =
                dt.Rows[0]["EmpPostingPlace"].ToString();

            Session["mobileno"] =
                dt.Rows[0]["MobileNo"].ToString();

            Session["email"] =
                dt.Rows[0]["EmailId"].ToString();

            Session["EmpType"] =
                dt.Rows[0]["EmpType"].ToString();
            Session["TrainerID"] =
    "";

            Session["TrainerType"] =
                "";

            Session["UserType"] =
                string.Equals(dt.Rows[0]["EmpType"].ToString(), "External", StringComparison.OrdinalIgnoreCase) ? "External" : "Internal";
            return true;
        }
        private bool LoadTrainerProfile(
      string trainerID,
      string correspondingEmpID)
        {
            string query = @"SELECT TrainerID,TrainerType,EmpID,EmpIDExternal,NameExternal,DesignationExternal,TrainerOrganizerExternal,MobileNo,EmailID FROM TrainerMaster WHERE TrainerID=@TrainerID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainerID",
            trainerID)
    };

            DataTable dt =
                cls.GetDataTable(
                query,
                param);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            string trainerType =
                dt.Rows[0]["TrainerType"].ToString();

            Session["TrainerID"] =
                trainerID;

            Session["TrainerType"] =
                trainerType;

            if (trainerType == "Internal")
            {

                bool loaded =
                    LoadEmployeeProfile(
                    correspondingEmpID);

                if (!loaded)
                {
                    return false;
                }

                Session["TrainerID"] =
                    trainerID;

                Session["TrainerType"] =
                    trainerType;

                return true;
            }

            string externalEmpID = dt.Rows[0]["EmpIDExternal"].ToString().Trim();

            if (externalEmpID != "")
            {
                bool employeeLoaded = LoadEmployeeProfile(externalEmpID);

                if (employeeLoaded)
                {
                    Session["TrainerID"] = trainerID;
                    Session["TrainerType"] = trainerType;
                    return true;
                }
            }

            Session["UserType"] =
                "External";

            Session["EmpID"] =
                trainerID;

            Session["name"] =
                dt.Rows[0]["NameExternal"].ToString();

            Session["company"] =
                dt.Rows[0]["TrainerOrganizerExternal"].ToString();

            Session["designation"] =
                dt.Rows[0]["DesignationExternal"].ToString();

            Session["posting"] =
                "";

            Session["mobileno"] =
                dt.Rows[0]["MobileNo"].ToString();

            Session["email"] =
                dt.Rows[0]["EmailID"].ToString();

            return true;
        }

        private bool LoadExternalTraineeProfile(
    string traineeID)
        {
            string query = @"

SELECT

TraineeID,

EmpIDExternal,

TraineeName,

OrganizationName,

Designation,

MobileNo,

EmailID,

EmpType

FROM TraineeMasterExternal

WHERE TraineeID=@TraineeID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TraineeID",
            traineeID)
    };

            DataTable dt =
                cls.GetDataTable(
                query,
                param);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            Session["UserType"] =
                "External";

            Session["EmpType"] =
                dt.Rows[0]["EmpType"].ToString();

            Session["EmpID"] =
                dt.Rows[0]["TraineeID"].ToString();

            Session["name"] =
                dt.Rows[0]["TraineeName"].ToString();

            Session["company"] =
                dt.Rows[0]["OrganizationName"].ToString();

            Session["designation"] =
                dt.Rows[0]["Designation"].ToString();

            Session["mobileno"] =
                dt.Rows[0]["MobileNo"].ToString();

            Session["email"] =
                dt.Rows[0]["EmailID"].ToString();

            return true;
        }

        protected void btnOTP_Click(
     object sender,
     EventArgs e)
        {
            try
            {
                string userID =
                    txtUserId.Text.Trim().ToUpperInvariant();

                string passwordInput =
                    txtPassword.Text.Trim();

                if
                (
                    Session["CaptchaCode"] == null
                )
                {
                    lblMsg.Text =
                        "Captcha Expired.";

                    return;
                }

                if
                (
                    txtCaptcha.Text.Trim()
                    !=
                    Session["CaptchaCode"].ToString()
                )
                {
                    lblMsg.ForeColor =
                        System.Drawing.Color.Red;

                    lblMsg.Text =
                        "CAPTCHA Verification Failed.";

                    return;
                }

                Session["UserID"] =
                    userID;

                string loginQuery = "SELECT TOP 1 LoginIDUserID,Password,Role,CorrespondingEmpID,Active,re FROM Login WHERE LoginIDUserID=@LoginIDUserID";

                SqlParameter[] loginParameters =
                {
                    new SqlParameter("@LoginIDUserID", userID)
                };

                DataTable dtLogin =
                    cls.GetDataTable(
                    loginQuery,
                    loginParameters);

                if (dtLogin.Rows.Count == 0)
                {
                    lblMsg.Text =
                        "Invalid User ID.";

                    return;
                }

                DataRow row =
                    dtLogin.Rows[0];

                Encryptor2 encryptor =
                    new Encryptor2();

                string dbPassword =
                    encryptor.Decrypt(
                    row["Password"].ToString());

                if
                (
                    dbPassword
                    !=
                    passwordInput
                )
                {
                    lblMsg.Text =
                        "Incorrect Password.";

                    return;
                }

                if
                (
                    row["Active"].ToString()
                    !=
                    "Y"
                )
                {
                    lblMsg.Text =
                        "User is Inactive.";

                    return;
                }

                Session["Role"] =
                    row["Role"].ToString();

                Session["CorrespondingID"] =
                    row["CorrespondingEmpID"].ToString();

                Session["IsFirstLogin"] =
                    encryptor.Decrypt(
                    row["re"].ToString());

                bool profileLoaded =
                    LoadUserProfile(
                    Session["Role"].ToString(),
                    userID,
                    Session["CorrespondingID"].ToString());

                if
                (
                    !profileLoaded
                )
                {
                    lblMsg.Text =
                        "User Profile Not Found.";

                    return;
                }

                string mobileQuery = "SELECT TOP 1 MobileNo FROM (SELECT L.LoginIDUserID AS LoginID,L.CorrespondingEmpID,E.MobileNo,L.Role FROM Login L INNER JOIN EmpBasicMaster E ON L.CorrespondingEmpID=E.EmpID WHERE L.LoginIDUserID=@LoginID AND L.Role IN ('Admin','SuperAdmin','Nodal','Trainee') UNION ALL SELECT TM.TrainerID AS LoginID,TM.EmpID AS CorrespondingEmpID,E.MobileNo,'Trainer' AS Role FROM TrainerMaster TM INNER JOIN EmpBasicMaster E ON TM.EmpID=E.EmpID WHERE TM.TrainerType='Internal' AND TM.TrainerID=@LoginID UNION ALL SELECT TM.TrainerID AS LoginID,TM.EmpIDExternal AS CorrespondingEmpID,COALESCE(NULLIF(E.MobileNo,''),TM.MobileNo) AS MobileNo,'Trainer' AS Role FROM TrainerMaster TM LEFT JOIN EmpBasicMaster E ON TM.EmpIDExternal=E.EmpID WHERE TM.TrainerType='External' AND TM.TrainerID=@LoginID UNION ALL SELECT M.ManagerID AS LoginID,M.EmpID AS CorrespondingEmpID,E.MobileNo,'Manager' AS Role FROM ManagerMaster M INNER JOIN EmpBasicMaster E ON M.EmpID=E.EmpID WHERE ISNULL(M.ActiveStatus,'Y')='Y' AND M.ManagerID=@LoginID UNION ALL SELECT TraineeID AS LoginID,TraineeID AS CorrespondingEmpID,MobileNo,'Trainee' AS Role FROM TraineeMasterExternal WHERE TraineeID=@LoginID AND ISNULL(MobileNo,'')<>'') AS U WHERE LoginID=@LoginID";

                SqlParameter[] mobileParameters =
                {
                    new SqlParameter("@LoginID", userID)
                };

                DataTable dtMobile =
                    cls.GetDataTable(
                    mobileQuery,
                    mobileParameters);

                if (dtMobile.Rows.Count == 0)
                {
                    lblMsg.Text =
                        "Mobile Number Not Registered.";

                    return;
                }

                string mobileNo =
                    dtMobile.Rows[0]["MobileNo"].ToString().Trim();

                if
                (
                    mobileNo.Length
                    !=
                    10
                )
                {
                    lblMsg.Text =
                        "Invalid Mobile Number.";

                    return;
                }

                Session.Remove(
                    "otp");

                Session.Remove(
                    "mobileno");

                Session["mobileno"] =
                    mobileNo;

                string otp =
                    SendSMSToMobile(
                    mobileNo);

                Session["otp"] =
                    otp;

                string mobileDisplay =
                    mobileNo.Substring(
                    0,
                    2)
                    +
                    "******"
                    +
                    mobileNo.Substring(
                    mobileNo.Length - 2,
                    2);

                lblMsg.ForeColor =
                    System.Drawing.Color.Green;

                lblMsg.Text =
                    "OTP has been sent to your Mobile No. : "
                    +
                    mobileDisplay;

                txtUserId.Enabled =
                    false;

                txtPassword.Enabled =
                    false;

                txtCaptcha.Enabled =
                    false;

                btnOTP.Visible =
                    false;

                btnOTP.Enabled =
                    false;

                otpVisible.Visible =
                    true;

                enterOTP.Enabled =
                    true;

                enterOTP.Text =
                    "";

                enterOTP.Focus();

                btnLogin.Visible =
                    true;

                btnLogin.Enabled =
                    true;

                lblTimer.Visible =
                    true;

                Page.ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "StartTimer",
                    "startTimerAfterDelay();",
                    true);
            }
            catch
            {
                lblMsg.ForeColor =
                    System.Drawing.Color.Red;

                lblMsg.Text =
                    "Unable to Process Login.";
            }
        }
        private bool LoadUserProfile(
            string role,
            string userID,
            string correspondingID)
        {
            switch (role)
            {
                case "Admin":

                    Session["UserType"] =
                        "Internal";

                    return
                        LoadEmployeeProfile(
                            correspondingID);

                case "SuperAdmin":

                    Session["UserType"] =
                        "Internal";

                    return
                        LoadEmployeeProfile(
                            correspondingID);

                case "Nodal":

                    Session["UserType"] =
                        "Internal";

                    return
                        LoadEmployeeProfile(
                        correspondingID);

                case "Trainer":

                    return
                        LoadTrainerProfile(
                        userID,
                        correspondingID);

                case "Manager":

                    Session["UserType"] = "Internal";

                    return LoadManagerProfile(userID);

                case "Trainee":

                    string query =
                        @"SELECT EmpType FROM EmpBasicMaster WHERE EmpID=@EmpID";

                    SqlParameter[] param =
                    {
                new SqlParameter(
                    "@EmpID",
                    userID)
            };

                    object empType =
                        cls.ExecuteScalar(
                        query,
                        param);

                    if (empType != null)
                    {
                        return
                            LoadEmployeeProfile(
                            userID);
                    }

                    Session["UserType"] =
                        "External";

                    return
                        LoadExternalTraineeProfile(
                        userID);

                default:

                    return false;
            }
        }

        private bool LoadManagerProfile(string managerID)
        {
            string query = "SELECT M.ManagerID,M.EmpID,M.MapForLocation,M.TrainingLocationID,E.EmpName,E.DOB,E.DOJ,E.MobileNo,E.EmailId,E.EmpPostingPlace,E.EmpDesignation FROM ManagerMaster M INNER JOIN EmpBasicMaster E ON M.EmpID=E.EmpID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y'";

            DataTable dt = cls.GetDataTable(query, new SqlParameter[] { new SqlParameter("@ManagerID", managerID) });

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            Session["UserType"] = "Internal";
            Session["EmpID"] = dt.Rows[0]["EmpID"].ToString();
            Session["name"] = dt.Rows[0]["EmpName"].ToString();
            Session["designation"] = dt.Rows[0]["EmpDesignation"].ToString();
            Session["posting"] = dt.Rows[0]["EmpPostingPlace"].ToString();
            Session["mobileno"] = dt.Rows[0]["MobileNo"].ToString();
            Session["email"] = dt.Rows[0]["EmailID"].ToString();
            Session["ManagerID"] = dt.Rows[0]["ManagerID"].ToString();
            Session["ManagerEmpID"] = dt.Rows[0]["EmpID"].ToString();
            Session["ManagerMapForLocation"] = dt.Rows[0]["MapForLocation"].ToString();
            Session["ManagerTrainingLocationID"] = dt.Rows[0]["TrainingLocationID"].ToString();

            return true;
        }

        protected void btnLogin_Click(
      object sender,
      EventArgs e)
        {
            try
            {
                if
                (
                    enterOTP.Text.Trim()
                    ==
                    ""
                )
                {
                    lblMsg.Text =
                        "Please Enter OTP.";

                    return;
                }

                if
                (
                    Session["otp"] == null
                )
                {
                    lblMsg.Text =
                        "OTP Expired.";

                    return;
                }

                if
                (
                    enterOTP.Text.Trim()
                    !=
                    Session["otp"].ToString()
                )
                {
                    lblMsg.Text =
                        "Invalid OTP.";

                    return;
                }

                Session.Remove(
                    "otp");

                if
                (
                    Session["IsFirstLogin"] != null
                    &&
                    Session["IsFirstLogin"].ToString()
                    ==
                    "Y"
                )
                {
                    Response.Redirect(
                        "~/ResetPassword.aspx",
                        false);

                    Context.ApplicationInstance
                        .CompleteRequest();

                    return;
                }

                RedirectUser();
            }
            catch
            {
                lblMsg.ForeColor =
                    System.Drawing.Color.Red;

                lblMsg.Text =
                    "Unable To Login.";
            }
        }

        protected void btnResendOTP_Click(
    object sender,
    EventArgs e)
        {
            try
            {
                if
                (
                    Session["mobileno"] == null
                )
                {
                    lblMsg.Text =
                        "Mobile Number Not Found.";

                    return;
                }

                string mobileNo =
                    Session["mobileno"].ToString();

                string otp =
                    SendSMSToMobile(
                    mobileNo);

                Session["otp"] =
                    otp;

                string pre =
                    mobileNo.Substring(
                    0,
                    2);

                string post =
                    mobileNo.Substring(
                    8,
                    2);

                lblMsg.Text =
                    "OTP has been Re-Sent to your Mobile No. : "
                    +
                    pre
                    +
                    "******"
                    +
                    post;

                enterOTP.Text =
                    "";

                enterOTP.Focus();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "ResetTimer",
                    @"
stopTimer();

document.getElementById('"
                    + btnResendOTP.ClientID +
                    @"').style.display='none';

setTimeout
(
function()
{
startTimer();
},
1000
);",
                    true);
            }
            catch
            {
                lblMsg.Text =
                    "Unable To Resend OTP.";
            }
        }
        private string SendSMSToMobile(
    string mobileNo)
        {
            string otp =
                string.Empty;

            string message =
                string.Empty;

            string role =
                Session["Role"] == null
                ?
                ""
                :
                Session["Role"].ToString();

            if
            (
                role == "cust1"
            )
            {
                otp =
                    "1421";

                return otp;
            }

            Random random =
                new Random();

            otp =
                "1111";

            //otp =
            //random.Next(
            //1000,
            //9999).ToString();

            message =
                otp;

            try
            {
                if
                (
                    mobileNo.Trim() != ""
                )
                {
                    //HttpWebRequest request =
                    //    (
                    //    HttpWebRequest
                    //    )
                    //    WebRequest.Create
                    //    (
                    //    "https://api.pinnacle.in/index.php/sms/urlsms?sender=BSPHCE&numbers="
                    //    +
                    //    mobileNo.Trim()
                    //    +
                    //    "&messagetype=TXT&message=One Time Password (OTP) for Training Portal Login is : "
                    //    +
                    //    message
                    //    +
                    //    " - BSPHCL&response=Y&username=dbabsphcl&pass=Dba$7803"
                    //    );

                    //HttpWebResponse response =
                    //    (
                    //    HttpWebResponse
                    //    )
                    //    request.GetResponse();

                    //StreamReader reader =
                    //    new StreamReader(
                    //    response.GetResponseStream());

                    //string result =
                    //    reader.ReadToEnd();

                    //reader.Close();

                    //response.Close();
                }
            }
            catch
            {

            }

            return otp;
        }
        private void RedirectUser()
        {
            string role =
                Session["Role"].ToString();

            switch (role)
            {
                case "Admin":

                    Session["InternalRedirect_Admin"] =
                        true;

                    Response.Redirect(
                        "~/Admin/Default.aspx");

                    break;

                case "SuperAdmin":

                    Session["InternalRedirect_SuperAdmin"] =
                        true;

                    Response.Redirect(
                        "~/SuperAdmin/Default.aspx");

                    break;

                case "Nodal":

                    Session["InternalRedirect_Nodal"] =
                        true;

                    Response.Redirect(
                        "~/Nodal/Default.aspx");

                    break;

                case "Trainer":

                    Session["InternalRedirect_Trainer"] =
                        true;

                    Response.Redirect(
                        "~/Trainer/Default.aspx");

                    break;

                case "Manager":

                    Session["InternalRedirect_Manager"] = true;

                    Response.Redirect("~/Manager/Default.aspx");

                    break;

                case "Trainee":

                    Session["InternalRedirect_Trainee"] =
                        true;

                    Response.Redirect(
                        "~/Trainee/PreTrainingExam.aspx");

                    break;

                default:

                    lblMsg.Text =
                        "You Are Not Authorized To Login.";

                    break;
            }
        }
    }
}