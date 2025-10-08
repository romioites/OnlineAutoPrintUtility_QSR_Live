using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Order_Display_System.App_Code
{
    class cls_SqlConnection
    {
        public static SqlConnection SqlConnectionOffline()
        {
            SqlCommand cmd = null;
            SqlConnection con = null;
            cmd = new SqlCommand();
            try
            {
                string scon = ConfigurationSettings.AppSettings["sqlKey"].ToString();
                con = new SqlConnection(scon);
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

            }
            catch (SqlException ex)
            {
            }
            return con;

        }
    }
}
