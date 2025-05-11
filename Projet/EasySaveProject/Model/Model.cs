using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using LoggerLib;

namespace NS_Model
{
    // This class represents the model of the application, which contains the list of works and their states.
    public class Model
    {
        public List<Work> Works { get; set; } = new List<Work>(); // List of works to be performed
        public State StateFile { get; set; } = new State(); // State of the work
        public string LogPath { get; set; } = "logs"; // Path to the log file

        // Function to add a new work to the list of works
        public void AddWork(string name, string src, string dst, BackupType type) 
        {
            Works.Add(new Work(name, src, dst, type));
        }

        // Function to save the list of works to a JSON file
        public void SaveWorks()
        {
            File.WriteAllText("works.json", JsonSerializer.Serialize(Works, new JsonSerializerOptions { WriteIndented = true }));
        }

        // Function to load the list of works from a JSON file
        public void LoadWorks()
        {
            if (File.Exists("works.json"))
                Works = JsonSerializer.Deserialize<List<Work>>(File.ReadAllText("works.json"));
        }

        //// Function to write log entries to a log file
        //public void LogAction(string backupName, string source, string destination, long size, long transferTime)
        //{
        //    Logger.LogDirectory = LogPath;

        //    Logger.WriteLog(new LogEntry
        //    {
        //        Timestamp = DateTime.Now,
        //        BackupName = backupName,
        //        SourcePath = source,
        //        DestinationPath = destination,
        //        FileSize = size,
        //        TransferTimeMs = transferTime
        //    });
        //}
    }
}
