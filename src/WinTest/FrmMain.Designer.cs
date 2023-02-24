namespace WinTest
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbShfe2 = new System.Windows.Forms.Label();
            this.lbDce2 = new System.Windows.Forms.Label();
            this.lbCzce2 = new System.Windows.Forms.Label();
            this.lbShfe = new System.Windows.Forms.Label();
            this.lbDce = new System.Windows.Forms.Label();
            this.lbCzce = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbShfe2
            // 
            this.lbShfe2.AutoSize = true;
            this.lbShfe2.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbShfe2.Location = new System.Drawing.Point(66, 103);
            this.lbShfe2.Name = "lbShfe2";
            this.lbShfe2.Size = new System.Drawing.Size(172, 27);
            this.lbShfe2.TabIndex = 0;
            this.lbShfe2.Text = "上海期货交易所：";
            // 
            // lbDce2
            // 
            this.lbDce2.AutoSize = true;
            this.lbDce2.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbDce2.Location = new System.Drawing.Point(66, 154);
            this.lbDce2.Name = "lbDce2";
            this.lbDce2.Size = new System.Drawing.Size(172, 27);
            this.lbDce2.TabIndex = 0;
            this.lbDce2.Text = "大连商品交易所：";
            // 
            // lbCzce2
            // 
            this.lbCzce2.AutoSize = true;
            this.lbCzce2.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbCzce2.Location = new System.Drawing.Point(66, 52);
            this.lbCzce2.Name = "lbCzce2";
            this.lbCzce2.Size = new System.Drawing.Size(172, 27);
            this.lbCzce2.TabIndex = 0;
            this.lbCzce2.Text = "郑州商品交易所：";
            // 
            // lbShfe
            // 
            this.lbShfe.AutoSize = true;
            this.lbShfe.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbShfe.ForeColor = System.Drawing.Color.Blue;
            this.lbShfe.Location = new System.Drawing.Point(237, 103);
            this.lbShfe.Name = "lbShfe";
            this.lbShfe.Size = new System.Drawing.Size(30, 27);
            this.lbShfe.TabIndex = 0;
            this.lbShfe.Text = "--";
            // 
            // lbDce
            // 
            this.lbDce.AutoSize = true;
            this.lbDce.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbDce.ForeColor = System.Drawing.Color.Blue;
            this.lbDce.Location = new System.Drawing.Point(237, 154);
            this.lbDce.Name = "lbDce";
            this.lbDce.Size = new System.Drawing.Size(30, 27);
            this.lbDce.TabIndex = 0;
            this.lbDce.Text = "--";
            // 
            // lbCzce
            // 
            this.lbCzce.AutoSize = true;
            this.lbCzce.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbCzce.ForeColor = System.Drawing.Color.Blue;
            this.lbCzce.Location = new System.Drawing.Point(237, 52);
            this.lbCzce.Name = "lbCzce";
            this.lbCzce.Size = new System.Drawing.Size(30, 27);
            this.lbCzce.TabIndex = 0;
            this.lbCzce.Text = "--";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 250);
            this.Controls.Add(this.lbCzce);
            this.Controls.Add(this.lbCzce2);
            this.Controls.Add(this.lbDce);
            this.Controls.Add(this.lbDce2);
            this.Controls.Add(this.lbShfe);
            this.Controls.Add(this.lbShfe2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "期货延时市场价服务";
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lbShfe2;
        private Label lbDce2;
        private Label lbCzce2;
        private Label lbShfe;
        private Label lbDce;
        private Label lbCzce;
    }
}