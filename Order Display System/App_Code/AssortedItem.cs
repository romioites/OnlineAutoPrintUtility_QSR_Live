using KOTPrintUtility.App_Code.Data_Set;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KOTPrintUtility.App_Code
{
    class AssortedItem
    {
        public static dsAssortedTemp objds = null;

        static AssortedItem()
        {
            objds = new dsAssortedTemp();
        }
        public static double AssortedQtyTaken = 0;
        public static DataTable GetAssortedItem(string I_code_fk)
        {
            DataTable dtTempData = new DataTable();
            var Query = from TempAssortedItem in objds.Tables["tbl_AssortedItem"].AsEnumerable()
                        where TempAssortedItem.Field<Int32>("i_code_fk") == Convert.ToInt32(I_code_fk)
                        select TempAssortedItem;
            if (Query.Count() > 0)
            {
                dtTempData = Query.CopyToDataTable();
            }
            return dtTempData;
        }

        public static DataTable GetAssortedItem_ByIndex(string I_code_fk, string ItemIndex)
        {
            DataTable dtTempData = new DataTable();
            var Query = from TempAssortedItem in objds.Tables["tbl_AssortedItem"].AsEnumerable()
                        where TempAssortedItem.Field<Int32>("i_code_fk") == Convert.ToInt32(I_code_fk) && TempAssortedItem.Field<String>("ItemIndex") == Convert.ToString(ItemIndex)
                        select TempAssortedItem;
            if (Query.Count() > 0)
            {
                dtTempData = Query.CopyToDataTable();
            }
            return dtTempData;
        }
        public static DataTable RemoveAssorted_Item(string ItemIndex)
        {
            DataTable dtTempData = objds.Tables["tbl_AssortedItem"];
            //DataRow[] drr = dtTempData.Select("ItemIndex=' " + I_code_Index + " ' ");
            DataRow[] drr = dtTempData.Select("ItemIndex='" + ItemIndex.ToString().Trim() + "'");
            for (int i = 0; i < drr.Length; i++)
                drr[i].Delete();
            dtTempData.AcceptChanges();
            return dtTempData;
        }
        public static void CancelAssortedItem()
        {
            objds.Tables["tbl_AssortedItem"].Rows.Clear();
        }
        public static DataTable GetAssortedItemByIndex(string I_code_fk, string ItemIndex)
        {
            DataTable dtTempData = new DataTable();
            var Query = from TempAssortedItem in objds.Tables["tbl_AssortedItem"].AsEnumerable()
                        where TempAssortedItem.Field<Int32>("i_code_fk") == Convert.ToInt32(I_code_fk) && TempAssortedItem.Field<String>("ItemIndex") == Convert.ToString(ItemIndex)
                        select TempAssortedItem;
            if (Query.Count() > 0)
            {
                dtTempData = Query.CopyToDataTable();
            }
            return dtTempData;
        }
    }
}
