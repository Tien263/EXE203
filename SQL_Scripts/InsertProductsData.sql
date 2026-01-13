-- =============================================
-- Script: Insert Categories and Products
-- Mộc Vị Store - Hoa Quả Sấy Mộc Châu
-- =============================================

USE MocViStoreDB;
GO

-- =============================================
-- 1. INSERT CATEGORIES
-- =============================================

-- Xóa dữ liệu cũ nếu có (optional)
-- DELETE FROM Products;
-- DELETE FROM Categories;

-- Insert Categories
SET IDENTITY_INSERT Categories ON;

INSERT INTO Categories (CategoryId, CategoryName, Description, ImageUrl, DisplayOrder, IsActive, CreatedDate)
VALUES 
(1, N'Sản Phẩm Sấy Dẻo', N'Hoa quả sấy dẻo giữ nguyên vị ngọt tự nhiên, mềm mại', '/images/categories/say-deo.jpg', 1, 1, GETDATE()),
(2, N'Sản Phẩm Sấy Giòn', N'Hoa quả sấy giòn tan, thơm ngon, giàu chất xơ', '/images/categories/say-gion.jpg', 2, 1, GETDATE()),
(3, N'Sản Phẩm Sấy Thăng Hoa', N'Công nghệ sấy thăng hoa hiện đại, giữ nguyên dinh dưỡng', '/images/categories/say-thang-hoa.jpg', 3, 1, GETDATE()),
(4, N'Mini Size Mix', N'Gói nhỏ tiện lợi để mix nhiều loại (tối thiểu 4 pack)', '/images/categories/mini-size.jpg', 4, 1, GETDATE());

SET IDENTITY_INSERT Categories OFF;
GO

-- =============================================
-- 2. INSERT PRODUCTS - SẤY DẺO (200g)
-- =============================================

SET IDENTITY_INSERT Products ON;

-- Mận sấy dẻo 200g
INSERT INTO Products (ProductId, ProductCode, ProductName, CategoryId, Description, ShortDescription, Price, OriginalPrice, StockQuantity, Unit, Weight, ImageUrl, IsActive, IsFeatured, IsNew, ViewCount, SoldCount, Rating, CreatedDate)
VALUES 
(1, 'SD-MAN-200', N'Mận Sấy Dẻo', 1, 
N'Mận sấy dẻo Mộc Châu được chế biến từ những trái mận chín mọng, tươi ngon. Sản phẩm giữ nguyên vị chua ngọt tự nhiên, mềm mại, thơm ngon. Giàu vitamin C, chất xơ tốt cho sức khỏe.',
N'Mận sấy dẻo giữ nguyên vị chua ngọt tự nhiên', 
65000, 75000, 100, N'Gói', N'200g', '/images/products/man-say-deo.jpg', 1, 1, 1, 0, 0, 5.0, GETDATE()),

-- Xoài sấy dẻo 200g
(2, 'SD-XOAI-200', N'Xoài Sấy Dẻo', 1,
N'Xoài sấy dẻo từ xoài Mộc Châu thơm ngon, ngọt tự nhiên. Sản phẩm giữ nguyên hương vị đặc trưng của xoài tươi, mềm dẻo, không chất bảo quản.',
N'Xoài Mộc Châu thơm ngon, ngọt tự nhiên',
70000, 80000, 100, N'Gói', N'200g', '/images/products/xoai-say-deo.jpg', 1, 1, 1, 0, 0, 5.0, GETDATE()),

-- Đào sấy dẻo 200g
(3, 'SD-DAO-200', N'Đào Sấy Dẻo', 1,
N'Đào sấy dẻo Mộc Châu với vị ngọt thanh, thơm mát. Sản phẩm giữ nguyên màu sắc tự nhiên, mềm dẻo, giàu vitamin và khoáng chất.',
N'Đào sấy dẻo vị ngọt thanh, thơm mát',
65000, 75000, 100, N'Gói', N'200g', '/images/products/dao-say-deo.jpg', 1, 1, 0, 0, 0, 5.0, GETDATE()),

-- Dâu sấy dẻo 200g
(4, 'SD-DAU-200', N'Dâu Sấy Dẻo', 1,
N'Dâu sấy dẻo Mộc Châu từ dâu tây tươi ngon, giàu vitamin C. Sản phẩm có vị chua ngọt hài hòa, màu đỏ tự nhiên, mềm dẻo thơm ngon.',
N'Dâu tây sấy dẻo giàu vitamin C',
90000, 100000, 80, N'Gói', N'200g', '/images/products/dau-say-deo.jpg', 1, 1, 1, 0, 0, 5.0, GETDATE()),

