using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FocusTreeManager.Views.CodeEditor
{
    public partial class CodeNavigator : UserControl
    {
        public double LinkedScrollViewerHeight { get; set; }

        public delegate void ScrollDelegate(double verticalOffset);

        public ScrollDelegate ScrollMethod { get; set; }

        private Size VisualRealSize;

        private double relativeHeightTransform;

        private double relativeScrollingY;

        private DrawingVisual drawing = new DrawingVisual();

        private double TextHeight;

        private bool drag = false;

        private Point startPt;

        private int hei;

        private double lastTop;

        private double CanvasTop;

        public CodeNavigator(FormattedText formattedText, Point textPos)
        {
            InitializeComponent();
            DrawingContext dc = drawing.RenderOpen();
            dc.DrawText(formattedText, textPos);
            dc.Close();
            TextHeight = formattedText.Height;
            VisualRealSize = new Size(formattedText.Width, drawing.ContentBounds.Height);
            //Set properties to make them exist
            NavigatorRectangle.SetValue(Canvas.TopProperty, 0.0);
            NavigatorRectangle.SetValue(Canvas.LeftProperty, 0.0);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            setVisuals();
        }

        private void setVisuals()
        {
            //Height
            relativeScrollingY = LinkedScrollViewerHeight / VisualRealSize.Height > 0.7
                                      ? 0.7 : LinkedScrollViewerHeight / VisualRealSize.Height;
            double heightToUse = ActualHeight < TextHeight ? ActualHeight : TextHeight;
            relativeHeightTransform = ActualHeight / VisualRealSize.Height > 0.7
                                      ? 0.7 : ActualHeight / VisualRealSize.Height;
            VisualRectangle.Height = VisualRealSize.Height * relativeHeightTransform;
            NavigatorRectangle.Height = (relativeScrollingY * VisualRealSize.Height) * relativeHeightTransform;
            //Width
            double relativeWidthTransform = NavigatorCanvas.Width / VisualRealSize.Width > 1
                                      ? 1 : NavigatorCanvas.Width / VisualRealSize.Width;
            //Transform and add the visuals
            ScaleTransform transform = new ScaleTransform(Math.Round(relativeWidthTransform, 2),
                                                          Math.Round(relativeHeightTransform, 2));
            VisualBrush brush = new VisualBrush(drawing);
            brush.Transform = transform;
            brush.AlignmentX = AlignmentX.Left;
            brush.AlignmentY = AlignmentY.Top;
            brush.TileMode = TileMode.None;
            brush.Viewport = new Rect(0, 0, NavigatorCanvas.Width, VisualRectangle.Height);
            brush.Stretch = Stretch.None;
            VisualRectangle.Fill = brush;
            //Set the current scrolling
            CanvasTop = (double)NavigatorRectangle.GetValue(Canvas.TopProperty);
            CanvasTop = CanvasTop * relativeScrollingY;
            NavigatorRectangle.SetValue(Canvas.TopProperty, CanvasTop);
        }

        public void UpdateText(FormattedText formattedText, Point textPos, double verticalOffset)
        {
            DrawingContext dc = drawing.RenderOpen();
            dc.DrawText(formattedText, textPos);
            dc.Close();
            TextHeight = formattedText.Height;
            VisualRealSize = new Size(formattedText.Width, drawing.ContentBounds.Height);
            NavigatorRectangle.SetValue(Canvas.TopProperty, verticalOffset);
            setVisuals();
        }

        public void setScrolling(double verticalOffset)
        {
            CanvasTop = verticalOffset * relativeScrollingY;
            NavigatorRectangle.SetValue(Canvas.TopProperty, CanvasTop);
        }

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            drag = true;
            Cursor = Cursors.Hand;
            startPt = e.GetPosition(NavigatorCanvas);
            hei = (int)NavigatorRectangle.Height;
            lastTop = CanvasTop;
            Mouse.Capture(NavigatorRectangle);
        }

        private void Border_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                var newY = e.GetPosition(NavigatorCanvas).Y;
                double offset = startPt.Y - lastTop;
                CanvasTop = Math.Max(0, Math.Min(newY - offset, NavigatorCanvas.Height - NavigatorRectangle.Height));
                NavigatorRectangle.SetValue(Canvas.TopProperty, CanvasTop);
                ScrollMethod(CanvasTop / relativeScrollingY);
            }
        }

        private void Border_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            drag = false;
            Cursor = Cursors.Arrow;
            Mouse.Capture(null);
        }

        private void VisualRectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var MouseY = e.GetPosition(NavigatorCanvas).Y;
            double plannedPos = MouseY - (NavigatorRectangle.Height / 2);
            CanvasTop = Math.Max(0, Math.Min(MouseY, NavigatorCanvas.Height - NavigatorRectangle.Height));
            NavigatorRectangle.SetValue(Canvas.TopProperty, CanvasTop);
            ScrollMethod(CanvasTop / relativeScrollingY);
        }
    }
}
