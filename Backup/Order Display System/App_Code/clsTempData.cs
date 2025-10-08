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
    }
}
