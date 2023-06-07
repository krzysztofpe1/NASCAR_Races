using System.Reflection;
using NASCAR_Races_Server;

namespace Nascar_Races_Client
{
    internal partial class MainClientWindow : Form
    {
        private Painter _painter;
        private RaceManager _raceManager;

        public MainClientWindow()
        {
            InitializeComponent();
            int maxX = mainPictureBox.Width;
            int maxY = mainPictureBox.Height;
            int straightLength = maxX / 2;
            int _turnRadius = maxX / 5;
            int pitPosY = maxY / 2 + maxX / 7;
            int turnCurveRadius = 0;
            int totalLength = (int)(maxX + 2 * 3.1415 * _turnRadius);
            int penCircuitSize = 60;
            int penCarSize = 1;
            _raceManager = new(straightLength, _turnRadius, pitPosY, turnCurveRadius, penCircuitSize, penCarSize, mainPictureBox);
            _raceManager.StartRace(); //TODO temporary
            _painter = new(_raceManager.WorldInformation);
            _painter.listOfCars = _raceManager.getCars();

            programTimer.Interval = 1;//Interval of Timer executing event "Tick" (in milliseconds)
            programTimer.Tick += new EventHandler(RunRace);
            programTimer.Start();
            this.FormClosing += MainWindowClosing_KillThreads;
        }
        private void mainPictureBox_Paint(object sender, PaintEventArgs e)
        {
            _painter.PaintCircuit(e.Graphics);
            _painter.PaintCarsPosition(e.Graphics);
        }

        private void RunRace(object sender, EventArgs e)
        {
            mainPictureBox.Invalidate();
        }

        private void MainWindowClosing_KillThreads(object sender, FormClosingEventArgs e)
        {
            _raceManager.EndRace();
            _raceManager.Dispose();
        }
    }
}