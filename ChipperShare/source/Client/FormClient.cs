using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ChipperShare
{
    public partial class FormClient : Form
    {
        private delegate void LogCallback(string text);
        private readonly ClientInstance _client = new();

        public FormClient()
        {
            InitializeComponent();
            _client.PublicLog += Log;
            _client.SaveFile += SaveFileLoc;
        }

        private void Log(string text)
        {
            if (this.textStatus.InvokeRequired)
            {
                LogCallback d = Log;
                Invoke(d, text);
            }
            else
            {
                textStatus.AppendText(Environment.NewLine);
                textStatus.AppendText(text);
            }
        }

        private string SaveFileLoc()
        {
            saveFileDialog.FileName = _client.FileName;
            saveFileDialog.Filter = $@"{_client.FileExt} file|*{_client.FileExt}";
            saveFileDialog.DefaultExt = _client.FileExt;
            var dr = saveFileDialog.ShowDialog();

            return dr == DialogResult.OK ? saveFileDialog.FileName : "";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            var valid = IPAddress.TryParse(textIP.Text, out var remoteIP);
            if (!valid)
            {
                MessageBox.Show(@"Invalid IP address!", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            var trying = true;
            while (trying)
            {
                var formKey = new FormKey();
                var res = formKey.ShowDialog();
                _client.IP = formKey.IP;
                if (res == DialogResult.OK)
                {
                    trying = false;
                    _client.Key = formKey.Key;
                }
                else
                {
                    var mbRes = MessageBox.Show(@"Key entering aborted. You will continue without encryption.",
                        @"Warning", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation);
                    switch (mbRes)
                    {
                        case DialogResult.Abort:
                            Log("Aborted.");
                            return;
                        case DialogResult.Retry:
                            break;
                        case DialogResult.Ignore:
                            _client.Key = new byte[] { 0x00 };
                            trying = false;
                            break;
                        case DialogResult.None:
                            Log("Aborted.");
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            _client.RemoteIP = remoteIP;
            var clientThread = new Thread(_client.Connect)
            {
                Name = "Client connect thread"
            };
            clientThread.SetApartmentState(ApartmentState.STA);
            clientThread.Start();
        }
    }
}
