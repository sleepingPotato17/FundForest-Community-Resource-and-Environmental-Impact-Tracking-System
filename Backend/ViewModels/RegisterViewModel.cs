using System;
using FundForest.Helpers;
using FundForest.Services;

namespace FundForest.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();

        private string _fullName       = string.Empty;
        private string _username       = string.Empty;
        private string _selectedRole   = "Local";
        private string _errorMessage   = string.Empty;
        private string _successMessage = string.Empty;

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }
        public string SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value);
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public bool RegistrationSuccessful { get; private set; } = false;

        public void ExecuteRegister(string password, string confirmPassword)
        {
            ErrorMessage   = string.Empty;
            SuccessMessage = string.Empty;
            RegistrationSuccessful = false;

            if (string.IsNullOrWhiteSpace(FullName))
            { ErrorMessage = "Full name is required."; return; }

            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 4)
            { ErrorMessage = "Username must be at least 4 characters."; return; }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            { ErrorMessage = "Password must be at least 6 characters."; return; }

            if (password != confirmPassword)
            { ErrorMessage = "Passwords do not match."; return; }

            // Block Admin role — frontend + backend both enforce this
            if (SelectedRole == "Admin")
            { ErrorMessage = "Admin accounts cannot be created here."; return; }

            if (SelectedRole != "Staff" && SelectedRole != "Local")
            { ErrorMessage = "Please select a valid role."; return; }

            try
            {
                if (_db.UsernameExists(Username.Trim()))
                { ErrorMessage = "Username already taken. Please choose another."; return; }

                string hashed = BCrypt.Net.BCrypt.HashPassword(password);
                bool success  = _db.CreateAdmin(Username.Trim(), hashed, FullName.Trim(), SelectedRole);

                if (success)
                {
                    SuccessMessage = "✅ Account created! Please wait for admin approval before signing in.";
                    RegistrationSuccessful = true;
                }
                else
                {
                    ErrorMessage = "Failed to create account. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }
        }
    }
}