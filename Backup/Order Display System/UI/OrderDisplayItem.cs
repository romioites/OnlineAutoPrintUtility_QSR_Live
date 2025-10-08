using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Order_Display_System;
using TuchScreenApp1Jan2013.App_Code;
using Order_Display_System.UI;
using System.Runtime.InteropServices;
using RTF;


namespace TuchScreenApp1Jan2013.UI
{
    public partial class OrderDisplayItem : Form
    {
        public OrderDisplayItem()
        {
            InitializeComponent();
        }
        private static IntPtr moduleHandle;
        //private static int richEditMajorVersion=4;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string libname);

        int coun = 0;
        DataGridView dgv;
        byte[] Dept_Image = new byte[0];
        string ORder_status_var = "0";
        private void OrderDisplayItem_Load(object sender, EventArgs e)
        {
            try
            {
                if (clsTempData.IsConnectedToInternet() == true)
                {
                    Program.CurrentKOTNO = string.Empty;
                    //FullScreenMode.FullScreen objFullscreen = new FullScreenMode.FullScreen(this);
                    //objFullscreen.ShowFullScreen();
                    // DisPlayData();
                    DisplayOrder();
                    //flowLayoutPanel1.Controls.OfType<VScrollBar>().First().Width = 20;
                    //flowLayoutPanel1.Controls.OfType<HScrollBar>().First().Height = 15; 
                    coun++;
                }
                else
                {
                    MessageBox.Show("Check Network Connection");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Code : 1 " + ex.Message + " " + "Check Network Connection");
                return;
            }

        }
        private int xPos = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //DisPlayData();
            DisplayOrder();
        }

        private void dgvOrderDetail_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dataGridView = sender as DataGridView;
            e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
        }
        private void DIsplayText(string NoofRunningtbl, string Table_Covered)
        {
            try
            {
                string Time = DateTime.Now.ToString("hh:mm tt");
                this.Text = "Time : " + Time + " ," + "         Running Table   : " + NoofRunningtbl + ",    Total Covered : " + Table_Covered;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Code : 6 " + ex.Message);
            }
        }

        private void DisplayOrder()
        {
            try
            {
                Program.Bill_Type = "K";
                flowLayoutPanel1.Controls.Clear();
                DataSet ds = new DataSet();
                ds = ADOC.GetObject.GetDatset("Usp_GetOrder_KOTNO", "tbl_bill");
                DataTable dt1 = ds.Tables[0];
                /// running order table bill
                if (dt1.Rows.Count > 0)
                {
                    DisPlayData(dt1);
                }
                /// Normal order table bill
                DataTable dt2 = ds.Tables[1];
                if (dt2.Rows.Count > 0)
                {
                    DisPlayData(dt2);
                }
                // take away
                DataTable dtTaakeAway = ds.Tables[2];
                if (dtTaakeAway.Rows.Count > 0)
                {
                    Program.Bill_Type = "TA";
                    DisPlayDataTA(dtTaakeAway);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Check Network Connection :" + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Display Table billing and homme delivery data
        /// </summary>
        /// <param name="dt"></param>
        public void DisPlayData(DataTable dt)
        {
            try
            {
                string Table_Covered = string.Empty;
                string NoofRunningtbl = string.Empty;
                //DataTable dt = ADOC.GetObject.GetTable("Usp_GetOrder_KOTNO");
                // DataTable dt = ADOC.GetObject.GetTable("Usp_GetOrder_KOTNO");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string KOT_No = dt.Rows[i]["Kot_No"].ToString();
                    NoofRunningtbl = dt.Rows[i]["NoofRunningtbl"].ToString();
                    string tableNo = dt.Rows[i]["tableNo"].ToString();
                    string BillNo = dt.Rows[i]["BillNo"].ToString();
                    string tbl_Name = dt.Rows[i]["tbl_Name"].ToString();
                    Table_Covered = dt.Rows[i]["Table_Covered"].ToString();
                    dgv = new DataGridView();
                    DataTable dtTran = ADOC.GetObject.GetTable("Usp_GetItemsKot @KOT_No='" + KOT_No + "',@Bill_No='" + BillNo + "'");
                    if (dtTran.Rows.Count > 0)
                    {
                        CreateShoData(flowLayoutPanel1, dgv, btnSupperDetp_Click, dtTran, tableNo, tbl_Name);
                    }
                }
                DIsplayText(NoofRunningtbl, Table_Covered);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Code : 3 " + ex.Message);
                return;
            }
        }


        /// <summary>
        /// display take away order
        /// </summary>
        /// <param name="dt"></param>
        public void DisPlayDataTA(DataTable dt)
        {
            try
            {
                string Table_Covered = string.Empty;
                string NoofRunningtbl = string.Empty;
                //DataTable dt = ADOC.GetObject.GetTable("Usp_GetOrder_KOTNO");
                // DataTable dt = ADOC.GetObject.GetTable("Usp_GetOrder_KOTNO");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string KOT_No = dt.Rows[i]["Kot_No"].ToString();
                    NoofRunningtbl = dt.Rows[i]["NoofRunningtbl"].ToString();
                    string tableNo = dt.Rows[i]["tableNo"].ToString();
                    string BillNo = dt.Rows[i]["BillNo"].ToString();
                    Table_Covered = dt.Rows[i]["Table_Covered"].ToString();
                    string Bill_Type = dt.Rows[i]["Bill_Type"].ToString();
                    if (Bill_Type == "H")
                        Program.Bill_Type = "HD";
                    else
                        Program.Bill_Type = "TA";

                    dgv = new DataGridView();
                    DataTable dtTran = ADOC.GetObject.GetTable("Usp_GetItemsKot_TA @KOT_No='" + KOT_No + "',@Bill_No='" + BillNo + "'");
                    if (dtTran.Rows.Count > 0)
                    {
                        CreateShoData(flowLayoutPanel1, dgv, btnSupperDetp_Click, dtTran, tableNo, "");
                    }
                }
                DIsplayText(NoofRunningtbl, Table_Covered);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Code : 4 " + ex.Message);
            }
        }
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            Program.CurrentKOTNO = string.Empty;
            DisplayOrder();
        }

