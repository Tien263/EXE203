-- Tạo voucher TEST10 không yêu cầu đơn hàng tối thiểu
DELETE FROM Vouchers WHERE VoucherCode = 'TEST10';

INSERT INTO Vouchers (VoucherCode, VoucherName, DiscountType, DiscountValue, MinOrderAmount, MaxDiscountAmount, UsageLimit, UsedCount, ValidFrom, ValidTo, IsActive, CreatedDate)
VALUES ('TEST10', N'Test voucher giảm 10k', 'Fixed', 10000, NULL, NULL, 100, 0, '2025-01-01', '2025-12-31', 1, GETDATE());

SELECT * FROM Vouchers WHERE VoucherCode = 'TEST10';
