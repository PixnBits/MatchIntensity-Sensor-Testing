using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPUtil;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SensorTesting
{
    class RacketCollection
    {
        private Dictionary<string, List<RacketData>> rackets = new Dictionary<string, List<RacketData>> { };

        // would like better to have NMEA0183 style messages rather than only one allowed
        // currently looking for format `^D2-C8-9F-3D@F9:00,F9:F8,F9:FF&`
        //private Regex statementParser = new Regex("^([0-9A-F]{2}-[0-9A-F]{2}-[0-9A-F]{2}-[0-9A-F]{2})@([A-F0-9]+):([A-F0-9]+),([A-F0-9]+):([A-F0-9]+),([A-F0-9]+):([A-F0-9]+)", RegexOptions.IgnoreCase);
        private Regex statementParser = new Regex("\\^([0-9A-F]{2}-[0-9A-F]{2}-[0-9A-F]{2}-[0-9A-F]{2})@([0-9A-F:]+),([0-9A-F:]+),([0-9A-F:]+)\\&", RegexOptions.IgnoreCase);

        private int fakeMillis = 0;

        public string parseSensorLine(string dataLine)
        {
            //Console.Write("dataLine: ");
            //Console.WriteLine(dataLine);


            Match matchParts = statementParser.Match(dataLine);

            if (matchParts.Success)
            {
                // groups are 1-indexed, captures are 0-indexed
                string id = matchParts.Groups[1].Captures[0].ToString();
                
                int xM = int.Parse(matchParts.Groups[2].Captures[0].ToString().Replace(":",String.Empty), NumberStyles.HexNumber);
                int yM = int.Parse(matchParts.Groups[3].Captures[0].ToString().Replace(":", String.Empty), NumberStyles.HexNumber);
                int zM = int.Parse(matchParts.Groups[4].Captures[0].ToString().Replace(":", String.Empty), NumberStyles.HexNumber);

                RacketData currentRacket = new RacketData(id, 0, ++fakeMillis, 0, 0, 0, xM, yM, zM);

                if (!this.rackets.ContainsKey(id))
                {
                    Console.WriteLine("Adding racket " + id + " to the list");
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

            return null;

        }

        public Complex[] getComplexPointsMagnitude(string racketName)
        {
            if(!this.rackets.ContainsKey(racketName)){
                return null;
            }
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
            if (!this.rackets.ContainsKey(racketName))
            {
                return null;
            }
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
            if(null == data){
                return null;
            }
            DSPUtil.Fourier.FFT(data.Length, data);

            return data;
        }
        public Complex[] getMagnitudeFftM(string racketName)
        {
            Complex[] data = this.getComplexPointsMagnitudeM(racketName);
            if (data == null)
            {
                return null;
            }
            DSPUtil.Fourier.FFT(data.Length, data);

            return data;
        }

        public double[] getMagnitudeFftMagnitude(string racketName)
        {
            Complex[] freq = this.getMagnitudeFft(racketName);

            if (freq == null)
            {
                return null;
            }

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
            if (freq == null)
            {
                return null;
            }
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

        public string[] getRacketNames() {
            string[] names = new string[this.rackets.Keys.Count];
            int i = 0;
            foreach (string racketName in this.rackets.Keys)
            {
                names[i++] = racketName;
            }

            return names;
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
