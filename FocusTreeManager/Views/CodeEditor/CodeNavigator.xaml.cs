using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        private readonly DrawingVisual drawing = new DrawingVisual();

        private double TextHeight;

        private bool drag;

        private Point startPt;

        private double lastTop;

        private double CanvasTop;

        public CodeNavigator(FormattedText formattedText, Point textPos)
        {
            InitializeComponent();
            DrawingContext dc = drawing.RenderOpen();
            dc.DrawText(formattedText, textPos);
            dc.Close();
            if (drawing.ContentBounds.IsEmpty) return;
            TextHeight = formattedText.Height;
            VisualRealSize = new Size(formattedText.Width, drawing.ContentBounds.Height);
            //Set properties to make them exist
            NavigatorRectangle.SetValue(Canvas.TopProperty, 0.0);
            NavigatorRectangle.SetValue(Canvas.LeftProperty, 0.0);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!drawing.ContentBounds.IsEmpty)
            {
                setVisuals();
            }
        }

        private void setVisuals()
        {
            //Height
            relativeScrollingY = LinkedScrollViewerHeight / VisualRealSize.Height > 0.7
                                      ? 0.7 : LinkedScrollViewerHeight / VisualRealSize.Height;
            relativeHeightTransform = ActualHeight / VisualRealSize.Height > 0.7
                                    ? 0.7 : ActualHeight / VisualRealSize.Height;
            VisualRectangle.Height = VisualRealSize.Height * relativeHeightTransform;
            //Width
            double relativeWidthTransform = NavigatorCanvas.Width / VisualRealSize.Width > 1
                                      ? 1 : NavigatorCanvas.Width / VisualRealSize.Width;
            //Check if rectangle must be shown
            if (ActualHeight < TextHeight)
            {
                NavigatorRectangle.Height = (relativeScrollingY * VisualRealSize.Height) *
                    relativeHeightTransform;
            }
            else
            {
                NavigatorRectangle.Height = 0;
            }
            //Transform and add the visuals
            ScaleTransform transform = new ScaleTransform(Math.Round(relativeWidthTransform, 2),
                                                          Math.Round(relativeHeightTransform, 2));
            VisualBrush brush = new VisualBrush(drawing)
            {
                Transform = transform,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                TileMode = TileMode.None,
                Viewport = new Rect(0, 0, NavigatorCanvas.Width, VisualRectangle.Height),
                Stretch = Stretch.None
            };
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
            if (drawing.ContentBounds.IsEmpty) return;
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
            lastTop = CanvasTop;
            Mouse.Capture(NavigatorRectangle);
        }

        private void Border_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!drag) return;
            double newY = e.GetPosition(NavigatorCanvas).Y;
            double offset = startPt.Y - lastTop;
            CanvasTop = Math.Max(0, Math.Min(newY - offset, NavigatorCanvas.Height 
                - NavigatorRectangle.Height));
            NavigatorRectangle.SetValue(Canvas.TopProperty, CanvasTop);
            ScrollMethod(CanvasTop / relativeScrollingY);
        }

        private void Border_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            drag = false;
            Cursor = Cursors.Arrow;
            Mouse.Capture(null);
        }

        private void VisualRectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double MouseY = e.GetPosition(NavigatorCanvas).Y;
            CanvasTop = Math.Max(0, Math.Min(MouseY, NavigatorCanvas.Height 
                - NavigatorRectangle.Height));
            NavigatorRectangle.SetValue(Canvas.TopProperty, CanvasTop);
            ScrollMethod(CanvasTop / relativeScrollingY);
        }
    }
}
