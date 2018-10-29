namespace MissionPlanner
{
    partial class SerialOutputPass
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SerialOutputPass));
            this.CMB_serialport = new System.Windows.Forms.ComboBox();
            this.BUT_connect = new MissionPlanner.Controls.MyButton();
            this.CMB_baudrate = new System.Windows.Forms.ComboBox();
            this.chk_write = new System.Windows.Forms.CheckBox();
            this.myLabel1 = new MissionPlanner.Controls.MyLabel();
            this.myLocalIP = new System.Windows.Forms.TextBox();
            this.myLocalPort = new System.Windows.Forms.TextBox();
            this.myLabel2 = new MissionPlanner.Controls.MyLabel();
            this.BTN_StartServer = new MissionPlanner.Controls.MyButton();
            this.myLabel3 = new MissionPlanner.Controls.MyLabel();
            this.SuspendLayout();
            // 
            // CMB_serialport
            // 
            this.CMB_serialport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_serialport.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_serialport, "CMB_serialport");
            this.CMB_serialport.Name = "CMB_serialport";
            // 
            // BUT_connect
            // 
            resources.ApplyResources(this.BUT_connect, "BUT_connect");
            this.BUT_connect.Name = "BUT_connect";
            this.BUT_connect.UseVisualStyleBackColor = true;
            this.BUT_connect.Click += new System.EventHandler(this.BUT_connect_Click);
            // 
            // CMB_baudrate
            // 
            this.CMB_baudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_baudrate.FormattingEnabled = true;
            this.CMB_baudrate.Items.AddRange(new object[] {
            resources.GetString("CMB_baudrate.Items"),
            resources.GetString("CMB_baudrate.Items1"),
            resources.GetString("CMB_baudrate.Items2"),
            resources.GetString("CMB_baudrate.Items3"),
            resources.GetString("CMB_baudrate.Items4"),
            resources.GetString("CMB_baudrate.Items5"),
            resources.GetString("CMB_baudrate.Items6"),
            resources.GetString("CMB_baudrate.Items7")});
            resources.ApplyResources(this.CMB_baudrate, "CMB_baudrate");
            this.CMB_baudrate.Name = "CMB_baudrate";
            // 
            // chk_write
            // 
            resources.ApplyResources(this.chk_write, "chk_write");
            this.chk_write.Name = "chk_write";
            this.chk_write.UseVisualStyleBackColor = true;
            this.chk_write.CheckedChanged += new System.EventHandler(this.chk_write_CheckedChanged);
            // 
            // myLabel1
            // 
            resources.ApplyResources(this.myLabel1, "myLabel1");
            this.myLabel1.Name = "myLabel1";
            this.myLabel1.resize = false;
            // 
            // myLocalIP
            // 
            resources.ApplyResources(this.myLocalIP, "myLocalIP");
            this.myLocalIP.Name = "myLocalIP";
            // 
            // myLocalPort
            // 
            resources.ApplyResources(this.myLocalPort, "myLocalPort");
            this.myLocalPort.Name = "myLocalPort";
            // 
            // myLabel2
            // 
            resources.ApplyResources(this.myLabel2, "myLabel2");
            this.myLabel2.Name = "myLabel2";
            this.myLabel2.resize = false;
            // 
            // BTN_StartServer
            // 
            resources.ApplyResources(this.BTN_StartServer, "BTN_StartServer");
            this.BTN_StartServer.Name = "BTN_StartServer";
            this.BTN_StartServer.UseVisualStyleBackColor = true;
            this.BTN_StartServer.Click += new System.EventHandler(this.BTN_StartServer_Click);
            // 
            // myLabel3
            // 
            resources.ApplyResources(this.myLabel3, "myLabel3");
            this.myLabel3.Name = "myLabel3";
            this.myLabel3.resize = false;
            // 
            // SerialOutputPass
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.myLabel3);
            this.Controls.Add(this.BTN_StartServer);
            this.Controls.Add(this.myLabel2);
            this.Controls.Add(this.myLocalPort);
            this.Controls.Add(this.myLocalIP);
            this.Controls.Add(this.myLabel1);
            this.Controls.Add(this.chk_write);
            this.Controls.Add(this.CMB_baudrate);
            this.Controls.Add(this.BUT_connect);
            this.Controls.Add(this.CMB_serialport);
            this.Name = "SerialOutputPass";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_serialport;
        private Controls.MyButton BUT_connect;
        private System.Windows.Forms.ComboBox CMB_baudrate;
        private System.Windows.Forms.CheckBox chk_write;
        private Controls.MyLabel myLabel1;
        private System.Windows.Forms.TextBox myLocalIP;
        private System.Windows.Forms.TextBox myLocalPort;
        private Controls.MyLabel myLabel2;
        private Controls.MyButton BTN_StartServer;
        private Controls.MyLabel myLabel3;
    }
}