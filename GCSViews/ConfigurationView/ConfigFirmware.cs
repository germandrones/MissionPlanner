using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using MissionPlanner.Arduino;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigFirmware : MyUserControl, IActivate
    {
        private readonly Firmware fw = new Firmware();

        string PX4_Serial_Port = "";
        string GD_Serial_Port = "";

        public ConfigFirmware()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if(PX_port.Items.Count == 0 || GD_Port.Items.Count == 0)
            {
                BUT_FW_Update.Enabled = false;
            }

            TB_UpdaterManual.Text = @"1. Please make sure Mission Planner is not connected to Autopilot.
2. Plug USB Cable into Service USB Socket in Songbird.
3. Click button [Start Bootloader Mode].
4. Click button [Refresh AP Serial Ports].
5. Click button [Upload Firmware from File].
6. Select Firmware archive .zip file.
7. After Firmware update, please re-plug the battery. ";
        }


        private void BUT_FW_Update_Click(object sender, EventArgs e)
        {
            // Cleanup temp files if already exists.
            RemoveTemporaries();
            FW_Uploader_log.Clear();

            using (var fd = new OpenFileDialog { Filter = "Firmware (*.zip)|*.zip;" })
            {
                fd.ShowDialog();

                UnzipArchive(fd.FileName);

                // Upload firmware on PX4
                if (PX4_Serial_Port.Length != 0)
                {
                    add_LogText("Uploading PX Firmware. Please Wait...");
                    if (!upload_PX4_Firmware("firmware_temp.px4", PX4_Serial_Port))
                    {
                        add_LogText("Unable to upload PX Firmware.");
                    }
                }

                //upload a GD Firmware
                if (GD_Serial_Port.Length != 0)
                {
                    add_LogText("Uploading GD Firmware. Please Wait...");
                    if (!upload_PGD_Firmware("firmware_temp.hex", GD_Serial_Port))
                    {
                        add_LogText("Unable to upload GD Firmware.");
                    }
                }
            }
            
            RemoveTemporaries();  
            add_LogText("Done!");
            add_LogText("Please replug battery...");
        }

        #region GD Uploader
        /// <summary>
        /// Search for GD serial port based on Vendor ID
        /// </summary>
        /// <returns></returns>
        private void FindGD_port()
        {
            // list of all available com ports
            string[] ports = System.IO.Ports.SerialPort.GetPortNames().Select(p => p.TrimEnd()).ToArray();
            List<string> portsFound = new List<string>();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity  WHERE Caption like '%(COM%'");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if (queryObj["DeviceID"].ToString().Contains("VID_0403")) // GD FTDI Vendor ID
                    {
                        string devCaption = queryObj["Caption"].ToString();
                        //portsFound.Add(p);

                        foreach (string p in ports)
                        {
                            if (devCaption.Contains("(" + p + ")")) // exactly expression
                            {
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

            // check each FTDI port on connection?
            IArduinoComms port = new ArduinoSTKv2{ BaudRate = 115200 };
            foreach (var p in portsFound)
            {
                try {
                    port.PortName = p;
                    port.Open();

                    if (port.connectAP())
                    {
                        GD_Serial_Port = p;
                        GD_Port.Items.Add(p);
                    }
                }
                catch { }
            }            
        }

        private bool upload_PGD_Firmware(string fileName, string comPort)
        {
            if (File.Exists(fileName))
            {
                fw.Progress += fw_Progress;

                var boardtype = BoardDetect.boards.b2560;
                try
                {
                    return fw.UploadFlash(comPort, fileName, boardtype);
                }
                catch (MissingFieldException)
                {
                    CustomMessageBox.Show("Please update, your install is currupt", Strings.ERROR);
                    return false;
                }                
            }
            return false;
        }

        #endregion

        #region PX4 Uploader
        /// <summary>
        /// Method search for the PX4 Board connected. Search based on Vendor ID
        /// </summary>
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
                                PX_port.Items.Add(p);                                
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

        /// <summary>
        /// Uploads a Pixhawk Firmware file on connected board.
        /// </summary>
        /// <param name="fileName">Firmware Filename</param>
        /// <param name="comPort">Connected Comport</param>
        /// <returns>True if upload successfull, otherwise false</returns>
        private bool upload_PX4_Firmware(string fileName, string comPort)
        {
            if (File.Exists(fileName))
            {
                fw.Progress += fw_Progress;

                var boardtype = BoardDetect.boards.px4v2;

                try
                {
                    return fw.UploadFlash(comPort, fileName, boardtype);
                }
                catch (MissingFieldException)
                {
                    CustomMessageBox.Show("Please update, your install is corrupt", Strings.ERROR);
                    return false;
                }
            }
            return false;
        }
        #endregion

        #region Archive file workers
        /// <summary>
        /// Unzip an archive where stored firmware files
        /// </summary>
        /// <param name="fileName">Path to zip file</param>
        private void UnzipArchive(string fileName)
        {
            if (!File.Exists(fileName)) return;

            //unzip files to the currentfolder
            using (ZipArchive archive = ZipFile.OpenRead(fileName))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".px4", StringComparison.OrdinalIgnoreCase))
                    {
                        entry.ExtractToFile("firmware_temp.px4");
                    }

                    if (entry.FullName.EndsWith(".hex", StringComparison.OrdinalIgnoreCase))
                    {
                        entry.ExtractToFile("firmware_temp.hex");
                    }
                }
            }
        }

        /// <summary>
        /// Removes Temporary files stored in initial directory
        /// </summary>
        private void RemoveTemporaries()
        {
            // remove all temporary files
            if (File.Exists("firmware_temp.px4")) { File.Delete("firmware_temp.px4"); }
            if (File.Exists("firmware_temp.hex")) { File.Delete("firmware_temp.hex"); }
        }
        #endregion

        #region Progress Reporter
        public void add_LogText(string text)
        {
            FW_Uploader_log.AppendText(text + Environment.NewLine);
        }

        private void fw_Progress(int progress, string status)
        {
            if (progress != -1) this.progress.Value = progress;
            lbl_status.Text = status;            
        }        
        #endregion

        private void GD_Port_SelectedIndexChanged(object sender, EventArgs e)
        {
            GD_Serial_Port = GD_Port.SelectedItem.ToString();
        }

        private void PX_port_SelectedIndexChanged(object sender, EventArgs e)
        {
            PX4_Serial_Port = PX_port.SelectedItem.ToString();
        }

        private void BUT_Serial_Refresh_Click(object sender, EventArgs e)
        {
            // reboot to bootloader
            add_LogText("Trying to find FW Board...");
            PX_port.Items.Clear();
            FindPX4_port();
            if (PX_port.Items.Count > 0) { PX_port.SelectedIndex = 0; } else add_LogText("FW Board not found!");

            add_LogText("Trying to find GD Board...");
            GD_Port.Items.Clear();
            FindGD_port();
            if (GD_Port.Items.Count > 0) { GD_Port.SelectedIndex = 0; } else add_LogText("GD Board not found!");


            if (PX_port.Items.Count == 0 || GD_Port.Items.Count == 0)
            {
                BUT_FW_Update.Enabled = false;
            }
            else
            {
                BUT_FW_Update.Enabled = true;
                add_LogText("Ready");
            }            
        }

        private void myButton1_Click(object sender, EventArgs e)
        {
            // reboot px4 in bootloader mode...
            try
            {
                MainV2.comPort.BaseStream.Open();
                MainV2.comPort.giveComport = true;
                if (MainV2.comPort.getHeartBeat().Length > 0)
                {
                    MainV2.comPort.doReboot(true, false);
                    MainV2.comPort.Close();
                    MainV2.comPort.BaseStream.Close();
                }
                else
                {
                    MainV2.comPort.BaseStream.Close();
                }
            }
            catch { }
            add_LogText("Bootloader Mode is enabled...");
        }
    }

}