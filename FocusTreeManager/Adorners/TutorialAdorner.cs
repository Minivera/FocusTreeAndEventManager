using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using FocusTreeManager.Helper;
using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro;

namespace FocusTreeManager.Adorners
{
    public class TutorialAdorner : Adorner
    {
        private const double bestMargin = 20;

        private const double ButtonHeightAndMargin = 60;

        private const double bestTextHeight = 250;

        private const double TopBarHeight = 30;

        private const double MinTestWidth = 600;

        private readonly FrameworkElement element;

        private readonly VisualCollection Children;

        private Border border;

        private Rect BorderPositionning;

        public TutorialAdorner(FrameworkElement el, FrameworkElement parent) : base(parent)
        {
            element = el;
            DataContext = new ViewModelLocator().Tutorial;
            Children = new VisualCollection(this);
            IsHitTestVisible = true;
        }

        protected override int VisualChildrenCount => Children.Count;

        protected override Visual GetVisualChild(int index) { return Children[index]; }
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Children.Clear();
            TutorialViewModel context = DataContext as TutorialViewModel;
            FrameworkElement parent = UiHelper.FindVisualParent<Window>(element, null);
            if (context != null && context.InTutorial)
            {
                //Draw a big fat overlay
                SolidColorBrush brush = new SolidColorBrush
                {
                    Color = Brushes.DarkGray.Color,
                    Opacity = 0.7
                };
                FrameworkElement component = getComponentFromParent(parent, context);
                //If the component cannot be found, keep overlay and only draw the text
                if (component == null)
                {
                    drawingContext.DrawGeometry(brush, null, new RectangleGeometry(
                            new Rect(new Point(0, TopBarHeight),
                                     new Point(parent.Width, TopBarHeight + parent.Height))));
                    //Build text in the middle
                    BuildText(parent, context, new Rect(new Point(0, 0), 
                                  new Point(parent.ActualWidth, parent.ActualHeight)));
                    return;
                }
                Point Position = component.TranslatePoint(new Point(0, 0), parent);
                Rect ComponentSpace = new Rect(Position,
                            new Point(Position.X + component.ActualWidth, 
                                      Position.Y + component.ActualHeight));
                //Run stuff on element if needed
                RunOnElement(context, parent, component);
                //Add rectangle or sphere around the control
                if (context.CurrentStep.IsCircle)
                {
                    double radius = Math.Max(component.ActualWidth / 2, component.ActualHeight / 2);
                    CombinedGeometry group = new CombinedGeometry
                    {
                        GeometryCombineMode = GeometryCombineMode.Xor,
                        Geometry1 = new EllipseGeometry
                        {
                            RadiusX = radius,
                            RadiusY = radius,
                            Center = new Point(Position.X + component.ActualWidth / 2,
                                Position.Y + component.ActualHeight / 2)
                        },
                        Geometry2 = new RectangleGeometry(
                            new Rect(new Point(0, TopBarHeight),
                                     new Point(parent.Width, TopBarHeight + parent.Height)))
                    };
                    drawingContext.DrawGeometry(brush, null, group);
                }
                else if (context.CurrentStep.IsSquare)
                {
                    CombinedGeometry group = new CombinedGeometry
                    {
                        GeometryCombineMode = GeometryCombineMode.Xor,
                        Geometry1 = new RectangleGeometry
                        {
                            Rect = new Rect(new Point(Position.X, Position.Y),
                            new Point(component.ActualWidth + Position.X,
                                      component.ActualHeight + Position.Y))
                        },
                        Geometry2 = new RectangleGeometry(
                            new Rect(new Point(0, TopBarHeight),
                                     new Point(parent.Width, TopBarHeight + parent.Height)))
                    };
                    drawingContext.DrawGeometry(brush, null, group);
                }
                //Add an arrow if needed
                if (context.CurrentStep.hasArrow)
                {
                    ResourceDictionary Icons = new ResourceDictionary
                    {
                        Source = new Uri("/FocusTreeManager;component/Resources/Icons.xaml",
                                         UriKind.Relative)
                    };
                    //Create the Arrow
                    double ArrowPosX = Position.X - 150 - bestMargin;
                    Canvas icon;
                    //Check if there is space on the right
                    if (ArrowPosX <= 0)
                    {
                        //If not, add it to the right
                        icon = (Canvas)Icons["appbar_arrow_left"];
                        ArrowPosX = Position.X + component.ActualWidth + bestMargin;
                        ComponentSpace = new Rect(Position.X,
                                                  Position.Y,
                                                  ArrowPosX + 150,
                                                  Math.Max(component.ActualHeight, 60));
                    }
                    else
                    {
                        //Otherwise, add it to the left.
                        icon = (Canvas)Icons["appbar_arrow_right"];
                        ComponentSpace = new Rect(ArrowPosX,
                                                  Position.Y,
                                                  150 + component.ActualWidth,
                                                  Math.Max(component.ActualHeight, 60));
                    }
                    if (icon == null) return;
                    VisualBrush control = new VisualBrush(icon);
                    drawingContext.DrawRectangle(control, null, new Rect(
                        ArrowPosX,
                        Position.Y + component.ActualHeight / 2 - 30,
                        150, 60));
                    //Move the space taken by the component to the left
                }
                BuildText(parent, context, ComponentSpace);
            }
            else
            {
                //Prevent this adorner from being updated again.
                DataContext = null;
            }
        }

