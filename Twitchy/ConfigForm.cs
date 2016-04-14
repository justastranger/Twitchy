using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Twitchy.api;

namespace Twitchy
{
    public partial class ConfigForm : Form
    {
        public static TabPage[] tabPages;

        static ConfigForm()
        {
            tabPages = new TabPage[ConfigManager.configs.Count];
            TabPage tmp;
            // because I want the foreach and a for at the same time...
            int counter = 0;
            foreach (KeyValuePair<string, JToken> config in ConfigManager.configs)
            {
                tmp = new TabPage(config.Key);
                DataGridView dgv = new DataGridView();
                dgv.Name = config.Key;
                dgv.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
                dgv.Columns.Add("key", "key");
                dgv.Columns.Add("value", "value");
                foreach (KeyValuePair<string, JToken> subConfig in (JObject)config.Value)
                {
                    using (DataGridViewRow dgvr = new DataGridViewRow())
                    {
                        dgvr.CreateCells(dgv, new string[] { subConfig.Key, subConfig.Value.ToString(Formatting.None) });
                        dgv.Rows.Add(dgvr);
                    }
                }
                tmp.Controls.Add(dgv);
                dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                dgv.CellEndEdit += new DataGridViewCellEventHandler(cellEndEdit);
                tabPages[counter] = tmp;
                counter++;
            }
        }

        static void cellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView s = (DataGridView)sender;
            string key = s.Rows[e.RowIndex].Cells[0].Value.ToString();
            string newValue = s.CurrentCell.Value.ToString();
            string plugin = s.Name;
            ConfigManager.changeSetting(plugin, key, newValue);
        }

        public ConfigForm()
        {
            this.FormClosed += new FormClosedEventHandler(context.OnFormClosed);
            this.Closed += new EventHandler(context.OnFormClosed);
            InitializeComponent();
            configTabs.TabPages.AddRange(tabPages);
        }
    }
}
