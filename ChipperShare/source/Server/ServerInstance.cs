using System;
using System.Net;
using System.Net.Sockets;
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

        public string FileLoc;

        private TcpListener _listenerServer;
        private IPAddress _remoteIP;
        private TcpClient _client;

        private AlgorithmInstance _algorithm = new();

        private byte[] _buffer = new byte[255];
        private byte[] _data;
        private byte[] _key;
        private NetworkStream _stream;

        public void Listen()
        {
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
                    var mbRes = MessageBox.Show(@"Key entering aborted. You will continue without encryption.",
                        @"Warning", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation);
                    switch (mbRes)
                    {
                        case DialogResult.Abort:
                            PublicLog?.Invoke("Aborted server start.");
                            return;
                        case DialogResult.Ignore:
                            _key = new byte[] { 0x00 };
                            trying = false;
                            break;
                        case DialogResult.Retry:
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


            _listenerServer = new TcpListener(IP, Port);
            _listenerServer.Start();
            PublicLog?.Invoke($@"Server started on IP {IP}");
            _listenerServer.Server.ReceiveTimeout = 1000 * 5;
            
            _client = _listenerServer.AcceptTcpClient();

            if (_client.Connected)
            {
                _remoteIP = ((IPEndPoint)_client.Client.RemoteEndPoint)?.Address;
                PublicLog?.Invoke($@"{_remoteIP} attempted a connection. Waiting for handshake.");
                Authenticate();
            }
            else
            {
                PublicLog?.Invoke("Connection timed out. Stopping server...");
                _listenerServer.Stop();
            }
        }

        private void Authenticate()
        {
            string expected = $"{IP}:{_remoteIP}:{ProtocolVersion}";

            _stream = _client.GetStream();

            int i = _stream.Read(_buffer, 0, _buffer.Length);
            _data = _buffer[..i];

            if (AlgorithmStatic.EncodeString(_algorithm.EncryptData(_data, _key)) != expected)
            {
                _stream.Write(AlgorithmStatic.EncodeBytes("ABORT"));
                _client.Close();
                _listenerServer.Stop();
                PublicLog?.Invoke("The client-server handshake failed. Connection aborted.");
                return;
            }

            PublicLog?.Invoke("Successful handshake.");

            _stream.Write(AlgorithmStatic.EncodeBytes("READY"));

            Send();
        }

        private void Send()
        {
            var sendData = LibChipper.External.LoadBin(FileLoc);
            sendData = _algorithm.EncryptData(sendData, _key);

            _stream.Write(sendData);

            _stream.Close();
            _client.Close();
            _listenerServer.Stop();
            PublicLog?.Invoke("File sent. Stopping server.");
        }
    }
}