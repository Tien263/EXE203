-- =============================================
-- Má»™c Vá»‹ Store Database Schema - EXTENDED VERSION
-- Há»— trá»£ bÃ¡n hÃ ng trá»±c tiáº¿p + Quáº£n lÃ½ khÃ¡ch hÃ ng
-- =============================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MocViStoreDB')
BEGIN
    CREATE DATABASE MocViStoreDB;
END
GO

USE MocViStoreDB;
GO

-- =============================================
-- Table: Categories (Danh má»¥c sáº£n pháº©m)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories (
        CategoryId INT PRIMARY KEY IDENTITY(1,1),
        CategoryName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        ImageUrl NVARCHAR(255),
        DisplayOrder INT DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME
    );
END
GO

-- =============================================
-- Table: Products (Sáº£n pháº©m)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        ProductId INT PRIMARY KEY IDENTITY(1,1),
        ProductCode NVARCHAR(50) UNIQUE NOT NULL, -- MÃ£ sáº£n pháº©m Ä‘á»ƒ quÃ©t barcode
        ProductName NVARCHAR(200) NOT NULL,
        CategoryId INT NOT NULL,
        Description NVARCHAR(MAX),
        ShortDescription NVARCHAR(500),
        Price DECIMAL(18,2) NOT NULL,
        OriginalPrice DECIMAL(18,2),
        CostPrice DECIMAL(18,2), -- GiÃ¡ vá»‘n
        DiscountPercent INT DEFAULT 0,
        StockQuantity INT DEFAULT 0,
        MinStockLevel INT DEFAULT 10, -- Má»©c tá»“n kho tá»‘i thiá»ƒu
        Unit NVARCHAR(50) DEFAULT N'gÃ³i',
        Weight NVARCHAR(50),
        ImageUrl NVARCHAR(255),
        ImageGallery NVARCHAR(MAX),
        IsActive BIT DEFAULT 1,
        IsFeatured BIT DEFAULT 0,
        IsNew BIT DEFAULT 0,
        ViewCount INT DEFAULT 0,
        SoldCount INT DEFAULT 0,
        Rating DECIMAL(3,2) DEFAULT 0,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME,
        FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
    );
END
GO

-- =============================================
-- Table: Suppliers (NhÃ  cung cáº¥p)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Suppliers')
BEGIN
    CREATE TABLE Suppliers (
        SupplierId INT PRIMARY KEY IDENTITY(1,1),
        SupplierCode NVARCHAR(50) UNIQUE NOT NULL,
        SupplierName NVARCHAR(200) NOT NULL,
        ContactPerson NVARCHAR(100),
        PhoneNumber NVARCHAR(20),
        Email NVARCHAR(100),
        Address NVARCHAR(255),
        City NVARCHAR(100),
        District NVARCHAR(100),
        TaxCode NVARCHAR(50),
        BankAccount NVARCHAR(50),
        BankName NVARCHAR(100),
        Notes NVARCHAR(500),
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME
    );
END
GO

-- =============================================
-- Table: PurchaseOrders (ÄÆ¡n nháº­p hÃ ng)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseOrders')
BEGIN
    CREATE TABLE PurchaseOrders (
        PurchaseOrderId INT PRIMARY KEY IDENTITY(1,1),
        PurchaseOrderCode NVARCHAR(50) UNIQUE NOT NULL,
        SupplierId INT NOT NULL,
        EmployeeId INT, -- NhÃ¢n viÃªn nháº­p hÃ ng
        TotalAmount DECIMAL(18,2) NOT NULL,
        PaidAmount DECIMAL(18,2) DEFAULT 0,
        RemainingAmount DECIMAL(18,2) DEFAULT 0,
        Status NVARCHAR(50) DEFAULT N'Chá» duyá»‡t', -- Chá» duyá»‡t, ÄÃ£ duyá»‡t, ÄÃ£ nháº­p kho, ÄÃ£ há»§y
        Notes NVARCHAR(500),
        OrderDate DATETIME DEFAULT GETDATE(),
        ReceivedDate DATETIME,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME,
        FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId)
    );
END
GO

