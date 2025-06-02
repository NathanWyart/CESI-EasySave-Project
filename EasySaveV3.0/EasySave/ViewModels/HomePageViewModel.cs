using NS_Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;
using EasySave.View;
using Encryption;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using EasySaveV3._0.EasySave.ThreadManagement;

namespace EasySave.ViewModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Work> Works { get; set; } = new ObservableCollection<Work>();

        public ICommand AddWorkCommand { get; }
        public ICommand DeleteWorkCommand { get; }
        public ICommand ExecuteWorkCommand { get; }
        public ICommand ExecuteAllCommand { get; }
        public ICommand DeleteAllCommand { get; }

        private readonly Model _model;
        private string _languageMode;
        private string _encryptedExtensions;
        private string _logFormat;
        private string _priorityExtensions;
        private int _maxParallelFileSizeKo = 1024;

        private static readonly SemaphoreSlim LargeFileSemaphore = new(1, 1);

        public HomePageViewModel()
        {
            _model = new Model();
            _model.LoadWorks();
            foreach (var work in _model.Works)
                Works.Add(work);

            AddWorkCommand = new RelayCommand(OpenAddWorkDialog);
            DeleteWorkCommand = new RelayCommand<Work>(DeleteWork);
            ExecuteWorkCommand = new RelayCommand<Work>(ExecuteWork);
            ExecuteAllCommand = new RelayCommand(ExecuteAll);
            DeleteAllCommand = new RelayCommand(DeleteAll);

            LoadSettings();

            BusinessSoftwareWatcher.OnBusinessSoftwarePaused += () =>
            {
                foreach (var work in Works)
                {
                    if (BusinessSignals.Pausers.TryGetValue(work.Name, out var p))
                    {
                        p.Reset();
                        _model.UpdateState(new State { Name = work.Name, StateStatus = "PAUSE" });
                    }
                }
            };

            BusinessSoftwareWatcher.OnBusinessSoftwareResumed += () =>
            {
                foreach (var work in Works)
                {
                    if (BusinessSignals.Pausers.TryGetValue(work.Name, out var p))
                    {
                        p.Set();
                        _model.UpdateState(new State { Name = work.Name, StateStatus = "ACTIVE" });
                    }
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OpenAddWorkDialog()
        {
            var dialog = new AddWorkWindow();
            if (dialog.ShowDialog() == true)
            {
                if (!Enum.TryParse<BackupType>(dialog.WorkType, out var type))
                {
                    MessageBox.Show("Type de sauvegarde invalide");
                    return;
                }

                var work = new Work(dialog.WorkName, dialog.WorkSrc, dialog.WorkDst, type);
                Works.Add(work);
                _model.Works.Add(work);
                _model.SaveWorks();
                MessageBox.Show($"Travail ajouté : {work.Name}");
            }
        }

        private void DeleteWork(Work work)
        {
            if (work != null)
            {
                Works.Remove(work);
                _model.Works.Remove(work);
                _model.SaveWorks();
                MessageBox.Show($"Travail supprimé : {work.Name}");
            }
        }

        private void ExecuteWork(Work work)
        {
            if (BusinessSoftwareWatcher.IsRunningBusinessSoftware)
            {
                MessageBox.Show("Un logiciel métier est actuellement en cours d'exécution.\nImpossible de lancer la sauvegarde.", "Blocage de sécurité", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ThreadSignals.InitSignals(work.Name);
            BusinessSignals.InitSignal(work.Name);
            BusinessSoftwareWatcher.RegisterRunningWork(work.Name);
            _model.ClearStateFile(new[] { work.Name });

            Thread thread = new(() =>
            {
                try
                {
                    string[] files = Directory.GetFiles(work.Src, "*", SearchOption.AllDirectories);

                    var priorityExts = (_priorityExtensions ?? "")
                        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(e => e.StartsWith('.') ? e : "." + e)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var priorityFiles = files.Where(f => priorityExts.Contains(Path.GetExtension(f))).ToList();
                    var normalFiles = files.Where(f => !priorityExts.Contains(Path.GetExtension(f))).ToList();

                    List<string> filesToProcess = new List<string>();
                    filesToProcess.AddRange(priorityFiles);
                    filesToProcess.AddRange(normalFiles);

                    int totalFiles = files.Length;
                    long totalSize = files.Sum(f => new FileInfo(f).Length);
                    int remainingFiles = totalFiles;
                    long remainingSize = totalSize;

                    var stateEntry = new State
                    {
                        Name = work.Name,
                        TotalFile = totalFiles,
                        TotalSize = totalSize,
                        LeftFile = remainingFiles,
                        LeftSize = remainingSize,
                        StateStatus = "ACTIVE",
                        CurrentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        CurrentPathSrc = "",
                        CurrentPathDst = "",
                        Progress = 0,
                    };

                    _model.UpdateState(stateEntry);

                    var encryptedExts = (_encryptedExtensions ?? "")
                        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(e => e.StartsWith('.') ? e : "." + e)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var cryptoSoft = new CryptoSoft();
                    object stateLock = new();

                    List<string> priorityQueue = new(filesToProcess);

                    while (priorityQueue.Any())
                    {
                        string file = priorityQueue.First();
                        priorityQueue.RemoveAt(0);

                        string fileExt = Path.GetExtension(file);

                        if (work.BackupType == BackupType.DIFFERENTIAL && work.LastBackupDate.HasValue)
                        {
                            DateTime lastWriteTime = File.GetLastWriteTime(file);
                            if (lastWriteTime <= work.LastBackupDate.Value)
                                continue;
                        }

                        ThreadSignals.Pausers[work.Name].WaitOne();
                        BusinessSignals.Pausers[work.Name].WaitOne();

                        if (ThreadSignals.Cancellers[work.Name].IsCancellationRequested)
                        {
                            stateEntry.StateStatus = "ANNULE";
                            _model.UpdateState(stateEntry);
                            break;
                        }

                        if (stateEntry.StateStatus != "ACTIVE")
                        {
                            stateEntry.StateStatus = "ACTIVE";
                            _model.UpdateState(stateEntry);
                        }

                        long fileSize = new FileInfo(file).Length;
                        bool isLargeFile = (fileSize / 1024.0) > _maxParallelFileSizeKo;

                        if (isLargeFile)
                        {
                            LargeFileSemaphore.Wait();
                        }

                        try
                        {
                            string relativePath = Path.GetRelativePath(work.Src, file);
                            string destFile = Path.Combine(work.Dst, relativePath);
                            Directory.CreateDirectory(Path.GetDirectoryName(destFile));

                            var watch = Stopwatch.StartNew();
                            double encryptionTime = 0;

                            if (encryptedExts.Contains(fileExt))
                            {
                                File.Copy(file, destFile, true);
                                encryptionTime = cryptoSoft.EncryptFile(destFile);
                            }
                            else
                            {
                                File.Copy(file, destFile, true);
                            }

                            watch.Stop();
                            long duration = watch.ElapsedMilliseconds;

                            lock (stateLock)
                            {
                                _model.LogAction(work.Name, file, destFile, fileSize, duration, GetCurrentLogFormat(), encryptionTime);

                                remainingFiles--;
                                remainingSize -= fileSize;

                                stateEntry.CurrentPathSrc = work.Src;
                                stateEntry.CurrentPathDst = work.Dst;
                                stateEntry.LeftFile = remainingFiles;
                                stateEntry.LeftSize = remainingSize;
                                stateEntry.CurrentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                stateEntry.Progress = (int)(((double)(totalSize - remainingSize) / totalSize) * 100);

                                _model.UpdateState(stateEntry);
                            }
                        }
                        catch (Exception ex)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                                MessageBox.Show($"Erreur sur le fichier : {file}\n{ex.Message}")
                            );
                        }
                        finally
                        {
                            if (isLargeFile)
                            {
                                LargeFileSemaphore.Release();
                            }
                        }
                    }

                    if (stateEntry.StateStatus != "ANNULE")
                        stateEntry.StateStatus = "END";

                    _model.UpdateState(stateEntry);
                    work.LastBackupDate = DateTime.Now;
                    _model.SaveWorks();

                    ThreadSignals.RemoveSignals(work.Name);
                    BusinessSignals.RemoveSignal(work.Name);
                    BusinessSoftwareWatcher.UnregisterRunningWork(work.Name);

                    MessageBox.Show($"Sauvegarde terminée : {work.Name}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur d'exécution : {ex.Message}");
                }
            });

            thread.Start();
            MessageBox.Show($"Exécution lancée pour : {work.Name}");
        }

        private void ExecuteAll()
        {
            if (BusinessSoftwareWatcher.IsRunningBusinessSoftware)
            {
                MessageBox.Show("Un logiciel métier est actuellement en cours d'exécution.\nImpossible de lancer les sauvegardes.", "Blocage de sécurité", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var manager = new ThreadManager(4);
            var workNames = Works.Select(w => w.Name).ToArray();
            _model.ClearStateFile(workNames);

            foreach (var work in Works)
            {
                manager.Enqueue(() => Task.Run(() => ExecuteWork(work)));
            }

            MessageBox.Show("Tous les travaux ont été lancés.");
        }

        private void DeleteAll()
        {
            if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer tous les travaux ?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Works.Clear();
                _model.Works.Clear();
                _model.SaveWorks();
                MessageBox.Show("Tous les travaux ont été supprimés.");
            }
        }

        private void LoadSettings()
        {
            if (File.Exists(Model.AppPaths.SettingsPath))
            {
                var json = File.ReadAllText(Model.AppPaths.SettingsPath);
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;

                _languageMode = root.GetProperty("Language").GetString() ?? "English";
                _logFormat = root.GetProperty("LogFormat").GetString() ?? "JSON";
                _encryptedExtensions = root.GetProperty("Extensions").GetString() ?? "";
                _priorityExtensions = root.TryGetProperty("PriorityExtensions", out var priExt)
                    ? priExt.GetString() ?? ""
                    : "";
                _maxParallelFileSizeKo = root.TryGetProperty("MaxParallelFileSizeKo", out var sizeProp) && sizeProp.TryGetInt32(out var sizeVal)
                    ? sizeVal
                    : 1024;
            }
        }

        public string GetCurrentLanguage() => _languageMode;
        public string GetCurrentLogFormat() => _logFormat;
        public string GetEncryptedExtensions() => _encryptedExtensions;
    }
}