using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using NS_Model;
using NS_View;
using System.Xml;
using static NS_Model.Model;

namespace NS_ViewModel
{
    // This class is the ViewModel of the application.
    public class ViewModel
    {
        // The ViewModel interacts with the Model and the View.
        private Model model = new Model();
        private View view;
        // The language mode can be either "English" or "French".
        private string _languageMode;

        private string _encryptedExtensions;
        
        // Constructor of the ViewModel.
        public ViewModel()
        {
            view = new View(this); // Initialize the view
            model.LoadWorks(); // Load works at the start
            LoadSettings(); // Load settings at the start
        }

        // This method is called to run the application.
        public void Run()
        {
            view.Menu();
        }

        // This method returns the list of works.
        public List<Work> GetWorks() => model.Works;

        // This method adds a new work to the list of works.
        public void AddWork(string name, string src, string dst, BackupType type)
        {
            // Check the limit of 5 works
            if (model.Works.Count >= 5)
            {
                Console.WriteLine(GetTranslation("MaxBackupLimit"));
                return;
            }
            // Add the work to the model
            model.AddWork(name, src, dst, type);
            model.SaveWorks();
            Console.WriteLine(GetTranslation("BackupAdded"));
        }

        // This method removes a work from the list of works.
        public bool RemoveWork(string name)
        {
            // Find the work by name and remove it
            var work = model.Works.FirstOrDefault(w => w.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (work != null)
            {
                // Remove the work from the list
                model.Works.Remove(work);
                model.SaveWorks();
                return true;
            }
            return false;
        }

        // This method executes a single work.
        private void ExecuteSingleWork(Work work)
        {
            Console.WriteLine($"{GetTranslation("ExecutingBackup")}: {work.Name} ...");

            try
            {
                string[] files = Directory.GetFiles(work.Src, "*", SearchOption.AllDirectories);
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

                // Update the state in the model
                List<State> stateList = new List<State> { stateEntry };
                model.UpdateState(stateEntry);

                // Iterate through the files and copy them
                foreach (string file in files)
                {
                    if (work.BackupType == BackupType.DIFFERENTIAL && work.LastBackupDate.HasValue)
                    {
                        DateTime lastWriteTime = File.GetLastWriteTime(file);
                        if (lastWriteTime <= work.LastBackupDate.Value)
                            continue;
                    }

                    string relativePath = Path.GetRelativePath(work.Src, file);
                    string destFile = Path.Combine(work.Dst, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));

                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    File.Copy(file, destFile, true);
                    watch.Stop();

                    long fileSize = new FileInfo(file).Length;
                    long duration = watch.ElapsedMilliseconds;

                    model.LogAction(work.Name, file, destFile, fileSize, duration, GetCurrentLogFormat());

                    remainingFiles--;
                    remainingSize -= fileSize;

                    stateEntry.CurrentPathSrc = work.Src;
                    stateEntry.CurrentPathDst = work.Dst;
                    stateEntry.LeftFile = remainingFiles;
                    stateEntry.LeftSize = remainingSize;
                    stateEntry.CurrentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    stateEntry.Progress = (int)(((double)(totalSize - remainingSize) / totalSize) * 100);

                    model.UpdateState(stateEntry);
                }

                stateEntry.StateStatus = "END";
                model.UpdateState(stateEntry);

                work.LastBackupDate = DateTime.Now;
                model.SaveWorks();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{GetTranslation("BackupError")}: {ex.Message}");
            }
        }


        // This method executes a work based on the command given by the user.
        public void ExecuteWork(string command)
        {
            var indicesToExecute = new List<int>();

            // Treatment of ranges (ex. ‘1-3’)
            if (command.Contains('-'))
            {
                var parts = command.Split('-');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int start) &&
                    int.TryParse(parts[1], out int end))
                {
                    if (start > end)
                    {
                        Console.WriteLine(GetTranslation("InvalidRange"));
                        return;
                    }

                    for (int i = start; i <= end; i++)
                        indicesToExecute.Add(i - 1); // index 0-based
                }
            }
            // Handling indexes separated by a semicolon (ex. ‘1;3’)
            else if (command.Contains(';'))
            {
                var parts = command.Split(';');
                foreach (var part in parts)
                {
                    if (int.TryParse(part, out int index))
                        indicesToExecute.Add(index - 1); // index 0-based
                }
            }
            // Processing a single index
            else if (int.TryParse(command, out int singleIndex))
            {
                indicesToExecute.Add(singleIndex - 1); // index 0-based
            }
            else
            {
                Console.WriteLine(GetTranslation("InvalidInput"));
                return;
            }

            var works = model.Works; // Get the list of works

            foreach (int i in indicesToExecute)
            {
                if (i >= 0 && i < works.Count)
                {
                    ExecuteSingleWork(works[i]);
                }
                else
                {
                    Console.WriteLine($"{GetTranslation("InvalidBackupIndex")} {i + 1}");
                }
            }

            Console.WriteLine(GetTranslation("ExecutionDone"));
        }


        // This method executes all works.
        public void ExecuteAll()
        {
            foreach (var work in model.Works)
            {
                ExecuteSingleWork(work);
            }
            Console.WriteLine(GetTranslation("ExecutionDone"));
        }

        public void ToggleLanguage() // Change the language mode
        {
            _languageMode = _languageMode == "English" ? "French" : "English";
            SaveSettings();
            Console.WriteLine($"{GetTranslation("LanguageSwitchedTo")} {_languageMode}");
        }

        public void SetEncryptedExtensions(string? extensions) // Set the encrypted extensions
        {
            if (extensions != null)
                _encryptedExtensions = extensions;
            SaveSettings();
        }

        public void AddEncryptedExtension(string? extension) // Add an encrypted extension
        {
            if (extension == null)
                return;

            if (_encryptedExtensions == "")
            {
                _encryptedExtensions += extension;
                SaveSettings();
            }
            else if (!_encryptedExtensions.Contains(extension))
            {
                _encryptedExtensions += $";{extension}";
                SaveSettings();
            }
        }

        // This method returns the current language mode.
        public string GetCurrentLanguage() => _languageMode;
        public string GetEncryptedExtensions() => _encryptedExtensions;
        private string _logFormat;

        private void LoadSettings()
        {
            if (File.Exists(Model.AppPaths.SettingsPath))
            {
                var json = File.ReadAllText(Model.AppPaths.SettingsPath);
                dynamic settings = JsonConvert.DeserializeObject(json);
                _languageMode = settings.Language ?? "English";
                _encryptedExtensions = settings.Extensions ?? "";
                _logFormat = settings.LogFormat ?? "JSON";
            }
            else
            {
                _languageMode = "English";
                _encryptedExtensions = "";
                _logFormat = "JSON";
                SaveSettings();
            }
        }
        private void SaveSettings()
        {
            var settings = new {
                Language = _languageMode,
                Extensions = _encryptedExtensions,
                LogFormat = _logFormat
            };
            var json = JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Model.AppPaths.SettingsPath, json);
        }

        public void ToggleLogFormat()
        {
            _logFormat = _logFormat == "JSON" ? "XML" : "JSON";
            SaveSettings();
            Console.WriteLine($"{GetTranslation("LogFormatSwitchedTo")} {_logFormat}");
        }

        public string GetCurrentLogFormat() => _logFormat;


        // This method returns the translation for a given key.
        public string GetTranslation(string key)
        {
            return LanguageManager.GetTranslation(key, _languageMode);
        }
    }
}
