using System;
using System.Collections.ObjectModel;
using System.Windows;
using FundForest.Helpers;
using FundForest.Models;
using FundForest.Services;

namespace FundForest.ViewModels
{
    public class ManageUsersViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();

        private ObservableCollection<Admin> _users = new();
        private Admin? _selectedUser;
        private string _errorMessage = string.Empty;

        public ObservableCollection<Admin> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public Admin? SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public int PendingCount => _users == null ? 0 :
            System.Linq.Enumerable.Count(_users, u => !u.IsApproved);

        public RelayCommand ApproveCommand { get; }
        public RelayCommand RejectCommand  { get; }
        public RelayCommand DeleteCommand  { get; }
        public RelayCommand RefreshCommand { get; }

        public ManageUsersViewModel()
        {
            ApproveCommand = new RelayCommand(Approve,  _ => SelectedUser != null && !SelectedUser.IsApproved);
            RejectCommand  = new RelayCommand(Reject,   _ => SelectedUser != null && !SelectedUser.IsApproved);
            DeleteCommand  = new RelayCommand(Delete,   _ => SelectedUser != null);
            RefreshCommand = new RelayCommand(LoadData);
            LoadData();
        }

        private void LoadData(object? _ = null)
        {
            try
            {
                Users = new ObservableCollection<Admin>(_db.GetAllUsers());
                OnPropertyChanged(nameof(PendingCount));
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void Approve(object? _)
        {
            if (SelectedUser == null) return;
            if (MessageBox.Show(
                $"Approve account for '{SelectedUser.FullName}' ({SelectedUser.Role})?",
                "Confirm Approval", MessageBoxButton.YesNo, MessageBoxImage.Question)
                != MessageBoxResult.Yes) return;
            try
            {
                _db.ApproveUser(SelectedUser.AdminID);
                LoadData();
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void Reject(object? _)
        {
            if (SelectedUser == null) return;
            if (MessageBox.Show(
                $"Reject and delete the account for '{SelectedUser.FullName}'?\nThis cannot be undone.",
                "Confirm Rejection", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;
            try
            {
                _db.RejectUser(SelectedUser.AdminID);
                LoadData();
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void Delete(object? _)
        {
            if (SelectedUser == null) return;
            if (MessageBox.Show(
                $"Permanently delete the account for '{SelectedUser.FullName}'?\nThis cannot be undone.",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;
            try
            {
                _db.DeleteUser(SelectedUser.AdminID);
                LoadData();
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }
    }
}