using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using System.IO.Ports;

namespace SensorTesting
{
    class SerialPortFinder
    {

        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int WM_DEVICECHANGE = 0x0219;

        // Delegate for event handler to handle the device events 
        public delegate void SerialPortFinderEventHandler(Object sender, fireDeviceChangedEventArgs e);

        public event SerialPortFinderEventHandler DeviceChanged;

        static string[] getNames()
        {
            return SerialPort.GetPortNames();
        }

        public string[] getSerialPortsInfo()
        {

            /*
            string[] portNames = SerialPort.GetPortNames();
            foreach(string portName in portNames){
                Console.WriteLine("Serial port name : {0}", portName);
            }//*/

            //*/
            // http://stackoverflow.com/a/2876126
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort"))
            {
                //searcher.Get();
                //Console.WriteLine("searcher {0}", searcher);


                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();

                foreach (var port in ports)
                {
                    Console.WriteLine("port: {0}", ports);
                }

                var tList = (from n in SerialPortFinder.getNames()
                             join p in ports on n equals p["DeviceID"].ToString()
                             select n).ToList();// + " - " + p["Caption"]).ToList();

                string[] portNames = new string[tList.Count()];
                int i = 0;
                foreach (string s in tList)
                {
                    portNames[i++] = s;
                }

                return portNames;
            }
        }

        internal void WndProc(System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    if (m.WParam.ToInt32() == DBT_DEVICEARRIVAL)
                    {
                        System.Windows.Forms.MessageBox.Show("MEDIA FOUND");
                    }
                    Console.WriteLine("Message: {0}, Param: {1} ({2})", m, m.WParam, m.WParam.ToInt32());

                    //getSerialPortsInfo();
                    Thread workerThread = new Thread(this.fireDeviceChangedEvent);
                    workerThread.Start();



                    break;
            }
        }

        private void fireDeviceChangedEvent(object arg)
        {
            SerialPortFinderEventHandler tempDeviceChanged = DeviceChanged;
            if (tempDeviceChanged != null)
            {
                fireDeviceChangedEventArgs e = new fireDeviceChangedEventArgs();

                e.portNames = SerialPortFinder.getNames();

                tempDeviceChanged(this, e);
            }
        }


    }

    class fireDeviceChangedEventArgs : EventArgs
    {
        public string[] portNames;
    }
}
