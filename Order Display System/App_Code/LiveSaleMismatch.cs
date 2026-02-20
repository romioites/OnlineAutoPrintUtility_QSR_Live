using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Script.Serialization;

namespace KOTPrintUtility.App_Code
{
    class LiveSaleMismatch
    {
        private Timer tmrStartSaleMismatch;
        public async Task StartSaleMismatch()
        {
            Loging.Log(LogType.Information, "Sale Mismatch started");
            GetandCompareOnlineOfflineSale(Program.Outlet_id);
            // Initialize the timer
            tmrStartSaleMismatch = new Timer(2000000); // 30 min
            tmrStartSaleMismatch.Elapsed += async (s, e) =>
            {
                try
                {
                    tmrStartSaleMismatch.Stop();
                    await GetandCompareOnlineOfflineSale(Program.Outlet_id);
                }
                catch (Exception ex)
                {
                    Loging.Log(LogType.Error, "Timer Tick error: " + ex.Message);
                }
                finally
                {
                    tmrStartSaleMismatch.Start();
                }
            };
            tmrStartSaleMismatch.AutoReset = true;
            tmrStartSaleMismatch.Start();
        }
        public static async Task GetandCompareOnlineOfflineSale(string outlet_id)
        {
            ADOC objADOC = new ADOC();
            Loging.Log(LogType.Information, "SaleMismatch.start");
            DataTable dtBill = new DataTable();
            SqlCommand cmd = new SqlCommand();

            try
            {
                SqlConnection con = new SqlConnection(Program.sqlkey);
                //con.Open();	
                string sql = "Usp_GetbillDetailtoCompare";
                cmd = new SqlCommand(sql);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dtBill);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();

                //read bill date wise
                for (int L = 0; L < dtBill.Rows.Count; L++)
                {
                    string bill_date = dtBill.Rows[L]["bill_date"].ToString();
                    // get online bill detail datewise
                    DataTable dtOnline = GetOnlineSale(bill_date, outlet_id);
                    // get offline bill detail datewise
                    DataTable dtOffline = GetOfflineSale(bill_date);
                    // read offline sale bill no wise
                    int Isscount = 0;
                    bool Result = false;
                    for (int Off = 0; Off < dtOffline.Rows.Count; Off++)
                    {
                        double Billamt_local = Convert.ToDouble(dtOffline.Rows[Off]["Bill_Amount"].ToString());
                        double tranamt_local = Convert.ToDouble(dtOffline.Rows[Off]["Amount_tran"].ToString());
                        double pmtsett_amt_local = Convert.ToDouble(dtOffline.Rows[Off]["Amount_pmtsett"].ToString());
                        double Tax1local = Convert.ToDouble(dtOffline.Rows[Off]["tax_amt"].ToString());
                        string Bill_no = dtOffline.Rows[Off]["Bill_no"].ToString();
                        string up_flag = dtOffline.Rows[Off]["up_flag"].ToString();
                        // compare bill detail on basis of bill_no
                        DataRow[] result = dtOnline.Select("Bill_no='" + Bill_no + "'");
                        if (result.Length > 0)
                        {
                            double Billamt_online = 0; double tranamt_online = 0; double pmtsettamt_online = 0; double TaxAmt_online = 0;
                            foreach (DataRow row in result)
                            {
                                Billamt_online = Convert.ToDouble(row["Bill_Amount"]);
                                tranamt_online = Convert.ToDouble(row["Amount_tran"]);
                                pmtsettamt_online = Convert.ToDouble(row["Amount_pmtsett"]);
                                TaxAmt_online = Convert.ToDouble(row["tax_amt"]);
                            }
                            if ((Billamt_local != Billamt_online) || (tranamt_local != tranamt_online) || (pmtsett_amt_local != pmtsettamt_online) || (Tax1local != TaxAmt_online))
                            {
                                // if mismatched online and offline
                                Loging.Log(LogType.Information, "SaleMismatch bill no:" + Bill_no);
                                Isscount++;
                                objADOC.ExecuteDML("Usp_UpdateBillStatus_offfline @bill_no='" + Bill_no + "',@mode='1'");
                                //Synchronize.GetObject.ExecuteDML("Usp_InsertErrorlog_service @error='Amount not matched',@outlet_id='" + outlet_id + "',@bill_no='" + Bill_no + "',@bill_Date='" + bill_date + "'");
                            }
                            else
                            {
                                // countinue row is matching bill wise with online
                                Result = true;
                                if (up_flag == "0")
                                {
                                    // update up_flag =1 in local, if up_flag=0 and matching with online(already uploded)
                                    objADOC.ExecuteDML("Usp_UpdateBillStatus_offfline @bill_no='" + Bill_no + "',@mode='4'");
                                }
                                continue;
                            }
                        }
                        else
                        {
                            // not found in online
                            Isscount++;
                            objADOC.ExecuteDML("Usp_UpdateBillStatus_offfline @bill_no='" + Bill_no + "',@mode='2'");
                            //Synchronize.GetObject.ExecuteDML("Usp_InsertErrorlog_service @error='Not found in online',@outlet_id='" + outlet_id + "',@bill_no='" + Bill_no + "',@bill_Date='" + bill_date + "'");
                        }
                    }
                    if (Isscount <= 0 && Result == true)
                    {
                        // sale matched online and off line for the day
                        objADOC.ExecuteDML("Usp_UpdateBillStatus_offfline @bill_date='" + bill_date + "',@mode='3'");
                        //Synchronize.GetObject.ExecuteDML("Usp_InsertErrorlog_service @error='online sale matched: " + bill_date + "',@outlet_id='" + outlet_id + "',@bill_no='0'");
                        Loging.Log(LogType.Information, "SaleMismatch.end" + bill_date);
                    }
                }
            }

        }


        public static DataTable GetOnlineSale(string bill_date, string Outlet_id)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                SqlConnection con = new SqlConnection(Program.sqlKeyOnline);
                //con.Open();
                string sql = "Usp_GetBillDetailToCompare_Online";
                cmd = new SqlCommand(sql);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bill_date", bill_date);
                cmd.Parameters.AddWithValue("@Outlet_id", Outlet_id);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adp.Fill(dt);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// Offline sale
        /// </summary>
        /// <param name="bill_date"></param>
        /// <returns></returns>
        public static DataTable GetOfflineSale(string bill_date)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();
            try
            {
                SqlConnection con = new SqlConnection(Program.sqlkey);
                //con.Open();
                string sql = "Usp_GetBillDetailToCompare_offline";
                cmd = new SqlCommand(sql);
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bill_date", bill_date);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                dt = new DataTable();
                adp.Fill(dt);
            }
            catch (Exception ex)
            {

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
