using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using FundForest.Helpers;
using FundForest.Models;
using FundForest.Services;
using Microsoft.Win32;

namespace FundForest.ViewModels
{
    // =====================================================================
    // DONORS VIEW MODEL
    // =====================================================================
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
        public string FormTitle     => IsEditMode ? "Edit Donor" : "Add Donor";
        public string SearchText    { get => _searchText;    set { SetProperty(ref _searchText, value); LoadData(); } }
        public string ErrorMessage  { get => _errorMessage;  set => SetProperty(ref _errorMessage, value); }
        public string[] TypeOptions = { "Individual", "Group" };

        public RelayCommand AddCommand    { get; }
        public RelayCommand EditCommand   { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand   { get; }
        public RelayCommand CancelCommand { get; }

        public DonorsViewModel()
        {
            AddCommand    = new RelayCommand(OpenAdd);
            EditCommand   = new RelayCommand(OpenEdit,    _ => SelectedDonor != null);
            DeleteCommand = new RelayCommand(DeleteSelected, _ => SelectedDonor != null);
            SaveCommand   = new RelayCommand(Save);
            CancelCommand = new RelayCommand(() => IsFormVisible = false);
            LoadData();
        }

        private void LoadData()
        {
            try { Donors = new ObservableCollection<Donor>(_db.GetDonors(SearchText)); }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void OpenAdd(object? _) { EditingDonor = new Donor(); IsEditMode = false; IsFormVisible = true; ErrorMessage = ""; }

        private void OpenEdit(object? _)
        {
            if (SelectedDonor == null) return;
            EditingDonor = new Donor { DonorID = SelectedDonor.DonorID, DonorName = SelectedDonor.DonorName,
                DonationType = SelectedDonor.DonationType, ContactInfo = SelectedDonor.ContactInfo, Address = SelectedDonor.Address };
            IsEditMode = true; IsFormVisible = true; ErrorMessage = "";
        }

        private void DeleteSelected(object? _)
        {
            if (SelectedDonor == null) return;
            if (MessageBox.Show($"Delete donor '{SelectedDonor.DonorName}'?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try { _db.DeleteDonor(SelectedDonor.DonorID); LoadData(); }
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

    // =====================================================================
    // DONATIONS VIEW MODEL
    // =====================================================================
    public class DonationsViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();
        private ObservableCollection<Donation> _donations = new();
        private ObservableCollection<Donor>    _donors    = new();
        private Donation? _selectedDonation;
        private Donation  _editingDonation   = new() { DonationDate = DateTime.Today };
        private bool   _isFormVisible;
        private bool   _isEditMode;
        private string _searchText    = string.Empty;
        private string _errorMessage  = string.Empty;
        private DateTime? _filterFrom;
        private DateTime? _filterTo;

        public ObservableCollection<Donation> Donations  { get => _donations;  set => SetProperty(ref _donations, value); }
        public ObservableCollection<Donor>    Donors     { get => _donors;     set => SetProperty(ref _donors, value); }
        public Donation? SelectedDonation { get => _selectedDonation; set => SetProperty(ref _selectedDonation, value); }
        public Donation  EditingDonation  { get => _editingDonation;  set => SetProperty(ref _editingDonation, value); }
        public bool IsFormVisible   { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }
        public bool IsEditMode      { get => _isEditMode;    set => SetProperty(ref _isEditMode, value); }
        public string FormTitle     => IsEditMode ? "Edit Donation" : "Record Donation";
        public string SearchText    { get => _searchText;    set { SetProperty(ref _searchText, value); LoadData(); } }
        public string ErrorMessage  { get => _errorMessage;  set => SetProperty(ref _errorMessage, value); }
        public DateTime? FilterFrom { get => _filterFrom;    set { SetProperty(ref _filterFrom, value); LoadData(); } }
        public DateTime? FilterTo   { get => _filterTo;      set { SetProperty(ref _filterTo, value);   LoadData(); } }
        public bool IsGoods => EditingDonation.DonationType == "Goods";
        public string[] TypeOptions = { "Cash", "Goods" };

        public RelayCommand AddCommand      { get; }
        public RelayCommand EditCommand     { get; }
        public RelayCommand DeleteCommand   { get; }
        public RelayCommand SaveCommand     { get; }
        public RelayCommand CancelCommand   { get; }
        public RelayCommand ExportCommand   { get; }
        public RelayCommand ClearFilterCmd  { get; }

        public DonationsViewModel()
        {
            AddCommand     = new RelayCommand(OpenAdd);
            EditCommand    = new RelayCommand(OpenEdit,    _ => SelectedDonation != null);
            DeleteCommand  = new RelayCommand(DeleteSelected, _ => SelectedDonation != null);
            SaveCommand    = new RelayCommand(Save);
            CancelCommand  = new RelayCommand(() => IsFormVisible = false);
            ExportCommand  = new RelayCommand(ExportToCsv);
            ClearFilterCmd = new RelayCommand(() => { FilterFrom = null; FilterTo = null; });

            EditingDonation.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(Donation.DonationType)) OnPropertyChanged(nameof(IsGoods)); };
            LoadData(); LoadDonors();
        }

        private void LoadData()
        {
            try { Donations = new ObservableCollection<Donation>(_db.GetDonations(SearchText, FilterFrom, FilterTo)); }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void LoadDonors()
        {
            try { Donors = new ObservableCollection<Donor>(_db.GetDonors()); }
            catch { }
        }

        private void OpenAdd(object? _) { EditingDonation = new Donation { DonationDate = DateTime.Today }; IsEditMode = false; IsFormVisible = true; ErrorMessage = ""; }

        private void OpenEdit(object? _)
        {
            if (SelectedDonation == null) return;
            EditingDonation = new Donation
            {
                DonationID = SelectedDonation.DonationID, DonorID = SelectedDonation.DonorID,
                DonorName = SelectedDonation.DonorName, Amount = SelectedDonation.Amount,
                DonationType = SelectedDonation.DonationType, GoodsDescription = SelectedDonation.GoodsDescription,
                DonationDate = SelectedDonation.DonationDate, Notes = SelectedDonation.Notes,
            };
            IsEditMode = true; IsFormVisible = true;
        }

        private void DeleteSelected(object? _)
        {
            if (SelectedDonation == null) return;
            if (MessageBox.Show("Delete this donation record?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try { _db.DeleteDonation(SelectedDonation.DonationID); LoadData(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        private void Save(object? _)
        {
            if (EditingDonation.DonorID <= 0) { ErrorMessage = "Please select a donor."; return; }
            if (EditingDonation.Amount < 0)   { ErrorMessage = "Amount must be non-negative."; return; }
            try
            {
                if (IsEditMode) _db.UpdateDonation(EditingDonation);
                else            _db.InsertDonation(EditingDonation);
                IsFormVisible = false; LoadData();
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void ExportToCsv(object? _)
        {
            var dlg = new SaveFileDialog { Filter = "CSV Files|*.csv", FileName = "donations_export" };
            if (dlg.ShowDialog() != true) return;
            var sb = new StringBuilder("Donor,Amount,Type,Goods Description,Date\n");
            foreach (var d in Donations)
                sb.AppendLine($"{d.DonorName},{d.Amount},{d.DonationType},{d.GoodsDescription},{d.DonationDate:yyyy-MM-dd}");
            File.WriteAllText(dlg.FileName, sb.ToString());
            MessageBox.Show("Exported successfully!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    // =====================================================================
    // PROGRAMS VIEW MODEL
    // =====================================================================
    public class ProgramsViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();
        private ObservableCollection<Models.Program> _programs = new();
        private Models.Program? _selectedProgram;
        private Models.Program  _editingProgram   = new();
        private bool   _isFormVisible;
        private bool   _isEditMode;
        private string _searchText    = string.Empty;
        private string _errorMessage  = string.Empty;

        public ObservableCollection<Models.Program> Programs { get => _programs; set => SetProperty(ref _programs, value); }
        public Models.Program? SelectedProgram { get => _selectedProgram; set => SetProperty(ref _selectedProgram, value); }
        public Models.Program  EditingProgram  { get => _editingProgram;  set => SetProperty(ref _editingProgram, value); }
        public bool IsFormVisible  { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }
        public bool IsEditMode     { get => _isEditMode;    set => SetProperty(ref _isEditMode, value); }
        public string FormTitle    => IsEditMode ? "Edit Program" : "New Program";
        public string SearchText   { get => _searchText;    set { SetProperty(ref _searchText, value); LoadData(); } }
        public string ErrorMessage { get => _errorMessage;  set => SetProperty(ref _errorMessage, value); }
        public string[] AudienceOptions = { "Group", "Barangay" };
        public string[] StatusOptions   = { "Active", "Archived" };

        public RelayCommand AddCommand     { get; }
        public RelayCommand EditCommand    { get; }
        public RelayCommand DeleteCommand  { get; }
        public RelayCommand SaveCommand    { get; }
        public RelayCommand CancelCommand  { get; }
        public RelayCommand ArchiveCommand { get; }

        public ProgramsViewModel()
        {
            AddCommand     = new RelayCommand(OpenAdd);
            EditCommand    = new RelayCommand(OpenEdit,   _ => SelectedProgram != null);
            DeleteCommand  = new RelayCommand(DeleteSelected, _ => SelectedProgram != null);
            SaveCommand    = new RelayCommand(Save);
            CancelCommand  = new RelayCommand(() => IsFormVisible = false);
            ArchiveCommand = new RelayCommand(Archive, _ => SelectedProgram != null);
            LoadData();
        }

        private void LoadData()
        {
            try { Programs = new ObservableCollection<Models.Program>(_db.GetPrograms(SearchText)); }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void OpenAdd(object? _) { EditingProgram = new Models.Program(); IsEditMode = false; IsFormVisible = true; ErrorMessage = ""; }

        private void OpenEdit(object? _)
        {
            if (SelectedProgram == null) return;
            EditingProgram = new Models.Program
            {
                ProgramID = SelectedProgram.ProgramID, ProgramName = SelectedProgram.ProgramName,
                StartDate = SelectedProgram.StartDate, EndDate = SelectedProgram.EndDate,
                TargetAudience = SelectedProgram.TargetAudience, Barangay = SelectedProgram.Barangay,
                Description = SelectedProgram.Description, Status = SelectedProgram.Status,
            };
            IsEditMode = true; IsFormVisible = true;
        }

        private void DeleteSelected(object? _)
        {
            if (SelectedProgram == null) return;
            if (MessageBox.Show($"Delete program '{SelectedProgram.ProgramName}'?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try { _db.DeleteProgram(SelectedProgram.ProgramID); LoadData(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        private void Archive(object? _)
        {
            if (SelectedProgram == null) return;
            SelectedProgram.Status = "Archived";
            try { _db.UpdateProgram(SelectedProgram); LoadData(); }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void Save(object? _)
        {
            if (string.IsNullOrWhiteSpace(EditingProgram.ProgramName)) { ErrorMessage = "Program name is required."; return; }
            try
            {
                if (IsEditMode) _db.UpdateProgram(EditingProgram);
                else            _db.InsertProgram(EditingProgram);
                IsFormVisible = false; LoadData();
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }
    }

    // =====================================================================
    // DISTRIBUTION VIEW MODEL
    // =====================================================================
    public class DistributionViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();
        private ObservableCollection<Distribution>     _distributions = new();
        private ObservableCollection<Beneficiary>      _beneficiaries = new();
        private ObservableCollection<Models.Program>   _programs      = new();
        private Distribution? _selectedDistribution;
        private Distribution  _editingDistribution  = new() { DistributionDate = DateTime.Today };
        private bool   _isFormVisible;
        private bool   _isEditMode;
        private string _searchText    = string.Empty;
        private string _errorMessage  = string.Empty;

        public ObservableCollection<Distribution>   Distributions  { get => _distributions;  set => SetProperty(ref _distributions, value); }
        public ObservableCollection<Beneficiary>    Beneficiaries  { get => _beneficiaries;  set => SetProperty(ref _beneficiaries, value); }
        public ObservableCollection<Models.Program> Programs       { get => _programs;        set => SetProperty(ref _programs, value); }
        public Distribution? SelectedDistribution { get => _selectedDistribution; set => SetProperty(ref _selectedDistribution, value); }
        public Distribution  EditingDistribution  { get => _editingDistribution;  set => SetProperty(ref _editingDistribution, value); }
        public bool IsFormVisible   { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }
        public bool IsEditMode      { get => _isEditMode;    set => SetProperty(ref _isEditMode, value); }
        public string FormTitle     => IsEditMode ? "Edit Distribution" : "Record Distribution";
        public string SearchText    { get => _searchText;    set { SetProperty(ref _searchText, value); LoadData(); } }
        public string ErrorMessage  { get => _errorMessage;  set => SetProperty(ref _errorMessage, value); }
        public string[] StatusOptions = { "Pending", "Completed" };

        public RelayCommand AddCommand    { get; }
        public RelayCommand EditCommand   { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveCommand   { get; }
        public RelayCommand CancelCommand { get; }

        public DistributionViewModel()
        {
            AddCommand    = new RelayCommand(OpenAdd);
            EditCommand   = new RelayCommand(OpenEdit,   _ => SelectedDistribution != null);
            DeleteCommand = new RelayCommand(DeleteSelected, _ => SelectedDistribution != null);
            SaveCommand   = new RelayCommand(Save);
            CancelCommand = new RelayCommand(() => IsFormVisible = false);
            LoadData(); LoadLookups();
        }

        private void LoadData()
        {
            try { Distributions = new ObservableCollection<Distribution>(_db.GetDistributions(SearchText)); }
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

        private void OpenAdd(object? _) { EditingDistribution = new Distribution { DistributionDate = DateTime.Today, Quantity = 1 }; IsEditMode = false; IsFormVisible = true; ErrorMessage = ""; }

        private void OpenEdit(object? _)
        {
            if (SelectedDistribution == null) return;
            EditingDistribution = new Distribution
            {
                DistributionID = SelectedDistribution.DistributionID, BeneficiaryID = SelectedDistribution.BeneficiaryID,
                ProgramID = SelectedDistribution.ProgramID, DistributionDate = SelectedDistribution.DistributionDate,
                DistributedItem = SelectedDistribution.DistributedItem, Quantity = SelectedDistribution.Quantity,
                Amount = SelectedDistribution.Amount, Status = SelectedDistribution.Status, Notes = SelectedDistribution.Notes,
            };
            IsEditMode = true; IsFormVisible = true;
        }

        private void DeleteSelected(object? _)
        {
            if (SelectedDistribution == null) return;
            if (MessageBox.Show("Delete this distribution record?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try { _db.DeleteDistribution(SelectedDistribution.DistributionID); LoadData(); }
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
