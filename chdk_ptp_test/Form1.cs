// Copyright Muck van Weerdenburg 2011.
// Distributed under the Boost Software License, Version 1.0.
// (See accompanying file LICENSE_1_0.txt or copy at
// http://www.boost.org/LICENSE_1_0.txt)

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using LibUsbDotNet;
using CHDKPTP;
using CHDKPTPRemote;
using Be.Windows.Forms;

namespace chdk_ptp_test
{
    public partial class Form1 : Form
    {
        private bool connected = false;
        private CHDKPTPDevice connected_device = null;
        private Session session = null;
        private Bitmap live_image = null;
        private Bitmap live_overlay = null;
        private StreamWriter Log;
        private int display_width, display_height;

        private void refresh_camera_list()
        {
            LogLine("refreshing camera list...");
            devicecombobox.Items.Clear();
            devicecombobox.Text = "<select a device>";

            try
            {
                foreach (CHDKPTPDevice dev in Session.ListDevices(false))
                {
                    LogLine("found device: " + dev.Name + (dev.CHDKSupported ? " (CHDK PTP supported)" : dev.PTPSupported ? " (PTP supported)" : ""));
                    if (dev.PTPSupported && !dev.CHDKSupported && dev.CHDKVersionMajor != -1)
                        LogLine("CHDK version: " + dev.CHDKVersionMajor + "." + dev.CHDKVersionMinor);
                    if (dev.CHDKSupported)
                        devicecombobox.Items.Add(dev);
                }
                LogLine("done.");
            }
            catch (Exception ex)
            {
                LogLine("exception: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                MessageBox.Show("could not open PTP session: " + ex.Message + "\n\n" + ex.StackTrace.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LogLine(string s)
        {
            Log.WriteLine(s);
            Log.Flush();
        }

        private void UsbDevice_UsbErrorEvent(object sender, UsbError e)
        {
            LogLine("usb error: " + e.ToString());
            MessageBox.Show("UsbError: " + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public Form1()
        {
            InitializeComponent();
            Log = File.AppendText("chdk_ptp_test.log");
            LogLine("=== program started ===");
            UsbDevice.UsbErrorEvent += new EventHandler<UsbError>(UsbDevice_UsbErrorEvent);
        }

        ~Form1()
        {
            LogLine("closing...");
            UsbDevice.Exit();
            LogLine("=== program ended ===");
        }

        private void refreshbutton_Click(object sender, EventArgs e)
        {
            refresh_camera_list();
        }

        private void connectbutton_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                MessageBox.Show("Already opened a device!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (devicecombobox.SelectedItem == null)
            {
                MessageBox.Show("No device selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            connected_device = devicecombobox.SelectedItem as CHDKPTPDevice;

            LogLine("opening device: " + connected_device.Name);
            try
            {
                session = new Session(connected_device);
                session.Connect();
            }
            catch (Exception ex)
            {
                LogLine("exception: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                connected = false;
                connected_device = null;
                session = null;
                MessageBox.Show("could not open PTP session: " + ex.Message + "\n\n" + ex.StackTrace.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogLine("connected.");
            connected = true;
            connect_button.Visible = false;
            disconnectbutton.Visible = true;
            statuslabel.Text = "Connected to: " + connected_device.ToString();
        }

        private void disconnectbutton_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                LogLine("closing connection...");
                try
                {
                    session.Disconnect();
                }
                catch (Exception ex)
                {
                    LogLine("exception: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                }

                LogLine("closed.");
                post_disconnect();
            }
        }

        private void post_disconnect()
        {
            statuslabel.Text = "Not connected";
            connected_device = null;
            connect_button.Visible = true;
            disconnectbutton.Visible = false;
            connected = false;
        }

        private void getimagebutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;
        }

        private void recordbutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            LogLine("switching to record mode...");
            try
            {
                session.ExecuteScript("switch_mode_usb(1)");
            }
            catch (Exception ex)
            {
                LogLine("exception: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                MessageBox.Show("could not switch to record mode: " + ex.Message + "\n\n" + ex.StackTrace.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogLine("done.");
        }

        private void playbackbutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            LogLine("switching to playback mode...");
            try
            {
                session.ExecuteScript("switch_mode_usb(0)");
            }
            catch (Exception ex)
            {
                LogLine("exception: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                MessageBox.Show("could not switch to playback mode: " + ex.Message + "\n\n" + ex.StackTrace.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LogLine("done.");
        }

        private void shutdownbutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            LogLine("shutting camera down... (may result in exceptions due to loss of connection)");
            try
            {
                session.ExecuteScript("shut_down()");
            }
            catch (Exception ex)
            {
                LogLine("exception: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
            }
            disconnectbutton.PerformClick();
            LogLine("shut down complete.");
        }

        private void rebootbutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            LogLine("rebooting... (may result in exceptions due to loss of connection)");
            try
            {
                session.ExecuteScript("reboot()");
            }
            catch (Exception ex)
            {
                LogLine("exception: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
            }
            post_disconnect();
            refresh_camera_list();
            LogLine("reboot complete.");
        }

        private void execbutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            LogLine("executing script: " + scriptedit.Text);
            try
            {
                object r = session.ExecuteScript(scriptedit.Text);
                if (r == null)
                {
                    outputlabel.Text = "(none)";
                }
                else if (r.GetType() == typeof(bool))
                {
                    outputlabel.Text = r.ToString();
                }
                else if (r.GetType() == typeof(int))
                {
                    outputlabel.Text = r.ToString();
                }
                else if (r.GetType() == typeof(string))
                {
                    outputlabel.Text = (string)r;
                }
                else if (r is IDictionary)
                {
                    outputlabel.Text = "(table)";
                    propertygrid.PropertySort = PropertySort.NoSort;
                    propertygrid.SelectedObject = new PropertyGridDictionaryAdapter((IDictionary)r);
                }
                else
                {
                    outputlabel.Text = "(unsupported type)";
                }
                propertygrid.Visible = r is IDictionary;
                hexbox.Visible = false;
            }
            catch (Exception ex)
            {
                LogLine("exception: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                outputlabel.Text = ex.Message;
            }
            LogLine("done.");
        }

        private void scriptedit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\n')
            {
                execbutton.PerformClick();
            }
        }

        private void overlaybutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (live_image != null)
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(live_image, getimagebutton.Left, getimagebutton.Bottom + 10, display_width, display_height);
            }
            if (live_overlay != null)
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(live_overlay, getimagebutton.Left, getimagebutton.Bottom + 10, display_width, display_height);
            }
        }

        private void getmemorybutton_Click(object sender, EventArgs e)
        {
            if (!connected)
                return;

            uint address;
            if (!uint.TryParse(scriptedit.Text, out address))
            {
                if (scriptedit.Text.StartsWith("0x"))
                {
                    address = Convert.ToUInt32(scriptedit.Text, 16);
                }
                else
                {
                    outputlabel.Text = "Invalid addresss";
                    return;
                }
            }

            outputlabel.Text = $"(0x{address:X})";
            byte[] buffer = session.GetMemory(address, 4096);
            hexbox.LineInfoOffset = address / hexbox.BytesPerLine;
            hexbox.ByteProvider = new DynamicByteProvider(buffer);
            hexbox.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: Load doesn't seem the right place as we don't get usb error messages here
            refresh_camera_list();
        }

    }
}
