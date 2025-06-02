using NS_Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace RemoteConsoleApp
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<State> States { get; set; } = new();

        private TcpClient commandClient;
        private TcpClient dataClient;
        private StreamWriter writer;
        private StreamReader reader;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            _ = Task.Run(ConnectAndReceiveLoop);
        }

        private async Task ConnectAndReceiveLoop()
        {
            while (true)
            {
                try
                {
                    commandClient?.Close();
                    commandClient = new TcpClient("127.0.0.1", 8889);
                    writer = new StreamWriter(commandClient.GetStream()) { AutoFlush = true };

                    dataClient?.Close();
                    dataClient = new TcpClient("127.0.0.1", 8888);
                    reader = new StreamReader(dataClient.GetStream());

                    await ReceiveUpdates(); 
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }
        }

        private async Task ReceiveUpdates()
        {
            while (true)
            {
                string? line;
                try
                {
                    line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) throw new IOException("Fin de flux ou ligne vide");

                    var container = JsonSerializer.Deserialize<StateFileContainer>(line);
                    if (container?.States != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            States.Clear();
                            foreach (var state in container.States)
                            {
                                States.Add(state);
                            }
                        });
                    }
                }
                catch
                {
                    break; 
                }
            }
        }

        private void SendCommand(string name, string command)
        {
            try
            {
                if (writer != null)
                {
                    var cmd = new RemoteCommand
                    {
                        Command = command,
                        Name = name
                    };

                    string json = JsonSerializer.Serialize(cmd);
                    writer.WriteLine(json);
                }
                else
                {
                    MessageBox.Show("Connexion non prête. Veuillez patienter.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'envoi : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is State s)
                SendCommand(s.Name, "pause");
        }

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is State s)
                SendCommand(s.Name, "resume");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is State s)
                SendCommand(s.Name, "cancel");
        }
    }

    public class StateFileContainer
    {
        public List<State> States { get; set; } = new();
    }

    public class RemoteCommand
    {
        public string Command { get; set; }
        public string Name { get; set; }
    }
}