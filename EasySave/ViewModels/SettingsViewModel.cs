using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using NS_Model;

namespace EasySave.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<string> EncryptedExtensions { get; set; } = new();
        public ObservableCollection<string> PriorityExtensions { get; set; } = new();
        public ObservableCollection<string> BusinessSoftwares { get; set; } = new();

        public string NewExtension { get; set; }
        public string NewPriorityExtension { get; set; }
        public string NewSoftware { get; set; }

        public List<string> AvailableLanguages { get; } = new() { "English", "French" };
        public List<string> AvailableLogFormats { get; } = new() { "JSON", "XML" };

        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
                SaveSettings();
            }
        }

        private string _selectedLogFormat;
        public string SelectedLogFormat
        {
            get => _selectedLogFormat;
            set
            {
                _selectedLogFormat = value;
                OnPropertyChanged(nameof(SelectedLogFormat));
                SaveSettings();
            }
        }

        private int _maxParallelFileSizeKo;
        public int MaxParallelFileSizeKo
        {
            get => _maxParallelFileSizeKo;
            set
            {
                _maxParallelFileSizeKo = value;
                OnPropertyChanged(nameof(MaxParallelFileSizeKo));
                SaveSettings();
            }
        }

        public ICommand AddExtensionCommand { get; }
        public ICommand RemoveExtensionCommand { get; }
        public ICommand AddPriorityExtensionCommand { get; }
        public ICommand RemovePriorityExtensionCommand { get; }
        public ICommand AddSoftwareCommand { get; }
        public ICommand RemoveSoftwareCommand { get; }

        public SettingsViewModel()
        {
            LoadSettings();

            AddExtensionCommand = new RelayCommand(AddExtension);
            RemoveExtensionCommand = new RelayCommand<string>(RemoveExtension);
            AddPriorityExtensionCommand = new RelayCommand(AddPriorityExtension);
            RemovePriorityExtensionCommand = new RelayCommand<string>(RemovePriorityExtension);
            AddSoftwareCommand = new RelayCommand(AddSoftware);
            RemoveSoftwareCommand = new RelayCommand<string>(RemoveSoftware);
        }

        private void AddExtension()
        {
            if (!string.IsNullOrWhiteSpace(NewExtension) && !EncryptedExtensions.Contains(NewExtension))
            {
                EncryptedExtensions.Add(NewExtension);
                NewExtension = "";
                OnPropertyChanged(nameof(NewExtension));
                SaveSettings();
            }
        }

        private void RemoveExtension(string ext)
        {
            if (EncryptedExtensions.Contains(ext))
            {
                EncryptedExtensions.Remove(ext);
                SaveSettings();
            }
        }

        private void AddPriorityExtension()
        {
            if (!string.IsNullOrWhiteSpace(NewPriorityExtension) && !PriorityExtensions.Contains(NewPriorityExtension))
            {
                PriorityExtensions.Add(NewPriorityExtension);
                NewPriorityExtension = "";
                OnPropertyChanged(nameof(NewPriorityExtension));
                SaveSettings();
            }
        }

        private void RemovePriorityExtension(string ext)
        {
            if (PriorityExtensions.Contains(ext))
            {
                PriorityExtensions.Remove(ext);
                SaveSettings();
            }
        }

        private void AddSoftware()
        {
            if (!string.IsNullOrWhiteSpace(NewSoftware) && !BusinessSoftwares.Contains(NewSoftware))
            {
                BusinessSoftwares.Add(NewSoftware);
                NewSoftware = "";
                OnPropertyChanged(nameof(NewSoftware));
                SaveSettings();
            }
        }

        private void RemoveSoftware(string sw)
        {
            if (BusinessSoftwares.Contains(sw))
            {
                BusinessSoftwares.Remove(sw);
                SaveSettings();
            }
        }

        private void LoadSettings()
        {
            if (File.Exists(Model.AppPaths.SettingsPath))
            {
                var json = File.ReadAllText(Model.AppPaths.SettingsPath);
                using JsonDocument document = JsonDocument.Parse(json);
                JsonElement root = document.RootElement;

                SelectedLanguage = root.GetProperty("Language").GetString() ?? "English";
                SelectedLogFormat = root.GetProperty("LogFormat").GetString() ?? "JSON";

                if (root.TryGetProperty("Extensions", out JsonElement extElement))
                {
                    var extArray = extElement.GetString()?.Split(';') ?? Array.Empty<string>();
                    foreach (var e in extArray)
                    {
                        if (!string.IsNullOrWhiteSpace(e))
                            EncryptedExtensions.Add(e);
                    }
                }

                if (root.TryGetProperty("PriorityExtensions", out JsonElement prioElement))
                {
                    var prioArray = prioElement.GetString()?.Split(';') ?? Array.Empty<string>();
                    foreach (var e in prioArray)
                    {
                        if (!string.IsNullOrWhiteSpace(e))
                            PriorityExtensions.Add(e);
                    }
                }

                if (root.TryGetProperty("WorkSoftware", out JsonElement softwareArray) && softwareArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var sw in softwareArray.EnumerateArray())
                    {
                        BusinessSoftwares.Add(sw.GetString());
                    }
                }

                if (root.TryGetProperty("MaxParallelFileSizeKo", out JsonElement sizeElement) && sizeElement.TryGetInt32(out int size))
                {
                    MaxParallelFileSizeKo = size;
                }
                else
                {
                    MaxParallelFileSizeKo = 10000;
                }
            }
            else
            {
                SelectedLanguage = "English";
                SelectedLogFormat = "JSON";
                MaxParallelFileSizeKo = 10000;
            }
        }

        private void SaveSettings()
        {
            var settings = new
            {
                Language = SelectedLanguage,
                LogFormat = SelectedLogFormat,
                Extensions = string.Join(";", EncryptedExtensions),
                PriorityExtensions = string.Join(";", PriorityExtensions),
                WorkSoftware = BusinessSoftwares,
                MaxParallelFileSizeKo = MaxParallelFileSizeKo
            };

            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Model.AppPaths.SettingsPath, json);
        }
    }
}