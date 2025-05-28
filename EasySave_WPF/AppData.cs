namespace EasySave_WPF.Model
{
    public static class AppData
    {
        public static Model Model { get; }

        static AppData()
        {
            Model = new Model();
            Model.LoadWorks(); // Chargement des travaux au démarrage
            // Tu peux ajouter ici d'autres chargements si besoin
        }
    }
}
