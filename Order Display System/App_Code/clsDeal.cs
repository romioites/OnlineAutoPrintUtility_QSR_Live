using KOTPrintUtility.App_Code.Data_Set;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KOTPrintUtility.App_Code
{
    class clsDeal
    {
        public static dsDeal objds = null;
        static clsDeal()
        {
            objds = new dsDeal();
        }

        #region Global variable declaration
        public class GetVariable
        {
            public static double SelectedQty = 0;
            //public static string Deal_Item = string.Empty;
            //public static string Deal_ItemIndex = "0";
            public static string DealSupperDept = string.Empty;
            public static string Deal_id_fk = string.Empty;
            public static string DealType = string.Empty;
            public static int No_of_Item_Selectable_deal = 0;
            public static int No_of_Item_Selectable_diescount = 0;
            public static double DealRate = 0;
            public static double DealAmount = 0;
            public static double DealTaxAmountWithOutSurcharge = 0;
            public static double DealSurchargeAmount = 0;
            public static double DealAVGTaxRate = 0;

            ////Sale order related new parameter add
            public static double Rate = 0;
            public static double Amount = 0;
            //public static double Dis_Rate = 0;
            //public static double Dis_Amount = 0;
        }
        #endregion

        public class GetMathod
        {
            public static DataTable GetDealItemByIndex(string I_code_fk, string ItemIndex)
            {
                DataTable dtTempData = new DataTable();
                var Query = from TempDealItem in objds.Tables["tbl_deals"].AsEnumerable()
                            where TempDealItem.Field<Int32>("i_code_fk") == Convert.ToInt32(I_code_fk) && TempDealItem.Field<String>("ItemIndex") == Convert.ToString(ItemIndex)
                            select TempDealItem;
                if (Query.Count() > 0)
                {
                    dtTempData = Query.CopyToDataTable();
                }
                return dtTempData;
            }

            public static DataTable RemoveDeal_Item(string ItemIndex)
            {
                DataTable dtTempData = objds.Tables["tbl_deals"];
                //DataRow[] drr = dtTempData.Select("ItemIndex=' " + I_code_Index + " ' ");
                DataRow[] drr = dtTempData.Select("ItemIndex='" + ItemIndex.ToString().Trim() + "'");
                for (int i = 0; i < drr.Length; i++)
                    drr[i].Delete();
                dtTempData.AcceptChanges();
                return dtTempData;
            }



            public static DataTable GetSingleDeal(string I_code_fk)
            {
                DataTable dtTempData = new DataTable();
                var Query = from TempDealItem in objds.Tables["tbl_deals"].AsEnumerable()
                            where TempDealItem.Field<Int32>("i_code_fk") == Convert.ToInt32(I_code_fk)
                            select TempDealItem;
                if (Query.Count() > 0)
                {
                    dtTempData = Query.CopyToDataTable();
                }
                return dtTempData;
            }

            public static void CancelDealItem()
            {
                objds.Tables["tbl_deals"].Rows.Clear();
            }
        }
    }

}