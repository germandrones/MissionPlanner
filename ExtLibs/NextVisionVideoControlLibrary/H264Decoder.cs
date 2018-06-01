// Decompiled with JetBrains decompiler
// Type: NextVisionVideoControlLibrary.H264Decoder
// Assembly: NextVisionVideoControlLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BAB46FF6-B7A5-4DE4-A450-676207D967B4
// Assembly location: C:\Program Files\NextVision\NextVision CCA2\NextVisionVideoControlLibrary.DLL

using FFmpeg.AutoGen;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;

namespace NextVisionVideoControlLibrary
{
    public class H264Decoder
    {
        public Mutex RGB_mutex = new Mutex();
        private int width = 1280;
        private int height = 720;
        private unsafe AVFrame* converted_frame_vga = ffmpeg.av_frame_alloc();
        private unsafe AVFrame* converted_frame_sd = ffmpeg.av_frame_alloc();
        private unsafe AVFrame* converted_frame_hd = ffmpeg.av_frame_alloc();
        private Mutex KLV_mutex = new Mutex();
        private klv_tag[] klv_tags_array = new klv_tag[256];
        private AesManaged h264_aes = new AesManaged();
        private Mpeg2TsDemux mp2ts_demux = new Mpeg2TsDemux();
        private const int width_vga = 640;
        private const int height_vga = 480;
        private const int width_sd = 720;
        private const int height_sd = 576;
        private const int width_hd = 1280;
        private const int height_hd = 720;
        private const int NUM_OF_KLV_TAGS = 256;
        private const int KLV_TAG_DATA_SIZE = 128;
        public byte[] RGB_output_buffer;
        public IPAddress url_ip;
        public int url_port;
        public string file_path;
        public stream_status stream_status;
        private BackgroundWorker decode_bgw;
        private bool bgw_restart_stream;
        private H264Decoder.RawFrameReadyCB RawFrameReadyCallBack;
        private unsafe AVCodecContext* codec_context;
        private unsafe AVCodec* codec;
        private unsafe SwsContext* convert_context_vga;
        private unsafe SwsContext* convert_context_sd;
        private unsafe SwsContext* convert_context_hd;
        private ICryptoTransform h264_aes_encryptor;

