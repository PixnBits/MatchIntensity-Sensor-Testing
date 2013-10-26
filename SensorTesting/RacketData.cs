using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorTesting
{
    class RacketData
    {
        public string id;
        public int batteryLevel;
        public int millis;
        public int x, y, z;
        public int xM, yM, zM;

        public RacketData(string id, int batteryLevel, int millis, int x, int y, int z, int xM, int yM, int zM)
        {
            this.id = id;
            this.batteryLevel = batteryLevel;
            this.millis = millis;
            this.x = x;
            this.y = y;
            this.z = z;
            this.xM = xM;
            this.yM = yM;
            this.zM = zM;
        }

        public RacketData()
        {
        }

        public RacketData(string id)
        {
            this.id = id;
        }

        public RacketData(string[] parts)
        {
            this.addData(parts);
        }

        internal void addData(string[] parts)
        {
            if (parts.Length > 0)
            {
                this.id = parts[0];
            }
            if (parts.Length > 1)
            {
                this.batteryLevel = int.Parse(parts[1]);
            }
            if (parts.Length > 2)
            {
                this.millis = int.Parse(parts[2]);
            }
            if (parts.Length > 3)
            {
                this.x = int.Parse(parts[3]);
            }
            if (parts.Length > 4)
            {
                this.y = int.Parse(parts[4]);
            }
            if (parts.Length > 5)
            {
                this.z = int.Parse(parts[5]);
            }
            if (parts.Length > 6)
            {
                this.xM = int.Parse(parts[6]);
            }
            if (parts.Length > 7)
            {
                this.yM = int.Parse(parts[7]);
            }
            if (parts.Length > 8)
            {
                this.zM = int.Parse(parts[8]);
            }
        }
    }
}
