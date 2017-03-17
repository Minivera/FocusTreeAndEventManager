using GalaSoft.MvvmLight;

namespace FocusTreeManager.Model
{
    public class CanvasLine : ObservableObject
    {
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

        private System.Windows.Media.Brush color;

        public System.Windows.Media.Brush Color
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
            double Y2, System.Windows.Media.Brush Color, bool dashed, 
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
    }
}
