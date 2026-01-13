-- ================================================
-- SCRIPT NHANH: Tạo Tài Khoản Staff
-- ================================================
-- Chạy script này sau khi đã đăng ký tài khoản qua web

USE MocViStoreDB;
GO

PRINT N'=== BẮT ĐẦU TẠO TÀI KHOẢN STAFF ===';
PRINT N'';

-- ================================================
-- BƯỚC 1: Tạo Employee
-- ================================================
PRINT N'[1/3] Tạo nhân viên...';

IF NOT EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = 'NV001')
BEGIN
    INSERT INTO Employees (
        EmployeeCode, 
        FullName, 
        Gender,
        PhoneNumber, 
        Email, 
        Position, 
        Department, 
        Salary,
        IsActive, 
        CreatedDate
    )
    VALUES (
        'NV001', 
        N'Nguyễn Văn A', 
        N'Nam',
        '0901234567', 
        'staff@mocvistore.com',
        N'Nhân viên bán hàng',
        N'Bán hàng',
        8000000,
        1,
        GETDATE()
    );
    PRINT N'   ✅ Đã tạo nhân viên NV001 - Nguyễn Văn A';
END
ELSE
BEGIN
    PRINT N'   ℹ️  Nhân viên NV001 đã tồn tại';
END
GO

-- ================================================
-- BƯỚC 2: Update User thành Staff
-- ================================================
PRINT N'';
PRINT N'[2/3] Cập nhật tài khoản thành Staff...';

DECLARE @EmployeeId INT;
SELECT @EmployeeId = EmployeeId FROM Employees WHERE EmployeeCode = 'NV001';

-- Kiểm tra user đã tồn tại chưa
IF EXISTS (SELECT 1 FROM Users WHERE Email = 'staff@mocvistore.com')
BEGIN
    -- Update user hiện tại
    UPDATE Users 
    SET 
        Role = 'Staff',
        EmployeeId = @EmployeeId
    WHERE Email = 'staff@mocvistore.com';
    
    PRINT N'   ✅ Đã cập nhật tài khoản staff@mocvistore.com thành Staff';
END
ELSE
BEGIN
    PRINT N'   ⚠️  Chưa có tài khoản staff@mocvistore.com';
    PRINT N'   👉 Vui lòng đăng ký tài khoản này trước tại: /Auth/Register';
END
GO

-- ================================================
-- BƯỚC 3: Kiểm tra kết quả
-- ================================================
PRINT N'';
PRINT N'[3/3] Kiểm tra kết quả...';
PRINT N'';

SELECT 
    u.UserId AS [ID],
    u.Email AS [Email],
    u.FullName AS [Họ Tên],
    u.Role AS [Vai Trò],
    e.EmployeeCode AS [Mã NV],
    e.Position AS [Chức Vụ],
    e.Department AS [Phòng Ban],
    CASE WHEN u.IsActive = 1 THEN N'Hoạt động' ELSE N'Khóa' END AS [Trạng Thái]
FROM Users u
LEFT JOIN Employees e ON u.EmployeeId = e.EmployeeId
WHERE u.Email = 'staff@mocvistore.com';

-- ================================================
-- THÔNG TIN ĐĂNG NHẬP
-- ================================================
PRINT N'';
PRINT N'=== HOÀN TẤT ===';
PRINT N'';
PRINT N'📧 Email: staff@mocvistore.com';
PRINT N'🔑 Password: (password bạn đã đăng ký)';
PRINT N'🌐 URL: http://localhost:5241/Auth/Login';
PRINT N'📊 Dashboard: http://localhost:5241/Staff/Dashboard';
PRINT N'';
PRINT N'=== HƯỚNG DẪN ===';
PRINT N'1. Nếu chưa có tài khoản, đăng ký tại: /Auth/Register';
PRINT N'2. Sau khi chạy script này, đăng nhập lại';
PRINT N'3. Truy cập /Staff/Dashboard để vào hệ thống quản lý';
PRINT N'';
GO
