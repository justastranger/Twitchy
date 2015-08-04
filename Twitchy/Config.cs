using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Twitchy
{
    public partial class Config : Form
    {
        private static char slash = System.IO.Path.DirectorySeparatorChar;
        private static string configPath = AppDomain.CurrentDomain.BaseDirectory + slash + "config.json";
        private static FileStream configFile = File.Open(configPath, FileMode.OpenOrCreate);
        private static JObject config = new JObject();

        //private static string oauthPath = AppDomain.CurrentDomain.BaseDirectory + slash + "oauth";
        // FileStream will fail and the app will crash if we don't have permissions
        //private static FileStream oauth = File.Open(oauthPath, FileMode.OpenOrCreate);
        public static string oauthToken;
        public static bool valid = true;

        public static bool closeAfterLaunch = false;
        public static bool openChatWindow = false;
        public static bool usePath = false;

        static Config()
        {
            if (configFile.Length == 0)
            {
                using (StreamWriter sw = new StreamWriter(configFile))
                {   // Prepare our JObject to be saved
                    config["closeAfterLaunch"] = closeAfterLaunch;
                    config["openChatWindow"] = openChatWindow;
                    config["usePath"] = usePath;
                    checkOauth(); // Make sure we have an OAuth token before we try to save it
                    config["oauth"] = oauthToken;
                    sw.Write(config.ToString(Formatting.Indented)); // Actually save it
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader(configFile))
                {   // Read an existing config file
                    config = JObject.Parse(sr.ReadToEnd());
                    oauthToken = config["oauth"].ToObject<string>();
                    closeAfterLaunch = config["closeAfterLaunch"].ToObject<bool>();
                    openChatWindow = config["openChatWindow"].ToObject<bool>();
                    usePath = config["usePath"].ToObject<bool>();
                }
            }
        }

        public Config()
        {
            this.FormClosed += new FormClosedEventHandler(context.OnFormClosed);
            this.Closed += new EventHandler(context.OnFormClosed);
            this.Closed += new EventHandler(Save_Click);
            InitializeComponent();

            closeAfterLaunchCheckBox.Checked = closeAfterLaunch;
            openChatWindowCheckBox.Checked = openChatWindow;
            usePathCheckBox.Checked = usePath;

        }

        private static void AddText(FileStream fs, string value)
        {   // Stole this from StackOverflow somewhere, google it for credits.
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        public static void checkOauth()
        {
            if (!valid) oauthToken = null;

            if (oauthToken == null)
            {
                oauthToken = Twitchy.Prompt.ShowDialog(@"Please enter your Oauth key, you can generate one at http://www.twitchapps.com/tmi/", "No OAuth Saved");
                config["oauth"] = oauthToken;
            }
        }

        private void closeAfterLaunchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            closeAfterLaunch = closeAfterLaunchCheckBox.Checked;
            config["closeAfterLaunch"] = closeAfterLaunch;
        }

        private void openChatWindowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            openChatWindow = openChatWindowCheckBox.Checked;
            config["openChatWindow"] = openChatWindow;
        }

        private void usePathCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            usePath = usePathCheckBox.Checked;
            config["usePath"] = usePath;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            configFile = File.Open(configPath, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(configFile))
            {
                sw.Write(config.ToString());
            }
            if (sender != this) { this.Close(); }
        }
    }
}
