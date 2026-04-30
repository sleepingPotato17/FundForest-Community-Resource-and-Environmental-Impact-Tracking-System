using System.Windows;

namespace FundForest.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow() => InitializeComponent();

        // Allow dragging the borderless window
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}
