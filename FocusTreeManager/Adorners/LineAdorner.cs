using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FocusTreeManager.Model;
using FocusTreeManager.Model.TabModels;

namespace FocusTreeManager.Adorners
{
    public class LineAdorner : Adorner
    {
        private readonly FrameworkElement element;

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

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException();
            return Children;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Child.Children.Clear();
            FocusGridModel context = DataContext as FocusGridModel;
            base.OnRender(drawingContext);
            if (context == null) return;
            foreach (CanvasLine line in context.CanvasLines)
            {
                Line newLine = new Line
                {
                    X1 = line.X1,
                    X2 = line.X2,
                    Y1 = line.Y1,
                    Y2 = line.Y2,
                    Visibility = Visibility.Visible,
                    StrokeThickness = 2,
                    Stroke = line.Color
                };
                if (line.Dashes)
                {
                    newLine.StrokeDashArray = new DoubleCollection(new double[] { 2, 2});
                }
                Child.Children.Add(newLine);
            }
            if (context.selectedLine == null) return;
            ResourceDictionary resourceLocalization = new ResourceDictionary
            {
                Source = new Uri("/FocusTreeManager;component/Resources/Icons.xaml", UriKind.Relative)
            };
            Rectangle visualBoard = new Rectangle
            {
                Width = 25,
                Height = 25
            };
            //Canvas.SetLeft(visualBoard, Math.Min(context.selectedLine.X1,
            //                                context.selectedLine.X2) + 
            //                                Math.Abs(context.selectedLine.X2 - 
            //                                         context.selectedLine.X1) / 2 - 12.5);
            //Canvas.SetTop(visualBoard, Math.Min(context.selectedLine.Y1,
            //                               context.selectedLine.Y2) + 
            //                               Math.Abs(context.selectedLine.Y2 -
            //                                        context.selectedLine.Y1) / 2 - 12.5);
            Point Position = Mouse.GetPosition(this);
            Canvas.SetTop(visualBoard, Position.Y);
            Canvas.SetLeft(visualBoard, Position.X);
            VisualBrush vBrush = new VisualBrush
            {
                Visual = (Visual) resourceLocalization["appbar_scissor"]
            };
            visualBoard.Fill = vBrush;
            Child.Children.Add(visualBoard);
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
