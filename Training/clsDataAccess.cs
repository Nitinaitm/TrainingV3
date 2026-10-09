using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Collections.Generic;

public class clsDataAccess
{
    private DataTable _dt; private int _vic; private Page _page; private string _id;
    SqlConnection con = new SqlConnection(); SqlTransaction Trans;
    public SqlConnection Connection { get { return con; } }
    public SqlTransaction Transaction { get { return Trans; } }
    public clsDataAccess() { string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString; con = new SqlConnection(connectionString); }
    private void SetAuditContext()
    {
        try
        {
            if (HttpContext.Current == null || con.State != ConnectionState.Open) return;
            string userID = HttpContext.Current.Session == null ? "" : (HttpContext.Current.Session["UserID"] ?? HttpContext.Current.Session["EmpID"] ?? "").ToString();
            string role = HttpContext.Current.Session == null || HttpContext.Current.Session["Role"] == null ? "" : HttpContext.Current.Session["Role"].ToString();
            string sessionID = HttpContext.Current.Session == null ? "" : HttpContext.Current.Session.SessionID;
            string ip = HttpContext.Current.Request.UserHostAddress ?? "";
            using (SqlCommand cmd = new SqlCommand("EXEC sys.sp_set_session_context @key=N'UserID',@value=@UserID; EXEC sys.sp_set_session_context @key=N'UserRole',@value=@UserRole; EXEC sys.sp_set_session_context @key=N'IPAddress',@value=@IPAddress; EXEC sys.sp_set_session_context @key=N'SessionID',@value=@SessionID;", con))
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@UserRole", role);
                cmd.Parameters.AddWithValue("@IPAddress", ip);
                cmd.Parameters.AddWithValue("@SessionID", sessionID);
                cmd.ExecuteNonQuery();
            }
        }
        catch { }
    }
    private void LogError(string source, string query, Exception ex)
    {
        try
        {
            string logPath = HttpContext.Current.Server.MapPath("~/App_Data/ErrorLog.txt");

            string entry =
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                " | " + source +
                " | " + ex.Message +
                " | Query: " + query +
                Environment.NewLine;

            System.IO.File.AppendAllText(logPath, entry);
        }
        catch
        {
            // If logging itself fails, never let it break the page.
        }
    }
    public DataTable GetDataTable(string query)
    {
        DataTable dt = new DataTable(); try { con.Open(); SetAuditContext(); SqlCommand cmd = new SqlCommand(); cmd.CommandText = query; SqlDataAdapter adap = new SqlDataAdapter(); cmd.Connection = con; adap.SelectCommand = cmd; adap.Fill(dt); return dt; }  catch (Exception ex)
        {
            LogError("GetDataTable(query)", query, ex);
            return dt;
        } finally { con.Close(); }
    }
    public DataTable GetDataTable(string query, SqlParameter[] param)
    {
        DataTable dt = new DataTable(); try { con.Open(); SetAuditContext(); SqlCommand cmd = new SqlCommand(); cmd.CommandText = query; if (param != null) foreach (SqlParameter prm in param) cmd.Parameters.Add(prm); SqlDataAdapter adap1 = new SqlDataAdapter(); cmd.Connection = con; adap1.SelectCommand = cmd; adap1.Fill(dt); return dt; } catch (Exception ex)
        {
            LogError("GetDataTable(query, param)", query, ex);
            return dt;
        } finally { con.Close(); }
    }
    public int ExecuteSql(string Query)
    {
        try { con.Open(); SetAuditContext(); SqlCommand cmd = new SqlCommand(); cmd.CommandText = Query; cmd.Connection = con; return cmd.ExecuteNonQuery(); } catch (Exception) { return 0; } finally { con.Close(); }
    }
    public int ExecuteSql(string Query, SqlParameter[] param)
    {
        try { con.Open(); SetAuditContext(); SqlCommand cmd = new SqlCommand(); cmd.CommandText = Query; if (param != null) foreach (SqlParameter prm in param) cmd.Parameters.Add(prm); cmd.Connection = con; return cmd.ExecuteNonQuery(); } catch (Exception ex) { HttpContext.Current.Response.Write(ex.Message); return 0; } finally { con.Close(); }
    }
    public int ExecuteSql(string Query, List<SqlParameter> param, Label lblMsg)
    {
        try { con.Open(); SetAuditContext(); SqlCommand cmd = new SqlCommand(); cmd.CommandText = Query; foreach (SqlParameter prm in param) cmd.Parameters.Add(prm); cmd.Connection = con; return cmd.ExecuteNonQuery(); } catch (Exception ex) { lblMsg.Text = ex.Message; return 0; } finally { con.Close(); }
    }
    public string ExecuteScalar(string strSql)
    {
        SqlCommand cmd = new SqlCommand(); try { cmd.CommandType = CommandType.Text; cmd.CommandText = strSql; cmd.Connection = con; cmd.Connection.Open(); SetAuditContext(); return cmd.ExecuteScalar().ToString(); } catch (Exception) { cmd.Connection.Close(); return ""; } finally { cmd.Connection.Close(); }
    }
    public void OpenConnection() { SqlCommand cmd = new SqlCommand(); if (con.State == ConnectionState.Closed) con.Open(); SetAuditContext(); cmd.Connection = con; }
    public void CloseConnection() { con.Close(); }
    public void BeginTransaction() { if (con.State == ConnectionState.Closed) con.Open(); SetAuditContext(); Trans = con.BeginTransaction(IsolationLevel.Serializable); }
    public void BeginTransaction(IsolationLevel level) { if (con.State == ConnectionState.Closed) con.Open(); SetAuditContext(); SqlCommand cmd = new SqlCommand(); Trans = con.BeginTransaction(level); cmd.Transaction = Trans; }
    public void Commit() { if (Trans != null) { Trans.Commit(); Trans.Dispose(); Trans = null; } if (con.State == ConnectionState.Open) con.Close(); }
    public void Rollback() { if (Trans != null) { Trans.Rollback(); Trans.Dispose(); Trans = null; } if (con.State == ConnectionState.Open) con.Close(); }
    public object ExecuteScalar(string Query, SqlParameter[] param)
    {
        SqlCommand cmd = new SqlCommand(); try { cmd.CommandType = CommandType.Text; cmd.CommandText = Query; cmd.Connection = con; cmd.Connection.Open(); SetAuditContext(); if (param != null) foreach (SqlParameter prm in param) cmd.Parameters.Add(prm); object objRet = cmd.ExecuteScalar(); if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close(); return objRet; } catch (Exception) { if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close(); return null; } finally { if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close(); }
    }
    public int ExecuteSql(string query, SqlParameter[] param, SqlTransaction trans)
    {
        try { SqlCommand cmd = new SqlCommand(); cmd.CommandText = query; cmd.Connection = trans.Connection; cmd.Transaction = trans; if (param != null) foreach (SqlParameter prm in param) cmd.Parameters.Add(prm); return cmd.ExecuteNonQuery(); } catch { throw; }
    }
    public object ExecuteScalar(string query, SqlParameter[] param, SqlTransaction trans)
    {
        SqlCommand cmd = new SqlCommand(); cmd.CommandText = query; cmd.Connection = trans.Connection; cmd.Transaction = trans; if (param != null) foreach (SqlParameter prm in param) cmd.Parameters.Add(prm); return cmd.ExecuteScalar();
    }
    public DataTable GetDataTable(string query, SqlParameter[] param, SqlTransaction trans)
    {
        DataTable dt = new DataTable(); SqlCommand cmd = new SqlCommand(); cmd.CommandText = query; cmd.Connection = trans.Connection; cmd.Transaction = trans; if (param != null) foreach (SqlParameter prm in param) cmd.Parameters.Add(prm); SqlDataAdapter da = new SqlDataAdapter(cmd); da.Fill(dt); return dt;
    }
    public SqlCommand CreateCommand()
    {
        SqlCommand cmd = new SqlCommand(); cmd.Connection = con; if (Trans != null) cmd.Transaction = Trans; return cmd;
    }
}