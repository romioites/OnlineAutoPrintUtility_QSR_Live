namespace TuchScreenApp1Jan2013.UI
{
    partial class ReportViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cRViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // cRViewer
            // 
            this.cRViewer.ActiveViewIndex = -1;
            this.cRViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cRViewer.DisplayGroupTree = false;
            this.cRViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cRViewer.Location = new System.Drawing.Point(0, 0);
            this.cRViewer.Name = "cRViewer";
            this.cRViewer.SelectionFormula = "";
            this.cRViewer.Size = new System.Drawing.Size(837, 464);
            this.cRViewer.TabIndex = 0;
            this.cRViewer.ViewTimeSelectionFormula = "";
            // 
            // ReportViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 464);
            this.Controls.Add(this.cRViewer);
            this.Name = "ReportViewer";
            this.Text = "ReportViewer";
            this.Load += new System.EventHandler(this.ReportViewer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public CrystalDecisions.Windows.Forms.CrystalReportViewer cRViewer;

    }
}