using Coho.IpcLibrary;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorTesting
{
    class MatchIntensityActionsClient
    {
        public static int ACTION_SHOT = 1;

        IpcClientPipe commClient;
        PipeStream commClientPipe = null;

        public bool connect(bool attemptReconnect = false, int timeout = 10)
        {
            // setup pipe to Match sheet
            commClient = new IpcClientPipe(".", "MatchIntensityActions");

            if (attemptReconnect)
            {
                Console.WriteLine("Warning: `attemptReconnect` not implemented");
            }

            try
            {
                commClientPipe = commClient.Connect(timeout);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Connection failed: " + exception);
                return false;
            }
        }

        public void close()
        {
            if (null != commClientPipe)
            {
                commClientPipe.Close();
                Console.WriteLine("pipe closed");
            }
        }


        internal string sendAction(int action)
        {
            string command = null;
            switch (action)
            {
                case 1://MatchIntensityActionsClient.ACTION_SHOT:
                    command = "Action: shot";
                    break;
                default:
                    throw new NotImplementedException();
            }

            this.sendString(command);
            return this.readString();
        }

        private bool sendString(string msg){
            if (null != commClientPipe)
            {
                // Asynchronously send data to the server
                Byte[] output = Encoding.UTF8.GetBytes(msg);
                commClientPipe.Write(output, 0, output.Length);

                return true;
            }
            return false;
        }

        private string readString()
        {
            if (null != commClientPipe)
            {

                // Read the result
                Byte[] data = new Byte[IpcServer.SERVER_OUT_BUFFER_SIZE];
                Int32 bytesRead = commClientPipe.Read(data, 0, data.Length);
                return Encoding.UTF8.GetString(data, 0, bytesRead);
            }

            return ""; // avoid nulls
        }
    }
}
