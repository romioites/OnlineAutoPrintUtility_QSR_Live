using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KOTPrintUtility.App_Code.Data_Set;
namespace KOTPrintUtility.App_Code
{
    class clsItems
    {

        public static dsItems objItems = null;
        static clsItems()
        {
            objItems = new dsItems();
        }
        public class GetMathod
        {

            public static DataTable GetCombobyStepName(string StepNameNsme, DataTable dt)
            {
                DataTable dtTempData = new DataTable();
                var Query = from TempDealItem in dt.AsEnumerable()
                            where TempDealItem.Field<String>("Step_Name").ToLower() == Convert.ToString(StepNameNsme)
                            select TempDealItem;
                if (Query.Count() > 0)
                {
                    dtTempData = Query.CopyToDataTable();
                }
                return dtTempData;
            }

            #region Get Item Index by tbl_items_tran or tbl_items table...
            //public static DataTable GetItemByIndex(string I_code_fk, string Index)
            //{
            //    DataTable dtTempData = new DataTable();
            //    var Query = from TempDealItem in objItems.Tables["tbl_Items_tran"].AsEnumerable()
            //                where TempDealItem.Field<Int32>("i_code_fk") == Convert.ToInt32(I_code_fk) && TempDealItem.Field<String>("Index") == Convert.ToString(Index)
            //                select TempDealItem;
            //    if (Query.Count() > 0)
            //    {
            //        dtTempData = Query.CopyToDataTable();
            //    }
            //    return dtTempData;
            //}

            public static DataTable GetItemByIndex(string I_code_fk, string Index)
            {
                DataTable dtTempData = new DataTable();
                var Query = from TempDealItem in objItems.Tables["tbl_Items"].AsEnumerable()
                            where TempDealItem.Field<Int32>("i_code_fk") == Convert.ToInt32(I_code_fk) && TempDealItem.Field<String>("Index") == Convert.ToString(Index)
                            select TempDealItem;
                if (Query.Count() > 0)
                {
                    dtTempData = Query.CopyToDataTable();
                }
                return dtTempData;
            }

            #endregion

            public static void AddItemsinList(string ItemName, string Qty, string I_Code, string IsDiscount, string Amount,
                                              string Step_Name, string No_of_Item, string i_type, string Deal_TYpe,
                string Index, string I_code_fk, string TaxRate, string Index_No, string DishComment)
            {
                int Item_Code_fk = Convert.ToInt32(I_code_fk);
                int Item_Code = Convert.ToInt32(I_Code);
                double Item_Qty = Convert.ToDouble(Qty);
                double Item_Amount = Convert.ToDouble(Amount);
                double Item_TaxRate = Convert.ToDouble(TaxRate);

                objItems.Tables["tbl_Items"].Rows.Add(ItemName, Item_Qty, Item_Code, IsDiscount, Item_Amount, Step_Name, No_of_Item,
                    i_type, Deal_TYpe, Index, I_code_fk, Item_TaxRate, Index_No, DishComment);
            }

            public static void CancelItem()
            {
                objItems.Tables["tbl_Items"].Rows.Clear();
            }

            public static DataTable RemoveItem(string Index)
            {
                DataTable dtTempData = objItems.Tables["tbl_Items"];
                //DataRow[] drr = dtTempData.Select("ItemIndex=' " + I_code_Index + " ' ");
                DataRow[] drr = dtTempData.Select("Index='" + Index.ToString().Trim() + "'");
                for (int i = 0; i < drr.Length; i++)
                    drr[i].Delete();
                dtTempData.AcceptChanges();
                return dtTempData;
            }

            //public static DataTable RemoveItemTran(string Index)
            //{
            //    DataTable dtTempData = objItems.Tables["tbl_Items_tran"];
            //    //DataRow[] drr = dtTempData.Select("ItemIndex=' " + I_code_Index + " ' ");
            //    DataRow[] drr = dtTempData.Select("Index='" + Index.ToString().Trim() + "'");
            //    for (int i = 0; i < drr.Length; i++)
            //        drr[i].Delete();
            //    dtTempData.AcceptChanges();
            //    return dtTempData;
            //}

