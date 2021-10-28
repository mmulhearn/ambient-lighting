using System;
using System.IO;

namespace AmbientLightingLib
{
    public static class Logger
    {
        private static StreamWriter _stream;

        static Logger()
        {
            var dt = DateTime.Now;
            var fileName = $"{dt.ToString("yyyyMMddhhmmss")}";
            _stream = File.CreateText($@".\{fileName}.txt");
        }

        public static void LogInformation(string message)
        {
            var dt = DateTime.Now;
            var msg = $"{dt} (Info) - {message}";
            _stream.WriteLine(msg);
            _stream.Flush();
        }

        public static void LogException(Exception ex)
        {
            var dt = DateTime.Now;
            var msg = $"{dt} (Info) - {ex.Message}";
            _stream.WriteLine(msg);
            _stream.Flush();
        }
    }
}
