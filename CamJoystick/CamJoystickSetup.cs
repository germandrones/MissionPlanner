using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner.CamJoystick
{
    public class CamJoystickSetup : Form
    {
        private bool startup = true;
        private int noButtons = 0;
        private IContainer components = (IContainer)null;
        private Label label5;
        private ComboBox CMB_joysticks;
        private MyButton BUT_enable;
        private MyButton BUT_save;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label9;
        private ComboBox CMB_CH8;
        private ComboBox CMB_CH7;
        private ComboBox CMB_CH6;
        private ComboBox CMB_CH5;
        private ComboBox CMB_CH4;
        private ComboBox CMB_CH3;
        private ComboBox CMB_CH2;
        private ComboBox CMB_CH1;
        private ComboBox CMB_CH12;
        private ComboBox CMB_CH11;
        private ComboBox CMB_CH10;
        private ComboBox CMB_CH9;
        private MyButton BUT_detch11;
        private MyButton BUT_detch10;
        private MyButton BUT_detch9;
        private MyButton BUT_detch4;
        private MyButton BUT_detch8;
        private MyButton BUT_detch3;
        private MyButton BUT_detch2;
        private MyButton BUT_detch1;
        private MyButton BUT_detch12;
        private MyButton BUT_detch5;
        private MyButton BUT_detch6;
        private MyButton BUT_detch7;
        private HorizontalProgressBar ReTrackingProgressBar;
        private HorizontalProgressBar TrackingProgressBar;
        private HorizontalProgressBar LaserProgressBar;
        private HorizontalProgressBar PicCaptureProgressBar;
        private HorizontalProgressBar RecProgressBar;
        private HorizontalProgressBar DayIRprogressBar;
        private HorizontalProgressBar BWhotprogressBar;
        private HorizontalProgressBar progressBarRoll;
        private HorizontalProgressBar NUCProgressBar;
        private HorizontalProgressBar ZoomOutProgressBar;
        private HorizontalProgressBar ZoomInProgressBar;
        private HorizontalProgressBar progressBarPitch;
        private Label label10;
        private CheckBox revCHpicCapture;
        private CheckBox revCHrec;
        private CheckBox revCHdayIr;
        private CheckBox revCH_BWhot;
        private CheckBox revCHnuc;
        private CheckBox revCHzoom_out;
        private CheckBox revCHzoom_in;
        private CheckBox revCHPitch;
        private CheckBox RevChRoll;
        private CheckBox RevChLaser;
        private CheckBox RevChTracking;
        private CheckBox RevChReTracking;
        private Timer timer1;
        private MyButton myButton1;
        private CheckBox RevChSingleYaw;
        private HorizontalProgressBar SingleYawProgressBar;
        private MyButton BUT_detch13;
        private ComboBox CMB_CH13;
        private Label label16;
        private CheckBox RevChBIT;
        private HorizontalProgressBar BITProgressBar;
        private MyButton BUT_detch14;
        private ComboBox CMB_CH14;
        private Label label18;
        private CheckBox RevChRetracting;
        private HorizontalProgressBar RetractingProgressBar;
        private MyButton BUT_detch15;
        private ComboBox CMB_CH15;
        private Label label17;
        private CheckBox RevChFollowTarget;
        private HorizontalProgressBar FollowTargetProgressBar;
        private MyButton BUT_detch16;
        private ComboBox CMB_CH16;
        private Label label19;

        public CamJoystickSetup()
        {
            this.InitializeComponent();
            Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void Joystick_Load(object sender, EventArgs e)
        {
            this.CMB_CH1.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH2.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH3.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH4.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH5.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH6.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH7.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH8.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH9.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH10.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH11.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH12.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH13.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH14.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH15.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            this.CMB_CH16.DataSource = (object)Enum.GetValues(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis));
            MissionPlanner.CamJoystick.CamJoystick camJoystick = new MissionPlanner.CamJoystick.CamJoystick();
            this.CMB_CH1.Text = camJoystick.getChannel(1).axis.ToString();
            this.CMB_CH2.Text = camJoystick.getChannel(2).axis.ToString();
            this.CMB_CH3.Text = camJoystick.getChannel(3).axis.ToString();
            this.CMB_CH4.Text = camJoystick.getChannel(4).axis.ToString();
            this.CMB_CH5.Text = camJoystick.getChannel(5).axis.ToString();
            this.CMB_CH6.Text = camJoystick.getChannel(6).axis.ToString();
            this.CMB_CH7.Text = camJoystick.getChannel(7).axis.ToString();
            this.CMB_CH8.Text = camJoystick.getChannel(8).axis.ToString();
            this.CMB_CH9.Text = camJoystick.getChannel(9).axis.ToString();
            this.CMB_CH10.Text = camJoystick.getChannel(10).axis.ToString();
            this.CMB_CH11.Text = camJoystick.getChannel(11).axis.ToString();
            this.CMB_CH12.Text = camJoystick.getChannel(12).axis.ToString();
            this.CMB_CH13.Text = camJoystick.getChannel(13).axis.ToString();
            this.CMB_CH14.Text = camJoystick.getChannel(14).axis.ToString();
            this.CMB_CH15.Text = camJoystick.getChannel(15).axis.ToString();
            this.CMB_CH16.Text = camJoystick.getChannel(16).axis.ToString();
            this.RevChRoll.Checked = !(camJoystick.getChannel(1).reverse.ToString().ToLower() == "false");
            this.revCHPitch.Checked = !(camJoystick.getChannel(2).reverse.ToString().ToLower() == "false");
            this.revCHzoom_in.Checked = !(camJoystick.getChannel(3).reverse.ToString().ToLower() == "false");
            this.revCHzoom_out.Checked = !(camJoystick.getChannel(4).reverse.ToString().ToLower() == "false");
            this.revCHnuc.Checked = !(camJoystick.getChannel(5).reverse.ToString().ToLower() == "false");
            this.revCH_BWhot.Checked = !(camJoystick.getChannel(6).reverse.ToString().ToLower() == "false");
            this.revCHdayIr.Checked = !(camJoystick.getChannel(7).reverse.ToString().ToLower() == "false");
            this.revCHrec.Checked = !(camJoystick.getChannel(8).reverse.ToString().ToLower() == "false");
            this.revCHpicCapture.Checked = !(camJoystick.getChannel(9).reverse.ToString().ToLower() == "false");
            this.RevChLaser.Checked = !(camJoystick.getChannel(10).reverse.ToString().ToLower() == "false");
            this.RevChTracking.Checked = !(camJoystick.getChannel(11).reverse.ToString().ToLower() == "false");
            this.RevChReTracking.Checked = !(camJoystick.getChannel(12).reverse.ToString().ToLower() == "false");
            this.RevChSingleYaw.Checked = !(camJoystick.getChannel(13).reverse.ToString().ToLower() == "false");
            this.RevChBIT.Checked = !(camJoystick.getChannel(14).reverse.ToString().ToLower() == "false");
            this.RevChRetracting.Checked = !(camJoystick.getChannel(15).reverse.ToString().ToLower() == "false");
            this.RevChFollowTarget.Checked = !(camJoystick.getChannel(16).reverse.ToString().ToLower() == "false");
            this.startup = false;
        }

        private void findandsetcontrol(string ctlname, string value)
        {
            Control control = this.Controls.Find(ctlname, false)[0];
            if (control is CheckBox)
                ((CheckBox)control).Checked = !(value.ToLower() == "false");
            else
                control.Text = value;
        }

        private void BUT_enable_Click()
        {
            MissionPlanner.CamJoystick.CamJoystick camJoystick = new MissionPlanner.CamJoystick.CamJoystick();
            if (!camJoystick.start(this.CMB_joysticks.Text))
            {
                int num = (int)CustomMessageBox.Show("Please Connect a Joystick", "No Joystick");
                camJoystick.Dispose();
            }
            else
            {
                Settings.Instance["joystick_name"] = this.CMB_joysticks.Text;
                MainV2.Camjoystick = camJoystick;
                MainV2.Camjoystick.enabled = true;
                this.timer1.Start();
            }
        }

        private void doButtontoUI(string name, int x, int y)
        {
            MyLabel myLabel = new MyLabel();
            ComboBox comboBox1 = new ComboBox();
            MyButton myButton1 = new MyButton();
            HorizontalProgressBar horizontalProgressBar = new HorizontalProgressBar();
            ComboBox comboBox2 = new ComboBox();
            MyButton myButton2 = new MyButton();
            MissionPlanner.CamJoystick.CamJoystick.JoyButton button = MissionPlanner.CamJoystick.CamJoystick.self.getButton(int.Parse(name));
            this.Controls.AddRange(new Control[6]
            {
        (Control) myLabel,
        (Control) comboBox1,
        (Control) myButton1,
        (Control) horizontalProgressBar,
        (Control) comboBox2,
        (Control) myButton2
            });
            myLabel.Location = new Point(x, y);
            myLabel.Size = new Size(47, 13);
            myLabel.Text = "Button " + (object)(int.Parse(name) + 1);
            comboBox1.Location = new Point(72, y);
            comboBox1.Size = new Size(70, 21);
            comboBox1.DataSource = (object)this.getButtonNumbers();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Name = "cmbbutton" + name;
            comboBox1.Text = button.buttonno.ToString();
            comboBox1.SelectedIndexChanged += new EventHandler(this.cmbbutton_SelectedIndexChanged);
            myButton1.Location = new Point(this.BUT_detch1.Left, y);
            myButton1.Size = this.BUT_detch1.Size;
            myButton1.Text = this.BUT_detch1.Text;
            myButton1.Name = "mybut" + name;
            myButton1.Click += new EventHandler(this.BUT_detbutton_Click);
            horizontalProgressBar.Location = new Point(this.progressBarRoll.Left, y);
            horizontalProgressBar.Size = this.progressBarRoll.Size;
            horizontalProgressBar.Name = "hbar" + name;
            comboBox2.Location = new Point(horizontalProgressBar.Right + 5, y);
            comboBox2.Size = new Size(100, 21);
            comboBox2.DataSource = (object)Enum.GetNames(typeof(MissionPlanner.CamJoystick.CamJoystick.buttonfunction));
            comboBox2.Tag = (object)name;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.Name = "cmbaction" + name;
            comboBox2.Text = button.function.ToString();
            comboBox2.SelectedIndexChanged += new EventHandler(this.cmbaction_SelectedIndexChanged);
            myButton2.Location = new Point(comboBox2.Right + 5, y);
            myButton2.Size = this.BUT_detch1.Size;
            myButton2.Text = "Settings";
            myButton2.Name = "butsettings" + name;
            myButton2.Click += new EventHandler(this.but_settings_Click);
            myButton2.Tag = (object)comboBox2;
            if (myButton2.Bottom + 30 <= this.Height)
                return;
            this.Height += 25;
        }

        private void BUT_detbutton_Click(object sender, EventArgs e)
        {
            this.Controls.Find("cmbbutton" + ((Control)sender).Name.Replace("mybut", ""), false)[0].Text = MissionPlanner.CamJoystick.CamJoystick.getPressedButton(this.CMB_joysticks.Text).ToString();
        }

        private void cmbbutton_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup)
                return;
            string s = ((Control)sender).Name.Replace("cmbbutton", "");
            MainV2.Camjoystick.changeButton(int.Parse(s), int.Parse(((Control)sender).Text));
        }

        private int[] getButtonNumbers()
        {
            int[] numArray = new int[128];
            numArray[0] = -1;
            for (int index = 0; index < numArray.Length - 1; ++index)
                numArray[index + 1] = index;
            return numArray;
        }

        private void cmbaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            int arrayoffset = int.Parse(((Control)sender).Tag.ToString());
            MissionPlanner.CamJoystick.CamJoystick.JoyButton button = MissionPlanner.CamJoystick.CamJoystick.self.getButton(arrayoffset);
            button.function = (MissionPlanner.CamJoystick.CamJoystick.buttonfunction)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.buttonfunction), ((Control)sender).Text);
            MissionPlanner.CamJoystick.CamJoystick.self.setButton(arrayoffset, button);
        }

        private void BUT_save_Click(object sender, EventArgs e)
        {
            MissionPlanner.CamJoystick.CamJoystick.self.saveconfig();
        }

        private void BUT_detch1_Click(object sender, EventArgs e)
        {
            this.CMB_CH1.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch2_Click(object sender, EventArgs e)
        {
            this.CMB_CH2.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch3_Click(object sender, EventArgs e)
        {
            this.CMB_CH3.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch4_Click(object sender, EventArgs e)
        {
            this.CMB_CH4.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch5_Click(object sender, EventArgs e)
        {
            this.CMB_CH5.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch6_Click(object sender, EventArgs e)
        {
            this.CMB_CH6.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch7_Click(object sender, EventArgs e)
        {
            this.CMB_CH7.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch8_Click(object sender, EventArgs e)
        {
            this.CMB_CH8.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch9_Click(object sender, EventArgs e)
        {
            this.CMB_CH9.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch10_Click(object sender, EventArgs e)
        {
            this.CMB_CH10.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch11_Click(object sender, EventArgs e)
        {
            this.CMB_CH11.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch12_Click(object sender, EventArgs e)
        {
            this.CMB_CH12.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch13_Click(object sender, EventArgs e)
        {
            this.CMB_CH13.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch14_Click(object sender, EventArgs e)
        {
            this.CMB_CH14.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch15_Click(object sender, EventArgs e)
        {
            this.CMB_CH15.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void BUT_detch16_Click(object sender, EventArgs e)
        {
            this.CMB_CH16.Text = MissionPlanner.CamJoystick.CamJoystick.getMovingAxis(this.CMB_joysticks.Text, 16000).ToString();
        }

        private void but_settings_Click(object sender, EventArgs e)
        {
            ComboBox tag = ((Control)sender).Tag as ComboBox;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.Camjoystick == null)
                {
                    MissionPlanner.CamJoystick.CamJoystick camJoystick = MainV2.Camjoystick;
                    if (camJoystick == null)
                    {
                        camJoystick = new MissionPlanner.CamJoystick.CamJoystick();
                        if (this.CMB_CH1.Text != "")
                            camJoystick.setChannel(1, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH1.Text), this.RevChRoll.Checked, 0);
                        if (this.CMB_CH2.Text != "")
                            camJoystick.setChannel(2, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH2.Text), this.revCHPitch.Checked, 0);
                        if (this.CMB_CH3.Text != "")
                            camJoystick.setChannel(3, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH3.Text), this.revCHzoom_in.Checked, 0);
                        if (this.CMB_CH4.Text != "")
                            camJoystick.setChannel(4, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH4.Text), this.revCHzoom_out.Checked, 0);
                        if (this.CMB_CH5.Text != "")
                            camJoystick.setChannel(5, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH5.Text), this.revCHnuc.Checked, 0);
                        if (this.CMB_CH6.Text != "")
                            camJoystick.setChannel(6, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH6.Text), this.revCH_BWhot.Checked, 0);
                        if (this.CMB_CH7.Text != "")
                            camJoystick.setChannel(7, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH7.Text), this.revCHdayIr.Checked, 0);
                        if (this.CMB_CH8.Text != "")
                            camJoystick.setChannel(8, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH8.Text), this.revCHrec.Checked, 0);
                        if (this.CMB_CH9.Text != "")
                            camJoystick.setChannel(9, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH9.Text), this.revCHpicCapture.Checked, 0);
                        if (this.CMB_CH10.Text != "")
                            camJoystick.setChannel(10, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH10.Text), this.RevChLaser.Checked, 0);
                        if (this.CMB_CH11.Text != "")
                            camJoystick.setChannel(11, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH11.Text), this.RevChTracking.Checked, 0);
                        if (this.CMB_CH12.Text != "")
                            camJoystick.setChannel(12, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH12.Text), this.RevChReTracking.Checked, 0);
                        if (this.CMB_CH13.Text != "")
                            camJoystick.setChannel(13, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH13.Text), this.RevChSingleYaw.Checked, 0);
                        if (this.CMB_CH14.Text != "")
                            camJoystick.setChannel(14, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH14.Text), this.RevChBIT.Checked, 0);
                        if (this.CMB_CH15.Text != "")
                            camJoystick.setChannel(15, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH15.Text), this.RevChRetracting.Checked, 0);
                        if (this.CMB_CH16.Text != "")
                            camJoystick.setChannel(16, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), this.CMB_CH16.Text), this.RevChFollowTarget.Checked, 0);
                        camJoystick.AcquireJoystick(this.CMB_joysticks.Text);
                        camJoystick.name = this.CMB_joysticks.Text;
                        this.noButtons = camJoystick.getNumButtons();
                        this.noButtons = Math.Min(15, this.noButtons);
                        MainV2.Camjoystick = camJoystick;
                        MissionPlanner.Utilities.ThemeManager.ApplyThemeTo((Control)this);
                        this.CMB_joysticks.SelectedIndex = this.CMB_joysticks.Items.IndexOf((object)camJoystick.name);
                    }
                    MainV2.comPort.MAV.cs.colibri_ch1 = camJoystick.getValueForChannel(1, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch2 = camJoystick.getValueForChannel(2, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch3 = camJoystick.getValueForChannel(3, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch4 = camJoystick.getValueForChannel(4, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch5 = camJoystick.getValueForChannel(5, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch6 = camJoystick.getValueForChannel(6, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch7 = camJoystick.getValueForChannel(7, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch8 = camJoystick.getValueForChannel(8, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch9 = camJoystick.getValueForChannel(9, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch10 = camJoystick.getValueForChannel(10, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch11 = camJoystick.getValueForChannel(11, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch12 = camJoystick.getValueForChannel(12, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch13 = camJoystick.getValueForChannel(13, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch14 = camJoystick.getValueForChannel(14, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch15 = camJoystick.getValueForChannel(15, this.CMB_joysticks.Text);
                    MainV2.comPort.MAV.cs.colibri_ch16 = camJoystick.getValueForChannel(16, this.CMB_joysticks.Text);
                }
            }
            catch (SharpDXException ex)
            {
                ex.ToString();
                if (MainV2.Camjoystick != null && MainV2.Camjoystick.enabled)
                    this.BUT_enable_Click();
            }
            catch
            {
            }
            this.progressBarRoll.Value = (int)MainV2.comPort.MAV.cs.colibri_ch1;
            this.progressBarPitch.Value = (int)MainV2.comPort.MAV.cs.colibri_ch2;
            this.ZoomInProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch3;
            this.ZoomOutProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch4;
            this.NUCProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch5;
            this.BWhotprogressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch6;
            this.DayIRprogressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch7;
            this.RecProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch8;
            this.PicCaptureProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch9;
            this.LaserProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch10;
            this.TrackingProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch11;
            this.ReTrackingProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch12;
            this.SingleYawProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch13;
            this.BITProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch14;
            this.RetractingProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch15;
            this.FollowTargetProgressBar.Value = (int)MainV2.comPort.MAV.cs.colibri_ch16;
            try
            {
                if (MainV2.Camjoystick != null)
                {
                    this.progressBarRoll.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(1);
                    this.progressBarPitch.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(2);
                    this.ZoomInProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(3);
                    this.ZoomOutProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(4);
                    this.NUCProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(5);
                    this.BWhotprogressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(6);
                    this.DayIRprogressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(7);
                    this.RecProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(8);
                    this.PicCaptureProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(9);
                    this.LaserProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(10);
                    this.TrackingProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(11);
                    this.ReTrackingProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(12);
                    this.SingleYawProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(13);
                    this.BITProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(14);
                    this.RetractingProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(15);
                    this.FollowTargetProgressBar.maxline = (int)MainV2.Camjoystick.getRawValueForChannel(16);
                }
            }
            catch
            {
            }
            try
            {
                for (int buttonno = 0; buttonno < this.noButtons; ++buttonno)
                    ((HorizontalProgressBar)this.Controls.Find("hbar" + buttonno.ToString(), false)[0]).Value = MainV2.Camjoystick.isButtonPressed(buttonno) ? 100 : 0;
            }
            catch
            {
            }
        }

        private void CMB_CH1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(1, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(2, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(3, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(4, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(5, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(6, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(7, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(8, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(9, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH10_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(10, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH11_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(11, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH12_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(12, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH13_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(13, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH14_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(14, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH15_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(15, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void CMB_CH16_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(16, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), ((Control)sender).Text));
        }

        private void revCH1_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(1, ((CheckBox)sender).Checked);
        }

        private void revCH2_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(2, ((CheckBox)sender).Checked);
        }

        private void revCH3_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(3, ((CheckBox)sender).Checked);
        }

        private void revCH4_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(4, ((CheckBox)sender).Checked);
        }

        private void revCH5_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(5, ((CheckBox)sender).Checked);
        }

        private void revCH6_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(6, ((CheckBox)sender).Checked);
        }

        private void revCH7_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(7, ((CheckBox)sender).Checked);
        }

        private void revCH8_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(8, ((CheckBox)sender).Checked);
        }

        private void revCH9_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(9, ((CheckBox)sender).Checked);
        }

        private void revCH10_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(10, ((CheckBox)sender).Checked);
        }

        private void revCH11_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(11, ((CheckBox)sender).Checked);
        }

        private void revCH12_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(12, ((CheckBox)sender).Checked);
        }

        private void revCH13_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(13, ((CheckBox)sender).Checked);
        }

        private void revCH14_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(14, ((CheckBox)sender).Checked);
        }

        private void RevChRetracting_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(15, ((CheckBox)sender).Checked);
        }

        private void RevChFollowTarget_CheckedChanged(object sender, EventArgs e)
        {
            if (MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setReverse(16, ((CheckBox)sender).Checked);
        }

        private void BUT_default_Click(object sender, EventArgs e)
        {
            if (this.startup || MainV2.Camjoystick == null)
                return;
            MainV2.Camjoystick.setAxis(1, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "X"));
            MainV2.Camjoystick.setAxis(2, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "Y"));
            MainV2.Camjoystick.setAxis(3, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn1"));
            MainV2.Camjoystick.setAxis(4, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn2"));
            MainV2.Camjoystick.setAxis(5, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn3"));
            MainV2.Camjoystick.setAxis(6, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn4"));
            MainV2.Camjoystick.setAxis(7, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn5"));
            MainV2.Camjoystick.setAxis(8, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn6"));
            MainV2.Camjoystick.setAxis(9, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn7"));
            MainV2.Camjoystick.setAxis(10, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn8"));
            MainV2.Camjoystick.setAxis(11, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn9"));
            MainV2.Camjoystick.setAxis(12, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn10"));
            MainV2.Camjoystick.setAxis(13, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn11"));
            MainV2.Camjoystick.setAxis(14, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn12"));
            MainV2.Camjoystick.setAxis(15, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn13"));
            MainV2.Camjoystick.setAxis(16, (MissionPlanner.CamJoystick.CamJoystick.joystickaxis)Enum.Parse(typeof(MissionPlanner.CamJoystick.CamJoystick.joystickaxis), "btn14"));
            this.CMB_CH1.Text = "X";
            this.CMB_CH2.Text = "Y";
            this.CMB_CH3.Text = "btn1";
            this.CMB_CH4.Text = "btn2";
            this.CMB_CH5.Text = "btn3";
            this.CMB_CH6.Text = "btn4";
            this.CMB_CH7.Text = "btn5";
            this.CMB_CH8.Text = "btn6";
            this.CMB_CH9.Text = "btn7";
            this.CMB_CH10.Text = "btn8";
            this.CMB_CH11.Text = "btn9";
            this.CMB_CH12.Text = "btn10";
            this.CMB_CH13.Text = "btn11";
            this.CMB_CH14.Text = "btn12";
            this.CMB_CH15.Text = "btn13";
            this.CMB_CH16.Text = "btn14";
        }

        private void CamJoystickSetup_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.timer1.Stop();
        }

        private void BUT_enable_click(object sender, EventArgs e)
        {
            if (!(this.CMB_joysticks.Text != ""))
                return;
            try
            {
                MainV2.Camjoystick.enabled = false;
                MainV2.Camjoystick.UnAcquireJoyStick();
                MainV2.Camjoystick = (MissionPlanner.CamJoystick.CamJoystick)null;
            }
            catch
            {
            }
            if (this.CMB_joysticks.Text == "Logitech Dual Action # 1")
                this.CMB_joysticks.SelectedIndex = 0;
            else if (this.CMB_joysticks.Text == "Logitech Dual Action # 2")
                this.CMB_joysticks.SelectedIndex = 1;
            this.BUT_enable_Click();
        }

        private void CMB_joysticks_MouseClick(object sender, MouseEventArgs e)
        {
            this.CMB_joysticks.Items.Clear();
            IList<DeviceInstance> devices = MissionPlanner.CamJoystick.CamJoystick.getDevices();
            int num = 1;
            foreach (DeviceInstance deviceInstance in (IEnumerable<DeviceInstance>)devices)
            {
                this.CMB_joysticks.Items.Add((object)(deviceInstance.ProductName + " # " + (object)num));
                ++num;
            }
            if (this.CMB_joysticks.Items.Count <= 0 || this.CMB_joysticks.SelectedIndex != -1)
                return;
            this.CMB_joysticks.SelectedIndex = 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new Container();
            this.label5 = new Label();
            this.CMB_joysticks = new ComboBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.label4 = new Label();
            this.label6 = new Label();
            this.label7 = new Label();
            this.label8 = new Label();
            this.label11 = new Label();
            this.label12 = new Label();
            this.label13 = new Label();
            this.label14 = new Label();
            this.label15 = new Label();
            this.label9 = new Label();
            this.CMB_CH8 = new ComboBox();
            this.CMB_CH7 = new ComboBox();
            this.CMB_CH6 = new ComboBox();
            this.CMB_CH5 = new ComboBox();
            this.CMB_CH4 = new ComboBox();
            this.CMB_CH3 = new ComboBox();
            this.CMB_CH2 = new ComboBox();
            this.CMB_CH1 = new ComboBox();
            this.CMB_CH12 = new ComboBox();
            this.CMB_CH11 = new ComboBox();
            this.CMB_CH10 = new ComboBox();
            this.CMB_CH9 = new ComboBox();
            this.label10 = new Label();
            this.revCHpicCapture = new CheckBox();
            this.revCHrec = new CheckBox();
            this.revCHdayIr = new CheckBox();
            this.revCH_BWhot = new CheckBox();
            this.revCHnuc = new CheckBox();
            this.revCHzoom_out = new CheckBox();
            this.revCHzoom_in = new CheckBox();
            this.revCHPitch = new CheckBox();
            this.RevChRoll = new CheckBox();
            this.RevChLaser = new CheckBox();
            this.RevChTracking = new CheckBox();
            this.RevChReTracking = new CheckBox();
            this.timer1 = new Timer(this.components);
            this.myButton1 = new MyButton();
            this.NUCProgressBar = new HorizontalProgressBar();
            this.ZoomOutProgressBar = new HorizontalProgressBar();
            this.ZoomInProgressBar = new HorizontalProgressBar();
            this.progressBarPitch = new HorizontalProgressBar();
            this.ReTrackingProgressBar = new HorizontalProgressBar();
            this.TrackingProgressBar = new HorizontalProgressBar();
            this.LaserProgressBar = new HorizontalProgressBar();
            this.PicCaptureProgressBar = new HorizontalProgressBar();
            this.RecProgressBar = new HorizontalProgressBar();
            this.DayIRprogressBar = new HorizontalProgressBar();
            this.BWhotprogressBar = new HorizontalProgressBar();
            this.progressBarRoll = new HorizontalProgressBar();
            this.BUT_detch7 = new MyButton();
            this.BUT_detch11 = new MyButton();
            this.BUT_detch6 = new MyButton();
            this.BUT_detch10 = new MyButton();
            this.BUT_detch5 = new MyButton();
            this.BUT_detch9 = new MyButton();
            this.BUT_detch12 = new MyButton();
            this.BUT_detch4 = new MyButton();
            this.BUT_detch8 = new MyButton();
            this.BUT_detch3 = new MyButton();
            this.BUT_detch2 = new MyButton();
            this.BUT_detch1 = new MyButton();
            this.BUT_enable = new MyButton();
            this.BUT_save = new MyButton();
            this.RevChSingleYaw = new CheckBox();
            this.SingleYawProgressBar = new HorizontalProgressBar();
            this.BUT_detch13 = new MyButton();
            this.CMB_CH13 = new ComboBox();
            this.label16 = new Label();
            this.RevChBIT = new CheckBox();
            this.BITProgressBar = new HorizontalProgressBar();
            this.BUT_detch14 = new MyButton();
            this.CMB_CH14 = new ComboBox();
            this.label18 = new Label();
            this.RevChRetracting = new CheckBox();
            this.RetractingProgressBar = new HorizontalProgressBar();
            this.BUT_detch15 = new MyButton();
            this.CMB_CH15 = new ComboBox();
            this.label17 = new Label();
            this.RevChFollowTarget = new CheckBox();
            this.FollowTargetProgressBar = new HorizontalProgressBar();
            this.BUT_detch16 = new MyButton();
            this.CMB_CH16 = new ComboBox();
            this.label19 = new Label();
            this.SuspendLayout();
            this.label5.AutoSize = true;
            this.label5.ImeMode = ImeMode.NoControl;
            this.label5.Location = new Point(12, 18);
            this.label5.Name = "label5";
            this.label5.Size = new Size(45, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Joystick";
            this.CMB_joysticks.Location = new Point(73, 15);
            this.CMB_joysticks.Name = "CMB_joysticks";
            this.CMB_joysticks.Size = new Size(235, 21);
            this.CMB_joysticks.TabIndex = 26;
            this.CMB_joysticks.MouseClick += new MouseEventHandler(this.CMB_joysticks_MouseClick);
            this.label1.AutoSize = true;
            this.label1.ImeMode = ImeMode.NoControl;
            this.label1.Location = new Point(12, 81);
            this.label1.Name = "label1";
            this.label1.Size = new Size(51, 13);
            this.label1.TabIndex = 58;
            this.label1.Text = "Roll/Yaw";
            this.label2.AutoSize = true;
            this.label2.ImeMode = ImeMode.NoControl;
            this.label2.Location = new Point(12, 110);
            this.label2.Name = "label2";
            this.label2.Size = new Size(31, 13);
            this.label2.TabIndex = 59;
            this.label2.Text = "Pitch";
            this.label3.AutoSize = true;
            this.label3.ImeMode = ImeMode.NoControl;
            this.label3.Location = new Point(12, 139);
            this.label3.Name = "label3";
            this.label3.Size = new Size(45, 13);
            this.label3.TabIndex = 60;
            this.label3.Text = "Zoom in";
            this.label4.AutoSize = true;
            this.label4.ImeMode = ImeMode.NoControl;
            this.label4.Location = new Point(12, 168);
            this.label4.Name = "label4";
            this.label4.Size = new Size(52, 13);
            this.label4.TabIndex = 59;
            this.label4.Text = "Zoom out";
            this.label6.AutoSize = true;
            this.label6.ImeMode = ImeMode.NoControl;
            this.label6.Location = new Point(12, 197);
            this.label6.Name = "label6";
            this.label6.Size = new Size(30, 13);
            this.label6.TabIndex = 60;
            this.label6.Text = "NUC";
            this.label7.AutoSize = true;
            this.label7.ImeMode = ImeMode.NoControl;
            this.label7.Location = new Point(12, 226);
            this.label7.Name = "label7";
            this.label7.Size = new Size(50, 13);
            this.label7.TabIndex = 61;
            this.label7.Text = "B/W Hot";
            this.label8.AutoSize = true;
            this.label8.ImeMode = ImeMode.NoControl;
            this.label8.Location = new Point(12, (int)byte.MaxValue);
            this.label8.Name = "label8";
            this.label8.Size = new Size(42, 13);
            this.label8.TabIndex = 62;
            this.label8.Text = "Day/IR";
            this.label11.AutoSize = true;
            this.label11.ImeMode = ImeMode.NoControl;
            this.label11.Location = new Point(12, 400);
            this.label11.Name = "label11";
            this.label11.Size = new Size(59, 13);
            this.label11.TabIndex = 67;
            this.label11.Text = "Retracking";
            this.label12.AutoSize = true;
            this.label12.ImeMode = ImeMode.NoControl;
            this.label12.Location = new Point(12, 371);
            this.label12.Name = "label12";
            this.label12.Size = new Size(49, 13);
            this.label12.TabIndex = 65;
            this.label12.Text = "Tracking";
            this.label13.AutoSize = true;
            this.label13.ImeMode = ImeMode.NoControl;
            this.label13.Location = new Point(12, 342);
            this.label13.Name = "label13";
            this.label13.Size = new Size(69, 13);
            this.label13.TabIndex = 63;
            this.label13.Text = "Laser On/Off";
            this.label14.AutoSize = true;
            this.label14.ImeMode = ImeMode.NoControl;
            this.label14.Location = new Point(12, 313);
            this.label14.Name = "label14";
            this.label14.Size = new Size(76, 13);
            this.label14.TabIndex = 66;
            this.label14.Text = "Image Capture";
            this.label15.AutoSize = true;
            this.label15.ImeMode = ImeMode.NoControl;
            this.label15.Location = new Point(12, 284);
            this.label15.Name = "label15";
            this.label15.Size = new Size(63, 13);
            this.label15.TabIndex = 64;
            this.label15.Text = "Rec On/Off";
            this.label9.AutoSize = true;
            this.label9.ImeMode = ImeMode.NoControl;
            this.label9.Location = new Point(118, 54);
            this.label9.Name = "label9";
            this.label9.Size = new Size(73, 13);
            this.label9.TabIndex = 68;
            this.label9.Text = "Controller Axis";
            this.CMB_CH8.FormattingEnabled = true;
            this.CMB_CH8.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH8.Location = new Point(120, 281);
            this.CMB_CH8.Name = "CMB_CH8";
            this.CMB_CH8.Size = new Size(70, 21);
            this.CMB_CH8.TabIndex = 76;
            this.CMB_CH8.SelectedIndexChanged += new EventHandler(this.CMB_CH8_SelectedIndexChanged);
            this.CMB_CH7.FormattingEnabled = true;
            this.CMB_CH7.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH7.Location = new Point(120, 252);
            this.CMB_CH7.Name = "CMB_CH7";
            this.CMB_CH7.Size = new Size(70, 21);
            this.CMB_CH7.TabIndex = 75;
            this.CMB_CH7.SelectedIndexChanged += new EventHandler(this.CMB_CH7_SelectedIndexChanged);
            this.CMB_CH6.FormattingEnabled = true;
            this.CMB_CH6.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH6.Location = new Point(120, 223);
            this.CMB_CH6.Name = "CMB_CH6";
            this.CMB_CH6.Size = new Size(70, 21);
            this.CMB_CH6.TabIndex = 74;
            this.CMB_CH6.SelectedIndexChanged += new EventHandler(this.CMB_CH6_SelectedIndexChanged);
            this.CMB_CH5.FormattingEnabled = true;
            this.CMB_CH5.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH5.Location = new Point(120, 194);
            this.CMB_CH5.Name = "CMB_CH5";
            this.CMB_CH5.Size = new Size(70, 21);
            this.CMB_CH5.TabIndex = 73;
            this.CMB_CH5.SelectedIndexChanged += new EventHandler(this.CMB_CH5_SelectedIndexChanged);
            this.CMB_CH4.FormattingEnabled = true;
            this.CMB_CH4.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH4.Location = new Point(120, 165);
            this.CMB_CH4.Name = "CMB_CH4";
            this.CMB_CH4.Size = new Size(70, 21);
            this.CMB_CH4.TabIndex = 72;
            this.CMB_CH4.SelectedValueChanged += new EventHandler(this.CMB_CH4_SelectedIndexChanged);
            this.CMB_CH3.FormattingEnabled = true;
            this.CMB_CH3.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH3.Location = new Point(120, 136);
            this.CMB_CH3.Name = "CMB_CH3";
            this.CMB_CH3.Size = new Size(70, 21);
            this.CMB_CH3.TabIndex = 71;
            this.CMB_CH3.SelectedIndexChanged += new EventHandler(this.CMB_CH3_SelectedIndexChanged);
            this.CMB_CH2.FormattingEnabled = true;
            this.CMB_CH2.Items.AddRange(new object[7]
            {
        (object) "None",
        (object) "X",
        (object) "Y",
        (object) "Z",
        (object) "RZ",
        (object) "HatUpDown",
        (object) "HatLeftRight"
            });
            this.CMB_CH2.Location = new Point(120, 107);
            this.CMB_CH2.Name = "CMB_CH2";
            this.CMB_CH2.Size = new Size(70, 21);
            this.CMB_CH2.TabIndex = 70;
            this.CMB_CH2.SelectedIndexChanged += new EventHandler(this.CMB_CH2_SelectedIndexChanged);
            this.CMB_CH1.FormattingEnabled = true;
            this.CMB_CH1.Items.AddRange(new object[7]
            {
        (object) "None",
        (object) "X",
        (object) "Y",
        (object) "Z",
        (object) "RZ",
        (object) "HatUpDown",
        (object) "HatLeftRight"
            });
            this.CMB_CH1.Location = new Point(120, 78);
            this.CMB_CH1.Name = "CMB_CH1";
            this.CMB_CH1.Size = new Size(70, 21);
            this.CMB_CH1.TabIndex = 69;
            this.CMB_CH1.SelectedIndexChanged += new EventHandler(this.CMB_CH1_SelectedIndexChanged);
            this.CMB_CH12.FormattingEnabled = true;
            this.CMB_CH12.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH12.Location = new Point(120, 397);
            this.CMB_CH12.Name = "CMB_CH12";
            this.CMB_CH12.Size = new Size(70, 21);
            this.CMB_CH12.TabIndex = 80;
            this.CMB_CH12.SelectedIndexChanged += new EventHandler(this.CMB_CH12_SelectedIndexChanged);
            this.CMB_CH11.FormattingEnabled = true;
            this.CMB_CH11.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH11.Location = new Point(120, 368);
            this.CMB_CH11.Name = "CMB_CH11";
            this.CMB_CH11.Size = new Size(70, 21);
            this.CMB_CH11.TabIndex = 79;
            this.CMB_CH11.SelectedIndexChanged += new EventHandler(this.CMB_CH11_SelectedIndexChanged);
            this.CMB_CH10.FormattingEnabled = true;
            this.CMB_CH10.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH10.Location = new Point(120, 339);
            this.CMB_CH10.Name = "CMB_CH10";
            this.CMB_CH10.Size = new Size(70, 21);
            this.CMB_CH10.TabIndex = 78;
            this.CMB_CH10.SelectedIndexChanged += new EventHandler(this.CMB_CH10_SelectedIndexChanged);
            this.CMB_CH9.FormattingEnabled = true;
            this.CMB_CH9.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH9.Location = new Point(120, 310);
            this.CMB_CH9.Name = "CMB_CH9";
            this.CMB_CH9.Size = new Size(70, 21);
            this.CMB_CH9.TabIndex = 77;
            this.CMB_CH9.SelectedIndexChanged += new EventHandler(this.CMB_CH9_SelectedIndexChanged);
            this.label10.AutoSize = true;
            this.label10.ImeMode = ImeMode.NoControl;
            this.label10.Location = new Point(346, 54);
            this.label10.Name = "label10";
            this.label10.Size = new Size(47, 13);
            this.label10.TabIndex = 101;
            this.label10.Text = "Reverse";
            this.revCHpicCapture.AutoSize = true;
            this.revCHpicCapture.ImeMode = ImeMode.NoControl;
            this.revCHpicCapture.Location = new Point(353, 312);
            this.revCHpicCapture.Name = "revCHpicCapture";
            this.revCHpicCapture.Size = new Size(15, 14);
            this.revCHpicCapture.TabIndex = 109;
            this.revCHpicCapture.UseVisualStyleBackColor = true;
            this.revCHpicCapture.CheckedChanged += new EventHandler(this.revCH9_CheckedChanged);
            this.revCHrec.AutoSize = true;
            this.revCHrec.ImeMode = ImeMode.NoControl;
            this.revCHrec.Location = new Point(353, 283);
            this.revCHrec.Name = "revCHrec";
            this.revCHrec.Size = new Size(15, 14);
            this.revCHrec.TabIndex = 108;
            this.revCHrec.UseVisualStyleBackColor = true;
            this.revCHrec.CheckedChanged += new EventHandler(this.revCH8_CheckedChanged);
            this.revCHdayIr.AutoSize = true;
            this.revCHdayIr.ImeMode = ImeMode.NoControl;
            this.revCHdayIr.Location = new Point(353, (int)byte.MaxValue);
            this.revCHdayIr.Name = "revCHdayIr";
            this.revCHdayIr.Size = new Size(15, 14);
            this.revCHdayIr.TabIndex = 107;
            this.revCHdayIr.UseVisualStyleBackColor = true;
            this.revCHdayIr.CheckedChanged += new EventHandler(this.revCH7_CheckedChanged);
            this.revCH_BWhot.AutoSize = true;
            this.revCH_BWhot.ImeMode = ImeMode.NoControl;
            this.revCH_BWhot.Location = new Point(353, 226);
            this.revCH_BWhot.Name = "revCH_BWhot";
            this.revCH_BWhot.Size = new Size(15, 14);
            this.revCH_BWhot.TabIndex = 106;
            this.revCH_BWhot.UseVisualStyleBackColor = true;
            this.revCH_BWhot.CheckedChanged += new EventHandler(this.revCH6_CheckedChanged);
            this.revCHnuc.AutoSize = true;
            this.revCHnuc.ImeMode = ImeMode.NoControl;
            this.revCHnuc.Location = new Point(353, 197);
            this.revCHnuc.Name = "revCHnuc";
            this.revCHnuc.Size = new Size(15, 14);
            this.revCHnuc.TabIndex = 105;
            this.revCHnuc.UseVisualStyleBackColor = true;
            this.revCHnuc.CheckedChanged += new EventHandler(this.revCH5_CheckedChanged);
            this.revCHzoom_out.AutoSize = true;
            this.revCHzoom_out.ImeMode = ImeMode.NoControl;
            this.revCHzoom_out.Location = new Point(353, 168);
            this.revCHzoom_out.Name = "revCHzoom_out";
            this.revCHzoom_out.Size = new Size(15, 14);
            this.revCHzoom_out.TabIndex = 104;
            this.revCHzoom_out.UseVisualStyleBackColor = true;
            this.revCHzoom_out.CheckedChanged += new EventHandler(this.revCH4_CheckedChanged);
            this.revCHzoom_in.AutoSize = true;
            this.revCHzoom_in.ImeMode = ImeMode.NoControl;
            this.revCHzoom_in.Location = new Point(353, 139);
            this.revCHzoom_in.Name = "revCHzoom_in";
            this.revCHzoom_in.Size = new Size(15, 14);
            this.revCHzoom_in.TabIndex = 103;
            this.revCHzoom_in.UseVisualStyleBackColor = true;
            this.revCHzoom_in.CheckedChanged += new EventHandler(this.revCH3_CheckedChanged);
            this.revCHPitch.AutoSize = true;
            this.revCHPitch.ImeMode = ImeMode.NoControl;
            this.revCHPitch.Location = new Point(353, 110);
            this.revCHPitch.Name = "revCHPitch";
            this.revCHPitch.Size = new Size(15, 14);
            this.revCHPitch.TabIndex = 102;
            this.revCHPitch.UseVisualStyleBackColor = true;
            this.revCHPitch.CheckedChanged += new EventHandler(this.revCH2_CheckedChanged);
            this.RevChRoll.AutoSize = true;
            this.RevChRoll.ImeMode = ImeMode.NoControl;
            this.RevChRoll.Location = new Point(353, 81);
            this.RevChRoll.Name = "RevChRoll";
            this.RevChRoll.Size = new Size(15, 14);
            this.RevChRoll.TabIndex = 117;
            this.RevChRoll.UseVisualStyleBackColor = true;
            this.RevChRoll.CheckedChanged += new EventHandler(this.revCH1_CheckedChanged);
            this.RevChLaser.AutoSize = true;
            this.RevChLaser.ImeMode = ImeMode.NoControl;
            this.RevChLaser.Location = new Point(353, 341);
            this.RevChLaser.Name = "RevChLaser";
            this.RevChLaser.Size = new Size(15, 14);
            this.RevChLaser.TabIndex = 116;
            this.RevChLaser.UseVisualStyleBackColor = true;
            this.RevChLaser.CheckedChanged += new EventHandler(this.revCH10_CheckedChanged);
            this.RevChTracking.AutoSize = true;
            this.RevChTracking.ImeMode = ImeMode.NoControl;
            this.RevChTracking.Location = new Point(353, 371);
            this.RevChTracking.Name = "RevChTracking";
            this.RevChTracking.Size = new Size(15, 14);
            this.RevChTracking.TabIndex = 115;
            this.RevChTracking.UseVisualStyleBackColor = true;
            this.RevChTracking.CheckedChanged += new EventHandler(this.revCH11_CheckedChanged);
            this.RevChReTracking.AutoSize = true;
            this.RevChReTracking.ImeMode = ImeMode.NoControl;
            this.RevChReTracking.Location = new Point(353, 400);
            this.RevChReTracking.Name = "RevChReTracking";
            this.RevChReTracking.Size = new Size(15, 14);
            this.RevChReTracking.TabIndex = 114;
            this.RevChReTracking.UseVisualStyleBackColor = true;
            this.RevChReTracking.CheckedChanged += new EventHandler(this.revCH12_CheckedChanged);
            this.timer1.Tick += new EventHandler(this.timer1_Tick);
            this.myButton1.ImeMode = ImeMode.NoControl;
            this.myButton1.Location = new Point(316, 13);
            this.myButton1.Name = "myButton1";
            this.myButton1.Size = new Size(75, 23);
            this.myButton1.TabIndex = 120;
            this.myButton1.Text = "Update";
            this.myButton1.UseVisualStyleBackColor = true;
            this.myButton1.Click += new EventHandler(this.BUT_enable_click);
            this.NUCProgressBar.DrawLabel = true;
            this.NUCProgressBar.ImeMode = ImeMode.NoControl;
            this.NUCProgressBar.Label = (string)null;
            this.NUCProgressBar.Location = new Point(247, 192);
            this.NUCProgressBar.Maximum = 2200;
            this.NUCProgressBar.maxline = 0;
            this.NUCProgressBar.Minimum = 800;
            this.NUCProgressBar.minline = 0;
            this.NUCProgressBar.Name = "NUCProgressBar";
            this.NUCProgressBar.Size = new Size(100, 23);
            this.NUCProgressBar.TabIndex = 100;
            this.NUCProgressBar.Value = 800;
            this.ZoomOutProgressBar.DrawLabel = true;
            this.ZoomOutProgressBar.ImeMode = ImeMode.NoControl;
            this.ZoomOutProgressBar.Label = (string)null;
            this.ZoomOutProgressBar.Location = new Point(247, 163);
            this.ZoomOutProgressBar.Maximum = 2200;
            this.ZoomOutProgressBar.maxline = 0;
            this.ZoomOutProgressBar.Minimum = 800;
            this.ZoomOutProgressBar.minline = 0;
            this.ZoomOutProgressBar.Name = "ZoomOutProgressBar";
            this.ZoomOutProgressBar.Size = new Size(100, 23);
            this.ZoomOutProgressBar.TabIndex = 99;
            this.ZoomOutProgressBar.Value = 800;
            this.ZoomInProgressBar.DrawLabel = true;
            this.ZoomInProgressBar.ImeMode = ImeMode.NoControl;
            this.ZoomInProgressBar.Label = (string)null;
            this.ZoomInProgressBar.Location = new Point(247, 134);
            this.ZoomInProgressBar.Maximum = 2200;
            this.ZoomInProgressBar.maxline = 0;
            this.ZoomInProgressBar.Minimum = 800;
            this.ZoomInProgressBar.minline = 0;
            this.ZoomInProgressBar.Name = "ZoomInProgressBar";
            this.ZoomInProgressBar.Size = new Size(100, 23);
            this.ZoomInProgressBar.TabIndex = 98;
            this.ZoomInProgressBar.Value = 800;
            this.progressBarPitch.DrawLabel = true;
            this.progressBarPitch.ImeMode = ImeMode.NoControl;
            this.progressBarPitch.Label = (string)null;
            this.progressBarPitch.Location = new Point(247, 105);
            this.progressBarPitch.Maximum = 1023;
            this.progressBarPitch.maxline = 0;
            this.progressBarPitch.minline = 0;
            this.progressBarPitch.Name = "progressBarPitch";
            this.progressBarPitch.Size = new Size(100, 23);
            this.progressBarPitch.TabIndex = 97;
            this.ReTrackingProgressBar.DrawLabel = true;
            this.ReTrackingProgressBar.ImeMode = ImeMode.NoControl;
            this.ReTrackingProgressBar.Label = (string)null;
            this.ReTrackingProgressBar.Location = new Point(247, 395);
            this.ReTrackingProgressBar.Maximum = 2200;
            this.ReTrackingProgressBar.maxline = 0;
            this.ReTrackingProgressBar.Minimum = 800;
            this.ReTrackingProgressBar.minline = 0;
            this.ReTrackingProgressBar.Name = "ReTrackingProgressBar";
            this.ReTrackingProgressBar.Size = new Size(100, 23);
            this.ReTrackingProgressBar.TabIndex = 96;
            this.ReTrackingProgressBar.Value = 800;
            this.TrackingProgressBar.DrawLabel = true;
            this.TrackingProgressBar.ImeMode = ImeMode.NoControl;
            this.TrackingProgressBar.Label = (string)null;
            this.TrackingProgressBar.Location = new Point(247, 366);
            this.TrackingProgressBar.Maximum = 2200;
            this.TrackingProgressBar.maxline = 0;
            this.TrackingProgressBar.Minimum = 800;
            this.TrackingProgressBar.minline = 0;
            this.TrackingProgressBar.Name = "TrackingProgressBar";
            this.TrackingProgressBar.Size = new Size(100, 23);
            this.TrackingProgressBar.TabIndex = 95;
            this.TrackingProgressBar.Value = 800;
            this.LaserProgressBar.DrawLabel = true;
            this.LaserProgressBar.ImeMode = ImeMode.NoControl;
            this.LaserProgressBar.Label = (string)null;
            this.LaserProgressBar.Location = new Point(247, 337);
            this.LaserProgressBar.Maximum = 2200;
            this.LaserProgressBar.maxline = 0;
            this.LaserProgressBar.Minimum = 800;
            this.LaserProgressBar.minline = 0;
            this.LaserProgressBar.Name = "LaserProgressBar";
            this.LaserProgressBar.Size = new Size(100, 23);
            this.LaserProgressBar.TabIndex = 94;
            this.LaserProgressBar.Value = 800;
            this.PicCaptureProgressBar.DrawLabel = true;
            this.PicCaptureProgressBar.ImeMode = ImeMode.NoControl;
            this.PicCaptureProgressBar.Label = (string)null;
            this.PicCaptureProgressBar.Location = new Point(247, 308);
            this.PicCaptureProgressBar.Maximum = 2200;
            this.PicCaptureProgressBar.maxline = 0;
            this.PicCaptureProgressBar.Minimum = 800;
            this.PicCaptureProgressBar.minline = 0;
            this.PicCaptureProgressBar.Name = "PicCaptureProgressBar";
            this.PicCaptureProgressBar.Size = new Size(100, 23);
            this.PicCaptureProgressBar.TabIndex = 93;
            this.PicCaptureProgressBar.Value = 800;
            this.RecProgressBar.DrawLabel = true;
            this.RecProgressBar.ImeMode = ImeMode.NoControl;
            this.RecProgressBar.Label = (string)null;
            this.RecProgressBar.Location = new Point(247, 279);
            this.RecProgressBar.Maximum = 2200;
            this.RecProgressBar.maxline = 0;
            this.RecProgressBar.Minimum = 800;
            this.RecProgressBar.minline = 0;
            this.RecProgressBar.Name = "RecProgressBar";
            this.RecProgressBar.Size = new Size(100, 23);
            this.RecProgressBar.TabIndex = 92;
            this.RecProgressBar.Value = 800;
            this.DayIRprogressBar.DrawLabel = true;
            this.DayIRprogressBar.ImeMode = ImeMode.NoControl;
            this.DayIRprogressBar.Label = (string)null;
            this.DayIRprogressBar.Location = new Point(247, 250);
            this.DayIRprogressBar.Maximum = 2200;
            this.DayIRprogressBar.maxline = 0;
            this.DayIRprogressBar.Minimum = 800;
            this.DayIRprogressBar.minline = 0;
            this.DayIRprogressBar.Name = "DayIRprogressBar";
            this.DayIRprogressBar.Size = new Size(100, 23);
            this.DayIRprogressBar.TabIndex = 91;
            this.DayIRprogressBar.Value = 800;
            this.BWhotprogressBar.DrawLabel = true;
            this.BWhotprogressBar.ImeMode = ImeMode.NoControl;
            this.BWhotprogressBar.Label = (string)null;
            this.BWhotprogressBar.Location = new Point(247, 221);
            this.BWhotprogressBar.Maximum = 2200;
            this.BWhotprogressBar.maxline = 0;
            this.BWhotprogressBar.Minimum = 800;
            this.BWhotprogressBar.minline = 0;
            this.BWhotprogressBar.Name = "BWhotprogressBar";
            this.BWhotprogressBar.Size = new Size(100, 23);
            this.BWhotprogressBar.TabIndex = 90;
            this.BWhotprogressBar.Value = 800;
            this.progressBarRoll.DrawLabel = true;
            this.progressBarRoll.ImeMode = ImeMode.NoControl;
            this.progressBarRoll.Label = (string)null;
            this.progressBarRoll.Location = new Point(247, 76);
            this.progressBarRoll.Maximum = 1023;
            this.progressBarRoll.maxline = 0;
            this.progressBarRoll.minline = 0;
            this.progressBarRoll.Name = "progressBarRoll";
            this.progressBarRoll.Size = new Size(100, 23);
            this.progressBarRoll.TabIndex = 89;
            this.BUT_detch7.ImeMode = ImeMode.NoControl;
            this.BUT_detch7.Location = new Point(196, 250);
            this.BUT_detch7.Name = "BUT_detch7";
            this.BUT_detch7.Size = new Size(45, 23);
            this.BUT_detch7.TabIndex = 88;
            this.BUT_detch7.Text = "Auto Detect";
            this.BUT_detch7.UseVisualStyleBackColor = true;
            this.BUT_detch7.Click += new EventHandler(this.BUT_detch7_Click);
            this.BUT_detch11.ImeMode = ImeMode.NoControl;
            this.BUT_detch11.Location = new Point(196, 366);
            this.BUT_detch11.Name = "BUT_detch11";
            this.BUT_detch11.Size = new Size(45, 23);
            this.BUT_detch11.TabIndex = 88;
            this.BUT_detch11.Text = "Auto Detect";
            this.BUT_detch11.UseVisualStyleBackColor = true;
            this.BUT_detch11.Click += new EventHandler(this.BUT_detch11_Click);
            this.BUT_detch6.ImeMode = ImeMode.NoControl;
            this.BUT_detch6.Location = new Point(196, 221);
            this.BUT_detch6.Name = "BUT_detch6";
            this.BUT_detch6.Size = new Size(45, 23);
            this.BUT_detch6.TabIndex = 87;
            this.BUT_detch6.Text = "Auto Detect";
            this.BUT_detch6.UseVisualStyleBackColor = true;
            this.BUT_detch6.Click += new EventHandler(this.BUT_detch6_Click);
            this.BUT_detch10.ImeMode = ImeMode.NoControl;
            this.BUT_detch10.Location = new Point(196, 337);
            this.BUT_detch10.Name = "BUT_detch10";
            this.BUT_detch10.Size = new Size(45, 23);
            this.BUT_detch10.TabIndex = 87;
            this.BUT_detch10.Text = "Auto Detect";
            this.BUT_detch10.UseVisualStyleBackColor = true;
            this.BUT_detch10.Click += new EventHandler(this.BUT_detch10_Click);
            this.BUT_detch5.ImeMode = ImeMode.NoControl;
            this.BUT_detch5.Location = new Point(196, 192);
            this.BUT_detch5.Name = "BUT_detch5";
            this.BUT_detch5.Size = new Size(45, 23);
            this.BUT_detch5.TabIndex = 86;
            this.BUT_detch5.Text = "Auto Detect";
            this.BUT_detch5.UseVisualStyleBackColor = true;
            this.BUT_detch5.Click += new EventHandler(this.BUT_detch5_Click);
            this.BUT_detch9.ImeMode = ImeMode.NoControl;
            this.BUT_detch9.Location = new Point(196, 308);
            this.BUT_detch9.Name = "BUT_detch9";
            this.BUT_detch9.Size = new Size(45, 23);
            this.BUT_detch9.TabIndex = 86;
            this.BUT_detch9.Text = "Auto Detect";
            this.BUT_detch9.UseVisualStyleBackColor = true;
            this.BUT_detch9.Click += new EventHandler(this.BUT_detch9_Click);
            this.BUT_detch12.ImeMode = ImeMode.NoControl;
            this.BUT_detch12.Location = new Point(196, 395);
            this.BUT_detch12.Name = "BUT_detch12";
            this.BUT_detch12.Size = new Size(45, 23);
            this.BUT_detch12.TabIndex = 85;
            this.BUT_detch12.Text = "Auto Detect";
            this.BUT_detch12.UseVisualStyleBackColor = true;
            this.BUT_detch12.Click += new EventHandler(this.BUT_detch12_Click);
            this.BUT_detch4.ImeMode = ImeMode.NoControl;
            this.BUT_detch4.Location = new Point(196, 163);
            this.BUT_detch4.Name = "BUT_detch4";
            this.BUT_detch4.Size = new Size(45, 23);
            this.BUT_detch4.TabIndex = 85;
            this.BUT_detch4.Text = "Auto Detect";
            this.BUT_detch4.UseVisualStyleBackColor = true;
            this.BUT_detch4.Click += new EventHandler(this.BUT_detch4_Click);
            this.BUT_detch8.ImeMode = ImeMode.NoControl;
            this.BUT_detch8.Location = new Point(196, 279);
            this.BUT_detch8.Name = "BUT_detch8";
            this.BUT_detch8.Size = new Size(45, 23);
            this.BUT_detch8.TabIndex = 84;
            this.BUT_detch8.Text = "Auto Detect";
            this.BUT_detch8.UseVisualStyleBackColor = true;
            this.BUT_detch8.Click += new EventHandler(this.BUT_detch8_Click);
            this.BUT_detch3.ImeMode = ImeMode.NoControl;
            this.BUT_detch3.Location = new Point(196, 134);
            this.BUT_detch3.Name = "BUT_detch3";
            this.BUT_detch3.Size = new Size(45, 23);
            this.BUT_detch3.TabIndex = 83;
            this.BUT_detch3.Text = "Auto Detect";
            this.BUT_detch3.UseVisualStyleBackColor = true;
            this.BUT_detch3.Click += new EventHandler(this.BUT_detch3_Click);
            this.BUT_detch2.ImeMode = ImeMode.NoControl;
            this.BUT_detch2.Location = new Point(196, 105);
            this.BUT_detch2.Name = "BUT_detch2";
            this.BUT_detch2.Size = new Size(45, 23);
            this.BUT_detch2.TabIndex = 82;
            this.BUT_detch2.Text = "Auto Detect";
            this.BUT_detch2.UseVisualStyleBackColor = true;
            this.BUT_detch2.Click += new EventHandler(this.BUT_detch2_Click);
            this.BUT_detch1.ImeMode = ImeMode.NoControl;
            this.BUT_detch1.Location = new Point(196, 76);
            this.BUT_detch1.Name = "BUT_detch1";
            this.BUT_detch1.Size = new Size(45, 23);
            this.BUT_detch1.TabIndex = 81;
            this.BUT_detch1.Text = "Auto Detect";
            this.BUT_detch1.UseVisualStyleBackColor = true;
            this.BUT_detch1.Click += new EventHandler(this.BUT_detch1_Click);
            this.BUT_enable.ImeMode = ImeMode.NoControl;
            this.BUT_enable.Location = new Point(405, 415);
            this.BUT_enable.Name = "BUT_enable";
            this.BUT_enable.Size = new Size(75, 23);
            this.BUT_enable.TabIndex = 28;
            this.BUT_enable.Text = "Default";
            this.BUT_enable.UseVisualStyleBackColor = true;
            this.BUT_enable.Click += new EventHandler(this.BUT_default_Click);
            this.BUT_save.ImeMode = ImeMode.NoControl;
            this.BUT_save.Location = new Point(397, 13);
            this.BUT_save.Name = "BUT_save";
            this.BUT_save.Size = new Size(75, 23);
            this.BUT_save.TabIndex = 27;
            this.BUT_save.Text = "Save";
            this.BUT_save.UseVisualStyleBackColor = true;
            this.BUT_save.Click += new EventHandler(this.BUT_save_Click);
            this.RevChSingleYaw.AutoSize = true;
            this.RevChSingleYaw.ImeMode = ImeMode.NoControl;
            this.RevChSingleYaw.Location = new Point(353, 428);
            this.RevChSingleYaw.Name = "RevChSingleYaw";
            this.RevChSingleYaw.Size = new Size(15, 14);
            this.RevChSingleYaw.TabIndex = (int)sbyte.MaxValue;
            this.RevChSingleYaw.UseVisualStyleBackColor = true;
            this.RevChSingleYaw.CheckedChanged += new EventHandler(this.revCH13_CheckedChanged);
            this.SingleYawProgressBar.DrawLabel = true;
            this.SingleYawProgressBar.ImeMode = ImeMode.NoControl;
            this.SingleYawProgressBar.Label = (string)null;
            this.SingleYawProgressBar.Location = new Point(247, 424);
            this.SingleYawProgressBar.Maximum = 2200;
            this.SingleYawProgressBar.maxline = 0;
            this.SingleYawProgressBar.Minimum = 800;
            this.SingleYawProgressBar.minline = 0;
            this.SingleYawProgressBar.Name = "SingleYawProgressBar";
            this.SingleYawProgressBar.Size = new Size(100, 23);
            this.SingleYawProgressBar.TabIndex = 126;
            this.SingleYawProgressBar.Value = 800;
            this.BUT_detch13.ImeMode = ImeMode.NoControl;
            this.BUT_detch13.Location = new Point(196, 424);
            this.BUT_detch13.Name = "BUT_detch13";
            this.BUT_detch13.Size = new Size(45, 23);
            this.BUT_detch13.TabIndex = 125;
            this.BUT_detch13.Text = "Auto Detect";
            this.BUT_detch13.UseVisualStyleBackColor = true;
            this.BUT_detch13.Click += new EventHandler(this.BUT_detch13_Click);
            this.CMB_CH13.FormattingEnabled = true;
            this.CMB_CH13.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH13.Location = new Point(120, 426);
            this.CMB_CH13.Name = "CMB_CH13";
            this.CMB_CH13.Size = new Size(70, 21);
            this.CMB_CH13.TabIndex = 124;
            this.CMB_CH13.SelectedIndexChanged += new EventHandler(this.CMB_CH13_SelectedIndexChanged);
            this.label16.AutoSize = true;
            this.label16.ImeMode = ImeMode.NoControl;
            this.label16.Location = new Point(12, 429);
            this.label16.Name = "label16";
            this.label16.Size = new Size(96, 13);
            this.label16.TabIndex = 123;
            this.label16.Text = "Single Yaw On/Off";
            this.RevChBIT.AutoSize = true;
            this.RevChBIT.ImeMode = ImeMode.NoControl;
            this.RevChBIT.Location = new Point(353, 457);
            this.RevChBIT.Name = "RevChBIT";
            this.RevChBIT.Size = new Size(15, 14);
            this.RevChBIT.TabIndex = 132;
            this.RevChBIT.UseVisualStyleBackColor = true;
            this.RevChBIT.CheckedChanged += new EventHandler(this.revCH14_CheckedChanged);
            this.BITProgressBar.DrawLabel = true;
            this.BITProgressBar.ImeMode = ImeMode.NoControl;
            this.BITProgressBar.Label = (string)null;
            this.BITProgressBar.Location = new Point(247, 453);
            this.BITProgressBar.Maximum = 2200;
            this.BITProgressBar.maxline = 0;
            this.BITProgressBar.Minimum = 800;
            this.BITProgressBar.minline = 0;
            this.BITProgressBar.Name = "BITProgressBar";
            this.BITProgressBar.Size = new Size(100, 23);
            this.BITProgressBar.TabIndex = 131;
            this.BITProgressBar.Value = 800;
            this.BUT_detch14.ImeMode = ImeMode.NoControl;
            this.BUT_detch14.Location = new Point(196, 453);
            this.BUT_detch14.Name = "BUT_detch14";
            this.BUT_detch14.Size = new Size(45, 23);
            this.BUT_detch14.TabIndex = 130;
            this.BUT_detch14.Text = "Auto Detect";
            this.BUT_detch14.UseVisualStyleBackColor = true;
            this.BUT_detch14.Click += new EventHandler(this.BUT_detch14_Click);
            this.CMB_CH14.FormattingEnabled = true;
            this.CMB_CH14.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH14.Location = new Point(120, 455);
            this.CMB_CH14.Name = "CMB_CH14";
            this.CMB_CH14.Size = new Size(70, 21);
            this.CMB_CH14.TabIndex = 129;
            this.CMB_CH14.SelectedIndexChanged += new EventHandler(this.CMB_CH14_SelectedIndexChanged);
            this.label18.AutoSize = true;
            this.label18.ImeMode = ImeMode.NoControl;
            this.label18.Location = new Point(12, 458);
            this.label18.Name = "label18";
            this.label18.Size = new Size(24, 13);
            this.label18.TabIndex = 128;
            this.label18.Text = "BIT";
            this.RevChRetracting.AutoSize = true;
            this.RevChRetracting.ImeMode = ImeMode.NoControl;
            this.RevChRetracting.Location = new Point(353, 486);
            this.RevChRetracting.Name = "RevChRetracting";
            this.RevChRetracting.Size = new Size(15, 14);
            this.RevChRetracting.TabIndex = 137;
            this.RevChRetracting.UseVisualStyleBackColor = true;
            this.RevChRetracting.CheckedChanged += new EventHandler(this.RevChRetracting_CheckedChanged);
            this.RetractingProgressBar.DrawLabel = true;
            this.RetractingProgressBar.ImeMode = ImeMode.NoControl;
            this.RetractingProgressBar.Label = (string)null;
            this.RetractingProgressBar.Location = new Point(247, 482);
            this.RetractingProgressBar.Maximum = 2200;
            this.RetractingProgressBar.maxline = 0;
            this.RetractingProgressBar.Minimum = 800;
            this.RetractingProgressBar.minline = 0;
            this.RetractingProgressBar.Name = "RetractingProgressBar";
            this.RetractingProgressBar.Size = new Size(100, 23);
            this.RetractingProgressBar.TabIndex = 136;
            this.RetractingProgressBar.Value = 800;
            this.BUT_detch15.ImeMode = ImeMode.NoControl;
            this.BUT_detch15.Location = new Point(196, 482);
            this.BUT_detch15.Name = "BUT_detch15";
            this.BUT_detch15.Size = new Size(45, 23);
            this.BUT_detch15.TabIndex = 135;
            this.BUT_detch15.Text = "Auto Detect";
            this.BUT_detch15.UseVisualStyleBackColor = true;
            this.BUT_detch15.Click += new EventHandler(this.BUT_detch15_Click);
            this.CMB_CH15.FormattingEnabled = true;
            this.CMB_CH15.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH15.Location = new Point(120, 484);
            this.CMB_CH15.Name = "CMB_CH15";
            this.CMB_CH15.Size = new Size(70, 21);
            this.CMB_CH15.TabIndex = 134;
            this.CMB_CH15.SelectedIndexChanged += new EventHandler(this.CMB_CH15_SelectedIndexChanged);
            this.label17.AutoSize = true;
            this.label17.ImeMode = ImeMode.NoControl;
            this.label17.Location = new Point(12, 487);
            this.label17.Name = "label17";
            this.label17.Size = new Size(56, 13);
            this.label17.TabIndex = 133;
            this.label17.Text = "Retracting";
            this.RevChFollowTarget.AutoSize = true;
            this.RevChFollowTarget.ImeMode = ImeMode.NoControl;
            this.RevChFollowTarget.Location = new Point(353, 515);
            this.RevChFollowTarget.Name = "RevChFollowTarget";
            this.RevChFollowTarget.Size = new Size(15, 14);
            this.RevChFollowTarget.TabIndex = 142;
            this.RevChFollowTarget.UseVisualStyleBackColor = true;
            this.RevChFollowTarget.CheckedChanged += new EventHandler(this.RevChFollowTarget_CheckedChanged);
            this.FollowTargetProgressBar.DrawLabel = true;
            this.FollowTargetProgressBar.ImeMode = ImeMode.NoControl;
            this.FollowTargetProgressBar.Label = (string)null;
            this.FollowTargetProgressBar.Location = new Point(247, 511);
            this.FollowTargetProgressBar.Maximum = 2200;
            this.FollowTargetProgressBar.maxline = 0;
            this.FollowTargetProgressBar.Minimum = 800;
            this.FollowTargetProgressBar.minline = 0;
            this.FollowTargetProgressBar.Name = "FollowTargetProgressBar";
            this.FollowTargetProgressBar.Size = new Size(100, 23);
            this.FollowTargetProgressBar.TabIndex = 141;
            this.FollowTargetProgressBar.Value = 800;
            this.BUT_detch16.ImeMode = ImeMode.NoControl;
            this.BUT_detch16.Location = new Point(196, 511);
            this.BUT_detch16.Name = "BUT_detch16";
            this.BUT_detch16.Size = new Size(45, 23);
            this.BUT_detch16.TabIndex = 140;
            this.BUT_detch16.Text = "Auto Detect";
            this.BUT_detch16.UseVisualStyleBackColor = true;
            this.BUT_detch16.Click += new EventHandler(this.BUT_detch16_Click);
            this.CMB_CH16.FormattingEnabled = true;
            this.CMB_CH16.Items.AddRange(new object[11]
            {
        (object) "None",
        (object) "btn1",
        (object) "btn2",
        (object) "btn3",
        (object) "btn4",
        (object) "btn5",
        (object) "btn6",
        (object) "btn7",
        (object) "btn8",
        (object) "btn9",
        (object) "btn10"
            });
            this.CMB_CH16.Location = new Point(120, 513);
            this.CMB_CH16.Name = "CMB_CH16";
            this.CMB_CH16.Size = new Size(70, 21);
            this.CMB_CH16.TabIndex = 139;
            this.CMB_CH16.SelectedIndexChanged += new EventHandler(this.CMB_CH16_SelectedIndexChanged);
            this.label19.AutoSize = true;
            this.label19.ImeMode = ImeMode.NoControl;
            this.label19.Location = new Point(12, 516);
            this.label19.Name = "label19";
            this.label19.Size = new Size(71, 13);
            this.label19.TabIndex = 138;
            this.label19.Text = "Follow Target";
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(506, 548);
            this.Controls.Add((Control)this.RevChFollowTarget);
            this.Controls.Add((Control)this.FollowTargetProgressBar);
            this.Controls.Add((Control)this.BUT_detch16);
            this.Controls.Add((Control)this.CMB_CH16);
            this.Controls.Add((Control)this.label19);
            this.Controls.Add((Control)this.RevChRetracting);
            this.Controls.Add((Control)this.RetractingProgressBar);
            this.Controls.Add((Control)this.BUT_detch15);
            this.Controls.Add((Control)this.CMB_CH15);
            this.Controls.Add((Control)this.label17);
            this.Controls.Add((Control)this.RevChBIT);
            this.Controls.Add((Control)this.BITProgressBar);
            this.Controls.Add((Control)this.BUT_detch14);
            this.Controls.Add((Control)this.CMB_CH14);
            this.Controls.Add((Control)this.label18);
            this.Controls.Add((Control)this.RevChSingleYaw);
            this.Controls.Add((Control)this.SingleYawProgressBar);
            this.Controls.Add((Control)this.BUT_detch13);
            this.Controls.Add((Control)this.CMB_CH13);
            this.Controls.Add((Control)this.label16);
            this.Controls.Add((Control)this.myButton1);
            this.Controls.Add((Control)this.RevChRoll);
            this.Controls.Add((Control)this.RevChLaser);
            this.Controls.Add((Control)this.RevChTracking);
            this.Controls.Add((Control)this.RevChReTracking);
            this.Controls.Add((Control)this.revCHpicCapture);
            this.Controls.Add((Control)this.revCHrec);
            this.Controls.Add((Control)this.revCHdayIr);
            this.Controls.Add((Control)this.revCH_BWhot);
            this.Controls.Add((Control)this.revCHnuc);
            this.Controls.Add((Control)this.revCHzoom_out);
            this.Controls.Add((Control)this.revCHzoom_in);
            this.Controls.Add((Control)this.revCHPitch);
            this.Controls.Add((Control)this.label10);
            this.Controls.Add((Control)this.NUCProgressBar);
            this.Controls.Add((Control)this.ZoomOutProgressBar);
            this.Controls.Add((Control)this.ZoomInProgressBar);
            this.Controls.Add((Control)this.progressBarPitch);
            this.Controls.Add((Control)this.ReTrackingProgressBar);
            this.Controls.Add((Control)this.TrackingProgressBar);
            this.Controls.Add((Control)this.LaserProgressBar);
            this.Controls.Add((Control)this.PicCaptureProgressBar);
            this.Controls.Add((Control)this.RecProgressBar);
            this.Controls.Add((Control)this.DayIRprogressBar);
            this.Controls.Add((Control)this.BWhotprogressBar);
            this.Controls.Add((Control)this.progressBarRoll);
            this.Controls.Add((Control)this.BUT_detch7);
            this.Controls.Add((Control)this.BUT_detch11);
            this.Controls.Add((Control)this.BUT_detch6);
            this.Controls.Add((Control)this.BUT_detch10);
            this.Controls.Add((Control)this.BUT_detch5);
            this.Controls.Add((Control)this.BUT_detch9);
            this.Controls.Add((Control)this.BUT_detch12);
            this.Controls.Add((Control)this.BUT_detch4);
            this.Controls.Add((Control)this.BUT_detch8);
            this.Controls.Add((Control)this.BUT_detch3);
            this.Controls.Add((Control)this.BUT_detch2);
            this.Controls.Add((Control)this.BUT_detch1);
            this.Controls.Add((Control)this.CMB_CH12);
            this.Controls.Add((Control)this.CMB_CH11);
            this.Controls.Add((Control)this.CMB_CH10);
            this.Controls.Add((Control)this.CMB_CH9);
            this.Controls.Add((Control)this.CMB_CH8);
            this.Controls.Add((Control)this.CMB_CH7);
            this.Controls.Add((Control)this.CMB_CH6);
            this.Controls.Add((Control)this.CMB_CH5);
            this.Controls.Add((Control)this.CMB_CH4);
            this.Controls.Add((Control)this.CMB_CH3);
            this.Controls.Add((Control)this.CMB_CH2);
            this.Controls.Add((Control)this.CMB_CH1);
            this.Controls.Add((Control)this.label9);
            this.Controls.Add((Control)this.label11);
            this.Controls.Add((Control)this.label12);
            this.Controls.Add((Control)this.label13);
            this.Controls.Add((Control)this.label14);
            this.Controls.Add((Control)this.label15);
            this.Controls.Add((Control)this.label8);
            this.Controls.Add((Control)this.label7);
            this.Controls.Add((Control)this.label6);
            this.Controls.Add((Control)this.label4);
            this.Controls.Add((Control)this.label3);
            this.Controls.Add((Control)this.label2);
            this.Controls.Add((Control)this.label1);
            this.Controls.Add((Control)this.BUT_enable);
            this.Controls.Add((Control)this.BUT_save);
            this.Controls.Add((Control)this.label5);
            this.Controls.Add((Control)this.CMB_joysticks);
            this.Name = nameof(CamJoystickSetup);
            this.Text = "Camera Joystick";
            this.FormClosed += new FormClosedEventHandler(this.CamJoystickSetup_FormClosed);
            this.Load += new EventHandler(this.Joystick_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
