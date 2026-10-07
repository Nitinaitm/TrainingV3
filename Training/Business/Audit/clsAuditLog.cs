using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

public static class clsAuditLog
{
    private static string ConnectionString { get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; } }

    public static int LogLogin(string userID,string role,string status,string failureReason)
    {
        try
        {
            using (SqlConnection con=new SqlConnection(ConnectionString))
            {
                con.Open();
                string sql="INSERT INTO UserLoginHistory(UserID,UserRole,LoginTime,LoginStatus,FailureReason,IPAddress,UserAgent,SessionID) VALUES(@UserID,@UserRole,GETDATE(),@LoginStatus,@FailureReason,@IPAddress,@UserAgent,@SessionID); SELECT CAST(SCOPE_IDENTITY() AS int)";
                using (SqlCommand cmd=new SqlCommand(sql,con))
                {
                    cmd.Parameters.AddWithValue("@UserID",userID??"");
                    cmd.Parameters.AddWithValue("@UserRole",role??"");
                    cmd.Parameters.AddWithValue("@LoginStatus",status??"");
                    cmd.Parameters.AddWithValue("@FailureReason",(object)failureReason??DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress",GetIPAddress());
                    cmd.Parameters.AddWithValue("@UserAgent",GetUserAgent());
                    cmd.Parameters.AddWithValue("@SessionID",GetSessionID());
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        catch { return 0; }
    }

    public static void LogLogout(object loginHistoryID)
    {
        try
        {
            if(loginHistoryID==null||string.IsNullOrWhiteSpace(loginHistoryID.ToString())) return;
            using(SqlConnection con=new SqlConnection(ConnectionString))
            {
                con.Open();
                string sql="UPDATE UserLoginHistory SET LogoutTime=GETDATE() WHERE LoginHistoryID=@LoginHistoryID AND LogoutTime IS NULL";
                using(SqlCommand cmd=new SqlCommand(sql,con))
                {
                    cmd.Parameters.AddWithValue("@LoginHistoryID",loginHistoryID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch { }
    }

    public static void LogActivity(string userID,string role,string actionType,string module,string pageName,string recordType,string recordID,string description)
    {
        try
        {
            using(SqlConnection con=new SqlConnection(ConnectionString))
            {
                con.Open();
                string sql="INSERT INTO UserActivityLog(UserID,UserRole,ActionType,Module,PageName,RecordType,RecordID,Description,ActivityTime,IPAddress,SessionID) VALUES(@UserID,@UserRole,@ActionType,@Module,@PageName,@RecordType,@RecordID,@Description,GETDATE(),@IPAddress,@SessionID)";
                using(SqlCommand cmd=new SqlCommand(sql,con))
                {
                    cmd.Parameters.AddWithValue("@UserID",userID??"");
                    cmd.Parameters.AddWithValue("@UserRole",role??"");
                    cmd.Parameters.AddWithValue("@ActionType",actionType??"");
                    cmd.Parameters.AddWithValue("@Module",(object)module??DBNull.Value);
                    cmd.Parameters.AddWithValue("@PageName",(object)pageName??DBNull.Value);
                    cmd.Parameters.AddWithValue("@RecordType",(object)recordType??DBNull.Value);
                    cmd.Parameters.AddWithValue("@RecordID",(object)recordID??DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description",(object)description??DBNull.Value);
                    cmd.Parameters.AddWithValue("@IPAddress",GetIPAddress());
                    cmd.Parameters.AddWithValue("@SessionID",GetSessionID());
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch { }
    }

    private static string GetIPAddress()
    {
        try
        {
            if(HttpContext.Current==null) return "";
            string forwarded=HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if(!string.IsNullOrWhiteSpace(forwarded)) return forwarded.Split(',')[0].Trim();
            return HttpContext.Current.Request.UserHostAddress??"";
        }
        catch { return ""; }
    }

    private static string GetUserAgent()
    {
        try { return HttpContext.Current==null||HttpContext.Current.Request.UserAgent==null?"":HttpContext.Current.Request.UserAgent; }
        catch { return ""; }
    }

    private static string GetSessionID()
    {
        try { return HttpContext.Current==null||HttpContext.Current.Session==null?"":HttpContext.Current.Session.SessionID; }
        catch { return ""; }
    }
}