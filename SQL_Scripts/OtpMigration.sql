IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Categories] (
    [CategoryId] int NOT NULL IDENTITY,
    [CategoryName] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [ImageUrl] nvarchar(255) NULL,
    [DisplayOrder] int NULL DEFAULT 0,
    [IsActive] bit NULL DEFAULT CAST(1 AS bit),
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    [UpdatedDate] datetime NULL,
    CONSTRAINT [PK__Categori__19093A0BEB7A4BFE] PRIMARY KEY ([CategoryId])
);
GO

CREATE TABLE [ContactMessages] (
    [MessageId] int NOT NULL IDENTITY,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [Subject] nvarchar(255) NULL,
    [Message] nvarchar(max) NOT NULL,
    [IsRead] bit NULL DEFAULT CAST(0 AS bit),
    [IsReplied] bit NULL DEFAULT CAST(0 AS bit),
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__ContactM__C87C0C9C21A4A6C8] PRIMARY KEY ([MessageId])
);
GO

CREATE TABLE [Customers] (
    [CustomerId] int NOT NULL IDENTITY,
    [CustomerCode] nvarchar(50) NULL,
    [FullName] nvarchar(100) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [Email] nvarchar(100) NULL,
    [DateOfBirth] date NULL,
    [Gender] nvarchar(10) NULL,
    [Address] nvarchar(255) NULL,
    [City] nvarchar(100) NULL,
    [District] nvarchar(100) NULL,
    [Ward] nvarchar(100) NULL,
    [CustomerType] nvarchar(50) NULL DEFAULT N'ThÆ°á»ng',
    [TotalPurchased] decimal(18,2) NULL DEFAULT 0.0,
    [TotalOrders] int NULL DEFAULT 0,
    [LoyaltyPoints] int NULL DEFAULT 0,
    [Notes] nvarchar(500) NULL,
    [IsActive] bit NULL DEFAULT CAST(1 AS bit),
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    [UpdatedDate] datetime NULL,
    [LastPurchaseDate] datetime NULL,
    CONSTRAINT [PK__Customer__A4AE64D8363B03CF] PRIMARY KEY ([CustomerId])
);
GO

CREATE TABLE [Employees] (
    [EmployeeId] int NOT NULL IDENTITY,
    [EmployeeCode] nvarchar(50) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [Email] nvarchar(100) NULL,
    [DateOfBirth] date NULL,
    [Gender] nvarchar(10) NULL,
    [Address] nvarchar(255) NULL,
    [Position] nvarchar(100) NULL,
    [Department] nvarchar(100) NULL,
    [Salary] decimal(18,2) NULL,
    [HireDate] date NULL,
    [IdentityCard] nvarchar(20) NULL,
    [BankAccount] nvarchar(50) NULL,
    [BankName] nvarchar(100) NULL,
    [IsActive] bit NULL DEFAULT CAST(1 AS bit),
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    [UpdatedDate] datetime NULL,
    CONSTRAINT [PK__Employee__7AD04F11FE470046] PRIMARY KEY ([EmployeeId])
);
GO

CREATE TABLE [OtpVerifications] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(100) NOT NULL,
    [OtpCode] nvarchar(6) NOT NULL,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [ExpiresAt] datetime NOT NULL,
    [IsUsed] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_OtpVerifications] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Settings] (
    [SettingId] int NOT NULL IDENTITY,
    [SettingKey] nvarchar(100) NOT NULL,
    [SettingValue] nvarchar(max) NULL,
    [Description] nvarchar(255) NULL,
    [UpdatedDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Settings__54372B1DFC4958A2] PRIMARY KEY ([SettingId])
);
GO

CREATE TABLE [Suppliers] (
    [SupplierId] int NOT NULL IDENTITY,
    [SupplierCode] nvarchar(50) NOT NULL,
    [SupplierName] nvarchar(200) NOT NULL,
    [ContactPerson] nvarchar(100) NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [Email] nvarchar(100) NULL,
    [Address] nvarchar(255) NULL,
    [City] nvarchar(100) NULL,
    [District] nvarchar(100) NULL,
    [TaxCode] nvarchar(50) NULL,
    [BankAccount] nvarchar(50) NULL,
    [BankName] nvarchar(100) NULL,
    [Notes] nvarchar(500) NULL,
    [IsActive] bit NULL DEFAULT CAST(1 AS bit),
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    [UpdatedDate] datetime NULL,
    CONSTRAINT [PK__Supplier__4BE666B4D1C018D4] PRIMARY KEY ([SupplierId])
);
GO

