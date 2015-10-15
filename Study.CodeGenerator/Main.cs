using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Study.CodeGenerator.Core;
using System.Threading;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;

namespace Study.CodeGenerator
{
    public partial class Main : Form
    {
        private readonly BackgroundWorker worker;

        #region delegate关键字声明委托
        
        delegate void ShowProgressDelegate();
        delegate void StartProgressDelegate(int max, int min, int step, int value);
        delegate void EndProgressDelegate();
        delegate void ShowLabelDelegate(string msg);
        
        #endregion

        #region Action关键字声明委托
        
        Action action1;
        Action<string> action2;
        Action<int, int, int, int> action3;
        
        #endregion

        void ShowProgress()
        {
            progressBar.PerformStep();
        }

        void StartProgress(int max, int min, int step, int value)
        {
            progressBar.Maximum = max;
            progressBar.Minimum = min; 
            progressBar.Step = step;
            progressBar.Value = value;
            progressBar.Visible = true;
            lblMsg.Visible = true;
        }

        void EndProgress()
        {
            progressBar.Visible = false;
            lblMsg.Visible = false;
        }

        void ShowLabel(string msg)
        {
            lblMsg.Text = msg;
        }

        public Main()
        {
            InitializeComponent();
            Init();
            worker = new BackgroundWorker { WorkerSupportsCancellation = true };
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        public void Init()
        {
            dgvColumns.AutoGenerateColumns = false;
            lstTable.SelectedIndexChanged +=new EventHandler(lstTable_SelectedIndexChanged);
            string connStr = GetConnStr();
            BindDatabaseData(connStr);
            Closing += new CancelEventHandler(Main_Closing);
            

            CodeEntityParameter entParam = CodeEntityParameter.Get();

            rbtnAddPrefix.Checked = entParam.IsAddOrRemovePrefix;
            rbtnRemovePrefix.Checked = !entParam.IsAddOrRemovePrefix;
            rbtnAddPrefix.Checked = entParam.IsAddOrRemovePrefix;
            chkSuffix.Checked = entParam.IsAddSuffix;
            txtNameSpace.Text = entParam.NameSpace;
            txtPrefix.Text = entParam.Prefix;
            txtSuffix.Text = entParam.Suffix;
            txtSuffix.Enabled = entParam.IsAddSuffix;
            txtSavePath.Text = entParam.SavePath;
            txtServer.Text = entParam.Server;
            txtUser.Text = entParam.UserId;
            txtPwd.Text = entParam.UserPwd;
            rbtnMSSQL.Checked = !entParam.IntegratedSecurity;
            rbtnWindows.Checked = entParam.IntegratedSecurity;

            cboDatabase.Text = entParam.DataBaseName ?? cboDatabase.Text;
        }

        void Main_Closing(object sender, CancelEventArgs e)
        {
            CodeEntityParameter entityParam = GetEntityParameter();
            entityParam.Save();
        }

        private string GetConnStr(string dataBaseName = "")
        {
            SqlConnectionStringBuilder sqlConnStr = new SqlConnectionStringBuilder();
            sqlConnStr.DataSource = string.IsNullOrWhiteSpace(txtServer.Text) ? "(local)" : txtServer.Text;
            if (rbtnWindows.Checked)
            {
                sqlConnStr.IntegratedSecurity = rbtnWindows.Checked;
            }
            else
            {
                sqlConnStr.UserID = txtUser.Text;
                sqlConnStr.Password = txtPwd.Text;
            }
            
            if (string.IsNullOrWhiteSpace(dataBaseName))
            {
                sqlConnStr.InitialCatalog = "master";
            }
            else
            {
                sqlConnStr.InitialCatalog = dataBaseName;
            }
            sqlConnStr.ConnectTimeout = 30;
            return sqlConnStr.ToString();
        }

        private void BindDatabaseData(string connStr, string errorMessage = "")
        {
            DataTable dt = new DataTable();

            cboDatabase.DisplayMember = "name";
            cboDatabase.ValueMember = "name";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    if (conn.State != ConnectionState.Open)
                    {
                        return;
                    }
                }
                catch (SqlException sqlEx)
                {
                    cboDatabase.DataSource = dt;

                    if (! string.IsNullOrWhiteSpace(errorMessage))
                    {
                        MessageBox.Show(this, errorMessage, "数据库连接异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(this, sqlEx.Message, "数据库连接异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select name from sys.databases order by database_id desc";

                    using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(dt);
                    }
                }
            } 
            
            cboDatabase.DataSource = dt;
        }

