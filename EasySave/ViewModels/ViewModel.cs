using Newtonsoft.Json;
using NS_Model;
using System.Diagnostics;
using System.IO;

namespace NS_ViewModel
{
    // This class is the ViewModel of the application.
    public class ViewModel
    {
        // The ViewModel interacts with the Model and the View.
        private Model model = new Model();
        // The language mode can be either "English" or "French".
        private string _languageMode;

        // Constructor of the ViewModel.
        public ViewModel()
        {
            model.LoadWorks(); // Load works at the start
        }

        public void ToggleLanguage() // Change the language mode
        {
            _languageMode = _languageMode == "English" ? "French" : "English";
            Console.WriteLine($"{GetTranslation("LanguageSwitchedTo")} {_languageMode}");
        }

        // This method returns the current language mode.
        public string GetCurrentLanguage() => _languageMode;
        private string _logFormat;

        public void ToggleLogFormat()
        {
            _logFormat = _logFormat == "JSON" ? "XML" : "JSON";
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
