using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KOTPrintUtility.App_Code
{
    class clsGetTaxDetail
    {
        public static double GetNewRateTran(string i_code, string NetAmount)
        {
            double tax = 0;
            try
            {
                string scon = ConfigurationSettings.AppSettings["sqlKey"].ToString();

                using (SqlConnection con = new SqlConnection(scon))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.Usp_GetTaxItemWise"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@i_code", i_code);
                        cmd.Parameters.AddWithValue("@NetAmount", NetAmount);
                        con.Open();
                        tax = Convert.ToDouble(cmd.ExecuteScalar());
                        con.Close();
                    }
                }
            }
            catch { }
            return tax;
        }
    }
}
