using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TuchScreenApp1Jan2013.UI;
using System.Configuration;

namespace Order_Display_System
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static int ReadData = Convert.ToInt32((ConfigurationSettings.AppSettings["ReadData"].ToString()));
        public static string Bill_Date = string.Empty;
        public static string KOTNO = string.Empty;
        public static string CurrentKOTNO = string.Empty;
        public static string Bill_Type = string.Empty;
        public static string OrderRemarks = string.Empty;     
        public static string CompanyName = string.Empty;
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
