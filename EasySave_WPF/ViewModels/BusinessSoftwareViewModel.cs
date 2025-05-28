using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using EasySave_WPF.Commands;

namespace EasySave_WPF.ViewModels
{
    public class BusinessSoftwareViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> BusinessSoftwares { get; } = new();

        public ICommand AddSoftwareCommand { get; }
        public ICommand RemoveSoftwareCommand { get; }

        public BusinessSoftwareViewModel()
        {
            // Chargement fictif (à remplacer par lecture JSON si besoin)
            BusinessSoftwares.Add("Calculator.exe");
            BusinessSoftwares.Add("Photoshop.exe");

            AddSoftwareCommand = new RelayCommand(_ =>
            {
                var name = Microsoft.VisualBasic.Interaction.InputBox("Nom du logiciel métier :", "Ajouter", "");
                if (!string.IsNullOrWhiteSpace(name))
                {
                    BusinessSoftwares.Add(name);
                    // TODO : sauvegarder dans fichier JSON
                }
            });

            RemoveSoftwareCommand = new RelayCommand(nameObj =>
            {
                if (nameObj is string name)
                {
                    BusinessSoftwares.Remove(name);
                    // TODO : supprimer du fichier JSON
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
