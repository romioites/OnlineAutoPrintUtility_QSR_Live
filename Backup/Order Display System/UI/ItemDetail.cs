using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TuchScreenApp1Jan2013.App_Code;
using Order_Display_System.App_Code;
using System.Configuration;

namespace Order_Display_System.UI
{
    public partial class ItemDetail : Form
    {
        string KOT_No = string.Empty;
        string strID_fk = string.Empty;
        string Act_time = string.Empty;
        string id_fk = string.Empty;
        public ItemDetail()
        {
            InitializeComponent();
        }
        private void ChechAll(DataGridView Objdgv, bool IsCheck)
        {
            for (int i = 0; i < Objdgv.Rows.Count; i++)
            {
                if (IsCheck == false)
                    Objdgv.Rows[i].Cells[0].Value = true;
                else
                    Objdgv.Rows[i].Cells[0].Value = false;
            }
        }

        private void ItemDetail_Load(object sender, EventArgs e)
        {
            id_fk = string.Empty;
            DataGridViewCheckBoxColumn colobj = new DataGridViewCheckBoxColumn();
            DataGridView objGrid = new DataGridView();
            objGrid = clsTempData.GetDataGrid();
            dgvItemDetails.Columns.Add("Item Name", "Item Name");
            dgvItemDetails.Columns.Add("qty", "qty");
            dgvItemDetails.Columns.Add("Comments", "Comments");
            dgvItemDetails.Columns.Add("id", "id");
            dgvItemDetails.Columns.Add("order_status", "order_status");
            dgvItemDetails.Columns.Add("Order_Type", "Order_Type");
            string Order_Type = string.Empty;
            for (int i = 0; i < objGrid.Rows.Count; i++)
            {
                string ItemName = objGrid.Rows[i].Cells["Item Name"].Value.ToString();
                string qty = objGrid.Rows[i].Cells["qty"].Value.ToString();
                string Comments = objGrid.Rows[i].Cells["Comments"].Value.ToString();
                //Act_time = objGrid.Rows[i].Cells["Act_time"].Value.ToString();
                string id = objGrid.Rows[i].Cells["id"].Value.ToString();
                string order_status = objGrid.Rows[i].Cells["order_status"].Value.ToString();
                Act_time = "10:15";
                Order_Type = objGrid.Rows[i].Cells["Order_Type"].Value.ToString();
                dgvItemDetails.Rows.Add(ItemName, qty, Comments, id, order_status, Order_Type);
            }
            colobj.Name = "Sel";
            dgvItemDetails.Columns.Insert(0, colobj);
            dgvItemDetails.Columns["Sel"].Width = 20;
            dgvItemDetails.Columns[0].HeaderText = "";
            dgvItemDetails.EnableHeadersVisualStyles = false;
            dgvItemDetails.Columns["Item Name"].ReadOnly = true;
            dgvItemDetails.Columns["qty"].ReadOnly = true;
            dgvItemDetails.Columns["Comments"].ReadOnly = true;
            dgvItemDetails.Columns["id"].Visible = false;
            dgvItemDetails.Columns["order_status"].Visible = false;
            dgvItemDetails.Columns["Order_Type"].Visible = false;
            dgvItemDetails.Columns["Item Name"].Width = 200;
            dgvItemDetails.Columns["Qty"].Width = 150;
            dgvItemDetails.Columns["Comments"].Visible = false;
            dgvItemDetails.AllowUserToAddRows = false;
            dgvItemDetails.AllowUserToDeleteRows = false;
            dgvItemDetails.AllowUserToOrderColumns = false;
            dgvItemDetails.AllowUserToResizeRows = false;
            dgvItemDetails.AllowUserToResizeColumns = false;
            dgvItemDetails.AllowUserToOrderColumns = false;
            dgvItemDetails.Columns["Sel"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvItemDetails.Columns["qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvItemDetails.ColumnHeadersHeightSizeMode =
        DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            //dgvItemDetails.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvItemDetails.Columns[0].ReadOnly = false;
            KOT_No = string.Empty;
            if (dgvItemDetails.Rows.Count > 0)
            {
                string header1 = objGrid.Columns[0].HeaderText;
                string header2 = objGrid.Columns[1].HeaderText;
                dgvItemDetails.Columns[1].HeaderText = header1;
                dgvItemDetails.Columns[2].HeaderText = header2;
                this.Text = (header1).Trim();
                string[] tableNo = header1.Split('-');
                if (this.Text.Contains("Table"))
                {
                    KOT_No = tableNo[2].ToString();
                }
                else
                    KOT_No = tableNo[1].ToString();
            }
            //ADOC.GetObject.ExecuteDML("Usp_Update_View_Time @bill_No='" + KOT_No + "',@Order_Done_by='KDS'");
            Act_time = clsDLL.GetAct_Time(KOT_No, "KDS");
            if (Order_Type == "A")
            {
                dgvItemDetails.ColumnHeadersDefaultCellStyle.BackColor = Color.Red;
            }
            else
            {
                dgvItemDetails.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
            }
            foreach (DataGridViewColumn col in dgvItemDetails.Columns)
            {
                col.HeaderCell.Style.Font = new Font("Arial", 13F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;
            }
            ChangeFont(dgvItemDetails);
            dgvItemDetails.ClearSelection();
            this.dgvItemDetails.CellBorderStyle =
    DataGridViewCellBorderStyle.None;
            this.dgvItemDetails.RowHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.None;
            timer1.Start();
            this.dgvItemDetails.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.None;
        }
        private void ChangeFont(DataGridView dgv)
        {
            for (int i = 0; i < dgvItemDetails.Rows.Count; i++)
            {
                string qty = dgvItemDetails.Rows[i].Cells["qty"].Value.ToString();
                if (qty.Contains("-"))
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    style.Font = new Font(dgv.Font.OriginalFontName, 7, FontStyle.Strikeout);
                    dgv.Rows[i].DefaultCellStyle.ApplyStyle(style);
                }
                string i_name = dgv.Rows[i].Cells["Item Name"].Value.ToString();
                if (i_name.Contains(">>"))
                {
                    dgv.Rows[i].Height = 40;
                }
                dgv.Rows[i].Cells["Sel"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Rows[i].Cells["Qty"].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            this.dgvItemDetails.DefaultCellStyle.WrapMode =
   DataGridViewTriState.True;
            dgvItemDetails.SelectAll();
        }
        private void btnCLose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void objDGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //dgvItemDetails.ClearSelection();
            }
            catch (Exception ex)
            {

            }
        }
        private void UpdateKOtStstus(DataGridView obhDgv, string Ststus)
        {
            try
            {
                string sql = string.Empty;
                string sql_tran = string.Empty;
                for (int i = 0; i < obhDgv.Rows.Count; i++)
                {
                    bool IsCheck = (bool)obhDgv.Rows[i].Cells[0].FormattedValue;
                    if (IsCheck == true)
                    {
                        string id = obhDgv.Rows[i].Cells["id"].Value.ToString();
                        if (id.Length > 0)
                        {
                            id_fk = id_fk + id + ",";
                            sql += "EXEC Usp_UpdateStstus @id='" + id + "',@status='" + Ststus + "'";
                            sql_tran += "EXEC Usp_UpdateStstus_tran @id='" + id + "',@status='" + Ststus + "'";
                        }
                    }
                }
                if (sql.Length > 5)
                {
                    bool IsUpdated = ADOC.GetObject.ExecuteDML(sql);
                    if (IsUpdated == false)
                    {
                        ADOC.GetObject.ExecuteDML(sql_tran);
                    }
                    sql = string.Empty;
                    string IsPrintKOT = ConfigurationSettings.AppSettings["IsPrintKOT"].ToString();

                    if (IsPrintKOT == "1")
                    {
                        bool IsKotPrinted = PrintKOT();
                        if (IsKotPrinted == false)
                        {
                            MessageBox.Show("Kot Not Printed, Please try again", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.DialogResult = DialogResult.None;
                        }
                        else
                        {
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Operation Successfully Completed", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    MessageBox.Show("Please select item before deliver, Please try again", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
        private void btnDelivered_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateKOtStstus(dgvItemDetails, "2");
            }
            catch (Exception ex)
            {

            }
        }

        private void chkBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            int RowIndex = dgvItemDetails.CurrentCell.RowIndex;
            bool IsCheck = (bool)dgvItemDetails.Rows[RowIndex].Cells[0].FormattedValue;
            ChechAll(dgvItemDetails, IsCheck);
        }

        private void dgvItemDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int RowIndex = dgvItemDetails.CurrentCell.RowIndex;
            int ColIndex = dgvItemDetails.CurrentCell.ColumnIndex;
            bool IsCheck = (bool)dgvItemDetails.Rows[RowIndex].Cells[0].FormattedValue;
            if (IsCheck == true)
                dgvItemDetails.Rows[RowIndex].Cells[0].Value = false;
            else
                dgvItemDetails.Rows[RowIndex].Cells[0].Value = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string current_Time1 = System.DateTime.Now.ToString("HH:mm:ss");

                TimeSpan Result = TimeSpan.Parse("00:00:00");
                TimeSpan current_Time2 = TimeSpan.Parse(current_Time1);
                TimeSpan Acttime2 = TimeSpan.Parse(Act_time);
                Result = current_Time2 - Acttime2;
                lblTime.Text = Result.ToString();
            }
            catch (Exception ex)
            {

            }
        }

        private void btnPrintKOT_Click(object sender, EventArgs e)
        {
            PrintKOT();
        }


        private bool PrintKOT()
        {
            bool Result = false;
            try
            {

                clsDLL objdll = new clsDLL();
                string KOT_No_fk = KOT_No;

                string KotType = string.Empty;
                string Bill_No_fk = KOT_No;
                string BillType = "C";
                string table_name = string.Empty;
                string Cover = string.Empty;
                string Cashier = string.Empty;
                string company_name = string.Empty;
                DataSet ds = objdll.GetConfidDetail(KOT_No_fk, id_fk.TrimEnd(','));
                DataTable dtConfig = ds.Tables[0];
                string Order_Flag = "0";
                //DataGridView objDGV = new DataGridView();
                if (dtConfig.Rows.Count > 0)
                {
                    Program.OrderRemarks = dtConfig.Rows[0]["OrderRemarks"].ToString();
                    table_name = dtConfig.Rows[0]["table_name"].ToString();
                    //Cashier = dtConfig.Rows[0]["table_name"].ToString();
                    Cashier = dtConfig.Rows[0]["cashier"].ToString();
                    BillType = dtConfig.Rows[0]["bill_type"].ToString();
                    Bill_No_fk = dtConfig.Rows[0]["bill_no"].ToString();
                    company_name = dtConfig.Rows[0]["company_name"].ToString();
                    Program.Bill_Date = dtConfig.Rows[0]["Bill_Date"].ToString();
                    Order_Flag = dtConfig.Rows[0]["Order_Flag"].ToString();

                    #region Create column in datagrid
                    DataTable dtBill = ds.Tables[1];

                    objDGV.AutoGenerateColumns = false;
                    objDGV.Columns.Add("ItemName", "Item Name");
                    objDGV.Columns.Add("Rate", "Rate");
                    objDGV.Columns.Add("Qty", "Qty");
                    objDGV.Columns.Add("Tax", "Tax");
                    objDGV.Columns.Add("Total", "Total");
                    objDGV.Columns.Add("DishCode", "DishCode");
                    objDGV.Columns.Add("DishComment", "DishComment");
                    objDGV.Columns.Add("IsNewKOT", "IsNewKOT");
                    objDGV.Columns.Add("Dept", "Dept");
                    objDGV.Columns.Add("TaxRate", "TaxRate");
                    objDGV.Columns.Add("isTaxable", "isTaxable");
                    objDGV.Columns.Add("Discount", "IsDiscount");
                    objDGV.Columns.Add("AddonCode", "AddonCode");
                    objDGV.Columns.Add("AddonCode_fk", "AddonCode_fk");
                    objDGV.Columns.Add("UrgentItem", "UrgentItem");
                    objDGV.Columns.Add("Rs", "Rs");
                    objDGV.Columns.Add("Pct", "Pct");
                    objDGV.Columns.Add("Category", "Category");
                    objDGV.Columns.Add("item_index", "item_index");
                    objDGV.Columns.Add("IsCouponDeal", "IsCouponDeal");
                    objDGV.Columns.Add("Service_Tax", "Service Tax");
                    for (int i = 0; i < dtBill.Rows.Count; i++)
                    {
                        string DishCode = dtBill.Rows[i]["Dish Code"].ToString();
                        string DishName = dtBill.Rows[i]["Dish Name"].ToString();
                        double Qty = Convert.ToDouble(dtBill.Rows[i]["Qty"].ToString());
                        string Rate = dtBill.Rows[i]["Rate"].ToString();
                        string Amount = dtBill.Rows[i]["Amount"].ToString();
                        string Counter = dtBill.Rows[i]["Counter_id"].ToString();
                        string Tax = dtBill.Rows[i]["Tax"].ToString();
                        string IsTaxable = dtBill.Rows[i]["IsTaxable"].ToString();
                        string tax_rate = dtBill.Rows[i]["tax_rate"].ToString();
                        string dept = dtBill.Rows[i]["dept"].ToString();
                        string IsDiscount = dtBill.Rows[i]["IsDiscount"].ToString();
                        string Comments = dtBill.Rows[i]["Comments"].ToString();
                        string dis_rate = dtBill.Rows[i]["dis_rate"].ToString();
                        string dis_amount = dtBill.Rows[i]["dis_amount"].ToString();
                        string Category = dtBill.Rows[i]["Category"].ToString();
                        string Group_Dish = dtBill.Rows[i]["Group_Dish"].ToString();
                        string Service_Tax_Rate = dtBill.Rows[i]["Service_Tax"].ToString();
                        objDGV.Rows.Add(DishName, Convert.ToDecimal(Rate).ToString("N2"), Qty.ToString(), Tax, Convert.ToDouble(Amount), DishCode, Comments, "0", dept, tax_rate, IsTaxable, IsDiscount, "0", "0", "0", dis_amount, dis_rate, Category, Group_Dish, "0", Service_Tax_Rate);
                    }
                    #endregion
                }
                //Print KOT
                string KotTitle = "Table Billing";
                if (BillType == "H")
                    KotTitle = "Home Delivery";
                else if (BillType == "C")
                    KotTitle = "Take Away";
                cls_PrintKot objPrint = new cls_PrintKot();
                //cls_TableBilling.PrintKOT(Bill_No, Program.CompanyName, Program.KotTitle, KotType, dgvItemDetails);
                //objPrint.Print_KOT(Bill_No, Program.CompanyName, Program.KotTitle, KotType, dgvItemDetails, kotNo);
                Result = objPrint.Print_KOT(Bill_No_fk, company_name, KotTitle, KotType, objDGV, KOT_No_fk, table_name, Cover, Cashier, BillType, id_fk.TrimEnd(','), Order_Flag);
                if (Result == false)
                {
                    MessageBox.Show("Kot not Printed Successfully", "KOT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.No;
                }
                //else
                //{
                //    MessageBox.Show("Kot not Printed Successfully", "KOT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kot not Printed Successfully: " + ex.Message, "KOT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.No;
            }
            return Result;
        }
    }
}
