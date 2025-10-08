using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TuchScreenApp1Jan2013.App_Code;

namespace KOTPrintUtility.App_Code
{
    class clsPrintBillZomato
    {
        DataTable dt_tblTemp_id = null;
        public clsPrintBillZomato()
        {
            dt_tblTemp_id = new DataTable();
        }
        /// <summary>
        /// GetOnlineOrdersDetail
        /// </summary>
        /// <param name="Bill_no"></param>
        /// <returns></returns>
        public System.Data.DataSet GetOnlineOrdersDetail(string Bill_no)
        {
            cls_ZomatoAPi objcls_ZomatoAPi = new cls_ZomatoAPi();
            System.Data.DataSet ds = new System.Data.DataSet();
            SqlCommand cmd = null;
            try
            {
                clsConnnectionManager jojclsConnnectionManager = new clsConnnectionManager();
                string ZomatoAPi = objcls_ZomatoAPi.GetAllPunchedZomatoIds();
                cmd = new SqlCommand();
                cmd.CommandText = "Usp_Runing_Item_Web_order_AggDis";
                cmd.Connection = jojclsConnnectionManager.SQlConnection_Online();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bill_Date", Program.DayEnd_BIllingDate);
                cmd.Parameters.AddWithValue("@outlet_id", Program.Outlet_id);
                cmd.Parameters.AddWithValue("@Bill_no", Bill_no);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cmd.Connection.Close();
            }
            return ds;
        }
        /// <summary>
        /// GetOnlineOrders
        /// </summary>
        public void GetOnlineOrders()
        {
            cls_ZomatoAPi objZomato = new cls_ZomatoAPi();
            ADOC objADOC = new ADOC();
            SqlCommand cmd = null;
            DataTable dt = new DataTable();
            try
            {
                clsConnnectionManager objclsConnnectionManager = new clsConnnectionManager();
                string ZomatoAPi = objZomato.GetAllPunchedZomatoIds();
                cmd = new SqlCommand();
                cmd.CommandText = "Usp_GetTblRunning_Service";
                cmd.Connection = objclsConnnectionManager.SQlConnection_Online();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@bill_date", Program.DayEnd_BIllingDate);
                cmd.Parameters.AddWithValue("@outlet_id", Program.Outlet_id);
                cmd.Parameters.AddWithValue("@Zomato_order_id", ZomatoAPi);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string bill_no = dr["Bill_no"].ToString();
                        string Cust_MobileNo = dr["Mobile_no"].ToString().Replace("+", "").Trim();
                        string Cust_Name = dr["Cust_Name"].ToString();
                        string Address = dr["Address"].ToString();
                        string City = dr["City"].ToString();
                        string email_id = dr["email_id"].ToString().Replace("'", "");
                        string Location_Id = dr["Location_Id"].ToString();
                        string UserName = dr["UserName"].ToString();
                        string currentStatus = dr["currentStatus"].ToString();
                        string Comments = dr["Comments"].ToString().Replace("\r", " ").Trim();
                        string Order_Source_Id = dr["Order_Source_Id"].ToString();
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
                        Program.External_Source_id = dr["External_Source_id"].ToString();


                        string sqlqLocal = "usp_insert_Update_customer_Info @Name='" + Cust_Name + "',@DOB='',@Address='" + Address.Replace("'","").Trim() +
                         "',@Mobile_No='" + Cust_MobileNo.Trim() + "',@Phone_No='" + Cust_MobileNo.Trim() + "',@City='" + City + "',@created_by='" + UserName +
                         "',@Email_id='" + email_id + "',@Location='" + Location + "',@Isflag='',@Remarks='',@Land_mark='" + Land_mark.Trim() + "',@pin_no='" + pin_no.Trim() +
                         "',@cust_flag='0',@Individual_Cor_status='1',@Tin_No='',@Flat_street='" + Flat_No + "',@Latitude='" + Latitude + "',@Longitude='" + Longitude +
                         "',@location_id='" + Location_Id + "',@Stateid='" + Company_Stateid + "'";
                        string cudt_codes = objADOC.GetSingleResult(sqlqLocal);
                        //Program.Cust_code = cudt_codes;
                        string DeliveryQuery = sqlqLocal;

                        Program.Cust_code = cudt_codes;
                        string payment_mode = dr["payment_mode"].ToString();
                        if (cudt_codes != "0")
                        {
                            Program.Supply_State = Program.Company_Stateid;
                            string AnotherAddress = string.Empty;
                            string AnotherLandmark = string.Empty;
                            string AnotherPinCode = string.Empty;
                            string AddressMain = string.Empty;
                            string Tranid = "0";
                            string Addr_ID = string.Empty;
                            if (Addr_ID == "")
                            {
                                DataTable dtMultipleAddress = new DataTable();
                                dtMultipleAddress = clsTempData.GetDatatable();
                                ClsGenratebill_new objBill = new ClsGenratebill_new();
                                if (dtMultipleAddress.Rows.Count > 0)
                                {
                                    for (int k = 0; k < dtMultipleAddress.Rows.Count; k++)
                                    {
                                        AnotherAddress = dtMultipleAddress.Rows[k]["Address"].ToString();
                                        AnotherLandmark = dtMultipleAddress.Rows[k]["Landmark"].ToString();
                                        AnotherPinCode = dtMultipleAddress.Rows[k]["PinCode"].ToString();

                                        bool isInsertCustTran = objBill.InsertMultipleAddress(Addr_ID, Cust_MobileNo.Trim(), AnotherAddress,
                                            AnotherPinCode, AnotherLandmark, Address.Trim(), out Tranid, Flat_No, Location);
                                    }
                                }
                                else
                                {
                                    bool isInsertCustTran = objBill.InsertMultipleAddress(Addr_ID, Cust_MobileNo.Trim(), Address.Trim(),
                                        pin_no.Trim(), Land_mark.Trim(), Address.Trim(), out Tranid, Flat_No, Location);
                                }
                            }
                            DataTable dtNull = new DataTable();
                            clsTempData.SetDatatable(dtNull);
                        }
                        //===============================================================
                      //  GetWebOrder(bill_no, payment_mode, UserName);
                        //====================================================================
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cmd.Connection.Close();
            }
        }



