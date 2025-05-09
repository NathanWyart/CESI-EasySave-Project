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
        // Path to the settings file.
        private readonly string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

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
                    var work = works[i];
                    Console.WriteLine($"{GetTranslation("ExecutingBackup")}: {work.Name}");
                    // TODO : Faire la fonctionnalité d'éxécution de sauvegarde
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
                ExecuteWork(work.Name);
            }
        }

        public void ToggleLanguage() // Change the language mode
        {
            _languageMode = _languageMode == "English" ? "French" : "English";
            SaveSettings();
            Console.WriteLine($"{GetTranslation("LanguageSwitchedTo")} {_languageMode}");
        }

        // This method returns the current language mode.
        public string GetCurrentLanguage() => _languageMode;

        private void LoadSettings()
        {
            if (File.Exists(settingsPath))
            {
                var json = File.ReadAllText(settingsPath);
                dynamic settings = JsonConvert.DeserializeObject(json);
                _languageMode = settings.Language ?? "English";
            }
            else
            {
                _languageMode = "English";
                SaveSettings();
            }
        }

        // This method saves the current settings to a file.
        private void SaveSettings()
        {
            var settings = new { Language = _languageMode };
            var json = JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(settingsPath, json);
        }

        // This method returns the translation for a given key.
        public string GetTranslation(string key)
        {
            return LanguageManager.GetTranslation(key, _languageMode);
        }
    }
}
