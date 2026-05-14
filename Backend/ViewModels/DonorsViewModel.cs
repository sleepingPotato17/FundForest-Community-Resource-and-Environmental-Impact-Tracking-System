using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FundForest.Helpers;
using FundForest.Models;
using FundForest.Services;

namespace FundForest.ViewModels
{
    public class DonorsViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();
        private ObservableCollection<Donor> _donors = new();
        private Donor? _selectedDonor;
        private Donor  _editingDonor   = new();
        private bool   _isFormVisible;
        private bool   _isEditMode;
        private string _searchText    = string.Empty;
        private string _errorMessage  = string.Empty;

        public ObservableCollection<Donor> Donors { get => _donors; set => SetProperty(ref _donors, value); }
        public Donor? SelectedDonor { get => _selectedDonor; set => SetProperty(ref _selectedDonor, value); }
        public Donor  EditingDonor  { get => _editingDonor;  set => SetProperty(ref _editingDonor, value); }
        public bool IsFormVisible   { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }
        public bool IsEditMode      { get => _isEditMode;    set => SetProperty(ref _isEditMode, value); }
        public string FormTitle     => IsEditMode ? "Update Donor" : "Add Donor";
        public string SearchText    { get => _searchText;    set { SetProperty(ref _searchText, value); LoadData(); } }
        public string ErrorMessage  { get => _errorMessage;  set => SetProperty(ref _errorMessage, value); }
        public string[] TypeOptions = { "Individual", "Group" };

        public bool CanEdit   => SessionService.Instance.CanEdit;
        public bool CanDelete => SessionService.Instance.CanDelete;
        public bool CanAdd    => SessionService.Instance.CanEdit;

        public RelayCommand AddCommand    { get; }
        public RelayCommand EditCommand   { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand   { get; }
        public RelayCommand CancelCommand { get; }

        public DonorsViewModel()
        {
            AddCommand    = new RelayCommand(OpenAdd);
            EditCommand   = new RelayCommand(OpenEdit);
            DeleteCommand = new RelayCommand(DeleteSelected);
            SaveCommand   = new RelayCommand(Save);
            CancelCommand = new RelayCommand(() => IsFormVisible = false);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var list = _db.GetDonors(SearchText).ToList();
                for (int i = 0; i < list.Count; i++)
                    list[i].RowNumber = i + 1;
                Donors = new ObservableCollection<Donor>(list);
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void OpenAdd(object? _)
        {
            if (!SessionService.Instance.CanEdit) return;
            EditingDonor = new Donor(); IsEditMode = false; IsFormVisible = true; ErrorMessage = "";
            OnPropertyChanged(nameof(FormTitle));
        }

        private void OpenEdit(object? parameter)
        {
            var target = (parameter as Donor) ?? SelectedDonor;
            if (target == null) return;
            if (!SessionService.Instance.CanEdit) return;
            EditingDonor = new Donor
            {
                DonorID      = target.DonorID,
                DonorName    = target.DonorName,
                DonationType = target.DonationType,
                ContactInfo  = target.ContactInfo,
                Address      = target.Address
            };
            IsEditMode = true; IsFormVisible = true; ErrorMessage = "";
            OnPropertyChanged(nameof(FormTitle));
        }

        private void DeleteSelected(object? parameter)
        {
            var target = (parameter as Donor) ?? SelectedDonor;
            if (target == null) return;
            if (!SessionService.Instance.CanDelete) return;
            if (MessageBox.Show($"Delete donor '{target.DonorName}'?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try { _db.DeleteDonor(target.DonorID); LoadData(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        private void Save(object? _)
        {
            if (string.IsNullOrWhiteSpace(EditingDonor.DonorName)) { ErrorMessage = "Donor name is required."; return; }
            try
            {
                if (IsEditMode) _db.UpdateDonor(EditingDonor);
                else            _db.InsertDonor(EditingDonor);
                IsFormVisible = false; LoadData();
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }
    }
}