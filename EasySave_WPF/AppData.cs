using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;


namespace EasySave_WPF.Model
{
    public static class AppData
    {
        public static Model Model { get; }
        public static ObservableCollection<State> States { get; } = new();

        static AppData()
        {
            Model = new Model();
            Model.LoadWorks(); // Chargement initial des travaux

            // Si besoin, charge aussi les états depuis le fichier
            LoadStates();
        }

        public static void LoadStates()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "state.json");
                if (!File.Exists(path)) return;

                string json = File.ReadAllText(path);
                var wrapper = System.Text.Json.JsonSerializer.Deserialize<StateWrapper>(json);

                if (wrapper?.States != null)
                {
                    States.Clear();
                    foreach (var state in wrapper.States)
                        States.Add(state);
                }
            }
            catch
            {
                // Tu peux logguer une erreur ici
            }
        }

        private class StateWrapper
        {
            public ObservableCollection<State> States { get; set; }
        }
    }
}
