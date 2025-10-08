using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using TuchScreenApp1Jan2013.App_Code;
using System.Drawing.Printing;
using LPrinterTest;
using Order_Display_System.App_Code;
using Order_Display_System;

namespace TuchScreenApp1Jan2013.UI
{
    public partial class TSC_Print : Form
    {
        [DllImport("TSCLIB.dll", EntryPoint = "about")]
        public static extern bool about();

        [DllImport("TSCLIB.dll", EntryPoint = "openport")]
        public static extern bool openport(string printer);

        [DllImport("TSCLIB.dll", EntryPoint = "barcode")]
        public static extern int barcode(string x, string y, string type,
        string height, string readable, string rotation,
        string narrow, string wide, string code);

        [DllImport("TSCLIB.dll", EntryPoint = "clearbuffer")]
        public static extern int clearbuffer();

        [DllImport("TSCLIB.dll", EntryPoint = "closeport")]
        public static extern int closeport();

        [DllImport("TSCLIB.dll", EntryPoint = "downloadpcx")]
        public static extern int downloadpcx(string filename, string image_name);

        [DllImport("TSCLIB.dll", EntryPoint = "formfeed")]
        public static extern int formfeed();

        [DllImport("TSCLIB.dll", EntryPoint = "nobackfeed")]
        public static extern int nobackfeed();

        [DllImport("TSCLIB.dll", EntryPoint = "printerfont")]
        public static extern int printerfont(string x, string y, string fonttype,
        string rotation, string xmul, string ymul,
        string text);

        [DllImport("TSCLIB.dll", EntryPoint = "printlabel")]
        public static extern int printlabel(string set, string copy);

        [DllImport("TSCLIB.dll", EntryPoint = "sendcommand")]
        public static extern int sendcommand(string printercommand);

        [DllImport("TSCLIB.dll", EntryPoint = "setup")]
        public static extern int setup(string width, string height,
        string speed, string density,
        string sensor, string vertical,
        String offset);

        [DllImport("TSCLIB.dll", EntryPoint = "windowsfont")]
        public static extern int windowsfont(int x, int y, int fontheight,
        int rotation, int fontstyle, int fontunderline,
        string szFaceName, string content);

        public TSC_Print()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                openport("TSC TTP-244 Pro");
                setup("50.8", "42.5", "4", "8", "0", "0", "0"); // Setup the media size and sensor type info
                clearbuffer(); // Clear image buffer
                //barcode("100", "100", "128", "100", "1", "0", "2", "2", "Barcode Test"); // Drawing barcode
                //printerfont("4", "2.5", "3", "0", "1", "1", "Drawing printer font"); // Drawing printer font
                //printerfont("4", "3.5", "3", "0", "1", "1", "Drawing printer font"); // Drawing printer font
                windowsfont(35, 250, 24, 0, 0, 0, "ARIAL", "Windows Arial Font Test"); // Draw windows font
                windowsfont(35, 275, 24, 0, 0, 0, "ARIAL", "Hello world"); // Draw windows font
                windowsfont(35, 300, 24, 0, 0, 0, "ARIAL", "Test"); // Draw windows font
                //downloadpcx ("UL.PCX", "UL.PCX"); // Download PCX file into printer
                //sendcommand ("PUTPCX 100,400, /" UL.PCX / ""); // Drawing PCX graphic
                printlabel("1", "1"); // Print labels
                closeport(); // Close specified printer driver
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region get all printer
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
        #endregion



        #region print kot from barcode and normal thermal printer
        /// <summary>
        /// print kot from barcode and normal thermal printer
        /// </summary>
        /// <param name="BillNo"></param>
        /// <param name="companyName"></param>
        /// <param name="KotTitle"></param>
        /// <param name="KOT_Type"></param>
        /// <param name="objDgv"></param>
        /// <param name="kot_no"></param>
        /// <returns></returns>