-- =============================================
-- Table: PurchaseOrderDetails (Chi tiáº¿t Ä‘Æ¡n nháº­p)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseOrderDetails')
BEGIN
    CREATE TABLE PurchaseOrderDetails (
        PurchaseOrderDetailId INT PRIMARY KEY IDENTITY(1,1),
        PurchaseOrderId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        TotalPrice DECIMAL(18,2) NOT NULL,
        FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrders(PurchaseOrderId),
        FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
    );
END
GO

-- =============================================
-- Table: InventoryTransactions (Lá»‹ch sá»­ xuáº¥t nháº­p kho)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InventoryTransactions')
BEGIN
    CREATE TABLE InventoryTransactions (
        TransactionId INT PRIMARY KEY IDENTITY(1,1),
        ProductId INT NOT NULL,
        TransactionType NVARCHAR(50) NOT NULL, -- Nháº­p kho, Xuáº¥t kho, Kiá»ƒm kÃª, Há»§y hÃ ng
        Quantity INT NOT NULL, -- Sá»‘ lÆ°á»£ng (+ nháº­p, - xuáº¥t)
        ReferenceType NVARCHAR(50), -- PurchaseOrder, Order, Adjustment
        ReferenceId INT, -- ID cá»§a Ä‘Æ¡n hÃ ng/phiáº¿u nháº­p
        Notes NVARCHAR(500),
        EmployeeId INT,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
    );
END
GO

-- =============================================
-- Table: Customers (KhÃ¡ch hÃ ng - tÃ¡ch riÃªng)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    CREATE TABLE Customers (
        CustomerId INT PRIMARY KEY IDENTITY(1,1),
        CustomerCode NVARCHAR(50) UNIQUE, -- MÃ£ khÃ¡ch hÃ ng
        FullName NVARCHAR(100) NOT NULL,
        PhoneNumber NVARCHAR(20) NOT NULL,
        Email NVARCHAR(100),
        DateOfBirth DATE,
        Gender NVARCHAR(10), -- Nam, Ná»¯, KhÃ¡c
        Address NVARCHAR(255),
        City NVARCHAR(100),
        District NVARCHAR(100),
        Ward NVARCHAR(100),
        CustomerType NVARCHAR(50) DEFAULT N'ThÆ°á»ng', -- ThÆ°á»ng, VIP, Äáº¡i lÃ½
        TotalPurchased DECIMAL(18,2) DEFAULT 0, -- Tá»•ng tiá»n Ä‘Ã£ mua
        TotalOrders INT DEFAULT 0, -- Tá»•ng sá»‘ Ä‘Æ¡n hÃ ng
        LoyaltyPoints INT DEFAULT 0, -- Äiá»ƒm tÃ­ch lÅ©y
        Notes NVARCHAR(500),
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME,
        LastPurchaseDate DATETIME
    );
END
GO

-- =============================================
-- Table: Employees (NhÃ¢n viÃªn)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
BEGIN
    CREATE TABLE Employees (
        EmployeeId INT PRIMARY KEY IDENTITY(1,1),
        EmployeeCode NVARCHAR(50) UNIQUE NOT NULL,
        FullName NVARCHAR(100) NOT NULL,
        PhoneNumber NVARCHAR(20),
        Email NVARCHAR(100),
        DateOfBirth DATE,
        Gender NVARCHAR(10),
        Address NVARCHAR(255),
        Position NVARCHAR(100), -- Quáº£n lÃ½, Thu ngÃ¢n, NhÃ¢n viÃªn bÃ¡n hÃ ng, Kho
        Department NVARCHAR(100), -- BÃ¡n hÃ ng, Kho, Káº¿ toÃ¡n
        Salary DECIMAL(18,2),
        HireDate DATE,
        IdentityCard NVARCHAR(20),
        BankAccount NVARCHAR(50),
        BankName NVARCHAR(100),
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME
    );
END
GO

