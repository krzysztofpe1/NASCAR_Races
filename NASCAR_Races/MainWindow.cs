using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace NASCAR_Races
{
    public partial class MainWindow : Form
    {
        private Painter _painter;
        private RaceManager _raceManager;
        public MainWindow()
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
            _raceManager = new(straightLength, _turnRadius, pitPosY, turnCurveRadius, penCircuitSize, mainPictureBox);
            _painter = new(maxX, maxY, straightLength, _turnRadius, pitPosY, penCircuitSize);

            int numberOfCars = 5;
            _painter.listOfCars = _raceManager.CreateListOfCars(numberOfCars);

            programTimer.Interval = 1;//Interval of Timer executing event "Tick" (in milliseconds)
            programTimer.Tick += new EventHandler(RunRace);
            programTimer.Start();
            _raceManager.StartRace();
            this.FormClosing += MainWindowClosing_KillThreads;
        }

        //metoda odœwie¿ania ekranu, wywo³ywana automatycznie, gdy system uwa¿a, ¿e nale¿y j¹ wywo³aæ.
        //Alternatywnie u¿yæ: mainPictureBox.Invalidate() w razie gdyby by³o potrzebne
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
            _raceManager.KillThreads();
        }

    }
}