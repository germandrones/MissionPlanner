using System;
using System.Collections.Generic;
using System.Collections;
using log4net;
using System.Reflection;
using System.IO;
using MissionPlanner.Utilities;
using SharpDX.DirectInput;
using MissionPlanner.GCSViews;
using MissionPlanner.ColibriControl;

namespace MissionPlanner.Joystick
{
    public static class Extensions
    {
        public static MyJoystickState CurrentJoystickState(this SharpDX.DirectInput.Joystick joystick)
        {
            return new MyJoystickState(joystick.GetCurrentState());
        }
    }

    public class MyJoystickState
    {
        internal JoystickState baseJoystickState;

        public MyJoystickState(JoystickState state)
        {
            baseJoystickState = state;
        }

        public int[] GetSlider()
        {
            return baseJoystickState.Sliders;
        }

        public int[] GetPointOfView()
        {
            return baseJoystickState.PointOfViewControllers;
        }

        public bool[] GetButtons()
        {
            return baseJoystickState.Buttons;
        }

        public int AZ
        {
            get { return baseJoystickState.AccelerationZ; }
        }

        public int AY
        {
            get { return baseJoystickState.AccelerationY; }
        }

        public int AX
        {
            get { return baseJoystickState.AccelerationX; }
        }

        public int ARz
        {
            get { return baseJoystickState.AngularAccelerationZ; }
        }

        public int ARy
        {
            get { return baseJoystickState.AngularAccelerationY; }
        }

        public int ARx
        {
            get { return baseJoystickState.AngularAccelerationX; }
        }

        public int FRx
        {
            get { return baseJoystickState.TorqueX; }
        }

        public int FRy
        {
            get { return baseJoystickState.TorqueY; }
        }

        public int FRz
        {
            get { return baseJoystickState.TorqueZ; }
        }

        public int FX
        {
            get { return baseJoystickState.ForceX; }
        }

        public int FY
        {
            get { return baseJoystickState.ForceY; }
        }

        public int FZ
        {
            get { return baseJoystickState.ForceZ; }
        }

        public int Rx
        {
            get { return baseJoystickState.RotationX; }
        }

        public int Ry
        {
            get { return baseJoystickState.RotationY; }
        }

        public int Rz
        {
            get { return baseJoystickState.RotationZ; }
        }

        public int VRx
        {
            get { return baseJoystickState.AngularVelocityX; }
        }

        public int VRy
        {
            get { return baseJoystickState.AngularVelocityY; }
        }

        public int VRz
        {
            get { return baseJoystickState.AngularVelocityZ; }
        }

        public int VX
        {
            get { return baseJoystickState.VelocityX; }
        }

        public int VY
        {
            get { return baseJoystickState.VelocityY; }
        }

        public int VZ
        {
            get { return baseJoystickState.VelocityZ; }
        }

        public int X
        {
            get { return baseJoystickState.X; }
        }

        public int Y
        {
            get { return baseJoystickState.Y; }
        }

        public int Z
        {
            get { return baseJoystickState.Z; }
        }
    }

    public class Joystick : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        SharpDX.DirectInput.Joystick joystick;
        MyJoystickState state;
        static DirectInput directInput = new DirectInput();
        public bool enabled = false;
        bool[] buttonpressed = new bool[128];
        public string name;
        public bool elevons = false;

        public bool manual_control = false;

        string joystickconfigbutton = "joystickbuttons.xml";
        string joystickconfigaxis = "joystickaxis.xml";

        // set to default midpoint
        int hat1 = 65535/2;
        int hat2 = 65535/2;
        int custom0 = 65535/2;
        int custom1 = 65535/2;


        public struct JoyChannel
        {
            public int channel;
            public joystickaxis axis;
            public bool reverse;
            public int expo;
        }

        public struct JoyButton
        {
            /// <summary>
            /// System button number
            /// </summary>
            public int buttonno;

            /// <summary>
            /// Fucntion we are doing for this button press
            /// </summary>
            public buttonfunction function;

            /// <summary>
            /// Mode we are changing to on button press
            /// </summary>
            public string mode;