-- =============================================
-- Table: Users (TÃ i khoáº£n Ä‘Äƒng nháº­p - liÃªn káº¿t Employee)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT PRIMARY KEY IDENTITY(1,1),
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        FullName NVARCHAR(100) NOT NULL,
        PhoneNumber NVARCHAR(20),
        ProfileImageUrl NVARCHAR(MAX) NULL,
        Role NVARCHAR(20) DEFAULT 'Customer', -- Admin, Manager, Cashier, Staff, Customer
        EmployeeId INT, -- LiÃªn káº¿t vá»›i nhÃ¢n viÃªn náº¿u lÃ  tÃ i khoáº£n ná»™i bá»™
        CustomerId INT, -- LiÃªn káº¿t vá»›i khÃ¡ch hÃ ng náº¿u lÃ  tÃ i khoáº£n online
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        LastLoginDate DATETIME,
        FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId),
        FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
    );
END
GO

-- =============================================
-- Table: Orders (ÄÆ¡n hÃ ng - cáº£ online vÃ  offline)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        OrderId INT PRIMARY KEY IDENTITY(1,1),
        OrderCode NVARCHAR(50) UNIQUE NOT NULL,
        CustomerId INT, -- KhÃ¡ch hÃ ng
        EmployeeId INT, -- NhÃ¢n viÃªn xá»­ lÃ½ (náº¿u bÃ¡n trá»±c tiáº¿p)
        OrderType NVARCHAR(50) DEFAULT N'Online', -- Online, POS (bÃ¡n trá»±c tiáº¿p)
        CustomerName NVARCHAR(100) NOT NULL,
        CustomerEmail NVARCHAR(100),
        CustomerPhone NVARCHAR(20) NOT NULL,
        ShippingAddress NVARCHAR(255),
        City NVARCHAR(100),
        District NVARCHAR(100),
        Ward NVARCHAR(100),
        TotalAmount DECIMAL(18,2) NOT NULL,
        ShippingFee DECIMAL(18,2) DEFAULT 0,
        DiscountAmount DECIMAL(18,2) DEFAULT 0,
        VoucherCode NVARCHAR(50),
        LoyaltyPointsUsed INT DEFAULT 0,
        LoyaltyPointsEarned INT DEFAULT 0,
        FinalAmount DECIMAL(18,2) NOT NULL,
        PaymentMethod NVARCHAR(50), -- Tiá»n máº·t, Chuyá»ƒn khoáº£n, Tháº», Momo, COD
        PaymentStatus NVARCHAR(50) DEFAULT N'ChÆ°a thanh toÃ¡n',
        OrderStatus NVARCHAR(50) DEFAULT N'Chá» xÃ¡c nháº­n',
        Note NVARCHAR(500),
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME,
        CompletedDate DATETIME,
        FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
        FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
    );
END
GO

-- =============================================
-- Table: OrderDetails (Chi tiáº¿t Ä‘Æ¡n hÃ ng)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderDetails')
BEGIN
    CREATE TABLE OrderDetails (
        OrderDetailId INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        ProductId INT NOT NULL,
        ProductName NVARCHAR(200) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        Quantity INT NOT NULL,
        DiscountPercent INT DEFAULT 0,
        TotalPrice DECIMAL(18,2) NOT NULL,
        FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
        FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
    );
END
GO

-- =============================================
-- Table: Payments (Thanh toÃ¡n - theo dÃµi chi tiáº¿t)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payments')
BEGIN
    CREATE TABLE Payments (
        PaymentId INT PRIMARY KEY IDENTITY(1,1),
        OrderId INT NOT NULL,
        PaymentMethod NVARCHAR(50) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        TransactionCode NVARCHAR(100),
        Status NVARCHAR(50) DEFAULT N'ThÃ nh cÃ´ng', -- ThÃ nh cÃ´ng, Tháº¥t báº¡i, Äang xá»­ lÃ½
        Notes NVARCHAR(500),
        EmployeeId INT, -- NhÃ¢n viÃªn thu tiá»n (náº¿u bÃ¡n trá»±c tiáº¿p)
        PaymentDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
        FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
    );
END
GO

-- =============================================
-- Table: Vouchers (MÃ£ giáº£m giÃ¡)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Vouchers')
BEGIN
    CREATE TABLE Vouchers (
        VoucherId INT PRIMARY KEY IDENTITY(1,1),
        VoucherCode NVARCHAR(50) UNIQUE NOT NULL,
        VoucherName NVARCHAR(200),
        DiscountType NVARCHAR(20), -- Percent, Fixed
        DiscountValue DECIMAL(18,2) NOT NULL,
        MinOrderAmount DECIMAL(18,2) DEFAULT 0,
        MaxDiscountAmount DECIMAL(18,2),
        UsageLimit INT, -- Sá»‘ láº§n sá»­ dá»¥ng tá»‘i Ä‘a
        UsedCount INT DEFAULT 0,
        ValidFrom DATETIME,
        ValidTo DATETIME,
        IsActive BIT DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE()
    );
