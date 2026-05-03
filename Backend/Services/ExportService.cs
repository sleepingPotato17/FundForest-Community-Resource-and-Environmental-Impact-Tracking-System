using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using FundForest.Models;

namespace FundForest.Services
{
    /// <summary>
    /// Handles CSV and future Excel export operations.
    /// </summary>
    public static class ExportService
    {
        public static void ExportDonationsToCsv(IEnumerable<Donation> donations)
        {
            var dlg = new SaveFileDialog
            {
                Filter   = "CSV Files (*.csv)|*.csv",
                FileName = $"donations_{System.DateTime.Today:yyyyMMdd}.csv"
            };
            if (dlg.ShowDialog() != true) return;

            var sb = new StringBuilder();
            sb.AppendLine("Donor Name,Amount (PHP),Type,Goods Description,Date,Notes");
            foreach (var d in donations)
                sb.AppendLine($"\"{d.DonorName}\",{d.Amount},{d.DonationType}," +
                              $"\"{d.GoodsDescription}\",{d.DonationDate:yyyy-MM-dd},\"{d.Notes}\"");

            File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Export completed successfully!", "Export",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ExportDistributionsToCsv(IEnumerable<Distribution> distributions)
        {
            var dlg = new SaveFileDialog
            {
                Filter   = "CSV Files (*.csv)|*.csv",
                FileName = $"distributions_{System.DateTime.Today:yyyyMMdd}.csv"
            };
            if (dlg.ShowDialog() != true) return;

            var sb = new StringBuilder();
            sb.AppendLine("Beneficiary,Program,Item,Quantity,Amount (PHP),Date,Status,Notes");
            foreach (var d in distributions)
                sb.AppendLine($"\"{d.BeneficiaryName}\",\"{d.ProgramName}\"," +
                              $"\"{d.DistributedItem}\",{d.Quantity},{d.Amount}," +
                              $"{d.DistributionDate:yyyy-MM-dd},{d.Status},\"{d.Notes}\"");

            File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Export completed successfully!", "Export",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
