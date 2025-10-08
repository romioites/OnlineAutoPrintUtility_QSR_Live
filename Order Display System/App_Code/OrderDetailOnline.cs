using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using System.Windows.Forms;
using TuchScreenApp1Jan2013.App_Code;
using System.Threading;

namespace KOTPrintUtility.App_Code
{
    public class OrderDetailOnline : IOnineOrderAutoAccept
    {
        clsConnnectionManager objclsConnnectionManager = new clsConnnectionManager();
        string Istesting = "0";
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataTable GetOnlineOrder()
        {
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            try
            {
                clsConnnectionManager objclsConnnectionManager = new clsConnnectionManager();
                string ZomatoAPi = GetAllPunchedZomatoIds();
                cmd = new SqlCommand();
                if (Istesting == "1")
                    cmd.CommandText = "Usp_GetTblRunning_Service_Test";// test
                else
                    cmd.CommandText = "Usp_GetTblRunning_Service";
                cmd.Connection = objclsConnnectionManager.SQlConnection_Online();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bill_date", Program.DayEnd_BIllingDate);
                cmd.Parameters.AddWithValue("@outlet_id", Program.Outlet_id);
                cmd.Parameters.AddWithValue("@Zomato_order_id", ZomatoAPi);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public DataSet GetOrderDetail(string Bill_no, string outlet_id, string bill_Date)
        {
            SqlCommand cmd = null;
            DataSet ds = new DataSet();
            try
            {
                clsConnnectionManager objclsConnnectionManager = new clsConnnectionManager();
                cmd = new SqlCommand();
                cmd.CommandText = "Usp_Runing_Item_Web_order_AutoPrint";
                cmd.Connection = objclsConnnectionManager.SQlConnection_Online();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Bill_no", Bill_no);
                cmd.Parameters.AddWithValue("@outlet_id", outlet_id);
                cmd.Parameters.AddWithValue("@bill_Date", bill_Date);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
            }
            catch (Exception ex)
            {

            }
            return ds;
        }
        public bool ExecuteDMLOnline(string sql)
        {
            bool result = false;
            SqlCommand cmd = null;
            try
            {
                cmd = new SqlCommand(sql);
                cmd.Connection = objclsConnnectionManager.SQlConnection_Online();
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 3000;
                int Count = cmd.ExecuteNonQuery();
                if (Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
            return result;
        }

        public int GenerateUniqIndex(string I_code, int IsDelete, DataTable dt_tblTemp_id)
        {
            int id;
            if (dt_tblTemp_id.Columns.Count == 0)
            {
                dt_tblTemp_id.Columns.Add("I_code");
            }
            if (IsDelete == 1)
            {
                dt_tblTemp_id.Rows.Clear();
            }
            dt_tblTemp_id.Rows.Add(I_code);
            id = dt_tblTemp_id.Rows.Count;
            if (id == 0)
                id = 1;
            return id;
        }

        /// <summary>
        /// GetAllPunchedZomatoIds
        /// </summary>
        /// <returns></returns>
        public string GetAllPunchedZomatoIds()
        {
            try
            {
                string retrun = string.Empty;
                string localSqlKey = ConfigurationSettings.AppSettings["sqlKey"].ToString();
                using (SqlConnection con = new SqlConnection(localSqlKey))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@Bill_Date", Program.DayEnd_BIllingDate);
                    cmd.CommandText = "dbo.USP_GetAllZomatoPunchedOrder";
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            retrun = Convert.ToString(dt.Rows[0]["ZomatoId"]);
                        }
                    }
                }
                return retrun;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// GetBillType
        /// </summary>
        /// <param name="Bill_no"></param>
        /// <returns></returns>
        public string GetBillType(string Bill_no)
        {
            string Bill_type = string.Empty;
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            try
            {
                clsConnnectionManager objclsConnnectionManager = new clsConnnectionManager();
                //string ZomatoAPi = GetAllPunchedZomatoIds();
                cmd = new SqlCommand();
                cmd.CommandText = "select top 1 bill_type from tbl_bill_web_order with(nolock) where bill_no='" + Bill_no + "'";
                cmd.Connection = objclsConnnectionManager.SQlConnection_Online();
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Bill_type = dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return Bill_type;
        }

        SqlCommand cmd = null;
        SqlTransaction transaction;



        private double GetQty(DataGridView objDgv, string ItemIndex)
        {
            double NetQty = 0;
            try
            {
                var query = from DataGridViewRow row in objDgv.Rows
                            where row.Cells["ItemIndex"].Value.ToString().ToLower() == ItemIndex
                            select new { qty = row.Cells["Qty"].Value.ToString() };
                foreach (var qry in query)
                {
                    NetQty = Convert.ToDouble(qry.qty.ToString());
                }
            }
            catch (Exception ex)
            {
            }
            return NetQty;
        }


        public bool GenerateBill_HD(string POS_code, string Bill_Type, string Bill_Date, string Bill_Amount,
                string Dis_Amount, string Service_Charge_Amount, string Paid_Amount, string Balance,
                string Payment_Mode, string Card_No, string No_of_Item, string To_Whom, string Comments,
                string Cashier, string tax1, string roundoff, string Discount_by, string TockenNo,
  string Service_Charge, string sur_charge, string Service_Tax, string HoldBill_No, string cardType, string cardAmount, string OutletId,
  string subTotal, string Bill_DiscountPct, string dept_head, string OrderNo, string Other_Charge, tbl_bill Objbill, out string Bill_No,
          DataTable dtDeal, string RegisterUserStatus, string SBC_Tax, string KKC_Tax, string order_status, string IsBillSattle,
          string weborderCommnet, string RedeemPoints, string lblRedeemPoints, string is_instant_order = "0")
        {
            string sql_Coupon = string.Empty;
            string hostName = Dns.GetHostName();
            string ipAddress = Dns.GetHostByName(hostName).AddressList[0].ToString();
            int result = 0;
            bool IsSuccess = false;
            string sql = string.Empty;
            string LastBillNo = string.Empty;
            if (IsBillSattle == "")
            {
                if (Payment_Mode == "5")
                    Payment_Mode = "5";
                else
                    Payment_Mode = "0";
            }
            string scon = ConfigurationSettings.AppSettings["sqlKey"].ToString();
            SqlConnection Sqlcon = new SqlConnection(scon);
            try
            {
                cmd = new SqlCommand();
                Sqlcon.Open();
                cmd.Connection = Sqlcon;
                cmd.CommandText = "Usp_Insert_in_tbl_Bill_HD_Utility";
                cmd.CommandType = CommandType.StoredProcedure;
                // Begin transaction
                transaction = Sqlcon.BeginTransaction();
                cmd.Transaction = transaction;
                cmd.Parameters.AddWithValue("@POS_code", POS_code);
                cmd.Parameters.AddWithValue("@Bill_Type", (Bill_Type == "" ? "H" : Bill_Type));
                cmd.Parameters.AddWithValue("@Cust_Code", Objbill.custCode);
                cmd.Parameters.AddWithValue("@Bill_Date", Bill_Date);
                cmd.Parameters.AddWithValue("@Bill_Amount", Bill_Amount);
                cmd.Parameters.AddWithValue("@Dis_Amount", Dis_Amount);
                cmd.Parameters.AddWithValue("@Service_Charge_Amount", Service_Charge_Amount);
                cmd.Parameters.AddWithValue("@Paid_Amount", Paid_Amount);
                cmd.Parameters.AddWithValue("@Balance", Balance);
                cmd.Parameters.AddWithValue("@Card_No", Card_No);
                cmd.Parameters.AddWithValue("@No_of_Item", No_of_Item);
                cmd.Parameters.AddWithValue("@To_Whom", To_Whom);
                cmd.Parameters.AddWithValue("@Cashier", Cashier);
                cmd.Parameters.AddWithValue("@tax1", tax1);
                cmd.Parameters.AddWithValue("@roundoff", roundoff);
                cmd.Parameters.AddWithValue("@Discount_by", Discount_by);
                cmd.Parameters.AddWithValue("@TockenNo", TockenNo);
                cmd.Parameters.AddWithValue("@HoldBill_No", HoldBill_No);
                cmd.Parameters.AddWithValue("@CardType", Objbill.CardType);
                cmd.Parameters.AddWithValue("@CardAmount", cardAmount);
                cmd.Parameters.AddWithValue("@OutletId", OutletId);
                cmd.Parameters.AddWithValue("@Table_No", "0");
                cmd.Parameters.AddWithValue("@Service_Tax", Service_Tax);
                cmd.Parameters.AddWithValue("@sur_charge", sur_charge);
                cmd.Parameters.AddWithValue("@subTotal", subTotal);
                cmd.Parameters.AddWithValue("@Bill_DiscountPct", Bill_DiscountPct);
                cmd.Parameters.AddWithValue("@dept_head", dept_head);
                cmd.Parameters.AddWithValue("@OrderNo", OrderNo);
                cmd.Parameters.AddWithValue("@Other_Charge", Other_Charge);
                cmd.Parameters.AddWithValue("@ipAddress", ipAddress);
                cmd.Parameters.AddWithValue("@web_order_comments", weborderCommnet);
                cmd.Parameters.AddWithValue("@IsManualPuch", "0");
                if (is_instant_order == "1")
                {
                    cmd.Parameters.AddWithValue("@is_instant_order", is_instant_order);
                }
                if (Objbill.bill_no_WebOrder != "")
                {
                    double discount = 0.00;
                    double.TryParse(Dis_Amount, out discount);
                    if (discount > 0)
                        Comments = Objbill.Channel + " discount";
                    cmd.Parameters.AddWithValue("@WebOrder_No", Objbill.bill_no_WebOrder);
                    cmd.Parameters.AddWithValue("@zomato_order_id", Objbill.ZomatoOrderNo);
                    //Payment_Mode = Program.PaymentModeOnline;

                    cmd.Parameters.AddWithValue("@Channel_Order_Id", Objbill.Channel_Id);
                    cmd.Parameters.AddWithValue("@Delivery_By", Objbill.Delivery_By);
                    cmd.Parameters.AddWithValue("@Aggr_dis_amount", Objbill.Aggr_dis_amount.Replace(",", ""));
                    cmd.Parameters.AddWithValue("@Aggr_dis_pct", Objbill.Aggr_dis_pct.Replace(",", ""));
                }
                cmd.Parameters.AddWithValue("@Channel", Objbill.Channel);
                cmd.Parameters.AddWithValue("@Comments", Comments);

                if (Program.DiscountCouponNo != "" && Program.DiscountCouponNo != "0")
                {
                    cmd.Parameters.AddWithValue("@dis_type", Objbill.dis_Type);
                    cmd.Parameters.AddWithValue("@Coupon_code", Program.DiscountCouponNo);
                }
                else if (Objbill.dis_Type == "Discount Card")
                {
                    cmd.Parameters.AddWithValue("@dis_type", Objbill.dis_Type);
                    cmd.Parameters.AddWithValue("@Coupon_code", "");
                }
                else
                    cmd.Parameters.AddWithValue("@dis_type", Objbill.dis_Type);
                cmd.Parameters.AddWithValue("@Payment_Mode", Payment_Mode);
                if (Objbill.bill_no_WebOrder != "")
                    cmd.Parameters.AddWithValue("@bill_no_TabOrder", Objbill.bill_no_WebOrder);
                cmd.Parameters.AddWithValue("@SBC_Tax", SBC_Tax);
                cmd.Parameters.AddWithValue("@KKC_Tax", KKC_Tax);
                if (order_status != "")
                    cmd.Parameters.AddWithValue("@order_status", order_status);
                if (IsBillSattle != "")
                    cmd.Parameters.AddWithValue("@IsBillSattle", IsBillSattle);

                cmd.Parameters.AddWithValue("@source_of_order", Program.Source_of_order);
                cmd.Parameters.AddWithValue("@Total_Points_Redeemed", RedeemPoints);
                cmd.Parameters.Add("@Bill_No", SqlDbType.Int);
                cmd.Parameters["@Bill_No"].Direction = ParameterDirection.Output;
                result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    LastBillNo = cmd.Parameters["@Bill_No"].Value.ToString();
                    for (int rowIndex = 0; rowIndex < Objbill.tran.Count; rowIndex++)
                    {
                        string DishCode = Convert.ToString(Objbill.tran[rowIndex].Assorted_Item);
                        string DishName = Convert.ToString(Objbill.tran[rowIndex].I_Name);
                        string Qty = Convert.ToString(Objbill.tran[rowIndex].qty);
                        string Price = Convert.ToString(Objbill.tran[rowIndex].Rate);
                        string TotalAmt = Convert.ToString(Objbill.tran[rowIndex].amount);
                        string dishComments = "";
                        string Tax = Convert.ToString(Objbill.tran[rowIndex].tax);
                        string ItemAddonCode = "0";
                        string TaxRate = Convert.ToString(Objbill.tran[rowIndex].TaxRate);
                        string Group_Dish = Convert.ToString(Objbill.tran[rowIndex].Group_Dish);
                        decimal DisAmount = 0;
                        string UrgentItem = "0";
                        string Discount = "1";
                        string DisRate = Objbill.tran[rowIndex].dis_rate.ToString();
                        DisAmount = Convert.ToDecimal(Objbill.tran[rowIndex].dis_amount.ToString());
                        double Service_Tax_Cap = 0;

                        if (dishComments == "null")
                            dishComments = dishComments.Replace("null", " ").Trim();
                        if (dishComments != string.Empty && dishComments != "null")
                        {
                            dishComments = dishComments.Replace(",", "\r+");
                        }
                        string addon_index = "0";
                        string addon_index_fk = "0";
                        decimal Agg_Discount = 0;

                        sql += "EXEC dbo.Usp_Insert_TblTran";

                        sql += " @Bill_No_FK='" + LastBillNo + "',@Bill_Date='" + Program.DayEnd_BIllingDate + "',@Item_Code='" + DishCode + "',@Qty='" + Qty.Replace(",", "") +
                        "',@Rate='" + Price.Replace(",", "") + "',@Amount='" + TotalAmt.Replace(",", "") + "',@cashier='admin',@Comments='" + dishComments + "',@Tax_Amount='" + Tax +
                        "',@Dis_Amount='" + DisAmount + "',@item_addon='" + ItemAddonCode + "',@Tax_Rate='" + TaxRate + "',@Dis_Rate='" + DisRate + "',@Group_Dish='" + Group_Dish + "',@STax_Rate='" + Service_Tax_Cap +
                        "',@STax_Amount='" + 0.ToString("N3") + "',@SBC_Rate='" + 0.ToString("N2") + "',@SBC_Amount='" + 0.ToString("N2") + "',@KKC_Rate='" + 0 + "',@KKC_Amount='" + 0.ToString("N2") + "',@id='" + (rowIndex + 1).ToString() +
                        "',@Urgent_Item='" + UrgentItem + "',@DELIVERY_AMOUNT='" + Other_Charge.Replace(",", "") + "',@SUNTOTAL='" + subTotal.Replace(",", "") + "',@addon_index='" + addon_index + "',@addon_index_fk='" + addon_index_fk +
                        "',@Agg_Discount='" + Agg_Discount.ToString() + "',@Agg_Tax_Calculation='" + Objbill.Agg_Tax_Calculation + "';";
                    }
                    // execute quuery :insert in tbl_bill_tran 
                    new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    int CountTran = Convert.ToInt32(cmd.ExecuteNonQuery());
                    // execute quuery :insert in tbl_assorted_item
                    string sqlAssorted = string.Empty;
                    for (int i = 0; i < dtDeal.Rows.Count; i++)
                    {
                        string i_code = dtDeal.Rows[i]["i_code"].ToString();
                        string i_code_fk = dtDeal.Rows[i]["i_code_fk"].ToString();
                        double Qty = Convert.ToDouble(dtDeal.Rows[i]["qty"].ToString());
                        string Order_Type = dtDeal.Rows[i]["order_type"].ToString();
                        if (Order_Type == "")
                            Order_Type = "A";
                        string item_index = dtDeal.Rows[i]["ItemIndex"].ToString();
                        string isDiscount = dtDeal.Rows[i]["isDiscount"].ToString();
                        //string Index_No = ""
                        string Index_No = "0";
                        string DishComment = dtDeal.Rows[i]["DishComment"].ToString();
                        string ComboAmt = dtDeal.Rows[i]["Amount"].ToString();
                        if (ComboAmt == "")
                            ComboAmt = "0";
                        double ComboRate = Convert.ToDouble(ComboAmt) / Qty;

                        //double TranQty = GetQty(ObjDgv, item_index) * Qty;
                        double TranQty = Qty;
                        if (TranQty == 0)
                        {
                            new SqlCommand();
                            cmd.CommandText = "";
                            cmd.ExecuteNonQuery();
                        }
                        sqlAssorted += "Exec Insert_AssortedN_Deals @i_code='" + i_code + "',@i_code_fk='" + i_code_fk + "',@Qty='" + TranQty +
                            "',@bill_no='" + LastBillNo + "',@created_by='admin',@bill_date='" + Program.DayEnd_BIllingDate +
                            "',@Order_Type='" + Order_Type + "',@item_index='" + item_index + "',@isDiscount='" + isDiscount + "',@Step_index='" + Index_No +
                            "',@DishComment='" + DishComment + "',@tran_rate='" + ComboRate.ToString("N3").Replace(",", "") + "',@tran_amount='" + ComboAmt.Replace(",", "") + "';";
                    }
                    int Count2 = 1;
                    if (sqlAssorted.Length > 0)
                    {
                        Count2 = 0;
                        new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sqlAssorted;
                        Count2 = Convert.ToInt32(cmd.ExecuteNonQuery());
                    }
                    string sqlSattle = string.Empty;

                    if (clsConfigSettings.Settlement_optin_HD == "0")
                    {
                        DataGridView ObjDgvPayment = new DataGridView();
                        ObjDgvPayment = clsTempData.GetDataGrid();

                        for (int rowIndex = 0; rowIndex < ObjDgvPayment.Rows.Count; rowIndex++)
                        {
                            string PaymentModeid = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["id"].Value);
                            string PaymentModeText = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["Payment Mode"].Value);
                            string Amount = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["Amount"].Value);
                            string ModeType = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["Type"].Value);
                            string hold_id = "0";
                            if (PaymentModeid == "7" || PaymentModeid == "5")
                            {
                                hold_id = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["hold_id"].Value);
                            }
                            string ModeValue = Convert.ToString(ObjDgvPayment.Rows[rowIndex].Cells["ModeValue"].Value); //Mode value use as coupons code..........
                            sqlSattle += "Exec dbo.Usp_Inserytbl_PMS @bill_no_fk='" + LastBillNo + "',@payment_mode='" + PaymentModeid + "',@Amount='" + Amount + "',@Cashier='" + Cashier +
                                 "',@card_type='" + ModeType + "',@Invoice_No='" + ModeValue + "',@hold_id='" + hold_id + "';";
                            //if (ValidateCoupon(ModeType) == true)
                            //{
                            //    sql_Coupon += "Exec Usp_UpdateCouponVaidity @Coupon_Code='" + ModeValue + "';";
                            //}
                        }
                    }

                    // payment settlemt 
                    int Count3 = 1;
                    if (sqlSattle.Length > 5)
                    {
                        Count3 = 0;
                        new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sqlSattle;
                        Count3 = Convert.ToInt32(cmd.ExecuteNonQuery());
                    }
                    // update coupon master 
                    int Count4 = 1;
                    if (sql_Coupon.Length > 10)
                    {
                        Count4 = 0;
                        new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql_Coupon;
                        Count4 = Convert.ToInt32(cmd.ExecuteNonQuery());
                    }

                    //bool OnlineUpdate = true;

                    // Packing Charges 
                    string sqlOtherCharges = string.Empty;
                    foreach (DataRow dr in PackingCharges.GetPackaingCharges.Rows)
                    {
                        string zomato_order_id = Convert.ToString(dr["Zomato_order_id"]);
                        string charge_name = Convert.ToString(dr["Charge_name"]);
                        string chargesVaue = Convert.ToString(dr["chargesValue"]);
                        string chargesTotalTax = Convert.ToString(dr["chargesTotaltax"]);
                        string I_code_fk = Convert.ToString(dr["I_Code_fk"]);

                        sqlOtherCharges += "Exec dbo.USP_Insert_OtherCharges @Zomato_order_id='" + zomato_order_id
                            + "',@Charge_name='" + charge_name + "',@chargesValue='" + chargesVaue + "',@chargesTotaltax='"
                            + chargesTotalTax + "',@I_Code_fk='" + I_code_fk + "',@Bill_no_fk='" + LastBillNo + "';";
                    }
                    int Count5 = 1;
                    if (sqlOtherCharges.Length > 0)
                    {
                        Count5 = 0;
                        new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sqlOtherCharges;
                        Count5 = Convert.ToInt32(cmd.ExecuteNonQuery());
                    }
                    int UpDateChannel = 1;
                    if (Objbill.Channel.ToLower() == "website")
                    {
                        new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "EXEC Usp_PendingOrderList_CURD @bill_date='" + Program.DayEnd_BIllingDate + "',@mode=0,@bill_no='" + LastBillNo + "',@fin_year='" + clsConfigSettings.fin_year + "',@online_bill_no='" + Objbill.bill_no_WebOrder.Replace("-", "").Trim().TrimEnd('-') + "',@Current_Status='" + EnumOrderStatus.Confirm.ToString() + "',@zomato_order_id='" + Objbill.ZomatoOrderNo.Replace("-", "") + "',@Status_Cloud_DB='1';";
                        UpDateChannel = Convert.ToInt32(cmd.ExecuteNonQuery());
                    }
                    //bool ResultobjOnline = true;
                    bool ResultobjOnline = clsUpdateOrder.UpdatOrderStatusOnlineCloudDB(Objbill.bill_no_WebOrder, LastBillNo, "Confirm", Objbill.Channel.ToLower());

                    //bool ResultobjOnline =objOnline.ExecuteDMLOnline("Usp_UpdateStatus_WebOrderWebsite @Order_No='" + Objbill.bill_no_WebOrder + "',@bill_no_local='" + LastBillNo + "',@IsManualPuch='0',@mode='Confirm'");

                    if (CountTran > 0 && Count2 > 0 && Count3 > 0 && Count4 > 0 && UpDateChannel > 0 & ResultobjOnline)
                    {
                        //commit tran
                        cmd.Transaction.Commit();
                        IsSuccess = true;
                        //if (Objbill.bill_no_WebOrder != "" && Objbill.bill_no_WebOrder != "0")
                        //{
                        //	UpdateBillStatus(Objbill.bill_no_WebOrder, LastBillNo);
                        //}
                    }
                    else
                    {
                        // rollback tran
                        cmd.Transaction.Rollback();
                        IsSuccess = false;
                    }
                }
                else
                {
                    // rollback tran
                    cmd.Transaction.Rollback();
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                // rollback tran
                cmd.Transaction.Rollback();
                IsSuccess = false;
                Loging.Log(LogType.Error, "Generate bill error  bill no=> " + ex.Message);
            }
            PackingCharges.Clear();
            Sqlcon.Close();
            Bill_No = LastBillNo;
            return IsSuccess;
        }




