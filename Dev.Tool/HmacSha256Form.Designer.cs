namespace Dev.Tool
{
    partial class HmacSha256Form
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
            this.txtInput = new System.Windows.Forms.TextBox();
            this.gboxPublic = new System.Windows.Forms.GroupBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.gboxPrivate = new System.Windows.Forms.GroupBox();
            this.btnCompute = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboxFormat = new System.Windows.Forms.ComboBox();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkReplaceEnter = new System.Windows.Forms.CheckBox();
            this.gboxPublic.SuspendLayout();
            this.gboxPrivate.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Location = new System.Drawing.Point(3, 17);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInput.Size = new System.Drawing.Size(541, 134);
            this.txtInput.TabIndex = 0;
            // 
            // gboxPublic
            // 
            this.gboxPublic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gboxPublic.Controls.Add(this.txtOutput);
            this.gboxPublic.Location = new System.Drawing.Point(19, 232);
            this.gboxPublic.Name = "gboxPublic";
            this.gboxPublic.Size = new System.Drawing.Size(547, 154);
            this.gboxPublic.TabIndex = 15;
            this.gboxPublic.TabStop = false;
            this.gboxPublic.Text = "输出";
            // 
            // txtOutput
            // 
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Location = new System.Drawing.Point(3, 17);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(541, 134);
            this.txtOutput.TabIndex = 0;
            // 
            // gboxPrivate
            // 
            this.gboxPrivate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gboxPrivate.Controls.Add(this.txtInput);
            this.gboxPrivate.Location = new System.Drawing.Point(19, 72);
            this.gboxPrivate.Name = "gboxPrivate";
            this.gboxPrivate.Size = new System.Drawing.Size(547, 154);
            this.gboxPrivate.TabIndex = 14;
            this.gboxPrivate.TabStop = false;
            this.gboxPrivate.Text = "输入";
            // 
            // btnCompute
            // 
            this.btnCompute.Location = new System.Drawing.Point(488, 43);
            this.btnCompute.Name = "btnCompute";
            this.btnCompute.Size = new System.Drawing.Size(75, 23);
            this.btnCompute.TabIndex = 13;
            this.btnCompute.Text = "计算";
            this.btnCompute.UseVisualStyleBackColor = true;
            this.btnCompute.Click += new System.EventHandler(this.btnCompute_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 16;
            this.label1.Text = "格式";
            // 
            // cboxFormat
            // 
            this.cboxFormat.FormattingEnabled = true;
            this.cboxFormat.Items.AddRange(new object[] {
            "Base64",
            "Hex",
            "Base64URL"});
            this.cboxFormat.Location = new System.Drawing.Point(60, 19);
            this.cboxFormat.Name = "cboxFormat";
            this.cboxFormat.Size = new System.Drawing.Size(77, 20);
            this.cboxFormat.TabIndex = 18;
            this.cboxFormat.Text = "Base64";
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(60, 45);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(410, 21);
            this.txtKey.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 19;
            this.label2.Text = "秘钥";
            // 
            // chkReplaceEnter
            // 
            this.chkReplaceEnter.AutoSize = true;
            this.chkReplaceEnter.Location = new System.Drawing.Point(165, 21);
            this.chkReplaceEnter.Name = "chkReplaceEnter";
            this.chkReplaceEnter.Size = new System.Drawing.Size(120, 16);
            this.chkReplaceEnter.TabIndex = 21;
            this.chkReplaceEnter.Text = "移除回车(\\r)换行";
            this.chkReplaceEnter.UseVisualStyleBackColor = true;
            // 
            // HmacSha256Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 406);
            this.Controls.Add(this.chkReplaceEnter);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboxFormat);
            this.Controls.Add(this.gboxPublic);
            this.Controls.Add(this.gboxPrivate);
            this.Controls.Add(this.btnCompute);
            this.Controls.Add(this.label1);
            this.Name = "HmacSha256Form";
            this.Text = "HMAC SHA256";
            this.gboxPublic.ResumeLayout(false);
            this.gboxPublic.PerformLayout();
            this.gboxPrivate.ResumeLayout(false);
            this.gboxPrivate.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.GroupBox gboxPublic;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.GroupBox gboxPrivate;
        private System.Windows.Forms.Button btnCompute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboxFormat;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkReplaceEnter;
    }
}