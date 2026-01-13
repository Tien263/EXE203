-- ============================================
-- Script: Clear Orders and Customers Data
-- Description: Xóa tất cả dữ liệu đơn hàng và khách hàng
-- Giữ lại: Products, Categories, Users (Admin/Staff)
-- ============================================

USE [MocViStoreDB]
GO

PRINT '🗑️  Bắt đầu xóa dữ liệu...'
GO

-- 1. Xóa OrderDetails (chi tiết đơn hàng)
PRINT '1️⃣  Xóa OrderDetails...'
DELETE FROM [dbo].[OrderDetails]
PRINT '   ✅ Đã xóa ' + CAST(@@ROWCOUNT AS VARCHAR) + ' OrderDetails'
GO

-- 2. Xóa Orders (đơn hàng)
PRINT '2️⃣  Xóa Orders...'
DELETE FROM [dbo].[Orders]
PRINT '   ✅ Đã xóa ' + CAST(@@ROWCOUNT AS VARCHAR) + ' Orders'
GO

-- 3. Xóa Carts (giỏ hàng) - nếu tồn tại
PRINT '3️⃣  Xóa Carts...'
IF OBJECT_ID('dbo.Carts', 'U') IS NOT NULL
BEGIN
    DELETE FROM [dbo].[Carts]
    PRINT '   ✅ Đã xóa ' + CAST(@@ROWCOUNT AS VARCHAR) + ' Carts'
END
ELSE
BEGIN
    PRINT '   ⚠️  Bảng Carts không tồn tại'
END
GO

-- 4. Xóa Reviews (đánh giá sản phẩm)
PRINT '4️⃣  Xóa Reviews...'
DELETE FROM [dbo].[Reviews]
PRINT '   ✅ Đã xóa ' + CAST(@@ROWCOUNT AS VARCHAR) + ' Reviews'
GO

-- 5. Xóa Users (chỉ xóa customers, giữ lại Admin/Staff) - TRƯỚC KHI XÓA CUSTOMERS
PRINT '5️⃣  Xóa Users (Customer role)...'
DELETE FROM [dbo].[Users] 
WHERE Role = 'Customer'
PRINT '   ✅ Đã xóa ' + CAST(@@ROWCOUNT AS VARCHAR) + ' Customer Users'
GO

-- 6. Xóa Customers (khách hàng) - SAU KHI XÓA USERS
PRINT '6️⃣  Xóa Customers...'
DELETE FROM [dbo].[Customers]
PRINT '   ✅ Đã xóa ' + CAST(@@ROWCOUNT AS VARCHAR) + ' Customers'
GO

-- 7. Reset Identity (nếu muốn bắt đầu lại từ ID = 1)
PRINT '7️⃣  Reset Identity Seeds...'

-- Reset OrderDetails
IF EXISTS (SELECT * FROM [dbo].[OrderDetails])
BEGIN
    PRINT '   ⚠️  OrderDetails không trống, không reset identity'
END
ELSE
BEGIN
    DBCC CHECKIDENT ('[dbo].[OrderDetails]', RESEED, 0)
    PRINT '   ✅ Reset OrderDetails identity'
END

-- Reset Orders
IF EXISTS (SELECT * FROM [dbo].[Orders])
BEGIN
    PRINT '   ⚠️  Orders không trống, không reset identity'
END
ELSE
BEGIN
    DBCC CHECKIDENT ('[dbo].[Orders]', RESEED, 0)
    PRINT '   ✅ Reset Orders identity'
END

-- Reset Carts (nếu tồn tại)
IF OBJECT_ID('dbo.Carts', 'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT * FROM [dbo].[Carts])
    BEGIN
        PRINT '   ⚠️  Carts không trống, không reset identity'
    END
    ELSE
    BEGIN
        DBCC CHECKIDENT ('[dbo].[Carts]', RESEED, 0)
        PRINT '   ✅ Reset Carts identity'
    END
END
ELSE
BEGIN
    PRINT '   ⚠️  Bảng Carts không tồn tại'
END

-- Reset Reviews
IF EXISTS (SELECT * FROM [dbo].[Reviews])
BEGIN
    PRINT '   ⚠️  Reviews không trống, không reset identity'
END
ELSE
BEGIN
    DBCC CHECKIDENT ('[dbo].[Reviews]', RESEED, 0)
    PRINT '   ✅ Reset Reviews identity'
END

-- Reset Customers
IF EXISTS (SELECT * FROM [dbo].[Customers])
BEGIN
    PRINT '   ⚠️  Customers không trống, không reset identity'
END
ELSE
BEGIN
    DBCC CHECKIDENT ('[dbo].[Customers]', RESEED, 0)
    PRINT '   ✅ Reset Customers identity'
END

GO

-- 8. Kiểm tra kết quả
PRINT ''
PRINT '📊 Kết quả sau khi xóa:'
PRINT '================================'

SELECT 'OrderDetails' AS TableName, COUNT(*) AS RecordCount FROM [dbo].[OrderDetails]
UNION ALL
SELECT 'Orders', COUNT(*) FROM [dbo].[Orders]
UNION ALL
SELECT 'Reviews', COUNT(*) FROM [dbo].[Reviews]
UNION ALL
SELECT 'Customers', COUNT(*) FROM [dbo].[Customers]
UNION ALL
SELECT 'Users (Customer)', COUNT(*) FROM [dbo].[Users] WHERE Role = 'Customer'
UNION ALL
SELECT 'Users (Admin/Staff)', COUNT(*) FROM [dbo].[Users] WHERE Role IN ('Admin', 'Staff')
UNION ALL
SELECT 'Products', COUNT(*) FROM [dbo].[Products]
UNION ALL
SELECT 'Categories', COUNT(*) FROM [dbo].[Categories]

PRINT ''
PRINT '✅ Hoàn thành! Dữ liệu đơn hàng và khách hàng đã được xóa.'
PRINT '✅ Sản phẩm và Categories vẫn được giữ nguyên.'
GO
