using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Simple.Common.Text;

namespace Dev.Tool
{
    public partial class Base64Form : Form
    {
        public Base64Form()
        {
            InitializeComponent();
        }

        private void btnEcode_Click(object sender, EventArgs e)
        {
            var txt = txtInput.Text;
            var buffer = Encoding.UTF8.GetBytes(txt);
            txtOutput.Text = Base64Url.Encode(buffer);
        }

        private void btnDecode_Click(object sender, EventArgs e)
        {
            var txt = txtOutput.Text;
            var buffer = Base64Url.Decode(txtOutput.Text);
            txtInput.Text = Encoding.UTF8.GetString(buffer);
        }

    }
}