-- Hồng sấy dẻo 200g
(5, 'SD-HONG-200', N'Hồng Sấy Dẻo', 1,
N'Hồng sấy dẻo Mộc Châu từ hồng giòn cao cấp. Sản phẩm giữ nguyên vị ngọt thanh, thơm mát đặc trưng của hồng tươi, mềm dẻo, bổ dưỡng.',
N'Hồng giòn sấy dẻo cao cấp',
95000, 110000, 80, N'Gói', N'200g', '/images/products/hong-say-deo.jpg', 1, 1, 1, 0, 0, 5.0, GETDATE()),

-- =============================================
-- 3. INSERT PRODUCTS - SẤY GIÒN (200g)
-- =============================================

-- Mít sấy giòn 200g
(6, 'SG-MIT-200', N'Mít Sấy Giòn', 2,
N'Mít sấy giòn Mộc Châu từ mít tươi ngon, thơm ngọt. Sản phẩm giòn tan, thơm nức, giữ nguyên hương vị đặc trưng của mít tươi. Giàu chất xơ, vitamin.',
N'Mít sấy giòn tan, thơm nức',
80000, 90000, 100, N'Gói', N'200g', '/images/products/mit-say-gion.jpg', 1, 1, 1, 0, 0, 5.0, GETDATE()),

-- Chuối sấy giòn 200g
(7, 'SG-CHUOI-200', N'Chuối Sấy Giòn', 2,
N'Chuối sấy giòn Mộc Châu từ chuối già chín tự nhiên. Sản phẩm giòn rụm, ngọt thanh, giàu kali và năng lượng. Thích hợp làm snack healthy.',
N'Chuối sấy giòn rụm, ngọt thanh',
80000, 90000, 100, N'Gói', N'200g', '/images/products/chuoi-say-gion.jpg', 1, 1, 0, 0, 0, 5.0, GETDATE()),

-- =============================================
-- 4. INSERT PRODUCTS - SẤY THĂNG HOA (100g)
-- =============================================

-- Dâu sấy thăng hoa 100g
(8, 'STH-DAU-100', N'Dâu Sấy Thăng Hoa', 3,
N'Dâu sấy thăng hoa với công nghệ hiện đại, giữ nguyên 98% dinh dưỡng. Sản phẩm giòn nhẹ, tan trong miệng, hương vị đậm đà. Không chất bảo quản.',
N'Công nghệ thăng hoa giữ nguyên dinh dưỡng',
140000, 160000, 60, N'Gói', N'100g', '/images/products/dau-say-thang-hoa.jpg', 1, 1, 1, 0, 0, 5.0, GETDATE()),

-- Sữa chua sấy thăng hoa 100g
(9, 'STH-SC-100', N'Sữa Chua Sấy Thăng Hoa', 3,
N'Sữa chua sấy thăng hoa độc đáo, mới lạ. Sản phẩm giòn tan, vị chua ngọt hài hòa, giàu men vi sinh có lợi. Thích hợp cho mọi lứa tuổi.',
N'Sữa chua sấy giòn tan, giàu men vi sinh',
95000, 110000, 60, N'Gói', N'100g', '/images/products/sua-chua-say-thang-hoa.jpg', 1, 1, 1, 0, 0, 5.0, GETDATE()),

-- =============================================
-- 5. INSERT PRODUCTS - MINI SIZE SẤY DẺO (50g)
-- =============================================

-- Mận sấy dẻo 50g
(10, 'SD-MAN-50', N'Mận Sấy Dẻo Mini', 4,
N'Mận sấy dẻo gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.',
N'Gói mini 50g tiện lợi (tối thiểu 4 pack)',
18000, 20000, 200, N'Gói', N'50g', '/images/products/man-say-deo-mini.jpg', 1, 0, 0, 0, 0, 5.0, GETDATE()),

-- Xoài sấy dẻo 50g
(11, 'SD-XOAI-50', N'Xoài Sấy Dẻo Mini', 4,
N'Xoài sấy dẻo gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.',
N'Gói mini 50g tiện lợi (tối thiểu 4 pack)',
20000, 22000, 200, N'Gói', N'50g', '/images/products/xoai-say-deo-mini.jpg', 1, 0, 0, 0, 0, 5.0, GETDATE()),

