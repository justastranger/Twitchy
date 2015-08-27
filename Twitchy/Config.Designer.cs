namespace Twitchy
{
    partial class Config
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Config));
            this.closeAfterLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.openChatWindowCheckBox = new System.Windows.Forms.CheckBox();
            this.usePathCheckBox = new System.Windows.Forms.CheckBox();
            this.Save = new System.Windows.Forms.Button();
            this.usePathToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.disableTitleUnescapingCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // closeAfterLaunchCheckBox
            // 
            this.closeAfterLaunchCheckBox.AutoSize = true;
            this.closeAfterLaunchCheckBox.Location = new System.Drawing.Point(12, 12);
            this.closeAfterLaunchCheckBox.Name = "closeAfterLaunchCheckBox";
            this.closeAfterLaunchCheckBox.Size = new System.Drawing.Size(208, 17);
            this.closeAfterLaunchCheckBox.TabIndex = 0;
            this.closeAfterLaunchCheckBox.Text = "Close Twitchy after launching a stream";
            this.closeAfterLaunchCheckBox.UseVisualStyleBackColor = true;
            this.closeAfterLaunchCheckBox.CheckedChanged += new System.EventHandler(this.closeAfterLaunchCheckBox_CheckedChanged);
            // 
            // openChatWindowCheckBox
            // 
            this.openChatWindowCheckBox.AutoSize = true;
            this.openChatWindowCheckBox.Location = new System.Drawing.Point(12, 35);
            this.openChatWindowCheckBox.Name = "openChatWindowCheckBox";
            this.openChatWindowCheckBox.Size = new System.Drawing.Size(227, 17);
            this.openChatWindowCheckBox.TabIndex = 1;
            this.openChatWindowCheckBox.Text = "Open a chat window along with the stream";
            this.openChatWindowCheckBox.UseVisualStyleBackColor = true;
            this.openChatWindowCheckBox.CheckedChanged += new System.EventHandler(this.openChatWindowCheckBox_CheckedChanged);
            // 
            // usePathCheckBox
            // 
            this.usePathCheckBox.AutoSize = true;
            this.usePathCheckBox.Location = new System.Drawing.Point(12, 58);
            this.usePathCheckBox.Name = "usePathCheckBox";
            this.usePathCheckBox.Size = new System.Drawing.Size(117, 17);
            this.usePathCheckBox.TabIndex = 2;
            this.usePathCheckBox.Text = "Use PATH defaults";
            this.usePathToolTip.SetToolTip(this.usePathCheckBox, "Check this to use a pre-installed copy of livestreamer and a compatible video pla" +
        "yer.");
            this.usePathCheckBox.UseVisualStyleBackColor = true;
            this.usePathCheckBox.CheckedChanged += new System.EventHandler(this.usePathCheckBox_CheckedChanged);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(188, 211);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(84, 38);
            this.Save.TabIndex = 3;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // disableTitleUnescapingCheckbox
            // 
            this.disableTitleUnescapingCheckbox.AutoSize = true;
            this.disableTitleUnescapingCheckbox.Location = new System.Drawing.Point(12, 81);
            this.disableTitleUnescapingCheckbox.Name = "disableTitleUnescapingCheckbox";
            this.disableTitleUnescapingCheckbox.Size = new System.Drawing.Size(144, 17);
            this.disableTitleUnescapingCheckbox.TabIndex = 4;
            this.disableTitleUnescapingCheckbox.Text = "Disable Title Unescaping";
            this.disableTitleUnescapingCheckbox.UseVisualStyleBackColor = true;
            this.disableTitleUnescapingCheckbox.CheckedChanged += new System.EventHandler(this.disableTitleUnescapingCheckbox_CheckedChanged);
            // 
            // Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.disableTitleUnescapingCheckbox);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.usePathCheckBox);
            this.Controls.Add(this.openChatWindowCheckBox);
            this.Controls.Add(this.closeAfterLaunchCheckBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Config";
            this.Text = "Twitchy Config";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox closeAfterLaunchCheckBox;
        private System.Windows.Forms.CheckBox openChatWindowCheckBox;
        private System.Windows.Forms.CheckBox usePathCheckBox;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.ToolTip usePathToolTip;
        private System.Windows.Forms.CheckBox disableTitleUnescapingCheckbox;
    }
}