using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TuchScreenApp1Jan2013.App_Code;

namespace KOTPrintUtility.App_Code
{
    class clsGenerateBillSweets
    {
        SqlTransaction transaction;

        //public bool GenerateTakeAwayBill(string POS_code, string Bill_Type, string Bill_Date, string Bill_Amount, string Dis_Amount, string Service_Charge_Amount,
        //      string Paid_Amount, string Balance, string Payment_Mode, string Card_No, string No_of_Item, string To_Whom, string Comments,
        //      string Cashier, string tax1, string roundoff, string Discount_by, string TockenNo, string Service_Charge, string sur_charge,
        //      string Service_Tax, string Bill_DiscountPct, string SubTotal, string card_type, string HoldBill_No, string dept_head, string Tips,
        //  string OrderNo, DataGridView ObjDgv, out string Bill_No, string SBC_Amount, string KKC_Amount, string HD_IGST, string BtcApprovedBy,
        //  string URNNO, string OtherCharge, string IsServiceRemoved, string BeforeDiscountSubTotal, string Total_Points_Redeemed, string InvNo_Unique, string acquirementId
        //    , string UserName)
        //{
        //    SqlCommand cmd = null;
        //    DataGridView ObjDgvPayment = new DataGridView();
        //    int result = 0;
        //    bool IsSuccess = false;
        //    string sql = string.Empty;
        //    string ItemRecipeSql = string.Empty;
        //    string LastBillNo = string.Empty;
        //    //clsRecipeDetail objRecipe = new clsRecipeDetail();
        //    int NoofItems = ObjDgv.Rows.Count;
        //    double BTCAmountTotalMulti = 0;
        //    string sqlKey = ConfigurationSettings.AppSettings["sqlKey"].ToString();
        //    //string KOT_No = ADOC.GetObject.GetSingleResult("dbo.Usp_GetMaxKotNo");
        //    SqlConnection Sqlcon = new SqlConnection(sqlKey);
        //    try
        //    {
        //        cmd = new SqlCommand();
        //        Sqlcon.Open();
        //        cmd.Connection = Sqlcon;
        //        cmd.CommandText = "Usp_Insert_TakeAway";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        // Begin transaction
        //        transaction = Sqlcon.BeginTransaction();
        //        cmd.Transaction = transaction;
        //        cmd.Parameters.AddWithValue("@POS_code", POS_code);
        //        cmd.Parameters.AddWithValue("@Bill_Type", Bill_Type);
        //        cmd.Parameters.AddWithValue("@Bill_Date", Bill_Date);
        //        cmd.Parameters.AddWithValue("@Bill_Amount", Bill_Amount);
        //        cmd.Parameters.AddWithValue("@Dis_Amount", Dis_Amount);
        //        cmd.Parameters.AddWithValue("@Service_Charge_Amount", Service_Charge_Amount);
        //        cmd.Parameters.AddWithValue("@Paid_Amount", Paid_Amount);
        //        cmd.Parameters.AddWithValue("@Balance", Balance);
        //        cmd.Parameters.AddWithValue("@Card_No", Card_No);
        //        cmd.Parameters.AddWithValue("@No_of_Item", No_of_Item);
        //        cmd.Parameters.AddWithValue("@To_Whom", To_Whom);
        //        cmd.Parameters.AddWithValue("@Comments", Comments);
        //        cmd.Parameters.AddWithValue("@Cashier", Cashier);
        //        cmd.Parameters.AddWithValue("@tax1", tax1);
        //        cmd.Parameters.AddWithValue("@roundoff", roundoff);
        //        cmd.Parameters.AddWithValue("@Discount_by", Discount_by);
        //        cmd.Parameters.AddWithValue("@TockenNo", TockenNo);
        //        cmd.Parameters.AddWithValue("@Service_Charge", Service_Charge);
        //        cmd.Parameters.AddWithValue("@sur_charge", sur_charge);
        //        cmd.Parameters.AddWithValue("@Service_Tax", Service_Tax);
        //        cmd.Parameters.AddWithValue("@Bill_DiscountPct", Bill_DiscountPct);
        //        cmd.Parameters.AddWithValue("@subtot", SubTotal);
        //        cmd.Parameters.AddWithValue("@card_type", card_type);
        //        cmd.Parameters.AddWithValue("@dept_head", dept_head);
        //        cmd.Parameters.AddWithValue("@card_amt", Program.CardAmount);
        //        cmd.Parameters.AddWithValue("@HoldBill_No", HoldBill_No);
        //        cmd.Parameters.AddWithValue("@tips", Tips);
        //        cmd.Parameters.AddWithValue("@ORDER_NO", OrderNo);
        //        cmd.Parameters.AddWithValue("@extBill", "0");
        //        cmd.Parameters.AddWithValue("@outlet_id", Program.Outlet_id);
        //        cmd.Parameters.AddWithValue("@cust_code", Program.Cust_code);
        //        cmd.Parameters.AddWithValue("@cust_dis_amount", Program.Discount_Amt_Show);
        //        cmd.Parameters.AddWithValue("@SBC_Amount", KKC_Amount);
        //        cmd.Parameters.AddWithValue("@KKC_Amount", KKC_Amount);
        //        ////////Devi ram related add on 26/05/2017 
        //        cmd.Parameters.AddWithValue("@cust_add_tran", Program.Addr_ID);
        //        ////////End Devi ram
        //        cmd.Parameters.AddWithValue("@IGST", HD_IGST);
        //        cmd.Parameters.AddWithValue("@stateid", Program.Supply_State);
        //        cmd.Parameters.AddWithValue("@BtcApprovedBy", BtcApprovedBy);
        //        cmd.Parameters.AddWithValue("@Other_Charge", OtherCharge);
        //        if (Program.bill_no_WebOrder.Trim().Length > 0 || Program.ZomatoOrderNo.Trim().Length > 0)
        //        {
        //            cmd.Parameters.AddWithValue("@source_of_order", Program.Source_of_order);
        //            cmd.Parameters.AddWithValue("@web_Order_comment", Program.online_Order_comment);
        //            cmd.Parameters.AddWithValue("@IsManualPuch", "0");
        //            cmd.Parameters.AddWithValue("@WebOrder_No", Program.bill_no_WebOrder);
        //            cmd.Parameters.AddWithValue("@zomato_order_id", Program.ZomatoOrderNo);
        //            cmd.Parameters.AddWithValue("@online_Order_PunchBy", Program.online_Order_PunchBy);


