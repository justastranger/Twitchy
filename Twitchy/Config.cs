using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Twitchy
{
    public partial class Config : Form
    {
        public static bool closeAfterLaunch = false;
        public static bool openChatWindow = false;
        public static bool usePath = false;

        public Config()
        {
            InitializeComponent();

            closeAfterLaunchCheckBox.Checked = closeAfterLaunch;
            openChatWindowCheckBox.Checked = openChatWindow;
            usePathCheckBox.Checked = usePath;

        }

        private void closeAfterLaunchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            closeAfterLaunch = closeAfterLaunchCheckBox.Checked;
        }

        private void openChatWindowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            openChatWindow = openChatWindowCheckBox.Checked;
        }

        private void usePathCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            usePath = usePathCheckBox.Checked;
        }
    }
}