CREATE TABLE [Vouchers] (
    [VoucherId] int NOT NULL IDENTITY,
    [VoucherCode] nvarchar(50) NOT NULL,
    [VoucherName] nvarchar(200) NULL,
    [DiscountType] nvarchar(20) NULL,
    [DiscountValue] decimal(18,2) NOT NULL,
    [MinOrderAmount] decimal(18,2) NULL DEFAULT 0.0,
    [MaxDiscountAmount] decimal(18,2) NULL,
    [UsageLimit] int NULL,
    [UsedCount] int NULL DEFAULT 0,
    [ValidFrom] datetime NULL,
    [ValidTo] datetime NULL,
    [IsActive] bit NULL DEFAULT CAST(1 AS bit),
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Vouchers__3AEE79213F792BF5] PRIMARY KEY ([VoucherId])
);
GO

CREATE TABLE [Products] (
    [ProductId] int NOT NULL IDENTITY,
    [ProductCode] nvarchar(50) NOT NULL,
    [ProductName] nvarchar(200) NOT NULL,
    [CategoryId] int NOT NULL,
    [Description] nvarchar(max) NULL,
    [ShortDescription] nvarchar(500) NULL,
    [Price] decimal(18,2) NOT NULL,
    [OriginalPrice] decimal(18,2) NULL,
    [CostPrice] decimal(18,2) NULL,
    [DiscountPercent] int NULL DEFAULT 0,
    [StockQuantity] int NULL DEFAULT 0,
    [MinStockLevel] int NULL DEFAULT 10,
    [Unit] nvarchar(50) NULL DEFAULT N'gÃ³i',
    [Weight] nvarchar(50) NULL,
    [ImageUrl] nvarchar(255) NULL,
    [ImageGallery] nvarchar(max) NULL,
    [IsActive] bit NULL DEFAULT CAST(1 AS bit),
    [IsFeatured] bit NULL DEFAULT CAST(0 AS bit),
    [IsNew] bit NULL DEFAULT CAST(0 AS bit),
    [ViewCount] int NULL DEFAULT 0,
    [SoldCount] int NULL DEFAULT 0,
    [Rating] decimal(3,2) NULL DEFAULT 0.0,
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    [UpdatedDate] datetime NULL,
    CONSTRAINT [PK__Products__B40CC6CDEA842406] PRIMARY KEY ([ProductId]),
    CONSTRAINT [FK__Products__Catego__47DBAE45] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId])
);
GO

CREATE TABLE [Expenses] (
    [ExpenseId] int NOT NULL IDENTITY,
    [ExpenseCode] nvarchar(50) NOT NULL,
    [ExpenseType] nvarchar(100) NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Description] nvarchar(500) NULL,
    [EmployeeId] int NULL,
    [ExpenseDate] date NOT NULL,
    [Status] nvarchar(50) NULL DEFAULT N'ÄÃ£ chi',
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Expenses__1445CFD3CEB81C45] PRIMARY KEY ([ExpenseId]),
    CONSTRAINT [FK__Expenses__Employ__40058253] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId])
);
GO

CREATE TABLE [Orders] (
    [OrderId] int NOT NULL IDENTITY,
    [OrderCode] nvarchar(50) NOT NULL,
    [CustomerId] int NULL,
    [EmployeeId] int NULL,
    [OrderType] nvarchar(50) NULL DEFAULT N'Online',
    [CustomerName] nvarchar(100) NOT NULL,
    [CustomerEmail] nvarchar(100) NULL,
    [CustomerPhone] nvarchar(20) NOT NULL,
    [ShippingAddress] nvarchar(255) NULL,
    [City] nvarchar(100) NULL,
    [District] nvarchar(100) NULL,
    [Ward] nvarchar(100) NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [ShippingFee] decimal(18,2) NULL DEFAULT 0.0,
    [DiscountAmount] decimal(18,2) NULL DEFAULT 0.0,
    [VoucherCode] nvarchar(50) NULL,
    [LoyaltyPointsUsed] int NULL DEFAULT 0,
    [LoyaltyPointsEarned] int NULL DEFAULT 0,
    [FinalAmount] decimal(18,2) NOT NULL,
    [PaymentMethod] nvarchar(50) NULL,
    [PaymentStatus] nvarchar(50) NULL DEFAULT N'ChÆ°a thanh toÃ¡n',
    [OrderStatus] nvarchar(50) NULL DEFAULT N'Chá» xÃ¡c nháº­n',
    [Note] nvarchar(500) NULL,
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    [UpdatedDate] datetime NULL,
    [CompletedDate] datetime NULL,
    CONSTRAINT [PK__Orders__C3905BCFF01C29D3] PRIMARY KEY ([OrderId]),
    CONSTRAINT [FK__Orders__Customer__7D439ABD] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]),
    CONSTRAINT [FK__Orders__Employee__7E37BEF6] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId])
);
GO

