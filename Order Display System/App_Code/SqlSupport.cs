using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KOTPrintUtility.App_Code
{
    class SqlSupport
    {
        DataGridView dgvobj = new DataGridView();
        DataTable dtvobj = new DataTable();
        int Row_Index = 0;
        public void SetDataGrid(DataGridView dgv)
        {
            dgvobj = dgv;
        }
        public DataGridView GetDataGrid()
        {
            return dgvobj;
        }
        string _connectionString = ConfigurationSettings.AppSettings["sqlKey"].ToString();
        // string _connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString.ToString();
        public SqlSupport()
        {

        }

        public DataTable GetDataTableBySqlQuery(string SqlQuery)
        {
            DataTable retrunValue = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlQuery;
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(retrunValue);
                    }
                }
            }
            return retrunValue;
        }


        public DataTable GetDataTableBySqlProcedure(string SqlProcedure)
        {
            DataTable retrunValue = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(retrunValue);
                    }
                }
            }
            return retrunValue;
        }

        public DataTable GetDataTableBySqlProcedure(string SqlProcedure, SqlParameter[] pram)
        {
            DataTable retrunValue = new DataTable();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(pram);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(retrunValue);
                    }
                }
            }
            return retrunValue;
        }



        public DataSet GetDataSetBySqlQuery(string SqlQuery)
        {
            DataSet retrunValue = new DataSet();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlQuery;
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(retrunValue);
                    }
                }
            }
            return retrunValue;
        }


        public DataSet GetDataSetBySqlProcedure(string SqlProcedure)
        {
            DataSet retrunValue = new DataSet();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(retrunValue);
                    }
                }
            }
            return retrunValue;
        }

        public DataSet GetDataSetBySqlProcedure(string SqlProcedure, SqlParameter[] pram)
        {
            DataSet retrunValue = new DataSet();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(pram);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(retrunValue);
                    }
                }
            }
            return retrunValue;
        }

        public bool ExecuteNonQueryBySqlQuery(string SqlQuery)
        {
            bool retrunValue = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlQuery;
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    if (rowAffected > 0)
                    {
                        retrunValue = true;
                    }
                }
            }
            return retrunValue;
        }

        public bool ExecuteNonQueryBySqlProcedure(string SqlProcedure, SqlParameter[] pram)
        {
            bool retrunValue = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(pram);
                    con.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    if (rowAffected > 0)
                    {
                        retrunValue = true;
                    }
                }
            }
            return retrunValue;
        }

        public int ExecuteNonQueryBySqlProcedureGetRowAffected(string SqlProcedure, SqlParameter[] pram)
        {
            int retrunValue;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(pram);
                    con.Open();
                    retrunValue = cmd.ExecuteNonQuery();
                }
            }
            return retrunValue;
        }

        public bool ExecuteNonQueryBySqlProcedure(string SqlProcedure)
        {
            bool retrunValue = false;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    if (rowAffected > 0)
                    {
                        retrunValue = true;
                    }
                }
            }
            return retrunValue;
        }

        public object GetObjectBySqlProcedure(string SqlProcedure, SqlParameter[] pram)
        {
            object retrunValue = new object();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlProcedure;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(pram);
                    con.Open();
                    retrunValue = cmd.ExecuteScalar();
                }
            }
            return retrunValue;
        }

        public object GetObjectBySqlQuery(string SqlQuery)
        {
            object retrunValue = new object();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandTimeout = 10000;
                    cmd.CommandText = SqlQuery;
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    retrunValue = cmd.ExecuteScalar();
                }
            }
            return retrunValue;
        }

        public DataSet FillReportDataSet(string SQlQuery, DataSet TypeDataObject, params string[] TableMappingOrder)
        {
            DataSet retrunValue = new DataSet();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                TypeDataObject = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(SQlQuery, con);
                da.TableMappings.Add("Table", TableMappingOrder[0].ToString().Trim());

                for (int i = 1; i < TableMappingOrder.Length; i++)
                {
                    da.TableMappings.Add("Table" + i, TableMappingOrder[i].ToString().Trim());
                }
                da.Fill(retrunValue); // Fill Data   
            }
            return retrunValue;
        }
    }
}
