using HiraSweets.App_Code;
using LPrinterTest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TuchScreenApp1Jan2013.App_Code;

namespace KOTPrintUtility.App_Code
{
    public class QSRApp
    {
        public bool _TransactionIsonProgress = false;
        clsPrintKOT objKOT = null;
        ADOC objADOC = null;
        static DataTable dt_tblTemp_id = null;
        IOnineOrderAutoAccept objcls = null;
        ClsPrintBill objClsPrintBill = null;
        OrderStatusUpdateLog objodrStatus = null;
        public QSRApp()
        {
            dt_tblTemp_id = new DataTable();
            objcls = new OrderDetailOnline();
            objClsPrintBill = new ClsPrintBill();
            objADOC = new ADOC();
            objKOT = new clsPrintKOT();
            objodrStatus = new OrderStatusUpdateLog();
        }


        #region GetItem_ByIndex
        /// <summary>
        /// GetItem_ByIndex
        /// </summary>
        /// <param name="I_code_fk"></param>
        /// <param name="dt"></param>
        /// <param name="item_index"></param>
        /// <returns></returns>
        private DataTable GetItem_ByIndex(string I_code_fk, DataTable dt, string item_index)
        {
            DataTable dtTempData = new DataTable();
            try
            {
                var Query = from TempAssortedItem in dt.AsEnumerable()
                            where TempAssortedItem.Field<Int64>("i_code_fk") == Convert.ToInt64(I_code_fk) && TempAssortedItem.Field<String>("item_index") == item_index
                            select TempAssortedItem;
                if (Query.Count() > 0)
                {
                    dtTempData = Query.CopyToDataTable();
                }
            }
            catch { }
            return dtTempData;
        }
        #endregion


        #region GetdealNAssorted_item
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable GetdealNAssorted_item()
        {
            int count = 0;
            DataTable dtdeal = new DataTable();
            DataTable dtAssorted_Item_Detail = AssortedItem.objds.Tables["tbl_AssortedItem"];
            string[] columnNames = (from dc in dtAssorted_Item_Detail.Columns.Cast<DataColumn>()
                                    select dc.ColumnName).ToArray();
            for (int RoIndex = 0; RoIndex < columnNames.Length; RoIndex++)
            {
                count = 0;
                string ColName = columnNames[RoIndex].ToString();
                if (ColName == "order_type")
                {
                    count++;
                    break;
                }
            }
            if (count == 0)
            {
                dtAssorted_Item_Detail.Columns.Add("order_type");
                dtAssorted_Item_Detail.Columns.Add("IsDiscount");
                dtAssorted_Item_Detail.Columns.Add("Index_No");
                if (dtAssorted_Item_Detail.Columns.Contains("DishComment"))
                    count = 0;
                else
                    dtAssorted_Item_Detail.Columns.Add("DishComment");
                if (dtAssorted_Item_Detail.Columns.Contains("Amount"))
                    count = 0;
                else
                    dtAssorted_Item_Detail.Columns.Add("Amount");
            }
            dtdeal = clsDeal.objds.Tables["tbl_deals"];
            for (int i = 0; i < dtdeal.Rows.Count; i++)
            {
                string i_code_fk = dtdeal.Rows[i]["i_code_fk"].ToString();
                string i_code = dtdeal.Rows[i]["i_code"].ToString();
                string qty = dtdeal.Rows[i]["qty"].ToString();
                string itemindex = dtdeal.Rows[i]["itemindex"].ToString();
                string i_name = dtdeal.Rows[i]["i_name"].ToString();
                string IsDiscount = dtdeal.Rows[i]["IsDiscount"].ToString();
                string Amount = dtdeal.Rows[i]["Amount"].ToString();
                string Order_type = "D";
                dtAssorted_Item_Detail.Rows.Add(i_code_fk, i_code, qty, itemindex, i_name, Order_type, IsDiscount, "0", Amount);
            }
            DataTable dtItems = clsItems.objItems.Tables["tbl_Items"];
            for (int a = 0; a < dtItems.Rows.Count; a++)
            {
                string i_code_fk = dtItems.Rows[a]["I_code_fk"].ToString();
                string i_code = dtItems.Rows[a]["i_code"].ToString();
                string qty = dtItems.Rows[a]["qty"].ToString();
                string itemindex = dtItems.Rows[a]["Index"].ToString();
                string i_name = dtItems.Rows[a]["Item Name"].ToString();
                string IsDiscount = dtItems.Rows[a]["IsDiscount"].ToString();
                string i_Type = dtItems.Rows[a]["i_Type"].ToString();
                string Index_No = dtItems.Rows[a]["Index_No"].ToString();
                string DishComment = dtItems.Rows[a]["DishComment"].ToString();
                string Amount = dtItems.Rows[a]["Amount"].ToString();

                string Order_type = "I";
                dtAssorted_Item_Detail.Rows.Add(i_code_fk, i_code, qty, itemindex, i_name, Order_type, IsDiscount, Index_No, DishComment, Amount);
            }
            return dtAssorted_Item_Detail;
        }
        #endregion


        #region dtAssortedItem
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtAssortedItem"></param>
        /// <param name="deal"></param>
        /// <returns></returns>
        private double AddAssortedItem(DataTable dtAssortedItem, string deal, string Deal_ItemIndex, string Deal_Item)
        {
            double TotalAmount = 0;
            try
            {
                for (int a = 0; a < dtAssortedItem.Rows.Count; a++)
                {
                    string Assorted_Item = dtAssortedItem.Rows[a]["i_code_fk"].ToString();
                    string i_code = dtAssortedItem.Rows[a]["i_code"].ToString();
                    string qty = dtAssortedItem.Rows[a]["qty"].ToString();
                    string i_name = dtAssortedItem.Rows[a]["i_name"].ToString();
                    string IsDiscount = dtAssortedItem.Rows[a]["IsDiscount"].ToString();
                    double amount = Convert.ToDouble(dtAssortedItem.Rows[a]["Amount"].ToString());
                    Program.Assorted_ItemIndex = dtAssortedItem.Rows[a]["item_index"].ToString();
                    string TaxRate = dtAssortedItem.Rows[a]["TaxRate"].ToString();
                    //double rate=
                    TotalAmount += amount;
                    string Step_Name = dtAssortedItem.Rows[a]["Step_Name"].ToString();
                    string No_of_Item = dtAssortedItem.Rows[a]["No_of_Item"].ToString();
                    string Deal_TYpe = dtAssortedItem.Rows[a]["i_type"].ToString();
                    string i_type = dtAssortedItem.Rows[a]["Deal_TYpe"].ToString();
                    string Index_No = dtAssortedItem.Rows[a]["Index_No"].ToString();
                    string DishComment = dtAssortedItem.Rows[a]["DishComment"].ToString();
                    if (deal == "deal")
                        clsDeal.objds.Tables["tbl_deals"].Rows.Add(Assorted_Item, i_code, qty, Deal_ItemIndex, i_name, IsDiscount, amount.ToString("N2"));
                    else
                        //AssortedItem.objds.Tables["tbl_AssortedItem"].Rows.Add(Program.Assorted_Item, i_code, qty, Program.Assorted_ItemIndex, i_code);
                        clsItems.GetMathod.AddItemsinList(i_name, qty.ToString(), i_code, IsDiscount, amount.ToString(), Step_Name, No_of_Item, i_type, Deal_TYpe, Deal_ItemIndex, Deal_Item, TaxRate, Index_No, DishComment);
                }
            }
            catch { }
            return TotalAmount;
        }
        #endregion


