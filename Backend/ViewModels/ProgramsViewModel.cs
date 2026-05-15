using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FundForest.Helpers;
using FundForest.Models;
using FundForest.Services;

namespace FundForest.ViewModels
{
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
        public string FormTitle    => IsEditMode ? "Update Program" : "New Program";
        public string SearchText   { get => _searchText;    set { SetProperty(ref _searchText, value); LoadData(); } }
        public string ErrorMessage { get => _errorMessage;  set => SetProperty(ref _errorMessage, value); }
        public string[] AudienceOptions = { "Group", "Barangay" };
        public string[] StatusOptions   = { "Active", "Archived" };

        public bool CanEdit   => SessionService.Instance.CanEdit;
        public bool CanDelete => SessionService.Instance.CanDelete;
        public bool CanAdd    => SessionService.Instance.CanEdit;

        public RelayCommand AddCommand     { get; }
        public RelayCommand EditCommand    { get; }
        public RelayCommand DeleteCommand  { get; }
        public RelayCommand SaveCommand    { get; }
        public RelayCommand CancelCommand  { get; }
        public RelayCommand ArchiveCommand { get; }

        public ProgramsViewModel()
        {
            AddCommand     = new RelayCommand(OpenAdd);
            EditCommand    = new RelayCommand(OpenEdit);
            DeleteCommand  = new RelayCommand(DeleteSelected);
            SaveCommand    = new RelayCommand(Save);
            CancelCommand  = new RelayCommand(() => IsFormVisible = false);
            ArchiveCommand = new RelayCommand(Archive);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var list = _db.GetPrograms(SearchText).ToList();
                for (int i = 0; i < list.Count; i++)
                    list[i].RowNumber = i + 1;
                Programs = new ObservableCollection<Models.Program>(list);
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void OpenAdd(object? _)
        {
            if (!SessionService.Instance.CanEdit) return;
            EditingProgram = new Models.Program(); IsEditMode = false; IsFormVisible = true; ErrorMessage = "";
            OnPropertyChanged(nameof(FormTitle));
        }

        private void OpenEdit(object? parameter)
        {
            var target = (parameter as Models.Program) ?? SelectedProgram;
            if (target == null) return;
            if (!SessionService.Instance.CanEdit) return;
            EditingProgram = new Models.Program
            {
                ProgramID      = target.ProgramID,
                ProgramName    = target.ProgramName,
                StartDate      = target.StartDate,
                EndDate        = target.EndDate,
                TargetAudience = target.TargetAudience,
                Barangay       = target.Barangay,
                Description    = target.Description,
                Status         = target.Status,
            };
            IsEditMode = true; IsFormVisible = true;
            OnPropertyChanged(nameof(FormTitle));
        }

        private void DeleteSelected(object? parameter)
        {
            var target = (parameter as Models.Program) ?? SelectedProgram;
            if (target == null) return;
            if (!SessionService.Instance.CanDelete) return;
            if (MessageBox.Show($"Delete program '{target.ProgramName}'?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try { _db.DeleteProgram(target.ProgramID); LoadData(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }

        private void Archive(object? parameter)
        {
            var target = (parameter as Models.Program) ?? SelectedProgram;
            if (target == null) return;
            if (!SessionService.Instance.CanEdit) return;
            if (target.Status == "Archived")
            {
                MessageBox.Show("This program is already archived.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (MessageBox.Show($"Archive '{target.ProgramName}'?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            target.Status = "Archived";
            try { _db.UpdateProgram(target); LoadData(); }
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
}