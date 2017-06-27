using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using CHDKPTPRemote;
using CHDKPTP;
using System.Drawing.Imaging;
using chdk_ptp_test.Properties;

namespace chdk_ptp_test
{
    public partial class PictureControl : UserControl
    {
        private Bitmap live_image = null;
        private Bitmap live_overlay = null;

        public PictureControl()
        {
            InitializeComponent();
        }

        private void LogLine(string s)
        {
            Log.WriteLine(s);
            Log.Flush();
        }

        public StreamWriter Log { get; set; }

        private Session session;
        public Session Session
        {
            get { return session; }
            set { session = value; }
        }

        private bool connected;
        public bool Connected
        {
            get { return connected; }
            set
            {
                connected = value;
                if (!connected)
                    pictureBox1.Visible = false;
            }
        }

        private void getimagebutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            LogLine("getting live image...");
            try
            {
                session.GetLiveData((int)LIVE_XFER_TYPE.VIEWPORT);

                if (live_image == null)
                {
                    live_image = session.GetLiveImage();
                }
                else
                {
                    session.GetLiveImage(live_image);
                }

                pictureBox1.Image = live_image;
                pictureBox1.Refresh();
                pictureBox1.Visible = true;
            }
            catch (Exception ex)
            {
                LogLine("exception: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                MessageBox.Show("could not get live image: " + ex.Message + "\n\n" + ex.StackTrace.ToString());
                return;
            }
        }

        private void overlaybutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            LogLine("getting live overlay...");
            try
            {
                session.GetLiveData((int)LIVE_XFER_TYPE.BITMAP + (int)LIVE_XFER_TYPE.PALETTE);
                if (live_overlay == null)
                {
                    live_overlay = session.GetLiveOverlay();
                }
                else
                {
                    session.GetLiveOverlay(live_overlay);
                }
            }
            catch (Exception ex)
            {
                LogLine("exception: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                MessageBox.Show("could not get live overlay: " + ex.Message + "\n\n" + ex.StackTrace.ToString());
                return;
            }


            if (pictureBox1.Image == null)
            {
                Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(live_overlay, 0, 0, pictureBox1.Width, pictureBox1.Height);

                }
                pictureBox1.Image = bmp;
            }


            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                //g.DrawLine(Pens.Black, _Previous.Value, e.Location);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(live_overlay, 0, 0, pictureBox1.Image.Width, pictureBox1.Image.Height);
            }

            pictureBox1.Refresh();
            pictureBox1.Visible = true;
        }

        private void savebutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            Image image = pictureBox1.Image;
            if (image == null)
            {
                overlaybutton.PerformClick();
                image = pictureBox1.Image;
            }

            LogLine("saving image...");

            ImageFormat format = Settings.Default.SaveImageFormat;
            DateTime dateTime = DateTime.Now;
            string ext;
            if (format == ImageFormat.Icon)
                ext = "ico";
            else
                ext = format.ToString().ToLower();
            string filename = $"chdkptp_{dateTime.Year:04}{dateTime.Month:02}{dateTime.Day:02}_{dateTime.Hour:02}{dateTime.Minute:02}{dateTime.Second:02}.{ext}";
            image.Save(filename, format);

            LogLine($"saved {filename}");
        }
    }
}
