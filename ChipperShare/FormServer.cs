using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            var listenThread = new Thread(_server.Listen)
            {
                Name = "Listen thread"
            };
            listenThread.Start();
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
    }
}
