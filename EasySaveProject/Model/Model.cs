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
        public static class AppPaths
        {
            public static string ProjectRootPath =>
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));

            public static string DataDirectory => Path.Combine(ProjectRootPath, "Data");

            public static string WorksPath => Path.Combine(DataDirectory, "works.json");
            public static string SettingsPath => Path.Combine(DataDirectory, "settings.json");
            public static string StatePath => Path.Combine(DataDirectory, "state.json");
            public static string LogsDirectory => Path.Combine(DataDirectory, "Logs");

            static AppPaths()
            {
                // Create directories if they do not exist
                Directory.CreateDirectory(DataDirectory);
                Directory.CreateDirectory(LogsDirectory);
            }
        }


        // Function to add a new work to the list of works
        public void AddWork(string name, string src, string dst, BackupType type)
        {
            Works.Add(new Work(name, src, dst, type));
        }

        // Function to save the list of works to a JSON file
        public void SaveWorks()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Works, options);
            File.WriteAllText(AppPaths.WorksPath, json);
        }

        // Function to load the list of works from a JSON file
        public void LoadWorks()
        {
            if (File.Exists(AppPaths.WorksPath))
                Works = JsonSerializer.Deserialize<List<Work>>(File.ReadAllText(AppPaths.WorksPath));
        }

        // Function to update the state of the works from a JSON file
        private static readonly object stateLock = new object();

        public void UpdateState(State updatedState)
        {
            lock (stateLock)
            {
                StateFileContainer stateFile;
                if (File.Exists(AppPaths.StatePath))
                {
                    string json = File.ReadAllText(AppPaths.StatePath);
                    stateFile = JsonSerializer.Deserialize<StateFileContainer>(json) ?? new StateFileContainer();
                }
                else
                {
                    stateFile = new StateFileContainer();
                }

                var existingState = stateFile.States.FirstOrDefault(s => s.Name == updatedState.Name);
                if (existingState != null)
                {
                    // Update the existing state
                    existingState.TotalFile = updatedState.TotalFile;
                    existingState.TotalSize = updatedState.TotalSize;
                    existingState.LeftFile = updatedState.LeftFile;
                    existingState.LeftSize = updatedState.LeftSize;
                    existingState.Progress = updatedState.Progress;
                    existingState.CurrentPathSrc = updatedState.CurrentPathSrc;
                    existingState.CurrentPathDst = updatedState.CurrentPathDst;
                    existingState.CurrentDateTime = updatedState.CurrentDateTime;
                    existingState.StateStatus = updatedState.StateStatus;
                }
                else
                {
                    stateFile.States.Add(updatedState);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(AppPaths.StatePath, JsonSerializer.Serialize(stateFile, options));
            }
        }

        // Function to write log entries to a log file
        public void LogAction(string backupName, string source, string destination, long size, double transferTime, string logFormat, double encryptionTime = 0)
        {
            Logger.LogDirectory = AppPaths.LogsDirectory;

            if (Enum.TryParse(logFormat, true, out LogFormat format))
            {
                Logger.CurrentLogFormat = format;
            }
            else
            {
                Logger.CurrentLogFormat = LogFormat.JSON; // fallback
            }

            Logger.WriteLog(new LogEntry
            {
                Name = backupName,
                FileSource = source,
                FileDestination = destination,
                FileSize = size,
                FileTransferTime = transferTime,
                FileEncryptionTime = encryptionTime,
                Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            });
        }
    }
}
