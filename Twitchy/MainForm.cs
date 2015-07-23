using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        // FileStream will fail and the app will crash if we don't have permissions
        private static FileStream oauth = File.Open(AppDomain.CurrentDomain.BaseDirectory + "\\oauth", FileMode.OpenOrCreate);
        private static string oauthToken;
        private static bool valid = true;

        private String Streamer;
        
        public MainForm()
        {   // This is essentially Main() at this point.
            // Split everything into smaller tasks
            InitializeComponent();
            init();
        }

        private static void checkOauth()
        {
            if (!valid)
            {
                oauth.Close();
                oauth = File.Open(AppDomain.CurrentDomain.BaseDirectory + "\\oauth", FileMode.Create);
                oauthToken = null;
            }

            if (oauthToken == null)
            {
                if (new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\oauth").Length == 0)
                {
                    oauthToken = Twitchy.Prompt.ShowDialog(@"Please enter your Oauth key, you can generate one at http://www.twitchapps.com/tmi/", "No OAuth Saved");
                    AddText(oauth, oauthToken);
                }
                else
                {
                    using (StreamReader reader = new StreamReader(oauth))
                    {
                        oauthToken = reader.ReadToEnd();
                    }
                }
                oauth.Close();
            }
        }

        private static void AddText(FileStream fs, string value)
        {   // Stole this from StackOverflow somewhere, google it for credits.
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
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
            checkOauth();

            List<StreamObject> ParsedStreams = new List<StreamObject>();

            dataGridView1.Rows.Clear();
                                // Ask Twitch's API nicely if we can see who our user is following
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/followed?oauth_token="+oauthToken);
            JObject jo;

            try {
                string responseText;
                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {   // Convert Twitch's response into a string.
                   StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                   responseText = reader.ReadToEnd();
                   jo = JsonConvert.DeserializeObject<JObject>(responseText);
                }
                JArray streams = (JArray)jo["streams"];
                foreach (JObject o in streams)
                {
                    StreamObject so = new StreamObject(o["channel"]["display_name"].ToString(),
                                                        o["game"].ToString(),
                                                        unescape(o["channel"]["status"].ToString()));
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
                
                valid = true;
            }
            catch (WebException e)
            {
                if (e.Message.Contains("401"))
                {
                    valid = false;
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
            livestreamer.StartInfo.FileName = @".\livestreamer\livestreamer.exe";
            if (checkBox2.Checked)
            {
                //http://www.twitch.tv/{Streamer}/chat
                Form chat = new ChatWindow("http://www.twitch.tv/" + Streamer + "/chat");
                chat.Closed += new EventHandler(context.OnFormClosed); // Add event handlers so that everything closes tidily
                chat.FormClosed += new FormClosedEventHandler(context.OnFormClosed);
                chat.Show();
            }
            livestreamer.StartInfo.Arguments = @"-p MPC-HC\mpc-hc.exe twitch.tv/" + Streamer + " best";
            livestreamer.Start();
            if (checkBox1.Checked) this.Close();
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
    }
}
