using System.Collections.Generic;

namespace NS_ViewModel
{
    public static class LanguageManager
    {
        private static readonly Dictionary<string, Dictionary<string, string>> translations = new()
        {
            {
                "English", new()
                {
                    { "AddBackup", "Add Backup" },
                    { "EnterName", "Work Name" },
                    { "EnterSource", "Source Folder" },
                    { "EnterDestination", "Destination Folder" },
                    { "EnterType", "Backup Type" }
                }
            },
            {
                "French", new()
                {
                    { "AddBackup", "Ajouter une sauvegarde" },
                    { "EnterName", "Nom du travail" },
                    { "EnterSource", "Dossier source" },
                    { "EnterDestination", "Dossier destination" },
                    { "EnterType", "Type de sauvegarde" }
                }
            }
        };

        public static string GetTranslation(string key, string language)
        {
            if (translations.ContainsKey(language) && translations[language].ContainsKey(key))
                return translations[language][key];
            return key;
        }

        public static Dictionary<string, string> GetAllTranslations(string language)
        {
            return translations.ContainsKey(language) ? translations[language] : new();
        }
    }
}