END
GO

-- =============================================
-- Table: LoyaltyPointsHistory (Lá»‹ch sá»­ Ä‘iá»ƒm tÃ­ch lÅ©y)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LoyaltyPointsHistory')
BEGIN
    CREATE TABLE LoyaltyPointsHistory (
        HistoryId INT PRIMARY KEY IDENTITY(1,1),
        CustomerId INT NOT NULL,
        OrderId INT,
        Points INT NOT NULL, -- + tÃ­ch Ä‘iá»ƒm, - tiÃªu Ä‘iá»ƒm
        TransactionType NVARCHAR(50), -- Earned, Redeemed, Adjusted
        Description NVARCHAR(255),
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
        FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
    );
END
GO

-- =============================================
-- Table: Shifts (Ca lÃ m viá»‡c)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Shifts')
BEGIN
    CREATE TABLE Shifts (
        ShiftId INT PRIMARY KEY IDENTITY(1,1),
        ShiftCode NVARCHAR(50) UNIQUE NOT NULL,
        EmployeeId INT NOT NULL,
        StartTime DATETIME NOT NULL,
        EndTime DATETIME,
        OpeningCash DECIMAL(18,2) DEFAULT 0, -- Tiá»n Ä‘áº§u ca
        ClosingCash DECIMAL(18,2), -- Tiá»n cuá»‘i ca
        TotalSales DECIMAL(18,2) DEFAULT 0, -- Tá»•ng doanh thu
        TotalOrders INT DEFAULT 0, -- Tá»•ng sá»‘ Ä‘Æ¡n
        Status NVARCHAR(50) DEFAULT N'Äang má»Ÿ', -- Äang má»Ÿ, ÄÃ£ Ä‘Ã³ng
        Notes NVARCHAR(500),
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
    );
END
GO

-- =============================================
-- Table: Cart (Giá» hÃ ng)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cart')
BEGIN
    CREATE TABLE Cart (
        CartId INT PRIMARY KEY IDENTITY(1,1),
        CustomerId INT,
        SessionId NVARCHAR(100),
        ProductId INT NOT NULL,
        Quantity INT NOT NULL DEFAULT 1,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
        FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
    );
END
GO

-- =============================================
-- Table: Reviews (ÄÃ¡nh giÃ¡ sáº£n pháº©m)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reviews')
BEGIN
    CREATE TABLE Reviews (
        ReviewId INT PRIMARY KEY IDENTITY(1,1),
        ProductId INT NOT NULL,
        CustomerId INT,
        CustomerName NVARCHAR(100) NOT NULL,
        Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
        Comment NVARCHAR(1000),
        IsApproved BIT DEFAULT 0,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
        FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
    );
END
GO

-- =============================================
-- Table: Blogs (BÃ i viáº¿t blog)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Blogs')
BEGIN
    CREATE TABLE Blogs (
        BlogId INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(255) NOT NULL,
        Slug NVARCHAR(255) UNIQUE NOT NULL,
        Content NVARCHAR(MAX) NOT NULL,
        ShortDescription NVARCHAR(500),
        ImageUrl NVARCHAR(255),
        AuthorId INT,
        AuthorName NVARCHAR(100),
        ViewCount INT DEFAULT 0,
        IsPublished BIT DEFAULT 1,
        PublishedDate DATETIME,
        CreatedDate DATETIME DEFAULT GETDATE(),
        UpdatedDate DATETIME,
        FOREIGN KEY (AuthorId) REFERENCES Users(UserId)
    );
END
GO

-- =============================================
-- Table: BlogComments (BÃ¬nh luáº­n blog)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BlogComments')
BEGIN
    CREATE TABLE BlogComments (
        CommentId INT PRIMARY KEY IDENTITY(1,1),
        BlogId INT NOT NULL,
        CustomerId INT,
        CustomerName NVARCHAR(100) NOT NULL,
        CustomerEmail NVARCHAR(100),
        Comment NVARCHAR(1000) NOT NULL,
        IsApproved BIT DEFAULT 0,
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (BlogId) REFERENCES Blogs(BlogId),
        FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
    );
