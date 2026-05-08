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
