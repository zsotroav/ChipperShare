using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ChipperShare
{
    public partial class FormKey : Form
    {
        public string Passphrase;
        public byte[] Key;
        public IPAddress IP;

        public FormKey()
        {
            InitializeComponent();

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily != AddressFamily.InterNetwork) continue;

                comboIP.Items.Add(ip);
                comboIP.Text = ip.ToString();
                IP = ip;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (TextKeyPass.Text == "")
            {
                DialogResult = DialogResult.Abort;
                Close();
            } else if (TextKeyPass.Text == TextKeyPass2.Text)
            {
                Passphrase = TextKeyPass.Text;
                Key = LibChipper.AlgorithmStatic.GenKey(Passphrase);
                DialogResult = DialogResult.OK;
                Close();
                IP = IPAddress.Parse(comboIP.Text);
            }
            else
            {
                MessageBox.Show(@"The two passphrases do not match!", @"Error!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
