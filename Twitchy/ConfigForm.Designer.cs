namespace Twitchy
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.configTabs = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // configTabs
            // 
            this.configTabs.Location = new System.Drawing.Point(12, 12);
            this.configTabs.Name = "configTabs";
            this.configTabs.SelectedIndex = 0;
            this.configTabs.Size = new System.Drawing.Size(260, 237);
            this.configTabs.TabIndex = 0;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.configTabs);
            this.Name = "ConfigForm";
            this.Text = "ConfigForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl configTabs;
    }
}