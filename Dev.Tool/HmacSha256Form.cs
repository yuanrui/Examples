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
    public partial class HmacSha256Form : Form
    {
        public HmacSha256Form()
        {
            InitializeComponent();
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            var input = GetInputText();
            var key = txtKey.Text ?? String.Empty;
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);


            var hmacsha256 = new HMACSHA256(keyBytes);
            var buffer = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            
            txtOutput.Text = GetFormatText(buffer, cboxFormat.Text);
        }

        private string GetInputText()
        {
            var input = txtInput.Text ?? String.Empty;

            if (chkReplaceEnter.Checked)
            {
                var lines = input.Split('\n');

                var text = string.Join("\n", lines.Select(m => m.TrimEnd('\r')));

                return text;
            }

            return input;
        }

        private string GetFormatText(Byte[] buffer, string fmt)
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
