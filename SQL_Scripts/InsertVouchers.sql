-- Insert Sample Vouchers for Testing
-- Run this script to create test vouchers in the database

USE MocViStore;
GO

-- Xóa voucher cũ nếu có
DELETE FROM Vouchers WHERE VoucherCode IN ('SUMMER2025', 'WELCOME10', 'FREESHIP');
GO

-- Insert vouchers
INSERT INTO Vouchers (VoucherCode, VoucherName, DiscountType, DiscountValue, MinOrderAmount, MaxDiscountAmount, UsageLimit, UsedCount, ValidFrom, ValidTo, IsActive, CreatedDate)
VALUES 
-- Voucher giảm 10% tối đa 50k
('SUMMER2025', N'Giảm giá mùa hè 2025', 'Percent', 10, 100000, 50000, 100, 0, '2025-01-01', '2025-12-31', 1, GETDATE()),

-- Voucher chào mừng giảm 10k
('WELCOME10', N'Chào mừng khách hàng mới', 'Fixed', 10000, 50000, NULL, 50, 0, '2025-01-01', '2025-12-31', 1, GETDATE()),

-- Voucher freeship
('FREESHIP', N'Miễn phí vận chuyển', 'Fixed', 20000, 0, NULL, NULL, 0, '2025-01-01', '2025-12-31', 1, GETDATE());
GO

-- Kiểm tra kết quả
SELECT 
    VoucherCode,
    VoucherName,
    DiscountType,
    DiscountValue,
    MinOrderAmount,
    MaxDiscountAmount,
    UsageLimit,
    UsedCount,
    IsActive,
    ValidFrom,
    ValidTo
FROM Vouchers
WHERE VoucherCode IN ('SUMMER2025', 'WELCOME10', 'FREESHIP');
GO

PRINT 'Vouchers inserted successfully!';