        public unsafe H264Decoder()
        {
            ffmpeg.av_register_all();
            ffmpeg.avdevice_register_all();
            ffmpeg.avcodec_register_all();
            ffmpeg.avformat_network_init();
            this.codec = ffmpeg.avcodec_find_decoder(AVCodecID.AV_CODEC_ID_H264);
            if ((IntPtr)this.codec == IntPtr.Zero)
                throw new ApplicationException("Could not find video decoder");
            this.codec_context = ffmpeg.avcodec_alloc_context3(this.codec);
            if ((IntPtr)this.codec_context == IntPtr.Zero)
                throw new ApplicationException("Could not open the codex context");
            if ((this.codec->capabilities & 8) == 8)
                this.codec_context->flags |= 65536;
            if (ffmpeg.avcodec_open2(this.codec_context, this.codec, (AVDictionary**)null) < 0)
                throw new ApplicationException("Could not open the codec");
            this.convert_context_vga = ffmpeg.sws_getContext(640, 480, AVPixelFormat.AV_PIX_FMT_YUV420P, 640, 480, AVPixelFormat.AV_PIX_FMT_BGR24, 1, (SwsFilter*)null, (SwsFilter*)null, (double*)null);
            if ((IntPtr)this.convert_context_vga == IntPtr.Zero)
                throw new ApplicationException("Could not initialize the conversion context");
            ffmpeg.avpicture_fill((AVPicture*)this.converted_frame_vga, (sbyte*)ffmpeg.av_malloc((ulong)ffmpeg.av_image_get_buffer_size(AVPixelFormat.AV_PIX_FMT_BGR24, 640, 480, 1)), AVPixelFormat.AV_PIX_FMT_BGR24, 640, 480);
            this.convert_context_sd = ffmpeg.sws_getContext(720, 576, AVPixelFormat.AV_PIX_FMT_YUV420P, 720, 576, AVPixelFormat.AV_PIX_FMT_BGR24, 1, (SwsFilter*)null, (SwsFilter*)null, (double*)null);
            if ((IntPtr)this.convert_context_sd == IntPtr.Zero)
                throw new ApplicationException("Could not initialize the conversion context");
            ffmpeg.avpicture_fill((AVPicture*)this.converted_frame_sd, (sbyte*)ffmpeg.av_malloc((ulong)ffmpeg.av_image_get_buffer_size(AVPixelFormat.AV_PIX_FMT_BGR24, 720, 576, 1)), AVPixelFormat.AV_PIX_FMT_BGR24, 720, 576);
            this.convert_context_hd = ffmpeg.sws_getContext(1280, 720, AVPixelFormat.AV_PIX_FMT_YUV420P, 1280, 720, AVPixelFormat.AV_PIX_FMT_BGR24, 1, (SwsFilter*)null, (SwsFilter*)null, (double*)null);
            if ((IntPtr)this.convert_context_hd == IntPtr.Zero)
                throw new ApplicationException("Could not initialize the conversion context");
            ffmpeg.avpicture_fill((AVPicture*)this.converted_frame_hd, (sbyte*)ffmpeg.av_malloc((ulong)ffmpeg.av_image_get_buffer_size(AVPixelFormat.AV_PIX_FMT_BGR24, 1280, 720, 1)), AVPixelFormat.AV_PIX_FMT_BGR24, 1280, 720);
            this.RGB_output_buffer = new byte[2764800];
            int num = 0;
            while (num < 2764800)
            {
                this.RGB_output_buffer[num + 1] = (byte)128;
                num += 3;
            }
            this.decode_bgw = new BackgroundWorker();
            this.decode_bgw.DoWork += new DoWorkEventHandler(this.decode_bgw_DoWork);
            for (int index = 0; index < 256; ++index)
            {
                this.klv_tags_array[index].data = new byte[128];
                this.klv_tags_array[index].valid = false;
            }
            this.h264_aes.Mode = CipherMode.ECB;
            this.h264_aes.IV = new byte[16];
            this.h264_aes.Key = new byte[16]
            {
        (byte) 50,
        (byte) 209,
        (byte) 71,
        (byte) 246,
        (byte) 233,
        (byte) 9,
        (byte) 38,
        (byte) 151,
        (byte) 82,
        (byte) 109,
        (byte) 75,
        (byte) 19,
        (byte) 47,
        (byte) 236,
        (byte) 239,
        (byte) 163
            };
            this.h264_aes.Padding = PaddingMode.None;
            this.h264_aes_encryptor = this.h264_aes.CreateEncryptor();
        }

        public void StartDecoder(IPAddress url_ip, int url_port, H264Decoder.RawFrameReadyCB raw_frame_cb)
        {
            this.url_ip = url_ip;
            this.url_port = url_port;
            this.file_path = (string)null;
            this.RawFrameReadyCallBack = raw_frame_cb;
            if (this.decode_bgw.IsBusy)
                this.bgw_restart_stream = true;
            else
                this.decode_bgw.RunWorkerAsync();
        }

        public void StartDecoder(string file_name, H264Decoder.RawFrameReadyCB raw_frame_cb)
        {
            this.file_path = file_name;
            this.RawFrameReadyCallBack = raw_frame_cb;
            if (this.decode_bgw.IsBusy)
                this.bgw_restart_stream = true;
            else
                this.decode_bgw.RunWorkerAsync();
        }

        public void LockRGBBuffer()
        {
            this.RGB_mutex.WaitOne();
        }

        public void UnLockRGBBuffer()
        {
            this.RGB_mutex.ReleaseMutex();
        }

        public byte[] GetOutputFrame()
        {
            return this.RGB_output_buffer;
        }

        public bool StartRecording(string rec_name)
        {
            if (!Directory.Exists(Path.GetDirectoryName(rec_name)) || !(Path.GetExtension(rec_name) == ".ts") || (this.stream_status != stream_status.StreamDetectionOk || this.mp2ts_demux.recording_status != recording_status.RecordingIdle) || this.stream_status != stream_status.StreamDetectionOk)
                return true;
            this.mp2ts_demux.recording_file_path = rec_name;
            this.mp2ts_demux.recording_cmd = recording_cmd.RecStart;
            return false;
        }

