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
using EasySaveProject; 
namespace NS_ViewModel
{
    public class ViewModel
    {
        private Model model = new Model();
        private View view;
        private string _languageMode;
        private readonly string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public ViewModel()
        {
            view = new View(this);
            model.LoadWorks();
            LoadSettings();
        }

        public void Run()
        {
            view.Menu();
        }

        public List<Work> GetWorks() => model.Works;

        public void AddWork(string name, string src, string dst, BackupType type)
        {
            if (model.Works.Count >= 5)
            {
                Console.WriteLine(GetTranslation("MaxBackupLimit"));
                return;
            }
            model.AddWork(name, src, dst, type);
            model.SaveWorks();
            Console.WriteLine(GetTranslation("BackupAdded"));
        }

        public bool RemoveWork(string name)
        {
            var work = model.Works.FirstOrDefault(w => w.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (work != null)
            {
                model.Works.Remove(work);
                model.SaveWorks();
                return true;
            }
            return false;
        }

        public void ExecuteWork(string command)
        {
            var indicesToExecute = new List<int>();

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
                        indicesToExecute.Add(i - 1);
                }
            }
            else if (command.Contains(';'))
            {
                var parts = command.Split(';');
                foreach (var part in parts)
                {
                    if (int.TryParse(part, out int index))
                        indicesToExecute.Add(index - 1);
                }
            }
            else if (int.TryParse(command, out int singleIndex))
            {
                indicesToExecute.Add(singleIndex - 1);
            }
            else
            {
                Console.WriteLine(GetTranslation("InvalidInput"));
                return;
            }

            var works = model.Works;

            foreach (int i in indicesToExecute)
            {
                if (i >= 0 && i < works.Count)
                {
                    var work = works[i];
                    Console.WriteLine($"{GetTranslation("ExecutingBackup")}: {work.Name}");

                    LanguageService languageService = new LanguageService();
                    IBackupType backupType;

                    // Correction ici : cast explicite vers BackupType
                    if ((BackupType)work.Type == BackupType.Full)
                    {
                        backupType = new FullBackup(languageService);
                    }
                    else
                    {
                        backupType = new DifferentialBackup(languageService);
                    }

                    backupType.Transfer(
                        id: i + 1,
                        name: work.Name,
                        source: work.SourcePath,
                        target: work.TargetPath
                    );
                }
                else
                {
                    Console.WriteLine($"{GetTranslation("InvalidBackupIndex")} {i + 1}");
                }
            }

            Console.WriteLine(GetTranslation("ExecutionDone"));
            // Ajout de la pause ici
            Console.WriteLine("Appuyez sur Entrée pour revenir au menu...");
            Console.ReadLine();
        }

        public void ExecuteAll()
        {
            for (int i = 0; i < model.Works.Count; i++)
            {
                ExecuteWork((i + 1).ToString());
            }
        }

        public void ToggleLanguage()
        {
            _languageMode = _languageMode == "English" ? "French" : "English";
            SaveSettings();
            Console.WriteLine($"{GetTranslation("LanguageSwitchedTo")} {_languageMode}");
        }

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

        private void SaveSettings()
        {
            var settings = new { Language = _languageMode };
            var json = JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(settingsPath, json);
        }

        public string GetTranslation(string key)
        {
            return LanguageManager.GetTranslation(key, _languageMode);
        }
    }
}
