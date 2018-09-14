﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWBT : UserControl, IActivate
    {
        private readonly Dictionary<int, int> baudmap = new Dictionary<int, int>
        {
            {57600, 7},
            {38400, 6},
            {9600, 4},
            {19200, 5},
            {115200, 8},
            {1200, 1},
            {2400, 2},
            {4800, 3}
        };

        public ConfigHWBT()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (MainV2.comPort.BaseStream.IsOpen)
            {
                Enabled = false;
            }
            Enabled = true;
        }

        private void BUT_btsettings_Click(object sender, EventArgs e)
        {
            string[] commands =
            {
                "AT",
                "AT+VERSION",
                string.Format("AT+ROLE={0}\r\n", 0),
                string.Format("AT+NAME={0}\r\n", txt_name.Text),
                string.Format("AT+NAME{0}", txt_name.Text),
                string.Format("AT+BAUD={0}\r\n", cmb_baud.Text),
                string.Format("AT+BAUD{0}", baudmap[int.Parse(cmb_baud.Text)]),
                string.Format("AT+PSWD={0}\r\n", txt_pin.Text),
                string.Format("AT+PIN{0}", txt_pin.Text),
                "AT+RESET"
            };

            var pass = false;

            foreach (var baud in baudmap)
            {
                using (var port = new SerialPort(MainV2.comPortName, baud.Key))
                {
                    try
                    {
                        port.Open();
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show(Strings.SelectComport + " " + ex.ToString(), Strings.ERROR);
                        return;
                    }

                    port.Write("AT");

                    Thread.Sleep(1100);

                    port.Write("\r\n");

                    Thread.Sleep(200);

                    var isok = port.ReadExisting();

                    if (isok.Contains("OK"))
                    {
                        foreach (var cmd in commands)
                        {
                            port.Write(cmd);
                            Thread.Sleep(1000);
                        }

                        pass = true;
                        break;
                    }
                    Thread.Sleep(1100);
                }
            }

            if (!pass)
                CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            else
                CustomMessageBox.Show(Strings.ProgrammedOK);
        }
    }
}