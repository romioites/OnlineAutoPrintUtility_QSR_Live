using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TuchScreenApp1Jan2013.UI;
using System.Configuration;
using KOTPrintUtility.UI;
using System.Threading;

namespace KOTPrintUtility
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static int ReadData = Convert.ToInt32((ConfigurationSettings.AppSettings["ReadData"].ToString()));
        public static string Outlet_name = string.Empty;
        public static string Bill_Date = string.Empty;
        public static string KOTNO = string.Empty;
        public static string CurrentKOTNO = string.Empty;
        public static string BillType = string.Empty;
        public static string OrderRemarks = string.Empty;     
        public static string CompanyName = string.Empty;
		public static string InstalledVersion = string.Empty;
		public static string AvailableVersion = string.Empty;
		public static string Outlet_Name = string.Empty;
        public static string TableName = string.Empty;
        public static string CashierName = string.Empty;
        public static string TableCover = string.Empty;
        public static string DayEnd_BIllingDate = string.Empty;
        public static string TableNo = string.Empty;
        public static string bill_SNno = string.Empty;
        public static string Footer1 = string.Empty;
        public static string Footer2 = string.Empty;
        public static string Footer3 = string.Empty;
        public static string sqlKeyOnline = string.Empty;
		public static string sqlkey = ConfigurationSettings.AppSettings["sqlkey"].ToString();
		public static string Outlet_id = string.Empty;
        public static string Cust_code = string.Empty;
        public static string Supply_State = string.Empty;
        public static string Company_Stateid = string.Empty;
        public static string Addr_ID = string.Empty;
        //public static string Assorted_Item = string.Empty;
        public static string Source_of_order = string.Empty;
        public static string online_Order_comment = string.Empty;
        public static string online_Order_PunchBy = string.Empty;
        public static string Delivery_Charge = string.Empty;
        public static string DiscountCouponNo = string.Empty;
        public static string paymentModeType = string.Empty;
        //public static string OnlineOrderNo = string.Empty;
        public static string Card_Type = string.Empty;
        public static string CardAmount = string.Empty;
        public static string Initial = string.Empty;
        public static string Version = string.Empty;
        public static string Company_City = string.Empty;
        public static string Address = string.Empty;
        public static string TinNo = string.Empty;
        public static string PhoneNo = string.Empty;

        public static string ServiceCharge = string.Empty;
        public static string STax_No = string.Empty;
        public static string cin_no = string.Empty;
        public static string fssai_no = string.Empty;
        public static string MobileNo = string.Empty;
        public static string Discount_Amt_Show = string.Empty;
        //public static string bill_no_WebOrder = string.Empty;
        public static string ZomatoOrderNo = string.Empty;
        public static string External_Source_id = string.Empty;
        public static string online_Payment_Mode = string.Empty;
        //public static string Discount_Pct = string.Empty;
        public static string Assorted_ItemIndex = string.Empty;
        //public static string dis_Type = string.Empty;
        //public static string bill_no_TabOrder = string.Empty;
        public static string LiveSaleUploadAPI = "";
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

					   
			Mutex mutex = new System.Threading.Mutex(false, "OrderPrintUtility");
			try
			{
				if (mutex.WaitOne(0, false))
				{
					// Run the application
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new Data_Center());
				}
				else
				{
					Application.Run(new MessageFromcs());					
					//MessageBox.Show("An instance of the application is already running.");
				}
			}
			finally
			{
				if (mutex != null)
				{
					mutex.Close();
					mutex = null;
				}
			}


			
        }
    }
}
