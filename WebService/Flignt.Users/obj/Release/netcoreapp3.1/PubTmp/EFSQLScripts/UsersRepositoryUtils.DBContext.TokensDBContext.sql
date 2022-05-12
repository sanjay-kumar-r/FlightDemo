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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220429221940_initialTokenMigration')
BEGIN
    CREATE TABLE [UserRefreshTokens] (
        [Id] bigint NOT NULL IDENTITY,
        [Token] nvarchar(max) NOT NULL,
        [RefreshToken] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [ExpirationDate] datetime2 NOT NULL,
        [IpAddress] nvarchar(max) NOT NULL,
        [IsInvalidated] bit NOT NULL,
        [UserId] bigint NOT NULL,
        CONSTRAINT [PrimaryKey_UserRefreshTokensId] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserRefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220429221940_initialTokenMigration')
BEGIN
    CREATE INDEX [IX_UserRefreshTokens_UserId] ON [UserRefreshTokens] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220429221940_initialTokenMigration')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220429221940_initialTokenMigration', N'5.0.16');
END;
GO

COMMIT;
GO

