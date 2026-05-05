# 🌿FundForest
## _Community Resource and Environmental Impact Tracking System_
<p align="justify">
FundForest is a centralized platform designed to improve transparency, accountability, and efficiency in community projects and donation drives. It allows administrators, donors, and volunteers to monitor the flow of funds and resources in real time.

This system addresses the common issue of mismanaged data, lack of tracking, and low public trust by replacing manual processes with a structured and reliable digital solution.
</p>
---

## 📁 Project Structure

```
FundForest/
├── Models/
│   └── Models.cs               # All entity models (Beneficiary, Donor, Donation, Program, Distribution, Admin)
├── Views/
│   ├── LoginWindow.xaml(.cs)   # Login screen
│   ├── MainWindow.xaml(.cs)    # Shell with sidebar navigation
│   ├── DashboardView.xaml(.cs) # Dashboard with stat cards + chart
│   ├── BeneficiariesView.xaml(.cs)
│   ├── DonorsView.xaml(.cs)
│   ├── DonationsView.xaml(.cs)
│   ├── ProgramsView.xaml(.cs)
│   └── DistributionView.xaml(.cs)
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── MainViewModel.cs        # Navigation hub
│   ├── DashboardViewModel.cs
│   └── ModuleViewModels.cs     # Donors, Donations, Programs, Distribution VMs
├── Services/
│   ├── DatabaseService.cs      # All MySQL CRUD operations
│   ├── NavigationService.cs    # Navigation + Session
│   └── ExportService.cs        # CSV export
├── Helpers/
│   └── Helpers.cs              # RelayCommand, BaseViewModel, Value Converters
├── Resources/
│   ├── Colors.xaml             # Forest green color palette
│   └── Styles.xaml             # Global WPF styles
├── database_schema.sql         # MySQL schema + seed data
├── app.manifest                # DPI awareness
└── FundForest.csproj           # .NET 8 project file
```

---

## ⚙️ Setup Instructions

### 1. Prerequisites

| Tool | Version |
|------|---------|
| .NET SDK | 8.0+ |
| Visual Studio | 2022 (with WPF workload) |
| MySQL / MariaDB | 8.0+ |

### 2. Database Setup

```sql
-- Run in MySQL Workbench or CLI:
source /path/to/database_schema.sql
```

Or manually:
```bash
mysql -u root -p < database_schema.sql
```

### 3. Configure Connection String

Open `Services/DatabaseService.cs` and update:

```csharp
private readonly string _connectionString =
    "Server=localhost;Port=3306;Database=fundforest;" +
    "Uid=root;Pwd=YOUR_PASSWORD_HERE;CharSet=utf8mb4;";
```

### 4. Default Login

| Field | Value |
|-------|-------|
| Username | `admin` |
| Password | `admin123` |

> ⚠️ **Important:** Update the admin password in the database before deploying to production. The schema stores a placeholder hash — replace it with a proper BCrypt hash:
> ```csharp
> var hash = BCrypt.Net.BCrypt.HashPassword("your_new_password");
> ```

### 5. Restore NuGet Packages & Run

```bash
cd FundForest
dotnet restore
dotnet run
```

Or open `FundForest.csproj` in Visual Studio and press **F5**.

---

## 📦 NuGet Packages

| Package | Purpose |
|---------|---------|
| `MySql.Data` 8.3.0 | MySQL/MariaDB connector |
| `LiveChartsCore.SkiaSharpView.WPF` | Dashboard chart |
| `BCrypt.Net-Next` | Password hashing |
| `CommunityToolkit.Mvvm` | Optional MVVM helpers |

---

## 🏗️ Architecture Overview

### MVVM Pattern
- **Models** — Pure data classes with `INotifyPropertyChanged`
- **Views** — XAML only, zero business logic in code-behind
- **ViewModels** — All logic, commands, and state; bound to Views via `DataContext`

### Navigation
`MainViewModel` holds a `CurrentPage` property (`BaseViewModel`). Navigation commands swap the ViewModel — WPF's `DataTemplate` mapping automatically renders the correct `UserControl`.

```
MainWindow
└── ContentControl (Content = CurrentPage ViewModel)
    ├── DashboardViewModel    → DashboardView
    ├── BeneficiariesViewModel → BeneficiariesView
    ├── DonorsViewModel       → DonorsView
    ├── DonationsViewModel    → DonationsView
    ├── ProgramsViewModel     → ProgramsView
    └── DistributionViewModel → DistributionView
```

### Database Layer
All SQL is in `DatabaseService`. ViewModels call service methods — no raw SQL outside `Services/`.

---

## ✨ Features

| Module | Features |
|--------|---------|
| **Dashboard** | 6 stat cards, Donations vs Distributions chart (last 6 months) |
| **Beneficiaries** | Person/Group toggle, full CRUD, search, side-panel form |
| **Donors** | Individual/Group, full CRUD, search |
| **Donations** | Cash/Goods type, date filter, CSV export |
| **Programs** | Active/Archived status, CRUD, archive action |
| **Distribution** | Beneficiary + Program linking, status tracking, CRUD |
| **Auth** | Login with BCrypt validation, session management, logout |

---

## 🎨 UI Theme

- **Primary:** Forest green `#2D6A4F`
- **Background:** Soft mint `#F4FAF6`
- **Surface:** Pure white cards with drop shadows
- **Sidebar:** Dark forest `#1B4332`
- **Font:** Segoe UI (system font)
- **Corner radius:** 8–12px throughout

---

## 🔒 Security Notes

1. Never commit connection strings — use environment variables or encrypted config in production
2. Use BCrypt for all password storage (dependency already included)
3. Implement role-based visibility for sensitive actions using `SessionService.Instance.IsAdmin`

---

## 📋 Database Tables

| Table | Description |
|-------|-------------|
| `Admins` | System users with role-based access |
| `Beneficiaries` | Both Person and Group beneficiaries |
| `Donors` | Individual and group donors |
| `Donations` | Cash and goods donations |
| `Programs` | Donation programs/campaigns |
| `Distributions` | Records of distributed aid |

---

## 🛠️ Extending the System

### Add a new module:
1. Add model to `Models/Models.cs`
2. Add CRUD methods to `Services/DatabaseService.cs`
3. Create `ViewModels/YourModuleViewModel.cs` extending `BaseViewModel`
4. Create `Views/YourModuleView.xaml` + `.xaml.cs`
5. Add `DataTemplate` in `MainWindow.xaml`
6. Add navigation command in `MainViewModel.cs`
7. Add sidebar button in `MainWindow.xaml`

---

  # 👥 Authors / Contributors
| Name | GitHub Username |
| :--- | :--- |
| **John Joseph S. Dimatulac** | [@Youuusoff](https://github.com/JayDimatulac) |
| **Jiselle Mae M. Silla** | [@ellemrsgn27](https://github.com/ellemrsgn27) |
| **Lance Kert O. Mendoza** | [@sleepingPotato17](https://github.com/sleepingPotato17) |

*Built with ❤️ for community-driven donation management.*
=======
# FundForest
>>>>>>> b92ec1fd59c2ce8fe970e69dd6438ae448e0858e
