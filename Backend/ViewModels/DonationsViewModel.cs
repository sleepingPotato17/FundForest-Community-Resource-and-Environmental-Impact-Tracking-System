using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FundForest.Helpers;
using FundForest.Models;
using FundForest.Services;
using Microsoft.Win32;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;

namespace FundForest.ViewModels
{
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
        public string FormTitle     => IsEditMode ? "Update Donation" : "Record Donation";
        public string SearchText    { get => _searchText;    set { SetProperty(ref _searchText, value); LoadData(); } }
        public string ErrorMessage  { get => _errorMessage;  set => SetProperty(ref _errorMessage, value); }
        public DateTime? FilterFrom { get => _filterFrom;    set { SetProperty(ref _filterFrom, value); LoadData(); } }
        public DateTime? FilterTo   { get => _filterTo;      set { SetProperty(ref _filterTo, value);   LoadData(); } }
        public bool IsGoods => EditingDonation.DonationType == "Goods";
        public string[] TypeOptions = { "Cash", "Goods" };

        public bool CanEdit   => SessionService.Instance.CanEdit;
        public bool CanDelete => SessionService.Instance.CanDelete;
        public bool CanAdd    => SessionService.Instance.CanEdit;

        public RelayCommand AddCommand     { get; }
        public RelayCommand EditCommand    { get; }
        public RelayCommand DeleteCommand  { get; }
        public RelayCommand SaveCommand    { get; }
        public RelayCommand CancelCommand  { get; }
        public RelayCommand ExportCommand  { get; }
        public RelayCommand ClearFilterCmd { get; }

        public DonationsViewModel()
        {
            AddCommand     = new RelayCommand(OpenAdd);
            EditCommand    = new RelayCommand(OpenEdit);
            DeleteCommand  = new RelayCommand(DeleteSelected);
            SaveCommand    = new RelayCommand(Save);
            CancelCommand  = new RelayCommand(() => IsFormVisible = false);
            ExportCommand  = new RelayCommand(ExportToPdf);
            ClearFilterCmd = new RelayCommand(() => { FilterFrom = null; FilterTo = null; });

            EditingDonation.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Donation.DonationType))
                    OnPropertyChanged(nameof(IsGoods));
            };

            LoadData();
            LoadDonors();
        }

        private void LoadData()
        {
            try
            {
                var list = _db.GetDonations(SearchText, FilterFrom, FilterTo).ToList();
                for (int i = 0; i < list.Count; i++)
                    list[i].RowNumber = i + 1;
                Donations = new ObservableCollection<Donation>(list);
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        }

        private void LoadDonors()
        {
            try { Donors = new ObservableCollection<Donor>(_db.GetDonors()); }
            catch { }
        }

        private void OpenAdd(object? _)
        {
            if (!SessionService.Instance.CanEdit) return;
            EditingDonation = new Donation { DonationDate = DateTime.Today };
            IsEditMode = false; IsFormVisible = true; ErrorMessage = "";
            OnPropertyChanged(nameof(FormTitle));
        }

        private void OpenEdit(object? parameter)
        {
            var target = (parameter as Donation) ?? SelectedDonation;
            if (target == null) return;
            if (!SessionService.Instance.CanEdit) return;
            EditingDonation = new Donation
            {
                DonationID       = target.DonationID,
                DonorID          = target.DonorID,
                DonorName        = target.DonorName,
                Amount           = target.Amount,
                DonationType     = target.DonationType,
                GoodsDescription = target.GoodsDescription,
                DonationDate     = target.DonationDate,
                Notes            = target.Notes,
            };
            IsEditMode = true; IsFormVisible = true;
            OnPropertyChanged(nameof(FormTitle));
        }

        private void DeleteSelected(object? parameter)
        {
            var target = (parameter as Donation) ?? SelectedDonation;
            if (target == null) return;
            if (!SessionService.Instance.CanDelete) return;
            if (MessageBox.Show("Delete this donation record?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try { _db.DeleteDonation(target.DonationID); LoadData(); }
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

        private void ExportToPdf(object? _)
        {
            var dlg = new SaveFileDialog { Filter = "PDF Files|*.pdf", FileName = "donations_export" };
            if (dlg.ShowDialog() != true) return;
            try
            {
                using var writer = new PdfWriter(dlg.FileName);
                using var pdf    = new PdfDocument(writer);
                using var doc    = new Document(pdf);

                doc.Add(new Paragraph("Donations Report")
                    .SetFontSize(18).SetFontColor(new DeviceRgb(27, 67, 50)).SetMarginBottom(4));
                doc.Add(new Paragraph($"Generated: {DateTime.Now:MMMM dd, yyyy}")
                    .SetFontSize(10).SetFontColor(new DeviceRgb(82, 183, 136)).SetMarginBottom(16));

                var table = new Table(new float[] { 2f, 1.5f, 1f, 2f, 1.2f }).UseAllAvailableWidth();
                var headerBg = new DeviceRgb(27, 67, 50);
                foreach (var h in new[] { "Donor", "Amount", "Type", "Goods Description", "Date" })
                    table.AddHeaderCell(new Cell()
                        .Add(new Paragraph(h).SetFontColor(ColorConstants.WHITE).SetFontSize(10))
                        .SetBackgroundColor(headerBg).SetPadding(8));

                var rowAlt = new DeviceRgb(216, 243, 220);
                bool alt = false;
                foreach (var d in Donations)
                {
                    var bg = alt ? rowAlt : ColorConstants.WHITE;
                    table.AddCell(new Cell().Add(new Paragraph(d.DonorName ?? "").SetFontSize(10)).SetBackgroundColor(bg).SetPadding(6));
                    table.AddCell(new Cell().Add(new Paragraph($"PHP {d.Amount:N2}").SetFontSize(10)).SetBackgroundColor(bg).SetPadding(6));
                    table.AddCell(new Cell().Add(new Paragraph(d.DonationType ?? "").SetFontSize(10)).SetBackgroundColor(bg).SetPadding(6));
                    table.AddCell(new Cell().Add(new Paragraph(d.GoodsDescription ?? "-").SetFontSize(10)).SetBackgroundColor(bg).SetPadding(6));
                    table.AddCell(new Cell().Add(new Paragraph(d.DonationDate.ToString("MM/dd/yyyy")).SetFontSize(10)).SetBackgroundColor(bg).SetPadding(6));
                    alt = !alt;
                }
                doc.Add(table);

                var total = 0m;
                foreach (var d in Donations) total += d.Amount;
                doc.Add(new Paragraph($"Total Donations: PHP {total:N2}")
                    .SetFontSize(12).SetFontColor(new DeviceRgb(27, 67, 50)).SetMarginTop(12));

                MessageBox.Show("PDF exported successfully!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                var msg = ex.Message.Contains("Access to the path") || ex.Message.Contains("denied")
                    ? "Export failed: The file is already open. Please close it and try again."
                    : $"Export failed: {ex.Message}\n\nInner: {ex.InnerException?.Message}";
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}