using System.Windows;
using FundForest.Services;

namespace FundForest
{
    
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
    

            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"Unexpected error: {args.Exception.Message}", "FundForest Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            try
            {
                var db = new DatabaseService();
                db.SeedAdminIfMissing();
                db.ResetAdminPassword(); // remove this line after first run
            }
            catch { }
        }
    }
}