using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KOTPrintUtility.App_Code
{
    class clsConnnectionManager
    {
        public  SqlConnection SQlConnection_Online()
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(Program.sqlKeyOnline);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                else
                {
                    con.Open();
                }
            }
            catch { }
            return con;
        }
		public SqlConnection SQlConnection_Offline()
		{
			SqlConnection con = null;
			try
			{
				con = new SqlConnection(Program.sqlkey);
				if (con.State == ConnectionState.Open)
				{
					con.Close();
				}
				else
				{
					con.Open();
				}
			}
			catch { }
			return con;
		}
	}
}
