CREATE TABLE [dbo].[InventorySensor] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [InventoryId] BIGINT         NOT NULL,
    [SensorId]      BIGINT         NULL,
    [IsDeleted]   BIT            NOT NULL,
    [CreatedDate] DATETIME2 (7)  NULL,
    [CreatedBy]   NVARCHAR (MAX) NULL,
    [UpdatedDate] DATETIME2 (7)  NULL,
    [UpdatedBy]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_InventorySensor] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_InventorySensor_Inventory_InventoryId] FOREIGN KEY ([InventoryId]) REFERENCES [dbo].[Inventory] ([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_InventorySensor_Sensor_SensorId] FOREIGN KEY ([SensorId]) REFERENCES [dbo].[Sensor] ([Id]) ON DELETE CASCADE, 
);


GO
CREATE NONCLUSTERED INDEX [IX_InventorySensor_InventoryId]
    ON [dbo].[InventorySensor]([InventoryId] ASC);

