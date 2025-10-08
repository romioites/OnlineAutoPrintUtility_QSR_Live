using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KOTPrintUtility.App_Code
{
    class clsConfigSettings
    {
        public static string IsBarcodeOrderReceiving = "0";

        public static string Bill_no_FK = string.Empty;
        public static string SMS_HD = string.Empty;
        public static string SMS_TA = string.Empty;
        public static string SMS_DN = string.Empty;
		
			public static string Log_Drive = string.Empty;
		public static string isKOT_3or4_EnchPrint = string.Empty;
        public static string Dual = string.Empty;
        public static string Dual_Display = string.Empty;
        public static string DeliveryCharge = "0";
        public static string ServiceTax = "0";
        public static string ServiceCharge = "0";
        public static string SurCharge = "0";
        public static string S_Charge_HD = "0";
        public static string IsItemSeperate = "0";

        public static string MailHost_Email = string.Empty;
        public static string Password_Email = string.Empty;
        public static string FromMail_Email = string.Empty;

        //public static string sqlKey = string.Empty;
        public static string Coupon_Item = string.Empty;
        public static string POSCode = string.Empty;
        public static string DiscountCap = string.Empty;
        public static string Print_Cap = string.Empty;
        public static string validate_ip = string.Empty;
        public static string PortName = string.Empty;

        public static string TaxCap = string.Empty;
        public static string Kot_Cut = string.Empty;
        public static string Day_End_Validation = string.Empty;
        public static string Validation_Date = string.Empty;
        public static string IsShowStockOut = string.Empty;

        public static string TenantID = string.Empty;
        public static string POS_NO = string.Empty;
        public static string ShiftNo = string.Empty;
        public static string IsTraining = string.Empty;

        public static string TRAN_STATUS = string.Empty;
        public static string host_log = string.Empty;
        public static string User_log = string.Empty;
        public static string pass_log = string.Empty;
        public static string header = string.Empty;
        public static string IsPrintSettlement = string.Empty;

        public static string TA_Stax = string.Empty;
        public static string DI_Stax = string.Empty;
        public static string HD_Stax = string.Empty;

        public static string TA_Sur_CH = string.Empty;
        public static string DI_Sur_CH = string.Empty;
        public static string HD_Sur_CH = string.Empty;

        public static string Print_No_of_HD_Bill = string.Empty;
        public static string KDS_Active = string.Empty;

        public static string EXE_Constants = string.Empty;
        public static string Validate_IP = string.Empty;
        public static string IsHdPC = string.Empty;


        public static string SpeechText = string.Empty;
        public static string IsNotifyOrder = "0";
        public static string Bill_Head = "INVOICE";
        public static string Print_hsn = "0";

        public static string SoftwareTitle = string.Empty;
        public static string loginExe = string.Empty;
        public static string Genrate_File = string.Empty;
        public static string MainFromTitle = string.Empty;
        public static string Noof_bill_TA = string.Empty;
        public static string Print_No_of_CB_Bill = string.Empty;
        public static string GRN_Cap = string.Empty;
        public static string SBC_Rate = string.Empty;
        public static string KKC_Rate = string.Empty;

        //End Here..........
        //For Email Configuration

        public static string dsc_damage_Email = string.Empty;
        public static string void_KOT_Email = string.Empty;
        public static string void_bill_Email = string.Empty;
        public static string Nc_Bill_Email = string.Empty;
        public static int PortNo = 0;
        //public static string Order_rec_variance_Email = string.Empty;
        public static string Bill_reprint_Email = string.Empty;
        public static string Hold_bill_cancelation_Email = string.Empty;
        public static string Start_Sub_Dept = string.Empty;
        public static string ShowTax_Dual_display = string.Empty;



        public static string Order_rec_variance = string.Empty;
        public static string Modify_bill_Email = string.Empty;
        public static string report_Email = string.Empty;
        public static string hold_bill_Email = string.Empty;
        public static string MailHost = string.Empty;
        public static string Password = string.Empty;
        public static string FromMail = string.Empty;
        public static string Food_Order_Email = string.Empty;
        public static string IsTabOrder = string.Empty;
        public static string QtyPriceAsk = string.Empty;
        public static string Settlement_optin_HD = string.Empty;
        public static string NoofPrint_KOT_TA = string.Empty;
        public static string NoofPrint_KOT_DN = string.Empty;
        public static string NoofPrint_KOT_HD = string.Empty;

        public static string CRMCap_TA = string.Empty;
        public static string CRMCap_DN = string.Empty;
        public static string IsActiveMgr = string.Empty;
        public static string Cutkot_UrgentItem = string.Empty;
        public static string CRM_Open_Cap = "0";
        public static string DelCharge_type = string.Empty;
        public static string GST_on_delivery = string.Empty;
        public static string Report_validation = string.Empty;
        public static string Print_poweredby = string.Empty;
        public static string MasterPrinter = string.Empty;
        public static string StockOutTime = string.Empty;
        public static string IsFill_Finishitem_DSC = "0";
        public static string DualPlayDualVideo = "0";
        public static string Aggr_Outlet_Id = "0";
        public static bool IsStop = false;
        public static string EnableGues_DualDisplay = "0";
        public static string IsTestingOnlineOrdering = "0";
        public static string EnableFeedback = "0";
        public static string dob = "";
        public static string IsClosingAmountSend = "0";
        public static string IsBillSend = "0";
        public static bool IsFeedbackTaken = false;
        public static string OpenCashDrawer = "0";
        public static string PrintComboRate = "0";
        public static string IsShiftCValidation = "0";
        public static string Weekoff_Validation = "0";
        public static string IsSkiponTA_bill = "1";
        public static string sqlkey_primary = "0";
        public static string Security_Deposit = "0";
        public static int Card_Expire_Days = 365;
        public static string Recharge_Amt = "10000";
        public static string PrintRemarksOnKot = "0";
        public static string IsopenCardSelect_inHDSettle = "0";
        public static string SingleKOT = "0";

        public static string pkg_charge_dn = "0";
        public static string pkg_charge_TA = "0";

        public static string Ischangeinyear = "0";
        public static string Ischangeinyear_msg = "0";
        public static string fin_year = "0";
        public static string IsPrintBackDateBill = "0";
        public static string IsEDC_Mandatory = "0";
        public static string print_size = "0";
        public static string Initial = "";

        public static string is_Print_HD_BIll = string.Empty;
        public static string is_Print_HD_KOT = string.Empty;
        public static string isPrintZomatoIdOnKotHD = "1";
    }
}