        //            Payment_Mode = Program.online_Payment_Mode;

        //            string WebPayment_Mode = "1";
        //            if (Payment_Mode == "Online")
        //            {
        //                WebPayment_Mode = "3";
        //                Payment_Mode = "3";
        //            }
        //            else if (Payment_Mode == "COD")
        //            {
        //                WebPayment_Mode = "0";
        //                Payment_Mode = "0";
        //            }
        //            cmd.Parameters.AddWithValue("@WebPayment_Mode", WebPayment_Mode);
        //            cmd.Parameters.AddWithValue("@External_Source_id", Program.External_Source_id);
        //            cmd.Parameters.AddWithValue("@Aggr_dis_amount", PackingCharges.Aggr_Discount);
        //            cmd.Parameters.AddWithValue("@Aggr_dis_pct", PackingCharges.Aggr_dis_pct);
        //        }
        //        else
        //            cmd.Parameters.AddWithValue("@source_of_order", Program.Source_of_order);

        //        cmd.Parameters.AddWithValue("@Payment_Mode", Payment_Mode);
        //        cmd.Parameters.AddWithValue("@Delivery_Type", "0");
        //        cmd.Parameters.AddWithValue("@Del_Time", "");
        //        cmd.Parameters.AddWithValue("@Total_Points_Redeemed", Total_Points_Redeemed);

        //        cmd.Parameters.AddWithValue("@Discount_card_no", "");
        //        cmd.Parameters.AddWithValue("@Discount_Coupon_no", Program.DiscountCouponNo);


        //        if (InvNo_Unique != string.Empty && InvNo_Unique.Length > 0)
        //        {
        //            cmd.Parameters.AddWithValue("@InvNo_Unique", InvNo_Unique);
        //            cmd.Parameters.AddWithValue("@acquirementId", acquirementId);
        //        }

