﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Management;
using System.Windows.Forms;

namespace MissionPlanner.Comms
{

    public class SerialPort : System.IO.Ports.SerialPort, ICommsSerial
    {
        static object locker = new object();

        public new bool DtrEnable { get { return base.DtrEnable; } set { if (base.DtrEnable == value) return; if (ispx4(base.PortName)) return; base.DtrEnable = value; } }
        public new bool RtsEnable { get { return base.RtsEnable; } set { if (base.RtsEnable == value) return; if (ispx4(base.PortName)) return; base.RtsEnable = value; } }
        

        public new void Open()
        {
            // 500ms write timeout - win32 api default
            this.WriteTimeout = 500;

            if (base.IsOpen)
                return;

            try
            {
                // this causes element not found with bluetooth devices.
                if (BaudRate > 115200)
                {
                    Console.WriteLine("Doing SerialPortFixer");
                    SerialPortFixer.Execute(this.PortName);
                    Console.WriteLine("Done SerialPortFixer");
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            if (PortName.StartsWith("/"))
                if (!File.Exists(PortName))
                    throw new Exception("No such device");

            try
            {
                base.Open();
                base.WriteTimeout = -1;
            }
            catch
            {
                try { Close(); }
                catch { }
#if DEBUG
                throw;
#else
                MessageBox.Show("Unable to open serial port. Please check connection settings.", "Connection fails", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
            }
        }

        public new void Close()
        {
            base.Close();
        }

        public void toggleDTR()
        {
            if (ispx4(this.PortName))
            {
                Console.WriteLine("PX4 - no DTR");
                return;
            }


            bool open = this.IsOpen;
            Console.WriteLine("toggleDTR " + this.IsOpen);
            try
            {
                if (!open)
                    this.Open();
            }
            catch { }


            base.DtrEnable = false;
            base.RtsEnable = false;

            System.Threading.Thread.Sleep(50);

            base.DtrEnable = true;
            base.RtsEnable = true;

            System.Threading.Thread.Sleep(50);

            try
            {
                if (!open)
                    this.Close();
            }
            catch { }
            Console.WriteLine("toggleDTR done " + this.IsOpen);
        }

        public new static string[] GetPortNames()
        {
            // prevent hammering
            lock (locker)
            {
                List<string> allPorts = new List<string>();

                string[] ports = null;

                try
                {
                    ports = System.IO.Ports.SerialPort.GetPortNames()
                    .Select(p => p.TrimEnd())
                    .Select(FixBlueToothPortNameBug)
                    .ToArray();

#region find all FTDI
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity  WHERE Caption like '%(COM%'");
                    List<string> ftdi_ports = new List<string>();

                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        if (queryObj["DeviceID"].ToString().Contains("VID_0403") || queryObj["DeviceID"].ToString().Contains("VID_26AC") || queryObj["DeviceID"].ToString().Contains("VID_1FFB"))
                        {
                            // this device is supported:
                            string devCaption = queryObj["Caption"].ToString();
                            foreach (string p in ports)
                            {
                                if (devCaption.Contains(p))
                                {
                                    ftdi_ports.Add(p);
                                    //allPorts.Add(p);
                                }
                            }
                        }
                    }
#endregion
                }
                catch { }

                if (ports != null) allPorts.AddRange(ports);

                return allPorts.ToArray();
            }
        }

        static Dictionary<string, string> comportnamecache = new Dictionary<string, string>();

        public static string GetNiceName(string port)
        {
            // make sure we are exclusive
            lock (locker)
            {
                portnamenice = "";

                if (comportnamecache.ContainsKey(port))
                {
                    return comportnamecache[port];
                }

                try
                {
                    CallWithTimeout(new Action<string>(GetName), 1000, port);
                }
                catch
                {
                }
                comportnamecache[port] = portnamenice;

                return (string)portnamenice.Clone();
            }
        }

        static string portnamenice = "";

        static void GetName(string port)
        {
            try
            {
                
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SerialPort");                // Win32_USBControllerDevice
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject obj2 in searcher.Get())
                    {
                        //DeviceID                     
                        if (obj2.Properties["DeviceID"].Value.ToString().ToUpper() == port.ToUpper())
                        {
                            portnamenice = obj2.Properties["Name"].Value.ToString();
                            return;
                        }
                    }
                }
                
            }
            catch { }

            portnamenice = "";
        }

