using IPAddressControlLib;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews
{
    public class NetworkSettings : Form
    {
        private IContainer components = (IContainer)null;
        public string network_url;
        public IPAddress url_ip;
        public int url_port;
        private Button CancelButton;
        private Button OkButton;
        private Label StreamPortLabel;
        private TextBox StreamPortTextBox;
        private Label StreamIPLabel;
        private IPAddressControl StreamIPAddressControl;

        public NetworkSettings()
        {
            this.network_url = (string)null;
            this.InitializeComponent();
            this.BackColor = Color.FromArgb(38, 39, 41);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            string ipString = this.StreamIPAddressControl.ToString();
            if (this.StreamPortTextBox.TextLength > 0)
            {
                int int32 = Convert.ToInt32(this.StreamPortTextBox.Text);
                IPAddress address;
                if (IPAddress.TryParse(ipString, out address) && int32 > 1024 && int32 < 65536 && ipString != "0.0.0.0")
                {
                    this.network_url = "rtp://" + ipString + ":" + (object)int32;
                    this.url_ip = address;
                    this.url_port = int32;
                    this.Close();
                    return;
                }
            }
            int num = (int)MessageBox.Show("Improper IP/Port Entered !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private void StreamPortTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Regex.IsMatch(e.KeyChar.ToString(), "[^\b]"))
                return;
            if (!Regex.IsMatch(e.KeyChar.ToString(), "\\d+") || this.StreamPortTextBox.TextLength > 4)
            {
                if (this.StreamPortTextBox.TextLength == this.StreamPortTextBox.SelectionLength && this.StreamPortTextBox.SelectionLength > 0)
                    return;
                e.Handled = true;
            }
            if (this.StreamPortTextBox.TextLength <= 0 || Convert.ToInt32(this.StreamPortTextBox.Text) <= 65536)
                return;
            this.StreamPortTextBox.Text = "65536";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NetworkSettings));
            this.CancelButton = new Button();
            this.OkButton = new Button();
            this.StreamPortLabel = new Label();
            this.StreamPortTextBox = new TextBox();
            this.StreamIPLabel = new Label();
            this.StreamIPAddressControl = new IPAddressControl();
            this.SuspendLayout();
            this.CancelButton.Location = new Point(203, 71);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new Size(64, 23);
            this.CancelButton.TabIndex = 16;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new EventHandler(this.CancelButton_Click);
            this.OkButton.Location = new Point(122, 71);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new Size(64, 23);
            this.OkButton.TabIndex = 15;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new EventHandler(this.OkButton_Click);
            this.StreamPortLabel.AutoSize = true;
            this.StreamPortLabel.ForeColor = SystemColors.ButtonHighlight;
            this.StreamPortLabel.Location = new Point(144, 37);
            this.StreamPortLabel.Name = "StreamPortLabel";
            this.StreamPortLabel.Size = new Size(62, 13);
            this.StreamPortLabel.TabIndex = 14;
            this.StreamPortLabel.Text = "Stream Port";
            this.StreamPortTextBox.Location = new Point(25, 30);
            this.StreamPortTextBox.Name = "StreamPortTextBox";
            this.StreamPortTextBox.Size = new Size(113, 20);
            this.StreamPortTextBox.TabIndex = 13;
            this.StreamPortTextBox.Text = "11024";
            this.StreamIPLabel.AutoSize = true;
            this.StreamIPLabel.ForeColor = SystemColors.ButtonHighlight;
            this.StreamIPLabel.Location = new Point(144, 9);
            this.StreamIPLabel.Name = "StreamIPLabel";
            this.StreamIPLabel.Size = new Size(53, 13);
            this.StreamIPLabel.TabIndex = 12;
            this.StreamIPLabel.Text = "Stream IP";
            this.StreamIPAddressControl.AllowInternalTab = false;
            this.StreamIPAddressControl.AutoHeight = true;
            this.StreamIPAddressControl.BackColor = Color.White;
            this.StreamIPAddressControl.BorderStyle = BorderStyle.Fixed3D;
            this.StreamIPAddressControl.Cursor = Cursors.IBeam;
            this.StreamIPAddressControl.Location = new Point(25, 5);
            this.StreamIPAddressControl.MinimumSize = new Size(87, 20);
            this.StreamIPAddressControl.Name = "StreamIPAddressControl";
            this.StreamIPAddressControl.ReadOnly = false;
            this.StreamIPAddressControl.Size = new Size(113, 20);
            this.StreamIPAddressControl.TabIndex = 17;
            this.StreamIPAddressControl.Text = "225.1.2.3";
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.ControlDarkDark;
            this.ClientSize = new Size(271, 105);
            this.Controls.Add((Control)this.StreamIPAddressControl);
            this.Controls.Add((Control)this.CancelButton);
            this.Controls.Add((Control)this.OkButton);
            this.Controls.Add((Control)this.StreamPortLabel);
            this.Controls.Add((Control)this.StreamPortTextBox);
            this.Controls.Add((Control)this.StreamIPLabel);
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.Name = nameof(NetworkSettings);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Video Network Settings";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
