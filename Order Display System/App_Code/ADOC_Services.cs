using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace KOTPrintUtility.App_Code
{
    class ADOC_Services
    {
        private SqlConnection con;
        private SqlCommand cmd;
        public ADOC_Services oADO;
        private DataTable dt;
        private DataSet ds;
        DataTable objdt;
        public ADOC_Services GetObject
        {
            get
            {

                if (oADO == null)
                    oADO = new ADOC_Services();
                return oADO;
            }
        }
        //C'tor
        public ADOC_Services()
        {
            con = new SqlConnection();
            // Connection String   ->       
            //constr = ConfigurationSettings.AppSettings["sqlKey"].ToString();
            //string scon = "Server=192.168.1.100; uid=hira;pwd=panna; database=hirasweets";    
            string scon = ConfigurationSettings.AppSettings["sqlKey"].ToString();
            // "Data Source=.\\SQLEXPRESS;Initial Catalog=HiraSweets;Integrated Security=True";
            //string scon = ConfigurationManager.ConnectionStrings["constr"].ToString();
            con.ConnectionString = scon;
            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandTimeout = 10000;
        }
        bool OpenCon()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                return true;
            }
            catch
            {
                return false;

            }
        }
        //Close the open connection
        bool CloseCon()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DataSet GetDatset(string qry, string tbl)
        {           
            try
            {                
                SqlSupport sqlSupport = new SqlSupport();
                ds = sqlSupport.GetDataSetBySqlQuery(qry);
                if (ds != null && ds.Tables.Count > 0 && tbl.Equals(string.Empty))
                {
                    int index = 0;
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (index == 0)
                        {
                            dt.TableName = tbl;
                        }
                        else
                        {
                            dt.TableName = tbl + index.ToString();
                        }
                        index++;
                    }
                }
                return ds;
            }
            catch
            {
                return null;
            }
            finally
            {
                //   con.Close();
            }
        }
    }
}