            /// <summary>
            /// param 1
            /// </summary>
            public float p1;

            /// <summary>
            /// param 2
            /// </summary>
            public float p2;

            /// <summary>
            /// param 3
            /// </summary>
            public float p3;

            /// <summary>
            /// param 4
            /// </summary>
            public float p4;

            /// <summary>
            /// Relay state
            /// </summary>
            public bool state;
        }

        // old standard implementation
        /*public enum buttonfunction
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
        }*/

        // Camera Control implementation
        public enum buttonfunction
        {
            CameraTrack,
            ZoomIn,
            ZoomOut,
        }


        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Implement reccomended best practice dispose pattern
        /// http://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="disposing"></param>
        virtual protected void Dispose(bool disposing)
        {
            try
            {
                //not sure if this is a problem from the finalizer?
                if (disposing && joystick != null && joystick.Properties != null)
                    joystick.Unacquire();
            }
            catch
            {
            }

            try
            {
                if (disposing && joystick != null)
                    joystick.Dispose();
            }
            catch
            {
            }

            //tell gc not to call finalize, this object will be GC'd quicker now.
            GC.SuppressFinalize(this);
        }

        #region Constructor
        public Joystick()
        {
            for (int a = 0; a < JoyButtons.Length; a++) JoyButtons[a].buttonno = -1;
            //use only arduplane setting
            if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
            {
                loadconfig("joystickbuttons" + MainV2.comPort.MAV.cs.firmware + ".xml", "joystickaxis" + MainV2.comPort.MAV.cs.firmware + ".xml");
            }
            else
            {
                loadconfig();
            }
        }
        #endregion

        #region Joystick config load/save xml
        public void loadconfig(string joystickconfigbutton = "joystickbuttons.xml", string joystickconfigaxis = "joystickaxis.xml")
        {
            log.Info("Loading joystick config files " + joystickconfigbutton + " " + joystickconfigaxis);

            // save for later
            this.joystickconfigbutton = Settings.GetUserDataDirectory() + joystickconfigbutton;
            this.joystickconfigaxis = Settings.GetUserDataDirectory() + joystickconfigaxis;

            // load config
            if (File.Exists(this.joystickconfigbutton) && File.Exists(this.joystickconfigaxis))
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer reader =
                        new System.Xml.Serialization.XmlSerializer(typeof (JoyButton[]), new Type[] {typeof (JoyButton)});

                    using (StreamReader sr = new StreamReader(this.joystickconfigbutton))
                    {
                        JoyButtons = (JoyButton[]) reader.Deserialize(sr);
                    }
                }
                catch
                {
                }

