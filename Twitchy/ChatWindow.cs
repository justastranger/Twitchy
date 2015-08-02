using System;
using System.Windows.Forms;

namespace Twitchy
{
    public partial class ChatWindow : Form
    {
        private string url;

        public static void ShowChat(string url){
            //http://www.twitch.tv/{Streamer}/chat
            Form chat = new ChatWindow("http://www.twitch.tv/" + url + "/chat");
            chat.Closed += new EventHandler(context.OnFormClosed); // Add event handlers so that everything closes tidily
            chat.FormClosed += new FormClosedEventHandler(context.OnFormClosed);
            chat.Show();
        }

        public ChatWindow(string url)
        {
            this.url = url;
            InitializeComponent();
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            webBrowser1.Navigate(this.url);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            context.twitcheroo();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            context.OnFormClosed(sender, e);
        }
    }
}
