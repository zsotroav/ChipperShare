using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibChipper;

namespace ChipperShare
{
    class ServerInstance
    {
        public delegate void LogDel(string log);
        public event LogDel PublicLog;

        public IPAddress IP = UtilStatic.GetLocalIPAddress();
        public int Port = 13000;
        public int ProtocolVersion = 1;

        private TcpListener _listenerServer;
        private IPAddress _remoteIP;
        private TcpClient _client;

        private AlgorithmInstance _algorithm = new();

        private byte[] buffer = new byte[255];
        private byte[] data;
        private byte[] _key;

        public void Listen()
        {
            _listenerServer = new TcpListener(IP, Port);
            _listenerServer.Start();
            PublicLog?.Invoke($@"Server started on IP {IP}");

            _client = _listenerServer.AcceptTcpClient();
            _remoteIP = ((IPEndPoint)_client.Client.RemoteEndPoint)?.Address;

            PublicLog?.Invoke($@"{_remoteIP} attempted a connection. Waiting for encryption key.");

            bool trying = true;
            while (trying)
            {
                var formKey = new FormKey();
                var res = formKey.ShowDialog();
                if (res == DialogResult.OK)
                {
                    trying = false;
                    _key = formKey.Key;
                }
                else
                {
                    var mbRes = MessageBox.Show(@"Key entering aborted. Do you want to continue without encryption?",
                        @"Error!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation);
                    switch (mbRes)
                    {
                        case DialogResult.Abort:
                            _client.Close();
                            _listenerServer.Stop();
                            PublicLog?.Invoke("Aborted.");
                            return;
                        case DialogResult.Ignore:
                            _key = new byte[] { 0x00 };
                            trying = false;
                            break;
                        case DialogResult.None:
                            _client.Close();
                            _listenerServer.Stop();
                            PublicLog?.Invoke("Aborted.");
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            Authenticate();
        }

        public void Authenticate()
        {
            string expected = $"{IP}:{_remoteIP}:{ProtocolVersion}";

            var stream = _client.GetStream();

            while (stream.Read(buffer, 0, buffer.Length) != 0)
            {
                data = buffer;

                stream.Write(data, 0, data.Length);
            }

            if (_algorithm.EncryptData(data, _key) != AlgorithmStatic.EncodeBytes(expected))
            {
                _client.Close();
                _listenerServer.Stop();
                PublicLog?.Invoke("The client-server handshake failed. Connection aborted.");
            }
            else
            {

            }
        }
    }
}
