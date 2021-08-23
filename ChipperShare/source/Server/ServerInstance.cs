using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using LibChipper;

namespace ChipperShare
{
    internal class ServerInstance
    {
        public delegate void LogDel(string log);
        public event LogDel PublicLog;

        public IPAddress IP;
        public static readonly int Port = 13000;
        public static readonly int ProtocolVersion = 2;

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
            var trying = true;
            while (trying)
            {
                var formKey = new FormKey();
                var res = formKey.ShowDialog();
                IP = formKey.IP;
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

            var start = DateTime.UtcNow;
            while (DateTime.UtcNow - start < TimeSpan.FromMinutes(1) && !_listenerServer.Pending())
                Thread.Sleep(50);

            if (_listenerServer.Pending())
            {
                _client = _listenerServer.AcceptTcpClient();

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
            var expected = $"{IP}:{_remoteIP}:{ProtocolVersion}";

            _stream = _client.GetStream();

            var i = _stream.Read(_buffer, 0, _buffer.Length);
            _data = _buffer[..i];

            var handsDec = AlgorithmStatic.EncodeString(_algorithm.EncryptData(_data, _key));
            if (handsDec != expected)
            {
                _stream.Write(AlgorithmStatic.EncodeBytes("ABORT"));
                _client.Close();
                _listenerServer.Stop();
                PublicLog?.Invoke("The client-server handshake failed. Connection aborted.");

                var handsDecSplit = handsDec.Split(':');
                if (handsDecSplit.Length > 2 && handsDecSplit[2] == ProtocolVersion.ToString())
                    PublicLog?.Invoke("The client-server protocol version is mismatched.");

                return;
            }

            PublicLog?.Invoke("Successful handshake.");

            _stream.Write(AlgorithmStatic.EncodeBytes("READY"));

            Send();
        }

        private void Send()
        {
            _data = _algorithm.EncryptData(External.LoadBin(FileLoc), _key);

            // File size
            var dataLength = BitConverter.GetBytes(_data.Length);
            _stream.Write(dataLength, 0, 4);

            var fileNameByte = AlgorithmStatic.EncodeBytes( External.NameFromPath(FileLoc));

            // File name size
            var fileNameSize = BitConverter.GetBytes(fileNameByte.Length);
            _stream.Write(fileNameSize, 0, 4);

            // File name
            fileNameByte = _algorithm.EncryptData(fileNameByte, _key);
            _stream.Write(fileNameByte, 0, fileNameByte.Length);

            var bytesSent = 0;
            var bytesLeft = _data.Length;

            // File
            while (bytesLeft > 0)
            {
                var curDataSize = Math.Min(1024, bytesLeft);

                _stream.Write(_data, bytesSent, curDataSize);

                bytesSent += curDataSize;
                bytesLeft -= curDataSize;
            }

            // Close connection
            _stream.Close();
            _client.Close();
            _listenerServer.Stop();
            PublicLog?.Invoke("File sent. Stopping server.");
        }
    }
}