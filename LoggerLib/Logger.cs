using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace LoggerLib
{
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
        public static string LogDirectory { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        public static void WriteLog(LogEntry entry)
        {
            string fileName = $"{DateTime.Now:yyyy-MM-dd}.json";
            string fullPath = Path.Combine(LogDirectory, fileName);

            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            List<LogEntry> existingLogs = new List<LogEntry>();

            if (File.Exists(fullPath))
            {
                try
                {
                    string existingJson = File.ReadAllText(fullPath);
                    existingLogs = JsonSerializer.Deserialize<List<LogEntry>>(existingJson) ?? new List<LogEntry>();
                }
                catch
                {
                    existingLogs = new List<LogEntry>(); // fallback if corrupted
                }
            }

            existingLogs.Add(entry);

            string newJson = JsonSerializer.Serialize(existingLogs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fullPath, newJson);
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

        public static void DisplayLogs(string date, int page = 1, int pageSize = 5)
        {
            var logs = ReadLogs(date);
            int totalPages = (int)Math.Ceiling((double)logs.Count / pageSize);

            if (logs.Count == 0)
            {
                Console.WriteLine("Aucun log trouvé pour cette date.");
                return;
            }

            if (page < 1 || page > totalPages)
            {
                Console.WriteLine("Page invalide.");
                return;
            }

            var pageLogs = logs.Skip((page - 1) * pageSize).Take(pageSize);

            Console.WriteLine($"\nAffichage des logs - Page {page}/{totalPages}:\n");

            foreach (var log in pageLogs)
            {
                Console.WriteLine($"Nom: {log.Name}");
                Console.WriteLine($"Fichier Source: {log.FileSource}");
                Console.WriteLine($"Fichier Cible: {log.FileDestination}");
                Console.WriteLine($"Taille (octets): {log.FileSize}");
                Console.WriteLine($"Durée de transfert (ms): {log.FileTransferTime:F3}");
                if (log.FileEncryptionTime == 0)
                {
                    Console.WriteLine($"Non crypté");
                }
                else
                {
                    Console.WriteLine($"Durée de cryptage (ms): { log.FileEncryptionTime:F3}");
                }
                Console.WriteLine($"Heure: {log.Time}");
                Console.WriteLine("--------------------------------------");
            }
        }
    }
}
