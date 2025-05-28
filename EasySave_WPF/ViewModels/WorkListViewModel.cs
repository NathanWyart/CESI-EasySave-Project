using EasySave_WPF.Commands;
using EasySave_WPF.Model;
using EasySave_WPF.View;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace EasySave_WPF.ViewModels
{
    public class WorkListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Work> Works { get; }

        // Champs pour formulaire d'ajout
        public string WorkName { get => _workName; set { _workName = value; OnPropertyChanged(); } }
        public string Source { get => _source; set { _source = value; OnPropertyChanged(); } }
        public string Destination { get => _destination; set { _destination = value; OnPropertyChanged(); } }
        public string BackupType { get => _backupType; set { _backupType = value; OnPropertyChanged(); } }

        private string _workName;
        private string _source;
        private string _destination;
        private string _backupType;

        private Window _popupWindow;

        // Commandes
        public ICommand OpenAddPopupCommand { get; }
        public ICommand ConfirmAddCommand { get; }
        public ICommand CancelAddCommand { get; }
        public ICommand DeleteWorkCommand { get; }
        public ICommand ExecuteWorkCommand { get; }
        public ICommand ExecuteAllCommand { get; }
        public ICommand DeleteAllCommand { get; }

        public WorkListViewModel()
        {
            Works = new ObservableCollection<Work>(AppData.Model.Works);

            OpenAddPopupCommand = new RelayCommand(_ => OpenAddPopup());
            ConfirmAddCommand = new RelayCommand(_ => ConfirmAdd());
            CancelAddCommand = new RelayCommand(_ => CancelAdd());

            DeleteWorkCommand = new RelayCommand(workObj =>
            {
                if (workObj is Work work)
                {
                    var result = MessageBox.Show($"Supprimer le travail '{work.Name}' ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        Works.Remove(work);
                        AppData.Model.Works.Remove(work);
                        AppData.Model.SaveWorks();
                    }
                }
            });

            ExecuteWorkCommand = new RelayCommand(workObj =>
            {
                if (workObj is Work work)
                {
                    // Simulation de l'exécution
                    MessageBox.Show($"[SIMULATION] Exécution du travail : {work.Name}");
                }
            });

            ExecuteAllCommand = new RelayCommand(_ =>
            {
                if (Works.Count == 0)
                {
                    MessageBox.Show("Aucun travail à exécuter.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                foreach (var work in Works)
                {
                    MessageBox.Show($"[SIMULATION] Exécution de : {work.Name}");
                }
            });

            DeleteAllCommand = new RelayCommand(_ =>
            {
                if (Works.Count == 0)
                    return;

                var result = MessageBox.Show("Supprimer tous les travaux ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Works.Clear();
                    AppData.Model.Works.Clear();
                    AppData.Model.SaveWorks();
                }
            });
        }

        private void OpenAddPopup()
        {
            ResetForm();

            var view = new AddWorkControl { DataContext = this };

            _popupWindow = new Window
            {
                Title = "Ajouter un travail",
                Content = view,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize
            };

            _popupWindow.ShowDialog();
        }

        private void ConfirmAdd()
        {
            if (!Enum.TryParse<BackupType>(BackupType, out var type))
            {
                MessageBox.Show("Type de sauvegarde invalide.");
                return;
            }

            var work = new Work(WorkName, Source, Destination, type);
            AppData.Model.AddWork(work.Name, work.Src, work.Dst, work.BackupType);
            AppData.Model.SaveWorks();
            Works.Add(work);

            _popupWindow?.Close();
        }

        private void CancelAdd()
        {
            _popupWindow?.Close();
        }

        private void ResetForm()
        {
            WorkName = "";
            Source = "";
            Destination = "";
            BackupType = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
