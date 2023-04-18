CREATE TABLE [dbo].[Inventory] (
    [Id]                  BIGINT         IDENTITY (1, 1) NOT NULL,
    [WarehouseId]         BIGINT         NOT NULL,
    [GatewayId]           BIGINT         NOT NULL,
    [Name]                NVARCHAR (MAX) NULL,
    [InventoryNumber]     NVARCHAR (MAX) NULL,
    [IsActive]            BIT            NOT NULL,
    [RegisterTypeId]      int            NULL,
    [WaslActivityType]    NVARCHAR (MAX) NULL,
    [SFDAStoringCategory] NVARCHAR (MAX) NULL,
    [IsLinkedWithWasl]    BIT            NOT NULL,
    [ReferenceKey]        NVARCHAR (MAX) NULL,
    [IsDeleted]           BIT            NOT NULL,
    [CreatedDate]         DATETIME2 (7)  NULL,
    [CreatedBy]           NVARCHAR (MAX) NULL,
    [UpdatedDate]         DATETIME2 (7)  NULL,
    [UpdatedBy]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Inventory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Inventory_Gateway_GatewayId] FOREIGN KEY ([GatewayId]) REFERENCES [dbo].[Gateway] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Inventory_Warehouse_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouse] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Inventory_GatewayId]
    ON [dbo].[Inventory]([GatewayId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Inventory_WarehouseId]
    ON [dbo].[Inventory]([WarehouseId] ASC);

