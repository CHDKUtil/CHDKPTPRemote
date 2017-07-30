// Copyright Muck van Weerdenburg 2011.
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

namespace chdk_ptp_test
{
    partial class Form1
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
            this.devicecombobox = new System.Windows.Forms.ComboBox();
            this.connect_button = new System.Windows.Forms.Button();
            this.refreshbutton = new System.Windows.Forms.Button();
            this.disconnectbutton = new System.Windows.Forms.Button();
            this.statuslabel = new System.Windows.Forms.Label();
            this.recordbutton = new System.Windows.Forms.Button();
            this.playbackbutton = new System.Windows.Forms.Button();
            this.shutdownbutton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.rebootbutton = new System.Windows.Forms.Button();
            this.tabcontrol = new System.Windows.Forms.TabControl();
            this.scriptpage = new System.Windows.Forms.TabPage();
            this.scriptcontrol = new chdk_ptp_test.ScriptControl();
            this.tabcontrol.SuspendLayout();
            this.scriptpage.SuspendLayout();
            this.SuspendLayout();
            // 
            // devicecombobox
            // 
            this.devicecombobox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.devicecombobox.FormattingEnabled = true;
            this.devicecombobox.Location = new System.Drawing.Point(12, 12);
            this.devicecombobox.Name = "devicecombobox";
            this.devicecombobox.Size = new System.Drawing.Size(314, 21);
            this.devicecombobox.TabIndex = 0;
            // 
            // connect_button
            // 
            this.connect_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.connect_button.Location = new System.Drawing.Point(332, 10);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(75, 23);
            this.connect_button.TabIndex = 1;
            this.connect_button.Text = "Connect";
            this.connect_button.UseVisualStyleBackColor = true;
            this.connect_button.Click += new System.EventHandler(this.connectbutton_Click);
            // 
            // refreshbutton
            // 
            this.refreshbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshbutton.Location = new System.Drawing.Point(413, 10);
            this.refreshbutton.Name = "refreshbutton";
            this.refreshbutton.Size = new System.Drawing.Size(75, 23);
            this.refreshbutton.TabIndex = 2;
            this.refreshbutton.Text = "Refresh List";
            this.refreshbutton.UseVisualStyleBackColor = true;
            this.refreshbutton.Click += new System.EventHandler(this.refreshbutton_Click);
            // 
            // disconnectbutton
            // 
            this.disconnectbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.disconnectbutton.Location = new System.Drawing.Point(332, 10);
            this.disconnectbutton.Name = "disconnectbutton";
            this.disconnectbutton.Size = new System.Drawing.Size(75, 23);
            this.disconnectbutton.TabIndex = 1;
            this.disconnectbutton.Text = "Disconnect";
            this.disconnectbutton.UseVisualStyleBackColor = true;
            this.disconnectbutton.Visible = false;
            this.disconnectbutton.Click += new System.EventHandler(this.disconnectbutton_Click);
            // 
            // statuslabel
            // 
            this.statuslabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statuslabel.AutoSize = true;
            this.statuslabel.Location = new System.Drawing.Point(12, 44);
            this.statuslabel.Name = "statuslabel";
            this.statuslabel.Size = new System.Drawing.Size(78, 13);
            this.statuslabel.TabIndex = 4;
            this.statuslabel.Text = "Not connected";
            // 
            // recordbutton
            // 
            this.recordbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.recordbutton.Location = new System.Drawing.Point(332, 68);
            this.recordbutton.Name = "recordbutton";
            this.recordbutton.Size = new System.Drawing.Size(75, 23);
            this.recordbutton.TabIndex = 6;
            this.recordbutton.Text = "Record";
            this.recordbutton.UseVisualStyleBackColor = true;
            this.recordbutton.Click += new System.EventHandler(this.recordbutton_Click);
            // 
            // playbackbutton
            // 
            this.playbackbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.playbackbutton.Location = new System.Drawing.Point(413, 68);
            this.playbackbutton.Name = "playbackbutton";
            this.playbackbutton.Size = new System.Drawing.Size(75, 23);
            this.playbackbutton.TabIndex = 7;
            this.playbackbutton.Text = "Playback";
            this.playbackbutton.UseVisualStyleBackColor = true;
            this.playbackbutton.Click += new System.EventHandler(this.playbackbutton_Click);
            // 
            // shutdownbutton
            // 
            this.shutdownbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.shutdownbutton.Location = new System.Drawing.Point(413, 39);
            this.shutdownbutton.Name = "shutdownbutton";
            this.shutdownbutton.Size = new System.Drawing.Size(75, 23);
            this.shutdownbutton.TabIndex = 9;
            this.shutdownbutton.Text = "Shut Down";
            this.shutdownbutton.UseVisualStyleBackColor = true;
            this.shutdownbutton.Click += new System.EventHandler(this.shutdownbutton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Mode:";
            // 
            // rebootbutton
            // 
            this.rebootbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rebootbutton.Location = new System.Drawing.Point(332, 39);
            this.rebootbutton.Name = "rebootbutton";
            this.rebootbutton.Size = new System.Drawing.Size(75, 23);
            this.rebootbutton.TabIndex = 16;
            this.rebootbutton.Text = "Reboot";
            this.rebootbutton.UseVisualStyleBackColor = true;
            this.rebootbutton.Click += new System.EventHandler(this.rebootbutton_Click);
            // 
            // tabcontrol
            // 
            this.tabcontrol.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabcontrol.Controls.Add(this.scriptpage);
            this.tabcontrol.Location = new System.Drawing.Point(12, 97);
            this.tabcontrol.Name = "tabcontrol";
            this.tabcontrol.SelectedIndex = 0;
            this.tabcontrol.Size = new System.Drawing.Size(476, 313);
            this.tabcontrol.TabIndex = 18;
            // 
            // scriptpage
            // 
            this.scriptpage.Controls.Add(this.scriptcontrol);
            this.scriptpage.Location = new System.Drawing.Point(4, 22);
            this.scriptpage.Name = "scriptpage";
            this.scriptpage.Padding = new System.Windows.Forms.Padding(3);
            this.scriptpage.Size = new System.Drawing.Size(468, 287);
            this.scriptpage.TabIndex = 0;
            this.scriptpage.Text = "Script";
            // 
            // scriptcontrol
            // 
            this.scriptcontrol.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scriptcontrol.Connected = false;
            this.scriptcontrol.Location = new System.Drawing.Point(7, 7);
            this.scriptcontrol.Log = null;
            this.scriptcontrol.Name = "scriptcontrol";
            this.scriptcontrol.Session = null;
            this.scriptcontrol.Size = new System.Drawing.Size(455, 274);
            this.scriptcontrol.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 422);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tabcontrol);
            this.Controls.Add(this.rebootbutton);
            this.Controls.Add(this.shutdownbutton);
            this.Controls.Add(this.playbackbutton);
            this.Controls.Add(this.recordbutton);
            this.Controls.Add(this.statuslabel);
            this.Controls.Add(this.disconnectbutton);
            this.Controls.Add(this.refreshbutton);
            this.Controls.Add(this.connect_button);
            this.Controls.Add(this.devicecombobox);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "CHDK PTP Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabcontrol.ResumeLayout(false);
            this.scriptpage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox devicecombobox;
        private System.Windows.Forms.Button connect_button;
        private System.Windows.Forms.Button refreshbutton;
        private System.Windows.Forms.Button disconnectbutton;
        private System.Windows.Forms.Label statuslabel;
        private System.Windows.Forms.Button recordbutton;
        private System.Windows.Forms.Button playbackbutton;
        private System.Windows.Forms.Button shutdownbutton;
        private System.Windows.Forms.Button rebootbutton;
        private System.Windows.Forms.TabControl tabcontrol;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage scriptpage;
        private ScriptControl scriptcontrol;
    }
}

