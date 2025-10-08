using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KOTPrintUtility.App_Code
{
    class Cls_Exception
    {
        public static void InsertException(string Mode, string Bill_Date, string UserName, string Exception, string CodeLine, string Function_Name, string Page_Name)
        {
            //DataTable dt = new DataTable();
            try
            {
                string sql = "dbo.Usp_Exception";
                SqlCommand cmd; cmd = new SqlCommand(sql);
                cmd.Connection = cls_SqlConnection.SqlConnectionOffline();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 3000;
                cmd.Parameters.AddWithValue("@Mode", Mode);
                cmd.Parameters.AddWithValue("@Bill_Date", Bill_Date);
                cmd.Parameters.AddWithValue("@UserName", UserName);
                cmd.Parameters.AddWithValue("@Exception", Exception);
                cmd.Parameters.AddWithValue("@CodeLine", CodeLine);
                cmd.Parameters.AddWithValue("@Function_Name", Function_Name);
                cmd.Parameters.AddWithValue("@Page_Name", Page_Name);
                //SqlDataAdapter adp = new SqlDataAdapter(cmd);
                //adp.Fill(dt);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            //return dt;
        }
    }
}
