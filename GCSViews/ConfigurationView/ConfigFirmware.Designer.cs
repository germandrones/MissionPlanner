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
            this.myButton1 = new MissionPlanner.Controls.MyButton();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.lbl_status = new System.Windows.Forms.Label();
            this.BUT_Serial_Refresh = new MissionPlanner.Controls.MyButton();
            this.PX_port = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.FW_Uploader_log = new System.Windows.Forms.TextBox();
            this.BUT_FW_Update = new MissionPlanner.Controls.MyButton();
            this.GD_Port = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TB_UpdaterManual = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.myButton1);
            this.panel1.Controls.Add(this.progress);
            this.panel1.Controls.Add(this.lbl_status);
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
            // myButton1
            // 
            resources.ApplyResources(this.myButton1, "myButton1");
            this.myButton1.Name = "myButton1";
            this.myButton1.UseVisualStyleBackColor = true;
            this.myButton1.Click += new System.EventHandler(this.myButton1_Click);
            // 
            // progress
            // 
            resources.ApplyResources(this.progress, "progress");
            this.progress.Name = "progress";
            // 
            // lbl_status
            // 
            resources.ApplyResources(this.lbl_status, "lbl_status");
            this.lbl_status.Name = "lbl_status";
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TB_UpdaterManual);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // TB_UpdaterManual
            // 
            resources.ApplyResources(this.TB_UpdaterManual, "TB_UpdaterManual");
            this.TB_UpdaterManual.Name = "TB_UpdaterManual";
            this.TB_UpdaterManual.ReadOnly = true;
            // 
            // ConfigFirmware
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Name = "ConfigFirmware";
            resources.ApplyResources(this, "$this");
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        private Panel panel1;
        private ComboBox PX_port;
        private Label label2;
        private Label label1;
        private Controls.MyButton BUT_FW_Update;
        private ComboBox GD_Port;
        private Controls.MyButton BUT_Serial_Refresh;
        private ProgressBar progress;
        private Label lbl_status;
        public TextBox FW_Uploader_log;
        private Controls.MyButton myButton1;
        private GroupBox groupBox1;
        private TextBox TB_UpdaterManual;
    }
}