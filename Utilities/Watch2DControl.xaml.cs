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
        private List<double> _Values;

        private double xmin = 0;
        private double xmax;
        private double ymin = 0;
        private double ymax;

        private Polyline pl;

        public List<double> Values
        {
            get { return _Values; }
            set { _Values = value; }
        }

        public Watch2DControl()
        {
            InitializeComponent();
            _Values = new List<double>() {2.0, 5.0, 3.0};
            //AddChart();
        }

        public void AddChart()
        {
            PlotCanvas.Children.Clear();
            // Draw sine curve:
            pl = new Polyline { Stroke = Brushes.Black };

            xmax = _Values.Count - 1;
            foreach (var xValue in _Values)
            {
                if (xValue > ymax) ymax = xValue;
            }

            for (int i = 0; i <= xmax; i++)
            {
                double x = i;
                double y = _Values[i];
                pl.Points.Add(CurvePoint(
                new Point(x, y)));
            }

            PlotCanvas.Children.Add(pl);
        }

        private Point CurvePoint(Point pt)
        {
            var result = new Point
            {
                X = (pt.X - xmin) * PlotCanvas.Width / (xmax - xmin),
                Y = PlotCanvas.Height - (pt.Y - ymin) * PlotCanvas.Height
                    / (ymax - ymin)
            };
            return result;
        }
    }

}
