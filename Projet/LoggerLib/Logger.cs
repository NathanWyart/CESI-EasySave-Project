
using System;
using System.IO;
using System.Text.Json;

namespace LoggerLib
{
    public static class Logger
    {
        private static string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static string logPath = Path.Combine(logDirectory, DateTime.Now.ToString("yyyy-MM-dd") + ".json");

        static Logger()
        {
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);
        }

        public static void Log(string backupName, string src, string dst, long size, long duration)
        {
            var entry = new
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Name = backupName,
                Source = src,
                Destination = dst,
                Size = size,
                Duration = duration
            };

            File.AppendAllText(logPath, JsonSerializer.Serialize(entry, new JsonSerializerOptions { WriteIndented = true }) + Environment.NewLine);
        }
    }
}