        private int GenerateUniqIndex(string I_code, int IsDelete)
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
        /// GetItem_ByIndex
        /// </summary>
        /// <param name="I_code_fk"></param>
        /// <param name="dt"></param>
        /// <param name="item_index"></param>
        /// <returns></returns>
        public static DataTable GetItem_ByIndex(string I_code_fk, DataTable dt, string item_index)
        {
            DataTable dtTempData = new DataTable();
            var Query = from TempAssortedItem in dt.AsEnumerable()
                        where TempAssortedItem.Field<Int64>("i_code_fk") == Convert.ToInt64(I_code_fk) && TempAssortedItem.Field<String>("item_index") == item_index
                        select TempAssortedItem;
            if (Query.Count() > 0)
            {
                dtTempData = Query.CopyToDataTable();
            }
            return dtTempData;
        }
        /// <summary>
        /// AddAssortedItem
        /// </summary>
        /// <param name="dtAssortedItem"></param>
        /// <param name="deal"></param>
        /// <returns></returns>
        private double AddAssortedItem(DataTable dtAssortedItem, string deal,string Assorted_Item,
			string Deal_Item,string Deal_ItemIndex)

		{
            double TotalAmount = 0;
            for (int a = 0; a < dtAssortedItem.Rows.Count; a++)
            {
                string i_code_fk = dtAssortedItem.Rows[a]["i_code_fk"].ToString();
                Assorted_Item = i_code_fk;
				Deal_Item = i_code_fk;
                string i_code = dtAssortedItem.Rows[a]["i_code"].ToString();
                string qty = dtAssortedItem.Rows[a]["qty"].ToString();
                string i_name = dtAssortedItem.Rows[a]["i_name"].ToString();
                string IsDiscount = dtAssortedItem.Rows[a]["IsDiscount"].ToString();
                double amount = Convert.ToDouble(dtAssortedItem.Rows[a]["Amount"].ToString());
                string index = dtAssortedItem.Rows[a]["Index"].ToString();
				Deal_ItemIndex = index;
                string TaxRate = dtAssortedItem.Rows[a]["TaxRate"].ToString();
                //double rate=
                TotalAmount += amount;
                string Step_Name = dtAssortedItem.Rows[a]["Step_Name"].ToString();
                string No_of_Item = dtAssortedItem.Rows[a]["No_of_Item"].ToString();
                string Deal_TYpe = dtAssortedItem.Rows[a]["i_type"].ToString();
                string i_type = dtAssortedItem.Rows[a]["Deal_TYpe"].ToString();
                string Index_No = dtAssortedItem.Rows[a]["Index_No"].ToString();
                string service_tax = dtAssortedItem.Rows[a]["service_tax"].ToString();
                decimal total = Convert.ToDecimal(qty) * Convert.ToDecimal(amount);
                if (deal == "deal")
                    clsDeal.objds.Tables["tbl_deals"].Rows.Add(Assorted_Item, i_code, qty,Deal_ItemIndex, i_name, IsDiscount, amount.ToString("N3"));
                else
                    //AssortedItem.objds.Tables["tbl_AssortedItem"].Rows.Add(Program.Assorted_Item, i_code, qty, Program.Assorted_ItemIndex, i_code);
                    clsItems.GetMathod.AddItemsinList(i_name, qty.ToString(), i_code, IsDiscount, amount.ToString(), Step_Name, No_of_Item, i_type, Deal_TYpe, Deal_ItemIndex, Deal_Item, TaxRate, Index_No, "0", total.ToString("N3"), service_tax);
            }
            return TotalAmount;
        }
		//private void GetWebOrder(string Bill_No,string PaymentMode, string UserName)
		//{
		//	try
		//	{