        public bool Print_KOT_LableWise(string BillNo, string companyName, string KotTitle, string KOT_Type,
            DataGridView objDgv, string kot_no, string CashierName, string BillType, string TableCover, string TableName,string Order_flag,string Order_ids_fk)
        {
            bool IsKotPrint = false;
            try
            {
                #region
                DataTable dtTran = new DataTable();
                dtTran.Columns.Add("i_name");
                dtTran.Columns.Add("i_code");
                dtTran.Columns.Add("qty");
                dtTran.Columns.Add("amount");
                dtTran.Columns.Add("counter_id");
                dtTran.Columns.Add("comments");
                dtTran.Columns.Add("id");
                dtTran.Columns.Add("urgentitem");
                dtTran.Columns.Add("item_index");
                #endregion
                string OrderRemarks = string.Empty;
                string AllId = string.Empty;
                string AllAssortedId = string.Empty;
                string Kot_Title = "";
                //KotTitle = KotTitle;
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
                clsPrintKOTsummary objKot = new clsPrintKOTsummary();


                ////////////////Print KOT //////////////////////////////////////               
                DataTable dt1 = new DataTable();
                DataSet ds = new DataSet();

                // Get Printer Name and department id ************** changes on 18-12-2015 by Sujit
                if (clsConfigSettings.IS_KOT_Print_dept == "1")
                    ds = ADOC.GetObject.GetDatset("[Usp_Get_Printerwithdept_kotBilling_printer_KDS] @Bill_No_FK='" + BillNo + "',@kot_no='" + kot_no + "',@Order_flag='" + Order_flag + "',@id_fk='"+Order_ids_fk+"'", "tbl_bill");
                // Get Printer Name and super department id ************** changes on 18-12-2015 by Sujit
                else if (clsConfigSettings.IS_KOT_Print_dept == "2")
                    ds = ADOC.GetObject.GetDatset("[Usp_Get_Printerwithdept_kotBilling_printer_SD_KDS] @Bill_No_FK='" + BillNo + "',@kot_no='" + kot_no + "',@Order_flag='" + Order_flag + "',@id_fk='" + Order_ids_fk + "'", "tbl_bill");
                // Get Printer Name and counter id ************** changes on 18-12-2015 by Sujit
                else
                    ds = ADOC.GetObject.GetDatset("[Usp_Get_PrinterwithCounterWise_kotBilling_printer_KDS] @Bill_No_FK='" + BillNo + "',@kot_no='" + kot_no + "',@Order_flag='" + Order_flag + "',@id_fk='" + Order_ids_fk + "'", "tbl_bill");

                dt1 = ds.Tables[0];
                DataTable dt = ds.Tables[1];
                if (dt1.Rows.Count > 0)
                {
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
                    string Isbarcode_Printer = dt1.Rows[PrinterIndex]["Isbarcode_Printer"].ToString();
                    string id = string.Empty;
                    if (CurrentPrinter.ToLower() == "none")
                    {
                        continue;
                    }

                    // get item detail from department id *********************** Changes on 18-12-2015 by Sujit
                    if (clsConfigSettings.IS_KOT_Print_dept == "1")
                        str1 = "[Usp_Get_Item_KOT_CounterBillbydeptt_tb_Printer_KDS] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "',@kot_no='" + kot_no + "',@Order_flag='" + Order_flag + "',@id_fk='" + Order_ids_fk + "'"; ///Search item by Department wise///////////// Changes 22-06-2012 ///
                    // get item detail from super department id *********************** Changes on 18-12-2015 by Sujit
                    else if (clsConfigSettings.IS_KOT_Print_dept == "2")
                        str1 = "[Usp_Get_Item_KOT_CounterBillbydeptt_tb_Printer_SD_KDS] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "',@kot_no='" + kot_no + "',@Order_flag='" + Order_flag + "',@id_fk='" + Order_ids_fk + "'"; ///Search Item by Super Department wise///////////// Changes 18-12-2015 ///
                    // get item detail from counter id *********************** Changes on 18-12-2015 by Sujit
                    else
                        str1 = "[Usp_Get_Item_KOT_CounterBillbyCounter_tb_Printer_KDS] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "',@kot_no='" + kot_no + "',@Order_flag='" + Order_flag + "',@id_fk='" + Order_ids_fk + "'"; ///Search Item by Kot wise///////////// Changes 22-06-2012 ///                                                                       

                    DataTable dt2 = ADOC.GetObject.GetTable(str1);
                    if (clsConfigSettings.Multi_Printer == "1")
                        dtPrinter = GetAllPrinter_multiPrinter(ds.Tables[2], Convert.ToInt64(deptid));
                    else if (clsConfigSettings.PrintKOT_SectionWise == "1")
                        dtPrinter = GetAllPrinter_multiPrinter(ds.Tables[2], Convert.ToInt64(deptid));
                    else
                    {
                        dtPrinter.Columns.Add("Printer_Name");
                        dtPrinter.Columns.Add("Config_id_fk");
                        dtPrinter.Columns.Add("Isbarcode_Printer");
                        dtPrinter.Rows.Add(CurrentPrinter, deptid, Isbarcode_Printer);
                    }
                    for (int p = 0; p < dtPrinter.Rows.Count; p++)
                    {
                        //pick printer
                        CurrentPrinter = dtPrinter.Rows[p]["Printer_Name"].ToString();
                        Isbarcode_Printer = dtPrinter.Rows[p]["Isbarcode_Printer"].ToString();
                        if (CurrentPrinter.ToLower() == "default")
                            CurrentPrinter = settings.PrinterName;
                        //PosPrinter.Open("testDoc", CurrentPrinter);

                        //openport(CurrentPrinter);
                        ////setup("100", "63.5", "4", "8", "0", "0", "0"); // Setup the media size and sensor type info
                        //setup("50.8", "42.2", "4", "8", "0", "0", "0"); // Setup the media size and sensor type info
                        //clearbuffer();
                        if (Isbarcode_Printer == "1")
                        {
                            for (int PrintIndex = 0; PrintIndex < NoofPrint; PrintIndex++)
                            {
                                objKot.KOTPrint3InchLablePrinting(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dt2, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, kot_no, Kot_Title, OrderRemarks, TableName);

                                try
                                {
                                    string[] arrPrint = SendOut.Split('&');
                                    string strValues = string.Empty;
                                    for (int item = 2; item < arrPrint.Length; item++)
                                    {
                                        bool IsOpen = openport(CurrentPrinter);
                                        //{
                                        //setup("100", "63.5", "4", "8", "0", "0", "0"); // Setup the media size and sensor type info

                                        //desc("width", "hieght", "speend=4","darkness", "0", "0", "0"); // Setup the media size and sensor type info
                                        setup("50.8", "42.5", "4", "8", "0", "0", "0"); // Setup the media size and sensor type info
                                        clearbuffer();
                                        if (arrPrint[item].ToString().Trim().Length > 0)
                                        {
                                            //windowsfont(33,(100) + Count, 30, 0, 0, 0, "ARIAL", strValues + "\r"); // Draw windows font
                                            //Count = Count + 25;

                                            string BillNowithDateTime = arrPrint[0].ToString();
                                            string TableNowithDateTime = arrPrint[1].ToString();
                                            strValues = UppercaseFirst(arrPrint[item].ToString().ToLower());

                                            //detail(print from line no=5,line space for each line), font, 0, 0, 0, "font name", comment+ "\r"); 
                                            windowsfont(1, (0), 33, 0, 0, 0, "Helvetica", "             " + KotTitle + "\r"); // Draw windows font
                                            windowsfont(1, (5) + 35, 33, 0, 0, 0, "Helvetica", BillNowithDateTime + "\r"); // Draw windows font
                                            windowsfont(1, (20) + 55, 33, 0, 0, 0, "Helvetica", TableNowithDateTime + "\r"); // Draw windows font
                                            windowsfont(1, (20) + 75, 30, 0, 0, 0, "Helvetica", "--------------------------------------------------\r"); // Draw windows font
                                            //windowsfont(5, (100) + 100, 30, 0, 0, 0, "ARIAL", strValues + "\r"); // Draw windows font
                                            //DataTable dtComments = GetAllComments(strValues, dt2);
                                            string[] str_Iname = strValues.Split('`');
                                            if (str_Iname.Length > 0)
                                            {
                                                windowsfont(1, (20) + 100, 33, 0, 0, 0, "Helvetica", str_Iname[0].ToLower() + "\r"); // Draw windows font
                                                if (str_Iname.Length > 1)
                                                    windowsfont(1, (20) + 125, 33, 0, 0, 0, "Helvetica", ">>" + str_Iname[1].ToLower() + "\r"); // Draw windows font
                                            }
                                            if (str_Iname.Length > 1)
                                            {
                                                windowsfont(1, (20) + 150, 30, 0, 0, 0, "Helvetica", "--------------------------------------------------\r"); // Draw windows font
                                                windowsfont(1, (20) + 175, 30, 0, 0, 0, "Helvetica", "STWD: " + CashierName.ToUpper() + " (KOT NO:" + kot_no + ")" + "\r"); // Draw windows font
                                            }
                                            else
                                            {
                                                windowsfont(1, (20) + 125, 30, 0, 0, 0, "Helvetica", "--------------------------------------------------\r"); // Draw windows font
                                                windowsfont(1, (20) + 150, 30, 0, 0, 0, "Helvetica", "STWD: " + CashierName.ToUpper() + " (KOT NO:" + kot_no + ")" + "\r"); // Draw w
                                            }
                                        }
                                        int Result = printlabel("1", "1"); // Print labels
                                        closeport(); // Close specified printer driver
                                        //}
                                    }
                                    string sql = "Usp_UpdateKOTPrinted_tempkot @Bill_No='" + BillNo + "',@Cashier='KDS',@tempid='" + AllIdOut.TrimEnd(',') + "',@assortedid='" + AllAssortedIdOut.TrimEnd(',') + "'";
                                    ADOC.GetObject.ExecuteDML(sql);
                                    Send = null;
                                    SendOut = null;
                                    IsKotPrint = true;
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                    closeport();
                                    Send = null;
                                    SendOut = null;
                                    IsKotPrint = false;
                                    ADOC.GetObject.ExecuteDML("Usp_CreateError_log @Form_Name='btnKOTOrder Line:1771',@Error_COde='002',@Error_name='The port " + PortName + " does not exist -" + BillNo + "',@Bill_date='" + Program.Bill_Date + "',@created_by='KDS'");
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            PosPrinter.Open("testDoc", CurrentPrinter);
                            for (int PrintIndex = 0; PrintIndex < NoofPrint; PrintIndex++)
                            {
                                if (clsConfigSettings.Single_Item_kot != "1")
                                {
                                    if (clsConfigSettings.isKOT_3or4_EnchPrint == "1")
                                        objKot.KOTPrint4Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dt2, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, kot_no, Kot_Title, OrderRemarks, Counter_Name, CashierName, BillType, TableCover, TableName);
                                    else
                                        objKot.KOTPrint3Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dt2, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, kot_no, OrderRemarks, Counter_Name, CashierName, TableCover, TableName);

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
                                else
                                {
                                    for (int s = 0; s < dt2.Rows.Count; s++)
                                    {
                                        dtTran.Rows.Clear();
                                        string i_name = dt2.Rows[s]["i_name"].ToString();
                                        string i_code = dt2.Rows[s]["i_code"].ToString();
                                        string qty = dt2.Rows[s]["qty"].ToString();
                                        string amount = dt2.Rows[s]["amount"].ToString();
                                        string counter_id = dt2.Rows[s]["counter_id"].ToString();
                                        string comments = dt2.Rows[s]["comments"].ToString();
                                        string idtran = dt2.Rows[s]["id"].ToString();
                                        string urgentitem = dt2.Rows[s]["urgentitem"].ToString();
                                        string item_index = dt2.Rows[s]["item_index"].ToString();

                                        dtTran.Rows.Add(i_name, i_code, qty, amount, counter_id, comments, idtran, urgentitem, item_index);
                                        if (clsConfigSettings.isKOT_3or4_EnchPrint == "1")
                                            objKot.KOTPrint4Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dtTran, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, kot_no, Kot_Title, OrderRemarks, Counter_Name, CashierName, BillType, TableCover, TableName);
                                        else
                                            objKot.KOTPrint3Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dtTran, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, kot_no, OrderRemarks, Counter_Name, CashierName, TableCover, TableName);

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
                                            throw ex;
                                            ADOC.GetObject.ExecuteDML("Usp_CreateError_log @Form_Name='btnKOTOrder Line:1771',@Error_COde='002',@Error_name='The port " + PortName + " does not exist -" + BillNo + "',@Bill_date='" + Program.Bill_Date + "',@created_by='KDS'");
                                            return false;
                                        }
                                    }
                                }
                                PosPrinter.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
            return IsKotPrint;
        }
        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
        #endregion


        public bool Print_KOT_Counter_DeptWise_Normal_LablePrint(string BillNo, string companyName, string KotTitle, string KOT_Type,
    DataGridView objDgv, string KOT_No, string Remarks, string CashierName, string TableCover, string TableName,string Bill_Type)
        {
            #region
            DataTable dtTran = new DataTable();
            dtTran.Columns.Add("i_name");
            dtTran.Columns.Add("i_code");
            dtTran.Columns.Add("qty");
            dtTran.Columns.Add("amount");
            dtTran.Columns.Add("counter_id");
            dtTran.Columns.Add("comments");
            dtTran.Columns.Add("id");
            dtTran.Columns.Add("urgentitem");
            dtTran.Columns.Add("item_index");
            #endregion
            string AllId = string.Empty;
            string AllAssortedId = string.Empty;
            bool isKoTComboPrint = false;
            string Kot_Title = KotTitle;
            //KotTitle = "KOT Type  : " + KotTitle;
            KOT_Type = "Order Type: " + KOT_Type;

            string NC_Item = clsConfigSettings.NC_Item;
            //string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
            //string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
            string Header = clsConfigSettings.header;
            string LineonKOT = clsConfigSettings.LineOnKOT;
            string PortName = string.Empty;
            //string IP = string.Empty;
            int NoofPrint = Convert.ToInt32(clsConfigSettings.No_of_Kot);
            string Send = null;
            bool IsKotPrint = false;
            string str1 = string.Empty;
            DateTime date = DateTime.ParseExact(Program.Bill_Date, "yyyy-MM-dd", null);
            string Time = System.DateTime.Now.ToShortTimeString();
            string SendOut = string.Empty;
            string AllIdOut = string.Empty;
            string AllAssortedIdOut = string.Empty;

            clsPrintKOTsummary objKot = new clsPrintKOTsummary();
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
                ds = ADOC.GetObject.GetDatset("[Usp_Get_Printerwithdept_kotBilling_printer_Normal] @Bill_No_FK='" + BillNo + "'", "tbl_bill");
            // Get Printer Name and super department id ************** changes on 18-12-2015 by Sujit
            else if (clsConfigSettings.IS_KOT_Print_dept == "2")
                ds = ADOC.GetObject.GetDatset("[Usp_Get_Printerwithdept_kotBilling_printer_Normal_SD] @Bill_No_FK='" + BillNo + "'", "tbl_bill");
            // Get Printer Name and counter id ************** changes on 18-12-2015 by Sujit
            else
                ds = ADOC.GetObject.GetDatset("[Usp_Get_PrinterwithCounterWise_kotBilling_printer_Normal] @Bill_No_FK='" + BillNo + "'", "tbl_bill");

            dt1 = ds.Tables[0];
            DataTable dt = ds.Tables[1];
            if (dt1.Rows.Count > 0)
            {

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
                string Isbarcode_Printer = dt1.Rows[PrinterIndex]["Isbarcode_Printer"].ToString();

                string id = string.Empty;

                //CurrentPrinter = PrintterAccordinttoDeptt;
                if (CurrentPrinter.ToLower() == "none")
                {
                    continue;
                }

                // get item detail from department id *********************** Changes on 18-12-2015 by Sujit
                if (clsConfigSettings.IS_KOT_Print_dept == "1")
                    str1 = "[Usp_Get_Item_KOT_CounterBillbydeptt_tb_Printer_Normal] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "'"; ///Search item by Department wise///////////// Changes 22-06-2012 ///
                // get item detail from super department id *********************** Changes on 18-12-2015 by Sujit
                else if (clsConfigSettings.IS_KOT_Print_dept == "2")
                    str1 = "[Usp_Get_Item_KOT_CounterBillbydeptt_tb_Printer_Normal_SD] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "'"; ///Search Item by Super Department wise///////////// Changes 18-12-2015 ///
                // get item detail from counter id *********************** Changes on 18-12-2015 by Sujit
                else
                    str1 = "[Usp_Get_Item_KOT_CounterBillbyCounter_tb_Printer_normal] @Bill_No_FK='" + BillNo + "',@dept='" + deptid + "'"; ///Search Item by Kot wise///////////// Changes 22-06-2012 ///                                                                       

                DataTable dt2 = ADOC.GetObject.GetTable(str1);

                if (clsConfigSettings.Multi_Printer == "1")
                    dtPrinter = GetAllPrinter_multiPrinter(ds.Tables[2], Convert.ToInt64(deptid));
                else if (clsConfigSettings.PrintKOT_SectionWise == "1")
                    dtPrinter = GetAllPrinter_multiPrinter(ds.Tables[2], Convert.ToInt64(deptid));
                else
                {
                    dtPrinter.Columns.Add("Printer_Name");
                    dtPrinter.Columns.Add("Config_id_fk");
                    dtPrinter.Columns.Add("Isbarcode_Printer");
                    dtPrinter.Rows.Add(CurrentPrinter, deptid, Isbarcode_Printer);
                }
                for (int p = 0; p < dtPrinter.Rows.Count; p++)
                {
                    //pick printer
                    CurrentPrinter = dtPrinter.Rows[p]["Printer_Name"].ToString();
                    if (CurrentPrinter.ToLower() == "default")
                        CurrentPrinter = settings.PrinterName;
                    Isbarcode_Printer = dtPrinter.Rows[p]["Isbarcode_Printer"].ToString();
                    //PosPrinter.Open("testDoc", CurrentPrinter);
                    if (Isbarcode_Printer == "1")
                    {
                        for (int PrintIndex = 0; PrintIndex < NoofPrint; PrintIndex++)
                        {
                            objKot.KOTPrint3InchLablePrinting(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dt2, id,
                                LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, KOT_No, Kot_Title, Remarks,TableName);

                            try
                            {
                                string[] arrPrint = SendOut.Split('&');
                                string strValues = string.Empty;
                                for (int item = 2; item < arrPrint.Length; item++)
                                {
                                    openport(CurrentPrinter);
                                    setup("50.8", "42.5", "4", "8", "0", "0", "0"); // Setup the media size and sensor type info

                                    //desc("width", "hieght", "speend=4","darkness", "0", "0", "0"); // Setup the media size and sensor type info
                                    setup("50.8", "42.5", "4", "8", "0", "0", "0"); // Setup the media size and sensor type info
                                    clearbuffer();
                                    if (arrPrint[item].ToString().Trim().Length > 0)
                                    {
                                        //windowsfont(33,(100) + Count, 30, 0, 0, 0, "ARIAL", strValues + "\r"); // Draw windows font
                                        //Count = Count + 25;

                                        string BillNowithDateTime = arrPrint[0].ToString();
                                        string TableNowithDateTime = arrPrint[1].ToString();
                                        strValues = UppercaseFirst(arrPrint[item].ToString().ToLower());

                                        windowsfont(1, (0), 33, 0, 0, 0, "Helvetica", "             " + KotTitle + "\r"); // Draw windows font
                                        windowsfont(1, (5) + 35, 33, 0, 0, 0, "Helvetica", BillNowithDateTime + "\r"); // Draw windows font
                                        windowsfont(1, (20) + 55, 33, 0, 0, 0, "Helvetica", TableNowithDateTime + "\r"); // Draw windows font
                                        windowsfont(1, (20) + 75, 30, 0, 0, 0, "Helvetica", "--------------------------------------------------\r"); // Draw windows font
                                        //windowsfont(5, (100) + 100, 30, 0, 0, 0, "ARIAL", strValues + "\r"); // Draw windows font
                                        //DataTable dtComments = GetAllComments(strValues, dt2);
                                        string[] str_Iname = strValues.Split('`');
                                        if (str_Iname.Length > 0)
                                        {
                                            windowsfont(1, (20) + 100, 33, 0, 0, 0, "Helvetica", str_Iname[0].ToLower() + "\r"); // Draw windows font
                                            if (str_Iname.Length > 1)
                                                windowsfont(1, (20) + 125, 33, 0, 0, 0, "Helvetica", ">>" + str_Iname[1].ToLower() + "\r"); // Draw windows font
                                        }
                                        if (str_Iname.Length > 1)
                                        {
                                            windowsfont(1, (20) + 150, 30, 0, 0, 0, "Helvetica", "--------------------------------------------------\r"); // Draw windows font
                                            windowsfont(1, (20) + 175, 30, 0, 0, 0, "Helvetica", "STWD: " + CashierName.ToUpper() + " (KOT NO:" + KOT_No + ")" + "\r"); // Draw windows font
                                        }
                                        else
                                        {
                                            windowsfont(1, (20) + 125, 30, 0, 0, 0, "Helvetica", "--------------------------------------------------\r"); // Draw windows font
                                            windowsfont(1, (20) + 150, 30, 0, 0, 0, "Helvetica", "STWD: " + CashierName.ToUpper() + " (KOT NO:" + KOT_No + ")" + "\r"); // Draw w
                                        }
                                    }
                                    printlabel("1", "1"); // Print labels
                                    closeport(); // Close specified printer driver
                                }
                                // string sql = "Usp_UpdateKOTPrinted_tempkot @Bill_No='" + BillNo + "',@Cashier='" + Program.UserName + "',@tempid='" + AllIdOut.TrimEnd(',') + "',@assortedid='" + AllAssortedIdOut.TrimEnd(',') + "'";
                                //ADOC.GetObject.ExecuteDML(sql);
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
                    }
                    else
                    {
                        PosPrinter.Open("testDoc", CurrentPrinter);
                        for (int PrintIndex = 0; PrintIndex < NoofPrint; PrintIndex++)
                        {
                            if (clsConfigSettings.Single_Item_kot != "1")
                            {
                                if (clsConfigSettings.isKOT_3or4_EnchPrint == "1")
                                    objKot.KOTPrint4Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dt2, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, KOT_No, Kot_Title, Remarks, Counter_Name, CashierName, Bill_Type, TableCover, TableName);
                                else
                                    objKot.KOTPrint3Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dt2, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, KOT_No, Remarks, Counter_Name,CashierName,TableCover,TableName);
                                try
                                {
                                    PosPrinter.Print(SendOut);

                                    string sql = "Usp_UpdateKOTPrinted_tempkot @Bill_No='" + BillNo + "',@Cashier='',@tempid='" + AllIdOut.TrimEnd(',') + "',@assortedid='" + AllAssortedIdOut.TrimEnd(',') + "'";
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
                            else
                            {
                                for (int s = 0; s < dt2.Rows.Count; s++)
                                {
                                    dtTran.Rows.Clear();
                                    string i_name = dt2.Rows[s]["i_name"].ToString();
                                    string i_code = dt2.Rows[s]["i_code"].ToString();
                                    string qty = dt2.Rows[s]["qty"].ToString();
                                    string amount = dt2.Rows[s]["amount"].ToString();
                                    string counter_id = dt2.Rows[s]["counter_id"].ToString();
                                    string comments = dt2.Rows[s]["comments"].ToString();
                                    string idtran = dt2.Rows[s]["id"].ToString();
                                    string urgentitem = dt2.Rows[s]["urgentitem"].ToString();
                                    string item_index = dt2.Rows[s]["item_index"].ToString();

                                    dtTran.Rows.Add(i_name, i_code, qty, amount, counter_id, comments, idtran, urgentitem, item_index);
                                    if (clsConfigSettings.isKOT_3or4_EnchPrint == "1")
                                        objKot.KOTPrint4Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dtTran, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, KOT_No, Kot_Title, Remarks, Counter_Name,CashierName,Bill_Type,TableCover,TableName);
                                    else
                                        objKot.KOTPrint3Inch(Send, companyName, Header, KotTitle, KOT_Type, BillNo, str1, AllId, AllAssortedId, dtTran, id, LineonKOT, objDgv, NC_Item, date, Time, dt, out SendOut, out AllIdOut, out AllAssortedIdOut, KOT_No, Remarks, Counter_Name,CashierName,TableCover,TableName);
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
                            }
                        }
                        PosPrinter.Close();
                    }
                }
            }

            return IsKotPrint;
        }

        private void TSC_Print_Load(object sender, EventArgs e)
        {
            try
            {
                //DLL.GetPrinter(cmbPrinter);
            }
            catch { }
        }
    }
}
