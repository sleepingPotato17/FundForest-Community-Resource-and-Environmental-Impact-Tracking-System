using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FundForest.Helpers;
using FundForest.Models;
using FundForest.Services;

namespace FundForest.ViewModels
{
    public class BeneficiariesViewModel : BaseViewModel
    {
        public bool CanEdit   => SessionService.Instance.CanEdit;
        public bool CanDelete => SessionService.Instance.CanDelete;
        public bool CanAdd    => SessionService.Instance.CanEdit;

        private readonly DatabaseService _db = new();
        private ObservableCollection<Beneficiary> _beneficiaries = new();
        private Beneficiary? _selectedBeneficiary;
        private Beneficiary  _editingBeneficiary = new();
        private bool   _isFormVisible;
        private bool   _isEditMode;
        private string _searchText   = string.Empty;
        private string _errorMessage = string.Empty;

        public ObservableCollection<Beneficiary> Beneficiaries
        {
            get => _beneficiaries;
            set => SetProperty(ref _beneficiaries, value);
        }
        public Beneficiary? SelectedBeneficiary
        {
            get => _selectedBeneficiary;
            set => SetProperty(ref _selectedBeneficiary, value);
        }
        public Beneficiary EditingBeneficiary
        {
            get => _editingBeneficiary;
            set => SetProperty(ref _editingBeneficiary, value);
        }
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set => SetProperty(ref _isFormVisible, value);
        }
        public bool IsPersonType => EditingBeneficiary.BeneficiaryType == "Person";
        public bool IsGroupType  => EditingBeneficiary.BeneficiaryType == "Group";
        public bool IsEditMode   { get => _isEditMode; set => SetProperty(ref _isEditMode, value); }
        public string FormTitle  => IsEditMode ? "Update Beneficiary" : "Add Beneficiary";
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); LoadData(); }
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string[] GenderOptions = { "Male", "Female", "Other", "None (Group)" };
        public string[] VulnOptions   = { "None", "Elderly", "PWD", "Indigent", "Solo Parent", "4Ps" };
        public string[] TypeOptions   = { "Person", "Group" };

        public RelayCommand AddCommand    { get; }
        public RelayCommand EditCommand   { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand   { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand SearchCommand { get; }

        public BeneficiariesViewModel()
        {
            AddCommand    = new RelayCommand(OpenAddForm);
            EditCommand   = new RelayCommand(OpenEditForm);   // accepts CommandParameter
            DeleteCommand = new RelayCommand(DeleteSelected); // accepts CommandParameter
            SaveCommand   = new RelayCommand(Save);
            CancelCommand = new RelayCommand(() => IsFormVisible = false);
            SearchCommand = new RelayCommand(LoadData);

            EditingBeneficiary.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Beneficiary.BeneficiaryType))
                {
                    OnPropertyChanged(nameof(IsPersonType));
                    OnPropertyChanged(nameof(IsGroupType));
                }
            };

            LoadData();
        }

        private void LoadData(object? _ = null)
        {
            try
            {
                var list = _db.GetBeneficiaries(SearchText)
                              .OrderBy(b => b.BeneficiaryID)
                              .ToList();

                for (int i = 0; i < list.Count; i++)
                    list[i].RowNumber = i + 1;

                Beneficiaries = new ObservableCollection<Beneficiary>(list);
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void OpenAddForm(object? _)
        {
            EditingBeneficiary = new Beneficiary();
            IsEditMode    = false;
            IsFormVisible = true;
            ErrorMessage  = string.Empty;
            OnPropertyChanged(nameof(FormTitle));
        }

        private void OpenEditForm(object? parameter)
        {
            // Accept the row's bound item directly from CommandParameter
            var target = (parameter as Beneficiary) ?? SelectedBeneficiary;
            if (target == null) return;

            EditingBeneficiary = new Beneficiary
            {
                BeneficiaryID       = target.BeneficiaryID,
                BeneficiaryType     = target.BeneficiaryType,
                FullName            = target.FullName,
                Gender              = target.Gender,
                Age                 = target.Age,
                GroupName           = target.GroupName,
                NumberOfMembers     = target.NumberOfMembers,
                GroupRepresentative = target.GroupRepresentative,
                VulnerabilityType   = target.VulnerabilityType,
                Address             = target.Address,
                ContactInfo         = target.ContactInfo,
                ContactDate         = target.ContactDate,
            };
            IsEditMode    = true;
            IsFormVisible = true;
            ErrorMessage  = string.Empty;
            OnPropertyChanged(nameof(FormTitle));
        }

        private void DeleteSelected(object? parameter)
        {
            var target = (parameter as Beneficiary) ?? SelectedBeneficiary;
            if (target == null) return;

            var confirm = MessageBox.Show(
                $"Delete beneficiary '{target.DisplayName}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;
            try
            {
                _db.DeleteBeneficiary(target.BeneficiaryID);
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        private void Save(object? _)
        {
            if (!Validate()) return;
            try
            {
                if (IsEditMode) _db.UpdateBeneficiary(EditingBeneficiary);
                else            _db.InsertBeneficiary(EditingBeneficiary);
                IsFormVisible = false;
                LoadData();
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private bool Validate()
        {
            ErrorMessage = string.Empty;
            if (EditingBeneficiary.BeneficiaryType == "Person" &&
                string.IsNullOrWhiteSpace(EditingBeneficiary.FullName))
            { ErrorMessage = "Full Name is required."; return false; }
            if (EditingBeneficiary.BeneficiaryType == "Group" &&
                string.IsNullOrWhiteSpace(EditingBeneficiary.GroupName))
            { ErrorMessage = "Group Name is required."; return false; }
            return true;
        }
    }
}