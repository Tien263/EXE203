-- Tạo bảng OtpVerifications nếu chưa tồn tại
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OtpVerifications]') AND type in (N'U'))
BEGIN
    CREATE TABLE [OtpVerifications] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(100) NOT NULL,
        [OtpCode] nvarchar(6) NOT NULL,
        [CreatedAt] datetime NOT NULL DEFAULT (getdate()),
        [ExpiresAt] datetime NOT NULL,
        [IsUsed] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_OtpVerifications] PRIMARY KEY ([Id])
    );
    
    PRINT 'Bảng OtpVerifications đã được tạo thành công!';
END
ELSE
BEGIN
    PRINT 'Bảng OtpVerifications đã tồn tại.';
END
GO
