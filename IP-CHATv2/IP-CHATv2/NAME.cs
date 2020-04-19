using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IP_CHATv2
{
    public partial class NAME : Form
    {
        public NAME()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (nametxt.Text != "")
            {
                Class1.i = nametxt.Text.ToString();
                Form1 frm2 = new Form1();
                frm2.FormClosed += new FormClosedEventHandler(frm2_FormClosed);
                frm2.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("You are forgetting that your name is compulsory");
            }
        }
        private void frm2_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }
    }
}
