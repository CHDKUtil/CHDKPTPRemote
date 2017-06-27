namespace chdk_ptp_test
{
    partial class ScriptControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.outputlabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.execbutton = new System.Windows.Forms.Button();
            this.scriptedit = new System.Windows.Forms.TextBox();
            this.propertygrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // outputlabel
            // 
            this.outputlabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputlabel.AutoSize = true;
            this.outputlabel.Location = new System.Drawing.Point(67, 27);
            this.outputlabel.Name = "outputlabel";
            this.outputlabel.Size = new System.Drawing.Size(0, 13);
            this.outputlabel.TabIndex = 25;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-3, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Script output:";
            // 
            // execbutton
            // 
            this.execbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.execbutton.Location = new System.Drawing.Point(525, 0);
            this.execbutton.Name = "execbutton";
            this.execbutton.Size = new System.Drawing.Size(75, 23);
            this.execbutton.TabIndex = 23;
            this.execbutton.Text = "Execute";
            this.execbutton.UseVisualStyleBackColor = true;
            this.execbutton.Click += new System.EventHandler(this.execbutton_Click);
            // 
            // scriptedit
            // 
            this.scriptedit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scriptedit.Location = new System.Drawing.Point(0, 0);
            this.scriptedit.Name = "scriptedit";
            this.scriptedit.Size = new System.Drawing.Size(519, 20);
            this.scriptedit.TabIndex = 22;
            this.scriptedit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.scriptedit_KeyPress);
            // 
            // propertygrid
            // 
            this.propertygrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertygrid.CommandsVisibleIfAvailable = false;
            this.propertygrid.HelpVisible = false;
            this.propertygrid.Location = new System.Drawing.Point(0, 43);
            this.propertygrid.Name = "propertygrid";
            this.propertygrid.Size = new System.Drawing.Size(597, 354);
            this.propertygrid.TabIndex = 21;
            this.propertygrid.Visible = false;
            // 
            // ScriptControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outputlabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.execbutton);
            this.Controls.Add(this.scriptedit);
            this.Controls.Add(this.propertygrid);
            this.Name = "ScriptControl";
            this.Size = new System.Drawing.Size(600, 400);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label outputlabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button execbutton;
        private System.Windows.Forms.TextBox scriptedit;
        private System.Windows.Forms.PropertyGrid propertygrid;
    }
}
