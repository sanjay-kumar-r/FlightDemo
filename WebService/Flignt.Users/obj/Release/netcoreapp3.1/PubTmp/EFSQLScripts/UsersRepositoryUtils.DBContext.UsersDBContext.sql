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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220429220950_initialUsersMigration')
BEGIN
    CREATE TABLE [AccountStatus] (
        [Id] int NOT NULL IDENTITY,
        [Status] nvarchar(512) NOT NULL,
        [Description] nvarchar(1024) NULL,
        CONSTRAINT [PrimaryKey_AccStatusId] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220429220950_initialUsersMigration')
BEGIN
    CREATE TABLE [Users] (
        [Id] bigint NOT NULL IDENTITY,
        [FirstName] nvarchar(512) NOT NULL,
        [LastName] nvarchar(512) NOT NULL,
        [EmailId] nvarchar(1024) NOT NULL,
        [Password] nvarchar(1024) NOT NULL,
        [AccountStatusId] int NOT NULL,
        [IsSuperAdmin] bit NOT NULL,
        [CreatedOn] datetime2 NULL,
        [ModifiedOn] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PrimaryKey_UserId] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Users_AccountStatus_AccountStatusId] FOREIGN KEY ([AccountStatusId]) REFERENCES [AccountStatus] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220429220950_initialUsersMigration')
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Status') AND [object_id] = OBJECT_ID(N'[AccountStatus]'))
        SET IDENTITY_INSERT [AccountStatus] ON;
    EXEC(N'INSERT INTO [AccountStatus] ([Id], [Description], [Status])
    VALUES (1, N''On user first time register'', N''Registered''),
    (2, N''On user first time login and there after'', N''Active''),
    (3, N''On user not loggedIn long time or updated by admin'', N''InActive''),
    (4, N''On user invalid/wrong attempt to login or update by admin'', N''Blocked'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Status') AND [object_id] = OBJECT_ID(N'[AccountStatus]'))
        SET IDENTITY_INSERT [AccountStatus] OFF;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220429220950_initialUsersMigration')
BEGIN
    CREATE INDEX [IX_Users_AccountStatusId] ON [Users] ([AccountStatusId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220429220950_initialUsersMigration')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220429220950_initialUsersMigration', N'5.0.16');
END;
GO

COMMIT;
GO