CREATE TABLE [Shifts] (
    [ShiftId] int NOT NULL IDENTITY,
    [ShiftCode] nvarchar(50) NOT NULL,
    [EmployeeId] int NOT NULL,
    [StartTime] datetime NOT NULL,
    [EndTime] datetime NULL,
    [OpeningCash] decimal(18,2) NULL DEFAULT 0.0,
    [ClosingCash] decimal(18,2) NULL,
    [TotalSales] decimal(18,2) NULL DEFAULT 0.0,
    [TotalOrders] int NULL DEFAULT 0,
    [Status] nvarchar(50) NULL DEFAULT N'Äang má»Ÿ',
    [Notes] nvarchar(500) NULL,
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Shifts__C0A83881BE5A36B7] PRIMARY KEY ([ShiftId]),
    CONSTRAINT [FK__Shifts__Employee__1CBC4616] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId])
);
GO

CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [Email] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [ProfileImageUrl] nvarchar(max) NULL,
    [Role] nvarchar(20) NULL DEFAULT N'Customer',
    [EmployeeId] int NULL,
    [CustomerId] int NULL,
    [IsActive] bit NULL DEFAULT CAST(1 AS bit),
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    [LastLoginDate] datetime NULL,
    CONSTRAINT [PK__Users__1788CC4C2A3ED859] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK__Users__CustomerI__71D1E811] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]),
    CONSTRAINT [FK__Users__EmployeeI__70DDC3D8] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId])
);
GO

CREATE TABLE [PurchaseOrders] (
    [PurchaseOrderId] int NOT NULL IDENTITY,
    [PurchaseOrderCode] nvarchar(50) NOT NULL,
    [SupplierId] int NOT NULL,
    [EmployeeId] int NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [PaidAmount] decimal(18,2) NULL DEFAULT 0.0,
    [RemainingAmount] decimal(18,2) NULL DEFAULT 0.0,
    [Status] nvarchar(50) NULL DEFAULT N'Chá» duyá»‡t',
    [Notes] nvarchar(500) NULL,
    [OrderDate] datetime NULL DEFAULT ((getdate())),
    [ReceivedDate] datetime NULL,
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    [UpdatedDate] datetime NULL,
    CONSTRAINT [PK__Purchase__036BACA4BA65F84C] PRIMARY KEY ([PurchaseOrderId]),
    CONSTRAINT [FK__PurchaseO__Suppl__5535A963] FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers] ([SupplierId])
);
GO

CREATE TABLE [Cart] (
    [CartId] int NOT NULL IDENTITY,
    [CustomerId] int NULL,
    [SessionId] nvarchar(100) NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL DEFAULT 1,
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Cart__51BCD7B77262D876] PRIMARY KEY ([CartId]),
    CONSTRAINT [FK__Cart__CustomerId__2180FB33] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]),
    CONSTRAINT [FK__Cart__ProductId__22751F6C] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId])
);
GO

CREATE TABLE [InventoryTransactions] (
    [TransactionId] int NOT NULL IDENTITY,
    [ProductId] int NOT NULL,
    [TransactionType] nvarchar(50) NOT NULL,
    [Quantity] int NOT NULL,
    [ReferenceType] nvarchar(50) NULL,
    [ReferenceId] int NULL,
    [Notes] nvarchar(500) NULL,
    [EmployeeId] int NULL,
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Inventor__55433A6BA758D326] PRIMARY KEY ([TransactionId]),
    CONSTRAINT [FK__Inventory__Produ__5CD6CB2B] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId])
);
GO

