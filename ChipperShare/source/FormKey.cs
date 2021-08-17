﻿using System;
using System.Windows.Forms;
using LibChipper;

namespace ChipperShare
{
    public partial class FormKey : Form
    {
        public string Passphrase;
        public byte[] Key;
        public FormKey()
        {
            InitializeComponent();
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
            }
            else
            {
                MessageBox.Show(@"The two passphrases do not match!", @"Error!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
