using KOTPrintUtility.App_Code.Data_Set;
using KOTPrintUtility.Report;
using LPrinterTest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuchScreenApp1Jan2013.UI;

namespace KOTPrintUtility.App_Code
{
    public class ClsPrintBill
    {
        private const char ESC = (char)27;
        private const char LF = (char)10;
        private const char FS = (char)28;
        private const char GS = (char)29;



        public static IEnumerable<string> Split(string inputText, int maxLength)
        {
            int i = 0;
            while (i + maxLength < inputText.Length)
            {
                var lastIndexofUnderScore = inputText.LastIndexOf(' ', i + maxLength);
                var lastIndexofSpace = inputText.LastIndexOf(' ', i + maxLength);
                var index = Math.Min(lastIndexofSpace, lastIndexofUnderScore);
                if (index <= 0)
                {
                    index = maxLength;
                }

                yield return inputText.Substring(i, index - i).Trim();

                i = index + 1;
            }

            yield return inputText.Substring(i).Trim();
        }

        public void PrintBillHomeDelivery(string Bill_no, string Cust_Code, int NoofPrint, string fin_year, out string NewTokenNo_fk, string BillType)
        {
            DataTable dtCustInfo = new DataTable();
            DataTable dtdiscount = new DataTable();
            string NewTokenNo = string.Empty;
            try
            {
                ADOC objADOC = new ADOC();
                //PrinterSettings settings = new PrinterSettings();
                //string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                //string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                //LPrinter PosPrinter = new LPrinter();
                //string CurrentPrinter = settings.PrinterName;
                string billTypeReportDisplay = string.Empty;
                ds_HouchScreenReport dsobjreport = new ds_HouchScreenReport();
                DataSet dsFinalreport = new DataSet();
                string SqlQuerry = string.Empty;
                billTypeReportDisplay = "Home Delivery";
                switch (BillType.ToUpper())
                {
                    case "C":
                        billTypeReportDisplay = "Take Away";
                        break;
                    case "D":
                        billTypeReportDisplay = "Dine In";
                        break;
                    case "H":
                        billTypeReportDisplay = "Home Delivery";
                        break;
                }
                SqlQuerry = "Usp_GetRetailInvoice_Print_HD @BillNo='" + Bill_no + "',@Cust_Code='" + Cust_Code + "'";
                if (fin_year != clsConfigSettings.fin_year)
                {
                    SqlQuerry = "Usp_GetRetailInvoice_Print_HD_fin_year @BillNo='" + Bill_no + "',@Cust_Code='" + Cust_Code + "',@fin_year='" + fin_year + "'";
                }

                dsFinalreport = objADOC.FillReportDataSet(SqlQuerry, dsobjreport, "tbl_Bill", "tbl_Bill_Tran", "TBL_CustomerInfo", "tbl_tax", "tbl_discount");
                dtCustInfo = dsFinalreport.Tables["TBL_CustomerInfo"];
                dtdiscount = dsFinalreport.Tables["tbl_discount"];
                rptInvoice_HD rptInvoiceobj = new rptInvoice_HD();
                using (ReportViewer rvobj = new ReportViewer())
                {
                    rptInvoiceobj.SetDataSource(dsFinalreport);
                    rvobj.cRViewer.ReportSource = rptInvoiceobj;
                    double RoundOff = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["RoundOff"].ToString());
                    string paymentMode = dsFinalreport.Tables["tbl_Bill"].Rows[0]["Payment_Mode"].ToString();
                    string web_order_comments = dsFinalreport.Tables["tbl_Bill"].Rows[0]["web_order_comments"].ToString();
                    NewTokenNo = Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["OrderNo"].ToString());
                    string IncludedTax = "**Included Tax @12.5% :";
                    double DeliveryCharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Other_Charge"].ToString());
                    if (DeliveryCharge > 0)
                    {
                        string Delivery_Charge = "Delivery Charge : ";
                        rptInvoiceobj.DataDefinition.FormulaFields["DeliveryCharge"].Text = "'" + Delivery_Charge + "    " + DeliveryCharge.ToString("N2") + "'";
                    }
                    rptInvoiceobj.DataDefinition.FormulaFields["C_name"].Text = "'" + dsFinalreport.Tables[0].Rows[0]["cashier"].ToString() + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["RoundOff"].Text = "'" + RoundOff.ToString("N2") + "'";
                    string PayMode = string.Empty;
                    string CardName = string.Empty;
                    // rptInvoiceobj.DataDefinition.FormulaFields["PaymentMode"].Text = "'" + mode + "'";

                    string DisCountCap = dsFinalreport.Tables[0].Rows[0]["Dis_Amount"].ToString();
                    NewTokenNo = dsFinalreport.Tables[0].Rows[0]["TockenNo"].ToString();
                    rptInvoiceobj.DataDefinition.FormulaFields["TockenNo"].Text = "'" + NewTokenNo + "'";
                    double Bill_Amount = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["Bill_Amount"].ToString());
                    double Taxamnt = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["tax1"].ToString());
                    string TaxCap = clsConfigSettings.TaxCap.ToString();
                    if ((Taxamnt) > 0)
                    {
                        if (Taxamnt > 0)
                        {
                            //IncludedTax = "*VAT @ 12.5% : ";
                            rptInvoiceobj.DataDefinition.FormulaFields["IncludedTax"].Text = "'" + TaxCap + "    " + Taxamnt.ToString("N2") + "'";
                        }
                        else
                        {
                            IncludedTax = TaxCap + " Included ** ";
                            rptInvoiceobj.DataDefinition.FormulaFields["IncludedTax"].Text = "'" + IncludedTax + "'";
                        }
                    }
                    double ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Charge_Amount"].ToString());
                    if (ServiceTax > 0)
                    {
                        string ServiceTaxCap = "Service Tax @" + clsConfigSettings.ServiceTax + "% :     ";
                        string STax = ServiceTaxCap + ServiceTax.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["ServiceTax"].Text = "'" + STax + "'";
                    }
                    double SBC_Tax = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["SBC_Tax"].ToString());
                    double KKC_Tax = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["KKC_Tax"].ToString());
                    if (SBC_Tax < 0)
                    {
                        string sbcamnt = "Service Tax @" + clsConfigSettings.SBC_Rate + "% :     ";
                        string STax = sbcamnt + SBC_Tax.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["sbc"].Text = "'" + STax + "'";
                    }
                    if (KKC_Tax < 0)
                    {
                        string sbcamnt = "Service Tax @" + clsConfigSettings.SBC_Rate + "% :     ";
                        string STax = sbcamnt + SBC_Tax.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["kkc"].Text = "'" + STax + "'";
                    }
                    DataTable dtTran = dsFinalreport.Tables["tbl_Bill_Tran"];
                    double BillAmnt = 0.0;
                    for (int i = 0; i < dtTran.Rows.Count; i++)
                    {
                        BillAmnt += double.Parse(dtTran.Rows[i]["Amount"].ToString());
                    }
                    double DisAmnt = double.Parse(DisCountCap);
                    double Sur_Charge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Sur_Charge"].ToString());
                    if (Sur_Charge > 0)
                    {
                        string Sur_ChargeCap = "Sur Charge@" + clsConfigSettings.SurCharge + "% :  on vat   ";
                        string Sur_ch = Sur_ChargeCap + Sur_Charge.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["surcharge"].Text = "'" + Sur_ch + "'";
                    }
                    //if (DisAmnt > 0)
                    //{
                    //    double PCTAmount = DisAmnt / BillAmnt * 100;
                    //    //DisCountCap = "Discount @" + PCTAmount.ToString("N0") + "%" + ", " + " Amount :";
                    //    DisCountCap = "Discount Amount :";
                    //    rptInvoiceobj.DataDefinition.FormulaFields["Dis_Count"].Text = "'" + DisCountCap + "   " + DisAmnt.ToString("N2") + "'";
                    //}
                    //else
                    //{
                    //    DisCountCap = "";
                    //    rptInvoiceobj.DataDefinition.FormulaFields["Dis_Count"].Text = "'" + DisCountCap + "'";
                    //    rptInvoiceobj.DataDefinition.FormulaFields["DisAmnt"].Text = "'" + DisCountCap + "'";
                    //}


                    if (paymentMode == "5")
                        rptInvoiceobj.DataDefinition.FormulaFields["grandTotal"].Text = "'0.00'";
                    else
                        rptInvoiceobj.DataDefinition.FormulaFields["grandTotal"].Text = "'" + Bill_Amount.ToString("N2") + "'";


                    //-----------Start Heder--------------------//////////                              
                    rptInvoiceobj.DataDefinition.FormulaFields["H_Company_Name"].Text = "'" + Program.CompanyName + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["H_address"].Text = "'" + Program.Address + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["H_Tin_No"].Text = "'" + Program.TinNo + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["H_Phone_No"].Text = "'" + Program.PhoneNo + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["Footer1"].Text = "'" + Program.Footer1 + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["Footer2"].Text = "'" + Program.Footer2 + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["STax_No"].Text = "'" + Program.STax_No + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["Footer3"].Text = "'" + Program.Footer3 + "'";
                    //rptInvoiceobj.DataDefinition.FormulaFields["C_name"].Text = "'" + Program.UserName + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["BillType"].Text = "'" + billTypeReportDisplay + "'";

                    rptInvoiceobj.DataDefinition.FormulaFields["fssai_no"].Text = "'" + "" + Program.fssai_no + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["cin_no"].Text = "'" + "CIN No :" + Program.cin_no + "'";
                    string channel = Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["Channel"].ToString()) + "(" +
                         Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["Channel_Order_Id"].ToString()) + ")";

                    rptInvoiceobj.DataDefinition.FormulaFields["Print_poweredby"].Text = "'" + clsConfigSettings.Print_poweredby + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["web_order_comments"].Text = "'" + web_order_comments.ToString() + "'";


                    rptInvoiceobj.DataDefinition.FormulaFields["Initial"].Text = "'" + clsConfigSettings.Initial + Bill_no + "'";

                    //clsBLL.GetPaymentMode(Bill_no, out PayMode, out CardName);
                    PayMode = Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["PayMode"].ToString());
                    string mode = "Payment Mode : " + PayMode;
                    if (PayMode.Length == 0)
                    {
                        mode = string.Empty;
                    }
                    rptInvoiceobj.DataDefinition.FormulaFields["OrderSource"].Text = "'" + channel + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["PaymentMode"].Text = "'" + mode + "'";
                    if (NoofPrint == 2)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 3)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 4)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 5)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 6)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    Program.TableNo = string.Empty;
                    //rvobj.Show();
                    rptInvoiceobj.Close();
                    rptInvoiceobj.Dispose();
                    AssortedItem.CancelAssortedItem();
                }
            }
            catch (Exception ex)
            {
            }
            NewTokenNo_fk = NewTokenNo;
        }



        public async Task PrintBillHomeDelivery_CrystalAsync(string Bill_no, string Cust_Code, int NoofPrint, string fin_year, Action<string> setTokenCallback, string BillType, string is_instant_order)
        {
            await Task.Run(() =>
            {
                string newToken = string.Empty;
                try
                {
                    if (is_instant_order == "1")
                    {
                        PrintBillHomeDelivery(Bill_no, Cust_Code, NoofPrint, fin_year, out newToken, BillType);
                    }
                    else
                    {
                        PrintBillHomeDelivery_Crystal(Bill_no, Cust_Code, NoofPrint, fin_year, out newToken, BillType);
                    }
                }
                catch (Exception ex)
                {
                    // Log the error
                }
                setTokenCallback?.Invoke(newToken); // safely pass token back to UI
            });
        }



        private void PrintBillHomeDelivery_Crystal(string Bill_no, string Cust_Code, int NoofPrint, string fin_year, out string NewTokenNo_fk, string BillType)
        {
            DataTable dtCustInfo = new DataTable();
            DataTable dtdiscount = new DataTable();
            string NewTokenNo = string.Empty;
            try
            {
                ADOC objADOC = new ADOC();
                //PrinterSettings settings = new PrinterSettings();
                //string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                //string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                //LPrinter PosPrinter = new LPrinter();
                //string CurrentPrinter = settings.PrinterName;
                string billTypeReportDisplay = string.Empty;
                ds_HouchScreenReport dsobjreport = new ds_HouchScreenReport();
                DataSet dsFinalreport = new DataSet();
                string SqlQuerry = string.Empty;

                billTypeReportDisplay = "Home Delivery";
                switch (BillType.ToUpper())
                {
                    case "C":
                        billTypeReportDisplay = "Take Away";
                        break;
                    case "D":
                        billTypeReportDisplay = "Dine In";
                        break;
                    case "H":
                        billTypeReportDisplay = "Home Delivery";
                        break;
                }
                SqlQuerry = "Usp_GetRetailInvoice_Print_HD @BillNo='" + Bill_no + "',@Cust_Code='" + Cust_Code + "'";
                if (fin_year != clsConfigSettings.fin_year)
                {
                    SqlQuerry = "Usp_GetRetailInvoice_Print_HD_fin_year @BillNo='" + Bill_no + "',@Cust_Code='" + Cust_Code + "',@fin_year='" + fin_year + "'";
                }

                dsFinalreport = objADOC.FillReportDataSet(SqlQuerry, dsobjreport, "tbl_Bill", "tbl_Bill_Tran", "TBL_CustomerInfo", "tbl_tax", "tbl_discount");
                dtCustInfo = dsFinalreport.Tables["TBL_CustomerInfo"];
                dtdiscount = dsFinalreport.Tables["tbl_discount"];
                rptInvoice_HD_NO_bolt rptInvoiceobj = new rptInvoice_HD_NO_bolt();
                using (ReportViewer rvobj = new ReportViewer())
                {
                    rptInvoiceobj.SetDataSource(dsFinalreport);
                    rvobj.cRViewer.ReportSource = rptInvoiceobj;
                    double RoundOff = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["RoundOff"].ToString());
                    string paymentMode = dsFinalreport.Tables["tbl_Bill"].Rows[0]["Payment_Mode"].ToString();
                    string web_order_comments = dsFinalreport.Tables["tbl_Bill"].Rows[0]["web_order_comments"].ToString();
                    NewTokenNo = Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["OrderNo"].ToString());
                    string IncludedTax = "**Included Tax @12.5% :";
                    double DeliveryCharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Other_Charge"].ToString());
                    if (DeliveryCharge > 0)
                    {
                        string Delivery_Charge = "Delivery Charge : ";
                        rptInvoiceobj.DataDefinition.FormulaFields["DeliveryCharge"].Text = "'" + Delivery_Charge + "    " + DeliveryCharge.ToString("N2") + "'";
                    }
                    rptInvoiceobj.DataDefinition.FormulaFields["C_name"].Text = "'" + dsFinalreport.Tables[0].Rows[0]["cashier"].ToString() + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["RoundOff"].Text = "'" + RoundOff.ToString("N2") + "'";
                    string PayMode = string.Empty;
                    string CardName = string.Empty;
                    // rptInvoiceobj.DataDefinition.FormulaFields["PaymentMode"].Text = "'" + mode + "'";

                    string DisCountCap = dsFinalreport.Tables[0].Rows[0]["Dis_Amount"].ToString();
                    NewTokenNo = dsFinalreport.Tables[0].Rows[0]["TockenNo"].ToString();
                    rptInvoiceobj.DataDefinition.FormulaFields["TockenNo"].Text = "'" + NewTokenNo + "'";
                    double Bill_Amount = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["Bill_Amount"].ToString());
                    double Taxamnt = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["tax1"].ToString());
                    string TaxCap = clsConfigSettings.TaxCap.ToString();
                    if ((Taxamnt) > 0)
                    {
                        if (Taxamnt > 0)
                        {
                            //IncludedTax = "*VAT @ 12.5% : ";
                            rptInvoiceobj.DataDefinition.FormulaFields["IncludedTax"].Text = "'" + TaxCap + "    " + Taxamnt.ToString("N2") + "'";
                        }
                        else
                        {
                            IncludedTax = TaxCap + " Included ** ";
                            rptInvoiceobj.DataDefinition.FormulaFields["IncludedTax"].Text = "'" + IncludedTax + "'";
                        }
                    }
                    double ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Charge_Amount"].ToString());
                    if (ServiceTax > 0)
                    {
                        string ServiceTaxCap = "Service Tax @" + clsConfigSettings.ServiceTax + "% :     ";
                        string STax = ServiceTaxCap + ServiceTax.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["ServiceTax"].Text = "'" + STax + "'";
                    }
                    double SBC_Tax = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["SBC_Tax"].ToString());
                    double KKC_Tax = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["KKC_Tax"].ToString());
                    if (SBC_Tax < 0)
                    {
                        string sbcamnt = "Service Tax @" + clsConfigSettings.SBC_Rate + "% :     ";
                        string STax = sbcamnt + SBC_Tax.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["sbc"].Text = "'" + STax + "'";
                    }
                    if (KKC_Tax < 0)
                    {
                        string sbcamnt = "Service Tax @" + clsConfigSettings.SBC_Rate + "% :     ";
                        string STax = sbcamnt + SBC_Tax.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["kkc"].Text = "'" + STax + "'";
                    }
                    DataTable dtTran = dsFinalreport.Tables["tbl_Bill_Tran"];
                    double BillAmnt = 0.0;
                    for (int i = 0; i < dtTran.Rows.Count; i++)
                    {
                        BillAmnt += double.Parse(dtTran.Rows[i]["Amount"].ToString());
                    }
                    double DisAmnt = double.Parse(DisCountCap);
                    double Sur_Charge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Sur_Charge"].ToString());
                    if (Sur_Charge > 0)
                    {
                        string Sur_ChargeCap = "Sur Charge@" + clsConfigSettings.SurCharge + "% :  on vat   ";
                        string Sur_ch = Sur_ChargeCap + Sur_Charge.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["surcharge"].Text = "'" + Sur_ch + "'";
                    }
                    //if (DisAmnt > 0)
                    //{
                    //    double PCTAmount = DisAmnt / BillAmnt * 100;
                    //    //DisCountCap = "Discount @" + PCTAmount.ToString("N0") + "%" + ", " + " Amount :";
                    //    DisCountCap = "Discount Amount :";
                    //    rptInvoiceobj.DataDefinition.FormulaFields["Dis_Count"].Text = "'" + DisCountCap + "   " + DisAmnt.ToString("N2") + "'";
                    //}
                    //else
                    //{
                    //    DisCountCap = "";
                    //    rptInvoiceobj.DataDefinition.FormulaFields["Dis_Count"].Text = "'" + DisCountCap + "'";
                    //    rptInvoiceobj.DataDefinition.FormulaFields["DisAmnt"].Text = "'" + DisCountCap + "'";
                    //}


                    if (paymentMode == "5")
                        rptInvoiceobj.DataDefinition.FormulaFields["grandTotal"].Text = "'0.00'";
                    else
                        rptInvoiceobj.DataDefinition.FormulaFields["grandTotal"].Text = "'" + Bill_Amount.ToString("N2") + "'";


                    //-----------Start Heder--------------------//////////                              
                    rptInvoiceobj.DataDefinition.FormulaFields["H_Company_Name"].Text = "'" + Program.CompanyName + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["H_address"].Text = "'" + Program.Address + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["H_Tin_No"].Text = "'" + Program.TinNo + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["H_Phone_No"].Text = "'" + Program.PhoneNo + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["Footer1"].Text = "'" + Program.Footer1 + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["Footer2"].Text = "'" + Program.Footer2 + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["STax_No"].Text = "'" + Program.STax_No + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["Footer3"].Text = "'" + Program.Footer3 + "'";
                    //rptInvoiceobj.DataDefinition.FormulaFields["C_name"].Text = "'" + Program.UserName + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["BillType"].Text = "'" + billTypeReportDisplay + "'";

                    rptInvoiceobj.DataDefinition.FormulaFields["fssai_no"].Text = "'" + "" + Program.fssai_no + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["cin_no"].Text = "'" + "CIN No :" + Program.cin_no + "'";
                    string channel = Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["Channel"].ToString()) + "(" +
                         Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["Channel_Order_Id"].ToString()) + ")";

                    rptInvoiceobj.DataDefinition.FormulaFields["Print_poweredby"].Text = "'" + clsConfigSettings.Print_poweredby + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["web_order_comments"].Text = "'" + web_order_comments.ToString() + "'";


                    rptInvoiceobj.DataDefinition.FormulaFields["Initial"].Text = "'" + clsConfigSettings.Initial + Bill_no + "'";
                    //clsBLL.GetPaymentMode(Bill_no, out PayMode, out CardName);
                    PayMode = Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["PayMode"].ToString());
                    string mode = "Payment Mode : " + PayMode;
                    if (PayMode.Length == 0)
                    {
                        mode = string.Empty;
                    }
                    rptInvoiceobj.DataDefinition.FormulaFields["OrderSource"].Text = "'" + channel + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["PaymentMode"].Text = "'" + mode + "'";
                    if (NoofPrint == 2)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 3)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 4)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 5)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 6)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    Program.TableNo = string.Empty;
                    //rvobj.Show();
                    rptInvoiceobj.Refresh();
                    rptInvoiceobj.Close();
                    rptInvoiceobj.Dispose();
                    AssortedItem.CancelAssortedItem();
                }
            }
            catch (Exception ex)
            {
            }
            NewTokenNo_fk = NewTokenNo;
        }

        public static void PrintBill_HD_TexFormat_new(string BillNo, string TableNo, string TenderAmount, string BalanceAmount, out string Token_No,
           out string OrderComment_out, string fin_year, string Bill_TypeName)

        {
            ADOC objADOC = new ADOC();

            string[] addr1;
            string[] addr2;
            string[] addr3;
            string OrderComment = string.Empty;
            string Send = string.Empty;
            string TockenNo_locall = string.Empty;
            string header = clsConfigSettings.header.ToString();
            PrinterSettings settings = new PrinterSettings();
            string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
            string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
            LPrinter PosPrinter = new LPrinter();
            string CurrentPrinter = settings.PrinterName;
            string Print_No_of_HD_Bill = clsConfigSettings.Print_No_of_HD_Bill.ToString();
            if (Print_No_of_HD_Bill == "")
                Print_No_of_HD_Bill = "1";
            //print text format bill
            try
            {
                PosPrinter.Open("testDoc", CurrentPrinter);
                ds_HouchScreenReport dsobjreport = new ds_HouchScreenReport();
                string SqlQuerry = "Usp_GetRetailInvoice_Print_HD_txtFormat @BillNo='" + BillNo + "',@Cust_Code='" + Program.Cust_code + "'";
                if (fin_year != clsConfigSettings.fin_year)
                {
                    SqlQuerry = "Usp_GetRetailInvoice_Print_HD_txtFormat_fy @BillNo='" + BillNo + "',@Cust_Code='" + Program.Cust_code + "',@Cust_Code='" + fin_year + "'";
                }
                DataSet dsFinalreport = objADOC.GetObject.FillReportDataSet(SqlQuerry, dsobjreport, "tbl_Bill", "tbl_Bill_Tran", "tbl_tax", "tbl_assorted_item", "TBL_CustomerInfo", "tbl_Discount");
                if (BillNo.Length == 1)
                {
                    BillNo = BillNo.Insert(0, "0");
                }
                string DisCountCap = dsFinalreport.Tables["tbl_bill"].Rows[0]["Dis_Amount"].ToString();
                TableNo = dsFinalreport.Tables["tbl_bill"].Rows[0]["table_no"].ToString();

                string DiscountPct = "0";
                double DisCountAmnt = Convert.ToDouble(DisCountCap);
                DataTable dttbl_Bill = new DataTable();
                DataTable dttbl_Bill_tran = new DataTable();
                DataTable dttbl_tax = new DataTable();
                DataTable dttbl_assorted_item = new DataTable();
                DataTable dtDiscount = new DataTable();
                dttbl_Bill = dsFinalreport.Tables["tbl_bill"];
                dttbl_Bill_tran = dsFinalreport.Tables["tbl_Bill_Tran"];
                dttbl_tax = dsFinalreport.Tables["tbl_tax"];
                dttbl_assorted_item = dsFinalreport.Tables["tbl_assorted_item"];
                dtDiscount = dsFinalreport.Tables["tbl_Discount"];
                string Bill_Type = Bill_TypeName;
                string BillType = dttbl_Bill.Rows[0]["bill_type"].ToString();
                double Bill_Amount = 0.00;
                DataTable dtHD = new DataTable();
                dtHD = dsFinalreport.Tables["TBL_CustomerInfo"];
                string Stwd = dttbl_Bill.Rows[0]["cashier"].ToString();
                string Remarks = dttbl_Bill.Rows[0]["Comments"].ToString();

                //added by jatinder on 21-nov-24 for burgerfarm BOLT orders
                string is_instant_order = "0";
                if (dttbl_Bill.Columns.Contains("is_instant_order"))
                {
                    is_instant_order = dttbl_Bill.Rows[0]["is_instant_order"].ToString();
                }

                double SBC_Tax = Convert.ToDouble(dttbl_Bill.Rows[0]["SBC_Tax"].ToString());
                double KKC_Tax = Convert.ToDouble(dttbl_Bill.Rows[0]["KKC_Tax"].ToString());
                string BillTime = dttbl_Bill.Rows[0]["BillTime"].ToString();
                int OrderNo = Convert.ToInt32(dsFinalreport.Tables["tbl_bill"].Rows[0]["OrderNo"].ToString());
                //string ZomatoOrderId = dttbl_Bill.Rows[0]["zomato_order_id"].ToString();
                // string OrderSource = dttbl_Bill.Rows[0]["OrderSource"].ToString();
                string ZomatoOrderId = dttbl_Bill.Rows[0]["Channel_Order_Id"].ToString();
                string OrderSource = dttbl_Bill.Rows[0]["Channel"].ToString();
                OrderComment = dttbl_Bill.Rows[0]["web_Order_comments"].ToString();
                DateTime BillDate_DateTime = DateTime.ParseExact(Program.DayEnd_BIllingDate, "yyyy-MM-dd", null);
                string BillDate = BillDate_DateTime.ToString("dd-MM-yyyy");
                string PayMode = dttbl_Bill.Rows[0]["PayMode"].ToString();
                TockenNo_locall = dttbl_Bill.Rows[0]["TockenNo"].ToString();
                string mode = "Payment Mode : " + PayMode;
                if (PayMode.Length == 0)
                {
                    mode = string.Empty;
                }

                if (clsConfigSettings.print_size == "0")
                {
                    #region Print section (Print_No_of_HD_Bill=1
                    //********************************************Print bill in notepad=========================  Copy 1****************
                    PosPrinter.Open("testDoc1", CurrentPrinter);


                    Send = ESC + "@";
                    //set alllinment of line center**********************************************************************
                    Send = Send + ESC + "a" + (char)(1);

                    //PosPrinter.Print(header + "  \n\n");
                    // PosPrinter.Print(Program.CompanyName + "  \n");
                    Send = Send + ESC + "a" + (char)(1);
                    Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                    Send = Send + Program.CompanyName + LF + "\n";
                    Send = Send + ESC + "!" + (char)(08 + 0.9);
                    Send = Send + header.ToUpper() + LF;
                    //Send = Send + Program.Outlet_name + LF;
                    Send = Send + ESC + "!" + (char)(08 + 0.5);

                    //1 - font B; 32 = Double width 
                    string[] addr = Program.Address.Split(',');
                    string address1 = string.Empty;
                    string address2 = string.Empty;
                    string address3 = string.Empty;
                    int Addresscount = addr.Length;
                    int count = 0;
                    if (Addresscount > 1)
                        count = 1;

                    for (int add = 0; add <= count; add++)
                    {
                        address1 += addr[add] + ",";
                    }

                    if (Addresscount > 3)
                        count = 3;
                    else
                        count = 2;
                    if (Addresscount > 2)
                    {
                        for (int add = 2; add <= count; add++)
                        {
                            address2 += addr[add] + ",";
                        }
                    }

                    if (Addresscount > 3)
                    {
                        for (int add = 4; add < addr.Length; add++)
                        {
                            address3 += addr[add] + ",";
                        }
                    }
                    if (address3 != string.Empty)
                    {
                        address3 = address3.Remove(address3.Length - 1);
                    }
                    if (address2 != string.Empty)
                    {
                        address2 = address2.Remove(address2.Length - 1);
                    }
                    //if (address2 == string.Empty)
                    address1 = address1.Remove(address1.Length - 1);

                    int Printcount = 0;
                    addr1 = address1.Split(',');
                    if (address1.Length > 0)
                        foreach (string addr1_1 in addr1)
                        {
                            Printcount++;
                            if (Printcount == addr1.Length)
                                Send = Send + addr1_1.Trim() + LF;
                            else
                                Send = Send + addr1_1.Trim() + "," + LF;
                        }
                    addr2 = address2.Split(',');
                    Printcount = 0;
                    if (address2.Length > 0)
                        foreach (string addr2_2 in addr2)
                        {
                            Printcount++;
                            if (Printcount == addr2.Length)
                                Send = Send + addr2_2.Trim() + LF;
                            else
                                Send = Send + addr2_2.Trim() + "," + LF;
                        }
                    Printcount = 0;
                    addr3 = address3.Split(',');
                    if (address3.Length > 0)
                        foreach (string addr3_3 in addr3)
                        {
                            Printcount++;
                            if (Printcount == addr3.Length)
                                Send = Send + addr3_3.Trim() + LF;
                            else
                                Send = Send + addr3_3.Trim() + "," + LF;
                        }
                    if (Program.PhoneNo.Trim().Length > 0)
                        Send = Send + Program.PhoneNo + LF;
                    if (Program.TinNo.Trim().Length > 0)
                        Send = Send + Program.TinNo + LF;
                    if (Program.STax_No.Trim().Length > 0)
                        Send = Send + Program.STax_No + LF;
                    if (Program.cin_no.Trim().Length > 0)
                        Send = Send + Program.cin_no + LF;
                    if (Program.fssai_no.Length > 0)
                        Send = Send + Program.fssai_no + LF;
                    Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                    Send = Send + clsConfigSettings.Bill_Head + LF;
                    Send = Send + ESC + "!" + (char)(8);
                    Send = Send + ESC + "a" + (char)(0);
                    PosPrinter.Print(Send);



                    PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "      Order No:" + OrderNo + "\n");
                    PosPrinter.Print("DATE     :" + BillDate + "\n");
                    PosPrinter.Print("TIME     :" + BillTime + "\n");
                    PosPrinter.Print("BILL TYPE:" + Bill_Type + "\n");
                    PosPrinter.Print("CASHIER  :" + Stwd + "\n");
                    if (dtHD.Rows.Count > 0)
                    {
                        PosPrinter.Print("------------------------------------------------\n");
                        PosPrinter.Print("Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                        PosPrinter.Print("Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                        PosPrinter.Print("Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                        string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                        if (Tin_no.Trim().Length > 1)
                            PosPrinter.Print("GSTIN    :" + Tin_no + "\n");
                    }
                    PosPrinter.Print("------------------------------------------\n");
                    PosPrinter.Print("DESCRIPTION           QTY   RATE   AMOUNT ");
                    PosPrinter.Print("\n------------------------------------------\n");
                    //dttbl_assorted_item
                    Bill_Amount = 0;
                    for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                    {
                        string PrintRow = string.Empty;
                        string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                        string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                        string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                        string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                        DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                        string Rate = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2");
                        string Amount = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2");
                        //if (isdeal != "0")
                        //{
                        //    Rate = ""; Amount = "";
                        //}
                        if (ItemName.Length <= 21)
                        {
                            for (int cn = ItemName.Length; cn <= 21; ++cn)
                                ItemName += " ";
                        }
                        PrintRow = "" + ItemName + " " +
                           dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + Rate + "  " + Amount + "\n";
                        PosPrinter.Print(PrintRow);
                        if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                            PosPrinter.Print("HSN: " + HSNCODE + "\n");

                        for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                        {
                            double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                            double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                            string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                            if (DealitemRate == 0)
                                RateWitAmnt = "0.00";
                            string PrintRow_Assorted = string.Empty;
                            string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                            if (i_name.Length <= 21)
                            {
                                for (int cn = i_name.Length; cn <= 21; ++cn)
                                    i_name += " ";
                            }
                            double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                            string str_rate = "";
                            if (clsConfigSettings.PrintComboRate == "1")
                            {
                                if (I_Rate > 0)
                                    str_rate = I_Rate.ToString("N2");
                            }
                            //PrintRow_Assorted = "*"+i_name + " " +
                            // double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                            PrintRow_Assorted = "" + i_name + " " +
                           dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";
                            PosPrinter.Print(PrintRow_Assorted);
                        }
                        Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                    }
                    DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();
                    PosPrinter.Print("------------------------------------------");
                    PosPrinter.Print("\n             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");

                    for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                    {
                        PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                    }
                    #region old code display tax
                    // if (DisCountAmnt > 0)
                    // {
                    //PosPrinter.Print("       DISCOUNT @" + float.Parse(DiscountPct).ToString("N2") + "% :    " + DisCountAmnt.ToString("N2") + "\n");
                    // PosPrinter.Print("         DISCOUNT AMOUNT   :    " + DisCountAmnt.ToString("N2") + "\n");
                    //}
                    //string VatPCT = clsConfigSettings.TaxCap.ToString();
                    //string ServiceTaxPCT = ConfigurationSettings.AppSettings["Service_Tax"].ToString();
                    //string ServiceTaxPCT = clsConfigSettings.ServiceTax;
                    //for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                    // {
                    //   PosPrinter.Print("         " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                    // }
                    //double ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Tax"].ToString());
                    //double RoundOff = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["RoundOff"].ToString());
                    //if (ServiceTax > 0)
                    //{
                    //  PosPrinter.Print(" SERVICE TAX   " + ServiceTaxPCT + ".00      :    " + ServiceTax.ToString("N2") + "\n");
                    //}
                    //if (SBC_Tax > 0)
                    //    PosPrinter.Print("         SBC   " + clsConfigSettings.SBC_Rate + ".00      :    " + SBC_Tax.ToString("N2") + "\n");
                    // if (KKC_Tax > 0)
                    //  PosPrinter.Print("         KKC   " + clsConfigSettings.KKC_Rate + ".00      :    " + KKC_Tax.ToString("N2") + "\n");

                    //double Surcharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["sur_charge"].ToString());
                    //if (Surcharge > 0)
                    //{
                    //   PosPrinter.Print(" SURCHARGE ON VAT@" + clsConfigSettings.SurCharge + ".00%    :    " + Surcharge.ToString("N2") + "\n");
                    //}
                    // PosPrinter.Print("              R. OFF       :    " + RoundOff.ToString("N2") + "\n");
                    #endregion
                    PosPrinter.Print("------------------------------------------\n");
                    PosPrinter.Print("         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                    PosPrinter.Print("------------------------------------------\n");

                    //PosPrinter.Print("\n");
                    if (Remarks.Length > 1)
                    {
                        PosPrinter.Print("Remarks : " + Remarks + "\n");
                        PosPrinter.Print("------------------------------------------\n");
                    }
                    if (dtDiscount != null)
                    {
                        for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                        {
                            string Category = dtDiscount.Rows[discount]["Category"].ToString();
                            string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                            PosPrinter.Print(Category + " : " + Dis_Amount + "\n");
                        }
                        if (dtDiscount.Rows.Count > 0)
                            PosPrinter.Print("------------------------------------------\n");
                    }
                    Send = "";
                    Send = ESC + "@";
                    Send = Send + ESC + "a" + (char)(1);
                    Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                    Send = Send + "Order No : " + OrderNo + LF + LF;
                    Send = Send + ESC + "!" + (char)(8);
                    Send = Send + ESC + "a" + (char)(0);
                    PosPrinter.Print("\n" + Send + "  \n");

                    if (mode.Length > 0)
                    {
                        PosPrinter.Print(mode + "\n");
                    }

                    if (OrderComment.Trim().Length > 0)
                    {
                        PosPrinter.Print("Order Comment : " + OrderComment + Environment.NewLine);
                    }

                    if (ZomatoOrderId.Trim().Length > 0)
                    {
                        PosPrinter.Print("Order's Source : " + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + Environment.NewLine);
                    }

                    //Send = "";
                    //Send = ESC + "@";
                    //Send = Send + ESC + "a" + (char)(1);
                    //Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                    //Send = Send + Program.Footer1 + LF;
                    //Send = Send + Program.Footer2 + LF;
                    //Send = Send + Program.Footer3 + LF + LF + LF + LF + LF + LF;
                    //Send = Send + ESC + "!" + (char)(8);
                    //Send = Send + ESC + "a" + (char)(0);
                    //PosPrinter.Print(Send);
                    Send = "";
                    Send = ESC + "@";
                    Send = Send + ESC + "a" + (char)(1);
                    Send = Send + ESC + "!" + (char)(08 + 0.9);
                    Send = Send + Program.Footer1 + LF;
                    Send = Send + Program.Footer2 + LF;
                    Send = Send + Program.Footer3 + LF;
                    Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                    Send = Send + ESC + "!" + (char)(8);
                    Send = Send + ESC + "a" + (char)(0);
                    PosPrinter.Print("\n" + Send);
                    PosPrinter.Print(PaperFullCut);
                    Send = "";
                    TockenNo_locall = dttbl_Bill.Rows[0]["TockenNo"].ToString();
                    #endregion
                    //********************************************Print bill in notepad=========================  Copy 2****************
                    if (Convert.ToDouble(Print_No_of_HD_Bill) > 1)
                    {
                        #region Print section (Print_No_of_HD_Bill=2
                        PosPrinter.Open("testDoc1", CurrentPrinter);


                        Send = ESC + "@";
                        //set alllinment of line center**********************************************************************


                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                        Send = Send + Program.CompanyName + LF + "\n";
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + header.ToUpper() + LF;
                        //Send = Send + Program.Outlet_name + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.5);


                        Printcount = 0;
                        if (address1.Length > 0)
                            foreach (string addr1_1 in addr1)
                            {
                                Printcount++;
                                if (Printcount == addr1.Length)
                                    Send = Send + addr1_1.Trim() + LF;
                                else
                                    Send = Send + addr1_1.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        if (address2.Length > 0)
                            foreach (string addr2_2 in addr2)
                            {
                                Printcount++;
                                if (Printcount == addr2.Length)
                                    Send = Send + addr2_2.Trim() + LF;
                                else
                                    Send = Send + addr2_2.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        if (address3.Length > 0)
                            foreach (string addr3_3 in addr3)
                            {
                                Printcount++;
                                if (Printcount == addr3.Length)
                                    Send = Send + addr3_3.Trim() + LF;
                                else
                                    Send = Send + addr3_3.Trim() + "," + LF;
                            }

                        if (Program.PhoneNo.Trim().Length > 0)
                            Send = Send + Program.PhoneNo + LF;
                        if (Program.TinNo.Trim().Length > 0)
                            Send = Send + Program.TinNo + LF;
                        if (Program.STax_No.Trim().Length > 0)
                            if (Program.STax_No.Length > 0)
                                Send = Send + Program.STax_No + LF;
                        if (Program.cin_no.Length > 0)
                            Send = Send + Program.cin_no + LF;
                        if (Program.fssai_no.Length > 0)
                            Send = Send + Program.fssai_no + LF;
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + clsConfigSettings.Bill_Head + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print(Send);


                        PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "      Order No:" + OrderNo + "\n");
                        PosPrinter.Print("DATE     :" + BillDate + "\n");
                        PosPrinter.Print("TIME     :" + BillTime + "\n");
                        PosPrinter.Print("BILL TYPE:" + Bill_TypeName + "\n");
                        //PosPrinter.Print("CASHIER  :" + Program.CashierName + "\n");
                        PosPrinter.Print("CASHIER  :" + Stwd + "\n");
                        if (dtHD.Rows.Count > 0)
                        {
                            PosPrinter.Print("------------------------------------------------\n");
                            dtHD = dsFinalreport.Tables["TBL_CustomerInfo"];
                            PosPrinter.Print("Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                            PosPrinter.Print("Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                            PosPrinter.Print("Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                            string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                            if (Tin_no.Trim().Length > 1)
                                PosPrinter.Print("GSTIN    :" + Tin_no + "\n");
                        }
                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("DESCRIPTION           QTY   RATE   AMOUNT ");
                        PosPrinter.Print("\n------------------------------------------\n");
                        //dttbl_assorted_item
                        Bill_Amount = 0;
                        for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                        {
                            string PrintRow = string.Empty;
                            string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                            string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                            string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                            string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                            DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                            string Rate = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2");
                            string Amount = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2");
                            //if (isdeal != "0")
                            //{
                            //    Rate = ""; Amount = "";
                            //}
                            if (ItemName.Length <= 21)
                            {
                                for (int cn = ItemName.Length; cn <= 21; ++cn)
                                    ItemName += " ";
                            }
                            PrintRow = "" + ItemName + " " +
                               dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + Rate + "  " + Amount + "\n";
                            PosPrinter.Print(PrintRow);
                            if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                                PosPrinter.Print("HSN: " + HSNCODE + "\n");
                            for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                            {
                                double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                                string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                                if (DealitemRate == 0)
                                    RateWitAmnt = "";
                                string PrintRow_Assorted = string.Empty;
                                string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                                if (i_name.Length <= 21)
                                {
                                    for (int cn = i_name.Length; cn <= 21; ++cn)
                                        i_name += " ";
                                }
                                double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                string str_rate = "";
                                if (clsConfigSettings.PrintComboRate == "1")
                                {
                                    if (I_Rate > 0)
                                        str_rate = I_Rate.ToString("N2");
                                }
                                //PrintRow_Assorted = i_name + " " +
                                //double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                                PrintRow_Assorted = i_name + " " +
                                dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";
                                PosPrinter.Print(PrintRow_Assorted);
                            }
                            Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                        }
                        DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();
                        PosPrinter.Print("------------------------------------------");
                        PosPrinter.Print("\n             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");

                        for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        {
                            PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        }
                        #region old code to dsisplay tax
                        // if (DisCountAmnt > 0)
                        //{
                        //PosPrinter.Print("       DISCOUNT @" + float.Parse(DiscountPct).ToString("N2") + "% :    " + DisCountAmnt.ToString("N2") + "\n");
                        //   PosPrinter.Print("         DISCOUNT AMOUNT   :    " + DisCountAmnt.ToString("N2") + "\n");
                        //}
                        // VatPCT = clsConfigSettings.TaxCap.ToString();
                        //string ServiceTaxPCT = ConfigurationSettings.AppSettings["Service_Tax"].ToString();
                        // ServiceTaxPCT = clsConfigSettings.ServiceTax;
                        //for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        //{
                        // PosPrinter.Print("         " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        //}
                        // ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Tax"].ToString());
                        // RoundOff = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["RoundOff"].ToString());
                        // if (ServiceTax > 0)
                        // {
                        //   PosPrinter.Print(" SERVICE TAX   " + ServiceTaxPCT + ".00      :    " + ServiceTax.ToString("N2") + "\n");
                        //}
                        //if (SBC_Tax > 0)
                        //   PosPrinter.Print("         SBC   " + clsConfigSettings.SBC_Rate + ".00      :    " + SBC_Tax.ToString("N2") + "\n");
                        //if (KKC_Tax > 0)
                        //    PosPrinter.Print("         KKC   " + clsConfigSettings.KKC_Rate + ".00      :    " + KKC_Tax.ToString("N2") + "\n");

                        // Surcharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["sur_charge"].ToString());
                        // if (Surcharge > 0)
                        // {
                        //    PosPrinter.Print(" SURCHARGE ON VAT@" + clsConfigSettings.SurCharge + ".00%    :    " + Surcharge.ToString("N2") + "\n");
                        //}
                        //PosPrinter.Print("              R. OFF       :    " + RoundOff.ToString("N2") + "\n");
                        #endregion

                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                        PosPrinter.Print("------------------------------------------\n");
                        //PosPrinter.Print("\n");
                        if (Remarks.Length > 1)
                        {
                            PosPrinter.Print("Remarks : " + Remarks + "\n");
                            PosPrinter.Print("------------------------------------------\n");
                        }

                        if (dtDiscount != null)
                        {
                            for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                            {
                                string Category = dtDiscount.Rows[discount]["Category"].ToString();
                                string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                                PosPrinter.Print(Category + " : " + Dis_Amount + "\n");

                            }
                            if (dtDiscount.Rows.Count > 0)
                                PosPrinter.Print("------------------------------------------\n");
                        }
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        Send = Send + "Order No : " + OrderNo + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send + "  \n");
                        if (mode.Length > 0)
                        {
                            PosPrinter.Print(mode + "\n");
                        }
                        if (OrderComment.Trim().Length > 0)
                        {
                            PosPrinter.Print("Order Comment : " + OrderComment + Environment.NewLine);
                        }

                        if (ZomatoOrderId.Trim().Length > 0)
                        {
                            PosPrinter.Print("Order's Source : " + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + Environment.NewLine);
                        }


                        //Send = "";
                        //Send = ESC + "@";
                        //Send = Send + ESC + "a" + (char)(1);
                        //Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        //Send = Send + Program.Footer1 + LF;
                        //Send = Send + Program.Footer2 + LF;
                        //Send = Send + Program.Footer3 + LF + LF + LF + LF + LF + LF;
                        //Send = Send + ESC + "!" + (char)(8);
                        //Send = Send + ESC + "a" + (char)(0);
                        //PosPrinter.Print(Send);
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + Program.Footer1 + LF;
                        Send = Send + Program.Footer2 + LF;
                        Send = Send + Program.Footer3 + LF;
                        Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send);
                        Send = "";
                        PosPrinter.Print(PaperFullCut);
                        #endregion
                    }
                    //********************************************Print bill in notepad=========================  Copy 3****************
                    if (Convert.ToDouble(Print_No_of_HD_Bill) > 2)
                    {
                        #region Print section (Print_No_of_HD_Bill=1
                        PosPrinter.Open("testDoc1", CurrentPrinter);

                        Send = ESC + "@";
                        //set alllinment of line center**********************************************************************
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                        Send = Send + Program.CompanyName + LF + "\n";
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + header.ToUpper() + LF;
                        //Send = Send + Program.Outlet_name + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.5);

                        Printcount = 0;
                        if (address1.Length > 0)
                            foreach (string addr1_1 in addr1)
                            {
                                Printcount++;
                                if (Printcount == addr1.Length)
                                    Send = Send + addr1_1.Trim() + LF;
                                else
                                    Send = Send + addr1_1.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        if (address2.Length > 0)
                            foreach (string addr2_2 in addr2)
                            {
                                Printcount++;
                                if (Printcount == addr2.Length)
                                    Send = Send + addr2_2.Trim() + LF;
                                else
                                    Send = Send + addr2_2.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        if (address3.Length > 0)
                            foreach (string addr3_3 in addr3)
                            {
                                Printcount++;
                                if (Printcount == addr3.Length)
                                    Send = Send + addr3_3.Trim() + LF;
                                else
                                    Send = Send + addr3_3.Trim() + "," + LF;
                            }

                        if (Program.PhoneNo.Trim().Length > 0)
                            Send = Send + Program.PhoneNo + LF;
                        if (Program.TinNo.Trim().Length > 0)
                            Send = Send + Program.TinNo + LF;
                        if (Program.STax_No.Trim().Length > 0)
                            if (Program.STax_No.Length > 0)
                                Send = Send + Program.STax_No + LF;
                        if (Program.cin_no.Length > 0)
                            Send = Send + Program.cin_no + LF;
                        if (Program.fssai_no.Length > 0)
                            Send = Send + Program.fssai_no + LF;
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + clsConfigSettings.Bill_Head + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print(Send);

                        PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "      Order No:" + OrderNo + "\n");
                        PosPrinter.Print("DATE     :" + BillDate + "\n");
                        PosPrinter.Print("TIME     :" + BillTime + "\n");
                        PosPrinter.Print("BILL TYPE:" + Bill_TypeName + "\n");
                        //PosPrinter.Print("CASHIER  :" + Program.CashierName + "\n");
                        PosPrinter.Print("CASHIER  :" + Stwd + "\n");
                        if (dtHD.Rows.Count > 0)
                        {
                            PosPrinter.Print("------------------------------------------------\n");
                            dtHD = dsFinalreport.Tables["TBL_CustomerInfo"];
                            PosPrinter.Print("Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                            PosPrinter.Print("Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                            PosPrinter.Print("Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                            string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                            if (Tin_no.Trim().Length > 1)
                                PosPrinter.Print("GSTIN    :" + Tin_no + "\n");
                        }
                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("DESCRIPTION           QTY   RATE   AMOUNT ");
                        PosPrinter.Print("\n------------------------------------------\n");
                        //dttbl_assorted_item
                        Bill_Amount = 0;
                        for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                        {
                            string PrintRow = string.Empty;
                            string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                            string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                            string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                            string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                            DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                            string Rate = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2");
                            string Amount = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2");
                            //if (isdeal != "0")
                            //{
                            //    Rate = ""; Amount = "";
                            //}
                            if (ItemName.Length <= 21)
                            {
                                for (int cn = ItemName.Length; cn <= 21; ++cn)
                                    ItemName += " ";
                            }
                            PrintRow = "" + ItemName + " " +
                               dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + Rate + "  " + Amount + "\n";
                            PosPrinter.Print(PrintRow);
                            if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                                PosPrinter.Print("HSN: " + HSNCODE + "\n");
                            for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                            {
                                double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                                string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                                if (DealitemRate == 0)
                                    RateWitAmnt = "";
                                string PrintRow_Assorted = string.Empty;
                                string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                                if (i_name.Length <= 21)
                                {
                                    for (int cn = i_name.Length; cn <= 21; ++cn)
                                        i_name += " ";
                                }
                                double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                string str_rate = "";
                                if (clsConfigSettings.PrintComboRate == "1")
                                {
                                    if (I_Rate > 0)
                                        str_rate = I_Rate.ToString("N2");
                                }
                                //PrintRow_Assorted = i_name + " " +
                                //double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                                PrintRow_Assorted = i_name + " " +
                                 dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";

                                PosPrinter.Print(PrintRow_Assorted);
                            }
                            Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                        }
                        DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();

                        PosPrinter.Print("------------------------------------------");
                        PosPrinter.Print("\n             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");
                        #region old code to display tax
                        //if (DisCountAmnt > 0)
                        //{
                        //PosPrinter.Print("       DISCOUNT @" + float.Parse(DiscountPct).ToString("N2") + "% :    " + DisCountAmnt.ToString("N2") + "\n");
                        // PosPrinter.Print("         DISCOUNT AMOUNT   :    " + DisCountAmnt.ToString("N2") + "\n");
                        //}
                        // VatPCT = clsConfigSettings.TaxCap.ToString();
                        //string ServiceTaxPCT = ConfigurationSettings.AppSettings["Service_Tax"].ToString();
                        // ServiceTaxPCT = clsConfigSettings.ServiceTax;
                        // for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        // {
                        //    PosPrinter.Print("                " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        //}
                        // ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Tax"].ToString());
                        // RoundOff = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["RoundOff"].ToString());
                        // if (ServiceTax > 0)
                        // {
                        //     PosPrinter.Print(" SERVICE TAX   " + ServiceTaxPCT + ".00      :    " + ServiceTax.ToString("N2") + "\n");
                        // }
                        //if (SBC_Tax > 0)
                        //   PosPrinter.Print("         SBC   " + clsConfigSettings.SBC_Rate + ".00      :    " + SBC_Tax.ToString("N2") + "\n");
                        //if (KKC_Tax > 0)
                        //   PosPrinter.Print("         KKC   " + clsConfigSettings.KKC_Rate + ".00      :    " + KKC_Tax.ToString("N2") + "\n");

                        // Surcharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["sur_charge"].ToString());
                        // if (Surcharge > 0)
                        // {
                        // PosPrinter.Print(" SURCHARGE ON VAT@" + clsConfigSettings.SurCharge + ".00%    :    " + Surcharge.ToString("N2") + "\n");
                        // }
                        // PosPrinter.Print("              R. OFF       :    " + RoundOff.ToString("N2") + "\n");
                        #endregion

                        for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        {
                            PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        }
                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                        PosPrinter.Print("------------------------------------------\n");

                        if (Remarks.Length > 1)
                        {
                            PosPrinter.Print("Remarks : " + Remarks + "\n");
                            PosPrinter.Print("------------------------------------------\n");
                        }

                        if (dtDiscount != null)
                        {
                            for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                            {
                                string Category = dtDiscount.Rows[discount]["Category"].ToString();
                                string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                                PosPrinter.Print(Category + " : " + Dis_Amount + "\n");

                            }
                            if (dtDiscount.Rows.Count > 0)
                                PosPrinter.Print("------------------------------------------\n");
                        }
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        Send = Send + "Order No : " + OrderNo + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send + "  \n");
                        if (mode.Length > 0)
                        {
                            PosPrinter.Print(mode + "\n");
                        }
                        if (OrderComment.Trim().Length > 0)
                        {
                            PosPrinter.Print("Order Comment : " + OrderComment + Environment.NewLine);
                        }

                        if (ZomatoOrderId.Trim().Length > 0)
                        {
                            PosPrinter.Print("Order's Source : " + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + Environment.NewLine);
                        }
                        //Send = "";
                        //Send = ESC + "@";
                        //Send = Send + ESC + "a" + (char)(1);
                        //Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        //Send = Send + Program.Footer1 + LF;
                        //Send = Send + Program.Footer2 + LF;
                        //Send = Send + Program.Footer3 + LF + LF + LF + LF + LF + LF;
                        //Send = Send + ESC + "!" + (char)(8);
                        //Send = Send + ESC + "a" + (char)(0);
                        //PosPrinter.Print(Send);
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + Program.Footer1 + LF;
                        Send = Send + Program.Footer2 + LF;
                        Send = Send + Program.Footer3 + LF;
                        Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send);
                        PosPrinter.Print(PaperFullCut);
                        #endregion
                        //=============================================================================
                    }
                    PosPrinter.Close();
                }
                else if (clsConfigSettings.print_size == "2")
                {
                    int SpaceCount = Convert.ToInt32(ConfigurationSettings.AppSettings["SPACE"].ToString());
                    string SPACE1 = string.Empty;
                    for (int i = 0; i < SpaceCount; i++)
                        SPACE1 += " ";


                    for (int i = 0; i < Convert.ToDouble(Print_No_of_HD_Bill); i++)
                    {
                        #region Print section Print_No_of_HD_Bill=1
                        PosPrinter.Open("testDoc1", CurrentPrinter);
                        Send = ESC + "@";
                        //set alllinment of line center**********************************************************************
                        Send = Send + ESC + "a" + (char)(1);

                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                        Send = Send + Program.CompanyName + LF + "\n";
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + header.ToUpper() + LF;
                        //Send = Send + Program.Outlet_name + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.5);

                        //1 - font B; 32 = Double width 
                        string[] addr = Program.Address.Split(',');
                        string address1 = string.Empty;
                        string address2 = string.Empty;
                        string address3 = string.Empty;
                        int Addresscount = addr.Length;
                        int count = 0;
                        if (Addresscount > 1)
                            count = 1;

                        for (int add = 0; add <= count; add++)
                        {
                            address1 += addr[add] + ",";
                        }

                        if (Addresscount > 3)
                            count = 3;
                        else
                            count = 2;
                        if (Addresscount > 2)
                        {
                            for (int add = 2; add <= count; add++)
                            {
                                address2 += addr[add] + ",";
                            }
                        }

                        if (Addresscount > 3)
                        {
                            for (int add = 4; add < addr.Length; add++)
                            {
                                address3 += addr[add] + ",";
                            }
                        }
                        if (address3 != string.Empty)
                        {
                            address3 = address3.Remove(address3.Length - 1);
                        }
                        if (address2 != string.Empty)
                        {
                            address2 = address2.Remove(address2.Length - 1);
                        }
                        //if (address2 == string.Empty)
                        address1 = address1.Remove(address1.Length - 1);

                        int Printcount = 0;
                        addr1 = address1.Split(',');
                        if (address1.Length > 0)
                            foreach (string addr1_1 in addr1)
                            {
                                Printcount++;
                                if (Printcount == addr1.Length)
                                    Send = Send + addr1_1.Trim() + LF;
                                else
                                    Send = Send + addr1_1.Trim() + "," + LF;
                            }
                        addr2 = address2.Split(',');
                        Printcount = 0;
                        if (address2.Length > 0)
                            foreach (string addr2_2 in addr2)
                            {
                                Printcount++;
                                if (Printcount == addr2.Length)
                                    Send = Send + addr2_2.Trim() + LF;
                                else
                                    Send = Send + addr2_2.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        addr3 = address3.Split(',');
                        if (address3.Length > 0)
                            foreach (string addr3_3 in addr3)
                            {
                                Printcount++;
                                if (Printcount == addr3.Length)
                                    Send = Send + addr3_3.Trim() + LF;
                                else
                                    Send = Send + addr3_3.Trim() + "," + LF;
                            }
                        if (Program.PhoneNo.Trim().Length > 0)
                            Send = Send + Program.PhoneNo + LF;
                        if (Program.TinNo.Trim().Length > 0)
                            Send = Send + Program.TinNo + LF;
                        if (Program.STax_No.Trim().Length > 0)
                            Send = Send + Program.STax_No + LF;
                        if (Program.cin_no.Trim().Length > 0)
                            Send = Send + Program.cin_no + LF;
                        if (Program.fssai_no.Length > 0)
                            Send = Send + Program.fssai_no + LF;
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + clsConfigSettings.Bill_Head + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print(Send);



                        PosPrinter.Print("\n" + SPACE1 + "BILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "      Order No:" + OrderNo + "\n");
                        PosPrinter.Print(SPACE1 + "DATE     :" + BillDate + "\n");
                        PosPrinter.Print(SPACE1 + "TIME     :" + BillTime + "\n");
                        PosPrinter.Print(SPACE1 + "BILL TYPE:" + Bill_Type + "\n");
                        PosPrinter.Print(SPACE1 + "CASHIER  :" + Stwd + "\n");
                        if (dtHD.Rows.Count > 0)
                        {
                            PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                            PosPrinter.Print(SPACE1 + "Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                            PosPrinter.Print(SPACE1 + "Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                            PosPrinter.Print(SPACE1 + "Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                            string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                            if (Tin_no.Trim().Length > 1)
                                PosPrinter.Print(SPACE1 + "GSTIN    :" + Tin_no + "\n");
                        }
                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        PosPrinter.Print(SPACE1 + "DESCRIPTION           QTY   RATE   AMOUNT ");
                        PosPrinter.Print("\n" + SPACE1 + "------------------------------------------\n");
                        //dttbl_assorted_item
                        Bill_Amount = 0;
                        for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                        {
                            string PrintRow = string.Empty;
                            string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                            string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                            string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                            string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                            DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                            string Rate = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2");
                            string Amount = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2");
                            //if (isdeal != "0")
                            //{
                            //    Rate = ""; Amount = "";
                            //}
                            if (ItemName.Length <= 21)
                            {
                                for (int cn = ItemName.Length; cn <= 21; ++cn)
                                    ItemName += " ";
                            }
                            PrintRow = SPACE1 + "" + ItemName + " " +
                               dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + Rate + "  " + Amount + "\n";
                            PosPrinter.Print(PrintRow);
                            if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                                PosPrinter.Print(SPACE1 + "HSN: " + HSNCODE + "\n");

                            for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                            {
                                double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                                string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                                if (DealitemRate == 0)
                                    RateWitAmnt = "0.00";
                                string PrintRow_Assorted = string.Empty;
                                string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                                if (i_name.Length <= 21)
                                {
                                    for (int cn = i_name.Length; cn <= 21; ++cn)
                                        i_name += " ";
                                }
                                double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                string str_rate = "";
                                if (clsConfigSettings.PrintComboRate == "1")
                                {
                                    if (I_Rate > 0)
                                        str_rate = I_Rate.ToString("N2");
                                }
                                //PrintRow_Assorted = "*"+i_name + " " +
                                // double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                                PrintRow_Assorted = SPACE1 + "" + i_name + " " +
                               dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";
                                PosPrinter.Print(PrintRow_Assorted);
                            }
                            Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                        }
                        DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();
                        PosPrinter.Print(SPACE1 + "------------------------------------------");
                        PosPrinter.Print(SPACE1 + "\n             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");

                        for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        {
                            PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        }

                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        PosPrinter.Print(SPACE1 + "         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");

                        //PosPrinter.Print("\n");
                        if (Remarks.Length > 1)
                        {
                            PosPrinter.Print(SPACE1 + "Remarks : " + Remarks + "\n");
                            PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        }
                        if (dtDiscount != null)
                        {
                            for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                            {
                                string Category = dtDiscount.Rows[discount]["Category"].ToString();
                                string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                                PosPrinter.Print(SPACE1 + Category + " : " + Dis_Amount + "\n");
                            }
                            if (dtDiscount.Rows.Count > 0)
                                PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        }
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        Send = Send + "Order No : " + OrderNo + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send + "  \n");

                        if (mode.Length > 0)
                        {
                            PosPrinter.Print(SPACE1 + mode + "\n");
                        }

                        if (OrderComment.Trim().Length > 0)
                        {
                            PosPrinter.Print(SPACE1 + "Order Comment : " + OrderComment + Environment.NewLine);
                        }

                        if (ZomatoOrderId.Trim().Length > 0)
                        {
                            PosPrinter.Print(SPACE1 + "Order's Source : " + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + Environment.NewLine);
                        }


                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + Program.Footer1 + LF;
                        Send = Send + Program.Footer2 + LF;
                        Send = Send + Program.Footer3 + LF;
                        Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send);
                        PosPrinter.Print(PaperFullCut);
                        Send = "";
                        TockenNo_locall = dttbl_Bill.Rows[0]["TockenNo"].ToString();
                        #endregion
                        PosPrinter.Close();
                    }
                }
                else if (clsConfigSettings.print_size == "3")
                {
                    int SpaceCount = Convert.ToInt32(ConfigurationSettings.AppSettings["SPACE"].ToString());
                    string SPACE1 = string.Empty;
                    for (int i = 0; i < SpaceCount; i++)
                        SPACE1 += " ";


                    for (int i = 0; i < Convert.ToDouble(Print_No_of_HD_Bill); i++)
                    {
                        #region Print section Print_No_of_HD_Bill=1
                        PosPrinter.Open("testDoc1", CurrentPrinter);
                        Send = ESC + "@";
                        //set alllinment of line center**********************************************************************
                        Send = Send + ESC + "a" + (char)(1);

                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                        Send = Send + Program.CompanyName + LF + "\n";
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + header.ToUpper() + LF;
                        //Send = Send + Program.Outlet_name + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.5);

                        //1 - font B; 32 = Double width 
                        string[] addr = Program.Address.Split(',');
                        string address1 = string.Empty;
                        string address2 = string.Empty;
                        string address3 = string.Empty;
                        int Addresscount = addr.Length;
                        int count = 0;
                        if (Addresscount > 1)
                            count = 1;

                        for (int add = 0; add <= count; add++)
                        {
                            address1 += addr[add] + ",";
                        }

                        if (Addresscount > 3)
                            count = 3;
                        else
                            count = 2;
                        if (Addresscount > 2)
                        {
                            for (int add = 2; add <= count; add++)
                            {
                                address2 += addr[add] + ",";
                            }
                        }

                        if (Addresscount > 3)
                        {
                            for (int add = 4; add < addr.Length; add++)
                            {
                                address3 += addr[add] + ",";
                            }
                        }
                        if (address3 != string.Empty)
                        {
                            address3 = address3.Remove(address3.Length - 1);
                        }
                        if (address2 != string.Empty)
                        {
                            address2 = address2.Remove(address2.Length - 1);
                        }
                        //if (address2 == string.Empty)
                        address1 = address1.Remove(address1.Length - 1);

                        int Printcount = 0;
                        addr1 = address1.Split(',');
                        if (address1.Length > 0)
                            foreach (string addr1_1 in addr1)
                            {
                                Printcount++;
                                if (Printcount == addr1.Length)
                                    Send = Send + addr1_1.Trim() + LF;
                                else
                                    Send = Send + addr1_1.Trim() + "," + LF;
                            }
                        addr2 = address2.Split(',');
                        Printcount = 0;
                        if (address2.Length > 0)
                            foreach (string addr2_2 in addr2)
                            {
                                Printcount++;
                                if (Printcount == addr2.Length)
                                    Send = Send + addr2_2.Trim() + LF;
                                else
                                    Send = Send + addr2_2.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        addr3 = address3.Split(',');
                        if (address3.Length > 0)
                            foreach (string addr3_3 in addr3)
                            {
                                Printcount++;
                                if (Printcount == addr3.Length)
                                    Send = Send + addr3_3.Trim() + LF;
                                else
                                    Send = Send + addr3_3.Trim() + "," + LF;
                            }
                        if (Program.PhoneNo.Trim().Length > 0)
                            Send = Send + Program.PhoneNo + LF;
                        if (Program.TinNo.Trim().Length > 0)
                            Send = Send + Program.TinNo + LF;
                        if (Program.STax_No.Trim().Length > 0)
                            Send = Send + Program.STax_No + LF;
                        if (Program.cin_no.Trim().Length > 0)
                            Send = Send + Program.cin_no + LF;
                        if (Program.fssai_no.Length > 0)
                            Send = Send + Program.fssai_no + LF;
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + clsConfigSettings.Bill_Head + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);


                        Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        Send = Send + "\n" + SPACE1 + "BILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "\n\n";
                        Send = Send + SPACE1 + "BILL TYPE:" + Bill_Type + "\n\n";
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);

                        Send = Send + ESC + "!" + (char)(28 + 8 + 1);
                        Send = Send + SPACE1 + "DATE     :" + BillDate + "\n";
                        Send = Send + SPACE1 + "TIME     :" + BillTime + "\n";
                        Send = Send + SPACE1 + "CASHIER:" + Stwd + "\n";
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print(Send);

                        if (dtHD.Rows.Count > 0)
                        {
                            PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                            PosPrinter.Print(SPACE1 + "Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                            PosPrinter.Print(SPACE1 + "Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                            PosPrinter.Print(SPACE1 + "Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                            string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                            if (Tin_no.Trim().Length > 1)
                                PosPrinter.Print(SPACE1 + "GSTIN    :" + Tin_no + "\n");
                        }
                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        PosPrinter.Print(SPACE1 + "DESCRIPTION           QTY   RATE   AMOUNT ");
                        PosPrinter.Print("\n" + SPACE1 + "------------------------------------------\n");
                        //dttbl_assorted_item
                        Bill_Amount = 0;
                        for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                        {
                            string PrintRow = string.Empty;
                            string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                            string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                            string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                            string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                            DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                            string Rate = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2");
                            string Amount = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2");

                            string I_Name_Desc = "";
                            if (dttbl_Bill_tran.Columns.Contains("I_Name_Desc"))
                            {
                                I_Name_Desc = "(" + dttbl_Bill_tran.Rows[ItemIdex]["I_Name_Desc"].ToString() + ")";
                            }


                            //if (isdeal != "0")
                            //{
                            //    Rate = ""; Amount = "";
                            //}
                            if (ItemName.Length <= 21)
                            {
                                for (int cn = ItemName.Length; cn <= 21; ++cn)
                                    ItemName += " ";
                            }
                            PrintRow = SPACE1 + "" + ItemName + " " +
                               dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + Rate + "  " + Amount + "\n";


                            if (I_Name_Desc.Length > 2)
                            {
                                if (I_Name_Desc.Length > 42)
                                {
                                    var inputText = I_Name_Desc;
                                    var length = 42;

                                    var splittedStrings = Split(inputText, length);

                                    foreach (var splittedString in splittedStrings)
                                    {
                                        PrintRow += SPACE1 + "" + splittedString + " " + "\n";

                                    }
                                    PrintRow += " " + "\n";
                                }
                                else
                                {
                                    PrintRow += SPACE1 + "" + I_Name_Desc + " " + "\n\n";
                                }
                            }
                            else
                            {
                                PrintRow += SPACE1 + " " + "\n";
                            }

                            PosPrinter.Print(PrintRow);
                            if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                                PosPrinter.Print(SPACE1 + "HSN: " + HSNCODE + "\n");

                            for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                            {
                                double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                                string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                                if (DealitemRate == 0)
                                    RateWitAmnt = "0.00";
                                string PrintRow_Assorted = string.Empty;
                                string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                                if (i_name.Length <= 21)
                                {
                                    for (int cn = i_name.Length; cn <= 21; ++cn)
                                        i_name += " ";
                                }
                                double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                string str_rate = "";
                                if (clsConfigSettings.PrintComboRate == "1")
                                {
                                    if (I_Rate > 0)
                                        str_rate = I_Rate.ToString("N2");
                                }
                                //PrintRow_Assorted = "*"+i_name + " " +
                                // double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                                PrintRow_Assorted = SPACE1 + "" + i_name + " " +
                               dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";
                                PosPrinter.Print(PrintRow_Assorted);
                            }
                            Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                        }
                        DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();
                        PosPrinter.Print(SPACE1 + "------------------------------------------");
                        PosPrinter.Print(SPACE1 + "\n             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");

                        for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        {
                            PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        }

                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        PosPrinter.Print(SPACE1 + "         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");

                        //PosPrinter.Print("\n");
                        if (Remarks.Length > 1)
                        {
                            PosPrinter.Print(SPACE1 + "Remarks : " + Remarks + "\n");
                            PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        }
                        if (dtDiscount != null)
                        {
                            for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                            {
                                string Category = dtDiscount.Rows[discount]["Category"].ToString();
                                string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                                PosPrinter.Print(SPACE1 + Category + " : " + Dis_Amount + "\n");
                            }
                            if (dtDiscount.Rows.Count > 0)
                                PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        }
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        Send = Send + "Order No : " + OrderNo + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        // PosPrinter.Print("\n" + Send + "  \n");

                        if (mode.Length > 0)
                        {
                            PosPrinter.Print(SPACE1 + mode + "\n");
                        }

                        if (OrderComment.Trim().Length > 0)
                        {
                            PosPrinter.Print(SPACE1 + "Order Comment : " + OrderComment + Environment.NewLine);
                        }

                        if (ZomatoOrderId.Trim().Length > 0)
                        {
                            Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                            Send = Send + SPACE1 + "Source : " + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + Environment.NewLine;
                            Send = Send + ESC + "!" + (char)(8);
                            Send = Send + ESC + "a" + (char)(0);
                            PosPrinter.Print(Send);

                            //  PosPrinter.Print(SPACE1 + "Order's Source : " + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + Environment.NewLine);
                        }


                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + Program.Footer1 + LF;
                        Send = Send + Program.Footer2 + LF;
                        Send = Send + Program.Footer3 + LF;
                        Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send);
                        PosPrinter.Print(PaperFullCut);
                        Send = "";
                        TockenNo_locall = dttbl_Bill.Rows[0]["TockenNo"].ToString();
                        #endregion
                        PosPrinter.Close();
                    }
                }

                else
                {
                    #region Print section (Print_No_of_HD_Bill=1
                    PosPrinter.Open("testDoc1", CurrentPrinter);

                    //================Print Order No=============                        
                    Send = ESC + "@";
                    Send = Send + ESC + "a" + (char)(1);
                    Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                    Send = Send + "Your order number is" + LF;
                    Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 8);

                    string PrintOrderNO = "";
                    PrintOrderNO = OrderNo.ToString().ToUpper();

                    if (is_instant_order == "1")
                    {
                        PrintOrderNO = PrintOrderNO + "";
                    }

                    Send = Send + PrintOrderNO + LF;
                    Send = Send + ESC + "!" + (char)(8);
                    Send = Send + ESC + "a" + (char)(0);
                    Send = Send + "------------------------------------------" + LF;
                    PosPrinter.Print("\n" + Send + "  \n");
                    Send = "";
                    //============================================


                    Send = ESC + "@";
                    //set alllinment of line center**********************************************************************
                    Send = Send + ESC + "a" + (char)(1);

                    //PosPrinter.Print(header + "  \n\n");
                    // PosPrinter.Print(Program.CompanyName + "  \n");
                    Send = Send + ESC + "a" + (char)(1);
                    Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                    Send = Send + Program.CompanyName + LF + "\n";
                    Send = Send + ESC + "!" + (char)(08 + 0.9);
                    Send = Send + header.ToUpper() + LF;
                    //Send = Send + Program.Outlet_name + LF;
                    Send = Send + ESC + "!" + (char)(08 + 0.5);

                    //1 - font B; 32 = Double width 
                    string[] addr = Program.Address.Split(',');
                    string address1 = string.Empty;
                    string address2 = string.Empty;
                    string address3 = string.Empty;
                    int Addresscount = addr.Length;
                    int count = 0;
                    if (Addresscount > 1)
                        count = 1;

                    for (int add = 0; add <= count; add++)
                    {
                        address1 += addr[add] + ",";
                    }

                    if (Addresscount > 3)
                        count = 3;
                    else
                        count = 2;
                    if (Addresscount > 2)
                    {
                        for (int add = 2; add <= count; add++)
                        {
                            address2 += addr[add] + ",";
                        }
                    }

                    if (Addresscount > 3)
                    {
                        for (int add = 4; add < addr.Length; add++)
                        {
                            address3 += addr[add] + ",";
                        }
                    }
                    if (address3 != string.Empty)
                    {
                        address3 = address3.Remove(address3.Length - 1);
                    }
                    if (address2 != string.Empty)
                    {
                        address2 = address2.Remove(address2.Length - 1);
                    }
                    //if (address2 == string.Empty)
                    address1 = address1.Remove(address1.Length - 1);

                    int Printcount = 0;
                    addr1 = address1.Split(',');
                    if (address1.Length > 0)
                        foreach (string addr1_1 in addr1)
                        {
                            Printcount++;
                            if (Printcount == addr1.Length)
                                Send = Send + addr1_1.Trim() + LF;
                            else
                                Send = Send + addr1_1.Trim() + "," + LF;
                        }
                    addr2 = address2.Split(',');
                    Printcount = 0;
                    if (address2.Length > 0)
                        foreach (string addr2_2 in addr2)
                        {
                            Printcount++;
                            if (Printcount == addr2.Length)
                                Send = Send + addr2_2.Trim() + LF;
                            else
                                Send = Send + addr2_2.Trim() + "," + LF;
                        }
                    Printcount = 0;
                    addr3 = address3.Split(',');
                    if (address3.Length > 0)
                        foreach (string addr3_3 in addr3)
                        {
                            Printcount++;
                            if (Printcount == addr3.Length)
                                Send = Send + addr3_3.Trim() + LF;
                            else
                                Send = Send + addr3_3.Trim() + "," + LF;
                        }
                    if (Program.PhoneNo.Trim().Length > 0)
                        Send = Send + Program.PhoneNo + LF;
                    if (Program.TinNo.Trim().Length > 0)
                        Send = Send + Program.TinNo + LF;
                    if (Program.STax_No.Trim().Length > 0)
                        Send = Send + Program.STax_No + LF;
                    if (Program.cin_no.Trim().Length > 0)
                        Send = Send + Program.cin_no + LF;
                    if (Program.fssai_no.Length > 0)
                        Send = Send + Program.fssai_no + LF;
                    Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                    Send = Send + clsConfigSettings.Bill_Head + LF;
                    Send = Send + ESC + "!" + (char)(8);
                    Send = Send + ESC + "a" + (char)(0);
                    PosPrinter.Print(Send);



                    PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "      Order No:" + OrderNo + "\n");
                    PosPrinter.Print("DATE     :" + BillDate + "\n");
                    PosPrinter.Print("TIME     :" + BillTime + "\n");
                    PosPrinter.Print("BILL TYPE:" + Bill_Type + "\n");
                    PosPrinter.Print("CASHIER  :" + Stwd + "\n");
                    if (dtHD.Rows.Count > 0)
                    {
                        PosPrinter.Print("------------------------------------------------\n");
                        PosPrinter.Print("Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                        PosPrinter.Print("Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                        PosPrinter.Print("Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                        string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                        if (Tin_no.Trim().Length > 1)
                            PosPrinter.Print("GSTIN    :" + Tin_no + "\n");
                    }
                    PosPrinter.Print("------------------------------------------\n");
                    PosPrinter.Print("DESCRIPTION           QTY   RATE   AMOUNT ");
                    PosPrinter.Print("\n------------------------------------------\n");
                    //dttbl_assorted_item
                    Bill_Amount = 0;
                    for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                    {
                        string PrintRow = string.Empty;
                        string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                        string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                        string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                        string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                        DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                        string Rate = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2");
                        string Amount = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2");
                        //if (isdeal != "0")
                        //{
                        //    Rate = ""; Amount = "";
                        //}

                        string I_Name_Desc = "";
                        if (dttbl_Bill_tran.Columns.Contains("I_Name_Desc"))
                        {
                            I_Name_Desc = "(" + dttbl_Bill_tran.Rows[ItemIdex]["I_Name_Desc"].ToString() + ")";
                        }


                        if (ItemName.Length <= 21)
                        {
                            for (int cn = ItemName.Length; cn <= 21; ++cn)
                                ItemName += " ";
                        }
                        PrintRow = "" + ItemName + " " +
                           dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + Rate + "  " + Amount + "\n";


                        if (I_Name_Desc.Length > 2)
                        {
                            if (I_Name_Desc.Length > 42)
                            {
                                var inputText = I_Name_Desc;
                                var length = 42;

                                var splittedStrings = Split(inputText, length);

                                foreach (var splittedString in splittedStrings)
                                {
                                    PrintRow += splittedString + " " + "\n";

                                }
                                PrintRow += " " + "\n";
                            }
                            else
                            {
                                PrintRow += I_Name_Desc + " " + "\n\n";
                            }
                        }
                        else
                        {
                            PrintRow += "\n";
                        }


                        PosPrinter.Print(PrintRow);
                        if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                            PosPrinter.Print("HSN: " + HSNCODE + "\n");

                        for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                        {
                            double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                            double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                            string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                            if (DealitemRate == 0)
                                RateWitAmnt = "0.00";
                            string PrintRow_Assorted = string.Empty;
                            string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                            if (i_name.Length <= 21)
                            {
                                for (int cn = i_name.Length; cn <= 21; ++cn)
                                    i_name += " ";
                            }
                            double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                            string str_rate = "";
                            if (clsConfigSettings.PrintComboRate == "1")
                            {
                                if (I_Rate > 0)
                                    str_rate = I_Rate.ToString("N2");
                            }
                            //PrintRow_Assorted = "*"+i_name + " " +
                            // double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                            PrintRow_Assorted = "" + i_name + " " +
                           dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";
                            PosPrinter.Print(PrintRow_Assorted);
                        }
                        Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                    }
                    DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();
                    PosPrinter.Print("------------------------------------------");
                    PosPrinter.Print("\n             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");

                    for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                    {
                        PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                    }
                    #region old code display tax
                    // if (DisCountAmnt > 0)
                    // {
                    //PosPrinter.Print("       DISCOUNT @" + float.Parse(DiscountPct).ToString("N2") + "% :    " + DisCountAmnt.ToString("N2") + "\n");
                    // PosPrinter.Print("         DISCOUNT AMOUNT   :    " + DisCountAmnt.ToString("N2") + "\n");
                    //}
                    //string VatPCT = clsConfigSettings.TaxCap.ToString();
                    //string ServiceTaxPCT = ConfigurationSettings.AppSettings["Service_Tax"].ToString();
                    //string ServiceTaxPCT = clsConfigSettings.ServiceTax;
                    //for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                    // {
                    //   PosPrinter.Print("         " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                    // }
                    //double ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Tax"].ToString());
                    //double RoundOff = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["RoundOff"].ToString());
                    //if (ServiceTax > 0)
                    //{
                    //  PosPrinter.Print(" SERVICE TAX   " + ServiceTaxPCT + ".00      :    " + ServiceTax.ToString("N2") + "\n");
                    //}
                    //if (SBC_Tax > 0)
                    //    PosPrinter.Print("         SBC   " + clsConfigSettings.SBC_Rate + ".00      :    " + SBC_Tax.ToString("N2") + "\n");
                    // if (KKC_Tax > 0)
                    //  PosPrinter.Print("         KKC   " + clsConfigSettings.KKC_Rate + ".00      :    " + KKC_Tax.ToString("N2") + "\n");

                    //double Surcharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["sur_charge"].ToString());
                    //if (Surcharge > 0)
                    //{
                    //   PosPrinter.Print(" SURCHARGE ON VAT@" + clsConfigSettings.SurCharge + ".00%    :    " + Surcharge.ToString("N2") + "\n");
                    //}
                    // PosPrinter.Print("              R. OFF       :    " + RoundOff.ToString("N2") + "\n");
                    #endregion
                    PosPrinter.Print("------------------------------------------\n");
                    PosPrinter.Print("         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                    PosPrinter.Print("------------------------------------------\n");

                    //PosPrinter.Print("\n");
                    if (Remarks.Length > 1)
                    {
                        PosPrinter.Print("Remarks : " + Remarks + "\n");
                        PosPrinter.Print("------------------------------------------\n");
                    }
                    if (dtDiscount != null)
                    {
                        for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                        {
                            string Category = dtDiscount.Rows[discount]["Category"].ToString();
                            string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                            PosPrinter.Print(Category + " : " + Dis_Amount + "\n");
                        }
                        if (dtDiscount.Rows.Count > 0)
                            PosPrinter.Print("------------------------------------------\n");
                    }
                    //Send = "";
                    //Send = ESC + "@";
                    //Send = Send + ESC + "a" + (char)(1);
                    //Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                    //Send = Send + "Order No : " + OrderNo + LF + LF;
                    //Send = Send + ESC + "!" + (char)(8);
                    //Send = Send + ESC + "a" + (char)(0);
                    //PosPrinter.Print("\n" + Send + "  \n");

                    if (mode.Length > 0)
                    {
                        PosPrinter.Print(mode + "\n");
                    }

                    if (OrderComment.Trim().Length > 0)
                    {
                        PosPrinter.Print("Order Comment : " + OrderComment + Environment.NewLine);
                    }

                    if (ZomatoOrderId.Trim().Length > 0)
                    {
                        //PosPrinter.Print("Order's Source : " + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + Environment.NewLine);
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        Send = Send + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send + "  \n");
                    }

                    //Send = "";
                    //Send = ESC + "@";
                    //Send = Send + ESC + "a" + (char)(1);
                    //Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                    //Send = Send + Program.Footer1 + LF;
                    //Send = Send + Program.Footer2 + LF;
                    //Send = Send + Program.Footer3 + LF + LF + LF + LF + LF + LF;
                    //Send = Send + ESC + "!" + (char)(8);
                    //Send = Send + ESC + "a" + (char)(0);
                    //PosPrinter.Print(Send);
                    Send = "";
                    Send = ESC + "@";
                    Send = Send + ESC + "a" + (char)(1);
                    Send = Send + ESC + "!" + (char)(08 + 0.9);
                    Send = Send + Program.Footer1 + LF;
                    Send = Send + Program.Footer2 + LF;
                    Send = Send + Program.Footer3 + LF;
                    Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                    Send = Send + ESC + "!" + (char)(8);
                    Send = Send + ESC + "a" + (char)(0);
                    PosPrinter.Print("\n" + Send);
                    PosPrinter.Print(PaperFullCut);
                    Send = "";
                    TockenNo_locall = dttbl_Bill.Rows[0]["TockenNo"].ToString();
                    #endregion
                    //********************************************Print bill in notepad=========================  Copy 2****************
                    if (Convert.ToDouble(Print_No_of_HD_Bill) > 1)
                    {
                        #region Print section (Print_No_of_HD_Bill=2
                        PosPrinter.Open("testDoc1", CurrentPrinter);

                        //================Print Order No=============                        
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + "Your order number is" + LF;
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 8);
                        Send = Send + OrderNo.ToString().ToUpper() + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        Send = Send + "------------------------------------------" + LF;
                        PosPrinter.Print("\n" + Send + "  \n");
                        Send = "";
                        //============================================
                        Send = ESC + "@";
                        //set alllinment of line center**********************************************************************


                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                        Send = Send + Program.CompanyName + LF + "\n";
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + header.ToUpper() + LF;
                        //Send = Send + Program.Outlet_name + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.5);


                        Printcount = 0;
                        if (address1.Length > 0)
                            foreach (string addr1_1 in addr1)
                            {
                                Printcount++;
                                if (Printcount == addr1.Length)
                                    Send = Send + addr1_1.Trim() + LF;
                                else
                                    Send = Send + addr1_1.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        if (address2.Length > 0)
                            foreach (string addr2_2 in addr2)
                            {
                                Printcount++;
                                if (Printcount == addr2.Length)
                                    Send = Send + addr2_2.Trim() + LF;
                                else
                                    Send = Send + addr2_2.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        if (address3.Length > 0)
                            foreach (string addr3_3 in addr3)
                            {
                                Printcount++;
                                if (Printcount == addr3.Length)
                                    Send = Send + addr3_3.Trim() + LF;
                                else
                                    Send = Send + addr3_3.Trim() + "," + LF;
                            }

                        if (Program.PhoneNo.Trim().Length > 0)
                            Send = Send + Program.PhoneNo + LF;
                        if (Program.TinNo.Trim().Length > 0)
                            Send = Send + Program.TinNo + LF;
                        if (Program.STax_No.Trim().Length > 0)
                            if (Program.STax_No.Length > 0)
                                Send = Send + Program.STax_No + LF;
                        if (Program.cin_no.Length > 0)
                            Send = Send + Program.cin_no + LF;
                        if (Program.fssai_no.Length > 0)
                            Send = Send + Program.fssai_no + LF;
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + clsConfigSettings.Bill_Head + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print(Send);


                        PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "      Order No:" + OrderNo + "\n");
                        PosPrinter.Print("DATE     :" + BillDate + "\n");
                        PosPrinter.Print("TIME     :" + BillTime + "\n");
                        PosPrinter.Print("BILL TYPE:" + Bill_TypeName + "\n");
                        //PosPrinter.Print("CASHIER  :" + Program.CashierName + "\n");
                        PosPrinter.Print("CASHIER  :" + Stwd + "\n");
                        if (dtHD.Rows.Count > 0)
                        {
                            PosPrinter.Print("------------------------------------------------\n");
                            dtHD = dsFinalreport.Tables["TBL_CustomerInfo"];
                            PosPrinter.Print("Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                            PosPrinter.Print("Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                            PosPrinter.Print("Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                            string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                            if (Tin_no.Trim().Length > 1)
                                PosPrinter.Print("GSTIN    :" + Tin_no + "\n");
                        }
                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("DESCRIPTION           QTY   RATE   AMOUNT ");
                        PosPrinter.Print("\n------------------------------------------\n");
                        //dttbl_assorted_item
                        Bill_Amount = 0;
                        for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                        {
                            string PrintRow = string.Empty;
                            string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                            string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                            string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                            string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                            DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                            string Rate = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2");
                            string Amount = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2");
                            //if (isdeal != "0")
                            //{
                            //    Rate = ""; Amount = "";
                            //}
                            if (ItemName.Length <= 21)
                            {
                                for (int cn = ItemName.Length; cn <= 21; ++cn)
                                    ItemName += " ";
                            }
                            PrintRow = "" + ItemName + " " +
                               dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + Rate + "  " + Amount + "\n";
                            PosPrinter.Print(PrintRow);
                            if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                                PosPrinter.Print("HSN: " + HSNCODE + "\n");
                            for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                            {
                                double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                                string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                                if (DealitemRate == 0)
                                    RateWitAmnt = "";
                                string PrintRow_Assorted = string.Empty;
                                string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                                if (i_name.Length <= 21)
                                {
                                    for (int cn = i_name.Length; cn <= 21; ++cn)
                                        i_name += " ";
                                }
                                double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                string str_rate = "";
                                if (clsConfigSettings.PrintComboRate == "1")
                                {
                                    if (I_Rate > 0)
                                        str_rate = I_Rate.ToString("N2");
                                }
                                //PrintRow_Assorted = i_name + " " +
                                //double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                                PrintRow_Assorted = i_name + " " +
                                dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";
                                PosPrinter.Print(PrintRow_Assorted);
                            }
                            Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                        }
                        DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();
                        PosPrinter.Print("------------------------------------------");
                        PosPrinter.Print("\n             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");

                        for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        {
                            PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        }
                        #region old code to dsisplay tax
                        // if (DisCountAmnt > 0)
                        //{
                        //PosPrinter.Print("       DISCOUNT @" + float.Parse(DiscountPct).ToString("N2") + "% :    " + DisCountAmnt.ToString("N2") + "\n");
                        //   PosPrinter.Print("         DISCOUNT AMOUNT   :    " + DisCountAmnt.ToString("N2") + "\n");
                        //}
                        // VatPCT = clsConfigSettings.TaxCap.ToString();
                        //string ServiceTaxPCT = ConfigurationSettings.AppSettings["Service_Tax"].ToString();
                        // ServiceTaxPCT = clsConfigSettings.ServiceTax;
                        //for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        //{
                        // PosPrinter.Print("         " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        //}
                        // ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Tax"].ToString());
                        // RoundOff = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["RoundOff"].ToString());
                        // if (ServiceTax > 0)
                        // {
                        //   PosPrinter.Print(" SERVICE TAX   " + ServiceTaxPCT + ".00      :    " + ServiceTax.ToString("N2") + "\n");
                        //}
                        //if (SBC_Tax > 0)
                        //   PosPrinter.Print("         SBC   " + clsConfigSettings.SBC_Rate + ".00      :    " + SBC_Tax.ToString("N2") + "\n");
                        //if (KKC_Tax > 0)
                        //    PosPrinter.Print("         KKC   " + clsConfigSettings.KKC_Rate + ".00      :    " + KKC_Tax.ToString("N2") + "\n");

                        // Surcharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["sur_charge"].ToString());
                        // if (Surcharge > 0)
                        // {
                        //    PosPrinter.Print(" SURCHARGE ON VAT@" + clsConfigSettings.SurCharge + ".00%    :    " + Surcharge.ToString("N2") + "\n");
                        //}
                        //PosPrinter.Print("              R. OFF       :    " + RoundOff.ToString("N2") + "\n");
                        #endregion

                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                        PosPrinter.Print("------------------------------------------\n");
                        //PosPrinter.Print("\n");
                        if (Remarks.Length > 1)
                        {
                            PosPrinter.Print("Remarks : " + Remarks + "\n");
                            PosPrinter.Print("------------------------------------------\n");
                        }

                        if (dtDiscount != null)
                        {
                            for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                            {
                                string Category = dtDiscount.Rows[discount]["Category"].ToString();
                                string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                                PosPrinter.Print(Category + " : " + Dis_Amount + "\n");

                            }
                            if (dtDiscount.Rows.Count > 0)
                                PosPrinter.Print("------------------------------------------\n");
                        }
                        //Send = "";
                        //Send = ESC + "@";
                        //Send = Send + ESC + "a" + (char)(1);
                        //Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        //Send = Send + "Order No : " + OrderNo + LF + LF;
                        //Send = Send + ESC + "!" + (char)(8);
                        //Send = Send + ESC + "a" + (char)(0);
                        //PosPrinter.Print("\n" + Send + "  \n");
                        if (mode.Length > 0)
                        {
                            PosPrinter.Print(mode + "\n");
                        }
                        if (OrderComment.Trim().Length > 0)
                        {
                            PosPrinter.Print("Order Comment : " + OrderComment + Environment.NewLine);
                        }

                        if (ZomatoOrderId.Trim().Length > 0)
                        {
                            //PosPrinter.Print("Order's Source : " + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + Environment.NewLine);
                            Send = "";
                            Send = ESC + "@";
                            Send = Send + ESC + "a" + (char)(1);
                            Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                            Send = Send + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + LF;
                            Send = Send + ESC + "!" + (char)(8);
                            Send = Send + ESC + "a" + (char)(0);
                            PosPrinter.Print("\n" + Send + "  \n");
                        }


                        //Send = "";
                        //Send = ESC + "@";
                        //Send = Send + ESC + "a" + (char)(1);
                        //Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        //Send = Send + Program.Footer1 + LF;
                        //Send = Send + Program.Footer2 + LF;
                        //Send = Send + Program.Footer3 + LF + LF + LF + LF + LF + LF;
                        //Send = Send + ESC + "!" + (char)(8);
                        //Send = Send + ESC + "a" + (char)(0);
                        //PosPrinter.Print(Send);
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + Program.Footer1 + LF;
                        Send = Send + Program.Footer2 + LF;
                        Send = Send + Program.Footer3 + LF;
                        Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send);
                        Send = "";
                        PosPrinter.Print(PaperFullCut);
                        #endregion
                    }
                    //********************************************Print bill in notepad=========================  Copy 3****************
                    if (Convert.ToDouble(Print_No_of_HD_Bill) > 2)
                    {
                        #region Print section (Print_No_of_HD_Bill=3
                        PosPrinter.Open("testDoc1", CurrentPrinter);
                        //================Print Order No=============                        
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + "Your order number is" + LF;
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 8);
                        Send = Send + OrderNo.ToString().ToUpper() + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        Send = Send + "------------------------------------------" + LF;
                        PosPrinter.Print("\n" + Send + "  \n");
                        Send = "";
                        //============================================
                        Send = ESC + "@";
                        //set alllinment of line center**********************************************************************
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                        Send = Send + Program.CompanyName + LF + "\n";
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + header.ToUpper() + LF;
                        //Send = Send + Program.Outlet_name + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.5);

                        Printcount = 0;
                        if (address1.Length > 0)
                            foreach (string addr1_1 in addr1)
                            {
                                Printcount++;
                                if (Printcount == addr1.Length)
                                    Send = Send + addr1_1.Trim() + LF;
                                else
                                    Send = Send + addr1_1.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        if (address2.Length > 0)
                            foreach (string addr2_2 in addr2)
                            {
                                Printcount++;
                                if (Printcount == addr2.Length)
                                    Send = Send + addr2_2.Trim() + LF;
                                else
                                    Send = Send + addr2_2.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        if (address3.Length > 0)
                            foreach (string addr3_3 in addr3)
                            {
                                Printcount++;
                                if (Printcount == addr3.Length)
                                    Send = Send + addr3_3.Trim() + LF;
                                else
                                    Send = Send + addr3_3.Trim() + "," + LF;
                            }

                        if (Program.PhoneNo.Trim().Length > 0)
                            Send = Send + Program.PhoneNo + LF;
                        if (Program.TinNo.Trim().Length > 0)
                            Send = Send + Program.TinNo + LF;
                        if (Program.STax_No.Trim().Length > 0)
                            if (Program.STax_No.Length > 0)
                                Send = Send + Program.STax_No + LF;
                        if (Program.cin_no.Length > 0)
                            Send = Send + Program.cin_no + LF;
                        if (Program.fssai_no.Length > 0)
                            Send = Send + Program.fssai_no + LF;
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + clsConfigSettings.Bill_Head + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print(Send);

                        PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "      Order No:" + OrderNo + "\n");
                        PosPrinter.Print("DATE     :" + BillDate + "\n");
                        PosPrinter.Print("TIME     :" + BillTime + "\n");
                        PosPrinter.Print("BILL TYPE:" + Bill_TypeName + "\n");
                        //PosPrinter.Print("CASHIER  :" + Program.CashierName + "\n");
                        PosPrinter.Print("CASHIER  :" + Stwd + "\n");
                        if (dtHD.Rows.Count > 0)
                        {
                            PosPrinter.Print("------------------------------------------------\n");
                            dtHD = dsFinalreport.Tables["TBL_CustomerInfo"];
                            PosPrinter.Print("Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                            PosPrinter.Print("Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                            PosPrinter.Print("Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                            string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                            if (Tin_no.Trim().Length > 1)
                                PosPrinter.Print("GSTIN    :" + Tin_no + "\n");
                        }
                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("DESCRIPTION           QTY   RATE   AMOUNT ");
                        PosPrinter.Print("\n------------------------------------------\n");
                        //dttbl_assorted_item
                        Bill_Amount = 0;
                        for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                        {
                            string PrintRow = string.Empty;
                            string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                            string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                            string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                            string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                            DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                            string Rate = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2");
                            string Amount = Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2");
                            //if (isdeal != "0")
                            //{
                            //    Rate = ""; Amount = "";
                            //}
                            if (ItemName.Length <= 21)
                            {
                                for (int cn = ItemName.Length; cn <= 21; ++cn)
                                    ItemName += " ";
                            }
                            PrintRow = "" + ItemName + " " +
                               dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + Rate + "  " + Amount + "\n";
                            PosPrinter.Print(PrintRow);
                            if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                                PosPrinter.Print("HSN: " + HSNCODE + "\n");
                            for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                            {
                                double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                                string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                                if (DealitemRate == 0)
                                    RateWitAmnt = "";
                                string PrintRow_Assorted = string.Empty;
                                string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                                if (i_name.Length <= 21)
                                {
                                    for (int cn = i_name.Length; cn <= 21; ++cn)
                                        i_name += " ";
                                }
                                double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                string str_rate = "";
                                if (clsConfigSettings.PrintComboRate == "1")
                                {
                                    if (I_Rate > 0)
                                        str_rate = I_Rate.ToString("N2");
                                }
                                //PrintRow_Assorted = i_name + " " +
                                //double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                                PrintRow_Assorted = i_name + " " +
                                 dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";

                                PosPrinter.Print(PrintRow_Assorted);
                            }
                            Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                        }
                        DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();

                        PosPrinter.Print("------------------------------------------");
                        PosPrinter.Print("\n             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");
                        #region old code to display tax
                        //if (DisCountAmnt > 0)
                        //{
                        //PosPrinter.Print("       DISCOUNT @" + float.Parse(DiscountPct).ToString("N2") + "% :    " + DisCountAmnt.ToString("N2") + "\n");
                        // PosPrinter.Print("         DISCOUNT AMOUNT   :    " + DisCountAmnt.ToString("N2") + "\n");
                        //}
                        // VatPCT = clsConfigSettings.TaxCap.ToString();
                        //string ServiceTaxPCT = ConfigurationSettings.AppSettings["Service_Tax"].ToString();
                        // ServiceTaxPCT = clsConfigSettings.ServiceTax;
                        // for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        // {
                        //    PosPrinter.Print("                " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        //}
                        // ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Tax"].ToString());
                        // RoundOff = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["RoundOff"].ToString());
                        // if (ServiceTax > 0)
                        // {
                        //     PosPrinter.Print(" SERVICE TAX   " + ServiceTaxPCT + ".00      :    " + ServiceTax.ToString("N2") + "\n");
                        // }
                        //if (SBC_Tax > 0)
                        //   PosPrinter.Print("         SBC   " + clsConfigSettings.SBC_Rate + ".00      :    " + SBC_Tax.ToString("N2") + "\n");
                        //if (KKC_Tax > 0)
                        //   PosPrinter.Print("         KKC   " + clsConfigSettings.KKC_Rate + ".00      :    " + KKC_Tax.ToString("N2") + "\n");

                        // Surcharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["sur_charge"].ToString());
                        // if (Surcharge > 0)
                        // {
                        // PosPrinter.Print(" SURCHARGE ON VAT@" + clsConfigSettings.SurCharge + ".00%    :    " + Surcharge.ToString("N2") + "\n");
                        // }
                        // PosPrinter.Print("              R. OFF       :    " + RoundOff.ToString("N2") + "\n");
                        #endregion

                        for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        {
                            PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        }
                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                        PosPrinter.Print("------------------------------------------\n");

                        if (Remarks.Length > 1)
                        {
                            PosPrinter.Print("Remarks : " + Remarks + "\n");
                            PosPrinter.Print("------------------------------------------\n");
                        }

                        if (dtDiscount != null)
                        {
                            for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                            {
                                string Category = dtDiscount.Rows[discount]["Category"].ToString();
                                string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                                PosPrinter.Print(Category + " : " + Dis_Amount + "\n");

                            }
                            if (dtDiscount.Rows.Count > 0)
                                PosPrinter.Print("------------------------------------------\n");
                        }
                        //Send = "";
                        //Send = ESC + "@";
                        //Send = Send + ESC + "a" + (char)(1);
                        //Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        //Send = Send + "Order No : " + OrderNo + LF + LF;
                        //Send = Send + ESC + "!" + (char)(8);
                        //Send = Send + ESC + "a" + (char)(0);
                        //PosPrinter.Print("\n" + Send + "  \n");
                        if (mode.Length > 0)
                        {
                            PosPrinter.Print(mode + "\n");
                        }
                        if (OrderComment.Trim().Length > 0)
                        {
                            PosPrinter.Print("Order Comment : " + OrderComment + Environment.NewLine);
                        }

                        if (ZomatoOrderId.Trim().Length > 0)
                        {
                            //PosPrinter.Print("Order's Source : " + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + Environment.NewLine);
                            Send = "";
                            Send = ESC + "@";
                            Send = Send + ESC + "a" + (char)(1);
                            Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                            Send = Send + OrderSource + " (" + ZomatoOrderId.Trim() + ")" + LF;
                            Send = Send + ESC + "!" + (char)(8);
                            Send = Send + ESC + "a" + (char)(0);
                            PosPrinter.Print("\n" + Send + "  \n");
                        }
                        //Send = "";
                        //Send = ESC + "@";
                        //Send = Send + ESC + "a" + (char)(1);
                        //Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        //Send = Send + Program.Footer1 + LF;
                        //Send = Send + Program.Footer2 + LF;
                        //Send = Send + Program.Footer3 + LF + LF + LF + LF + LF + LF;
                        //Send = Send + ESC + "!" + (char)(8);
                        //Send = Send + ESC + "a" + (char)(0);
                        //PosPrinter.Print(Send);
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + Program.Footer1 + LF;
                        Send = Send + Program.Footer2 + LF;
                        Send = Send + Program.Footer3 + LF;
                        Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send);
                        PosPrinter.Print(PaperFullCut);
                        #endregion //=============================================================================
                    }
                    PosPrinter.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            Token_No = TockenNo_locall;
            OrderComment_out = OrderComment;
        }

        public static DataTable GetAssortedItem(Int64 ItemCode, DataTable dt, string Index)
        {
            DataTable dtobjItems = new DataTable();
            try
            {
                var currentStatRow = (from currentStat in dt.AsEnumerable()
                                      where currentStat.Field<Int64>("i_code") == ItemCode && currentStat.Field<String>("item_index") == Index
                                      select currentStat);
                if (currentStatRow.Count() > 0)
                {
                    dtobjItems = new DataTable();
                    dtobjItems = currentStatRow.CopyToDataTable();
                }
            }
            catch (Exception ex)
            {

            }
            return dtobjItems;
        }



        public static void PrintBill_TDT_TexFormat_New(string BillNo, string TableNo, string TenderAmount, string BalanceAmount, out string Token_No,
           out string OrderComment_out, string fin_year, string Bill_TypeName)
        {
            ADOC objADOC = new ADOC();
            string[] addr1;
            string[] addr2;
            string[] addr3;
            string OrderComment = string.Empty;
            string Send = string.Empty;
            string TockenNo_locall = string.Empty;
            string header = clsConfigSettings.header.ToString();
            PrinterSettings settings = new PrinterSettings();
            string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
            string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
            LPrinter PosPrinter = new LPrinter();
            string CurrentPrinter = settings.PrinterName;
            int Noof_bill_TA = Convert.ToInt32(clsConfigSettings.Noof_bill_TA);
            OrderComment_out = string.Empty;
            Token_No = string.Empty;
            //print text format bill
            try
            {
                PosPrinter.Open("testDoc", CurrentPrinter);
                ds_HouchScreenReport dsobjreport = new ds_HouchScreenReport();

                string sql = "Usp_GetRetailInvoice_Print_txtFormat @BillNo='" + BillNo + "'";
                if (fin_year != clsConfigSettings.fin_year)
                {
                    sql = "Usp_GetRetailInvoice_Print_txtFormat_fy @BillNo='" + BillNo + "',@fin_year='" + fin_year + "'";
                }

                DataSet dsFinalreport = objADOC.FillReportDataSet(sql, dsobjreport, "tbl_Bill", "tbl_Bill_Tran", "tbl_tax", "tbl_assorted_item", "tbl_Discount", "TBL_CustomerInfo");
                if (BillNo.Length == 1)
                {
                    BillNo = BillNo.Insert(0, "0");
                }
                string DisCountCap = dsFinalreport.Tables["tbl_bill"].Rows[0]["Dis_Amount"].ToString();
                TableNo = dsFinalreport.Tables["tbl_bill"].Rows[0]["table_no"].ToString();
                string Order = dsFinalreport.Tables["tbl_bill"].Rows[0]["OrderNo"].ToString();
                string CarNo = dsFinalreport.Tables["tbl_bill"].Rows[0]["IP"].ToString();
                if (Order == "")
                    Order = "0";
                int OrderNo = Convert.ToInt32(dsFinalreport.Tables["tbl_bill"].Rows[0]["OrderNo"].ToString());
                OrderComment = dsFinalreport.Tables["tbl_bill"].Rows[0]["Comments"].ToString();
                string DiscountPct = "0";
                double DisCountAmnt = Convert.ToDouble(DisCountCap);
                DataTable dttbl_Bill = new DataTable();
                DataTable dttbl_Bill_tran = new DataTable();
                DataTable dttbl_tax = new DataTable();
                DataTable dttbl_assorted_item = new DataTable();
                DataTable dtDiscount = new DataTable();
                dttbl_Bill = dsFinalreport.Tables["tbl_bill"];
                dttbl_Bill_tran = dsFinalreport.Tables["tbl_Bill_Tran"];
                dttbl_tax = dsFinalreport.Tables["tbl_tax"];
                dttbl_assorted_item = dsFinalreport.Tables["tbl_assorted_item"];
                dtDiscount = dsFinalreport.Tables["tbl_Discount"];
                DataTable dtHD = new DataTable();
                dtHD = dsFinalreport.Tables["TBL_CustomerInfo"];
                double SBC_Tax = Convert.ToDouble(dttbl_Bill.Rows[0]["SBC_Tax"].ToString());
                double KKC_Tax = Convert.ToDouble(dttbl_Bill.Rows[0]["KKC_Tax"].ToString());
                double Bill_Amount = 0.00;
                string Stwd = dttbl_Bill.Rows[0]["cashier"].ToString();
                string Remarks = dttbl_Bill.Rows[0]["Comments"].ToString();
                string BillType = dttbl_Bill.Rows[0]["Bill_Type"].ToString();
                string BillTime = dttbl_Bill.Rows[0]["BillTime"].ToString();
                DateTime BillDate_DateTime = DateTime.ParseExact(Program.DayEnd_BIllingDate, "yyyy-MM-dd", null);

                if (clsConfigSettings.print_size == "1")
                {
                    string BillDate = BillDate_DateTime.ToString("dd-MM-yyyy");
                    for (int PrintIndex = 0; PrintIndex < Noof_bill_TA; PrintIndex++)
                    {
                        Send = string.Empty;
                        string Bill_Type = "Take Away";
                        if (BillType == "D")
                            Bill_Type = "Dine-In";
                        string PayMode = string.Empty;
                        string CardName = string.Empty;
                        clsBLL.GetPaymentMode(BillNo, out PayMode, out CardName);
                        string mode = "Payment Mode : " + PayMode;
                        if (PayMode.Length == 0)
                        {
                            mode = string.Empty;
                        }
                        //===============================Print bill in notepad=========================
                        PosPrinter.Open("testDoc1", CurrentPrinter);
                        //================Print Order No=============                        
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + "Your order number is" + LF;
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 8);
                        Send = Send + OrderNo.ToString().ToUpper() + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        Send = Send + "------------------------------------------" + LF;
                        PosPrinter.Print("\n" + Send + "  \n");
                        Send = "";
                        //============================================

                        Send = ESC + "@";
                        //set alllinment of line center**********************************************************************
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                        Send = Send + Program.CompanyName + LF + "\n";
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + header.ToUpper() + LF;
                        //Send = Send + Program.Outlet_name + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.5);

                        //1 - font B; 32 = Double width 
                        string[] addr = Program.Address.Split(',');
                        string address1 = string.Empty;
                        string address2 = string.Empty;
                        string address3 = string.Empty;
                        int Addresscount = addr.Length;
                        int count = 0;
                        if (Addresscount > 1)
                            count = 1;

                        for (int add = 0; add <= count; add++)
                        {
                            address1 += addr[add] + ",";
                        }

                        if (Addresscount > 3)
                            count = 3;
                        else
                            count = 2;
                        if (Addresscount > 2)
                        {
                            for (int add = 2; add <= count; add++)
                            {
                                address2 += addr[add] + ",";
                            }
                        }

                        if (Addresscount > 3)
                        {
                            for (int add = 4; add < addr.Length; add++)
                            {
                                address3 += addr[add] + ",";
                            }
                        }
                        if (address3 != string.Empty)
                        {
                            address3 = address3.Remove(address3.Length - 1);
                        }
                        if (address2 != string.Empty)
                        {
                            address2 = address2.Remove(address2.Length - 1);
                        }
                        //if (address2 == string.Empty)
                        address1 = address1.Remove(address1.Length - 1);
                        int Printcount = 0;
                        addr1 = address1.Split(',');
                        if (address1.Length > 0)
                            foreach (string addr1_1 in addr1)
                            {
                                Printcount++;
                                if (Printcount == addr1.Length)
                                    Send = Send + addr1_1.Trim() + LF;
                                else
                                    Send = Send + addr1_1.Trim() + "," + LF;
                            }
                        addr2 = address2.Split(',');
                        Printcount = 0;
                        if (address2.Length > 0)
                            foreach (string addr2_2 in addr2)
                            {
                                Printcount++;
                                if (Printcount == addr2.Length)
                                    Send = Send + addr2_2.Trim() + LF;
                                else
                                    Send = Send + addr2_2.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        addr3 = address3.Split(',');
                        if (address3.Length > 0)
                            foreach (string addr3_3 in addr3)
                            {
                                Printcount++;
                                if (Printcount == addr3.Length)
                                    Send = Send + addr3_3.Trim() + LF;
                                else
                                    Send = Send + addr3_3.Trim() + "," + LF;
                            }
                        if (Program.PhoneNo.Trim().Length > 0)
                            Send = Send + Program.PhoneNo + LF;
                        if (Program.TinNo.Trim().Length > 0)
                            Send = Send + Program.TinNo + LF;
                        if (Program.STax_No.Trim().Length > 0)
                            Send = Send + Program.STax_No + LF;
                        if (Program.cin_no.Trim().Length > 0)
                            Send = Send + Program.cin_no + LF;
                        if (Program.fssai_no.Length > 0)
                            Send = Send + Program.fssai_no + LF;


                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + clsConfigSettings.Bill_Head + LF;
                        Send = Send + ESC + "!" + (char)(8);

                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print(Send);
                        if (TableNo != "")
                            PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "        Table No : " + TableNo + "\n");
                        else if (OrderNo > 0)
                            PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "        Order No : " + OrderNo.ToString() + "\n");
                        else
                            PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "\n");

                        //PosPrinter.Print("DATE   :" + Program.DayEnd_BIllingDate + "    Cover    :" + TableCover + "\n");
                        PosPrinter.Print("DATE     :" + BillDate + "\n");
                        PosPrinter.Print("TIME     :" + BillTime + "\n");
                        PosPrinter.Print("BILL TYPE:" + Bill_Type + "\n");
                        PosPrinter.Print("CASHIER  :" + Stwd + "\n");
                        if (dtHD.Rows.Count > 0)
                        {
                            PosPrinter.Print("------------------------------------------\n");
                            if (dtHD.Rows[0]["Name"].ToString().Length > 0)
                                PosPrinter.Print("Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                            PosPrinter.Print("Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                            string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                            if (Tin_no.Trim().Length > 1)
                                PosPrinter.Print("GSTIN    :" + Tin_no + "\n");
                            //PosPrinter.Print("Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                        }
                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("DESCRIPTION           QTY   RATE   AMOUNT ");
                        PosPrinter.Print("\n------------------------------------------\n");
                        //dttbl_assorted_item
                        Bill_Amount = 0;
                        for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                        {
                            string PrintRow_Assorted = string.Empty;
                            string PrintRow = string.Empty;
                            string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                            string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                            string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                            string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                            DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                            if (ItemName.Length <= 21)
                            {
                                for (int cn = ItemName.Length; cn <= 21; ++cn)
                                    ItemName += " ";
                            }

                            PrintRow = "" + ItemName + " " +
                               dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + double.Parse(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2") + "  " + double.Parse(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2") + "\n";
                            PosPrinter.Print(PrintRow);
                            if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                                PosPrinter.Print("HSN: " + HSNCODE + "\n");
                            for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                            {
                                double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                                string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                                //if (DealitemRate == 0)
                                RateWitAmnt = "0.00";
                                PrintRow_Assorted = string.Empty;
                                string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                                if (i_name.Length <= 21)
                                {
                                    for (int cn = i_name.Length; cn <= 21; ++cn)
                                        i_name += " ";
                                }
                                double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                string str_rate = "";
                                if (clsConfigSettings.PrintComboRate == "1")
                                {
                                    if (I_Rate > 0)
                                        str_rate = I_Rate.ToString("N2");
                                }
                                PrintRow_Assorted = "" + i_name + " " +
                                //double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + " " + double.Parse(dtItems.Rows[Index]["rate"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                                //PosPrinter.Print(PrintRow_Assorted);
                                dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";
                                PosPrinter.Print(PrintRow_Assorted);
                            }
                            Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                        }
                        DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();
                        PosPrinter.Print("------------------------------------------");
                        PosPrinter.Print("\n             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");

                        for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        {
                            PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        }
                        #region old code to display tax
                        //if (DisCountAmnt > 0)
                        //{
                        //PosPrinter.Print("       DISCOUNT @" + float.Parse(DiscountPct).ToString("N2") + "% :    " + DisCountAmnt.ToString("N2") + "\n");
                        //PosPrinter.Print("         DISCOUNT AMOUNT   :    " + DisCountAmnt.ToString("N2") + "\n");
                        //}
                        //string VatPCT = clsConfigSettings.TaxCap.ToString();
                        //string ServiceTaxPCT = ConfigurationSettings.AppSettings["Service_Tax"].ToString();
                        //string ServiceTaxPCT = clsConfigSettings.ServiceTax;
                        //for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        // {
                        // PosPrinter.Print("         " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        //}
                        //double ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Tax"].ToString());
                        //double RoundOff = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["RoundOff"].ToString());
                        //if (ServiceTax > 0)
                        // PosPrinter.Print(" SERVICE TAX   @" + ServiceTaxPCT + "%       :    " + ServiceTax.ToString("N2") + "\n");
                        // if (SBC_Tax > 0)
                        //    PosPrinter.Print("         SBC   @" + clsConfigSettings.SBC_Rate + "%       :    " + SBC_Tax.ToString("N2") + "\n");
                        // if (KKC_Tax > 0)
                        //  PosPrinter.Print("         KKC   @" + clsConfigSettings.KKC_Rate + "%       :    " + KKC_Tax.ToString("N2") + "\n");


                        //double Surcharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["sur_charge"].ToString());
                        // if (Surcharge > 0)
                        //{
                        //  PosPrinter.Print(" SURCHARGE ON VAT@" + clsConfigSettings.SurCharge + "%       :    " + Surcharge.ToString("N2") + "\n");
                        //}
                        //PosPrinter.Print("              R. OFF       :    " + RoundOff.ToString("N2") + "\n");
                        #endregion

                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                        PosPrinter.Print("------------------------------------------\n");
                        TockenNo_locall = dttbl_Bill.Rows[0]["TockenNo"].ToString();
                        //PosPrinter.Print("\n");
                        if (Remarks.Length > 1)
                        {
                            PosPrinter.Print("Remarks : " + Remarks + "\n");
                            PosPrinter.Print("------------------------------------------\n");
                        }
                        if (CarNo.Length > 1)
                        {
                            //PosPrinter.Print("Car No : " + CarNo + "\n");
                            //PosPrinter.Print("------------------------------------------\n");
                            Send = "";
                            Send = ESC + "@";
                            Send = Send + ESC + "a" + (char)(1);
                            Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                            Send = Send + "Car No : " + CarNo.ToUpper() + LF + LF;
                            Send = Send + ESC + "!" + (char)(8);
                            Send = Send + ESC + "a" + (char)(0);
                            PosPrinter.Print("\n" + Send + "  \n");
                        }
                        if (dtDiscount != null)
                        {
                            for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                            {
                                string Category = dtDiscount.Rows[discount]["Category"].ToString();
                                string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                                PosPrinter.Print(Category + " : " + Dis_Amount + "\n");

                            }
                            if (dtDiscount.Rows.Count > 0)
                                PosPrinter.Print("------------------------------------------\n");
                        }
                        //Send = "";
                        //Send = ESC + "@";
                        //Send = Send + ESC + "a" + (char)(1);
                        //Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        //Send = Send + "Order No : " + OrderNo + LF + LF;
                        //Send = Send + ESC + "!" + (char)(8);
                        //Send = Send + ESC + "a" + (char)(0);
                        //PosPrinter.Print("\n" + Send + "  \n");

                        PosPrinter.Print(TenderAmount + "\n");
                        PosPrinter.Print(BalanceAmount + "\n");
                        if (mode.Length > 0)
                        {
                            PosPrinter.Print(mode + "\n");
                        }

                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + Program.Footer1 + LF;
                        Send = Send + Program.Footer2 + LF;
                        Send = Send + Program.Footer3 + LF;
                        Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send);
                        PosPrinter.Print(PaperFullCut);
                        //PosPrinter.Print("\n---------------Thank You -----\n\n\n\n\n\n");
                        ///Re Print if nc						
                        PosPrinter.Close();
                    }
                }
                else if (clsConfigSettings.print_size == "2")
                {
                    int SpaceCount = Convert.ToInt32(ConfigurationSettings.AppSettings["SPACE"].ToString());
                    string SPACE1 = string.Empty;
                    for (int i = 0; i < SpaceCount; i++)
                        SPACE1 += " ";
                    string BillDate = BillDate_DateTime.ToString("dd-MM-yyyy");
                    for (int PrintIndex = 0; PrintIndex < Noof_bill_TA; PrintIndex++)
                    {
                        Send = string.Empty;
                        string Bill_Type = "Take Away";
                        if (BillType == "D")
                            Bill_Type = "Dine-In";
                        string PayMode = string.Empty;
                        string CardName = string.Empty;
                        clsBLL.GetPaymentMode(BillNo, out PayMode, out CardName);
                        string mode = "Payment Mode : " + PayMode;
                        if (PayMode.Length == 0)
                        {
                            mode = string.Empty;
                        }
                        //===============================Print bill in notepad=========================
                        PosPrinter.Open("testDoc1", CurrentPrinter);
                        //================Print Order No=============                        
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + "Your order number is" + LF;
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 8);
                        Send = Send + OrderNo.ToString().ToUpper() + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        Send = Send + "------------------------------------------" + LF;
                        PosPrinter.Print("\n" + Send + "  \n");
                        Send = "";
                        //============================================

                        Send = ESC + "@";
                        //set alllinment of line center**********************************************************************
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                        Send = Send + Program.CompanyName + LF + "\n";
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + header.ToUpper() + LF;
                        //Send = Send + Program.Outlet_name + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.5);

                        //1 - font B; 32 = Double width 
                        string[] addr = Program.Address.Split(',');
                        string address1 = string.Empty;
                        string address2 = string.Empty;
                        string address3 = string.Empty;
                        int Addresscount = addr.Length;
                        int count = 0;
                        if (Addresscount > 1)
                            count = 1;

                        for (int add = 0; add <= count; add++)
                        {
                            address1 += addr[add] + ",";
                        }

                        if (Addresscount > 3)
                            count = 3;
                        else
                            count = 2;
                        if (Addresscount > 2)
                        {
                            for (int add = 2; add <= count; add++)
                            {
                                address2 += addr[add] + ",";
                            }
                        }

                        if (Addresscount > 3)
                        {
                            for (int add = 4; add < addr.Length; add++)
                            {
                                address3 += addr[add] + ",";
                            }
                        }
                        if (address3 != string.Empty)
                        {
                            address3 = address3.Remove(address3.Length - 1);
                        }
                        if (address2 != string.Empty)
                        {
                            address2 = address2.Remove(address2.Length - 1);
                        }
                        //if (address2 == string.Empty)
                        address1 = address1.Remove(address1.Length - 1);
                        int Printcount = 0;
                        addr1 = address1.Split(',');
                        if (address1.Length > 0)
                            foreach (string addr1_1 in addr1)
                            {
                                Printcount++;
                                if (Printcount == addr1.Length)
                                    Send = Send + addr1_1.Trim() + LF;
                                else
                                    Send = Send + addr1_1.Trim() + "," + LF;
                            }
                        addr2 = address2.Split(',');
                        Printcount = 0;
                        if (address2.Length > 0)
                            foreach (string addr2_2 in addr2)
                            {
                                Printcount++;
                                if (Printcount == addr2.Length)
                                    Send = Send + addr2_2.Trim() + LF;
                                else
                                    Send = Send + addr2_2.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        addr3 = address3.Split(',');
                        if (address3.Length > 0)
                            foreach (string addr3_3 in addr3)
                            {
                                Printcount++;
                                if (Printcount == addr3.Length)
                                    Send = Send + addr3_3.Trim() + LF;
                                else
                                    Send = Send + addr3_3.Trim() + "," + LF;
                            }
                        if (Program.PhoneNo.Trim().Length > 0)
                            Send = Send + Program.PhoneNo + LF;
                        if (Program.TinNo.Trim().Length > 0)
                            Send = Send + Program.TinNo + LF;
                        if (Program.STax_No.Trim().Length > 0)
                            Send = Send + Program.STax_No + LF;
                        if (Program.cin_no.Trim().Length > 0)
                            Send = Send + Program.cin_no + LF;
                        if (Program.fssai_no.Length > 0)
                            Send = Send + Program.fssai_no + LF;


                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + clsConfigSettings.Bill_Head + LF;
                        Send = Send + ESC + "!" + (char)(8);

                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print(Send);
                        if (TableNo != "")
                            PosPrinter.Print("\n" + SPACE1 + "BILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "        Table No : " + TableNo + "\n");
                        else if (OrderNo > 0)
                            PosPrinter.Print("\n" + SPACE1 + "BILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "        Order No : " + OrderNo.ToString() + "\n");
                        else
                            PosPrinter.Print("\n" + SPACE1 + "BILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "\n");

                        //PosPrinter.Print("DATE   :" + Program.DayEnd_BIllingDate + "    Cover    :" + TableCover + "\n");
                        PosPrinter.Print(SPACE1 + "DATE     :" + BillDate + "\n");
                        PosPrinter.Print(SPACE1 + "TIME     :" + BillTime + "\n");
                        PosPrinter.Print(SPACE1 + "BILL TYPE:" + Bill_Type + "\n");
                        PosPrinter.Print(SPACE1 + "CASHIER  :" + Stwd + "\n");
                        if (dtHD.Rows.Count > 0)
                        {
                            PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                            if (dtHD.Rows[0]["Name"].ToString().Length > 0)
                                PosPrinter.Print(SPACE1 + "Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                            PosPrinter.Print(SPACE1 + "Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                            string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                            if (Tin_no.Trim().Length > 1)
                                PosPrinter.Print(SPACE1 + "GSTIN    :" + Tin_no + "\n");
                            //PosPrinter.Print("Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                        }
                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        PosPrinter.Print(SPACE1 + "DESCRIPTION           QTY   RATE   AMOUNT ");
                        PosPrinter.Print("\n" + SPACE1 + "------------------------------------------\n");
                        //dttbl_assorted_item
                        Bill_Amount = 0;
                        for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                        {
                            string PrintRow_Assorted = string.Empty;
                            string PrintRow = string.Empty;
                            string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                            string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                            string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                            string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                            DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                            if (ItemName.Length <= 21)
                            {
                                for (int cn = ItemName.Length; cn <= 21; ++cn)
                                    ItemName += " ";
                            }

                            PrintRow = SPACE1 + "" + ItemName + " " +
                               dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + double.Parse(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2") + "  " + double.Parse(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2") + "\n";
                            PosPrinter.Print(PrintRow);
                            if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                                PosPrinter.Print(SPACE1 + "HSN: " + HSNCODE + "\n");
                            for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                            {
                                double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                                string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                                //if (DealitemRate == 0)
                                RateWitAmnt = "0.00";
                                PrintRow_Assorted = string.Empty;
                                string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                                if (i_name.Length <= 21)
                                {
                                    for (int cn = i_name.Length; cn <= 21; ++cn)
                                        i_name += " ";
                                }
                                double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                string str_rate = "";
                                if (clsConfigSettings.PrintComboRate == "1")
                                {
                                    if (I_Rate > 0)
                                        str_rate = I_Rate.ToString("N2");
                                }
                                PrintRow_Assorted = SPACE1 + "" + i_name + " " +
                                //double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + " " + double.Parse(dtItems.Rows[Index]["rate"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                                //PosPrinter.Print(PrintRow_Assorted);
                                dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";
                                PosPrinter.Print(PrintRow_Assorted);
                            }
                            Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                        }
                        DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();
                        PosPrinter.Print(SPACE1 + "------------------------------------------");
                        PosPrinter.Print("\n" + SPACE1 + "             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");

                        for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        {
                            PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        }

                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        PosPrinter.Print(SPACE1 + "         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        TockenNo_locall = dttbl_Bill.Rows[0]["TockenNo"].ToString();
                        //PosPrinter.Print("\n");
                        if (Remarks.Length > 1)
                        {
                            PosPrinter.Print(SPACE1 + "Remarks : " + Remarks + "\n");
                            PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        }
                        if (CarNo.Length > 1)
                        {
                            //PosPrinter.Print("Car No : " + CarNo + "\n");
                            //PosPrinter.Print("------------------------------------------\n");
                            Send = "";
                            Send = ESC + "@";
                            Send = Send + ESC + "a" + (char)(1);
                            Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                            Send = Send + "Car No : " + CarNo.ToUpper() + LF + LF;
                            Send = Send + ESC + "!" + (char)(8);
                            Send = Send + ESC + "a" + (char)(0);
                            PosPrinter.Print("\n" + Send + "  \n");
                        }
                        if (dtDiscount != null)
                        {
                            for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                            {
                                string Category = dtDiscount.Rows[discount]["Category"].ToString();
                                string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                                PosPrinter.Print(SPACE1 + Category + " : " + Dis_Amount + "\n");

                            }
                            if (dtDiscount.Rows.Count > 0)
                                PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        }
                        //Send = "";
                        //Send = ESC + "@";
                        //Send = Send + ESC + "a" + (char)(1);
                        //Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        //Send = Send + "Order No : " + OrderNo + LF + LF;
                        //Send = Send + ESC + "!" + (char)(8);
                        //Send = Send + ESC + "a" + (char)(0);
                        //PosPrinter.Print("\n" + Send + "  \n");

                        PosPrinter.Print(SPACE1 + TenderAmount + "\n");
                        PosPrinter.Print(SPACE1 + BalanceAmount + "\n");
                        if (mode.Length > 0)
                        {
                            PosPrinter.Print(SPACE1 + mode + "\n");
                        }

                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + Program.Footer1 + LF;
                        Send = Send + Program.Footer2 + LF;
                        Send = Send + Program.Footer3 + LF;
                        Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send);
                        PosPrinter.Print(PaperFullCut);
                        //PosPrinter.Print("\n---------------Thank You -----\n\n\n\n\n\n");
                        ///Re Print if nc					
                        PosPrinter.Close();
                    }
                }
                else if (clsConfigSettings.print_size == "3")
                {
                    int SpaceCount = Convert.ToInt32(ConfigurationSettings.AppSettings["SPACE"].ToString());
                    string SPACE1 = string.Empty;
                    for (int i = 0; i < SpaceCount; i++)
                        SPACE1 += " ";
                    string BillDate = BillDate_DateTime.ToString("dd-MM-yyyy");
                    for (int PrintIndex = 0; PrintIndex < Noof_bill_TA; PrintIndex++)
                    {
                        Send = string.Empty;
                        string Bill_Type = "Take Away";
                        if (BillType == "D")
                            Bill_Type = "Dine-In";
                        string PayMode = string.Empty;
                        string CardName = string.Empty;
                        clsBLL.GetPaymentMode(BillNo, out PayMode, out CardName);
                        string mode = "Payment Mode : " + PayMode;
                        if (PayMode.Length == 0)
                        {
                            mode = string.Empty;
                        }
                        //===============================Print bill in notepad=========================
                        PosPrinter.Open("testDoc1", CurrentPrinter);
                        //================Print Order No=============                        
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + "Your order number is" + LF;
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 8);
                        Send = Send + OrderNo.ToString().ToUpper() + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        Send = Send + "------------------------------------------" + LF;
                        PosPrinter.Print("\n" + Send + "  \n");
                        Send = "";
                        //============================================

                        Send = ESC + "@";
                        //set alllinment of line center**********************************************************************
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                        Send = Send + Program.CompanyName + LF + "\n";
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + header.ToUpper() + LF;
                        //Send = Send + Program.Outlet_name + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.5);

                        //1 - font B; 32 = Double width 
                        string[] addr = Program.Address.Split(',');
                        string address1 = string.Empty;
                        string address2 = string.Empty;
                        string address3 = string.Empty;
                        int Addresscount = addr.Length;
                        int count = 0;
                        if (Addresscount > 1)
                            count = 1;

                        for (int add = 0; add <= count; add++)
                        {
                            address1 += addr[add] + ",";
                        }

                        if (Addresscount > 3)
                            count = 3;
                        else
                            count = 2;
                        if (Addresscount > 2)
                        {
                            for (int add = 2; add <= count; add++)
                            {
                                address2 += addr[add] + ",";
                            }
                        }

                        if (Addresscount > 3)
                        {
                            for (int add = 4; add < addr.Length; add++)
                            {
                                address3 += addr[add] + ",";
                            }
                        }
                        if (address3 != string.Empty)
                        {
                            address3 = address3.Remove(address3.Length - 1);
                        }
                        if (address2 != string.Empty)
                        {
                            address2 = address2.Remove(address2.Length - 1);
                        }
                        //if (address2 == string.Empty)
                        address1 = address1.Remove(address1.Length - 1);
                        int Printcount = 0;
                        addr1 = address1.Split(',');
                        if (address1.Length > 0)
                            foreach (string addr1_1 in addr1)
                            {
                                Printcount++;
                                if (Printcount == addr1.Length)
                                    Send = Send + addr1_1.Trim() + LF;
                                else
                                    Send = Send + addr1_1.Trim() + "," + LF;
                            }
                        addr2 = address2.Split(',');
                        Printcount = 0;
                        if (address2.Length > 0)
                            foreach (string addr2_2 in addr2)
                            {
                                Printcount++;
                                if (Printcount == addr2.Length)
                                    Send = Send + addr2_2.Trim() + LF;
                                else
                                    Send = Send + addr2_2.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        addr3 = address3.Split(',');
                        if (address3.Length > 0)
                            foreach (string addr3_3 in addr3)
                            {
                                Printcount++;
                                if (Printcount == addr3.Length)
                                    Send = Send + addr3_3.Trim() + LF;
                                else
                                    Send = Send + addr3_3.Trim() + "," + LF;
                            }
                        if (Program.PhoneNo.Trim().Length > 0)
                            Send = Send + Program.PhoneNo + LF;
                        if (Program.TinNo.Trim().Length > 0)
                            Send = Send + Program.TinNo + LF;
                        if (Program.STax_No.Trim().Length > 0)
                            Send = Send + Program.STax_No + LF;
                        if (Program.cin_no.Trim().Length > 0)
                            Send = Send + Program.cin_no + LF;
                        if (Program.fssai_no.Length > 0)
                            Send = Send + Program.fssai_no + LF;
                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + clsConfigSettings.Bill_Head + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);


                        Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        Send = Send + "\n" + SPACE1 + "BILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "\n\n";
                        Send = Send + SPACE1 + "BILL TYPE:" + Bill_Type + "\n\n";
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);

                        Send = Send + ESC + "!" + (char)(28 + 8 + 1);
                        Send = Send + SPACE1 + "DATE     :" + BillDate + "\n";
                        Send = Send + SPACE1 + "TIME     :" + BillTime + "\n";
                        Send = Send + SPACE1 + "CASHIER:" + Stwd + "\n";
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print(Send);

                        if (dtHD.Rows.Count > 0)
                        {
                            PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                            if (dtHD.Rows[0]["Name"].ToString().Length > 0)
                                PosPrinter.Print(SPACE1 + "Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                            PosPrinter.Print(SPACE1 + "Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                            string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                            if (Tin_no.Trim().Length > 1)
                                PosPrinter.Print(SPACE1 + "GSTIN    :" + Tin_no + "\n");
                            //PosPrinter.Print("Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                        }
                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        PosPrinter.Print(SPACE1 + "DESCRIPTION           QTY   RATE   AMOUNT ");
                        PosPrinter.Print("\n" + SPACE1 + "------------------------------------------\n");
                        //dttbl_assorted_item
                        Bill_Amount = 0;
                        for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                        {
                            string PrintRow_Assorted = string.Empty;
                            string PrintRow = string.Empty;
                            string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                            string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                            string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                            string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                            DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                            if (ItemName.Length <= 21)
                            {
                                for (int cn = ItemName.Length; cn <= 21; ++cn)
                                    ItemName += " ";
                            }

                            PrintRow = SPACE1 + "" + ItemName + " " +
                               dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + double.Parse(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2") + "  " + double.Parse(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2") + "\n";
                            PosPrinter.Print(PrintRow);
                            if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                                PosPrinter.Print(SPACE1 + "HSN: " + HSNCODE + "\n");
                            for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                            {
                                double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                                string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                                //if (DealitemRate == 0)
                                RateWitAmnt = "0.00";
                                PrintRow_Assorted = string.Empty;
                                string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                                if (i_name.Length <= 21)
                                {
                                    for (int cn = i_name.Length; cn <= 21; ++cn)
                                        i_name += " ";
                                }
                                double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                string str_rate = "";
                                if (clsConfigSettings.PrintComboRate == "1")
                                {
                                    if (I_Rate > 0)
                                        str_rate = I_Rate.ToString("N2");
                                }
                                PrintRow_Assorted = SPACE1 + "" + i_name + " " +
                                //double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + " " + double.Parse(dtItems.Rows[Index]["rate"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                                //PosPrinter.Print(PrintRow_Assorted);
                                dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";
                                PosPrinter.Print(PrintRow_Assorted);
                            }
                            Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                        }
                        DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();
                        PosPrinter.Print(SPACE1 + "------------------------------------------");
                        PosPrinter.Print("\n" + SPACE1 + "             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");

                        for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        {
                            PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        }

                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        PosPrinter.Print(SPACE1 + "         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                        PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        TockenNo_locall = dttbl_Bill.Rows[0]["TockenNo"].ToString();
                        //PosPrinter.Print("\n");
                        if (Remarks.Length > 1)
                        {
                            PosPrinter.Print(SPACE1 + "Remarks : " + Remarks + "\n");
                            PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        }
                        if (CarNo.Length > 1)
                        {
                            //PosPrinter.Print("Car No : " + CarNo + "\n");
                            //PosPrinter.Print("------------------------------------------\n");
                            Send = "";
                            Send = ESC + "@";
                            Send = Send + ESC + "a" + (char)(1);
                            Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                            Send = Send + "Car No : " + CarNo.ToUpper() + LF + LF;
                            Send = Send + ESC + "!" + (char)(8);
                            Send = Send + ESC + "a" + (char)(0);
                            PosPrinter.Print("\n" + Send + "  \n");
                        }
                        if (dtDiscount != null)
                        {
                            for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                            {
                                string Category = dtDiscount.Rows[discount]["Category"].ToString();
                                string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                                PosPrinter.Print(SPACE1 + Category + " : " + Dis_Amount + "\n");

                            }
                            if (dtDiscount.Rows.Count > 0)
                                PosPrinter.Print(SPACE1 + "------------------------------------------\n");
                        }
                        //Send = "";
                        //Send = ESC + "@";
                        //Send = Send + ESC + "a" + (char)(1);
                        //Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        //Send = Send + "Order No : " + OrderNo + LF + LF;
                        //Send = Send + ESC + "!" + (char)(8);
                        //Send = Send + ESC + "a" + (char)(0);
                        //PosPrinter.Print("\n" + Send + "  \n");

                        PosPrinter.Print(SPACE1 + TenderAmount + "\n");
                        PosPrinter.Print(SPACE1 + BalanceAmount + "\n");
                        if (mode.Length > 0)
                        {
                            PosPrinter.Print(SPACE1 + mode + "\n");
                        }

                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + Program.Footer1 + LF;
                        Send = Send + Program.Footer2 + LF;
                        Send = Send + Program.Footer3 + LF;
                        Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send);
                        PosPrinter.Print(PaperFullCut);
                        //PosPrinter.Print("\n---------------Thank You -----\n\n\n\n\n\n");
                        ///Re Print if nc

                        PosPrinter.Close();
                    }
                }
                else if (clsConfigSettings.print_size == "4")
                {
                    string BillDate = BillDate_DateTime.ToString("dd-MM-yyyy");
                    for (int PrintIndex = 0; PrintIndex < Noof_bill_TA; PrintIndex++)
                    {
                        Send = string.Empty;
                        string Bill_Type = "Take Away";
                        if (BillType == "D")
                            Bill_Type = "Dine-In";
                        string PayMode = string.Empty;
                        string CardName = string.Empty;
                        clsBLL.GetPaymentMode(BillNo, out PayMode, out CardName);
                        string mode = "Payment Mode : " + PayMode;
                        if (PayMode.Length == 0)
                        {
                            mode = string.Empty;
                        }
                        //===============================Print bill in notepad=========================
                        PosPrinter.Open("testDoc1", CurrentPrinter);
                        Send = ESC + "@";
                        //set alllinment of line center**********************************************************************
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 8 + 0.9);
                        //Send = Send + ESC + "a" + (char)(0);

                        //Send = Send + ESC + "a" + (char)(1);
                        //Send = Send + ESC + "!" + (char)(16 + 16 );
                        string[] Company = Program.CompanyName.Split(',');      //1 - font B; 32 = Double width   
                        foreach (string author in Company)
                        {
                            Send = Send + author + LF;
                        }
                        Send = Send + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + header.ToUpper() + LF;
                        //Send = Send + Program.Outlet_name + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.5);

                        //1 - font B; 32 = Double width 
                        string[] addr = Program.Address.Split(',');
                        string address1 = string.Empty;
                        string address2 = string.Empty;
                        string address3 = string.Empty;
                        int Addresscount = addr.Length;
                        int count = 0;
                        if (Addresscount > 1)
                            count = 1;

                        for (int add = 0; add <= count; add++)
                        {
                            address1 += addr[add] + ",";
                        }

                        if (Addresscount > 3)
                            count = 3;
                        else
                            count = 2;
                        if (Addresscount > 2)
                        {
                            for (int add = 2; add <= count; add++)
                            {
                                address2 += addr[add] + ",";
                            }
                        }

                        if (Addresscount > 3)
                        {
                            for (int add = 4; add < addr.Length; add++)
                            {
                                address3 += addr[add] + ",";
                            }
                        }
                        if (address3 != string.Empty)
                        {
                            address3 = address3.Remove(address3.Length - 1);
                        }
                        if (address2 != string.Empty)
                        {
                            address2 = address2.Remove(address2.Length - 1);
                        }
                        //if (address2 == string.Empty)
                        address1 = address1.Remove(address1.Length - 1);
                        int Printcount = 0;
                        addr1 = address1.Split(',');
                        if (address1.Length > 0)
                            foreach (string addr1_1 in addr1)
                            {
                                Printcount++;
                                if (Printcount == addr1.Length)
                                    Send = Send + addr1_1.Trim() + LF;
                                else
                                    Send = Send + addr1_1.Trim() + "," + LF;
                            }
                        addr2 = address2.Split(',');
                        Printcount = 0;
                        if (address2.Length > 0)
                            foreach (string addr2_2 in addr2)
                            {
                                Printcount++;
                                if (Printcount == addr2.Length)
                                    Send = Send + addr2_2.Trim() + LF;
                                else
                                    Send = Send + addr2_2.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        addr3 = address3.Split(',');
                        if (address3.Length > 0)
                            foreach (string addr3_3 in addr3)
                            {
                                Printcount++;
                                if (Printcount == addr3.Length)
                                    Send = Send + addr3_3.Trim() + LF;
                                else
                                    Send = Send + addr3_3.Trim() + "," + LF;
                            }
                        if (Program.PhoneNo.Trim().Length > 0)
                            Send = Send + Program.PhoneNo + LF;
                        if (Program.TinNo.Trim().Length > 0)
                            Send = Send + Program.TinNo + LF;
                        if (Program.STax_No.Trim().Length > 0)
                            Send = Send + Program.STax_No + LF;
                        if (Program.cin_no.Trim().Length > 0)
                            Send = Send + Program.cin_no + LF;
                        if (Program.fssai_no.Length > 0)
                            Send = Send + Program.fssai_no + LF;


                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + clsConfigSettings.Bill_Head + LF;
                        Send = Send + ESC + "!" + (char)(8);

                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print(Send);
                        if (TableNo != "")
                            PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "        Table No : " + TableNo + "\n");
                        else if (OrderNo > 0)
                            PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "        Order No : " + OrderNo.ToString() + "\n");
                        else
                            PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "\n");

                        //PosPrinter.Print("DATE   :" + Program.DayEnd_BIllingDate + "    Cover    :" + TableCover + "\n");
                        PosPrinter.Print("DATE     :" + BillDate + "\n");
                        PosPrinter.Print("TIME     :" + BillTime + "\n");
                        PosPrinter.Print("BILL TYPE:" + Bill_Type + "\n");
                        PosPrinter.Print("CASHIER  :" + Stwd + "\n");
                        if (dtHD.Rows.Count > 0)
                        {
                            PosPrinter.Print("------------------------------------------------\n");
                            if (dtHD.Rows[0]["Name"].ToString().Length > 0)
                                PosPrinter.Print("Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                            PosPrinter.Print("Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                            string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                            if (Tin_no.Trim().Length > 1)
                                PosPrinter.Print("GSTIN    :" + Tin_no + "\n");
                            //PosPrinter.Print("Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                        }
                        PosPrinter.Print("==============================================\n");
                        PosPrinter.Print("DESCRIPTION          QTY     RATE     AMOUNT ");
                        PosPrinter.Print("\n==============================================\n");
                        //dttbl_assorted_item
                        Bill_Amount = 0;
                        for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                        {
                            string PrintRow_Assorted = string.Empty;
                            string PrintRow = string.Empty;
                            string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                            string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                            string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                            string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                            DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                            string QTY = (dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString());
                            string Rate = double.Parse(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2");
                            string Amount = double.Parse(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2");


                            if (ItemName.Length <= 21)
                            {
                                for (int cn = ItemName.Length; cn <= 21; ++cn)
                                    ItemName += " ";
                            }
                            if (ItemName.Length < 16)
                            {
                                for (int cn = ItemName.Length; cn <= 15; ++cn)
                                    ItemName += " ";
                            }


                            //if (QTY.Length < 7)
                            //{
                            //    for (int cn = QTY.Length; cn <= 6; ++cn)
                            //        QTY = " " + QTY;
                            //}

                            if (Rate.Length < 8)
                            {
                                for (int cn = Rate.Length; cn <= 7; ++cn)
                                    Rate = " " + Rate;
                            }

                            if (Amount.Length < 8)
                            {
                                for (int cn = Amount.Length; cn <= 7; ++cn)
                                    Amount = " " + Amount;
                            }
                            Send = Send + PrintRow + LF;
                            PrintRow = ItemName + "" +
                            QTY + "   " + Rate + "   " + Amount + "\n";

                            Send = Send + PrintRow + LF;


                            //PrintRow = "" + ItemName + " " +
                            //            QTY + "  " + Rate + " " + Amount + "\n";
                            PosPrinter.Print(PrintRow);
                            if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                                PosPrinter.Print("HSN: " + HSNCODE + "\n");
                            for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                            {
                                double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                                string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                                //if (DealitemRate == 0)
                                RateWitAmnt = "0.00";
                                PrintRow_Assorted = string.Empty;
                                string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                                if (i_name.Length <= 21)
                                {
                                    for (int cn = i_name.Length; cn <= 21; ++cn)
                                        i_name += " ";
                                }
                                double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                string str_rate = "";
                                if (clsConfigSettings.PrintComboRate == "1")
                                {
                                    if (I_Rate > 0)
                                        str_rate = I_Rate.ToString("N2");
                                }
                                PrintRow_Assorted = "" + i_name + " " +
                                //double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + " " + double.Parse(dtItems.Rows[Index]["rate"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                                //PosPrinter.Print(PrintRow_Assorted);
                                dtItems.Rows[Index]["Qty"].ToString() + "  " + str_rate + "\n";
                                PosPrinter.Print(PrintRow_Assorted);
                            }
                            Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                        }
                        DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();

                        //PosPrinter.Print("----------------------------------------------");

                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + "-----------------------------------------------" + LF;
                        Send = Send + "              " + "BILL AMOUNT :       " + Bill_Amount.ToString("N2") + LF;
                        for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        {
                            string PrintTaxRow = string.Empty;
                            string TaxName = dttbl_tax.Rows[a]["tax"].ToString();
                            string tax_amount = Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2");
                            if (TaxName.Length > 16)
                                TaxName = TaxName.Substring(0, 16);
                            if (TaxName.Length < 16)
                            {
                                for (int cn = TaxName.Length; cn <= 15; ++cn)
                                    TaxName += "";
                            }

                            if (tax_amount.Length < 13)
                            {
                                for (int cn = tax_amount.Length; cn <= 12; ++cn)
                                    tax_amount = " " + tax_amount;
                            }
                            PrintTaxRow = "  " + TaxName + "  "
                                  + tax_amount + " ";
                            Send = Send + "                " + PrintTaxRow + LF;

                        }
                        PosPrinter.Print(Send);
                        #region old code to display tax
                        //if (DisCountAmnt > 0)
                        //{
                        //PosPrinter.Print("       DISCOUNT @" + float.Parse(DiscountPct).ToString("N2") + "% :    " + DisCountAmnt.ToString("N2") + "\n");
                        //PosPrinter.Print("         DISCOUNT AMOUNT   :    " + DisCountAmnt.ToString("N2") + "\n");
                        //}
                        //string VatPCT = clsConfigSettings.TaxCap.ToString();
                        //string ServiceTaxPCT = ConfigurationSettings.AppSettings["Service_Tax"].ToString();
                        //string ServiceTaxPCT = clsConfigSettings.ServiceTax;
                        //for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        // {
                        // PosPrinter.Print("         " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        //}
                        //double ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Tax"].ToString());
                        //double RoundOff = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["RoundOff"].ToString());
                        //if (ServiceTax > 0)
                        // PosPrinter.Print(" SERVICE TAX   @" + ServiceTaxPCT + "%       :    " + ServiceTax.ToString("N2") + "\n");
                        // if (SBC_Tax > 0)
                        //    PosPrinter.Print("         SBC   @" + clsConfigSettings.SBC_Rate + "%       :    " + SBC_Tax.ToString("N2") + "\n");
                        // if (KKC_Tax > 0)
                        //  PosPrinter.Print("         KKC   @" + clsConfigSettings.KKC_Rate + "%       :    " + KKC_Tax.ToString("N2") + "\n");


                        //double Surcharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["sur_charge"].ToString());
                        // if (Surcharge > 0)
                        //{
                        //  PosPrinter.Print(" SURCHARGE ON VAT@" + clsConfigSettings.SurCharge + "%       :    " + Surcharge.ToString("N2") + "\n");
                        //}
                        //PosPrinter.Print("              R. OFF       :    " + RoundOff.ToString("N2") + "\n");
                        #endregion

                        PosPrinter.Print("----------------------------------------------\n");
                        PosPrinter.Print("         PAYABLE AMOUNT     :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                        PosPrinter.Print("----------------------------------------------\n");
                        TockenNo_locall = dttbl_Bill.Rows[0]["TockenNo"].ToString();
                        //PosPrinter.Print("\n");
                        if (Remarks.Length > 1)
                        {
                            PosPrinter.Print("Remarks : " + Remarks + "\n");
                            PosPrinter.Print("------------------------------------------\n");
                        }
                        if (dtDiscount != null)
                        {
                            for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                            {
                                string Category = dtDiscount.Rows[discount]["Category"].ToString();
                                string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                                PosPrinter.Print(Category + " : " + Dis_Amount + "\n");

                            }
                            if (dtDiscount.Rows.Count > 0)
                                PosPrinter.Print("------------------------------------------\n");
                        }
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        Send = Send + "Order No : " + OrderNo + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send + "  \n");

                        PosPrinter.Print(TenderAmount + "\n");
                        PosPrinter.Print(BalanceAmount + "\n");
                        if (mode.Length > 0)
                        {
                            PosPrinter.Print(mode + "\n");
                        }

                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + Program.Footer1 + LF;
                        Send = Send + Program.Footer2 + LF;
                        Send = Send + Program.Footer3 + LF;
                        Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send);
                        PosPrinter.Print(PaperFullCut);

                        PosPrinter.Close();
                    }
                }
                else
                {
                    string BillDate = BillDate_DateTime.ToString("dd-MM-yyyy");
                    for (int PrintIndex = 0; PrintIndex < Noof_bill_TA; PrintIndex++)
                    {
                        Send = string.Empty;
                        string Bill_Type = "Take Away";
                        if (BillType == "D")
                            Bill_Type = "Dine-In";
                        string PayMode = string.Empty;
                        string CardName = string.Empty;
                        clsBLL.GetPaymentMode(BillNo, out PayMode, out CardName);
                        string mode = "Payment Mode : " + PayMode;
                        if (PayMode.Length == 0)
                        {
                            mode = string.Empty;
                        }
                        //===============================Print bill in notepad=========================
                        PosPrinter.Open("testDoc1", CurrentPrinter);
                        Send = ESC + "@";
                        //set alllinment of line center**********************************************************************
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 16 + 16 + 1);          //1 - font B; 32 = Double width                  
                        Send = Send + Program.CompanyName + LF + "\n";
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + header.ToUpper() + LF;
                        //Send = Send + Program.Outlet_name + LF;
                        Send = Send + ESC + "!" + (char)(08 + 0.5);

                        //1 - font B; 32 = Double width 
                        string[] addr = Program.Address.Split(',');
                        string address1 = string.Empty;
                        string address2 = string.Empty;
                        string address3 = string.Empty;
                        int Addresscount = addr.Length;
                        int count = 0;
                        if (Addresscount > 1)
                            count = 1;

                        for (int add = 0; add <= count; add++)
                        {
                            address1 += addr[add] + ",";
                        }

                        if (Addresscount > 3)
                            count = 3;
                        else
                            count = 2;
                        if (Addresscount > 2)
                        {
                            for (int add = 2; add <= count; add++)
                            {
                                address2 += addr[add] + ",";
                            }
                        }

                        if (Addresscount > 3)
                        {
                            for (int add = 4; add < addr.Length; add++)
                            {
                                address3 += addr[add] + ",";
                            }
                        }
                        if (address3 != string.Empty)
                        {
                            address3 = address3.Remove(address3.Length - 1);
                        }
                        if (address2 != string.Empty)
                        {
                            address2 = address2.Remove(address2.Length - 1);
                        }
                        //if (address2 == string.Empty)
                        address1 = address1.Remove(address1.Length - 1);
                        int Printcount = 0;
                        addr1 = address1.Split(',');
                        if (address1.Length > 0)
                            foreach (string addr1_1 in addr1)
                            {
                                Printcount++;
                                if (Printcount == addr1.Length)
                                    Send = Send + addr1_1.Trim() + LF;
                                else
                                    Send = Send + addr1_1.Trim() + "," + LF;
                            }
                        addr2 = address2.Split(',');
                        Printcount = 0;
                        if (address2.Length > 0)
                            foreach (string addr2_2 in addr2)
                            {
                                Printcount++;
                                if (Printcount == addr2.Length)
                                    Send = Send + addr2_2.Trim() + LF;
                                else
                                    Send = Send + addr2_2.Trim() + "," + LF;
                            }
                        Printcount = 0;
                        addr3 = address3.Split(',');
                        if (address3.Length > 0)
                            foreach (string addr3_3 in addr3)
                            {
                                Printcount++;
                                if (Printcount == addr3.Length)
                                    Send = Send + addr3_3.Trim() + LF;
                                else
                                    Send = Send + addr3_3.Trim() + "," + LF;
                            }
                        if (Program.PhoneNo.Trim().Length > 0)
                            Send = Send + Program.PhoneNo + LF;
                        if (Program.TinNo.Trim().Length > 0)
                            Send = Send + Program.TinNo + LF;
                        if (Program.STax_No.Trim().Length > 0)
                            Send = Send + Program.STax_No + LF;
                        if (Program.cin_no.Trim().Length > 0)
                            Send = Send + Program.cin_no + LF;
                        if (Program.fssai_no.Length > 0)
                            Send = Send + Program.fssai_no + LF;


                        Send = Send + ESC + "!" + (char)(31 + 8 + 1);
                        Send = Send + clsConfigSettings.Bill_Head + LF;
                        Send = Send + ESC + "!" + (char)(8);

                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print(Send);
                        if (TableNo != "")
                            PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "        Table No : " + TableNo + "\n");
                        else if (OrderNo > 0)
                            PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "        Order No : " + OrderNo.ToString() + "\n");
                        else
                            PosPrinter.Print("\nBILL NO  :" + dttbl_Bill.Rows[0]["Bill_No"].ToString() + "\n");

                        //PosPrinter.Print("DATE   :" + Program.DayEnd_BIllingDate + "    Cover    :" + TableCover + "\n");
                        PosPrinter.Print("DATE     :" + BillDate + "\n");
                        PosPrinter.Print("TIME     :" + BillTime + "\n");
                        PosPrinter.Print("BILL TYPE:" + Bill_Type + "\n");
                        PosPrinter.Print("CASHIER  :" + Stwd + "\n");
                        if (dtHD.Rows.Count > 0)
                        {
                            PosPrinter.Print("------------------------------------------------\n");
                            if (dtHD.Rows[0]["Name"].ToString().Length > 0)
                                PosPrinter.Print("Name     :" + dtHD.Rows[0]["Name"].ToString() + "\n");
                            PosPrinter.Print("Mobile No:" + dtHD.Rows[0]["Mobile_No"].ToString() + "\n");
                            string Tin_no = dtHD.Rows[0]["Tin_no"].ToString().Trim();
                            if (Tin_no.Trim().Length > 1)
                                PosPrinter.Print("GSTIN    :" + Tin_no + "\n");
                            //PosPrinter.Print("Address  :" + dtHD.Rows[0]["Address"].ToString() + "\n");
                        }
                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("DESCRIPTION           QTY   RATE   AMOUNT ");
                        PosPrinter.Print("\n------------------------------------------\n");
                        //dttbl_assorted_item
                        Bill_Amount = 0;
                        for (int ItemIdex = 0; ItemIdex < dttbl_Bill_tran.Rows.Count; ++ItemIdex)
                        {
                            string PrintRow_Assorted = string.Empty;
                            string PrintRow = string.Empty;
                            string ItemName = dttbl_Bill_tran.Rows[ItemIdex]["I_Name"].ToString();
                            string HSNCODE = dttbl_Bill_tran.Rows[ItemIdex]["HSNCODE"].ToString();
                            string item_code = dttbl_Bill_tran.Rows[ItemIdex]["item_code"].ToString();
                            string isdeal = dttbl_Bill_tran.Rows[ItemIdex]["isdeal"].ToString();
                            DataTable dtItems = GetAssortedItem(Convert.ToInt64(item_code), dttbl_assorted_item, isdeal);
                            if (ItemName.Length <= 21)
                            {
                                for (int cn = ItemName.Length; cn <= 21; ++cn)
                                    ItemName += " ";
                            }

                            PrintRow = "" + ItemName + " " +
                               dttbl_Bill_tran.Rows[ItemIdex]["Qty"].ToString() + "   " + double.Parse(dttbl_Bill_tran.Rows[ItemIdex]["Rate"].ToString()).ToString("N2") + "  " + double.Parse(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString()).ToString("N2") + "\n";
                            PosPrinter.Print(PrintRow);
                            if (clsConfigSettings.Print_hsn == "1" && HSNCODE.Length > 0)
                                PosPrinter.Print("HSN: " + HSNCODE + "\n");
                            for (int Index = 0; Index < dtItems.Rows.Count; ++Index)
                            {
                                double DealitemRate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                double DealitemAmnt = Convert.ToDouble(dtItems.Rows[Index]["Amount"].ToString());
                                string RateWitAmnt = DealitemRate.ToString("N2") + "  " + DealitemAmnt.ToString("N2");
                                //if (DealitemRate == 0)
                                RateWitAmnt = "0.00";
                                PrintRow_Assorted = string.Empty;
                                string i_name = dtItems.Rows[Index]["I_Name"].ToString();
                                if (i_name.Length <= 21)
                                {
                                    for (int cn = i_name.Length; cn <= 21; ++cn)
                                        i_name += " ";
                                }
                                double I_Rate = Convert.ToDouble(dtItems.Rows[Index]["Rate"].ToString());
                                string str_rate = "";
                                if (clsConfigSettings.PrintComboRate == "1")
                                {
                                    if (I_Rate > 0)
                                        str_rate = I_Rate.ToString("N2");
                                }
                                PrintRow_Assorted = "" + i_name + " " +
                                //double.Parse(dtItems.Rows[Index]["Qty"].ToString()).ToString("N2") + " " + double.Parse(dtItems.Rows[Index]["rate"].ToString()).ToString("N2") + "  " + RateWitAmnt + "\n";
                                //PosPrinter.Print(PrintRow_Assorted);
                                dtItems.Rows[Index]["Qty"].ToString() + "   " + str_rate + "\n";
                                PosPrinter.Print(PrintRow_Assorted);
                            }
                            Bill_Amount += Convert.ToDouble(dttbl_Bill_tran.Rows[ItemIdex]["Amount"].ToString());
                        }
                        DiscountPct = dttbl_Bill.Rows[0]["bill_discountpct"].ToString();
                        PosPrinter.Print("------------------------------------------");
                        PosPrinter.Print("\n             BILL AMOUNT   :   " + Bill_Amount.ToString("N2") + "\n");

                        for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        {
                            PosPrinter.Print("                 " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        }
                        #region old code to display tax
                        //if (DisCountAmnt > 0)
                        //{
                        //PosPrinter.Print("       DISCOUNT @" + float.Parse(DiscountPct).ToString("N2") + "% :    " + DisCountAmnt.ToString("N2") + "\n");
                        //PosPrinter.Print("         DISCOUNT AMOUNT   :    " + DisCountAmnt.ToString("N2") + "\n");
                        //}
                        //string VatPCT = clsConfigSettings.TaxCap.ToString();
                        //string ServiceTaxPCT = ConfigurationSettings.AppSettings["Service_Tax"].ToString();
                        //string ServiceTaxPCT = clsConfigSettings.ServiceTax;
                        //for (int a = 0; a < dttbl_tax.Rows.Count; a++)
                        // {
                        // PosPrinter.Print("         " + dttbl_tax.Rows[a]["tax"].ToString() + "   " + Convert.ToDouble(dttbl_tax.Rows[a]["Tax_Amount"].ToString()).ToString("N2") + "\n");
                        //}
                        //double ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Tax"].ToString());
                        //double RoundOff = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["RoundOff"].ToString());
                        //if (ServiceTax > 0)
                        // PosPrinter.Print(" SERVICE TAX   @" + ServiceTaxPCT + "%       :    " + ServiceTax.ToString("N2") + "\n");
                        // if (SBC_Tax > 0)
                        //    PosPrinter.Print("         SBC   @" + clsConfigSettings.SBC_Rate + "%       :    " + SBC_Tax.ToString("N2") + "\n");
                        // if (KKC_Tax > 0)
                        //  PosPrinter.Print("         KKC   @" + clsConfigSettings.KKC_Rate + "%       :    " + KKC_Tax.ToString("N2") + "\n");


                        //double Surcharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["sur_charge"].ToString());
                        // if (Surcharge > 0)
                        //{
                        //  PosPrinter.Print(" SURCHARGE ON VAT@" + clsConfigSettings.SurCharge + "%       :    " + Surcharge.ToString("N2") + "\n");
                        //}
                        //PosPrinter.Print("              R. OFF       :    " + RoundOff.ToString("N2") + "\n");
                        #endregion

                        PosPrinter.Print("------------------------------------------\n");
                        PosPrinter.Print("         PAYABLE AMOUNT    :     " + Convert.ToDouble(dttbl_Bill.Rows[0]["Bill_Amount"].ToString()).ToString("N2") + "\n");
                        PosPrinter.Print("------------------------------------------\n");
                        TockenNo_locall = dttbl_Bill.Rows[0]["TockenNo"].ToString();
                        //PosPrinter.Print("\n");
                        if (Remarks.Length > 1)
                        {
                            PosPrinter.Print("Remarks : " + Remarks + "\n");
                            PosPrinter.Print("------------------------------------------\n");
                        }
                        if (dtDiscount != null)
                        {
                            for (int discount = 0; discount < dtDiscount.Rows.Count; discount++)
                            {
                                string Category = dtDiscount.Rows[discount]["Category"].ToString();
                                string Dis_Amount = dtDiscount.Rows[discount]["Dis_Amount"].ToString();
                                PosPrinter.Print(Category + " : " + Dis_Amount + "\n");

                            }
                            if (dtDiscount.Rows.Count > 0)
                                PosPrinter.Print("------------------------------------------\n");
                        }
                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(16 + 32 + 1);
                        Send = Send + "Order No : " + OrderNo + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send + "  \n");

                        PosPrinter.Print(TenderAmount + "\n");
                        PosPrinter.Print(BalanceAmount + "\n");
                        if (mode.Length > 0)
                        {
                            PosPrinter.Print(mode + "\n");
                        }

                        Send = "";
                        Send = ESC + "@";
                        Send = Send + ESC + "a" + (char)(1);
                        Send = Send + ESC + "!" + (char)(08 + 0.9);
                        Send = Send + Program.Footer1 + LF;
                        Send = Send + Program.Footer2 + LF;
                        Send = Send + Program.Footer3 + LF;
                        Send = Send + clsConfigSettings.Print_poweredby + LF + LF + LF + LF + LF + LF;
                        Send = Send + ESC + "!" + (char)(8);
                        Send = Send + ESC + "a" + (char)(0);
                        PosPrinter.Print("\n" + Send);
                        PosPrinter.Print(PaperFullCut);

                        PosPrinter.Close();
                    }
                }
                //=============================================================================
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            OrderComment_out = OrderComment;
            Token_No = TockenNo_locall;
        }

        public async Task PrintBillHomeDelivery_AssortedQty(string Bill_no, string Cust_Code, int NoofPrint, string fin_year, Action<string> setTokenCallback, string BillType, string is_instant_order)
        {
            await Task.Run(() =>
            {
                string newToken = string.Empty;
                try
                {
                    if (is_instant_order == "1")
                    {
                        PrintBillHomeDelivery(Bill_no, Cust_Code, NoofPrint, fin_year, out newToken, BillType);
                    }
                    else
                    {
                        PrintBillHomeDelivery_Crystal_AssortedQty(Bill_no, Cust_Code, NoofPrint, fin_year, out newToken, BillType);
                    }
                }
                catch (Exception ex)
                {
                    // Log the error
                }
                setTokenCallback?.Invoke(newToken); // safely pass token back to UI
            });
        }

        public void PrintBillHomeDelivery_Crystal_AssortedQty(string Bill_no, string Cust_Code, int NoofPrint, string fin_year, out string NewTokenNo_fk, string BillType)
        {
            DataTable dtCustInfo = new DataTable();
            DataTable dtdiscount = new DataTable();
            string NewTokenNo = string.Empty;
            try
            {
                ADOC objADOC = new ADOC();
                //PrinterSettings settings = new PrinterSettings();
                //string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
                //string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
                //LPrinter PosPrinter = new LPrinter();
                //string CurrentPrinter = settings.PrinterName;
                string billTypeReportDisplay = string.Empty;
                ds_HouchScreenReport dsobjreport = new ds_HouchScreenReport();
                DataSet dsFinalreport = new DataSet();
                string SqlQuerry = string.Empty;

                billTypeReportDisplay = "Home Delivery";
                switch (BillType.ToUpper())
                {
                    case "C":
                        billTypeReportDisplay = "Take Away";
                        break;
                    case "D":
                        billTypeReportDisplay = "Dine In";
                        break;
                    case "H":
                        billTypeReportDisplay = "Home Delivery";
                        break;
                }
                SqlQuerry = "[Usp_GetRetailInvoice_Print_HD_New] @BillNo='" + Bill_no + "',@Cust_Code='" + Cust_Code + "'";
                if (fin_year != clsConfigSettings.fin_year)
                {
                    SqlQuerry = "Usp_GetRetailInvoice_Print_HD_fin_year @BillNo='" + Bill_no + "',@Cust_Code='" + Cust_Code + "',@fin_year='" + fin_year + "'";
                }

                dsFinalreport = objADOC.FillReportDataSet(SqlQuerry, dsobjreport, "tbl_Bill", "tbl_Bill_Tran", "TBL_CustomerInfo", "tbl_tax", "tbl_discount");
                dtCustInfo = dsFinalreport.Tables["TBL_CustomerInfo"];
                dtdiscount = dsFinalreport.Tables["tbl_discount"];
                rptInvoice_HD_NO_bolt rptInvoiceobj = new rptInvoice_HD_NO_bolt();
                using (ReportViewer rvobj = new ReportViewer())
                {
                    rptInvoiceobj.SetDataSource(dsFinalreport);
                    rvobj.cRViewer.ReportSource = rptInvoiceobj;
                    double RoundOff = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["RoundOff"].ToString());
                    string paymentMode = dsFinalreport.Tables["tbl_Bill"].Rows[0]["Payment_Mode"].ToString();
                    string web_order_comments = dsFinalreport.Tables["tbl_Bill"].Rows[0]["web_order_comments"].ToString();
                    NewTokenNo = Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["OrderNo"].ToString());
                    string IncludedTax = "**Included Tax @12.5% :";
                    double DeliveryCharge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Other_Charge"].ToString());
                    if (DeliveryCharge > 0)
                    {
                        string Delivery_Charge = "Delivery Charge : ";
                        rptInvoiceobj.DataDefinition.FormulaFields["DeliveryCharge"].Text = "'" + Delivery_Charge + "    " + DeliveryCharge.ToString("N2") + "'";
                    }
                    rptInvoiceobj.DataDefinition.FormulaFields["C_name"].Text = "'" + dsFinalreport.Tables[0].Rows[0]["cashier"].ToString() + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["RoundOff"].Text = "'" + RoundOff.ToString("N2") + "'";
                    string PayMode = string.Empty;
                    string CardName = string.Empty;
                    // rptInvoiceobj.DataDefinition.FormulaFields["PaymentMode"].Text = "'" + mode + "'";

                    string DisCountCap = dsFinalreport.Tables[0].Rows[0]["Dis_Amount"].ToString();
                    NewTokenNo = dsFinalreport.Tables[0].Rows[0]["TockenNo"].ToString();
                    rptInvoiceobj.DataDefinition.FormulaFields["TockenNo"].Text = "'" + NewTokenNo + "'";
                    double Bill_Amount = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["Bill_Amount"].ToString());
                    double Taxamnt = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["tax1"].ToString());
                    string TaxCap = clsConfigSettings.TaxCap.ToString();
                    if ((Taxamnt) > 0)
                    {
                        if (Taxamnt > 0)
                        {
                            //IncludedTax = "*VAT @ 12.5% : ";
                            rptInvoiceobj.DataDefinition.FormulaFields["IncludedTax"].Text = "'" + TaxCap + "    " + Taxamnt.ToString("N2") + "'";
                        }
                        else
                        {
                            IncludedTax = TaxCap + " Included ** ";
                            rptInvoiceobj.DataDefinition.FormulaFields["IncludedTax"].Text = "'" + IncludedTax + "'";
                        }
                    }
                    double ServiceTax = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Service_Charge_Amount"].ToString());
                    if (ServiceTax > 0)
                    {
                        string ServiceTaxCap = "Service Tax @" + clsConfigSettings.ServiceTax + "% :     ";
                        string STax = ServiceTaxCap + ServiceTax.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["ServiceTax"].Text = "'" + STax + "'";
                    }
                    double SBC_Tax = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["SBC_Tax"].ToString());
                    double KKC_Tax = Convert.ToDouble(dsFinalreport.Tables["tbl_Bill"].Rows[0]["KKC_Tax"].ToString());
                    if (SBC_Tax < 0)
                    {
                        string sbcamnt = "Service Tax @" + clsConfigSettings.SBC_Rate + "% :     ";
                        string STax = sbcamnt + SBC_Tax.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["sbc"].Text = "'" + STax + "'";
                    }
                    if (KKC_Tax < 0)
                    {
                        string sbcamnt = "Service Tax @" + clsConfigSettings.SBC_Rate + "% :     ";
                        string STax = sbcamnt + SBC_Tax.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["kkc"].Text = "'" + STax + "'";
                    }
                    DataTable dtTran = dsFinalreport.Tables["tbl_Bill_Tran"];
                    double BillAmnt = 0.0;
                    for (int i = 0; i < dtTran.Rows.Count; i++)
                    {
                        BillAmnt += double.Parse(dtTran.Rows[i]["Amount"].ToString());
                    }
                    double DisAmnt = double.Parse(DisCountCap);
                    double Sur_Charge = Convert.ToDouble(dsFinalreport.Tables[0].Rows[0]["Sur_Charge"].ToString());
                    if (Sur_Charge > 0)
                    {
                        string Sur_ChargeCap = "Sur Charge@" + clsConfigSettings.SurCharge + "% :  on vat   ";
                        string Sur_ch = Sur_ChargeCap + Sur_Charge.ToString("N2");
                        rptInvoiceobj.DataDefinition.FormulaFields["surcharge"].Text = "'" + Sur_ch + "'";
                    }
                    //if (DisAmnt > 0)
                    //{
                    //    double PCTAmount = DisAmnt / BillAmnt * 100;
                    //    //DisCountCap = "Discount @" + PCTAmount.ToString("N0") + "%" + ", " + " Amount :";
                    //    DisCountCap = "Discount Amount :";
                    //    rptInvoiceobj.DataDefinition.FormulaFields["Dis_Count"].Text = "'" + DisCountCap + "   " + DisAmnt.ToString("N2") + "'";
                    //}
                    //else
                    //{
                    //    DisCountCap = "";
                    //    rptInvoiceobj.DataDefinition.FormulaFields["Dis_Count"].Text = "'" + DisCountCap + "'";
                    //    rptInvoiceobj.DataDefinition.FormulaFields["DisAmnt"].Text = "'" + DisCountCap + "'";
                    //}


                    if (paymentMode == "5")
                        rptInvoiceobj.DataDefinition.FormulaFields["grandTotal"].Text = "'0.00'";
                    else
                        rptInvoiceobj.DataDefinition.FormulaFields["grandTotal"].Text = "'" + Bill_Amount.ToString("N2") + "'";


                    //-----------Start Heder--------------------//////////                              
                    rptInvoiceobj.DataDefinition.FormulaFields["H_Company_Name"].Text = "'" + Program.CompanyName + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["H_address"].Text = "'" + Program.Address + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["H_Tin_No"].Text = "'" + Program.TinNo + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["H_Phone_No"].Text = "'" + Program.PhoneNo + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["Footer1"].Text = "'" + Program.Footer1 + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["Footer2"].Text = "'" + Program.Footer2 + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["STax_No"].Text = "'" + Program.STax_No + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["Footer3"].Text = "'" + Program.Footer3 + "'";
                    //rptInvoiceobj.DataDefinition.FormulaFields["C_name"].Text = "'" + Program.UserName + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["BillType"].Text = "'" + billTypeReportDisplay + "'";

                    rptInvoiceobj.DataDefinition.FormulaFields["fssai_no"].Text = "'" + "" + Program.fssai_no + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["cin_no"].Text = "'" + "CIN No :" + Program.cin_no + "'";
                    string channel = Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["Channel"].ToString()) + "(" +
                         Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["Channel_Order_Id"].ToString()) + ")";

                    rptInvoiceobj.DataDefinition.FormulaFields["Print_poweredby"].Text = "'" + clsConfigSettings.Print_poweredby + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["web_order_comments"].Text = "'" + web_order_comments.ToString() + "'";


                    rptInvoiceobj.DataDefinition.FormulaFields["Initial"].Text = "'" + clsConfigSettings.Initial + Bill_no + "'";
                    //clsBLL.GetPaymentMode(Bill_no, out PayMode, out CardName);
                    PayMode = Convert.ToString(dsFinalreport.Tables["tbl_bill"].Rows[0]["PayMode"].ToString());
                    string mode = "Payment Mode : " + PayMode;
                    if (PayMode.Length == 0)
                    {
                        mode = string.Empty;
                    }
                    rptInvoiceobj.DataDefinition.FormulaFields["OrderSource"].Text = "'" + channel + "'";
                    rptInvoiceobj.DataDefinition.FormulaFields["PaymentMode"].Text = "'" + mode + "'";
                    if (NoofPrint == 2)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 3)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 4)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 5)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else if (NoofPrint == 6)
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    else
                    {
                        rptInvoiceobj.PrintToPrinter(1, true, 1, 4);
                    }
                    Program.TableNo = string.Empty;
                    //rvobj.Show();
                    rptInvoiceobj.Refresh();
                    rptInvoiceobj.Close();
                    rptInvoiceobj.Dispose();
                    AssortedItem.CancelAssortedItem();
                }
            }
            catch (Exception ex)
            {
            }
            NewTokenNo_fk = NewTokenNo;
        }

    }

}
