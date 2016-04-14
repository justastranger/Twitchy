using System;
using Newtonsoft.Json.Linq;
using Twitchy.api;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace Twitchy.Plugins
{
    public class Hitbox : Plugin
    {
        public override string name { get; set; }
        // config[oauth] could store the OAuth token, only really accessed here
        public override JObject config { get; set; }

        public Hitbox()
        {
            name = "Hitbox";
            config = new JObject();
            config["user"] = "changethis";
            config = ConfigManager.registerPlugin(name, config);
        }

        public override TabPage getPage()
        {
            if (config["user"].ToObject<string>() == "changethis") return null;
            TabPage page = new TabPage(name);
            page.Name = name;
            DataGridView contents = new DataGridView();
            contents.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            contents.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            contents.EditMode = DataGridViewEditMode.EditProgrammatically;
            contents.Columns.Add("name", "Name");
            page.Controls.Add(contents);

            JArray following = (JArray)JObject.Parse(Http.Get(@"https://api.hitbox.tv/following/user?user_name=" + config["user"].ToObject<string>()))["following"];
            foreach (JToken obj in following)
            {
                using (DataGridViewRow row = new DataGridViewRow())
                {
                    row.CreateCells(contents, new string[] { obj["user_name"].ToObject<string>() });
                    contents.Rows.Add(row);
                }
            }

            return page;
        }

        public override string formatLivestreamer(string Streamer)
        {
            return @"hitbox.tv/" + Streamer + " best";
        }
    }
}
