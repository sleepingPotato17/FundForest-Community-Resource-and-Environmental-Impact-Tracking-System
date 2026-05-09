<div align="center">

# FundForest

### Community Resource and Environmental Impact Tracking System

> A structured desktop application for managing donations, beneficiaries, programs, and distributions within community organizations — built for transparency, accountability, and operational efficiency.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-WPF-239120?style=flat-square&logo=csharp)
![MariaDB](https://img.shields.io/badge/MariaDB-Database-003545?style=flat-square&logo=mariadb)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)
![Status](https://img.shields.io/badge/Status-Active-brightgreen?style=flat-square)

</div>

<br>

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Features](#features)
3. [Tech Stack](#tech-stack)
4. [Object-Oriented Programming Principles](#object-oriented-programming-principles)
5. [System Architecture](#system-architecture)
6. [Project Structure](#project-structure)
7. [Installation Guide](#installation-guide)
8. [Database Setup](#database-setup)
9. [Authentication and Security](#authentication-and-security)
10. [User Interface Preview](#user-interface-preview)
11. [Future Improvements](#future-improvements)
12. [Developers](#developers)
13. [Lessons Learned](#lessons-learned)
14. [License](#license)
15. [Acknowledgements](#acknowledgements)

---

<br>

## Project Overview

**FundForest** is a Windows desktop application developed using C# and WPF (.NET 8) with a MariaDB relational database backend. It was created to address a persistent problem in community-based organizations — the lack of a structured, transparent, and auditable system for managing donated funds and distributed resources.

Many barangay-level organizations, community cooperatives, and local government units still rely on manual record-keeping through spreadsheets or paper forms. These methods are prone to error, difficult to audit, and offer no real-time visibility into how resources are being allocated. FundForest replaces these outdated processes with a centralized, role-aware, and database-backed management system.

The system covers the full lifecycle of community resource management: from recording a donation and registering a donor, to creating a program, enrolling beneficiaries, and logging each distribution — all tracked, searchable, and secured by role-based access control.

**Target Users:**
- Barangay administrators managing community aid programs
- Staff responsible for recording donations and distributions
- Local users who need read-only visibility into program data
- System administrators overseeing user accounts and approvals

**Core Value Proposition:**
FundForest does not simply store records — it enforces accountability. Every action in the system is tied to a role, every distribution is linked to a beneficiary and program, and every donation is attributed to a donor. This creates a complete, traceable audit trail that can be reviewed at any time.

<br>

---

## Features

### User Authentication and Authorization
- Secure login system with BCrypt-hashed password verification
- Three-tier role system: Admin, Staff, and Local
- New account registration with mandatory Admin approval before access is granted
- Role-based UI rendering — navigation items, action buttons, and form panels are shown or hidden based on the authenticated user's role
- Accounts pending approval are blocked from login with a descriptive error message

### Dashboard
- Real-time summary statistics: total donations, programs, beneficiaries, and distributions
- Quick-glance counts with module navigation shortcuts
- Visual branding consistent with the application's green-themed identity

### Beneficiary Management
- Register and manage both individual persons and community groups as beneficiaries
- Capture demographic data including full name, age, gender, vulnerability classification, contact information, and address
- Vulnerability categories: None, Elderly, Senior Citizen, PWD, Indigent, Solo Parent, 4Ps
- Gender options: Male, Female, Other, None (for groups)
- Search beneficiaries by name, contact number, or address
- Edit and delete existing records through a slide-in side panel form
- Row numbers displayed sequentially based on current list position, not database ID

### Donor Management
- Maintain a complete registry of donors categorized as Individual or Group
- Store contact information and address per donor entry
- Full Create, Read, Update, and Delete (CRUD) operations
- Live search filtering

### Donation Recording
- Record incoming donations of type Cash or Goods
- Link each donation to a registered donor
- Capture donation date, amount, goods description (for in-kind donations), and notes
- Filter donations by donor name and date range
- Export donation records to PDF format for external reporting

### Program Management
- Create community programs with defined start and end dates
- Assign target audience (Group or Barangay) and a specific location/barangay
- Set and update program status (Active or Archived)
- Archive programs directly from the main list view
- Full-text search across program names

### Distribution Tracking
- Record distributions linking a beneficiary to a program
- Capture distributed item name, quantity, monetary amount, distribution date, and notes
- Set distribution status: Completed or Pending
- Full CRUD operations with a structured side panel form
- Search distributions by beneficiary name or program

### User Management (Admin Only)
- View all registered system users with their role, registration date, and approval status
- Approve or reject pending user accounts
- Delete user accounts from the system
- Real-time display of the number of accounts awaiting approval

<br>

---

## Tech Stack

| Category | Technology | Purpose |
|----------|------------|---------|
| Language | C# 12 | Primary application logic and backend |
| UI Framework | WPF (Windows Presentation Foundation) | Desktop user interface |
| Markup | XAML | Declarative UI layout and styling |
| Architecture Pattern | MVVM (Model-View-ViewModel) | Separation of concerns |
| Runtime | .NET 8.0 | Application execution environment |
| Database | MariaDB / MySQL | Relational data storage |
| Database Access | MySql.Data (ADO.NET) | Parameterized SQL query execution |
| Password Security | BCrypt.Net-Next | Secure password hashing and verification |
| Charts | LiveChartsCore.SkiaSharpView.WPF | Data visualization on the dashboard |
| Reporting | FastReport.OpenSource | Report generation and PDF export |
| MVVM Utilities | CommunityToolkit.Mvvm | Observable properties and commands |
| Version Control | Git / GitHub | Source control and collaboration |
| Development IDE | Visual Studio / VS Code | Code editing and debugging |

<br>

---

## Object-Oriented Programming Principles

FundForest was designed and developed with strict adherence to the four fundamental principles of Object-Oriented Programming. These principles are not merely theoretical — they are concretely applied throughout the codebase to ensure maintainability, scalability, and correctness.

<br>

### 1. Encapsulation

Encapsulation is the practice of bundling data and the methods that operate on that data within a single unit, and restricting direct access to internal implementation details.

**Application in FundForest:**

All model classes in the system encapsulate their properties using C# auto-properties with controlled access modifiers. For example, the `Beneficiary` model exposes its fields as public properties while keeping validation logic internal.

```csharp
public class Beneficiary
{
    public int BeneficiaryID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string VulnerabilityType { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // Computed display property — internal logic hidden from the UI
    public string DisplayName =>
        string.IsNullOrWhiteSpace(FullName) ? GroupName : FullName;
}
```

The `DatabaseService` class encapsulates all database interaction. No ViewModel or View directly constructs SQL queries — all data access is routed through `DatabaseService`, which exposes clean, named methods such as `GetAllBeneficiaries()`, `CreateBeneficiary()`, and `DeleteBeneficiary()`. The connection string, connection lifecycle, and error handling are all hidden behind this service boundary.

The `BaseViewModel` class encapsulates the `INotifyPropertyChanged` implementation. All ViewModels inherit from `BaseViewModel` and call `SetProperty()` without needing to know how property change notification works internally.

```csharp
public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }
}
```

<br>

### 2. Inheritance

Inheritance allows a class to acquire the properties and behaviors of a parent class, promoting code reuse and establishing a consistent base structure across related classes.

**Application in FundForest:**

Every ViewModel in the application inherits from `BaseViewModel`. This means that `BeneficiariesViewModel`, `DonorsViewModel`, `DonationsViewModel`, `ProgramsViewModel`, `DistributionViewModel`, `ManageUsersViewModel`, `LoginViewModel`, and `RegisterViewModel` all share the same `SetProperty()`, `OnPropertyChanged()`, and `PropertyChanged` infrastructure without duplicating a single line.

```csharp
// Base class — defines shared behavior
public abstract class BaseViewModel : INotifyPropertyChanged { ... }

// Derived classes — inherit all base behavior
public class BeneficiariesViewModel : BaseViewModel { ... }
public class DonorsViewModel : BaseViewModel { ... }
public class DonationsViewModel : BaseViewModel { ... }
public class ProgramsViewModel : BaseViewModel { ... }
public class DistributionViewModel : BaseViewModel { ... }
public class ManageUsersViewModel : BaseViewModel { ... }
public class LoginViewModel : BaseViewModel { ... }
public class RegisterViewModel : BaseViewModel { ... }
```

The `RelayCommand` class also demonstrates inheritance by implementing the `ICommand` interface, allowing it to be used uniformly wherever WPF expects a command binding.

In the UI layer, WPF's component model itself relies on inheritance — all custom `UserControl` views (such as `BeneficiariesView`, `DonorsView`, and `ProgramsView`) inherit from `UserControl`, giving them the full WPF rendering and binding infrastructure.

<br>

### 3. Polymorphism

Polymorphism allows objects of different types to be treated through a common interface, enabling a single method or reference to behave differently depending on the actual runtime type.

**Application in FundForest:**

FundForest uses polymorphism extensively through the `IValueConverter` interface. Multiple converter classes implement the same interface — `Convert()` and `ConvertBack()` — but each behaves differently at runtime depending on its purpose.

```csharp
// All of these implement IValueConverter
// but each Convert() method behaves differently

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, ...) =>
        value is bool b && b ? Visibility.Visible : Visibility.Collapsed;
}

public class StringNotEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, ...) =>
        string.IsNullOrWhiteSpace(value?.ToString())
            ? Visibility.Collapsed : Visibility.Visible;
}

public class RowNumberConverter : IValueConverter
{
    public object Convert(object value, ...) =>
        value is DataGridRow row ? row.GetIndex() + 1 : string.Empty;
}

public class PesoConverter : IValueConverter
{
    public object Convert(object value, ...) =>
        value is decimal d ? $"₱{d:N2}" : "₱0.00";
}

public class EnumToBoolConverter : IValueConverter
{
    public object Convert(object value, ...) =>
        value?.ToString() == parameter?.ToString();
}
```

When WPF calls `Convert()` on a binding, it does not need to know which specific converter it is invoking — it simply calls the method through the `IValueConverter` interface and receives the appropriate result based on the actual type. This is runtime polymorphism in direct, practical use.

The `ICommand` interface also demonstrates polymorphism. The `RelayCommand` class implements `ICommand`, and WPF button bindings interact with it solely through `CanExecute()` and `Execute()` — regardless of what the actual command does at runtime.

<br>

### 4. Abstraction

Abstraction means exposing only the essential details of a system while hiding its internal complexity. It allows developers to work with high-level representations without needing to understand low-level implementation details.

**Application in FundForest:**

The most prominent use of abstraction in FundForest is the `DatabaseService` class. It abstracts the entire data access layer from the rest of the application. ViewModels never write SQL — they call meaningful, intention-revealing methods:

```csharp
// What the ViewModel sees (abstraction)
var beneficiaries = _db.GetAllBeneficiaries();
bool success = _db.CreateBeneficiary(newBeneficiary);
_db.DeleteBeneficiary(id);

// What DatabaseService hides internally
private MySqlConnection GetConnection() =>
    new MySqlConnection(_connectionString);

public List<Beneficiary> GetAllBeneficiaries()
{
    var list = new List<Beneficiary>();
    using var conn = GetConnection();
    conn.Open();
    using var cmd = new MySqlCommand("SELECT * FROM Beneficiaries", conn);
    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        list.Add(new Beneficiary
        {
            BeneficiaryID = reader.GetInt32("BeneficiaryID"),
            FullName = reader.GetString("FullName"),
            // ...
        });
    }
    return list;
}
```

The `BaseViewModel` class is itself an abstract class — it cannot be instantiated directly. It defines the contract that all ViewModels must follow, while leaving the specific implementation of properties and commands to each derived class.

`ICommand` and `IValueConverter` are both interfaces that define abstract contracts. The application depends on these abstractions rather than concrete implementations, making it possible to swap or extend behavior without modifying the consuming code.

<br>

---

## System Architecture

FundForest is built on the **MVVM (Model-View-ViewModel)** architectural pattern, which enforces a clean separation between three distinct layers of the application.

``` bash
+-----------------------------------------------+
|                 VIEW LAYER                    |
|   XAML UserControls, Windows, Data Bindings   |
|   BeneficiariesView, DonorsView, etc.         |
+--------------------+--------------------------+
|
Data Binding
Commands
|
+--------------------v--------------------------+
|              VIEWMODEL LAYER                  |
|   Business Logic, State Management           |
|   ObservableProperties, RelayCommands        |
|   BeneficiariesViewModel, DonorsViewModel    |
+--------------------+--------------------------+
|
Method Calls
|
+--------------------v--------------------------+
|              SERVICE LAYER                    |
|   DatabaseService                            |
|   All SQL queries, connection management     |
+--------------------+--------------------------+
|
ADO.NET / MySql.Data
|
+--------------------v--------------------------+
|              DATABASE LAYER                   |
|   MariaDB                                    |
|   Users, Donors, Beneficiaries,              |
|   Donations, Programs, Distributions         |
+-----------------------------------------------+


```
### Layer Responsibilities

**View Layer** — XAML files that define the visual structure and layout. Views bind to ViewModel properties and commands. They contain no business logic. All UI decisions (what to show, what to hide) are driven by ViewModel state through data binding and value converters.

**ViewModel Layer** — C# classes that manage application state and respond to user actions through commands. ViewModels call the service layer to retrieve and persist data, then expose it to the View through observable properties. They are fully testable without a UI.

**Service Layer** — The `DatabaseService` class is the single point of contact with the database. It opens connections, executes parameterized queries, maps result sets to model objects, and closes connections. No SQL exists outside this class.

**Database Layer** — A MariaDB relational database with normalized tables. Each entity (donors, beneficiaries, donations, programs, distributions, users) has its own table with appropriate foreign key relationships.

### Authentication Flow
```bash 
User enters credentials
|
v
LoginViewModel.LoginCommand executes
|
v
DatabaseService.ValidateLogin(username, password)
|
v
Query Users table by username
|
v
BCrypt.Verify(inputPassword, storedHash)
|
+----+----+
|         |
False      True
|         |
Block      Check IsApproved
login         |
+----+----+
|         |
False      True
|         |
Show          Load user role
pending       Open MainWindow
message       with role-filtered
navigation
```

<br>

---

## Project Structure
```bash
FundForest/
├── Frontend/
│   ├── Views/
│   │   ├── LoginWindow.xaml
│   │   ├── LoginWindow.xaml.cs
│   │   ├── RegisterWindow.xaml
│   │   ├── RegisterWindow.xaml.cs
│   │   ├── MainWindow.xaml
│   │   ├── MainWindow.xaml.cs
│   │   ├── DashboardView.xaml
│   │   ├── DashboardView.xaml.cs
│   │   ├── BeneficiariesView.xaml
│   │   ├── BeneficiariesView.xaml.cs
│   │   ├── DonorsView.xaml
│   │   ├── DonorsView.xaml.cs
│   │   ├── DonationsView.xaml
│   │   ├── DonationsView.xaml.cs
│   │   ├── ProgramsView.xaml
│   │   ├── ProgramsView.xaml.cs
│   │   ├── DistributionView.xaml
│   │   ├── DistributionView.xaml.cs
│   │   ├── ManageUsersView.xaml
│   │   └── ManageUsersView.xaml.cs
│   └── ViewModels/
│       ├── BeneficiariesViewModel.cs
│       ├── DashboardViewModel.cs
│       ├── LoginViewModel.cs
│       ├── MainViewModel.cs
│       └── ManageUsersViewModel.cs
│       └── ModuleViewModel.cs
│       └──  RegisterViewModel.cs        
├── Models/
│   ├── Models.cs
├── Services/
│   └── DatabaseService.cs
├── Helpers/
│   ├── Helpers.cs
├── Resources/
│   └── styles.xaml
│   └── colors.xaml
├── .gitignore
├── app.manifest
├── App.xaml
├── App.xaml.cs
├── debug.txt
├── FundForest.csproj
├── FundForest.sln
└── README.md

```
<br>

---

## Installation Guide

### Prerequisites

Before running FundForest, ensure the following are installed on your machine:

| Requirement | Version | Download |
|-------------|---------|----------|
| .NET SDK | 8.0 or later | https://dotnet.microsoft.com/download |
| MariaDB | 10.6 or later | https://mariadb.org/download |
| Git | Latest | https://git-scm.com |
| Windows OS | 10 or later | Required for WPF |

<br>

### Step 1 — Clone the Repository

```bash
git clone https://github.com/sleepingPotato17/FundForest-Community-Resource-and-Environmental-Impact-Tracking-System.git
cd FundForest-Community-Resource-and-Environmental-Impact-Tracking-System
```

### Step 2 — Restore NuGet Packages

```bash
dotnet restore
```

This will automatically install all required dependencies declared in `FundForest.csproj`:

- MySql.Data
- BCrypt.Net-Next
- CommunityToolkit.Mvvm
- LiveChartsCore.SkiaSharpView.WPF
- FastReport.OpenSource

### Step 3 — Set Up the Database

Start your MariaDB server and create the database:

```sql
CREATE DATABASE FUNDFOREST;
```

Import the provided SQL schema and seed file:

```bash
mysql -u root -p FUNDFOREST < fundforest.sql
```

### Step 4 — Configure the Connection String

Open `Services/DatabaseService.cs` and update the connection string to match your local MariaDB credentials:

```csharp
private readonly string _connectionString =
    "Server=localhost;Port=3306;Database=FUNDFOREST;Uid=root;Pwd=yourpassword;";
```

### Step 5 — Build and Run

```bash
dotnet build
dotnet run
```

The application window will launch. Log in using the default Admin credentials seeded in the SQL file, or register a new account and approve it through the Admin panel.

<br>

---

## Database Setup

### Schema Overview

FundForest uses a normalized relational schema with six primary tables. All tables use integer primary keys with auto-increment, and foreign key constraints ensure referential integrity between related records.

| Table | Primary Key | Description |
|-------|-------------|-------------|
| `Users` | `UserID` | System accounts with role and approval status |
| `Donors` | `DonorID` | Registered donation contributors |
| `Beneficiaries` | `BeneficiaryID` | Community members receiving aid |
| `Donations` | `DonationID` | Incoming donation records linked to donors |
| `Programs` | `ProgramID` | Community aid programs and campaigns |
| `Distributions` | `DistributionID` | Aid delivery records linked to beneficiaries and programs |

### Relationships
```bash
Donors (1) --------< Donations (many)
Programs (1) -------< Distributions (many)
Beneficiaries (1) --< Distributions (many)
```
### ENUM Definitions

```sql
-- Users.Role
ENUM('Admin', 'Staff', 'Local')

-- Beneficiaries.Gender
ENUM('Male', 'Female', 'Other', 'None')

-- Beneficiaries.VulnerabilityType
ENUM('None', 'Elderly', 'Senior Citizen', 'PWD', 'Indigent', 'Solo Parent', '4Ps')

-- Beneficiaries.BeneficiaryType
ENUM('Person', 'Group')

-- Donors.DonationType
ENUM('Individual', 'Group')

-- Donations.DonationType
ENUM('Cash', 'Goods')

-- Programs.Status
ENUM('Active', 'Archived')

-- Programs.TargetAudience
ENUM('Group', 'Barangay')

-- Distributions.Status
ENUM('Completed', 'Pending')
```

<br>

---

## Authentication and Security

### Password Hashing

All user passwords are hashed using **BCrypt** before being stored in the database. The plain-text password is never written to disk at any point. During login, the entered password is verified against the stored hash using `BCrypt.Net.BCrypt.Verify()`.

```csharp
// Registration
string hashed = BCrypt.Net.BCrypt.HashPassword(plainTextPassword);

// Login verification
bool isValid = BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
```

BCrypt includes a built-in salt and work factor, making the stored hashes resistant to rainbow table attacks and brute force attempts.

### Role-Based Access Control

The system enforces three distinct access levels:

| Role | Permissions |
|------|-------------|
| Admin | Full access to all modules including User Management. Can approve, reject, and delete user accounts. |
| Staff | Can perform CRUD operations on Donors, Beneficiaries, Donations, Programs, and Distributions. Cannot access User Management. |
| Local | Read-only access. Can view records but cannot add, edit, or delete any data. |

Role enforcement occurs at two levels:
1. **Navigation** — Menu items and tabs are shown or hidden based on the authenticated user's role
2. **UI Controls** — Add, Edit, and Delete buttons are conditionally rendered using `Visibility` bindings tied to role-aware boolean properties in each ViewModel

### Account Approval System

New accounts registered through the Create Account screen are set to `IsApproved = false` by default. They are blocked from logging in until an Admin explicitly approves them through the Manage Users panel. This prevents unauthorized users from gaining access to the system simply by registering.

### SQL Injection Prevention

All database queries in `DatabaseService` use ADO.NET's `MySqlCommand` with named parameters. No string concatenation is used to build SQL queries.

```csharp
var cmd = new MySqlCommand(
    "INSERT INTO Donors (DonorName, ContactInfo) VALUES (@name, @contact)", conn);
cmd.Parameters.AddWithValue("@name", donor.DonorName);
cmd.Parameters.AddWithValue("@contact", donor.ContactInfo);
```

<br>

---

## User Interface Preview

> The following table lists the primary screens of FundForest. Replace the image paths with actual screenshots from the running application before final submission.

| Screen | Description | Preview |
|--------|-------------|---------|
| Login | Credential entry with role-aware error messaging | ![Login](screenshots/login.png) |
| Register | Account creation with role information panel | ![Register](screenshots/register.png) |
| Dashboard | Summary statistics and quick navigation | ![Dashboard](screenshots/dashboard.png) |
| Beneficiaries | Beneficiary list with search and side-panel form | ![Beneficiaries](screenshots/beneficiaries.png) |
| Donors | Donor registry with CRUD operations | ![Donors](screenshots/donors.png) |
| Donations | Donation records with date filtering and PDF export | ![Donations](screenshots/donations.png) |
| Programs | Program list with archive functionality | ![Programs](screenshots/programs.png) |
| Distribution | Distribution tracking linked to programs and beneficiaries | ![Distribution](screenshots/distribution.png) |
| Manage Users | Admin-only user approval and management panel | ![Users](screenshots/users.png) |

<br>

---

## Future Improvements

The following enhancements are planned or recommended for future development cycles:

| Enhancement | Description |
|-------------|-------------|
| Cloud Database | Host MariaDB on a cloud provider (AWS RDS, Azure Database, PlanetScale) to allow multi-location access |
| Analytics Dashboard | Extend the dashboard with trend charts showing monthly donation volume, program activity, and distribution rates |
| Email Notifications | Send automated email alerts for pending account approvals, new donations, and upcoming program deadlines |
| Audit Log | Record every Create, Update, and Delete action with the user who performed it and the timestamp |
| Mobile Companion App | A read-only mobile interface for administrators to review key metrics remotely |
| Multi-language Support | Toggle between Filipino and English for broader accessibility |
| Backup and Restore | Built-in database backup scheduling and one-click restore functionality |
| AI-Assisted Insights | Pattern recognition to suggest optimal distribution timing and flag unusual donation activity |
| Unit Testing | Formal test coverage for ViewModel logic and DatabaseService methods using xUnit or NUnit |

<br>

---

## Developers

<div align="justify">

| Name | GitHub |
|------|--------|
| John Joseph S. Dimatulac | [@Youuusoff](https://github.com/Youuusoff) |
| Jisselle Mae M. Silla | [@ellemrsgn27](https://github.com/ellemrsgn27) |
| Lance Kert O. Mendoza | [@sleepingPotato17](https://github.com/sleepingPotato17) |

</div>

<br>

---

## Lessons Learned

### Technical Challenges

**WPF Data Binding Complexity**
WPF's binding system is powerful but unforgiving. Early in development, binding errors were silent — properties would simply not update in the UI without any runtime exception. We learned to use `INotifyPropertyChanged` correctly through `BaseViewModel`, and to always set `UpdateSourceTrigger=PropertyChanged` on two-way bound TextBox controls.

**Custom ComboBox Templating**
Replacing the default WPF ComboBox style proved unexpectedly difficult. The built-in template uses a `ToggleButton` internally, and our custom visual `Border` was intercepting all mouse events, making the dropdown unresponsive. The solution was to set `IsHitTestVisible="False"` on the visual layer and place a transparent `ToggleButton` with its own minimal `ControlTemplate` on top of it as a full-area click target.

**Sequential Row Numbering**
The initial approach of binding the `#` column directly to the database primary key produced gaps whenever records were deleted (e.g., IDs 1, 2, 5, 8 instead of 1, 2, 3, 4). We resolved this by implementing a custom `RowNumberConverter` that retrieves the row's current index within the `DataGrid` using `DataGridRow.GetIndex() + 1`, producing gapless sequential numbers that update automatically as records are added or removed.

**XAML Tag Nesting Errors**
Several build errors were caused by mismatched or missing closing tags in deeply nested XAML. The WPF XAML compiler error messages often pointed to a symptom line rather than the actual cause. We learned to work methodically from the outermost containers inward when debugging structural XAML issues.

**Role-Based UI Without Code-Behind Logic**
Enforcing role-based visibility purely through XAML bindings — without writing conditional logic in code-behind — required careful use of `BoolToVisibilityConverter` and ViewModel boolean properties like `CanAdd`, `CanEdit`, and `CanDelete`. This kept the Views clean and the logic testable.

### Architectural Insights

Adopting MVVM from the beginning of the project was a deliberate decision that paid significant dividends during development. Because Views contain no business logic, it was straightforward to redesign or replace UI components without touching the underlying data or command logic. The strict separation also made it easier for team members to work on different layers simultaneously without causing merge conflicts.

Centralizing all database operations in `DatabaseService` meant that schema changes only required updates in one file. When the `Beneficiaries` table's `Gender` ENUM was extended to include `'None'`, the change was isolated to the database and the service class — no Views or ViewModels required modification.

### Team and Process

Version control through GitHub was essential for parallel development. Each team member worked on separate feature branches, and pull requests were used to review changes before merging into the main branch. This workflow prevented several potential conflicts and gave us a clean history of what changed and why.

Code review sessions, even informal ones, consistently caught issues before they became embedded in the codebase — particularly around XAML structure and ViewModel property naming conventions.

<br>

---

## License

``` bash
MIT License
Copyright (c) 2026 FundForest Development Team

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

```
<br>

---

## Acknowledgements

We extend our sincere gratitude to the following:

- **Our Project Adviser** — for the consistent technical guidance, constructive feedback, and patience throughout every phase of this project
- **The Open-Source Community** — for the libraries that made FundForest possible: BCrypt.Net-Next, LiveChartsCore, FastReport.OpenSource, CommunityToolkit.Mvvm, and MySql.Data
- **Our Families** — for the understanding and encouragement during the long hours of development and documentation

<br>

---

<div align="center">

FundForest was built with the intention of making a real difference in how community organizations manage the resources entrusted to them.

If this project was useful to you or served as a reference for your own work, please consider leaving a star on the repository.

**FundForest — 2026**

</div>

## <p align="center"><i>🌱 FundForest: Transparency that Roots, Impact that Grows</i></p>
