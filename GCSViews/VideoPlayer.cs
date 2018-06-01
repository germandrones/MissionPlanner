using MissionPlanner.Controls;
using NextVisionVideoControlLibrary;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews
{
    public class VideoPlayer : MyUserControl, IActivate, IDeactivate
    {
        public bool m_bFormStatus = false;
        private VideoControl m_oVideoControl = new VideoControl();
        private bool toHide = true;
        private IContainer components = (IContainer)null;
        private Timer klv_timer;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private GroupBox groupBox2;
        private Label RecordingFileSizeLabel;
        private Label RecordStatusLabel;
        private Button VideoGrabBtn;
        private GroupBox groupBoxVidInfo;
        private Label VidStdLabel;
        private Label InputInterfaceLabel;
        private Label StreamStatusLabel;
        private Button btnOpenNetVideo;
        private DataGridView dataGridViewStandartTags;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridView dataGridViewCustomTags;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private Button btnHidePanel;
        private Label lblVidPlayerVer;
        private GroupBox groupBox1;

        public VideoPlayer()
        {
            this.InitializeComponent();
            int major = 0;
            int minor = 0;
            int build = 0;
            this.m_oVideoControl.VideoControlGetVersion(ref major, ref minor, ref build);
            Label lblVidPlayerVer = this.lblVidPlayerVer;
            lblVidPlayerVer.Text = lblVidPlayerVer.Text + (object)major + "." + (object)minor + "." + (object)build;
            this.m_oVideoControl.Dock = DockStyle.Fill;
            this.splitContainer2.Panel2.Controls.Add((Control)this.m_oVideoControl);
            this.klv_timer.Start();
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Unix Time Stamp",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Platform Heading Angle",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Platform Pitch Angle",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Platform Roll Angle",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Image Sensor",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Image Coordinate System",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Horizontal Fov",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Vertical Fov",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Relative Azimuth",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Relative Elevation",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Relative Roll",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Slant Range",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Frame Center Latitude",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Frame Center Longitude",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Frame Center Elevation",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Offset Corner Lat Point 1",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Offset Corner Lon Point 1",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Offset Corner Lat Point 2",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Offset Corner Lon Point 2",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Offset Corner Lat Point 3",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Offset Corner Lon Point 3",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Offset Corner Lat Point 4",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "Offset Corner Lon Point 4",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "North Velocity",
        "0 .0"
            });
            this.dataGridViewStandartTags.Rows.Add((object[])new string[2]
            {
        "East Velocity",
        "0 .0"
            });
            this.dataGridViewCustomTags.Rows.Add((object[])new string[2]
            {
        "Video Channel",
        "0 .0"
            });
            this.dataGridViewCustomTags.Rows.Add((object[])new string[2]
            {
        "Laser State",
        "0 .0"
            });
            this.dataGridViewCustomTags.Rows.Add((object[])new string[2]
            {
        "PIP State",
        "0 .0"
            });
            this.dataGridViewCustomTags.Rows.Add((object[])new string[2]
            {
        "Camera Mode",
        "0 .0"
            });
            this.dataGridViewCustomTags.Rows.Add((object[])new string[2]
            {
        "Recorder State",
        "0 .0"
            });
            this.dataGridViewCustomTags.Rows.Add((object[])new string[2]
            {
        "Tracker State",
        "0 .0"
            });
            this.dataGridViewCustomTags.Rows.Add((object[])new string[2]
            {
        "Gimbal Mount Position",
        "0 .0"
            });
            this.dataGridViewCustomTags.Rows.Add((object[])new string[2]
            {
        "Line Of Sight Elevation",
        "0 .0"
            });
        }

        public void StartVideoPlayer()
        {
            this.m_bFormStatus = true;
        }

        public bool GetStatus()
        {
            return this.m_bFormStatus;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "ts";
            openFileDialog.AddExtension = true;
            openFileDialog.ValidateNames = true;
            openFileDialog.Filter = "ts files (*.ts)|*.ts";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            this.m_oVideoControl.VideoControlStartPlayback(openFileDialog.FileName, new H264Decoder.RawFrameReadyCB(this.raw_frame_callback));
        }

        private void raw_frame_callback(byte[] frame_buf, stream_status status, int width, int height)
        {
            if (status != stream_status.StreamLost)
                return;
            string s = "IP Link Loss";
            PointF point = new PointF(65f, (float)(height - 46));
            using (Graphics graphics = Graphics.FromImage((Image)new Bitmap(width, height, 3 * width, PixelFormat.Format24bppRgb, Marshal.UnsafeAddrOfPinnedArrayElement<byte>(frame_buf, 0))))
            {
                using (Font font = new Font("Arial", 25f))
                    graphics.DrawString(s, font, Brushes.Red, point);
            }
        }

        private void Klv_timer_Tick(object sender, EventArgs e)
        {
            string[] strArray1 = new string[4]
            {
        "Idle",
        "Acquiring",
        "Detection Ok",
        "Lost"
            };
            string[] strArray2 = new string[3]
            {
        "Idle",
        "Recording",
        "Failure"
            };
            stream_status streamStatus = this.m_oVideoControl.VideoControlGetStreamStatus();
            recording_status recordStatus = this.m_oVideoControl.VideoControlGetRecordStatus();
            this.StreamStatusLabel.Text = "Stream Status -".PadRight(10) + strArray1[(int)streamStatus];
            this.RecordStatusLabel.Text = "Record Status -".PadRight(10) + strArray2[(int)recordStatus];
            this.RecordingFileSizeLabel.Text = "File Size[KB] -".PadRight(10) + (object)(this.m_oVideoControl.VideoControlGetRecordFileSize() / 1024);
            if (streamStatus == stream_status.StreamDetectionOk)
            {
                switch (this.m_oVideoControl.VideoControlGetStreamWidth())
                {
                    case 640:
                        this.InputInterfaceLabel.Text = "Input Interface -".PadRight(10) + "Digital - HDMI";
                        this.VidStdLabel.Text = "Video Standard -".PadRight(10) + "VGA";
                        break;
                    case 720:
                        if (this.m_oVideoControl.VideoControlGetStreamHeight() == 480)
                        {
                            this.InputInterfaceLabel.Text = "Input Interface -".PadRight(10) + "Analog - CVBS";
                            this.VidStdLabel.Text = "Video Standard -".PadRight(10) + "NTSC";
                            break;
                        }
                        this.InputInterfaceLabel.Text = "Input Interface -".PadRight(10) + "Analog - CVBS";
                        this.VidStdLabel.Text = "Video Standard -".PadRight(10) + "PAL";
                        break;
                    case 1280:
                        this.InputInterfaceLabel.Text = "Input Interface -".PadRight(10) + "Digital- HDMI";
                        this.VidStdLabel.Text = "Video Standard -".PadRight(10) + "720p";
                        break;
                }
            }
            klv_tag tag;
            if (this.m_oVideoControl.VideoControlGetKlvTag(2, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[0].Cells[1].Value = (object)new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((double)(BitConverter.ToUInt64(tag.data, 0) / 1000UL));
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(5, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[1].Cells[1].Value = (object)((double)BitConverter.ToUInt16(tag.data, 0) * 360.0 / (double)ushort.MaxValue).ToString("0.000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(6, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                short int16 = BitConverter.ToInt16(tag.data, 0);
                this.dataGridViewStandartTags.Rows[2].Cells[1].Value = ((int)int16 & (int)ushort.MaxValue) != 32768 ? (object)((double)int16 * 40.0 / (double)ushort.MaxValue).ToString("0.000") : (object)"Out of Range";
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(7, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                short int16 = BitConverter.ToInt16(tag.data, 0);
                this.dataGridViewStandartTags.Rows[3].Cells[1].Value = ((int)int16 & (int)ushort.MaxValue) != 32768 ? (object)((double)int16 * 100.0 / (double)ushort.MaxValue).ToString("0.000") : (object)"Out of Range";
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(11, out tag))
                this.dataGridViewStandartTags.Rows[4].Cells[1].Value = (object)Encoding.Default.GetString(tag.data).TrimEnd(new char[1]);
            if (this.m_oVideoControl.VideoControlGetKlvTag(12, out tag))
                this.dataGridViewStandartTags.Rows[5].Cells[1].Value = (object)Encoding.Default.GetString(tag.data).TrimEnd(new char[1]);
            if (this.m_oVideoControl.VideoControlGetKlvTag(16, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[6].Cells[1].Value = (object)((double)BitConverter.ToUInt16(tag.data, 0) * 180.0 / (double)ushort.MaxValue).ToString("0.000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(17, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[7].Cells[1].Value = (object)((double)BitConverter.ToUInt16(tag.data, 0) * 180.0 / (double)ushort.MaxValue).ToString("0.000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(18, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[8].Cells[1].Value = (object)((double)BitConverter.ToUInt32(tag.data, 0) * 360.0 / (double)uint.MaxValue).ToString("0.000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(19, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                int num = BitConverter.ToInt32(tag.data, 0);
                if (((ulong)num & 2147483648UL) > 0UL)
                    num = (~num + 1) * -1;
                this.dataGridViewStandartTags.Rows[9].Cells[1].Value = (object)((double)num * 360.0 / (double)uint.MaxValue).ToString("0.000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(20, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[10].Cells[1].Value = (object)((double)BitConverter.ToUInt32(tag.data, 0) * 360.0 / (double)uint.MaxValue).ToString("0.000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(21, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[11].Cells[1].Value = (object)((double)BitConverter.ToUInt32(tag.data, 0) * 5000000.0 / (double)uint.MaxValue).ToString("0.000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(23, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[12].Cells[1].Value = (object)((double)BitConverter.ToInt32(tag.data, 0) * 180.0 / (double)uint.MaxValue).ToString("0.000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(24, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[13].Cells[1].Value = (object)((double)BitConverter.ToInt32(tag.data, 0) * 360.0 / (double)uint.MaxValue).ToString("0.000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(25, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[14].Cells[1].Value = (object)((double)((int)BitConverter.ToInt16(tag.data, 0) * 19900) / (double)ushort.MaxValue - 900.0).ToString("0.000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(26, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[15].Cells[1].Value = (object)((double)BitConverter.ToInt16(tag.data, 0) * 0.075 / (double)short.MaxValue).ToString("0.000000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(27, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[16].Cells[1].Value = (object)((double)BitConverter.ToInt16(tag.data, 0) * 0.075 / (double)short.MaxValue).ToString("0.000000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(28, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[17].Cells[1].Value = (object)((double)BitConverter.ToInt16(tag.data, 0) * 0.075 / (double)short.MaxValue).ToString("0.000000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(29, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[18].Cells[1].Value = (object)((double)BitConverter.ToInt16(tag.data, 0) * 0.075 / (double)short.MaxValue).ToString("0.000000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(30, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[19].Cells[1].Value = (object)((double)BitConverter.ToInt16(tag.data, 0) * 0.075 / (double)short.MaxValue).ToString("0.000000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(31, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[20].Cells[1].Value = (object)((double)BitConverter.ToInt16(tag.data, 0) * 0.075 / (double)short.MaxValue).ToString("0.000000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(32, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[21].Cells[1].Value = (object)((double)BitConverter.ToInt16(tag.data, 0) * 0.075 / (double)short.MaxValue).ToString("0.000000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(33, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[22].Cells[1].Value = (object)((double)BitConverter.ToInt16(tag.data, 0) * 0.075 / (double)short.MaxValue).ToString("0.000000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(101, out tag))
                this.dataGridViewCustomTags.Rows[0].Cells[1].Value = tag.data[0] > (byte)0 ? (object)"Thermal" : (object)"Daylight";
            if (this.m_oVideoControl.VideoControlGetKlvTag(102, out tag))
                this.dataGridViewCustomTags.Rows[1].Cells[1].Value = tag.data[0] > (byte)0 ? (object)"On" : (object)"Off";
            if (this.m_oVideoControl.VideoControlGetKlvTag(103, out tag))
                this.dataGridViewCustomTags.Rows[2].Cells[1].Value = tag.data[0] > (byte)0 ? (object)"Enabled" : (object)"Disabled";
            if (this.m_oVideoControl.VideoControlGetKlvTag(104, out tag))
            {
                string[] strArray3 = new string[14]
                {
          "Rate-0",
          "Point To Coordinate",
          "Hold Coordinate",
          "Pilot",
          "Stow",
          "N/A",
          "Rate-6",
          "DDC",
          "Park",
          "N/A",
          "Gyro Calibration",
          "GRR",
          "Int-1",
          "Int-2"
                };
                this.dataGridViewCustomTags.Rows[3].Cells[1].Value = tag.data[0] <= (byte)13 ? (object)strArray3[(int)tag.data[0]] : (tag.data[0] != (byte)31 ? (object)"N/A" : (object)"EXT");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(105, out tag))
                this.dataGridViewCustomTags.Rows[4].Cells[1].Value = tag.data[0] > (byte)0 ? (object)"Enabled" : (object)"Disabled";
            if (this.m_oVideoControl.VideoControlGetKlvTag(106, out tag))
            {
                string[] strArray3 = new string[7]
                {
          "Idle",
          "Enabled",
          "Tracking",
          "Retrack",
          "N/A",
          "Position Track #1",
          "Position Track #2"
                };
                this.dataGridViewCustomTags.Rows[5].Cells[1].Value = tag.data[0] >= (byte)7 ? (object)"N/A" : (object)strArray3[(int)tag.data[0]];
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(107, out tag))
                this.dataGridViewCustomTags.Rows[6].Cells[1].Value = tag.data[0] > (byte)0 ? (object)"Nose" : (object)"Belly";
            if (this.m_oVideoControl.VideoControlGetKlvTag(79, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[23].Cells[1].Value = (object)((double)((int)BitConverter.ToInt16(tag.data, 0) * 327) / (double)short.MaxValue).ToString("0.000000");
            }
            if (this.m_oVideoControl.VideoControlGetKlvTag(80, out tag))
            {
                Array.Reverse((Array)tag.data, 0, tag.len);
                this.dataGridViewStandartTags.Rows[24].Cells[1].Value = (object)((double)((int)BitConverter.ToInt16(tag.data, 0) * 327) / (double)short.MaxValue).ToString("0.000000");
            }
            if (!this.m_oVideoControl.VideoControlGetKlvTag(108, out tag))
                return;
            this.dataGridViewCustomTags.Rows[7].Cells[1].Value = (object)BitConverter.ToSingle(tag.data, 0).ToString("0.000000");
        }

        private void VideoGrabBtn_Click(object sender, EventArgs e)
        {
            if (this.m_oVideoControl.VideoControlGetRecordStatus() == recording_status.RecordingIdle && this.m_oVideoControl.VideoControlGetStreamStatus() == stream_status.StreamDetectionOk)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "ts";
                saveFileDialog.AddExtension = true;
                saveFileDialog.ValidateNames = true;
                saveFileDialog.Filter = "ts files (*.ts)|*.ts";
                if (saveFileDialog.ShowDialog() != DialogResult.OK || this.m_oVideoControl.VideoControlStartRec(saveFileDialog.FileName))
                    return;
                this.VideoGrabBtn.Text = "Stop Grab";
            }
            else
            {
                if (this.m_oVideoControl.VideoControlGetRecordStatus() != recording_status.RecordingEnabled || this.m_oVideoControl.VideoControlStopRec())
                    return;
                this.VideoGrabBtn.Text = "Start Grab";
            }
        }

        private void DrawOSDString(Bitmap bitmap, string str, PointF position, VideoPlayer.OSDcolor color)
        {
            using (Graphics graphics1 = Graphics.FromImage((Image)bitmap))
            {
                using (Font font = new Font("Arial", 10f, FontStyle.Bold))
                {
                    SizeF sizeF = graphics1.MeasureString(str, font);
                    RectangleF rectangleF = new RectangleF(position.X, position.Y, sizeF.Width, sizeF.Height);
                    Graphics graphics2 = graphics1;
                    int alpha = 80;
                    Color black = Color.Black;
                    int r = (int)black.R;
                    black = Color.Black;
                    int g = (int)black.G;
                    int b = (int)Color.Black.B;
                    SolidBrush solidBrush = new SolidBrush(Color.FromArgb(alpha, r, g, b));
                    RectangleF rect = rectangleF;
                    graphics2.FillRectangle((Brush)solidBrush, rect);
                    graphics1.DrawString(str, font, (Brush)this.convertOSDCodeToBrush(color), position.X, position.Y);
                }
            }
        }

        private SolidBrush convertOSDCodeToBrush(VideoPlayer.OSDcolor color)
        {
            SolidBrush solidBrush = new SolidBrush(Color.White);
            if (color == VideoPlayer.OSDcolor.Yellow)
                solidBrush = new SolidBrush(Color.Yellow);
            if (color == VideoPlayer.OSDcolor.White)
                solidBrush = new SolidBrush(Color.White);
            if (color == VideoPlayer.OSDcolor.Red)
                solidBrush = new SolidBrush(Color.Red);
            return solidBrush;
        }

        public void Activate()
        {
            this.m_oVideoControl.activate();
        }

        public void Deactivate()
        {
            this.m_oVideoControl.deactivate();
        }

        private void btnHidePanel_Click(object sender, EventArgs e)
        {
            if (this.toHide)
            {
                this.splitContainer2.Panel1Collapsed = true;
                this.splitContainer2.Panel1.Hide();
                this.toHide = false;
                this.btnHidePanel.Text = "Show Panel";
            }
            else
            {
                this.splitContainer2.Panel1Collapsed = false;
                this.splitContainer2.Panel1.Show();
                this.toHide = true;
                this.btnHidePanel.Text = "Hide Panel";
            }
        }

        private void btnOpenNetVideo_Click(object sender, EventArgs e)
        {
            NetworkSettings networkSettingscs = new NetworkSettings();
            int num = (int)networkSettingscs.ShowDialog();
            if (networkSettingscs.network_url == null)
                return;
            this.m_oVideoControl.VideoControlStartStream(networkSettingscs.url_ip.ToString(), networkSettingscs.url_port, new H264Decoder.RawFrameReadyCB(this.raw_frame_callback), new VideoControl.VideoControlClickDelegate(VideoPlayer.click_callback));
        }

        public static void click_callback(int x, int y)
        {
            if (x == -1)
                return;
            Point point = new Point(x, y);
            MainV2.Colibri.EditingControlTrackOnPos = point;
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
            DataGridViewCellStyle gridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle gridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle gridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle gridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle gridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle gridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle gridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle gridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle gridViewCellStyle9 = new DataGridViewCellStyle();
            DataGridViewCellStyle gridViewCellStyle10 = new DataGridViewCellStyle();
            this.klv_timer = new Timer(this.components);
            this.splitContainer1 = new SplitContainer();
            this.splitContainer2 = new SplitContainer();
            this.groupBox1 = new GroupBox();
            this.lblVidPlayerVer = new Label();
            this.dataGridViewStandartTags = new DataGridView();
            this.dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            this.dataGridViewCustomTags = new DataGridView();
            this.dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            this.groupBox2 = new GroupBox();
            this.RecordingFileSizeLabel = new Label();
            this.RecordStatusLabel = new Label();
            this.VideoGrabBtn = new Button();
            this.groupBoxVidInfo = new GroupBox();
            this.VidStdLabel = new Label();
            this.InputInterfaceLabel = new Label();
            this.StreamStatusLabel = new Label();
            this.btnOpenNetVideo = new Button();
            this.btnHidePanel = new Button();
            this.splitContainer1.BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((ISupportInitialize)this.dataGridViewStandartTags).BeginInit();
            ((ISupportInitialize)this.dataGridViewCustomTags).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBoxVidInfo.SuspendLayout();
            this.SuspendLayout();
            this.klv_timer.Enabled = true;
            this.klv_timer.Tick += new EventHandler(this.Klv_timer_Tick);
            this.splitContainer1.Dock = DockStyle.Fill;
            this.splitContainer1.Location = new Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1Collapsed = true;
            this.splitContainer1.Panel2.Controls.Add((Control)this.splitContainer2);
            this.splitContainer1.Size = new Size(1346, 730);
            this.splitContainer1.SplitterDistance = 177;
            this.splitContainer1.TabIndex = 1;
            this.splitContainer2.Dock = DockStyle.Fill;
            this.splitContainer2.Location = new Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = Orientation.Horizontal;
            this.splitContainer2.Panel1.Controls.Add((Control)this.groupBox1);
            this.splitContainer2.Panel1.Controls.Add((Control)this.dataGridViewStandartTags);
            this.splitContainer2.Panel1.Controls.Add((Control)this.dataGridViewCustomTags);
            this.splitContainer2.Panel1.Controls.Add((Control)this.groupBox2);
            this.splitContainer2.Panel1.Controls.Add((Control)this.groupBoxVidInfo);
            this.splitContainer2.Panel1.Controls.Add((Control)this.btnOpenNetVideo);
            this.splitContainer2.Panel2.Controls.Add((Control)this.btnHidePanel);
            this.splitContainer2.Size = new Size(1346, 730);
            this.splitContainer2.SplitterDistance = 199;
            this.splitContainer2.TabIndex = 0;
            this.groupBox1.Controls.Add((Control)this.lblVidPlayerVer);
            this.groupBox1.Location = new Point(625, 101);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(148, 39);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Video Player DLL Version";
            this.lblVidPlayerVer.AutoSize = true;
            this.lblVidPlayerVer.Font = new Font("Consolas", 9f);
            this.lblVidPlayerVer.Location = new Point(6, 16);
            this.lblVidPlayerVer.Name = "lblVidPlayerVer";
            this.lblVidPlayerVer.Size = new Size(0, 14);
            this.lblVidPlayerVer.TabIndex = 36;
            this.dataGridViewStandartTags.AllowUserToAddRows = false;
            gridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewStandartTags.AlternatingRowsDefaultCellStyle = gridViewCellStyle1;
            this.dataGridViewStandartTags.BackgroundColor = SystemColors.Control;
            gridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridViewCellStyle2.BackColor = SystemColors.Control;
            gridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)177);
            gridViewCellStyle2.ForeColor = SystemColors.WindowText;
            gridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            gridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            gridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            this.dataGridViewStandartTags.ColumnHeadersDefaultCellStyle = gridViewCellStyle2;
            this.dataGridViewStandartTags.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewStandartTags.Columns.AddRange((DataGridViewColumn)this.dataGridViewTextBoxColumn6, (DataGridViewColumn)this.dataGridViewTextBoxColumn7);
            this.dataGridViewStandartTags.Location = new Point(153, 0);
            this.dataGridViewStandartTags.Name = "dataGridViewStandartTags";
            gridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridViewCellStyle3.BackColor = SystemColors.Control;
            gridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)177);
            gridViewCellStyle3.ForeColor = SystemColors.WindowText;
            gridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            gridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            gridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            this.dataGridViewStandartTags.RowHeadersDefaultCellStyle = gridViewCellStyle3;
            this.dataGridViewStandartTags.RowHeadersVisible = false;
            this.dataGridViewStandartTags.RowHeadersWidth = 20;
            gridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewStandartTags.RowsDefaultCellStyle = gridViewCellStyle4;
            this.dataGridViewStandartTags.Size = new Size(225, 174);
            this.dataGridViewStandartTags.TabIndex = 35;
            this.dataGridViewTextBoxColumn6.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = gridViewCellStyle5;
            this.dataGridViewTextBoxColumn6.FillWeight = 121.8274f;
            this.dataGridViewTextBoxColumn6.HeaderText = "Standard Tads";
            this.dataGridViewTextBoxColumn6.MinimumWidth = 50;
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn7.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridViewCellStyle6.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            this.dataGridViewTextBoxColumn7.DefaultCellStyle = gridViewCellStyle6;
            this.dataGridViewTextBoxColumn7.FillWeight = 78.17258f;
            this.dataGridViewTextBoxColumn7.HeaderText = "Value";
            this.dataGridViewTextBoxColumn7.MinimumWidth = 60;
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewCustomTags.AllowUserToAddRows = false;
            this.dataGridViewCustomTags.BackgroundColor = SystemColors.Control;
            gridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridViewCellStyle7.BackColor = SystemColors.Control;
            gridViewCellStyle7.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)177);
            gridViewCellStyle7.ForeColor = SystemColors.WindowText;
            gridViewCellStyle7.SelectionBackColor = SystemColors.Highlight;
            gridViewCellStyle7.SelectionForeColor = SystemColors.HighlightText;
            gridViewCellStyle7.WrapMode = DataGridViewTriState.True;
            this.dataGridViewCustomTags.ColumnHeadersDefaultCellStyle = gridViewCellStyle7;
            this.dataGridViewCustomTags.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCustomTags.Columns.AddRange((DataGridViewColumn)this.dataGridViewTextBoxColumn1, (DataGridViewColumn)this.dataGridViewTextBoxColumn2);
            this.dataGridViewCustomTags.Location = new Point(393, 0);
            this.dataGridViewCustomTags.Name = "dataGridViewCustomTags";
            gridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridViewCellStyle8.BackColor = SystemColors.Control;
            gridViewCellStyle8.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)177);
            gridViewCellStyle8.ForeColor = SystemColors.WindowText;
            gridViewCellStyle8.SelectionBackColor = SystemColors.Highlight;
            gridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText;
            gridViewCellStyle8.WrapMode = DataGridViewTriState.True;
            this.dataGridViewCustomTags.RowHeadersDefaultCellStyle = gridViewCellStyle8;
            this.dataGridViewCustomTags.RowHeadersVisible = false;
            this.dataGridViewCustomTags.RowHeadersWidth = 20;
            this.dataGridViewCustomTags.Size = new Size(225, 172);
            this.dataGridViewCustomTags.TabIndex = 34;
            this.dataGridViewTextBoxColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = gridViewCellStyle9;
            this.dataGridViewTextBoxColumn1.FillWeight = 121.8274f;
            this.dataGridViewTextBoxColumn1.HeaderText = "Non Standard Tads";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 50;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gridViewCellStyle10.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridViewCellStyle10.Font = new Font("Microsoft Sans Serif", 8.25f);
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = gridViewCellStyle10;
            this.dataGridViewTextBoxColumn2.FillWeight = 78.17258f;
            this.dataGridViewTextBoxColumn2.HeaderText = "Value";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 60;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.groupBox2.Controls.Add((Control)this.RecordingFileSizeLabel);
            this.groupBox2.Controls.Add((Control)this.RecordStatusLabel);
            this.groupBox2.Controls.Add((Control)this.VideoGrabBtn);
            this.groupBox2.Location = new Point(854, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(224, 86);
            this.groupBox2.TabIndex = 32;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Video Grab";
            this.RecordingFileSizeLabel.AutoSize = true;
            this.RecordingFileSizeLabel.Font = new Font("Consolas", 9f);
            this.RecordingFileSizeLabel.Location = new Point(9, 34);
            this.RecordingFileSizeLabel.Name = "RecordingFileSizeLabel";
            this.RecordingFileSizeLabel.Size = new Size(112, 14);
            this.RecordingFileSizeLabel.TabIndex = 26;
            this.RecordingFileSizeLabel.Text = "File Size[KB] -";
            this.RecordStatusLabel.AutoSize = true;
            this.RecordStatusLabel.Font = new Font("Consolas", 9f);
            this.RecordStatusLabel.Location = new Point(9, 16);
            this.RecordStatusLabel.Name = "RecordStatusLabel";
            this.RecordStatusLabel.Size = new Size(119, 14);
            this.RecordStatusLabel.TabIndex = 22;
            this.RecordStatusLabel.Text = "Record Status - ";
            this.VideoGrabBtn.Location = new Point(128, 55);
            this.VideoGrabBtn.Name = "VideoGrabBtn";
            this.VideoGrabBtn.Size = new Size(80, 24);
            this.VideoGrabBtn.TabIndex = 25;
            this.VideoGrabBtn.Text = "Start Grab";
            this.VideoGrabBtn.UseVisualStyleBackColor = true;
            this.VideoGrabBtn.Click += new EventHandler(this.VideoGrabBtn_Click);
            this.groupBoxVidInfo.Controls.Add((Control)this.VidStdLabel);
            this.groupBoxVidInfo.Controls.Add((Control)this.InputInterfaceLabel);
            this.groupBoxVidInfo.Controls.Add((Control)this.StreamStatusLabel);
            this.groupBoxVidInfo.Location = new Point(624, 6);
            this.groupBoxVidInfo.Name = "groupBoxVidInfo";
            this.groupBoxVidInfo.Size = new Size(224, 93);
            this.groupBoxVidInfo.TabIndex = 31;
            this.groupBoxVidInfo.TabStop = false;
            this.groupBoxVidInfo.Text = "Stream Info";
            this.VidStdLabel.AutoSize = true;
            this.VidStdLabel.Font = new Font("Consolas", 9f);
            this.VidStdLabel.Location = new Point(7, 62);
            this.VidStdLabel.Name = "VidStdLabel";
            this.VidStdLabel.Size = new Size(126, 14);
            this.VidStdLabel.TabIndex = 21;
            this.VidStdLabel.Text = "Video Standard - ";
            this.InputInterfaceLabel.AutoSize = true;
            this.InputInterfaceLabel.Font = new Font("Consolas", 9f);
            this.InputInterfaceLabel.Location = new Point(7, 39);
            this.InputInterfaceLabel.Name = "InputInterfaceLabel";
            this.InputInterfaceLabel.Size = new Size(133, 14);
            this.InputInterfaceLabel.TabIndex = 20;
            this.InputInterfaceLabel.Text = "Input Interface - ";
            this.StreamStatusLabel.AutoSize = true;
            this.StreamStatusLabel.Font = new Font("Consolas", 9f);
            this.StreamStatusLabel.Location = new Point(7, 16);
            this.StreamStatusLabel.Name = "StreamStatusLabel";
            this.StreamStatusLabel.Size = new Size(119, 14);
            this.StreamStatusLabel.TabIndex = 19;
            this.StreamStatusLabel.Text = "Stream Status - ";
            this.btnOpenNetVideo.Font = new Font("Microsoft Sans Serif", 10f);
            this.btnOpenNetVideo.Location = new Point(6, 76);
            this.btnOpenNetVideo.Name = "btnOpenNetVideo";
            this.btnOpenNetVideo.Size = new Size(144, 44);
            this.btnOpenNetVideo.TabIndex = 30;
            this.btnOpenNetVideo.Text = "Open Network Video";
            this.btnOpenNetVideo.UseVisualStyleBackColor = true;
            this.btnOpenNetVideo.Click += new EventHandler(this.btnOpenNetVideo_Click);
            this.btnHidePanel.Location = new Point(14, 7);
            this.btnHidePanel.Name = "btnHidePanel";
            this.btnHidePanel.Size = new Size(75, 23);
            this.btnHidePanel.TabIndex = 0;
            this.btnHidePanel.Text = "Hide Panel";
            this.btnHidePanel.UseVisualStyleBackColor = true;
            this.btnHidePanel.Click += new EventHandler(this.btnHidePanel_Click);
            this.Controls.Add((Control)this.splitContainer1);
            this.Name = nameof(VideoPlayer);
            this.Size = new Size(1346, 730);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((ISupportInitialize)this.dataGridViewStandartTags).EndInit();
            ((ISupportInitialize)this.dataGridViewCustomTags).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxVidInfo.ResumeLayout(false);
            this.groupBoxVidInfo.PerformLayout();
            this.ResumeLayout(false);
        }

        public enum OSDcolor
        {
            White,
            Yellow,
            Red,
        }
    }

    public class VideoPlayerForm : Form
    {
        private IContainer components = (IContainer)null;
        private VideoPlayer videoPlayerUserControl;

        public VideoPlayerForm(MyUserControl uc)
        {
            this.InitializeComponent();
            this.videoPlayerUserControl = (VideoPlayer)uc;
            uc.Dock = DockStyle.Fill;
            this.Controls.Add((Control)uc);
        }

        private void VideoPlayerForm_Activated(object sender, EventArgs e)
        {
            this.videoPlayerUserControl.Activate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(VideoPlayerForm));
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1305, 610);
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.Name = nameof(VideoPlayerForm);
            this.Text = "Nextvision Video Player";
            this.Activated += new EventHandler(this.VideoPlayerForm_Activated);
            this.ResumeLayout(false);
        }
    }
}
