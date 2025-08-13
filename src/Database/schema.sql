-- Create database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BRELTV')
BEGIN
    CREATE DATABASE BRELTV;
END
GO

USE BRELTV;
GO

-- Create CustomerProfiles table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CustomerProfiles')
BEGIN
    CREATE TABLE CustomerProfiles (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProfileBand NVARCHAR(50) NOT NULL,
        Description NVARCHAR(255) NOT NULL,
        MinScore INT NULL,
        MaxScore INT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL,
        CreatedBy NVARCHAR(100) NOT NULL,
        UpdatedAt DATETIME NULL,
        UpdatedBy NVARCHAR(100) NULL
    );

    -- Add unique constraint on ProfileBand
    ALTER TABLE CustomerProfiles
    ADD CONSTRAINT UQ_CustomerProfiles_ProfileBand UNIQUE (ProfileBand);
END
GO

-- Create BusinessRules table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BusinessRules')
BEGIN
    CREATE TABLE BusinessRules (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CustomerProfileId INT NOT NULL,
        NoIncomeProofLTV DECIMAL(5,2) NOT NULL,
        MaxLTVWithProof DECIMAL(5,2) NOT NULL,
        FIRequirement NVARCHAR(50) NOT NULL,
        MinIncomeProofAmount DECIMAL(18,2) NOT NULL,
        MinFloatingMoneyPercentage DECIMAL(5,2) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 0,
        IsApproved BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME NOT NULL,
        CreatedBy NVARCHAR(100) NOT NULL,
        UpdatedAt DATETIME NULL,
        UpdatedBy NVARCHAR(100) NULL,
        ApprovedAt DATETIME NULL,
        ApprovedBy NVARCHAR(100) NULL,
        CONSTRAINT FK_BusinessRules_CustomerProfiles FOREIGN KEY (CustomerProfileId) REFERENCES CustomerProfiles(Id)
    );
END
GO

-- Create LoanEvaluations table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LoanEvaluations')
BEGIN
    CREATE TABLE LoanEvaluations (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CustomerProfileBand NVARCHAR(50) NOT NULL,
        IncomeProofAvailable BIT NOT NULL,
        IncomeProofAmount DECIMAL(18,2) NOT NULL,
        FloatingMoneyAfterExpensesAndEMI DECIMAL(5,2) NOT NULL,
        AssignedLTV DECIMAL(5,2) NOT NULL,
        FIRequirement NVARCHAR(50) NOT NULL,
        Reason NVARCHAR(MAX) NOT NULL,
        EvaluatedAt DATETIME NOT NULL,
        EvaluatedBy NVARCHAR(100) NOT NULL
    );
END
GO

-- Create RuleApprovals table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RuleApprovals')
BEGIN
    CREATE TABLE RuleApprovals (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        BusinessRuleId INT NOT NULL,
        ApprovalStatus NVARCHAR(50) NOT NULL, -- 'Pending', 'Approved', 'Rejected'
        RequestedBy NVARCHAR(100) NOT NULL,
        RequestedAt DATETIME NOT NULL,
        ApprovedBy NVARCHAR(100) NULL,
        ApprovedAt DATETIME NULL,
        Comments NVARCHAR(MAX) NULL,
        CONSTRAINT FK_RuleApprovals_BusinessRules FOREIGN KEY (BusinessRuleId) REFERENCES BusinessRules(Id)
    );
END
GO

-- Insert default customer profiles
IF NOT EXISTS (SELECT TOP 1 1 FROM CustomerProfiles)
BEGIN
    INSERT INTO CustomerProfiles (ProfileBand, Description, MinScore, MaxScore, IsActive, CreatedAt, CreatedBy)
    VALUES 
        ('0', 'No Credit History', NULL, NULL, 1, GETUTCDATE(), 'System'),
        ('-1', 'Negative Credit History', NULL, NULL, 1, GETUTCDATE(), 'System'),
        ('650-700', 'Medium Risk', 650, 700, 1, GETUTCDATE(), 'System'),
        ('700-750', 'Low Risk', 700, 750, 1, GETUTCDATE(), 'System'),
        ('750+', 'Very Low Risk', 750, NULL, 1, GETUTCDATE(), 'System');
END
GO

-- Insert default business rules
IF NOT EXISTS (SELECT TOP 1 1 FROM BusinessRules)
BEGIN
    DECLARE @ZeroProfileId INT, @NegativeProfileId INT, @MediumRiskProfileId INT, @LowRiskProfileId INT, @VeryLowRiskProfileId INT;

    SELECT @ZeroProfileId = Id FROM CustomerProfiles WHERE ProfileBand = '0';
    SELECT @NegativeProfileId = Id FROM CustomerProfiles WHERE ProfileBand = '-1';
    SELECT @MediumRiskProfileId = Id FROM CustomerProfiles WHERE ProfileBand = '650-700';
    SELECT @LowRiskProfileId = Id FROM CustomerProfiles WHERE ProfileBand = '700-750';
    SELECT @VeryLowRiskProfileId = Id FROM CustomerProfiles WHERE ProfileBand = '750+';

    INSERT INTO BusinessRules (
        CustomerProfileId, NoIncomeProofLTV, MaxLTVWithProof, FIRequirement, 
        MinIncomeProofAmount, MinFloatingMoneyPercentage, IsActive, IsApproved, 
        CreatedAt, CreatedBy, ApprovedAt, ApprovedBy)
    VALUES 
        (@ZeroProfileId, 75, 85, 'FI Mandatory', 25000, 50, 1, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
        (@NegativeProfileId, 75, 85, 'FI Mandatory', 25000, 50, 1, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
        (@MediumRiskProfileId, 75, 85, 'FI Mandatory', 25000, 50, 1, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
        (@LowRiskProfileId, 80, 95, 'FI Waiver', 25000, 50, 1, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System'),
        (@VeryLowRiskProfileId, 85, 100, 'FI Waiver', 25000, 50, 1, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END
GO

