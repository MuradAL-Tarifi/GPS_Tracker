CREATE TABLE [dbo].[SensorAlert] (
    [Id]                      BIGINT          IDENTITY (1, 1) NOT NULL,
    [InventorySensorId]       BIGINT          NOT NULL,
    [SensorAlertTypeLookupId] INT             NOT NULL,
    [IsActive]                BIT             NOT NULL,
    [IsSMS]                   BIT             NOT NULL,
    [IsEmail]                 BIT             NOT NULL,
    [FromValue]               DECIMAL (18, 2) NULL,
    [ToValue]                 DECIMAL (18, 2) NULL,
    CONSTRAINT [PK_SensorAlert] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SensorAlert_InventorySensor_InventorySensorId] FOREIGN KEY ([InventorySensorId]) REFERENCES [dbo].[InventorySensor] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SensorAlert_SensorAlertTypeLookup_SensorAlertTypeLookupId] FOREIGN KEY ([SensorAlertTypeLookupId]) REFERENCES [dbo].[SensorAlertTypeLookup] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_SensorAlert_SensorAlertTypeLookupId]
    ON [dbo].[SensorAlert]([SensorAlertTypeLookupId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SensorAlert_InventorySensorId]
    ON [dbo].[SensorAlert]([InventorySensorId] ASC);