        public bool StopRecording()
        {
            if (this.mp2ts_demux.recording_status != recording_status.RecordingEnabled)
                return true;
            this.mp2ts_demux.DemuxRecordStop();
            return false;
        }

        public recording_status GetRecordingStatus()
        {
            return this.mp2ts_demux.recording_status;
        }

        public int GetRecordingFileSize()
        {
            return this.mp2ts_demux.recording_file_size;
        }

        public void PausePlayback(bool pause_flag)
        {
            this.mp2ts_demux.DemuxPausePlayback(pause_flag);
        }

        private unsafe void decode_bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            /*while (true)
            {
                AVPacket* avPacketPtr = &new AVPacket();
                AVFrame* frame = ffmpeg.av_frame_alloc();
                Stopwatch stopwatch = new Stopwatch();
                this.stream_status = stream_status.StreamAcquiring;
                this.mp2ts_demux.DemuxStart(this.url_ip, this.url_port, this.file_path);
                do
                {
                    stopwatch.Restart();
                    byte[] ba;
                    while (this.mp2ts_demux.DemuxGetFrame(this.mp2ts_demux.h264_frame_queue, out ba))
                    {
                        if (this.file_path == null && stopwatch.Elapsed.Seconds > 1)
                        {
                            this.stream_status = stream_status.StreamLost;
                            this.LockRGBBuffer();
                            H264Decoder.RawFrameReadyCB frameReadyCallBack = this.RawFrameReadyCallBack;
                            if (frameReadyCallBack != null)
                                frameReadyCallBack(this.RGB_output_buffer, this.stream_status, this.width, this.height);
                            this.UnLockRGBBuffer();
                            goto label_31;
                        }
                        else
                            Thread.Sleep(10);
                    }
                    void* voidPtr = ffmpeg.av_malloc((ulong)ba.Length);
                    Marshal.Copy(ba, 0, (IntPtr)voidPtr, ba.Length);
                    ffmpeg.av_packet_from_data(avPacketPtr, (sbyte*)voidPtr, ba.Length);
                    if (avPacketPtr->size > 0)
                    {
                        if (this.klv_parser(avPacketPtr))
                            ffmpeg.av_packet_unref(avPacketPtr);
                        else if (!this.validate_stream(avPacketPtr))
                            ffmpeg.av_packet_unref(avPacketPtr);
                        else if (ffmpeg.avcodec_send_packet(this.codec_context, avPacketPtr) < 0)
                        {
                            ffmpeg.av_packet_unref(avPacketPtr);
                            ffmpeg.av_frame_unref(frame);
                        }
                        else if (ffmpeg.avcodec_receive_frame(this.codec_context, frame) < 0)
                        {
                            ffmpeg.av_packet_unref(avPacketPtr);
                            ffmpeg.av_frame_unref(frame);
                        }
                        else
                        {
                            this.width = frame->width;
                            this.height = frame->height;
                            SwsContext* c;
                            AVFrame* avFramePtr;
                            if (this.width == 640)
                            {
                                c = this.convert_context_vga;
                                avFramePtr = this.converted_frame_vga;
                            }
                            else if (this.width == 720)
                            {
                                c = this.convert_context_sd;
                                avFramePtr = this.converted_frame_sd;
                            }
                            else if (this.width == 1280)
                            {
                                c = this.convert_context_hd;
                                avFramePtr = this.converted_frame_hd;
                            }
                            else
                            {
                                ffmpeg.av_frame_unref(frame);
                                ffmpeg.av_packet_unref(avPacketPtr);
                                continue;
                            }
                            sbyte** srcSlice = &frame->data0;
                            sbyte** dst = &avFramePtr->data0;
                            // ISSUE: reference to a compiler-generated field
                            int* srcStride = &frame->linesize.FixedElementField;
                            // ISSUE: reference to a compiler-generated field
                            int* dstStride = &avFramePtr->linesize.FixedElementField;
                            if (ffmpeg.sws_scale(c, srcSlice, srcStride, 0, this.height, dst, dstStride) != this.height)
                            {
                                ffmpeg.av_packet_unref(avPacketPtr);
                                ffmpeg.av_frame_unref(frame);
                            }
                            else
                            {
                                ffmpeg.av_frame_unref(frame);
                                ffmpeg.av_packet_unref(avPacketPtr);
                                this.stream_status = stream_status.StreamDetectionOk;
                                this.LockRGBBuffer();
                                Marshal.Copy((IntPtr)((void*)avFramePtr->data0), this.RGB_output_buffer, 0, this.width * this.height * 3);
                                H264Decoder.RawFrameReadyCB frameReadyCallBack = this.RawFrameReadyCallBack;
                                if (frameReadyCallBack != null)
                                    frameReadyCallBack(this.RGB_output_buffer, this.stream_status, this.width, this.height);
                                this.UnLockRGBBuffer();
                            }
                        }
                    }
                }
                while (!this.bgw_restart_stream);
                this.bgw_restart_stream = false;
                label_31:
                ffmpeg.av_packet_unref(avPacketPtr);
                ffmpeg.av_frame_unref(frame);
                ffmpeg.avcodec_flush_buffers(this.codec_context);
            }*/
        }

