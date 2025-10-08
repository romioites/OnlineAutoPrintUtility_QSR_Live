using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOTPrintUtility.App_Code
{
	public class clsOrderListStatus
	{
		public string Bill_no { get; set; }
		public string OnlineBillNo_no { get; set; }
		public bool API_Status { get; set; }
		public bool CloudDB_Status { get; set; }
		public string Ststus { get; set; }
	}
	public class OrderStatusUpdateLog
	{
		private static List<clsOrderListStatus> lstOrderListStatusWesite = null;
		public OrderStatusUpdateLog()
		{
			lstOrderListStatusWesite = new List<clsOrderListStatus>();
		}
		public static void AddStatusList(clsOrderListStatus lst, EnumStatusType bStatusType)
		{
			try
			{
				var Newlst = lstOrderListStatusWesite.FindAll(x => x.Bill_no == lst.Bill_no && x.Ststus==lst.Ststus).ToList();
				if (Newlst != null)
				{
					if (Newlst.Count() > 0)
					{
						int Index = lstOrderListStatusWesite.FindIndex(r => r.Bill_no == lst.Bill_no && r.Ststus == lst.Ststus);
						switch (bStatusType)
						{
							case EnumStatusType.APIStatus:
								{
									lstOrderListStatusWesite[Index].API_Status = true;
								}
								break;
							case EnumStatusType.CloudDBStatus:
								{
									lstOrderListStatusWesite[Index].CloudDB_Status = true;
								}
								break;
						}
					}
					else
					{
						lstOrderListStatusWesite.Add(new clsOrderListStatus { Bill_no = lst.Bill_no, API_Status = lst.API_Status, CloudDB_Status = lst.CloudDB_Status, OnlineBillNo_no = lst.OnlineBillNo_no,Ststus=lst.Ststus });
					}
				}
				else
				{
					lstOrderListStatusWesite = new List<clsOrderListStatus>();
				}
			}
			catch (Exception ex)
			{
				Loging.Log(LogType.Error, "OrderStatusUpdateLog.AddStatusList error " + ex.Message);
			}
		}

		public static bool FindOrderStatus(EnumOrderStatus bStatus, EnumStatusType bStatusType,string bill_no)
		{
			try
			{
				var Newlst = lstOrderListStatusWesite.FindAll(x => x.Bill_no == bill_no && x.Ststus == bStatus.ToString()).ToList();
				if (Newlst != null)
				{
					if (Newlst.Count() > 0)
					{
						return true;
					}					
				}				
			}
			catch (Exception ex)
			{
				Loging.Log(LogType.Error, "OrderStatusUpdateLog.FindOrderStatus error " + ex.Message);
			}
			return false;
		}	
	}
}