        private CodeEntityParameter GetEntityParameter()
        {
            CodeEntityParameter entParam = new CodeEntityParameter();
            entParam.IsAddOrRemovePrefix = rbtnAddPrefix.Checked;
            entParam.IsAddOrRemovePrefix = !rbtnRemovePrefix.Checked;
            entParam.IsAddSuffix = chkSuffix.Checked;
            entParam.NameSpace = txtNameSpace.Text;
            entParam.Prefix = txtPrefix.Text;
            entParam.Suffix = txtSuffix.Text;
            entParam.SavePath = txtSavePath.Text;
            entParam.Server = txtServer.Text;
            entParam.UserId = txtUser.Text;
            entParam.UserPwd = txtPwd.Text;
            entParam.IntegratedSecurity = rbtnWindows.Checked;
            entParam.DataBaseName = cboDatabase.Text;
            return entParam;
        }

        private void rbtnWindows_CheckedChanged(object sender, EventArgs e)
        {
            txtUser.Enabled = ! rbtnWindows.Checked;
            txtPwd.Enabled = ! rbtnWindows.Checked;
            string connStr = GetConnStr();
            BindDatabaseData(connStr);
        }

        private void rbtnMSSQL_CheckedChanged(object sender, EventArgs e)
        {
            txtUser.Enabled = rbtnMSSQL.Checked;
            txtPwd.Enabled = rbtnMSSQL.Checked;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboDatabase.Text) || cboDatabase.SelectedValue == null)
            {
                MessageBox.Show(string.Format("数据库未选择{0}未连接数据库? 请尝试连接数据库! ", Environment.NewLine));
                return;
            }
            
