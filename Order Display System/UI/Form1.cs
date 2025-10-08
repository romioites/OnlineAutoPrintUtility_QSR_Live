using KOTPrintUtility.App_Code;
using LPrinterTest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KOTPrintUtility.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            PrinterSettings settings = new PrinterSettings();
            string PaperFullCut = Convert.ToString((char)27) + Convert.ToString((char)105);
            string PaperPartialCut = Convert.ToString((char)27) + Convert.ToString((char)109);
            LPrinter PosPrinter = new LPrinter();
            string CurrentPrinter = textBox1.Text.Trim();
            string Header = clsConfigSettings.header;
            string LineonKOT =""; ;


            bool IsOpen = PosPrinter.Open("testDoc", CurrentPrinter);

            if (IsOpen == true)
                MessageBox.Show("Printer Is Connected");
            else
                MessageBox.Show("Printer Not Connected");

            PosPrinter.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool Ispinging=clsDLL.PingPrinter(textBox1.Text);
            if (Ispinging == true)
                MessageBox.Show("Printer Is Pinging");
            else
                MessageBox.Show("Printer is not Pinging");
        }
    }
}
