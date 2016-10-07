using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Media;

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
                Set<double>(() => this.X1, ref this.x1, value);
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
                Set<double>(() => this.Y1, ref this.y1, value);
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
                Set<double>(() => this.X2, ref this.x2, value);
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
                Set<double>(() => this.Y2, ref this.y2, value);
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
                Set<System.Windows.Media.Brush>(() => this.Color, ref this.color, value);
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
                Set<bool>(() => this.Dashes, ref this.dashes, value);
            }
        }

        public ISet InternalSet { get; private set; }

        public CanvasLine(double X1, double Y1, double X2, double Y2, System.Windows.Media.Brush Color, bool dashed, ISet set)
        {
            this.X1 = X1;
            this.Y1 = Y1;
            this.X2 = X2;
            this.Y2 = Y2;
            this.Color = Color;
            this.dashes = dashed;
            InternalSet = set;
        }
    }
}
