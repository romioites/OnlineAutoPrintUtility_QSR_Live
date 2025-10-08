using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOTPrintUtility.App_Code
{
	public enum LogType
	{
		Information = 0,
		Error,
		EndTransaction
	}
	public class Loging
	{
		private static string _Path = string.Empty;
		private static bool DEBUG = true;
		private static string GenerateLog = ConfigurationSettings.AppSettings["GenerateLog"].ToString();

		public static void Log(LogType logtype, string msg)
		{
			if (GenerateLog == "1")
			{
				Task.Run(() => BeginLog(logtype, msg));
			}
		}

		public static void BeginLog(LogType logtype, string msg)
		{
			try
			{
				bool Initialization = false;
				string str = Program.DayEnd_BIllingDate;
				_Path = clsConfigSettings.Log_Drive + @":\RomioLog";
				if (!Directory.Exists(_Path))
					Directory.CreateDirectory(_Path);

				string fileName = _Path + @"\AutoAcceptOrderUtilityLog " + str + ".txt";
				if (!File.Exists(fileName))
				{
					FileStream fs = new FileStream(fileName, FileMode.CreateNew);
					fs.Close();
					Initialization = true;
				}
				using (StreamWriter w = File.AppendText(fileName))
				{
					Write(logtype, msg, w, Initialization);
				}
			}
			catch (Exception e)
			{
				//Handle
			}
		}
		static private void Write(LogType logtype, string msg, TextWriter w, bool IsInitilize)
		{
			try
			{
				if (IsInitilize)
				{
					w.Write("[{0} {1}]", Program.Outlet_Name, "	Log initialization " + Program.DayEnd_BIllingDate);
					w.WriteLine(" {0}", Environment.NewLine + "============================================================================================================");
					w.Write(Environment.NewLine);
				}
				if (logtype == LogType.EndTransaction)
				{
					w.Write("[{0} {1}]", Program.DayEnd_BIllingDate + " " + System.DateTime.Now.ToString("hh:mm:ss.fff tt"), logtype.ToString());
					w.WriteLine(" {0}", msg);
					w.WriteLine(" {0}", Environment.NewLine + "============================================================================================================");
				}
				else
				{
					w.Write("[{0} {1}]", Program.DayEnd_BIllingDate + " " + System.DateTime.Now.ToString("hh:mm:ss.fff tt"), logtype.ToString());
					w.WriteLine(" {0}", msg);
				}
			}
			catch (Exception e)
			{
				//Handle
			}
		}
	}
}
