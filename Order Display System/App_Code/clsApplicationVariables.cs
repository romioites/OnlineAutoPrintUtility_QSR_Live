using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KOTPrintUtility.App_Code
{
    class clsApplicationVariables
    {
        public class clsBill
        {
            //Bill Variables
            public static string Bill_No = "0";
            public static string Bill_SubTotal = "0";
            public static string Bill_Discount = "0";
            public static string ReturnAmount = string.Empty;
            public static string Bill_TaxAmount = "0";
            public static string Bill_GTotal = "0";
            public static string Bill_DiscountAmount = "0";
            public static string Bill_DiscountPct = "0";
            public static string Bill_Type = string.Empty;
           // public static string Bill_TypeName = string.Empty;
            public static string Bill_Cancel_By = string.Empty;
        }
    }
}