		//		DataSet dsBillDetail = GetOnlineOrdersDetail(Bill_No);

		//		if (dsBillDetail.Tables[0].Rows.Count > 0)
		//		{
		//			//clsApplicationVariables.clsBill.Bill_TypeName = dsBillDetail.Tables[0].Rows[0]["Type"].ToString();
		//			//lblBillType.Text = clsApplicationVariables.clsBill.Bill_TypeName;
		//		string BillType = dsBillDetail.Tables[0].Rows[0]["Bill_Type"].ToString();
		//			//lblBillNo.Text = dsBillDetail.Tables[0].Rows[0]["Bill_no"].ToString();
		//			//clsApplicationVariables.clsBill.Bill_DiscountPct = dsBillDetail.Tables[0].Rows[0]["Bill_DiscountPct"].ToString();
		//			//Program.CommentType = "";
		//			Program.online_Order_comment = dsBillDetail.Tables[0].Rows[0]["Comments"].ToString();
		//			clsApplicationVariables.clsBill.Bill_DiscountAmount = dsBillDetail.Tables[0].Rows[0]["dis_amount"].ToString();
		//			Program.Source_of_order = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["Order_Source_Id"]);

		//			string Billamount = dsBillDetail.Tables[0].Rows[0]["bill_amount"].ToString();
		//			string discountby = dsBillDetail.Tables[0].Rows[0]["Discount_by"].ToString();
		//			string Bill_Punch_By = dsBillDetail.Tables[0].Rows[0]["bill_punch_by"].ToString();
		//			Program.online_Order_PunchBy = Bill_Punch_By;
		//			string Delivery_Charge = dsBillDetail.Tables[0].Rows[0]["Delivery_Charge"].ToString();
		//			Program.Delivery_Charge = Delivery_Charge;
		//			string Aggr_dis_pct = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["dis_pct_Agg"]);
		//			string Agg_Tax_Calculation = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["Agg_Tax_Calculation"]);
		//			string Aggr_Discount = Convert.ToString(dsBillDetail.Tables[0].Rows[0]["Aggr_Discount"]);
		//			Program.DiscountCouponNo = dsBillDetail.Tables[0].Rows[0]["Coupon_code"].ToString();
		//			string disType = dsBillDetail.Tables[0].Rows[0]["dis_type"].ToString();
		//			Program.online_Payment_Mode = dsBillDetail.Tables[0].Rows[0]["Payment_Mode"].ToString();

