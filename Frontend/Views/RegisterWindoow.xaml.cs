using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FundForest.ViewModels;

namespace FundForest.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
            => this.Close();

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                if (RoleComboBox.SelectedItem is ComboBoxItem item)
                    vm.SelectedRole = item.Content.ToString() ?? "Local";

                vm.ExecuteRegister(PwdBox.Password, ConfirmPwdBox.Password);

                if (vm.RegistrationSuccessful)
                {
                    MessageBox.Show("Account created successfully! You can now sign in.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}