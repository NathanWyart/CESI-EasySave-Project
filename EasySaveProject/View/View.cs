using System;
using System.Collections.Generic;
using NS_Model;
using NS_ViewModel;

namespace NS_View
{
    // This class represents the view layer of the application.
    public class View
    {
        private ViewModel _viewModel;

        // Constructor that takes a ViewModel instance.
        public View(ViewModel vm)
        {
            _viewModel = vm;
        }

        // Main menu of the application.
        public void Menu()
        {
            while (true)
            {
                // Clear the console and display the main menu.
                Console.Clear();
                Console.WriteLine($"===== {_viewModel.GetTranslation("AppTitle")} =====\n");
                Console.WriteLine("1 : " + _viewModel.GetTranslation("BackupWorkMenu"));
                Console.WriteLine("2 : " + _viewModel.GetTranslation("ExecutionMenu"));
                Console.WriteLine("3 : " + _viewModel.GetTranslation("Settings"));
                Console.WriteLine("4 : " + _viewModel.GetTranslation("Quit"));
                Console.Write("\n" + _viewModel.GetTranslation("Choice") + " ");

                // Get user input and validate it.
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 4)
                {
                    switch (choice)
                    {
                        case 1:
                            DisplayBackupWorkMenu();
                            break;
                        case 2:
                            DisplayExecutionMenu();
                            break;
                        case 3:
                            DisplaySettingsMenu();
                            break;
                        case 4:
                            Console.WriteLine(_viewModel.GetTranslation("Exiting"));
                            return;
                    }
                }
                else
                {
                    Console.WriteLine(_viewModel.GetTranslation("InvalidInput"));
                    Console.ReadLine();
                }
            }
        }

