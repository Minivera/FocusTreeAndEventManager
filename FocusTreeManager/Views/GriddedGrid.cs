using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FocusTreeManager.Views
{
    public class GriddedGrid : Grid
    {
        #region GridLinesVisibilityEnum
        public enum GridLinesVisibilityEnum
        {
            Both,
            Vertical,
            Horizontal,
            None
        }
        #endregion


        #region Properties
        public bool ShowCustomGridLines
        {
            get { return (bool)GetValue(ShowCustomGridLinesProperty); }
            set { SetValue(ShowCustomGridLinesProperty, value); }
        }

        public static readonly DependencyProperty ShowCustomGridLinesProperty =
            DependencyProperty.Register("ShowCustomGridLines", typeof(bool), typeof(GriddedGrid), new UIPropertyMetadata(false));

        public GridLinesVisibilityEnum GridLinesVisibility
        {
            get { return (GridLinesVisibilityEnum)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        public static readonly DependencyProperty GridLinesVisibilityProperty =
            DependencyProperty.Register("GridLinesVisibility", typeof(GridLinesVisibilityEnum), typeof(GriddedGrid), new UIPropertyMetadata(GridLinesVisibilityEnum.Both));

        public Brush GridLineBrush
        {
            get { return (Brush)GetValue(GridLineBrushProperty); }
            set { SetValue(GridLineBrushProperty, value); }
        }

        public static readonly DependencyProperty GridLineBrushProperty =
            DependencyProperty.Register("GridLineBrush", typeof(Brush), typeof(GriddedGrid), new UIPropertyMetadata(Brushes.Black));

        public double GridLineThickness
        {
            get { return (double)GetValue(GridLineThicknessProperty); }
            set { SetValue(GridLineThicknessProperty, value); }
        }

        public static readonly DependencyProperty GridLineThicknessProperty =
            DependencyProperty.Register("GridLineThickness", typeof(double), typeof(GriddedGrid), new UIPropertyMetadata(1.0));
        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            if (ShowCustomGridLines)
            {
                if (GridLinesVisibility == GridLinesVisibilityEnum.Both)
                {
                    foreach (var rowDefinition in RowDefinitions.Skip(1))
                    {
                        dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(0, rowDefinition.Offset), new Point(ActualWidth, rowDefinition.Offset));
                    }
                    foreach (var columnDefinition in ColumnDefinitions.Skip(1))
                    {
                        dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(columnDefinition.Offset, 0), new Point(columnDefinition.Offset, ActualHeight));
                    }
                    //dc.DrawRectangle(Brushes.Transparent, new Pen(GridLineBrush, GridLineThickness), new Rect(0, 0, ActualWidth, ActualHeight));
                }
                else if (GridLinesVisibility == GridLinesVisibilityEnum.Vertical)
                {
                    foreach (var columnDefinition in ColumnDefinitions)
                    {
                        dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(columnDefinition.Offset, 0), new Point(columnDefinition.Offset, ActualHeight));
                    }
                    dc.DrawRectangle(Brushes.Transparent, new Pen(GridLineBrush, GridLineThickness), new Rect(0, 0, ActualWidth, ActualHeight));
                }
                else if (GridLinesVisibility == GridLinesVisibilityEnum.Horizontal)
                {
                    foreach (var rowDefinition in RowDefinitions)
                    {
                        dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(0, rowDefinition.Offset), new Point(ActualWidth, rowDefinition.Offset));
                    }
                    dc.DrawRectangle(Brushes.Transparent, new Pen(GridLineBrush, GridLineThickness), new Rect(0, 0, ActualWidth, ActualHeight));
                }

            }
            base.OnRender(dc);
        }

        static GriddedGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GriddedGrid), new FrameworkPropertyMetadata(typeof(GriddedGrid)));
        }
    }
}
