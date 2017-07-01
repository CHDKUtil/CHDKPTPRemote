namespace chdk_ptp_test
{
    partial class PictureControl
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.savebutton = new System.Windows.Forms.Button();
            this.saveasbutton = new System.Windows.Forms.Button();
            this.saveaddbutton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.Location = new System.Drawing.Point(0, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(360, 240);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // savebutton
            // 
            this.savebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.savebutton.Location = new System.Drawing.Point(380, 0);
            this.savebutton.Name = "savebutton";
            this.savebutton.Size = new System.Drawing.Size(75, 23);
            this.savebutton.TabIndex = 3;
            this.savebutton.Text = "Save Image";
            this.savebutton.UseVisualStyleBackColor = true;
            this.savebutton.Click += new System.EventHandler(this.savebutton_Click);
            // 
            // saveasbutton
            // 
            this.saveasbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveasbutton.Location = new System.Drawing.Point(380, 29);
            this.saveasbutton.Name = "saveasbutton";
            this.saveasbutton.Size = new System.Drawing.Size(75, 23);
            this.saveasbutton.TabIndex = 4;
            this.saveasbutton.Text = "Save As...";
            this.saveasbutton.UseVisualStyleBackColor = true;
            this.saveasbutton.Click += new System.EventHandler(this.saveasbutton_Click);
            // 
            // saveaddbutton
            // 
            this.saveaddbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveaddbutton.Location = new System.Drawing.Point(380, 58);
            this.saveaddbutton.Name = "saveaddbutton";
            this.saveaddbutton.Size = new System.Drawing.Size(75, 23);
            this.saveaddbutton.TabIndex = 5;
            this.saveaddbutton.Text = "Save Add";
            this.saveaddbutton.UseVisualStyleBackColor = true;
            this.saveaddbutton.Click += new System.EventHandler(this.saveaddbutton_Click);
            // 
            // PictureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.saveaddbutton);
            this.Controls.Add(this.saveasbutton);
            this.Controls.Add(this.savebutton);
            this.Controls.Add(this.pictureBox1);
            this.Name = "PictureControl";
            this.Size = new System.Drawing.Size(455, 266);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button savebutton;
        private System.Windows.Forms.Button saveasbutton;
        private System.Windows.Forms.Button saveaddbutton;
    }
}
