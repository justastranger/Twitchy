using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
                tabPages[counter] = tmp;
                counter++;
            }
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
