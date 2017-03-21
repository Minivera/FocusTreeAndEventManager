using System;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace FocusTreeManager.Model
{
    public class CanvasLine : ObservableObject
    {
        private const int SELECTION_FUZZINESS = 5;

        private double x1;

        public double X1
        {
            get
            {
                return x1;
            }
            set
            {
                Set(() => X1, ref x1, value);
            }
        }

        private double y1;

        public double Y1
        {
            get
            {
                return y1;
            }
            set
            {
                Set(() => Y1, ref y1, value);
            }
        }

        private double x2;

        public double X2
        {
            get
            {
                return x2;
            }
            set
            {
                Set(() => X2, ref x2, value);
            }
        }

        private double y2;

        public double Y2
        {
            get
            {
                return y2;
            }
            set
            {
                Set(() => Y2, ref y2, value);
            }
        }

        private Brush color;

        public Brush Color
        {
            get
            {
                return color;
            }
            set
            {
                Set(() => Color, ref color, value);
            }
        }

        private bool dashes;

        public bool Dashes
        {
            get
            {
                return dashes;
            }
            set
            {
                Set(() => Dashes, ref dashes, value);
            }
        }

        public ISet InternalSet { get; private set; }

        public FocusModel Relative { get; private set; }

        public CanvasLine(double X1, double Y1, double X2, 
            double Y2, Brush Color, bool dashed, 
            ISet set, FocusModel relative = null)
        {
            this.X1 = X1;
            this.Y1 = Y1;
            this.X2 = X2;
            this.Y2 = Y2;
            this.Color = Color;
            dashes = dashed;
            InternalSet = set;
            Relative = relative;
        }

        public bool ContainsPoint(Point point)
        {
            LineGeometry lineGeo = new LineGeometry(new Point(X1, Y1), new Point(X2, Y2));
            Point leftPoint;
            Point rightPoint;
            // Normalize start/end to left right to make the offset calc simpler.
            if (lineGeo.StartPoint.X <= lineGeo.EndPoint.X)
            {
                leftPoint = lineGeo.StartPoint;
                rightPoint = lineGeo.EndPoint;
            }
            else
            {
                leftPoint = lineGeo.EndPoint;
                rightPoint = lineGeo.StartPoint;
            }
            // If point is out of bounds, no need to do further checks.                  
            if (point.X + SELECTION_FUZZINESS < leftPoint.X
                || rightPoint.X < point.X - SELECTION_FUZZINESS)
            {
                return false;

            }
            if (point.Y + SELECTION_FUZZINESS < Math.Min(leftPoint.Y, rightPoint.Y) ||
                Math.Max(leftPoint.Y, rightPoint.Y) < point.Y - SELECTION_FUZZINESS)
            {
                return false;
            }
            double deltaX = rightPoint.X - leftPoint.X;
            double deltaY = rightPoint.Y - leftPoint.Y;
            // If the line is straight, the earlier boundary check is enough
            // to determine that the point is on the line.
            // Also prevents division by zero exceptions.
            if (deltaX == 0 || deltaY == 0)
            {
                return true;
            }
            double slope = deltaY / deltaX;
            double offset = leftPoint.Y - leftPoint.X * slope;
            double calculatedY = point.X * slope + offset;
            // Check calculated Y matches the points Y coord with some easing.
            bool lineContains = point.Y - SELECTION_FUZZINESS 
                <= calculatedY && calculatedY <= point.Y + SELECTION_FUZZINESS;
            return lineContains;
        }
    }
}