CREATE TABLE [Reviews] (
    [ReviewId] int NOT NULL IDENTITY,
    [ProductId] int NOT NULL,
    [CustomerId] int NULL,
    [CustomerName] nvarchar(100) NOT NULL,
    [Rating] int NOT NULL,
    [Comment] nvarchar(1000) NULL,
    [IsApproved] bit NULL DEFAULT CAST(0 AS bit),
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Reviews__74BC79CE911E2C1F] PRIMARY KEY ([ReviewId]),
    CONSTRAINT [FK__Reviews__Custome__29221CFB] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]),
    CONSTRAINT [FK__Reviews__Product__282DF8C2] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId])
);
GO

CREATE TABLE [LoyaltyPointsHistory] (
    [HistoryId] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [OrderId] int NULL,
    [Points] int NOT NULL,
    [TransactionType] nvarchar(50) NULL,
    [Description] nvarchar(255) NULL,
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__LoyaltyP__4D7B4ABDD68DD771] PRIMARY KEY ([HistoryId]),
    CONSTRAINT [FK__LoyaltyPo__Custo__1332DBDC] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]),
    CONSTRAINT [FK__LoyaltyPo__Order__14270015] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId])
);
GO

CREATE TABLE [OrderDetails] (
    [OrderDetailId] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [ProductName] nvarchar(200) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Quantity] int NOT NULL,
    [DiscountPercent] int NULL DEFAULT 0,
    [TotalPrice] decimal(18,2) NOT NULL,
    CONSTRAINT [PK__OrderDet__D3B9D36C71A16D84] PRIMARY KEY ([OrderDetailId]),
    CONSTRAINT [FK__OrderDeta__Order__02084FDA] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]),
    CONSTRAINT [FK__OrderDeta__Produ__02FC7413] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId])
);
GO

CREATE TABLE [Payments] (
    [PaymentId] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [PaymentMethod] nvarchar(50) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [TransactionCode] nvarchar(100) NULL,
    [Status] nvarchar(50) NULL DEFAULT N'ThÃ nh cÃ´ng',
    [Notes] nvarchar(500) NULL,
    [EmployeeId] int NULL,
    [PaymentDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Payments__9B556A38BCA55591] PRIMARY KEY ([PaymentId]),
    CONSTRAINT [FK__Payments__Employ__08B54D69] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]),
    CONSTRAINT [FK__Payments__OrderI__07C12930] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId])
);
GO

CREATE TABLE [Blogs] (
    [BlogId] int NOT NULL IDENTITY,
    [Title] nvarchar(255) NOT NULL,
    [Slug] nvarchar(255) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [ShortDescription] nvarchar(500) NULL,
    [ImageUrl] nvarchar(255) NULL,
    [AuthorId] int NULL,
    [AuthorName] nvarchar(100) NULL,
    [ViewCount] int NULL DEFAULT 0,
    [IsPublished] bit NULL DEFAULT CAST(1 AS bit),
    [PublishedDate] datetime NULL,
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    [UpdatedDate] datetime NULL,
    CONSTRAINT [PK__Blogs__54379E30D7FB4623] PRIMARY KEY ([BlogId]),
    CONSTRAINT [FK__Blogs__AuthorId__2FCF1A8A] FOREIGN KEY ([AuthorId]) REFERENCES [Users] ([UserId])
);
GO

CREATE TABLE [PurchaseOrderDetails] (
    [PurchaseOrderDetailId] int NOT NULL IDENTITY,
    [PurchaseOrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [TotalPrice] decimal(18,2) NOT NULL,
    CONSTRAINT [PK__Purchase__5026B6986489F6F8] PRIMARY KEY ([PurchaseOrderDetailId]),
    CONSTRAINT [FK__PurchaseO__Produ__59063A47] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId]),
    CONSTRAINT [FK__PurchaseO__Purch__5812160E] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([PurchaseOrderId])
);
GO

CREATE TABLE [BlogComments] (
    [CommentId] int NOT NULL IDENTITY,
    [BlogId] int NOT NULL,
    [CustomerId] int NULL,
    [CustomerName] nvarchar(100) NOT NULL,
    [CustomerEmail] nvarchar(100) NULL,
    [Comment] nvarchar(1000) NOT NULL,
    [IsApproved] bit NULL DEFAULT CAST(0 AS bit),
    [CreatedDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__BlogComm__C3B4DFCAB7874396] PRIMARY KEY ([CommentId]),
    CONSTRAINT [FK__BlogComme__BlogI__3493CFA7] FOREIGN KEY ([BlogId]) REFERENCES [Blogs] ([BlogId]),
    CONSTRAINT [FK__BlogComme__Custo__3587F3E0] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId])
);
GO

