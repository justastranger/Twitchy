using CefSharp;
using CefSharp.WinForms;
using System;
using System.Windows.Forms;

namespace Twitchy
{
    public partial class AuthorizationWindow : Form
    {
        // TODO find out why this just doesn't work
        // for some reason it freezes when opened

        public ChromiumWebBrowser browser;
        
        public AuthorizationWindow(string url)
        {
            InitializeComponent();
            initChrome(url);
            this.Show();
        }

        void initChrome(string url)
        {
            if (!Cef.IsInitialized)
            {
                Cef.Initialize(new CefSettings());
            }
            browser = new ChromiumWebBrowser(url);
            browser.AddressChanged += new EventHandler<CefSharp.AddressChangedEventArgs>(addressChanged);
            Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            context.OnFormClosed(sender, e);
            Cef.Shutdown();
        }

        private void addressChanged(object sender, AddressChangedEventArgs e)
        {
            if (e.Address.IndexOf("access_token") > -1)
            {
                Config.setOauth(e.Address.Substring(e.Address.IndexOf("=") + 1, 30));
                // Hide the form
                Invoke(new Action(Hide));
                // Pretend that we actually closed it since actually doing so crashes?
                context.OnFormClosed(sender, e);
            }

        }

    }
}
