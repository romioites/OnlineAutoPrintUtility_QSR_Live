using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KOTPrintUtility.App_Code
{
    class PackingCharges
    {
        //public static string Channel { get; set; }
        //public static string Channel_Id { get; set; }
        //public static string Delivery_By { get; set; }
        //public static string Aggr_dis_pct { get; set; }
        //public static string Aggr_dis_amount { get; set; }
        //public static string Agg_Tax_Calculation { get; set; }
        //public static string Aggr_Discount { get; set; }
        
        static DataTable dtCharges = null;
        static PackingCharges()
        {
            dtCharges = new DataTable();
            dtCharges.Columns.Add("chargesId");
            dtCharges.Columns.Add("Zomato_order_id");
            dtCharges.Columns.Add("Charge_name");
            dtCharges.Columns.Add("chargesValue");
            dtCharges.Columns.Add("chargesTotaltax");
            dtCharges.Columns.Add("I_Code_fk");
        }

        public static DataTable GetPackaingCharges
        {
            get
            {
                return dtCharges;
            }
        }

        public static void AddCharges(DataRow chargesRow)
        {
            dtCharges.Rows.Add(chargesRow);
        }

        public static void Clear()
        {
            dtCharges.Rows.Clear();
        }
    }
}
