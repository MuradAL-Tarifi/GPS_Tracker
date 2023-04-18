CREATE TABLE [dbo].[Alert] (
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [AlertTypeLookupId] INT             NOT NULL,
    [FleetId]           BIGINT          NOT NULL,
    [WarehouseId]       BIGINT          NULL,
    [InventoryId] BIGINT          NULL,
    [SensorId] BIGINT          NULL,
    [AlertTextAr]       NVARCHAR (MAX)  NULL,
    [AlertTextEn]       NVARCHAR (MAX)  NULL,
    [AlertForValueAr]   NVARCHAR (MAX)  NULL,
    [AlertForValueEn]   NVARCHAR (MAX)  NULL,
    [AlertDateTime]     DATETIME2 (7)   NOT NULL,
    [IsDismissed]       BIT             NOT NULL,
    [CustomAlertId]            BIGINT             NOT NULL,
    [Temperature]  DECIMAL (8, 2) NULL,
    [Humidity]     DECIMAL (8, 2) NULL,
    [IsDeleted]         BIT             NOT NULL,
    [CreatedDate]       DATETIME2 (7)   NOT NULL,
    CONSTRAINT [PK_Alert] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Alert_CustomAlert] FOREIGN KEY ([CustomAlertId]) REFERENCES [dbo].[Gateway] ([Id])
);

