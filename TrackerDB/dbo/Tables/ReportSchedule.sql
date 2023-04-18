CREATE TABLE [dbo].[ReportSchedule] (
    [Id]                 BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]             NVARCHAR (MAX) NULL,
    [FleetId]            BIGINT         NOT NULL,
    [ReportTypeLookupId] INT            NOT NULL,
    [WarehouseId]        BIGINT         NULL,
    [InventoryId]        BIGINT         NULL,
    [SensorSerial]       BIGINT         NULL,
    [AlertTypeLookupId]  INT            NULL,
    [Name]               NVARCHAR (MAX) NULL,
    [NewerToOlder]       BIT            NOT NULL,
    [Daily]              BIT            NULL,
    [Weekly]             BIT            NULL,
    [Monthly]            BIT            NULL,
    [Yearly]             BIT            NULL,
    [DayOfWeekId]        INT            NULL,
    [DayOfMonthId]       INT            NULL,
    [DailyRepeat]        INT            NULL,
    [WeeklyRepeat]       INT            NULL,
    [MonthlyRepeat]      INT            NULL,
    [DailyTime]          NVARCHAR (MAX) NULL,
    [WeeklyTime]         NVARCHAR (MAX) NULL,
    [MonthlyTime]        NVARCHAR (MAX) NULL,
    [Emails]             NVARCHAR (MAX) NULL,
    [IsActive]           BIT            NOT NULL,
    [IsEnglish]          BIT            NOT NULL,
    [PDF]                BIT            NULL,
    [Excel]              BIT            NULL,
    [IsDeleted]          BIT            NOT NULL,
    [CreatedDate]        DATETIME2 (7)  NULL,
    [CreatedBy]          NVARCHAR (MAX) NULL,
    [UpdatedDate]        DATETIME2 (7)  NULL,
    [UpdatedBy]          NVARCHAR (MAX) NULL,
    [GroupUpdatesByType] VARCHAR (4)    NULL,
    [GroupUpdatesValue]  INT            NULL,
    CONSTRAINT [PK_ReportSchedule] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReportSchedule_DayOfWeekLookup_DayOfWeekId] FOREIGN KEY ([DayOfWeekId]) REFERENCES [dbo].[DayOfWeekLookup] ([Id]),
    CONSTRAINT [FK_ReportSchedule_Fleet_FleetId] FOREIGN KEY ([FleetId]) REFERENCES [dbo].[Fleet] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ReportSchedule_Inventory_InventoryId] FOREIGN KEY ([InventoryId]) REFERENCES [dbo].[Inventory] ([Id]),
    CONSTRAINT [FK_ReportSchedule_ReportTypeLookup_ReportTypeLookupId] FOREIGN KEY ([ReportTypeLookupId]) REFERENCES [dbo].[ReportTypeLookup] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ReportSchedule_Warehouse_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouse] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ReportSchedule_DayOfWeekId]
    ON [dbo].[ReportSchedule]([DayOfWeekId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ReportSchedule_FleetId]
    ON [dbo].[ReportSchedule]([FleetId] ASC);




GO
CREATE NONCLUSTERED INDEX [IX_ReportSchedule_InventoryId]
    ON [dbo].[ReportSchedule]([InventoryId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ReportSchedule_ReportTypeLookupId]
    ON [dbo].[ReportSchedule]([ReportTypeLookupId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ReportSchedule_WarehouseId]
    ON [dbo].[ReportSchedule]([WarehouseId] ASC);