        private void lblMarquee_DoubleClick(object sender, EventArgs e)
        {
            FullScreenMode.FullScreen objFullscreen = new FullScreenMode.FullScreen(this);
            objFullscreen.ResetTaskBar();
            this.Close();
        }

        private void DoColor(DataGridView obj)
        {
            try
            {
                for (int i = 0; i < obj.Rows.Count; i++)
                {
                    string Order_Type = obj.Rows[i].Cells["Order_Type"].Value.ToString();
                    string i_name = obj.Rows[i].Cells["Item Name"].Value.ToString();
                    if (i_name.Contains(">>"))
                    {
                        obj.Rows[i].Height = 30;
                    }
                    if (Order_Type == "A")
                    {
                        obj.ColumnHeadersDefaultCellStyle.BackColor = Color.Red;
                    }
                    else if (Order_Type == "B")
                    {
                        obj.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
                    }
                    else if (Order_Type == "C")
                    {
                        obj.ColumnHeadersDefaultCellStyle.BackColor = Color.LightBlue;
                    }
                    else
                        obj.ColumnHeadersDefaultCellStyle.BackColor = Color.Yellow;
                    if (i_name.Length > 28)
                    {
                        obj.Rows[i].Height = 30;
                    }
                    obj.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                    string qty = obj.Rows[i].Cells["qty"].Value.ToString();
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    if (qty.Contains("-"))
                    {
                        style.Font = new Font(dgv.Font.OriginalFontName, 8, FontStyle.Strikeout);
                        dgv.Rows[i].DefaultCellStyle.ApplyStyle(style);
                    }
                    else
                    {
                        style.Font = new Font(dgv.Font.OriginalFontName, 8, FontStyle.Regular);
                        dgv.Rows[i].DefaultCellStyle.ApplyStyle(style);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Code : 8 " + ex.Message);
            }
        }

        private void dgvOrderDetail_DataBindingComplete_1(object sender, DataGridViewBindingCompleteEventArgs e)
        {

        }
        void btnSupperDetp_Click(object sender, EventArgs e)
        {
        }

        private void BuilderCodeTExt(RTFBuilderbase sb, string Text)
        {
            sb.ForeColor(KnownColor.Red).AppendLine(Text);
        }

        #region Create supper department and its event
        public void CreateShoData(FlowLayoutPanel objFlowlayoutID, DataGridView dgv, EventHandler btnClickEventClick, DataTable dt, string OrderNo, string tbl_Name)
        {
            try
            {
                //DataGridViewCheckBoxColumn colobj = new DataGridViewCheckBoxColumn();
                dgv.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.objDGV_RowsAdded);
                dgv.EnableHeadersVisualStyles = false;
                dgv.Margin = new Padding(1);
                dgv.Name = "gvItems" + OrderNo;
                DataTable dtItemDetail = GetItem(Convert.ToInt64(OrderNo), dt);
                if (dt.Rows.Count > 0)
                {
                    dtItemDetail.Columns.RemoveAt(2);
                    dtItemDetail.AcceptChanges();
                    AddDataInGrid(dtItemDetail, dgv, OrderNo, tbl_Name);
                    dgv.RowsDefaultCellStyle.SelectionBackColor = Color.White;
                    dgv.RowsDefaultCellStyle.SelectionForeColor = Color.Black;
                    dgv.ClearSelection();
                    dgv.AllowUserToResizeColumns = false;
                    dgv.AllowUserToAddRows = false;
                    // dgv.Columns[0].HeaderText = "Table No";
                    // dgv.Columns[1].HeaderText = OrderNo;
                    dgv.ColumnHeadersHeight = 40;
                    dgv.BackgroundColor = Color.White;
                    dgv.Width = 321;
                    dgv.Height = 190;
                    dgv.RowHeadersVisible = false;
                    dgv.BorderStyle = BorderStyle.FixedSingle;
                    //dgv.BorderStyle = BorderStyle.None;
                    this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                    dgv.AllowUserToOrderColumns = true;
                    dgv.AllowUserToResizeRows = false;
                    dgv.AllowUserToResizeRows = false;
                    // dgv.BackgroundColor = Color.Gainsboro;
                    this.dgv.DefaultCellStyle.WrapMode =
          DataGridViewTriState.True;
                    //dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgv.ScrollBars = ScrollBars.Vertical;
                    foreach (DataGridViewColumn col in dgv.Columns)
                    {
                        //col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.HeaderCell.Style.Font = new Font("Arial", 13F, FontStyle.Bold, GraphicsUnit.Pixel);
                        col.HeaderCell.Style.ForeColor = Color.Black;
                    }
                    DoColor(dgv);
                    dgv.Columns["qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    objFlowlayoutID.Controls.Add(dgv);
                    dgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellClick);
                    dgv.Columns["qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgv.Columns["Item Name"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgv.Columns["Item Name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Code : 5 " + ex.Message);
            }
        }
        #endregion
        public void AddDataInGrid(DataTable dt, DataGridView objDGV, string TableNo, string tbl_Name)
        {
            try
            {
                //create column in grid
                objDGV.Columns.Add("Item Name", "Item Name");
                objDGV.Columns.Add("qty", "qty");
                objDGV.Columns.Add("Comments", "Comments");
                objDGV.Columns.Add("id", "id");
                objDGV.Columns.Add("order_status", "order_status");
                objDGV.Columns.Add("Order_Type", "Order_Type");
                objDGV.Columns.Add("KOT_NO", "KOT_NO");

                objDGV.Columns["Item Name"].Width = 185;
                objDGV.Columns["qty"].Width = 132;
                objDGV.Columns["Comments"].Width = 150;
                string Created_on = string.Empty;
                string Cashier = string.Empty;
                string KOT_NO = string.Empty;
                string Order_Type = string.Empty;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string Comments = dt.Rows[i]["Comments"].ToString();
                    string id = dt.Rows[i]["id"].ToString();
                    string order_status = dt.Rows[i]["order_status"].ToString();
                    Order_Type = dt.Rows[i]["Order_Type"].ToString();
                    Created_on = dt.Rows[i]["createdOn"].ToString();
                    Cashier = dt.Rows[i]["Cashier"].ToString();
                    KOT_NO = dt.Rows[i]["KOT_No"].ToString();
                    Program.KOTNO = KOT_NO;
                    string Comment = string.Empty;
                    ORder_status_var = order_status;
                    if (Comments.Length > 2)
                    {
                        Comment = "\n>>" + Comments;
                    }
                    string i_name = dt.Rows[i]["Item Name"].ToString() + Comment;
                    //if (order_status != "0")
                    objDGV.Rows.Add(i_name, dt.Rows[i]["qty"].ToString(), Comments, id, order_status, Order_Type, KOT_NO);
                }
                objDGV.DefaultCellStyle.BackColor = Color.White;
                //objDGV.BackgroundColor= Color.White;
                objDGV.Columns["Item Name"].ReadOnly = true;
                objDGV.Columns["qty"].ReadOnly = true;
                objDGV.Columns["Comments"].ReadOnly = true;
                objDGV.Columns["id"].Visible = false;
                objDGV.Columns["order_status"].Visible = false;
                objDGV.Columns["Order_Type"].Visible = false;
                objDGV.Columns["Comments"].Visible = false;
                objDGV.Columns["KOT_NO"].Visible = false;
                objDGV.AllowUserToAddRows = false;
                objDGV.AllowUserToDeleteRows = false;
                objDGV.AllowUserToOrderColumns = false;
                objDGV.AllowUserToResizeColumns = false;
                objDGV.AllowUserToOrderColumns = false;
                objDGV.ColumnHeadersHeightSizeMode =
            DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                objDGV.CellBorderStyle = DataGridViewCellBorderStyle.None;
                if (dt.Rows.Count > 0)
                {
                    if (Program.Bill_Type != "TA" && Program.Bill_Type != "HD")
                        dgv.Columns[0].HeaderText = " Table No- " + tbl_Name + "\r  " + "KOT No  - " + KOT_NO;
                    else
                    {
                        if (Program.Bill_Type == "TA")
                            tbl_Name = "Take Away";
                        else
                            tbl_Name = "Home Delivery";
                        //dgv.Columns[0].HeaderText = "KOT No  - " + KOT_NO;
                        dgv.Columns[0].HeaderText = tbl_Name + "\r " + "KOT No  - " + KOT_NO;
                    }
                    dgv.Columns[1].HeaderText = "Time - " + Created_on + "\r User- " + Cashier.ToUpper();
                }
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Code : 7 " + ex.Message);
            }
        }
        private void data_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {

        }
        public DataTable GetItem(Int64 OrderNo, DataTable dtItem)
        {
            DataTable dtobj = null;
            try
            {
                var currentStatRow = (from currentStat in dtItem.AsEnumerable()
                                      where currentStat.Field<Int64>("orderno") == OrderNo
                                      select currentStat);
                if (currentStatRow.Count() > 0)
                {
                    dtobj = new DataTable();
                    dtobj = currentStatRow.CopyToDataTable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Code : 9 " + ex.Message);
            }
            return dtobj;
        }

        private void objDGV_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {

        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //foreach (Control control1 in flowLayoutPanel1.Controls)
                //{
                //    MessageBox.Show(control1.Name);
                //    //DataGridView gv = control1.Name;
                //}   
                ItemDetail obj = new ItemDetail();
                ORder_status_var = "0";
                DataGridView OBJataGridView = (DataGridView)sender;
                if (OBJataGridView.Rows.Count > 0)
                {

                    int RowIndex = OBJataGridView.CurrentCell.RowIndex;
                    int ColIndex = OBJataGridView.CurrentCell.ColumnIndex;
                    clsTempData.SetDataGrid(OBJataGridView);
                    if (OBJataGridView.Rows.Count > 0)
                    {
                        obj.ShowDialog();
                        DisplayOrder();
                    }
                    else
                    {

                        //OBJataGridView.BackgroundColor = Color.LightSteelBlue;
                        OBJataGridView.DefaultCellStyle.BackColor = Color.LightSteelBlue;
                        // OBJataGridView.SelectAll();
                        OBJataGridView.ClearSelection();
                    }
                    return;
                    string values = OBJataGridView.Rows[RowIndex].Cells[""].Value.ToString();
                    string OrderNo = OBJataGridView.Columns["KOT"].HeaderText;
                }
                else
                {
                    string Header = OBJataGridView.Columns[0].HeaderText;
                    string[] KOTNo = Header.Split('-');
                    if (KOTNo[2].Length > 0)
                    {
                        bool IsTrue = ADOC.GetObject.ExecuteDML("dbo.Usp_UpdatestatusByKotNo @KOT_No='" + KOTNo[2].ToString() + "'");
                        if (IsTrue == true)
                        {
                            DisplayOrder();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Code : 10 " + ex.Message);
            }
        }
        private void UpdateKOtStstus(DataGridView obhDgv, string Ststus)
        {
            try
            {
                string sql = string.Empty;
                for (int i = 0; i < obhDgv.Rows.Count; i++)
                {
                    bool IsCheck = (bool)obhDgv.Rows[i].Cells[0].FormattedValue;
                    if (IsCheck == true)
                    {
                        string id = obhDgv.Rows[i].Cells["id"].Value.ToString();
                        if (id.Length > 0)
                            sql += "EXEC Usp_UpdateStstus @id='" + id + "',@status='" + Ststus + "'";
                    }
                }
                if (sql.Length > 5)
                {
                    ADOC.GetObject.ExecuteDML(sql);
                    sql = string.Empty;
                    MessageBox.Show("Operation Successfully Completed", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("This order already accepted", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                DisplayOrder();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Code : 11 " + ex.Message);
            }
        }
        private void ChechAll(DataGridView Objdgv, bool IsCheck)
        {
            for (int i = 0; i < Objdgv.Rows.Count; i++)
            {
                if (IsCheck == true)
                    Objdgv.Rows[i].Cells[0].Value = true;
                else
                    Objdgv.Rows[i].Cells[0].Value = false;
            }
        }

        private void btnCLose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblMarquee_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResizeForm();
        }
        private void ResizeForm()
        {
            // Enable auto-scrolling for the form. 
            this.AutoScroll = true;

            // Resize the form.
            Rectangle r = this.ClientRectangle;
            // Subtract 100 pixels from each side of the Rectangle.
            r.Inflate(-100, -100);
            this.Bounds = this.RectangleToScreen(r);

            // Make sure button2 is visible. 
            this.ScrollControlIntoView(button2);
        }
        private void btnDelivered_Click(object sender, EventArgs e)
        {
            DataGridView objDGV = new DataGridView();
            objDGV = clsTempData.GetDataGrid();
            if (objDGV.Rows.Count > 0)
            {

                try
                {
                    // UpdateKOtStstus(objDGV, "2");

                    // this.Close();
                }
                catch (Exception ex)
                {

                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ResizeForm();
        }
    }
}