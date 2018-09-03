using log4net;
using MissionPlanner.Utilities;
using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

namespace MissionPlanner.CamJoystick
{
    public class CamJoystick : IDisposable
    {
        public static bool[] CamJoyButtons = new bool[1];
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static SharpDX.DirectInput.DirectInput directInput = new SharpDX.DirectInput.DirectInput();
        public static MissionPlanner.CamJoystick.CamJoystick.JoyChannel[] JoyChannels = new MissionPlanner.CamJoystick.CamJoystick.JoyChannel[20];
        public static MissionPlanner.CamJoystick.CamJoystick.JoyButton[] JoyButtons = new MissionPlanner.CamJoystick.CamJoystick.JoyButton[128];
        private static bool m_iPolarity_bool = false;
        private static byte prev_Polarity = byte.MaxValue;
        private static bool m_iCameraType_bool = false;
        private static byte prev_m_iCameraType = byte.MaxValue;
        private static bool m_iRecord_bool = false;
        private static byte prev_m_iRecord = byte.MaxValue;
        private static MissionPlanner.CamJoystick.CamJoystick.Tracker_State m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.IDLE;
        private static byte prev_m_iTracker = byte.MaxValue;
        private static byte prev_m_iReTracker = byte.MaxValue;
        private static bool m_iSingleYaw_bool = false;
        private static byte prev_m_iSingleYaw = byte.MaxValue;
        private static bool m_iRetracting_bool = false;
        private static byte prev_Retracting = byte.MaxValue;
        private static bool m_iFollowTarget_bool = false;
        private static byte prev_FollowTarget = byte.MaxValue;

        private static bool m_iSwitchMode_bool = false;
        private static byte prev_SwitchMode = byte.MaxValue;