                try
                {
                    System.Xml.Serialization.XmlSerializer reader =
                        new System.Xml.Serialization.XmlSerializer(typeof (JoyChannel[]),
                            new Type[] {typeof (JoyChannel)});

                    using (StreamReader sr = new StreamReader(this.joystickconfigaxis))
                    {
                        JoyChannels = (JoyChannel[]) reader.Deserialize(sr);
                    }
                }
                catch
                {
                }
            }
        }

        public void saveconfig()
        {
            log.Info("Saving joystick config files " + joystickconfigbutton + " " + joystickconfigaxis);

            // save config
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof (JoyButton[]), new Type[] {typeof (JoyButton)});

            using (StreamWriter sw = new StreamWriter(joystickconfigbutton))
            {
                writer.Serialize(sw, JoyButtons);
            }

            writer = new System.Xml.Serialization.XmlSerializer(typeof (JoyChannel[]), new Type[] {typeof (JoyChannel)});

            using (StreamWriter sw = new StreamWriter(joystickconfigaxis))
            {
                writer.Serialize(sw, JoyChannels);
            }
        }
        #endregion

        JoyChannel[] JoyChannels = new JoyChannel[9]; // we are base 1
        JoyButton[] JoyButtons = new JoyButton[128]; // base 0

        public static IList<DeviceInstance> getDevices()
        {
            return directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
        }

        public static SharpDX.DirectInput.Joystick getJoyStickByName(string name)
        {
            var joysticklist = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);

            foreach (DeviceInstance device in joysticklist)
            {
                if (device.ProductName.TrimUnPrintable() == name)
                {
                    return new SharpDX.DirectInput.Joystick(directInput, device.InstanceGuid);
                }
            }
            return null;
        }

        public SharpDX.DirectInput.Joystick AcquireJoystick(string name)
        {
            joystick = getJoyStickByName(name);

            if (joystick == null)
                return null;

            joystick.Acquire();

            joystick.Poll();

            return joystick;
        }

        public bool start(string name)
        {
            this.name = name;

            joystick = AcquireJoystick(name);

            if (joystick == null)
                return false;

            enabled = true;

            System.Threading.Thread t11 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
            {
                Name = "Joystick loop",
                Priority = System.Threading.ThreadPriority.AboveNormal,
                IsBackground = true
            };
            t11.Start();

            return true;
        }

        public static joystickaxis getMovingAxis(string name, int threshold)
        {
            var joystick = new Joystick().AcquireJoystick(name);

            if (joystick == null)
                return joystickaxis.ARx;

            joystick.Poll();

            System.Threading.Thread.Sleep(300);

            joystick.Poll();

            var obj = joystick.CurrentJoystickState();
            Hashtable values = new Hashtable();

            // get the state of the joystick before.
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                values[property.Name] = int.Parse(property.GetValue(obj, null).ToString());
            }
            values["Slider1"] = obj.GetSlider()[0];
            values["Slider2"] = obj.GetSlider()[1];
            values["Hatud1"] = obj.GetPointOfView()[0];
            values["Hatlr2"] = obj.GetPointOfView()[0];
            values["Custom1"] = 0;
            values["Custom2"] = 0;

            CustomMessageBox.Show("Please move the joystick axis you want assigned to this function after clicking ok");

            DateTime start = DateTime.Now;

            while (start.AddSeconds(10) > DateTime.Now)
            {
                joystick.Poll();
                System.Threading.Thread.Sleep(50);
                var nextstate = joystick.CurrentJoystickState();

                int[] slider = nextstate.GetSlider();

                int[] hat1 = nextstate.GetPointOfView();

                type = nextstate.GetType();
                properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    //Console.WriteLine("Name: " + property.Name + ", Value: " + property.GetValue(obj, null));

                    log.InfoFormat("test name {0} old {1} new {2} ", property.Name, values[property.Name],
                        int.Parse(property.GetValue(nextstate, null).ToString()));
                    log.InfoFormat("{0}  {1} {2}", property.Name, (int) values[property.Name],
                        (int.Parse(property.GetValue(nextstate, null).ToString()) + threshold));
                    if ((int) values[property.Name] >
                        (int.Parse(property.GetValue(nextstate, null).ToString()) + threshold) ||
                        (int) values[property.Name] <
                        (int.Parse(property.GetValue(nextstate, null).ToString()) - threshold))
                    {
                        log.Info(property.Name);
                        joystick.Unacquire();
                        return (joystickaxis) Enum.Parse(typeof (joystickaxis), property.Name);
                    }
                }

                // slider1
                if ((int) values["Slider1"] > (slider[0] + threshold) ||
                    (int) values["Slider1"] < (slider[0] - threshold))
                {
                    joystick.Unacquire();
                    return joystickaxis.Slider1;
                }

                // slider2
                if ((int) values["Slider2"] > (slider[1] + threshold) ||
                    (int) values["Slider2"] < (slider[1] - threshold))
                {
                    joystick.Unacquire();
                    return joystickaxis.Slider2;
                }

                // Hatud1
                if ((int) values["Hatud1"] != (hat1[0]))
                {
                    joystick.Unacquire();
                    return joystickaxis.Hatud1;
                }

                // Hatlr2
                if ((int) values["Hatlr2"] != (hat1[0]))
                {
                    joystick.Unacquire();
                    return joystickaxis.Hatlr2;
                }
            }

            CustomMessageBox.Show("No valid option was detected");

            return joystickaxis.None;
        }

        public static int getPressedButton(string name)
        {
            var joystick = getJoyStickByName(name);

            if (joystick == null)
                return -1;

            //joystick.SetDataFormat(DeviceDataFormat.Joystick);

            joystick.Acquire();

            System.Threading.Thread.Sleep(500);

            joystick.Poll();

            var obj = joystick.CurrentJoystickState();

            var buttonsbefore = obj.GetButtons();

            CustomMessageBox.Show("Please press the joystick button you want assigned to this function after clicking ok");

            DateTime start = DateTime.Now;

            while (start.AddSeconds(10) > DateTime.Now)
            {
                joystick.Poll();
                var nextstate = joystick.CurrentJoystickState();

                var buttons = nextstate.GetButtons();

                for (int a = 0; a < joystick.Capabilities.ButtonCount; a++)
                {
                    if (buttons[a] != buttonsbefore[a])
                        return a;
                }
            }

            CustomMessageBox.Show("No valid option was detected");

            return -1;
        }

        public void setReverse(int channel, bool reverse)
        {
            JoyChannels[channel].reverse = reverse;
        }

        public void setAxis(int channel, joystickaxis axis)
        {
            JoyChannels[channel].axis = axis;
        }

        public void setChannel(int channel, joystickaxis axis, bool reverse, int expo)
        {
            JoyChannel joy = new JoyChannel();
            joy.axis = axis;
            joy.channel = channel;
            joy.expo = expo;
            joy.reverse = reverse;

            JoyChannels[channel] = joy;
        }

        public void setChannel(JoyChannel chan)
        {
            JoyChannels[chan.channel] = chan;
        }

        public JoyChannel getChannel(int channel)
        {
            return JoyChannels[channel];
        }

        public void setButton(int arrayoffset, JoyButton buttonconfig)
        {
            JoyButtons[arrayoffset] = buttonconfig;
        }

        public JoyButton getButton(int arrayoffset)
        {
            return JoyButtons[arrayoffset];
        }

        public void changeButton(int buttonid, int newid)
        {
            JoyButtons[buttonid].buttonno = newid;
        }

        public int getHatSwitchDirection()
        {
            return (state.GetPointOfView())[0];
        }

        public int getNumberPOV()
        {
            return joystick.Capabilities.PovCount;
        }

        int BOOL_TO_SIGN(bool input)
        {
            if (input == true)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        

        /// <summary>
        /// Updates the rcoverride values and controls the mode changes
        /// </summary>
        void mainloop()
        {
            while (enabled && joystick != null && !joystick.IsDisposed)
            {
                try
                {
                    System.Threading.Thread.Sleep(50);
                    if (joystick.IsDisposed)
                        return;
                    //joystick stuff
                    joystick.Poll();
                    state = joystick.CurrentJoystickState();

                    //Console.WriteLine(state);

                    if (getNumberPOV() > 0)
                    {
                        int pov = getHatSwitchDirection();

                        if (pov != -1)
                        {
                            int angle = pov/100;

                            //0 = down = 18000
                            //0 = up = 0

                            // 0
                            if (angle > 270 || angle < 90)
                                hat1 += 500;
                            // 180
                            if (angle > 90 && angle < 270)
                                hat1 -= 500;
                            // 90
                            if (angle > 0 && angle < 180)
                                hat2 += 500;
                            // 270
                            if (angle > 180 && angle < 360)
                                hat2 -= 500;
                        }
                    }

                    //get data from axis 1 and 2
                    short roll = pickchannel(1, JoyChannels[1].axis, false, JoyChannels[1].expo);
                    short pitch = pickchannel(2, JoyChannels[2].axis, false, JoyChannels[2].expo);

                    //stop camera movement if roll and pitch is in the middle
                    if(roll < 1510 && roll > 1490 && pitch < 1510 && pitch > 1490)
                    {
                        if (MainV2.mav_proto != null) MainV2.mav_proto.MavlinkCamStop();
                    }
                    else // if any axis moved do camera movement
                    {

                        double roll_speed    = map(roll, 1000.0, 2000.0, -3.0, 3.0); //double precission of speed
                        double pitch_speed   = map(pitch, 1000.0, 2000.0, -3.0, 3.0); //double precission of speed

                        roll_speed *= BOOL_TO_SIGN(JoyChannels[1].reverse);
                        pitch_speed*= BOOL_TO_SIGN(JoyChannels[2].reverse);

                        if (MainV2.mav_proto != null)
                        {
                            MainV2.mav_proto.MavlinkMoveCam((float)pitch_speed, (float)roll_speed);
                        }
                    }

                    if (MainV2.mav_proto == null) { continue; }



                    #region Joystick Gimbal Controls Pitch & Roll
                    /*short roll = pickchannel(1, JoyChannels[1].axis, false, JoyChannels[1].expo);
                    short pitch = pickchannel(2, JoyChannels[2].axis, false, JoyChannels[2].expo);

                    if (roll > 1550)
                    {
                        float speed = map(roll, 1000, 1500, 5, 0);
                        if (MainV2.mav_proto.roll_pos > -180) MainV2.mav_proto.roll_pos += speed; else MainV2.mav_proto.roll_pos = -180;
                    }
                    else if(roll < 1450)
                    {
                        float speed = map(roll, 1500, 2000, 0, 5);
                        if (MainV2.mav_proto.roll_pos < 180) MainV2.mav_proto.roll_pos-=speed; else MainV2.mav_proto.roll_pos = 180;
                    }


                    if (pitch > 1550)
                    {
                        float speed = map(pitch, 1000, 1500, 5, 0);
                        if (MainV2.mav_proto.pitch_pos < 0) MainV2.mav_proto.pitch_pos += speed; else MainV2.mav_proto.pitch_pos = 0;
                    }
                    else if (pitch < 1450)
                    {
                        float speed = map(pitch, 1500, 2000, 0, 5);
                        if (MainV2.mav_proto.pitch_pos > -90) MainV2.mav_proto.pitch_pos -= speed; else MainV2.mav_proto.pitch_pos = -90;
                    }
                    MainV2.mav_proto.MavlinkUpdatePosMode(MainV2.mav_proto.pitch_pos, MainV2.mav_proto.roll_pos);*/
                    #endregion

                    #region Joystick Gimbal Controls zoom in out
                    short zoom = pickchannel(3, JoyChannels[3].axis, false, JoyChannels[3].expo);
                    if (zoom > 1550)
                    {
                        //zoom in
                        MainV2.mav_proto.MavlinkCamZoomIn();
                    }
                    else if(zoom < 1450)
                    {
                        //zoom out
                        MainV2.mav_proto.MavlinkCamZoomOut();
                    }
                    else
                    {
                        //zoom stop
                        MainV2.mav_proto.MavlinkCamZoomStop();
                    }
                    #endregion

                    // disable button actions when not connected.
                    if (MainV2.comPort.BaseStream.IsOpen) DoJoystickButtonFunction();
                }
                catch (SharpDX.SharpDXException ex)
                {
                    log.Error(ex);
                    MainV2.instance.Invoke((System.Action) delegate { CustomMessageBox.Show("Lost Joystick", "Lost Joystick"); });
                    return;
                }
                catch (Exception ex)
                {
                    log.Info("Joystick thread error " + ex.ToString());
                }
            }
        }


        #region Button events
        public void DoJoystickButtonFunction()
        {
            foreach (JoyButton but in JoyButtons)
            {
                if (but.buttonno != -1)
                {
                    getButtonState(but, but.buttonno);
                }
            }
        }

        bool getButtonState(JoyButton but, int buttonno)
        {
            var buts = state.GetButtons();

            // button down
            bool ans = buts[buttonno] && !buttonpressed[buttonno]; // press check + debounce
            if (ans) ButtonDown(but);

            // button up
            ans = !buts[buttonno] && buttonpressed[buttonno];
            if (ans) ButtonUp(but);

            buttonpressed[buttonno] = buts[buttonno]; // set only this button
            return ans;
        }

        public bool isButtonPressed(int buttonno)
        {
            var buts = state.GetButtons();

            if (buts == null || JoyButtons[buttonno].buttonno < 0)
                return false;

            return buts[JoyButtons[buttonno].buttonno];
        }

        void ButtonDown(JoyButton but)
        {
            ProcessButtonEvent(but, true);
        }

        void ButtonUp(JoyButton but)
        {
            ProcessButtonEvent(but, false);
        }

        void ProcessButtonEvent(JoyButton but, bool buttondown)
        {
            if (but.buttonno != -1)
            {
                switch (but.function)
                {
                    case buttonfunction.CameraTrack:
                        {
                            /*if(MainV2.mav_proto.isHoldMode)
                            {
                                MainV2.mav_proto.isHoldMode = false;
                                MainV2.mav_proto.MavlinkUpdateCameraMode(ColibriMavlink.CameraMode.e_Position);
                            }
                            else
                            {
                                MainV2.mav_proto.isHoldMode = true;
                                MainV2.mav_proto.MavlinkUpdateCameraMode(ColibriMavlink.CameraMode.e_HoldCordinate);
                            }*/
                            break;
                        }

                    case buttonfunction.ZoomIn:
                        {
                            if(buttondown) MainV2.mav_proto.MavlinkCamZoomIn(); else MainV2.mav_proto.MavlinkCamZoomStop();
                            break;
                        }

                    case buttonfunction.ZoomOut:
                        {
                            if (buttondown) MainV2.mav_proto.MavlinkCamZoomOut(); else MainV2.mav_proto.MavlinkCamZoomStop();
                            break;
                        }
                }
            }
        }
        #endregion


        public enum joystickaxis
        {
            None,
            Pass,
            ARx,
            ARy,
            ARz,
            AX,
            AY,
            AZ,
            FRx,
            FRy,
            FRz,
            FX,
            FY,
            FZ,
            Rx,
            Ry,
            Rz,
            VRx,
            VRy,
            VRz,
            VX,
            VY,
            VZ,
            X,
            Y,
            Z,
            Slider1,
            Slider2,
            Hatud1,
            Hatlr2,
            Custom1,
            Custom2
        }

        const int RESXu = 1024;
        const int RESXul = 1024;
        const int RESXl = 1024;
        const int RESKul = 100;



        public void UnAcquireJoyStick()
        {
            if (joystick == null)
                return;
            joystick.Unacquire();
        }

        
        

        public int getNumButtons()
        {
            if (joystick == null)
                return 0;
            return joystick.Capabilities.ButtonCount;
        }

        public joystickaxis getJoystickAxis(int channel)
        {
            try
            {
                return JoyChannels[channel].axis;
            }
            catch
            {
                return joystickaxis.None;
            }
        }

        

        public short getValueForChannel(int channel, string name)
        {
            if (joystick == null)
                return 0;

            joystick.Poll();

            state = joystick.CurrentJoystickState();

            short ans = pickchannel(channel, JoyChannels[channel].axis, JoyChannels[channel].reverse,
                JoyChannels[channel].expo);
            log.DebugFormat("{0} = {1} = {2}", channel, ans, state.X);
            return ans;
        }

        public short getRawValueForChannel(int channel)
        {
            if (joystick == null)
                return 0;

            joystick.Poll();

            state = joystick.CurrentJoystickState();

            short ans = pickchannel(channel, JoyChannels[channel].axis, false, 0);
            log.DebugFormat("{0} = {1} = {2}", channel, ans, state.X);
            return ans;
        }

        short pickchannel(int chan, joystickaxis axis, bool rev, int expo)
        {
            int min, max, trim = 0;

            min = 1000;
            max = 2000;
            trim = 1500;
            
            if (manual_control)
            {
                min = -1000;
                max = 1000;
                trim = 0;
            }

            if (chan == 3)
            {
                trim = (min + max)/2;
            }

            int range = Math.Abs(max - min);

            int working = 0;

            switch (axis)
            {
                case joystickaxis.None:
                    working = ushort.MaxValue/2;
                    break;
                case joystickaxis.Pass:
                    working = (int) (((float) (trim - min)/range)*ushort.MaxValue);
                    break;
                case joystickaxis.ARx:
                    working = state.ARx;
                    break;

                case joystickaxis.ARy:
                    working = state.ARy;
                    break;

                case joystickaxis.ARz:
                    working = state.ARz;
                    break;

                case joystickaxis.AX:
                    working = state.AX;
                    break;

                case joystickaxis.AY:
                    working = state.AY;
                    break;

                case joystickaxis.AZ:
                    working = state.AZ;
                    break;

                case joystickaxis.FRx:
                    working = state.FRx;
                    break;

                case joystickaxis.FRy:
                    working = state.FRy;
                    break;

                case joystickaxis.FRz:
                    working = state.FRz;
                    break;

                case joystickaxis.FX:
                    working = state.FX;
                    break;

                case joystickaxis.FY:
                    working = state.FY;
                    break;

                case joystickaxis.FZ:
                    working = state.FZ;
                    break;

                case joystickaxis.Rx:
                    working = state.Rx;
                    break;

                case joystickaxis.Ry:
                    working = state.Ry;
                    break;

                case joystickaxis.Rz:
                    working = state.Rz;
                    break;

                case joystickaxis.VRx:
                    working = state.VRx;
                    break;

                case joystickaxis.VRy:
                    working = state.VRy;
                    break;

                case joystickaxis.VRz:
                    working = state.VRz;
                    break;

                case joystickaxis.VX:
                    working = state.VX;
                    break;

                case joystickaxis.VY:
                    working = state.VY;
                    break;

                case joystickaxis.VZ:
                    working = state.VZ;
                    break;

                case joystickaxis.X:
                    working = state.X;
                    break;

                case joystickaxis.Y:
                    working = state.Y;
                    break;

                case joystickaxis.Z:
                    working = state.Z;
                    break;

                case joystickaxis.Slider1:
                    int[] slider = state.GetSlider();
                    working = slider[0];
                    break;

                case joystickaxis.Slider2:
                    int[] slider1 = state.GetSlider();
                    working = slider1[1];
                    break;

                case joystickaxis.Hatud1:
                    hat1 = (int) constrain(hat1, 0, 65535);
                    working = hat1;
                    break;

                case joystickaxis.Hatlr2:
                    hat2 = (int) constrain(hat2, 0, 65535);
                    working = hat2;
                    break;

                case joystickaxis.Custom1:
                    working = (int)(((float)(custom0 - min) / range) * ushort.MaxValue);
                    working = (int)constrain(working, 0, 65535);
                    break;

                case joystickaxis.Custom2:
                    working = (int)(((float)(custom1 - min) / range) * ushort.MaxValue);
                    working = (int)constrain(working, 0, 65535);
                    break;
            }
            // between 0 and 65535 - convert to int -500 to 500
            working = (int)map(working, 0, 65535, -500, 500);

            if (rev) working *= -1;

            // save for later
            int raw = working;

            working = (int) Expo(working, expo, min, max, trim);

            //add limits to movement
            working = Math.Max(min, working);
            working = Math.Min(max, working);

            return (short) working;
        }

        #region internal helpers
        public static double Expo(double input, double expo, double min, double max, double mid)
        {
            // input range -500 to 500

            double expomult = expo/100.0;

            if (input >= 0)
            {
                // linear scale
                double linearpwm = map(input, 0, 500, mid, max);

                double expomid = (max - mid)/2;

                double factor = 0;

                // over half way though input
                if (input > 250)
                {
                    factor = 250 - (input - 250);
                }
                else
                {
                    factor = input;
                }

                return linearpwm - (factor*expomult);
            }
            else
            {
                double linearpwm = map(input, -500, 0, min, mid);

                double expomid = (mid - min)/2;

                double factor = 0;

                // over half way though input
                if (input < -250)
                {
                    factor = -250 - (input + 250);
                }
                else
                {
                    factor = input;
                }

                return linearpwm - (factor*expomult);
            }
        }

        static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min)*(out_max - out_min)/(in_max - in_min) + out_min;
        }

        static long map(long x, long in_min, long in_max, long out_min, long out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        double constrain(double value, double min, double max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
        #endregion
    }
}