        //        cmd.Parameters.Add("@Bill_No", SqlDbType.Int);
        //        cmd.Parameters["@Bill_No"].Direction = ParameterDirection.Output;
        //        result = cmd.ExecuteNonQuery();
        //        LastBillNo = cmd.Parameters["@Bill_No"].Value.ToString();
        //        if (result > 0 && Convert.ToInt32(LastBillNo) > 0)
        //        {
        //            string sqlSattle = string.Empty;
        //            for (int rowIndex = 0; rowIndex < ObjDgv.Rows.Count; rowIndex++)
        //            {
        //                string sbc_rate = "0"; string kkc_rate = "0"; double SBCAmount_tran = 0; double KKCAmount_tran = 0;
        //                double dis_amount = 0;
        //                string DishCode = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["DishCode"].Value);
        //                string DishName = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["ItemName"].Value);
        //                double Qty = Convert.ToDouble(ObjDgv.Rows[rowIndex].Cells["Qty"].Value);
        //                string Price = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["Rate"].Value);
        //                string TotalAmt = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["Total"].Value);
        //                string dishComments = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["DishComment"].Value);
        //                string Discount = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["Discount"].Value);
        //                Bill_DiscountPct = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["dis pct"].Value);

        //                if (Bill_DiscountPct == null || Bill_DiscountPct == "")
        //                    Bill_DiscountPct = "0";
        //                //if (Discount == "Yes")
        //                //    Bill_DiscountPct = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["Dis Pct"].Value);
        //                dis_amount = Convert.ToDouble(TotalAmt) * Convert.ToDouble(Bill_DiscountPct) / 100;

        //                string TaxAmount = "0";
        //                string TaxRate = ObjDgv.Rows[rowIndex].Cells["TaxRate"].Value.ToString();
        //                if (Payment_Mode != "5")
        //                {
        //                    TaxAmount = ObjDgv.Rows[rowIndex].Cells["Tax"].Value.ToString();
        //                }
        //                string AddonCode = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["AddonCode"].Value);
        //                string AddonCode_fk = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["AddonCode_fk"].Value);
        //                string Tax = string.Empty;
        //                //if (Program.PaymentMode == "NC")
        //                //{
        //                //    Bill_DiscountPct = "100";   /////new condition added by satyabir on 010519
        //                //    Tax = "0";
        //                //    dis_amount = Convert.ToDouble(TotalAmt) * Convert.ToDouble(Bill_DiscountPct) / 100;
        //                //}
        //                //else
        //                Tax = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["Tax"].Value);
        //                if (dishComments != string.Empty)
        //                    dishComments = dishComments;
        //                //double ServiceCharge = 0;
        //                //ServiceCharge = (Convert.ToDouble(TotalAmt) - dis_amount) * Convert.ToDouble(Service_charge_rate) / 100;
        //                double Service_Tax_Cap = 0;
        //                double S_TaxAmount = 0;

        //                string item_index = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["item_index"].Value);

        //                #region Compain Items related Added by satyabir on 05062018
        //                string Compain = "0";
        //                if (ObjDgv.Columns.Contains("IsCompain"))
        //                {
        //                    Compain = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["IsCompain"].Value);
        //                }
        //                #endregion

        //                #region Gift Hamper related added by satyabir on 260319
        //                string IsGiftHamper = "0";
        //                if (ObjDgv.Columns.Contains("IsGiftHamper"))
        //                {
        //                    IsGiftHamper = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["IsGiftHamper"].Value);
        //                }
        //                #endregion

        //                double Agg_Discount = 0;
        //                string IsRestaurantItem = "0";
        //                try
        //                {
        //                    if (ObjDgv.Columns.Contains("IsRestaurantItem") && Program.BillType == "H")
        //                        IsRestaurantItem = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["IsRestaurantItem"].Value);
        //                }
        //                catch { }

        //                if (PackingCharges.Agg_Tax_Calculation == "1")
        //                    sql += "EXEC dbo.Usp_InsertInTRan_HD";
        //                else
        //                    sql += "EXEC dbo.Usp_InsertInTRan";

