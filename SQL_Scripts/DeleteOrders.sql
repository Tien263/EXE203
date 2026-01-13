-- Script xóa dữ liệu đơn hàng
USE MocViStoreDB;
GO

-- Xóa tất cả OrderDetails trước (vì có foreign key)
DELETE FROM OrderDetails;
GO

-- Xóa tất cả Orders
DELETE FROM Orders;
GO

-- Reset identity seed (để ID bắt đầu lại từ 1)
DBCC CHECKIDENT ('OrderDetails', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
GO

PRINT 'Đã xóa tất cả đơn hàng thành công!';
GO
