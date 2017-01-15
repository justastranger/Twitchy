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

        private String Streamer;
        private List<StreamObject> ParsedStreams = new List<StreamObject>();
        
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
            if (backgroundWorker1.IsBusy != true)
            {
                dataGridView1.Rows.Clear();
                using (DataGridViewRow a = new DataGridViewRow())
                {   // Complain about the invalid OAuth token.
                    a.CreateCells(dataGridView1, new string[] { "Working..." });
                    dataGridView1.Rows.Add(a);
                }
                backgroundWorker1.RunWorkerAsync();
            }
            else MessageBox.Show("Streamer list is already updating, please wait.");
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
            //TODO rename these config values since Livestreamer is defunct now
            if (Config.config["useCustomLivestreamer"].ToObject<bool>())
            {
                livestreamer.StartInfo.FileName = Config.config["livestreamer"].ToString();
            } else {
                livestreamer.StartInfo.FileName = @".\streamlink\Streamlink.exe";
            }
            if (Config.config["useCustomPlayer"].ToObject<bool>())
            {
                livestreamer.StartInfo.Arguments = "-p " + Config.config["player"].ToString() + " twitch.tv/" + Streamer + " best";
            } else {
                livestreamer.StartInfo.Arguments = "-p " + @".\MPC-HC\mpc-hc.exe" + " twitch.tv/" + Streamer + " best";
            }
            
            livestreamer.Start();
            if (Config.config["openChatWindow"].ToObject<bool>()) ChatWindow.ShowChat(Streamer);
            if (Config.config["closeAfterLaunch"].ToObject<bool>()) this.Close();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button1_Click(sender, e);
        }
        
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {   // Event handler to make sure everything closes tidily.
            context.OnFormClosed(sender, e);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            Streamer = dataGridView1.CurrentRow.Cells[0].Value.ToString();
        }
        
        private void Config_Click(object sender, EventArgs e)
        {   // Construct a new config menu and show it.
            Form conf = new Config();
            conf.Show();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //Config.checkOauth();

            ParsedStreams = new List<StreamObject>();
            // Ask Twitch's API nicely if we can see who our user is following
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/followed");
            request.Headers.Add("Accept", "application/vnd.twitchtv.v3+json");
            request.Headers.Add("Authorization", "OAuth " + Config.config["oauth"].ToObject<string>());
            JObject jo;

            try
            {
                string responseText;
                using (Stream stream = request.GetResponse().GetResponseStream())
                {   // Convert Twitch's response into a string and then into a JObject that we can play with
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseText = reader.ReadToEnd();
                    jo = JsonConvert.DeserializeObject<JObject>(responseText);
                }
                JArray streams = (JArray)jo["streams"];
                foreach (JObject o in streams)
                {                           // If null, assign to the first option
                    string title = o["channel"]["status"] == null ? o["channel"]["display_name"].ToString() + "'s Stream"
                        // Otherwise, check if title unescaping is enabled, if so use the unescaped title
                                        : Config.config["disableTitleUnescaping"].ToObject<bool>() ? o["channel"]["status"].ToString()
                        // Finally, if all else fails, use the unescaped title
                                        : unescape(o["channel"]["status"].ToString());
                    StreamObject so = new StreamObject(o["channel"]["display_name"].ToString(),
                                                        o["game"].ToString(),
                                                        title
                                                       );
                    ParsedStreams.Add(so);
                }
                Config.valid = true;
            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("401"))
                {
                    Config.valid = false;
                    using (DataGridViewRow a = new DataGridViewRow())
                    {   // Complain about the invalid OAuth token.
                        a.CreateCells(dataGridView1, new string[] { "Invalid OAuth Token.", "Please press refresh." });
                        dataGridView1.Rows.Add(a);
                    }
                }
                else
                {
                    using (DataGridViewRow a = new DataGridViewRow())
                    {   // Complain about the lack of internet
                        a.CreateCells(dataGridView1, new string[] { "Connection Issue: " + ex.Status, "Twitch couldn't be reached,", "Check your connection and whether or not Twitch is up." });
                        dataGridView1.Rows.Add(a);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                using (DataGridViewRow a = new DataGridViewRow())
                {   // Complain about the lack of internet
                    a.CreateCells(dataGridView1, new string[] { "Someone is doing something", "interesting with their title.", "Disable unescaping in the config and refresh to resolve this problem." });
                    dataGridView1.Rows.Add(a);
                }
            }
            catch (Exception ex)
            {
                using (DataGridViewRow a = new DataGridViewRow())
                {   // Complain about the lack of internet
                    a.CreateCells(dataGridView1, new string[] { "There was a problem", ex.Message, "Pressing Refresh some more will probably fix it." });
                    dataGridView1.Rows.Add(a);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.Rows.Clear();
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
            autoSize();
        }
        
        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (Config.config["minimizeToTaskbar"].ToObject<bool>() && this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            this.WindowState = FormWindowState.Normal;
        }
    }
}
