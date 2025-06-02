using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Timers;
using NS_Model;

namespace EasySaveV3._0.EasySave.ThreadManagement
{
    public static class BusinessSoftwareWatcher
    {
        private static System.Timers.Timer _timer;
        private static List<string> _softwareList = new();
        public static bool IsRunningBusinessSoftware { get; private set; }

        public static event Action OnBusinessSoftwarePaused;
        public static event Action OnBusinessSoftwareResumed;

        private static readonly HashSet<string> _runningWorkNames = new();
        private static readonly object _lock = new();
        private static readonly Model _model = new();

        public static void Start()
        {
            LoadSoftwareList();

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (s, e) => CheckRunningSoftware();
            _timer.AutoReset = true;
            _timer.Start();
        }

        public static void RegisterRunningWork(string workName)
        {
            lock (_lock)
            {
                _runningWorkNames.Add(workName);
            }
        }

        public static void UnregisterRunningWork(string workName)
        {
            lock (_lock)
            {
                _runningWorkNames.Remove(workName);
            }
        }

        private static void LoadSoftwareList()
        {
            if (!File.Exists(Model.AppPaths.SettingsPath))
                return;

            var json = File.ReadAllText(Model.AppPaths.SettingsPath);
            var root = JsonDocument.Parse(json).RootElement;

            if (root.TryGetProperty("WorkSoftware", out var array) && array.ValueKind == JsonValueKind.Array)
            {
                _softwareList = array
                    .EnumerateArray()
                    .Select(v => Path.GetFileNameWithoutExtension(v.GetString() ?? string.Empty))
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToList();
            }
        }

        private static void CheckRunningSoftware()
        {
            bool isRunning = _softwareList.Any(name => Process.GetProcessesByName(name).Length > 0);

            if (isRunning && !IsRunningBusinessSoftware)
            {
                IsRunningBusinessSoftware = true;
                OnBusinessSoftwarePaused?.Invoke();
                UpdateStates("PAUSE");
            }
            else if (!isRunning && IsRunningBusinessSoftware)
            {
                IsRunningBusinessSoftware = false;
                OnBusinessSoftwareResumed?.Invoke();
                UpdateStates("ACTIVE");
            }
        }

        private static void UpdateStates(string newState)
        {
            lock (_lock)
            {
                foreach (var name in _runningWorkNames.ToList())
                {
                    var updated = new State { Name = name, StateStatus = newState };
                    _model.UpdateState(updated);
                }
            }
        }
    }
}