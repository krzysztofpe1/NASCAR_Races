using System.Windows.Forms;
using System.Threading;

namespace NASCAR_Races
{
    public partial class MainWindow : Form
    {
        Painter painter;
        RaceManager raceManager;
        List<Car> listOfCars;
        public MainWindow()
        {
            InitializeComponent();
            int maxX = mainPictureBox.Width;
            int maxY = mainPictureBox.Height;
            int straightLength = maxX / 2;
            int turnRadius = maxX / 5;
            int pitPosY = maxY / 2 + maxX / 7;
            int turnCurveRadius = 0;
            //int totalLength = (int)(maxX + 2 * 3.1415 * turnRadius);
            raceManager = new(straightLength, turnRadius, pitPosY, turnCurveRadius);
            painter = new(maxX, maxY, straightLength, turnRadius, pitPosY);

            int numberOfCars = 1;
            listOfCars = RaceManager.CreateListOfCars(numberOfCars);
            programTimer.Interval = 10;//Interval of Timer executing event "Tick" (in milliseconds)
            programTimer.Tick += new EventHandler(RunRace);
            programTimer.Start();
        }

        //metoda odœwie¿ania ekranu, wywo³ywana automatycznie, gdy system uwa¿a, ¿e nale¿y j¹ wywo³aæ.
        //Alternatywnie u¿yæ: mainPictureBox.Invalidate() w razie gdyby by³o potrzebne
        private void mainPictureBox_Paint(object sender, PaintEventArgs e)
        {
            painter.PaintCircuit(e.Graphics);
            painter.PaintCarsPosition(e.Graphics, listOfCars);
        }

        internal void RunRace(object sender, EventArgs e)
        {
            RaceManager.MoveCars(listOfCars);
            mainPictureBox.Invalidate();
        }

    }
}