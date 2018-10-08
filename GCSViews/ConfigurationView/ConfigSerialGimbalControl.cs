using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.Controls;
using MissionPlanner.Properties;
using MissionPlanner.Utilities;

namespace GCSViews.ConfigurationView.ConfigurationView
{
    partial class ConfigSerialGimbalControl : MyUserControl, IActivate
    {
        private Label label1;
        private ComboBox CMB_serialPort;
        private MyButton BUT_portOpen;
        private SerialPort serialControlPort;


        public ConfigSerialGimbalControl()
        {
            this.InitializeComponent();
            Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        // Initialzie and arrange all GUI Stuff here.
        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.label1.AutoSize = true;
            this.label1.ImeMode = ImeMode.NoControl;
            this.label1.Location = new Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new Size(45, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Serial port";
            this.Controls.Add((Control)this.label1);

            this.CMB_serialPort = new ComboBox();
            this.CMB_serialPort.Location = new Point(73, 15);
            this.CMB_serialPort.Name = "CMB_SerialPort";
            this.CMB_serialPort.Size = new Size(100, 21);
            this.CMB_serialPort.TabIndex = 26;
            this.CMB_serialPort.MouseClick += new MouseEventHandler(this.CMB_serialPort_MouseClick);
            this.Controls.Add((Control)this.CMB_serialPort);

            this.BUT_portOpen = new MyButton();
            this.BUT_portOpen.ImeMode = ImeMode.NoControl;
            this.BUT_portOpen.Location = new Point(200, 13);
            this.BUT_portOpen.Name = "myButton1";
            this.BUT_portOpen.Size = new Size(75, 21);
            this.BUT_portOpen.TabIndex = 120;
            //this.BUT_portOpen.Text = MainV2.Camjoystick == null ? "Enable" : "Disable";\
            this.BUT_portOpen.Text = "Open Port";
            this.BUT_portOpen.Click += new EventHandler(this.BUT_portOpen_click);
            this.Controls.Add((Control)this.BUT_portOpen);
        }

        private void BUT_portOpen_click(object sender, EventArgs e)
        {
            serialControlPort = new SerialPort(CMB_serialPort.Text, 9600, Parity.None, 8, StopBits.One);
            serialControlPort.Open();
            serialControlPort.DataReceived += serialPort_DataReceived;
        }


        private Queue<byte> recievedData = new Queue<byte>();
        private void serialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[serialControlPort.BytesToRead];
            serialControlPort.Read(data, 0, data.Length);
            data.ToList().ForEach(b => recievedData.Enqueue(b));
            processData();
        }

        private void processData()
        {

        }

            private void CMB_serialPort_MouseClick(object sender, EventArgs e)
        {
            CMB_serialPort.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            CMB_serialPort.Items.AddRange(ports);
        }

        public void Activate()
        {

        }
    }
}
