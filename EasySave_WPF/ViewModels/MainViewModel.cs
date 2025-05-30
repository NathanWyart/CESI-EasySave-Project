using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EasySave_WPF.View;
using EasySave_WPF.Commands;

namespace EasySave_WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ICommand MaCommande { get; }

        // Propriétés pour l’interface graphique
        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        // Commandes de navigation
        public ICommand ShowWorkListCommand { get; }
        public ICommand ShowWorkStateCommand { get; }
        public ICommand ShowSettingsCommand { get; }
        public ICommand ShowBusinessSoftwareCommand { get; } // ✅ nouvelle commande

        // Constructeur
        public MainViewModel()
        {
            ShowWorkListCommand = new RelayCommand(_ => CurrentPage = new WorkListPage());
            ShowWorkStateCommand = new RelayCommand(_ => CurrentPage = new WorkStatePage());
            ShowSettingsCommand = new RelayCommand(_ => CurrentPage = new SettingsPage());
            ShowBusinessSoftwareCommand = new RelayCommand(_ => CurrentPage = new BusinessSoftwarePage()); // ✅ logique d'affichage

            // Page d’accueil par défaut
            CurrentPage = new WorkListPage();
        }

        // Implémentation de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
