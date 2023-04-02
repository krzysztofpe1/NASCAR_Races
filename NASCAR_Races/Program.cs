namespace NASCAR_Races
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            MainWindow mainWindow = new MainWindow();
            Application.Run(mainWindow);
            mainWindow.RunRace();
        }
    }
}