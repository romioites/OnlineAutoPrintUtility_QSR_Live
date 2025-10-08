using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Printing;
using LPrinterTest;
using System.Configuration;
using TuchScreenApp1Jan2013.App_Dataset;
using KOTPrintUtility.Report;
using TuchScreenApp1Jan2013.UI;

namespace TuchScreenApp1Jan2013.App_Code
{
	class cls_ActionClass
	{
		string LastBill_No = string.Empty;



		public static DataTable GetAddonToPrint(Int64 item_code, DataTable dt, string addon_index)
		{
			DataTable dtobj = null;
			try
			{
				var currentStatRow = (from currentStat in dt.AsEnumerable()
									  where currentStat.Field<String>("addon_index") == addon_index
									  select currentStat);
				if (currentStatRow.Count() > 0)
				{
					dtobj = new DataTable();
					dtobj = currentStatRow.CopyToDataTable();
				}
			}
			catch { }
			return dtobj;
		}
		private DataTable GetAssortedItemToPrint(Int64 ItemCode, DataTable dt, string DishGroup)
		{
			DataTable dtobj = null;
			try
			{
				var currentStatRow = (from currentStat in dt.AsEnumerable()
									  where currentStat.Field<Int64>("i_code_fk") == ItemCode && currentStat.Field<String>("item_index") == DishGroup
									  select currentStat);
				if (currentStatRow.Count() > 0)
				{
					dtobj = new DataTable();
					dtobj = currentStatRow.CopyToDataTable();
				}
			}
			catch { }
			return dtobj;
		}
		private string SplitConmment(string Discomments)
		{
			string FinalComment = string.Empty;
			string[] CommentDetail = Discomments.Split('+');
			int CommnetIndex = Discomments.Split('+').Length;
			for (int i = 0; i < CommentDetail.Length; i++)
			{
				if (i == 0)
					FinalComment += ">>" + CommentDetail[i].ToString();
				else
					FinalComment += "\n>>" + CommentDetail[i].ToString();
			}
			return FinalComment;
		}


		public string GetMainItems(string i_name)
		{
			string str1 = string.Empty;
			string qty = string.Empty;
			if (i_name.Length <= 36)
			{
				for (int cn = i_name.Length; cn <= 35; ++cn)
					i_name += " ";
			}

			return str1 = "" + i_name + "  " + qty + "\n";
		}
	}
}