using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using CHDKPTPRemote;
using CHDKPTP;
using chdk_ptp_test.Properties;

namespace chdk_ptp_test
{
    public partial class PictureControl : UserControl, IDisposable
    {
        private Bitmap live_image = null;
        private Bitmap live_overlay = null;
        private BackgroundWorker worker;
        private Image saveImage;

        public PictureControl()
        {
            InitializeComponent();
            VisibleChanged += PictureControl_VisibleChanged;
            worker = new BackgroundWorker();
        }

        void IDisposable.Dispose()
        {
            worker.Dispose();
        }

        private void PictureControl_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                worker.DoWork += Worker_DoWork;
                worker.RunWorkerAsync();
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            }
            else
                worker.DoWork -= Worker_DoWork;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Yield();

            if (!connected)
                return;

            session.GetLiveData((int)LIVE_XFER_TYPE.VIEWPORT);

            if (live_image == null)
                live_image = session.GetLiveImage();
            else
                session.GetLiveImage(live_image);

            var image = live_image;

            Thread.Yield();

            if (!connected)
                return;

            session.GetLiveData((int)LIVE_XFER_TYPE.BITMAP | (int)LIVE_XFER_TYPE.PALETTE);

            if (live_overlay == null)
                live_overlay = session.GetLiveOverlay();
            else
                session.GetLiveOverlay(live_overlay);

            Thread.Yield();

            pictureBox1.BeginInvoke((Action<Image>)RefreshPicture, image);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.RunWorkerAsync();
        }

        private void RefreshPicture(Image image)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(live_overlay, 0, 0, image.Width, image.Height);
            }

            pictureBox1.Image = live_image;
            pictureBox1.Visible = true;
            pictureBox1.Refresh();
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
                {
                    worker.DoWork -= Worker_DoWork;
                    pictureBox1.Visible = false;
                }
            }
        }

        private void savebutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            saveImage = pictureBox1.Image;

            LogLine("saving image...");

            ImageFormat format = Settings.Default.SaveImageFormat;
            DateTime dateTime = DateTime.Now;
            string ext;
            if (format == ImageFormat.Icon)
                ext = "ico";
            else
                ext = format.ToString().ToLower();
            string filename = $"chdkptp_{dateTime:yyyyMMdd}_{dateTime:HHmmss}.{ext}";
            saveImage.Save(filename, format);

            LogLine($"saved {filename}");
        }

        private void saveasbutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            saveImage = pictureBox1.Image;

            ImageFormat[] formats =
            {
                null,
                ImageFormat.Bmp,
                ImageFormat.Emf,
                ImageFormat.Wmf,
                ImageFormat.Gif,
                ImageFormat.Jpeg,
                ImageFormat.Png,
                ImageFormat.Tiff,
                ImageFormat.Exif,
                ImageFormat.Icon,
            };

            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "BMP files|*.bmp|EMF files|*.emf|WMF files|*.wmf|GIF files|*.gif|JPEG files|*.jpeg|PNG files|*.png|TIFF files|*.tiff|EXIF files|*.exif|ICO files|*.ico|All files|*.*",
                FilterIndex = GetFilterIndex(formats),
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.FilterIndex > 0 & dlg.FilterIndex < formats.Length)
                {
                    ImageFormat format = formats[dlg.FilterIndex];
                    Settings.Default.SaveImageFormat = format;
                    var encoder = GetEncoderInfo(format);
                    var encoderParams = GetSaveEncoderParams(format);
                    LogLine("saving image...");
                    string filename = dlg.FileName;
                    saveImage.Save(filename, encoder, encoderParams);
                    LogLine($"saved {filename}");
                }
            }
        }

        private void saveaddbutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            if (saveImage == null)
                return;

            ImageFormat format = Settings.Default.SaveImageFormat;
            var encoderParams = GetSaveAddEncoderParams(format);
            if (encoderParams == null)
                return;

            LogLine("adding image...");

            try
            {
                saveImage.SaveAdd(pictureBox1.Image, encoderParams);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LogLine($"image added");
        }

        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].FormatID == format.Guid)
                    return encoders[j];
            }
            return null;
        }

        private static EncoderParameters GetSaveEncoderParams(ImageFormat format)
        {
            if (format != ImageFormat.Tiff)
                return new EncoderParameters(0);
            return new EncoderParameters
            {
                Param = new[]
                {
                    new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionCCITT4),
                    new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame),
                }
            };
        }

        private static EncoderParameters GetSaveAddEncoderParams(ImageFormat format)
        {
            if (format != ImageFormat.Tiff)
                return null;
            return new EncoderParameters
            {
                Param = new[]
                {
                    new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage),
                }
            };
        }

        private static int GetFilterIndex(ImageFormat[] formats)
        {
            for (int i = 0; i < formats.Length; i++)
                if (Settings.Default.SaveImageFormat.Equals(formats[i]))
                    return i;
            return 0;
        }
    }
}