        private void BuildText(FrameworkElement parent, TutorialViewModel context
            , Rect ComponentSpace)
        {
            //Write text somewhere not in the way
            Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);
            TextBlock block = new TextBlock
            {
                Text = LocalizationHelper.getValueForKey(context.CurrentStep.TextKey, LocalesSource.Tutorial),
                Foreground = Brushes.White,
                FontSize = 18,
                TextWrapping = TextWrapping.Wrap
            };
            FormattedText formattedText = new FormattedText(
                LocalizationHelper.getValueForKey(context.CurrentStep.TextKey, LocalesSource.Tutorial),
                CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                new Typeface(block.FontFamily, block.FontStyle, block.FontWeight, block.FontStretch),
                block.FontSize, Brushes.White)
            {
                MaxTextWidth = Math.Min(parent.Width / 2, MinTestWidth),
                MaxTextHeight = bestTextHeight
            };
            BorderPositionning = getNotInTheWayRectangle(ComponentSpace, parent, formattedText);
            border = new Border
            {
                Height = formattedText.Height + ButtonHeightAndMargin,
                Padding = new Thickness(5),
                Width = formattedText.Width + bestMargin,
                Background = (Brush)appStyle.Item1.Resources["WindowBackgroundBrush"],
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush((Color)appStyle.Item2.Resources["AccentColor"]),
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = new StackPanel
                {
                    Orientation = Orientation.Vertical
                }
            };
            ((StackPanel) border.Child).Children.Add(block);
            if (string.IsNullOrEmpty(context.CurrentStep.WaitForMessage))
            {
                ((StackPanel)border.Child).Children.Add(new Button
                {
                    Content = LocalizationHelper.getValueForKey("Tutorial_Continue", LocalesSource.Tutorial),
                    Style = (Style)parent.FindResource("SquareButtonStyle"),
                    Command = context.ContinueCommand,
                    Margin = new Thickness(5),
                    Width = 150,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            }
            Children.Add(border);
        }

        private FrameworkElement getComponentFromParent(DependencyObject parent,
                                                        TutorialViewModel context)
        {
            if (string.IsNullOrEmpty(context.CurrentStep.ComponentToHighlight)) return null;
            //Try to get the Type
            Type ComponentType = Type.GetType(context.CurrentStep.ComponentToHighlight);
            FrameworkElement ToSearch;
            if (ComponentType != null)
            {
                ToSearch = (FrameworkElement)UiHelper.FindChildWithType(parent, ComponentType);
            }
            //Otherwise, return with string
            ToSearch = (FrameworkElement)UiHelper.FindChildWithName(parent, 
                                        context.CurrentStep.ComponentToHighlight);
            if (ToSearch == null)
            {
                throw new InvalidOperationException("An element defined to be highlighted " +
                                                    "in a tutorial must exist.");
            }
            return ToSearch;
        }

        private Rect getNotInTheWayRectangle(Rect InTheWayrect,
                                             FrameworkElement parent,
                                             FormattedText formattedText)
        {
            //Check if there is enough space on the left of the added stuff
            if (InTheWayrect.Left - formattedText.Width - bestMargin * 2 >= 0)
            {
                Point TopLeft = new Point(InTheWayrect.Left - bestMargin - formattedText.Width
                    , InTheWayrect.Top + InTheWayrect.Height / 2);
                Point BottomRight = new Point(TopLeft.X + formattedText.Width + bestMargin, 
                    TopLeft.Y + formattedText.Height + ButtonHeightAndMargin);
                return new Rect(TopLeft, BottomRight);
            }
            //Otherwise, check on the right
            if (InTheWayrect.Right + formattedText.Width + bestMargin * 2 <= parent.Width)
            {
                Point TopLeft = new Point(InTheWayrect.Right + bestMargin
                    , InTheWayrect.Top + InTheWayrect.Height / 2);
                Point BottomRight = new Point(TopLeft.X + formattedText.Width + bestMargin,
                    TopLeft.Y + formattedText.Height + ButtonHeightAndMargin);
                return new Rect(TopLeft, BottomRight);
            }
            //If not, try to put it under, check if there is enough under
            if (InTheWayrect.Bottom + bestMargin * 2 + ButtonHeightAndMargin
                + formattedText.Height <= parent.Height)
            {
                Point TopLeft = new Point(bestMargin, InTheWayrect.Bottom + bestMargin);
                Point BottomRight = new Point(TopLeft.X + formattedText.Width + bestMargin,
                    TopLeft.Y + formattedText.Height + ButtonHeightAndMargin);
                return new Rect(TopLeft, BottomRight);
            }
            //If no space under, try above
            if (InTheWayrect.Top - bestMargin * 2 - ButtonHeightAndMargin - 
                formattedText.Height >= 0)
            {
                Point TopLeft = new Point(bestMargin, InTheWayrect.Top - bestMargin);
                Point BottomRight = new Point(TopLeft.X + formattedText.Width + bestMargin,
                    TopLeft.Y + formattedText.Height + ButtonHeightAndMargin);
                return new Rect(TopLeft, BottomRight);
            }
            //Just put it slightly in the middle
            return new Rect(parent.Width / 4, parent.Height / 2,
                            formattedText.Width + bestMargin, 
                            formattedText.Height + ButtonHeightAndMargin);
        }

        private void RunOnElement(TutorialViewModel context, FrameworkElement parent, 
                                  FrameworkElement component)
        {
            //Check if we need to right click
            if (context.CurrentStep.RightClickOnComponent)
            {
                //Right click in the middle of the managed component
                Point Position = component.TranslatePoint(new Point(0, 0), parent);
                RightMouseClick(Convert.ToInt32(Position.X + component.ActualWidth / 2),
                                Convert.ToInt32(Position.Y + component.ActualHeight / 2));
            }
            //Check if a command exist to run
            if (!string.IsNullOrEmpty(context.CurrentStep.RunThisCommand))
            {
                Type VmType = context.GetType();
                MethodInfo method = VmType.GetMethod(context.CurrentStep.RunThisCommand);
                method.Invoke(element, null);
            }
            //Check if we need to send a message
            if (!string.IsNullOrEmpty(context.CurrentStep.SendThisMessage))
            {
                Messenger.Default.Send(new NotificationMessage(context, 
                    context.CurrentStep.SendThisMessage));
            }
        }
        
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, 
            int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x010;

        //This simulates a left mouse click
        public static void RightMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, xpos, ypos, 0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (border == null) return finalSize;
            border.Arrange(BorderPositionning);
            return finalSize;
        }
    }
}
