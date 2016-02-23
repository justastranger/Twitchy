using CefSharp;
using System;
using System.Windows.Forms;

namespace Twitchy
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Cef.Initialize(new CefSettings(), shutdownOnProcessExit: true, performDependencyCheck: true);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new context());
        }
    }

    public class context : ApplicationContext
    {
        public static Form twitchy;
        public static int formCount = 0;

        public static void twitcheroo(){
            twitchy = new MainForm();
            twitchy.Show();
        }

        public context(){
            twitcheroo();
        }

        public static void OnFormClosed(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count == 0)
            {
                Application.ExitThread();
            }
        }
    }


    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 150;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.Text = caption;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            Label textLabel = new Label() { Left = 15, Top = 20, Width = 450, Text = text };
            TextBox textBox = new TextBox() { Left = 15, Top = 50, Width = 450 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.ShowDialog();
            return textBox.Text;
        }
    }
}
