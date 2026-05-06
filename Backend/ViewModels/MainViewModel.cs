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

        public string AdminName  => SessionService.Instance.CurrentAdmin?.FullName ?? "User";
        public string AdminRole  => SessionService.Instance.CurrentAdmin?.Role ?? "";
        public bool   IsAdmin    => SessionService.Instance.IsAdmin;

        public int TotalDonations { get; private set; }
        public int TotalPrograms  { get; private set; }

        public RelayCommand NavDashboardCommand     { get; }
        public RelayCommand NavBeneficiariesCommand { get; }
        public RelayCommand NavProgramsCommand      { get; }
        public RelayCommand NavDonorsCommand        { get; }
        public RelayCommand NavDonationsCommand     { get; }
        public RelayCommand NavDistributionCommand  { get; }
        public RelayCommand NavManageUsersCommand   { get; }
        public RelayCommand LogoutCommand           { get; }

        public MainViewModel()
        {

            NavDashboardCommand     = new RelayCommand(() => Navigate("Dashboard"));
            NavBeneficiariesCommand = new RelayCommand(() => Navigate("Beneficiaries"));
            NavProgramsCommand      = new RelayCommand(() => Navigate("Programs"));
            NavDonorsCommand        = new RelayCommand(() => Navigate("Donors"));
            NavDonationsCommand     = new RelayCommand(() => Navigate("Donations"));
            NavDistributionCommand  = new RelayCommand(() => Navigate("Distribution"));
            NavManageUsersCommand   = new RelayCommand(() => Navigate("ManageUsers"));
            LogoutCommand           = new RelayCommand(Logout);

            _currentPage = new DashboardViewModel();
            LoadSidebarStats();
        }

        private void LoadSidebarStats()
        {
            try
            {
                var db    = new Services.DatabaseService();
                var stats = db.GetDashboardStats();
                TotalDonations = stats.TotalDonations;
                TotalPrograms  = stats.TotalPrograms;
                OnPropertyChanged(nameof(TotalDonations));
                OnPropertyChanged(nameof(TotalPrograms));
            }
            catch { }
        }

        private void Navigate(string page)
        {
            if (page == "ManageUsers" && !SessionService.Instance.IsAdmin) return;

            CurrentPageName = page;
            CurrentPage = page switch
            {
                "Beneficiaries" => new BeneficiariesViewModel(),
                "Programs"      => new ProgramsViewModel(),
                "Donors"        => new DonorsViewModel(),
                "Donations"     => new DonationsViewModel(),
                "Distribution"  => new DistributionViewModel(),
                "ManageUsers"   => new ManageUsersViewModel(),
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