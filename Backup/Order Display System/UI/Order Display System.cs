using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using TuchScreenApp1Jan2013.UI;
using Order_Display_System.App_Code;

namespace Order_Display_System
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //FullScreenMode.FullScreen objFullscreen = new FullScreenMode.FullScreen(this);
            //  objFullscreen.ShowFullScreen();
            try
            {
                clsConfigSettings objnConfig = new clsConfigSettings();
                objnConfig.GetLoginData();
            }
            catch { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //FullScreenMode.FullScreen objFullscreen = new FullScreenMode.FullScreen(this);
            // objFullscreen.ResetTaskBar();
        }

        private void btnOrderItemDetail_Click(object sender, EventArgs e)
        {
            OrderDisplayItem frmObj = new OrderDisplayItem();
            frmObj.Show();
        }

        private void btnOrderDetail_Click(object sender, EventArgs e)
        {
            FinishOrderDisplay frmObj = new FinishOrderDisplay();
            frmObj.Show();
        }

        private void btnManageOrder_Click(object sender, EventArgs e)
        {
            ModifyOrderStstus frmObj = new ModifyOrderStstus();
            frmObj.Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            FullScreenMode.FullScreen objFullscreen = new FullScreenMode.FullScreen(this);
            objFullscreen.ResetTaskBar();
            Application.Exit();
        }

    }
}
