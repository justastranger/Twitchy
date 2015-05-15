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
        public FileStream oauth = File.Open(AppDomain.CurrentDomain.BaseDirectory + "\\oauth", FileMode.OpenOrCreate);
        public string oauthToken;
        public Regex RegStreamers = new Regex("\"name\":\"(.*?)\",");
        public Regex RegGames = new Regex("\"game\":\"(.*?)\",");
        public Regex RegTitles = new Regex("\"status\":\"(.*?)\",");

        public String Streamer;
        
        public Form1()
        {
            InitializeComponent();
            if (new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\oauth").Length == 0)
            {
                oauthToken = Twitchy.Prompt.ShowDialog("Please enter your Oauth key.", "No OAuth Saved");
                AddText(oauth, oauthToken);
            }
            else
            {
                using( StreamReader reader = new StreamReader(oauth)){
                    oauthToken = reader.ReadToEnd();
                }
            }
            oauth.Close();
            init();
        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        private void init()
        {
            List<String> ParsedStreamers = new List<string>();
            List<String> ParsedGames = new List<string>();
            List<String> ParsedTitles = new List<string>();
            listBox1.Items.Clear();
            listBox1.Items.Add("Loading...");
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
                ParsedStreamers.Add(a.Groups[1].ToString());
            }
            foreach (Match a in RegGames.Matches(responseText.ToString()))
            {
                ParsedGames.Add(a.Groups[1].ToString());
            }
            foreach (Match a in RegTitles.Matches(responseText.ToString()))
            {
                ParsedTitles.Add(a.Groups[1].ToString());
            }
            //MessageBox.Show(ParsedGames.Count.ToString() + ParsedStreamers.Count.ToString() + ParsedTitles.Count.ToString());
            listBox1.Items.Clear();
            if (ParsedGames.Count / 2 == ParsedStreamers.Count && ParsedGames.Count / 2 == ParsedTitles.Count)
            {
                String[] a = ParsedStreamers.ToArray();
                String[] b = ParsedGames.ToArray();
                String[] c = ParsedTitles.ToArray();

                for (int x = 0; x < ParsedStreamers.ToArray().Length; x++)
                {
                    string temp = a[x] + "," + b[x] + "," + c[x];
                    listBox1.Items.Add(temp);
                }
            }
            else
            {
                listBox1.Items.Add("Twitch API Response is malformed.");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Streamer = listBox1.SelectedItem.ToString().Split(',')[0].ToString();
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
                //MessageBox.Show(Streamer);
            }
        }
    }
}
