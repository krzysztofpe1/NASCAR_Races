namespace NASCAR_Races_Server
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            MainServerWindow mainWindow = new MainServerWindow();
            Application.Run(mainWindow);
        }
    }
}