using CefSharp;
using CefSharp.WinForms;
using System;
using System.Windows.Forms;

namespace Twitchy
{
    public partial class AuthorizationWindow : Form
    {
        public ChromiumWebBrowser browser;
        
        public AuthorizationWindow(string url)
        {
            browser = new ChromiumWebBrowser("about:blank");
            InitializeComponent();
            Controls.Add(browser);
            browser.AddressChanged += new EventHandler<CefSharp.AddressChangedEventArgs>(addressChanged);
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

        private void addressChanged(object sender, AddressChangedEventArgs e)
        {
            if (e.Address.IndexOf("access_token") > -1)
            {
                Config.setOauth(e.Address.Substring(e.Address.IndexOf("=") + 1, 30));
                this.Hide();
                //MessageBox.Show(e.Address.Substring(e.Address.IndexOf("=")+1,30));
            }

        }

    }
}
