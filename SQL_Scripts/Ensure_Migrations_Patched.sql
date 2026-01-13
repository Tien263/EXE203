-- Ensure_Migrations_Patched.sql
-- Adds missing column(s) and performance indexes required by the app migrations.

USE MocViStoreDB;
GO

-- 1) Add ProfileImageUrl to Users if missing
IF COL_LENGTH('dbo.Users', 'ProfileImageUrl') IS NULL
BEGIN
    PRINT N'Adding column ProfileImageUrl to dbo.Users';
    ALTER TABLE dbo.Users
    ADD ProfileImageUrl NVARCHAR(MAX) NULL;
END
ELSE
BEGIN
    PRINT N'Column ProfileImageUrl already exists on dbo.Users';
END
GO

-- 2) Create performance indexes (if not present)
-- Users: IX_Users_IsActive
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_IsActive' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    PRINT N'Creating index IX_Users_IsActive on dbo.Users(IsActive)';
    CREATE INDEX IX_Users_IsActive ON dbo.Users(IsActive);
END
ELSE
BEGIN
    PRINT N'Index IX_Users_IsActive already exists';
END
GO

-- Users: IX_Users_Role
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Role' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    PRINT N'Creating index IX_Users_Role on dbo.Users(Role)';
    CREATE INDEX IX_Users_Role ON dbo.Users(Role);
END
ELSE
BEGIN
    PRINT N'Index IX_Users_Role already exists';
END
GO

-- Add additional indexes from AddPerformanceIndexes migration as needed
-- Products: IX_Products_CategoryId_IsActive
IF OBJECT_ID('dbo.Products') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_CategoryId_IsActive' AND object_id = OBJECT_ID('dbo.Products'))
    BEGIN
        PRINT N'Creating index IX_Products_CategoryId_IsActive on dbo.Products(CategoryId, IsActive)';
        CREATE INDEX IX_Products_CategoryId_IsActive ON dbo.Products(CategoryId, IsActive);
    END
END
GO

-- OtpVerifications indexes (ensure table exists)
IF OBJECT_ID('dbo.OtpVerifications') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OtpVerifications_Email' AND object_id = OBJECT_ID('dbo.OtpVerifications'))
    BEGIN
        PRINT N'Creating index IX_OtpVerifications_Email on dbo.OtpVerifications(Email)';
        CREATE INDEX IX_OtpVerifications_Email ON dbo.OtpVerifications(Email);
    END
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OtpVerifications_Email_IsUsed' AND object_id = OBJECT_ID('dbo.OtpVerifications'))
    BEGIN
        PRINT N'Creating index IX_OtpVerifications_Email_IsUsed on dbo.OtpVerifications(Email, IsUsed)';
        CREATE INDEX IX_OtpVerifications_Email_IsUsed ON dbo.OtpVerifications(Email, IsUsed);
    END
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OtpVerifications_ExpiresAt' AND object_id = OBJECT_ID('dbo.OtpVerifications'))
    BEGIN
        PRINT N'Creating index IX_OtpVerifications_ExpiresAt on dbo.OtpVerifications(ExpiresAt)';
        CREATE INDEX IX_OtpVerifications_ExpiresAt ON dbo.OtpVerifications(ExpiresAt);
    END
END
GO

PRINT N'Ensure_Migrations_Patched.sql completed.'
GO

-- 3) Ensure EF Migrations History contains entries for known migrations so EF Core
-- will consider them applied when the code checks __EFMigrationsHistory.
IF OBJECT_ID(N'dbo.__EFMigrationsHistory') IS NULL
BEGIN
    CREATE TABLE dbo.__EFMigrationsHistory (
        MigrationId nvarchar(150) NOT NULL,
        ProductVersion nvarchar(32) NOT NULL,
        CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );
END
GO

-- Insert migration records if missing (idempotent)
IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = '20251021145127_AddOtpVerification')
BEGIN
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251021145127_AddOtpVerification', '7.0.0');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = '20251021170153_AddProfileImageUrlToUser')
BEGIN
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251021170153_AddProfileImageUrlToUser', '7.0.0');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = '20251030032532_AddPerformanceIndexes')
BEGIN
    INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20251030032532_AddPerformanceIndexes', '7.0.0');
END
GO

PRINT N'EF Migrations history ensured.'
