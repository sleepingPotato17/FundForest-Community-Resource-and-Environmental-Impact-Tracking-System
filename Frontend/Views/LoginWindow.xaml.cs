using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace FundForest.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ((Storyboard)Resources["BlobFloat1"]).Begin();
            ((Storyboard)Resources["BlobFloat2"]).Begin();
            ((Storyboard)Resources["BlobFloat3"]).Begin();
            ((Storyboard)Resources["LeafFall1"]).Begin();
            ((Storyboard)Resources["LeafFall2"]).Begin();
            ((Storyboard)Resources["LeafFall3"]).Begin();
            ((Storyboard)Resources["Particle1"]).Begin();
            ((Storyboard)Resources["Particle2"]).Begin();
            ((Storyboard)Resources["Particle3"]).Begin();
            ((Storyboard)Resources["LogoPulse"]).Begin();
            ((Storyboard)Resources["TaglineFadeIn"]).Begin();
            ((Storyboard)Resources["FormFadeIn"]).Begin();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Please contact your system administrator to reset your password.\n\n" +
                "Email: admin@fundforest.com",
                "Forgot Password",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}