        static void CallWithTimeout(Action<string> action, int timeoutMilliseconds, string data)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action(data);
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }

        internal bool ispx4(string port)
        {
            try
            {
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SerialPort");// Win32_USBControllerDevice
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject obj2 in searcher.Get())
                    {
                        //DeviceID                     
                        if (obj2.Properties["DeviceID"].Value.ToString().ToUpper() == port.ToUpper())
                        {
                            if (obj2.Properties["Name"].Value.ToString().ToLower().Contains("px4")) return true;
                        }
                    }
                }
            }
            catch { }

            return false;
        }

        // .NET bug: sometimes bluetooth ports are enumerated with bogus characters 
        // eg 'COM10' becomes 'COM10c' - one workaround is to remove the non numeric  
        // char. Annoyingly, sometimes a numeric char is added, which means this 
        // does not work in all cases. 
        // See http://connect.microsoft.com/VisualStudio/feedback/details/236183/system-io-ports-serialport-getportnames-error-with-bluetooth 
        private static string FixBlueToothPortNameBug(string portName)
        {
            if (!portName.StartsWith("COM"))
                return portName;
            var newPortName = "COM";                                // Start over with "COM" 
            foreach (var portChar in portName.Substring(3).ToCharArray())  //  Remove "COM", put the rest in a character array 
            {
                if (char.IsDigit(portChar))
                    newPortName += portChar.ToString(); // Good character, append to portName 
                //  else
                //log.WarnFormat("Bad (Non Numeric) character in port name '{0}' - removing", portName);
            }

            return newPortName;
        }
    }

    public sealed class SerialPortFixer : IDisposable
    {
        public static void Execute(string portName)
        {
            using (new SerialPortFixer(portName))
            {
            }
        }
#region IDisposable Members

        public void Dispose()
        {
            if (m_Handle != null)
            {
                m_Handle.Dispose();
                m_Handle = null;
            }
            GC.SuppressFinalize(this);
        }

#endregion

#region Implementation

        private const int DcbFlagAbortOnError = 14;
        private const int CommStateRetries = 10;
        private SafeFileHandle m_Handle;

        private SerialPortFixer(string portName)
        {
            const int dwFlagsAndAttributes = 0x40000000;
            const int dwAccess = unchecked((int)0xC0000000); if ((portName == null) || !portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid Serial Port", "portName");
            }
            SafeFileHandle hFile = NativeMethods.CreateFile(@"\\.\" + portName, dwAccess, 0, IntPtr.Zero, 3, dwFlagsAndAttributes,
                                              IntPtr.Zero);
            if (hFile.IsInvalid)
            {
                WinIoError();
            }
            try
            {
                int fileType = NativeMethods.GetFileType(hFile);
                if ((fileType != 2) && (fileType != 0))
                {
                    throw new ArgumentException("Invalid Serial Port", "portName");
                }
                m_Handle = hFile;
                InitializeDcb();
            }
            catch
            {
                hFile.Dispose();
                m_Handle = null;
                throw;
            }
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern int FormatMessage(int dwFlags, HandleRef lpSource, int dwMessageId, int dwLanguageId,
                                                    StringBuilder lpBuffer, int nSize, IntPtr arguments);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool GetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool SetCommState(SafeFileHandle hFile, ref Dcb lpDcb);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern bool ClearCommError(SafeFileHandle hFile, ref int lpErrors, ref Comstat lpStat);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode,
                                                            IntPtr securityAttrs, int dwCreationDisposition,
                                                            int dwFlagsAndAttributes, IntPtr hTemplateFile);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern int GetFileType(SafeFileHandle hFile);

        }

        private void InitializeDcb()
        {
            Dcb dcb = new Dcb();
            GetCommStateNative(ref dcb);
            dcb.Flags &= ~(1u << DcbFlagAbortOnError);
            SetCommStateNative(ref dcb);
        }

        private static string GetMessage(int errorCode)
        {
            StringBuilder lpBuffer = new StringBuilder(0x200);
            if (
                NativeMethods.FormatMessage(0x3200, new HandleRef(null, IntPtr.Zero), errorCode, 0, lpBuffer, lpBuffer.Capacity,
                              IntPtr.Zero) != 0)
            {
                return lpBuffer.ToString();
            }
            return "Unknown Error";
        }

        private static int MakeHrFromErrorCode(int errorCode)
        {
            return (int)(0x80070000 | (uint)errorCode);
        }

        private static void WinIoError()
        {
            int errorCode = Marshal.GetLastWin32Error();
            throw new IOException(GetMessage(errorCode), MakeHrFromErrorCode(errorCode));
        }

        private void GetCommStateNative(ref Dcb lpDcb)
        {
            int commErrors = 0;
            Comstat comStat = new Comstat();

            for (int i = 0; i < CommStateRetries; i++)
            {
                if (!NativeMethods.ClearCommError(m_Handle, ref commErrors, ref comStat))
                {
                    WinIoError();
                }
                if (NativeMethods.GetCommState(m_Handle, ref lpDcb))
                {
                    break;
                }
                if (i == CommStateRetries - 1)
                {
                    WinIoError();
                }
            }
        }
        private void SetCommStateNative(ref Dcb lpDcb)
        {
            int commErrors = 0;
            Comstat comStat = new Comstat(); for (int i = 0; i < CommStateRetries; i++)
            {
                if (!NativeMethods.ClearCommError(m_Handle, ref commErrors, ref comStat))
                {
                    WinIoError();
                }
                if (NativeMethods.SetCommState(m_Handle, ref lpDcb))
                {
                    break;
                }
                if (i == CommStateRetries - 1)
                {
                    WinIoError();
                }
            }
        }

