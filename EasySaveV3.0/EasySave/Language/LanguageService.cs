using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Collections.Generic;

namespace NS_ViewModel
{
    public class LanguageService : INotifyPropertyChanged
    {
        private static LanguageService _instance;
        public static LanguageService Instance => _instance ??= new LanguageService();

        private string _currentLanguage;
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    SaveLanguage();
                    NotifyTranslationsChanged();
                }
            }
        }

        public string this[string key] => LanguageManager.GetTranslation(key, _currentLanguage);

        private LanguageService()
        {
            LoadLanguage();
        }

        private void LoadLanguage()
        {
            if (File.Exists(NS_Model.Model.AppPaths.SettingsPath))
            {
                var json = File.ReadAllText(NS_Model.Model.AppPaths.SettingsPath);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("Language", out var lang))
                    _currentLanguage = lang.GetString() ?? "English";
                else
                    _currentLanguage = "English";
            }
            else
                _currentLanguage = "English";
        }

        private void SaveLanguage()
        {
            var path = NS_Model.Model.AppPaths.SettingsPath;
            var json = File.Exists(path) ? File.ReadAllText(path) : "{}";
            var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new();
            settings["Language"] = _currentLanguage;
            var updated = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, updated);
        }

        private void NotifyTranslationsChanged()
        {
            foreach (var key in LanguageManager.GetAllTranslations(_currentLanguage).Keys)
            {
                OnPropertyChanged($"Item[{key}]");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}