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

namespace Twitchy
{
    
    public partial class Form1 : Form
    {
        // FileStream will fail and the app will crash if we don't have permissions
        public static FileStream oauth = File.Open(AppDomain.CurrentDomain.BaseDirectory + "\\oauth", FileMode.OpenOrCreate);
        public static string oauthToken;

        public String Streamer;
        
        public Form1()
        {   // This is essentially Main() at this point.
            // Split everything into smaller tasks
            InitializeComponent();
            checkOauth();
            init();
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private static void checkOauth()
        {
            if (new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\oauth").Length == 0)
            {
                oauthToken = Twitchy.Prompt.ShowDialog("Please enter your Oauth key.", "No OAuth Saved");
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

        private static void AddText(FileStream fs, string value)
        {   // Stole this from StackOverflow somewhere, google it for credits.
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        private void init()
        {
            Regex RegStreamers = new Regex("\"name\":\"(.*?)\",");
            Regex RegGames = new Regex("\"game\":\"(.*?)\",");
            Regex RegTitles = new Regex("\"status\":\"(.*?)\",");

            List<String> ParsedStreamers = new List<string>(); // Prepare lists to store names, games, and titles.
            List<String> ParsedGames = new List<string>();
            List<String> ParsedTitles = new List<string>();

            dataGridView1.Rows.Clear();
                                // Ask Twitch's API nicely if we can see who our user is following
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/followed?oauth_token="+oauthToken);
            WebResponse response = request.GetResponse();
            string responseText;
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                responseText = reader.ReadToEnd();
            }
            foreach (Match a in RegStreamers.Matches(responseText.ToString()))
            {
                ParsedStreamers.Add(a.Groups[1].ToString()); // regex the response looking for streamer names
            }
            foreach (Match a in RegGames.Matches(responseText.ToString()))
            {
                ParsedGames.Add(a.Groups[1].ToString());     // Games
            }
            foreach (Match a in RegTitles.Matches(responseText.ToString()))
            {
                ParsedTitles.Add(a.Groups[1].ToString());    // and lame-ass titles
            }
            // TODO add the ChrW nonsense for unescaping unicode characters
            if (ParsedGames.Count / 2 == ParsedStreamers.Count && ParsedGames.Count / 2 == ParsedTitles.Count)
            {
                using(var ps = ParsedStreamers.GetEnumerator())
                using(var pg = ParsedGames.GetEnumerator())
                using(var pt = ParsedTitles.GetEnumerator()){
                    while (ps.MoveNext() && pg.MoveNext() && pt.MoveNext())
                    {
                        pg.MoveNext();
                        //string temp = ps.Current + "," + pg.Current + "," + pt.Current; // Will replace that soon with something allowing multiple columns
                        //listBox1.Items.Add(temp); // Populate the list
                        using (DataGridViewRow row = new DataGridViewRow())
                        {
                            //row.SetValues(new string[] { ps.Current, pg.Current, pt.Current });
                            row.CreateCells(dataGridView1, new string[] { ps.Current, pg.Current, pt.Current });
                            dataGridView1.Rows.Add(row);
                        }
                    }
                }
            }
            else
            {
                using (DataGridViewRow a = new DataGridViewRow())
                {
                    a.CreateCells(dataGridView1, "Twitch API Response is malformed.");
                    dataGridView1.Rows.Add(a);
                }
                //listBox1.Items.Add("Twitch API Response is malformed."); // The numbers don't match up.
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {   // Refresh
            init();
        }

        private void button1_Click(object sender, EventArgs e)
        {   // Start Stream
            if (Streamer == null)
            {
                MessageBox.Show("Select a streamer.");
            }
            Process livestreamer = new Process();
            livestreamer.StartInfo.FileName = ".\\livestreamer\\livestreamer.exe";
            if(Streamer != null){
                if (checkBox2.Checked)
                {
                    //http://www.twitch.tv/{Streamer}/chat
                    new Form2("http://www.twitch.tv/" + Streamer + "/chat").Show();
                }
                livestreamer.StartInfo.Arguments = "-p MPC-HC\\mpc-hc.exe twitch.tv/" + Streamer + " best";
                livestreamer.Start();
                if (checkBox1.Checked) Environment.Exit(1);
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button1_Click(sender, e);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (dataGridView1.CurrentCell != null)
            Streamer = (dataGridView1.CurrentRow.Cells[0].Value.ToString());
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
    }
}
