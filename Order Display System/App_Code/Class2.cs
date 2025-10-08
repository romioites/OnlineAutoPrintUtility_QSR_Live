using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOTPrintUtility.App_Code
{
	public class tbl_bill
	{
		public string bill_no_WebOrder { set; get; }
		public string Bill_TypeName { set; get; }
		public string Delivery_Charge { set; get; }
		public string Channel { get; set; }
		public string Channel_Id { get; set; }
		public string Delivery_By { get; set; }
		public string Aggr_dis_pct { get; set; }
		public string Aggr_dis_amount { get; set; }
		public string Agg_Tax_Calculation { get; set; }
		public string Aggr_Discount { get; set; }
		public string dis_Type { set; get; }
		public string ModifiedBill_No { set; get; }
		public string ModifiedAmount { set; get; }
		public string DiscountModifiedBy { set; get; }
		public string Modified_DiscountAmnt { set; get; }
		public string Modified_DiscountPct { set; get; }
		public string Remarks { set; get; }
		public List	<tbl_tran> tran { set; get; }
		public string ZomatoOrderNo { set; get; }
		public string custCode { set; get; }
		public string CardType { set; get; }

		public string is_instant_order { set; get; }

		public string Card_Type_Settlement { set; get; }
	}

	public class tbl_tran
	{
		public string qty { set; get; }
		public string Assorted_Item { set; get; }		
		public string Rate { set; get; }
		public string I_Name { set; get; }
		public string TaxRate { set; get; }
		public string IsTaxable { set; get; }
		public string tax { set; get; }
		public string Discount { set; get; }
		public string dept { set; get; }
		public string Group_Dish { set; get; }
		public string service_tax { set; get; }
		public string dis_rate { set; get; }
		public string dis_amount { set; get; }
		public double	amount { set; get; }
	}
}