CREATE INDEX [IX_BlogComments_BlogId] ON [BlogComments] ([BlogId]);
GO

CREATE INDEX [IX_BlogComments_CustomerId] ON [BlogComments] ([CustomerId]);
GO

CREATE INDEX [IX_Blogs_AuthorId] ON [Blogs] ([AuthorId]);
GO

CREATE UNIQUE INDEX [UQ__Blogs__BC7B5FB6F961F801] ON [Blogs] ([Slug]);
GO

CREATE INDEX [IX_Cart_CustomerId] ON [Cart] ([CustomerId]);
GO

CREATE INDEX [IX_Cart_ProductId] ON [Cart] ([ProductId]);
GO

CREATE UNIQUE INDEX [UQ__Customer__06678521EFABD034] ON [Customers] ([CustomerCode]) WHERE [CustomerCode] IS NOT NULL;
GO

CREATE UNIQUE INDEX [UQ__Employee__1F64254817C2AECF] ON [Employees] ([EmployeeCode]);
GO

CREATE INDEX [IX_Expenses_EmployeeId] ON [Expenses] ([EmployeeId]);
GO

CREATE UNIQUE INDEX [UQ__Expenses__5064CAE114428009] ON [Expenses] ([ExpenseCode]);
GO

CREATE INDEX [IX_InventoryTransactions_ProductId] ON [InventoryTransactions] ([ProductId]);
GO

CREATE INDEX [IX_LoyaltyPointsHistory_CustomerId] ON [LoyaltyPointsHistory] ([CustomerId]);
GO

CREATE INDEX [IX_LoyaltyPointsHistory_OrderId] ON [LoyaltyPointsHistory] ([OrderId]);
GO

CREATE INDEX [IX_OrderDetails_OrderId] ON [OrderDetails] ([OrderId]);
GO

CREATE INDEX [IX_OrderDetails_ProductId] ON [OrderDetails] ([ProductId]);
GO

CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);
GO

CREATE INDEX [IX_Orders_EmployeeId] ON [Orders] ([EmployeeId]);
GO

CREATE UNIQUE INDEX [UQ__Orders__999B52299F25A08F] ON [Orders] ([OrderCode]);
GO

CREATE INDEX [IX_Payments_EmployeeId] ON [Payments] ([EmployeeId]);
GO

CREATE INDEX [IX_Payments_OrderId] ON [Payments] ([OrderId]);
GO

CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
GO

CREATE UNIQUE INDEX [UQ__Products__2F4E024F2D65F9C7] ON [Products] ([ProductCode]);
GO

CREATE INDEX [IX_PurchaseOrderDetails_ProductId] ON [PurchaseOrderDetails] ([ProductId]);
GO

CREATE INDEX [IX_PurchaseOrderDetails_PurchaseOrderId] ON [PurchaseOrderDetails] ([PurchaseOrderId]);
GO

CREATE INDEX [IX_PurchaseOrders_SupplierId] ON [PurchaseOrders] ([SupplierId]);
GO

CREATE UNIQUE INDEX [UQ__Purchase__E282C5B440FB451B] ON [PurchaseOrders] ([PurchaseOrderCode]);
GO

CREATE INDEX [IX_Reviews_CustomerId] ON [Reviews] ([CustomerId]);
GO

CREATE INDEX [IX_Reviews_ProductId] ON [Reviews] ([ProductId]);
GO

CREATE UNIQUE INDEX [UQ__Settings__01E719AD5D4C6367] ON [Settings] ([SettingKey]);
GO

CREATE INDEX [IX_Shifts_EmployeeId] ON [Shifts] ([EmployeeId]);
GO

CREATE UNIQUE INDEX [UQ__Shifts__9377D562DFAD97A7] ON [Shifts] ([ShiftCode]);
GO

CREATE UNIQUE INDEX [UQ__Supplier__44BE981B6C09723B] ON [Suppliers] ([SupplierCode]);
GO

CREATE INDEX [IX_Users_CustomerId] ON [Users] ([CustomerId]);
GO

CREATE INDEX [IX_Users_EmployeeId] ON [Users] ([EmployeeId]);
GO

CREATE UNIQUE INDEX [UQ__Users__A9D10534D9539C96] ON [Users] ([Email]);
GO

CREATE UNIQUE INDEX [UQ__Vouchers__7F0ABCA91B0360D6] ON [Vouchers] ([VoucherCode]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251021145127_AddOtpVerification', N'8.0.0');
GO

COMMIT;
GO