        private unsafe bool klv_parser(AVPacket* pkt_ptr)
        {
            byte[] destination = new byte[512];
            int length = pkt_ptr->size > 512 ? 512 : pkt_ptr->size;
            Marshal.Copy((IntPtr)((void*)pkt_ptr->data), destination, 0, length);
            if (destination[0] != (byte)6 || destination[1] != (byte)14 || (destination[2] != (byte)43 || destination[3] != (byte)52))
                return false;
            int index1 = 18;
            while (index1 < length)
            {
                int index2 = (int)destination[index1];
                int num = (int)destination[index1 + 1];
                if (num < 128)
                {
                    this.KLV_mutex.WaitOne();
                    this.klv_tags_array[index2].id = index2;
                    this.klv_tags_array[index2].len = num;
                    for (int index3 = 0; index3 < num; ++index3)
                        this.klv_tags_array[index2].data[index3] = destination[index1 + index3 + 2];
                    this.klv_tags_array[index2].valid = true;
                    this.KLV_mutex.ReleaseMutex();
                }
                index1 += 2 + num;
            }
            return true;
        }

        public bool get_klv_tag(int tag_id, out klv_tag tag)
        {
            tag = new klv_tag();
            if (tag_id > (int)byte.MaxValue)
                return false;
            this.KLV_mutex.WaitOne();
            if (this.klv_tags_array[tag_id].valid)
            {
                tag.id = this.klv_tags_array[tag_id].id;
                tag.len = this.klv_tags_array[tag_id].len;
                tag.valid = this.klv_tags_array[tag_id].valid;
                tag.data = new byte[128];
                Array.Copy((Array)this.klv_tags_array[tag_id].data, (Array)tag.data, tag.len);
                this.KLV_mutex.ReleaseMutex();
                return true;
            }
            this.KLV_mutex.ReleaseMutex();
            return false;
        }

        public int get_frame_height()
        {
            return this.height;
        }

        public int get_frame_width()
        {
            return this.width;
        }

        private unsafe bool validate_stream(AVPacket* pkt_ptr)
        {
            byte[] destination = new byte[256];
            int length = pkt_ptr->size > 256 ? 256 : pkt_ptr->size;
            byte[] numArray = new byte[16];
            byte[] outputBuffer = new byte[16];
            Marshal.Copy((IntPtr)((void*)pkt_ptr->data), destination, 0, length);
            int num = (int)destination[13] << 24 | (int)destination[12] << 16 | (int)destination[11] << 8 | (int)destination[10];
            if (destination[3] != (byte)1 && destination[4] != (byte)9 || destination[5] != (byte)16 && destination[5] != (byte)48)
                return false;
            Buffer.BlockCopy((Array)destination, 6, (Array)numArray, 0, 4);
            byte[] bytes = BitConverter.GetBytes(num);
            Array.Resize<byte>(ref bytes, 16);
            this.h264_aes_encryptor.TransformBlock(bytes, 0, 16, outputBuffer, 0);
            for (int index = 0; index < 4; ++index)
            {
                if ((int)outputBuffer[index] != (int)numArray[index])
                    return false;
            }
            return true;
        }

        public delegate void RawFrameReadyCB(byte[] frame_buf, stream_status status, int width, int height);
    }
}
