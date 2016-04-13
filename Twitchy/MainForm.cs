using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Twitchy
{

    public partial class MainForm : Form
    {
        private static char slash = System.IO.Path.DirectorySeparatorChar;
        private TabControl tabControl1;

        public MainForm()
        {   // This is essentially Main() at this point.
            // Split everything into smaller tasks
            this.FormClosed += new FormClosedEventHandler(context.OnFormClosed);
            this.Closed += new EventHandler(context.OnFormClosed);
            InitializeComponent();
        }
        
        public void addPage(TabPage page)
        {
            if (page != null) tabControl1.TabPages.Add(page);
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            ConfigForm form = new ConfigForm();
            form.Show();
        }
    }
}
