namespace Study.CodeGenerator
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.dgvColumns = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DBType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsPrimaryKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsForeignKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsNullAble = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblPwd = new System.Windows.Forms.Label();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.rbtnWindows = new System.Windows.Forms.RadioButton();
            this.rbtnMSSQL = new System.Windows.Forms.RadioButton();
            this.cboDatabase = new System.Windows.Forms.ComboBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.lstTable = new System.Windows.Forms.ListBox();
            this.pnlFixTable = new System.Windows.Forms.Panel();
            this.lblSuffix = new System.Windows.Forms.Label();
            this.txtSuffix = new System.Windows.Forms.TextBox();
            this.chkSuffix = new System.Windows.Forms.CheckBox();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.rbtnRemovePrefix = new System.Windows.Forms.RadioButton();
            this.rbtnAddPrefix = new System.Windows.Forms.RadioButton();
            this.lblSavePath = new System.Windows.Forms.Label();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.btnSelectSavePath = new System.Windows.Forms.Button();
            this.btnGenerateCode = new System.Windows.Forms.Button();
            this.btnGenerateAllCode = new System.Windows.Forms.Button();
            this.lblNameSpace = new System.Windows.Forms.Label();
            this.txtNameSpace = new System.Windows.Forms.TextBox();
            this.lblTable = new System.Windows.Forms.Label();
            this.btnConnDB = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblMsg = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumns)).BeginInit();
            this.pnlFixTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvColumns
            // 
            this.dgvColumns.AllowUserToAddRows = false;
            this.dgvColumns.AllowUserToDeleteRows = false;
            this.dgvColumns.AllowUserToResizeColumns = false;
            this.dgvColumns.AllowUserToResizeRows = false;
            this.dgvColumns.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvColumns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.DBType,
            this.Type,
            this.Comment,
            this.IsPrimaryKey,
            this.IsForeignKey,
            this.IsNullAble});
            this.dgvColumns.Location = new System.Drawing.Point(207, 127);
            this.dgvColumns.MultiSelect = false;
            this.dgvColumns.Name = "dgvColumns";
            this.dgvColumns.ReadOnly = true;
            this.dgvColumns.RowHeadersVisible = false;
            this.dgvColumns.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dgvColumns.RowTemplate.Height = 23;
            this.dgvColumns.Size = new System.Drawing.Size(705, 240);
            this.dgvColumns.TabIndex = 1;
            // 
            // ColumnName
            // 
            this.ColumnName.DataPropertyName = "Name";
            this.ColumnName.FillWeight = 17.7665F;
            this.ColumnName.HeaderText = "列名";
            this.ColumnName.MinimumWidth = 80;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // DBType
            // 
            this.DBType.DataPropertyName = "DBType";
            this.DBType.FillWeight = 17.7665F;
            this.DBType.HeaderText = "SQL数据类型";
            this.DBType.MinimumWidth = 120;
            this.DBType.Name = "DBType";
            this.DBType.ReadOnly = true;
            // 
            // Type
            // 
            this.Type.DataPropertyName = "TypeName";
            this.Type.FillWeight = 593.401F;
            this.Type.HeaderText = ".Net类型";
            this.Type.MinimumWidth = 100;
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            // 
            // Comment
            // 
            this.Comment.DataPropertyName = "Comment";
            this.Comment.FillWeight = 17.7665F;
            this.Comment.HeaderText = "列说明";
            this.Comment.MinimumWidth = 120;
            this.Comment.Name = "Comment";
            this.Comment.ReadOnly = true;
            // 
            // IsPrimaryKey
            // 
            this.IsPrimaryKey.DataPropertyName = "IsPrimaryKey";
            this.IsPrimaryKey.FillWeight = 17.7665F;
            this.IsPrimaryKey.HeaderText = "是否主键";
            this.IsPrimaryKey.MinimumWidth = 80;
            this.IsPrimaryKey.Name = "IsPrimaryKey";
            this.IsPrimaryKey.ReadOnly = true;
            // 
            // IsForeignKey
            // 
            this.IsForeignKey.DataPropertyName = "IsForeignKey";
            this.IsForeignKey.FillWeight = 17.7665F;
            this.IsForeignKey.HeaderText = "是否外键";
            this.IsForeignKey.MinimumWidth = 80;
            this.IsForeignKey.Name = "IsForeignKey";
            this.IsForeignKey.ReadOnly = true;
            // 
            // IsNullAble
            // 
            this.IsNullAble.DataPropertyName = "IsNullAble";
            this.IsNullAble.FillWeight = 17.7665F;
            this.IsNullAble.HeaderText = "是否为空";
            this.IsNullAble.MinimumWidth = 80;
            this.IsNullAble.Name = "IsNullAble";
            this.IsNullAble.ReadOnly = true;
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(12, 31);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(47, 12);
            this.lblServer.TabIndex = 2;
            this.lblServer.Text = "服务器:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(65, 28);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(109, 21);
            this.txtServer.TabIndex = 3;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(12, 61);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(47, 12);
            this.lblUser.TabIndex = 4;
            this.lblUser.Text = "用户名:";
            // 
            // txtUser
            // 
            this.txtUser.Enabled = false;
            this.txtUser.Location = new System.Drawing.Point(65, 58);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(83, 21);
            this.txtUser.TabIndex = 5;
            // 
            // lblPwd
            // 
            this.lblPwd.AutoSize = true;
            this.lblPwd.Location = new System.Drawing.Point(154, 61);
            this.lblPwd.Name = "lblPwd";
            this.lblPwd.Size = new System.Drawing.Size(35, 12);
            this.lblPwd.TabIndex = 6;
            this.lblPwd.Text = "密码:";
            // 
            // txtPwd
            // 
            this.txtPwd.Enabled = false;
            this.txtPwd.Location = new System.Drawing.Point(195, 58);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.Size = new System.Drawing.Size(85, 21);
            this.txtPwd.TabIndex = 7;
            // 
            // rbtnWindows
            // 
            this.rbtnWindows.AutoSize = true;
            this.rbtnWindows.Checked = true;
            this.rbtnWindows.Location = new System.Drawing.Point(183, 29);
            this.rbtnWindows.Name = "rbtnWindows";
            this.rbtnWindows.Size = new System.Drawing.Size(89, 16);
            this.rbtnWindows.TabIndex = 8;
            this.rbtnWindows.TabStop = true;
            this.rbtnWindows.Text = "Windows登陆";
            this.rbtnWindows.UseVisualStyleBackColor = true;
            this.rbtnWindows.CheckedChanged += new System.EventHandler(this.rbtnWindows_CheckedChanged);
            // 
            // rbtnMSSQL
            // 
            this.rbtnMSSQL.AutoSize = true;
            this.rbtnMSSQL.Location = new System.Drawing.Point(278, 29);
            this.rbtnMSSQL.Name = "rbtnMSSQL";
            this.rbtnMSSQL.Size = new System.Drawing.Size(77, 16);
            this.rbtnMSSQL.TabIndex = 9;
            this.rbtnMSSQL.Text = "MSSQL登陆";
            this.rbtnMSSQL.UseVisualStyleBackColor = true;
            this.rbtnMSSQL.CheckedChanged += new System.EventHandler(this.rbtnMSSQL_CheckedChanged);
            // 
            // cboDatabase
            // 
            this.cboDatabase.FormattingEnabled = true;
            this.cboDatabase.Location = new System.Drawing.Point(336, 58);
            this.cboDatabase.Name = "cboDatabase";
            this.cboDatabase.Size = new System.Drawing.Size(117, 20);
            this.cboDatabase.TabIndex = 10;
            this.cboDatabase.SelectedIndexChanged += new System.EventHandler(this.cboDatabase_SelectedIndexChanged);
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(283, 61);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(47, 12);
            this.lblDatabase.TabIndex = 11;
            this.lblDatabase.Text = "数据库:";
            // 
            // lstTable
            // 
            this.lstTable.FormattingEnabled = true;
            this.lstTable.ItemHeight = 12;
            this.lstTable.Location = new System.Drawing.Point(12, 97);
            this.lstTable.Name = "lstTable";
            this.lstTable.Size = new System.Drawing.Size(189, 268);
            this.lstTable.TabIndex = 13;
            // 
            // pnlFixTable
            // 
            this.pnlFixTable.Controls.Add(this.lblSuffix);
            this.pnlFixTable.Controls.Add(this.txtSuffix);
            this.pnlFixTable.Controls.Add(this.chkSuffix);
            this.pnlFixTable.Controls.Add(this.txtPrefix);
            this.pnlFixTable.Controls.Add(this.lblPrefix);
            this.pnlFixTable.Controls.Add(this.rbtnRemovePrefix);
            this.pnlFixTable.Controls.Add(this.rbtnAddPrefix);
            this.pnlFixTable.Location = new System.Drawing.Point(14, 405);
            this.pnlFixTable.Name = "pnlFixTable";
            this.pnlFixTable.Size = new System.Drawing.Size(327, 63);
            this.pnlFixTable.TabIndex = 14;
            // 
            // lblSuffix
            // 
            this.lblSuffix.AutoSize = true;
            this.lblSuffix.Location = new System.Drawing.Point(181, 38);
            this.lblSuffix.Name = "lblSuffix";
            this.lblSuffix.Size = new System.Drawing.Size(47, 12);
            this.lblSuffix.TabIndex = 6;
            this.lblSuffix.Text = "后缀名:";
            // 
            // txtSuffix
            // 
            this.txtSuffix.Location = new System.Drawing.Point(234, 36);
            this.txtSuffix.Name = "txtSuffix";
            this.txtSuffix.Size = new System.Drawing.Size(82, 21);
            this.txtSuffix.TabIndex = 5;
            // 
            // chkSuffix
            // 
            this.chkSuffix.AutoSize = true;
            this.chkSuffix.Location = new System.Drawing.Point(79, 38);
            this.chkSuffix.Name = "chkSuffix";
            this.chkSuffix.Size = new System.Drawing.Size(96, 16);
            this.chkSuffix.TabIndex = 4;
            this.chkSuffix.Text = "是否添加后缀";
            this.chkSuffix.UseVisualStyleBackColor = true;
            this.chkSuffix.CheckedChanged += new System.EventHandler(this.chkSuffix_CheckedChanged);
            // 
            // txtPrefix
            // 
            this.txtPrefix.Location = new System.Drawing.Point(234, 10);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(82, 21);
            this.txtPrefix.TabIndex = 3;
            // 
            // lblPrefix
            // 
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Location = new System.Drawing.Point(181, 13);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(47, 12);
            this.lblPrefix.TabIndex = 2;
            this.lblPrefix.Text = "前缀名:";
            // 
            // rbtnRemovePrefix
            // 
            this.rbtnRemovePrefix.AutoSize = true;
            this.rbtnRemovePrefix.Location = new System.Drawing.Point(92, 11);
            this.rbtnRemovePrefix.Name = "rbtnRemovePrefix";
            this.rbtnRemovePrefix.Size = new System.Drawing.Size(83, 16);
            this.rbtnRemovePrefix.TabIndex = 1;
            this.rbtnRemovePrefix.Text = "删除表前缀";
            this.rbtnRemovePrefix.UseVisualStyleBackColor = true;
            // 
            // rbtnAddPrefix
            // 
            this.rbtnAddPrefix.AutoSize = true;
            this.rbtnAddPrefix.Checked = true;
            this.rbtnAddPrefix.Location = new System.Drawing.Point(3, 11);
            this.rbtnAddPrefix.Name = "rbtnAddPrefix";
            this.rbtnAddPrefix.Size = new System.Drawing.Size(83, 16);
            this.rbtnAddPrefix.TabIndex = 0;
            this.rbtnAddPrefix.TabStop = true;
            this.rbtnAddPrefix.Text = "添加表前缀";
            this.rbtnAddPrefix.UseVisualStyleBackColor = true;
            // 
            // lblSavePath
            // 
            this.lblSavePath.AutoSize = true;
            this.lblSavePath.Location = new System.Drawing.Point(11, 380);
            this.lblSavePath.Name = "lblSavePath";
            this.lblSavePath.Size = new System.Drawing.Size(83, 12);
            this.lblSavePath.TabIndex = 15;
            this.lblSavePath.Text = "项目存放目录:";
            // 
            // txtSavePath
            // 
            this.txtSavePath.Location = new System.Drawing.Point(100, 377);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.Size = new System.Drawing.Size(703, 21);
            this.txtSavePath.TabIndex = 16;
            // 
            // btnSelectSavePath
            // 
            this.btnSelectSavePath.Location = new System.Drawing.Point(809, 375);
            this.btnSelectSavePath.Name = "btnSelectSavePath";
            this.btnSelectSavePath.Size = new System.Drawing.Size(93, 23);
            this.btnSelectSavePath.TabIndex = 17;
            this.btnSelectSavePath.Text = "选择存放路径";
            this.btnSelectSavePath.UseVisualStyleBackColor = true;
            this.btnSelectSavePath.Click += new System.EventHandler(this.btnSelectSavePath_Click);
            // 
            // btnGenerateCode
            // 
            this.btnGenerateCode.Location = new System.Drawing.Point(617, 411);
            this.btnGenerateCode.Name = "btnGenerateCode";
            this.btnGenerateCode.Size = new System.Drawing.Size(122, 51);
            this.btnGenerateCode.TabIndex = 18;
            this.btnGenerateCode.Text = "生成实体";
            this.btnGenerateCode.UseVisualStyleBackColor = true;
            this.btnGenerateCode.Click += new System.EventHandler(this.btnGenerateCode_Click);
            // 
            // btnGenerateAllCode
            // 
            this.btnGenerateAllCode.Location = new System.Drawing.Point(778, 411);
            this.btnGenerateAllCode.Name = "btnGenerateAllCode";
            this.btnGenerateAllCode.Size = new System.Drawing.Size(124, 51);
            this.btnGenerateAllCode.TabIndex = 19;
            this.btnGenerateAllCode.Text = "生成所有表实体";
            this.btnGenerateAllCode.UseVisualStyleBackColor = true;
            this.btnGenerateAllCode.Click += new System.EventHandler(this.btnGenerateAllCode_Click);
            // 
            // lblNameSpace
            // 
            this.lblNameSpace.AutoSize = true;
            this.lblNameSpace.Location = new System.Drawing.Point(347, 411);
            this.lblNameSpace.Name = "lblNameSpace";
            this.lblNameSpace.Size = new System.Drawing.Size(59, 12);
            this.lblNameSpace.TabIndex = 20;
            this.lblNameSpace.Text = "命名空间:";
            // 
            // txtNameSpace
            // 
            this.txtNameSpace.Location = new System.Drawing.Point(412, 408);
            this.txtNameSpace.Name = "txtNameSpace";
            this.txtNameSpace.Size = new System.Drawing.Size(109, 21);
            this.txtNameSpace.TabIndex = 21;
            // 
            // lblTable
            // 
            this.lblTable.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTable.Location = new System.Drawing.Point(207, 101);
            this.lblTable.Name = "lblTable";
            this.lblTable.Size = new System.Drawing.Size(705, 18);
            this.lblTable.TabIndex = 22;
            this.lblTable.Text = "注释,表名,类名";
            this.lblTable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnConnDB
            // 
            this.btnConnDB.Location = new System.Drawing.Point(371, 26);
            this.btnConnDB.Name = "btnConnDB";
            this.btnConnDB.Size = new System.Drawing.Size(82, 23);
            this.btnConnDB.TabIndex = 23;
            this.btnConnDB.Text = "连接数据库";
            this.btnConnDB.UseVisualStyleBackColor = true;
            this.btnConnDB.Click += new System.EventHandler(this.btnConnDB_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 496);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(900, 13);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 24;
            this.progressBar.Visible = false;
            // 
            // lblMsg
            // 
            this.lblMsg.Location = new System.Drawing.Point(11, 480);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(500, 12);
            this.lblMsg.TabIndex = 25;
            this.lblMsg.Text = "提示";
            this.lblMsg.Visible = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 516);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnConnDB);
            this.Controls.Add(this.lblTable);
            this.Controls.Add(this.txtNameSpace);
            this.Controls.Add(this.lblNameSpace);
            this.Controls.Add(this.btnGenerateAllCode);
            this.Controls.Add(this.btnGenerateCode);
            this.Controls.Add(this.btnSelectSavePath);
            this.Controls.Add(this.txtSavePath);
            this.Controls.Add(this.lblSavePath);
            this.Controls.Add(this.pnlFixTable);
            this.Controls.Add(this.lstTable);
            this.Controls.Add(this.lblDatabase);
            this.Controls.Add(this.cboDatabase);
            this.Controls.Add(this.rbtnMSSQL);
            this.Controls.Add(this.rbtnWindows);
            this.Controls.Add(this.txtPwd);
            this.Controls.Add(this.lblPwd);
            this.Controls.Add(this.txtUser);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.lblServer);
            this.Controls.Add(this.dgvColumns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "MSSQL实体生成工具 beta2";
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumns)).EndInit();
            this.pnlFixTable.ResumeLayout(false);
            this.pnlFixTable.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvColumns;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblPwd;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.RadioButton rbtnWindows;
        private System.Windows.Forms.RadioButton rbtnMSSQL;
        private System.Windows.Forms.ComboBox cboDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.ListBox lstTable;
        private System.Windows.Forms.Panel pnlFixTable;
        private System.Windows.Forms.RadioButton rbtnAddPrefix;
        private System.Windows.Forms.RadioButton rbtnRemovePrefix;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.CheckBox chkSuffix;
        private System.Windows.Forms.TextBox txtSuffix;
        private System.Windows.Forms.Label lblSuffix;
        private System.Windows.Forms.Label lblSavePath;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Button btnSelectSavePath;
        private System.Windows.Forms.Button btnGenerateCode;
        private System.Windows.Forms.Button btnGenerateAllCode;
        private System.Windows.Forms.Label lblNameSpace;
        private System.Windows.Forms.TextBox txtNameSpace;
        private System.Windows.Forms.Label lblTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DBType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsPrimaryKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsForeignKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsNullAble;
        private System.Windows.Forms.Button btnConnDB;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblMsg;
    }
}

