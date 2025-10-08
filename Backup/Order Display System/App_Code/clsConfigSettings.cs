using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Order_Display_System.App_Code
{
    class clsConfigSettings
    {
        public static string IS_KOT_Print_dept = string.Empty;
        public static string IS_KOT_Print_Port = string.Empty;
        public static string Check_IP_PortWise = string.Empty;
        public static string PrinterName = string.Empty;
        public static string IsPrintDefaultKOT = string.Empty;
        public static string NC_Item = string.Empty;
        public static string LineOnKOT = string.Empty;
        public static string No_of_Kot = string.Empty;
        public static string isKOT_3or4_EnchPrint = string.Empty;
        public static string Multi_Printer = string.Empty;
        public static string PrintKOT_SectionWise = string.Empty;
        public static string BillNo_on_kot = string.Empty;
        public static string Initial = string.Empty;
        public static string PrintKOTRmarks_HD = string.Empty;
        public static string Take_Away = string.Empty;
        public static string Home_Delivery = string.Empty;
        public static string Single_Item_kot = string.Empty;
        public static string header = string.Empty;
        public static string Print_Cap = string.Empty;
        public static string Kot_Cut = string.Empty;
        public static string PortName = string.Empty;
        public void GetLoginData()
        {
            try
            {
                clsDLL objdll = new clsDLL();
                DataTable dtConfigFile = objdll.GetConfidDetailLoginTime();
                               
                clsConfigSettings.Print_Cap = dtConfigFile.Rows[0]["Print_Cap"].ToString();
                clsConfigSettings.PortName = dtConfigFile.Rows[0]["PortName"].ToString();
                clsConfigSettings.Kot_Cut = dtConfigFile.Rows[0]["Kot_Cut"].ToString();               
                clsConfigSettings.header = dtConfigFile.Rows[0]["header"].ToString();              
                clsConfigSettings.No_of_Kot = dtConfigFile.Rows[0]["No_of_Kot"].ToString();               
                clsConfigSettings.IS_KOT_Print_dept = dtConfigFile.Rows[0]["IS_KOT_Print_dept"].ToString();
                clsConfigSettings.IS_KOT_Print_Port = dtConfigFile.Rows[0]["IS_KOT_Print_Port"].ToString();               
                clsConfigSettings.PrinterName = dtConfigFile.Rows[0]["PrinterName"].ToString();               
                clsConfigSettings.Take_Away = dtConfigFile.Rows[0]["Take_Away"].ToString();
                clsConfigSettings.Home_Delivery = dtConfigFile.Rows[0]["Home_Delivery"].ToString();               
                clsConfigSettings.IsPrintDefaultKOT = dtConfigFile.Rows[0]["IsPrintDefaultKOT"].ToString();               
                clsConfigSettings.LineOnKOT = dtConfigFile.Rows[0]["LineOnKOT"].ToString();
                clsConfigSettings.NC_Item = dtConfigFile.Rows[0]["NC_Item"].ToString();              
                clsConfigSettings.Check_IP_PortWise = dtConfigFile.Rows[0]["Check_IP_PortWise"].ToString();
                clsConfigSettings.isKOT_3or4_EnchPrint = dtConfigFile.Rows[0]["isKOT_3or4_EnchPrint"].ToString();              
                clsConfigSettings.Kot_Cut = dtConfigFile.Rows[0]["KOT_Cut"].ToString();             
                clsConfigSettings.PrintKOTRmarks_HD = dtConfigFile.Rows[0]["PrintKOTRmarks_HD"].ToString();
                clsConfigSettings.Multi_Printer = dtConfigFile.Rows[0]["Multi_Printer"].ToString();               
                clsConfigSettings.PrintKOT_SectionWise = dtConfigFile.Rows[0]["PrintKOT_SectionWise"].ToString();
                clsConfigSettings.Initial = (dtConfigFile.Rows[0]["Initial"].ToString()).Trim();               
                clsConfigSettings.BillNo_on_kot = (dtConfigFile.Rows[0]["BillNo_on_kot"].ToString()).Trim();               
                clsConfigSettings.Single_Item_kot = (dtConfigFile.Rows[0]["Single_Item_kot"].ToString()).Trim();
            }
            catch { }
        }            
        
    }
}
