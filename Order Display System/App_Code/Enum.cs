using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOTPrintUtility.App_Code
{
	public enum EnumOrderStatus
	{
		Confirm,
		BillPunch,
		FoodReady,
		OrdderAssin,
		Cancel,
		CustomerDataSent
	}
	public enum EnumStatusType
	{
		APIStatus,
		CloudDBStatus,
		RetryCount
	}
}
