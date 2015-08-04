using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Twitchy
{
    
    public partial class MainForm : Form
    {
        private static char slash = System.IO.Path.DirectorySeparatorChar;

        private String Streamer;
        
        public MainForm()
        {   // This is essentially Main() at this point.
            // Split everything into smaller tasks
            this.FormClosed += new FormClosedEventHandler(context.OnFormClosed);
            this.Closed += new EventHandler(context.OnFormClosed);
            InitializeComponent();
            init();
        }


        private static List<string> unescape(List<string> toUnescape)
        {
            List<string> unescaped = new List<string>();
            foreach (string s in toUnescape)
            {
                unescaped.Add(Regex.Unescape(s));
            }
            return unescaped;
        }

        private static string unescape(string toUnescape)
        {
            return Regex.Unescape(toUnescape);
        }


        private void init() 
        {
            Config.checkOauth();
            dataGridView1.Rows.Clear();

            List<StreamObject> ParsedStreams = new List<StreamObject>();
                                // Ask Twitch's API nicely if we can see who our user is following
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/followed?oauth_token="+Config.oauthToken);
            JObject jo;

            try {
                string responseText;
                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {   // Convert Twitch's response into a string and then into a JObject that we can play with
                   StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                   responseText = reader.ReadToEnd();
                   jo = JsonConvert.DeserializeObject<JObject>(responseText);
                }
                JArray streams = (JArray)jo["streams"];
                foreach (JObject o in streams)
                {
                    StreamObject so = new StreamObject(o["channel"]["display_name"].ToString(),
                                                        o["game"].ToString(),
                                                        o["channel"]["status"] != null ? unescape(o["channel"]["status"].ToString()) : o["channel"]["display_name"].ToString() + "'s Stream");
                    ParsedStreams.Add(so);
                }

                using (var ps = ParsedStreams.GetEnumerator())
                {
                    while (ps.MoveNext())
                    {
                        using (DataGridViewRow row = new DataGridViewRow())
                        {   // Populate the DataGridView.
                            StreamObject temp = ps.Current;
                            row.CreateCells(dataGridView1, new string[] { temp.name, temp.game, temp.title });
                            dataGridView1.Rows.Add(row);
                        }
                    }
                }
                
                Config.valid = true;
            }
            catch (WebException e)
            {
                if (e.Message.Contains("401"))
                {
                    Config.valid = false;
                    using (DataGridViewRow a = new DataGridViewRow())
                    {   // Complain about the invalid OAuth token.
                        a.CreateCells(dataGridView1, new string[] { "Invalid OAuth Token.", "Please press refresh."});
                        dataGridView1.Rows.Add(a);
                    }
                }
                else
                {
                    using (DataGridViewRow a = new DataGridViewRow())
                    {   // Complain about the lack of internet
                        a.CreateCells(dataGridView1, new string[] { "Connection Issue: "+e.Status, "Twitch couldn't be reached,", "Check your connection and check to see if Twitch is up." });
                        dataGridView1.Rows.Add(a);
                    }
                }
            }
            catch (Exception e)
            {
                using (DataGridViewRow a = new DataGridViewRow())
                {   // Complain about the lack of internet
                    a.CreateCells(dataGridView1, new string[] { "There was a problem", e.Message, "Pressing Refresh some more will probably fix it." });
                    dataGridView1.Rows.Add(a);
                }
            }
        }

        private void autoSize()
        {
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            int cWidth = dataGridView1.RowHeadersWidth;
            foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
            {
                cWidth += dgvc.Width;
            }
            cWidth += 45; // Magic number, it's approximately the original difference in size
                          // between dataGridView1 and the main form, 300-256, rounded up to the nearest fifth pixel.
            context.twitchy.Width = cWidth;

            int rHeight = dataGridView1.ColumnHeadersHeight;
            foreach (DataGridViewRow dgvr in dataGridView1.Rows)
            {
                rHeight += dgvr.Height;
            }
            rHeight += 145; // Another magic number, approximately the original difference between
                            // the original size of dataGridView1 and the main form, 315-173, rounded up to the nearest fifth pixel.
            context.twitchy.Height = rHeight;
        }

        private void button2_Click(object sender, EventArgs e)
        {   // Refresh
            init();
            autoSize();
        }

        private void button1_Click(object sender, EventArgs e)
        {   // Start Stream
            if (Streamer == null)
            {   // If no streamer is selected, complain and break.
                MessageBox.Show("Select a streamer.");
                return;
            }

            Process livestreamer = new Process();
            if (Config.usePath) { // Either use whatever is in PATH or use the packaged versions, might help with *nix compat
                livestreamer.StartInfo.FileName = "livestreamer";
                livestreamer.StartInfo.Arguments = "twitch.tv/" + Streamer + " best";
            } else {
                livestreamer.StartInfo.FileName = "." + slash + "ls" + slash + "livestreamer.exe";
                livestreamer.StartInfo.Arguments = "-p MPC-HC" + slash + "mpc-hc.exe twitch.tv/" + Streamer + " best";
            }
            livestreamer.Start();
            if (Config.openChatWindow) ChatWindow.ShowChat(Streamer);
            if (Config.closeAfterLaunch) this.Close();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button1_Click(sender, e);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {   // Dirty hack to fit the table's elements.
            // Shouldn't be needed anymore, since it works in Form1_Shown
            // autoSize();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {   // Event handler to make sure everything closes tidily.
            context.OnFormClosed(sender, e);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            Streamer = (dataGridView1.CurrentRow.Cells[0].Value.ToString());
        }

        private void Form1_Shown(object sender, EventArgs e)
        {   // This works, for some reason.
            autoSize();
        }

        private void Config_Click(object sender, EventArgs e)
        {
            Form conf = new Config();
            conf.Show();
        }
    }
}