		//			string OnlineOrderNo = Bill_No;
		//			string bill_no_WebOrder = Bill_No;

		//			if (Program.online_Payment_Mode.Trim().Equals("1"))
		//			{
		//				PaymentMode = "CASH";
		//				Program.online_Payment_Mode = "COD";
		//			}
		//			else if (Program.online_Payment_Mode.Trim().Equals("3"))
		//			{
		//				PaymentMode = "CARD";
		//				Program.online_Payment_Mode = "Online";
		//			}
		//			else if (Program.online_Payment_Mode.Trim().Equals("11"))
		//			{
		//				PaymentMode = "CARD";
		//				Program.online_Payment_Mode = "PayLater";
		//			}
		//			Program.paymentModeType = PaymentMode;
		//			DataTable dtTran = dsBillDetail.Tables[1];
		//			AssortedItem.CancelAssortedItem();
		//			clsDeal.GetMathod.CancelDealItem();
		//			clsItems.GetMathod.CancelItem();
		//			GenerateUniqIndex("", 1);
		//			DataTable dtAssortedItem = dsBillDetail.Tables[2];
		//			if (dtTran.Rows.Count > 0)
		//			{
		//				//if (dgvItemDetails.Rows.Count > 0)
		//				//	dgvItemDetails.Rows.Clear();
		//				//dgvItemDetails.AutoGenerateColumns = false;
		//				for (int i = 0; i < dtTran.Rows.Count; i++)
		//				{
		//					string Assorted_Item = dtTran.Rows[i]["I_code"].ToString();
		//					string qty = dtTran.Rows[i]["qty"].ToString();
		//					string Rate = dtTran.Rows[i]["Rate"].ToString();
		//					string I_Name = dtTran.Rows[i]["I_Name"].ToString();
		//					string TaxRate = dtTran.Rows[i]["TaxRate"].ToString();
		//					string IsTaxable = dtTran.Rows[i]["IsTaxable"].ToString();
		//					decimal tax = Convert.ToDecimal(dtTran.Rows[i]["Tax"].ToString());
		//					string Discount = dtTran.Rows[i]["Discount"].ToString();
		//					string dept = dtTran.Rows[i]["dept"].ToString();
		//					string Group_Dish = dtTran.Rows[i]["Group_Dish"].ToString();
		//					string service_tax = dtTran.Rows[i]["service_tax"].ToString();
		//					string dis_rate = dtTran.Rows[i]["dis_rate"].ToString();
		//					string dis_amount = dtTran.Rows[i]["dis_amount"].ToString();
		//					//clsDeal.GetVariable.Deal_Item = Program.Assorted_Item;
		//					double Amount = Convert.ToDouble(dtTran.Rows[i]["amout"].ToString());

