using System;
using System.Diagnostics;
using System.IO.Ports;
using AmbientLightingLib;

namespace AmbientLighting
{
    class Program
    {
        static void Main(string[] args)
        {
            var portNames = SerialPort.GetPortNames();
            var port = new SerialPort(portNames[0]) { BaudRate = 9600 };
            var data = new byte[4];
            var stopwatch = new Stopwatch();

            //var rgbHelper = new ScreenRgbProvider();
            var rgbHelper = new ScreenRgbProvider();

            rgbHelper.Initialize();

            while (true == true)
            {
                stopwatch.Restart();
                var rgbResult = rgbHelper.GetRgbValues();
                stopwatch.Stop();                
                data[0] = 255;
                data[1] = rgbResult.R;
                data[2] = rgbResult.G;
                data[3] = rgbResult.B;

                Console.WriteLine($"Red: {data[1]}, Green: {data[2]}, Blue: {data[3]}, Time: {stopwatch.ElapsedMilliseconds}");

                port.Open();
                port.Write(data, 0, data.Length);
                port.Dispose();
            }
        }
    }
}