        // Displays the backup work management menu.
        public void DisplayBackupWorkMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"===== {_viewModel.GetTranslation("AppTitle")} =====\n");
                Console.WriteLine($"=== {_viewModel.GetTranslation("BackupWorkMenu")} ===\n");
                Console.WriteLine("1 : " + _viewModel.GetTranslation("AddBackup"));
                Console.WriteLine("2 : " + _viewModel.GetTranslation("DeleteBackup"));
                Console.WriteLine("3 : " + _viewModel.GetTranslation("ListBackup"));
                Console.WriteLine("4 : " + _viewModel.GetTranslation("Back"));
                Console.Write("\n" + _viewModel.GetTranslation("Choice") + " ");

                // Get user input and validate it.
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 4)
                {
                    switch (choice)
                    {
                        case 1:
                            CreateWork();
                            break;
                        case 2:
                            DeleteWork();
                            break;
                        case 3:
                            ListWorks();
                            Console.WriteLine("\n" + _viewModel.GetTranslation("PressEnter"));
                            Console.ReadLine();
                            break;
                        case 4:
                            return;
                    }
                }
                else
                {
                    Console.WriteLine(_viewModel.GetTranslation("InvalidInput"));
                    Console.ReadLine();
                }
            }
        }

        // Displays the execution management menu.
        public void DisplayExecutionMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"===== {_viewModel.GetTranslation("AppTitle")} =====\n");
                Console.WriteLine($"=== {_viewModel.GetTranslation("ExecutionMenu")} ===\n");
                Console.WriteLine("1 : " + _viewModel.GetTranslation("ExecuteOne"));
                Console.WriteLine("2 : " + _viewModel.GetTranslation("ExecuteAll"));
                Console.WriteLine("3 : " + _viewModel.GetTranslation("Back"));
                Console.Write("\n" + _viewModel.GetTranslation("Choice") + " ");

                // Get user input and validate it.
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 3)
                {
                    switch (choice)
                    {
                        case 1:
                            Console.Write(_viewModel.GetTranslation("EnterWorkIndexes") + " ");
                            Console.WriteLine("");
                            string cmd = Console.ReadLine();
                            _viewModel.ExecuteWork(cmd);
                            Console.ReadLine();
                            break;
                        case 2:
                            _viewModel.ExecuteAll();
                            Console.WriteLine(_viewModel.GetTranslation("AllExecuted"));
                            Console.ReadLine();
                            break;
                        case 3:
                            return;
                    }
                }
                else
                {
                    Console.WriteLine(_viewModel.GetTranslation("InvalidInput"));
                    Console.ReadLine();
                }
            }
        }

        // Displays the settings menu.
        public void DisplaySettingsMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"===== {_viewModel.GetTranslation("AppTitle")} =====\n");
                Console.WriteLine($"=== {_viewModel.GetTranslation("Settings")} ===\n");
                Console.WriteLine(_viewModel.GetTranslation("LanguageMode") + $" : {_viewModel.GetCurrentLanguage()}");
                Console.WriteLine("1 : " + _viewModel.GetTranslation("ChangeLanguage"));
                Console.WriteLine("2 : Change encripted extensions");
                Console.WriteLine("3 : " + _viewModel.GetTranslation("Back"));
                Console.Write("\n" + _viewModel.GetTranslation("Choice") + " ");

                // Get user input and validate it.
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 3)
                {
                    switch (choice)
                    {
                        case 1:
                            _viewModel.ToggleLanguage();
                            break;
                        case 2:
                            DisplayEncryptionSettings();
                            break;
                        case 3:
                            return;
                    }
                }
                else
                {
                    Console.WriteLine(_viewModel.GetTranslation("InvalidInput"));
                    Console.ReadLine();
                }
            }
        }

        public void DisplayEncryptionSettings()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"===== {_viewModel.GetTranslation("AppTitle")} =====\n");
                Console.WriteLine($"=== {_viewModel.GetTranslation("EncryptionSettings")} ===\n");
                Console.WriteLine($"Current Encrypted Extensions : {_viewModel.GetEncryptedExtensions()}");
                Console.WriteLine("1 : " + _viewModel.GetTranslation("Reset"));
                Console.WriteLine("2 : " + "Set extensions to encrypt");
                Console.WriteLine("3 : " + "Add extensions to encrypt");
                Console.WriteLine("4 : " + _viewModel.GetTranslation("Back"));
                Console.Write("\n" + _viewModel.GetTranslation("Choice") + " ");

                // Get user input and validate it.
                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 4)
                {
                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine("Confirm reset ? (Y/N) : ");
                            string? choice1 = Console.ReadLine();
                            if (choice1 == "Y" || choice1 == "y")
                                _viewModel.SetEncryptedExtensions("");
                            break;
                        case 2:
                            Console.WriteLine("Enter the extensions to encrypt (ex. .txt;.docx) : ");
                            string? extensions = Console.ReadLine();
                            Console.WriteLine($"Confirm Change from '{_viewModel.GetEncryptedExtensions()}' to '{extensions}' ? (Y/N) : ");
                            string? choice2 = Console.ReadLine();
                            if (choice2 == "Y" || choice2 == "y")
                                _viewModel.SetEncryptedExtensions(extensions);
                            break;
                        case 3:
                            Console.WriteLine("Enter the extensions to add (ex. .txt;.docx) : ");
                            string? extensions2 = Console.ReadLine();
                            Console.WriteLine($"Confirm Change from '{_viewModel.GetEncryptedExtensions()}' to '{_viewModel.GetEncryptedExtensions()};{extensions2}' ? (Y/N) : ");
                            string? choice3 = Console.ReadLine();
                            if (choice3 == "Y" || choice3 == "y")
                                _viewModel.AddEncryptedExtension(extensions2);
                            break;
                        case 4:
                            return;
                    }
                }
                else
                {
                    Console.WriteLine(_viewModel.GetTranslation("InvalidInput"));
                    Console.ReadLine();
                }
            }
        }

        // Creates a new backup work.
        private void CreateWork()
        {
            Console.Write(_viewModel.GetTranslation("EnterName") + " ");
            string? name = Console.ReadLine();
            Console.Write(_viewModel.GetTranslation("EnterSource") + " ");
            string? src = Console.ReadLine();
            Console.Write(_viewModel.GetTranslation("EnterDestination") + " ");
            string? dst = Console.ReadLine();
            Console.Write(_viewModel.GetTranslation("EnterType") + " ");
            string? typeInput = Console.ReadLine();

            // Validate input
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(src) || string.IsNullOrWhiteSpace(dst) || string.IsNullOrWhiteSpace(typeInput))
            {
                Console.WriteLine(_viewModel.GetTranslation("InvalidInputAllFields"));
                Console.ReadLine();
                return;
            }

            // Validate backup type
            if (!Enum.TryParse<BackupType>(typeInput, true, out BackupType type))
            {
                Console.WriteLine(_viewModel.GetTranslation("InvalidBackupType"));
                Console.ReadLine();
                return;
            }

            // Add the work
            _viewModel.AddWork(name, src, dst, type);
            Console.WriteLine(_viewModel.GetTranslation("PressEnter"));
            Console.ReadLine();
        }

        // Deletes a backup work.
        private void DeleteWork()
        {
            Console.Write(_viewModel.GetTranslation("EnterNameToDelete") + " ");
            string name = Console.ReadLine();
            // Validate input
            if (_viewModel.RemoveWork(name))
                Console.WriteLine(_viewModel.GetTranslation("BackupDeleted"));
            else
                Console.WriteLine(_viewModel.GetTranslation("BackupNotFound"));
            Console.WriteLine(_viewModel.GetTranslation("PressEnter"));
            Console.ReadLine();
        }

        // Lists all backup works.
        private void ListWorks()
        {
            // Get the list of works from the ViewModel and display them.
            var works = _viewModel.GetWorks();
            int i = 1;
            Console.WriteLine(_viewModel.GetTranslation("ListOfBackups") + "\n");
            foreach (var work in works)
            {
                Console.WriteLine($"[{i}] {work.Name} - {work.Src} -> {work.Dst} ({work.BackupType}) \n({_viewModel.GetTranslation("LastBackupUpdate")}{work.LastBackupDate})");
                i++;
            }
        }
    }
}
