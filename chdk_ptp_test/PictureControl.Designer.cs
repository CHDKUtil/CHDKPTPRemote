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
            this.overlaybutton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.getimagebutton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // overlaybutton
            // 
            this.overlaybutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.overlaybutton.Location = new System.Drawing.Point(381, 0);
            this.overlaybutton.Name = "overlaybutton";
            this.overlaybutton.Size = new System.Drawing.Size(75, 23);
            this.overlaybutton.TabIndex = 22;
            this.overlaybutton.Text = "Get Overlay";
            this.overlaybutton.UseVisualStyleBackColor = true;
            this.overlaybutton.Click += new System.EventHandler(this.overlaybutton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.Location = new System.Drawing.Point(0, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(360, 240);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // getimagebutton
            // 
            this.getimagebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.getimagebutton.Location = new System.Drawing.Point(300, 0);
            this.getimagebutton.Name = "getimagebutton";
            this.getimagebutton.Size = new System.Drawing.Size(75, 23);
            this.getimagebutton.TabIndex = 20;
            this.getimagebutton.Text = "Get Image";
            this.getimagebutton.UseVisualStyleBackColor = true;
            this.getimagebutton.Click += new System.EventHandler(this.getimagebutton_Click);
            // 
            // PictureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.overlaybutton);
            this.Controls.Add(this.getimagebutton);
            this.Controls.Add(this.pictureBox1);
            this.Name = "PictureControl";
            this.Size = new System.Drawing.Size(455, 266);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button overlaybutton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button getimagebutton;
    }
}