END
GO

-- =============================================
-- Table: ContactMessages (Tin nháº¯n liÃªn há»‡)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ContactMessages')
BEGIN
    CREATE TABLE ContactMessages (
        MessageId INT PRIMARY KEY IDENTITY(1,1),
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        PhoneNumber NVARCHAR(20),
        Subject NVARCHAR(255),
        Message NVARCHAR(MAX) NOT NULL,
        IsRead BIT DEFAULT 0,
        IsReplied BIT DEFAULT 0,
        CreatedDate DATETIME DEFAULT GETDATE()
    );
END
GO

-- =============================================
-- Table: Expenses (Chi phÃ­)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Expenses')
BEGIN
    CREATE TABLE Expenses (
        ExpenseId INT PRIMARY KEY IDENTITY(1,1),
        ExpenseCode NVARCHAR(50) UNIQUE NOT NULL,
        ExpenseType NVARCHAR(100), -- Tiá»n Ä‘iá»‡n, Tiá»n nÆ°á»›c, LÆ°Æ¡ng, Váº­n chuyá»ƒn, KhÃ¡c
        Amount DECIMAL(18,2) NOT NULL,
        Description NVARCHAR(500),
        EmployeeId INT,
        ExpenseDate DATE NOT NULL,
        Status NVARCHAR(50) DEFAULT N'ÄÃ£ chi', -- ÄÃ£ chi, Chá» duyá»‡t
        CreatedDate DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
    );
END
GO

-- =============================================
-- Table: Settings (Cáº¥u hÃ¬nh website)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Settings')
BEGIN
    CREATE TABLE Settings (
        SettingId INT PRIMARY KEY IDENTITY(1,1),
        SettingKey NVARCHAR(100) UNIQUE NOT NULL,
        SettingValue NVARCHAR(MAX),
        Description NVARCHAR(255),
        UpdatedDate DATETIME DEFAULT GETDATE()
    );
END
GO

PRINT 'Extended database schema created successfully!';
PRINT 'Há»— trá»£: BÃ¡n hÃ ng trá»±c tiáº¿p (POS), Quáº£n lÃ½ khÃ¡ch hÃ ng, Quáº£n lÃ½ kho, NhÃ¢n viÃªn, Ca lÃ m viá»‡c';
GO
-- =============================================
-- INSERT SAMPLE DATA
-- =============================================

PRINT '========================================';
PRINT 'BƯỚC 2: THÊM DỮ LIỆU MẪU';
PRINT '========================================';
GO

-- Categories
SET IDENTITY_INSERT Categories ON;
INSERT INTO Categories (CategoryId, CategoryName, Description, ImageUrl, DisplayOrder, IsActive)
VALUES 
    (1, N'Hoa quả sấy dẻo', N'Các loại hoa quả sấy dẻo giữ nguyên vị ngọt tự nhiên', '/images/kind-1.jpg', 1, 1),
    (2, N'Hoa quả sấy giòn', N'Hoa quả sấy giòn tan, thơm ngon', '/images/kind-2.jpg', 2, 1),
    (3, N'Hoa quả sấy thăng hoa', N'Công nghệ sấy thăng hoa hiện đại', '/images/kind-3.jpg', 3, 1),
    (4, N'Combo quà tặng', N'Các combo quà tặng dịp lễ Tết', '/images/kind-4.jpg', 4, 1);
SET IDENTITY_INSERT Categories OFF;
PRINT 'Đã thêm 4 danh mục sản phẩm';
GO

-- Suppliers
SET IDENTITY_INSERT Suppliers ON;
INSERT INTO Suppliers (SupplierId, SupplierCode, SupplierName, ContactPerson, PhoneNumber, Email, Address, City, IsActive)
VALUES 
    (1, 'NCC001', N'Nông trại Mộc Châu', N'Nguyễn Văn A', '0912345678', 'mocchau@example.com', N'Xã Tân Lập, Mộc Châu', N'Sơn La', 1),
    (2, 'NCC002', N'Hợp tác xã Đà Lạt', N'Trần Thị B', '0923456789', 'dalat@example.com', N'Phường 1, Đà Lạt', N'Lâm Đồng', 1);
