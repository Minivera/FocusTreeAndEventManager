using FocusTreeManager.Model;
using FocusTreeManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace FocusTreeManager
{
    class LineAdorner : Adorner
    {
        private FrameworkElement element;

        private Canvas Children;

        public Canvas Child
        {
            get { return Children; }
            set
            {
                if (Children != null)
                {
                    RemoveVisualChild(Children);
                }
                Children = value;
                if (Children != null)
                {
                    AddVisualChild(Children);
                }
            }
        }

        public LineAdorner(UIElement el, FocusGridModel DataContext) : base(el)
        {
            element = el as FrameworkElement;
            this.DataContext = DataContext;
            Children = new Canvas();
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException();
            return Children;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Child.Children.Clear();
            FocusGridModel DataContext = this.DataContext as FocusGridModel;
            base.OnRender(drawingContext);
            foreach (CanvasLine line in DataContext.CanvasLines)
            {
                //Pen LinePen = new Pen(line.Color, 2);
                Line newLine = new Line()
                {
                    X1 = line.X1,
                    X2 = line.X2,
                    Y1 = line.Y1,
                    Y2 = line.Y2,
                    Visibility = System.Windows.Visibility.Visible,
                    StrokeThickness = 2,
                    Stroke = line.Color
                };
                if (line.Dashes)
                {
                    newLine.StrokeDashArray = new DoubleCollection(new double[] { 2, 2});
                    //LinePen.DashStyle = DashStyles.Dash;
                }
                //drawingContext.DrawLine(LinePen, 
                //    new Point(line.X1, line.Y1), 
                //    new Point(line.X2, line.Y2));
                Child.Children.Add(newLine);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double desiredWidth = element.ActualWidth;
            double desiredHeight = element.ActualHeight;
            Child.Arrange(new Rect(0, 0, desiredWidth, desiredHeight));
            return finalSize;
        }
    }
}
