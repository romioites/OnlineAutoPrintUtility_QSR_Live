using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;


namespace KOTPrintUtility.App_Code
{
	public class ResponsePacket
	{
		public string Message { get; set; }
	}

	public class Root
	{
		public int StatusCode { get; set; }
		public object Message { get; set; }
		public ResponsePacket ResponsePacket { get; set; }
		public bool Success { get; set; }
		public object Errors { get; set; }
	}


	class cls_ZomatoAPi
	{
		ConsumeWebApi _consumeWebApi = GetZomatoReferenceClass.GetObject_ConsumeWebApi;
		public static DataTable GetDataTable(string SqlQuery)
		{
			DataTable retrun = new DataTable();
			try
			{
				using (SqlConnection con = new SqlConnection(Program.sqlKeyOnline))
				{
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = con;
					cmd.CommandText = SqlQuery;
					cmd.CommandType = CommandType.Text;
					SqlDataAdapter da = new SqlDataAdapter(cmd);
					da.Fill(retrun);
				}
				return retrun;
			}
			catch (Exception ex)
			{
				return retrun;
			}
		}
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






		public Dictionary<string, string> CommonToAllAPiCancelBill(Dictionary<string, string> postData, string ApiKeyName, string ApiKey, string Uri, string Method)
		{
			Dictionary<string, string> returnValue = new Dictionary<string, string>();
			returnValue = ExecuteAPICancelBill(Uri, postData, ApiKeyName, ApiKey, Method);
			return returnValue;
		}

		private Dictionary<string, string> ExecuteAPICancelBill(string Uri, Dictionary<string, string> postData, string ApiKeyName, string ApiKey, string Method)
		{
			//sponsePacket oblcls=new ResponsePacket (ResponsePacketResponsePacket)
			Dictionary<string, string> returnValue = new Dictionary<string, string>();
			JavaScriptSerializer js = GetZomatoReferenceClass.GetObject_JavaScriptSerializer;
			string jsonData = js.Serialize(postData);
			Dictionary<string, string> Headers = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(ApiKeyName) && !string.IsNullOrEmpty(ApiKey))
			{
				Headers.Add(ApiKeyName, ApiKey);
			}
			ApiRetrun apiRetrun = _consumeWebApi.PostRequest(Uri, Headers, jsonData, Method);

			if (apiRetrun.Status)
			{
				Root Objcls = js.Deserialize<Root>(apiRetrun.JsonString);
				returnValue["status"] = Objcls.Success.ToString();
				returnValue["error"] = Objcls.ResponsePacket.Message;
				//returnValue = js.Deserialize<Dictionary<string, string>>(apiRetrun.JsonString);
			}
			else
			{

				Headers.Add("Accept", "application/json");
				Headers.Add("ContentType", "application/json");
				string requestheaderstring = js.Serialize(Headers);
				//InsertErrorLog(Uri, requestheaderstring, jsonData, Method, apiRetrun.JsonString);
				returnValue["status"] = "false";
				returnValue["error"] = apiRetrun.JsonString;
			}
			return returnValue;
		}

		public Dictionary<string, string> CommonToAllAPiUploadSale(RootCustomerData cust, string ApiKeyName, string ApiKey, string Uri, string Method)
		{
			Dictionary<string, string> returnValue = new Dictionary<string, string>();
			returnValue = ExecuteAPIUploadSale(Uri, cust, ApiKeyName, ApiKey, Method);
			return returnValue;
		}

		private Dictionary<string, string> ExecuteAPIUploadSale(string Uri, RootCustomerData cust, string ApiKeyName, string ApiKey, string Method)
		{
			//sponsePacket oblcls=new ResponsePacket (ResponsePacketResponsePacket)
			Dictionary<string, string> returnValue = new Dictionary<string, string>();
			JavaScriptSerializer js = GetZomatoReferenceClass.GetObject_JavaScriptSerializer;
			string jsonData = js.Serialize(cust);
			Dictionary<string, string> Headers = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(ApiKeyName) && !string.IsNullOrEmpty(ApiKey))
			{
				Headers.Add(ApiKeyName, ApiKey);
			}
			ApiRetrun apiRetrun = _consumeWebApi.PostRequest(Uri, Headers, jsonData, Method);

			if (apiRetrun.Status)
			{
				WebhookResponse Objcls = js.Deserialize<WebhookResponse>(apiRetrun.JsonString);
				returnValue["status"] = Objcls.status_code == 200 ? "True" : "false";
				returnValue["error"] = Objcls.response.message;
			}
			else
			{

				Headers.Add("Accept", "application/json");
				Headers.Add("ContentType", "application/json");
				string requestheaderstring = js.Serialize(Headers);
				returnValue["status"] = "false";
				returnValue["error"] = apiRetrun.JsonString;
			}
			return returnValue;
		}

		public ResponseAPI ExecuteAPIGlobal(string Uri, clsBillList cust, string ApiKeyName, string ApiKey, string Method)
		{
			Loging.Log(LogType.Information, "LiveSale.ExecuteAPIGlobal.start");
			ResponseAPI Objcls = new ResponseAPI();
			Objcls.status = false;
			try
			{
				Dictionary<string, string> returnValue = new Dictionary<string, string>();
				JavaScriptSerializer js = GetZomatoReferenceClass.GetObject_JavaScriptSerializer;
				string jsonData = js.Serialize(cust);
				Dictionary<string, string> Headers = new Dictionary<string, string>();
				if (!string.IsNullOrEmpty(ApiKeyName) && !string.IsNullOrEmpty(ApiKey))
				{
					Headers.Add(ApiKeyName, ApiKey);
				}
				ApiRetrun apiRetrun = _consumeWebApi.PostRequest(Uri, Headers, jsonData, Method);

				if (apiRetrun.Status)
				{
					Objcls.status = true;
					Objcls = js.Deserialize<ResponseAPI>(apiRetrun.JsonString);
					Loging.Log(LogType.Information, "LiveSale.ExecuteAPIGlobal.end success " + apiRetrun.JsonString);
				}
				else
				{
					Objcls.message = apiRetrun.JsonString;
					Loging.Log(LogType.Information, "LiveSale.ExecuteAPIGlobal.fail "+ Objcls.message);
				}
			}
			catch (Exception ex)
			{
				Objcls.message = ex.Message;
				Loging.Log(LogType.Error, "LiveSale.ExecuteAPIGlobal.error "+ex.Message);
			}
			return Objcls;
		}
	}
}
