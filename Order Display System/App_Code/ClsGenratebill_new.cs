using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KOTPrintUtility.App_Code
{
    class ClsGenratebill_new
    {
        public bool InsertMultipleAddress(string Addr_ID, string Mobile_no, string Address, string AnotherPinCode, string AnotherLandmark, string txtAddress, out string Tranid, string Flat_street, string Location)
        {
            SqlCommand cmd;
            int result = 0;
            bool IsSuccess = false;
            string tran_id = string.Empty;
            string sql = string.Empty;
            string localSqlKey = ConfigurationSettings.AppSettings["sqlKey"].ToString();
            SqlConnection Sqlcon = new SqlConnection(localSqlKey);
            try
            {
                cmd = new SqlCommand();
                Sqlcon.Open();
                cmd.Connection = Sqlcon;
                cmd.CommandText = "usp_insert_Update_customer_tran";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Mobile_no", Mobile_no);
                cmd.Parameters.AddWithValue("@Address", Address);
                cmd.Parameters.AddWithValue("@PinNo", AnotherPinCode);
                cmd.Parameters.AddWithValue("@Landmark", AnotherLandmark);
                cmd.Parameters.AddWithValue("@Mobile_No2", Mobile_no);
                cmd.Parameters.AddWithValue("@outlet_id", Program.Outlet_id);
                cmd.Parameters.AddWithValue("@status", "0");
                cmd.Parameters.AddWithValue("@AddressMain", txtAddress);
                cmd.Parameters.AddWithValue("@Addr_id", Addr_ID);
                cmd.Parameters.AddWithValue("@Flat_street", Flat_street);
                cmd.Parameters.AddWithValue("@Location", Location);
                cmd.Parameters.Add("@tran_id", SqlDbType.Int);
                cmd.Parameters["@tran_id"].Direction = ParameterDirection.Output;
                result = Convert.ToInt32(cmd.ExecuteScalar());
                if (result > 0)
                {
                    tran_id = result.ToString();
                    IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                IsSuccess = false;
            }
            Sqlcon.Close();
            if (tran_id == "")
                tran_id = Addr_ID;
            Tranid = tran_id;
            Program.Addr_ID = Tranid;
            return IsSuccess;
        }
    }
}