        private void UpdateBillStatus(string bill_no_WebOrder, string LastBillNo)
        {
            try
            {
                Thread syncThread = new Thread(() =>
                {
                    UpdateNow(bill_no_WebOrder, LastBillNo);
                });
                syncThread.Start();
            }
            catch (Exception ex)
            {

            }
        }
        private void UpdateNow(string bill_no_WebOrder, string LastBillNo)
        {
            for (int i = 0; i < 3; i++)
            {
                OrderDetailOnline objOnline = new OrderDetailOnline();
                bool Result = objOnline.ExecuteDMLOnline("Usp_UpdateStatus_WebOrder @Order_No='" + bill_no_WebOrder + "',@bill_no_local='" + LastBillNo + "',@IsManualPuch='0'");

                if (Result)
                    break;
            }
        }

        public DataSet GetOrderStatusLog()
        {
            SqlCommand cmd = null;
            DataSet ds = new DataSet();
            try
            {
                clsConnnectionManager objclsConnnectionManager = new clsConnnectionManager();
                cmd = new SqlCommand();
                cmd.CommandText = "Usp_PendingOrderList_CURD";
                cmd.Connection = objclsConnnectionManager.SQlConnection_Offline();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bill_date", Program.DayEnd_BIllingDate);
                cmd.Parameters.AddWithValue("@mode", "1");
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string id = ds.Tables[0].Rows[i]["id"].ToString();
                            string bill_no = ds.Tables[0].Rows[i]["bill_no"].ToString();
                            string fin_year = ds.Tables[0].Rows[i]["fin_year"].ToString();
                            string Status_Website_api = ds.Tables[0].Rows[i]["Status_Website_api"].ToString();
                            string online_bill_no = ds.Tables[0].Rows[i]["online_bill_no"].ToString();
                            string Status_Cloud_DB = ds.Tables[0].Rows[i]["Status_Cloud_DB"].ToString();
                            string zomato_order_id = ds.Tables[0].Rows[i]["zomato_order_id"].ToString();
                            string Current_Status = ds.Tables[0].Rows[i]["Current_Status"].ToString();
                            EnumOrderStatus OrderStatus = (EnumOrderStatus)Enum.Parse(typeof(EnumOrderStatus), Current_Status);
                            switch (OrderStatus)
                            {
                                case EnumOrderStatus.Confirm:
                                    {
                                        if (Status_Website_api == "0")
                                        {
                                            if (!OrderStatusUpdateLog.FindOrderStatus(EnumOrderStatus.Confirm, EnumStatusType.APIStatus, bill_no))
                                                clsUpdateOrder.UpdateWebsiteStatusAPI(zomato_order_id.Replace("-", "").Trim().TrimEnd('-'), bill_no, online_bill_no, EnumOrderStatus.Confirm, true);
                                        }
                                        else if (Status_Cloud_DB == "0")
                                        {
                                            clsUpdateOrder.UpdatOrderStatusOnlineCloudDB(zomato_order_id.Replace("-", "").Trim().TrimEnd('-'), bill_no, "Confirm");
                                        }
                                    }
                                    break;
                                case EnumOrderStatus.FoodReady:
                                    {
                                        if (Status_Website_api == "0")
                                        {
                                            if (!OrderStatusUpdateLog.FindOrderStatus(EnumOrderStatus.FoodReady, EnumStatusType.APIStatus, bill_no))
                                                clsUpdateOrder.UpdateWebsiteStatusAPI(zomato_order_id.Replace("-", "").Trim().TrimEnd('-'), bill_no, online_bill_no, EnumOrderStatus.FoodReady, true);

                                        }
                                        else if (Status_Cloud_DB == "0")
                                        {
                                            clsUpdateOrder.UpdatOrderStatusOnlineCloudDB(zomato_order_id.Replace("-", "").Trim().TrimEnd('-'), bill_no, "FoodReady");
                                        }
                                    }
                                    break;
                                case EnumOrderStatus.OrdderAssin:
                                    {
                                        if (Status_Website_api == "0")
                                        {
                                            if (!OrderStatusUpdateLog.FindOrderStatus(EnumOrderStatus.OrdderAssin, EnumStatusType.APIStatus, bill_no))
                                                clsUpdateOrder.UpdateWebsiteStatusAPI(zomato_order_id.Replace("-", "").Trim().TrimEnd('-'), bill_no, online_bill_no, EnumOrderStatus.OrdderAssin, true);
                                        }
                                        else if (Status_Cloud_DB == "0")
                                        {
                                            clsUpdateOrder.UpdatOrderStatusOnlineCloudDB(zomato_order_id.Replace("-", "").Trim().TrimEnd('-'), bill_no, "OrdderAssin");
                                        }
                                    }
                                    break;
                                case EnumOrderStatus.Cancel:
                                    {
                                        if (Status_Website_api == "0")
                                        {
                                            if (!OrderStatusUpdateLog.FindOrderStatus(EnumOrderStatus.Cancel, EnumStatusType.APIStatus, bill_no))
                                                clsUpdateOrder.UpdateWebsiteStatusAPI(zomato_order_id.Replace("-", "").Trim().TrimEnd('-'), bill_no, online_bill_no, EnumOrderStatus.Cancel, true);
                                        }
                                        else if (Status_Cloud_DB == "0")
                                        {
                                            clsUpdateOrder.UpdatOrderStatusOnlineCloudDB(zomato_order_id.Replace("-", "").Trim().TrimEnd('-'), bill_no, "Cancel");
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Loging.Log(LogType.Information, "GetOrderStatusLog error" + ex.Message);
            }
            return ds;
        }
    }
}