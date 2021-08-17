using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using LibChipper;

namespace ChipperShare
{
    class ClientInstance
    {
        public delegate string SaveDel();
        public event SaveDel SaveFile;

        public delegate void LogDel(string log);
        public event LogDel PublicLog;

        public IPAddress IP = UtilStatic.GetLocalIPAddress();
        public IPAddress RemoteIP;

        public int Port = 13000;
        public int ProtocolVersion = 1;
        
        private TcpClient _client;

        private AlgorithmInstance _algorithm = new();

        private byte[] _buffer = new byte[255];
        private byte[] _data;
        private List<byte> _dataList = new();
        private NetworkStream _stream;

        public byte[] Key;

        public void Connect()
        {
            PublicLog?.Invoke($@"Attempting connection to {RemoteIP}");

            try
            {
                _client = new TcpClient(RemoteIP.ToString(), Port);
            }
            catch (SocketException e)
            {
                PublicLog?.Invoke(e.Message);
                MessageBox.Show(e.Message, @"Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (ArgumentNullException e)
            {
                PublicLog?.Invoke(e.Message);
                MessageBox.Show(e.Message, @"Argument Null Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Authenticate();
        }

        public void Authenticate()
        {
            string send = $"{RemoteIP}:{IP}:{ProtocolVersion}";

            byte[] handRaw = LibChipper.AlgorithmStatic.EncodeBytes(send);
            byte[] handShake = _algorithm.EncryptData(handRaw, Key);
            
            _stream = _client.GetStream();
            _stream.Write(handShake, 0, handShake.Length);

            int i = _stream.Read(_buffer, 0, _buffer.Length);
            _data = _buffer[..i];

            if (AlgorithmStatic.EncodeString(_data) == "READY")
                Receive();
            else if (AlgorithmStatic.EncodeString(_data) == "ABORT")
            {
                _stream.Close();
                _client.Close();
                PublicLog?.Invoke("The server refused the connection.");
                PublicLog?.Invoke("Connection aborted.");
            }
        }

        public void Receive()
        {
            PublicLog?.Invoke("The server accepted the connection. Receiving file...");
            _data = null;
            _dataList = new List<byte>();

            int i;
            while ((i = _stream.Read(_buffer, 0, _buffer.Length)) != 0)
            {
                foreach (var b in _buffer[..i])
                {
                    _dataList.Add(b);
                }
            }

            _data = _dataList.ToArray();
            
            _data = _algorithm.EncryptData(_data, Key);
            PublicLog?.Invoke("File received.");

            _stream.Close();
            _client.Close();
            PublicLog?.Invoke("Connection closed.");

            string loc = SaveFile?.Invoke();
            if (string.IsNullOrEmpty(loc))
            {
                MessageBox.Show(@"No save location returned, defaulting to Documents/Received.file",
                    @"Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                loc = External.CombinePath(External.DocumentsDir, "Received.file");
            }

            LibChipper.External.SaveBin(loc,_data);
            PublicLog?.Invoke("Saved file.");
        }
    }
}
