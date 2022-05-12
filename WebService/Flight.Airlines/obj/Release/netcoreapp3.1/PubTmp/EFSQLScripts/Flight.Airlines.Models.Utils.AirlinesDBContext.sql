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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424171245_airlinesMigration')
BEGIN
    CREATE TABLE [Airlines] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(512) NOT NULL,
        [AirlineCode] nvarchar(100) NOT NULL,
        [ContactNumber] nvarchar(50) NOT NULL,
        [ContactAddress] nvarchar(1024) NULL,
        [TotalSeats] int NOT NULL,
        [TotalBCSeats] int NOT NULL,
        [TotalNBCSeats] int NOT NULL,
        [BCTicketCost] float NOT NULL,
        [NBCTicketCost] float NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [Createdby] bigint NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        CONSTRAINT [PrimaryKey_AirlineId] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424171245_airlinesMigration')
BEGIN
    CREATE TABLE [DiscountTags] (
        [Id] bigint NOT NULL IDENTITY,
        [Name] nvarchar(512) NOT NULL,
        [DiscountCode] nvarchar(100) NOT NULL,
        [Description] nvarchar(1024) NULL,
        [Discount] real NOT NULL,
        [IsByRate] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedOn] datetime2 NOT NULL,
        [ModifiedOn] datetime2 NOT NULL,
        [Createdby] bigint NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        CONSTRAINT [PrimaryKey_DiscountTagId] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424171245_airlinesMigration')
BEGIN
    CREATE TABLE [AirlineSchedules] (
        [Id] bigint NOT NULL IDENTITY,
        [AirlineId] bigint NOT NULL,
        [From] nvarchar(512) NOT NULL,
        [To] nvarchar(512) NOT NULL,
        [IsRegular] bit NOT NULL,
        [DepartureDay] int NULL,
        [DepartureDate] datetime2 NULL,
        [DepartureTime] datetime2 NOT NULL,
        [ArrivalDay] int NULL,
        [ArrivalDate] datetime2 NULL,
        [ArrivalTime] datetime2 NOT NULL,
        [CreatedOn] datetime2 NULL,
        [ModifiedOn] datetime2 NULL,
        [Createdby] bigint NOT NULL,
        [ModifiedBy] bigint NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PrimaryKey_ScheduleId] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AirlineSchedules_Airlines_AirlineId] FOREIGN KEY ([AirlineId]) REFERENCES [Airlines] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424171245_airlinesMigration')
BEGIN
    CREATE TABLE [AirlineDiscountTagMappings] (
        [Id] bigint NOT NULL IDENTITY,
        [AirlineId] bigint NOT NULL,
        [DiscountTagId] bigint NOT NULL,
        [TaggedBy] bigint NOT NULL,
        CONSTRAINT [PrimaryKey_AirDiscTagMapId] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AirlineDiscountTagMappings_Airlines_AirlineId] FOREIGN KEY ([AirlineId]) REFERENCES [Airlines] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AirlineDiscountTagMappings_DiscountTags_DiscountTagId] FOREIGN KEY ([DiscountTagId]) REFERENCES [DiscountTags] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424171245_airlinesMigration')
BEGIN
    CREATE TABLE [AirlineScheduleTracker] (
        [Id] bigint NOT NULL IDENTITY,
        [ScheduleId] bigint NOT NULL,
        [ActualDepartureDate] datetime2 NOT NULL,
        [ActualArrivalDate] datetime2 NULL,
        [BCSeatsRemaining] int NOT NULL,
        [NBCSeatsRemaining] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PrimaryKey_ScheduleTrackerId] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AirlineScheduleTracker_AirlineSchedules_ScheduleId] FOREIGN KEY ([ScheduleId]) REFERENCES [AirlineSchedules] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424171245_airlinesMigration')
BEGIN
    CREATE INDEX [IX_AirlineDiscountTagMappings_AirlineId] ON [AirlineDiscountTagMappings] ([AirlineId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424171245_airlinesMigration')
BEGIN
    CREATE INDEX [IX_AirlineDiscountTagMappings_DiscountTagId] ON [AirlineDiscountTagMappings] ([DiscountTagId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424171245_airlinesMigration')
BEGIN
    CREATE INDEX [IX_AirlineSchedules_AirlineId] ON [AirlineSchedules] ([AirlineId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424171245_airlinesMigration')
BEGIN
    CREATE INDEX [IX_AirlineScheduleTracker_ScheduleId] ON [AirlineScheduleTracker] ([ScheduleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220424171245_airlinesMigration')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220424171245_airlinesMigration', N'5.0.16');
END;
GO

COMMIT;
GO

