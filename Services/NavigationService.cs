using System;
using FundForest.Helpers;

namespace FundForest.Services
{
    /// <summary>
    /// Simple navigation service using event-based page switching.
    /// The MainViewModel subscribes and swaps the CurrentView.
    /// </summary>
    public class NavigationService
    {
        public event Action<string>? NavigationRequested;

        public void NavigateTo(string pageName) => NavigationRequested?.Invoke(pageName);
    }

    /// <summary>
    /// Singleton session – holds logged-in admin info.
    /// </summary>
    public class SessionService
    {
        private static SessionService? _instance;
        public static SessionService Instance => _instance ??= new SessionService();

        public Models.Admin? CurrentAdmin { get; set; }
        public bool IsLoggedIn => CurrentAdmin != null;
        public bool IsAdmin    => CurrentAdmin?.Role == "Admin";
    }
}
