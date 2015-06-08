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

namespace Twitchy
{
    
    public partial class Form1 : Form
    {
        // FileStream will fail and the app will crash if we don't have permissions
        public static FileStream oauth = File.Open(AppDomain.CurrentDomain.BaseDirectory + "\\oauth", FileMode.OpenOrCreate);
        public static string oauthToken;
        public static bool valid = true;

        public String Streamer;
        
        public Form1()
        {   // This is essentially Main() at this point.
            // Split everything into smaller tasks
            InitializeComponent();
            init();
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
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

        List<string> unescape(List<string> toUnescape)
        {
            List<string> unescaped = new List<string>();
            foreach (string s in toUnescape)
            {
                unescaped.Add(Regex.Unescape(s));
            }
            return unescaped;
        }


        private void init() 
        {
            checkOauth();
            Regex RegStreamers = new Regex("\"name\":\"(.*?)\",");
            Regex RegGames = new Regex("\"game\":\"(.*?)\",");
            Regex RegTitles = new Regex("\"status\":\"(.*?)\",");

            List<String> ParsedStreamers = new List<string>(); // Prepare lists to store names, games, and titles.
            List<String> ParsedGames = new List<string>();
            List<String> ParsedTitles = new List<string>();

            dataGridView1.Rows.Clear();
                                // Ask Twitch's API nicely if we can see who our user is following
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/followed?oauth_token="+oauthToken);
            
            try {
                string responseText;
                WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {   // Convert Twitch's response into a string.
                   StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                   responseText = reader.ReadToEnd();
                }
                foreach (Match a in RegStreamers.Matches(responseText.ToString()))
                {
                    ParsedStreamers.Add(a.Groups[1].ToString()); // regex the response looking for and populating the lists with streamer names
                }
                foreach (Match a in RegGames.Matches(responseText.ToString()))
                {
                    ParsedGames.Add(a.Groups[1].ToString());     // Games
                }
                foreach (Match a in RegTitles.Matches(responseText.ToString()))
                {
                    ParsedTitles.Add(a.Groups[1].ToString());    // and lame-ass titles
                }
                ParsedTitles = unescape(ParsedTitles);  // Unescaoe the titles so you can see the fancy unicode stuff people come up with.
                if (ParsedGames.Count / 2 == ParsedStreamers.Count && ParsedGames.Count / 2 == ParsedTitles.Count)
                {
                    using (var ps = ParsedStreamers.GetEnumerator())
                    using (var pg = ParsedGames.GetEnumerator())
                    using (var pt = ParsedTitles.GetEnumerator())
                    {
                        while (ps.MoveNext() && pg.MoveNext() && pt.MoveNext())
                        {   // Skip ever other game name, since I can't be arsed to deduplicate.
                            pg.MoveNext();
                            using (DataGridViewRow row = new DataGridViewRow())
                            {   // Populate the DataGridView.
                                row.CreateCells(dataGridView1, new string[] { ps.Current, pg.Current, pt.Current });
                                dataGridView1.Rows.Add(row);
                            }
                        }
                    }
                }
                else
                {
                    using (DataGridViewRow a = new DataGridViewRow())
                    {   // Complain about Twitch's api response not being constant
                        a.CreateCells(dataGridView1, "Twitch API Response is malformed.");
                        dataGridView1.Rows.Add(a);
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
            }
            
            
            
        }

        private void button2_Click(object sender, EventArgs e)
        {   // Refresh
            init();
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
                Form chat = new Form2("http://www.twitch.tv/" + Streamer + "/chat");
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
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {   // Event handler to make sure everything closes tidily.
            context.OnFormClosed(sender, e);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            Streamer = (dataGridView1.CurrentRow.Cells[0].Value.ToString());
        }
    }
}
