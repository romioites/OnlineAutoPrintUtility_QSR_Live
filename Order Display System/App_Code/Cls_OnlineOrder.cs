using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOTPrintUtility.App_Code
{
	public class BillRespose
	{
		public string Bill_no { get; set; }
		public bool Result { get; set; }
	}
	public class Cls_OnlineOrder
	{
		public bool status { set; get; }
		public string message { set; get; }
		public List<clsOrdersBill> bill { get; set; }
	}
	public class clsOrdersBill
	{
		public int bill_no { get; set; }
		public string bill_Type { get; set; }
		public string Type { get; set; }
		public decimal Bill_DiscountPct { get; set; }
		public decimal dis_amount { get; set; }
		public string Discount_by { get; set; }
		public string Comments { get; set; }
		public string Order_Source_Id { get; set; }
		public decimal bill_amount { get; set; }
		public decimal Delivery_Charge { get; set; }
		public string cust_code { get; set; }
		public string dis_type { get; set; }
		public string Coupon_code { get; set; }
		public decimal service_charge { get; set; }
		public string bill_punch_by { get; set; }
		public string DeliveryBy { get; set; }
		public string External_Source_id { get; set; }
		public decimal dis_pct_Agg { get; set; }
		public string Agg_Tax_Calculation { get; set; }
		public decimal Aggr_Discount { get; set; }
		public string payment_mode { get; set; }
		public string Online_Payment_Mode { get; set; }
		public string ChanelID { get; set; }
		public int Noofitem { get; set; }
		public string zomato_order_id { get; set; }
		public int is_instant_order { get; set; }
		public string Card_Type_Settlement { get; set; }
		public clsOrdersCustomer customers { get; set; }
		public List<clsOrdersItem> items { get; set; }
		public List<clsComboItem> combos { get; set; }
		public List<clsOrdersCharges> charges { get; set; }
	}


	public class clsOrdersCustomer
	{
		public int Bill_no_fk { get; set; }
		public string Zomato_order_id { get; set; }
		public string order_status { get; set; }
		public string Mobile_no { get; set; }
		public string Cust_Name { get; set; }
		public string bill_typetext { get; set; }
		public string bill_type { get; set; }
		public string Address { get; set; }
		public string Location { get; set; }
		public string Location_Id { get; set; }
		public string City { get; set; }
		public string email_id { get; set; }
		public string Colony_Id { get; set; }
		public string pin_no { get; set; }
		public string Land_mark { get; set; }
		public string Payment_Mode { get; set; }
		public string OrderSouce { get; set; }
		public string TockenNo { get; set; }
		public string Order_Source_Id { get; set; }
		public string OnineBillNo { get; set; }
		public string currentStatus { get; set; }
		public string LocalBillNo { get; set; }
		public string Flat_No { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public string Comments { get; set; }
		public string External_Source_id { get; set; }
		public string tableno { get; set; }
		public string tablename { get; set; }
		public string UserName { get; set; }
	}
	public class clsOrdersItem
	{
		public int Bill_No_FK { get; set; }
		public int id { get; set; }
		public string I_code { get; set; }
		public double Rate { get; set; }
		public string i_name { get; set; }
		public double qty { get; set; }
		public double amout { get; set; }
		public double Tax { get; set; }
		public string comments { get; set; }
		public decimal TaxRate { get; set; }
		public string IsTaxable { get; set; }
		public string Discount { get; set; }
		public string dept { get; set; }
		public string Group_Dish { get; set; }
		public string service_tax { get; set; }
		public decimal dis_rate { get; set; }
		public decimal dis_amount { get; set; }
		public string Counter_id { get; set; }
		public string Category { get; set; }
		public string service_tax_rate { get; set; }
		public string Surcharge_rate { get; set; }
		public string Service_charge_rate { get; set; }
		public double dis_pct_Agg { get; set; }
		public double Aggr_Discount { get; set; }
	}
	public class clsComboItem
	{
		public string i_code { get; set; }
		public string i_code_fk { get; set; }
		public decimal Qty { get; set; }
		public int bill_no_fk { get; set; }
		public string bill_date { get; set; }
		public string cancel_qty { get; set; }
		public string Order_Type { get; set; }
		public string i_name { get; set; }
		public decimal amount { get; set; }
		public string IsDiscount { get; set; }
		public string index { get; set; }
		public string ItemIndex { get; set; }
		public string item_index { get; set; }
		public decimal TaxRate { get; set; }
		public string Step_Name { get; set; }
		public int No_of_Item { get; set; }
		public string i_type { get; set; }
		public string Deal_TYpe { get; set; }
		public string Index_No { get; set; }
		public string DishComment { get; set; }
		public string service_tax { get; set; }
	}
	public class clsOrdersCharges
	{
		public int id { get; set; }
		public int chargesId { get; set; }
		public string I_Code_fk { get; set; }
		public int Bill_no_fk { get; set; }
		public string Zomato_order_id { get; set; }
		public string Charge_name { get; set; }
		public decimal chargesValue { get; set; }
		public decimal chargesTotaltax { get; set; }
	}
}
