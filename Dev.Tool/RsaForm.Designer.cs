namespace Dev.Tool
{
    partial class RsaForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGenerate = new System.Windows.Forms.Button();
            this.gboxPrivate = new System.Windows.Forms.GroupBox();
            this.txtPrivate = new System.Windows.Forms.TextBox();
            this.gboxPublic = new System.Windows.Forms.GroupBox();
            this.txtPublic = new System.Windows.Forms.TextBox();
            this.cboxKeyStrength = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboxKeyFmt = new System.Windows.Forms.ComboBox();
            this.gboxPrivate.SuspendLayout();
            this.gboxPublic.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(357, 12);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 0;
            this.btnGenerate.Text = "生成";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // gboxPrivate
            // 
            this.gboxPrivate.Controls.Add(this.txtPrivate);
            this.gboxPrivate.Location = new System.Drawing.Point(12, 41);
            this.gboxPrivate.Name = "gboxPrivate";
            this.gboxPrivate.Size = new System.Drawing.Size(423, 138);
            this.gboxPrivate.TabIndex = 1;
            this.gboxPrivate.TabStop = false;
            this.gboxPrivate.Text = "私钥";
            // 
            // txtPrivate
            // 
            this.txtPrivate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPrivate.Location = new System.Drawing.Point(3, 17);
            this.txtPrivate.Multiline = true;
            this.txtPrivate.Name = "txtPrivate";
            this.txtPrivate.ReadOnly = true;
            this.txtPrivate.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPrivate.Size = new System.Drawing.Size(417, 118);
            this.txtPrivate.TabIndex = 0;
            // 
            // gboxPublic
            // 
            this.gboxPublic.Controls.Add(this.txtPublic);
            this.gboxPublic.Location = new System.Drawing.Point(12, 185);
            this.gboxPublic.Name = "gboxPublic";
            this.gboxPublic.Size = new System.Drawing.Size(423, 111);
            this.gboxPublic.TabIndex = 2;
            this.gboxPublic.TabStop = false;
            this.gboxPublic.Text = "公钥";
            // 
            // txtPublic
            // 
            this.txtPublic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPublic.Location = new System.Drawing.Point(3, 17);
            this.txtPublic.Multiline = true;
            this.txtPublic.Name = "txtPublic";
            this.txtPublic.ReadOnly = true;
            this.txtPublic.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPublic.Size = new System.Drawing.Size(417, 91);
            this.txtPublic.TabIndex = 0;
            // 
            // cboxKeyStrength
            // 
            this.cboxKeyStrength.FormattingEnabled = true;
            this.cboxKeyStrength.Items.AddRange(new object[] {
            "256",
            "384",
            "512",
            "1024",
            "2048",
            "4096"});
            this.cboxKeyStrength.Location = new System.Drawing.Point(72, 14);
            this.cboxKeyStrength.Name = "cboxKeyStrength";
            this.cboxKeyStrength.Size = new System.Drawing.Size(50, 20);
            this.cboxKeyStrength.TabIndex = 3;
            this.cboxKeyStrength.Text = "256";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "密钥位数";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(128, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "密钥格式";
            // 
            // cboxKeyFmt
            // 
            this.cboxKeyFmt.FormattingEnabled = true;
            this.cboxKeyFmt.Items.AddRange(new object[] {
            "pkcs#1",
            "pkcs#8",
            "xml",
            "json"});
            this.cboxKeyFmt.Location = new System.Drawing.Point(187, 14);
            this.cboxKeyFmt.Name = "cboxKeyFmt";
            this.cboxKeyFmt.Size = new System.Drawing.Size(62, 20);
            this.cboxKeyFmt.TabIndex = 6;
            this.cboxKeyFmt.Text = "pkcs#1";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 311);
            this.Controls.Add(this.cboxKeyFmt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboxKeyStrength);
            this.Controls.Add(this.gboxPublic);
            this.Controls.Add(this.gboxPrivate);
            this.Controls.Add(this.btnGenerate);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.Text = "RSA工具";
            this.gboxPrivate.ResumeLayout(false);
            this.gboxPrivate.PerformLayout();
            this.gboxPublic.ResumeLayout(false);
            this.gboxPublic.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.GroupBox gboxPrivate;
        private System.Windows.Forms.GroupBox gboxPublic;
        private System.Windows.Forms.TextBox txtPrivate;
        private System.Windows.Forms.TextBox txtPublic;
        private System.Windows.Forms.ComboBox cboxKeyStrength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboxKeyFmt;
    }
}

