using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Twitchy
{
    public partial class Form2 : Form
    {
        private string url;

        public Form2(string url)
        {
            this.url = url;
            InitializeComponent();
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            webBrowser1.Navigate(this.url);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            context.twitcheroo();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            context.OnFormClosed(sender, e);
        }
    }
}
