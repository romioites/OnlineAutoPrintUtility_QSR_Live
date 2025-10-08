using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace Order_Display_System.App_Code
{
    class clsDLL
    {

        public static string GetAct_Time(string bill_No, string Order_Done_by)
        {
            string str = string.Empty;
            try
            {
                string scon = ConfigurationSettings.AppSettings["sqlKey"].ToString();

                using (SqlConnection con = new SqlConnection(scon))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.Usp_Update_View_Time"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@bill_No", bill_No);
                        cmd.Parameters.AddWithValue("@Order_Done_by", Order_Done_by);                           
                        con.Open();
                        str = Convert.ToString(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
            }
            catch { }
            return str;
        }

        public DataSet GetConfidDetail(string Kot_No, string Id_fk)
        {
            DataSet ds = null;
            SqlCommand cmd = null;
            try
            {
                string sql = "Usp_GetConfigDetail_KDS";
                cmd = new SqlCommand(sql);
                cmd.Connection = cls_SqlConnection.SqlConnectionOffline();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Kot_No", Kot_No);
                cmd.Parameters.AddWithValue("@id_fk", Id_fk);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                ds = new DataSet();
                adp.Fill(ds);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
            return ds;
        }



        public DataTable GetConfidDetailLoginTime()
        {
            DataTable dt = null;
            SqlCommand cmd = null;
            try
            {
                string sql = "Usp_GetConfigDetail";
                cmd = new SqlCommand(sql);
                cmd.Connection = cls_SqlConnection.SqlConnectionOffline();
                cmd.CommandType = CommandType.StoredProcedure;                
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adp.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
            return dt;
        }
    }
}
