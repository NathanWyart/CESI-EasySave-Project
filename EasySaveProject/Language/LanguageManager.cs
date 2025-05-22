using System.Collections.Generic;

namespace NS_ViewModel
{
    // This class manages the language translations for the application.
    public static class LanguageManager
    {
        // Dictionary to hold translations for different languages
        private static readonly Dictionary<string, Dictionary<string, string>> translations = new()
        {
            {
                "English", new Dictionary<string, string>
                {
                    { "MaxBackupLimit", "Maximum number of backup works reached." },
                    { "BackupAdded", "Backup work added successfully." },
                    { "ExecutingBackup", "Executing backup" },
                    { "BackupNotFound", "Backup work not found." },
                    { "LanguageSwitchedTo", "Language switched to" },
                    { "AppTitle", "EasySave 2.0" },
                    { "BackupWorkMenu", "Backup Work Management" },
                    { "ExecutionMenu", "Execution Backup Management" },
                    { "Settings", "Settings" },
                    { "Quit", "Quit" },
                    { "Choice", "Choice:" },
                    { "Exiting", "Exiting..." },
                    { "InvalidInput", "Invalid input. Press Enter to retry..." },
                    { "AddBackup", "Add a Backup Work" },
                    { "DeleteBackup", "Delete a Backup Work" },
                    { "ListBackup", "List Backup Works" },
                    { "Back", "Back" },
                    { "PressEnter", "Press Enter to continue..." },
                    { "ExecuteOne", "Execute a Backup Work" },
                    { "ExecuteAll", "Execute All Backup Works" },
                    { "EnterWorkName", "Enter the name of the work to execute:" },
                    { "ExecutionDone", "Execution done. Press Enter to continue..." },
                    { "AllExecuted", "All works executed. Press Enter to continue..." },
                    { "LanguageMode", "Language" },
                    { "ChangeLanguage", "Change Language" },
                    { "EncryptionTargets",  "Select the extensions to encrypt"},
                    { "EnterName", "Name:" },
                    { "EnterSource", "Source:" },
                    { "EnterDestination", "Destination:" },
                    { "EnterType", "Type (FULL/DIFFERENTIAL):" },
                    { "InvalidInputAllFields", "Invalid input. All fields are required. Press Enter to retry..." },
                    { "InvalidBackupType", "Invalid backup type. Use FULL or DIFFERENTIAL. Press Enter to retry..." },
                    { "EnterNameToDelete", "Enter the name of the backup work to delete:" },
                    { "BackupDeleted", "Backup work deleted." },
                    { "ListOfBackups", "List of Backup Works:" },
                    { "InvalidRange", "Invalid Range." },
                    { "InvalidBackupIndex", "Invalid Backup Work Index." },
                    { "EnterWorkIndexes", "Enter the indexes of the backups to be run (ex. 1-3 or 1;3) : " },
                    { "LastBackupUpdate", "Last Backup Update :" },
                    { "EncryptionSettings", "Encryption Settings" },
                    { "Reset", "Reset" },
                    { "LogFormat", "Log format" },
                    { "ChangeLogFormat", "Change log format" },
                    { "LogFormatSwitchedTo", "Log format switched to" },
                    { "LogFormatChangedSuccessfully", "Log format changed successfully." }
                }
            },
            {
                "French", new Dictionary<string, string>
                {
                    { "MaxBackupLimit", "Nombre maximum de sauvegardes atteint." },
                    { "BackupAdded", "Sauvegarde ajoutée avec succès." },
                    { "ExecutingBackup", "Exécution de la sauvegarde" },
                    { "BackupNotFound", "Sauvegarde non trouvée." },
                    { "LanguageSwitchedTo", "Langue changée en" },
                    { "AppTitle", "EasySave 2.0" },
                    { "BackupWorkMenu", "Gestion des travaux de sauvegarde" },
                    { "ExecutionMenu", "Gestion de l'exécution des sauvegardes" },
                    { "Settings", "Paramètres" },
                    { "Quit", "Quitter" },
                    { "Choice", "Choix :" },
                    { "Exiting", "Fermeture..." },
                    { "InvalidInput", "Entrée invalide. Appuyez sur Entrée pour réessayer..." },
                    { "AddBackup", "Ajouter un travail de sauvegarde" },
                    { "DeleteBackup", "Supprimer un travail de sauvegarde" },
                    { "ListBackup", "Lister les travaux de sauvegarde" },
                    { "Back", "Retour" },
                    { "PressEnter", "Appuyez sur Entrée pour continuer..." },
                    { "ExecuteOne", "Exécuter un travail de sauvegarde" },
                    { "ExecuteAll", "Exécuter tous les travaux de sauvegarde" },
                    { "EnterWorkName", "Entrez le nom du travail à exécuter :" },
                    { "ExecutionDone", "Exécution terminée. Appuyez sur Entrée pour continuer..." },
                    { "AllExecuted", "Tous les travaux ont été exécutés. Appuyez sur Entrée pour continuer..." },
                    { "LanguageMode", "Langue" },
                    { "ChangeLanguage", "Changer la langue" },
                    { "EncryptionTargets",  "Sélectionner les extensions à encrypter"},
                    { "EnterName", "Nom :" },
                    { "EnterSource", "Source :" },
                    { "EnterDestination", "Destination :" },
                    { "EnterType", "Type (FULL/DIFFERENTIAL) :" },
                    { "InvalidInputAllFields", "Entrée invalide. Tous les champs sont requis. Appuyez sur Entrée pour réessayer..." },
                    { "InvalidBackupType", "Type de sauvegarde invalide. Utilisez FULL ou DIFFERENTIAL. Appuyez sur Entrée pour réessayer..." },
                    { "EnterNameToDelete", "Entrez le nom du travail de sauvegarde à supprimer :" },
                    { "BackupDeleted", "Travail de sauvegarde supprimé." },
                    { "ListOfBackups", "Liste des travaux de sauvegarde :" },
                    { "InvalidRange", "Plage invalide." },
                    { "InvalidBackupIndex", "Index de sauvegarde invalide." },
                    { "EnterWorkIndexes", "Entrez les index des sauvegardes à exécuter (ex: 1-3 ou 1;3) : " },
                    { "LastBackupUpdate", "Dernière mise à jour :" },
                    { "EncryptionSettings", "Paramètres de Cryptage" },
                    { "Reset", "Réinitialisation" },
                    { "LogFormat", "Format des logs" },
                    { "ChangeLogFormat", "Changer le format des logs" },
                    { "LogFormatSwitchedTo", "Format des logs changé en" },
                    { "LogFormatChangedSuccessfully", "Le format des logs a été modifié avec succès." }
                }
            }
        };

        // Method to get the translation for a given key and language
        public static string GetTranslation(string key, string language)
        {
            if (translations.ContainsKey(language) && translations[language].ContainsKey(key))
            {
                return translations[language][key];
            }
            return key;
        }
    }
}