        #region GetBillDetail
        /// <summary>
        /// GetBillDetail
        /// </summary>
        /// <param name="Bill_No"></param>
        /// <param name="dgvItemDetails"></param>
        private async Task GetBillDetailAsync(string Bill_No, string CustName, string custMobileNo, string ZomatoOrderNo, string Cust_code)
        {
            tbl_bill objBill = null;
            string Bill_TypeName_Test = string.Empty;
            try
            {
                dt_tblTemp_id = new DataTable();
                DataSet dsBillDetail = objcls.GetOrderDetail(Bill_No, Program.Outlet_id, Program.DayEnd_BIllingDate);
                if (dsBillDetail.Tables[0].Rows.Count > 0)
                {
                    Loging.Log(LogType.Information, "QSRApp.GetBillDetailAsync no of item=>  " + dsBillDetail.Tables[1].Rows.Count.ToString() + " ZomatoOrderNo " + ZomatoOrderNo);
                    string payment_mode = dsBillDetail.Tables[0].Rows[0]["payment_mode"].ToString();
                    objBill = new tbl_bill();
                    objBill.bill_no_WebOrder = Bill_No;
                    objBill.custCode = Cust_code;
                    Bill_TypeName_Test = dsBillDetail.Tables[0].Rows[0]["Type"].ToString();
                    objBill.Bill_TypeName = dsBillDetail.Tables[0].Rows[0]["Bill_Type"].ToString();
                    Program.BillType = objBill.Bill_TypeName;
                    objBill.ModifiedBill_No = dsBillDetail.Tables[0].Rows[0]["Bill_no"].ToString();
                    objBill.ModifiedAmount = dsBillDetail.Tables[0].Rows[0]["bill_amount"].ToString();
                    objBill.DiscountModifiedBy = dsBillDetail.Tables[0].Rows[0]["Discount_by"].ToString();
                    objBill.Modified_DiscountAmnt = dsBillDetail.Tables[0].Rows[0]["dis_amount"].ToString();
                    objBill.Modified_DiscountPct = dsBillDetail.Tables[0].Rows[0]["Bill_DiscountPct"].ToString();
                    objBill.Remarks = dsBillDetail.Tables[0].Rows[0]["Comments"].ToString().Trim();
                    objBill.Delivery_Charge = dsBillDetail.Tables[0].Rows[0]["Delivery_Charge"].ToString();

                    objBill.Channel = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["bill_punch_by"]);
                    objBill.Channel_Id = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["External_Source_id"]);
                    objBill.Delivery_By = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["DeliveryBy"]);
                    objBill.Aggr_dis_pct = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["dis_pct_Agg"]);
                    objBill.Aggr_dis_amount = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["Aggr_Discount"]);
                    objBill.Agg_Tax_Calculation = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["Agg_Tax_Calculation"]);
                    objBill.dis_Type = dsBillDetail.Tables[0].Rows[0]["dis_type"].ToString();
                    objBill.ZomatoOrderNo = ZomatoOrderNo;
                    objBill.CardType = dsBillDetail.Tables[0].Rows[0]["Online_Payment_Mode"].ToString();

                    if (dsBillDetail.Tables[0].Columns.Contains("is_instant_order"))
                    {
                        objBill.is_instant_order = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["is_instant_order"]);
                    }
                    else
                    {
                        objBill.is_instant_order = "0";
                    }

                    if (dsBillDetail.Tables[0].Columns.Contains("Card_Type_Settlement"))
                    {
                        objBill.Card_Type_Settlement = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["Card_Type_Settlement"]);
                    }
                    else
                    {
                        objBill.Card_Type_Settlement = "0";
                    }

                    string BType = objBill.Bill_TypeName;
                    if (BType.Trim() == "")
                    {
                        BType = objcls.GetBillType(Bill_No);
                    }
                    //Program.OnlineOrderNo = Bill_No;
                    DataTable dtTran = dsBillDetail.Tables[1];
                    //AssortedItem.CancelAssortedItem();
                    //clsDeal.GetMathod.CancelDealItem();
                    //clsItems.GetMathod.CancelItem();
                    //OrderDetailOnline.GenerateUniqIndex("", 1, dt_tblTemp_id);
                    DataTable dtAssortedItem = dsBillDetail.Tables[2];
                    DataTable dtCharges = dsBillDetail.Tables[3];
                    if (dtCharges != null)
                    {
                        foreach (DataRow dr in dtCharges.Rows)
                        {
                            DataRow drNew = PackingCharges.GetPackaingCharges.NewRow();
                            drNew["Zomato_order_id"] = dr["Zomato_order_id"];
                            drNew["Charge_name"] = dr["Charge_name"];
                            drNew["chargesValue"] = dr["chargesValue"];
                            drNew["chargesTotaltax"] = dr["chargesTotaltax"];
                            drNew["I_Code_fk"] = dr["I_Code_fk"];
                            drNew["chargesId"] = dr["chargesId"];
                            PackingCharges.AddCharges(drNew);
                        }
                    }

                    if (dtTran.Rows.Count > 0)
                    {
                        //Program.Discount_Pct = "0";
                        List<tbl_tran> objlst = new List<tbl_tran>();
                        for (int i = 0; i < dtTran.Rows.Count; i++)
                        {
                            tbl_tran trn = new tbl_tran();
                            trn.Assorted_Item = dtTran.Rows[i]["I_code"].ToString();
                            trn.qty = dtTran.Rows[i]["qty"].ToString();
                            trn.Rate = dtTran.Rows[i]["Rate"].ToString();
                            trn.I_Name = dtTran.Rows[i]["I_Name"].ToString();
                            trn.TaxRate = dtTran.Rows[i]["TaxRate"].ToString();
                            trn.IsTaxable = dtTran.Rows[i]["IsTaxable"].ToString();
                            trn.tax = Convert.ToString(dtTran.Rows[i]["Tax"].ToString());
                            trn.Discount = dtTran.Rows[i]["Discount"].ToString();
                            trn.dept = dtTran.Rows[i]["dept"].ToString();
                            trn.Group_Dish = dtTran.Rows[i]["Group_Dish"].ToString();
                            trn.service_tax = dtTran.Rows[i]["service_tax"].ToString();
                            trn.dis_rate = dtTran.Rows[i]["dis_rate"].ToString();
                            trn.dis_amount = dtTran.Rows[i]["dis_amount"].ToString();
                            //clsDeal.GetVariable.Deal_Item = trn.Assorted_Item;
                            //clsDeal.GetVariable.Deal_ItemIndex = trn.Group_Dish;

                            trn.amount = Convert.ToDouble(dtTran.Rows[i]["amout"].ToString());
                            //dgvItemDetails.Rows.Add(trn.I_Name.Trim(), trn.Rate, trn.qty, Amount.ToString("N2"), tax, dtTran.Rows[i]["I_code"].ToString(), dtTran.Rows[i]["Comments"].ToString(), "0", "0", TaxRate, IsTaxable, Discount, Group_Dish, dept, "0", service_tax, "0", dis_rate, dis_amount, "0", "0", "0", "0", "");
                            //double TotalRate = 0;
                            //if (dtAssortedItem.Rows.Count > 0)
                            //{
                            try
                            {
                                if (trn.dis_amount != "" && trn.dis_amount != "0")
                                {
                                    if (Convert.ToDouble(trn.dis_amount) > Convert.ToDouble(trn.amount))
                                    {
                                        trn.dis_amount = trn.amount.ToString();
                                        trn.dis_rate = (Convert.ToDouble(trn.dis_amount) / trn.amount * 100).ToString("N4");
                                    }
                                }
                            }
                            catch { }

                            objlst.Add(trn);
                        }
                        objBill.tran = objlst;
                        //DataTable dtdeal = GetdealNAssorted_item();
                        string SubTotal = string.Empty;
                        string NetTotal = string.Empty;
                        string TotalVAT = string.Empty;
                        string ServiceTax = string.Empty;
                        string Surcharge = string.Empty;
                        string DeliveryCharge = string.Empty;
                        string BillTotal = string.Empty;
                        string GrandTotal = string.Empty;
                        string Discount_Amount = "0";
                        string sbc = "0";
                        string kkc = "0";
                        clsCalculateBill.CalculateBillDetailQSR(objlst, out SubTotal, out NetTotal, out ServiceTax, out TotalVAT, out BillTotal, out GrandTotal,
                            out Surcharge, out DeliveryCharge, Convert.ToDecimal(objBill.Modified_DiscountPct), "1", out Discount_Amount, out sbc, out kkc, objBill.Bill_TypeName, objBill.Delivery_Charge);

                        if (Discount_Amount != "" && Discount_Amount != "0")
                        {
                            if (Convert.ToDouble(Discount_Amount) > Convert.ToDouble(GrandTotal))
                            {
                                objBill.Modified_DiscountAmnt = SubTotal;
                            }
                        }
                        double tamt = double.Parse(NetTotal.Replace(",", "").Trim()) + double.Parse(TotalVAT.Replace(",", "").Trim()) + Convert.ToDouble(DeliveryCharge.Replace(",", "").Trim());

                        double TotalAmount = Math.Round(tamt);
                        double ReplaceAmount = TotalAmount - tamt;          //Round off amount
                        string srBillNo = string.Empty;
                        string is_instant_order = "0";
                        if (objBill.is_instant_order.Length > 0)
                        {
                            is_instant_order = objBill.is_instant_order;
                        }
                        if (objBill.Channel.ToLower() == "website")
                        {

                        }
                        //commented by jatinder for auto settle of website order 

                        //if (objBill.Channel.ToLower() == "website")
                        //{
                        //	DataGridView ObjDgvPayment = new DataGridView();
                        //	clsTempData.SetDataGrid(ObjDgvPayment);
                        //}
                        //if (BType.ToUpper() != "C" && objBill.Channel.ToLower() != "website")
                        //{
                        //	if (objBill.Card_Type_Settlement.Length > 1)
                        //	{
                        //		bool IsAdded = AddPayment_Mode(payment_mode, payment_mode, objBill.Card_Type_Settlement, "", GrandTotal.Replace(",", "").Trim());
                        //	}
                        //}

                        if (objBill.Card_Type_Settlement.Length > 1)
                        {
                            bool IsAdded = AddPayment_Mode(payment_mode, payment_mode, objBill.Card_Type_Settlement, "", GrandTotal.Replace(",", "").Trim());
                        }


                        try
                        {
                            Loging.Log(LogType.Information, "QSRApp.GenerateBill_HD.start bill generation json " + JsonConvert.SerializeObject(objBill));
                        }
                        catch { }
                        //clsGenerateBillQSR objBillGenerate = new clsGenerateBillQSR();
                        bool Result = objcls.GenerateBill_HD("U", BType, Program.DayEnd_BIllingDate, GrandTotal, Discount_Amount, "0", "0", "0", payment_mode, "", objBill.tran.Count.ToString(), "", objBill.Remarks, "admin", TotalVAT, ReplaceAmount.ToString("N2"),
                              objBill.Channel, "0", "0", "0", "0", "", "", "0", Program.Outlet_id, NetTotal, objBill.Modified_DiscountPct, "0", "0", DeliveryCharge, objBill, out srBillNo, dtAssortedItem, "0", "0", "0", "0", "0", objBill.Remarks, "0", "0", objBill.is_instant_order);
                        Loging.Log(LogType.Information, "QSRApp.GenerateBill_HD.end Result " + Result.ToString());
                        if (Result == true)
                        {
                            if (objBill.Channel.ToLower() == "website")
                            {
                                clsUpdateOrder.UpdateBillStatusWithWebsiteWebAPI(ZomatoOrderNo.Replace("-", "").Trim().TrimEnd('-'), srBillNo, objBill.bill_no_WebOrder, EnumOrderStatus.Confirm);
                            }
                            string TockenNo = string.Empty;
                            string OrderComment = string.Empty;
                            string Print_No_of_HD_Bill = clsConfigSettings.Print_No_of_HD_Bill.ToString();
                            //if (objBill.Bill_TypeName != "C")
                            string PrintCap = clsConfigSettings.Print_Cap.ToString();


                            //if (is_instant_order == "1")
                            //{

                            //	objClsPrintBill.PrintBillHomeDelivery(srBillNo, Program.Cust_code, Convert.ToInt32(Print_No_of_HD_Bill), clsConfigSettings.fin_year, out TockenNo, BType);
                            //}
                            //else
                            //{
                            Loging.Log(LogType.Information, "QSRApp.PrintBillHomeDelivery_CrystalAsync.start bill printing bill no=> " + srBillNo);
                            if (PrintCap == "0")
                            {
                                ClsPrintBill.PrintBill_HD_TexFormat_new(srBillNo, "", "", "", out TockenNo, out OrderComment, clsConfigSettings.fin_year, Bill_TypeName_Test);
                            }
                            //Added by Yachna for New BillPrint
                            else if (PrintCap == "2")
                            {
                                objClsPrintBill.PrintBillHomeDelivery_AssortedQty(srBillNo, Program.Cust_code, Convert.ToInt32(Print_No_of_HD_Bill), clsConfigSettings.fin_year, token => { TockenNo = token; }, BType, is_instant_order);
                            }
                            else
                            {
                                objClsPrintBill.PrintBillHomeDelivery_CrystalAsync(srBillNo, Program.Cust_code, Convert.ToInt32(Print_No_of_HD_Bill), clsConfigSettings.fin_year, token => { TockenNo = token; }, BType, is_instant_order);
                            }
                            //}
                            Loging.Log(LogType.Information, "QSRApp.PrintBillHomeDelivery_CrystalAsync.end bill printing  bill no=> " + srBillNo);
                            //else
                            //	ClsPrintBill.PrintBill_TDT_TexFormat_New(srBillNo, "", "", "", out TockenNo, out OrderComment, clsConfigSettings.fin_year, Bill_TypeName_Test);

                            int NoofPrint_Kot = Convert.ToInt32(clsConfigSettings.NoofPrint_KOT_TA);
                            if (objBill.Bill_TypeName == "H")
                                NoofPrint_Kot = Convert.ToInt32(clsConfigSettings.NoofPrint_KOT_HD);
                            else if (objBill.Bill_TypeName == "D")
                                NoofPrint_Kot = Convert.ToInt32(clsConfigSettings.NoofPrint_KOT_DN);

                            string SingleKOT = clsConfigSettings.SingleKOT;

                            if (clsConfigSettings.PrintRemarksOnKot != "1")
                                OrderComment = "";

                            Loging.Log(LogType.Information, "QSRApp start kot print no of kot " + NoofPrint_Kot + "  bill no=> " + srBillNo);

                            if (SingleKOT != "1")
                            {
                                if (NoofPrint_Kot > 0)
                                {
                                    if (clsConfigSettings.isKOT_3or4_EnchPrint == "1")
                                        objKOT.PrintKOTBigFontBigSize(NoofPrint_Kot, custMobileNo.Trim(), CustName.Trim(), srBillNo, objlst, TockenNo, OrderComment);
                                    if (clsConfigSettings.isKOT_3or4_EnchPrint == "2")
                                        objKOT.PrintKOTSmallFontSmallSize("1", TockenNo, NoofPrint_Kot, custMobileNo, CustName, srBillNo, OrderComment, is_instant_order);
                                    else
                                        objKOT.PrintKOTSmallFontSmallSize("1", TockenNo, NoofPrint_Kot, custMobileNo, CustName, srBillNo, OrderComment, is_instant_order);
                                    Loging.Log(LogType.Information, "QSRApp end kot print  bill no=> " + srBillNo);
                                }
                            }
                            else
                            {
                                //Added by Yachna for Single Item Single qty Single KOT 
                                objKOT.PrintKOTSmallFontSmallSize_SeperateKot_WithoutHeaders("1", TockenNo, NoofPrint_Kot, custMobileNo, CustName, srBillNo, OrderComment);
                            }
                            Loging.Log(LogType.Information, "============================================================================================================");

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Loging.Log(LogType.Error, "GetBillDetailAsync  error " + ex.Message + " online bill_no " + Bill_No);
            }
            finally
            {
                objBill = new tbl_bill();
                dt_tblTemp_id = new DataTable();
                dt_tblTemp_id.Rows.Clear();
                AssortedItem.CancelAssortedItem();
                clsDeal.GetMathod.CancelDealItem();
                clsItems.GetMathod.CancelItem();
            }
        }

        #endregion


        #region AddPayment_Mode
        private bool AddPayment_Mode(string Payment_Mode_id, string Payment_mode, string CardType, string ModeValue, string Bill_Amount)
        {
            int Count = 0;
            bool IsAdded = false;
            try
            {
                string hold_id = "0";
                DataGridView objDGV = new DataGridView();
                objDGV.AllowUserToAddRows = false;

                if (Convert.ToDouble(Bill_Amount) > 0)
                {
                    clsTempData.InsertInGrid(objDGV, Payment_Mode_id, Payment_mode, CardType, Bill_Amount, ModeValue, hold_id);
                    Count++;
                }
                clsTempData.SetDataGrid(objDGV);
                IsAdded = true;
            }
            catch (Exception ex)
            {
                IsAdded = false;
            }
            return IsAdded;
        }
        #endregion


        #region 
        /// <summary>
        /// GetOrder GetOnlineOrder
        /// </summary>
        public void GetOrder(Label lblNoofOrder)
        {
            try
            {
                int count = 0;
                DataTable dtOrder = objcls.GetOnlineOrder();
                if (dtOrder != null)
                {
                    SetLableText("No of order in queue=> " + dtOrder.Rows.Count.ToString(), lblNoofOrder);
                    Loging.Log(LogType.Information, "QSRApp.GetOrder: no of order=>  " + dtOrder.Rows.Count.ToString());
                    foreach (DataRow dr in dtOrder.Rows)
                    {
                        #region customer info
                        count++;
                        string bill_no = dr["Bill_no"].ToString();
                        string bill_no_TabOrder = bill_no;
                        string Cust_MobileNo = dr["Mobile_no"].ToString().Replace("+", "").Trim();
                        string Cust_Name = dr["Cust_Name"].ToString();
                        string Address = dr["Address"].ToString();
                        string City = dr["City"].ToString();
                        string email_id = dr["email_id"].ToString().Replace("'", "");
                        string Location_Id = dr["Location_Id"].ToString();
                        string UserName = dr["UserName"].ToString();
                        string currentStatus = dr["currentStatus"].ToString();
                        string Comments = dr["Comments"].ToString().Replace("\r", " ").Trim();
                        Program.Source_of_order = dr["Order_Source_Id"].ToString();
                        string Payment_Mode = dr["Payment_Mode"].ToString();
                        string OrderSouce = dr["OrderSouce"].ToString();
                        string Land_mark = dr["Land_mark"].ToString();
                        string Location = dr["Location"].ToString();
                        string Flat_No = dr["Flat_No"].ToString();
                        string pin_no = dr["pin_no"].ToString();
                        string Longitude = dr["Longitude"].ToString();
                        string Company_Stateid = Program.Company_Stateid;
                        string Latitude = dr["Latitude"].ToString();
                        string OnlinePaymentMode = (Payment_Mode.Trim().Equals("1") ? "CASH" : "CARD");
                        Program.ZomatoOrderNo = dr["Zomato_order_id"].ToString();
                        string ZomatoOrderNo = dr["Zomato_order_id"].ToString();
                        string External_Source_id = dr["External_Source_id"].ToString();
                        string Channel_Id = dr["External_Source_id"].ToString();

                        string sqlqLocal = "usp_insert_Update_customer @Name='" + Cust_Name.Replace("'", " ") + "',@DOB='',@Address='" + Address.Replace("'", " ") +
                                  "',@Mobile_No='" + Cust_MobileNo + "',@Phone_No='" + Cust_MobileNo + "',@City='" + City.Replace("'", "") + "',@created_by='" + "Utility" +
                                  "',@Email_id='" + email_id + "',@Location='" + Location.Replace("'", " ") + "',@Isflag='0',@Remarks='',@location_id='" + Location_Id + "',@Colony_id='" + "1" + "'";

                        string cudt_codes = objADOC.GetObject.GetSingleResult(sqlqLocal);
                        Program.Cust_code = cudt_codes;

                        if (Program.Cust_code != "" && Program.Cust_code != "0")
                        {
                            GetBillDetailAsync(bill_no, Cust_Name, Cust_MobileNo, ZomatoOrderNo, cudt_codes);
                        }
                        SetLableText("No of order in queue=> " + dtOrder.Rows.Count.ToString() + " printed=> " + count.ToString(), lblNoofOrder);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Loging.Log(LogType.Error, "GetOrder error " + ex.Message);
            }
            finally
            {
                //objcls.GetOrderStatusLog();
            }
        }
        #endregion

        public void SetLableText(string text, Label lblOrder)
        {
            try
            {
                if (lblOrder.InvokeRequired)
                {
                    // Call this same 
                    Action safeWrite = delegate { SetLableText($"{text}", lblOrder); };
                    lblOrder.Invoke(safeWrite);
                }
                else
                    lblOrder.Text = text;

            }
            catch (Exception ex)
            {
                Loging.Log(LogType.Error, "WriteTextSafe error " + ex.Message);
            }
        }

        public bool CheckForInternet()
        {
            string Validate_IP = clsConfigSettings.validate_ip.ToString();
            bool IsRunning = false;
            Ping p = new Ping();
            try
            {
                //if (Validate_IP.Length > 0)
                //{
                //	PingReply reply = p.Send(Validate_IP, 3000);
                //	if (reply.Status == IPStatus.Success)
                //		IsRunning = true;
                //}
                //else
                //{
                IsRunning = true;
                //}
            }
            catch
            {
                IsRunning = false;
            }
            return IsRunning;
        }
        /// <summary>
        /// GetOrderAPI
        /// </summary>
        /// <param name="lblNoofOrder"></param>
        public async void GetOrderAPI(Label lblNoofOrder)
        {
            try
            {
                _TransactionIsonProgress = true;
                int count = 0;
                var dtOrder = await objcls.GetOnlineOrderFromApiAsync();
                if (dtOrder != null && dtOrder.status)
                {
                    //var dtOrder = ds.Tables[1];
                    SetLableText("No of order in queue=> " + dtOrder.bill.Count.ToString(), lblNoofOrder);
                    Loging.Log(LogType.Information, "QSRApp.GetOrder: no of order=>  " + dtOrder.bill.Count.ToString());
                    foreach (var dr in dtOrder.bill)
                    {
                        #region customer info
                        count++;
                        string bill_no = dr.customers.Bill_no_fk.ToString();
                        string bill_no_TabOrder = dr.customers.Bill_no_fk.ToString();
                        string Cust_MobileNo = dr.customers.Mobile_no.Replace("+", "").Trim();
                        string Cust_Name = dr.customers.Cust_Name;
                        string Address = dr.customers.Address;
                        string City = dr.customers.City;
                        string email_id = dr.customers.email_id.ToString().Replace("'", "");
                        string Location_Id = dr.customers.Location_Id;
                        string UserName = dr.customers.UserName;
                        string currentStatus = dr.customers.currentStatus;
                        string Comments = dr.customers.Comments.Replace("\r", " ").Trim();
                        Program.Source_of_order = dr.customers.Order_Source_Id;
                        string Payment_Mode = dr.customers.Payment_Mode;
                        string OrderSouce = dr.customers.OrderSouce;
                        string Land_mark = dr.customers.Land_mark;
                        string Location = dr.customers.Location;
                        string Flat_No = dr.customers.Flat_No;
                        string pin_no = dr.customers.pin_no;
                        string Longitude = dr.customers.Longitude;
                        string Company_Stateid = Program.Company_Stateid;
                        string Latitude = dr.customers.Latitude;
                        string OnlinePaymentMode = (Payment_Mode.Trim().Equals("1") ? "CASH" : "CARD");
                        Program.ZomatoOrderNo = dr.customers.Zomato_order_id;
                        string ZomatoOrderNo = dr.customers.Zomato_order_id;
                        string External_Source_id = dr.customers.External_Source_id;
                        string Channel_Id = dr.customers.External_Source_id;

                        string sqlqLocal = "usp_insert_Update_customer @Name='" + Cust_Name.Replace("'", " ") + "',@DOB='',@Address='" + Address.Replace("'", " ") +
                                  "',@Mobile_No='" + Cust_MobileNo + "',@Phone_No='" + Cust_MobileNo + "',@City='" + City.Replace("'", "") + "',@created_by='" + "Utility" +
                                  "',@Email_id='" + email_id + "',@Location='" + Location.Replace("'", " ") + "',@Isflag='0',@Remarks='',@location_id='" + Location_Id + "',@Colony_id='" + "1" + "'";

                        string cudt_codes = objADOC.GetObject.GetSingleResult(sqlqLocal);
                        Program.Cust_code = cudt_codes;

                        if (Program.Cust_code != "" && Program.Cust_code != "0")
                        {
                            GetBillDetailAsyncAPI(bill_no, Cust_Name, Cust_MobileNo, ZomatoOrderNo, cudt_codes, dr);
                        }
                        SetLableText("No of order in queue=> " + dtOrder.bill.Count.ToString() + " printed=> " + count.ToString(), lblNoofOrder);
                        #endregion
                    }

                    foreach (var dr in dtOrder.Cancelbills)
                    {
                        Loging.Log(LogType.Information, "QSRApp.GetOrder: (cancelled bills) =>  " + dtOrder.Cancelbills.Count.ToString());
                        string bill_no_local = dr.bill_no_local.ToString();
                        string zomato_order_id = dr.zomato_order_id.ToString();
                        string Online_bill_no = dr.Online_bill_no.ToString();
                        DataTable dtItemDetail = null;
                        var objadoc = new ADOC();
                        dtItemDetail = objadoc.GetTable("Usp_CheckValidBill_utility @bill_no_fk='" + bill_no_local.Trim() + "',@zomato_order_id='" + zomato_order_id.Trim() + "',@Online_bill_no='" + Online_bill_no.Trim() + "';");
                       
                        if (dtItemDetail != null && dtItemDetail.Rows.Count > 0)
                        {
                            // double GrandTotal = Convert.ToDouble(objadoc.GetSingleResult("select bill_amount from tbl_bill with (nolock) where bill_no='" + bill_no_local.Trim() + "'"));
                            string Pmt_mode = dtItemDetail.Rows[0]["Payment_Mode"].ToString();
                            double GrandTotal = Convert.ToDouble(dtItemDetail.Rows[0]["bill_amount"].ToString());                            
                            string Cancellation_Reason = dtItemDetail.Rows[0]["Cancellation_Reason"].ToString();

                            string BillVoidDescription = "Cancelled From Online";
                            string Channel = string.Empty;
                            Channel = dtItemDetail.Rows[0]["Channel"].ToString();

                            //string Web_order_no = "";
                            //if (dtItemDetail.Columns.Contains("Web_order_no"))
                            //    Web_order_no = dtItemDetail.Rows[0]["Web_order_no"].ToString();

                            string sqlCardquery = string.Empty;
                            var dgvDetail = new DataGridView();

                            var objCancel = new clsCancelBill();
                            string Made_Unmade = "";
                            bool IsBillCanceled = objCancel.CancelBill_Made_Unmade(bill_no_local.Trim(), BillVoidDescription, Program.CashierName, Program.DayEnd_BIllingDate, dgvDetail, Made_Unmade, Pmt_mode, sqlCardquery, Channel, Online_bill_no, zomato_order_id);
                          
                            if (IsBillCanceled == true)
                            {
                                string Online_sql = "[dbo].[Usp_Cancel_Online_Order] @Cancelled_by='" + Program.CashierName + "',@Bill_No='" + Online_bill_no.Trim()
                                    + "',@bill_no_local='" + bill_no_local.Trim() + "',@Zomato_order_id='" + zomato_order_id.Trim() + "';";

                                //changed by jatinder on 15-12-2025 to sync to clouddb connection
                                Synchronize_Sql_CloudDB_Connection.GetObject.ExecuteDML(Online_sql);
                                if(Cancellation_Reason.Length > 2)
                                {
                                }
                                else
                                {
                                    Loging.Log(LogType.Information, "QSRApp.PrintCancelBill. Start bill printing  bill no=> " + bill_no_local);
                                    PrintCancelBill(GrandTotal, bill_no_local);
                                    Loging.Log(LogType.Information, "QSRApp.PrintCancelBill. End bill printing  bill no=> " + bill_no_local);
                                }                             
                            }

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                _TransactionIsonProgress = false;
                Loging.Log(LogType.Error, "GetOrder error " + ex.Message);
            }
            finally
            {
                _TransactionIsonProgress = false;
            }
        }

        private async Task GetBillDetailAsyncAPI(string Bill_No, string CustName, string custMobileNo, string ZomatoOrderNo, string Cust_code, clsOrdersBill dsBillDetail)
        {
            tbl_bill objBill = null;
            string Bill_TypeName_Test = string.Empty;
            try
            {
                dt_tblTemp_id = new DataTable();
                if (dsBillDetail != null && dsBillDetail.items.Count > 0)
                {
                    Loging.Log(LogType.Information, "QSRApp.GetBillDetailAsync no of item=>  " + dsBillDetail.items.Count.ToString() + " ZomatoOrderNo " + ZomatoOrderNo);
                    string payment_mode = dsBillDetail.payment_mode;
                    dsBillDetail.Card_Type_Settlement = dsBillDetail.Card_Type_Settlement == null ? "" : dsBillDetail.Card_Type_Settlement;
                    objBill = new tbl_bill();
                    objBill.bill_no_WebOrder = Bill_No;
                    objBill.custCode = Cust_code;
                    Bill_TypeName_Test = dsBillDetail.Type;
                    objBill.Bill_TypeName = dsBillDetail.bill_Type;
                    Program.BillType = objBill.Bill_TypeName;
                    objBill.ModifiedBill_No = dsBillDetail.bill_no.ToString();
                    objBill.ModifiedAmount = dsBillDetail.bill_amount.ToString();
                    objBill.DiscountModifiedBy = dsBillDetail.Discount_by;
                    objBill.Modified_DiscountAmnt = dsBillDetail.dis_amount.ToString();
                    objBill.Modified_DiscountPct = dsBillDetail.Bill_DiscountPct.ToString();
                    objBill.Remarks = dsBillDetail.Comments.ToString();
                    objBill.Delivery_Charge = dsBillDetail.Delivery_Charge.ToString();

                    objBill.Channel = dsBillDetail.bill_punch_by;
                    objBill.Channel_Id = dsBillDetail.External_Source_id;
                    objBill.Delivery_By = dsBillDetail.DeliveryBy;
                    objBill.Aggr_dis_pct = dsBillDetail.dis_pct_Agg.ToString();
                    objBill.Aggr_dis_amount = dsBillDetail.Aggr_Discount.ToString();
                    objBill.Agg_Tax_Calculation = dsBillDetail.Agg_Tax_Calculation;
                    objBill.dis_Type = dsBillDetail.dis_type;
                    objBill.ZomatoOrderNo = ZomatoOrderNo;
                    objBill.CardType = dsBillDetail.Online_Payment_Mode;
                    objBill.is_instant_order = dsBillDetail.is_instant_order.ToString();
                    objBill.Card_Type_Settlement = Convert.ToString(dsBillDetail.Card_Type_Settlement);


                    string BType = objBill.Bill_TypeName;
                    if (BType.Trim() == "")
                    {
                        BType = objcls.GetBillType(Bill_No);
                    }
                    //Program.OnlineOrderNo = Bill_No;
                    //DataTable dtTran = dsBillDetail.Tables[1];
                    //AssortedItem.CancelAssortedItem();
                    //clsDeal.GetMathod.CancelDealItem();
                    //clsItems.GetMathod.CancelItem();
                    //OrderDetailOnline.GenerateUniqIndex("", 1, dt_tblTemp_id);
                    //DataTable dtAssortedItem = dsBillDetail.Tables[2];
                    //DataTable dtCharges = dsBillDetail.Tables[3];
                    if (dsBillDetail.charges != null)
                    {
                        foreach (var dr in dsBillDetail.charges)
                        {
                            DataRow drNew = PackingCharges.GetPackaingCharges.NewRow();
                            drNew["Zomato_order_id"] = dr.Zomato_order_id;
                            drNew["Charge_name"] = dr.Charge_name;
                            drNew["chargesValue"] = dr.chargesValue;
                            drNew["chargesTotaltax"] = dr.chargesTotaltax;
                            drNew["I_Code_fk"] = dr.I_Code_fk;
                            drNew["chargesId"] = dr.chargesId;
                            PackingCharges.AddCharges(drNew);
                        }
                    }

                    if (dsBillDetail.items.Count > 0)
                    {
                        //Program.Discount_Pct = "0";
                        List<tbl_tran> objlst = new List<tbl_tran>();
                        for (int i = 0; i < dsBillDetail.items.Count; i++)
                        {
                            tbl_tran trn = new tbl_tran();
                            trn.Assorted_Item = dsBillDetail.items[i].I_code;
                            trn.qty = dsBillDetail.items[i].qty.ToString();
                            trn.Rate = dsBillDetail.items[i].Rate.ToString();
                            trn.I_Name = dsBillDetail.items[i].i_name;
                            trn.TaxRate = dsBillDetail.items[i].TaxRate.ToString();
                            trn.IsTaxable = dsBillDetail.items[i].IsTaxable.ToString();
                            trn.tax = Convert.ToString(dsBillDetail.items[i].Tax.ToString());
                            trn.Discount = dsBillDetail.items[i].Discount;
                            trn.dept = dsBillDetail.items[i].dept;
                            trn.Group_Dish = dsBillDetail.items[i].Group_Dish;
                            trn.service_tax = dsBillDetail.items[i].service_tax;
                            trn.dis_rate = dsBillDetail.items[i].dis_rate.ToString();
                            trn.dis_amount = dsBillDetail.items[i].dis_amount.ToString();
                            //clsDeal.GetVariable.Deal_Item = trn.Assorted_Item;
                            //clsDeal.GetVariable.Deal_ItemIndex = trn.Group_Dish;

                            trn.amount = Convert.ToDouble(dsBillDetail.items[i].amout.ToString());
                            //dgvItemDetails.Rows.Add(trn.I_Name.Trim(), trn.Rate, trn.qty, Amount.ToString("N2"), tax, dtTran.Rows[i]["I_code"].ToString(), dtTran.Rows[i]["Comments"].ToString(), "0", "0", TaxRate, IsTaxable, Discount, Group_Dish, dept, "0", service_tax, "0", dis_rate, dis_amount, "0", "0", "0", "0", "");
                            //double TotalRate = 0;
                            //if (dtAssortedItem.Rows.Count > 0)
                            //{
                            try
                            {
                                if (trn.dis_amount != "" && trn.dis_amount != "0")
                                {
                                    if (Convert.ToDouble(trn.dis_amount) > Convert.ToDouble(trn.amount))
                                    {
                                        trn.dis_amount = trn.amount.ToString();
                                        trn.dis_rate = (Convert.ToDouble(trn.dis_amount) / trn.amount * 100).ToString("N4");
                                    }
                                }
                            }
                            catch { }

                            objlst.Add(trn);
                        }
                        objBill.tran = objlst;
                        //DataTable dtdeal = GetdealNAssorted_item();
                        string SubTotal = string.Empty;
                        string NetTotal = string.Empty;
                        string TotalVAT = string.Empty;
                        string ServiceTax = string.Empty;
                        string Surcharge = string.Empty;
                        string DeliveryCharge = string.Empty;
                        string BillTotal = string.Empty;
                        string GrandTotal = string.Empty;
                        string Discount_Amount = "0";
                        string sbc = "0";
                        string kkc = "0";
                        clsCalculateBill.CalculateBillDetailQSR(objlst, out SubTotal, out NetTotal, out ServiceTax, out TotalVAT, out BillTotal, out GrandTotal,
                            out Surcharge, out DeliveryCharge, Convert.ToDecimal(objBill.Modified_DiscountPct), "1", out Discount_Amount, out sbc, out kkc, objBill.Bill_TypeName, objBill.Delivery_Charge);

                        if (Discount_Amount != "" && Discount_Amount != "0")
                        {
                            if (Convert.ToDouble(Discount_Amount) > Convert.ToDouble(GrandTotal))
                            {
                                objBill.Modified_DiscountAmnt = SubTotal;
                            }
                        }
                        double tamt = double.Parse(NetTotal.Replace(",", "").Trim()) + double.Parse(TotalVAT.Replace(",", "").Trim()) + Convert.ToDouble(DeliveryCharge.Replace(",", "").Trim());

                        double TotalAmount = Math.Round(tamt);
                        double ReplaceAmount = TotalAmount - tamt;          //Round off amount
                        string srBillNo = string.Empty;
                        string is_instant_order = "0";
                        if (objBill.is_instant_order.Length > 0)
                        {
                            is_instant_order = objBill.is_instant_order;
                        }
                        //commented by jatinder for auto settle of website order 
                        //if (objBill.Channel.ToLower() == "website")
                        //{
                        //	DataGridView ObjDgvPayment = new DataGridView();
                        //	clsTempData.SetDataGrid(ObjDgvPayment);
                        //}
                        //if (BType.ToUpper() != "C" && objBill.Channel.ToLower() != "website")
                        //{
                        //	if (objBill.Card_Type_Settlement.Length > 1)
                        //	{
                        //		bool IsAdded = AddPayment_Mode(payment_mode, payment_mode, objBill.Card_Type_Settlement, "", GrandTotal.Replace(",", "").Trim());
                        //	}
                        //}

                        if (objBill.Card_Type_Settlement.Length > 1)
                        {
                            bool IsAdded = AddPayment_Mode(payment_mode, payment_mode, objBill.Card_Type_Settlement, "", GrandTotal.Replace(",", "").Trim());
                        }


                        try
                        {
                            Loging.Log(LogType.Information, "QSRApp.GenerateBill_HD.start bill generation json " + JsonConvert.SerializeObject(objBill));
                        }
                        catch { }
                        BillRespose res = new BillRespose();

                        res = await objcls.GenerateBill_HDAPI(dsBillDetail, "U", BType, Program.DayEnd_BIllingDate, GrandTotal, Discount_Amount, "0", "0", "0", payment_mode, "", objBill.tran.Count.ToString(), "", objBill.Remarks, "admin", TotalVAT, ReplaceAmount.ToString("N2"),
                              objBill.Channel, "0", "0", "0", "0", "", "", "0", Program.Outlet_id, NetTotal, objBill.Modified_DiscountPct, "0", "0", DeliveryCharge, objBill, "0", "0", "0", "0", "0", objBill.Remarks, "0", "0", objBill.is_instant_order);
                        Loging.Log(LogType.Information, "QSRApp.GenerateBill_HD.end Result " + res.Result.ToString());
                        if (res.Result == true)
                        {
                            srBillNo = res.Bill_no;
                            if (objBill.Channel.ToLower() == "website")
                            {
                                clsUpdateOrder.UpdateBillStatusWithWebsiteWebAPI(ZomatoOrderNo.Replace("-", "").Trim().TrimEnd('-'), srBillNo, objBill.bill_no_WebOrder, EnumOrderStatus.Confirm);
                            }
                            string TockenNo = string.Empty;
                            string OrderComment = string.Empty;
                            string Print_No_of_HD_Bill = clsConfigSettings.Print_No_of_HD_Bill.ToString();
                            //if (objBill.Bill_TypeName != "C")
                            string PrintCap = clsConfigSettings.Print_Cap.ToString();


                            //if (is_instant_order == "1")
                            //{

                            //	objClsPrintBill.PrintBillHomeDelivery(srBillNo, Program.Cust_code, Convert.ToInt32(Print_No_of_HD_Bill), clsConfigSettings.fin_year, out TockenNo, BType);
                            //}
                            //else
                            //{
                            Loging.Log(LogType.Information, "QSRApp.PrintBillHomeDelivery_CrystalAsync.start bill printing bill no=> " + srBillNo);
                            if (PrintCap == "0")
                            {
                                ClsPrintBill.PrintBill_HD_TexFormat_new(srBillNo, "", "", "", out TockenNo, out OrderComment, clsConfigSettings.fin_year, Bill_TypeName_Test);
                            }
                            //Added by Yachna for New BillPrint
                            else if (PrintCap == "2")
                            {
                                objClsPrintBill.PrintBillHomeDelivery_AssortedQty(srBillNo, Program.Cust_code, Convert.ToInt32(Print_No_of_HD_Bill), clsConfigSettings.fin_year, token => { TockenNo = token; }, BType, is_instant_order);
                            }
                            else
                            {
                                objClsPrintBill.PrintBillHomeDelivery_CrystalAsync(srBillNo, Program.Cust_code, Convert.ToInt32(Print_No_of_HD_Bill), clsConfigSettings.fin_year, token => { TockenNo = token; }, BType, is_instant_order);
                            }
                            //}
                            Loging.Log(LogType.Information, "QSRApp.PrintBillHomeDelivery_CrystalAsync.end bill printing  bill no=> " + srBillNo);
                            //else
                            //	ClsPrintBill.PrintBill_TDT_TexFormat_New(srBillNo, "", "", "", out TockenNo, out OrderComment, clsConfigSettings.fin_year, Bill_TypeName_Test);

                            int NoofPrint_Kot = Convert.ToInt32(clsConfigSettings.NoofPrint_KOT_TA);
                            if (objBill.Bill_TypeName == "H")
                                NoofPrint_Kot = Convert.ToInt32(clsConfigSettings.NoofPrint_KOT_HD);
                            else if (objBill.Bill_TypeName == "D")
                                NoofPrint_Kot = Convert.ToInt32(clsConfigSettings.NoofPrint_KOT_DN);

                            string SingleKOT = clsConfigSettings.SingleKOT;

                            if (clsConfigSettings.PrintRemarksOnKot != "1")
                                OrderComment = "";

                            Loging.Log(LogType.Information, "QSRApp start kot print no of kot " + NoofPrint_Kot + "  bill no=> " + srBillNo);

                            if (SingleKOT != "1")
                            {
                                if (NoofPrint_Kot > 0)
                                {
                                    if (clsConfigSettings.isKOT_3or4_EnchPrint == "1")
                                        objKOT.PrintKOTBigFontBigSize(NoofPrint_Kot, custMobileNo.Trim(), CustName.Trim(), srBillNo, objlst, TockenNo, OrderComment);
                                    if (clsConfigSettings.isKOT_3or4_EnchPrint == "2")
                                        objKOT.PrintKOTSmallFontSmallSize("1", TockenNo, NoofPrint_Kot, custMobileNo, CustName, srBillNo, OrderComment, is_instant_order);
                                    else
                                        objKOT.PrintKOTSmallFontSmallSize("1", TockenNo, NoofPrint_Kot, custMobileNo, CustName, srBillNo, OrderComment, is_instant_order);
                                    Loging.Log(LogType.Information, "QSRApp end kot print  bill no=> " + srBillNo);
                                }
                            }
                            else
                            {
                                //Added by Yachna for Single Item Single qty Single KOT 
                                objKOT.PrintKOTSmallFontSmallSize_SeperateKot_WithoutHeaders("1", TockenNo, NoofPrint_Kot, custMobileNo, CustName, srBillNo, OrderComment);
                            }
                            Loging.Log(LogType.Information, "============================================================================================================");

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Loging.Log(LogType.Error, "GetBillDetailAsync  error " + ex.Message + " online bill_no " + Bill_No);
            }
            finally
            {
                objBill = new tbl_bill();
                dt_tblTemp_id = new DataTable();
                dt_tblTemp_id.Rows.Clear();
                AssortedItem.CancelAssortedItem();
                clsDeal.GetMathod.CancelDealItem();
                clsItems.GetMathod.CancelItem();
            }
        }

        public void PrintCancelBill(double GrandTotal,string bill_no)
        {
            PrinterSettings settings = new PrinterSettings();
            string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
            string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
            LPrinter PosPrinter = new LPrinter();
            string CurrentPrinter = settings.PrinterName;
            PosPrinter.Open("testDoc", CurrentPrinter);
            string str1;
            DateTime Date = DateTime.ParseExact(Program.DayEnd_BIllingDate, "yyyy-MM-dd", null);
            string sql = "Usp_Get_Cancel_qot @Bill_No='" + bill_no.Trim() + "'";
            ADOC objadoc = new ADOC();
            DataSet ds = objadoc.GetDatset(sql, "tbl_bill_tran");
            DataTable dt2 = ds.Tables[0];
            if (dt2.Rows.Count > 0)
            {
                string OTP = "";
                if (dt2.Columns.Contains("OTP"))
                    OTP = dt2.Rows[0]["OTP"].ToString();

                PosPrinter.Print("        --------" + Program.CompanyName + "------\n");
                PosPrinter.Print("'" + Program.Address + "'\n\n");
                //PosPrinter.Print("             -----KOT-----\n");
                PosPrinter.Print("          -----Cancel Bill & KOT----\n\n");
                if (OTP.Length > 0)
                {
                    PosPrinter.Print("Bill No:-" + bill_no.Trim() + "  OTP: " + OTP + "\n");
                }
                else
                {
                    PosPrinter.Print("Bill No:-" + bill_no.Trim() + "\n");
                }
                PosPrinter.Print("Date   :-" + Date.ToString("dd-MM-yyyy") + "\n");
                PosPrinter.Print("Time   :-" + DateTime.Now.ToString("HH:mm") + "\n");
                PosPrinter.Print("\n---------------------------------------------\n");
                PosPrinter.Print("Item Name                             Qty");
                PosPrinter.Print("\n---------------------------------------------\n");
                for (int b = 0; b < dt2.Rows.Count; ++b)
                {
                    string item_addon = dt2.Rows[b]["item_addon"].ToString();
                    string i_name = dt2.Rows[b]["i_name"].ToString();
                    if (item_addon != "0")
                        i_name = "+" + i_name;
                    if (i_name.Length <= 36)
                    {
                        for (int cn = i_name.Length; cn <= 35; ++cn)
                            i_name += " ";
                    }
                    str1 = "" + i_name + "  " + dt2.Rows[b]["cancel_qty"].ToString() + "\n";
                    PosPrinter.Print(str1);
                    string i_code = dt2.Rows[b]["item_code"].ToString();
                    DataTable dt = ds.Tables[1];
                    DataTable dtDoughnuts = GetAssortedItemToPrint(Convert.ToInt64(i_code), dt);
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
                            string PrintQuery = "" + item_name + "  " + dtDoughnuts.Rows[i]["cancel_qty"].ToString() + "\n";
                            PosPrinter.Print(PrintQuery);
                        }
                    }
                }

                PosPrinter.Print("------------------------------------------");
                PosPrinter.Print("\n Total Bill Amount(Included Tax):   " + GrandTotal.ToString("N2") + "\n");
                PosPrinter.Print("\n Cancel By: " + Program.CashierName + "\n");
                PosPrinter.Print("\n Remark: " + "Cancelled From Online" + "\n");

                PosPrinter.Print("------------------------------------------\n\n\n\n\n\n\n\n");
                PosPrinter.Print(PaperFullCut);
            }
            PosPrinter.Close();
        }

        public DataTable GetAssortedItemToPrint(Int64 ItemCode, DataTable dt)
        {
            DataTable dtobj = null;
            var currentStatRow = (from currentStat in dt.AsEnumerable()
                                  where currentStat.Field<Int64>("i_code_fk") == ItemCode
                                  select currentStat);
            if (currentStatRow.Count() > 0)
            {
                dtobj = new DataTable();
                dtobj = currentStatRow.CopyToDataTable();
            }
            return dtobj;
        }
    }
}
