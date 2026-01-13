-- Script tạo tài khoản Staff mẫu
-- Chạy script này để tạo nhân viên và tài khoản Staff để test hệ thống

USE MocViStore;
GO

-- 1. Tạo nhân viên mẫu (nếu chưa có)
IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = 'NV001')
BEGIN
    INSERT INTO Employees (
        EmployeeCode, FullName, Gender, PhoneNumber, Email, 
        Address, Position, Department, Salary, IsActive, CreatedDate
    )
    VALUES (
        'NV001', 
        N'Nguyễn Văn A', 
        N'Nam', 
        '0901234567', 
        'staff@mocvistore.com',
        N'123 Đường ABC, Quận 1, TP.HCM',
        N'Nhân viên bán hàng',
        N'Bán hàng',
        8000000,
        1,
        GETDATE()
    );
    
    PRINT N'Đã tạo nhân viên NV001 - Nguyễn Văn A';
END
ELSE
BEGIN
    PRINT N'Nhân viên NV001 đã tồn tại';
END
GO

-- 2. Lấy EmployeeId vừa tạo
DECLARE @EmployeeId INT;
SELECT @EmployeeId = EmployeeId FROM Employees WHERE EmployeeCode = 'NV001';

-- 3. Tạo tài khoản User cho Staff (nếu chưa có)
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'staff@mocvistore.com')
BEGIN
    -- Password: Staff@123 (đã hash bằng BCrypt)
    -- Bạn cần hash password này bằng BCrypt trong code C#
    INSERT INTO Users (
        Email, PasswordHash, FullName, PhoneNumber, 
        Role, EmployeeId, IsActive, CreatedDate
    )
    VALUES (
        'staff@mocvistore.com',
        '$2a$11$YourHashedPasswordHere', -- Cần thay bằng password đã hash
        N'Nguyễn Văn A',
        '0901234567',
        'Staff',
        @EmployeeId,
        1,
        GETDATE()
    );
    
    PRINT N'Đã tạo tài khoản Staff: staff@mocvistore.com';
    PRINT N'Password mặc định: Staff@123';
    PRINT N'LƯU Ý: Cần hash password bằng BCrypt trong code C#!';
END
ELSE
BEGIN
    PRINT N'Tài khoản staff@mocvistore.com đã tồn tại';
END
GO

-- 4. Tạo thêm một số nhân viên khác (tùy chọn)
IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = 'NV002')
BEGIN
    INSERT INTO Employees (
        EmployeeCode, FullName, Gender, PhoneNumber, Email, 
        Address, Position, Department, Salary, IsActive, CreatedDate
    )
    VALUES (
        'NV002', 
        N'Trần Thị B', 
        N'Nữ', 
        '0907654321', 
        'staff2@mocvistore.com',
        N'456 Đường XYZ, Quận 3, TP.HCM',
        N'Nhân viên bán hàng',
        N'Bán hàng',
        8000000,
        1,
        GETDATE()
    );
    
    PRINT N'Đã tạo nhân viên NV002 - Trần Thị B';
END
GO

-- 5. Tạo Admin account (nếu cần)
IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = 'ADMIN001')
BEGIN
    INSERT INTO Employees (
        EmployeeCode, FullName, Gender, PhoneNumber, Email, 
        Address, Position, Department, Salary, IsActive, CreatedDate
    )
    VALUES (
        'ADMIN001', 
        N'Quản Trị Viên', 
        N'Nam', 
        '0909999999', 
        'admin@mocvistore.com',
        N'Văn phòng chính',
        N'Quản lý',
        N'Quản lý',
        15000000,
        1,
        GETDATE()
    );
    
    PRINT N'Đã tạo nhân viên ADMIN001 - Quản Trị Viên';
END
GO

DECLARE @AdminEmployeeId INT;
SELECT @AdminEmployeeId = EmployeeId FROM Employees WHERE EmployeeCode = 'ADMIN001';

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@mocvistore.com')
BEGIN
    INSERT INTO Users (
        Email, PasswordHash, FullName, PhoneNumber, 
        Role, EmployeeId, IsActive, CreatedDate
    )
    VALUES (
        'admin@mocvistore.com',
        '$2a$11$YourHashedPasswordHere', -- Cần thay bằng password đã hash
        N'Quản Trị Viên',
        '0909999999',
        'Admin',
        @AdminEmployeeId,
        1,
        GETDATE()
    );
    
    PRINT N'Đã tạo tài khoản Admin: admin@mocvistore.com';
    PRINT N'Password mặc định: Admin@123';
    PRINT N'LƯU Ý: Cần hash password bằng BCrypt trong code C#!';
END
GO

-- 6. Hiển thị thông tin tài khoản đã tạo
PRINT N'';
PRINT N'=== DANH SÁCH TÀI KHOẢN STAFF ===';
SELECT 
    u.UserId,
    u.Email,
    u.FullName,
    u.Role,
    e.EmployeeCode,
    e.Position,
    e.Department,
    u.IsActive
FROM Users u
LEFT JOIN Employees e ON u.EmployeeId = e.EmployeeId
WHERE u.Role IN ('Staff', 'Admin')
ORDER BY u.Role, u.Email;

PRINT N'';
PRINT N'=== HƯỚNG DẪN ===';
PRINT N'1. Cần hash password bằng BCrypt trong code C#';
PRINT N'2. Đăng nhập vào hệ thống với email và password đã tạo';
PRINT N'3. Truy cập /Staff/Dashboard để vào hệ thống quản lý';
PRINT N'';
PRINT N'=== TÀI KHOẢN MẪU ===';
PRINT N'Staff: staff@mocvistore.com / Staff@123';
PRINT N'Admin: admin@mocvistore.com / Admin@123';
GO
