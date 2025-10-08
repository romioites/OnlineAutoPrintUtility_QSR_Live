namespace Order_Display_System
{
    partial class Form1
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
            this.btnManageOrder = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOrderItemDetail = new System.Windows.Forms.Button();
            this.btnOrderDetail = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnManageOrder
            // 
            this.btnManageOrder.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnManageOrder.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnManageOrder.BackgroundImage = global::Order_Display_System.Properties.Resources.Settings123;
            this.btnManageOrder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnManageOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManageOrder.Font = new System.Drawing.Font("Arial Black", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManageOrder.ForeColor = System.Drawing.Color.Black;
            this.btnManageOrder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnManageOrder.Location = new System.Drawing.Point(615, 150);
            this.btnManageOrder.Name = "btnManageOrder";
            this.btnManageOrder.Size = new System.Drawing.Size(272, 103);
            this.btnManageOrder.TabIndex = 252;
            this.btnManageOrder.Text = "Modify Pending Order";
            this.btnManageOrder.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnManageOrder.UseVisualStyleBackColor = false;
            this.btnManageOrder.Visible = false;
            this.btnManageOrder.Click += new System.EventHandler(this.btnManageOrder_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Arial Narrow", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.Location = new System.Drawing.Point(750, 377);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(156, 71);
            this.btnClose.TabIndex = 253;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.btnOrderItemDetail);
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Controls.Add(this.btnOrderDetail);
            this.groupBox1.Controls.Add(this.btnManageOrder);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)), true);
            this.groupBox1.Location = new System.Drawing.Point(48, 86);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(912, 454);
            this.groupBox1.TabIndex = 254;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ROMIO DISPLAY ORDER SYSTEM";
            // 
            // btnOrderItemDetail
            // 
            this.btnOrderItemDetail.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnOrderItemDetail.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnOrderItemDetail.BackgroundImage = global::Order_Display_System.Properties.Resources.kitchen;
            this.btnOrderItemDetail.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOrderItemDetail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOrderItemDetail.Font = new System.Drawing.Font("Arial Black", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOrderItemDetail.ForeColor = System.Drawing.Color.Transparent;
            this.btnOrderItemDetail.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOrderItemDetail.Location = new System.Drawing.Point(25, 150);
            this.btnOrderItemDetail.Name = "btnOrderItemDetail";
            this.btnOrderItemDetail.Size = new System.Drawing.Size(272, 103);
            this.btnOrderItemDetail.TabIndex = 250;
            this.btnOrderItemDetail.Text = "Kitchen Display";
            this.btnOrderItemDetail.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOrderItemDetail.UseVisualStyleBackColor = false;
            this.btnOrderItemDetail.Click += new System.EventHandler(this.btnOrderItemDetail_Click);
            // 
            // btnOrderDetail
            // 
            this.btnOrderDetail.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnOrderDetail.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnOrderDetail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOrderDetail.Font = new System.Drawing.Font("Arial Black", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOrderDetail.ForeColor = System.Drawing.Color.Black;
            this.btnOrderDetail.Image = global::Order_Display_System.Properties.Resources.barCodei;
            this.btnOrderDetail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOrderDetail.Location = new System.Drawing.Point(319, 150);
            this.btnOrderDetail.Name = "btnOrderDetail";
            this.btnOrderDetail.Size = new System.Drawing.Size(272, 103);
            this.btnOrderDetail.TabIndex = 251;
            this.btnOrderDetail.Text = "Customer Display";
            this.btnOrderDetail.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOrderDetail.UseVisualStyleBackColor = false;
            this.btnOrderDetail.Visible = false;
            this.btnOrderDetail.Click += new System.EventHandler(this.btnOrderDetail_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Image = global::Order_Display_System.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(656, 616);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(307, 79);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 256;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Order Display System";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOrderItemDetail;
        private System.Windows.Forms.Button btnOrderDetail;
        private System.Windows.Forms.Button btnManageOrder;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

