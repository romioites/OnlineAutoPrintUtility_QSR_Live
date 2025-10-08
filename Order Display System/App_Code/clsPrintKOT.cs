using KOTPrintUtility.App_Dataset;
using KOTPrintUtility.Report;
using LPrinterTest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TuchScreenApp1Jan2013.App_Code;
using TuchScreenApp1Jan2013.App_Dataset;

using TuchScreenApp1Jan2013.UI;

namespace KOTPrintUtility.App_Code
{
    public class clsPrintKOT
    {
        private const char ESC = (char)27;
        private const char LF = (char)10;
        private const char FS = (char)28;
        private const char GS = (char)29;
        ADOC objADOC = new ADOC();



        private DataTable GetAssortedItemToPrint(Int64 ItemCode, DataTable dt, string DishGroup)
        {
            DataTable dtobj = null;
            try
            {
                var currentStatRow = (from currentStat in dt.AsEnumerable()
                                      where currentStat.Field<Int64>("i_code_fk") == ItemCode && currentStat.Field<String>("item_index") == DishGroup
                                      select currentStat);
                if (currentStatRow.Count() > 0)
                {
                    dtobj = new DataTable();
                    dtobj = currentStatRow.CopyToDataTable();
                }
            }
            catch (Exception ex)
            {
            }
            return dtobj;
        }

        public void KOTPrint4Inch(string Send, string companyName, string Header, string KOT_Type, string BillNo, DataTable dt2,
          string LineonKOT, DataGridView objDgv, DateTime date, string Time, DataTable dt, out string SendOut, string OrderNo,
          string CustName, string MobileNo, string CounterNo, string OrderComment, string Channel, string Channel_Order_Id)
        {
            string str1 = string.Empty;
            // print outlet name 
            //// underline printing************
            #region
            //Send = Send + ESC + "-" + (char)1;
            //Send = Send + companyName + LF;
            //Send = Send + ESC + "-" + (char)0;
            #endregion
            //set alllinment of line center**********************************************************************
            Send = Send + ESC + "a" + (char)(1);
            //Send = Send + companyName + LF + LF;
            //LF= For new line
            // BOld************
            Send = Send + ESC + "!" + (char)(16 + 32 + 1);             //1 - font B; 32 = Double width               
            Send = Send + companyName + LF + LF;
            // BOld************

            Send = Send + ESC + "!" + (char)(8);
            // print header 
            if (Header.Length > 0)
                Send = Send + Header + LF;
            // print sub header
            Send = Send + ESC + "a" + (char)(0);
            //set alllinment of line center off and set to left**********************************************************************

            Send = Send + " Bill Type : " + KOT_Type + LF;

            Send = Send + " Bill No   : " + BillNo + LF;
            Send = Send + " Date      : " + date.ToString("dd-MM-yyyy") + "   Time : " + Time + LF;
            Send = Send + " Counter No: " + CounterNo + LF;
            Send = Send + ESC + "!" + (char)(31 + 8 + 1);
            if (CustName.Length > 0)
                Send = Send + "->: " + CustName + LF + LF;
            if (MobileNo.Length > 0)
                Send = Send + "->: " + MobileNo + LF;
            Send = Send + ESC + "a" + (char)(1);
            Send = Send + ESC + "!" + (char)(16 + 16 + 2);             //1 - font B; 32 = Double width 
            Send = Send + " Order No  : " + OrderNo + LF;
            Send = Send + ESC + "!" + (char)(8);
            Send = Send + ESC + "a" + (char)(0);
            Send = Send + "===============================================" + LF;
            Send = Send + " QTY             ITEMS                      " + LF;
            Send = Send + "===============================================" + LF;

            //Send = Send + ESC + "!" + (char)(6 + 26 + 1);             //1 - font B; 32 = Double width           
            Send = Send + ESC + "!" + (char)(16 + 32 + 1);             //1 - font B; 32 = Double width           
            for (int b = 0; b < dt2.Rows.Count; ++b)
            {
                string item_code = dt2.Rows[b]["item_code"].ToString();
                string i_name = dt2.Rows[b]["i_name"].ToString();
                //string UrgentItem = dt2.Rows[b]["UrgentItem"].ToString();
                string item_index = dt2.Rows[b]["Group_dish"].ToString();

                DataTable dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(item_code), dt, item_index);

                if (i_name.Length < 36)
                {
                    for (int cn = i_name.Length; cn <= 35; ++cn)
                        i_name += " ";
                }
                //if (item_index == "0")
                //{
                //str1 = "" + i_name + "     " + dt2.Rows[b]["qty"].ToString() + LF;
                str1 = " " + dt2.Rows[b]["qty"].ToString() + "  " + i_name + LF;
                Send = Send + str1;


                //}
                if (dt2.Rows[b]["Comments"].ToString() != "")
                {
                    Send = Send + " >>" + dt2.Rows[b]["Comments"].ToString() + LF;
                }
                int count = 0;
                var query = from DataGridViewRow row in objDgv.Rows
                            where row.Cells["AddonCode_fk"].Value.ToString() == item_code.ToString()
                            select new
                            {
                                item_name = row.Cells["ItemName"].Value.ToString(),
                                orderqty = row.Cells["qty"].Value.ToString(),
                                totalamount = row.Cells["Total"].Value.ToString(),
                                DishComment = row.Cells["DishComment"].Value.ToString()
                            };

                foreach (var qry in query)
                {
                    string Addoni_name = qry.item_name.ToString();
                    if (Addoni_name.Length >= 30)
                        Addoni_name = Addoni_name.Substring(0, 30);
                    string qty = qry.orderqty.ToString();
                    double amount = Convert.ToDouble(qry.totalamount.ToString());
                    string DishComments = qry.DishComment.ToString();
                    if (Addoni_name.Length < 36)
                    {
                        for (int cn = Addoni_name.Length; cn <= 35; ++cn)
                            Addoni_name += " ";
                        count++;
                    }
                    //str1 = "" + Addoni_name + "     " + qty + LF;
                    str1 = " " + qty + "  " + Addoni_name + LF;
                    Send = Send + str1;
                    if (DishComments != "" && DishComments != "")
                    {
                        Send = Send + " >>" + DishComments + LF;
                    }
                    if (count > 0)
                        Send = Send + LF;
                }
                if (dtDoughnuts != null)
                {
                    for (int i = 0; i < dtDoughnuts.Rows.Count; i++)
                    {
                        string item_name = dtDoughnuts.Rows[i]["i_name"].ToString();
                        string aid = dtDoughnuts.Rows[i]["id"].ToString();
                        item_name = "*" + item_name;
                        if (item_name.Length < 36)
                        {
                            for (int dn = item_name.Length; dn <= 35; ++dn)
                                item_name += " ";
                        }
                        //string PrintQuery = "" + item_name + "     " + dtDoughnuts.Rows[i]["Qty"].ToString() + LF;
                        string PrintQuery = " " + dtDoughnuts.Rows[i]["Qty"].ToString() + "  " + item_name + LF;
                        Send = Send + PrintQuery;
                    }
                }
            }
            Send = Send + ESC + "!" + (char)(8);
            Send = Send + "===============================================" + LF;
            Send = Send + " User -   " + "Utility" + LF;
            if (OrderComment.Length > 0)
                Send = Send + " Order Comment -   " + OrderComment + LF;


            Send = Send + "===============================================" + LF;
            // Full cut**********************************************************************
            Send = Send + GS + "V" + (char)66 + (char)0;
            Send = Send + GS + "r" + (char)1;


            if (Channel_Order_Id.Trim().Length > 0)
            {
                Send = Send + ESC + "a" + (char)(1);
                Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                Send = Send + Channel + " (" + Channel_Order_Id.Trim() + ")" + LF;
                Send = Send + ESC + "!" + (char)(8);
                Send = Send + ESC + "a" + (char)(0);
            }

            // Full cut***********************************************************************
            SendOut = Send;
        }


