-- Tạo voucher SUMMER2025 để test
-- Copy và chạy trực tiếp trong SQL Server Management Studio

-- Xóa nếu đã tồn tại
DELETE FROM Vouchers WHERE VoucherCode = 'SUMMER2025';

-- Tạo voucher mới
INSERT INTO Vouchers (VoucherCode, VoucherName, DiscountType, DiscountValue, MinOrderAmount, MaxDiscountAmount, UsageLimit, UsedCount, ValidFrom, ValidTo, IsActive, CreatedDate)
VALUES ('SUMMER2025', N'Giảm giá mùa hè 2025', 'Percent', 10, 100000, 50000, 100, 0, '2025-01-01', '2025-12-31', 1, GETDATE());

-- Kiểm tra
SELECT * FROM Vouchers WHERE VoucherCode = 'SUMMER2025';
