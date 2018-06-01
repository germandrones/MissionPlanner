// Decompiled with JetBrains decompiler
// Type: NextVisionVideoControlLibrary.Mpeg2TsDemux
// Assembly: NextVisionVideoControlLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BAB46FF6-B7A5-4DE4-A450-676207D967B4
// Assembly location: C:\Program Files\NextVision\NextVision CCA2\NextVisionVideoControlLibrary.DLL

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NextVisionVideoControlLibrary
{
    internal class Mpeg2TsDemux
    {
        public const int MP2TS_FRAME_SIZE = 188;
        public const int RTP_HEADER_SIZE = 12;
        public IPAddress url_ip;
        public int url_port;
        public string file_path;
        public UdpClient udp_socket;
        public BackgroundWorker demux_bgw;
        public bool bgw_restart_demux;
        public Queue mp2ts_frame_queue;
        public Queue h264_frame_queue;
        public recording_cmd recording_cmd;
        public string recording_file_path;
        public recording_status recording_status;
        public int recording_file_size;
        public FileStream file_stream;
        public BinaryWriter bin_writer;
        public bool playback_pause_demux_flag;

        public Mpeg2TsDemux()
        {
            this.demux_bgw = new BackgroundWorker();
            this.mp2ts_frame_queue = new Queue();
            this.mp2ts_frame_queue = Queue.Synchronized(this.mp2ts_frame_queue);
            this.h264_frame_queue = new Queue();
            this.h264_frame_queue = Queue.Synchronized(this.h264_frame_queue);
            this.demux_bgw.DoWork += new DoWorkEventHandler(this.DemuxBgwDoWork);
        }

        public void DemuxStart(IPAddress url_ip, int url_port, string file_path = null)
        {
            this.url_ip = url_ip;
            this.url_port = url_port;
            this.file_path = file_path;
            if (this.demux_bgw.IsBusy)
                this.bgw_restart_demux = true;
            else
                this.demux_bgw.RunWorkerAsync();
        }

        public bool DemuxGetFrame(Queue queue, out byte[] ba)
        {
            if (queue.Count > 0)
            {
                ba = (byte[])queue.Dequeue();
                return false;
            }
            ba = (byte[])null;
            return true;
        }

        public void DemuxRecordStop()
        {
            this.DemuxRecordManager((byte[])null, true);
        }

        public void DemuxPausePlayback(bool pause_flag)
        {
            this.playback_pause_demux_flag = pause_flag;
        }

        private void DemuxBgwDoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                while (this.file_path != null)
                    this.DemuxFileLoop();
                this.DemuxNetworkLoop();
            }
        }

        private void DemuxNetworkLoop()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, this.url_port);
            receiver_states receiverStates = receiver_states.SyncOnIFrame;
            int num1 = 0;
            byte[] numArray1 = new byte[4];
            byte[] numArray2 = new byte[1048576];
            int length1 = int.MaxValue;
            byte[] numArray3 = new byte[1024];
            int length2 = 0;
            bool flag = false;
            byte[] ba = (byte[])null;
            this.udp_socket = new UdpClient();
            try
            {
                byte[] addressBytes = this.url_ip.GetAddressBytes();
                this.udp_socket.ExclusiveAddressUse = false;
                this.udp_socket.Client.ReceiveTimeout = 500;
                this.udp_socket.Client.ReceiveBufferSize = 4194304;
                this.udp_socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                if (addressBytes[0] >= (byte)224 && addressBytes[0] < (byte)240)
                    this.udp_socket.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, (object)new MulticastOption(this.url_ip, IPAddress.Any));
                this.udp_socket.Client.Bind((EndPoint)ipEndPoint);
            }
            catch
            {
                Thread.Sleep(500);
                return;
            }
            this.udp_socket.BeginReceive(new AsyncCallback(Mpeg2TsDemux.DemuxReceiveCallback), (object)this);
            label_6:
            do
            {
                while (!this.DemuxGetFrame(this.mp2ts_frame_queue, out ba))
                {
                    while (true)
                    {
                        if (ba.Length != 0)
                        {
                            if (receiverStates == receiver_states.SyncOnIFrame)
                            {
                                int index = 200;
                                if (ba.Length > index && ba[index] == (byte)71 && (ba[index + 1] == (byte)64 && ba[index + 2] == (byte)32))
                                    receiverStates = receiver_states.CatchFrames;
                                else
                                    goto label_28;
                            }
                            else
                                break;
                        }
                        else
                            goto label_6;
                    }
                    if (receiverStates == receiver_states.CatchFrames)
                    {
                        int sourceIndex1 = 12;
                        while (sourceIndex1 < ba.Length)
                        {
                            byte[] frame = new byte[188];
                            Array.Copy((Array)ba, sourceIndex1, (Array)frame, 0, 188);
                            this.DemuxRecordManager(frame, false);
                            if (frame[0] == (byte)71 && frame[2] == (byte)33)
                            {
                                if (frame[1] == (byte)64)
                                {
                                    num1 = BitConverter.ToInt32(frame, 28) + 14;
                                    int destinationIndex = 0;
                                    Array.Copy((Array)frame, 18, (Array)numArray2, destinationIndex, 170);
                                    length1 = destinationIndex + 170;
                                }
                                else if (((int)frame[3] & 48) == 48)
                                {
                                    int num2 = (int)frame[4];
                                    Array.Copy((Array)frame, num2 + 5, (Array)numArray2, length1, 183 - num2);
                                    length1 += 183 - num2;
                                }
                                else
                                {
                                    Array.Copy((Array)frame, 4, (Array)numArray2, length1, 184);
                                    length1 += 184;
                                }
                            }
                            else if (frame[0] == (byte)71 && frame[2] == (byte)40)
                            {
                                if (frame[1] == (byte)64)
                                {
                                    length2 = (int)BitConverter.ToUInt16(new byte[2]
                                    {
                    frame[26],
                    frame[25]
                                    }, 0);
                                    numArray3 = new byte[length2];
                                    Array.Copy((Array)frame, 30, (Array)numArray3, 0, 158);
                                    flag = true;
                                }
                                else if (frame[1] == (byte)0 & flag)
                                {
                                    int sourceIndex2 = (int)frame[4] + 5;
                                    Array.Copy((Array)frame, sourceIndex2, (Array)numArray3, 158, length2 - 158);
                                    flag = false;
                                    this.h264_frame_queue.Enqueue((object)numArray3);
                                }
                            }
                            sourceIndex1 += 188;
                        }
                    }
                    label_28:
                    if (receiverStates == receiver_states.CatchFrames)
                    {
                        if (length1 == num1)
                        {
                            byte[] numArray4 = new byte[length1];
                            Array.Copy((Array)numArray2, (Array)numArray4, length1);
                            this.h264_frame_queue.Enqueue((object)numArray4);
                        }
                        else if (length1 > num1)
                            this.bgw_restart_demux = true;
                    }
                    if (this.bgw_restart_demux)
                    {
                        this.bgw_restart_demux = false;
                        this.udp_socket.Client.Close();
                        return;
                    }
                }
                Thread.Sleep(1);
            }
            while (!this.bgw_restart_demux);
            this.bgw_restart_demux = false;
            this.udp_socket.Client.Close();
        }

        private void DemuxFileLoop()
        {
            int num1 = 0;
            byte[] numArray1 = new byte[4];
            byte[] numArray2 = new byte[1048576];
            int length1 = int.MaxValue;
            byte[] numArray3 = new byte[1024];
            int length2 = 0;
            bool flag = false;
            this.playback_pause_demux_flag = false;
            FileStream fileStream;
            BinaryReader binaryReader;
            try
            {
                fileStream = System.IO.File.OpenRead(this.file_path);
                binaryReader = new BinaryReader((Stream)fileStream);
            }
            catch
            {
                return;
            }
            do
            {
                while (!this.playback_pause_demux_flag)
                {
                    byte[] numArray4 = binaryReader.ReadBytes(188);
                    if (numArray4.Length == 188)
                    {
                        if (numArray4[0] == (byte)71 && numArray4[2] == (byte)33)
                        {
                            if (numArray4[1] == (byte)64)
                            {
                                num1 = BitConverter.ToInt32(numArray4, 28) + 14;
                                int destinationIndex = 0;
                                Array.Copy((Array)numArray4, 18, (Array)numArray2, destinationIndex, 170);
                                length1 = destinationIndex + 170;
                            }
                            else if (((int)numArray4[3] & 48) == 48)
                            {
                                int num2 = (int)numArray4[4];
                                Array.Copy((Array)numArray4, num2 + 5, (Array)numArray2, length1, 183 - num2);
                                length1 += 183 - num2;
                            }
                            else
                            {
                                Array.Copy((Array)numArray4, 4, (Array)numArray2, length1, 184);
                                length1 += 184;
                            }
                        }
                        else if (numArray4[0] == (byte)71 && numArray4[2] == (byte)40)
                        {
                            if (numArray4[1] == (byte)64)
                            {
                                length2 = (int)BitConverter.ToUInt16(new byte[2]
                                {
                  numArray4[26],
                  numArray4[25]
                                }, 0);
                                numArray3 = new byte[length2];
                                Array.Copy((Array)numArray4, 30, (Array)numArray3, 0, 158);
                                flag = true;
                            }
                            else if (numArray4[1] == (byte)0 & flag)
                            {
                                int sourceIndex = (int)numArray4[4] + 5;
                                Array.Copy((Array)numArray4, sourceIndex, (Array)numArray3, 158, length2 - 158);
                                flag = false;
                                this.h264_frame_queue.Enqueue((object)numArray3);
                            }
                        }
                        if (length1 == num1)
                        {
                            byte[] numArray5 = new byte[length1];
                            Array.Copy((Array)numArray2, (Array)numArray5, length1);
                            this.h264_frame_queue.Enqueue((object)numArray5);
                            Thread.Sleep(40);
                            length1 = 0;
                        }
                        else if (length1 != int.MaxValue && length1 > num1)
                            this.bgw_restart_demux = true;
                    }
                    else if (numArray4.Length == 0)
                        this.bgw_restart_demux = true;
                    if (this.bgw_restart_demux)
                    {
                        this.bgw_restart_demux = false;
                        binaryReader.Close();
                        fileStream.Close();
                        return;
                    }
                }
                Thread.Sleep(10);
            }
            while (!this.bgw_restart_demux);
            this.bgw_restart_demux = false;
            binaryReader.Close();
            fileStream.Close();
        }

        private void DemuxRecordManager(byte[] frame, bool stop_rec_flag)
        {
            if (stop_rec_flag)
            {
                if (this.recording_status != recording_status.RecordingEnabled)
                    return;
                this.DemuxRecordManagerClear();
            }
            else
            {
                switch (this.recording_status)
                {
                    case recording_status.RecordingIdle:
                        if (this.recording_cmd != recording_cmd.RecStart || frame[0] != (byte)71 || (frame[1] != (byte)64 || frame[2] != (byte)33) || frame[23] != (byte)16)
                            break;
                        this.recording_cmd = recording_cmd.RecNoCmd;
                        try
                        {
                            this.file_stream = System.IO.File.Create(this.recording_file_path, 2048, FileOptions.None);
                        }
                        catch
                        {
                            break;
                        }
                        this.bin_writer = new BinaryWriter((Stream)this.file_stream);
                        try
                        {
                            this.bin_writer.Write(frame);
                        }
                        catch
                        {
                            this.DemuxRecordManagerClear();
                            break;
                        }
                        this.recording_status = recording_status.RecordingEnabled;
                        this.recording_file_size += 188;
                        break;
                    case recording_status.RecordingEnabled:
                        try
                        {
                            this.bin_writer.Write(frame);
                        }
                        catch
                        {
                            this.DemuxRecordManagerClear();
                            break;
                        }
                        this.recording_file_size += 188;
                        if (this.recording_cmd != recording_cmd.RecStop)
                            break;
                        this.DemuxRecordManagerClear();
                        break;
                }
            }
        }

        private void DemuxRecordManagerClear()
        {
            this.recording_cmd = recording_cmd.RecNoCmd;
            this.recording_file_size = 0;
            this.recording_status = recording_status.RecordingIdle;
            try
            {
                this.bin_writer.Close();
                this.file_stream.Close();
            }
            catch
            {
            }
        }

        private static void DemuxReceiveCallback(IAsyncResult result)
        {
            Mpeg2TsDemux asyncState = result.AsyncState as Mpeg2TsDemux;
            IPEndPoint remoteEP = new IPEndPoint(0L, 0);
            try
            {
                byte[] numArray = asyncState.udp_socket.EndReceive(result, ref remoteEP);
                if (numArray.Length != 0)
                    asyncState.mp2ts_frame_queue.Enqueue((object)numArray);
            }
            catch
            {
                return;
            }
            try
            {
                asyncState.udp_socket.BeginReceive(new AsyncCallback(Mpeg2TsDemux.DemuxReceiveCallback), (object)asyncState);
            }
            catch
            {
            }
        }
    }
}
