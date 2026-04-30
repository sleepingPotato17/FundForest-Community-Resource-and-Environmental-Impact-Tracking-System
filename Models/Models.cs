using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FundForest.Helpers;

namespace FundForest.Models
{
    // =============================================
    // Admin
    // =============================================
    public class Admin
    {
        public int AdminID   { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role     { get; set; } = "User";
    }

    // =============================================
    // Beneficiary
    // =============================================
    public class Beneficiary : BaseViewModel
    {
        private int    _beneficiaryID;
        private string _beneficiaryType = "Person";
        private string _fullName        = string.Empty;
        private string _gender          = "Male";
        private int    _age;
        private string _groupName       = string.Empty;
        private int    _numberOfMembers;
        private string _groupRepresentative = string.Empty;
        private string _vulnerabilityType   = string.Empty;
        private string _address         = string.Empty;
        private string _contactInfo     = string.Empty;
        private DateTime? _contactDate;

        public int    BeneficiaryID       { get => _beneficiaryID;       set => SetProperty(ref _beneficiaryID, value); }
        public string BeneficiaryType     { get => _beneficiaryType;     set => SetProperty(ref _beneficiaryType, value); }
        public string FullName            { get => _fullName;            set => SetProperty(ref _fullName, value); }
        public string Gender              { get => _gender;              set => SetProperty(ref _gender, value); }
        public int    Age                 { get => _age;                 set => SetProperty(ref _age, value); }
        public string GroupName           { get => _groupName;           set => SetProperty(ref _groupName, value); }
        public int    NumberOfMembers     { get => _numberOfMembers;     set => SetProperty(ref _numberOfMembers, value); }
        public string GroupRepresentative { get => _groupRepresentative; set => SetProperty(ref _groupRepresentative, value); }
        public string VulnerabilityType   { get => _vulnerabilityType;   set => SetProperty(ref _vulnerabilityType, value); }
        public string Address             { get => _address;             set => SetProperty(ref _address, value); }
        public string ContactInfo         { get => _contactInfo;         set => SetProperty(ref _contactInfo, value); }
        public DateTime? ContactDate      { get => _contactDate;         set => SetProperty(ref _contactDate, value); }

        // Display name used in grids
        public string DisplayName => BeneficiaryType == "Person" ? FullName : GroupName;
    }

    // =============================================
    // Donor
    // =============================================
    public class Donor : BaseViewModel
    {
        private int    _donorID;
        private string _donorName    = string.Empty;
        private string _donationType = "Individual";
        private string _contactInfo  = string.Empty;
        private string _address      = string.Empty;

        public int    DonorID      { get => _donorID;      set => SetProperty(ref _donorID, value); }
        public string DonorName    { get => _donorName;    set => SetProperty(ref _donorName, value); }
        public string DonationType { get => _donationType; set => SetProperty(ref _donationType, value); }
        public string ContactInfo  { get => _contactInfo;  set => SetProperty(ref _contactInfo, value); }
        public string Address      { get => _address;      set => SetProperty(ref _address, value); }
    }

    // =============================================
    // Donation
    // =============================================
    public class Donation : BaseViewModel
    {
        private int      _donationID;
        private int      _donorID;
        private string   _donorName       = string.Empty;
        private decimal  _amount;
        private string   _donationType    = "Cash";
        private string   _goodsDescription = string.Empty;
        private DateTime _donationDate    = DateTime.Today;
        private string   _notes          = string.Empty;

        public int      DonationID        { get => _donationID;        set => SetProperty(ref _donationID, value); }
        public int      DonorID           { get => _donorID;           set => SetProperty(ref _donorID, value); }
        public string   DonorName         { get => _donorName;         set => SetProperty(ref _donorName, value); }
        public decimal  Amount            { get => _amount;            set => SetProperty(ref _amount, value); }
        public string   DonationType      { get => _donationType;      set => SetProperty(ref _donationType, value); }
        public string   GoodsDescription  { get => _goodsDescription;  set => SetProperty(ref _goodsDescription, value); }
        public DateTime DonationDate      { get => _donationDate;      set => SetProperty(ref _donationDate, value); }
        public string   Notes             { get => _notes;             set => SetProperty(ref _notes, value); }
    }

    // =============================================
    // Program
    // =============================================
    public class Program : BaseViewModel
    {
        private int      _programID;
        private string   _programName    = string.Empty;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private string   _targetAudience = "Group";
        private string   _barangay       = string.Empty;
        private string   _description    = string.Empty;
        private string   _status         = "Active";

        public int      ProgramID      { get => _programID;      set => SetProperty(ref _programID, value); }
        public string   ProgramName    { get => _programName;    set => SetProperty(ref _programName, value); }
        public DateTime? StartDate     { get => _startDate;      set => SetProperty(ref _startDate, value); }
        public DateTime? EndDate       { get => _endDate;        set => SetProperty(ref _endDate, value); }
        public string   TargetAudience { get => _targetAudience; set => SetProperty(ref _targetAudience, value); }
        public string   Barangay       { get => _barangay;       set => SetProperty(ref _barangay, value); }
        public string   Description    { get => _description;    set => SetProperty(ref _description, value); }
        public string   Status         { get => _status;         set => SetProperty(ref _status, value); }
    }

    // =============================================
    // Distribution
    // =============================================
    public class Distribution : BaseViewModel
    {
        private int      _distributionID;
        private int      _beneficiaryID;
        private string   _beneficiaryName  = string.Empty;
        private int?     _programID;
        private string   _programName      = string.Empty;
        private DateTime _distributionDate = DateTime.Today;
        private string   _distributedItem  = string.Empty;
        private int      _quantity         = 1;
        private decimal  _amount;
        private string   _status           = "Pending";
        private string   _notes            = string.Empty;

        public int      DistributionID   { get => _distributionID;   set => SetProperty(ref _distributionID, value); }
        public int      BeneficiaryID    { get => _beneficiaryID;    set => SetProperty(ref _beneficiaryID, value); }
        public string   BeneficiaryName  { get => _beneficiaryName;  set => SetProperty(ref _beneficiaryName, value); }
        public int?     ProgramID        { get => _programID;        set => SetProperty(ref _programID, value); }
        public string   ProgramName      { get => _programName;      set => SetProperty(ref _programName, value); }
        public DateTime DistributionDate { get => _distributionDate; set => SetProperty(ref _distributionDate, value); }
        public string   DistributedItem  { get => _distributedItem;  set => SetProperty(ref _distributedItem, value); }
        public int      Quantity         { get => _quantity;         set => SetProperty(ref _quantity, value); }
        public decimal  Amount           { get => _amount;           set => SetProperty(ref _amount, value); }
        public string   Status           { get => _status;           set => SetProperty(ref _status, value); }
        public string   Notes            { get => _notes;            set => SetProperty(ref _notes, value); }
    }

    // =============================================
    // Dashboard summary
    // =============================================
    public class DashboardStats
    {
        public int     TotalBeneficiaries  { get; set; }
        public int     TotalPrograms       { get; set; }
        public int     TotalDonors         { get; set; }
        public int     TotalDonations      { get; set; }
        public int     TotalDistributions  { get; set; }
        public decimal TotalAmountDistributed { get; set; }
    }

    public class MonthlyActivity
    {
        public string Month     { get; set; } = string.Empty;
        public double Donations { get; set; }
        public double Distributions { get; set; }
    }
}
