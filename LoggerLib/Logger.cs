using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml;

namespace LoggerLib
{
    public enum LogFormat
    {
        JSON,
        XML
    }

    public class LogEntry
    {
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileDestination { get; set; }
        public long FileSize { get; set; }
        public double FileTransferTime { get; set; } 
        public double FileEncryptionTime { get; set; }
        public string Time { get; set; }
    }

    public static class Logger
    {
        private static readonly object fileLock = new();
        public static string LogDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        public static LogFormat CurrentLogFormat { get; set; } = LogFormat.JSON;

        private static readonly Mutex mutex = new Mutex(false, "Global\\EasySaveLogMutex");

        public static void WriteLogJson(LogEntry entry)
        {
            string fileName = $"{DateTime.Now:yyyy-MM-dd}.json";
            string fullPath = Path.Combine(LogDirectory, fileName);

            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            try
            {
                mutex.WaitOne();

                List<LogEntry> existingLogs = new();

                if (File.Exists(fullPath))
                {
                    try
                    {
                        string existingJson = File.ReadAllText(fullPath);
                        existingLogs = JsonSerializer.Deserialize<List<LogEntry>>(existingJson) ?? new();
                    }
                    catch
                    {
                        existingLogs = new();
                    }
                }

                existingLogs.Add(entry);
                string newJson = JsonSerializer.Serialize(existingLogs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fullPath, newJson);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private static void WriteLogXml(LogEntry entry)
        {
            string fileName = $"{DateTime.Now:yyyy-MM-dd}.xml";
            string fullPath = Path.Combine(LogDirectory, fileName);

            try
            {
                mutex.WaitOne(); // 🔒 Verrou global partagé

                var doc = new XmlDocument();
                XmlElement root;

                if (File.Exists(fullPath))
                {
                    doc.Load(fullPath);
                    root = doc.DocumentElement;
                }
                else
                {
                    root = doc.CreateElement("Logs");
                    doc.AppendChild(root);
                }

                XmlElement log = doc.CreateElement("Log");

                void AddElem(string name, string value)
                {
                    var elem = doc.CreateElement(name);
                    elem.InnerText = value;
                    log.AppendChild(elem);
                }

                AddElem("Name", entry.Name);
                AddElem("FileSource", entry.FileSource);
                AddElem("FileDestination", entry.FileDestination);
                AddElem("FileSize", entry.FileSize.ToString());
                AddElem("FileTransferTime", entry.FileTransferTime.ToString("F3"));
                AddElem("FileEncryptionTime", entry.FileEncryptionTime.ToString("F3"));
                AddElem("Time", entry.Time);

                root.AppendChild(log);
                doc.Save(fullPath);
            }
            finally
            {
                mutex.ReleaseMutex(); // 🔓 Libération du verrou
            }
        }

        public static void WriteLog(LogEntry entry)
        {
            switch (CurrentLogFormat)
            {
                case LogFormat.JSON:
                    WriteLogJson(entry);
                    break;
                case LogFormat.XML:
                    WriteLogXml(entry);
                    break;
            }
        }

        public static List<LogEntry> ReadLogs(string date)
        {
            string fileName = $"{date}.json";
            string fullPath = Path.Combine(LogDirectory, fileName);

            if (!File.Exists(fullPath))
                return new List<LogEntry>();

            string json = File.ReadAllText(fullPath);
            return JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
        }
    }
}
