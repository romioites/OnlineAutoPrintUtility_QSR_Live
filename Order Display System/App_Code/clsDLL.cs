using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System.Net.NetworkInformation;

namespace KOTPrintUtility.App_Code
{
    class clsDLL
    {

        /// <summary>
        /// UpdateRemarks
        /// </summary>
        /// <param name="bill_No"></param>
        /// <param name="Order_Done_by"></param>
        /// <returns></returns>
        public static bool UpdateRemarks(string KOT_NO, string remarks)
        {
            bool result = false;
            try
            {
                string scon = ConfigurationSettings.AppSettings["sqlKey"].ToString();

                using (SqlConnection con = new SqlConnection(scon))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.Usp_UpdateRemarks"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@KOT_NO", KOT_NO);
                        cmd.Parameters.AddWithValue("@remarks", remarks);
                        cmd.Parameters.AddWithValue("@created_by", "KDS");
                        con.Open();
                        int count = cmd.ExecuteNonQuery();
                        if (count > 0)
                        {
                            result = true;
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            { 

            }
            return result;
        }

        /// <summary>
        /// GetAct_Time
        /// </summary>
        /// <param name="bill_No"></param>
        /// <param name="Order_Done_by"></param>
        /// <param name="Remarks"></param>
        /// <returns></returns>
        public static string GetAct_Time(string bill_No, string Order_Done_by, out string Remarks)
        {
            string str = string.Empty;
            Remarks = string.Empty;
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
                        DataTable dt = new DataTable();
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            str = Convert.ToString(dt.Rows[0]["Result"].ToString());
                            Remarks = Convert.ToString(dt.Rows[0]["Remarks"].ToString());
                        }
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
                //MessageBox.Show("Error" + ex.Message);
                throw ex;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
            return dt;
        }


        public DataTable GetImage_ofItem(string i_code_fk,string kot_no)
        {
            DataTable dt = null;
            SqlCommand cmd = null;
            try
            {
                string sql = "Usp_GetDishPics";
                cmd = new SqlCommand(sql);
                cmd.Connection = cls_SqlConnection.SqlConnectionOffline();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", i_code_fk);
                cmd.Parameters.AddWithValue("@kot_no", kot_no);
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

        public DataSet DisplayOrderKDS()
        {
            DataSet ds = null;
            SqlCommand cmd = null;
            try
            {
                string sql = "Usp_GetOrder_KOTNO";
                cmd = new SqlCommand(sql);
                cmd.Connection = cls_SqlConnection.SqlConnectionOffline();
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@Kot_No", Kot_No);
                //cmd.Parameters.AddWithValue("@id_fk", Id_fk);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                ds = new DataSet();
                adp.Fill(ds);

            }
            catch (Exception ex)
            {
                throw ex; 
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
            return ds;
        }

        public DataTable  DisplayOrderKDSTran(String KOT_No,String Bill_No)
        {
            DataTable ds = null;
            SqlCommand cmd = null;
            try
            {
                string sql = "Usp_GetItemsKot";
                cmd = new SqlCommand(sql);
                cmd.Connection = cls_SqlConnection.SqlConnectionOffline();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@KOT_No", KOT_No);
                cmd.Parameters.AddWithValue("@Bill_No", Bill_No);
               
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                ds = new DataTable();
                adp.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
            return ds;
        }


        public static bool PingPrinter(string Validate_IP)
        {
            bool IsRunning = false;
            try
            {                

                Ping p = new Ping();
                //try
                //{
                if (Validate_IP.Length > 0)
                {
                    PingReply reply = p.Send(Validate_IP, 3000);
                    if (reply.Status == IPStatus.Success)
                        IsRunning = true;
                }
                else
                {
                    IsRunning = true;
                }
            }
            catch
            {
                IsRunning = false;
            }
            return IsRunning;
        }
    }
}
