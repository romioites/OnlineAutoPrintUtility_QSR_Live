using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Collections;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using TuchScreenApp1Jan2013;
using TuchScreenApp1Jan2013.App_Code;
using KOTPrintUtility;

namespace HiraSweets.App_Code
{
   public class Synchronize_Sql_CloudDB_Connection
    {
        private SqlConnection con;
        private SqlCommand cmd;
        private static Synchronize_Sql_CloudDB_Connection oADO;
        private DataTable dt;
        private DataSet ds;
        static bool IsInserted = false;


        private Synchronize_Sql_CloudDB_Connection()
    {
        con = new SqlConnection();
        string scon = Program.sqlkey_CloudDB;             
        con.ConnectionString = scon;
        cmd = new SqlCommand();
        cmd.Connection = con;
    }
    public static Synchronize_Sql_CloudDB_Connection GetObject
    {
        get
        {

            if (oADO == null)
                oADO = new Synchronize_Sql_CloudDB_Connection();
            return oADO;
        }
    }
    //Open a connection
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
    public void quit(Form frm)
    {
        frm.Close();
        frm = null;
    }
    //Public Functions
    //------BOOLIAN RETURN TYPE---------------------------------------------------------------------------------------
    public bool ExecuteDML(string sql) //Used for Insert,Delete and Modify records
    {
        if (!OpenCon()) return false;
        //sql = convertQuote(sql);
        cmd.CommandText = sql;
        try
        {
            int nor = cmd.ExecuteNonQuery();
            if (nor <= 0)
                return false;
            return true;
        }
        catch(Exception ex)
        {
           
            return false;
        }
    }
    public DataSet GetDatset(string qry, string tbl)
    {
        if (!OpenCon()) return null;
        ds = new DataSet();
        try
        {
            SqlDataAdapter da = new SqlDataAdapter(qry, con);
            da.Fill(ds, tbl);
            return ds;
        }
        catch(Exception ex)
        {
           
            return null;
        }
        finally
        {
            con.Close();
        }
    }
    public string GetSingleResult(string sql) //Get single record
    {
        if (!OpenCon()) return "false";
        cmd.CommandText = sql;
        try
        {
            object data = cmd.ExecuteScalar();
            return data.ToString();
        }
        catch (Exception ex)
        {
         
            return "0";//"Not Found"
        }
    }
      
        //public static bool Insert_FOC_Report(string Bill_No, string Bill_amount, string Discount_by, string remarks, string Report_Date,
        //                                    string created_by)
        //{

        //    try
        //    {                              
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = "Insert_Update_FOC_Report";
        //        cmd.Parameters.AddWithValue("@Bill_No", Bill_No);
        //        cmd.Parameters.AddWithValue("@Bill_amount", Bill_amount);
        //        cmd.Parameters.AddWithValue("@Discount_by", Discount_by);
        //        cmd.Parameters.AddWithValue("@remarks", remarks);
        //        cmd.Parameters.AddWithValue("@Report_Date", Report_Date);
        //        cmd.Parameters.AddWithValue("@created_by", created_by);
        //        result = cmd.ExecuteNonQuery();
        //        if (result > 0)
        //        {
        //            IsInserted = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        cmd.Connection.Close();
        //    }
        //    return IsInserted;
        //}
    public DataTable GetTable(string query)
    {
        if (!OpenCon()) return null;
        dt = new DataTable();
        try
        {
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            this.con.Close();
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            
            return null;
        }
    }
    }
}
