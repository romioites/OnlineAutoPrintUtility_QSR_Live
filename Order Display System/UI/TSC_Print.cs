using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using TuchScreenApp1Jan2013.App_Code;
using System.Drawing.Printing;
using LPrinterTest;
using KOTPrintUtility.App_Code;
using KOTPrintUtility;

namespace TuchScreenApp1Jan2013.UI
{
    public partial class TSC_Print : Form
    {
        ADOC objADOC = new ADOC();
        [DllImport("TSCLIB.dll", EntryPoint = "about")]
        public static extern bool about();

        [DllImport("TSCLIB.dll", EntryPoint = "openport")]
        public static extern bool openport(string printer);

        [DllImport("TSCLIB.dll", EntryPoint = "barcode")]
        public static extern int barcode(string x, string y, string type,
        string height, string readable, string rotation,
        string narrow, string wide, string code);

        [DllImport("TSCLIB.dll", EntryPoint = "clearbuffer")]
        public static extern int clearbuffer();

        [DllImport("TSCLIB.dll", EntryPoint = "closeport")]
        public static extern int closeport();

        [DllImport("TSCLIB.dll", EntryPoint = "downloadpcx")]
        public static extern int downloadpcx(string filename, string image_name);

        [DllImport("TSCLIB.dll", EntryPoint = "formfeed")]
        public static extern int formfeed();

        [DllImport("TSCLIB.dll", EntryPoint = "nobackfeed")]
        public static extern int nobackfeed();

        [DllImport("TSCLIB.dll", EntryPoint = "printerfont")]
        public static extern int printerfont(string x, string y, string fonttype,
        string rotation, string xmul, string ymul,
        string text);

        [DllImport("TSCLIB.dll", EntryPoint = "printlabel")]
        public static extern int printlabel(string set, string copy);

        [DllImport("TSCLIB.dll", EntryPoint = "sendcommand")]
        public static extern int sendcommand(string printercommand);

        [DllImport("TSCLIB.dll", EntryPoint = "setup")]
        public static extern int setup(string width, string height,
        string speed, string density,
        string sensor, string vertical,
        String offset);

        [DllImport("TSCLIB.dll", EntryPoint = "windowsfont")]
        public static extern int windowsfont(int x, int y, int fontheight,
        int rotation, int fontstyle, int fontunderline,
        string szFaceName, string content);

        public TSC_Print()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                openport("TSC TTP-244 Pro");
                setup("50.8", "42.5", "4", "8", "0", "0", "0"); // Setup the media size and sensor type info
                clearbuffer(); // Clear image buffer
                //barcode("100", "100", "128", "100", "1", "0", "2", "2", "Barcode Test"); // Drawing barcode
                //printerfont("4", "2.5", "3", "0", "1", "1", "Drawing printer font"); // Drawing printer font
                //printerfont("4", "3.5", "3", "0", "1", "1", "Drawing printer font"); // Drawing printer font
                windowsfont(35, 250, 24, 0, 0, 0, "ARIAL", "Windows Arial Font Test"); // Draw windows font
                windowsfont(35, 275, 24, 0, 0, 0, "ARIAL", "Hello world"); // Draw windows font
                windowsfont(35, 300, 24, 0, 0, 0, "ARIAL", "Test"); // Draw windows font
                //downloadpcx ("UL.PCX", "UL.PCX"); // Download PCX file into printer
                //sendcommand ("PUTPCX 100,400, /" UL.PCX / ""); // Drawing PCX graphic
                printlabel("1", "1"); // Print labels
                closeport(); // Close specified printer driver
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

        private void TSC_Print_Load(object sender, EventArgs e)
        {
            try
            {
                //DLL.GetPrinter(cmbPrinter);
            }
            catch { }
        }
    }
}
