using CefSharp.WinForms;
using System;
using System.Windows.Forms;

namespace Twitchy
{
    public partial class ChatWindow : Form
    {
        private ChromiumWebBrowser browser;
        
        public static void ShowChat(string streamer){
            //http://www.twitch.tv/{Streamer}/chat
            Form chat = new ChatWindow("http://www.twitch.tv/" + streamer + "/chat");
            chat.Closed += new EventHandler(context.OnFormClosed); // Add event handlers so that everything closes tidily
            chat.FormClosed += new FormClosedEventHandler(context.OnFormClosed);
            chat.Show();
        }

        public ChatWindow(string url)
        {
            browser = new ChromiumWebBrowser("about:blank");
            InitializeComponent();
            Controls.Add(browser);
            browser.Load(url);
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            
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