SET IDENTITY_INSERT Suppliers OFF;
PRINT 'Đã thêm 2 nhà cung cấp';
GO

-- Products
SET IDENTITY_INSERT Products ON;
INSERT INTO Products (ProductId, ProductCode, ProductName, CategoryId, Description, ShortDescription, Price, OriginalPrice, CostPrice, DiscountPercent, StockQuantity, MinStockLevel, Unit, Weight, ImageUrl, IsActive, IsFeatured, IsNew, Rating)
VALUES 
    (1, 'SP001', N'Mận sấy dẻo Mộc Châu', 1, N'Mận sấy dẻo từ mận tươi Mộc Châu, giữ nguyên vị chua ngọt tự nhiên', N'Mận sấy dẻo 100% tự nhiên', 85000, 100000, 60000, 15, 100, 10, N'gói', N'200g', '/images/prod-1.jpg', 1, 1, 1, 4.8),
    (2, 'SP002', N'Dâu tây sấy dẻo', 1, N'Dâu tây Đà Lạt sấy dẻo cao cấp', N'Dâu tây Đà Lạt cao cấp', 120000, 150000, 85000, 20, 80, 10, N'gói', N'150g', '/images/prod-2.jpg', 1, 1, 1, 4.9),
    (3, 'SP003', N'Kiwi sấy dẻo', 1, N'Kiwi sấy dẻo giàu vitamin C', N'Kiwi New Zealand', 95000, 110000, 70000, 14, 120, 10, N'gói', N'200g', '/images/prod-3.jpg', 1, 1, 0, 4.7),
    (4, 'SP004', N'Xoài sấy giòn', 2, N'Xoài sấy giòn tan, ngọt tự nhiên', N'Xoài cát Hòa Lộc', 75000, 85000, 50000, 12, 150, 10, N'gói', N'100g', '/images/prod-4.jpg', 1, 1, 0, 4.6),
    (5, 'SP005', N'Chuối sấy giòn', 2, N'Chuối già Laba sấy giòn', N'Chuối già Laba', 65000, 75000, 45000, 13, 200, 10, N'gói', N'100g', '/images/prod-5.jpg', 1, 0, 0, 4.5),
    (6, 'SP006', N'Khoai lang tím sấy giòn', 2, N'Khoai lang tím Mộc Châu sấy giòn', N'Khoai lang tím Mộc Châu', 70000, 80000, 48000, 13, 100, 10, N'gói', N'150g', '/images/prod-6.jpg', 1, 0, 1, 4.8),
    (7, 'SP007', N'Dứa sấy thăng hoa', 3, N'Dứa sấy thăng hoa công nghệ hiện đại', N'Dứa Queen sấy thăng hoa', 110000, 130000, 80000, 15, 60, 10, N'gói', N'50g', '/images/prod-7.jpg', 1, 1, 1, 4.9),
    (8, 'SP008', N'Dâu tây sấy thăng hoa', 3, N'Dâu tây sấy thăng hoa giòn tan', N'Dâu tây Đà Lạt thăng hoa', 135000, 160000, 95000, 16, 50, 10, N'gói', N'30g', '/images/prod-8.jpg', 1, 1, 1, 5.0),
    (9, 'SP009', N'Combo quà tặng Tết', 4, N'Combo 5 loại hoa quả sấy cao cấp', N'Combo 5 loại hoa quả sấy', 350000, 450000, 250000, 22, 30, 5, N'hộp', N'1kg', '/images/prod-9.jpg', 1, 1, 1, 4.9);
SET IDENTITY_INSERT Products OFF;
PRINT 'Đã thêm 9 sản phẩm';
GO

-- Employees
SET IDENTITY_INSERT Employees ON;
INSERT INTO Employees (EmployeeId, EmployeeCode, FullName, PhoneNumber, Email, Position, Department, HireDate, IsActive)
VALUES 
    (1, 'NV001', N'Nguyễn Văn Quản Lý', '0901234567', 'manager@mocvistore.com', N'Quản lý', N'Quản lý', '2024-01-01', 1),
    (2, 'NV002', N'Trần Thị Thu Ngân', '0902234567', 'cashier1@mocvistore.com', N'Thu ngân', N'Bán hàng', '2024-02-01', 1),
    (3, 'NV003', N'Lê Văn Kho', '0903234567', 'warehouse@mocvistore.com', N'Thủ kho', N'Kho', '2024-03-01', 1);