            string connStr = GetConnStr(cboDatabase.SelectedValue.ToString());
            CodeEntityDataReader entDataReader = new CodeEntityDataReader(connStr);
            var tableList = entDataReader.GetTables();
            lstTable.Items.Clear();
            if (tableList != null && tableList.Any())
            {
                lstTable.Items.AddRange(tableList.ToArray());
            }
            else
            {
                return;
            }
            lstTable.DisplayMember = "Name";
            lstTable.ValueMember = "Name";
            dgvColumns.DataSource = null;
        }

        private void lstTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDatabase.SelectedValue == null || string.IsNullOrWhiteSpace(cboDatabase.SelectedValue.ToString()))
            {
                MessageBox.Show("未选择数据!");
                return;
            }

            var table = lstTable.SelectedItem as Table;
            if (table == null)
            {
                return;
            }
     
            string dbName = cboDatabase.SelectedValue.ToString();

            var columnList = new CodeEntityDataReader(GetConnStr(dbName)).GetColumns(table);
            dgvColumns.DataSource = columnList;

            string className = table.Name;
            if (rbtnAddPrefix.Checked)
            {
                if (! table.Name.StartsWith(txtPrefix.Text))
                {
                    className = string.Format("{0}{1}", txtPrefix.Text, table.Name);
                }
            }
            else
            {
                if (rbtnRemovePrefix.Checked)
                {
                    if (table.Name.StartsWith(txtPrefix.Text))
                    {
                        className = table.Name.Remove(0, txtPrefix.Text.Count());
                    }
                }
            }

            if (chkSuffix.Checked)
            {
                className = string.Format("{0}{1}", className, txtSuffix.Text);
            }

            lblTable.Text = string.Format("注释:{2} 表名:{0} 实体类名:{1}", table.Name, className, table.Comment);
        }

        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            Table table = new Table();
            
            if (cboDatabase.SelectedValue == null)
            {
                MessageBox.Show("数据库未选择,未连接数据库?");
                return;
            }
            string dbName = cboDatabase.SelectedValue.ToString();
            CodeEntityDataReader entDataReader = new CodeEntityDataReader(GetConnStr(dbName));
            
            table = lstTable.SelectedItem as Table;
            if (table == null)
            {
                MessageBox.Show("未选择要生成的数据表");
                return;
            }
            string tableName = table.Name;
            List<Table> tableList = entDataReader.GetTables().ToList();

            table = tableList.FirstOrDefault(m => m.Name == tableName);
            entDataReader.GetColumns(table);

            CodeEntityParameter entParam = GetEntityParameter();

            CodeGenerator.Core.CodeGenerator code = new CodeGenerator.Core.CodeGenerator();
            code.CreateCodeFile(table, entParam);

            MessageBox.Show("生成成功!");
        }

        private void btnSelectSavePath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult dialogResult = fbd.ShowDialog();
                if (dialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    txtSavePath.Text = fbd.SelectedPath;
                }                
            }            
        }

        private void btnGenerateAllCode_Click(object sender, EventArgs e)
        {
            if (cboDatabase.SelectedValue == null)
            {
                MessageBox.Show("数据库未选择,未连接数据库?");
                return;
            }
            
            string dbName = cboDatabase.SelectedValue.ToString();
           
            CodeEntityParameter entParam = GetEntityParameter();
            
            worker.RunWorkerAsync(entParam);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //EndProgressDelegate endProgress = new EndProgressDelegate(EndProgress);
            //progressBar.Invoke(endProgress);

            action1 = EndProgress;
            progressBar.Invoke(action1);
            MessageBox.Show("生成成功!");
        }
        
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (worker != null && worker.CancellationPending != true)
            {
                CodeEntityParameter entParam = e.Argument as CodeEntityParameter;
            
                CodeGenerator.Core.CodeGenerator code = new CodeGenerator.Core.CodeGenerator();
            
                CodeEntityDataReader entDataReader = new CodeEntityDataReader(GetConnStr(entParam.DataBaseName));

                List<Table> list = new List<Table>();
                foreach (var table in lstTable.Items)
                {
                    list.Add(table as Table);
                }
                StartProgressDelegate startProgress = new StartProgressDelegate(StartProgress);
                //progressBar.Invoke(startProgress, new object[] { list.Count, 0, 1, 0 });

                action3 = StartProgress;
                progressBar.Invoke(action3, new object[] { list.Count, 0, 1, 0 });

                #region 用PLinq方式执行生成操作
            
                //var pList = list.AsParallel<Table>();
                //pList.ForAll((t) => {
                //    entDataReader.ColumnList(t as Table);
                //    code.CreateCodeFile(t as Table, entParam);
                
                //    ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);
                //    progressBar.Invoke(showProgress);

                //    ShowLabelDelegate showLable = new ShowLabelDelegate(ShowLabel);
                //    lblMsg.Invoke(showLable, string.Format("正在处理数据表:{0}", t.Name));
                //});

                #endregion

                #region 传统方式foreach执行生成操作

                //foreach (var table in list)
                //{
                //    entDataReader.ColumnList(table as Table);
                //    code.CreateCodeFile(table as Table, entParam);

                //    ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);
                //    progressBar.Invoke(showProgress);
                //}

                #endregion

                #region Parallel并行执行生成操作

                Parallel.ForEach(list, (t) => {
                    entDataReader.GetColumns(t as Table);
                    code.CreateCodeFile(t as Table, entParam);

                #region 委托方式使用1
                
                //ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);
                //progressBar.Invoke(showProgress);

                //ShowLabelDelegate showLable = new ShowLabelDelegate(ShowLabel);
                //lblMsg.Invoke(showLable, string.Format("正在处理数据表:{0}", t.Name));
                
                #endregion

                #region 委托方式使用2
                
                action1 = ShowProgress;
                progressBar.Invoke(action1);

                action2 = ShowLabel;
                lblMsg.Invoke(action2, string.Format("正在处理数据表:{0}", t.Name));
                
                #endregion
                });

                #endregion
            }
        }

        private void btnConnDB_Click(object sender, EventArgs e)
        {
            string connStr = GetConnStr();
            BindDatabaseData(connStr);
        }

        private void chkSuffix_CheckedChanged(object sender, EventArgs e)
        {
            txtSuffix.Enabled = chkSuffix.Checked;
        }

        private void cboDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboDatabase.Text) || cboDatabase.SelectedValue == null)
            {
                MessageBox.Show(string.Format("数据库未选择{0}未连接数据库? 请尝试连接数据库! ", Environment.NewLine));
                return;
            }

            string connStr = GetConnStr(cboDatabase.SelectedValue.ToString());
            CodeEntityDataReader entDataReader = new CodeEntityDataReader(connStr);
            var tableList = entDataReader.GetTables();
            dgvColumns.DataSource = null;
            
            lstTable.DisplayMember = "Name";
            lstTable.ValueMember = "Name";
            
            lstTable.Items.Clear();
            if (tableList != null && tableList.Any())
            {
                lstTable.Items.AddRange(tableList.ToArray());
            }
            else
            {
                return;
            }
        }
    }
}