		//					//dgvItemDetails.Rows.Add(I_Name.Trim(), Rate, qty, tax, Amount.ToString("N3"), dtTran.Rows[i]["I_code"].ToString(),
		//					//	dtTran.Rows[i]["Comments"].ToString(), "0", dept, TaxRate, IsTaxable, Discount, dtTran.Rows[i]["I_code"].ToString(),
		//					//	dtTran.Rows[i]["I_code"].ToString(), service_tax, Group_Dish, dis_rate, "0", "0", "0", "0", "0");
		//					double TotalRate = 0;
		//					//string Item = dgvItemDetails.Rows[i].Cells["ItemName"].Value.ToString();
		//					//if (Item.Contains("+"))
		//					//{
		//					//	dgvItemDetails.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
		//					//}
		//					if (dtAssortedItem.Rows.Count > 0)
		//					{
		//						DataTable dtItem = GetItem_ByIndex(dtTran.Rows[i]["I_code"].ToString(), dtAssortedItem, Group_Dish);
		//						if (dtItem.Rows.Count > 0)
		//						{
		//							//TotalRate = AddAssortedItem(dtItem, dept,Assorted_Item,Deal_Item,Deal_ItemIndex);
		//							//if (dtItem.Rows.Count > 0)
		//							//{
		//							//	int rowcount = dgvItemDetails.Rows.Count - 1;
		//							//	dgvItemDetails.Rows[rowcount].Cells["Item_Index"].Value = clsDeal.GetVariable.Deal_ItemIndex;
		//							//}
		//							int CountItem = 0;
		//							string i_nmae = string.Empty;
		//							//DataTable dtExistingitem = clsItems.GetMathod.GetItemByIndex(clsDeal.GetVariable.Deal_Item, clsDeal.GetVariable.Deal_ItemIndex);
		//							for (int s = 0; s < dtItem.Rows.Count; s++)
		//							{
		//								string iname = dtItem.Rows[s]["i_Name"].ToString();
		//								if (iname != "NA")
		//								{
		//									CountItem++;
		//									i_nmae += "\r*" + iname + "(" + dtItem.Rows[s]["amount"].ToString() + ")";
		//								}
		//							}
		//							if (i_nmae.Length > 5)
		//							{
		//								//int rowcount = dgvItemDetails.Rows.Count - 1;
		//						//		dgvItemDetails.Rows[rowcount].Cells["ItemName"].Value = I_Name + i_nmae;
		//								//dgvItemDetails.Rows[rowcount].Height = CountItem * 18;
		//							}
		//						}
		//					}
		//					if (Convert.ToDecimal(Rate) == 0 && TotalRate > 0)
		//					{
		//					//	dgvItemDetails.Rows[i].Cells["Rate"].Value = (TotalRate / Convert.ToSingle(qty)).ToString("N3");
		//					}
		//				}
		//			}
		//			if (dsBillDetail.Tables[0].Columns.Contains("card_type") && PaymentMode == "CARD")
		//			{
		//				Program.Card_Type = dsBillDetail.Tables[0].Rows[0]["card_type"].ToString();
		//				Program.CardAmount = Billamount;
		//			}

		//			if (dgvItemDetails.Rows.Count > 0)
		//			{
		//				string SubTotal = string.Empty;
		//				string DiscountAmount = string.Empty;
		//				string NetTotal = string.Empty;
		//				string NetVAT = string.Empty;
		//				double TotalVAT = 0;
		//				string ServiceTax = string.Empty;
		//				string ServiceCharge = string.Empty;
		//				string SurCharge = string.Empty;
		//				string SBC = string.Empty;
		//				string KKC = string.Empty;
		//				string DeliveryCharge = string.Empty;
		//				string GrandTotal = string.Empty;
		//				decimal DisPCT = 0;
		//				//DLL.CalculateBillAmount(ObjDgv, out GrandTotal, out TotalVat, Convert.ToDecimal(clsApplicationVariables.clsBill.Bill_DiscountPct), out SubTotal, out ServiceCharge, out ServiceTax, out GrossSubTotal, out NetVAT);
		//				clsCalculateBill.CalculateBillDetail(dgvItemDetails, out SubTotal, DisPCT, out DiscountAmount, out NetTotal, out ServiceCharge, out ServiceTax, out NetVAT, out SurCharge, out TotalVAT, out GrandTotal, Program.BillType, out SBC, out KKC, out DeliveryCharge);

		//				bool isBillPrint = false;
		//				string paymentMode = string.Empty;
		//				switch (PaymentMode)
		//				{
		//					case "CASH":
		//						{
		//							paymentMode = "1";
		//							isBillPrint = true;
		//							Program.Card_Type = "";
		//							Program.CardAmount = "0";
		//						}
		//						break;
		//					case "CARD":
		//						{
		//							isBillPrint = true;
		//							paymentMode = "3";
		//							Program.Card_Type = Program.online_Order_PunchBy;
		//							Program.CardAmount = GrandTotal;
		//						}
		//						break;
		//				}

		//				//string DiscountPctRs = "0", DiscountPctRsShow = "0";
		//				//if (DiscountAmount == "0")
		//				//{
		//				//    DiscountPctRs = clsApplicationVariables.cls_DiscountForm.Bill_Discount_Amt;
		//				//}
		//				//else
		//				//{
		//				//    DiscountPctRs = DiscountAmt;
		//				//    DiscountPctRsShow = Program.Discount_Amt_Show;
		//				//}


