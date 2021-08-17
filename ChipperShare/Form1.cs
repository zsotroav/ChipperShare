using System;
using System.Windows.Forms;

namespace ChipperShare
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OpenServer(object sender, EventArgs e)
        {
            var formServer = new FormServer();
            formServer.Show();
        }

        private void OpenClient(object sender, EventArgs e)
        {

        }
    }
}
