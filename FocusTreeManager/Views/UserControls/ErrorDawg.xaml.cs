using System.Windows.Controls;
using System.Windows.Input;

namespace FocusTreeManager.Views.UserControls
{
    public partial class ErrorDawg : UserControl
    {
        public ErrorDawg()
        {
            InitializeComponent();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Errors view = new Errors();
            view.Show();
        }
    }
}
