using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KOTPrintUtility.App_Code
{
	public class ApiRetrun
	{
		public bool Status { get; set; }
		public string JsonString { get; set; }
		public bool authentication { get; set; }

	}
	public class RequestData
	{
		public string Uri { get; set; }
		public string ApiKeyName { get; set; }
		public string ApiKey { get; set; }
		public string ActionType { get; set; }
		public string SourceName { get; set; }
		public bool Status { get; set; }
	}
	public class WebhookResponse
	{
		public int status_code { get; set; }
		public ResponseData response { get; set; }
	}

	public class ResponseData
	{
		public string message { get; set; }
	}

	class clsUpdateOrder
	{
		#region Get Request data
		private static RequestData GetRequestdata(string ActionType)
		{
			RequestData returndata = new RequestData();
			try
			{
				string localSqlKey = ConfigurationSettings.AppSettings["sqlKey"].ToString();
				using (SqlConnection con = new SqlConnection(localSqlKey))
				{
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = con;
					cmd.Parameters.AddWithValue("@SourceId", Program.Source_of_order);
					cmd.Parameters.AddWithValue("@actionType", ActionType);
					cmd.CommandText = "dbo.USP_UrlConfig";
					cmd.CommandType = CommandType.StoredProcedure;
					SqlDataAdapter da = new SqlDataAdapter(cmd);
					DataTable dt = new DataTable();
					da.Fill(dt);
					if (dt != null && dt.Rows.Count > 0)
					{
						DataRow dr = dt.Rows[0];
						returndata.Status = true;
						returndata.Uri = Convert.ToString(dr["url"]);
						returndata.ApiKeyName = Convert.ToString(dr["ApiKeyName"]);
						returndata.ApiKey = Convert.ToString(dr["ApiKey"]);
						returndata.ActionType = Convert.ToString(dr["actionType"]);
						returndata.SourceName = Convert.ToString(dr["SourceName"]);
					}
					else
					{
						returndata.Status = false;
					}

				}
			}
			catch (Exception ex)
			{
				returndata.Status = false;
			}
			Program.Source_of_order = string.Empty;
			return returndata;
		}
		#endregion

		#region Update Bill Status With Website WebAPI
		public static bool UpdateWebsiteStatusAPI(string zomato_order_id, string bill_no, string online_bill_no, EnumOrderStatus Status, bool IsDirect = false)
		{
			bool result = false;
			try
			{				
				for (int i = 0; i < 3; i++)
				{
					RequestData requestData = GetRequestdata("OrderStatusWeb");
					if (requestData.SourceName.ToLower().Equals("urban piper"))
					{
						Dictionary<string, string> fromData = new Dictionary<string, string>();
						string new_status = string.Empty;
						string message = string.Empty;

						switch (Status)
						{
							case EnumOrderStatus.Confirm:
								new_status = "confirm";
								message = "Order Accept";							
								break;
							case EnumOrderStatus.FoodReady:
								new_status = "Food Ready";
								message = "Food Ready";
								break;
							case EnumOrderStatus.OrdderAssin:
								new_status = "Assign";
								message = "Order Assign";
								break;
							case EnumOrderStatus.Cancel:
								new_status = "Cancelled";
								message = "Order Cancelled";
								break;
							case EnumOrderStatus.CustomerDataSent:
								break;
							default:
								break;
						}
						fromData.Add("new_status", new_status);
						fromData.Add("message", message);
						fromData.Add("WebOrderNo", zomato_order_id);
						bool respounceStatus = true;
						Dictionary<string, string> returnValue = null;
						if (requestData.Status)
						{
							respounceStatus = false;
							returnValue = GetZomatoReferenceClass.GetObject_ZomatoAPi.CommonToAllAPiCancelBill(fromData, requestData.ApiKeyName, requestData.ApiKey, requestData.Uri.Replace("$", zomato_order_id), "POST");
							respounceStatus = returnValue["status"].Equals("True");
							try
							{
								Loging.Log(LogType.Information, "zomato_order_id " + zomato_order_id + " responce " + returnValue["error"].ToString());
							}
							catch { }

							if (respounceStatus)
							{
								result = true;
								OrderStatusUpdateLog.AddStatusList(new clsOrderListStatus { Bill_no = bill_no, API_Status = true, CloudDB_Status = false, OnlineBillNo_no = online_bill_no, Ststus = Status.ToString() }, EnumStatusType.APIStatus);
								UpdateOrderStatusOffline("2", bill_no, online_bill_no, Status.ToString(), "1", "0", zomato_order_id);
								break;
							}
							else
							{
								UpdateOrderStatusOffline("4", bill_no, online_bill_no, Status.ToString(), "1", "0", zomato_order_id);
								Thread.Sleep(50);
							}
						}
					}
				}				
			}
			catch (Exception x)
			{

			}
			return result;
		}
		#endregion

		#region Update Bill tatus With Website WebAPI
		public static void UpdateBillStatusWithWebsiteWebAPI(string zomato_order_id, string bill_no, string online_bill_no, EnumOrderStatus Status)
		{
			try
			{
				Thread syncThread = new Thread(() =>
				{
					UpdateWebsiteStatusAPI(zomato_order_id, bill_no, online_bill_no, Status, true);
				});
				syncThread.Start();
			}
			catch (Exception ex)
			{

			}
		}
		#endregion


		#region Update Order Status Offline
		public static int UpdateOrderStatusOffline(string mode, string bill_no, string online_bill_no, string Current_Status, string Status_Website_api, string Status_Cloud_DB = "0", string zomato_order_id = "")
		{
			int Count = 0;
			try
			{
				string sql = "dbo.Usp_PendingOrderList_CURD";
				SqlCommand cmd = new SqlCommand(sql);
				cmd.Connection = cls_SqlConnection.SqlConnectionOffline();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@bill_date", Program.DayEnd_BIllingDate);
				cmd.Parameters.AddWithValue("@mode", mode);
				cmd.Parameters.AddWithValue("@bill_no", bill_no);
				cmd.Parameters.AddWithValue("@fin_year", clsConfigSettings.fin_year);
				cmd.Parameters.AddWithValue("@online_bill_no", online_bill_no);
				cmd.Parameters.AddWithValue("@Current_Status", Current_Status);
				cmd.Parameters.AddWithValue("@Status_Website_api", Status_Website_api);
				cmd.Parameters.AddWithValue("@Status_Cloud_DB", Status_Cloud_DB);
				cmd.Parameters.AddWithValue("@zomato_order_id", zomato_order_id);
				Count = cmd.ExecuteNonQuery();
				if (Count > 0)
				{
					Loging.Log(LogType.Error, "UpdateOrderStatusOnline.success  bill no=> " + bill_no);
				}
				else
				{
					Loging.Log(LogType.Error, "UpdateOrderStatusOnline.else.fail  bill no=> " + bill_no);
				}
			}
			catch (SqlException ex)
			{
				Loging.Log(LogType.Error, "UpdateOrderStatusOnline.fail error ocurred  bill no=> " + bill_no + " error " + ex.Message);
			}
			return Count;
		}
		#endregion

		#region Updat Order Status Online and CloudDB
		public static bool UpdatOrderStatusOnlineCloudDB(string bill_no_WebOrder, string LastBillNo, string status, string Channel = "website", bool UpdateLOcalDB = false)
		{
			bool Result = false;
			try
			{
				for (int i = 0; i < 3; i++)
				{
					OrderDetailOnline objOnline = new OrderDetailOnline();
					Result = objOnline.ExecuteDMLOnline("Usp_UpdateStatus_WebOrderWebsite @Order_No='" + bill_no_WebOrder + "',@bill_no_local='" + LastBillNo + "',@IsManualPuch='0',@mode='" + status + "'");

					if (Result && Channel.ToLower() == "website")
					{
						OrderStatusUpdateLog.AddStatusList(new clsOrderListStatus { Bill_no = LastBillNo, API_Status = false, CloudDB_Status = true, OnlineBillNo_no = bill_no_WebOrder, Ststus = status }, EnumStatusType.CloudDBStatus);

						if (UpdateLOcalDB)
						{
							UpdateOrderStatusOffline("3", LastBillNo, bill_no_WebOrder, status, "0", "1", "");
						}						
					}
					if(Result)
					  break;
				}
			}
			catch (Exception ex)
			{
				Loging.Log(LogType.Error, "UpdatOrderStatusOnline.fail error ocurred  bill_no_WebOrder no=> " + bill_no_WebOrder + " error " + ex.Message);
			}
			return Result;
		}
		#endregion

		public static bool UpdateWebsiteStatusGuestData(RootCustomerData cust, string bill_no, string online_bill_no, EnumOrderStatus Status, bool IsDirect = false)
		{
			bool result = false;
			try
			{
				for (int i = 0; i < 3; i++)
				{
					RequestData requestData = GetRequestdata("UploadGuestfamepilot");
					if (requestData.SourceName.ToLower().Equals("urban piper"))
					{				

						bool respounceStatus = true;
						Dictionary<string, string> returnValue = null;
						if (requestData.Status)
						{
							respounceStatus = false;
							returnValue = GetZomatoReferenceClass.GetObject_ZomatoAPi.CommonToAllAPiUploadSale(cust, requestData.ApiKeyName,requestData.ApiKey, requestData.Uri, "POST");
							respounceStatus = returnValue["status"].Equals("True");
							try
							{
								Loging.Log(LogType.Information, "UpdateWebsiteStatusGuestData Guest uploaded " + returnValue["error"].ToString());
							}
							catch { }

							if (respounceStatus)
							{
								result = true;
								UpdateOrderStatusOffline("5", bill_no, online_bill_no, Status.ToString(), "1", "0", "");
								break;
							}
						}
					}			
				}
			}
			catch (Exception x)
			{

			}
			return result;
		}		
	}
}
