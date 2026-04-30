using System.Windows;

namespace FundForest
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Global exception handler
            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"Unexpected error: {args.Exception.Message}", "FundForest Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
}
