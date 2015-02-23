using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utilities
{
    public partial class Watch2DControl
    {
        private List<double> _values;

        private double xmin = 0;
        private double _xmax;
        private double ymin = 0;
        private double _ymax;

        private Polyline _pl;

        private int _selectedType;

        public List<double> Values
        {
            get { return _values; }
            set { _values = value; }
        }

        public int SelectedType
        {
            get { return _selectedType; }
            set { _selectedType = value; }
        }

        public Watch2DControl()
        {
            InitializeComponent();
            _values = new List<double>() {2.0, 5.0, 3.0};
            SelectedType = 0;
        }

        public void AddChart()
        {
            PlotCanvas.Children.Clear();

            if (_selectedType == 0)
            {
                DrawPlot();
            }
            else
            {
                //DrawHisto();
            }
            
        }

        private void DrawHisto()
        {
            var recWidth = PlotCanvas.Width/(double) Values.Count;

            var rectangle = new Rectangle();

            // Create a SolidColorBrush with a red color to fill the 
            // Ellipse with.
            var mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);

            rectangle.Height = Values[0];
            rectangle.Width = recWidth;

            PlotCanvas.Children.Add(rectangle);
        }

        private void DrawPlot()
        {
            _pl = new Polyline {Stroke = Brushes.Black};

            _xmax = _values.Count - 1;
            foreach (var xValue in _values)
            {
                if (xValue > _ymax) _ymax = xValue;
            }

            for (int i = 0; i <= _xmax; i++)
            {
                double x = i;
                double y = _values[i];
                _pl.Points.Add(CurvePoint(
                    new Point(x, y)));
            }

            PlotCanvas.Children.Add(_pl);
        }

        private Point CurvePoint(Point pt)
        {
            var result = new Point
            {
                X = (pt.X - xmin) * PlotCanvas.Width / (_xmax - xmin),
                Y = PlotCanvas.Height - (pt.Y - ymin) * PlotCanvas.Height
                    / (_ymax - ymin)
            };
            return result;
        }

        private void CanvasType_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _selectedType = CanvasType.SelectedIndex;
        }
    }

}
