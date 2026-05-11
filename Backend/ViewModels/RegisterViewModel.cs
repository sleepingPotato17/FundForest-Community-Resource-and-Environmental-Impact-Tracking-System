using System.ComponentModel;
using System.Runtime.CompilerServices;
using FundForest.Services;

namespace FundForest.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private string _fullName = string.Empty;
        private string _username = string.Empty;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;

        private readonly DatabaseService _db = new();

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set { _successMessage = value; OnPropertyChanged(); }
        }

        public void Register(string password, string confirmPassword, string role)
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(FullName))
            {
                ErrorMessage = "Full name is required.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Username is required.";
                return;
            }

            if (Username.Trim().Length < 4)
            {
                ErrorMessage = "Username must be at least 4 characters.";
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Password is required.";
                return;
            }

            if (password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters.";
                return;
            }

            if (password != confirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                ErrorMessage = "Please select a role.";
                return;
            }

            if (role == "Admin")
            {
                ErrorMessage = "Admin accounts cannot be created here.";
                return;
            }

            if (role != "Staff" && role != "Local")
            {
                ErrorMessage = "Invalid role selected.";
                return;
            }

            if (_db.UsernameExists(Username.Trim()))
            {
                ErrorMessage = "Username already exists. Please choose another.";
                return;
            }

            string hashed = BCrypt.Net.BCrypt.HashPassword(password);
            _db.CreateAdmin(Username.Trim(), hashed, FullName.Trim(), role);

            SuccessMessage = "Account created! Please wait for admin approval before logging in.";

            // Clear fields after successful registration
            FullName = string.Empty;
            Username = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}