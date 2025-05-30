using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using EasySave_WPF.Commands;
using EasySave_WPF.Model;

namespace EasySave_WPF.ViewModels
{
    public class WorkStateViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<State> States { get; } = new();

        public ICommand PauseCommand { get; }
        public ICommand ResumeCommand { get; }
        public ICommand CancelCommand { get; }

        private FileSystemWatcher _watcher;

        public WorkStateViewModel()
        {
            PauseCommand = new RelayCommand(PauseWork);
            ResumeCommand = new RelayCommand(ResumeWork);
            CancelCommand = new RelayCommand(CancelWork);

            LoadStatesFromFile();

            string stateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "state.json");
            string stateDir = Path.GetDirectoryName(stateFilePath);

            _watcher = new FileSystemWatcher(stateDir, "state.json")
            {
                NotifyFilter = NotifyFilters.LastWrite
            };

            _watcher.Changed += (_, __) =>
            {
                Application.Current.Dispatcher.Invoke(LoadStatesFromFile);
            };

            _watcher.EnableRaisingEvents = true;
        }

        private void LoadStatesFromFile()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "state.json");

            if (!File.Exists(path)) return;

            try
            {
                string json = File.ReadAllText(path);
                var wrapper = JsonSerializer.Deserialize<StateWrapper>(json);

                if (wrapper?.States != null)
                {
                    States.Clear();
                    foreach (var state in wrapper.States)
                        States.Add(state);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement des états : {ex.Message}");
            }
        }

        private void PauseWork(object obj)
        {
            if (obj is State state)
                MessageBox.Show($"[SIMULATION] Pause du travail : {state.Name}");
        }

        private void ResumeWork(object obj)
        {
            if (obj is State state)
                MessageBox.Show($"[SIMULATION] Reprise du travail : {state.Name}");
        }

        private void CancelWork(object obj)
        {
            if (obj is State state)
                MessageBox.Show($"[SIMULATION] Annulation du travail : {state.Name}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private class StateWrapper
        {
            public ObservableCollection<State> States { get; set; }
        }
    }
}
