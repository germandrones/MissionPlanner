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
            this.BTN_Custom_firmware = new System.Windows.Forms.Button();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.lbl_status = new System.Windows.Forms.Label();
            this.BTN_Custom_GD_firmware = new System.Windows.Forms.Button();
            this.GD_Port = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // BTN_Custom_firmware
            // 
            resources.ApplyResources(this.BTN_Custom_firmware, "BTN_Custom_firmware");
            this.BTN_Custom_firmware.Name = "BTN_Custom_firmware";
            this.BTN_Custom_firmware.UseVisualStyleBackColor = true;
            this.BTN_Custom_firmware.Click += new System.EventHandler(this.BTN_Custom_firmware_Click);
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
            // BTN_Custom_GD_firmware
            // 
            resources.ApplyResources(this.BTN_Custom_GD_firmware, "BTN_Custom_GD_firmware");
            this.BTN_Custom_GD_firmware.Name = "BTN_Custom_GD_firmware";
            this.BTN_Custom_GD_firmware.UseVisualStyleBackColor = true;
            this.BTN_Custom_GD_firmware.Click += new System.EventHandler(this.BTN_Custom_GD_firmware_Click);
            // 
            // GD_Port
            // 
            this.GD_Port.FormattingEnabled = true;
            resources.ApplyResources(this.GD_Port, "GD_Port");
            this.GD_Port.Name = "GD_Port";
            this.GD_Port.SelectedValueChanged += new System.EventHandler(this.GD_Port_SelectedValueChanged);
            // 
            // ConfigFirmware
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.GD_Port);
            this.Controls.Add(this.BTN_Custom_GD_firmware);
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.BTN_Custom_firmware);
            this.Name = "ConfigFirmware";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Button BTN_Custom_firmware;
        private ProgressBar progress;
        private Label lbl_status;
        private Button BTN_Custom_GD_firmware;
        private ComboBox GD_Port;
    }
}