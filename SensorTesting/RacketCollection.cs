using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPUtil;

namespace SensorTesting
{
    class RacketCollection
    {
        private Dictionary<string, List<RacketData>> rackets = new Dictionary<string, List<RacketData>> { };

        private string deliminator = "|";
        private string doubleDelim = "||";

        public string parseSensorLine(string dataLine)
        {
            //Console.Write("dataLine: ");
            //Console.WriteLine(dataLine);

            dataLine = dataLine.Replace("\r", String.Empty);
            while (dataLine.IndexOf(doubleDelim) > -1)
            {
                dataLine = dataLine.Replace(doubleDelim, deliminator);
            }
            if (dataLine.EndsWith(deliminator))
            {
                dataLine = dataLine.Substring(0, dataLine.Length - 1);
            }
            //Console.Write("dataLine: ");
            //Console.WriteLine(dataLine);

            string[] parts = dataLine.Split(deliminator[0]);
            //Console.Write("parts: ");
            //Console.WriteLine(parts);

            //Console.WriteLine("parts length", parts.Length);
            if (parts.Length < 1)
            {
                return null;
            }

            RacketData currentRacket = new RacketData(parts);
            
            if (!this.rackets.ContainsKey(parts[0]))
            {
                rackets.Add(currentRacket.id, new List<RacketData> { });
            }
            rackets[currentRacket.id].Add(currentRacket);

            //Console.Write("racketId: ");
            //Console.WriteLine(currentRacket.id);
            //Console.Write("data points: ");
            //Console.WriteLine(rackets[currentRacket.id].Count);

            if (rackets[currentRacket.id].Count > 50)
            {
                rackets[currentRacket.id].RemoveAt(0);
            }

            return currentRacket.id;
        }

        public Complex[] getComplexPointsMagnitude(string racketName)
        {
            List<RacketData> racket = this.rackets[racketName];
            Complex[] points = new Complex[racket.Count];
            int index = 0;
            foreach (RacketData data in racket)
            {
                points[index++].Re = Math.Sqrt(data.x * data.x + data.y * data.y + data.z * data.z);
            }

            return points;
        }

        public Complex[] getComplexPointsMagnitudeM(string racketName){
            List<RacketData> racket = this.rackets[racketName];
            Complex[] points = new Complex[racket.Count];
            int index = 0;
            foreach(RacketData data in racket){
                points[index++].Re = Math.Sqrt(data.xM * data.xM + data.yM * data.yM + data.zM * data.zM);
            }

            return points;
        }


        public Complex[] getMagnitudeFft(string racketName)
        {
            Complex[] data = this.getComplexPointsMagnitude(racketName);
            DSPUtil.Fourier.FFT(data.Length, data);

            return data;
        }
        public Complex[] getMagnitudeFftM(string racketName)
        {
            Complex[] data = this.getComplexPointsMagnitudeM(racketName);
            DSPUtil.Fourier.FFT(data.Length, data);

            return data;
        }

        public double[] getMagnitudeFftMagnitude(string racketName)
        {
            Complex[] freq = this.getMagnitudeFft(racketName);
            double[] data = new double[freq.Count()];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = freq[i].Magnitude;
            }

            return data;
        }
        public double[] getMagnitudeFftMagnitudeM(string racketName)
        {
            Complex[] freq = this.getMagnitudeFftM(racketName);
            double[] data = new double[freq.Count()];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = freq[i].Magnitude;
            }

            return data;
        }

        public void trimRacketDataToLength(string racketName, int maxIndexes)
        {
            int length = this.rackets[racketName].Count();
            if (length > maxIndexes)
            {
                this.rackets[racketName].RemoveRange(0, length - maxIndexes);
            }
        }

        internal List<RacketData> get(string racketId)
        {
            if (!this.rackets.ContainsKey(racketId))
            {
                return null;
            }
            return rackets[racketId];
        }

        internal RacketData getLastData(string racketId)
        {
            List<RacketData> data = this.get(racketId);
            if (null == data)
            {
                return null;
            }

            if (data.Count < 1)
            {
                return null;
            }
            return data[data.Count - 1];
        }
    }
}