        //                sql += " @Bill_No_FK='" + LastBillNo + "',@Bill_Date='" + Program.DayEnd_BIllingDate + "',@Item_Code='" + DishCode +
        //                    "',@Qty='" + Qty.ToString("N3").Replace(",", "") + "',@Rate='" + Price + "',@Amount='" + TotalAmt.Replace(",", "") + "',@cashier='" + UserName +
        //                     "',@Dis_Rate='" + Bill_DiscountPct.Replace(",", "") + "',@Dis_Amount='" + dis_amount.ToString("N4").Replace(",", "") + "',@Tax_Rate='" + TaxRate.Replace(",", "") +
        //                     "',@Tax_Amount='" + TaxAmount.Replace(",", "") + "',@Comments='" + dishComments + "',@item_addon='" + AddonCode_fk + "',@Service_charge_rate='0',@service_tax_rate='" + Service_Tax_Cap +
        //                     "',@Surcharge_rate='0',@KOT_No='0',@Order_Type='H',@stax_amount='" + S_TaxAmount.ToString("N3") +
        //                     "',@outlet_id='" + Program.Outlet_id + "',@item_index='" + item_index + "',@SBC_Rate='" + sbc_rate + "',@SBC_Amount='" + SBCAmount_tran.ToString("N2") +
        //                     "',@KKC_Rate='" + kkc_rate + "',@KKC_Amount='" + KKCAmount_tran.ToString("N2") + "',@Bill_Type='" + Bill_Type + "',@HD_IGST='" + HD_IGST +
        //                     "',@DELIVERY_AMOUNT='" + OtherCharge + "',@SUNTOTAL='" + SubTotal.Replace(",", "") + "', @IsServiceRemoved='" + IsServiceRemoved +
        //                     "',@Compain='" + Compain + "',@IsGiftHamper='" + IsGiftHamper + "',@Agg_Discount='" + Agg_Discount.ToString() +
        //                     "',@Agg_Tax_Calculation='" + PackingCharges.Agg_Tax_Calculation + "',@ZomatoOrderNo='" + Program.ZomatoOrderNo + "',@PunchBy='" + UserName + "',@IsRestaurantItem='" + IsRestaurantItem + "',@IsOnlineOrderHD='0';";
        //            }
        //            if (Bill_Type != "H" )
        //            {
        //                ObjDgvPayment = clsTempData.GetDataGrid();
        //                for (int rowIndex = 0; rowIndex < ObjDgvPayment.Rows.Count; rowIndex++)
        //                {
        //                    string PaymentModeid = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["id"].Value);
        //                    string PaymentModeText = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["Payment Mode"].Value);
        //                    string Amount = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["Amount"].Value);
        //                    string ModeType = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["Type"].Value);
        //                    string hold_id = "0";
        //                    if (PaymentModeid == "7" || PaymentModeid == "5")
        //                    {
        //                        hold_id = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["hold_id"].Value);
        //                    }

        //                    //Multi mode in BTC related added by Satyabir on 270220
        //                    if (Payment_Mode == "9")
        //                    {
        //                        if (PaymentModeid == "8")
        //                        {
        //                            BTCAmountTotalMulti += Convert.ToDouble(Amount);
        //                        }
        //                    }
        //                    //Multi mode in BTC related end by Satyabir on 270220  

        //                    string ModeValue = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["ModeValue"].Value);

        //                    sqlSattle += "Exec dbo.Usp_Inserytbl_PMS @bill_no_fk='" + LastBillNo + "',@payment_mode='" + PaymentModeid + "',@Amount='" + Amount +
        //                        "',@Cashier='" + UserName + "',@card_type='" + ModeType + "',@Invoice_No='" + ModeValue + "',@hold_id='" + hold_id + "';";
        //                }
        //            }

        //            // deal****************
        //            string sqlAssorted = string.Empty;
        //            DataTable dtDeal1 = clsItems.objItems.Tables["tbl_Items"];
        //            //for (int i = 0; i < dtDeal.Rows.Count; i++)
        //            if (dtDeal1.Rows.Count > 0)
        //            {
        //                string DishCodeMain = string.Empty;
        //                string item_indexMain = string.Empty;
        //                for (int rowIndex = 0; rowIndex < ObjDgv.Rows.Count; rowIndex++)
        //                {
        //                    DishCodeMain = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["DishCode"].Value);
        //                    item_indexMain = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["item_index"].Value);
        //                    Bill_DiscountPct = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["dis pct"].Value);
        //                    if (Bill_DiscountPct == null || Bill_DiscountPct == "")
        //                        Bill_DiscountPct = "0";

