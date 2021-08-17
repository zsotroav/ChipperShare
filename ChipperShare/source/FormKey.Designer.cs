
namespace ChipperShare
{
    partial class FormKey
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TextKeyName = new System.Windows.Forms.TextBox();
            this.TextKeyPass = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TextKeyPass2 = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.comboIP = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TextKeyPass
            // 
            this.TextKeyPass.Location = new System.Drawing.Point(115, 12);
            this.TextKeyPass.Name = "TextKeyPass";
            this.TextKeyPass.Size = new System.Drawing.Size(203, 23);
            this.TextKeyPass.TabIndex = 2;
            this.TextKeyPass.UseSystemPasswordChar = true;
            // 
            // TextKeyPass2
            // 
            this.TextKeyPass2.Location = new System.Drawing.Point(115, 41);
            this.TextKeyPass2.Name = "TextKeyPass2";
            this.TextKeyPass2.Size = new System.Drawing.Size(203, 23);
            this.TextKeyPass2.TabIndex = 4;
            this.TextKeyPass2.UseSystemPasswordChar = true;
            // 
            // comboIP
            // 
            this.comboIP.FormattingEnabled = true;
            this.comboIP.Location = new System.Drawing.Point(115, 70);
            this.comboIP.Name = "comboIP";
            this.comboIP.Size = new System.Drawing.Size(102, 23);
            this.comboIP.TabIndex = 6;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(223, 70);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(95, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Submit";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "Key Passphrase";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "Passphrase again";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "IP address";
            // 
            // FormKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 105);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboIP);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TextKeyPass2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextKeyPass);
            this.Controls.Add(this.TextKeyName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormKey";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enter the encryption key";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextKeyName;
        private System.Windows.Forms.TextBox TextKeyPass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextKeyPass2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox comboIP;
        private System.Windows.Forms.Label label4;
    }
}