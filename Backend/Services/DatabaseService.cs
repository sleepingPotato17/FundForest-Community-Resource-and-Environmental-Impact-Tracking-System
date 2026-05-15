using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using FundForest.Models;

namespace FundForest.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString =
            "Server=localhost;Port=3306;Database=fundforest;" +
            "Uid=fundforest;Pwd=ff_pass_2024;CharSet=utf8mb4;";

                private MySqlConnection GetConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        // =====================================================================
        // AUTHENTICATION
        // =====================================================================
        public Admin? ValidateLogin(string username, string password)
        {
            using var conn = GetConnection();
            using var cmd  = new MySqlCommand(
                "SELECT AdminID, Username, Password, FullName, Role, IsApproved FROM Admins WHERE Username=@u LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@u", username);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;

            var storedHash = rd.GetString("Password");
            bool valid = BCrypt.Net.BCrypt.Verify(password, storedHash);
            if (!valid) return null;

            var admin = new Admin();
            admin.AdminID    = rd.GetInt32("AdminID");
            admin.Username   = rd.GetString("Username");
            admin.FullName   = rd.IsDBNull("FullName") ? "" : rd.GetString("FullName");
            admin.Role       = rd.GetString("Role");
            admin.IsApproved = rd.GetInt32("IsApproved") == 1;
            return admin;
        }

        // =====================================================================
        // ADMIN SEEDING
        // =====================================================================
        public void SeedAdminIfMissing()
        {
            using var conn = GetConnection();
            using var checkCmd = new MySqlCommand(
                "SELECT COUNT(*) FROM Admins WHERE Role='Admin'", conn);
            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0) return;

            string hashed = BCrypt.Net.BCrypt.HashPassword("admin123");
            using var cmd = new MySqlCommand(@"
                INSERT INTO Admins (Username, Password, FullName, Role, IsApproved)
                VALUES ('admin', @p, 'System Administrator', 'Admin', 1)", conn);
            cmd.Parameters.AddWithValue("@p", hashed);
            cmd.ExecuteNonQuery();
        }

        // =====================================================================
        // ACCOUNT REGISTRATION
        // =====================================================================
        public bool UsernameExists(string username)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM Admins WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public bool CreateAdmin(string username, string hashedPassword, string fullName, string role)
        {
            if (role == "Admin")
                throw new InvalidOperationException("Admin accounts cannot be created through registration.");
            if (role != "Staff" && role != "Local")
                throw new InvalidOperationException($"Invalid role: {role}");

            using var conn = GetConnection();
            using var cmd = new MySqlCommand(@"
                INSERT INTO Admins (Username, Password, FullName, Role, IsApproved)
                VALUES (@u, @p, @f, @r, 0)", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", hashedPassword);
            cmd.Parameters.AddWithValue("@f", fullName);
            cmd.Parameters.AddWithValue("@r", role);
            return cmd.ExecuteNonQuery() > 0;
        }

        // =====================================================================
        // USER MANAGEMENT (Admin only)
        // =====================================================================
        public List<Admin> GetAllUsers()
        {
            var list = new List<Admin>();
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "SELECT AdminID, Username, FullName, Role, IsApproved, CreatedAt FROM Admins WHERE Role != 'Admin' ORDER BY IsApproved ASC, CreatedAt DESC", conn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                list.Add(new Admin
                {
                    AdminID    = rd.GetInt32("AdminID"),
                    Username   = rd.GetString("Username"),
                    FullName   = rd.IsDBNull("FullName") ? "" : rd.GetString("FullName"),
                    Role       = rd.GetString("Role"),
                    IsApproved = rd.GetInt32("IsApproved") == 1,
                    CreatedAt  = rd.IsDBNull("CreatedAt") ? (DateTime?)null : rd.GetDateTime("CreatedAt"),
                });
            return list;
        }

        public void ApproveUser(int adminId)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "UPDATE Admins SET IsApproved=1 WHERE AdminID=@id AND Role != 'Admin'", conn);
            cmd.Parameters.AddWithValue("@id", adminId);
            cmd.ExecuteNonQuery();
        }

        public void RejectUser(int adminId)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "DELETE FROM Admins WHERE AdminID=@id AND Role != 'Admin'", conn);
            cmd.Parameters.AddWithValue("@id", adminId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int adminId)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "DELETE FROM Admins WHERE AdminID=@id AND Role != 'Admin'", conn);
            cmd.Parameters.AddWithValue("@id", adminId);
            cmd.ExecuteNonQuery();
        }

        // =====================================================================
        // DASHBOARD
        // =====================================================================
        public DashboardStats GetDashboardStats()
        {
            using var conn = GetConnection();
            var stats = new DashboardStats();
            string sql = @"
                SELECT
                  (SELECT COUNT(*) FROM Beneficiaries)                        AS Bens,
                  (SELECT COUNT(*) FROM Programs WHERE Status='Active')       AS Progs,
                  (SELECT COUNT(*) FROM Donors)                               AS Donors,
                  (SELECT COUNT(*) FROM Donations)                            AS Donations,
                  (SELECT COUNT(*) FROM Distributions)                        AS Dists,
                  (SELECT COALESCE(SUM(Amount),0) FROM Distributions
                   WHERE Status='Completed')                                   AS AmtDist";
            using var cmd = new MySqlCommand(sql, conn);
            using var rd  = cmd.ExecuteReader();
            if (rd.Read())
            {
                stats.TotalBeneficiaries     = rd.GetInt32("Bens");
                stats.TotalPrograms          = rd.GetInt32("Progs");
                stats.TotalDonors            = rd.GetInt32("Donors");
                stats.TotalDonations         = rd.GetInt32("Donations");
                stats.TotalDistributions     = rd.GetInt32("Dists");
                stats.TotalAmountDistributed = rd.GetDecimal("AmtDist");
            }
            return stats;
        }

        public List<MonthlyActivity> GetMonthlyActivity()
        {
            var result = new List<MonthlyActivity>();
            using var conn = GetConnection();
            string sql = @"
                SELECT m.Mon,
                       COALESCE(d.Total,0)  AS Donations,
                       COALESCE(dt.Total,0) AS Distributions
                FROM (
                    SELECT DATE_FORMAT(DATE_SUB(CURDATE(), INTERVAL n MONTH),'%b %Y') AS Mon,
                           DATE_FORMAT(DATE_SUB(CURDATE(), INTERVAL n MONTH),'%Y-%m') AS YM
                    FROM (SELECT 5 n UNION SELECT 4 UNION SELECT 3 UNION SELECT 2 UNION SELECT 1 UNION SELECT 0) t
                ) m
                LEFT JOIN (
                    SELECT DATE_FORMAT(DonationDate,'%Y-%m') AS YM, SUM(Amount) AS Total
                    FROM Donations GROUP BY YM
                ) d  ON d.YM = m.YM
                LEFT JOIN (
                    SELECT DATE_FORMAT(DistributionDate,'%Y-%m') AS YM, SUM(Amount) AS Total
                    FROM Distributions GROUP BY YM
                ) dt ON dt.YM = m.YM
                ORDER BY m.YM";
            using var cmd = new MySqlCommand(sql, conn);
            using var rd  = cmd.ExecuteReader();
            while (rd.Read())
                result.Add(new MonthlyActivity
                {
                    Month         = rd.GetString("Mon"),
                    Donations     = Convert.ToDouble(rd["Donations"]),
                    Distributions = Convert.ToDouble(rd["Distributions"]),
                });
            return result;
        }

        // =====================================================================
        // BENEFICIARIES
        // =====================================================================
        public List<Beneficiary> GetBeneficiaries(string search = "")
        {
            var list = new List<Beneficiary>();
            using var conn = GetConnection();
            string sql = @"
                SELECT * FROM Beneficiaries
                WHERE FullName LIKE @s OR GroupName LIKE @s OR ContactInfo LIKE @s
                ORDER BY BeneficiaryID DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", $"%{search}%");
            using var rd = cmd.ExecuteReader();
            while (rd.Read()) list.Add(ReadBeneficiary(rd));
            return list;
        }

        public void InsertBeneficiary(Beneficiary b)
        {
            using var conn = GetConnection();
            string sql = @"
                INSERT INTO Beneficiaries
                (BeneficiaryType,FullName,Gender,Age,GroupName,NumberOfMembers,GroupRepresentative,
                 VulnerabilityType,Address,ContactInfo,ContactDate)
                VALUES(@t,@fn,@g,@a,@gn,@nm,@gr,@vt,@addr,@ci,@cd)";
            using var cmd = new MySqlCommand(sql, conn);
            BindBeneficiary(cmd, b);
            cmd.ExecuteNonQuery();
        }

        public void UpdateBeneficiary(Beneficiary b)
        {
            using var conn = GetConnection();
            string sql = @"
                UPDATE Beneficiaries SET
                  BeneficiaryType=@t, FullName=@fn, Gender=@g, Age=@a,
                  GroupName=@gn, NumberOfMembers=@nm, GroupRepresentative=@gr,
                  VulnerabilityType=@vt, Address=@addr, ContactInfo=@ci, ContactDate=@cd
                WHERE BeneficiaryID=@id";
            using var cmd = new MySqlCommand(sql, conn);
            BindBeneficiary(cmd, b);
            cmd.Parameters.AddWithValue("@id", b.BeneficiaryID);
            cmd.ExecuteNonQuery();
        }

        public void DeleteBeneficiary(int id)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand("DELETE FROM Beneficiaries WHERE BeneficiaryID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private static Beneficiary ReadBeneficiary(MySqlDataReader rd) => new()
        {
            BeneficiaryID       = rd.GetInt32("BeneficiaryID"),
            BeneficiaryType     = rd.GetString("BeneficiaryType"),
            FullName            = rd.IsDBNull("FullName")            ? "" : rd.GetString("FullName"),
            Gender              = rd.IsDBNull("Gender")              ? "" : rd.GetString("Gender"),
            Age                 = rd.IsDBNull("Age")                 ? 0  : rd.GetInt32("Age"),
            GroupName           = rd.IsDBNull("GroupName")           ? "" : rd.GetString("GroupName"),
            NumberOfMembers     = rd.IsDBNull("NumberOfMembers")     ? 0  : rd.GetInt32("NumberOfMembers"),
            GroupRepresentative = rd.IsDBNull("GroupRepresentative") ? "" : rd.GetString("GroupRepresentative"),
            VulnerabilityType   = rd.IsDBNull("VulnerabilityType")   ? "" : rd.GetString("VulnerabilityType"),
            Address             = rd.IsDBNull("Address")             ? "" : rd.GetString("Address"),
            ContactInfo         = rd.IsDBNull("ContactInfo")         ? "" : rd.GetString("ContactInfo"),
            ContactDate         = rd.IsDBNull("ContactDate")         ? (DateTime?)null : rd.GetDateTime("ContactDate"),
        };

        private static void BindBeneficiary(MySqlCommand cmd, Beneficiary b)
        {
            cmd.Parameters.AddWithValue("@t",    b.BeneficiaryType);
            cmd.Parameters.AddWithValue("@fn",   b.FullName);
            cmd.Parameters.AddWithValue("@g",    b.Gender);
            cmd.Parameters.AddWithValue("@a",    b.Age);
            cmd.Parameters.AddWithValue("@gn",   b.GroupName);
            cmd.Parameters.AddWithValue("@nm",   b.NumberOfMembers);
            cmd.Parameters.AddWithValue("@gr",   b.GroupRepresentative);
            cmd.Parameters.AddWithValue("@vt",   b.VulnerabilityType);
            cmd.Parameters.AddWithValue("@addr", b.Address);
            cmd.Parameters.AddWithValue("@ci",   b.ContactInfo);
            cmd.Parameters.AddWithValue("@cd",   (object?)b.ContactDate ?? DBNull.Value);
        }

        // =====================================================================
        // DONORS
        // =====================================================================
        public List<Donor> GetDonors(string search = "")
        {
            var list = new List<Donor>();
            using var conn = GetConnection();
            string sql = "SELECT * FROM Donors WHERE DonorName LIKE @s OR ContactInfo LIKE @s ORDER BY DonorID DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", $"%{search}%");
            using var rd = cmd.ExecuteReader();
            while (rd.Read()) list.Add(new Donor
            {
                DonorID      = rd.GetInt32("DonorID"),
                DonorName    = rd.GetString("DonorName"),
                DonationType = rd.GetString("DonationType"),
                ContactInfo  = rd.IsDBNull("ContactInfo") ? "" : rd.GetString("ContactInfo"),
                Address      = rd.IsDBNull("Address")     ? "" : rd.GetString("Address"),
            });
            return list;
        }

        public void InsertDonor(Donor d)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "INSERT INTO Donors (DonorName,DonationType,ContactInfo,Address) VALUES(@n,@t,@c,@a)", conn);
            BindDonor(cmd, d); cmd.ExecuteNonQuery();
        }

        public void UpdateDonor(Donor d)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "UPDATE Donors SET DonorName=@n,DonationType=@t,ContactInfo=@c,Address=@a WHERE DonorID=@id", conn);
            BindDonor(cmd, d);
            cmd.Parameters.AddWithValue("@id", d.DonorID);
            cmd.ExecuteNonQuery();
        }

        public void DeleteDonor(int id)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand("DELETE FROM Donors WHERE DonorID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery();
        }

        private static void BindDonor(MySqlCommand cmd, Donor d)
        {
            cmd.Parameters.AddWithValue("@n", d.DonorName);
            cmd.Parameters.AddWithValue("@t", d.DonationType);
            cmd.Parameters.AddWithValue("@c", d.ContactInfo);
            cmd.Parameters.AddWithValue("@a", d.Address);
        }

        // =====================================================================
        // DONATIONS
        // =====================================================================
        public List<Donation> GetDonations(string search = "", DateTime? from = null, DateTime? to = null)
        {
            var list = new List<Donation>();
            using var conn = GetConnection();
            string sql = @"
                SELECT dn.*, d.DonorName FROM Donations dn
                JOIN Donors d ON d.DonorID = dn.DonorID
                WHERE (d.DonorName LIKE @s)
                  AND (@f IS NULL OR dn.DonationDate >= @f)
                  AND (@t IS NULL OR dn.DonationDate <= @t)
                ORDER BY dn.DonationID DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", $"%{search}%");
            cmd.Parameters.AddWithValue("@f", (object?)from ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@t", (object?)to   ?? DBNull.Value);
            using var rd = cmd.ExecuteReader();
            while (rd.Read()) list.Add(new Donation
            {
                DonationID       = rd.GetInt32("DonationID"),
                DonorID          = rd.GetInt32("DonorID"),
                DonorName        = rd.GetString("DonorName"),
                Amount           = rd.GetDecimal("Amount"),
                DonationType     = rd.GetString("DonationType"),
                GoodsDescription = rd.IsDBNull("GoodsDescription") ? "" : rd.GetString("GoodsDescription"),
                DonationDate     = rd.GetDateTime("DonationDate"),
                Notes            = rd.IsDBNull("Notes") ? "" : rd.GetString("Notes"),
            });
            return list;
        }

        public void InsertDonation(Donation d)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(@"
                INSERT INTO Donations (DonorID,Amount,DonationType,GoodsDescription,DonationDate,Notes)
                VALUES(@did,@amt,@t,@gd,@date,@n)", conn);
            BindDonation(cmd, d); cmd.ExecuteNonQuery();
        }

        public void UpdateDonation(Donation d)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(@"
                UPDATE Donations SET DonorID=@did,Amount=@amt,DonationType=@t,
                GoodsDescription=@gd,DonationDate=@date,Notes=@n WHERE DonationID=@id", conn);
            BindDonation(cmd, d);
            cmd.Parameters.AddWithValue("@id", d.DonationID);
            cmd.ExecuteNonQuery();
        }

        public void DeleteDonation(int id)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand("DELETE FROM Donations WHERE DonationID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery();
        }

        private static void BindDonation(MySqlCommand cmd, Donation d)
        {
            cmd.Parameters.AddWithValue("@did",  d.DonorID);
            cmd.Parameters.AddWithValue("@amt",  d.Amount);
            cmd.Parameters.AddWithValue("@t",    d.DonationType);
            cmd.Parameters.AddWithValue("@gd",   d.GoodsDescription);
            cmd.Parameters.AddWithValue("@date", d.DonationDate);
            cmd.Parameters.AddWithValue("@n",    d.Notes);
        }

        // =====================================================================
        // PROGRAMS
        // =====================================================================
        public List<Models.Program> GetPrograms(string search = "")
        {
            var list = new List<Models.Program>();
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "SELECT * FROM Programs WHERE ProgramName LIKE @s ORDER BY ProgramID DESC", conn);
            cmd.Parameters.AddWithValue("@s", $"%{search}%");
            using var rd = cmd.ExecuteReader();
            while (rd.Read()) list.Add(new Models.Program
            {
                ProgramID      = rd.GetInt32("ProgramID"),
                ProgramName    = rd.GetString("ProgramName"),
                StartDate      = rd.IsDBNull("StartDate") ? (DateTime?)null : rd.GetDateTime("StartDate"),
                EndDate        = rd.IsDBNull("EndDate")   ? (DateTime?)null : rd.GetDateTime("EndDate"),
                TargetAudience = rd.GetString("TargetAudience"),
                Barangay       = rd.IsDBNull("Barangay")    ? "" : rd.GetString("Barangay"),
                Description    = rd.IsDBNull("Description") ? "" : rd.GetString("Description"),
                Status         = rd.GetString("Status"),
            });
            return list;
        }

        public void InsertProgram(Models.Program p)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(@"
                INSERT INTO Programs (ProgramName,StartDate,EndDate,TargetAudience,Barangay,Description,Status)
                VALUES(@n,@sd,@ed,@ta,@br,@desc,@st)", conn);
            BindProgram(cmd, p); cmd.ExecuteNonQuery();
        }

        public void UpdateProgram(Models.Program p)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(@"
                UPDATE Programs SET ProgramName=@n,StartDate=@sd,EndDate=@ed,
                TargetAudience=@ta,Barangay=@br,Description=@desc,Status=@st WHERE ProgramID=@id", conn);
            BindProgram(cmd, p);
            cmd.Parameters.AddWithValue("@id", p.ProgramID);
            cmd.ExecuteNonQuery();
        }

        public void DeleteProgram(int id)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand("DELETE FROM Programs WHERE ProgramID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery();
        }

        private static void BindProgram(MySqlCommand cmd, Models.Program p)
        {
            cmd.Parameters.AddWithValue("@n",    p.ProgramName);
            cmd.Parameters.AddWithValue("@sd",   (object?)p.StartDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ed",   (object?)p.EndDate   ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ta",   p.TargetAudience);
            cmd.Parameters.AddWithValue("@br",   p.Barangay);
            cmd.Parameters.AddWithValue("@desc", p.Description);
            cmd.Parameters.AddWithValue("@st",   p.Status);
        }

        // =====================================================================
        // DISTRIBUTIONS
        // =====================================================================
        public List<Distribution> GetDistributions(string search = "")
        {
            var list = new List<Distribution>();
            using var conn = GetConnection();
            string sql = @"
                SELECT dt.*,
                       COALESCE(b.FullName, b.GroupName) AS BenName,
                       COALESCE(p.ProgramName,'—') AS ProgName
                FROM Distributions dt
                JOIN Beneficiaries b ON b.BeneficiaryID = dt.BeneficiaryID
                LEFT JOIN Programs p ON p.ProgramID = dt.ProgramID
                WHERE COALESCE(b.FullName, b.GroupName) LIKE @s
                   OR dt.DistributedItem LIKE @s
                ORDER BY dt.DistributionID DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", $"%{search}%");
            using var rd = cmd.ExecuteReader();
            while (rd.Read()) list.Add(new Distribution
            {
                DistributionID   = rd.GetInt32("DistributionID"),
                BeneficiaryID    = rd.GetInt32("BeneficiaryID"),
                BeneficiaryName  = rd.GetString("BenName"),
                ProgramID        = rd.IsDBNull("ProgramID") ? (int?)null : rd.GetInt32("ProgramID"),
                ProgramName      = rd.GetString("ProgName"),
                DistributionDate = rd.GetDateTime("DistributionDate"),
                DistributedItem  = rd.IsDBNull("DistributedItem") ? "" : rd.GetString("DistributedItem"),
                Quantity         = rd.GetInt32("Quantity"),
                Amount           = rd.GetDecimal("Amount"),
                Status           = rd.GetString("Status"),
                Notes            = rd.IsDBNull("Notes") ? "" : rd.GetString("Notes"),
            });
            return list;
        }

        public void InsertDistribution(Distribution d)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(@"
                INSERT INTO Distributions
                (BeneficiaryID,ProgramID,DistributionDate,DistributedItem,Quantity,Amount,Status,Notes)
                VALUES(@bid,@pid,@date,@item,@qty,@amt,@st,@n)", conn);
            BindDistribution(cmd, d); cmd.ExecuteNonQuery();
        }

        public void UpdateDistribution(Distribution d)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(@"
                UPDATE Distributions SET BeneficiaryID=@bid,ProgramID=@pid,DistributionDate=@date,
                DistributedItem=@item,Quantity=@qty,Amount=@amt,Status=@st,Notes=@n
                WHERE DistributionID=@id", conn);
            BindDistribution(cmd, d);
            cmd.Parameters.AddWithValue("@id", d.DistributionID);
            cmd.ExecuteNonQuery();
        }

        public void DeleteDistribution(int id)
        {
            using var conn = GetConnection();
            using var cmd = new MySqlCommand("DELETE FROM Distributions WHERE DistributionID=@id", conn);
            cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery();
        }

        private static void BindDistribution(MySqlCommand cmd, Distribution d)
        {
            cmd.Parameters.AddWithValue("@bid",  d.BeneficiaryID);
            cmd.Parameters.AddWithValue("@pid",  (object?)d.ProgramID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@date", d.DistributionDate);
            cmd.Parameters.AddWithValue("@item", d.DistributedItem);
            cmd.Parameters.AddWithValue("@qty",  d.Quantity);
            cmd.Parameters.AddWithValue("@amt",  d.Amount);
            cmd.Parameters.AddWithValue("@st",   d.Status);
            cmd.Parameters.AddWithValue("@n",    d.Notes);
        }

        // =====================================================================
        // EXPORT HELPERS
        // =====================================================================
        public DataTable GetDonationsAsTable()
        {
            var dt = new DataTable();
            using var conn = GetConnection();
            string sql = @"SELECT d.DonorName,dn.Amount,dn.DonationType,dn.GoodsDescription,dn.DonationDate
                           FROM Donations dn JOIN Donors d ON d.DonorID=dn.DonorID ORDER BY dn.DonationDate DESC";
            using var adapter = new MySqlDataAdapter(sql, conn);
            adapter.Fill(dt);
            return dt;
        }

        // TEMPORARY — remove after first successful login
        public void ResetAdminPassword()
        {
            string hashed = BCrypt.Net.BCrypt.HashPassword("admin123");
            using var conn = GetConnection();
            using var cmd = new MySqlCommand(
                "UPDATE Admins SET Password=@p, IsApproved=1 WHERE Username='admin'", conn);
            cmd.Parameters.AddWithValue("@p", hashed);
            cmd.ExecuteNonQuery();
        }
    }
}
