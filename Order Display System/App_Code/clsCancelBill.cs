using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using KOTPrintUtility.App_Code;
using KOTPrintUtility;

namespace TuchScreenApp1Jan2013.App_Code
{
    class clsCancelBill
    {
        SqlCommand cmd;
        SqlTransaction transaction;



        public bool CancelBill_Made_Unmade(string Bill_No, string reason, string cancelled_by, string @Bill_date, DataGridView ObjDgv, string Made_unMade,
            string Payment_mode, string sqlCardquery,string Channel,string Web_order_no,string zomato_order_id)
        {
            int result = 0;
            bool IsSuccess = false;
            string sql = string.Empty;
            string scon = ConfigurationSettings.AppSettings["sqlKey"].ToString();
            SqlConnection Sqlcon = new SqlConnection(scon);
            try
            {
                cmd = new SqlCommand();
                Sqlcon.Open();
                cmd.Connection = Sqlcon;
                cmd.CommandText = "Usp_Cancel_Bill";
                cmd.CommandType = CommandType.StoredProcedure;
                // Begin transaction
                transaction = Sqlcon.BeginTransaction();
                cmd.Transaction = transaction;
                cmd.Parameters.AddWithValue("@Bill_No", Bill_No);
                cmd.Parameters.AddWithValue("@reason", reason);
                cmd.Parameters.AddWithValue("@cancelled_by", cancelled_by);
                cmd.Parameters.AddWithValue("@Bill_date", Bill_date);
                result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    //if (Made_unMade == "M")
                    //{
                    //    sql = string.Empty;
                    //    for (int rowIndex = 0; rowIndex < ObjDgv.Rows.Count; rowIndex++)
                    //    {
                    //        string id = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["id"].Value);
                    //        string Qty = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["Qty"].Value);
                    //        bool IsCheck = (bool)ObjDgv.Rows[rowIndex].Cells[0].FormattedValue;
                    //        if (IsCheck == true)
                    //            Made_unMade = "M";
                    //        else
                    //            Made_unMade = "U";
                    //        sql += "EXEC dbo.Usp_Update_Bill_Detail_Cancel @Bill_No='" + Bill_No + "',@id='" + id + "',@Cancel_Type='" + Made_unMade + "';";
                    //    }
                    //}
                    //else
                    //{
                    //    for (int rowIndex = 0; rowIndex < ObjDgv.Rows.Count; rowIndex++)
                    //    {
                    //        string id = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["id"].Value);
                    //        string Qty = Convert.ToString(ObjDgv.Rows[rowIndex].Cells["Qty"].Value);
                    //        sql += "EXEC dbo.Usp_Update_Bill_Detail_Cancel @Bill_No='" + Bill_No + "',@id='" + id + "',@Cancel_Type='" + Made_unMade + "';";
                    //    }
                    //}
                    int Count = 1;
                    // execute quuery :insert in tbl_assorted_item
                    if (sql.Length > 0)
                    {
                        Count = 0;
                        new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        Count = Convert.ToInt32(cmd.ExecuteNonQuery());
                    }
                    bool CountCashCard = true;
                   
					bool WebsiteResult = true;
					if (Channel == "Website")
					{
						WebsiteResult = false;
						int res = UpdateOrderStatusOffline("0", Bill_No, Web_order_no, "Cancel", "0", "0", zomato_order_id);
						if (res > 0)
						{
							WebsiteResult = true;
						}
					}
					if (Count > 0 && CountCashCard == true && WebsiteResult)
                    {
                        //commit tran
                        cmd.Transaction.Commit();
                        IsSuccess = true;
                    }
                    else
                    {
                        // rollback tran
                        cmd.Transaction.Rollback();
                        IsSuccess = false;
                    }
                }
                else
                {
                    // rollback tran
                    cmd.Transaction.Rollback();
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                // rollback tran
                cmd.Transaction.Rollback();
                IsSuccess = false;
            }
            Sqlcon.Close();
            return IsSuccess;
        }


		public int UpdateOrderStatusOffline(string mode, string bill_no, string online_bill_no, string Current_Status, string Status_Website_api,string Status_Cloud_DB,string zomato_order_id)
		{
			int Count = 0;
			try
			{
				string sql = "dbo.Usp_PendingOrderList_CURD";
				SqlCommand cmd = new SqlCommand(sql);
				cmd.Connection = cls_SqlConnection.SqlConnectionOffline();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@bill_date", Program.DayEnd_BIllingDate);
				cmd.Parameters.AddWithValue("@mode", mode);
				cmd.Parameters.AddWithValue("@bill_no", bill_no);
				cmd.Parameters.AddWithValue("@fin_year", clsConfigSettings.fin_year);
				cmd.Parameters.AddWithValue("@online_bill_no", online_bill_no);
				cmd.Parameters.AddWithValue("@Current_Status", Current_Status);
				cmd.Parameters.AddWithValue("@Status_Cloud_DB", Status_Cloud_DB);
				cmd.Parameters.AddWithValue("@Status_Website_api", Status_Website_api);
				cmd.Parameters.AddWithValue("@zomato_order_id", zomato_order_id);
				Count = cmd.ExecuteNonQuery();				
			}
			catch (SqlException ex)
			{
				//Loging.Log(LogType.Error, "UpdateOrderStatusOnline.fail error ocurred  bill no=> " + bill_no + " error " + ex.Message);
			}
			return Count;
		}

	}
}
