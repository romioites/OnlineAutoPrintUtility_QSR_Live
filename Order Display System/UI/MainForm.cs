using KOTPrintUtility.App_Code;
using LPrinterTest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KOTPrintUtility.UI
{
    public partial class Data_Center : Form
    {
        int TimerIsStopedForLastSixtySecod = 0;
        string Localtime = string.Empty;
        System.Windows.Forms.Timer myTimer;
        QSRApp objclsBill = null;
        public Data_Center()
        {
            InitializeComponent();
        }
        // static DataGridView dgvItemDetails = new DataGridView();

        private void Data_Center_Load(object sender, EventArgs e)
        {
            try
            {
                objclsBill = new QSRApp();
                #region create column
                if (clsConfigSettings.Log_Drive == "")
                    clsConfigSettings.Log_Drive = "C";
                lblNoofOrder.Text = "No of order=> 0";
                lblTimer.Text = Program.DayEnd_BIllingDate + " " + DateTime.Now.ToString("hh:mm:ss tt");
                //===================================================================================
                dgvItemDetails = new DataGridView();
                dgvItemDetails.Columns.Add("ItemName", "Item Name");
                dgvItemDetails.Columns.Add("Rate", "Rate");
                dgvItemDetails.Columns.Add("QTY", "Qty");
                dgvItemDetails.Columns.Add("Total", "Total");
                dgvItemDetails.Columns.Add("Tax", "Tax");
                dgvItemDetails.Columns.Add("DishCode", "DishCode");
                dgvItemDetails.Columns.Add("DishComment", "DishComment");
                dgvItemDetails.Columns.Add("IsNewKOT", "IsNewKOT");
                dgvItemDetails.Columns.Add("ItemAddonCode", "ItemAddonCode");
                dgvItemDetails.Columns.Add("TaxRate", "TaxRate");
                dgvItemDetails.Columns.Add("isTaxable", "isTaxable");
                dgvItemDetails.Columns.Add("Discount", "IsDiscount");
                dgvItemDetails.Columns.Add("ItemIndex", "ItemIndex");
                dgvItemDetails.Columns.Add("dept", "dept");
                dgvItemDetails.Columns.Add("Dis_Rate", "Dis_Rate");
                dgvItemDetails.Columns.Add("Service_Tax", "Service_Tax");
                dgvItemDetails.Columns.Add("Offer_Status", "Offer_Status");
                //----------------This two line add in case of use Line Discount-------------//Sujit
                dgvItemDetails.Columns.Add("Pct", "Pct");
                dgvItemDetails.Columns.Add("Rs", "Rs");
                dgvItemDetails.Columns.Add("IsCouponDeal", "IsCouponDeal");
                dgvItemDetails.Columns.Add("UrgentItem", "UrgentItem");
                dgvItemDetails.Columns.Add("addon_index", "addon_index");
                dgvItemDetails.Columns.Add("addon_index_fk", "addon_index_fk");
                dgvItemDetails.Columns.Add("IdentityColumn", "IdentityColumn");
                dgvItemDetails.Columns.Add("Cat", "Cat");
                dgvItemDetails.AutoGenerateColumns = false;
                dgvItemDetails.AllowUserToAddRows = false;
                //=============================================================================================
                myTimer = new System.Windows.Forms.Timer();
                myTimer.Interval = 1000;
                myTimer.Enabled = true;
                myTimer.Start();
                myTimer.Tick += MyTimer_Tick;
                btnStatus.BackColor = Color.Green;
                //================================================================================================
                Program.InstalledVersion = FileVersionInfo.GetVersionInfo("OrderPrintUtility.exe").FileVersion;
                this.Text = "Online Order Print Utility VS-" + Program.InstalledVersion;
                #endregion


                picUpdateAvailble.Visible = false;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                tmrStart.Start();
            }
        }


        private async void TmrKOTPrint_Tick(object sender, EventArgs e)
        {
            try
            {
                TimerIsStopedForLastSixtySecod = 0;
                TmrKOTPrint.Stop();
                if (objclsBill.CheckForInternet() == true)
                {
                    if (btnStatus.BackColor == Color.Red)
                        btnStatus.BackColor = Color.Green;

                    if (!objclsBill._TransactionIsonProgress)
                    {
                        objclsBill.SetLableText("Processing...", lblNoofOrder);
						await Task.Run(() => objclsBill.GetOrderAPI(lblNoofOrder));
					}
                }
                else
                {
                    btnStatus.BackColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                btnStatus.BackColor = Color.Red;
                Loging.Log(LogType.Error, "TmrKOTPrint_Tick error " + ex.Message);
            }
            finally
            {
                TmrKOTPrint.Start();
            }
        }

        private void tmrStart_Tick(object sender, EventArgs e)
        {
            try
            {
                objclsBill._TransactionIsonProgress = true;
                cls_ConfigurationMaster objcls = new cls_ConfigurationMaster();
                bool Result = objcls.GetConfigurationDetail();
                Result = clsUpdateOrder.GetRequestdataAll();

                if (Result == true)
                {
                    objclsBill._TransactionIsonProgress = false;
                    clsUpdateOrderStatusWebsite objclsStatus = new clsUpdateOrderStatusWebsite();
                    LiveSaleUpdate objSale = new LiveSaleUpdate();
                    lblTimer.Text = Program.DayEnd_BIllingDate + " " + DateTime.Now.ToString("hh:mm:ss tt");
                    Task.Run(() => objclsStatus.StatrtUploadStatus());
					Task.Run(() => objSale.StatrtUploadSale());

					Loging.Log(LogType.Information, "Application Started");
                    picUpdateAvailble.Visible = false;
                    if (Program.InstalledVersion != Program.AvailableVersion && Program.AvailableVersion != "")
                    {
                        picUpdateAvailble.Visible = true;
                    }
                }
                else
                {
                    btnStatus.BackColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                Loging.Log(LogType.Error, "tmrStart_Tick " + ex.Message);
            }
            finally
            {
                tmrStart.Stop();
                TmrKOTPrint.Start();
            }
        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {
            try
            {

                ShowTime();
                if (TimerIsStopedForLastSixtySecod >= 90)
                {
                    objclsBill._TransactionIsonProgress = false;
                    TimerIsStopedForLastSixtySecod = 0;
                    Loging.Log(LogType.Error, "Main Timer is stoped  , try to reboot");
                    ReBootTimer();
                }
            }
            catch (Exception ex)
            {
                Loging.Log(LogType.Error, "MyTimer_Tick() error " + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            tmrStart.Start();
        }
        private void ShowTime()
        {
            try
            {
                Localtime = Program.DayEnd_BIllingDate + " " + DateTime.Now.ToString("hh:mm:ss tt");
                objclsBill.SetLableText(Localtime, lblTimer);
            }
            catch (Exception ex)
            {
                Loging.Log(LogType.Error, "ShowTime() error " + ex.Message);
            }
        }
        private void ReBootTimer()
        {
            try
            {
                try
                {
                    TmrKOTPrint.Stop();
                }
                catch (Exception ex)
                {
                    Loging.Log(LogType.Error, "TmrKOTPrint.Stop() error " + ex.Message);
                }
                TmrKOTPrint.Start();
            }
            catch (Exception ex)
            {
                Loging.Log(LogType.Error, "ReBootTimer() error " + ex.Message);
            }
        }

        private void picUpdateAvailble_Click(object sender, EventArgs e)
        {
            ADOC objADOC = new ADOC();
            try
            {
                string strUpdate = "New Update Available in Utility system. \r........................................................................................................\rTo accept this update you need to click on Yes button, Do you want to update now?\n\n\n		Old Version  : " + Program.InstalledVersion + "\r\r		New Version : " + Program.AvailableVersion + "\n........................................................................................................";
                if (MessageBox.Show(strUpdate, "xxxxxxxxxx Version Conflict xxxxxxxxxx", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string currentProcessName = Process.GetCurrentProcess().ProcessName;
                    int currentProcessId = Process.GetCurrentProcess().Id;

                    // Find other running instances (excluding this one)
                    var otherInstances = Process.GetProcessesByName("SoftwareUpdater")
                                                .Where(p => p.Id != currentProcessId);

                    if (otherInstances.Count() == 0)
                    {
                        string path = Application.StartupPath + "\\SoftwareUpdater.exe";
                        string CMDarguments = "OrderPrintUtility.exe";

                        ProcessStartInfo startInfo = new ProcessStartInfo()
                        {
                            FileName = path,
                            Arguments = CMDarguments,
                            UseShellExecute = false // optional: use true if you want to open non-console apps with shell
                        };
                        System.Diagnostics.Process.Start(startInfo);
                        //string localadd = @"C:\Romio\";
                        objADOC.GetObject.ExecuteDML("exec Usp_UpadteFileVersion @local_POS_Upgrade_address='" + Application.StartupPath + "',@POS_Type='utility'");
                        //clsBLL.KillApplicationProcess();
                        Application.Exit();
                    }
                    else
                    {
                        MessageBox.Show("Update is already on progress, please close and start POS software again.");
                        //clsBLL.KillApplicationProcess();
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
