using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace KOTPrintUtility.App_Code
{
    class cls_ConfigurationMaster
    {
        public DataSet GetHeader()
        {
            SqlCommand cmd = null;
            DataSet ds = new DataSet();
            try
            {
                string sql = "Usp_GetCompany_Mast_OrderPrint";
                cmd = new SqlCommand(sql);
                cmd.Connection = cls_SqlConnection.SqlConnectionOffline();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1200;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
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
        public bool GetConfigurationDetail()
        {
            bool Result = false;
            ////-----------Start Header--------------------//////////
            DataTable dtheader = new DataTable();
            DataTable dtConfigFile = new DataTable();
            DataTable dtEmailConfig = new DataTable();
            DataTable dtLoyaltycard = new DataTable();
            // DataTable dtZomato = new DataTable();
            DataSet dsHeadCap = new DataSet();
            DataSet dsConfigCap = new DataSet();
            string ExpiryDate = string.Empty;
            DataTable dtCRMConfig = new DataTable();
            try
            {
                dsHeadCap = GetHeader();
                string Support_No = string.Empty;
                if (dsHeadCap.Tables.Count > 0)
                {
                    dtheader = dsHeadCap.Tables[0];
                    dtConfigFile = dsHeadCap.Tables[1];
                    dtEmailConfig = dsHeadCap.Tables[2];
                    dtLoyaltycard = dsHeadCap.Tables[3];
                    dtCRMConfig = dsHeadCap.Tables[4];
                    #region Tbl_company_master Details
                    if (dtheader.Rows.Count > 0)
                    {
                        Program.Outlet_id = dtheader.Rows[0]["outlet_id"].ToString();
                        Program.sqlKeyOnline = dtheader.Rows[0]["sqlkey_erp"].ToString();
                        Program.CompanyName = dtheader.Rows[0]["Company_Name"].ToString();
                        Program.Outlet_Name = dtheader.Rows[0]["Outlet_Name"].ToString();

                        Program.Address = dtheader.Rows[0]["address"].ToString();
                        string tinno = dtheader.Rows[0]["Tin_No"].ToString();
                        if (tinno.Trim().Length > 0)
                            Program.TinNo = tinno;
                        string PhoneNo = dtheader.Rows[0]["phone_no"].ToString();
                        if (PhoneNo.Trim().Length > 0)
                            PhoneNo = dtheader.Rows[0]["phone_no"].ToString();

                        Program.Outlet_id = dtheader.Rows[0]["Outlet_ID"].ToString();
                        ////-----------End Heder--------------------//////////  
                        ExpiryDate = dtheader.Rows[0]["Validity"].ToString();
                        Program.Footer1 = dtheader.Rows[0]["Footer1"].ToString();
                        Program.Footer2 = dtheader.Rows[0]["Footer2"].ToString();
                        Program.Footer3 = dtheader.Rows[0]["Footer3"].ToString();
                        Program.ServiceCharge = dtheader.Rows[0]["ServiceCharge"].ToString();
                        string STaxNo = dtheader.Rows[0]["stax_no"].ToString();
                        if (STaxNo.Trim().Length > 0)
                            Program.STax_No = STaxNo;
                        //Program.TinNo = dtheader.Rows[0]["Tin_No"].ToString();
                        Program.cin_no = dtheader.Rows[0]["cin_no"].ToString();
                        Program.fssai_no = dtheader.Rows[0]["fssai_no"].ToString();
                        string CIN_No = dtheader.Rows[0]["cin_no"].ToString();
                        string FSSAI_No = dtheader.Rows[0]["fssai_no"].ToString();
                        if (CIN_No.Length > 2)
                            Program.cin_no = CIN_No;
                        if (FSSAI_No.Length > 2)
                            Program.fssai_no = FSSAI_No;
                        Program.Outlet_name = dtheader.Rows[0]["Outlet_Name"].ToString();
                        Program.DayEnd_BIllingDate = dtheader.Rows[0]["Bill_date"].ToString();
						Program.AvailableVersion = dtConfigFile.Rows[0]["AvailableVersion"].ToString();

                        if (dtheader.Columns.Contains("LiveSaleUploadApi"))
                        {
                            Program.LiveSaleUploadAPI = dtheader.Rows[0]["LiveSaleUploadApi"].ToString();
                        }
                        else
                        {
                            Program.LiveSaleUploadAPI = "";
                        }
                    }
                    else
                    {

                    }
                    #endregion

                    #region Global Variable of Configuration Master
                    if (dtConfigFile.Rows.Count > 0)
                    {
                        clsConfigSettings.ServiceCharge = Convert.ToDouble(dtConfigFile.Rows[0]["Service_Charge"].ToString()).ToString();
                        clsConfigSettings.ServiceTax = Convert.ToDouble(dtConfigFile.Rows[0]["Service_Tax"].ToString()).ToString();
                        clsConfigSettings.SurCharge = Convert.ToDouble(dtConfigFile.Rows[0]["Sur_charge"].ToString()).ToString();
                        clsConfigSettings.DeliveryCharge = Convert.ToDouble(dtConfigFile.Rows[0]["DeliveryCharge"].ToString()).ToString();

                        clsConfigSettings.Coupon_Item = dtConfigFile.Rows[0]["Coupon_Item"].ToString();

                        clsConfigSettings.Log_Drive = dtConfigFile.Rows[0]["Log_Drive"].ToString();
                        if (clsConfigSettings.Log_Drive == "")
                            clsConfigSettings.Log_Drive = "C";
                        //clsConfigSettings.POSCode = ConfigurationSettings.AppSettings["POSCode"].ToString();


                        // clsApplicationVariables.clsMainFrom.POSCode = DLL.GetPOS();
                        clsConfigSettings.Print_Cap = dtConfigFile.Rows[0]["Print_Cap"].ToString();
                        clsConfigSettings.validate_ip = dtConfigFile.Rows[0]["validate_ip"].ToString();
                        clsConfigSettings.PortName = dtConfigFile.Rows[0]["PortName"].ToString();
                        clsConfigSettings.Kot_Cut = dtConfigFile.Rows[0]["Kot_Cut"].ToString();

                        clsConfigSettings.DiscountCap = dtConfigFile.Rows[0]["DiscountCap"].ToString();
                        clsConfigSettings.POS_NO = dtConfigFile.Rows[0]["POS_NO"].ToString();
                        clsConfigSettings.SBC_Rate = Convert.ToDouble(dtConfigFile.Rows[0]["SBC_Tax"].ToString()).ToString();
                        clsConfigSettings.KKC_Rate = Convert.ToDouble(dtConfigFile.Rows[0]["KKC_Tax"].ToString()).ToString();
                        clsConfigSettings.Day_End_Validation = dtConfigFile.Rows[0]["Day_End_Validation"].ToString();
                        clsConfigSettings.ShiftNo = dtConfigFile.Rows[0]["ShiftNo"].ToString();
                        clsConfigSettings.IsTraining = dtConfigFile.Rows[0]["IsTraining"].ToString();

                        clsConfigSettings.Validation_Date = dtConfigFile.Rows[0]["Validation_Date"].ToString();
                        clsConfigSettings.TRAN_STATUS = dtConfigFile.Rows[0]["TRAN_STATUS"].ToString();

                        clsConfigSettings.host_log = dtConfigFile.Rows[0]["host_log"].ToString();
                        clsConfigSettings.TenantID = dtConfigFile.Rows[0]["TenantID"].ToString();
                        clsConfigSettings.pass_log = dtConfigFile.Rows[0]["pass_log"].ToString();
                        clsConfigSettings.User_log = dtConfigFile.Rows[0]["User_log"].ToString();



                        clsConfigSettings.header = dtConfigFile.Rows[0]["header"].ToString();
                        clsConfigSettings.IsPrintSettlement = dtConfigFile.Rows[0]["IsPrintSettlement"].ToString();

                        clsConfigSettings.TA_Stax = dtConfigFile.Rows[0]["TA_Stax"].ToString();
                        clsConfigSettings.DI_Stax = dtConfigFile.Rows[0]["DI_Stax"].ToString();
                        clsConfigSettings.HD_Stax = dtConfigFile.Rows[0]["HD_Stax"].ToString();
                        clsConfigSettings.TA_Sur_CH = dtConfigFile.Rows[0]["TA_Sur_CH"].ToString();
                        clsConfigSettings.DI_Sur_CH = dtConfigFile.Rows[0]["DI_Sur_CH"].ToString();
                        clsConfigSettings.HD_Sur_CH = dtConfigFile.Rows[0]["HD_Sur_CH"].ToString();
                        clsConfigSettings.Print_No_of_HD_Bill = dtConfigFile.Rows[0]["Print_No_of_HD_Bill"].ToString();
                        clsConfigSettings.KDS_Active = dtConfigFile.Rows[0]["KDS_Active"].ToString();


                        //clsConfigSettings.Day_End_Validation = dtConfigFile.Rows[0]["Day_End_Validation"].ToString();
                        //clsConfigSettings.Validation_Date = dtConfigFile.Rows[0]["Validation_Date"].ToString();
                        clsConfigSettings.IsShowStockOut = dtConfigFile.Rows[0]["IsShowStockOut"].ToString();
                        clsConfigSettings.EXE_Constants = dtConfigFile.Rows[0]["EXE_Constants"].ToString();
                        //clsConfigSettings.Validate_IP = dtConfigFile.Rows[0]["IsShowStockOut"].ToString();
                        clsConfigSettings.IsHdPC = dtConfigFile.Rows[0]["IsHdPC"].ToString();
                        clsConfigSettings.TaxCap = dtConfigFile.Rows[0]["TaxCap"].ToString();

                        clsConfigSettings.SoftwareTitle = dtConfigFile.Rows[0]["SoftwareTitle"].ToString();

                        clsConfigSettings.Dual_Display = dtConfigFile.Rows[0]["Dual_Display"].ToString();

                        clsConfigSettings.loginExe = dtConfigFile.Rows[0]["loginExe"].ToString();

                        clsConfigSettings.Genrate_File = dtConfigFile.Rows[0]["Genrate_File"].ToString();
                        clsConfigSettings.MainFromTitle = dtConfigFile.Rows[0]["MainFromTitle"].ToString();
                        clsConfigSettings.Noof_bill_TA = dtConfigFile.Rows[0]["Noof_bill_TA"].ToString();
                        clsConfigSettings.Print_No_of_CB_Bill = dtConfigFile.Rows[0]["Print_No_of_CB_Bill"].ToString();
                        clsConfigSettings.GRN_Cap = dtConfigFile.Rows[0]["GRN_Cap"].ToString();
                        clsConfigSettings.IsTabOrder = dtConfigFile.Rows[0]["IsTabOrder"].ToString();
                        clsConfigSettings.QtyPriceAsk = dtConfigFile.Rows[0]["QtyPriceAsk"].ToString();
                        clsConfigSettings.Settlement_optin_HD = dtConfigFile.Rows[0]["Settlement_optin_HD"].ToString();
                        clsConfigSettings.NoofPrint_KOT_TA = dtConfigFile.Rows[0]["NoofPrint_KOT_TA"].ToString();
                        clsConfigSettings.NoofPrint_KOT_DN = dtConfigFile.Rows[0]["NoofPrint_KOT_DN"].ToString();
                        clsConfigSettings.NoofPrint_KOT_HD = dtConfigFile.Rows[0]["NoofPrint_KOT_HD"].ToString();
                        clsConfigSettings.isKOT_3or4_EnchPrint = dtConfigFile.Rows[0]["isKOT_3or4_EnchPrint"].ToString();

                        clsConfigSettings.CRMCap_TA = dtConfigFile.Rows[0]["CRMCap_TA"].ToString();
                        clsConfigSettings.CRMCap_DN = dtConfigFile.Rows[0]["CRMCap_DN"].ToString();
                        clsConfigSettings.CRM_Open_Cap = dtConfigFile.Rows[0]["CRM_Open_Cap"].ToString();

                        clsConfigSettings.SMS_HD = dtConfigFile.Rows[0]["SMS_HD"].ToString();
                        clsConfigSettings.SMS_TA = dtConfigFile.Rows[0]["SMS_TA"].ToString();
                        clsConfigSettings.SMS_DN = dtConfigFile.Rows[0]["SMS_DN"].ToString();
                        //clsConfigSettings.IsActiveMgr = ConfigurationSettings.AppSettings["IsActiveMgr"].ToString();
                        clsConfigSettings.Cutkot_UrgentItem = dtConfigFile.Rows[0]["Cutkot_UrgentItem"].ToString();
                        clsConfigSettings.Start_Sub_Dept = dtConfigFile.Rows[0]["Start_Sub_Dept"].ToString();
                        clsConfigSettings.ShowTax_Dual_display = dtConfigFile.Rows[0]["ShowTax_Dual_display"].ToString();
                        // cls_ZomatoAPi.IsZomato = dtConfigFile.Rows[0]["IsZomato"].ToString();
                        clsConfigSettings.IsItemSeperate = dtConfigFile.Rows[0]["IsItemSeperate"].ToString();
                        clsConfigSettings.DelCharge_type = (dtConfigFile.Rows[0]["DelCharge_type"].ToString()).Trim();
                        clsConfigSettings.GST_on_delivery = (dtConfigFile.Rows[0]["GST_on_delivery"].ToString()).Trim();
                        clsConfigSettings.Report_validation = (dtConfigFile.Rows[0]["Report_validation"].ToString()).Trim();
                        clsConfigSettings.SpeechText = dtConfigFile.Rows[0]["SpeechText"].ToString().Trim();
                        clsConfigSettings.IsNotifyOrder = dtConfigFile.Rows[0]["IsNotifyOrder"].ToString().Trim();
                        clsConfigSettings.Bill_Head = dtConfigFile.Rows[0]["Bill_Head"].ToString().Trim();
                        clsConfigSettings.Print_hsn = dtConfigFile.Rows[0]["Print_hsn"].ToString().Trim();
                        clsConfigSettings.Print_poweredby = dtConfigFile.Rows[0]["PrintPoweredby"].ToString().Trim() + "(ver " + Program.Version + ")";
                        //clsConfigSettings.MasterPrinter = dtConfigFile.Rows[0]["MasterPrinter"].ToString().Trim();                    

                        clsConfigSettings.StockOutTime = dtConfigFile.Rows[0]["StockOutTime"].ToString().Trim();
                        clsConfigSettings.IsFill_Finishitem_DSC = dtConfigFile.Rows[0]["IsFill_Finishitem_DSC"].ToString().Trim();
                        clsConfigSettings.Aggr_Outlet_Id = dtConfigFile.Rows[0]["Aggr_Outlet_Id"].ToString().Trim();


                        clsConfigSettings.IsClosingAmountSend = dtConfigFile.Rows[0]["IsClosingAmountSend"].ToString().Trim();
                        clsConfigSettings.IsBillSend = dtConfigFile.Rows[0]["IsBillSend"].ToString().Trim();
                        clsConfigSettings.IsShiftCValidation = dtConfigFile.Rows[0]["IsShiftCValidation"].ToString().Trim();

                        clsConfigSettings.Weekoff_Validation = dtConfigFile.Rows[0]["Weekoff_Validation"].ToString().Trim();
                        clsConfigSettings.PrintComboRate = dtConfigFile.Rows[0]["PrintComboRate"].ToString().Trim();
                        clsConfigSettings.sqlkey_primary = dtheader.Rows[0]["sqlkey_primary"].ToString();
                        clsConfigSettings.Card_Expire_Days = Convert.ToInt32(dtConfigFile.Rows[0]["Card_Expire_Days"].ToString());
                        clsConfigSettings.Security_Deposit = dtConfigFile.Rows[0]["Security_Deposit"].ToString();
                        clsConfigSettings.PrintRemarksOnKot = dtConfigFile.Rows[0]["IsPrintRemarksOnKotHD"].ToString().Trim();
                        clsConfigSettings.IsopenCardSelect_inHDSettle = dtConfigFile.Rows[0]["IsopenCardSelect_inHDSettle"].ToString().Trim();
                        clsConfigSettings.SingleKOT = dtConfigFile.Rows[0]["SingleKOT"].ToString().Trim();

                        clsConfigSettings.pkg_charge_dn = dtConfigFile.Rows[0]["pkg_charge_dn"].ToString().Trim();
                        clsConfigSettings.pkg_charge_TA = dtConfigFile.Rows[0]["pkg_charge_TA"].ToString().Trim();
                        clsConfigSettings.Ischangeinyear = dtConfigFile.Rows[0]["Ischangeinyear"].ToString().Trim();
                        clsConfigSettings.Ischangeinyear_msg = dtConfigFile.Rows[0]["Ischangeinyear_msg"].ToString().Trim();
                        clsConfigSettings.fin_year = dtConfigFile.Rows[0]["fin_year"].ToString().Trim();
                        clsConfigSettings.IsPrintBackDateBill = dtConfigFile.Rows[0]["IsPrintBackDateBill"].ToString().Trim();
                        clsConfigSettings.IsEDC_Mandatory = dtConfigFile.Rows[0]["IsEDC_Live"].ToString().Trim();
                        clsConfigSettings.print_size = dtConfigFile.Rows[0]["print_size"].ToString().Trim();
                        clsConfigSettings.print_size = dtConfigFile.Rows[0]["print_size"].ToString().Trim();
                        if (dtConfigFile.Columns.Contains("Initial"))
                            clsConfigSettings.Initial = dtConfigFile.Rows[0]["Initial"].ToString().Trim();
                        //===========================================================Config=====================================
                        clsConfigSettings.MasterPrinter = ConfigurationManager.AppSettings["MasterPrinter"].ToString();

                        clsConfigSettings.is_Print_HD_BIll = ConfigurationManager.AppSettings["is_Print_HD_BIll"].ToString();
                        clsConfigSettings.is_Print_HD_KOT = ConfigurationManager.AppSettings["is_Print_HD_KOT"].ToString();
                        if (clsConfigSettings.is_Print_HD_BIll == "0")
                        {
                            clsConfigSettings.Print_No_of_HD_Bill = "0";
                        }
                        if (clsConfigSettings.is_Print_HD_KOT == "0")
                        {
                            clsConfigSettings.NoofPrint_KOT_TA = "0";
                            clsConfigSettings.NoofPrint_KOT_HD = "0";
                            clsConfigSettings.NoofPrint_KOT_DN = "0";
                        }
                        if (dtConfigFile.Columns.Contains("isPrintZomatoIdOnKotHD"))
                            clsConfigSettings.isPrintZomatoIdOnKotHD = dtConfigFile.Rows[0]["isPrintZomatoIdOnKotHD"].ToString().Trim();
                        Result = true;
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true); Cls_Exception.InsertException("1", Program.Bill_Date, Program.UserName, ex.Message, trace.GetFrame(0).GetFileLineNumber().ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, "cls_ConfigurationMaster.cs");
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                dtConfigFile.Dispose();
                dtEmailConfig.Dispose();
                dsConfigCap.Dispose();
            }
            return Result;
        }



	}
}
