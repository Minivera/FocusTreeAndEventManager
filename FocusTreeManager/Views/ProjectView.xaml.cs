using System;
using System.Collections.Generic;
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

namespace FocusTreeManager.Views
{
    /// <summary>
    /// Logique d'interaction pour ProjectView.xaml
    /// </summary>
    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            InitializeComponent();
            Localization locale = new Localization();
            ResourceDictionary resourceLocalization = new ResourceDictionary();
            resourceLocalization.Source = new Uri(locale.getLanguageFile(), UriKind.Relative);
            this.Resources.MergedDictionaries.Add(resourceLocalization);
        }

        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).BorderThickness = new Thickness(1);
        }

        private void TextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).BorderThickness = new Thickness(0);
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
                ((TextBox)sender).BorderThickness = new Thickness(0);
            }
        }
    }
}
