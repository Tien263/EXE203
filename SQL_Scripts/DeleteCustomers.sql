-- Script xóa dữ liệu khách hàng và đơn hàng
USE MocViStoreDB;
GO

-- 1. Xóa OrderDetails trước (vì có foreign key)
DELETE FROM OrderDetails;
GO

-- 2. Xóa Orders
DELETE FROM Orders;
GO

-- 3. Xóa CartItems
DELETE FROM CartItems;
GO

-- 4. Xóa Carts
DELETE FROM Carts;
GO

-- 5. Xóa Users (GIỮ LẠI Admin và Staff)
DELETE FROM Users 
WHERE Role NOT IN ('Admin', 'Staff');
GO

-- Reset identity seed
DBCC CHECKIDENT ('OrderDetails', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('CartItems', RESEED, 0);
DBCC CHECKIDENT ('Carts', RESEED, 0);
GO

PRINT 'Đã xóa tất cả khách hàng và đơn hàng (giữ lại Admin/Staff)!';
GO

-- Hiển thị Users còn lại
SELECT UserId, FullName, Email, Role 
FROM Users;
GO
