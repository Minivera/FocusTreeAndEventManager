using System;
using System.Windows;
using System.Windows.Controls;
using FocusTreeManager.ViewModel;

namespace FocusTreeManager.Views.UserControls
{
    /// <summary>
    /// Interaction logic for TutorialButton.xaml
    /// </summary>
    public partial class TutorialButton : UserControl
    {
        public FrameworkElement LinkedElement
        {
            get { return (FrameworkElement) GetValue(LinkedElementProperty); }
            set { SetValue(LinkedElementProperty, value); }
        }

        public static readonly DependencyProperty LinkedElementProperty =
            DependencyProperty.Register("LinkedElement", typeof(FrameworkElement),
                typeof(TutorialButton), new PropertyMetadata(null));

        public TutorialButton()
        {
            InitializeComponent();
        }
    }
}
