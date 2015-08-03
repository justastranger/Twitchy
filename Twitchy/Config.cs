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

        public static bool closeAfterLaunch = false;
        public static bool openChatWindow = false;
        public static bool usePath = false;

        static Config()
        {
            if (configFile.Length == 0)
            {
                using (StreamWriter sw = new StreamWriter(configFile))
                {
                    config["closeAfterLaunch"] = closeAfterLaunch;
                    config["openChatWindow"] = openChatWindow;
                    config["usePath"] = usePath;
                    sw.Write(config.ToString(Formatting.Indented));
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader(configFile))
                {
                    config = JObject.Parse(sr.ReadToEnd());
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
            InitializeComponent();

            closeAfterLaunchCheckBox.Checked = closeAfterLaunch;
            openChatWindowCheckBox.Checked = openChatWindow;
            usePathCheckBox.Checked = usePath;

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
            this.Close();
        }
    }
}
