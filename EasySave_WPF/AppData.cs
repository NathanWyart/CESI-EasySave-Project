using System.Collections.Generic;

namespace EasySave_WPF.Model
{
    public static class AppData
    {
        public static Model Model { get; }
        public static List<string> PriorityExtensions { get; private set; }

        static AppData()
        {
            Model = new Model();
            Model.LoadWorks(); // Chargement des travaux

            LoadPriorityExtensions(); // Chargement des extensions prioritaires
        }

        private static void LoadPriorityExtensions()
        {
            // Liste prédéfinie — pourra être chargée depuis un fichier JSON plus tard
            PriorityExtensions = new List<string>
            {
                ".pdf", ".docx", ".xlsx"
            };
        }
    }
}
