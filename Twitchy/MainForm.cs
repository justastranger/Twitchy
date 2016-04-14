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
using Twitchy.api;

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

        private void clearPages()
        {
            tabControl1.TabPages.Clear();
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            ConfigForm form = new ConfigForm();
            form.Show();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            clearPages();
            PluginManager.initializePages();
            DataGridView dgv = (DataGridView)tabControl1.SelectedTab.Controls[0];
            dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void launchButton_Click(object sender, EventArgs e)
        {
            TabPage openPage = tabControl1.SelectedTab;
            // Grab the contents of the first cell in the selected row, it should be the streamer
            string streamer = ((DataGridView)openPage.Controls[0]).CurrentRow.Cells[0].Value.ToString();
            Process livestreamer = new Process();
            livestreamer.StartInfo.FileName = @".\ls\livestreamer.exe";
            livestreamer.StartInfo.Arguments = @"-p .\MPC-HC\mpc-hc.exe " + PluginManager.plugins[openPage.Name].formatLivestreamer(streamer);
            livestreamer.Start();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedTab != null) ((DataGridView)tabControl1.SelectedTab.Controls[0]).AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
    }
}
