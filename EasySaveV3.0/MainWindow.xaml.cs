using System.Windows;
using EasySave.View;
using EasySave.ViewModels;
using EasySaveV3._0.EasySave.ThreadManagement;
using EasySave.Network; 

namespace EasySaveV3._0
{
    public partial class MainWindow : Window
    {
        private readonly SocketServer _socketServer = new(); 

        public MainWindow()
        {
            InitializeComponent();

            _socketServer.Start();

            BusinessSoftwareWatcher.Start();

            BusinessSoftwareWatcher.OnBusinessSoftwarePaused += () =>
            {
                if (!BusinessSignals.AnyRunning())
                    return;

                BusinessSignals.PauseAll();
                MessageBox.Show("Un logiciel métier a été détecté. Les sauvegardes sont en pause.", "Logiciel métier détecté", MessageBoxButton.OK, MessageBoxImage.Warning);
            };

            BusinessSoftwareWatcher.OnBusinessSoftwareResumed += () =>
            {
                BusinessSignals.ResumeAll();
                MessageBox.Show("Les logiciels métiers ont été fermés. Les sauvegardes peuvent reprendre.", "Reprise des sauvegardes", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            ContentFrame.Content = new HomePage
            {
                DataContext = new HomePageViewModel()
            };
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new HomePage
            {
                DataContext = new HomePageViewModel()
            };
        }

        private void ExecutionButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new ExecutionPage
            {
                DataContext = new ExecutionViewModel()
            };
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Content = new SettingsPage
            {
                DataContext = new SettingsViewModel()
            };
        }
    }
}