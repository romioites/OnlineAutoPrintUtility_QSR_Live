using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace KOTPrintUtility.App_Code
{
    class GetZomatoReferenceClass
    {
		private static ConsumeWebApi _consumeWebApi;
		public static ConsumeWebApi GetObject_ConsumeWebApi
		{
			get
			{
				if (_consumeWebApi == null)
					_consumeWebApi = new ConsumeWebApi();
				return _consumeWebApi;
			}
		}

		private static cls_ZomatoAPi _cls_ZomatoAPi;
		public static cls_ZomatoAPi GetObject_ZomatoAPi
		{
			get
			{
				if (_cls_ZomatoAPi == null)
					_cls_ZomatoAPi = new cls_ZomatoAPi();
				return _cls_ZomatoAPi;
			}
		}
		private static JavaScriptSerializer _javaScriptSerializer;
		public static JavaScriptSerializer GetObject_JavaScriptSerializer
		{
			get
			{
				if (_javaScriptSerializer == null)
					_javaScriptSerializer = new JavaScriptSerializer();
				return _javaScriptSerializer;
			}
		}
		private static ApiRetrun _ApiRetrun;
		public static ApiRetrun GetObject_ApiRetrun
		{
			get
			{
				if (_ApiRetrun == null)
					_ApiRetrun = new ApiRetrun();
				return _ApiRetrun;
			}
		}
		//public Dictionary<string, string> CommonToAllAPi(Dictionary<string, string> postData, string ApiKeyName, string ApiKey, string Uri, string Method)
		//{
		//    Dictionary<string, string> returnValue = new Dictionary<string, string>();
		//    returnValue = ExecuteAPI(Uri, postData, ApiKeyName, ApiKey, Method);
		//    return returnValue;
		//}
		//private static JavaScriptSerializer _javaScriptSerializer;
		//public static JavaScriptSerializer GetObject_JavaScriptSerializer
		//{
		//    get
		//    {
		//        if (_javaScriptSerializer == null)
		//            _javaScriptSerializer = new JavaScriptSerializer();
		//        return _javaScriptSerializer;
		//    }
		//}
		//public Dictionary<string, string> ExecuteAPI(string Uri, Dictionary<string, string> postData, string ApiKeyName, string ApiKey, string Method)
		//{
		//    Dictionary<string, string> returnValue = new Dictionary<string, string>();
		//    JavaScriptSerializer js = GetZomatoReferenceClass.GetObject_JavaScriptSerializer;
		//    string jsonData = js.Serialize(postData);
		//    Dictionary<string, string> Headers = new Dictionary<string, string>();
		//    if (!string.IsNullOrEmpty(ApiKeyName) && !string.IsNullOrEmpty(ApiKey))
		//    {
		//        Headers.Add(ApiKeyName, ApiKey);
		//    }
		//    ApiRetrun apiRetrun = _consumeWebApi.PostRequest(Uri, Headers, jsonData, Method);

		//    if (apiRetrun.Status)
		//    {
		//        returnValue = js.Deserialize<Dictionary<string, string>>(apiRetrun.JsonString);
		//    }
		//    else
		//    {
		//        Headers.Add("Accept", "application/json");
		//        Headers.Add("ContentType", "application/json");
		//        string requestheaderstring = js.Serialize(Headers);
		//        //InsertErrorLog(Uri, requestheaderstring, jsonData, Method, apiRetrun.JsonString);
		//        returnValue["status"] = "false";
		//        returnValue["error"] = apiRetrun.JsonString;
		//    }
		//    return returnValue;
		//}
	}
}
