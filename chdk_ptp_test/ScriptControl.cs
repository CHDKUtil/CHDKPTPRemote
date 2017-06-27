using System;
using System.Windows.Forms;
using System.Collections;
using CHDKPTPRemote;
using System.IO;

namespace chdk_ptp_test
{
    public partial class ScriptControl : UserControl
    {
        public ScriptControl()
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
                    propertygrid.Visible = false;
            }
        }

        private void scriptedit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                execbutton.PerformClick();
                e.Handled = true;
            }
        }

        private void execbutton_Click(object sender, EventArgs e)
        {
            if (!Connected)
                return;

            LogLine("executing script: " + scriptedit.Text);
            try
            {
                object r = Session.ExecuteScript(scriptedit.Text);
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
            }
            catch (Exception ex)
            {
                LogLine("exception: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                outputlabel.Text = ex.Message;
            }
            LogLine("done.");
        }
    }
}
