using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TuchScreenApp1Jan2013;
namespace TuchScreenApp1Jan2013.UI
{
    public partial class FinishOrderDisplay : Form
    {
        public FinishOrderDisplay()
        {
            InitializeComponent();
        }
        byte[] Dept_Image = new byte[0];
        private void OrderDisplayItem_Load(object sender, EventArgs e)
        {
            try
            {
                Order_Display_System.Program.Bill_Date = ADOC.GetObject.GetSingleResult("select top 1 convert(varchar(10),bill_date,120) from tbl_day_end where isdayend=0 order by id desc");
                FullScreenMode.FullScreen objFullscreen = new FullScreenMode.FullScreen(this);
                objFullscreen.ShowFullScreen();
                timer1.Start();
                CreateSupperDepartment();
                //System.Media.SystemSounds.Beep.Play();
            }
            catch (Exception ex)
            {

            }
        }


        #region Create supper department and its event
        public void CreateSupperDepartmentButtons(FlowLayoutPanel objFlowlayoutID, Button objbtn, string ButtonText, byte[] imageURL, EventHandler btnClickEventClick_Supper_dept, int width, int height, int fontsize)
        {
            objbtn.BackColor = System.Drawing.Color.Gray;
            objbtn.ForeColor = System.Drawing.Color.Red;
            objbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            objbtn.Font = new System.Drawing.Font("Arial Narrow", fontsize, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            if (imageURL.Length > 0)
            {
                System.IO.MemoryStream objms = new System.IO.MemoryStream(imageURL);
                objbtn.Image = Image.FromStream(objms);

            }
            objbtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            objbtn.Location = new System.Drawing.Point(3, 3);
            objbtn.Name = "btnSupperDept";
            objbtn.Margin = new Padding(1);
            objbtn.Size = new System.Drawing.Size(width, height);
            objbtn.TabIndex = 33;
            objbtn.Text = ButtonText;
            objbtn.TextAlign = ContentAlignment.MiddleCenter;
            objbtn.Text.PadLeft(1);
            objbtn.UseCompatibleTextRendering = true;
            objbtn.UseVisualStyleBackColor = false;
            objFlowlayoutID.Controls.Add(objbtn);
            objbtn.Click += new System.EventHandler(btnClickEventClick_Supper_dept);
        }
        #endregion
        int countRow = 0;
        public void CreateSupperDepartment()
        {
            DataTable dtSupperDepartment = null;
            DataTable dtOrder_No = new DataTable();
            Dept_Image = new byte[0];
            dtSupperDepartment = ADOC.GetObject.GetTable("Usp_getFinishOrder");
           
            flowLayoutPanel1.Controls.Clear();
            for (int i = 0; i < dtSupperDepartment.Rows.Count; i++)
            {
                string current_order = dtSupperDepartment.Rows[i]["current_order"].ToString();
                string orderno = dtSupperDepartment.Rows[i]["order no"].ToString();
                if (current_order == "1")
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                    player.SoundLocation = "alarm1.wav";
                    player.Play();
                    ADOC.GetObject.ExecuteDML("update tbl_bill set current_order=0 where orderno='" + orderno + "' and convert(varchar(10),bill_date,120)='" + Order_Display_System.Program.Bill_Date + "'");
                }                
                if (dtSupperDepartment.Rows.Count > countRow)
                {
                    //mp3
                    //System.Diagnostics.Process.Start("alarm.mp3");
                    countRow = dtSupperDepartment.Rows.Count;
                }
                else
                {
                    countRow = dtSupperDepartment.Rows.Count;
                }

                Button btnDepartment = new Button();
                Dept_Image = new byte[0];
                //if (dtSupperDepartment.Rows.Count <= 12)
                //{
                //    CreateSupperDepartmentButtons(flowLayoutPanel1, btnDepartment, dtSupperDepartment.Rows[i]["Order No"].ToString(), Dept_Image, btnSupperDetp_Click, 310, 240, 120);
                //}
                //else if (dtSupperDepartment.Rows.Count >= 13 && dtSupperDepartment.Rows.Count <= 20)
               if (dtSupperDepartment.Rows.Count <= 20)
                {
                    CreateSupperDepartmentButtons(flowLayoutPanel1, btnDepartment, dtSupperDepartment.Rows[i]["Order No"].ToString(), Dept_Image, btnSupperDetp_Click, 250, 140, 100);
                    
                }

                //else if (dtSupperDepartment.Rows.Count >= 21 && dtSupperDepartment.Rows.Count <= 28)
                //{
                //    CreateSupperDepartmentButtons(flowLayoutPanel1, btnDepartment, dtSupperDepartment.Rows[i]["Order No"].ToString(), Dept_Image, btnSupperDetp_Click, 200, 120, 60);
                //}
            }
        }
        void btnSupperDetp_Click(object sender, EventArgs e)
        {
        }

        private int xPos = 0;
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            CreateSupperDepartment();
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            FullScreenMode.FullScreen objFullscreen = new FullScreenMode.FullScreen(this);
            objFullscreen.ResetTaskBar();
            this.Close();
        }

        private void lblMarquee_DoubleClick(object sender, EventArgs e)
        {
            FullScreenMode.FullScreen objFullscreen = new FullScreenMode.FullScreen(this);
            objFullscreen.ResetTaskBar();
            this.Close();
        }



    }
}
