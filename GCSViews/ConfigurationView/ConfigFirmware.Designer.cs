using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO.Ports;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Net;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigFirmware : MyUserControl
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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigFirmware));
            this.panel1 = new System.Windows.Forms.Panel();
            this.BUT_Serial_Refresh = new MissionPlanner.Controls.MyButton(); //new System.Windows.Forms.Button();
            this.PX_port = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.FW_Uploader_log = new System.Windows.Forms.TextBox();
            this.BUT_FW_Update = new MissionPlanner.Controls.MyButton(); //new System.Windows.Forms.Button();
            this.GD_Port = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BUT_Serial_Refresh);
            this.panel1.Controls.Add(this.PX_port);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.FW_Uploader_log);
            this.panel1.Controls.Add(this.BUT_FW_Update);
            this.panel1.Controls.Add(this.GD_Port);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // BUT_Serial_Refresh
            // 
            resources.ApplyResources(this.BUT_Serial_Refresh, "BUT_Serial_Refresh");
            this.BUT_Serial_Refresh.Name = "BUT_Serial_Refresh";
            this.BUT_Serial_Refresh.UseVisualStyleBackColor = true;
            this.BUT_Serial_Refresh.Click += new System.EventHandler(this.BUT_Serial_Refresh_Click);
            // 
            // PX_port
            // 
            this.PX_port.FormattingEnabled = true;
            resources.ApplyResources(this.PX_port, "PX_port");
            this.PX_port.Name = "PX_port";
            this.PX_port.SelectedIndexChanged += new System.EventHandler(this.PX_port_SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // FW_Uploader_log
            // 
            resources.ApplyResources(this.FW_Uploader_log, "FW_Uploader_log");
            this.FW_Uploader_log.Name = "FW_Uploader_log";
            this.FW_Uploader_log.ReadOnly = true;
            // 
            // BUT_FW_Update
            // 
            resources.ApplyResources(this.BUT_FW_Update, "BUT_FW_Update");
            this.BUT_FW_Update.Name = "BUT_FW_Update";
            this.BUT_FW_Update.UseVisualStyleBackColor = true;
            this.BUT_FW_Update.Click += new System.EventHandler(this.BUT_FW_Update_Click);
            // 
            // GD_Port
            // 
            this.GD_Port.FormattingEnabled = true;
            resources.ApplyResources(this.GD_Port, "GD_Port");
            this.GD_Port.Name = "GD_Port";
            this.GD_Port.SelectedIndexChanged += new System.EventHandler(this.GD_Port_SelectedIndexChanged);
            // 
            // ConfigFirmware
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.panel1);
            this.Name = "ConfigFirmware";
            resources.ApplyResources(this, "$this");
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private Panel panel1;
        private ComboBox PX_port;
        private Label label2;
        private Label label1;
        private TextBox FW_Uploader_log;
        private Controls.MyButton BUT_FW_Update;
        private ComboBox GD_Port;
        private Controls.MyButton BUT_Serial_Refresh;
    }
}