        /// <summary>
        /// BigFontBigSize KOT counter/dept wise
        /// </summary>
        /// <param name="NoofPrint"></param>
        /// <param name="ContactNo"></param>
        /// <param name="custName"></param>
        /// <param name="BillNo"></param>
        /// <param name="objDgv"></param>
        /// <param name="OrderNo"></param>
        public void PrintKOTBigFontBigSize(int NoofPrint, string ContactNo, string custName, string BillNo,
       List<tbl_tran> objDgv, string OrderNo, string OrderComment)
        {
            string Channel = string.Empty;
            string Channel_Order_Id = string.Empty;
            string strMasterPrinter = string.Empty;
            try
            {
                PrinterSettings settings = new PrinterSettings();
                string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                LPrinter PosPrinter = new LPrinter();
                string CurrentPrinter = settings.PrinterName;


                DateTime date = DateTime.ParseExact(Program.DayEnd_BIllingDate, "yyyy-MM-dd", null);
                string Time = System.DateTime.Now.ToShortTimeString();
                ////////////////Print KOT //////////////////////////////////////
                string Bill_Type = string.Empty;
                if (Program.BillType == "C")
                    Bill_Type = "Take Away";
                else if (Program.BillType == "H")
                    Bill_Type = "Home Delivery";
                else
                {
                    Bill_Type = "Dine-In";
                }
                DataTable dt1 = new DataTable();

                string header = clsConfigSettings.header.ToString();
                int CountKOT = 0;
                // Get priner according to item and counter
                string Send = null;


                string SendOut = string.Empty;
                DataSet ds = objADOC.GetObject.GetDatset("Usp_Get_Printer_by_Counter_Normal @Bill_No_FK='" + BillNo + "'", "tbl_dish_mast");
                dt1 = ds.Tables[0];
                DataTable dt = ds.Tables[1];
                PosPrinter.Print(PaperFullCut);
                for (int a = 0; a < dt1.Rows.Count; ++a)
                {
                    //CurrentPrinter = settings.PrinterName;
                    if (dt1.Columns.Contains("Channel"))
                    {
                        Channel = dt1.Rows[a]["Channel"].ToString();
                        Channel_Order_Id = dt1.Rows[a]["Channel_Order_Id"].ToString();
                    }
                    string[] strPrinter = dt1.Rows[a]["printer"].ToString().Split(',');
                    for (int PCount = 0; PCount < strPrinter.Length; PCount++)
                    {
                        CurrentPrinter = settings.PrinterName;
                        //string Printter_DB = dt1.Rows[a]["printer"].ToString();
                        string Printter_DB = strPrinter[PCount].ToString();

                        if ((Printter_DB.ToLower() == "none" || Printter_DB.ToLower().Trim() == "0"))
                        {
                            continue;
                        }
                        else if (Printter_DB != "0" && Printter_DB.ToLower() != "none" && Printter_DB.ToLower() != "default")
                        {
                            CurrentPrinter = Printter_DB;
                        }
                        PosPrinter.Open("testDoc", CurrentPrinter);
                        string str1 = "Usp_Get_Item_KOT_Counter_Normal @Bill_No_FK='" + BillNo + "',@counter_id='" + dt1.Rows[a]["c_code"].ToString() + "'";
                        //search item by counter id
                        DataTable dt2 = objADOC.GetObject.GetTable(str1);
                        Send = ESC + "@";
                        string CounterNo = dt1.Rows[a]["counter"].ToString();
                        for (int PrintIndex = 0; PrintIndex < NoofPrint; PrintIndex++)
                        {
                            //KOTPrint4Inch(Send, Program.CompanyName, clsConfigSettings.header, Bill_Type, BillNo, dt2, "0", objDgv, date, Time, dt, out SendOut, OrderNo, custName, ContactNo, CounterNo);
                            //KOTPrint4Inch(Send, Program.CompanyName, Program.Address, Bill_Type, BillNo, dt2, "0", objDgv, date, Time, dt, out SendOut, OrderNo, custName, ContactNo, CounterNo, OrderComment, Channel, Channel_Order_Id);

                            try
                            {
                                PosPrinter.Print(SendOut);
                                Send = null;
                                SendOut = null;
                            }
                            catch (Exception ex)
                            {
                                PosPrinter.Close();
                                Send = null;
                                SendOut = null;
                            }
                        }
                        PosPrinter.Close();
                    }
                }
                strMasterPrinter = clsConfigSettings.MasterPrinter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (strMasterPrinter.Length > 0)
            {
                //PrintKOTBigFontBigSize_MasterKOT(NoofPrint, ContactNo, custName, BillNo, objDgv, OrderNo, strMasterPrinter, OrderComment, Channel, Channel_Order_Id);
            }
        }

        /// <summary>
        /// Master KOT in Big FONT
        /// </summary>
        /// <param name="NoofPrint"></param>
        /// <param name="ContactNo"></param>
        /// <param name="custName"></param>
        /// <param name="BillNo"></param>
        /// <param name="objDgv"></param>
        /// <param name="OrderNo"></param>
        public void PrintKOTBigFontBigSize_MasterKOT(int NoofPrint, string ContactNo, string custName, string BillNo,
    DataGridView objDgv, string OrderNo, string MasterPrinterName, string OrderComment, string Channel, string Channel_Order_Id)
        {
            try
            {
                PrinterSettings settings = new PrinterSettings();
                string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                LPrinter PosPrinter = new LPrinter();
                //string CurrentPrinter = MasterPrinterName;

                DateTime date = DateTime.ParseExact(Program.DayEnd_BIllingDate, "yyyy-MM-dd", null);
                string Time = System.DateTime.Now.ToShortTimeString();
                ////////////////Print KOT //////////////////////////////////////
                string Bill_Type = string.Empty;
                if (Program.BillType == "C")
                    Bill_Type = "Take Away";
                else if (Program.BillType == "H")
                    Bill_Type = "Home Delivery";
                else
                {
                    Bill_Type = "Dine-In";
                }
                DataTable dt1 = new DataTable();

                string header = clsConfigSettings.header.ToString();
                int CountKOT = 0;
                // Get priner according to item and counter
                string Send = null;

                string SendOut = string.Empty;
                DataSet ds = objADOC.GetObject.GetDatset("Usp_Get_Printer_by_Counter @Bill_No_FK='" + BillNo + "'", "tbl_dish_mast");
                dt1 = ds.Tables[0];
                DataTable dt = ds.Tables[1];
                PosPrinter.Print(PaperFullCut);

                string[] strPrinter = MasterPrinterName.Split(',');
                for (int PCount = 0; PCount < strPrinter.Length; PCount++)
                {
                    string CurrentPrinter = strPrinter[PCount];
                    PosPrinter.Open("testDoc", CurrentPrinter);
                    string str1 = "Usp_Get_Item_MasterKOT @Bill_No_FK='" + BillNo + "'";
                    //search item by counter id
                    DataTable dt2 = objADOC.GetObject.GetTable(str1);
                    Send = ESC + "@";
                    string CounterNo = "Master KOT";
                    for (int PrintIndex = 0; PrintIndex < NoofPrint; PrintIndex++)
                    {
                        //KOTPrint4Inch(Send, Program.CompanyName, clsConfigSettings.header, Bill_Type, BillNo, dt2, "0", objDgv, date, Time, dt, out SendOut, OrderNo, custName, ContactNo, CounterNo);
                        KOTPrint4Inch(Send, Program.CompanyName, Program.Address, Bill_Type, BillNo, dt2, "0", objDgv, date, Time, dt, out SendOut, OrderNo, custName, ContactNo, CounterNo, OrderComment, Channel, Channel_Order_Id);

                        try
                        {
                            PosPrinter.Print(SendOut);
                            Send = null;
                            SendOut = null;
                        }
                        catch (Exception ex)
                        {
                            PosPrinter.Close();
                            Send = null;
                            SendOut = null;
                        }
                    }
                    PosPrinter.Close();
                }
            }
            catch { }
        }

        public string SplitConmment(string Discomments)
        {
            string FinalComment = string.Empty;
            try
            {
                string[] CommentDetail = Discomments.Split('+');
                int CommnetIndex = Discomments.Split('+').Length;
                for (int i = 0; i < CommentDetail.Length; i++)
                {
                    if (i == 0)
                        FinalComment += ">>" + CommentDetail[i].ToString();
                    else
                        FinalComment += "\n>>" + CommentDetail[i].ToString();
                }
            }
            catch { }
            return FinalComment;
        }


        public string GetKOT_Detail(string companyName, string Header, string KOT_Type, DateTime date, string BillNo, string Time, string CounterNo,
            string MobileNo, string CustName, string strType, string TOKEN_NO)
        {
            string Send = ESC + "@";
            try
            {
                if (strType == "H")
                {
                    Send = Send + ESC + "a" + (char)(1);
                    //Send = Send + companyName + LF + LF;
                    //LF= For new line
                    // BOld************
                    Send = Send + ESC + "!" + (char)(16 + 32 + 1);             //1 - font B; 32 = Double width               
                    Send = Send + companyName + LF + LF;
                    // BOld************

                    Send = Send + ESC + "!" + (char)(8);
                    // print header 
                    if (Header.Length > 0)
                        Send = Send + Header + LF;
                    // print sub header
                    Send = Send + ESC + "a" + (char)(0);
                    //set alllinment of line center off and set to left**********************************************************************

                    //Send = Send + "Bill Type : " + KOT_Type + LF;

                    Send = Send + "Bill No   : " + BillNo + LF;
                    Send = Send + "Date      : " + date.ToString("dd-MM-yyyy") + "   Time : " + Time + LF;
                    Send = Send + "Counter No: " + CounterNo + LF;
                    Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                    if (CustName.Length > 0)
                        Send = Send + "->:" + CustName.ToUpper() + LF;
                    if (MobileNo.Length > 0)
                        Send = Send + "->:" + MobileNo + LF;
                    Send = Send + ESC + "!" + (char)(8);
                    Send = Send + ESC + "a" + (char)(0);

                    Send = Send + ESC + "a" + (char)(1);
                    Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                    Send = Send + LF + KOT_Type + LF;
                    Send = Send + ESC + "!" + (char)(8);
                    Send = Send + ESC + "a" + (char)(0);
                }
                else
                {
                    Send = Send + ESC + "!" + (char)(8);
                    Send = Send + "===============================================" + LF;
                    Send = Send + " CASHIER  -   " + "Utility" + LF;
                    Send = Send + " TOKEN NO -   " + TOKEN_NO + LF;
                    Send = Send + "===============================================" + LF;
                    // Full cut**********************************************************************
                    Send = Send + GS + "V" + (char)66 + (char)0;
                    Send = Send + GS + "r" + (char)1;
                }

            }
            catch { }
            return Send;

        }
        public static DataTable GetAddonToPrint(Int64 item_code, DataTable dt, string addon_index)
        {
            DataTable dtobj = null;
            try
            {
                var currentStatRow = (from currentStat in dt.AsEnumerable()
                                      where currentStat.Field<String>("addon_index") == addon_index
                                      select currentStat);
                if (currentStatRow.Count() > 0)
                {
                    dtobj = new DataTable();
                    dtobj = currentStatRow.CopyToDataTable();
                }
            }
            catch { }
            return dtobj;
        }

        public void PrintKOTSmallFontSmallSize_SeperateKot_WithoutHeaders(string Kot_Cut, string TokenNo, int NoofPrint, string txtContactNo, string txtcustNAme, string lblBillNo, string OrderComment)
        {
            string Channel = string.Empty;
            string Channel_Order_Id = string.Empty;
            string strMasterPrinter = string.Empty;
            try
            {
                clsPrintKOT objkot = new clsPrintKOT();
                PrinterSettings settings = new PrinterSettings();
                string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                LPrinter PosPrinter = new LPrinter();
                string CurrentPrinter = settings.PrinterName;

                string MobileNo = string.Empty;
                string CustName = string.Empty;
                if (txtContactNo.Trim().Length > 5)
                    MobileNo = txtContactNo.Trim();
                if (txtcustNAme.Trim().Length > 3)
                    CustName = txtcustNAme.Trim();
                ////////////////Print KOT //////////////////////////////////////
                string Bill_Type = string.Empty;
                if (Program.BillType == "C")
                    Bill_Type = "Take Away";
                else if (Program.BillType == "H")
                    Bill_Type = "Home Delivery";
                else
                {
                    Bill_Type = "Dine-In";
                }
                DataTable dt1 = new DataTable();

                string header = clsConfigSettings.header.ToString();
                int CountKOT = 0;
                // Get priner according to item and counter
                DataSet ds = objADOC.GetDatset("Usp_Get_Printer_by_Counter_MP @Bill_No_FK='" + lblBillNo + "'", "tbl_dish_mast");
                //old
                //DataSet ds = ADOC.GetObject.GetDatset("Usp_Get_Printer_by_Counter @Bill_No_FK='" + lblBillNo.Text + "'", "tbl_dish_mast");
                dt1 = ds.Tables[0];
                DataTable dtAddonAll = ds.Tables[2];
                PosPrinter.Print(PaperFullCut);
                for (int a = 0; a < dt1.Rows.Count; ++a)
                {
                    //CurrentPrinter = settings.PrinterName;
                    string[] strPrinter = dt1.Rows[a]["printer"].ToString().Split(',');

                    if (dt1.Columns.Contains("Channel"))
                    {
                        Channel = dt1.Rows[a]["Channel"].ToString();
                        Channel_Order_Id = dt1.Rows[a]["Channel_Order_Id"].ToString();
                    }
                    TokenNo = dt1.Rows[a]["TokenNo"].ToString();
                    for (int PCount = 0; PCount < strPrinter.Length; PCount++)
                    {
                        CurrentPrinter = settings.PrinterName;
                        //string Printter_DB = dt1.Rows[a]["printer"].ToString();
                        string Printter_DB = strPrinter[PCount].ToString();

                        if ((Printter_DB.ToLower() == "none" || Printter_DB.ToLower().Trim() == "0"))
                        {
                            continue;
                        }
                        else if (Printter_DB != "0" && Printter_DB.ToLower() != "none" && Printter_DB.ToLower() != "default")
                        {
                            CurrentPrinter = Printter_DB;
                        }
                        PosPrinter.Open("testDoc", CurrentPrinter);
                        if (Kot_Cut == "1")
                            PosPrinter.Print(PaperFullCut);

                        string str1 = "Usp_Get_Item_KOT_Counter_MP @Bill_No_FK='" + lblBillNo + "',@counter_id='" + dt1.Rows[a]["c_code"].ToString().Trim() + "'";

                        // Get items for this counter
                        DataTable dt2 = objADOC.GetTable(str1);

                        for (int PrintCount = 0; PrintCount < NoofPrint; PrintCount++)
                        {
                            for (int b = 0; b < dt2.Rows.Count; ++b)
                            {
                                int Quantity = Convert.ToInt32(dt2.Rows[b]["Qty"]);

                                // Loop for each quantity → one slip each
                                for (int q = 1; q <= Quantity; q++)
                                {
                                    DateTime kot_date = System.DateTime.Now;
                                    string KotDate = DateTime.Now.ToString("dd-MM-yyyy");
                                    string KotTime = DateTime.Now.ToString("HH:mm");

                                    string Send1 = ESC + "@";
                                    Send1 += ESC + "a" + (char)(1);
                                    Send1 += ESC + "!" + (char)(31 + 8 + 1);
                                    Send1 += LF + Bill_Type + LF + LF;
                                    Send1 += ESC + "!" + (char)(10);
                                    Send1 += ESC + "a" + (char)(0);

                                    PosPrinter.Print(Send1);

                                    CountKOT++;

                                    string i_name = dt2.Rows[b]["i_name"].ToString();
                                    string i_code = dt2.Rows[b]["item_code"].ToString();
                                    string Group_Dish = dt2.Rows[b]["Group_Dish"].ToString();
                                    string Urgent_Item = dt2.Rows[b]["Urgent_Item"].ToString();
                                    string addon_index = dt2.Rows[b]["addon_index"].ToString();
                                    string kot_flag = dt2.Rows[b]["kot_flag"].ToString();

                                    if (i_name.Length <= 36)
                                    {
                                        for (int cn = i_name.Length; cn <= 35; ++cn)
                                            i_name += " ";
                                    }

                                    //// Print Main Item with Qty = 1
                                    //str1 = "" + i_name + "     1\n";
                                    //PosPrinter.Print(str1);

                                    if (string.IsNullOrWhiteSpace(i_name))
                                    {
                                        str1 = "               \n";
                                    }
                                    else
                                    {
                                        str1 = i_name + "     1\n";
                                    }
                                    PosPrinter.Print(str1);

                                    // Item Description
                                    if (dt2.Rows[b]["I_Name_Desc"].ToString() != "")
                                    {
                                        string I_Name_Desc = ">" + dt2.Rows[b]["I_Name_Desc"].ToString();
                                        PosPrinter.Print(I_Name_Desc + "\n");
                                    }

                                    // Item Comment
                                    if (dt2.Rows[b]["Comments"].ToString() != "")
                                    {
                                        string Conmment = SplitConmment(dt2.Rows[b]["Comments"].ToString());
                                        PosPrinter.Print(Conmment + "\n");
                                    }

                                    if (kot_flag == "1")
                                    {
                                        string str = "  ~~~~~~~~~~~~\n";
                                        PosPrinter.Print(str);
                                    }

                                    // Combo Items (dtDoughnuts)
                                    DataTable dt = ds.Tables[1];
                                    DataTable dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(i_code), dt, Group_Dish);

                                    if (dtDoughnuts != null)
                                    {
                                        for (int i = 0; i < dtDoughnuts.Rows.Count; i++)
                                        {
                                            string comboName = dtDoughnuts.Rows[i]["i_name"].ToString();
                                            kot_flag = dtDoughnuts.Rows[i]["kot_flag"].ToString();

                                            if (comboName.Length <= 36)
                                                comboName = comboName.PadRight(36);

                                            string PrintQuery = comboName + "     1\n";
                                            PosPrinter.Print(PrintQuery);

                                            if (dtDoughnuts.Rows[i]["DishComment"].ToString() != "")
                                            {
                                                string Conmment = SplitConmment(dtDoughnuts.Rows[i]["DishComment"].ToString());
                                                PosPrinter.Print(Conmment + "\n");
                                            }

                                            if (kot_flag == "1")
                                            {
                                                string str = "  ~~~~~~~~~~~~\n";
                                                PosPrinter.Print(str);
                                            }
                                        }
                                    }

                                    // Addons
                                    if (addon_index != "0" && addon_index != "")
                                    {
                                        DataTable dt_adon_fk = cls_ActionClass.GetAddonToPrint(Convert.ToInt64(i_code), dtAddonAll, addon_index);
                                        if (dt_adon_fk != null)
                                        {
                                            for (int ad = 0; ad < dt_adon_fk.Rows.Count; ad++)
                                            {
                                                string addonName = dt_adon_fk.Rows[ad]["i_name"].ToString();
                                                kot_flag = dt_adon_fk.Rows[ad]["kot_flag"].ToString();

                                                if (addonName.Length <= 36)
                                                    addonName = addonName.PadRight(36);

                                                str1 = "" + addonName + "  1\n";
                                                PosPrinter.Print(str1);

                                                if (dt_adon_fk.Rows[ad]["Comments"].ToString() != "")
                                                {
                                                    string Conmment = SplitConmment(dt_adon_fk.Rows[ad]["Comments"].ToString());
                                                    PosPrinter.Print(Conmment + "\n");
                                                }

                                                if (kot_flag == "1")
                                                {
                                                    string str = "  ~~~~~~~~~~~~\n";
                                                    PosPrinter.Print(str);
                                                }
                                            }
                                        }
                                    }

                                    // Footer
                                    PosPrinter.Print("--------------------------------------------");
                                    PosPrinter.Print("\n TOKEN NO: " + TokenNo + "   Date:" + KotDate + "   Time:" + KotTime + "\n");

                                    if (OrderComment.Length > 0)
                                        PosPrinter.Print("\n ORDER COMMENT: " + OrderComment + "\n");

                                    if (clsConfigSettings.isPrintZomatoIdOnKotHD == "1")
                                    {
                                        if (Channel_Order_Id.Trim().Length > 0)
                                        {
                                            string Send = ESC + "@";
                                            Send += ESC + "a" + (char)(1);
                                            Send += ESC + "!" + (char)(16 + 32 + 1);
                                            Send += Channel + " (" + Channel_Order_Id.Trim() + ")" + LF;
                                            Send += ESC + "!" + (char)(8);
                                            Send += ESC + "a" + (char)(0);
                                            PosPrinter.Print("\n" + Send + "  \n");
                                        }
                                    }
                             

                                    // CUT after every single qty slip
                                    PosPrinter.Print("--------------------------------------------\n\n\n\n\n\n");
                                    PosPrinter.Print(PaperFullCut);
                                }
                            }
                        }

                        // If no KOT, still cut paper
                        if (Kot_Cut == "1" && CountKOT == 0)
                        {
                            PosPrinter.Open("testDoc", CurrentPrinter);
                            PosPrinter.Print(PaperFullCut);
                            PosPrinter.Close();
                        }
                        PosPrinter.Close();

                        PosPrinter.Close();
                    }
                }
                strMasterPrinter = clsConfigSettings.MasterPrinter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void PrintKOTSmallFontSmallSize(string Kot_Cut, string TokenNo, int NoofPrint, string txtContactNo,
            string txtcustNAme, string lblBillNo, string OrderComment, string isinstantorder = "0")
        {
            try
            {
                string Channel = string.Empty;
                string Channel_Order_Id = string.Empty;
                string strMasterPrinter = string.Empty;
                try
                {
                    clsPrintKOT objkot = new clsPrintKOT();
                    PrinterSettings settings = new PrinterSettings();
                    string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                    string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                    LPrinter PosPrinter = new LPrinter();
                    string CurrentPrinter = settings.PrinterName;

                    string MobileNo = string.Empty;
                    string CustName = string.Empty;
                    if (txtContactNo.Trim().Length > 5)
                        MobileNo = txtContactNo.Trim();
                    if (txtcustNAme.Trim().Length > 3)
                        CustName = txtcustNAme.Trim();
                    ////////////////Print KOT //////////////////////////////////////
                    string Bill_Type = string.Empty;
                    if (Program.BillType == "C")
                        Bill_Type = "Take Away";
                    else if (Program.BillType == "H")
                        Bill_Type = "Home Delivery";
                    else
                    {
                        Bill_Type = "Dine-In";
                    }
                    ReportViewer rvobj = new ReportViewer();
                    if (isinstantorder == "1")
                    {
                        DataTable dt1 = new DataTable();

                       
                        rptCounterKOT_Pab rptKOTobj = new rptCounterKOT_Pab();
                        ADOC objADOC = new ADOC();
                        DataSet ds = objADOC.GetDatset("Usp_Get_Printer_by_Counter_Normal @Bill_No_FK='" + lblBillNo + "'", "tbl_dish_mast");

                        dt1 = ds.Tables[0];
                        DataTable dtAddonAll = ds.Tables[1];

                        for (int a = 0; a < dt1.Rows.Count; ++a)
                        {

                            //CurrentPrinter = settings.PrinterName;
                            string[] strPrinter = dt1.Rows[a]["printer"].ToString().Split(',');

                            if (dt1.Columns.Contains("Channel"))
                            {
                                Channel = dt1.Rows[a]["Channel"].ToString();
                                Channel_Order_Id = dt1.Rows[a]["Channel_Order_Id"].ToString();
                            }

                            for (int PCount = 0; PCount < strPrinter.Length; PCount++)
                            {
                                CurrentPrinter = settings.PrinterName;
                                //string Printter_DB = dt1.Rows[a]["printer"].ToString();
                                string Printter_DB = strPrinter[PCount].ToString();

                                if ((Printter_DB.ToLower() == "none" || Printter_DB.ToLower().Trim() == "0"))
                                {
                                    continue;
                                }
                                else if (Printter_DB != "0" && Printter_DB.ToLower() != "none" && Printter_DB.ToLower() != "default")
                                {
                                    CurrentPrinter = Printter_DB;
                                }
                                BoltKOT dsobjreport = new BoltKOT();
                                string Counter = ds.Tables[0].Rows[0]["Counter"].ToString();
                                string sql = "Usp_Get_Item_KOT_Counter_Normal @Bill_No_FK='" + lblBillNo + "',@counter_id='" + dt1.Rows[a]["c_code"].ToString() + "'";
                                DataSet dsFinalreport = objADOC.FillReportDataSet(sql, dsobjreport, "tbl_Bill_Tran");
                                 
                                if (dsFinalreport.Tables[0].Rows.Count > 0)
                                {
                                    DataTable dtTran = dsFinalreport.Tables[0];
                                    DataTable Biil_tran = new DataTable();
                                    Biil_tran = dsobjreport.Tables["tbl_bill_Tran"];
                                    DataTable assortedItemTable = new DataTable();
                                    for (int b = 0; b < dtTran.Rows.Count; ++b)
                                    {
                                        string item_addon = dtTran.Rows[b]["item_addon"].ToString();

                                        string i_code = dtTran.Rows[b]["item_code"].ToString();
                                        string Tran_i_name = dtTran.Rows[b]["i_name"].ToString();
                                        string i_name_qty = dtTran.Rows[b]["Qty"].ToString();
                                        string addon_index = dtTran.Rows[b]["addon_index"].ToString();
                                        string Group_Dish = dtTran.Rows[b]["Group_Dish"].ToString();
                                        string dishcomments = dtTran.Rows[b]["Comments"].ToString();
                                        string zomato_order_id = dtTran.Rows[b]["zomato_order_id"].ToString();

                                        DataRow Tran_newRow = Biil_tran.NewRow();
                                        Tran_newRow["i_name"] = Tran_i_name; // Replace "Column1" with the actual column name
                                        Tran_newRow["qty"] = i_name_qty;    // Replace "Column2" with the actual column name
                                        Tran_newRow["DishComment"] = dishcomments;
                                        Tran_newRow["zomato_order_id"] = zomato_order_id;
                                        // Add the new row to the table
                                        Biil_tran.Rows.Add(Tran_newRow);


                                        if (Group_Dish != "0" && Group_Dish != "")
                                        {
                                            //   DataTable dt_adon_fk = cls_ActionClass.GetAddonToPrint(Convert.ToInt64(i_code), dtAddonAll, addon_index);
                                            DataTable dt_adon_fk = GetAssortedItemToPrint(Convert.ToInt64(i_code), dtAddonAll, Group_Dish);
                                            if (dt_adon_fk != null)
                                            {
                                                // Assuming dsFinalreport contains a DataTable named "tbl_AssortedItem"
                                                assortedItemTable = dsobjreport.Tables["tbl_assorteditem"];

                                                for (int ad = 0; ad < dt_adon_fk.Rows.Count; ad++)
                                                {
                                                    string Conmment = string.Empty;
                                                    string i_name = dt_adon_fk.Rows[ad]["i_name"].ToString();
                                                    string kot_flag = dt_adon_fk.Rows[ad]["kot_flag"].ToString();

                                                    // Rest of your code...
                                                    string qty = dt_adon_fk.Rows[ad]["Qty"].ToString();
                                                    if (dt_adon_fk.Rows[ad]["DishComment"].ToString() != "")
                                                    {
                                                        Conmment = SplitConmment(dt_adon_fk.Rows[ad]["DishComment"].ToString());
                                                    }
                                                    // Check if addon index is greater than 0
                                                    if (ad >= 0)
                                                    {
                                                        // Create a new row in the "tbl_AssortedItem" table
                                                        DataRow newRow = assortedItemTable.NewRow();
                                                        newRow["i_name"] = i_name; // Replace "Column1" with the actual column name
                                                        newRow["qty"] = qty;    // Replace "Column2" with the actual column name
                                                        newRow["DishComment"] = Conmment;

                                                        // Add the new row to the table
                                                        assortedItemTable.Rows.Add(newRow);


                                                        DataRow billnewRow = Biil_tran.NewRow();
                                                        billnewRow["i_name"] = i_name; // Replace "Column1" with the actual column name
                                                        billnewRow["qty"] = " " + qty;    // Replace "Column2" with the actual column name
                                                        billnewRow["DishComment"] = Conmment;

                                                        Biil_tran.Rows.Add(billnewRow);
                                                    }
                                                }
                                            }
                                        }

                                    }

                                    rptKOTobj.SetDataSource(dsobjreport);
                                    rvobj.cRViewer.ReportSource = rptKOTobj;


                                    rptKOTobj.DataDefinition.FormulaFields["C_name"].Text = "'" + "CASHIER : " + Program.CashierName + "'";
                                    rptKOTobj.DataDefinition.FormulaFields["Counter_no"].Text = "'" + Counter.ToString() + "'";

                                    rptKOTobj.DataDefinition.FormulaFields["BILL_NO"].Text = "'" + lblBillNo + "'";

                                    string NewTokenNo = " TOKEN NO: " + TokenNo;

                                    rptKOTobj.DataDefinition.FormulaFields["TockenNo"].Text = "'" + NewTokenNo + "'";


                                    //-----------Start Heder--------------------//////////
                                    rptKOTobj.DataDefinition.FormulaFields["Bill_Header"].Text = "'" + clsConfigSettings.Bill_Head + "'";

                                    rptKOTobj.DataDefinition.FormulaFields["H_Company_name2"].Text = "'" + Program.CompanyName + "'";
                                    rptKOTobj.DataDefinition.FormulaFields["BillType"].Text = "'" + Bill_Type + "'";
                                    rptKOTobj.DataDefinition.FormulaFields["is_instant_order"].Text = "'" + isinstantorder + "'";


                                    rptKOTobj.PrintOptions.PrinterName = CurrentPrinter;
                                    if (NoofPrint == 2)
                                    {
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                    }
                                    if (NoofPrint == 3)
                                    {
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                    }
                                    if (NoofPrint == 4)
                                    {
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                    }
                                    if (NoofPrint == 5)
                                    {
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                    }
                                    if (NoofPrint == 6)
                                    {
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                    }
                                    else
                                    {
                                        rptKOTobj.PrintToPrinter(1, true, 1, 4);
                                    }
                                    // rvobj.Show();

                                }
                            }
                        }
                    }
                    else
                    {
                        DataTable dt1 = new DataTable();
                        string header = clsConfigSettings.header.ToString();
                        int CountKOT = 0;
                        // Get priner according to item and counter
                        DataSet ds = objADOC.GetObject.GetDatset("Usp_Get_Printer_by_Counter_Normal @Bill_No_FK='" + lblBillNo + "'", "tbl_dish_mast");
                        //DataSet ds = ADOC.GetObject.GetDatset("Usp_Get_Printer_PrintKot @Bill_No_FK='" + lblBillNo.Text + "'", "tbl_dish_mast");
                        dt1 = ds.Tables[0];

                        string is_instant_order = "0";
                        if (dt1.Columns.Contains("is_instant_order"))
                        {
                            is_instant_order = dt1.Rows[0]["is_instant_order"].ToString();
                        }
                        DataTable dtAddonAll = ds.Tables[2];
                        PosPrinter.Print(PaperFullCut);
                        for (int a = 0; a < dt1.Rows.Count; ++a)
                        {
                            //CurrentPrinter = settings.PrinterName;
                            string[] strPrinter = dt1.Rows[a]["printer"].ToString().Split(',');

                            if (dt1.Columns.Contains("Channel"))
                            {
                                Channel = dt1.Rows[a]["Channel"].ToString();
                                Channel_Order_Id = dt1.Rows[a]["Channel_Order_Id"].ToString();
                            }

                            for (int PCount = 0; PCount < strPrinter.Length; PCount++)
                            {
                                CurrentPrinter = settings.PrinterName;
                                //string Printter_DB = dt1.Rows[a]["printer"].ToString();
                                string Printter_DB = strPrinter[PCount].ToString();

                                if ((Printter_DB.ToLower() == "none" || Printter_DB.ToLower().Trim() == "0"))
                                {
                                    continue;
                                }
                                else if (Printter_DB != "0" && Printter_DB.ToLower() != "none" && Printter_DB.ToLower() != "default")
                                {
                                    CurrentPrinter = Printter_DB;
                                }
                                PosPrinter.Open("testDoc", CurrentPrinter);
                                if (Kot_Cut == "1")
                                    PosPrinter.Print(PaperFullCut);
                                string str1 = "Usp_Get_Item_KOT_Counter_Normal @Bill_No_FK='" + lblBillNo + "',@counter_id='" + dt1.Rows[a]["c_code"].ToString() + "'";
                                //search item by counter id
                                DataTable dt2 = objADOC.GetObject.GetTable(str1);

                                for (int PrintCount = 0; PrintCount < NoofPrint; PrintCount++)
                                {
                                    DateTime kot_date = System.DateTime.Now;
                                    string KotTime = DateTime.Now.ToString("HH:mm");

                                    string PrintBill_Type = "";
                                    PrintBill_Type = Bill_Type;
                                    if (is_instant_order == "1")
                                    {
                                        PrintBill_Type = PrintBill_Type + "";
                                    }
                                    string strHeader = objkot.GetKOT_Detail(Program.CompanyName, "", PrintBill_Type, kot_date, lblBillNo, KotTime, dt1.Rows[a]["counter"].ToString(), MobileNo, CustName, "H", TokenNo);


                                    PosPrinter.Print(strHeader);
                                    PosPrinter.Print("\n----------------------------------------\n");
                                    PosPrinter.Print("Item Name                            Qty");
                                    PosPrinter.Print("\n----------------------------------------\n");
                                    for (int b = 0; b < dt2.Rows.Count; ++b)
                                    {
                                        CountKOT++;
                                        DataTable dtDoughnuts = new DataTable();
                                        string item_addon = dt2.Rows[b]["item_addon"].ToString();
                                        string i_name = dt2.Rows[b]["i_name"].ToString();
                                        int ItemLenght = i_name.Length;
                                        string i_code = dt2.Rows[b]["item_code"].ToString();
                                        string Group_Dish = dt2.Rows[b]["Group_Dish"].ToString();
                                        string Urgent_Item = dt2.Rows[b]["Urgent_Item"].ToString();
                                        string addon_index = dt2.Rows[b]["addon_index"].ToString();
                                        string kot_flag = dt2.Rows[b]["kot_flag"].ToString();


                                        DataTable dt = ds.Tables[1];
                                        dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(i_code), dt, Group_Dish);
                                        if (i_name.Length <= 36)
                                        {
                                            for (int cn = i_name.Length; cn <= 35; ++cn)
                                                i_name += " ";
                                        }
                                        string qty = dt2.Rows[b]["Qty"].ToString();
                                        if (dtDoughnuts != null)
                                            qty = "";
                                        str1 = "" + i_name + "  " + qty + "\n";
                                        PosPrinter.Print(str1);
                                        if (dt2.Rows[b]["I_Name_Desc"].ToString() != "")
                                        {
                                            string I_Name_Desc = ">" + dt2.Rows[b]["I_Name_Desc"].ToString();
                                            PosPrinter.Print(I_Name_Desc + "\n");
                                        }

                                        if (dt2.Rows[b]["Comments"].ToString() != "")
                                        {
                                            string Conmment = SplitConmment(dt2.Rows[b]["Comments"].ToString());
                                            PosPrinter.Print(Conmment + "\n");
                                        }
                                        if (kot_flag == "1")
                                        {
                                            string str = "  ~~~~~~~~~~~~\n";
                                            PosPrinter.Print(str);
                                        }
                                        int CountCombo = 0;
                                        if (dtDoughnuts != null)
                                        {
                                            for (int i = 0; i < dtDoughnuts.Rows.Count; i++)
                                            {
                                                CountCombo = 1;
                                                string item_name = dtDoughnuts.Rows[i]["i_name"].ToString();
                                                kot_flag = dtDoughnuts.Rows[i]["kot_flag"].ToString();
                                                //item_name = "*" + item_name;
                                                if (i_name.Length <= 36)
                                                {
                                                    for (int dn = item_name.Length; dn <= 35; ++dn)
                                                        item_name += " ";
                                                }
                                                string PrintQuery = "" + item_name + "  " + dtDoughnuts.Rows[i]["Qty"].ToString() + "\n";
                                                PosPrinter.Print(PrintQuery);
                                                if (dtDoughnuts.Rows[i]["DishComment"].ToString() != "")
                                                {
                                                    string Conmment = SplitConmment(dtDoughnuts.Rows[i]["DishComment"].ToString());
                                                    PosPrinter.Print(Conmment + "\n");
                                                }
                                                if (kot_flag == "1")
                                                {
                                                    string str = "  ~~~~~~~~~~~~\n";
                                                    PosPrinter.Print(str);
                                                }
                                            }
                                        }
                                        if (addon_index != "0" && addon_index != "")
                                        {
                                            DataTable dt_adon_fk = GetAddonToPrint(Convert.ToInt64(i_code), dtAddonAll, addon_index);
                                            if (dt_adon_fk != null)
                                            {
                                                for (int ad = 0; ad < dt_adon_fk.Rows.Count; ad++)
                                                {
                                                    i_name = dt_adon_fk.Rows[ad]["i_name"].ToString();
                                                    kot_flag = dt_adon_fk.Rows[ad]["kot_flag"].ToString();
                                                    if (i_name.Length <= 36)
                                                    {
                                                        for (int cn = i_name.Length; cn <= 35; ++cn)
                                                            i_name += " ";
                                                    }
                                                    qty = dt_adon_fk.Rows[ad]["Qty"].ToString();

                                                    str1 = "" + i_name + "  " + qty + "\n";
                                                    PosPrinter.Print(str1);
                                                    if (dt_adon_fk.Rows[ad]["Comments"].ToString() != "")
                                                    {
                                                        string Conmment = SplitConmment(dt_adon_fk.Rows[ad]["Comments"].ToString());
                                                        PosPrinter.Print(Conmment + "\n");
                                                    }
                                                    if (kot_flag == "1")
                                                    {
                                                        string str = "  ~~~~~~~~~~~~\n";
                                                        PosPrinter.Print(str);
                                                    }
                                                }
                                            }
                                        }
                                        if (clsConfigSettings.Cutkot_UrgentItem == "1" && CountCombo > 0)
                                        {
                                            if (dt2.Rows.Count != (b + 1))
                                            {
                                                PosPrinter.Print("-----------------------------------------");
                                                PosPrinter.Print("\n CASHIER : " + Program.CashierName);
                                                PosPrinter.Print("\n TOKEN NO: " + TokenNo + "\n");
                                                if (OrderComment.Length > 0)
                                                    PosPrinter.Print("\n Order Comment : " + OrderComment + "\n");

                                                PosPrinter.Print("-----------------------------------------\n\n\n\n\n\n");
                                                PosPrinter.Print(PaperFullCut);
                                                PosPrinter.Print(strHeader);
                                                PosPrinter.Print("----------------------------------------\n");
                                                PosPrinter.Print("Item Name                            Qty");
                                                PosPrinter.Print("\n----------------------------------------\n");
                                            }
                                        }
                                    }
                                    PosPrinter.Print("-----------------------------------------");
                                    PosPrinter.Print("\n CASHIER : " + Program.CashierName);

                                    string PrintTOkenNo = "";
                                    PrintTOkenNo = TokenNo;
                                    if (is_instant_order == "1")
                                    {
                                        PrintTOkenNo = PrintTOkenNo + "";
                                    }
                                    PosPrinter.Print("\n TOKEN NO: " + PrintTOkenNo + "\n");

                                    if (OrderComment.Length > 0)
                                        PosPrinter.Print("\n Order Comment : " + OrderComment + "\n");


                                    if (Channel_Order_Id.Trim().Length > 0)
                                    {
                                        string Send = string.Empty;
                                        Send = "";
                                        Send = ESC + "@";
                                        Send = Send + ESC + "a" + (char)(1);
                                        Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                                        Send = Send + Channel + " (" + Channel_Order_Id.Trim() + ")" + LF;
                                        Send = Send + ESC + "!" + (char)(8);
                                        Send = Send + ESC + "a" + (char)(0);
                                        PosPrinter.Print("\n" + Send + "  \n");
                                    }
                                    PosPrinter.Print("-----------------------------------------\n\n\n\n\n\n");
                                    PosPrinter.Print(PaperFullCut);

                                }
                                if (Kot_Cut == "1" && CountKOT == 0)
                                {
                                    PosPrinter.Open("testDoc", CurrentPrinter);
                                    PosPrinter.Print(PaperFullCut);
                                    PosPrinter.Close();
                                }
                                PosPrinter.Close();
                            }
                        }
                        strMasterPrinter = clsConfigSettings.MasterPrinter;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (strMasterPrinter.Length > 0)
                {
                    if (isinstantorder == "1")
                    {
                       // PrintKOTSmallFontSmallSize_MasterKOT_InstantOrder("1", TokenNo, 1, txtContactNo, txtcustNAme, lblBillNo, strMasterPrinter, OrderComment, Channel, Channel_Order_Id, Program.BillType);
                    }
                    else
                    {
                        PrintKOTSmallFontSmallSize_MasterKOT("1", TokenNo, 1, txtContactNo, txtcustNAme, lblBillNo, strMasterPrinter, OrderComment, Channel, Channel_Order_Id, Program.BillType);
                    }
                }
            }
            catch { }
        }


        public void PrintKOTSmallFontSmallSize2(string Kot_Cut, string TokenNo, int NoofPrint, string txtContactNo,
            string txtcustNAme, string lblBillNo, string OrderComment)
        {
            try
            {
                string Channel = string.Empty;
                string Channel_Order_Id = string.Empty;
                string strMasterPrinter = string.Empty;
                try
                {
                    clsPrintKOT objkot = new clsPrintKOT();
                    PrinterSettings settings = new PrinterSettings();
                    string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                    string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                    LPrinter PosPrinter = new LPrinter();
                    string CurrentPrinter = settings.PrinterName;

                    string MobileNo = string.Empty;
                    string CustName = string.Empty;
                    if (txtContactNo.Trim().Length > 5)
                        MobileNo = txtContactNo.Trim();
                    if (txtcustNAme.Trim().Length > 3)
                        CustName = txtcustNAme.Trim();
                    ////////////////Print KOT //////////////////////////////////////
                    string Bill_Type = string.Empty;
                    if (Program.BillType == "C")
                        Bill_Type = "Take Away";
                    else if (Program.BillType == "H")
                        Bill_Type = "Home Delivery";
                    else
                    {
                        Bill_Type = "Dine-In";
                    }
                    DataTable dt1 = new DataTable();



                    string header = clsConfigSettings.header.ToString();
                    int CountKOT = 0;
                    // Get priner according to item and counter
                    DataSet ds = objADOC.GetObject.GetDatset("Usp_Get_Printer_by_Counter_Normal2 @Bill_No_FK='" + lblBillNo + "'", "tbl_dish_mast");
                    //DataSet ds = ADOC.GetObject.GetDatset("Usp_Get_Printer_PrintKot @Bill_No_FK='" + lblBillNo.Text + "'", "tbl_dish_mast");
                    dt1 = ds.Tables[0];
                    DataTable dtAddonAll = ds.Tables[2];
                    PosPrinter.Print(PaperFullCut);
                    for (int a = 0; a < dt1.Rows.Count; ++a)
                    {
                        //CurrentPrinter = settings.PrinterName;
                        string[] strPrinter = dt1.Rows[a]["printer"].ToString().Split(',');

                        if (dt1.Columns.Contains("Channel"))
                        {
                            Channel = dt1.Rows[a]["Channel"].ToString();
                            Channel_Order_Id = dt1.Rows[a]["Channel_Order_Id"].ToString();
                        }

                        for (int PCount = 0; PCount < strPrinter.Length; PCount++)
                        {
                            CurrentPrinter = settings.PrinterName;
                            //string Printter_DB = dt1.Rows[a]["printer"].ToString();
                            string Printter_DB = strPrinter[PCount].ToString();

                            if ((Printter_DB.ToLower() == "none" || Printter_DB.ToLower().Trim() == "0"))
                            {
                                continue;
                            }
                            else if (Printter_DB != "0" && Printter_DB.ToLower() != "none" && Printter_DB.ToLower() != "default")
                            {
                                CurrentPrinter = Printter_DB;
                            }
                            PosPrinter.Open("testDoc", CurrentPrinter);
                            if (Kot_Cut == "1")
                                PosPrinter.Print(PaperFullCut);
                            string str1 = "Usp_Get_Item_KOT_Counter_Normal2 @Bill_No_FK='" + lblBillNo + "',@counter_id='" + dt1.Rows[a]["c_code"].ToString() + "'";
                            //search item by counter id
                            DataTable dt2 = objADOC.GetObject.GetTable(str1);
                            for (int PrintCount = 0; PrintCount < NoofPrint; PrintCount++)
                            {
                                DateTime kot_date = System.DateTime.Now;
                                string KotTime = DateTime.Now.ToString("HH:mm");
                                string strHeader = objkot.GetKOT_Detail(Program.CompanyName, "", Bill_Type, kot_date, lblBillNo, KotTime, dt1.Rows[a]["counter"].ToString(), MobileNo, CustName, "H", TokenNo);
                                PosPrinter.Print(strHeader);
                                PosPrinter.Print("\n----------------------------------------\n");
                                PosPrinter.Print("Item Name                            Qty");
                                PosPrinter.Print("\n----------------------------------------\n");
                                for (int b = 0; b < dt2.Rows.Count; ++b)
                                {
                                    CountKOT++;
                                    DataTable dtDoughnuts = new DataTable();
                                    string item_addon = dt2.Rows[b]["item_addon"].ToString();
                                    string i_name = dt2.Rows[b]["i_name"].ToString();
                                    int ItemLenght = i_name.Length;
                                    string i_code = dt2.Rows[b]["item_code"].ToString();
                                    string Group_Dish = dt2.Rows[b]["Group_Dish"].ToString();
                                    string Urgent_Item = dt2.Rows[b]["Urgent_Item"].ToString();
                                    string addon_index = dt2.Rows[b]["addon_index"].ToString();
                                    string kot_flag = dt2.Rows[b]["kot_flag"].ToString();
                                    DataTable dt = ds.Tables[1];
                                    dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(i_code), dt, Group_Dish);
                                    if (i_name.Length <= 36)
                                    {
                                        for (int cn = i_name.Length; cn <= 35; ++cn)
                                            i_name += " ";
                                    }
                                    string qty = dt2.Rows[b]["Qty"].ToString();
                                    if (dtDoughnuts != null)
                                        qty = "";
                                    str1 = "" + i_name + "  " + qty + "\n";
                                    PosPrinter.Print(str1);
                                    if (dt2.Rows[b]["Comments"].ToString() != "")
                                    {
                                        string Conmment = SplitConmment(dt2.Rows[b]["Comments"].ToString());
                                        PosPrinter.Print(Conmment + "\n");
                                    }
                                    if (kot_flag == "1")
                                    {
                                        string str = "  ~~~~~~~~~~~~\n";
                                        PosPrinter.Print(str);
                                    }
                                    int CountCombo = 0;
                                    if (dtDoughnuts != null)
                                    {
                                        for (int i = 0; i < dtDoughnuts.Rows.Count; i++)
                                        {
                                            CountCombo = 1;
                                            string item_name = dtDoughnuts.Rows[i]["i_name"].ToString();
                                            kot_flag = dtDoughnuts.Rows[i]["kot_flag"].ToString();
                                            //item_name = "*" + item_name;
                                            if (i_name.Length <= 36)
                                            {
                                                for (int dn = item_name.Length; dn <= 35; ++dn)
                                                    item_name += " ";
                                            }
                                            string PrintQuery = "" + item_name + "  " + dtDoughnuts.Rows[i]["Qty"].ToString() + "\n";
                                            PosPrinter.Print(PrintQuery);
                                            if (dtDoughnuts.Rows[i]["DishComment"].ToString() != "")
                                            {
                                                string Conmment = SplitConmment(dtDoughnuts.Rows[i]["DishComment"].ToString());
                                                PosPrinter.Print(Conmment + "\n");
                                            }
                                            if (kot_flag == "1")
                                            {
                                                string str = "  ~~~~~~~~~~~~\n";
                                                PosPrinter.Print(str);
                                            }
                                        }
                                    }
                                    if (addon_index != "0" && addon_index != "")
                                    {
                                        DataTable dt_adon_fk = GetAddonToPrint(Convert.ToInt64(i_code), dtAddonAll, addon_index);
                                        if (dt_adon_fk != null)
                                        {
                                            for (int ad = 0; ad < dt_adon_fk.Rows.Count; ad++)
                                            {
                                                i_name = dt_adon_fk.Rows[ad]["i_name"].ToString();
                                                kot_flag = dt_adon_fk.Rows[ad]["kot_flag"].ToString();
                                                if (i_name.Length <= 36)
                                                {
                                                    for (int cn = i_name.Length; cn <= 35; ++cn)
                                                        i_name += " ";
                                                }
                                                qty = dt_adon_fk.Rows[ad]["Qty"].ToString();

                                                str1 = "" + i_name + "  " + qty + "\n";
                                                PosPrinter.Print(str1);
                                                if (dt_adon_fk.Rows[ad]["Comments"].ToString() != "")
                                                {
                                                    string Conmment = SplitConmment(dt_adon_fk.Rows[ad]["Comments"].ToString());
                                                    PosPrinter.Print(Conmment + "\n");
                                                }
                                                if (kot_flag == "1")
                                                {
                                                    string str = "  ~~~~~~~~~~~~\n";
                                                    PosPrinter.Print(str);
                                                }
                                            }
                                        }
                                    }
                                    if (clsConfigSettings.Cutkot_UrgentItem == "1" && CountCombo > 0)
                                    {
                                        if (dt2.Rows.Count != (b + 1))
                                        {
                                            PosPrinter.Print("-----------------------------------------");
                                            PosPrinter.Print("\n CASHIER : " + Program.CashierName);
                                            PosPrinter.Print("\n TOKEN NO: " + TokenNo + "\n");
                                            if (OrderComment.Length > 0)
                                                PosPrinter.Print("\n Order Comment : " + OrderComment + "\n");

                                            PosPrinter.Print("-----------------------------------------\n\n\n\n\n\n");
                                            PosPrinter.Print(PaperFullCut);
                                            PosPrinter.Print(strHeader);
                                            PosPrinter.Print("----------------------------------------\n");
                                            PosPrinter.Print("Item Name                            Qty");
                                            PosPrinter.Print("\n----------------------------------------\n");
                                        }
                                    }
                                }
                                PosPrinter.Print("-----------------------------------------");
                                PosPrinter.Print("\n CASHIER : " + Program.CashierName);
                                PosPrinter.Print("\n TOKEN NO: " + TokenNo + "\n");
                                if (OrderComment.Length > 0)
                                    PosPrinter.Print("\n Order Comment : " + OrderComment + "\n");


                                if (Channel_Order_Id.Trim().Length > 0)
                                {
                                    string Send = string.Empty;
                                    Send = "";
                                    Send = ESC + "@";
                                    Send = Send + ESC + "a" + (char)(1);
                                    Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                                    Send = Send + Channel + " (" + Channel_Order_Id.Trim() + ")" + LF;
                                    Send = Send + ESC + "!" + (char)(8);
                                    Send = Send + ESC + "a" + (char)(0);
                                    PosPrinter.Print("\n" + Send + "  \n");
                                }
                                PosPrinter.Print("-----------------------------------------\n\n\n\n\n\n");
                                PosPrinter.Print(PaperFullCut);

                            }
                            if (Kot_Cut == "1" && CountKOT == 0)
                            {
                                PosPrinter.Open("testDoc", CurrentPrinter);
                                PosPrinter.Print(PaperFullCut);
                                PosPrinter.Close();
                            }
                            PosPrinter.Close();
                        }
                    }
                    strMasterPrinter = clsConfigSettings.MasterPrinter;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (strMasterPrinter.Length > 0)
                {
                    PrintKOTSmallFontSmallSize_MasterKOT("1", TokenNo, 1, txtContactNo, txtcustNAme, lblBillNo, strMasterPrinter, OrderComment, Channel, Channel_Order_Id, Program.BillType);
                }
            }
            catch { }
        }

        
        public void PrintKOTSmallFontSmallSize_MasterKOT(string Kot_Cut, string TokenNo, int NoofPrint, string txtContactNo,
          string txtcustNAme, string lblBillNo, string strMasterPrinter, string OrderComment, string Channel, string Channel_Order_Id, string BillType)
        {
            clsPrintKOT objkot = new clsPrintKOT();
            PrinterSettings settings = new PrinterSettings();
            string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
            string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
            LPrinter PosPrinter = new LPrinter();
            //string CurrentPrinter = strMasterPrinter;

            string MobileNo = string.Empty;
            string CustName = string.Empty;
            if (txtContactNo.Trim().Length > 5)
                MobileNo = txtContactNo.Trim();
            if (txtcustNAme.Trim().Length > 3)
                CustName = txtcustNAme.Trim();
            ////////////////Print KOT //////////////////////////////////////
            string Bill_Type = string.Empty;
            if (BillType == "C")
                Bill_Type = "Take Away";
            else if (BillType == "H")
                Bill_Type = "Home Delivery";
            else
            {
                Bill_Type = "Dine-In";
            }
            DataTable dt1 = new DataTable();

            string header = clsConfigSettings.header.ToString();
            int CountKOT = 0;
            // Get priner according to item and counter
            DataSet ds = objADOC.GetObject.GetDatset("Usp_Get_Printer_by_Counter @Bill_No_FK='" + lblBillNo + "'", "tbl_dish_mast");
            //DataSet ds = ADOC.GetObject.GetDatset("Usp_Get_Printer_PrintKot @Bill_No_FK='" + lblBillNo.Text + "'", "tbl_dish_mast");
            dt1 = ds.Tables[0];
            DataTable dtAddonAll = ds.Tables[2];
            PosPrinter.Print(PaperFullCut);

            string[] strPrinter = strMasterPrinter.Split(',');

            for (int PCount = 0; PCount < strPrinter.Length; PCount++)
            {
                string CurrentPrinter = strPrinter[PCount];
                PosPrinter.Open("testDoc", CurrentPrinter);
                if (Kot_Cut == "1")
                    PosPrinter.Print(PaperFullCut);
                string str1 = "Usp_Get_Item_MasterKOT @Bill_No_FK='" + lblBillNo + "'";
                //search item by counter id
                DataTable dt2 = objADOC.GetObject.GetTable(str1);

                for (int PrintCount = 0; PrintCount < NoofPrint; PrintCount++)
                {
                    DateTime kot_date = System.DateTime.Now;
                    string KotTime = DateTime.Now.ToString("HH:mm");
                    string strHeader = objkot.GetKOT_Detail(Program.CompanyName, "", Bill_Type, kot_date, lblBillNo, KotTime, "Master KOT", MobileNo, CustName, "H", TokenNo);
                    PosPrinter.Print(strHeader);
                    PosPrinter.Print("\n----------------------------------------\n");
                    PosPrinter.Print("Item Name                            Qty");
                    PosPrinter.Print("\n----------------------------------------\n");
                    for (int b = 0; b < dt2.Rows.Count; ++b)
                    {
                        CountKOT++;
                        DataTable dtDoughnuts = new DataTable();
                        string item_addon = dt2.Rows[b]["item_addon"].ToString();
                        string i_name = dt2.Rows[b]["i_name"].ToString();
                        int ItemLenght = i_name.Length;
                        string i_code = dt2.Rows[b]["item_code"].ToString();
                        string Group_Dish = dt2.Rows[b]["Group_Dish"].ToString();
                        string Urgent_Item = dt2.Rows[b]["Urgent_Item"].ToString();
                        string addon_index = dt2.Rows[b]["addon_index"].ToString();
                        string kot_flag = dt2.Rows[b]["kot_flag"].ToString();
                        DataTable dt = ds.Tables[1];
                        dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(i_code), dt, Group_Dish);
                        if (i_name.Length <= 36)
                        {
                            for (int cn = i_name.Length; cn <= 35; ++cn)
                                i_name += " ";
                        }
                        string qty = dt2.Rows[b]["Qty"].ToString();
                        if (dtDoughnuts != null)
                            qty = "";
                        str1 = "" + i_name + "  " + qty + "\n";
                        PosPrinter.Print(str1);
                        if (dt2.Rows[b]["Comments"].ToString() != "")
                        {
                            string Conmment = SplitConmment(dt2.Rows[b]["Comments"].ToString());
                            PosPrinter.Print(Conmment + "\n");
                        }
                        if (kot_flag == "1")
                        {
                            string str = "  ~~~~~~~~~~~~\n";
                            PosPrinter.Print(str);
                        }
                        int CountCombo = 0;
                        if (dtDoughnuts != null)
                        {
                            for (int i = 0; i < dtDoughnuts.Rows.Count; i++)
                            {
                                CountCombo = 1;
                                string item_name = dtDoughnuts.Rows[i]["i_name"].ToString();
                                kot_flag = dtDoughnuts.Rows[i]["kot_flag"].ToString();
                                //item_name = "*" + item_name;
                                if (i_name.Length <= 36)
                                {
                                    for (int dn = item_name.Length; dn <= 35; ++dn)
                                        item_name += " ";
                                }
                                string PrintQuery = "" + item_name + "  " + dtDoughnuts.Rows[i]["Qty"].ToString() + "\n";
                                PosPrinter.Print(PrintQuery);
                                if (dtDoughnuts.Rows[i]["DishComment"].ToString() != "")
                                {
                                    string Conmment = SplitConmment(dtDoughnuts.Rows[i]["DishComment"].ToString());
                                    PosPrinter.Print(Conmment + "\n");
                                }
                                if (kot_flag == "1")
                                {
                                    string str = "  ~~~~~~~~~~~~\n";
                                    PosPrinter.Print(str);
                                }
                            }
                        }
                        if (addon_index != "0" && addon_index != "")
                        {
                            DataTable dt_adon_fk = GetAddonToPrint(Convert.ToInt64(i_code), dtAddonAll, addon_index);
                            if (dt_adon_fk != null)
                            {
                                for (int ad = 0; ad < dt_adon_fk.Rows.Count; ad++)
                                {
                                    i_name = dt_adon_fk.Rows[ad]["i_name"].ToString();
                                    kot_flag = dt_adon_fk.Rows[ad]["kot_flag"].ToString();
                                    if (i_name.Length <= 36)
                                    {
                                        for (int cn = i_name.Length; cn <= 35; ++cn)
                                            i_name += " ";
                                    }
                                    qty = dt_adon_fk.Rows[ad]["Qty"].ToString();

                                    str1 = "" + i_name + "  " + qty + "\n";
                                    PosPrinter.Print(str1);
                                    if (dt_adon_fk.Rows[ad]["Comments"].ToString() != "")
                                    {
                                        string Conmment = SplitConmment(dt_adon_fk.Rows[ad]["Comments"].ToString());
                                        PosPrinter.Print(Conmment + "\n");
                                    }
                                    if (kot_flag == "1")
                                    {
                                        string str = "  ~~~~~~~~~~~~\n";
                                        PosPrinter.Print(str);
                                    }
                                }
                            }
                        }
                        if (clsConfigSettings.Cutkot_UrgentItem == "1" && CountCombo > 0)
                        {
                            if (dt2.Rows.Count != (b + 1))
                            {
                                PosPrinter.Print("-----------------------------------------");
                                PosPrinter.Print("\n CASHIER : " + Program.CashierName);
                                PosPrinter.Print("\n TOKEN NO: " + TokenNo + "\n");
                                if (OrderComment.Length > 0)
                                    PosPrinter.Print("\n ORDER COMMENT: " + OrderComment + "\n");
                                PosPrinter.Print("-----------------------------------------\n\n\n\n\n\n");
                                PosPrinter.Print(PaperFullCut);
                                PosPrinter.Print(strHeader);
                                PosPrinter.Print("----------------------------------------\n");
                                PosPrinter.Print("Item Name                            Qty");
                                PosPrinter.Print("\n----------------------------------------\n");
                            }
                        }
                    }
                    PosPrinter.Print("-----------------------------------------");
                    PosPrinter.Print("\n CASHIER : " + Program.CashierName);
                    PosPrinter.Print("\n TOKEN NO: " + TokenNo + "\n");
                    if (OrderComment.Length > 0)
                        PosPrinter.Print("\n ORDER COMMENT: " + OrderComment + "\n");

                    if (Channel_Order_Id.Trim().Length > 0)
                    {
                        string Send = string.Empty;
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        Send = Send + Channel + " (" + Channel_Order_Id.Trim() + ")" + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send + "  \n");
                    }

                    PosPrinter.Print("-----------------------------------------\n\n\n\n\n\n");
                    PosPrinter.Print(PaperFullCut);

                }
                if (Kot_Cut == "1" && CountKOT == 0)
                {
                    PosPrinter.Open("testDoc", CurrentPrinter);
                    PosPrinter.Print(PaperFullCut);
                    PosPrinter.Close();
                }
                PosPrinter.Close();
            }
        }
    }
}
