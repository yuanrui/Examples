namespace Dev.Tool
{
    partial class BitAndForm
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
            this.txtNumber = new System.Windows.Forms.TextBox();
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
            this.txtInput.Size = new System.Drawing.Size(417, 141);
            this.txtInput.TabIndex = 0;
            // 
            // gboxPublic
            // 
            this.gboxPublic.Controls.Add(this.txtOutput);
            this.gboxPublic.Location = new System.Drawing.Point(15, 206);
            this.gboxPublic.Name = "gboxPublic";
            this.gboxPublic.Size = new System.Drawing.Size(423, 173);
            this.gboxPublic.TabIndex = 9;
            this.gboxPublic.TabStop = false;
            this.gboxPublic.Text = "输出";
            // 
            // txtOutput
            // 
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Location = new System.Drawing.Point(3, 17);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(417, 153);
            this.txtOutput.TabIndex = 0;
            // 
            // gboxPrivate
            // 
            this.gboxPrivate.Controls.Add(this.txtInput);
            this.gboxPrivate.Location = new System.Drawing.Point(15, 39);
            this.gboxPrivate.Name = "gboxPrivate";
            this.gboxPrivate.Size = new System.Drawing.Size(423, 161);
            this.gboxPrivate.TabIndex = 8;
            this.gboxPrivate.TabStop = false;
            this.gboxPrivate.Text = "输入";
            // 
            // btnCompute
            // 
            this.btnCompute.Location = new System.Drawing.Point(360, 10);
            this.btnCompute.Name = "btnCompute";
            this.btnCompute.Size = new System.Drawing.Size(75, 23);
            this.btnCompute.TabIndex = 7;
            this.btnCompute.Text = "计算";
            this.btnCompute.UseVisualStyleBackColor = true;
            this.btnCompute.Click += new System.EventHandler(this.btnCompute_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "数值";
            // 
            // txtNumber
            // 
            this.txtNumber.Location = new System.Drawing.Point(56, 12);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.Size = new System.Drawing.Size(100, 21);
            this.txtNumber.TabIndex = 12;
            // 
            // BitAndForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 391);
            this.Controls.Add(this.txtNumber);
            this.Controls.Add(this.gboxPublic);
            this.Controls.Add(this.gboxPrivate);
            this.Controls.Add(this.btnCompute);
            this.Controls.Add(this.label1);
            this.Name = "BitAndForm";
            this.Text = "按位与计算";
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
        private System.Windows.Forms.TextBox txtNumber;
    }
}