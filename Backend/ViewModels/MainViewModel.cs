using FundForest.Helpers;
using FundForest.Services;

namespace FundForest.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentPage;
        private string _currentPageName = "Dashboard";

        public BaseViewModel CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public string CurrentPageName
        {
            get => _currentPageName;
            set => SetProperty(ref _currentPageName, value);
        }

        public string AdminName => SessionService.Instance.CurrentAdmin?.FullName ?? "Admin";

        // Navigation commands
        public RelayCommand NavDashboardCommand     { get; }
        public RelayCommand NavBeneficiariesCommand { get; }
        public RelayCommand NavProgramsCommand      { get; }
        public RelayCommand NavDonorsCommand        { get; }
        public RelayCommand NavDonationsCommand     { get; }
        public RelayCommand NavDistributionCommand  { get; }
        public RelayCommand LogoutCommand           { get; }

        public MainViewModel()
        {
            NavDashboardCommand     = new RelayCommand(() => Navigate("Dashboard"));
            NavBeneficiariesCommand = new RelayCommand(() => Navigate("Beneficiaries"));
            NavProgramsCommand      = new RelayCommand(() => Navigate("Programs"));
            NavDonorsCommand        = new RelayCommand(() => Navigate("Donors"));
            NavDonationsCommand     = new RelayCommand(() => Navigate("Donations"));
            NavDistributionCommand  = new RelayCommand(() => Navigate("Distribution"));
            LogoutCommand           = new RelayCommand(Logout);

            // Default page
            _currentPage = new DashboardViewModel();
        }

        private void Navigate(string page)
        {
            CurrentPageName = page;
            CurrentPage = page switch
            {
                "Beneficiaries" => new BeneficiariesViewModel(),
                "Programs"      => new ProgramsViewModel(),
                "Donors"        => new DonorsViewModel(),
                "Donations"     => new DonationsViewModel(),
                "Distribution"  => new DistributionViewModel(),
                _               => new DashboardViewModel(),
            };
        }

        private void Logout(object? _)
        {
            SessionService.Instance.CurrentAdmin = null;
            var login = new Views.LoginWindow();
            login.Show();
            foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
                if (w is Views.MainWindow) { w.Close(); break; }
        }
    }
}
