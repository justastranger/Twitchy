using System;
using Newtonsoft.Json.Linq;
using Twitchy.api;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Twitchy.Plugins
{
    class Twitch : Plugin
    {
        
        private List<StreamObject> ParsedStreams = new List<StreamObject>();

        public override string name { get; set; }
        // config[oauth] could store the OAuth token, only really accessed here
        public override JObject config { get; set; }

        public Twitch()
        {
            name = "Twitch";
            config = new JObject();
            config["oauth"] = "changethis";
            config["disableTitleUnescaping"] = false;
            config = ConfigManager.registerPlugin(name, config);
        }

        public override string formatLivestreamer(string Streamer)
        {
            return @"twitch.tv/"+Streamer+" best";
        }

        public override TabPage getPage()
        {
            if (config["oauth"].ToObject<string>() == "changethis") return null;
            TabPage page = new TabPage();
            page.Name = this.name;
            DataGridView contents = new DataGridView();
            contents.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            contents.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            contents.EditMode = DataGridViewEditMode.EditProgrammatically;
            contents.Columns.Add("name", "Name");
            contents.Columns.Add("game", "Game");
            contents.Columns.Add("title", "Title");
            page.Controls.Add(contents);
            if(fillDGV(contents))
            {
                contents.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                return page;
            } else {
                return null;
            }
        }

        private bool fillDGV(DataGridView dgv)
        {
            ParsedStreams = new List<StreamObject>();
            // Ask Twitch's API nicely if we can see who our user is following
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/followed?oauth_token=" + config["oauth"].ToObject<string>());
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
                                        : config["disableTitleUnescaping"].ToObject<bool>() ? o["channel"]["status"].ToString()
                                        // Finally, if all else fails, use the unescaped title
                                        : unescape(o["channel"]["status"].ToString());
                    using (DataGridViewRow row = new DataGridViewRow())
                    {   // Populate the DataGridView.
                        row.CreateCells(dgv, new string[] { o["channel"]["display_name"].ToString(), o["game"].ToString(), title });
                        dgv.Rows.Add(row);
                    }
                }
                return true;
            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("401"))
                {
                    using (DataGridViewRow a = new DataGridViewRow())
                    {   // Complain about the invalid OAuth token.
                        a.CreateCells(dgv, new string[] { "Invalid OAuth Token.", "Please press refresh." });
                        dgv.Rows.Add(a);
                    }
                    return false;
                }
                else
                {
                    using (DataGridViewRow a = new DataGridViewRow())
                    {   // Complain about the lack of internet
                        a.CreateCells(dgv, new string[] { "Connection Issue: " + ex.Status, "Twitch couldn't be reached,", "Check your connection and whether or not Twitch is up." });
                        dgv.Rows.Add(a);
                    }
                    return true;
                }
            }
            catch (ArgumentException ex)
            {
                using (DataGridViewRow a = new DataGridViewRow())
                {   // Complain about the lack of internet
                    a.CreateCells(dgv, new string[] { "Someone is doing something", "interesting with their title.", "Disable unescaping in the config and refresh to resolve this problem." });
                    dgv.Rows.Add(a);
                }
                return true;
            }
            catch (Exception ex)
            {
                using (DataGridViewRow a = new DataGridViewRow())
                {   // Complain about the lack of internet
                    a.CreateCells(dgv, new string[] { "There was a problem", ex.Message, "Pressing Refresh some more will probably fix it." });
                    dgv.Rows.Add(a);
                }
                return true;
            }
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

    }

    class StreamObject
    {
        public string name { get; set; }
        public string game { get; set; }
        public string title { get; set; }


        public StreamObject(string name, string game, string title)
        {
            this.name = name;
            this.game = game;
            this.title = title;
        }
    }
}
