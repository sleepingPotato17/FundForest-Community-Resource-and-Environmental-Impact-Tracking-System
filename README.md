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
- Export donation records to CSV format for external reporting

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

## 🧩 UML Diagram

## 🛠️ Features and Functionalities of the System

## 🔄 Flow Chart

## ⚙️ How the Program Works

## 🧱 Implementation of OOP Principles

## 📋 Understanding the Code

## 📁 Documentation

## ▶️ How to Run the Program

## 👥 Developers / Contributors
| Name | GitHub Username |
| :--- | :--- |
| **John Joseph S. Dimatulac** | [@Youuusoff](https://github.com/JayDimatulac) |
| **Jiselle Mae M. Silla** | [@ellemrsgn27](https://github.com/ellemrsgn27) |
| **Lance Kert O. Mendoza** | [@sleepingPotato17](https://github.com/sleepingPotato17) |

## <p align="justify"><i>🌱 FundForest: Transparency that Roots, Impact that Grows</i></p>
