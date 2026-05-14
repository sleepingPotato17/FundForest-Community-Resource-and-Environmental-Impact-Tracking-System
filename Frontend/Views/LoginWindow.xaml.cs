using System.Windows;
using System.Windows.Media.Animation;

namespace FundForest.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Start all animations
            (Resources["BlobFloat1"] as Storyboard)?.Begin();
            (Resources["BlobFloat2"] as Storyboard)?.Begin();
            (Resources["BlobFloat3"] as Storyboard)?.Begin();
            (Resources["LeafFall1"]  as Storyboard)?.Begin();
            (Resources["LeafFall2"]  as Storyboard)?.Begin();
            (Resources["LeafFall3"]  as Storyboard)?.Begin();
            (Resources["Particle1"]  as Storyboard)?.Begin();
            (Resources["Particle2"]  as Storyboard)?.Begin();
            (Resources["Particle3"]  as Storyboard)?.Begin();
            (Resources["LogoPulse"]  as Storyboard)?.Begin();
            (Resources["TaglineFadeIn"] as Storyboard)?.Begin();
            (Resources["FormFadeIn"]    as Storyboard)?.Begin(); // ← this makes the form visible
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e) => this.Close();

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "If you have forgotten your password, please contact your system administrator to reset it.",
                "Forgot Password",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var register = new RegisterWindow();
            register.Show();
            this.Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (RevealPasswordBox != null)
                RevealPasswordBox.Text = PasswordBox.Password;
        }
    }
}