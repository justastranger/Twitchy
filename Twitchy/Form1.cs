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
        public static FileStream oauth = File.Open(AppDomain.CurrentDomain.BaseDirectory + "\\oauth", FileMode.OpenOrCreate);
        public static string oauthToken;
        public static Regex RegStreamers = new Regex("\"name\":\"(.*?)\",");
        public static Regex RegGames = new Regex("\"game\":\"(.*?)\",");
        public static Regex RegTitles = new Regex("\"status\":\"(.*?)\",");

        public String Streamer;
        
        public Form1()
        {   // This is essentially Main() at this point.
            InitializeComponent();
            checkOauth();
            oauth.Flush();
            oauth.Close();
            init();
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
        }

        private static void AddText(FileStream fs, string value)
        {   // Stole this from StackOverflow somewhere, google it for credits.
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        private void init()
        {
            List<String> ParsedStreamers = new List<string>(); // Prepare lists to store names, games, and titles.
            List<String> ParsedGames = new List<string>();
            List<String> ParsedTitles = new List<string>();
            listBox1.Items.Clear();
            listBox1.Items.Add("Loading...");
                                // Ask Twitch's API nicely to see who our user is following
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
            listBox1.Items.Clear(); // Make sure that the listbox is empty before we start thinking of actually populating it
            if (ParsedGames.Count / 2 == ParsedStreamers.Count && ParsedGames.Count / 2 == ParsedTitles.Count)
            {
                String[] a = ParsedStreamers.ToArray(); // Temp arrays for joining in a moment.
                String[] b = ParsedGames.ToArray();
                String[] c = ParsedTitles.ToArray();

                for (int x = 0; x < a.Length; x++)
                {
                    string temp = a[x] + "," + b[x] + "," + c[x]; // Will replace that soon with something allowing multiple columns
                    listBox1.Items.Add(temp); // Populate the list
                }
            }
            else
            {
                listBox1.Items.Add("Twitch API Response is malformed."); // The numbers don't match up.
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Streamer = listBox1.SelectedItem.ToString().Split(',')[0].ToString(); // Grabs the name of the streamer, since it's always the first thing before the first comma.
        }

        private void button2_Click(object sender, EventArgs e)
        {
            init();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process livestreamer = new Process();
            livestreamer.StartInfo.FileName = ".\\livestreamer\\livestreamer.exe";
            if(Streamer != null){
                livestreamer.StartInfo.Arguments = "-p MPC-HC\\mpc-hc.exe twitch.tv/" + Streamer + " best";
                livestreamer.Start();
            }
        }
    }
}
