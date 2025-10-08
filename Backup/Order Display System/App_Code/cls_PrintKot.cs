using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TuchScreenApp1Jan2013.UI;
using System.Data;

namespace Order_Display_System.App_Code
{
    class cls_PrintKot
    {

        public bool Print_KOT(string BillNo, string companyName, string KotTitle, string KOT_Type, DataGridView objDgv, string kot_no, string TableName,
            string TableCover, string CashierName, string BillType, string id_fk, string Order_flag)
        {
            bool Result = false;
            clsPrintKOTsummary objKOtSummery = new clsPrintKOTsummary();
            try
            {
                if (Program.OrderRemarks == "")
                    Program.OrderRemarks = "";
                if (clsConfigSettings.IS_KOT_Print_Port == "1")
                    Result = objKOtSummery.Print_KOT_Counter_DeptWise_PortWise(BillNo, companyName, KotTitle, KOT_Type, objDgv, kot_no, CashierName, TableCover, BillType, TableName);
                else if (clsConfigSettings.IS_KOT_Print_Port == "2")
                    Result = objKOtSummery.PrintKOT2Inch(BillNo, companyName, KotTitle, KOT_Type, objDgv, kot_no, TableName, Program.OrderRemarks, CashierName, TableCover, BillType);
                else if (clsConfigSettings.IS_KOT_Print_Port == "3")
                {
                    TSC_Print objCls = new TSC_Print();
                    Result = objCls.Print_KOT_LableWise(BillNo, companyName, KotTitle, KOT_Type, objDgv, kot_no, CashierName, BillType, TableCover, TableName,Order_flag,id_fk);
                }
                else
                    Result = objKOtSummery.Print_KOT_Counter_DeptWise(BillNo, companyName, KotTitle, KOT_Type, objDgv, kot_no, TableName, TableCover, CashierName, BillType);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Print_KOT: " + ex.Message, "KOT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
                //this.DialogResult = DialogResult.No;
            }
            return Result;
        }

    }
}
