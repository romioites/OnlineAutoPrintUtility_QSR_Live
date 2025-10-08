using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
namespace TuchScreenApp1Jan2013.App_Code
{

    public static class clsTempData
    {
        static DataGridView dgvobj = new DataGridView();
        static DataTable dtvobj = new DataTable();
        public static void SetDataGrid(DataGridView dgv)
        {
            dgvobj = dgv;
        }
        public static DataGridView GetDataGrid()
        {
            return dgvobj;
        }

        public static void SetDatatable(DataTable dt)
        {
            dtvobj = dt;
        }
        public static DataTable GetDatatable()
        {
            return dtvobj;
        }
        public static bool IsConnectedToInternet()
        {
            bool isConnected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            if (isConnected == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool InsertInGrid(DataGridView dgvPayment, string Mode_Id, string Mode, string CardType, string Amount, string ModeValue, string hold_id)
        {
            bool IsInserted = false;
            try
            {
                if (dgvPayment.Rows.Count == 0)
                {
                    dgvPayment.Columns.Add("id", "id");
                    dgvPayment.Columns.Add("Payment Mode", "Payment Mode");
                    dgvPayment.Columns.Add("Type", "Type");
                    dgvPayment.Columns.Add("Amount", "Amount");
                    dgvPayment.Columns.Add("ModeValue", "ModeValue");
                    dgvPayment.Columns.Add("hold_id", "hold_id");
                }
                dgvPayment.Rows.Add(Mode_Id, Mode, CardType, Amount, ModeValue, hold_id);
                IsInserted = true;
            }
            catch (Exception ex)
            {
                IsInserted = false;
            }
            return IsInserted;
        }
    }
}
