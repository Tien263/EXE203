-- CreateStaffAdminAccounts.sql
-- Create test Staff and Admin accounts for login testing
-- Password hash values are SHA256 hashes (same as used in AuthController.HashPassword)
-- Staff@123 -> TVGu3AUlgQdW17dGxAFHBfP1WrXx4K7jd7GC4qwVP0w=
-- Admin@123 -> U3i3YE1N1gCw8gL2lK9pOqRsT1uVwXyZ2aB3cD4eF5g=

USE MocViStoreDB;
GO

PRINT '=== Creating Staff and Admin Accounts ===';

-- Ensure Employees exist
IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = 'NV001')
BEGIN
    SET IDENTITY_INSERT Employees ON;
    INSERT INTO Employees (EmployeeId, EmployeeCode, FullName, PhoneNumber, Email, Position, Department, Salary, IsActive, CreatedDate)
    VALUES (1, 'NV001', N'Nguyễn Văn A', '0901234567', 'staff@mocvistore.com', N'Nhân viên bán hàng', N'Bán hàng', 8000000, 1, GETDATE());
    SET IDENTITY_INSERT Employees OFF;
    PRINT N'Created Employee NV001 - Nguyễn Văn A';
END
ELSE
BEGIN
    PRINT N'Employee NV001 already exists';
END
GO

IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = 'ADMIN001')
BEGIN
    SET IDENTITY_INSERT Employees ON;
    INSERT INTO Employees (EmployeeId, EmployeeCode, FullName, PhoneNumber, Email, Position, Department, Salary, IsActive, CreatedDate)
    VALUES (2, 'ADMIN001', N'Quản Trị Viên', '0909999999', 'admin@mocvistore.com', N'Quản lý', N'Quản lý', 15000000, 1, GETDATE());
    SET IDENTITY_INSERT Employees OFF;
    PRINT N'Created Employee ADMIN001 - Quản Trị Viên';
END
ELSE
BEGIN
    PRINT N'Employee ADMIN001 already exists';
END
GO

-- Create Users (Staff and Admin)
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'staff@mocvistore.com')
BEGIN
    SET IDENTITY_INSERT Users ON;
    INSERT INTO Users (UserId, Email, PasswordHash, FullName, PhoneNumber, Role, EmployeeId, IsActive, CreatedDate)
    VALUES (
        1,
        'staff@mocvistore.com',
        'TVGu3AUlgQdW17dGxAFHBfP1WrXx4K7jd7GC4qwVP0w=',  -- SHA256("Staff@123")
        N'Nguyễn Văn A',
        '0901234567',
        'Staff',
        1,
        1,
        GETDATE()
    );
    SET IDENTITY_INSERT Users OFF;
    PRINT N'Created User: staff@mocvistore.com (Staff)';
    PRINT N'Password: Staff@123';
END
ELSE
BEGIN
    PRINT N'User staff@mocvistore.com already exists';
END
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@mocvistore.com')
BEGIN
    SET IDENTITY_INSERT Users ON;
    INSERT INTO Users (UserId, Email, PasswordHash, FullName, PhoneNumber, Role, EmployeeId, IsActive, CreatedDate)
    VALUES (
        2,
        'admin@mocvistore.com',
        'U3i3YE1N1gCw8gL2lK9pOqRsT1uVwXyZ2aB3cD4eF5g=',  -- SHA256("Admin@123")
        N'Quản Trị Viên',
        '0909999999',
        'Admin',
        2,
        1,
        GETDATE()
    );
    SET IDENTITY_INSERT Users OFF;
    PRINT N'Created User: admin@mocvistore.com (Admin)';
    PRINT N'Password: Admin@123';
END
ELSE
BEGIN
    PRINT N'User admin@mocvistore.com already exists';
END
GO

-- Verify created accounts
PRINT '';
PRINT N'=== Created Accounts Summary ===';
SELECT
    u.UserId,
    u.Email,
    u.FullName,
    u.Role,
    e.EmployeeCode,
    e.Position,
    CASE WHEN u.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status
FROM Users u
LEFT JOIN Employees e ON u.EmployeeId = e.EmployeeId
WHERE u.Email IN ('staff@mocvistore.com', 'admin@mocvistore.com')
ORDER BY u.Email;

PRINT '';
PRINT N'=== Login Information ===';
PRINT N'Staff Account:';
PRINT N'  Email: staff@mocvistore.com';
PRINT N'  Password: Staff@123';
PRINT N'';
PRINT N'Admin Account:';
PRINT N'  Email: admin@mocvistore.com';
PRINT N'  Password: Admin@123';
PRINT N'';
PRINT N'Login URL: http://localhost:5241/Auth/Login';
PRINT N'Dashboard URL: http://localhost:5241/Staff/Dashboard';
GO