            public static DataTable GetSingleDeal(string I_code_fk)
            {
                DataTable dtTempData = new DataTable();
                var Query = from TempDealItem in objItems.Tables["tbl_Items"].AsEnumerable()
                            where TempDealItem.Field<Int32>("i_code_fk") == Convert.ToInt32(I_code_fk)
                            select TempDealItem;
                if (Query.Count() > 0)
                {
                    dtTempData = Query.CopyToDataTable();
                }
                return dtTempData;
            }
            //public static DataTable GetSingleDeal_tran(string I_code_fk)
            //{
            //    DataTable dtTempData = new DataTable();
            //    var Query = from TempDealItem in objItems.Tables["tbl_Items_tran"].AsEnumerable()
            //                where TempDealItem.Field<Int32>("i_code_fk") == Convert.ToInt32(I_code_fk)
            //                select TempDealItem;
            //    if (Query.Count() > 0)
            //    {
            //        dtTempData = Query.CopyToDataTable();
            //    }
            //    return dtTempData;
            //}

            public static void CancelItemTemp()
            {
                objItems.Tables["tbl_Items"].Rows.Clear();
            }

            //public static void CancelItemTemp_tran()
            //{
            //    objItems.Tables["tbl_Items_tran"].Rows.Clear();
            //}






            public static void AddItemsinList(string ItemName, string Qty, string I_Code, string IsDiscount, string Amount,
                                            string Step_Name, string No_of_Item, string i_type, string Deal_TYpe,
              string Index, string I_code_fk, string TaxRate, string Index_No, string isNewKOT, string total, string service_tax)
            {
                int Item_Code_fk = Convert.ToInt32(I_code_fk);
                int Item_Code = Convert.ToInt32(I_Code);
                double Item_Qty = Convert.ToDouble(Qty);
                double Item_Amount = Convert.ToDouble(Amount);
                double Item_TaxRate = Convert.ToDouble(TaxRate);

                objItems.Tables["tbl_Items"].Rows.Add(ItemName, Item_Qty.ToString("N4"), Item_Code, IsDiscount, Item_Amount, Step_Name, No_of_Item,
                    i_type, Deal_TYpe, Index, I_code_fk, Item_TaxRate, Index_No, isNewKOT, total, service_tax);
            }

            public static DataTable GetItem_ByIndex(string I_code_fk, DataTable dt, string item_index)
            {
                DataTable dtTempData = new DataTable();
                var Query = from TempAssortedItem in dt.AsEnumerable()
                            where TempAssortedItem.Field<Int64>("i_code_fk") == Convert.ToInt64(I_code_fk) && TempAssortedItem.Field<String>("index") == item_index
                            select TempAssortedItem;
                if (Query.Count() > 0)
                {
                    dtTempData = Query.CopyToDataTable();
                }
                return dtTempData;
            }

            public static DataTable GetItemIsNewKOTqty(DataTable dt)
            {
                DataTable dtTempData = new DataTable();
                var Query = from TempAssortedItem in dt.AsEnumerable()
                            where TempAssortedItem.Field<String>("isNewKOT") == "1"
                            select TempAssortedItem;
                if (Query.Count() > 0)
                {
                    dtTempData = Query.CopyToDataTable();
                }
                return dtTempData;
            }



            //public static void AddItemsinList_Tran(string ItemName, string Qty, string I_Code, string IsDiscount, string Amount,
            //                                  string Step_Name, string No_of_Item, string i_type, string Deal_TYpe, string Index, string I_code_fk, string TaxRate, string Index_No)
            //{
            //    int Item_Code_fk = Convert.ToInt32(I_code_fk);
            //    int Item_Code = Convert.ToInt32(I_Code);
            //    double Item_Qty = Convert.ToDouble(Qty);
            //    double Item_Amount = Convert.ToDouble(Amount);
            //    double Item_TaxRate = Convert.ToDouble(TaxRate);
            //    objItems.Tables["tbl_Items_tran"].Rows.Add(ItemName, Item_Qty.ToString("N4"), Item_Code, IsDiscount, Item_Amount, Step_Name, No_of_Item,
            //        i_type, Deal_TYpe, Index, I_code_fk, Item_TaxRate, Index_No);
            //}

        }
        class clsOfferDetail
        {
            public static void AddItemsinOffer(string deal_id_fk, string No_of_Item, string bill_type, string Discount_label,
            string i_code, string i_name, string discount_type, string discount_value, string Deal_Type, string Index, string DealName)
            {
                // objItems.Tables["dtOffer"].Rows.Add(DealName, No_of_Item, Item_Code, IsDiscount, Item_Amount, Step_Name, No_of_Item,
                //   i_type, Deal_TYpe, Index, I_code_fk, Item_TaxRate);
            }
        }


    }


}