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

public class ADOC
{
    //private var
    private SqlConnection con;
    private SqlCommand cmd;
    private static ADOC oADO;
    private DataTable dt;
    private DataSet ds;
    public static ADOC GetObject
    {
        get
        {

            if (oADO == null)
                oADO = new ADOC();
            return oADO;
        }
    }
    //C'tor
    private ADOC()
    {
        con = new SqlConnection();
        // Connection String   ->       
        //constr = ConfigurationSettings.AppSettings["sqlKey"].ToString();
        //string scon = "Server=192.168.1.100; uid=hira;pwd=panna; database=hirasweets";    
        string scon = ConfigurationSettings.AppSettings["sqlKey"].ToString();// "Data Source=.\\SQLEXPRESS;Initial Catalog=HiraSweets;Integrated Security=True";
        con.ConnectionString = scon;
        cmd = new SqlCommand();
        cmd.Connection = con;
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
        catch
        {
            return false;
        }
    }
    //--------STRING RETURN TYPE--------------------------------------------------------------------------------------
    public string GetSingleResult(string sql) //Get single record
    {
        if (!OpenCon()) return "false";
        cmd.CommandText = sql;
        try
        {
            object data = cmd.ExecuteScalar();
            return data.ToString();
        }
        catch
        {
            return "0";//"Not Found"
        }
    }
    //============================  Add some methods by AMAN =========================

    //*********************** Log File Write Logic **************************
    public static void writeToLogFile( string Events, string logMessage)
    {
        string strLogMessage = string.Empty;
        //string strLogFile = System.Configuration.ConfigurationManager.AppSettings["logFilePath"].ToString();
        string strLogFile = ConfigurationSettings.AppSettings["logFilePath"].ToString();
        StreamWriter swLog;

        strLogMessage = string.Format("{0} -- {1} --  {2}", DateTime.Now.ToString("dd-MM-yyyy:hh:mm:ss:tt"),Events, logMessage);

        if (!File.Exists(strLogFile))
        {
            swLog = new StreamWriter(strLogFile);
        }
        else
        {
            swLog = File.AppendText(strLogFile);
        }

        swLog.WriteLine(strLogMessage);
        swLog.WriteLine();
        swLog.Close();
    }  
    //*****************************************************************
  /// <summary>
  /// 
  /// </summary>
  /// <param name="To"></param>
  /// <param name="subject"></param>
  /// <param name="MailBody"></param>
  /// <returns></returns>
  /// 

    public static bool SendEmail(string To, string subject, string MailBody)
    {
        bool isMailSent = false;
        MailMessage message = new MailMessage("info@romiotech.com", To);
        message.Body = MailBody;
        message.Subject = subject;
        message.IsBodyHtml = true;
        SmtpClient emailClient = new SmtpClient();
        emailClient.Host = "mail.romiotech.com";
        emailClient.Credentials = new NetworkCredential("info@romiotech.com", "Hello32");
        emailClient.UseDefaultCredentials = true;
        emailClient.EnableSsl = false;
        emailClient.Send(message);
        isMailSent = false;
        return isMailSent;
    }
    public static bool IsValidEmailAddress(string InputEmailAddress)
    {
        return Regex.IsMatch(InputEmailAddress, @"[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z_+])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9}$");
    }