-- Đào sấy dẻo 50g
(12, 'SD-DAO-50', N'Đào Sấy Dẻo Mini', 4,
N'Đào sấy dẻo gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.',
N'Gói mini 50g tiện lợi (tối thiểu 4 pack)',
18000, 20000, 200, N'Gói', N'50g', '/images/products/dao-say-deo-mini.jpg', 1, 0, 0, 0, 0, 5.0, GETDATE()),

-- Dâu sấy dẻo 50g
(13, 'SD-DAU-50', N'Dâu Sấy Dẻo Mini', 4,
N'Dâu sấy dẻo gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.',
N'Gói mini 50g tiện lợi (tối thiểu 4 pack)',
25000, 28000, 200, N'Gói', N'50g', '/images/products/dau-say-deo-mini.jpg', 1, 0, 0, 0, 0, 5.0, GETDATE()),

-- Hồng sấy dẻo 50g
(14, 'SD-HONG-50', N'Hồng Sấy Dẻo Mini', 4,
N'Hồng sấy dẻo gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.',
N'Gói mini 50g tiện lợi (tối thiểu 4 pack)',
28000, 32000, 200, N'Gói', N'50g', '/images/products/hong-say-deo-mini.jpg', 1, 0, 0, 0, 0, 5.0, GETDATE()),

-- =============================================
-- 6. INSERT PRODUCTS - MINI SIZE SẤY GIÒN (50g)
-- =============================================

-- Mít sấy giòn 50g
(15, 'SG-MIT-50', N'Mít Sấy Giòn Mini', 4,
N'Mít sấy giòn gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.',
N'Gói mini 50g tiện lợi (tối thiểu 4 pack)',
22000, 25000, 200, N'Gói', N'50g', '/images/products/mit-say-gion-mini.jpg', 1, 0, 0, 0, 0, 5.0, GETDATE()),

-- Chuối sấy giòn 50g
(16, 'SG-CHUOI-50', N'Chuối Sấy Giòn Mini', 4,
N'Chuối sấy giòn gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.',
N'Gói mini 50g tiện lợi (tối thiểu 4 pack)',
22000, 25000, 200, N'Gói', N'50g', '/images/products/chuoi-say-gion-mini.jpg', 1, 0, 0, 0, 0, 5.0, GETDATE()),

-- =============================================
-- 7. INSERT PRODUCTS - MINI SIZE SẤY THĂNG HOA (50g)
-- =============================================

-- Dâu sấy thăng hoa 50g
(17, 'STH-DAU-50', N'Dâu Sấy Thăng Hoa Mini', 4,
N'Dâu sấy thăng hoa gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.',
N'Gói mini 50g tiện lợi (tối thiểu 4 pack)',
75000, 85000, 150, N'Gói', N'50g', '/images/products/dau-say-thang-hoa-mini.jpg', 1, 0, 0, 0, 0, 5.0, GETDATE()),

-- Sữa chua sấy thăng hoa 50g
(18, 'STH-SC-50', N'Sữa Chua Sấy Thăng Hoa Mini', 4,
N'Sữa chua sấy thăng hoa gói mini 50g tiện lợi. Thích hợp để mix nhiều loại, mang theo du lịch. Tối thiểu đặt 4 pack.',
N'Gói mini 50g tiện lợi (tối thiểu 4 pack)',
50000, 58000, 150, N'Gói', N'50g', '/images/products/sua-chua-say-thang-hoa-mini.jpg', 1, 0, 0, 0, 0, 5.0, GETDATE());

SET IDENTITY_INSERT Products OFF;
GO

-- =============================================
-- 8. VERIFY DATA
-- =============================================

-- Kiểm tra Categories
SELECT * FROM Categories ORDER BY DisplayOrder;

-- Kiểm tra Products theo Category
SELECT 
    c.CategoryName,
    p.ProductCode,
    p.ProductName,
    p.Weight,
    p.Price,
    p.OriginalPrice,
    p.StockQuantity,
    p.IsActive
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.CategoryId
ORDER BY c.DisplayOrder, p.ProductId;

-- Thống kê
SELECT 
    c.CategoryName,
    COUNT(p.ProductId) AS TotalProducts,
    MIN(p.Price) AS MinPrice,
    MAX(p.Price) AS MaxPrice
FROM Categories c
LEFT JOIN Products p ON c.CategoryId = p.CategoryId
GROUP BY c.CategoryName, c.DisplayOrder
ORDER BY c.DisplayOrder;

PRINT 'Data inserted successfully!';
GO
