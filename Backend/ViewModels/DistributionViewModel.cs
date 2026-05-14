using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FundForest.Helpers;
using FundForest.Models;
using FundForest.Services;

namespace FundForest.ViewModels
{
    public class DistributionViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();
        private ObservableCollection<Distribution>   _distributions = new();
        private ObservableCollection<Beneficiary>    _beneficiaries = new();
        private ObservableCollection<Models.Program> _programs      = new();
        private Distribution? _selectedDistribution;
        private Distribution  _editingDistribution  = new() { DistributionDate = DateTime.Today };
        private bool   _isFormVisible;
        private bool   _isEditMode;
        private string _searchText    = string.Empty;
        private string _errorMessage  = string.Empty;

        public ObservableCollection<Distribution>   Distributions { get => _distributions; set => SetProperty(ref _distributions, value); }
        public ObservableCollection<Beneficiary>    Beneficiaries { get => _beneficiaries; set => SetProperty(ref _beneficiaries, value); }
        public ObservableCollection<Models.Program> Programs      { get => _programs;      set => SetProperty(ref _programs, value); }
        public Distribution? SelectedDistribution { get => _selectedDistribution; set => SetProperty(ref _selectedDistribution, value); }
        public Distribution  EditingDistribution  { get => _editingDistribution;  set => SetProperty(ref _editingDistribution, value); }
        public bool IsFormVisible   { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }
        public bool IsEditMode      { get => _isEditMode;    set => SetProperty(ref _isEditMode, value); }
        public string FormTitle     => IsEditMode ? "Update Distribution" : "Record Distribution";
        public string SearchText    { get => _searchText;    set { SetProperty(ref _searchText, value); LoadData(); } }
        public string ErrorMessage  { get => _errorMessage;  set => SetProperty(ref _errorMessage, value); }
        public string[] StatusOptions = { "Pending", "Completed" };

        public bool CanEdit   => SessionService.Instance.CanEdit;
        public bool CanDelete => SessionService.Instance.CanDelete;
        public bool CanAdd    => SessionService.Instance.CanEdit;

        public RelayCommand AddCommand    { get; }
        public RelayCommand EditCommand   { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand   { get; }
        public RelayCommand CancelCommand { get; }

        public DistributionViewModel()
        {
            AddCommand    = new RelayCommand(OpenAdd);
            EditCommand   = new RelayCommand(OpenEdit);
            DeleteCommand = new RelayCommand(DeleteSelected);
            SaveCommand   = new RelayCommand(Save);
            CancelCommand = new RelayCommand(() => IsFormVisible = false);
            LoadData();
            LoadLookups();
        }

        private void LoadData()
        {
            try
            {
                var list = _db.GetDistributions(SearchText).ToList();
                for (int i = 0; i < list.Count; i++)
                    list[i].RowNumber = i + 1;
                Distributions = new ObservableCollection<Distribution>(list);
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void LoadLookups()
        {
            try
            {
                Beneficiaries = new ObservableCollection<Beneficiary>(_db.GetBeneficiaries());
                Programs      = new ObservableCollection<Models.Program>(_db.GetPrograms());
            }
            catch { }
        }

        private void OpenAdd(object? _)
        {
            if (!SessionService.Instance.CanEdit) return;
            EditingDistribution = new Distribution { DistributionDate = DateTime.Today, Quantity = 1 };
            IsEditMode = false; IsFormVisible = true; ErrorMessage = "";
            OnPropertyChanged(nameof(FormTitle));
        }

        private void OpenEdit(object? parameter)
        {
            var target = (parameter as Distribution) ?? SelectedDistribution;
            if (target == null) return;
            if (!SessionService.Instance.CanEdit) return;
            EditingDistribution = new Distribution
            {
                DistributionID   = target.DistributionID,
                BeneficiaryID    = target.BeneficiaryID,
                ProgramID        = target.ProgramID,
                DistributionDate = target.DistributionDate,
                DistributedItem  = target.DistributedItem,
                Quantity         = target.Quantity,
                Amount           = target.Amount,
                Status           = target.Status,
                Notes            = target.Notes,
            };
            IsEditMode = true; IsFormVisible = true;
            OnPropertyChanged(nameof(FormTitle));
        }

        private void DeleteSelected(object? parameter)
        {
            var target = (parameter as Distribution) ?? SelectedDistribution;
            if (target == null) return;
            if (!SessionService.Instance.CanDelete) return;
            if (MessageBox.Show("Delete this distribution record?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try { _db.DeleteDistribution(target.DistributionID); LoadData(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        private void Save(object? _)
        {
            if (EditingDistribution.BeneficiaryID <= 0) { ErrorMessage = "Please select a beneficiary."; return; }
            if (EditingDistribution.Quantity < 1)       { ErrorMessage = "Quantity must be at least 1."; return; }
            try
            {
                if (IsEditMode) _db.UpdateDistribution(EditingDistribution);
                else            _db.InsertDistribution(EditingDistribution);
                IsFormVisible = false; LoadData();
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }
    }
}