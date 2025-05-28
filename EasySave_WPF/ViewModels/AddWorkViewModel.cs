using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EasySave_WPF.Commands;

namespace EasySave_WPF.ViewModels
{
    public class AddWorkViewModel : INotifyPropertyChanged
    {
        public event EventHandler Confirmed;
        public event EventHandler Canceled;

        private string _workName;
        public string WorkName
        {
            get => _workName;
            set { _workName = value; OnPropertyChanged(); }
        }

        private string _source;
        public string Source
        {
            get => _source;
            set { _source = value; OnPropertyChanged(); }
        }

        private string _destination;
        public string Destination
        {
            get => _destination;
            set { _destination = value; OnPropertyChanged(); }
        }

        private string _backupType;
        public string BackupType
        {
            get => _backupType;
            set { _backupType = value; OnPropertyChanged(); }
        }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public AddWorkViewModel()
        {
            ConfirmCommand = new RelayCommand(_ => Confirmed?.Invoke(this, EventArgs.Empty));
            CancelCommand = new RelayCommand(_ => Canceled?.Invoke(this, EventArgs.Empty));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
