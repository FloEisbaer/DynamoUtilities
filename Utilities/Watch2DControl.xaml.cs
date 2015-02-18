using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utilities
{
    public partial class Watch2DControl
    {
        private double xmin = 0;
        private double xmax = 6.5;

        public double ymin = -1.1;
        public double ymax = 1.1;
        private Polyline pl;

        public Watch2DControl()
        {
            InitializeComponent();
            //AddChart();
        }

        public void AddChart()
        {
            // Draw sine curve:
            pl = new Polyline { Stroke = Brushes.Black };
            for (int i = 0; i < 70; i++)
            {
                double x = i / 5.0;
                double y = Math.Sin(x);
                pl.Points.Add(CurvePoint(
                new Point(x, y)));
            }

            PlotCanvas.Children.Add(pl);
            // Draw cosine curve:
            pl = new Polyline
            {
                Stroke = Brushes.Black,
                StrokeDashArray = new DoubleCollection(
                    new double[] { 4, 3 })
            };
            for (int i = 0; i < 70; i++)
            {
                double x = i / 5.0;
                double y = Math.Cos(x);
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
