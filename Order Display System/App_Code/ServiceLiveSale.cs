using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOTPrintUtility.App_Code
{


	public class ResponseAPI
	{
		public bool status { get; set; }
		public string message { get; set; }
		public List<cls_Bill_list> bill_Insert { set; get; }
		public List<cls_Bill_list> bill_Update { set; get; }
		public List<cls_RespList> cust { set; get; }
		public List<cls_RespList> Exp { set; get; }
	}
	public class ResposeBill
	{
		public string bill_no { get; set; }
	}

	public class cls_Bill_list
	{
		public string bill_No { set; get; }
		public string error { set; get; }
		public bool IsUpdated { set; get; }
	}
	public class cls_RespList
	{
		public string id { set; get; }
		public string error { set; get; }
		public bool IsUpdated { set; get; }
	}
	public class clsBillList
	{
		public List<cls_tbl_bill> bill { set; get; }
		public List<clsCustomer> custs { set; get; }
		public List<clsExpMaster> expm { set; get; }		
		public string sqlcon { set; get; }
		public string outlet_id { set; get; }
	}

	public class cls_tbl_bill
	{
		public string POS_code { set; get; }
		public string Bill_Date { set; get; }
		public string Bill_Type { set; get; }
		public string Bill_No_local { set; get; }
		public string Cust_Code { set; get; }
		public string Table_No { set; get; }
		public string Bill_Amount { set; get; }
		public string Dis_Amount { set; get; }
		public string Service_Charge_Amount { set; get; }
		public string Service_ch_Rate { set; get; }
		public string TaxSale { set; get; }
		public string Tax_Rate1 { set; get; }
		public string Tax1 { set; get; }
		public string Other_Charge { set; get; }
		public string No_of_Item { set; get; }
		public string Grand_Total { set; get; }
		public string Payment_Mode { set; get; }
		public string Card_No { set; get; }
		public string Paid_Amount { set; get; }
		public string Balance { set; get; }
		public string Bill_Status { set; get; }
		public string Tips { set; get; }
		public string To_Whom { set; get; }
		public string Nos_Of_Printing { set; get; }
		public string CancelDate { set; get; }
		public string CamcelTime { set; get; }
		public string cancel_reason { set; get; }
		public string cancel_amt { set; get; }
		public string timeout { set; get; }
		public string timeIN { set; get; }
		public string Waiter_Code { set; get; }
		public string Tot_amt { set; get; }
		public string IP { set; get; }
		public string Reprint_by { set; get; }
		public string Cancelled_By { set; get; }
		public string Bill_Modify_by { set; get; }
		public string bill_Modification_Reason { set; get; }
		public string Comments { set; get; }
		public string Advance { set; get; }
		public string Orderno { set; get; }
		public string Order_Date { set; get; }
		public string Cashier { set; get; }
		public string createdOn { set; get; }
		public string IsBillSattle { set; get; }
		public string RoundOff { set; get; }
		public string card_amt { set; get; }
		public string TockenNo { set; get; }
		public string Discount_by { set; get; }
		public string Service_Charge { set; get; }
		public string Upflag { set; get; }
		public string table_coverd { set; get; }
		public string HoldSta { set; get; }
		public string Service_tax { set; get; }
		public string card_type { set; get; }
		public string sur_charge { set; get; }
		public string Reprint_Time { set; get; }
		public string Reprint_Reason { set; get; }
		public string outlet { set; get; }
		public string Subtot { set; get; }
		public string DeptHead { set; get; }
		public string Modified_amount { set; get; }
		public string modified_date { set; get; }
		//New Column Add
		public string isModify { set; get; }
		public string Order_Status { set; get; }
		public string Order_View_by { set; get; }
		public string Order_View_Time { set; get; }
		public string Order_Done_Time { set; get; }
		public string Order_Done_by { set; get; }
		public string Dispatch_by { set; get; }
		public string Dispatch_Time { set; get; }
		public string Allocated_To { set; get; }
		public string Allocated_Time { set; get; }
		public string Allocated_by { set; get; }
		public string Change_Amount { set; get; }
		public string dis_type { set; get; }
		public string Coupon_code { set; get; }
		public string Web_order_no { set; get; }
		public string web_order_comments { set; get; }
		public string SBC_Tax { set; get; }


		public string KKC_Tax { set; get; }
		public string source_of_order { set; get; }


		public string WebOrder_No { set; get; }
		public string zomato_order_id { set; get; }
		public string IsManualPuch { set; get; }
		public string Online_bill_PunchBy { set; get; }
		public string loyality_type { set; get; }
		public string loyality_rate { set; get; }
		public string loyality_Point_CR { set; get; }
		public string loyality_Point_DR { set; get; }
		public string Total_Points_Redeemed { set; get; }
		public string Delivery_By { set; get; }
		public string Channel { set; get; }
		public string Channel_Order_Id { set; get; }
		public string is_api_sync { set; get; }

		public string Aggr_dis_amount { set; get; }
		public string Aggr_dis_pct { set; get; }
		public List<cls_tbl_billTran> tran { set; get; }
		public List<tbl_PaymentSettlement> payment { set; get; }
		public List<clsTax> taxes { set; get; }
		public List<tblAssortedItem> assortedItem { set; get; }
		public int IsModifyFlag { set; get; }
		public string bill_DiscountPct { set; get; }
	}
	public class cls_tbl_billTran
	{
		public string bill_no_fk { set; get; }
		public string ItemCode { set; get; }
		public string Qty { set; get; }
		public string Price { set; get; }
		public string TotalAmt { set; get; }
		public string Bill_Date { set; get; }
		public string Kot_date { set; get; }
		public string Comments { set; get; }
		public string Dis_Rate { set; get; }
		public string Dis_Cycling { set; get; }
		public string Dis_Code { set; get; }
		public string Dis_Amount1 { set; get; }
		public string Tax_Rate { set; get; }
		public string Sale_Tax_Cycling { set; get; }
		public string Taxable_Amount { set; get; }
		public string Tax_Amount { set; get; }
		public string Group_Dish { set; get; }
		public string cancel_qty { set; get; }
		public string item_addon { set; get; }
		public string head_id { set; get; }
		public string order_status_tran { set; get; }
		public string Done_Time { set; get; }
		public string STax_Rate { set; get; }

		public string SBC_Rate { set; get; }
		public string SBC_Amount { set; get; }
		public string KKC_Rate { set; get; }
		public string KKC_Amount { set; get; }
		public string Urgent_Item { set; get; }
		public string addon_index { set; get; }
		public string addon_index_fk { set; get; }
		public string user { set; get; }

		public string load_time { set; get; }
		public string Ready_Time { set; get; }
		public string Turnarround_Time { set; get; }


	}

	public class tbl_PaymentSettlement
	{
		public string bill_no_fk { set; get; }
		public string payment_mode { set; get; }
		public string Amount { set; get; }
		public string Cashier { set; get; }
		public string cardtype { set; get; }
		public string Invoice_No { set; get; }
		public string Bill_Date { set; get; }
		public string hold_id_fk { set; get; }
		public string created_on { set; get; }
	}

	public class tblAssortedItem
	{
		public string id { set; get; }
		public string i_code { set; get; }
		public string i_code_fk { set; get; }
		public string Qty { set; get; }
		public string bill_no { set; get; }
		public string bill_date { set; get; }
		public string created_by { set; get; }
		public string created_on { set; get; }
		public string cancel_qty { set; get; }
		public string Order_Type { set; get; }
		public string item_index { set; get; }
		public string isDiscount { set; get; }
		public string Order_status { set; get; }
		public string Step_index { set; get; }
		public string CancelType { set; get; }
		public string DishComment { set; get; }
		public string tran_rate { set; get; }
		public string tran_amount { set; get; }
	}
	public class clsTax
	{
		public string bill_no_fk { set; get; }
		public string Bill_Date { set; get; }
		public string Item_Code_FK { set; get; }
		public string Taxable_Amt { set; get; }
		public string Tax_Rate { set; get; }
		public string Tax_Amt { set; get; }
		public string Tax_Code { set; get; }
		public string Tax_Name { set; get; }
		public string Tax_Type { set; get; }
		public string Tax_Calc_Formula { set; get; }
		public string Tax_display_name { set; get; }
		public string Group_Name { set; get; }
		public string IsAssorted { set; get; }
		public string AmountBeforeTax { set; get; }
		public string AmountAfterTax { set; get; }
		public string Cashier { set; get; }
		public string Created_on { set; get; }
		public string Updated_by { set; get; }
		public string Updated_on { set; get; }
	}

	public class clsCustomer
	{
		public string GuestID { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string mobile { get; set; }
		public string Address { get; set; }
		public string dob { get; set; }
		public string city { get; set; }
		public string Createby { get; set; }
		public string email_id { get; set; }
		public string location_id { get; set; }
		public string Colony_id { get; set; }
		public string Created_on { get; set; }
		public string Location { get; set; }
		public string Remarks { get; set; }
		public string Tin_no { get; set; }
		public string SourceOfOrder { get; set; }
		public string state_id_fk { get; set; }
		public string Anniversity { get; set; }
		public string NameofInstitution { get; set; }
		public string gender { get; set; }
		public string Modify_date { get; set; }
		public string PIN_Code { get; set; }
		public string Age { get; set; }
		public string Income_Range { get; set; }
		public string City_id { get; set; }
	}

	public class clsExpMaster
	{
		public string id { get; set; }
		public string Bill_No { get; set; }
		public string Date { get; set; }
		public string acode { get; set; }
		public string bill_date { get; set; }
		public string paid_amt { get; set; }
		public string paid_date { get; set; }
		public string pay_type { get; set; }
		public string chq_no { get; set; }
		public string bank_acode { get; set; }
		public string remark { get; set; }
		public string createdby { get; set; }
		public string createdDate { get; set; }
	}
}