        //                    DataTable dtDeal = clsItems.GetMathod.GetItemByIndex(DishCodeMain, item_indexMain);
        //                    for (int i = 0; i < dtDeal.Rows.Count; i++)
        //                    {
        //                        string i_code = dtDeal.Rows[i]["i_code"].ToString();
        //                        string i_code_fk = dtDeal.Rows[i]["i_code_fk"].ToString();
        //                        double Qty = Convert.ToDouble(dtDeal.Rows[i]["qty"].ToString());
        //                        string Order_Type = "d";
        //                        string item_index = dtDeal.Rows[i]["index"].ToString();
        //                        string isDiscount = dtDeal.Rows[i]["isDiscount"].ToString();
        //                        string item_index_value = dtDeal.Rows[i]["Index_no"].ToString();
        //                        //double TranQty = GetQty(ObjDgv, item_index) * Qty;

        //                        ////**********************liner item tax details related new condition added on 02/05/2017******************
        //                        double MainItemQty = GetQty(ObjDgv, item_index);
        //                        double TranQty = MainItemQty * Qty;
        //                        string Order_type = "D";
        //                        double Rate = Convert.ToDouble(dtDeal.Rows[i]["Amount"]);
        //                        //double Amount = Convert.ToDouble(dtDeal.Rows[i]["total"]);
        //                        double Total_Amount = Rate * TranQty;
        //                        double Dis_Rate = Convert.ToDouble(Bill_DiscountPct);
        //                        double Dis_Amount1 = (Total_Amount * Dis_Rate) / 100;

        //                        double Tax_Rate = Convert.ToDouble(dtDeal.Rows[i]["TaxRate"]);
        //                        double TotalTax = (((Total_Amount - Dis_Amount1) * Tax_Rate) / 100);
        //                        /////******************************************Liner item tax end********************************************

        //                        if (TranQty == 0)
        //                        {
        //                            TranQty = Qty;
        //                        }
        //                        sqlAssorted += "Exec Insert_AssortedN_Deals @i_code='" + i_code + "',@i_code_fk='" + i_code_fk + "',@Qty='" + TranQty +
        //                            "',@bill_no='" + LastBillNo + "',@created_by='" + UserName + "',@bill_date='" + Program.DayEnd_BIllingDate +
        //                            "',@Order_Type='" + Order_Type + "',@item_index='" + item_index + "',@isDiscount='" + isDiscount +
        //                            "',@kot_nos='0',@item_index_value='" + item_index_value + "',@Tax_Rate='" + Tax_Rate +
        //                            "',@Tax_Amount='" + TotalTax + "',@Rate='" + Rate + "',@Amount='" + Total_Amount + "',@Dis_Rate='" + Dis_Rate +
        //                            "',@Dis_Amount='" + Dis_Amount1 + "',@Bill_Type='" + Bill_Type + "';";
        //                    }
        //                }
        //            }
        //            int Count3 = 1;
        //            if (sqlAssorted.Length > 0)
        //            {
        //                Count3 = 0;
        //                new SqlCommand();
        //                cmd.CommandType = CommandType.Text;
        //                cmd.CommandText = sqlAssorted;
        //                Count3 = Convert.ToInt32(cmd.ExecuteNonQuery());
        //            }

        //            // execute query :insert in tbl_bill_tran and recipe 
        //            cmd.CommandType = CommandType.Text;
        //            cmd.CommandText = sql;
        //            int Count1 = Convert.ToInt32(cmd.ExecuteNonQuery());

        //            int Count2 = 1;
        //            if (sqlSattle.Length > 0)
        //            {
        //                Count2 = 0;
        //                cmd.CommandText = sqlSattle;
        //                Count2 = Convert.ToInt32(cmd.ExecuteNonQuery());
        //            }


        //            int Count4 = 1;

