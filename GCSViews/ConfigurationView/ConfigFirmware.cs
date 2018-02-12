using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Arduino;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigFirmware : MyUserControl, IActivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);        
        private readonly Firmware fw = new Firmware();
        private string custom_fw_dir = "";
        
        // Comport where PX4 board is connected
        string PX4_Serial_Port = "";
        
        // Comport where GDPilot board is connected
        string GD_Serial_Port = "";

        private IProgressReporterDialogue pdr;

        public ConfigFirmware()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            FindGD_port();
        }

        private void FindPX4_port()
        {
            // list of all available com ports
            string[] ports = System.IO.Ports.SerialPort.GetPortNames().Select(p => p.TrimEnd()).ToArray();

            try
            {
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SerialPort");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if (queryObj.Properties["PNPDeviceID"].Value.ToString().Contains(@"USB\VID_26AC&PID_0011")) // PX4 Vendor ID
                    {
                        string devCaption = queryObj["Caption"].ToString();
                        foreach (string p in ports)
                        {
                            if (devCaption.Contains(p))
                            {                                
                                PX4_Serial_Port = p; // px4 port found!
                            }

                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("PX4 Board is not connected!");
            }
        }

        private void BTN_Custom_firmware_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog { Filter = "Firmware (*.hex;*.px4;*.vrx;*.apj)|*.hex;*.px4;*.vrx;*.apj|All files (*.*)|*.*" })
            {
                if (Directory.Exists(custom_fw_dir)) fd.InitialDirectory = custom_fw_dir;
                fd.ShowDialog();

                if (File.Exists(fd.FileName))
                {
                    custom_fw_dir = Path.GetDirectoryName(fd.FileName);

                    FindPX4_port(); if (PX4_Serial_Port.Length == 0){ return; }
                    
                    fw.Progress -= fw_Progress;
                    fw.Progress += fw_Progress1;

                    var boardtype = BoardDetect.boards.none;
                    if (fd.FileName.ToLower().EndsWith(".px4") || fd.FileName.ToLower().EndsWith(".apj"))
                    {
                        boardtype = BoardDetect.boards.px4v2;
                    }
                    fw.UploadFlash(PX4_Serial_Port, fd.FileName, boardtype);
                }
            }
        }

        private void FindGD_port()
        {
            // list of all available com ports
            string[] ports = System.IO.Ports.SerialPort.GetPortNames().Select(p => p.TrimEnd()).ToArray();

            List<string> portsFound = new List<string>();
            GD_Port.Items.Clear();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity  WHERE Caption like '%(COM%'");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if (queryObj["DeviceID"].ToString().Contains("VID_0403")) // GD FTDI Vendor ID
                    {
                        string devCaption = queryObj["Caption"].ToString();
                        foreach (string p in ports)
                        {
                            if (devCaption.Contains(p))
                            {
                                GD_Serial_Port = p; // px4 port found!
                                GD_Port.Items.Add(p);
                                portsFound.Add(p);
                            }

                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("PX4 Board is not connected!");
            }
        }

        private void BTN_Custom_GD_firmware_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog { Filter = "Firmware (*.hex)|*.hex;" })
            {
                if (Directory.Exists(custom_fw_dir)) fd.InitialDirectory = custom_fw_dir;
                fd.ShowDialog();

                if (File.Exists(fd.FileName))
                {
                    custom_fw_dir = Path.GetDirectoryName(fd.FileName);                    

                    fw.Progress -= fw_Progress;
                    fw.Progress += fw_Progress1;

                    var boardtype = BoardDetect.boards.b2560;

                    fw.UploadArduino(GD_Serial_Port, fd.FileName, boardtype);
                }
            }
            
        }


        private void fw_Progress(int progress, string status)
        {
            pdr.UpdateProgressAndStatus(progress, status);
        }

        private void fw_Progress1(int progress, string status)
        {
            var change = false;

            if (progress != -1)
            {
                if (this.progress.Value != progress)
                {
                    this.progress.Value = progress;
                    change = true;
                }
            }
            if (lbl_status.Text != status)
            {
                lbl_status.Text = status;
                change = true;
            }

            if (change)
                Refresh();
        }

        private void GD_Port_SelectedValueChanged(object sender, EventArgs e)
        {
            GD_Serial_Port = GD_Port.SelectedItem.ToString();
        }
    }
}