using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KOTPrintUtility.App_Code
{
    public interface IOnineOrderAutoAccept
    {

		/// <summary>
		/// GetOnlineOrder
		/// </summary>
		/// <returns></returns>
		DataTable GetOnlineOrder();


		/// <summary>
		/// GetBillType
		/// </summary>
		/// <param name="Bill_no"></param>
		/// <returns></returns>
		string GetBillType(string Bill_no);


		/// <summary>
		/// GetAllPunchedZomatoIds
		/// </summary>
		/// <returns></returns>
		string GetAllPunchedZomatoIds();


		/// <summary>
		/// GenerateUniqIndex
		/// </summary>
		/// <param name="I_code"></param>
		/// <param name="IsDelete"></param>
		/// <param name="dt_tblTemp_id"></param>
		/// <returns></returns>
		int GenerateUniqIndex(string I_code, int IsDelete, DataTable dt_tblTemp_id);

		/// <summary>
		/// ExecuteDMLOnline
		/// </summary>
		/// <param name="sql"></param>
		bool ExecuteDMLOnline(string sql);
		/// <summary>
		/// GetOrderDetail
		/// </summary>
		/// <param name="Bill_no"></param>
		/// <param name="outlet_id"></param>
		/// <param name="bill_Date"></param>
		/// <returns></returns>
		DataSet GetOrderDetail(string Bill_no, string outlet_id, string bill_Date);


		/// <summary>
		/// GenerateBill_HD
		/// </summary>
		/// <param name="POS_code"></param>
		/// <param name="Bill_Type"></param>
		/// <param name="Bill_Date"></param>
		/// <param name="Bill_Amount"></param>
		/// <param name="Dis_Amount"></param>
		/// <param name="Service_Charge_Amount"></param>
		/// <param name="Paid_Amount"></param>
		/// <param name="Balance"></param>
		/// <param name="Payment_Mode"></param>
		/// <param name="Card_No"></param>
		/// <param name="No_of_Item"></param>
		/// <param name="To_Whom"></param>
		/// <param name="Comments"></param>
		/// <param name="Cashier"></param>
		/// <param name="tax1"></param>
		/// <param name="roundoff"></param>
		/// <param name="Discount_by"></param>
		/// <param name="TockenNo"></param>
		/// <param name="Service_Charge"></param>
		/// <param name="sur_charge"></param>
		/// <param name="Service_Tax"></param>
		/// <param name="HoldBill_No"></param>
		/// <param name="cardType"></param>
		/// <param name="cardAmount"></param>
		/// <param name="OutletId"></param>
		/// <param name="subTotal"></param>
		/// <param name="Bill_DiscountPct"></param>
		/// <param name="dept_head"></param>
		/// <param name="OrderNo"></param>
		/// <param name="Other_Charge"></param>
		/// <param name="Objbill"></param>
		/// <param name="Bill_No"></param>
		/// <param name="dtDeal"></param>
		/// <param name="RegisterUserStatus"></param>
		/// <param name="SBC_Tax"></param>
		/// <param name="KKC_Tax"></param>
		/// <param name="order_status"></param>
		/// <param name="IsBillSattle"></param>
		/// <param name="weborderCommnet"></param>
		/// <param name="RedeemPoints"></param>
		/// <param name="lblRedeemPoints"></param>
		/// <param name="is_instant_order"></param>
		/// <returns></returns>
		bool GenerateBill_HD(string POS_code, string Bill_Type, string Bill_Date, string Bill_Amount, string Dis_Amount, string Service_Charge_Amount, string Paid_Amount, string Balance, string Payment_Mode, string Card_No, string No_of_Item, string To_Whom, string Comments, string Cashier, string tax1, string roundoff, string Discount_by, string TockenNo,
		string Service_Charge, string sur_charge, string Service_Tax, string HoldBill_No, string cardType, string cardAmount, string OutletId, string subTotal, string Bill_DiscountPct, string dept_head, string OrderNo, string Other_Charge, tbl_bill Objbill, out string Bill_No, DataTable dtDeal, string RegisterUserStatus, string SBC_Tax, string KKC_Tax,
		string order_status, string IsBillSattle, string weborderCommnet, string RedeemPoints, string lblRedeemPoints, string is_instant_order = "0");

		DataSet GetOrderStatusLog();


		Task<Cls_OnlineOrder> GetOnlineOrderFromApiAsync();

		Task<BillRespose> GenerateBill_HDAPI(clsOrdersBill dsBillDetail, string POS_code, string Bill_Type, string Bill_Date, string Bill_Amount, string Dis_Amount, string Service_Charge_Amount, string Paid_Amount, string Balance, string Payment_Mode, string Card_No, string No_of_Item, string To_Whom, string Comments, string Cashier, string tax1, string roundoff, string Discount_by, string TockenNo,
string Service_Charge, string sur_charge, string Service_Tax, string HoldBill_No, string cardType, string cardAmount, string OutletId, string subTotal, string Bill_DiscountPct, string dept_head, string OrderNo, string Other_Charge, tbl_bill Objbill, string RegisterUserStatus, string SBC_Tax, string KKC_Tax,
string order_status, string IsBillSattle, string weborderCommnet, string RedeemPoints, string lblRedeemPoints, string is_instant_order = "0");
	}
}
