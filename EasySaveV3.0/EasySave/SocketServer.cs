using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using EasySaveV3._0.EasySave.ThreadManagement;
using NS_Model;

namespace EasySave.Network
{
    public class RemoteCommand
    {
        public string Command { get; set; }
        public string Name { get; set; }
    }

    public class SocketServer
    {
        private TcpListener _listenerState;
        private TcpListener _listenerCommand;
        private CancellationTokenSource _cts = new();

        public void Start(int statePort = 8888, int commandPort = 8889)
        {
            _listenerState = new TcpListener(IPAddress.Any, statePort);
            _listenerCommand = new TcpListener(IPAddress.Any, commandPort);

            _listenerState.Start();
            _listenerCommand.Start();

            _ = Task.Run(() => AcceptClients(_listenerState, HandleStateClient));
            _ = Task.Run(() => AcceptClients(_listenerCommand, HandleCommandClient));
        }

        private async Task AcceptClients(TcpListener listener, Func<TcpClient, Task> handler)
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    var client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine($"Client connecté sur le port {((IPEndPoint)listener.LocalEndpoint).Port}");
                    _ = Task.Run(() => handler(client));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur AcceptTcpClientAsync : {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            _cts.Cancel();
            _listenerState?.Stop();
            _listenerCommand?.Stop();
        }

        private async Task HandleStateClient(TcpClient client)
        {
            var stream = client.GetStream();
            var writer = new StreamWriter(stream) { AutoFlush = true };

            try
            {
                while (true)
                {
                    if (File.Exists(Model.AppPaths.StatePath))
                    {
                        string stateJson = File.ReadAllText(Model.AppPaths.StatePath);
                        string oneLine = stateJson.Replace("\n", "").Replace("\r", "");
                        await writer.WriteLineAsync(oneLine);
                    }
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur d'envoi vers client (état) : " + ex.Message);
                client.Close();
            }
        }

        private async Task HandleCommandClient(TcpClient client)
        {
            var stream = client.GetStream();
            var reader = new StreamReader(stream);

            try
            {
                while (true)
                {
                    string cmdLine = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(cmdLine)) continue;

                    Console.WriteLine("Commande reçue : " + cmdLine);

                    var cmd = JsonSerializer.Deserialize<RemoteCommand>(cmdLine);
                    if (cmd == null) continue;

                    switch (cmd.Command.ToLower())
                    {
                        case "pause":
                            if (ThreadSignals.Pausers.TryGetValue(cmd.Name, out var p))
                            {
                                p.Reset();
                                new Model().UpdateStateStatusOnly(cmd.Name, "PAUSE");
                            }
                            break;
                        case "resume":
                            if (ThreadSignals.Pausers.TryGetValue(cmd.Name, out var r))
                            {
                                r.Set();
                                new Model().UpdateStateStatusOnly(cmd.Name, "ACTIVE");
                            }
                            break;
                        case "cancel":
                            if (ThreadSignals.Cancellers.TryGetValue(cmd.Name, out var c))
                            {
                                c.Cancel();
                                new Model().UpdateStateStatusOnly(cmd.Name, "ANNULE");
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur de lecture commande : " + ex.Message);
                client.Close();
            }
        }
    }
}