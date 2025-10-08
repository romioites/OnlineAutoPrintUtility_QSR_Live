using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KOTPrintUtility.App_Code
{
	class clsUpdateOrderStatusWebsite
	{
		private Timer tmrStarttmr;		

		public async Task StatrtUploadMaster()
		{
			// Initialize the timer
			tmrStarttmr = new Timer(30000); // 15 sec
			tmrStarttmr.Elapsed += async (s, e) =>
			{
				try
				{
					tmrStarttmr.Stop();
					await UpdateStatus();
				}
				catch (Exception ex)
				{
					Loging.Log(LogType.Error, "Timer Tick error: " + ex.Message);
				}
				finally
				{
					tmrStarttmr.Start();
				}
			};
			tmrStarttmr.AutoReset = true;
			tmrStarttmr.Start();
		}


		private static async Task UpdateStatus()
		{
			try
			{
				SqlCommand cmd = new SqlCommand();
				DataSet ds = new DataSet();
				try
				{									
					cmd.CommandText = "Usp_PendingOrderList_CURD";
					SqlConnection con = new SqlConnection(Program.sqlkey);
					con.Open();
					cmd.Connection = con;
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
											if (Status_Cloud_DB == "0")
											{
												clsUpdateOrder.UpdatOrderStatusOnlineCloudDB(zomato_order_id.Replace("-", "").Trim().TrimEnd('-'), bill_no, "FoodReady", "website", true);
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
											if (Status_Cloud_DB == "0")
											{
												clsUpdateOrder.UpdatOrderStatusOnlineCloudDB(zomato_order_id.Replace("-", "").Trim().TrimEnd('-'), bill_no, "OrdderAssin", "website", true);
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
											if (Status_Cloud_DB == "0")
											{
												clsUpdateOrder.UpdatOrderStatusOnlineCloudDB(zomato_order_id.Replace("-", "").Trim().TrimEnd('-'), bill_no, "Cancel","website", true);
											}
										}
										break;
									case EnumOrderStatus.CustomerDataSent:
										{
											string sql = "";
											RootCustomerData cust = new RootCustomerData();
											cust.Data = new List<CustomerData>();
											SqlSupport objS = new SqlSupport();
											var dt = objS.GetDataTableBySqlQuery("EXEC UspGetCustomerCloud @Custcode='" + id + "'");
											if (dt != null)
											{
												if (dt.Rows.Count > 0)
												{
													foreach (DataRow item in dt.Rows)
													{
														CustomerData c = new CustomerData();
														c.Name = Convert.ToString(item["Name"]);
														c.Address = Convert.ToString(item["Address"]);
														c.City = Convert.ToString(item["City"]);
														c.DateTime = Convert.ToString(item["DateTime"]);
														c.Location = Convert.ToString(item["Location"]);
														c.Mobile_No = Convert.ToString(item["Mobile_No"]);
														c.Order_From = Convert.ToString(item["Order_From"]);
														c.OutletName = Convert.ToString(item["OutletName"]);
														c.Order_type = Convert.ToString(item["Order_type"]);
														c.Order_ID= Convert.ToString(item["bill_no"]); 
														cust.Data.Add(c);
														//sql += "exec Usp_PendingOrderList_CURD @bill_date='"+Program.DayEnd_BIllingDate+ "',@mode='5',@bill_no='"+bill_no+"';";
													}
													clsUpdateOrder.UpdateWebsiteStatusGuestData(cust, bill_no, online_bill_no, EnumOrderStatus.Cancel, true);
												}
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
					Loging.Log(LogType.Error, "GetOrderStatusLog error 1" + ex.Message);
				}
				finally
				{
					cmd.Connection.Close();
					cmd.Connection.Dispose();
					cmd.Dispose();
				}
			}
			catch (Exception ex)
			{
				Loging.Log(LogType.Error, "GetOrderStatusLog error 2" + ex.Message);
			}
		}
	}
}
