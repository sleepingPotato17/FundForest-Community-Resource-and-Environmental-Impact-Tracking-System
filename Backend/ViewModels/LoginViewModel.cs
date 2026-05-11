using System.Windows;
using FundForest.Helpers;
using FundForest.Services;
using FundForest.Views;

namespace FundForest.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();

        private string _username     = string.Empty;
        private string _errorMessage = string.Empty;
        private bool   _isLoading;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public RelayCommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        private void ExecuteLogin(object? param)
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Please enter your username.";
                return;
            }

            var passwordBox = param as System.Windows.Controls.PasswordBox;
            string password = passwordBox?.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Please enter your password.";
                return;
            }

            IsLoading = true;
            try
            {
                var admin = _db.ValidateLogin(Username.Trim(), password);

                if (admin == null)
                {
                    ErrorMessage = "Invalid username or password.";
                    return;
                }

                if (!admin.IsApproved)
                {
                    ErrorMessage = "Your account is awaiting admin approval. Please try again later.";
                    return;
                }

                SessionService.Instance.CurrentAdmin = admin;

                var mainWindow = new MainWindow();
                mainWindow.Show();

                foreach (Window w in Application.Current.Windows)
                    if (w is LoginWindow) { w.Close(); break; }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}