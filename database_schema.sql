-- FundForest Database Schema (MySQL / MariaDB)
-- Run this script to initialize the database

CREATE DATABASE IF NOT EXISTS fundforest CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE fundforest;

-- =============================================
-- TABLE: Admins
-- =============================================
CREATE TABLE IF NOT EXISTS Admins (
    AdminID     INT AUTO_INCREMENT PRIMARY KEY,
    Username    VARCHAR(100) NOT NULL UNIQUE,
    Password    VARCHAR(255) NOT NULL,  -- store bcrypt hash
    FullName    VARCHAR(200),
    Role        ENUM('Admin','User') DEFAULT 'User',
    CreatedAt   DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Default admin (password: admin123 — change in production)
INSERT IGNORE INTO Admins (Username, Password, FullName, Role)
VALUES ('admin', '$2b$12$defaultHashChangeMe', 'System Administrator', 'Admin');

-- =============================================
-- TABLE: Programs
-- =============================================
CREATE TABLE IF NOT EXISTS Programs (
    ProgramID       INT AUTO_INCREMENT PRIMARY KEY,
    ProgramName     VARCHAR(200) NOT NULL,
    StartDate       DATE,
    EndDate         DATE,
    TargetAudience  ENUM('Group','Barangay') DEFAULT 'Group',
    Barangay        VARCHAR(200),
    Description     TEXT,
    Status          ENUM('Active','Archived') DEFAULT 'Active',
    CreatedAt       DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- =============================================
-- TABLE: Beneficiaries
-- =============================================
CREATE TABLE IF NOT EXISTS Beneficiaries (
    BeneficiaryID       INT AUTO_INCREMENT PRIMARY KEY,
    BeneficiaryType     ENUM('Person','Group') NOT NULL DEFAULT 'Person',
    -- Person fields
    FullName            VARCHAR(200),
    Gender              ENUM('Male','Female','Other'),
    Age                 INT,
    -- Group fields
    GroupName           VARCHAR(200),
    NumberOfMembers     INT,
    GroupRepresentative VARCHAR(200),
    -- Shared
    VulnerabilityType   VARCHAR(100),
    Address             TEXT,
    ContactInfo         VARCHAR(200),
    ContactDate         DATE,
    CreatedAt           DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- =============================================
-- TABLE: Donors
-- =============================================
CREATE TABLE IF NOT EXISTS Donors (
    DonorID         INT AUTO_INCREMENT PRIMARY KEY,
    DonorName       VARCHAR(200) NOT NULL,
    DonationType    ENUM('Individual','Group') DEFAULT 'Individual',
    ContactInfo     VARCHAR(200),
    Address         TEXT,
    CreatedAt       DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- =============================================
-- TABLE: Donations
-- =============================================
CREATE TABLE IF NOT EXISTS Donations (
    DonationID          INT AUTO_INCREMENT PRIMARY KEY,
    DonorID             INT NOT NULL,
    Amount              DECIMAL(15,2) DEFAULT 0,
    DonationType        ENUM('Cash','Goods') NOT NULL DEFAULT 'Cash',
    GoodsDescription    TEXT,
    DonationDate        DATE NOT NULL,
    Notes               TEXT,
    CreatedAt           DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (DonorID) REFERENCES Donors(DonorID) ON DELETE RESTRICT
);

-- =============================================
-- TABLE: Distributions
-- =============================================
CREATE TABLE IF NOT EXISTS Distributions (
    DistributionID      INT AUTO_INCREMENT PRIMARY KEY,
    BeneficiaryID       INT NOT NULL,
    ProgramID           INT,
    DistributionDate    DATE NOT NULL,
    DistributedItem     VARCHAR(200),
    Quantity            INT DEFAULT 1,
    Amount              DECIMAL(15,2) DEFAULT 0,
    Status              ENUM('Completed','Pending') DEFAULT 'Pending',
    Notes               TEXT,
    CreatedAt           DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (BeneficiaryID) REFERENCES Beneficiaries(BeneficiaryID) ON DELETE RESTRICT,
    FOREIGN KEY (ProgramID)     REFERENCES Programs(ProgramID) ON DELETE SET NULL
);

-- =============================================
-- Sample seed data
-- =============================================
INSERT IGNORE INTO Programs (ProgramName, StartDate, EndDate, TargetAudience, Description, Status)
VALUES
('Livelihood Support', '2024-01-01', '2024-12-31', 'Group', 'Financial assistance for livelihood programs', 'Active'),
('Food Pack Distribution', '2024-03-01', '2024-09-30', 'Barangay', 'Monthly food pack distribution', 'Active');
