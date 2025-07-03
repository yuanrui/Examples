using Simple.Common.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Dev.Tool
{
    public partial class MD5Form : Form
    {
        public MD5Form()
        {
            InitializeComponent();
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            var input = txtInput.Text ?? String.Empty;
            var md5 = new MD5CryptoServiceProvider();
            var buffer = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            
            txtOutput.Text = GetText(buffer, cboxFormat.Text);
        }

        private string GetText(Byte[] buffer, string fmt)
        {
            switch (fmt)
            {
                case "Base64":
                    return Convert.ToBase64String(buffer);
                case "Base64URL":
                    return Base64Url.Encode(buffer);
                case "Hex":
                    return HexEncoding.GetString(buffer);
                default:
                    break;
            }

            return string.Empty;
        }
    }
}
