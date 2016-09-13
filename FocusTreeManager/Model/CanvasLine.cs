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
        private int x1;

        public int X1
        {
            get
            {
                return x1;
            }
            set
            {
                Set<int>(() => this.X1, ref this.x1, value);
            }
        }

        private int y1;

        public int Y1
        {
            get
            {
                return y1;
            }
            set
            {
                Set<int>(() => this.Y1, ref this.y1, value);
            }
        }

        private int x2;

        public int X2
        {
            get
            {
                return x2;
            }
            set
            {
                Set<int>(() => this.X2, ref this.x2, value);
            }
        }

        private int y2;

        public int Y2
        {
            get
            {
                return y2;
            }
            set
            {
                Set<int>(() => this.Y2, ref this.y2, value);
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

        public CanvasLine(int X1, int Y1, int X2, int Y2, System.Windows.Media.Brush Color, bool dashed, ISet set)
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