		//				if (isBillPrint == true)
		//				{
		//					//string orderNo = string.Empty;
		//					//orderNo = ADOC.GetObject.GetSingleResult("GetTockenNo");
		//					DataGridView objDGV = new DataGridView();
		//					objDGV.AllowUserToAddRows = false;
		//					if (Convert.ToDouble(Program.CardAmount) > 0)
		//					{
		//						clsTempData.InsertInGrid(objDGV, "3", "CARD", Program.online_Order_PunchBy, Program.CardAmount, "0", "");
		//					}

		//					else if (paymentMode != "9")
		//					{
		//						clsTempData.InsertInGrid(objDGV, "1", "CASH", "", GrandTotal, "", "");
		//					}
		//					clsTempData.SetDataGrid(objDGV);
		//					// SubTotal = SubTotalAfterDiscount;

		//					double tamt = double.Parse(SubTotal) + double.Parse(NetVAT);
		//					double totalCardbalance = Convert.ToDouble(GrandTotal);
		//					totalCardbalance = Math.Round(totalCardbalance, 0, MidpointRounding.AwayFromZero);
		//					double TempGTotal_Amount = (double.Parse(SubTotal) + Convert.ToDouble(NetVAT) + Convert.ToDouble(DeliveryCharge)) - Convert.ToDouble(DiscountAmount);
		//					string TotalAmount = Math.Round(Convert.ToDouble(TempGTotal_Amount.ToString("N2")), 0, MidpointRounding.AwayFromZero).ToString("N2");
		//					double ReplaceAmount = Convert.ToDouble(TotalAmount) - TempGTotal_Amount;          //Round of f amount


		//					bool isBillInsert = false;
		//					/////////////insert through Procedure
		//					string POSCode = "S";
		//					//string TockenNo = DLL.GetNewTokenNo();
		//					string Bill_NoLocal = string.Empty;
		//					string Discounted_by = string.Empty;
		//					if (DiscountAmount != "0")
		//					{
		//						Discounted_by = UserName;

		//					}
		//					string sql = string.Empty;

		//					isBillInsert = false;


		//					string hold_dept = "1";//string.Empty;

		//					clsGenerateBillSweets objGenerateBill = new clsGenerateBillSweets();


		//					isBillInsert = objGenerateBill.GenerateTakeAwayBill(POSCode, Program.BillType, Program.DayEnd_BIllingDate, GrandTotal, DiscountAmount, ServiceCharge
		//					, GrandTotal, totalCardbalance.ToString(), paymentMode, "", dgvItemDetails.Rows.Count.ToString(), "", Program.online_Order_comment, UserName
		//					, NetVAT, ReplaceAmount.ToString("N3"), Discounted_by, "", ServiceCharge, "", "", DisPCT.ToString(), NetTotal, Program.Card_Type,
		//					"", hold_dept, "0", "0", dgvItemDetails, out Bill_No, "0", "0", "0"
		//					, "0", "", DeliveryCharge, "0", SubTotal.Replace(",", ""), "0", "", "", UserName);


		//					if (isBillInsert == true && Bill_No != "")
		//					{
		//						dgvItemDetails.Rows.Clear();
		//						//Program.discounted_by = string.Empty;
		//						//Program.BtcApprovedBy = string.Empty;
		//						//Program.DepartmentName = string.Empty;
		//						//Program.CategoryName = string.Empty;
		//						//Program.GetTaxRate = string.Empty;
		//						PackingCharges.Aggr_dis_pct = "0";
		//						PackingCharges.Agg_Tax_Calculation = "0";
		//						Application.DoEvents();
		//						//dgvItemDetails.Rows.Clear();
		//					}
		//				}
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		//System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true); Cls_Exception.InsertException("1", Program.Bill_Date, Program.UserName, ex.Message, trace.GetFrame(0).GetFileLineNumber().ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, this.Name);
		//		//MessageBox.Show(ex.Message + " Page & Fun details :- " + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, this.Name);
		//	}
		//}
	}
}
