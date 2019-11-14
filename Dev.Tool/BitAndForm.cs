using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Dev.Tool
{
    public partial class BitAndForm : Form
    {
        public BitAndForm()
        {
            InitializeComponent();
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            var txt = txtInput.Text;
            var numTxt = txtNumber.Text;
            var num = 0L;
            Int64.TryParse(numTxt, out num);

            if (string.IsNullOrWhiteSpace(numTxt) || string.IsNullOrWhiteSpace(txt))
            {
                return;
            }

            var list = GetNumber(txt);
            var result = new List<Int64>();
            foreach (var item in list)
            {
                if ((item & num) == num)
                {
                    result.Add(item);
                }
            }

            txtOutput.Text = string.Join(",", result);
        }

        private List<Int64> GetNumber(string input)
        {
            var result = new List<long>();
            if (string.IsNullOrWhiteSpace(input))
            {
                return result;
            }

            var values = input.Split(',',';');
            var value = 0L;
            foreach (var item in values)
            {
                if (Int64.TryParse(item, out value))
                {
                    result.Add(value);
                }
            }


            return result;
        }
    }
}
