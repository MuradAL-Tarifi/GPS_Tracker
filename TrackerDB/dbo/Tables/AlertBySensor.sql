CREATE TABLE [dbo].[AlertBySensor] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [InventoryId]         BIGINT         NULL,
    [WarehouseId]         BIGINT         NULL,
    [MinValueTemperature] FLOAT (53)     NULL,
    [MaxValueTemperature] FLOAT (53)     NULL,
    [MinValueHumidity]    FLOAT (53)     NULL,
    [MaxValueHumidity]    FLOAT (53)     NULL,
    [ToEmails]            NVARCHAR (MAX) NULL,
    [AlertTypeLookupId]   INT            NULL,
    [Interval]            INT            NULL,
    [Serial]              NVARCHAR (50)  NULL,
    [CreatedDate]         DATETIME2 (7)  NULL,
    [UserName]            NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AlertBySensor] PRIMARY KEY CLUSTERED ([Id] ASC)
);

