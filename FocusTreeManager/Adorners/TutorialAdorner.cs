using System;
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

        private const double bestTextHeight = 250;

        private const double TopBarHeight = 30;

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

        public TutorialAdorner(FrameworkElement el, FrameworkElement parent) : base(parent)
        {
            element = el;
            DataContext = new ViewModelLocator().Tutorial;
            Children = new Canvas {IsHitTestVisible = true};
            Panel.SetZIndex(Children, 10);
            IsHitTestVisible = true;
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException();
            return Children;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
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
                    //Build text in the middle
                    BuildText(parent, context, new Rect(new Point(0, 0), 
                              new Point(parent.ActualWidth, parent.ActualHeight)));
                    return;
                }
                Child.Children.Clear();
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
                    Canvas icon = (Canvas)Icons["appbar_arrow_right"];
                    if (icon == null) return;
                    Canvas.SetLeft(icon, Position.X - 150 - bestMargin);
                    Canvas.SetTop(icon, Position.Y + component.ActualHeight / 2 - 30);
                    icon.Height = 60;
                    icon.Width = 150;
                    Child.Children.Add(icon);
                    //Move the space taken by the component to the left
                    ComponentSpace = new Rect(new Point(Position.X - 150 - bestMargin,
                                                        Position.Y),
                                              new Point(Position.X + component.ActualWidth,
                                                        Position.Y + component.ActualHeight));
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
            ResourceDictionary language = new ResourceDictionary
            {
                Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative)
            };
            Rect notInTheway = getNotInTheWayRectangle(ComponentSpace, parent);
            Border border = new Border
            {
                MaxHeight = 350,
                Padding = new Thickness(10),
                MaxWidth = parent.ActualWidth / 2,
                Background = (Brush)appStyle.Item1.Resources["WindowBackgroundBrush"],
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush((Color)appStyle.Item2.Resources["AccentColor"]),
                HorizontalAlignment = HorizontalAlignment.Left,
                Child = new StackPanel
                {
                    Orientation = Orientation.Vertical
                }
            };
            ((StackPanel)border.Child).Children.Add(new TextBlock
                {
                    Text = (string) language[context.CurrentStep.TextKey],
                    Foreground = Brushes.White,
                    FontSize = 18,
                    TextWrapping = TextWrapping.Wrap
                });
            if (string.IsNullOrEmpty(context.CurrentStep.WaitForMessage))
            {
                ((StackPanel)border.Child).Children.Add(new Button
                {
                    Content = (string)language["Tutorial_Continue"],
                    Style = (Style)parent.FindResource("SquareButtonStyle"),
                    Command = context.ContinueCommand,
                    Margin = new Thickness(5),
                    Width = 150,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            }
            Canvas.SetTop(border, notInTheway.Y);
            Canvas.SetLeft(border, notInTheway.X);
            Child.Children.Add(border);
        }

        private FrameworkElement getComponentFromParent(DependencyObject parent,
                                                        TutorialViewModel context)
        {
            //Try to get the Type
            Type ComponentType = Type.GetType(context.CurrentStep.ComponentToHighlight);
            if (ComponentType != null)
            {
                return (FrameworkElement)UiHelper.FindChildWithType(element, ComponentType);
            }
            //Otherwise, return with string
            return (FrameworkElement)UiHelper.FindChildWithName(parent, 
                                        context.CurrentStep.ComponentToHighlight);
        }

        private Rect getNotInTheWayRectangle(Rect InTheWayrect,
                                             FrameworkElement parent)
        {
            //Check if there is enough space on the left of the added stuff
            if (InTheWayrect.Left - parent.Width / 2 - bestMargin >= 0)
            {
                Point TopLeft = new Point(InTheWayrect.Left - bestMargin - parent.Width / 2
                    , InTheWayrect.Top);
                Point BottomRight = new Point(TopLeft.X + parent.Width / 2, 
                    TopLeft.Y + bestTextHeight);
                return new Rect(TopLeft, BottomRight);
            }
            //If yes, try to put it under, check if there is enough under
            if (InTheWayrect.Bottom + bestMargin * 2 + bestTextHeight <= parent.Height)
            {
                Point TopLeft = new Point(bestMargin, InTheWayrect.Bottom + bestMargin);
                Point BottomRight = new Point(TopLeft.X + parent.Width / 2,
                    TopLeft.Y + bestTextHeight);
                return new Rect(TopLeft, BottomRight);
            }
            //If no space under, try above
            if (InTheWayrect.Top - bestMargin * 2 - bestTextHeight >= 0)
            {
                Point TopLeft = new Point(bestMargin, InTheWayrect.Top - bestMargin);
                Point BottomRight = new Point(TopLeft.X + parent.Width / 2,
                    TopLeft.Y + bestTextHeight);
                return new Rect(TopLeft, BottomRight);
            }
            //Just put it slightly in the middle
            return new Rect(new Point(parent.Width / 4, parent.Height / 2),
                            new Point(parent.Width / 2, bestTextHeight));
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
            FrameworkElement parent = UiHelper.FindVisualParent<Window>(element, null);
            double desiredWidth = parent.ActualWidth;
            double desiredHeight = parent.ActualHeight;
            Child.Arrange(new Rect(0, 0, desiredWidth, desiredHeight));
            return finalSize;
        }
    }
}
