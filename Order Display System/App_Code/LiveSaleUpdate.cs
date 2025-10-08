using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Script.Serialization;

namespace KOTPrintUtility.App_Code
{
	class LiveSaleUpdate
	{
		private Timer tmrStartLiveSale;
		public async Task StatrtUploadSale()
		{
			Loging.Log(LogType.Information, "Sale Upload started");
			Upload();
			// Initialize the timer
			tmrStartLiveSale = new Timer(150000); // 15 sec
			tmrStartLiveSale.Elapsed += async (s, e) =>
			{
				try
				{
					tmrStartLiveSale.Stop();
					await Upload();
				}
				catch (Exception ex)
				{
					Loging.Log(LogType.Error, "Timer Tick error: " + ex.Message);
				}
				finally
				{
					tmrStartLiveSale.Start();
				}
			};
			tmrStartLiveSale.AutoReset = true;
			tmrStartLiveSale.Start();
		}


		private static async Task Upload()
		{
			Loging.Log(LogType.Information, "LiveSale.Upload.start");
			SqlCommand cmd = new SqlCommand();
			DataSet ds = new DataSet();
			clsBillList data = new clsBillList();
			try
			{
				cmd.CommandText = "Usp_GetBill_Detail_LiveSale";
				SqlConnection con = new SqlConnection(Program.sqlkey);
				con.Open();
				cmd.Connection = con;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@IsBillSattle", "1");
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				adp.Fill(ds);
				if (ds.Tables.Count > 0)
				{
					List<cls_tbl_bill> lstBill = new List<cls_tbl_bill>();
					List<cls_tbl_billTran> lstBillTraan = new List<cls_tbl_billTran>();
					List<tbl_PaymentSettlement> lstPaymt = new List<tbl_PaymentSettlement>();
					List<clsTax> lsttx = new List<clsTax>();
					var objbill = new clsBillList();
					objbill.custs = new List<clsCustomer>();
					objbill.expm = new List<clsExpMaster>();
					List<tblAssortedItem> lstAssorted = new List<tblAssortedItem>();


					if (ds.Tables[0].Rows.Count > 0)
					{
						#region ===============tbl_bill====================================
						var dttbl_bill = ds.Tables[0];
						for (int billIndex = 0; billIndex < dttbl_bill.Rows.Count; billIndex++)
						{
							var b = new cls_tbl_bill();
							b.POS_code = dttbl_bill.Rows[billIndex]["POS_code"].ToString();
							b.Bill_Date = dttbl_bill.Rows[billIndex]["Bill_date"].ToString();
							b.Bill_Type = dttbl_bill.Rows[billIndex]["Bill_Type"].ToString();
							b.Bill_No_local = dttbl_bill.Rows[billIndex]["Bill_No"].ToString();
							b.Cust_Code = dttbl_bill.Rows[billIndex]["Cust_Code"].ToString();
							b.Table_No = dttbl_bill.Rows[billIndex]["Table_No"].ToString();
							b.Bill_Amount = dttbl_bill.Rows[billIndex]["Bill_Amount"].ToString();
							b.Dis_Amount = dttbl_bill.Rows[billIndex]["Dis_Amount"].ToString();
							b.Service_Charge_Amount = dttbl_bill.Rows[billIndex]["Service_Charge_Amount"].ToString();
							b.Service_ch_Rate = dttbl_bill.Rows[billIndex]["Service_Charge_Rate"].ToString();
							b.TaxSale = dttbl_bill.Rows[billIndex]["Tax_Sale1"].ToString();
							b.Tax_Rate1 = dttbl_bill.Rows[billIndex]["Tax_Rate1"].ToString();
							b.Tax1 = dttbl_bill.Rows[billIndex]["Tax1"].ToString();
							b.Other_Charge = dttbl_bill.Rows[billIndex]["Other_Charge"].ToString();
							b.No_of_Item = dttbl_bill.Rows[billIndex]["No_of_Item"].ToString();
							b.Grand_Total = dttbl_bill.Rows[billIndex]["Grand_Total"].ToString();
							b.Payment_Mode = dttbl_bill.Rows[billIndex]["Payment_Mode"].ToString();
							b.Card_No = dttbl_bill.Rows[billIndex]["Card_No"].ToString();
							b.Paid_Amount = dttbl_bill.Rows[billIndex]["Paid_Amount"].ToString();
							b.Balance = dttbl_bill.Rows[billIndex]["Balance"].ToString();
							b.Bill_Status = dttbl_bill.Rows[billIndex]["Bill_Status"].ToString();
							b.Tips = dttbl_bill.Rows[billIndex]["Tips"].ToString();
							b.To_Whom = dttbl_bill.Rows[billIndex]["To_Whom"].ToString();
							b.Nos_Of_Printing = dttbl_bill.Rows[billIndex]["Nos_Of_Printing"].ToString();
							b.CancelDate = dttbl_bill.Rows[billIndex]["Cancelled_Date"].ToString();
							b.CamcelTime = dttbl_bill.Rows[billIndex]["Cancelled_Time"].ToString();
							b.cancel_reason = dttbl_bill.Rows[billIndex]["Cancellation_Reason"].ToString();
							b.cancel_amt = dttbl_bill.Rows[billIndex]["Cancelled_Amount"].ToString();
							b.timeout = dttbl_bill.Rows[billIndex]["TimeOut"].ToString();
							b.timeIN = dttbl_bill.Rows[billIndex]["TimeIn"].ToString();
							b.Waiter_Code = dttbl_bill.Rows[billIndex]["Waiter_Code"].ToString();
							b.Tot_amt = dttbl_bill.Rows[billIndex]["TOT_Amount"].ToString();
							b.IP = dttbl_bill.Rows[billIndex]["IP"].ToString();
							b.Reprint_by = dttbl_bill.Rows[billIndex]["Reprint_by"].ToString();
							b.Cancelled_By = dttbl_bill.Rows[billIndex]["Cancelled_By"].ToString();
							b.Bill_Modify_by = dttbl_bill.Rows[billIndex]["Bill_Modify_by"].ToString();
							b.bill_Modification_Reason = dttbl_bill.Rows[billIndex]["bill_Modification_Reason"].ToString();
							b.Comments = dttbl_bill.Rows[billIndex]["Comments"].ToString();
							b.Advance = dttbl_bill.Rows[billIndex]["Advance"].ToString();
							b.Orderno = dttbl_bill.Rows[billIndex]["OrderNo"].ToString();
							b.Order_Date = dttbl_bill.Rows[billIndex]["OrderDate"].ToString();
							b.Cashier = dttbl_bill.Rows[billIndex]["Cashier"].ToString();
							b.createdOn = dttbl_bill.Rows[billIndex]["createdOn"].ToString();
							b.IsBillSattle = dttbl_bill.Rows[billIndex]["IsBillSattle"].ToString();
							b.RoundOff = dttbl_bill.Rows[billIndex]["RoundOff"].ToString();
							b.card_amt = dttbl_bill.Rows[billIndex]["card_amt"].ToString();
							b.TockenNo = dttbl_bill.Rows[billIndex]["TockenNo"].ToString();
							b.Discount_by = dttbl_bill.Rows[billIndex]["Discount_by"].ToString();
							b.Service_Charge = dttbl_bill.Rows[billIndex]["Service_Charge"].ToString();
							b.Upflag = dttbl_bill.Rows[billIndex]["up_flag"].ToString();
							b.table_coverd = dttbl_bill.Rows[billIndex]["Table_Covered"].ToString();
							b.HoldSta = dttbl_bill.Rows[billIndex]["Hold_Status"].ToString();
							b.Service_tax = dttbl_bill.Rows[billIndex]["Service_Tax"].ToString();
							b.card_type = dttbl_bill.Rows[billIndex]["card_type"].ToString();
							b.sur_charge = dttbl_bill.Rows[billIndex]["sur_charge"].ToString();
							b.Reprint_Time = dttbl_bill.Rows[billIndex]["Reprint_Time"].ToString();
							b.Reprint_Reason = dttbl_bill.Rows[billIndex]["Reprint_Reason"].ToString();
							b.outlet = dttbl_bill.Rows[billIndex]["outlet_id"].ToString();
							b.Subtot = dttbl_bill.Rows[billIndex]["subTotal"].ToString();
							b.DeptHead = dttbl_bill.Rows[billIndex]["dept_head"].ToString();
							b.Modified_amount = dttbl_bill.Rows[billIndex]["Modified_amount"].ToString();
							b.modified_date = dttbl_bill.Rows[billIndex]["modified_date"].ToString();
							b.isModify = dttbl_bill.Rows[billIndex]["isModify"].ToString();
							b.Order_Status = dttbl_bill.Rows[billIndex]["Order_Status"].ToString();
							b.Order_View_by = dttbl_bill.Rows[billIndex]["Order_View_by"].ToString();
							b.Order_View_Time = dttbl_bill.Rows[billIndex]["Order_View_Time"].ToString();
							b.Order_Done_Time = dttbl_bill.Rows[billIndex]["Order_Done_Time"].ToString();
							b.Order_Done_by = dttbl_bill.Rows[billIndex]["Order_Done_by"].ToString();
							b.Dispatch_by = dttbl_bill.Rows[billIndex]["Dispatch_by"].ToString();
							b.Dispatch_Time = dttbl_bill.Rows[billIndex]["Dispatch_Time"].ToString();
							b.Allocated_To = dttbl_bill.Rows[billIndex]["Allocated_To"].ToString();
							b.Allocated_Time = dttbl_bill.Rows[billIndex]["Allocated_Time"].ToString();
							b.Allocated_by = dttbl_bill.Rows[billIndex]["Allocated_by"].ToString();
							b.Change_Amount = dttbl_bill.Rows[billIndex]["Change_Amount"].ToString();
							b.dis_type = dttbl_bill.Rows[billIndex]["dis_type"].ToString();
							b.Coupon_code = dttbl_bill.Rows[billIndex]["Coupon_code"].ToString();
							b.Web_order_no = dttbl_bill.Rows[billIndex]["Web_order_no"].ToString();
							b.web_order_comments = dttbl_bill.Rows[billIndex]["web_order_comments"].ToString();
							b.SBC_Tax = dttbl_bill.Rows[billIndex]["SBC_Tax"].ToString();
							b.KKC_Tax = dttbl_bill.Rows[billIndex]["KKC_Tax"].ToString();
							b.source_of_order = dttbl_bill.Rows[billIndex]["source_of_order"].ToString();
							b.WebOrder_No = dttbl_bill.Rows[billIndex]["WebOrder_No"].ToString();
							b.zomato_order_id = dttbl_bill.Rows[billIndex]["zomato_order_id"].ToString();
							b.IsManualPuch = dttbl_bill.Rows[billIndex]["IsManualPuch"].ToString();
							b.Online_bill_PunchBy = dttbl_bill.Rows[billIndex]["Online_bill_PunchBy"].ToString();
							b.loyality_type = dttbl_bill.Rows[billIndex]["loyality_type"].ToString();
							b.loyality_rate = dttbl_bill.Rows[billIndex]["loyality_rate"].ToString();
							b.loyality_Point_CR = dttbl_bill.Rows[billIndex]["loyality_Point_CR"].ToString();
							b.loyality_Point_DR = dttbl_bill.Rows[billIndex]["loyality_Point_DR"].ToString();
							b.Total_Points_Redeemed = dttbl_bill.Rows[billIndex]["Total_Points_Redeemed"].ToString();
							b.Delivery_By = dttbl_bill.Rows[billIndex]["Delivery_By"].ToString();
							b.Channel = dttbl_bill.Rows[billIndex]["Channel"].ToString();
							b.Channel_Order_Id = dttbl_bill.Rows[billIndex]["Channel_Order_Id"].ToString();
							b.is_api_sync = dttbl_bill.Rows[billIndex]["is_api_sync"].ToString();
							b.Aggr_dis_amount = dttbl_bill.Rows[billIndex]["Aggr_dis_amount"].ToString();
							b.Aggr_dis_pct = dttbl_bill.Rows[billIndex]["Aggr_dis_pct"].ToString();
							b.IsModifyFlag = Convert.ToInt32(dttbl_bill.Rows[billIndex]["IsModifyFlag"]);
							b.bill_DiscountPct = dttbl_bill.Rows[billIndex]["bill_DiscountPct"].ToString();
							lstBill.Add(b);
						}
						#endregion

						#region  =======================tbl_billTran======================
						var dtonjtran = ds.Tables[1];
						for (int rowIndex = 0; rowIndex < dtonjtran.Rows.Count; rowIndex++)
						{
							var trn = new cls_tbl_billTran();
							trn.bill_no_fk = dtonjtran.Rows[rowIndex]["bill_no_fk"].ToString();
							trn.ItemCode = dtonjtran.Rows[rowIndex]["Item_Code"].ToString();
							trn.Qty = dtonjtran.Rows[rowIndex]["Qty"].ToString();
							trn.Price = dtonjtran.Rows[rowIndex]["Rate"].ToString();
							trn.TotalAmt = dtonjtran.Rows[rowIndex]["Amount"].ToString();
							trn.Bill_Date = dtonjtran.Rows[rowIndex]["bill_date"].ToString();
							trn.Kot_date = dtonjtran.Rows[rowIndex]["kot_date"].ToString();
							trn.Comments = dtonjtran.Rows[rowIndex]["Comments"].ToString();
							trn.Dis_Rate = dtonjtran.Rows[rowIndex]["Dis_Rate"].ToString();
							trn.Dis_Cycling = dtonjtran.Rows[rowIndex]["Dis_Cycling"].ToString();
							trn.Dis_Code = dtonjtran.Rows[rowIndex]["Dis_Code"].ToString();
							trn.Dis_Amount1 = dtonjtran.Rows[rowIndex]["Dis_Amount"].ToString();
							trn.Tax_Rate = dtonjtran.Rows[rowIndex]["Tax_Rate"].ToString();
							trn.Sale_Tax_Cycling = dtonjtran.Rows[rowIndex]["Sale_Tax_Cycling"].ToString();
							trn.Taxable_Amount = dtonjtran.Rows[rowIndex]["Taxable_Amount"].ToString();
							trn.Tax_Amount = dtonjtran.Rows[rowIndex]["Tax_Amount"].ToString();
							trn.Group_Dish = dtonjtran.Rows[rowIndex]["Group_Dish"].ToString();
							trn.cancel_qty = dtonjtran.Rows[rowIndex]["cancel_qty"].ToString();
							trn.item_addon = dtonjtran.Rows[rowIndex]["item_addon"].ToString();
							trn.head_id = dtonjtran.Rows[rowIndex]["head_id"].ToString();
							trn.order_status_tran = dtonjtran.Rows[rowIndex]["order_status_tran"].ToString();
							trn.Done_Time = dtonjtran.Rows[rowIndex]["Done_Time"].ToString();
							trn.STax_Rate = dtonjtran.Rows[rowIndex]["STax_Rate"].ToString();
							trn.SBC_Rate = dtonjtran.Rows[rowIndex]["SBC_Rate"].ToString();
							trn.SBC_Amount = dtonjtran.Rows[rowIndex]["SBC_Amount"].ToString();
							trn.KKC_Rate = dtonjtran.Rows[rowIndex]["KKC_Rate"].ToString();
							trn.KKC_Amount = dtonjtran.Rows[rowIndex]["KKC_Amount"].ToString();
							trn.Urgent_Item = dtonjtran.Rows[rowIndex]["Urgent_Item"].ToString();
							trn.addon_index = dtonjtran.Rows[rowIndex]["addon_index"].ToString();
							trn.addon_index_fk = dtonjtran.Rows[rowIndex]["addon_index_fk"].ToString();
							trn.user = dtonjtran.Rows[rowIndex]["cashier"].ToString();
							trn.load_time = dtonjtran.Rows[rowIndex]["load_time"].ToString();
							trn.Ready_Time = dtonjtran.Rows[rowIndex]["Ready_Time"].ToString();
							trn.Turnarround_Time = dtonjtran.Rows[rowIndex]["Turnarround_Time"].ToString();
							lstBillTraan.Add(trn);
						}
						#endregion

						#region =======================tbl_PaymentSettlement======================
						var dtPayment = ds.Tables[2];
						for (int i = 0; i < dtPayment.Rows.Count; i++)
						{
							var pmt = new tbl_PaymentSettlement();
							pmt.bill_no_fk = dtPayment.Rows[i]["bill_no_fk"].ToString();
							pmt.payment_mode = dtPayment.Rows[i]["payment_mode"].ToString();
							pmt.Amount = dtPayment.Rows[i]["Amount"].ToString();
							pmt.Cashier = dtPayment.Rows[i]["Cashier"].ToString();
							pmt.cardtype = dtPayment.Rows[i]["card_type"].ToString();
							pmt.Invoice_No = dtPayment.Rows[i]["Invoice_No"].ToString();
							pmt.Bill_Date = dtPayment.Rows[i]["Bill_Date"].ToString();
							pmt.hold_id_fk = dtPayment.Rows[i]["hold_id"].ToString();
							pmt.created_on = dtPayment.Rows[i]["created_on"].ToString();
							lstPaymt.Add(pmt);
						}
						#endregion

						#region =======================tblAssortedItem======================
						var dtassorted = ds.Tables[3];
						for (int i = 0; i < dtassorted.Rows.Count; i++)
						{
							var assorted = new tblAssortedItem();
							assorted.bill_no = dtassorted.Rows[i]["bill_no"].ToString();
							assorted.id = dtassorted.Rows[i]["id"].ToString();
							assorted.i_code = dtassorted.Rows[i]["i_code"].ToString();
							assorted.i_code_fk = dtassorted.Rows[i]["i_code_fk"].ToString();
							assorted.Qty = dtassorted.Rows[i]["Qty"].ToString();
							assorted.bill_no = dtassorted.Rows[i]["bill_no"].ToString();
							assorted.bill_date = dtassorted.Rows[i]["bill_date"].ToString();
							assorted.created_by = dtassorted.Rows[i]["created_by"].ToString();
							assorted.created_on = dtassorted.Rows[i]["created_on"].ToString();
							assorted.cancel_qty = dtassorted.Rows[i]["cancel_qty"].ToString();
							assorted.Order_Type = dtassorted.Rows[i]["Order_Type"].ToString();
							assorted.item_index = dtassorted.Rows[i]["item_index"].ToString();
							assorted.isDiscount = dtassorted.Rows[i]["isDiscount"].ToString();
							assorted.Order_status = dtassorted.Rows[i]["Order_status"].ToString();
							assorted.Step_index = dtassorted.Rows[i]["Step_index"].ToString();
							assorted.CancelType = dtassorted.Rows[i]["CancelType"].ToString();
							assorted.DishComment = dtassorted.Rows[i]["DishComment"].ToString();
							assorted.tran_rate = dtassorted.Rows[i]["tran_rate"].ToString();
							assorted.tran_amount = dtassorted.Rows[i]["tran_amount"].ToString();
							lstAssorted.Add(assorted);
						}
						#endregion

						#region =======================tbl_bill_tax======================
						var dtBill_Tax = ds.Tables[4];
						for (int i = 0; i < dtBill_Tax.Rows.Count; i++)
						{
							var tx = new clsTax();
							tx.bill_no_fk = dtBill_Tax.Rows[i]["bill_no_fk"].ToString();
							tx.Bill_Date = dtBill_Tax.Rows[i]["Bill_Date"].ToString();
							tx.Item_Code_FK = dtBill_Tax.Rows[i]["Item_Code_FK"].ToString();
							tx.Taxable_Amt = dtBill_Tax.Rows[i]["Taxable_Amt"].ToString();
							tx.Tax_Rate = dtBill_Tax.Rows[i]["Tax_Rate"].ToString();
							tx.Tax_Amt = dtBill_Tax.Rows[i]["Tax_Amt"].ToString();
							tx.Tax_Code = dtBill_Tax.Rows[i]["Tax_Code"].ToString();
							tx.Tax_Name = dtBill_Tax.Rows[i]["Tax_Name"].ToString();
							tx.Tax_Type = dtBill_Tax.Rows[i]["Tax_Type"].ToString();
							tx.Tax_Calc_Formula = dtBill_Tax.Rows[i]["Tax_Calc_Formula"].ToString();
							tx.Tax_display_name = dtBill_Tax.Rows[i]["Tax_display_name"].ToString();
							tx.Group_Name = dtBill_Tax.Rows[i]["Group_Name"].ToString();
							tx.IsAssorted = dtBill_Tax.Rows[i]["IsAssorted"].ToString();
							tx.AmountBeforeTax = dtBill_Tax.Rows[i]["AmountBeforeTax"].ToString();
							tx.AmountAfterTax = dtBill_Tax.Rows[i]["AmountAfterTax"].ToString();
							tx.Cashier = dtBill_Tax.Rows[i]["cashier"].ToString();
							tx.Created_on = dtBill_Tax.Rows[i]["Created_on"].ToString();
							tx.Updated_by = dtBill_Tax.Rows[i]["Updated_by"].ToString();
							tx.Updated_on = dtBill_Tax.Rows[i]["Updated_on"].ToString();
							lsttx.Add(tx);
						}
						#endregion


						if (ds.Tables.Count > 5)
						{
							var objdt = ds.Tables[5];
							for (int i = 0; i < objdt.Rows.Count; i++)
							{
								var cust = new clsCustomer();
								cust.GuestID = objdt.Rows[i]["ID"].ToString();
								cust.Name = objdt.Rows[i]["Name"].ToString();
								cust.Phone = objdt.Rows[i]["Phone_No"].ToString();
								cust.mobile = objdt.Rows[i]["Mobile_No"].ToString();
								cust.Address = objdt.Rows[i]["Address"].ToString();
								cust.dob = objdt.Rows[i]["DOB"].ToString();
								cust.city = objdt.Rows[i]["City"].ToString();
								cust.Createby = objdt.Rows[i]["Created_by"].ToString();

								cust.email_id = objdt.Rows[i]["email_id"].ToString();
								cust.location_id = objdt.Rows[i]["location_id"].ToString();
								cust.Colony_id = objdt.Rows[i]["Colony_id"].ToString();
								cust.Created_on = objdt.Rows[i]["Created_on"].ToString();
								cust.Location = objdt.Rows[i]["Location"].ToString();
								cust.Remarks = objdt.Rows[i]["Remarks"].ToString();
								cust.Tin_no = objdt.Rows[i]["Tin_no"].ToString();
								cust.SourceOfOrder = objdt.Rows[i]["SourceOfOrder"].ToString();
								cust.state_id_fk = objdt.Rows[i]["state_id_fk"].ToString();
								cust.Anniversity = objdt.Rows[i]["Anniversity"].ToString();
								cust.NameofInstitution = objdt.Rows[i]["NameofInstitution"].ToString();
								cust.gender = objdt.Rows[i]["gender"].ToString();
								cust.Modify_date = objdt.Rows[i]["Modify_date"].ToString();
								cust.PIN_Code = objdt.Rows[i]["PIN_Code"].ToString();
								cust.Age = objdt.Rows[i]["Age"].ToString();
								cust.Income_Range = objdt.Rows[i]["Income_Range"].ToString();
								cust.City_id = objdt.Rows[i]["City_id"].ToString();
								objbill.custs.Add(cust);
							}
						}

						if (ds.Tables.Count > 5)
						{
							var dt = ds.Tables[6];
							for (int i = 0; i < dt.Rows.Count; i++)
							{
								var expMast = new clsExpMaster();
								expMast.id = dt.Rows[i]["id"].ToString();
								expMast.Bill_No = dt.Rows[i]["bill_no"].ToString();
								expMast.Date = dt.Rows[i]["datr1"].ToString();
								expMast.acode = dt.Rows[i]["acode"].ToString();
								expMast.bill_date = dt.Rows[i]["bill_date"].ToString();
								expMast.paid_amt = dt.Rows[i]["paid_amt"].ToString();
								expMast.paid_date = dt.Rows[i]["paid_date"].ToString();
								expMast.pay_type = dt.Rows[i]["pay_type"].ToString();
								expMast.chq_no = dt.Rows[i]["chq_no"].ToString();
								expMast.bank_acode = dt.Rows[i]["bank_acode"].ToString();
								expMast.remark = dt.Rows[i]["remark"].ToString();
								expMast.createdby = dt.Rows[i]["createdby"].ToString();
								expMast.createdDate = dt.Rows[i]["createdDate"].ToString();
								objbill.expm.Add(expMast);
							}
						}
						if (lstBill.Count > 0)
						{
							objbill.bill = new List<cls_tbl_bill>();
							objbill.outlet_id = Program.Outlet_id;
							objbill.sqlcon = Program.sqlKeyOnline;

							List<cls_tbl_bill> UniqueBill = lstBill.GroupBy(x => x.Bill_No_local).Select(g => g.First()).ToList();
							for (int b = 0; b < UniqueBill.Count; b++)
							{
								var bl = new cls_tbl_bill();
								var bill_no = UniqueBill[b].Bill_No_local;

								bl = UniqueBill.Find(x => x.Bill_No_local == bill_no);
								bl.tran = lstBillTraan.FindAll(x => x.bill_no_fk == bill_no).ToList();
								bl.taxes = lsttx.FindAll(x => x.bill_no_fk == bill_no).ToList();
								bl.payment = lstPaymt.FindAll(x => x.bill_no_fk == bill_no).ToList();
								bl.assortedItem = lstAssorted.FindAll(x => x.bill_no == bill_no).ToList();
								objbill.bill.Add(bl);
							}
							//JavaScriptSerializer js = GetZomatoReferenceClass.GetObject_JavaScriptSerializer;
							//string jsonData = js.Serialize(objbill);

							//await SendJsonToApi(objbill, "http://thirdpartyapi.skipthequeue.in");
							if (Program.LiveSaleUploadAPI == "")
							{
								Program.LiveSaleUploadAPI = "http://thirdpartyapi.skipthequeue.in";
							}
							await SendJsonToApi(objbill, Program.LiveSaleUploadAPI);
							//await SendJsonToApi(objbill, "http://localhost:2753/");
							Loging.Log(LogType.EndTransaction, "LiveSale.Upload.End");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Loging.Log(LogType.Error, "Upload error 1" + ex.Message);
			}
			finally
			{
				cmd.Connection.Close();
				cmd.Connection.Dispose();
				cmd.Dispose();
			}
		}

		private static async Task SendJsonToApi(clsBillList data, string APIBaseURL)
		{
			try
			{
				try
				{
					Loging.Log(LogType.Information, "LiveSale.SendJsonToApi.start");
					string apiUrl = APIBaseURL.TrimEnd('/') + "/api/UploadLiveSale/POS";					
					CallAPI(apiUrl, data, "55");
					Loging.Log(LogType.Information, "LiveSale.SendJsonToApi.end");
				}
				catch (TaskCanceledException ex)
				{
					Console.WriteLine("[TIMEOUT] Request took too long: " + ex.Message);
				}
			}
			catch (HttpRequestException ex)
			{
				Console.WriteLine("[HTTP ERROR] " + ex.Message);
			}

		}
		private static async Task<string> PostDataAsync(string endpoint, string data, string request_id)
		{
			string error = string.Empty;
			try
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

				using (var client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromSeconds(60);
					client.DefaultRequestHeaders.ExpectContinue = false;
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
					{
						Content = new StringContent(data, Encoding.UTF8, "application/json")
					};

					Loging.Log(LogType.Information, $"[{request_id}] Sending request to: {endpoint}");

					using (var httpResponse = await client.SendAsync(request).ConfigureAwait(false))
					{
						Loging.Log(LogType.Information, $"[{request_id}] Response received: {httpResponse.StatusCode}");
						string response = await httpResponse.Content.ReadAsStringAsync();

						ResponseAPI res = JsonConvert.DeserializeObject<ResponseAPI>(response);

						if (res?.status == true)
						{
							Loging.Log(LogType.EndTransaction, "Upload Completed");
							StringBuilder sql = new StringBuilder();

							foreach (var bl in res.bill_Insert.Where(b => b.IsUpdated))
								sql.AppendLine($"EXEC Usp_UpdateBillstatus @bill_No = '{bl.bill_No}';");

							foreach (var bl in res.bill_Update.Where(b => b.IsUpdated))
								sql.AppendLine($"EXEC Usp_UpdateBillstatus @bill_No = '{bl.bill_No}';");

							foreach (var bl in res.cust.Where(b => b.IsUpdated))
								sql.AppendLine($"EXEC Usp_UpdateGuestData @guestID = '{bl.id}';");

							foreach (var bl in res.Exp.Where(b => b.IsUpdated))
								sql.AppendLine($"EXEC Usp_UpdateExpenseMaster @id = '{bl.id}';");

							if (sql.Length > 0)
							{
								SqlSupport objcls = new SqlSupport();
								objcls.ExecuteNonQueryBySqlQuery(sql.ToString());
							}
						}
						return response;
					}
				}
			}
			catch (Exception ex)
			{
				error = "PostDataAsync error " + ex.Message;
				if (ex.InnerException != null)
					error += " InnerException: " + ex.InnerException.Message;
			}
			return "{\"error\":\"" + error + "\"}";
		}


		public static bool CallAPI(string endpoint, clsBillList data, string request_id)
		{
			bool result = false;
			try
			{
				Loging.Log(LogType.Information, "LiveSale.CallAPI.start");
				if (data != null)
				{					
					ResponseAPI res = GetZomatoReferenceClass.GetObject_ZomatoAPi.ExecuteAPIGlobal(endpoint, data, "", "", "POST");
					if (res.status)
					{
						Loging.Log(LogType.Information, "Upload Completed");
						StringBuilder sql = new StringBuilder();

						foreach (var bl in res.bill_Insert.Where(b => b.IsUpdated))
							sql.AppendLine($"EXEC Usp_UpdateBillstatus @bill_No = '{bl.bill_No}';");

						foreach (var bl in res.bill_Update.Where(b => b.IsUpdated))
							sql.AppendLine($"EXEC Usp_UpdateBillstatus @bill_No = '{bl.bill_No}';");


						foreach (var bl in res.cust.Where(b => b.IsUpdated))
							sql.AppendLine($"EXEC Usp_UpdateGuestData @guestID = '{bl.id}';");

						foreach (var bl in res.Exp.Where(b => b.IsUpdated))
							sql.AppendLine($"EXEC Usp_UpdateExpenseMaster @id = '{bl.id}';");

						if (sql.Length > 0)
						{
							SqlSupport objcls = new SqlSupport();
							result=objcls.ExecuteNonQueryBySqlQuery(sql.ToString());
						}
					}
				}
				Loging.Log(LogType.Information, "LiveSale.CallAPI.end");
			}
			catch (Exception x)
			{
				Loging.Log(LogType.Information, "LiveSale.CallAPI error "+x.Message);
			}
			return result;
		}
		//private static async Task<string> PostDataAsync(string endpoint, string data, string request_id)
		//{
		//    string error = string.Empty;
		//    try
		//    {
		//        using (var client = new HttpClient())
		//        {
		//            client.Timeout = TimeSpan.FromSeconds(5000);
		//            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
		//            request.Headers.Add("Accept", "application/json");

		//            // Send JSON with proper Content-Type
		//            var content = new StringContent(data, Encoding.UTF8, "application/json");
		//            request.Content = content;

		//            using (HttpResponseMessage httpResponse = await client.SendAsync(request).ConfigureAwait(false))
		//            {
		//                string response = await httpResponse.Content.ReadAsStringAsync();
		//                ResponseAPI res = JsonConvert.DeserializeObject<ResponseAPI>(response);
		//                if (res.status)
		//                {
		//                    Loging.Log(LogType.EndTransaction, "Upload Completed");
		//                    string sql = string.Empty;
		//                    foreach (var bl in res.bill_Insert)
		//                    {
		//                        if (bl.IsUpdated)
		//                            sql += "EXEC Usp_UpdateBillstatus @bill_No = '" + bl.bill_No + "';";
		//                    }
		//                    foreach (var bl in res.bill_Update)
		//                    {
		//                        if (bl.IsUpdated)
		//                            sql += "EXEC Usp_UpdateBillstatus @bill_No = '" + bl.bill_No + "';";
		//                    }
		//                    foreach (var bl in res.cust)
		//                    {
		//                        if (bl.IsUpdated)
		//                            sql += "EXEC Usp_UpdateGuestData @guestID = '" + bl.id + "';";
		//                    }
		//                    foreach (var bl in res.Exp)
		//                    {
		//                        if (bl.IsUpdated)
		//                            sql += "EXEC Usp_UpdateExpenseMaster @id = '" + bl.id + "';";
		//                    }
		//                    if (sql.Length > 0)
		//                    {
		//                        SqlSupport objcls = new SqlSupport();
		//                        bool count = objcls.ExecuteNonQueryBySqlQuery(sql);
		//                    }
		//                }
		//                return response;
		//            }
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        error = "PostDataAsync error " + ex.Message;
		//        if (ex.InnerException != null)
		//            error += " InnerException: " + ex.InnerException.Message;
		//    }
		//    return "{\"error\":\"" + error + "\"}";
		//}
	}
}
