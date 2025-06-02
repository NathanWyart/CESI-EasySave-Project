using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using System.Windows.Threading;
using EasySaveV3._0.EasySave.ThreadManagement;
using NS_Model;

namespace EasySave.ViewModels
{
    public class ExecutionViewModel
    {
        public ObservableCollection<State> RunningStates { get; set; } = new();

        public ICommand PauseCommand { get; set; }
        public ICommand ResumeCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private readonly Model _model = new();

        public ExecutionViewModel()
        {
            LoadStates();

            PauseCommand = new RelayCommand<string>(PauseExecution);
            ResumeCommand = new RelayCommand<string>(ResumeExecution);
            CancelCommand = new RelayCommand<string>(CancelExecution);

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (s, e) => LoadStates();
            timer.Start();
        }

        private void LoadStates()
        {
            if (!File.Exists(Model.AppPaths.StatePath)) return;

            try
            {
                var json = File.ReadAllText(Model.AppPaths.StatePath);
                var container = JsonSerializer.Deserialize<StateFileContainer>(json);

                if (container?.States != null)
                {
                    RunningStates.Clear();
                    foreach (var state in container.States)
                        RunningStates.Add(state);
                }
            }
            catch (IOException) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur de lecture state.json : {ex.Message}");
            }
        }

        private void PauseExecution(string workName)
        {
            if (ThreadSignals.Pausers.TryGetValue(workName, out var pauseEvent))
            {
                pauseEvent.Reset();

                UpdateStateStatus(workName, "PAUSE");
            }
        }

        private void ResumeExecution(string workName)
        {
            if (ThreadSignals.Pausers.TryGetValue(workName, out var pauseEvent))
            {
                pauseEvent.Set();

                UpdateStateStatus(workName, "ACTIVE");
            }
        }

        private void CancelExecution(string workName)
        {
            if (ThreadSignals.Cancellers.TryGetValue(workName, out var cts))
            {
                cts.Cancel();

                UpdateStateStatus(workName, "ANNULE");
            }
        }

        private void UpdateStateStatus(string workName, string newStatus)
        {
            if (!File.Exists(Model.AppPaths.StatePath)) return;

            try
            {
                string json = File.ReadAllText(Model.AppPaths.StatePath);
                var container = JsonSerializer.Deserialize<StateFileContainer>(json) ?? new();

                var state = container.States.FirstOrDefault(s => s.Name == workName);
                if (state != null)
                {
                    state.StateStatus = newStatus;
                    _model.UpdateState(state);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur UpdateStateStatus: {ex.Message}");
            }
        }
    }
}