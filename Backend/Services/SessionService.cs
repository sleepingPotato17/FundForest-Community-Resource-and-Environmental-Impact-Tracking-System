using FundForest.Models;

namespace FundForest.Services
{
    public class SessionService
    {
        private static SessionService? _instance;
        public static SessionService Instance => _instance ??= new SessionService();
        private SessionService() { }

        public Admin? CurrentAdmin { get; set; }

        public bool IsAdmin => CurrentAdmin?.Role == "Admin";
        public bool IsStaff => CurrentAdmin?.Role == "Staff";
        public bool IsLocal => CurrentAdmin?.Role == "Local";
        public bool CanEdit => IsAdmin || IsStaff;
        public bool CanDelete => IsAdmin;
        public bool CanManageUsers => IsAdmin;
    }
}