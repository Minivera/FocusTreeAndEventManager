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

namespace FocusTreeManager
{
    class LineAdorner : Adorner
    {
        private FrameworkElement element;

        private new FocusGridViewModel DataContext;

        public LineAdorner(UIElement el, FocusGridViewModel DataContext) : base(el)
        {
            element = el as FrameworkElement;
            this.DataContext = DataContext;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            foreach (CanvasLine line in DataContext.CanvasLines)
            {
                Pen LinePen = new Pen(line.Color, 2);
                if (line.Dashes)
                {
                    LinePen.DashStyle = DashStyles.Dash;
                }
                drawingContext.DrawLine(LinePen, 
                    new Point(line.X1, line.Y1), 
                    new Point(line.X2, line.Y2));
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            IInputElement element = e.Source as IInputElement;
            base.OnMouseRightButtonDown(e);
            Point Position = e.GetPosition(element);
            List<CanvasLine> clickedElements = DataContext.CanvasLines.Where((line) => 
                                            inRange(line.X1, line.X2, (int)Position.X) &&
                                            inRange(line.Y1, line.Y2, (int)Position.Y)).ToList();
            foreach (CanvasLine line in clickedElements)
            {
                line.InternalSet.DeleteSetRelations();
                //Make sur to add all lines linked to that set
                clickedElements.AddRange(DataContext.CanvasLines.Where((l) => l.InternalSet == line.InternalSet));
            }
            DataContext.CanvasLines = new ObservableCollection<CanvasLine>(
                                      DataContext.CanvasLines.Except(clickedElements).ToList());
            DataContext.DrawOnCanvas();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            //TODO: Draw something over a line
            
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            //TODO: Remove the drawed something
        }

        private bool inRange(int Range1, int Range2, int Value)
        {
            int smallest = Math.Min(Range1, Range2);
            int highest = Math.Max(Range1, Range2);
            return ((smallest - 1 <= Value && Value <= highest - 1) ||
                    (smallest <= Value && Value <= highest) ||
                    (smallest + 1 <= Value && Value <= highest + 1));
        }
    }
}
