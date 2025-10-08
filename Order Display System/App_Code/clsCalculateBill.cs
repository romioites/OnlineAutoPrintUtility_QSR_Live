using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KOTPrintUtility.App_Code
{
    class clsCalculateBill
    {
        public static bool CalculateBillDetailQSR(List<tbl_tran> dgv, out string SubTotal_Val, out string NetTotal_Val, out string ServiceTax_Amount_Val,
   out string NetVAT_Val, out string BillTotal_Val, out string GrandTotal_Val, out string Surcharge_Val, out string DeliveryCharge_Val,
            decimal dissount_pctNoInUse, string payment_mode, out string Discount_Val, 
			out string sbc_val, out string kkc_val,string BillType,string Delivery_Charge)
        {
            bool Issuccess = false;
            string SubTotal_local = "0";
            string NetTotal_local = "0";
            string ServiceTax_Amount_local = "0";
            string NetVAT_local = "0";
            string BillTotal_local = "0";
            string GrandTotal_local = "0";
            string Surcharge_local = "0";
            string DeliveryCharge_local = "0";
            string Discount_local = "0";
            string sbcTotalAmount = "0";
            string kkcTotalAmount = "0";
            try
            {
                string _Service_Tax_cap = string.Empty;
                string _Surcharge_cap = string.Empty;
				double TotalDiscount = 0;
				//declate global vriable
				double SubTotal = 0;
				double NetTotal = 0;
				double TotalVat = 0;
				double DeliveryCharge = 0;
                string Sur_ChargeCap = "0";
                if (_Surcharge_cap != "0")
                {
                    Sur_ChargeCap = "0";
                }
                double ServiceTxAmnt = 0;
                if (BillType == "H")
                {
                    if (Program.ZomatoOrderNo.Trim().Length > 0)
                    {
                        DeliveryCharge = Convert.ToDouble(Delivery_Charge);
                    }
                    else
                    {
                        DeliveryCharge = Convert.ToDouble(clsConfigSettings.DeliveryCharge);
                    }
                }
                if (BillType == "D")
                    DeliveryCharge = Convert.ToDouble(clsConfigSettings.pkg_charge_dn);
                if (BillType == "C")
                    DeliveryCharge = Convert.ToDouble(clsConfigSettings.pkg_charge_TA);


                for (int rowIndex = 0; rowIndex < dgv.Count; rowIndex++)
                {
					double Vat = 0;
					double SBC = 0;
					double KKC = 0;
					double Rate = Convert.ToDouble(dgv[rowIndex].Rate);
					double Qty = Convert.ToDouble(dgv[rowIndex].qty);
                    double ServiceTaxCap = 0;
                    // calculate amount by rate and qty
                    double TotalAmount = (Rate * Qty);
					// assign amount in order list
					dgv[rowIndex].amount = TotalAmount;
					double Tax_Rate = Convert.ToDouble(dgv[rowIndex].TaxRate);
					double amnt = Convert.ToDouble(dgv[rowIndex].amount);
					double DisAmount = Convert.ToDouble(dgv[rowIndex].dis_amount);
                    if (_Service_Tax_cap != "0")
                    {
						ServiceTaxCap = 0;
                    }
                  //  string IsDiscount = dgv.Rows[rowIndex].Cells["Discount"].Value.ToString();
                    //calculate sub total
                    SubTotal += amnt;
					//calculate taxable amount
					double TaxableAmont = amnt - DisAmount;
                    Vat = (TaxableAmont * Tax_Rate / 100);
                    dgv[rowIndex].tax = Vat.ToString("N3");
                    //calculate total vat
                    TotalVat += Vat;

                    TotalDiscount += DisAmount;

                    //calculate net total
                    NetTotal += TaxableAmont;
                }
                double GrandTotal = 0;
                // Sub total amount/ before discount
                SubTotal_local = SubTotal.ToString("N2");
                // Net Total Amount
                NetTotal_local = NetTotal.ToString("N2");

                ///     Service Tax====================  Net Total Amount * 4.944 /100=================     
                ///     
                string DelChargeAmt = "0";
                if (Convert.ToDouble(DeliveryCharge) > 0)
                {
                    for (int rowIndex = 0; rowIndex < dgv.Count; rowIndex++)
                    {
						double total = Convert.ToDouble(dgv[rowIndex].amount.ToString());
						double DisAmount = Convert.ToDouble(dgv[rowIndex].dis_amount.ToString());
						double TaxableAmont = total - DisAmount;
                        string DishCode = dgv[rowIndex].Assorted_Item.ToString();
                        //DelveryCharge
                        if (clsConfigSettings.DelCharge_type != "rs")
                            DelChargeAmt = (NetTotal * DeliveryCharge / 100).ToString("N2");
                        else
                            DelChargeAmt = (DeliveryCharge).ToString("N2");
                        if (clsConfigSettings.GST_on_delivery == "1" && Convert.ToDecimal(NetTotal_local) > 0)
                        {
                            double PerUnitamt = Convert.ToDouble(DelChargeAmt) / Convert.ToDouble(NetTotal_local);

							double NetAmount = TaxableAmont * PerUnitamt;

                            double GST_on_DeliveryCharge = clsGetTaxDetail.GetNewRateTran(DishCode, NetAmount.ToString("N6").Replace(",", ""));
                            TotalVat += Convert.ToDouble(GST_on_DeliveryCharge);

                            double gst = Convert.ToDouble(dgv[rowIndex].tax.ToString());
                            dgv[rowIndex].tax = (GST_on_DeliveryCharge + gst).ToString("N4");
                        }
                    }
                }
                ServiceTax_Amount_local = 0.ToString("N2");
                sbcTotalAmount = 0.ToString("N2");
                kkcTotalAmount = 0.ToString("N2");
                //vat
                NetVAT_local = TotalVat.ToString("N3");

                Surcharge_local = 0.ToString("N2");
                if (clsConfigSettings.DelCharge_type != "rs")
                {
                    DelChargeAmt = (NetTotal * DeliveryCharge / 100).ToString("N2");
                    DeliveryCharge = Convert.ToDouble(DelChargeAmt.ToString());
                }
                // total vat
                Discount_local = TotalDiscount.ToString("N2");

                double TotalTaxAmount = Convert.ToDouble(NetVAT_local);
                //DelveryCharge
                DeliveryCharge_local = DeliveryCharge.ToString("N2");
                //gross amount amount
                GrandTotal = Convert.ToDouble(NetTotal.ToString()) + TotalTaxAmount + DeliveryCharge;
                BillTotal_local = GrandTotal.ToString("N2");
                //grand total/ net payable
                GrandTotal_local = Math.Round(GrandTotal).ToString("N2");
                Issuccess = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Problem occurred please logout and login again " + ex.Message);
                Issuccess = false;
            }
            sbc_val = sbcTotalAmount;
            kkc_val = kkcTotalAmount;
            SubTotal_Val = SubTotal_local;
            NetTotal_Val = NetTotal_local;
            ServiceTax_Amount_Val = ServiceTax_Amount_local;
            NetVAT_Val = NetVAT_local;
            Surcharge_Val = Surcharge_local;
            Discount_Val = Discount_local;
            DeliveryCharge_Val = DeliveryCharge_local;
            BillTotal_Val = BillTotal_local;
            GrandTotal_Val = GrandTotal_local;
            return Issuccess;
        }

        public static string GetTaxRate(string TaxSlot)
        {
            string TaxRate = "0.00";
            try
            {
                string sqlKey = ConfigurationSettings.AppSettings["sqlKey"].ToString();
                using (SqlConnection con = new SqlConnection(sqlKey))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.USP_GetTaxRate", con))
                    {
                        cmd.Parameters.AddWithValue("@Tax_slot", TaxSlot);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                TaxRate = Convert.ToDouble(dt.Rows[0]["TaxRate"]).ToString("N3");
                            }
                        }
                    }
                }
            }
            catch { }
            return TaxRate;
        }
        public static void CalculateBillDetail(DataGridView dgv, out string SubTotal_Val, decimal dissount_pct, out string Dis_Amount_Val,
        out string NetTotal_Val, out string ServiceCharge_Amount_Val, out string ServiceTax_Amount_Val, out string NetVAT, out string SurCharge_val,
        out double BillTotalVat_Val, out string GrandTotal_Val, string BillType,
		out string SBC_Val, out string KKC_Val, out string DeliveryCharge_Val)
        {
            decimal dis_pct_item = 0;        
            decimal Net_Tax = 0;
            decimal ServiceTxAmnt = 0;
            decimal SubTotal = 0;
            decimal NetTotal = 0;
            decimal TotalVat = 0;
            decimal TotalDiscount = 0;
          
            string DeliveryCharge_local = "0";
            decimal SBC_local = 0;
            decimal KKC_local = 0;
            decimal DeliveryCharge = 0;
           
            if (BillType == "H")
            {                
                if (Program.ZomatoOrderNo.Trim().Length > 0)
                {                    
                    decimal.TryParse(Program.Delivery_Charge, out DeliveryCharge);
                }
                else
                {
                    decimal.TryParse(clsConfigSettings.DeliveryCharge, out DeliveryCharge);                   
                }
            }
            decimal Netamnt = 0;
            for (int rowIndex = 0; rowIndex < dgv.Rows.Count; rowIndex++)
            {
                decimal DisAmount = 0;
                decimal Rate = Convert.ToDecimal(dgv.Rows[rowIndex].Cells["Rate"].Value);
                decimal Qty = Convert.ToDecimal(dgv.Rows[rowIndex].Cells["Qty"].Value);
                string Discount = dgv.Rows[rowIndex].Cells["Discount"].Value.ToString();
                decimal TotalAmount = (Rate * Qty);
                if (dissount_pct == 0 )
                {
                    dis_pct_item = Convert.ToDecimal(dgv.Rows[rowIndex].Cells["Dis Pct"].Value.ToString());
                    DisAmount = TotalAmount * dis_pct_item / 100;
                    if (DisAmount > 0)
                        dgv.Rows[rowIndex].Cells["Discount"].Value = "Yes";
                }
                else
                {
                    if (Discount.ToLower() == "yes")
                    {
                        dis_pct_item = Convert.ToDecimal(dgv.Rows[rowIndex].Cells["Dis Pct"].Value);
                        DisAmount = TotalAmount * dis_pct_item / 100;
                    }
                }
                Netamnt += (TotalAmount - DisAmount);
            }
            for (int rowIndex = 0; rowIndex < dgv.Rows.Count; rowIndex++)
            {
                decimal STax_Amount = 0;
                decimal DisAmount = 0;
                decimal Vat = 0;
                decimal Rate = 0;
                Rate = Convert.ToDecimal(dgv.Rows[rowIndex].Cells["Rate"].Value);
                decimal Qty = Convert.ToDecimal(dgv.Rows[rowIndex].Cells["Qty"].Value);
                decimal TotalAmount = (Rate * Qty);
                dgv.Rows[rowIndex].Cells["total"].Value = TotalAmount.ToString("N4");
                dgv.Rows[rowIndex].Cells["Qty"].Value = Qty.ToString("N4");
                decimal Tax_Rate = Convert.ToDecimal(dgv.Rows[rowIndex].Cells["TaxRate"].Value);
                string Discount = dgv.Rows[rowIndex].Cells["Discount"].Value.ToString();
                decimal amnt = Convert.ToDecimal(dgv.Rows[rowIndex].Cells["total"].Value);
                decimal ServiceTax_Cap = 0;
                SubTotal += amnt;


                if (dissount_pct == 0)
                {
                    dis_pct_item = Convert.ToDecimal(dgv.Rows[rowIndex].Cells["Dis Pct"].Value);
                    DisAmount = amnt * dis_pct_item / 100;
                }
                else
                {
                    if (Discount.ToLower() == "yes")
                    {
                        dis_pct_item = Convert.ToDecimal(dgv.Rows[rowIndex].Cells["Dis Pct"].Value);
                        DisAmount = amnt * dis_pct_item / 100;
                    }
                }
                amnt = amnt - DisAmount;

                //=======================================
                string IsRestaurantItem = "0";
                try
                {
                    if (dgv.Columns.Contains("IsRestaurantItem") && BillType == "H")
                        IsRestaurantItem = dgv.Rows[rowIndex].Cells["IsRestaurantItem"].Value.ToString();

                }
                catch { }
                //////***************Modefied GST Related******************************************************
                //if (clsConfigSettings.IsOnlineOrderHD == "1" && Program.BillType == "H" && IsRestaurantItem == "1")
                //{
                //    Vat = 0;
                //    dgv.Rows[rowIndex].Cells["TaxRate"].Value = "0";
                //}
                //else
                    Vat = amnt * Tax_Rate / 100;

                dgv.Rows[rowIndex].Cells["Tax"].Value = Vat.ToString("N4");
               

                TotalVat += Vat;
                NetTotal += amnt;
                TotalDiscount += DisAmount;
                ServiceTxAmnt += STax_Amount;
            }
            if (dis_pct_item > 0)
                dissount_pct = dis_pct_item;
            double ServeceChargeAmnt = 0;
            double SurChargeAmnt = 0;
            double GrandTotal = 0;
            // service charge = ==============10 % of bill amount
            //ServeceChargeAmnt = Convert.ToDouble(NetTotal) * Convert.ToDouble(Service_Charge) / 100;
            ///         Sur Charge==================== 5% of Total VAT
            //SurChargeAmnt = Convert.ToDouble(TotalVat) * double.Parse(SerChargeCap) / 100;
            // Sub total amount/ before discount
            SubTotal_Val = SubTotal.ToString("N4");
            // Net Total Amount
            NetTotal_Val = NetTotal.ToString("N4");


            string DelChargeAmt = "0";
            if (BillType == "H")
            {
                //DelveryCharge
                if (clsConfigSettings.DelCharge_type.Trim().ToLower() != "rs")
                {
                    //delvery charhe in case of home delivery
                    if (Program.ZomatoOrderNo.Trim().Length > 0)
                        DelChargeAmt = (DeliveryCharge).ToString("N3");
                    else
                        DelChargeAmt = (NetTotal * DeliveryCharge / 100).ToString("N3");
                }
                else
                    DelChargeAmt = (DeliveryCharge).ToString("N3");
                DeliveryCharge_local = DelChargeAmt.ToString();
                // GST ON DELIVERY
                if (clsConfigSettings.GST_on_delivery == "1")
                {
                    //string taxRate = "0.00";
                    //if (string.IsNullOrEmpty(clsConfigSettings.DelCharge_TaxRate.Trim()))
                    //{
                    //    taxRate = GetTaxRate(clsConfigSettings.DelCharge_TaxSlot);
                    //    clsConfigSettings.DelCharge_TaxRate = taxRate;
                    //}
                    //else
                    //{
                    //    taxRate = clsConfigSettings.DelCharge_TaxRate;
                    //}
                    //decimal GST_on_DeliveryCharge = Convert.ToDecimal(DelChargeAmt) * Convert.ToDecimal(taxRate) / 100;
                    //TotalVat += Convert.ToDecimal(GST_on_DeliveryCharge);
                }
            }
            // Discount Amount
            Dis_Amount_Val = TotalDiscount.ToString("N4");
            ///tax detail
            ServiceCharge_Amount_Val = ServeceChargeAmnt.ToString("N4");
            ServiceTax_Amount_Val = ServiceTxAmnt.ToString("N4");
            SurCharge_val = SurChargeAmnt.ToString("N4");
            NetVAT = TotalVat.ToString("N4");
            SBC_Val = SBC_local.ToString("N4");
            KKC_Val = KKC_local.ToString("N4");
            DeliveryCharge_local = DelChargeAmt;
            DeliveryCharge_Val = DeliveryCharge_local;
            // total vat
            BillTotalVat_Val = Convert.ToDouble(TotalVat.ToString("N4")) + Convert.ToDouble(SurCharge_val) + Convert.ToDouble(ServiceTax_Amount_Val) +
            Convert.ToDouble(ServiceCharge_Amount_Val) + Convert.ToDouble(SBC_Val) + Convert.ToDouble(KKC_Val);
            //Garand tottal / Net payable amount

            //***************Old
            GrandTotal = double.Parse(NetTotal.ToString()) + BillTotalVat_Val + Convert.ToDouble(DelChargeAmt);


            //New Add ***************
            //GrandTotal = double.Parse(NetTotal.ToString()) + double.Parse(Net_Tax.ToString());
            //***********************
            //GrandTotal_Val = Math.Round(GrandTotal, 0, MidpointRounding.AwayFromZero).ToString("N4");
            GrandTotal_Val = Math.Round(Convert.ToDouble(GrandTotal.ToString("N2")), MidpointRounding.AwayFromZero).ToString("N2");
            //Program.OrderBooking = "";
        }
    }
}