#region Nested type: COMSTAT

        [StructLayout(LayoutKind.Sequential)]
        private struct Comstat
        {
            public readonly uint Flags;
            public readonly uint cbInQue;
            public readonly uint cbOutQue;
        }

#endregion

#region Nested type: DCB

        /*
         * https://msdn.microsoft.com/en-us/library/windows/desktop/aa363214(v=vs.85).aspx
  DWORD fBinary  :1;
  DWORD fParity  :1;
  DWORD fOutxCtsFlow  :1;
  DWORD fOutxDsrFlow  :1;
  DWORD fDtrControl  :2;
  DWORD fDsrSensitivity  :1;
  DWORD fTXContinueOnXoff  :1;
  DWORD fOutX  :1;
  DWORD fInX  :1;
  DWORD fErrorChar  :1;
  DWORD fNull  :1;
  DWORD fRtsControl  :2;
  DWORD fAbortOnError  :1;
  DWORD fDummy2  :17;
         */

        [StructLayout(LayoutKind.Sequential)]
        public struct Dcb
        {
            public readonly uint DCBlength;
            public readonly uint BaudRate;
            public uint Flags;
            public readonly ushort wReserved;
            public readonly ushort XonLim;
            public readonly ushort XoffLim;
            public readonly byte ByteSize;
            public readonly byte Parity;
            public readonly byte StopBits;
            public readonly byte XonChar;
            public readonly byte XoffChar;
            public readonly byte ErrorChar;
            public readonly byte EofChar;
            public readonly byte EvtChar;
            public readonly ushort wReserved1;
        }

#endregion

#endregion
    }
}