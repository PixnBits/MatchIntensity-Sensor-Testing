using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSPUtil;

namespace SensorTesting
{
    public partial class Form1 : Form
    {

        private string serialReadBuffer = String.Empty;
        private RacketCollection racketManager;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            racketManager = new RacketCollection();

            serialPort1.DataReceived += serialPort1_DataReceived;
            serialPort1.Open();

            chart1.Series.RemoveAt(0);

            System.Windows.Forms.DataVisualization.Charting.Series
                x = new System.Windows.Forms.DataVisualization.Charting.Series("x"),
                y = new System.Windows.Forms.DataVisualization.Charting.Series("y"),
                z = new System.Windows.Forms.DataVisualization.Charting.Series("z"),
                mag = new System.Windows.Forms.DataVisualization.Charting.Series("mag"),
                xM = new System.Windows.Forms.DataVisualization.Charting.Series("xM"),
                yM = new System.Windows.Forms.DataVisualization.Charting.Series("yM"),
                zM = new System.Windows.Forms.DataVisualization.Charting.Series("zM"),
                magM = new System.Windows.Forms.DataVisualization.Charting.Series("magM"),
                magFft = new System.Windows.Forms.DataVisualization.Charting.Series("magFft"),
                magMFft = new System.Windows.Forms.DataVisualization.Charting.Series("magMFft");

            chart1.Series.Add(x);
            chart1.Series.Add(y);
            chart1.Series.Add(z);
            chart1.Series.Add(mag);
            chart1.Series.Add(xM);
            chart1.Series.Add(yM);
            chart1.Series.Add(zM);
            chart1.Series.Add(magM);
            chart1.Series.Add(magFft);
            chart1.Series.Add(magMFft);

            x.ChartType =
            y.ChartType =
            z.ChartType =
            mag.ChartType =
            xM.ChartType =
            yM.ChartType =
            zM.ChartType =
            magM.ChartType =
                System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            

            xM.ChartArea =
            yM.ChartArea =
            zM.ChartArea =
            magM.ChartArea = 
                chart1.ChartAreas[1].Name;

            magFft.ChartArea =
            magMFft.ChartArea =
                chart1.ChartAreas[2].Name;
        }

        void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            /*
            //Console.WriteLine("Serial data recieved!");
            serialReadBuffer += serialPort1.ReadExisting();   

            // look for \n
            int carriageReturnLocation = serialReadBuffer.IndexOf("\n");
            //Console.Write("carriageReturnLocation: ");
            //Console.WriteLine(carriageReturnLocation);
            if (carriageReturnLocation >= 0)
            {
                processSensorLine(serialReadBuffer.Substring(0,carriageReturnLocation));
                if (serialReadBuffer.Length > carriageReturnLocation)
                {
                    serialReadBuffer = serialReadBuffer.Substring(carriageReturnLocation + 1);
                }
                else
                {
                    serialReadBuffer = String.Empty;
                }
            }
            //Console.Write("left over: ");
            //Console.WriteLine(serialReadBuffer);
            //*/

            serialReadBuffer = serialPort1.ReadLine();
            if (serialReadBuffer.Length > 0)
            {
                processSensorLine(serialReadBuffer);
            }
        }

        void processSensorLine(string reportedLine)
        {
            //Console.Write("whole line? ");
            //Console.WriteLine(reportedLine);
            string racketId = racketManager.parseSensorLine(reportedLine);
            //Console.WriteLine("racketId:", racketId);

            addPoint_threadsafe();
        }

        private void addPoint()
        {
            int maxXPoints = 50;

            System.Windows.Forms.DataVisualization.Charting.Series
                x = chart1.Series["x"],
                y = chart1.Series["y"],
                z = chart1.Series["z"],
                mag = chart1.Series["mag"],
                xM = chart1.Series["xM"],
                yM = chart1.Series["yM"],
                zM = chart1.Series["zM"],
                magM = chart1.Series["magM"],
                magFft = chart1.Series["magFft"],
                magMFft = chart1.Series["magMFft"];

            foreach (string racketName in racketManager.getRacketNames())
            {
                RacketData data = racketManager.getLastData(racketName);

                if (null == data)
                {
                    return;
                }

                if (null != data.x)
                {
                    x.Points.Add((double)data.x);
                }
                if (null != data.y)
                {
                    y.Points.Add((double)data.y);
                }
                if (null != data.z)
                {
                    z.Points.Add((double)data.z);
                }
                if (null != data.x && null != data.y && null != data.z)
                {
                    mag.Points.Add(Math.Sqrt(data.x * data.x + data.y * data.y + data.z * data.z));
                }

                while (x.Points.Count > maxXPoints)
                {
                    x.Points.RemoveAt(0);
                }

                while (y.Points.Count > maxXPoints)
                {
                    y.Points.RemoveAt(0);
                }

                while (z.Points.Count > maxXPoints)
                {
                    z.Points.RemoveAt(0);
                }

                while (mag.Points.Count > maxXPoints)
                {
                    mag.Points.RemoveAt(0);
                }


                if (null != data.xM)
                {
                    xM.Points.Add((double)data.xM);
                }
                if (null != data.yM)
                {
                    yM.Points.Add((double)data.yM);
                }
                if (null != data.zM)
                {
                    zM.Points.Add((double)data.zM);
                }
                if (null != data.xM && null != data.yM && null != data.zM)
                {
                    magM.Points.Add(Math.Sqrt(data.xM * data.xM + data.yM * data.yM + data.zM * data.zM));
                }

                while (xM.Points.Count > maxXPoints)
                {
                    xM.Points.RemoveAt(0);
                }

                while (yM.Points.Count > maxXPoints)
                {
                    yM.Points.RemoveAt(0);
                }

                while (zM.Points.Count > maxXPoints)
                {
                    zM.Points.RemoveAt(0);
                }

                while (magM.Points.Count > maxXPoints)
                {
                    magM.Points.RemoveAt(0);
                }

                // remove all points from the series
                while (magFft.Points.Count() > 0)
                {
                    magFft.Points.RemoveAt(0);
                }
                while (magMFft.Points.Count() > 0)
                {
                    magMFft.Points.RemoveAt(0);
                }
                // compute the FFT of the racket
                double[] magFftData = racketManager.getMagnitudeFftMagnitude(racketName);
                double[] magMFftData = racketManager.getMagnitudeFftMagnitudeM(racketName);
                for (int i = 0; i < magFftData.Length; i++)
                {
                    magFft.Points.Add(magFftData[i]);
                }
                for (int i = 0; i < magMFftData.Length; i++)
                {
                    magMFft.Points.Add(magMFftData[i] / 40);
                }
                racketManager.trimRacketDataToLength(racketName, maxXPoints);
            }

        }

        private void addPoint_threadsafe()
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.addPoint();
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }
    }
}
