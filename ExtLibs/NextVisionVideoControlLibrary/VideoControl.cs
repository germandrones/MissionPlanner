// Decompiled with JetBrains decompiler
// Type: NextVisionVideoControlLibrary.VideoControl
// Assembly: NextVisionVideoControlLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BAB46FF6-B7A5-4DE4-A450-676207D967B4
// Assembly location: C:\Program Files\NextVision\NextVision CCA2\NextVisionVideoControlLibrary.DLL

using SharpGL;
using SharpGL.Version;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NextVisionVideoControlLibrary
{
    public class VideoControl : UserControl
    {
        private static uint[] textures = new uint[1];
        private static bool en_rotation_fix = false;
        private static float rotation_fix_val = 0.0f;
        private static H264Decoder h264_decoder;
        private const int ver_major = 1;
        private const int ver_minor = 2;
        private const int ver_build = 21;
        private VideoControl.VideoControlClickDelegate vid_ctl_click_delegate;
        private IContainer components;
        private OpenGLControl openGLControl;

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        public VideoControl()
        {
            VideoControl.SetDllDirectory("FFMpeg");
            this.InitializeComponent();
            VideoControl.h264_decoder = new H264Decoder();
        }

        public int VideoControlStartStream(string ip_address, int port, H264Decoder.RawFrameReadyCB raw_frame_cb, VideoControl.VideoControlClickDelegate click_delegate)
        {
            this.vid_ctl_click_delegate = click_delegate;
            IPAddress address;
            if (!IPAddress.TryParse(ip_address, out address) || port <= 1024 || (port >= 65536 || !(ip_address != "0.0.0.0")))
                return -1;
            VideoControl.h264_decoder.StartDecoder(address, port, raw_frame_cb);
            return 0;
        }

        public int VideoControlStartPlayback(string file_path, H264Decoder.RawFrameReadyCB raw_frame_cb)
        {
            string extension = Path.GetExtension(file_path);
            if (!System.IO.File.Exists(file_path) || !(extension == ".ts"))
                return -1;
            VideoControl.h264_decoder.StartDecoder(file_path, raw_frame_cb);
            return 0;
        }

        public bool VideoControlStartRec(string rec_name)
        {
            return VideoControl.h264_decoder.StartRecording(rec_name);
        }

        public bool VideoControlStopRec()
        {
            return VideoControl.h264_decoder.StopRecording();
        }

        public bool VideoControlGetKlvTag(int tag_id, out klv_tag tag)
        {
            return VideoControl.h264_decoder.get_klv_tag(tag_id, out tag);
        }

        public stream_status VideoControlGetStreamStatus()
        {
            return VideoControl.h264_decoder.stream_status;
        }

        public recording_status VideoControlGetRecordStatus()
        {
            return VideoControl.h264_decoder.GetRecordingStatus();
        }

        public int VideoControlGetRecordFileSize()
        {
            return VideoControl.h264_decoder.GetRecordingFileSize();
        }

        public void VideoControlEnRotationFix(bool enable)
        {
            VideoControl.en_rotation_fix = enable;
        }

        public void VideoControlUpdateRotationFix(float angle)
        {
            VideoControl.rotation_fix_val = angle;
        }

        public int VideoControlGetStreamHeight()
        {
            return VideoControl.h264_decoder.get_frame_height();
        }

        public int VideoControlGetStreamWidth()
        {
            return VideoControl.h264_decoder.get_frame_width();
        }

        public void VideoControlGetVersion(ref int major, ref int minor, ref int build)
        {
            major = 1;
            minor = 2;
            build = 21;
        }

        public void VideoControlPausePlayback(bool pause_flag)
        {
            VideoControl.h264_decoder.PausePlayback(pause_flag);
        }

        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs args)
        {
            OpenGL openGl = this.openGLControl.OpenGL;
            openGl.Clear(16640U);
            openGl.LoadIdentity();
            openGl.Translate(0.0f, 0.0f, -3.47f);
            if (VideoControl.en_rotation_fix)
                openGl.Rotate(0.0f, 0.0f, VideoControl.rotation_fix_val);
            else
                openGl.Rotate(0.0f, 0.0f, 0.0f);
            openGl.BindTexture(3553U, VideoControl.textures[0]);
            VideoControl.h264_decoder.LockRGBBuffer();
            openGl.TexImage2D(3553U, 0, 6407U, VideoControl.h264_decoder.get_frame_width(), VideoControl.h264_decoder.get_frame_height(), 0, 32992U, 5121U, VideoControl.h264_decoder.GetOutputFrame());
            VideoControl.h264_decoder.UnLockRGBBuffer();
            float x = (float)VideoControl.h264_decoder.get_frame_width() / (float)VideoControl.h264_decoder.get_frame_height();
            openGl.Begin(7U);
            openGl.TexCoord(1f, 1f);
            openGl.Vertex(x, -1f, 1f);
            openGl.TexCoord(1f, 0.0f);
            openGl.Vertex(x, 1f, 1f);
            openGl.TexCoord(0.0f, 0.0f);
            openGl.Vertex(-x, 1f, 1f);
            openGl.TexCoord(0.0f, 1f);
            openGl.Vertex(-x, -1f, 1f);
            openGl.End();
            openGl.Flush();
        }

        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            OpenGL openGl = this.openGLControl.OpenGL;
            openGl.Enable(3553U);
            openGl.GenTextures(1, VideoControl.textures);
            openGl.BindTexture(3553U, VideoControl.textures[0]);
            openGl.TexParameter(3553U, 10241U, 9729f);
            openGl.TexParameter(3553U, 10240U, 9729f);
        }

        public void activate()
        {
            this.openGLControl.OpenGL.MakeCurrent();
        }

        public void deactivate()
        {
            this.openGLControl.OpenGL.MakeNothingCurrent();
        }

        private void openGLControl_Click(object sender, EventArgs e)
        {
            if (VideoControl.h264_decoder.stream_status != stream_status.StreamDetectionOk)
            {
                VideoControl.VideoControlClickDelegate ctlClickDelegate = this.vid_ctl_click_delegate;
                if (ctlClickDelegate != null)
                    ctlClickDelegate(-1, -1);
            }
            Size size = this.Size;
            MouseEventArgs mouseEventArgs = e as MouseEventArgs;
            int button = (int)mouseEventArgs.Button;
            double num1;
            double num2;
            switch (VideoControl.h264_decoder.get_frame_width())
            {
                case 640:
                    num1 = 640.0;
                    num2 = 480.0;
                    break;
                case 720:
                    num1 = 720.0;
                    num2 = 576.0;
                    break;
                case 1280:
                    num1 = 1280.0;
                    num2 = 720.0;
                    break;
                default:
                    num1 = 1280.0;
                    num2 = 720.0;
                    break;
            }
            double num3 = num1 / num2;
            int num4 = (int)((double)size.Height * 0.013 - 1.45);
            int num5 = size.Height - 2 * num4;
            int num6 = (int)((double)num5 * num3);
            int num7 = (size.Width - num6) / 2;
            int num8 = mouseEventArgs.X - num7;
            int num9 = mouseEventArgs.Y - num4;
            int x_pos = (int)(1280.0 / (double)num6 * (double)num8);
            int y_pos = (int)(720.0 / (double)num5 * (double)num9);
            if (mouseEventArgs.X < num7 || mouseEventArgs.X > num7 + num6 || (mouseEventArgs.Y < num4 || mouseEventArgs.Y > num4 + num5))
            {
                VideoControl.VideoControlClickDelegate ctlClickDelegate = this.vid_ctl_click_delegate;
                if (ctlClickDelegate == null)
                    return;
                ctlClickDelegate(-1, -1);
            }
            else
            {
                VideoControl.VideoControlClickDelegate ctlClickDelegate = this.vid_ctl_click_delegate;
                if (ctlClickDelegate == null)
                    return;
                ctlClickDelegate(x_pos, y_pos);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.openGLControl = new OpenGLControl();
            ((ISupportInitialize)this.openGLControl).BeginInit();
            this.SuspendLayout();
            this.openGLControl.Dock = DockStyle.Fill;
            this.openGLControl.DrawFPS = false;
            this.openGLControl.FrameRate = 100;
            this.openGLControl.Location = new Point(0, 0);
            this.openGLControl.Name = "openGLControl";
            this.openGLControl.OpenGLVersion = OpenGLVersion.OpenGL2_1;
            this.openGLControl.RenderContextType = RenderContextType.NativeWindow;
            this.openGLControl.RenderTrigger = RenderTrigger.TimerBased;
            this.openGLControl.Size = new Size(770, 644);
            this.openGLControl.TabIndex = 0;
            this.openGLControl.OpenGLInitialized += new EventHandler(this.openGLControl_OpenGLInitialized);
            this.openGLControl.OpenGLDraw += new RenderEventHandler(this.openGLControl_OpenGLDraw);
            this.openGLControl.Click += new EventHandler(this.openGLControl_Click);
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add((Control)this.openGLControl);
            this.Name = nameof(VideoControl);
            this.Size = new Size(770, 644);
            ((ISupportInitialize)this.openGLControl).EndInit();
            this.ResumeLayout(false);
        }

        public delegate void VideoControlClickDelegate(int x_pos, int y_pos);
    }
}