SET IDENTITY_INSERT Employees OFF;
PRINT 'Đã thêm 3 nhân viên';
GO

-- Users (Tài khoản cho nhân viên)
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserId, Email, PasswordHash, FullName, PhoneNumber, Role, EmployeeId, IsActive)
VALUES 
    (1, 'admin@mocvistore.com', 'HASHED_PASSWORD_ADMIN', N'Admin', '0901234567', 'Admin', 1, 1),
    (2, 'cashier@mocvistore.com', 'HASHED_PASSWORD_CASHIER', N'Thu Ngân', '0902234567', 'Cashier', 2, 1),
    (3, 'warehouse@mocvistore.com', 'HASHED_PASSWORD_WAREHOUSE', N'Thủ Kho', '0903234567', 'Staff', 3, 1);
SET IDENTITY_INSERT Users OFF;
PRINT 'Đã thêm 3 tài khoản nhân viên';
GO

-- Customers
SET IDENTITY_INSERT Customers ON;
INSERT INTO Customers (CustomerId, CustomerCode, FullName, PhoneNumber, Email, Address, City, CustomerType, LoyaltyPoints, IsActive)
VALUES 
    (1, 'KH001', N'Phạm Văn Khách', '0911111111', 'khach1@gmail.com', N'123 Lê Lợi', N'Hà Nội', N'VIP', 500, 1),
    (2, 'KH002', N'Hoàng Thị Mua', '0922222222', 'khach2@gmail.com', N'456 Trần Hưng Đạo', N'TP.HCM', N'Thường', 100, 1),
    (3, 'KH003', N'Vũ Văn Thường', '0933333333', 'khach3@gmail.com', N'789 Nguyễn Huệ', N'Đà Nẵng', N'Thường', 50, 1);
SET IDENTITY_INSERT Customers OFF;
PRINT 'Đã thêm 3 khách hàng';
GO

-- Vouchers
SET IDENTITY_INSERT Vouchers ON;
INSERT INTO Vouchers (VoucherId, VoucherCode, VoucherName, DiscountType, DiscountValue, MinOrderAmount, ValidFrom, ValidTo, UsageLimit, IsActive)
VALUES 
    (1, 'WELCOME10', N'Giảm 10% cho khách mới', 'Percent', 10, 100000, '2024-01-01', '2024-12-31', 1000, 1),
    (2, 'TET2024', N'Giảm 50K dịp Tết', 'Fixed', 50000, 200000, '2024-01-20', '2024-02-15', 500, 1),
    (3, 'VIP20', N'Giảm 20% cho VIP', 'Percent', 20, 300000, '2024-01-01', '2024-12-31', NULL, 1);
SET IDENTITY_INSERT Vouchers OFF;
PRINT 'Đã thêm 3 voucher';
GO

-- Settings
INSERT INTO Settings (SettingKey, SettingValue, Description)
VALUES 
    ('SiteName', N'Mộc Vị Store', N'Tên website'),
    ('SiteEmail', 'contact@mocvistore.com', N'Email liên hệ'),
    ('SitePhone', '1900-xxxx', N'Hotline'),
    ('SiteAddress', N'Mộc Châu, Sơn La', N'Địa chỉ cửa hàng'),
    ('LoyaltyPointsRate', '1000', N'1000 VNĐ = 1 điểm'),
    ('LoyaltyPointsValue', '1000', N'1 điểm = 1000 VNĐ');
PRINT 'Đã thêm cấu hình website';
GO

PRINT '========================================';
PRINT 'HOÀN THÀNH!';
PRINT 'Database: MocViStoreDB';
PRINT 'Tổng số bảng: 24';
PRINT 'Danh mục: 4 | Sản phẩm: 9';
PRINT 'Nhân viên: 3 | Khách hàng: 3';
PRINT 'Nhà cung cấp: 2 | Voucher: 3';
PRINT '========================================';
GO
