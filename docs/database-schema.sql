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
CREATE TABLE [Departments] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
);

CREATE TABLE [Projects] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY ([Id])
);

CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [CurrentPosition] int NOT NULL,
    [Salary] decimal(18,2) NOT NULL,
    [DepartmentId] int NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Employees_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [EmployeeProjects] (
    [EmployeeId] int NOT NULL,
    [ProjectId] int NOT NULL,
    CONSTRAINT [PK_EmployeeProjects] PRIMARY KEY ([EmployeeId], [ProjectId]),
    CONSTRAINT [FK_EmployeeProjects_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeeProjects_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [PositionHistories] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [Position] nvarchar(50) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    CONSTRAINT [PK_PositionHistories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PositionHistories_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Departments]'))
    SET IDENTITY_INSERT [Departments] ON;
INSERT INTO [Departments] ([Id], [Name])
VALUES (1, N'Engineering'),
(2, N'Human Resources'),
(3, N'Operations');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Departments]'))
    SET IDENTITY_INSERT [Departments] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Projects]'))
    SET IDENTITY_INSERT [Projects] ON;
INSERT INTO [Projects] ([Id], [Name])
VALUES (1, N'Emergency Dispatch Platform'),
(2, N'Fleet Tracking System'),
(3, N'Internal HR Portal');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Projects]'))
    SET IDENTITY_INSERT [Projects] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CurrentPosition', N'DepartmentId', N'Name', N'Salary') AND [object_id] = OBJECT_ID(N'[Employees]'))
    SET IDENTITY_INSERT [Employees] ON;
INSERT INTO [Employees] ([Id], [CurrentPosition], [DepartmentId], [Name], [Salary])
VALUES (1, 10, 1, N'Laura Gómez', 9000.0),
(2, 1, 1, N'Carlos Pérez', 5000.0),
(3, 2, 2, N'Ana Rodríguez', 4000.0),
(4, 12, 3, N'Jorge Ramírez', 12000.0),
(5, 1, 1, N'María Torres', 4800.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CurrentPosition', N'DepartmentId', N'Name', N'Salary') AND [object_id] = OBJECT_ID(N'[Employees]'))
    SET IDENTITY_INSERT [Employees] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'EmployeeId', N'ProjectId') AND [object_id] = OBJECT_ID(N'[EmployeeProjects]'))
    SET IDENTITY_INSERT [EmployeeProjects] ON;
INSERT INTO [EmployeeProjects] ([EmployeeId], [ProjectId])
VALUES (1, 1),
(2, 1),
(2, 2),
(4, 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'EmployeeId', N'ProjectId') AND [object_id] = OBJECT_ID(N'[EmployeeProjects]'))
    SET IDENTITY_INSERT [EmployeeProjects] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'EmployeeId', N'EndDate', N'Position', N'StartDate') AND [object_id] = OBJECT_ID(N'[PositionHistories]'))
    SET IDENTITY_INSERT [PositionHistories] ON;
INSERT INTO [PositionHistories] ([Id], [EmployeeId], [EndDate], [Position], [StartDate])
VALUES (1, 1, '2022-12-31T00:00:00.0000000', N'Developer', '2020-01-01T00:00:00.0000000'),
(2, 1, NULL, N'Manager', '2023-01-01T00:00:00.0000000'),
(3, 2, NULL, N'Developer', '2021-03-15T00:00:00.0000000'),
(4, 3, NULL, N'Analyst', '2022-06-01T00:00:00.0000000'),
(5, 4, '2024-05-31T00:00:00.0000000', N'Manager', '2019-01-01T00:00:00.0000000'),
(6, 4, NULL, N'Director', '2024-06-01T00:00:00.0000000'),
(7, 5, NULL, N'Developer', '2023-02-01T00:00:00.0000000');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'EmployeeId', N'EndDate', N'Position', N'StartDate') AND [object_id] = OBJECT_ID(N'[PositionHistories]'))
    SET IDENTITY_INSERT [PositionHistories] OFF;

CREATE INDEX [IX_EmployeeProjects_ProjectId] ON [EmployeeProjects] ([ProjectId]);

CREATE INDEX [IX_Employees_DepartmentId] ON [Employees] ([DepartmentId]);

CREATE INDEX [IX_PositionHistories_EmployeeId_StartDate] ON [PositionHistories] ([EmployeeId], [StartDate]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260704043945_InitialCreate', N'9.0.17');

COMMIT;
GO

