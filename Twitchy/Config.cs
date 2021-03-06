﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Twitchy
{
    public partial class Config : Form
    {
        // Soemthing something cross-platform
        private static char slash = System.IO.Path.DirectorySeparatorChar;
        // Determine the absolute path of the config file
        private static string configPath = AppDomain.CurrentDomain.BaseDirectory + slash + "config.json";
        // Create a file to load/save config settings from/to
        private static FileStream configFile = File.Open(configPath, FileMode.OpenOrCreate);
        // Create an empty Object to store config settings in and access from elsewhere
        public static JObject config = new JObject();

        // OAuth token to use with the Twitch API
        public static string oauthToken;
        // If an API response comes back invalid, this becomes false
        // The next time the token is checked (before every refresh),
        //   the user will be prompted for a valid token
        public static bool valid = true;
        
        // Initialize our config options
        static Config()
        {
            // Prepare our JObject with all the current config options
            //   all of which default to false
            config["closeAfterLaunch"] = false;
            // Disabled until I can find a solution that a.) works, b.) doesn't leak memory like a bottomless bucket
            config["openChatWindow"] = false;
            config["disableTitleUnescaping"] = false;
            config["useCustomLivestreamer"] = false;
            config["livestreamer"] = @".\streamlink\Streamlink.exe";
            config["useCustomPlayer"] = false;
            config["player"] = @".\MPC-HC\mpc-hc.exe";
            config["minimizeToTaskbar"] = true;
            config["showConsole"] = false;
            
            // If our config file doesn't already exist
            if (configFile.Length == 0)
            {
                // Create it
                using (StreamWriter sw = new StreamWriter(configFile))
                {
                    checkOauth();
                    sw.Write(config.ToString(Formatting.Indented));
                }
            }
            else // Otherwise
            {
                // Read it into a temporary object
                JObject tmp;
                using (StreamReader sr = new StreamReader(configFile))
                {
                    // Assign the object
                    tmp = JObject.Parse(sr.ReadToEnd());
                }
                // And then assign the values of the tmp object to our actual object
                foreach (KeyValuePair<String, JToken> kvp in tmp)
                {
                    config[kvp.Key] = kvp.Value;
                }
                // Finally, we save the final version of our config object which includes
                //   all the defaults for options not found in the previous version of 
                //   the file
                // First, re-create the FileStream that was closed by closing the StreamReader
                configFile = File.Open(configPath, FileMode.Open);
                // Then create a StreamWriter
                using (StreamWriter sw = new StreamWriter(configFile))
                {
                    // Save the file
                    sw.Write(config.ToString(Formatting.Indented));
                }
                // make sure that we have an OAuth token
                checkOauth();
            }
        }

        // The constructor doesn't actually affect any of the config settings,
        //   it just reads them and checks any checkboxes that were already checked
        public Config()
        {
            // These are for form-counting so the process only terminates when all forms
            //   are closed, it's mainly aimed at the chat windows, but is applied to all forms
            this.FormClosed += new FormClosedEventHandler(context.OnFormClosed);
            this.Closed += new EventHandler(context.OnFormClosed);
            // This saves all the config options when the form is closed
            this.Closed += new EventHandler(Save_Click);
            InitializeComponent();

            // Initialize the checkboxes
            closeAfterLaunchCheckBox.Checked = config["closeAfterLaunch"].ToObject<bool>();
            openChatWindowCheckBox.Checked = config["openChatWindow"].ToObject<bool>();
            disableTitleUnescapingCheckbox.Checked = config["disableTitleUnescaping"].ToObject<bool>();
            useCustomLivestreamerCheckBox.Checked = config["useCustomLivestreamer"].ToObject<bool>();
            useCustomPlayerCheckBox.Checked = config["useCustomPlayer"].ToObject<bool>();
            minimizeToTaskbarCheckbox.Checked = config["minimizeToTaskbar"].ToObject<bool>();
            showConsoleCheckBox.Checked = config["showConsole"].ToObject<bool>();
        }

        private static void AddText(FileStream fs, string value)
        {   // Stole this from StackOverflow somewhere, google it for credits.
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        public static void checkOauth()
        {   // If we've marked the oauth token as invalid, null the config option for it
            if (!valid) config["oauth"] = null;
            // If the config option for the oauth token is null, send users to the authorization page.
            if (config["oauth"] == null)
            {
                //config["oauth"] = Twitchy.Prompt.ShowDialog(@"Please enter your OAuth token, you can generate one at http://www.twitchapps.com/tmi/", "No OAuth Saved");
                //AuthorizationWindow bw = new AuthorizationWindow("https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id=bdz2mqmz5jcpihdfjic2ofbpew6xzcy&redirect_uri=http://localhost&scope=user_read");
                //bw.Show();

            }
        }

        public static void setOauth(string newToken)
        {
            config["oauth"] = newToken;
            // OVerwrite the previous config file
            configFile = File.Open(configPath, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(configFile))
            {   // and save our config object to it
                sw.Write(config.ToString());
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {   // OVerwrite the previous config file
            configFile = File.Open(configPath, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(configFile))
            {   // and save our config object to it
                sw.Write(config.ToString());
            }
            // And if this function is called by button press 
            //   instead of an event handler, close the form
            if (sender != this) { this.Close(); }
        }

        // TODO make this next section nicer

        private void closeAfterLaunchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            config["closeAfterLaunch"] = closeAfterLaunchCheckBox.Checked;
        }

        private void openChatWindowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            config["openChatWindow"] = openChatWindowCheckBox.Checked;
        }

        private void disableTitleUnescapingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            config["disableTitleUnescaping"] = disableTitleUnescapingCheckbox.Checked;
        }

        private void useCustomLivestreamerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            config["useCustomLivestreamer"] = useCustomLivestreamerCheckBox.Checked;
        }

        private void useCustomPlayerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            config["useCustomPlayer"] = useCustomPlayerCheckBox.Checked;
        }

        private void minimizeToTaskbarCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            config["minimizeToTaskbar"] = minimizeToTaskbarCheckbox.Checked;
        }

        private void showConsoleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            config["showConsole"] = showConsoleCheckBox.Checked;
        }

        private void setLivestreamerButton_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            config["livestreamer"] = fd.FileName;
        }

        private void setPlayerButton_Click(object sender, EventArgs e)
        {
            FileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            config["player"] = fd.FileName;
        }
    }
}
