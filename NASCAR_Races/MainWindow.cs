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
            _raceManager = new(straightLength, _turnRadius, pitPosY, turnCurveRadius, mainPictureBox);
            _painter = new(maxX, maxY, straightLength, _turnRadius, pitPosY);

            int numberOfCars = 1;
            _painter.listOfCars = _raceManager.CreateListOfCars(numberOfCars);

            programTimer.Interval = 1;//Interval of Timer executing event "Tick" (in milliseconds)
            programTimer.Tick += new EventHandler(RunRace);
            programTimer.Start();
            _raceManager.StartRace();
        }

        //metoda od�wie�ania ekranu, wywo�ywana automatycznie, gdy system uwa�a, �e nale�y j� wywo�a�.
        //Alternatywnie u�y�: mainPictureBox.Invalidate() w razie gdyby by�o potrzebne
        private void mainPictureBox_Paint(object sender, PaintEventArgs e)
        {
            _painter.PaintCircuit(e.Graphics);
            _painter.PaintCarsPosition(e.Graphics);
        }

        internal void RunRace(object sender, EventArgs e)
        {
            mainPictureBox.Invalidate();
        }

    }
}