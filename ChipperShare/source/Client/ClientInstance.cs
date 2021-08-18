using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using LibChipper;

namespace ChipperShare
{
    internal class ClientInstance
    {
        public delegate string SaveDel();
        public event SaveDel SaveFile;

        public delegate void LogDel(string log);
        public event LogDel PublicLog;

        public IPAddress IP;
        public IPAddress RemoteIP;

        public static readonly int Port = 13000;
        public static readonly int ProtocolVersion = 1;
        
        private TcpClient _client;

        private AlgorithmInstance _algorithm = new();

        private byte[] _buffer = new byte[255];
        private byte[] _data;
        private NetworkStream _stream;

        public byte[] Key;
        public string FileName;
        public string FileExt;

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
            var send = $"{RemoteIP}:{IP}:{ProtocolVersion}";

            var handRaw = AlgorithmStatic.EncodeBytes(send);
            var handShake = _algorithm.EncryptData(handRaw, Key);
            
            _stream = _client.GetStream();
            _stream.Write(handShake, 0, handShake.Length);

            var i = _stream.Read(_buffer, 0, _buffer.Length);
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

            // File name
            int i = _stream.Read(_buffer, 0, _buffer.Length);
            _data = _buffer[..i];
            FileName = AlgorithmStatic.EncodeString(_algorithm.EncryptData(_data, Key));
            FileExt = External.ExtFromPath(FileName);

            // File size
            var fileSizeBytes = new byte[4];
            _stream.Read(fileSizeBytes, 0, 4);
            var dataLength = BitConverter.ToInt32(fileSizeBytes, 0);

            var bytesLeft = dataLength;
            var bytesRead = 0;

            // File
            _data = new byte[dataLength];
            while (bytesLeft > 0)
            {
                var curDataSize = Math.Min(1024, bytesLeft);
                if (_client.Available < curDataSize)
                    curDataSize = _client.Available; 

                _stream.Read(_data, bytesRead, curDataSize);

                bytesRead += curDataSize;
                bytesLeft -= curDataSize;
            }

            // Decrypt data
            _data = _algorithm.EncryptData(_data, Key);
            PublicLog?.Invoke("File received.");

            // Close connection
            _stream.Close();
            _client.Close();
            PublicLog?.Invoke("Connection closed.");

            // Save file
            var loc = SaveFile?.Invoke();
            while (string.IsNullOrEmpty(loc))
            {
                var dr = MessageBox.Show(
                    @"The save file dialog was forcibly closed. Do you want to discard the received file?",
                    @"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dr == DialogResult.Yes)
                    return;
                loc = SaveFile?.Invoke();
            }

            External.SaveBin(loc,_data);
            PublicLog?.Invoke("Saved file.");
        }
    }
}
