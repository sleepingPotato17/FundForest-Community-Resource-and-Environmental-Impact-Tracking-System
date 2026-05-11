using System.Windows;

namespace FundForest.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is FundForest.ViewModels.RegisterViewModel vm)
            {
                var role = (RoleComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)
                        ?.Content?.ToString() ?? string.Empty;

                vm.Register(PwdBox.Password, ConfirmPwdBox.Password, role);
            }
        }

        private void PwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (RevealPwdBox != null)
                RevealPwdBox.Text = PwdBox.Password;
        }

        private void ConfirmPwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (RevealConfirmPwdBox != null)
                RevealConfirmPwdBox.Text = ConfirmPwdBox.Password;
        }
    }
}