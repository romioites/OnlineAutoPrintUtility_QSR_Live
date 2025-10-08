using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace KOTPrintUtility.App_Code
{
    public class clsBLL
    {
        public static void GetPaymentMode(string BillNo, out string Pay_Mpde, out string CardName)
        {
            ADOC objADOC = new ADOC();
            string payment_modeString = string.Empty;
            string Card_Type = string.Empty;
            DataSet ds = objADOC.GetObject.GetDatset("Usp_GetPayMentModeDistinct @bill_no_fk='" + BillNo + "'", "tbl");
            if (ds.Tables[0].Rows.Count > 0 || ds.Tables[1].Rows.Count > 0)
            {
                DataTable dt_payment_mode = ds.Tables[1];
                DataTable dt_Card_Detail = ds.Tables[0];

                // payment mode
                if (dt_payment_mode.Rows.Count > 0)
                {
                    for (int i = 0; i < dt_payment_mode.Rows.Count; i++)
                    {
                        string Mode = dt_payment_mode.Rows[i]["Payment mode"].ToString();
                        payment_modeString = payment_modeString + "," + Mode;
                    }
                    payment_modeString = payment_modeString.Remove(0, 1);
                }

                // card type
                if (dt_Card_Detail.Rows.Count > 0)
                {
                    for (int i = 0; i < dt_Card_Detail.Rows.Count; i++)
                    {
                        string Mode = dt_Card_Detail.Rows[i]["card_type"].ToString();
                        Card_Type = Card_Type + "," + Mode;
                    }
                    Card_Type = Card_Type.Remove(0, 1);
                }
            }
            Pay_Mpde = payment_modeString;
            CardName = Card_Type;
        }

    }
}
