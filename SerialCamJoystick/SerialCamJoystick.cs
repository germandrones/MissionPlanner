using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.SerialCamControl
{
    public class SerialCamJoystick
    {
        private SerialPort port;
        private Queue<byte> recievedData = new Queue<byte>();
        private int bytesToRead = 7;

        private string serialPort;
        private int baudRate;
        private List<string> receivedData = new List<string>(); // all data stored in list

        public SerialCamJoystick(string serialPort = "COM1", int baudRate = 9600)
        {
            this.serialPort = serialPort;
            this.baudRate = baudRate;
        }

        public bool startThread()
        {
            try
            {
                port = new SerialPort(serialPort, baudRate, Parity.None, 8, StopBits.One);
                port.Open();
                port.DataReceived += this.serialPort_DataReceived;
                return true;
            }
            catch { return false; }
        }

        private void serialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[port.BytesToRead];
            port.Read(data, 0, data.Length);
            data.ToList().ForEach(b => recievedData.Enqueue(b));
            this.processData();
        }

        private void processData()
        {
            if (recievedData.Count >= bytesToRead)
            {
                var packet = Enumerable.Range(0, bytesToRead).Select(i => recievedData.Dequeue());
                byte[] array = packet.ToArray();
                receivedData.Add(BitConverter.ToString(array));
                //receivedSeq = BitConverter.ToString(array);
            }
        }

        string receivedSeq = "";

        public string getAction()
        {
            //string ret = receivedSeq;
            //receivedSeq = "";
            //return ret;

            if (receivedData.Count <= 0) return "";
            string action = receivedData[0];
            receivedData.RemoveAt(0);
            return action;
        }
    }
}
