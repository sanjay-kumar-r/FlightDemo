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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424164743_bookingInitialMigration')
BEGIN
    CREATE TABLE [BookingStatus] (
        [Id] int NOT NULL IDENTITY,
        [Status] nvarchar(512) NOT NULL,
        [Description] nvarchar(1024) NULL,
        CONSTRAINT [PrimaryKey_BookingStatusId] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424164743_bookingInitialMigration')
BEGIN
    CREATE TABLE [Bookings] (
        [Id] bigint NOT NULL IDENTITY,
        [UserId] bigint NOT NULL,
        [ScheduleId] bigint NOT NULL,
        [DateBookedFor] datetime2 NOT NULL,
        [BCSeats] int NOT NULL,
        [NBCSeats] int NOT NULL,
        [ActualPaidAmount] float NOT NULL,
        [BookingStatusId] int NOT NULL,
        [PNR] nvarchar(max) NULL,
        [CreatedOn] datetime2 NULL,
        [CanceledOn] datetime2 NULL,
        [IsRefunded] bit NULL,
        CONSTRAINT [PrimaryKey_BookingId] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Bookings_BookingStatus_BookingStatusId] FOREIGN KEY ([BookingStatusId]) REFERENCES [BookingStatus] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424164743_bookingInitialMigration')
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Status') AND [object_id] = OBJECT_ID(N'[BookingStatus]'))
        SET IDENTITY_INSERT [BookingStatus] ON;
    EXEC(N'INSERT INTO [BookingStatus] ([Id], [Description], [Status])
    VALUES (1, N''When airline schedule successfully booked'', N''Booked''),
    (2, N''When seats are already filled'', N''Waiting''),
    (3, N''When user cancels Booking'', N''Canceled''),
    (4, N''When user gets refunded back'', N''Refunded''),
    (5, N''When user initiates an invalid booking'', N''Invalid'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Status') AND [object_id] = OBJECT_ID(N'[BookingStatus]'))
        SET IDENTITY_INSERT [BookingStatus] OFF;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424164743_bookingInitialMigration')
BEGIN
    CREATE INDEX [IX_Bookings_BookingStatusId] ON [Bookings] ([BookingStatusId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424164743_bookingInitialMigration')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220424164743_bookingInitialMigration', N'5.0.16');
END;
GO

COMMIT;
GO

