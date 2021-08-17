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

            _server.FileLoc = OpenFile();
            if (_server.FileLoc == "") return;

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

        private string OpenFile()
        {
            openFileDialog.ShowDialog();
            string loc = openFileDialog.FileName;

            if (File.Exists(loc))
                return loc;
            return "";
        }
    }
}
