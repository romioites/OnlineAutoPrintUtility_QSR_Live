using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Data;
using LPrinterTest;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Threading;

namespace Order_Display_System.App_Code
{
    class clsPrintKOTsummary
    {
        private const char ESC = (char)27;
        private const char LF = (char)10;
        private const char FS = (char)28;
        private const char GS = (char)29;

        public bool Print_KOT_Counter_DeptWise(string BillNo, string companyName, string KotTitle, string KOT_Type,
            DataGridView objDgv, string kot_no, string TableName, string TableCover, string CashierName, string BillType)
        {
            bool IsResult = false;
            try
            {
                IsResult = Print_KOT_Counter_DeptWise_PrinterWise(BillNo, companyName, KotTitle, KOT_Type, objDgv, kot_no, TableName, TableCover, CashierName, BillType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return IsResult;

        }



        #region KOTPrint3InchLablePrinting
        /// <summary>
        /// KOTPrint3InchLablePrinting
        /// </summary>
        /// <param name="Send"></param>
        /// <param name="companyName"></param>
        /// <param name="Header"></param>
        /// <param name="KotTitle"></param>
        /// <param name="KOT_Type"></param>
        /// <param name="BillNo"></param>
        /// <param name="str1"></param>
        /// <param name="AllId"></param>
        /// <param name="AllAssortedId"></param>
        /// <param name="dt2"></param>
        /// <param name="id"></param>
        /// <param name="LineonKOT"></param>
        /// <param name="objDgv"></param>
        /// <param name="NC_Item"></param>
        /// <param name="date"></param>
        /// <param name="Time"></param>
        /// <param name="dt"></param>
        /// <param name="SendOut"></param>
        /// <param name="AllIdOut"></param>
        /// <param name="AllAssortedIdOut"></param>
        /// <param name="kot_no"></param>
        /// <param name="Remarks"></param>
        /// <param name="Counter_Name"></param>
        /// <param name="TableName"></param>
        public void KOTPrint3InchLablePrinting(string Send, string companyName, string Header, string KotTitle, string KOT_Type, string BillNo,
    string str1, string AllId, string AllAssortedId, DataTable dt2, string id, string LineonKOT, DataGridView objDgv, string NC_Item,
    DateTime date, string Time, DataTable dt, out string SendOut, out string AllIdOut, out string AllAssortedIdOut, string
            kot_no, string Remarks, string Counter_Name, string TableName)
        {
            try
            {
                Send = Send + "Date:" + date.ToString("dd-MM-yyyy") + "  Time:" + Time + LF + "&";
                //Send = Send + "Bill No:" + clsConfigSettings.Initial + BillNo + " Date:" + date.ToString("dd-MM-yyyy") + " " + Time + LF + "&";
                if (clsConfigSettings.BillNo_on_kot == "1")
                {
                    if (TableName.Length > 0)
                        Send = Send + "Table No: " + TableName + "   Bill No:" + clsConfigSettings.Initial + BillNo + LF + "&";
                    else
                        Send = Send + "Bill No:" + clsConfigSettings.Initial + BillNo + LF + "&";
                }
                else
                    if (TableName.Length > 0)
                        Send = Send + "Table No: " + TableName + LF + "&";
                    else
                        Send = Send + "Bill No:" + clsConfigSettings.Initial + BillNo + LF + "&";

                id = string.Empty;
                AllId = string.Empty;
                for (int b = 0; b < dt2.Rows.Count; ++b)
                {
                    string item_code = dt2.Rows[b]["i_code"].ToString();
                    string i_name = dt2.Rows[b]["i_name"].ToString().ToUpper();
                    if (i_name.Length > 30)
                        i_name = i_name.Substring(0, 30);
                    string UrgentItem = dt2.Rows[b]["UrgentItem"].ToString();
                    string item_index = dt2.Rows[b]["item_index"].ToString();
                    id = dt2.Rows[b]["id"].ToString();
                    AllId += id + ",";
                    DataTable dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(item_code), dt, item_index);
                    bool IsExist = IsNCItem(NC_Item, item_code);
                    if (IsExist == true)
                    {
                        i_name = "**" + i_name;
                    }
                    if (i_name.Length < 30)
                    {
                        for (int cn = i_name.Length; cn <= 29; ++cn)
                            i_name += " ";
                    }
                    //if (item_index == "0")
                    //{
                    str1 = "" + i_name + " " + dt2.Rows[b]["qty"].ToString();
                    //Send = Send + str1 + LF;

                    //if (UrgentItem == "1")
                    //    Send = Send + "******************" + LF;
                    //if (LineonKOT == "1")
                    //    Send = Send + "---------------------------------" ;
                    //}
                    if (dt2.Rows[b]["Comments"].ToString() != "")
                    {
                        // Send = Send + dt2.Rows[b]["Comments"].ToString();
                        Send = Send + str1 + LF + "`" + dt2.Rows[b]["Comments"].ToString() + "&";
                    }
                    else
                        Send = Send + str1 + LF + "&";
                    int count = 0;
                    var query = from DataGridViewRow row in objDgv.Rows
                                where row.Cells["AddonCode_fk"].Value.ToString() == item_code.ToString() && row.Cells["IsNewKOT"].Value.ToString() == "0"
                                select new
                                {
                                    item_name = row.Cells["ItemName"].Value.ToString().ToUpper(),
                                    orderqty = row.Cells["qty"].Value.ToString(),
                                    totalamount = row.Cells["Total"].Value.ToString(),
                                    DishComment = row.Cells["DishComment"].Value.ToString()
                                };

                    foreach (var qry in query)
                    {
                        string Addoni_name = qry.item_name.ToString();
                        if (Addoni_name.Length >= 30)
                            Addoni_name = Addoni_name.Substring(0, 35);
                        string qty = qry.orderqty.ToString();
                        double amount = Convert.ToDouble(qry.totalamount.ToString());
                        string DishComments = qry.DishComment.ToString();
                        if (Addoni_name.Length < 30)
                        {
                            for (int cn = Addoni_name.Length; cn <= 30; ++cn)
                                Addoni_name += " ";
                            count++;
                        }
                        str1 = "" + Addoni_name + "  " + qty;
                        //Send = Send + str1 + LF + "&";
                        if (DishComments != "" && DishComments != "")
                        {
                            //Send = Send + " >>" + DishComments;
                            Send = Send + str1 + LF + "`" + DishComments + "&";
                        }
                        else
                            Send = Send + str1 + LF + "&";
                        if (count > 0)
                            Send = Send + LF;
                    }
                    AllAssortedId = string.Empty;
                    if (dtDoughnuts != null)
                    {
                        for (int i = 0; i < dtDoughnuts.Rows.Count; i++)
                        {
                            string item_name = dtDoughnuts.Rows[i]["i_name"].ToString().ToUpper();
                            string aid = dtDoughnuts.Rows[i]["id"].ToString();
                            AllAssortedId += aid + ",";
                            item_name = "*" + item_name;
                            if (item_name.Length <= 30)
                            {
                                for (int dn = item_name.Length; dn <= 30; ++dn)
                                    item_name += " ";
                            }
                            string PrintQuery = "" + item_name + "  " + dtDoughnuts.Rows[i]["Qty"].ToString();
                            Send = Send + PrintQuery + LF + "&";
                        }
                    }
                }
                //Send = Send + "---------------------------------" + LF;
                //Send = Send + "STWD - " + Program.CashierName + LF;
                if (clsConfigSettings.PrintKOTRmarks_HD == "1" && Remarks.Length > 0)
                {
                    //Send = Send + ESC + "a" + (char)(1);
                    //Send = Send + ESC + "!" + (char)(16 + 16 + 2);             //1 - font B; 32 = Double width 
                    Send = Send + LF + "Remarks-" + Remarks + "&";
                    //Send = Send + ESC + "!" + (char)(8);
                    // Send = Send + ESC + "a" + (char)(0);
                }
                //Send = Send + "---------------------------------" + LF;
                // Full cut**********************************************************************
                // Send = Send + GS + "V" + (char)66 + (char)0;
                // Send = Send + GS + "r" + (char)1;
                // Full cut***********************************************************************
            }
            catch (Exception ex)
            {
                throw ex;
            }
            SendOut = Send.TrimEnd('&');
            AllIdOut = AllId;
            AllAssortedIdOut = AllAssortedId.TrimEnd('&');
        }

        #endregion

        #region Print KOT Counter DeptWise PrinterWise
        /// <summary>
        /// Print_KOT_Counter_DeptWise_PrinterWise
        /// </summary>
        /// <param name="BillNo"></param>
        /// <param name="companyName"></param>
        /// <param name="KotTitle"></param>
        /// <param name="KOT_Type"></param>
        /// <param name="objDgv"></param>
        /// <param name="kot_no"></param>
        /// <param name="TableName"></param>
        /// <param name="TableCover"></param>
        /// <param name="CashierName"></param>
        /// <param name="BillType"></param>
        /// <returns></returns>
        private bool Print_KOT_Counter_DeptWise_PrinterWise(string BillNo, string companyName, string KotTitle,
            string KOT_Type, DataGridView objDgv, string kot_no, string TableName, string TableCover, string CashierName, string BillType)
        {
            bool isKoTComboPrint = false;
            bool IsKotPrint = false;
            try
            {
                string OrderRemarks = string.Empty;
                string AllId = string.Empty;
                string AllAssortedId = string.Empty;
                string Kot_Title = "";
                KotTitle = "KOT Type  : " + KotTitle;
                if (KOT_Type.Length > 0)
                    KOT_Type = "Order Type: " + KOT_Type;

                string NC_Item = clsConfigSettings.NC_Item;

                string Header = string.Empty;//clsConfigSettings.header;
                ////******************End Header related************************************************


                string LineonKOT = clsConfigSettings.LineOnKOT;
                string PortName = string.Empty;
                //string IP = string.Empty;
                int NoofPrint = Convert.ToInt32(clsConfigSettings.No_of_Kot);
                string Send = null;

                string str1 = string.Empty;
                DateTime date = DateTime.ParseExact(Program.Bill_Date, "yyyy-MM-dd", null);
                string Time = System.DateTime.Now.ToShortTimeString();
                string SendOut = string.Empty;
                string AllIdOut = string.Empty;
                string AllAssortedIdOut = string.Empty;


                string IncludedTax = string.Empty;
                PrinterSettings settings = new PrinterSettings();
                string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                LPrinter PosPrinter = new LPrinter();
                string CurrentPrinter = settings.PrinterName;
                DataTable dtPrinter = new DataTable();


                ////////////////Print KOT //////////////////////////////////////               
                DataTable dt1 = new DataTable();
                DataSet ds = new DataSet();

                // Get Printer Name and department id ************** changes on 18-12-2015 by Sujit
                if (clsConfigSettings.IS_KOT_Print_dept == "1")
                    ds = ADOC.GetObject.GetDatset("[Usp_Get_Printerwithdept_kotBilling_printer_KDS] @Bill_No_FK='" + BillNo + "',@kot_no='" + kot_no + "'", "tbl_bill");
                // Get Printer Name and super department id ************** changes on 18-12-2015 by Sujit
                else if (clsConfigSettings.IS_KOT_Print_dept == "2")
                    ds = ADOC.GetObject.GetDatset("[Usp_Get_Printerwithdept_kotBilling_printer_SD_KDS] @Bill_No_FK='" + BillNo + "',@kot_no='" + kot_no + "'", "tbl_bill");
                // Get Printer Name and counter id ************** changes on 18-12-2015 by Sujit
                else
                    ds = ADOC.GetObject.GetDatset("[Usp_Get_PrinterwithCounterWise_kotBilling_printer_KDS] @Bill_No_FK='" + BillNo + "',@kot_no='" + kot_no + "'", "tbl_bill");

                dt1 = ds.Tables[0];
                DataTable dt = ds.Tables[1];
                if (dt1.Rows.Count > 0)
                {
                    //Send = ESC + "( A";
                    Send = ESC + "@";
                    OrderRemarks = dt1.Rows[0]["Remarks"].ToString();
                    if (OrderRemarks == "0")
                        OrderRemarks = "";
                }
                else
                {
                    IsKotPrint = false;
                }
                //*******************************Send request to the printer****************************  
                for (int PrinterIndex = 0; PrinterIndex < dt1.Rows.Count; PrinterIndex++)
                {
                    dtPrinter = new DataTable();
                    CurrentPrinter = settings.PrinterName;
                    string PrintterAccordinttoDeptt = dt1.Rows[PrinterIndex]["printer"].ToString();
                    string Counter_Name = dt1.Rows[PrinterIndex]["Counter_Name"].ToString();
                    if ((PrintterAccordinttoDeptt.ToLower() == "none" || PrintterAccordinttoDeptt.ToLower() == "0"))// && Program.BillType != "H")
                        continue;
                    else if (PrintterAccordinttoDeptt != "0" && PrintterAccordinttoDeptt.ToLower() != "none" && PrintterAccordinttoDeptt.ToLower() != "default")
                    {
                        CurrentPrinter = PrintterAccordinttoDeptt;
                    }
                    string deptid = dt1.Rows[PrinterIndex]["id"].ToString();

                    string id = string.Empty;

                    //CurrentPrinter = PrintterAccordinttoDeptt;
                    if (CurrentPrinter.ToLower() == "none")
                    {
                        continue;
                    }

                    // get item detail from department id *********************** Changes on 18-12-2015 by Sujit
                    if (clsConfigSettings.IS_KOT_Print_dept == "1")
                        str1 = "[Usp_Get_Item_KOT_CounterBillbydeptt_tb_Printer_KDS] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "',@kot_no='" + kot_no + "'"; ///Search item by Department wise///////////// Changes 22-06-2012 ///
                    // get item detail from super department id *********************** Changes on 18-12-2015 by Sujit
                    else if (clsConfigSettings.IS_KOT_Print_dept == "2")
                        str1 = "[Usp_Get_Item_KOT_CounterBillbydeptt_tb_Printer_SD_KDS] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "',@kot_no='" + kot_no + "'"; ///Search Item by Super Department wise///////////// Changes 18-12-2015 ///
                    // get item detail from counter id *********************** Changes on 18-12-2015 by Sujit
                    else
                        str1 = "[Usp_Get_Item_KOT_CounterBillbyCounter_tb_Printer_KDS] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "',@kot_no='" + kot_no + "'"; ///Search Item by Kot wise///////////// Changes 22-06-2012 ///                                                                       

                    DataTable dt2 = ADOC.GetObject.GetTable(str1);
                    if (clsConfigSettings.Multi_Printer == "1")
                        dtPrinter = GetAllPrinter_multiPrinter(ds.Tables[2], Convert.ToInt64(deptid));
                    else if (clsConfigSettings.PrintKOT_SectionWise == "1")
                        dtPrinter = GetAllPrinter_multiPrinter(ds.Tables[2], Convert.ToInt64(deptid));
                    else
                    {
                        dtPrinter.Columns.Add("Printer_Name");
                        dtPrinter.Columns.Add("Config_id_fk");
                        dtPrinter.Rows.Add(CurrentPrinter, deptid);
                    }
                    for (int p = 0; p < dtPrinter.Rows.Count; p++)
                    {
                        //pick printer
                        CurrentPrinter = dtPrinter.Rows[p]["Printer_Name"].ToString();
                        if (CurrentPrinter.ToLower() == "default")
                            CurrentPrinter = settings.PrinterName;
                        PosPrinter.Open("testDoc", CurrentPrinter);
                        for (int PrintIndex = 0; PrintIndex < NoofPrint; PrintIndex++)
                        {
                            if (clsConfigSettings.isKOT_3or4_EnchPrint == "1")
                                KOTPrint4Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dt2, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, kot_no, Kot_Title, OrderRemarks, Counter_Name, CashierName, BillType, TableCover, TableName);
                            else
                                KOTPrint3Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dt2, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, kot_no, OrderRemarks, Counter_Name, CashierName, TableCover, TableName);
                            try
                            {
                                PosPrinter.Print(SendOut);
                                string sql = "Usp_UpdateKOTPrinted_tempkot @Bill_No='" + BillNo + "',@Cashier='KDS',@tempid='" + AllIdOut.TrimEnd(',') + "',@assortedid='" + AllAssortedIdOut.TrimEnd(',') + "'";
                                ADOC.GetObject.ExecuteDML(sql);
                                Send = null;
                                SendOut = null;
                                IsKotPrint = true;

                            }
                            catch (Exception ex)
                            {
                                PosPrinter.Close();
                                Send = null;
                                SendOut = null;
                                IsKotPrint = false;
                                ADOC.GetObject.ExecuteDML("Usp_CreateError_log @Form_Name='btnKOTOrder Line:1771',@Error_COde='002',@Error_name='The port " + PortName + " does not exist -" + BillNo + "',@Bill_date='" + Program.Bill_Date + "',@created_by='KDS'");
                                return false;
                            }
                        }
                        PosPrinter.Close();
                    }
                }
                string CashierPrinter = clsConfigSettings.PrinterName;
                if (CashierPrinter.Trim() != "Epson" && CashierPrinter.Trim() != "Epson1" && CashierPrinter.Trim() != "Epson 1")
                {
                    PrintCashierCopy(BillNo, Header, "Cashier Copy", KOT_Type, date, Time, objDgv, CashierPrinter, OrderRemarks, CashierName, TableName, TableCover);
                }
                string IsPrintDefauktKOT = clsConfigSettings.IsPrintDefaultKOT;
                if (IsPrintDefauktKOT == "1")
                {
                    PrintDefaultKOT(BillNo, Header, "Cashier Copy", KOT_Type, date, Time, objDgv, CashierPrinter, OrderRemarks,
                        KotTitle, TableName, TableCover, CashierName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return IsKotPrint;
        }
        #endregion

        #region Print PrintDefaultKOT
        /// <summary>
        /// PrintDefaultKOT
        /// </summary>
        /// <param name="Bill_No"></param>
        /// <param name="Header"></param>
        /// <param name="KotTitle"></param>
        /// <param name="KOT_Type"></param>
        /// <param name="date"></param>
        /// <param name="Time"></param>
        /// <param name="objDgv"></param>
        /// <param name="Printer"></param>
        /// <param name="Remarks"></param>
        /// <param name="Title"></param>
        private void PrintDefaultKOT(string Bill_No, string Header, string KotTitle, string KOT_Type, DateTime date,
    string Time, DataGridView objDgv, string Printer, string Remarks, string Title, string TableName, string TableCover, string CashierName)
        {
            try
            {
                string CompanyName = Program.CompanyName.Replace("''", "'");
                string NC_Item = clsConfigSettings.NC_Item;
                PrinterSettings settings = new PrinterSettings();
                string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                LPrinter PosPrinter = new LPrinter();
                string CurrentPrinter = settings.PrinterName;
                string str1 = "Usp_Get_Item_KOT_Cashier_copy @Bill_No_FK='" + Bill_No + "'"; ///Search Kot Item///////////// Changes 22-06-2012 ///
                DataTable dt0 = new DataTable();
                DataSet ds = ADOC.GetObject.GetDatset(str1, "tbl_bill");
                dt0 = ds.Tables[0];
                DataTable dt1 = ds.Tables[1];
                if (dt0.Rows.Count > 0)
                {
                    PosPrinter.Open("testDoc", CurrentPrinter);
                    PosPrinter.Print("             " + CompanyName + "    \n");
                    PosPrinter.Print("        " + Header + "    \n");
                    PosPrinter.Print("             -----KOT-----\n");
                    PosPrinter.Print("          -----" + KotTitle + "-----\n");
                    PosPrinter.Print("          -----" + KOT_Type + "-----\n");
                    PosPrinter.Print("          -----" + Title + "-----\n");

                    if (clsConfigSettings.BillNo_on_kot == "1")
                        PosPrinter.Print("Bill No : " + clsConfigSettings.Initial + Bill_No + "     Table No: " + TableName + "\n");
                    else
                        PosPrinter.Print("Table No: " + TableName + "\n");
                    PosPrinter.Print("Date : " + date.ToString("dd-MM-yyyy") + "   Time: " + Time);
                    PosPrinter.Print("\nCover: " + TableCover);
                    PosPrinter.Print("\n----------------------------------------\n");
                    PosPrinter.Print("Item Name                           Qty");
                    PosPrinter.Print("\n----------------------------------------\n");
                    for (int b = 0; b < dt0.Rows.Count; ++b)
                    {
                        string item_code = dt0.Rows[b]["i_code"].ToString();
                        string i_name = dt0.Rows[b]["i_name"].ToString();
                        string UrgentItem = dt0.Rows[b]["UrgentItem"].ToString();
                        string item_index = dt0.Rows[b]["item_index"].ToString();
                        DataTable dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(item_code), dt1, item_index);
                        bool IsExist = IsNCItem(NC_Item, item_code);
                        if (IsExist == true)
                        {
                            //i_name += "**";
                            i_name = "**" + i_name;
                        }
                        if (i_name.Length < 36)
                        {
                            for (int cn = i_name.Length; cn <= 35; ++cn)
                                i_name += " ";
                        }
                        str1 = "" + i_name + "  " + dt0.Rows[b]["Qty"].ToString() + "\n";
                        PosPrinter.Print(str1);
                        if (UrgentItem == "1")
                            PosPrinter.Print("----------------------------------------\n");

                        if (dt0.Rows[b]["Comments"].ToString() != "")
                        {
                            PosPrinter.Print(" >>" + dt0.Rows[b]["Comments"].ToString() + "\n");
                        }
                        int count = 0;
                        var query = from DataGridViewRow row in objDgv.Rows
                                    where row.Cells["AddonCode_fk"].Value.ToString() == item_code.ToString() && row.Cells["IsNewKOT"].Value.ToString() == "0"
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
                            if (Addoni_name.Length >= 35)
                                Addoni_name = Addoni_name.Substring(0, 35);
                            string qty = qry.orderqty.ToString();
                            double amount = Convert.ToDouble(qry.totalamount.ToString());
                            string DishComments = qry.DishComment.ToString();
                            if (Addoni_name.Length < 35)
                            {
                                for (int cn = Addoni_name.Length; cn <= 35; ++cn)
                                    Addoni_name += " ";
                                count++;
                            }
                            str1 = "" + Addoni_name + "  " + qty + "\n";
                            PosPrinter.Print(str1);
                            if (DishComments != "" && DishComments != "")
                            {
                                PosPrinter.Print(" >>" + DishComments + "\n");
                            }
                        }
                        if (count > 0)
                            PosPrinter.Print("\n");
                        if (dtDoughnuts != null)
                        {
                            for (int i = 0; i < dtDoughnuts.Rows.Count; i++)
                            {
                                string item_name = dtDoughnuts.Rows[i]["i_name"].ToString();
                                item_name = "*" + item_name;
                                if (i_name.Length <= 36)
                                {
                                    for (int dn = item_name.Length; dn <= 35; ++dn)
                                        item_name += " ";
                                }
                                string PrintQuery = "" + item_name + "  " + dtDoughnuts.Rows[i]["Qty"].ToString() + "\n";
                                PosPrinter.Print(PrintQuery);
                            }
                        }
                    }
                    PosPrinter.Print("----------------------------------------");
                    PosPrinter.Print("\n STWD: " + CashierName + "\n");
                    if (Program.OrderRemarks == "1" && Remarks.Length > 0)
                    {
                        PosPrinter.Print("\n\nRemarks: " + Remarks + "\n");
                    }
                    PosPrinter.Print("----------------------------------------\n\n\n\n\n\n");
                    PosPrinter.Print(PaperFullCut);
                    PosPrinter.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region PrintCashierCopy
        /// <summary>
        /// PrintCashierCopy
        /// </summary>
        /// <param name="Bill_No"></param>
        /// <param name="Header"></param>
        /// <param name="KotTitle"></param>
        /// <param name="KOT_Type"></param>
        /// <param name="date"></param>
        /// <param name="Time"></param>
        /// <param name="objDgv"></param>
        /// <param name="Printer"></param>
        /// <param name="Remarks"></param>
        /// <param name="CashierName"></param>
        /// <param name="TableName"></param>
        /// <param name="TableCover"></param>
        private void PrintCashierCopy(string Bill_No, string Header, string KotTitle, string KOT_Type,
    DateTime date, string Time, DataGridView objDgv, string Printer, string Remarks, string CashierName, string TableName, string TableCover)
        {
            try
            {
                string NC_Item = clsConfigSettings.NC_Item;
                PrinterSettings settings = new PrinterSettings();
                string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                LPrinter PosPrinter = new LPrinter();
                string CurrentPrinter = Printer;
                string CompanyName = Program.CompanyName.Replace("''", "'");
                //string CurrentPrinter = settings.PrinterName;
                string str1 = "Usp_Get_Item_KOT_Cashier_copy @Bill_No_FK='" + Bill_No + "'"; ///Search Kot Item///////////// Changes 22-06-2012 ///
                DataTable dt2 = new DataTable();
                DataSet ds = ADOC.GetObject.GetDatset(str1, "tbl_bill");
                dt2 = ds.Tables[0];
                DataTable dt = ds.Tables[1];
                PosPrinter.Open("testDoc", CurrentPrinter);
                PosPrinter.Print("             " + CompanyName + "    \n");
                PosPrinter.Print("        " + Header + "    \n");
                PosPrinter.Print("             -----KOT-----\n");
                PosPrinter.Print("          -----" + KotTitle + "-----\n");
                PosPrinter.Print("          -----" + KOT_Type + "-----\n");
                if (clsConfigSettings.BillNo_on_kot == "1")
                    PosPrinter.Print("Bill No : " + clsConfigSettings.Initial + Bill_No + "     Table No: " + TableName + "\n");
                else
                    PosPrinter.Print("Table No: " + TableName + "\n");

                PosPrinter.Print("Date : " + date.ToString("dd-MM-yyyy") + "   Time: " + Time);
                PosPrinter.Print("\nCover: " + TableCover);
                PosPrinter.Print("\n----------------------------------------\n");
                PosPrinter.Print("Item Name                           Qty");
                PosPrinter.Print("\n----------------------------------------\n");
                for (int b = 0; b < dt2.Rows.Count; ++b)
                {
                    string item_code = dt2.Rows[b]["i_code"].ToString();
                    string i_name = dt2.Rows[b]["i_name"].ToString();
                    string item_index = dt2.Rows[b]["item_index"].ToString();
                    DataTable dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(item_code), dt, item_index);
                    bool IsExist = IsNCItem(NC_Item, item_code);
                    if (IsExist == true)
                    {
                        i_name = "**" + i_name;
                    }
                    if (i_name.Length < 36)
                    {
                        for (int cn = i_name.Length; cn <= 35; ++cn)
                            i_name += " ";
                    }
                    str1 = "" + i_name + " " + dt2.Rows[b]["Qty"].ToString() + "\n";
                    PosPrinter.Print(str1);
                    if (dt2.Rows[b]["Comments"].ToString() != "")
                    {
                        PosPrinter.Print(dt2.Rows[b]["Comments"].ToString() + "\n");
                    }
                    int count = 0;
                    var query = from DataGridViewRow row in objDgv.Rows
                                where row.Cells["AddonCode_fk"].Value.ToString() == item_code.ToString() && row.Cells["IsNewKOT"].Value.ToString() == "0"
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
                        if (Addoni_name.Length >= 35)
                            Addoni_name = Addoni_name.Substring(0, 35);
                        string qty = qry.orderqty.ToString();
                        double amount = Convert.ToDouble(qry.totalamount.ToString());
                        string DishComments = qry.DishComment.ToString();
                        if (Addoni_name.Length < 35)
                        {
                            for (int cn = Addoni_name.Length; cn <= 35; ++cn)
                                Addoni_name += " ";
                            count++;
                        }
                        str1 = "" + Addoni_name + "  " + qty + "\n";
                        PosPrinter.Print(str1);
                        if (DishComments != "" && DishComments != "")
                        {
                            PosPrinter.Print(" >>" + DishComments + "\n");
                        }
                    }
                    if (count > 0)
                        PosPrinter.Print("\n");
                    if (dtDoughnuts != null)
                    {
                        for (int i = 0; i < dtDoughnuts.Rows.Count; i++)
                        {
                            string item_name = dtDoughnuts.Rows[i]["i_name"].ToString();
                            item_name = "*" + item_name;
                            if (i_name.Length <= 36)
                            {
                                for (int dn = item_name.Length; dn <= 35; ++dn)
                                    item_name += " ";
                            }
                            string PrintQuery = "" + item_name + "  " + dtDoughnuts.Rows[i]["Qty"].ToString() + "\n";
                            PosPrinter.Print(PrintQuery);
                        }
                    }
                }
                PosPrinter.Print("----------------------------------------");
                PosPrinter.Print("\n STWD: " + CashierName + "\n");
                if (Program.OrderRemarks == "1" && Remarks.Length > 0)
                {
                    PosPrinter.Print("\n\nRemarks: " + Remarks + "\n");
                }
                PosPrinter.Print("----------------------------------------\n\n\n\n\n\n");
                PosPrinter.Print(PaperFullCut);
                PosPrinter.Close();
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region KOTPrint4Inch
        /// <summary>
        /// KOTPrint4Inch
        /// </summary>
        /// <param name="Send"></param>
        /// <param name="companyName"></param>
        /// <param name="Header"></param>
        /// <param name="KotTitle"></param>
        /// <param name="KOT_Type"></param>
        /// <param name="BillNo"></param>
        /// <param name="str1"></param>
        /// <param name="AllId"></param>
        /// <param name="AllAssortedId"></param>
        /// <param name="dt2"></param>
        /// <param name="id"></param>
        /// <param name="LineonKOT"></param>
        /// <param name="objDgv"></param>
        /// <param name="NC_Item"></param>
        /// <param name="date"></param>
        /// <param name="Time"></param>
        /// <param name="dt"></param>
        /// <param name="SendOut"></param>
        /// <param name="AllIdOut"></param>
        /// <param name="AllAssortedIdOut"></param>
        /// <param name="kot_no"></param>
        /// <param name="Kot_Title"></param>
        /// <param name="Remarks"></param>
        /// <param name="CounterName"></param>
        /// <param name="CashierName"></param>
        /// <param name="BillType"></param>
        /// <param name="TableCover"></param>
        /// <param name="TableName"></param>
        public void KOTPrint4Inch(string Send, string companyName, string Header, string KotTitle, string KOT_Type, string BillNo, string str1, string AllId, string AllAssortedId, DataTable dt2, string id, string LineonKOT, DataGridView objDgv, string NC_Item, DateTime date, string Time, DataTable dt,
out string SendOut, out string AllIdOut, out string AllAssortedIdOut, string kot_no, string Kot_Title, string Remarks, string CounterName,
            string CashierName, string BillType, string TableCover, string TableName)
        {
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
            Send = Send + " " + KotTitle + LF;
            Send = Send + " " + KOT_Type + LF;
            if (clsConfigSettings.BillNo_on_kot == "1")
            {
                if (clsConfigSettings.Take_Away == "1" || clsConfigSettings.Home_Delivery == "1" || BillType == "K")
                    Send = Send + " Bill No   : " + clsConfigSettings.Initial + BillNo + "  Cover : " + TableCover + LF;
                else
                    Send = Send + " Bill No   : " + clsConfigSettings.Initial + BillNo + LF;
            }
            else
            {
                if (clsConfigSettings.Take_Away == "1" || clsConfigSettings.Home_Delivery == "1" || BillType == "K")
                    Send = Send + "  Cover : " + TableCover + LF;
            }
            Send = Send + " KOT No    : " + kot_no + LF;

            Send = Send + " Date      : " + date.ToString("dd-MM-yyyy") + "   Time : " + Time + LF + LF;
            Send = Send + ESC + "a" + (char)(1);
            Send = Send + ESC + "!" + (char)(16 + 16 + 2);             //1 - font B; 32 = Double width 
            Send = Send + Kot_Title + LF;

            Send = Send + CounterName + LF;
            if (clsConfigSettings.Take_Away == "1" || clsConfigSettings.Home_Delivery == "1" || BillType == "K")
                Send = Send + " Table No  : " + TableName + LF;
            Send = Send + ESC + "!" + (char)(8);
            Send = Send + ESC + "a" + (char)(0);
            Send = Send + "===============================================" + LF;
            Send = Send + " QTY             ITEMS                      " + LF;
            Send = Send + "===============================================" + LF;
            id = string.Empty;
            AllId = string.Empty;
            //Send = Send + ESC + "!" + (char)(6 + 26 + 1);             //1 - font B; 32 = Double width           
            Send = Send + ESC + "!" + (char)(16 + 32 + 1);             //1 - font B; 32 = Double width           
            for (int b = 0; b < dt2.Rows.Count; ++b)
            {
                string item_code = dt2.Rows[b]["i_code"].ToString();
                string i_name = dt2.Rows[b]["i_name"].ToString();
                string UrgentItem = dt2.Rows[b]["UrgentItem"].ToString();
                string item_index = dt2.Rows[b]["item_index"].ToString();
                id = dt2.Rows[b]["id"].ToString();
                AllId += id + ",";
                DataTable dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(item_code), dt, item_index);
                bool IsExist = IsNCItem(NC_Item, item_code);
                if (IsExist == true)
                {
                    i_name = "**" + i_name;
                }
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

                if (UrgentItem == "1")
                    Send = Send + "***" + LF;
                if (LineonKOT == "1")
                    Send = Send + "-----------------------------------------------" + LF;
                //}
                if (dt2.Rows[b]["Comments"].ToString() != "")
                {
                    Send = Send + " >>" + dt2.Rows[b]["Comments"].ToString() + LF;
                }
                int count = 0;
                var query = from DataGridViewRow row in objDgv.Rows
                            where row.Cells["AddonCode_fk"].Value.ToString() == item_code.ToString() && row.Cells["IsNewKOT"].Value.ToString() == "0"
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
                AllAssortedId = string.Empty;
                if (dtDoughnuts != null)
                {
                    for (int i = 0; i < dtDoughnuts.Rows.Count; i++)
                    {
                        string item_name = dtDoughnuts.Rows[i]["i_name"].ToString();
                        string aid = dtDoughnuts.Rows[i]["id"].ToString();
                        AllAssortedId += aid + ",";
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
            Send = Send + " STWD -   " + CashierName + LF;
            if (clsConfigSettings.PrintKOTRmarks_HD == "1" && Remarks.Length > 0)
            {
                Send = Send + ESC + "!" + (char)(16 + 16 + 2);             //1 - font B; 32 = Double width 
                Send = Send + LF + "Remarks-" + Remarks + LF;
                Send = Send + ESC + "!" + (char)(8);
            }
            Send = Send + "===============================================" + LF;
            // Full cut**********************************************************************
            Send = Send + GS + "V" + (char)66 + (char)0;
            Send = Send + GS + "r" + (char)1;
            // Full cut***********************************************************************
            SendOut = Send;
            AllIdOut = AllId;
            AllAssortedIdOut = AllAssortedId;
        }

        #endregion

        #region KOTPrint3Inch
        /// <summary>
        /// KOTPrint3Inch
        /// </summary>
        /// <param name="Send"></param>
        /// <param name="companyName"></param>
        /// <param name="Header"></param>
        /// <param name="KotTitle"></param>
        /// <param name="KOT_Type"></param>
        /// <param name="BillNo"></param>
        /// <param name="str1"></param>
        /// <param name="AllId"></param>
        /// <param name="AllAssortedId"></param>
        /// <param name="dt2"></param>
        /// <param name="id"></param>
        /// <param name="LineonKOT"></param>
        /// <param name="objDgv"></param>
        /// <param name="NC_Item"></param>
        /// <param name="date"></param>
        /// <param name="Time"></param>
        /// <param name="dt"></param>
        /// <param name="SendOut"></param>
        /// <param name="AllIdOut"></param>
        /// <param name="AllAssortedIdOut"></param>
        /// <param name="kot_no"></param>
        /// <param name="Remarks"></param>
        /// <param name="Counter_Name"></param>
        /// <param name="CashierName"></param>
        /// <param name="TableCover"></param>
        /// <param name="TableName"></param>
        public void KOTPrint3Inch(string Send, string companyName, string Header, string KotTitle, string KOT_Type, string BillNo,
    string str1, string AllId, string AllAssortedId, DataTable dt2, string id, string LineonKOT, DataGridView objDgv, string NC_Item,
    DateTime date, string Time, DataTable dt, out string SendOut, out string AllIdOut, out string AllAssortedIdOut,
            string kot_no, string Remarks, string Counter_Name, string CashierName, string TableCover, string TableName)
        {
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
            Send = Send + KotTitle + LF;
            Send = Send + KOT_Type + LF;
            if (clsConfigSettings.BillNo_on_kot == "1")
                Send = Send + "Bill No   : " + clsConfigSettings.Initial + BillNo + "  Cover : " + TableCover + LF;
            else
                Send = Send + "Cover : " + TableCover + LF;
            Send = Send + "KOT No    :" + kot_no + LF;
            Send = Send + "Date: " + date.ToString("dd-MM-yyyy") + "  Time: " + Time + LF + LF;
            if (TableName.Length > 0)
                Send = Send + "    Table No: " + TableName + LF;
            Send = Send + Counter_Name + LF;
            Send = Send + "---------------------------------" + LF;
            Send = Send + "Item Name                    Qty" + LF;
            Send = Send + "---------------------------------" + LF;
            id = string.Empty;
            AllId = string.Empty;
            for (int b = 0; b < dt2.Rows.Count; ++b)
            {
                string item_code = dt2.Rows[b]["i_code"].ToString();
                string i_name = dt2.Rows[b]["i_name"].ToString();
                string UrgentItem = dt2.Rows[b]["UrgentItem"].ToString();
                string item_index = dt2.Rows[b]["item_index"].ToString();
                id = dt2.Rows[b]["id"].ToString();
                AllId += id + ",";
                DataTable dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(item_code), dt, item_index);
                bool IsExist = IsNCItem(NC_Item, item_code);
                if (IsExist == true)
                {
                    i_name = "**" + i_name;
                }
                if (i_name.Length < 28)
                {
                    for (int cn = i_name.Length; cn <= 27; ++cn)
                        i_name += " ";
                }
                //if (item_index == "0")
                //{
                str1 = "" + i_name + " " + dt2.Rows[b]["qty"].ToString() + LF;
                Send = Send + str1;

                if (UrgentItem == "1")
                    Send = Send + "******************" + LF;
                if (LineonKOT == "1")
                    Send = Send + "---------------------------------" + LF;
                //}
                if (dt2.Rows[b]["Comments"].ToString() != "")
                {
                    Send = Send + dt2.Rows[b]["Comments"].ToString() + LF;
                }
                int count = 0;
                var query = from DataGridViewRow row in objDgv.Rows
                            where row.Cells["AddonCode_fk"].Value.ToString() == item_code.ToString() && row.Cells["IsNewKOT"].Value.ToString() == "0"
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
                    if (Addoni_name.Length >= 27)
                        Addoni_name = Addoni_name.Substring(0, 27);
                    string qty = qry.orderqty.ToString();
                    double amount = Convert.ToDouble(qry.totalamount.ToString());
                    string DishComments = qry.DishComment.ToString();
                    if (Addoni_name.Length < 27)
                    {
                        for (int cn = Addoni_name.Length; cn <= 27; ++cn)
                            Addoni_name += " ";
                        count++;
                    }
                    str1 = "" + Addoni_name + "  " + qty + LF;
                    Send = Send + str1;
                    if (DishComments != "" && DishComments != "")
                    {
                        Send = Send + " >>" + DishComments + LF;
                    }
                    if (count > 0)
                        Send = Send + LF;
                }
                AllAssortedId = string.Empty;
                if (dtDoughnuts != null)
                {
                    for (int i = 0; i < dtDoughnuts.Rows.Count; i++)
                    {
                        string item_name = dtDoughnuts.Rows[i]["i_name"].ToString();
                        string aid = dtDoughnuts.Rows[i]["id"].ToString();
                        AllAssortedId += aid + ",";
                        item_name = "*" + item_name;
                        if (item_name.Length <= 28)
                        {
                            for (int dn = item_name.Length; dn <= 27; ++dn)
                                item_name += " ";
                        }
                        string PrintQuery = "" + item_name + "  " + dtDoughnuts.Rows[i]["Qty"].ToString() + LF;
                        Send = Send + PrintQuery;
                    }
                }
            }
            Send = Send + "---------------------------------" + LF;
            Send = Send + "STWD - " + CashierName + LF;
            if (clsConfigSettings.PrintKOTRmarks_HD == "1" && Remarks.Length > 0)
            {
                Send = Send + ESC + "a" + (char)(1);
                Send = Send + ESC + "!" + (char)(16 + 16 + 2);             //1 - font B; 32 = Double width 
                Send = Send + LF + "Remarks-" + Remarks + LF;
                Send = Send + ESC + "!" + (char)(8);
                Send = Send + ESC + "a" + (char)(0);
            }
            Send = Send + "---------------------------------" + LF;
            // Full cut**********************************************************************
            Send = Send + GS + "V" + (char)66 + (char)0;
            Send = Send + GS + "r" + (char)1;
            // Full cut***********************************************************************
            SendOut = Send;
            AllIdOut = AllId;
            AllAssortedIdOut = AllAssortedId;

        }

        #endregion

        #region Private/local method
        private DataTable GetAllPrinter_multiPrinter(DataTable dt, Int64 Config_id_fk)
        {
            DataTable dtobj = new DataTable();
            try
            {
                var currentStatRow = (from currentStat in dt.AsEnumerable()
                                      where currentStat.Field<Int64>("Config_id_fk") == Config_id_fk
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
        private static bool IsNCItem(string values, string I_code)
        {
            bool isExist = false;
            try
            {
                string[] str = values.Split(',');
                for (int i = 0; i < str.Length; i++)
                {
                    string item_id = str[i];
                    if (item_id == I_code)
                    {
                        isExist = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return isExist;
        }
        public bool CheckReadDatabyIPAddressorPortWise(SerialPort comport, string Send, string IP)
        {
            bool IsResult = false;
            try
            {
                // Read the Port Data by IP Address ************* if Check_IP_PortWise=1 ,check IP address 0 for Check ReadData
                if (clsConfigSettings.Check_IP_PortWise == "1")
                {
                    Application.DoEvents();
                    Ping ping = new Ping();
                    PingReply pingresult = ping.Send(IP);
                    if (pingresult.Status.ToString() == "Success")
                    {
                        comport.Write(Send);
                        IsResult = true;
                    }
                    else
                    {
                        IsResult = false;
                    }
                }
                else
                {
                    comport.Write(Send);
                    IsResult = ReadData(ref comport);
                }
            }
            catch (Exception ex)
            {

            }
            return IsResult;
        }
        private Queue<byte> recievedData = new Queue<byte>();
        internal bool ReadData(ref SerialPort port)
        {
            //Buffer to hold input string
            string InString = string.Empty;
            string buffer = string.Empty;
            //Application.DoEvents();
            Thread.Sleep(Program.ReadData);
            byte[] data = new byte[port.BytesToRead];
            port.Read(data, 0, data.Length);
            data.ToList().ForEach(b => recievedData.Enqueue(b));
            int hello = recievedData.Count;
            var packet = Enumerable.Range(0, 50).Select(i => recievedData.Dequeue());
            if (recievedData.Count > 0)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region  PrintKOT2Inch
        /// <summary>
        /// PrintKOT2Inch
        /// </summary>
        /// <param name="Bill_No"></param>
        /// <param name="Header"></param>
        /// <param name="KotTitle"></param>
        /// <param name="KOT_Type"></param>
        /// <param name="objDgv"></param>
        /// <param name="kot_no"></param>
        /// <param name="tableName"></param>
        /// <param name="Remarks"></param>
        /// <param name="CashierName"></param>
        /// <param name="TableCover"></param>
        /// <param name="BillType"></param>
        public bool PrintKOT2Inch(string Bill_No, string Header, string KotTitle, string KOT_Type, DataGridView objDgv, string kot_no,
string tableName, string Remarks, string CashierName, string TableCover, string BillType)
        {
            bool Result = false;
            try
            {
                string CompanyName = Program.CompanyName.Replace("''", "'");
                string NC_Item = clsConfigSettings.NC_Item;
                PrinterSettings settings = new PrinterSettings();
                string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                LPrinter PosPrinter = new LPrinter();
                string CurrentPrinter = settings.PrinterName;
                DateTime date = DateTime.ParseExact(Program.Bill_Date, "yyyy-MM-dd", null);
                string Time = System.DateTime.Now.ToShortTimeString();
                string str1 = string.Empty;
                if ((BillType == "C" || BillType == "H") && (clsConfigSettings.Take_Away == "0" || clsConfigSettings.Home_Delivery == "0"))
                    str1 = "Usp_Get_Item_KOT_Tran_Cashier_copy_KDS @Bill_No_FK='" + Bill_No + "',@kot_no='" + kot_no + "'"; ///Search Kot Item///////////// Changes 22-06-2012 ///
                else
                    str1 = "Usp_Get_Item_KOT_Cashier_copy_KDS @Bill_No_FK='" + Bill_No + "',@kot_no='" + kot_no + "'"; ///Search Kot Item///////////// Changes 22-06-2012 ///
                DataTable dt0 = new DataTable();
                DataSet ds = ADOC.GetObject.GetDatset(str1, "tbl_bill");
                dt0 = ds.Tables[0];
                DataTable dt1 = ds.Tables[1];
                if (dt0.Rows.Count > 0)
                {
                    DataTable dtPrinter = new DataTable();
                    DataTable dt2 = ADOC.GetObject.GetTable(str1);
                    if (clsConfigSettings.Multi_Printer == "1")
                        dtPrinter = GetAllPrinter_multiPrinter(ds.Tables[2], Convert.ToInt64(Bill_No));
                    else
                    {
                        dtPrinter.Columns.Add("Printer_Name");
                        dtPrinter.Columns.Add("Config_id_fk");
                        dtPrinter.Rows.Add(CurrentPrinter, Bill_No);
                    }
                    for (int p = 0; p < dtPrinter.Rows.Count; p++)
                    {
                        //pick printer
                        CurrentPrinter = dtPrinter.Rows[p]["Printer_Name"].ToString();

                        PosPrinter.Open("testDoc", CurrentPrinter);
                        PosPrinter.Print("         " + CompanyName + "  \n");
                        PosPrinter.Print("        " + Header + "    \n");
                        PosPrinter.Print("             -----KOT-----\n");
                        PosPrinter.Print("      ----" + KotTitle + "-----\n");
                        PosPrinter.Print("      ----" + KOT_Type + "-----\n");
                        if (clsConfigSettings.BillNo_on_kot == "1")
                            PosPrinter.Print("Bill No :  " + clsConfigSettings.Initial + Bill_No + " Table No:  " + tableName + "\n");
                        else
                            PosPrinter.Print("Table No:  " + tableName + "\n");
                        PosPrinter.Print("Date :  " + date.ToString("dd-MM-yyyy") + "    Time: " + Time);
                        PosPrinter.Print("\nKOT No : " + kot_no);
                        PosPrinter.Print("\nCover: " + TableCover);
                        PosPrinter.Print("\n----------------------------------------\n");
                        PosPrinter.Print("Item Name                            Qty");
                        PosPrinter.Print("\n----------------------------------------\n");
                        for (int b = 0; b < dt0.Rows.Count; ++b)
                        {
                            string item_code = dt0.Rows[b]["i_code"].ToString();
                            string i_name = dt0.Rows[b]["i_name"].ToString();
                            string UrgentItem = dt0.Rows[b]["UrgentItem"].ToString();
                            string item_index = dt0.Rows[b]["item_index"].ToString();
                            DataTable dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(item_code), dt1, item_index);
                            bool IsExist = IsNCItem(NC_Item, item_code);
                            if (IsExist == true)
                            {
                                //i_name += "**";
                                i_name = "**" + i_name;
                            }
                            if (i_name.Length < 36)
                            {
                                for (int cn = i_name.Length; cn <= 35; ++cn)
                                    i_name += " ";
                            }
                            str1 = "" + i_name + "  " + dt0.Rows[b]["Qty"].ToString() + "\n";
                            PosPrinter.Print(str1);
                            if (UrgentItem == "1")
                                PosPrinter.Print("--------------------------------\n");

                            if (dt0.Rows[b]["Comments"].ToString() != "")
                            {
                                PosPrinter.Print(" >>" + dt0.Rows[b]["Comments"].ToString() + "\n");
                            }
                            int count = 0;
                            var query = from DataGridViewRow row in objDgv.Rows
                                        where row.Cells["AddonCode_fk"].Value.ToString() == item_code.ToString() && row.Cells["IsNewKOT"].Value.ToString() == "0"
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
                                if (Addoni_name.Length >= 35)
                                    Addoni_name = Addoni_name.Substring(0, 35);
                                string qty = qry.orderqty.ToString();
                                double amount = Convert.ToDouble(qry.totalamount.ToString());
                                string DishComments = qry.DishComment.ToString();
                                if (Addoni_name.Length < 35)
                                {
                                    for (int cn = Addoni_name.Length; cn <= 35; ++cn)
                                        Addoni_name += " ";
                                    count++;
                                }
                                str1 = "" + Addoni_name + "  " + qty + "\n";
                                PosPrinter.Print(str1);
                                if (DishComments != "" && DishComments != "")
                                {
                                    PosPrinter.Print(" >>" + DishComments + "\n");
                                }
                            }
                            if (count > 0)
                                PosPrinter.Print("\n");
                            if (dtDoughnuts != null)
                            {
                                for (int i = 0; i < dtDoughnuts.Rows.Count; i++)
                                {
                                    string item_name = dtDoughnuts.Rows[i]["i_name"].ToString();
                                    item_name = "*" + item_name;
                                    if (i_name.Length <= 36)
                                    {
                                        for (int dn = item_name.Length; dn <= 35; ++dn)
                                            item_name += " ";
                                    }
                                    string PrintQuery = "" + item_name + "  " + dtDoughnuts.Rows[i]["Qty"].ToString() + "\n";
                                    PosPrinter.Print(PrintQuery);
                                }
                            }
                        }
                        PosPrinter.Print("-----------------------------------------");
                        PosPrinter.Print("\n STWD: " + CashierName + "\n");
                        if (clsConfigSettings.PrintKOTRmarks_HD == "1" && Remarks.Length > 0)
                        {
                            PosPrinter.Print("\n\nRemarks: " + Remarks + "\n");
                        }
                        PosPrinter.Print("-----------------------------------------\n\n\n\n\n\n");
                        PosPrinter.Print(PaperFullCut);
                        PosPrinter.Close();
                        Result = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Result = false;
            }
            return Result;
        }
        #endregion

        #region Print_KOT_Counter_DeptWise_PortWise
        /// <summary>
        /// Print_KOT_Counter_DeptWise_PortWise
        /// </summary>
        /// <param name="BillNo"></param>
        /// <param name="companyName"></param>
        /// <param name="KotTitle"></param>
        /// <param name="KOT_Type"></param>
        /// <param name="objDgv"></param>
        /// <param name="kot_no"></param>
        /// <param name="CashierName"></param>
        /// <param name="TableCover"></param>
        /// <param name="BillType"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool Print_KOT_Counter_DeptWise_PortWise(string BillNo, string companyName, string KotTitle, string KOT_Type,
        DataGridView objDgv, string kot_no, string CashierName, string TableCover, string BillType, string TableName)
        {
            string OrderRemarks = string.Empty;
            string AllId = string.Empty;
            string AllAssortedId = string.Empty;
            bool isKoTComboPrint = false;
            string Kot_Title = KotTitle;
            KotTitle = "KOT Type  : " + KotTitle;
            KOT_Type = "Order Type: " + KOT_Type;
            string NC_Item = clsConfigSettings.NC_Item;
            //string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
            //string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
            ////*******************header related added by satyabir on 02/02/2017*******************
            string Header = string.Empty;//clsConfigSettings.header;
            ////******************End Header related************************************************




            string LineonKOT = clsConfigSettings.LineOnKOT;
            string PortName = string.Empty;
            string IP = string.Empty;
            int NoofPrint = Convert.ToInt32(clsConfigSettings.No_of_Kot);
            string Send = null;
            bool IsKotPrint = false;
            string str1 = string.Empty;
            DateTime date = DateTime.ParseExact(Program.Bill_Date, "yyyy-MM-dd", null);
            string Time = System.DateTime.Now.ToShortTimeString();
            string SendOut = string.Empty;
            string AllIdOut = string.Empty;
            string AllAssortedIdOut = string.Empty;
            DataTable dtPrinter = new DataTable();
            try
            {
                ////////////////Print KOT //////////////////////////////////////               
                DataTable dt1 = new DataTable();
                DataSet ds = new DataSet();

                // Search port name, department id and ip  and  according to department id ***** changes on 18-12-2015 by Sujit
                if (clsConfigSettings.IS_KOT_Print_dept == "1")
                    ds = ADOC.GetObject.GetDatset("[Usp_Get_Printerwithdept_Comport_printer_KDS] @Bill_No_FK='" + BillNo + "',@kot_no='" + kot_no + "'", "tbl_bill");
                // Search port name, super department id and ip  and  according to super department id ***** changes on 18-12-2015 by Sujit
                else if (clsConfigSettings.IS_KOT_Print_dept == "2")
                    ds = ADOC.GetObject.GetDatset("[Usp_Get_Printerwithdept_Comport_printer_SD_kds] @Bill_No_FK='" + BillNo + "',@kot_no='" + kot_no + "'", "tbl_bill");
                // Search port name, counter id and ip  and  according to counter ***** changes on 18-12-2015 by Sujit
                else
                    ds = ADOC.GetObject.GetDatset("[Usp_Get_PrinterwithCounterWise_Comport_printer_KDS] @Bill_No_FK='" + BillNo + "',@kot_no='" + kot_no + "'", "tbl_bill");

                dt1 = ds.Tables[0];
                DataTable dt = ds.Tables[1];
                if (dt1.Rows.Count > 0)
                {
                    OrderRemarks = ds.Tables[0].Rows[0]["Remarks"].ToString();
                    if (OrderRemarks == "0")
                        OrderRemarks = "";
                    //Send = ESC + "( A";
                    Send = ESC + "@";
                }
                else
                {
                    IsKotPrint = false;
                }
                //*******************************Send request to the printer****************************  
                for (int PrinterIndex = 0; PrinterIndex < dt1.Rows.Count; PrinterIndex++)
                {
                    dtPrinter = new DataTable();
                    PortName = dt1.Rows[PrinterIndex]["Print_Comport"].ToString();// To get the port of the printer
                    string deptid = dt1.Rows[PrinterIndex]["id"].ToString();
                    IP = dt1.Rows[PrinterIndex]["IP"].ToString();
                    string id = string.Empty;
                    string Counter_Name = dt1.Rows[PrinterIndex]["Counter_Name"].ToString();
                    // get item detail from department id *********************** Changes on 18-12-2015 by Sujit
                    if (clsConfigSettings.IS_KOT_Print_dept == "1")
                        str1 = "[Usp_Get_Item_KOT_CounterBillbydeptt_tb_Printer_KDS] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "',@kot_no='" + kot_no + "'"; ///Search item by Department wise///////////// Changes 22-06-2012 ///
                    // get item detail from super department id *********************** Changes on 18-12-2015 by Sujit
                    else if (clsConfigSettings.IS_KOT_Print_dept == "2")
                        str1 = "[Usp_Get_Item_KOT_CounterBillbydeptt_tb_Printer_SD_KDS] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "',@kot_no='" + kot_no + "'"; ///Search Item by Super Department wise///////////// Changes 18-12-2015 ///
                    // get item detail from counter id *********************** Changes on 18-12-2015 by Sujit
                    else
                        str1 = "[Usp_Get_Item_KOT_CounterBillbyCounter_tb_Printer_KDS] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "',@kot_no='" + kot_no + "'"; ///Search Item by Kot wise///////////// Changes 22-06-2012 ///                                                                       


                    DataTable dt2 = ADOC.GetObject.GetTable(str1);
                    if (clsConfigSettings.Multi_Printer == "1")
                        dtPrinter = GetAllPrinter_multiPrinter(ds.Tables[2], Convert.ToInt64(deptid));
                    else if (clsConfigSettings.PrintKOT_SectionWise == "1")
                        dtPrinter = GetAllPrinter_multiPrinter(ds.Tables[2], Convert.ToInt64(deptid));
                    else
                    {
                        dtPrinter.Columns.Add("Printer_Name");
                        dtPrinter.Columns.Add("Config_id_fk");
                        dtPrinter.Columns.Add("ip_address");
                        dtPrinter.Rows.Add(IP, deptid);
                    }
                    for (int p = 0; p < dtPrinter.Rows.Count; p++)
                    {
                        PortName = dtPrinter.Rows[p]["Printer_Name"].ToString();
                        IP = dtPrinter.Rows[p]["ip_address"].ToString();

                        for (int PrintIndex = 0; PrintIndex <= NoofPrint; PrintIndex++)
                        {
                            if (clsConfigSettings.isKOT_3or4_EnchPrint == "1")
                                KOTPrint4Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dt2, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, kot_no, Kot_Title, OrderRemarks, Counter_Name, CashierName, BillType, TableCover, TableName);
                            else
                                KOTPrint3Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dt2, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, kot_no, OrderRemarks, Counter_Name, CashierName, TableCover, TableName);

                            SerialPort comport = new SerialPort();
                            comport.PortName = PortName;
                            comport.BaudRate = 9600;
                            comport.Parity = Parity.None;
                            comport.DataBits = 8;
                            comport.StopBits = StopBits.One;
                            bool result = false;
                            try
                            {
                                comport.Open();
                                if (comport.IsOpen == true)
                                {

                                }
                                result = CheckReadDatabyIPAddressorPortWise(comport, SendOut, IP);
                                if (result == true)
                                {

                                    string sql = "Usp_UpdateKOTPrinted_tempkot @Bill_No='" + BillNo + "',@Cashier='KDS',@tempid='" + AllIdOut.TrimEnd(',') + "',@assortedid='" + AllAssortedIdOut.TrimEnd(',') + "'";
                                    ADOC.GetObject.ExecuteDML(sql);
                                    comport.Close();
                                    comport.Dispose();
                                    Send = null;
                                    SendOut = null;
                                    comport = null;
                                    IsKotPrint = true;
                                    //MessageBox.Show("Printer is connected!");
                                }
                                else
                                {
                                    comport.Close();
                                    comport.Dispose();
                                    Send = null;
                                    SendOut = null;
                                    comport = null;
                                    IsKotPrint = false;
                                    ADOC.GetObject.ExecuteDML("Usp_CreateError_log @Form_Name='btnKOTOrder Line:1757',@Error_COde='001',@Error_name='" + PortName + " :Printer is not Connected :" + BillNo + "',@Bill_date='" + Program.Bill_Date + "',@created_by='KDS'");
                                    MessageBox.Show("Printer is not connected !!!");
                                    //if (DialogResult.Yes == MessageBox.Show("Printer is not connected !!! Do You want to print again !!! ", "Print KOT", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                    //{
                                    //    Print_KOT(BillNo, companyName, KotTitle, KOT_Type, objDgv);
                                    //}
                                }

                            }
                            catch (Exception ex)
                            {
                                comport.Close();
                                comport.Dispose();
                                Send = null;
                                comport = null;
                                SendOut = null;
                                IsKotPrint = false;
                                MessageBox.Show(ex.Message + " : Printer is not connected!");
                                ADOC.GetObject.ExecuteDML("Usp_CreateError_log @Form_Name='btnKOTOrder Line:1771',@Error_COde='002',@Error_name='The port " + PortName + " does not exist -" + BillNo + "',@Bill_date='" + Program.Bill_Date + "',@created_by='KDS'");
                                return false;

                            }
                        }
                    }
                }

                string CashierPrinter = clsConfigSettings.PrinterName;
                if (CashierPrinter.Trim().Length > 0)
                {
                    PrintCashierCopy(BillNo, Header, "Cashier Copy", KOT_Type, date, Time, objDgv, CashierPrinter, OrderRemarks, CashierName, TableName, TableCover);
                }
                string IsPrintDefauktKOT = clsConfigSettings.IsPrintDefaultKOT;
                if (IsPrintDefauktKOT == "1")
                {
                    PrintDefaultKOT(BillNo, Header, "Cashier Copy", KOT_Type, date, Time, objDgv, CashierPrinter, OrderRemarks, KotTitle, TableName, TableCover, CashierName);
                }
            }
            catch { }
            return IsKotPrint;
        }

        #endregion


    }
}
