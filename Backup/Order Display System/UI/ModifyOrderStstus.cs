using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace TuchScreenApp1Jan2013.UI
{
    public partial class ModifyOrderStstus : Form
    {
        public ModifyOrderStstus()
        {
            InitializeComponent();
        }

        private void ModifyOrderStstus_Load(object sender, EventArgs e)
        {
            DisPlayData();
            timer1.Start();
        }
        public void DisPlayData()
        {
            try
            {
                dgvOrderDetail.EnableHeadersVisualStyles = false;
                dgvOrderDetail.ColumnHeadersDefaultCellStyle.BackColor = Color.Green;
                DataTable dt = ADOC.GetObject.GetTable("Usp_getAllOrder");
                dgvOrderDetail.DataSource = dt;
                dgvOrderDetail.ClearSelection();
            }
            catch (Exception ex)
            {

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvOrderDetail_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dataGridView = sender as DataGridView;

            e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
            dgvOrderDetail.ClearSelection();
        }

        private void dgvOrderDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                int columnIndex = dgvOrderDetail.CurrentCell.ColumnIndex;

                if (dgvOrderDetail.Columns[columnIndex].Name == "Modify")
                {
                    if (DialogResult.Yes == MessageBox.Show("Do you want finish this order ??", "Manage Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        string Bill_no = dgvOrderDetail.Rows[e.RowIndex].Cells["bill_no"].Value.ToString();
                        bool IsUpdated = ADOC.GetObject.ExecuteDML("update tbl_bill set orderStatus=1 where bill_no='" + Bill_no + "'");
                        if (IsUpdated == true)
                        {
                            MessageBox.Show("Order Updated Successfully", "Manage Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DisPlayData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DisPlayData();
        }
    }
}