        public bool enabled = false;
        private bool[] buttonpressed = new bool[128];
        public bool elevons = false;
        private string joystickconfigbutton = "Camjoystickbuttons.xml";
        private string joystickconfigaxis = "Camjoystickaxis.xml";
        private int hat1 = (int)short.MaxValue;
        private int hat2 = (int)short.MaxValue;
        private int custom0 = (int)short.MaxValue;
        private int custom1 = (int)short.MaxValue;
        private SharpDX.DirectInput.Joystick Camjoystick;
        private CamJoystickState state;
        public string name;
        public static MissionPlanner.CamJoystick.CamJoystick self;
        private const int RESXu = 1024;
        private const int RESXul = 1024;
        private const int RESXl = 1024;
        private const int RESKul = 100;

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing && this.Camjoystick != null && this.Camjoystick.Properties != null)
                    this.Camjoystick.Unacquire();
            }
            catch
            {
            }
            try
            {
                if (disposing && this.Camjoystick != null)
                    this.Camjoystick.Dispose();
            }
            catch
            {
            }
            GC.SuppressFinalize((object)this);
        }

        public CamJoystick()
        {
            MissionPlanner.CamJoystick.CamJoystick.self = this;
            for (int index = 0; index < MissionPlanner.CamJoystick.CamJoystick.JoyButtons.Length; ++index)
                MissionPlanner.CamJoystick.CamJoystick.JoyButtons[index].buttonno = -1;
            this.loadconfig("Camjoystickbuttons" + (object)MainV2.comPort.MAV.cs.firmware + ".xml", "Camjoystickaxis" + (object)MainV2.comPort.MAV.cs.firmware + ".xml");
        }


        public void loadconfig(string joystickconfigbutton = "Camjoystickbuttons.xml", string joystickconfigaxis = "Camjoystickaxis.xml")
        {
            MissionPlanner.CamJoystick.CamJoystick.log.Info((object)("Loading Camjoystick config files " + joystickconfigbutton + " " + joystickconfigaxis));
            this.joystickconfigbutton = Settings.GetUserDataDirectory() + joystickconfigbutton;
            this.joystickconfigaxis = Settings.GetUserDataDirectory() + joystickconfigaxis;
            if (!File.Exists(this.joystickconfigbutton) || !File.Exists(this.joystickconfigaxis))
                return;
            try
            {
                using (StreamReader streamReader = new StreamReader(this.joystickconfigbutton))
                    MissionPlanner.CamJoystick.CamJoystick.JoyButtons = (MissionPlanner.CamJoystick.CamJoystick.JoyButton[])new XmlSerializer(typeof(MissionPlanner.CamJoystick.CamJoystick.JoyButton[]), new Type[1]
                    {
            typeof (MissionPlanner.CamJoystick.CamJoystick.JoyButton)
                    }).Deserialize((TextReader)streamReader);
            }
            catch
            {
            }
            try
            {
                using (StreamReader streamReader = new StreamReader(this.joystickconfigaxis))
                    MissionPlanner.CamJoystick.CamJoystick.JoyChannels = (MissionPlanner.CamJoystick.CamJoystick.JoyChannel[])new XmlSerializer(typeof(MissionPlanner.CamJoystick.CamJoystick.JoyChannel[]), new Type[1]
                    {
            typeof (MissionPlanner.CamJoystick.CamJoystick.JoyChannel)
                    }).Deserialize((TextReader)streamReader);
            }
            catch
            {
            }
        }

        public void saveconfig()
        {
            MissionPlanner.CamJoystick.CamJoystick.log.Info((object)("Saving Camjoystick config files " + this.joystickconfigbutton + " " + this.joystickconfigaxis));
            using (StreamWriter streamWriter = new StreamWriter(this.joystickconfigbutton))
                new XmlSerializer(typeof(MissionPlanner.CamJoystick.CamJoystick.JoyButton[]), new Type[1]
                {
          typeof (MissionPlanner.CamJoystick.CamJoystick.JoyButton)
                }).Serialize((TextWriter)streamWriter, (object)MissionPlanner.CamJoystick.CamJoystick.JoyButtons);
            using (StreamWriter streamWriter = new StreamWriter(this.joystickconfigaxis))
                new XmlSerializer(typeof(MissionPlanner.CamJoystick.CamJoystick.JoyChannel[]), new Type[1]
                {
          typeof (MissionPlanner.CamJoystick.CamJoystick.JoyChannel)
                }).Serialize((TextWriter)streamWriter, (object)MissionPlanner.CamJoystick.CamJoystick.JoyChannels);
        }

        public static IList<DeviceInstance> getDevices()
        {
            return MissionPlanner.CamJoystick.CamJoystick.directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
        }

        public static SharpDX.DirectInput.Joystick getJoyStickByName(string name)
        {
            IList<DeviceInstance> devices = MissionPlanner.CamJoystick.CamJoystick.directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            int num = 1;
            foreach (DeviceInstance deviceInstance in (IEnumerable<DeviceInstance>)devices)
            {
                if (deviceInstance.ProductName + " # " + (object)num == name)
                    return new SharpDX.DirectInput.Joystick(MissionPlanner.CamJoystick.CamJoystick.directInput, deviceInstance.InstanceGuid);
                ++num;
            }
            return (SharpDX.DirectInput.Joystick)null;
        }

        public SharpDX.DirectInput.Joystick AcquireJoystick(string name)
        {
            this.Camjoystick = MissionPlanner.CamJoystick.CamJoystick.getJoyStickByName(name);
            if (this.Camjoystick == null)
                return (SharpDX.DirectInput.Joystick)null;
            this.Camjoystick.Acquire();
            Thread.Sleep(500);
            this.Camjoystick.Poll();
            return this.Camjoystick;
        }

        public bool start(string name)
        {
            MissionPlanner.CamJoystick.CamJoystick.self.name = name;
            this.Camjoystick = this.AcquireJoystick(name);
            if (this.Camjoystick == null)
                return false;
            this.enabled = true;
            new Thread(new ThreadStart(this.mainloop))
            {
                Name = "CamJoystick loop",
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true
            }.Start();
            return true;
        }

        public static MissionPlanner.CamJoystick.CamJoystick.joystickaxis getMovingAxis(string name, int threshold)
        {
            MissionPlanner.CamJoystick.CamJoystick.self.name = name;
            //SharpDX.DirectInput.Joystick CamJStick = new MissionPlanner.CamJoystick.CamJoystick().AcquireJoystick(name);
            var CamJStick = new MissionPlanner.CamJoystick.CamJoystick().AcquireJoystick(name);
            if (CamJStick == null) return MissionPlanner.CamJoystick.CamJoystick.joystickaxis.None;

            Thread.Sleep(50);
            CamJoystickState camJoystickState1 = CamJStick.CurrentCamJoystickState();

            Hashtable hashtable = new Hashtable();
            foreach (PropertyInfo property in camJoystickState1.GetType().GetProperties())
            {
                hashtable[(object)property.Name] = (object)int.Parse(property.GetValue((object)camJoystickState1, (object[])null).ToString());
            }

            hashtable[(object)"Slider1"] = (object)camJoystickState1.GetSlider()[0];
            hashtable[(object)"Slider2"] = (object)camJoystickState1.GetSlider()[1];
            hashtable[(object)"Hatud1"] = (object)camJoystickState1.GetPointOfView()[0];
            hashtable[(object)"Hatlr2"] = (object)camJoystickState1.GetPointOfView()[0];
            hashtable[(object)"Custom1"] = (object)0;
            hashtable[(object)"Custom2"] = (object)0;

            int num1 = (int)CustomMessageBox.Show("Please move the Camjoystick axis you want assigned to this function after clicking ok");

            DateTime now = DateTime.Now;
            while (now.AddSeconds(10.0) > DateTime.Now)
            {
                CamJStick.Poll();
                Thread.Sleep(50);
                CamJoystickState camJoystickState2 = CamJStick.CurrentCamJoystickState();
                camJoystickState2.GetSlider();

                int[] pointOfView = camJoystickState2.GetPointOfView();
                foreach (PropertyInfo property in camJoystickState2.GetType().GetProperties())
                {
                    MissionPlanner.CamJoystick.CamJoystick.log.InfoFormat("test name {0} old {1} new {2} ", (object)property.Name, hashtable[(object)property.Name], (object)int.Parse(property.GetValue((object)camJoystickState2, (object[])null).ToString()));
                    MissionPlanner.CamJoystick.CamJoystick.log.InfoFormat("{0}  {1} {2}", (object)property.Name, (object)(int)hashtable[(object)property.Name], (object)(int.Parse(property.GetValue((object)camJoystickState2, (object[])null).ToString()) + threshold));
                    if ((int)hashtable[(object)property.Name] > int.Parse(property.GetValue((object)camJoystickState2, (object[])null).ToString()) + threshold || (int)hashtable[(object)property.Name] < int.Parse(property.GetValue((object)camJoystickState2, (object[])null).ToString()) - threshold)
                    {
                        MissionPlanner.CamJoystick.CamJoystick.log.Info((object)property.Name);
                        CamJStick.Unacquire();
                        return (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), property.Name);
                    }
                }

                if ((int)hashtable[(object)"Hatud1"] != pointOfView[0])
                {
                    CamJStick.Unacquire();
                    return MissionPlanner.CamJoystick.CamJoystick.joystickaxis.HatUpDown;
                }

                if ((int)hashtable[(object)"Hatlr2"] != pointOfView[0])
                {
                    CamJStick.Unacquire();
                    return MissionPlanner.CamJoystick.CamJoystick.joystickaxis.HatLeftRight;
                }
            }

            int num2 = (int)CustomMessageBox.Show("No valid option was detected");
            return MissionPlanner.CamJoystick.CamJoystick.joystickaxis.None;
        }

        public static int getPressedButton(string name)
        {
            MissionPlanner.CamJoystick.CamJoystick.self.name = name;
            var joyStickByName = MissionPlanner.CamJoystick.CamJoystick.getJoyStickByName(name);
            if (joyStickByName == null)
                return -1;
            joyStickByName.Acquire();
            Thread.Sleep(500);
            joyStickByName.Poll();
            bool[] buttons1 = joyStickByName.CurrentCamJoystickState().GetButtons();
            int num1 = (int)CustomMessageBox.Show("Please press the Camjoystick button you want assigned to this function after clicking ok");
            DateTime now = DateTime.Now;
            while (now.AddSeconds(10.0) > DateTime.Now)
            {
                joyStickByName.Poll();
                bool[] buttons2 = joyStickByName.CurrentCamJoystickState().GetButtons();
                for (int index = 0; index < joyStickByName.Capabilities.ButtonCount; ++index)
                {
                    if (buttons2[index] != buttons1[index])
                        return index;
                }
            }
            int num2 = (int)CustomMessageBox.Show("No valid option was detected");
            return -1;
        }

        public void setReverse(int channel, bool reverse)
        {
            MissionPlanner.CamJoystick.CamJoystick.JoyChannels[channel].reverse = reverse;
        }

        public void setAxis(int channel, MissionPlanner.CamJoystick.CamJoystick.joystickaxis axis)
        {
            MissionPlanner.CamJoystick.CamJoystick.JoyChannels[channel].axis = axis;
        }

        public void setChannel(int channel, MissionPlanner.CamJoystick.CamJoystick.joystickaxis axis, bool reverse, int expo)
        {
            MissionPlanner.CamJoystick.CamJoystick.JoyChannels[channel] = new MissionPlanner.CamJoystick.CamJoystick.JoyChannel()
            {
                axis = axis,
                channel = channel,
                expo = expo,
                reverse = reverse
            };
        }

        public void setChannel(MissionPlanner.CamJoystick.CamJoystick.JoyChannel chan)
        {
            MissionPlanner.CamJoystick.CamJoystick.JoyChannels[chan.channel] = chan;
        }

        public MissionPlanner.CamJoystick.CamJoystick.JoyChannel getChannel(int channel)
        {
            return MissionPlanner.CamJoystick.CamJoystick.JoyChannels[channel];
        }

        public void setButton(int arrayoffset, MissionPlanner.CamJoystick.CamJoystick.JoyButton buttonconfig)
        {
            MissionPlanner.CamJoystick.CamJoystick.JoyButtons[arrayoffset] = buttonconfig;
        }

        public MissionPlanner.CamJoystick.CamJoystick.JoyButton getButton(int arrayoffset)
        {
            return MissionPlanner.CamJoystick.CamJoystick.JoyButtons[arrayoffset];
        }

        public void changeButton(int buttonid, int newid)
        {
            MissionPlanner.CamJoystick.CamJoystick.JoyButtons[buttonid].buttonno = newid;
        }

        public int getHatSwitchDirection()
        {
            return this.state.GetPointOfView()[0];
        }

        public int getNumberPOV()
        {
            return this.Camjoystick.Capabilities.PovCount;
        }

        private int BOOL_TO_SIGN(bool input)
        {
            return input ? -1 : 1;
        }

        private void mainloop()
        {
            while (this.enabled && this.Camjoystick != null)
            {
                try
                {
                    Thread.Sleep(100); // The same 10 Hz as V2_Extension message
                    this.Camjoystick.Poll();
                    this.state = this.Camjoystick.CurrentCamJoystickState();
                    if (this.getNumberPOV() > 0)
                    {
                        int hatSwitchDirection = this.getHatSwitchDirection();
                        if (hatSwitchDirection != -1)
                        {
                            int num = hatSwitchDirection / 100;
                            if (num > 270 || num < 90)
                                this.hat1 += 2500;
                            if (num > 90 && num < 270)
                                this.hat1 -= 2500;
                            if (num > 0 && num < 180)
                                this.hat2 += 2500;
                            if (num > 180 && num < 360)
                                this.hat2 -= 2500;
                        }
                        else
                        {
                            this.hat1 = (int)short.MaxValue;
                            this.hat2 = (int)short.MaxValue;
                        }
                    }

                    if ((uint)this.getJoystickAxis(1) > 0U)
                    {
                        MainV2.comPort.MAV.cs.colibri_ch1 = this.pickchannel(1, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[1].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[1].reverse, 0);
                        MainV2.comPort.MAV.cs.colibri_ch1_rev = this.pickchannel(1, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[1].axis, !MissionPlanner.CamJoystick.CamJoystick.JoyChannels[1].reverse, 0);
                    }

                    if ((uint)this.getJoystickAxis(2) > 0U)
                    {
                        MainV2.comPort.MAV.cs.colibri_ch2 = this.pickchannel(2, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[2].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[2].reverse, 0);
                    }


                    if ((uint)this.getJoystickAxis(3) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch3 = this.pickchannel(3, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[3].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[3].reverse, 0);
                    if ((uint)this.getJoystickAxis(4) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch4 = this.pickchannel(4, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[4].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[4].reverse, 0);
                    if ((uint)this.getJoystickAxis(5) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch5 = this.pickchannel(5, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[5].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[5].reverse, 0);
                    if ((uint)this.getJoystickAxis(6) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch6 = this.pickchannel(6, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[6].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[6].reverse, 0);
                    if ((uint)this.getJoystickAxis(7) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch7 = this.pickchannel(7, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[7].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[7].reverse, 0);
                    if ((uint)this.getJoystickAxis(8) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch8 = this.pickchannel(8, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[8].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[8].reverse, 0);
                    if ((uint)this.getJoystickAxis(9) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch9 = this.pickchannel(9, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[9].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[9].reverse, 0);
                    if ((uint)this.getJoystickAxis(10) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch10 = this.pickchannel(10, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[10].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[10].reverse, 0);
                    if ((uint)this.getJoystickAxis(11) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch11 = this.pickchannel(11, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[11].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[11].reverse, 0);
                    if ((uint)this.getJoystickAxis(12) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch12 = this.pickchannel(12, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[12].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[12].reverse, 0);
                    if ((uint)this.getJoystickAxis(13) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch13 = this.pickchannel(13, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[13].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[13].reverse, 0);
                    if ((uint)this.getJoystickAxis(14) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch14 = this.pickchannel(14, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[14].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[14].reverse, 0);
                    if ((uint)this.getJoystickAxis(15) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch15 = this.pickchannel(15, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[15].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[15].reverse, 0);
                    if ((uint)this.getJoystickAxis(16) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch16 = this.pickchannel(16, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[16].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[16].reverse, 0);
                    if ((uint)this.getJoystickAxis(17) > 0U)
                        MainV2.comPort.MAV.cs.colibri_ch17 = this.pickchannel(17, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[17].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[17].reverse, 0);

                    if (MainV2.comPort.BaseStream.IsOpen) this.DoJoystickButtonFunction();
                }
                catch (SharpDXException ex)
                {
                    MissionPlanner.CamJoystick.CamJoystick.log.Error((object)ex);
                    int num;
                }
                catch (Exception ex)
                {
                    MissionPlanner.CamJoystick.CamJoystick.log.Info((object)("Joystick thread error " + ex.ToString()));
                }
            }
        }

        public void DoJoystickButtonFunction()
        {
            foreach (MissionPlanner.CamJoystick.CamJoystick.JoyButton joyButton in MissionPlanner.CamJoystick.CamJoystick.JoyButtons)
            {
                if (joyButton.buttonno != -1)
                    this.getButtonState(joyButton, joyButton.buttonno);
            }
        }

        private void ProcessButtonEvent(MissionPlanner.CamJoystick.CamJoystick.JoyButton but, bool buttondown)
        {
            if (but.buttonno == -1 || !buttondown && (but.function != MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Do_Set_Relay && but.function != MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Button_axis0 && but.function != MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Button_axis1))
                return;
            switch (but.function)
            {
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.ChangeMode:
                    string mode = but.mode;
                    if (mode != null)
                    {
                        MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                        {
                            try
                            {
                                MainV2.comPort.setMode(mode);
                            }
                            catch
                            {
                                int num = (int)CustomMessageBox.Show("Failed to change Modes");
                            }
                        });
                        break;
                    }
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Do_Set_Relay:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_RELAY, (float)(int)but.p1, buttondown ? 1f : 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, true);
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to DO_SET_RELAY");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Do_Repeat_Relay:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_REPEAT_RELAY, (float)(int)but.p1, (float)(int)but.p2, (float)(int)but.p3, 0.0f, 0.0f, 0.0f, 0.0f, true);
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to DO_REPEAT_RELAY");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Do_Set_Servo:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, (float)(int)but.p1, (float)(int)but.p2, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, true);
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to DO_SET_SERVO");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Do_Repeat_Servo:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_REPEAT_SERVO, (float)(int)but.p1, (float)(int)but.p2, (float)(int)but.p3, (float)(int)but.p4, 0.0f, 0.0f, 0.0f, true);
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to DO_REPEAT_SERVO");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Arm:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            MainV2.comPort.doARM(true);
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to Arm");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Disarm:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            MainV2.comPort.doARM(false);
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to Disarm");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Digicam_Control:
                    MainV2.comPort.setDigicamControl(true);
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.TakeOff:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            MainV2.comPort.setMode("Guided");
                            if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2)
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 2f, true);
                            else
                                MainV2.comPort.doCommand(MAVLink.MAV_CMD.TAKEOFF, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 20f, true);
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to takeoff");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Mount_Mode:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            MainV2.comPort.setParam("MNT_MODE", (double)but.p1, false);
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to change mount mode");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Toggle_Pan_Stab:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            MainV2.comPort.setParam("MNT_STAB_PAN", (double)(float)MainV2.comPort.MAV.param["MNT_STAB_PAN"] > 0.0 ? 0.0 : 1.0, false);
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to Toggle_Pan_Stab");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Gimbal_pnt_track:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_ROI, 0.0f, 0.0f, 0.0f, 0.0f, MainV2.comPort.MAV.cs.gimballat, MainV2.comPort.MAV.cs.gimballng, (float)MainV2.comPort.MAV.cs.GimbalPoint.Alt, true);
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to Gimbal_pnt_track");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Mount_Control_0:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            MainV2.comPort.setMountControl(0.0, 0.0, 0.0, false);
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to Mount_Control_0");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Button_axis0:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            int p1 = (int)but.p1;
                            int p2 = (int)but.p2;
                            if (buttondown)
                                this.custom0 = p2;
                            else
                                this.custom0 = p1;
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to Button_axis0");
                        }
                    });
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.buttonfunction.Button_axis1:
                    MainV2.instance.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate ()
                    {
                        try
                        {
                            int p1 = (int)but.p1;
                            int p2 = (int)but.p2;
                            if (buttondown)
                                this.custom1 = p2;
                            else
                                this.custom1 = p1;
                        }
                        catch
                        {
                            int num = (int)CustomMessageBox.Show("Failed to Button_axis1");
                        }
                    });
                    break;
            }
        }

        public void UnAcquireJoyStick()
        {
            if (this.Camjoystick == null)
                return;
            this.Camjoystick.Unacquire();
        }

        private bool getButtonState(MissionPlanner.CamJoystick.CamJoystick.JoyButton but, int buttonno)
        {
            bool[] buttons = this.state.GetButtons();
            if (buttons[buttonno] && !this.buttonpressed[buttonno])
                this.ButtonDown(but);
            bool flag = !buttons[buttonno] && this.buttonpressed[buttonno];
            if (flag)
                this.ButtonUp(but);
            this.buttonpressed[buttonno] = buttons[buttonno];
            return flag;
        }

        private void ButtonDown(MissionPlanner.CamJoystick.CamJoystick.JoyButton but)
        {
            this.ProcessButtonEvent(but, true);
        }

        private void ButtonUp(MissionPlanner.CamJoystick.CamJoystick.JoyButton but)
        {
            this.ProcessButtonEvent(but, false);
        }

        public int getNumButtons()
        {
            if (this.Camjoystick == null)
                return 0;
            return this.Camjoystick.Capabilities.ButtonCount;
        }

        public MissionPlanner.CamJoystick.CamJoystick.joystickaxis getJoystickAxis(int channel)
        {
            try
            {
                return MissionPlanner.CamJoystick.CamJoystick.JoyChannels[channel].axis;
            }
            catch
            {
                return MissionPlanner.CamJoystick.CamJoystick.joystickaxis.None;
            }
        }

        public bool isButtonPressed(int buttonno)
        {
            bool[] buttons = this.state.GetButtons();
            if (buttons == null || MissionPlanner.CamJoystick.CamJoystick.JoyButtons[buttonno].buttonno < 0)
                return false;
            return buttons[MissionPlanner.CamJoystick.CamJoystick.JoyButtons[buttonno].buttonno];
        }

        public ushort getValueForChannel(int channel, string name)
        {
            if (this.Camjoystick == null)
                return 0;
            this.Camjoystick.Poll();
            this.state = this.Camjoystick.CurrentCamJoystickState();
            ushort num = this.pickchannel(channel, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[channel].axis, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[channel].reverse, 0);
            MissionPlanner.CamJoystick.CamJoystick.log.DebugFormat("{0} = {1} = {2}", (object)channel, (object)num, (object)this.state.X);
            return num;
        }

        public ushort getRawValueForChannel(int channel)
        {
            if (this.Camjoystick == null)
                return 0;
            this.Camjoystick.Poll();
            this.state = this.Camjoystick.CurrentCamJoystickState();
            ushort num = this.pickchannel(channel, MissionPlanner.CamJoystick.CamJoystick.JoyChannels[channel].axis, false, 0);
            MissionPlanner.CamJoystick.CamJoystick.log.DebugFormat("{0} = {1} = {2}", (object)channel, (object)num, (object)this.state.X);
            return num;
        }

        private ushort pickchannel(int chan, MissionPlanner.CamJoystick.CamJoystick.joystickaxis axis, bool rev, int expo)
        {
            if (axis == MissionPlanner.CamJoystick.CamJoystick.joystickaxis.None)
                return 0;
            int val1_1 = 0;
            int val1_2 = 1023;
            int num1 = 512;
            Math.Abs(val1_2 - val1_1);
            int num2 = 0;
            MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons = this.state.GetButtons();
            bool flag = false;
            switch (axis)
            {
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.None:
                    num2 = (int)short.MaxValue;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.X:
                    num2 = this.state.X;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.Y:
                    num2 = this.state.Y;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.Z:
                    num2 = this.state.Z;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.RZ:
                    num2 = this.state.RZ;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.HatUpDown:
                    this.hat1 = (int)this.Constrain((double)this.hat1, 0.0, (double)ushort.MaxValue);
                    num2 = this.hat1;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.HatLeftRight:
                    this.hat2 = (int)this.Constrain((double)this.hat2, 0.0, (double)ushort.MaxValue);
                    num2 = this.hat2;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn1:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[0]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn2:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[1]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn3:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[2]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn4:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[3]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn5:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[4]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn6:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[5]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn7:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[6]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn8:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[7]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn9:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[8]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn10:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[9]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn11:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[10]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn12:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[11]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn13:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[12]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn14:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[13]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn15:
                    num2 = 1000 + 1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[14]);
                    flag = true;
                    break;
                case MissionPlanner.CamJoystick.CamJoystick.joystickaxis.btn16:
                    num2 = 1000 * (1000 * (int)Convert.ToInt16(MissionPlanner.CamJoystick.CamJoystick.CamJoyButtons[15]));
                    flag = true;
                    break;
            }
            if (flag)
            {
                if (rev)
                    num2 = num2 != 1000 ? 1000 : 2000;
                return (ushort)num2;
            }
            int num3 = (int)((double)num2 / 65.535) - 500;
            if (rev)
                num3 *= -1;
            int val2_1 = (int)MissionPlanner.CamJoystick.CamJoystick.Expo((double)num3, (double)expo, (double)val1_1, (double)val1_2, (double)num1);
            if (val2_1 > 507 && val2_1 < 517)
                val2_1 = 512;
            int val2_2 = Math.Max(val1_1, val2_1);
            return (ushort)Math.Min(val1_2, val2_2);
        }

        public static double Expo(double input, double expo, double min, double max, double mid)
        {
            double num1 = expo / 100.0;
            if (input >= 0.0)
            {
                double num2 = MissionPlanner.CamJoystick.CamJoystick.map(input, 0.0, 500.0, mid, max);
                double num3 = (max - mid) / 2.0;
                double num4 = input <= 250.0 ? input : 250.0 - (input - 250.0);
                return num2 - num4 * num1;
            }
            double num5 = MissionPlanner.CamJoystick.CamJoystick.map(input, -500.0, 0.0, min, mid);
            double num6 = (mid - min) / 2.0;
            double num7 = input >= -250.0 ? input : -250.0 - (input + 250.0);
            return num5 - num7 * num1;
        }

        private static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private double Constrain(double value, double min, double max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        public struct JoyChannel
        {
            public int channel;
            public MissionPlanner.CamJoystick.CamJoystick.joystickaxis axis;
            public bool reverse;
            public int expo;
        }

        public struct JoyButton
        {
            public int buttonno;
            public MissionPlanner.CamJoystick.CamJoystick.buttonfunction function;
            public string mode;
            public float p1;
            public float p2;
            public float p3;
            public float p4;
            public bool state;
        }

        public enum buttonfunction
        {
            ChangeMode,
            Do_Set_Relay,
            Do_Repeat_Relay,
            Do_Set_Servo,
            Do_Repeat_Servo,
            Arm,
            Disarm,
            Digicam_Control,
            TakeOff,
            Mount_Mode,
            Toggle_Pan_Stab,
            Gimbal_pnt_track,
            Mount_Control_0,
            Button_axis0,
            Button_axis1,
        }

        public enum joystickaxis
        {
            None,
            X,
            Y,
            Z,
            RZ,
            HatUpDown,
            HatLeftRight,
            btn1,
            btn2,
            btn3,
            btn4,
            btn5,
            btn6,
            btn7,
            btn8,
            btn9,
            btn10,
            btn11,
            btn12,
            btn13,
            btn14,
            btn15,
            btn16,
        }

        public enum Tracker_State
        {
            IDLE,
            ENABLED,
            TRACK,
            RETRACK,
            TRACK_ON_POS1,
            TRACK_ON_POS2,
        }

        public class Colibri_Protocol
        {
            private byte m_iProtocolType = 119;
            private byte m_iCameraType = 0;
            private byte m_iLaserMode = 0;
            private byte m_iPictureInPicture = 0;
            private byte m_iCameraMode = 6;
            private byte m_iStabilization = 0;
            private byte m_iOsdText = 0;
            private byte m_iOsdGraphic = 1;
            private byte m_iTec = 0;
            private byte m_iFreeze = 0;
            private byte m_iRateCalc = 1;
            private byte m_iThermalColorType = 0;
            private byte m_iNuc = 0;
            private byte m_iPolarity = 0;
            private byte m_iRetracting = 0;
            private byte m_iFollowTarget = 0;
            private byte m_iSwitchMode = 0;
            private byte m_iGain = 0;
            private byte m_iLevel = 0;
            private byte m_iRecord = 0;
            private byte m_iPicCapture = 0;
            private byte m_iTracker = 0;
            private byte m_iReTracker = 0;
            private byte m_iPictureInPictureMode = 0;
            private byte m_iZoomIn = 0;
            private byte m_iZoomOut = 0;
            private ushort m_iPitch = 512;
            private ushort m_iRoll = 512;
            private ushort m_iYaw = 512;
            private byte m_iWriteCorrelatorIndex = 0;
            private byte m_iReadCorrelatorIndex = 0;
            private int m_iTrackOnPos_x = 0;
            private int m_iTrackOnPos_y = 0;
            private byte[] m_iActivatedWriteCorelator = new byte[64];
            private int[] m_iWriteCorelator = new int[64];
            private byte[] m_iActivatedReadCorelator = new byte[64];
            private int[] m_iReadCorelator = new int[64];

            public byte EditingControlProtocolType
            {
                get
                {
                    return this.m_iProtocolType;
                }
                set
                {
                    this.m_iProtocolType = value;
                }
            }

            public byte EditingControlCameraType
            {
                get
                {
                    return this.m_iCameraType;
                }
                set
                {
                    if ((int)MissionPlanner.CamJoystick.CamJoystick.prev_m_iCameraType != (int)value)
                    {
                        MissionPlanner.CamJoystick.CamJoystick.prev_m_iCameraType = value;
                        if (value == (byte)1)
                            MissionPlanner.CamJoystick.CamJoystick.m_iCameraType_bool = !MissionPlanner.CamJoystick.CamJoystick.m_iCameraType_bool;
                    }
                    this.m_iCameraType = Convert.ToByte(MissionPlanner.CamJoystick.CamJoystick.m_iCameraType_bool);
                }
            }

            public byte EditingControlLaserMode
            {
                get
                {
                    return this.m_iLaserMode;
                }
                set
                {
                    this.m_iLaserMode = value;
                }
            }

            public byte EditingControlPictureInPicture
            {
                get
                {
                    return this.m_iPictureInPicture;
                }
                set
                {
                    this.m_iPictureInPicture = value;
                }
            }

            public byte EditingControlCameraMode
            {
                get
                {
                    return this.m_iCameraMode;
                }
                set
                {
                    this.m_iCameraMode = value;
                }
            }

            public byte EditingControlStabilization
            {
                get
                {
                    return this.m_iStabilization;
                }
                set
                {
                    this.m_iStabilization = value;
                }
            }

            public byte EditingControlOsdText
            {
                get
                {
                    return this.m_iOsdText;
                }
                set
                {
                    this.m_iOsdText = value;
                }
            }

            public byte EditingControlOsdGraphic
            {
                get
                {
                    return this.m_iOsdGraphic;
                }
                set
                {
                    this.m_iOsdGraphic = value;
                }
            }

            public byte EditingControlTec
            {
                get
                {
                    return this.m_iTec;
                }
                set
                {
                    this.m_iTec = value;
                }
            }

            public byte EditingControlFreeze
            {
                get
                {
                    return this.m_iFreeze;
                }
                set
                {
                    this.m_iFreeze = value;
                }
            }

            public byte EditingControlRateCalc
            {
                get
                {
                    return this.m_iRateCalc;
                }
                set
                {
                    this.m_iRateCalc = value;
                }
            }

            public byte EditingControlThermalColorType
            {
                get
                {
                    return this.m_iThermalColorType;
                }
                set
                {
                    this.m_iThermalColorType = value;
                }
            }

            public byte EditingControlPolarity
            {
                get
                {
                    return this.m_iPolarity;
                }
                set
                {
                    if ((int)MissionPlanner.CamJoystick.CamJoystick.prev_Polarity != (int)value)
                    {
                        MissionPlanner.CamJoystick.CamJoystick.prev_Polarity = value;
                        if (value == (byte)1)
                            MissionPlanner.CamJoystick.CamJoystick.m_iPolarity_bool = !MissionPlanner.CamJoystick.CamJoystick.m_iPolarity_bool;
                    }
                    this.m_iPolarity = Convert.ToByte(MissionPlanner.CamJoystick.CamJoystick.m_iPolarity_bool);
                }
            }

            public byte EditingControlNuc
            {
                get
                {
                    return this.m_iNuc;
                }
                set
                {
                    this.m_iNuc = value;
                }
            }

            public byte EditingControlGain
            {
                get
                {
                    return this.m_iGain;
                }
                set
                {
                    this.m_iGain = value;
                }
            }

            public byte EditingControlLevel
            {
                get
                {
                    return this.m_iLevel;
                }
                set
                {
                    this.m_iLevel = value;
                }
            }

            public byte EditingControlRecord
            {
                get
                {
                    return this.m_iRecord;
                }
                set
                {
                    if ((int)MissionPlanner.CamJoystick.CamJoystick.prev_m_iRecord != (int)value)
                    {
                        MissionPlanner.CamJoystick.CamJoystick.prev_m_iRecord = value;
                        if (value == (byte)1)
                            MissionPlanner.CamJoystick.CamJoystick.m_iRecord_bool = !MissionPlanner.CamJoystick.CamJoystick.m_iRecord_bool;
                    }
                    this.m_iRecord = Convert.ToByte(MissionPlanner.CamJoystick.CamJoystick.m_iRecord_bool);
                }
            }

            public byte EditingPictureCapture
            {
                get
                {
                    return this.m_iPicCapture;
                }
                set
                {
                    this.m_iPicCapture = value;
                }
            }

            public byte EditingControlTracker
            {
                get
                {
                    return this.m_iTracker;
                }
                set
                {
                    if ((int)MissionPlanner.CamJoystick.CamJoystick.prev_m_iTracker != (int)value)
                    {
                        MissionPlanner.CamJoystick.CamJoystick.prev_m_iTracker = value;
                        if (value == (byte)1)
                        {
                            switch (MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state)
                            {
                                case MissionPlanner.CamJoystick.CamJoystick.Tracker_State.IDLE:
                                    MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.ENABLED;
                                    break;
                                case MissionPlanner.CamJoystick.CamJoystick.Tracker_State.ENABLED:
                                    MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK;
                                    break;
                                case MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK:
                                    MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.IDLE;
                                    break;
                                case MissionPlanner.CamJoystick.CamJoystick.Tracker_State.RETRACK:
                                    MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK;
                                    break;
                            }
                        }
                    }
                    this.m_iTracker = Convert.ToByte((object)MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state);
                }
            }

            public byte EditingControlReTracker
            {
                get
                {
                    return this.m_iReTracker;
                }
                set
                {
                    if ((int)MissionPlanner.CamJoystick.CamJoystick.prev_m_iReTracker == (int)value)
                        return;
                    MissionPlanner.CamJoystick.CamJoystick.prev_m_iReTracker = value;
                    if (value == (byte)1)
                    {
                        switch (MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state)
                        {
                            case MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK:
                                MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.RETRACK;
                                this.m_iTracker = Convert.ToByte((object)MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state);
                                break;
                            case MissionPlanner.CamJoystick.CamJoystick.Tracker_State.RETRACK:
                                MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK;
                                this.m_iTracker = Convert.ToByte((object)MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state);
                                break;
                        }
                    }
                }
            }

            public Point EditingControlTrackOnPos
            {
                get
                {
                    return new Point();
                }
                set
                {
                    if (MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state != MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS1 && MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state != MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS2)
                    {
                        MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS1;
                        this.m_iTracker = Convert.ToByte((object)MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state);
                    }
                    else if (MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state == MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS1)
                    {
                        MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS2;
                        this.m_iTracker = Convert.ToByte((object)MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state);
                    }
                    else
                    {
                        MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS1;
                        this.m_iTracker = Convert.ToByte((object)MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state);
                    }
                    this.m_iTrackOnPos_x = value.X;
                    this.m_iTrackOnPos_y = value.Y;
                }
            }

            public byte EditingControlSingleYaw
            {
                get
                {
                    return Convert.ToByte(MissionPlanner.CamJoystick.CamJoystick.m_iSingleYaw_bool);
                }
                set
                {
                    if ((int)MissionPlanner.CamJoystick.CamJoystick.prev_m_iSingleYaw != (int)value)
                    {
                        MissionPlanner.CamJoystick.CamJoystick.prev_m_iSingleYaw = value;
                        if (value == (byte)1)
                            MissionPlanner.CamJoystick.CamJoystick.m_iSingleYaw_bool = !MissionPlanner.CamJoystick.CamJoystick.m_iSingleYaw_bool;
                    }
                    if (MissionPlanner.CamJoystick.CamJoystick.m_iSingleYaw_bool)
                        this.m_iProtocolType = (byte)121;
                    else
                        this.m_iProtocolType = (byte)119;
                }
            }

            public byte EditingControlRetracting
            {
                get
                {
                    return this.m_iRetracting;
                }
                set
                {
                    if ((int)MissionPlanner.CamJoystick.CamJoystick.prev_Retracting != (int)value)
                    {
                        MissionPlanner.CamJoystick.CamJoystick.prev_Retracting = value;
                        if (value == (byte)1)
                            MissionPlanner.CamJoystick.CamJoystick.m_iRetracting_bool = !MissionPlanner.CamJoystick.CamJoystick.m_iRetracting_bool;
                    }
                    this.m_iRetracting = Convert.ToByte(MissionPlanner.CamJoystick.CamJoystick.m_iRetracting_bool);
                }
            }

            public byte EditingControlFollowTarget
            {
                get
                {
                    return this.m_iFollowTarget;
                }
                set
                {
                    if ((int)MissionPlanner.CamJoystick.CamJoystick.prev_FollowTarget != (int)value)
                    {
                        MissionPlanner.CamJoystick.CamJoystick.prev_FollowTarget = value;
                        if (value == (byte)1)
                            MissionPlanner.CamJoystick.CamJoystick.m_iFollowTarget_bool = !MissionPlanner.CamJoystick.CamJoystick.m_iFollowTarget_bool;
                    }
                    this.m_iFollowTarget = Convert.ToByte(MissionPlanner.CamJoystick.CamJoystick.m_iFollowTarget_bool);
                }
            }

            public byte EditingSwitchMode
            {
                get
                {
                    return this.m_iSwitchMode;
                }
                set
                {
                    if ((int)MissionPlanner.CamJoystick.CamJoystick.prev_SwitchMode != (int)value)
                    {
                        MissionPlanner.CamJoystick.CamJoystick.prev_SwitchMode = value;
                        if (value == (byte)1)
                            MissionPlanner.CamJoystick.CamJoystick.m_iSwitchMode_bool = !MissionPlanner.CamJoystick.CamJoystick.m_iSwitchMode_bool;
                    }
                    this.m_iSwitchMode = Convert.ToByte(MissionPlanner.CamJoystick.CamJoystick.m_iSwitchMode_bool);
                }
            }


            public byte EditingControlFollowTargetClear
            {
                get
                {
                    return 0;
                }
                set
                {
                    MissionPlanner.CamJoystick.CamJoystick.m_iFollowTarget_bool = false;
                    MissionPlanner.CamJoystick.CamJoystick.prev_FollowTarget = byte.MaxValue;
                    this.m_iFollowTarget = (byte)0;
                }
            }

            public byte EditingControlPictureInPictureMode
            {
                get
                {
                    return this.m_iPictureInPictureMode;
                }
                set
                {
                    this.m_iPictureInPictureMode = value;
                }
            }

            public byte EditingControlZoomIn
            {
                get
                {
                    return this.m_iZoomIn;
                }
                set
                {
                    this.m_iZoomIn = value;
                }
            }

            public byte EditingControlZoomOut
            {
                get
                {
                    return this.m_iZoomOut;
                }
                set
                {
                    this.m_iZoomOut = value;
                }
            }

            public ushort EditingControlPitch
            {
                get
                {
                    return this.m_iPitch;
                }
                set
                {
                    this.m_iPitch = value;
                    if (MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state != MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS1 && MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state != MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS2 || this.m_iPitch == (ushort)512)
                        return;
                    MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.IDLE;
                    this.m_iTracker = Convert.ToByte((object)MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state);
                }
            }

            public ushort EditingControlRoll
            {
                get
                {
                    return this.m_iRoll;
                }
                set
                {
                    this.m_iRoll = value;
                    if (MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state != MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS1 && MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state != MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS2 || this.m_iRoll == (ushort)512)
                        return;
                    MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.IDLE;
                    this.m_iTracker = Convert.ToByte((object)MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state);
                }
            }

            public ushort EditingControlYaw
            {
                get
                {
                    return this.m_iYaw;
                }
                set
                {
                    this.m_iYaw = value;
                    if (MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state != MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS1 && MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state != MissionPlanner.CamJoystick.CamJoystick.Tracker_State.TRACK_ON_POS2 || this.m_iYaw == (ushort)512)
                        return;
                    MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state = MissionPlanner.CamJoystick.CamJoystick.Tracker_State.IDLE;
                    this.m_iTracker = Convert.ToByte((object)MissionPlanner.CamJoystick.CamJoystick.m_iTracker_state);
                }
            }

            public void EditingControlWriteCorrelator(byte a_iIndex, int a_iCorrelatorData, byte a_iStatus)
            {
                this.m_iActivatedWriteCorelator[(int)a_iIndex] = a_iStatus;
                this.m_iWriteCorelator[(int)a_iIndex] = a_iCorrelatorData;
            }

            public void EditingControlReadCorrelator(byte a_iIndex, byte a_iStatus)
            {
                this.m_iActivatedReadCorelator[(int)a_iIndex] = a_iStatus;
            }

            private byte EditingControlGetActivatedWriteCorrelator
            {
                get
                {
                    byte num = byte.MaxValue;
                    for (int index = 0; index < 64; ++index)
                    {
                        if (this.m_iActivatedWriteCorelator[(int)this.m_iWriteCorrelatorIndex] == (byte)1)
                            num = this.m_iWriteCorrelatorIndex;
                        ++this.m_iWriteCorrelatorIndex;
                        if (this.m_iWriteCorrelatorIndex >= (byte)64)
                            this.m_iWriteCorrelatorIndex = (byte)0;
                        if (num != byte.MaxValue)
                            return num;
                    }
                    return num;
                }
            }

            private byte EditingControlGetActivatedReadCorrelator
            {
                get
                {
                    byte num = byte.MaxValue;
                    for (int index = 0; index < 64; ++index)
                    {
                        if (this.m_iActivatedReadCorelator[(int)this.m_iReadCorrelatorIndex] == (byte)1)
                            num = this.m_iReadCorrelatorIndex;
                        ++this.m_iReadCorrelatorIndex;
                        if (this.m_iReadCorrelatorIndex >= (byte)64)
                            this.m_iReadCorrelatorIndex = (byte)0;
                        if (num != byte.MaxValue)
                            return num;
                    }
                    return num;
                }
            }

            public void GetransmitPacket(byte[] a_pBuffer)
            {
                a_pBuffer[0] = (byte)176;
                a_pBuffer[1] = (byte)59;
                a_pBuffer[2] = this.m_iProtocolType;
                a_pBuffer[3] = (byte)(((int)this.m_iCameraType & 1) << 7 | ((int)this.m_iLaserMode & 1) << 6 | ((int)this.m_iPictureInPicture & 1) << 5 | (int)this.m_iCameraMode & 31);
                a_pBuffer[4] = (byte)(((int)this.m_iStabilization & 1) << 7 | ((int)this.m_iOsdText & 1) << 6 | ((int)this.m_iOsdGraphic & 1) << 5 | ((int)this.m_iTec & 1) << 4 | ((int)this.m_iFreeze & 1) << 2 | (int)this.m_iRateCalc & 3);
                a_pBuffer[5] = (byte)(((int)this.m_iThermalColorType & 3) << 6 | ((int)this.m_iNuc & 1) << 5 | ((int)this.m_iPolarity & 1) << 4 | ((int)this.m_iGain & 3) << 2 | (int)this.m_iLevel & 3);
                a_pBuffer[6] = (byte)0;
                a_pBuffer[6] = (byte)(((int)this.m_iRecord & 1) << 4 | ((int)this.m_iPicCapture & 1) << 5 | (int)this.m_iTracker & 15);
                a_pBuffer[7] = (byte)0;
                a_pBuffer[8] = (byte)0;
                byte activatedWriteCorrelator = this.EditingControlGetActivatedWriteCorrelator;
                a_pBuffer[9] = (byte)(((int)this.m_iPictureInPictureMode & 3) << 6 | (int)activatedWriteCorrelator & 15);
                a_pBuffer[10] = (byte)this.m_iTrackOnPos_x;
                a_pBuffer[11] = (byte)(this.m_iTrackOnPos_x >> 8);
                a_pBuffer[12] = (byte)this.m_iTrackOnPos_y;
                a_pBuffer[13] = (byte)(this.m_iTrackOnPos_y >> 8);
                a_pBuffer[14] = (byte)(((int)this.m_iZoomIn & 1) << 7 | ((int)this.m_iZoomOut & 1) << 6 | ((int)this.m_iPitch & 3) << 4 | ((int)this.m_iRoll & 3) << 2 | (int)this.m_iYaw & 3);
                a_pBuffer[15] = (byte)((uint)this.m_iPitch >> 2);
                a_pBuffer[16] = (byte)((uint)this.m_iRoll >> 2);
                a_pBuffer[17] = (byte)((uint)this.m_iYaw >> 2);
                a_pBuffer[18] = (byte)0;
                a_pBuffer[19] = (byte)0;
                for (byte index = 0; index < (byte)19; ++index)
                    a_pBuffer[19] += a_pBuffer[(int)index];
            }

            public enum CameraMode
            {
                e_Rate = 0,
                e_PointToCordinate = 1,
                e_HoldCordinate = 2,
                e_Pilot = 3,
                e_Stow = 4,
                e_RateDriftOff = 6,
                e_DinamicDrift = 7,
                e_Park = 8,
                e_GyroCalibration = 10, // 0x0000000A
                e_Position = 12, // 0x0000000C
                e_GRR = 17, // 0x00000011
            }
        }
    }
}
