using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ChipperShare
{
    public partial class FormServer : Form
    {
        private delegate void LogCallback(string text);
        private readonly ServerInstance _server = new();

        public FormServer()
        {
            InitializeComponent();
            _server.PublicLog += Log;

            openFileDialog.ShowDialog();
            var loc = openFileDialog.FileName;
            if (string.IsNullOrEmpty(loc) || !File.Exists(loc))
            {
                Log("File open aborted, cancelling server...");
                return;
            }

            _server.FileLoc = loc;

            var listenThread = new Thread(_server.Listen)
            {
                Name = "Listen thread"
            };
            listenThread.SetApartmentState(ApartmentState.STA);
            listenThread.Start();
        }

        private void Log(string text)
        {
            if (textStatus.InvokeRequired)
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
    }
}