    public static DataTable RemoveDuplicate(DataTable dTable, string colName)
    {
        Hashtable hTable = new Hashtable();
        ArrayList duplicateList = new ArrayList();

        foreach (DataRow drow in dTable.Rows)
        {
            if (hTable.Contains(drow[colName]))
            {
                duplicateList.Add(drow);
            }
            else
            {
                hTable.Add(drow[colName], string.Empty);
            }
        }

        foreach (DataRow dRow in duplicateList)
            dTable.Rows.Remove(dRow);

        return dTable;
    }
    public static bool IsConnectedToInternet()
    {
        bool isConnected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        if (isConnected == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public ComboBox Bind_Combo(string strsql, ComboBox cmb)
    {
        if (!OpenCon()) OpenCon();
        cmd.CommandText = strsql;
        try
        {
           SqlCommand cmdd=new SqlCommand(strsql,con);
           SqlDataReader drr=cmdd.ExecuteReader();
            cmb.Items.Clear();
            while( drr.Read())
            {
                cmb.Items.Add(drr.ToString());
            }
        }
        catch
        {
           
        }
        return cmb;

    }

    //============================  Add some methods by AMAN =========================

    //public static DataTable RemoveDuplicateRows(DataTable SourceTable, params string[] FieldNames)
    //{
        //object[] lastValues;
        //DataTable newTable;
        //DataRow[] orderedRows;

        //if (FieldNames == null || FieldNames.Length == 0)
        //{
        //    throw new ArgumentNullException("FieldNames");
        //}
        //lastValues = new object[FieldNames.Length];
        //newTable = new DataTable();

        //foreach (string fieldName in FieldNames)
        //{
        //    newTable.Columns.Add(fieldName, SourceTable.Columns[fieldName].DataType);
        //}

        //orderedRows = SourceTable.Select("", string.Join(", ", FieldNames));

        //foreach (DataRow row in orderedRows)
        //{
        //    if (!fieldValuesAreEqual(lastValues, row, FieldNames))
        //    {
        //        newTable.Rows.Add(createRowClone(row, newTable.NewRow(), FieldNames));
        //        setLastValues(lastValues, row, FieldNames);
        //    }
        //}
        //return newTable;
    //}

    public object GetSingleResultObj(string sql) //Get single record
    {
        if (!OpenCon()) return "false";
        cmd.CommandText = sql;
        try
        {
            object data = cmd.ExecuteScalar();
            return data;
        }
        catch
        {
            return "0";//"Not Found"
        }
    }
    //-------SQLDATAREADER RETUNN TYPE--------------------------------------------------------------------------------
    public SqlDataReader GetDataByReader(string qry)//Get multipal records simple
    {
        if (!OpenCon()) return null;
        cmd.CommandText = qry;
        cmd.CommandType = CommandType.Text;
        SqlDataReader dr = null;
        try
        {
            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return dr;
        }
        catch
        {
            return null;
        }
        finally
        {
            //cmd.Parameters.Clear();
            //dr.Close();
        }
    }

    public SqlDataReader GetDataByReader(string qry, SqlParameter[] arrparam)//Get multipal records 
    {
        if (!OpenCon()) return null;
        cmd.CommandText = qry;
        cmd.CommandType = CommandType.Text;
        foreach (SqlParameter param in arrparam)
            cmd.Parameters.Add(param);
        try
        {
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return dr;

        }
        catch
        {

            return null;
        }
    }

    public SqlDataReader GetDataByReader(string qry, CommandType cmdtype, SqlParameter[] arrparam)//Get multipal records
    {
        if (!OpenCon()) return null;
        cmd.CommandText = qry;
        cmd.CommandType = cmdtype;
        cmd.CommandType = CommandType.StoredProcedure;
        foreach (SqlParameter param in arrparam)
            cmd.Parameters.Add(param);
        try
        {
            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return dr;
        }
        catch
        {
            return null;
        }
    }

    //made by ...........
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
        catch
        {
            return null;
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
        catch
        {
            return null;
        }
        finally
        {
            con.Close();
        }
    }
   public  DataSet FillReportDataSet(string SQlQuery,DataSet TypeDataObject, params string[] TableMappingOrder)
    {
        try
        {
          
                DataSet ds = new DataSet();
                TypeDataObject = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(SQlQuery, con);
                da.TableMappings.Add("Table", TableMappingOrder[0].ToString().Trim());
                for (int i = 1; i < TableMappingOrder.Length; i++)
                {
                    da.TableMappings.Add("Table" + i, TableMappingOrder[i].ToString().Trim());
                }
                da.Fill(TypeDataObject); // Fill Data
        
            
        }
        catch (Exception)
        {
            return TypeDataObject = null;
        }
        return TypeDataObject;

    }

    public string convertQuote(string s)
    {
        string newstr = "";
        int i, l, opt = 0;
        l = s.Length;
        if (s.Substring(0, 1) == "i") opt = 1;
        if (s.Substring(0, 1) == "u") opt = 2;
        if (s.Substring(0, 1) == "s" || s.Substring(0, 1) == "d") opt = 3;
        if (opt != 0)
        {
            if (opt == 1)
            {
                for (i = 0; i < l - 1; i++)
                {
                    if (s.Substring(i, 1) == "'" && s.Substring(i + 1, 1) != "'" && s.Substring(i + 1, 1) != "," && s.Substring(i + 1, 1) != ")")
                        if (i > 0)
                            if (s.Substring(i - 1, 1) != "," && s.Substring(i - 1, 1) != "(" && s.Substring(i - 1, 1) != " ")
                                newstr = newstr + "'";
                    newstr = newstr + s.Substring(i, 1);
                }
            }

            if (opt == 2)
            {
                newstr = newstr + s.Substring(0, 1);
                for (i = 1; i < l - 1; i++)
                {
                    if (s.Substring(i, 1) == "'" && s.Substring(i + 1, 1) != "," && s.Substring(i + 1, 1) != "'" && s.Substring(i - 1, 1) != "=" && s.Substring(i - 1, 1) != "'" && s.Substring(i + 1, 1) != " ")
                    {
                        newstr = newstr + "'";
                    }
                    newstr = newstr + s.Substring(i, 1);
                }
            }
            //////////
            if (opt == 3)
            {
                newstr = newstr + s.Substring(0, 1);
                for (i = 1; i < l - 1; i++)
                {
                    if (s.Substring(i, 1) == "'" && s.Substring(i + 1, 1) != "," && s.Substring(i + 1, 1) != "'" && s.Substring(i - 1, 1) != "=" && s.Substring(i - 1, 1) != "'" && s.Substring(i + 1, 1) != " " && s.Substring(i - 1, 1) != " " && s.Substring(i + 1, 1) != ")")
                    {
                        newstr = newstr + "'";
                    }
                    newstr = newstr + s.Substring(i, 1);
                }
            }
            ////////
            newstr = newstr + s.Substring(l - 1, 1);
        }
        else
            newstr = s;
        return newstr;
    }
    public string GetOutlet()
    {
        string OutletID = ConfigurationSettings.AppSettings["OutletID"].ToString();
        return OutletID;
    }
    //public object GetDatset(object p)
    //{
    //    throw new Exception("The method or operation is not implemented.");
    //}


}