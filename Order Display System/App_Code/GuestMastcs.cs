using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOTPrintUtility.App_Code
{
	public class RootCustomerData
	{
		public List<CustomerData> Data { get; set; }
	}
	public class CustomerData
	{
		public string Name { get; set; }
		public string Mobile_No { get; set; }
		public string City { get; set; }
		public string Location { get; set; }
		public string Address { get; set; }
		public string OutletName { get; set; }
		public string Order_type { get; set; }
		public string Order_From { get; set; }
		public string DateTime { get; set; }
		public string Order_ID { get; set; }
	}
	class GuestMastcs
	{

	}
}