        //            /////////************* new code added on 07/054/2018
        //            string sql2 = string.Empty;
        //            if (URNNO.Length > 5)
        //            {
        //                if (BTCAmountTotalMulti > 0)
        //                {
        //                    Paid_Amount = (Convert.ToDouble(Paid_Amount) - BTCAmountTotalMulti).ToString();
        //                    Payment_Mode = "8";
        //                }
        //                URNNO = URNNO + LastBillNo;
        //                sql2 = "exec Usp_InsertCustPmtLdgr @Company_id='" + Program.Cust_code + "',@Paid_Amount='" + Paid_Amount + "',@Balance_Amount='0',@Bill_Amount='" + Bill_Amount
        //                          + "',@Ref_Bill_No='" + LastBillNo + "',@created_by='" + UserName + "',@Bill_date='" + Program.DayEnd_BIllingDate +
        //                        "',@Payment_mode='" + Payment_Mode + "',@card_no='',@Card_type='" + Program.Card_Type + "',@sale_order_no='0',@Bill_Type='" + Program.BillType + "',@URNNO='" + URNNO + "',@Btc_Received='0'";
        //            }
        //            ////////*****************End New Code*****************


        //            if (Payment_Mode == "8")
        //            {
        //                if (sql2.Length > 0)
        //                {
        //                    Count4 = 0;
        //                    new SqlCommand();
        //                    cmd.CommandType = CommandType.Text;
        //                    cmd.CommandText = sql2;
        //                    Count4 = Convert.ToInt32(cmd.ExecuteNonQuery());
        //                }
        //            }




        //            if (Count1 > 0 && Count2 > 0 && Count3 > 0 && Count4 > 0)
        //            {
        //                //commit Tran
        //                cmd.Transaction.Commit();
        //                IsSuccess = true;

        //                if (Program.bill_no_WebOrder.Trim().Length > 0)
        //                {
        //                    if (UserName.ToLower() == "admin")
        //                    {
        //                        cls_ZomatoAPi.GetDataTable("Usp_UpdateStatus_WebOrder @Order_No='" + Program.bill_no_WebOrder + "',@bill_no_local='" + LastBillNo + "',@IsManualPuch='0',@Cust_code_local='0'");
        //                    }
        //                    else
        //                    {
        //                        ADOC objADOC = new ADOC();
        //                        cls_ZomatoAPi.GetDataTable("Usp_UpdateStatus_WebOrder @Order_No='" + Program.bill_no_WebOrder + "',@bill_no_local='" + LastBillNo + "',@IsManualPuch='0',@Cust_code_local='" + Program.Cust_code + "'");
        //                        string update = objADOC.GetSingleResult("update TBL_CustomerInfo set Up_flag=1 where id='" + Program.Cust_code + "'");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // rollback Tran
        //                cmd.Transaction.Rollback();
        //                Bill_No = string.Empty;
        //                IsSuccess = false;
        //            }
        //        }
        //        else
        //        {
        //            // rollback Tran
        //            cmd.Transaction.Rollback();
        //            Bill_No = string.Empty;
        //            IsSuccess = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // rollback Tran
        //        cmd.Transaction.Rollback();
        //        Bill_No = string.Empty;
        //        IsSuccess = false;
        //        MessageBox.Show("GenerateTakeAwayBill" + ex.Message);
        //    }
        //    Sqlcon.Close();

        //    Bill_No = LastBillNo;
        //    return IsSuccess;
        //}

        private double GetQty(DataGridView objDgv, string ItemIndex)
        {
            double NetQty = 0;
            try
            {
                var query = from DataGridViewRow row in objDgv.Rows
                            where row.Cells["Item_Index"].Value.ToString().ToLower() == ItemIndex
                            select new { qty = row.Cells["Qty"].Value.ToString() };
                foreach (var qry in query)
                {
                    NetQty = Convert.ToDouble(qry.qty.ToString());
                }
            }
            catch (Exception ex)
            {
               // ADOC.GetObject.ExecuteDML("Usp_CreateError_log @Form_Name='clsGenerateBill Line:41',@Error_name='" + ex.Message + "',@Bill_date='" + Program.DayEnd_BIllingDate + "',@created_by='" + Program.UserName + "'");
            }
            return NetQty;
        }
    }
}
