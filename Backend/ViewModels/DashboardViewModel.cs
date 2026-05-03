using System.Collections.Generic;
using System.Collections.ObjectModel;
using FundForest.Helpers;
using FundForest.Models;
using FundForest.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace FundForest.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly DatabaseService _db = new();

        private int     _totalBeneficiaries;
        private int     _totalPrograms;
        private int     _totalDonors;
        private int     _totalDonations;
        private int     _totalDistributions;
        private decimal _totalAmountDistributed;

        public int     TotalBeneficiaries      { get => _totalBeneficiaries;      set => SetProperty(ref _totalBeneficiaries, value); }
        public int     TotalPrograms           { get => _totalPrograms;           set => SetProperty(ref _totalPrograms, value); }
        public int     TotalDonors             { get => _totalDonors;             set => SetProperty(ref _totalDonors, value); }
        public int     TotalDonations          { get => _totalDonations;          set => SetProperty(ref _totalDonations, value); }
        public int     TotalDistributions      { get => _totalDistributions;      set => SetProperty(ref _totalDistributions, value); }
        public decimal TotalAmountDistributed  { get => _totalAmountDistributed;  set => SetProperty(ref _totalAmountDistributed, value); }

        // Chart
        public ISeries[]     ChartSeries { get; private set; } = [];
        public Axis[]        XAxes       { get; private set; } = [];
        public Axis[]        YAxes       { get; private set; } = [];

        public RelayCommand RefreshCommand { get; }

        public DashboardViewModel()
        {
            RefreshCommand = new RelayCommand(LoadData);
            LoadData();
        }

        private void LoadData(object? _ = null)
        {
            try
            {
                var stats = _db.GetDashboardStats();
                TotalBeneficiaries     = stats.TotalBeneficiaries;
                TotalPrograms          = stats.TotalPrograms;
                TotalDonors            = stats.TotalDonors;
                TotalDonations         = stats.TotalDonations;
                TotalDistributions     = stats.TotalDistributions;
                TotalAmountDistributed = stats.TotalAmountDistributed;

                LoadChart();
            }
            catch { /* handle gracefully in production */ }
        }

        private void LoadChart()
        {
            var monthly = _db.GetMonthlyActivity();
            var labels  = new List<string>();
            var donations = new List<double>();
            var dists     = new List<double>();

            foreach (var m in monthly)
            {
                labels.Add(m.Month);
                donations.Add(m.Donations);
                dists.Add(m.Distributions);
            }

            ChartSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Name   = "Donations",
                    Values = donations,
                    Stroke = new SolidColorPaint(SKColor.Parse("#2D6A4F"), 2.5f),
                    Fill   = new SolidColorPaint(SKColor.Parse("#2D6A4F").WithAlpha(30)),
                    GeometrySize  = 6,
                    GeometryFill  = new SolidColorPaint(SKColor.Parse("#2D6A4F")),
                    GeometryStroke= new SolidColorPaint(SKColor.Parse("#FFFFFF"), 1.5f),
                },
                new ColumnSeries<double>
                {
                    Name   = "Distributions",
                    Values = dists,
                    Fill   = new SolidColorPaint(SKColor.Parse("#74C69D").WithAlpha(160)),
                    Rx     = 4,
                    Ry     = 4,
                },
            };

            XAxes = new[]
            {
                new Axis
                {
                    Labels       = labels,
                    TextSize     = 11,
                    LabelsPaint  = new SolidColorPaint(SKColor.Parse("#4A6741")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E8F5EB")),
                }
            };

            YAxes = new[]
            {
                new Axis
                {
                    TextSize     = 11,
                    LabelsPaint  = new SolidColorPaint(SKColor.Parse("#4A6741")),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E8F5EB")),
                    Labeler      = v => $"₱{v:N0}",
                }
            };

            OnPropertyChanged(nameof(ChartSeries));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
        }
    }
}
