using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace KOTPrintUtility.App_Code
{
    class ConsumeWebApi
    {
		private static string _AcceptHeader = "application/json";
		private static string _ContentTypeHeader = "application/json";


		public ApiRetrun PostRequest(string Uri, Dictionary<string, string> Headers, string jsonData, string Method)
		{
			ApiRetrun apiRetrun = GetZomatoReferenceClass.GetObject_ApiRetrun;
			apiRetrun.JsonString = string.Empty;
			apiRetrun.Status = false;
			try
			{
				//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
				string RetrunString = string.Empty;
				HttpWebRequest httpWebRequest = WebRequest.Create(new Uri(Uri)) as HttpWebRequest;
				httpWebRequest.Accept = _AcceptHeader;
				httpWebRequest.ContentType = _ContentTypeHeader;
				httpWebRequest.Method = Method;
				//ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

				foreach (KeyValuePair<string, string> kvp in Headers)
				{
					httpWebRequest.Headers.Add(kvp.Key, kvp.Value);
				}
				using (Stream stream = httpWebRequest.GetRequestStream())
				{
					using (StreamWriter streamWriter = new StreamWriter(stream))
						streamWriter.Write(jsonData);
				}
				using (Stream stream = httpWebRequest.GetResponse().GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(stream))
					{
						var ResponceResult = streamReader.ReadToEnd();
						apiRetrun.Status = true;
						apiRetrun.authentication = true;
						apiRetrun.JsonString = ResponceResult;
						Loging.Log(LogType.Information, "cls_ZomatoAPi.PostRequest.response: " + jsonData);
					}
				}
				return apiRetrun;
			}
			catch (Exception ex)
			{
				Loging.Log(LogType.Error, "cls_ZomatoAPi.PostRequest: " + ex.Message);
				apiRetrun.Status = false;
				apiRetrun.JsonString = ex.Message;
				return apiRetrun;
			}
		}
	}
}
