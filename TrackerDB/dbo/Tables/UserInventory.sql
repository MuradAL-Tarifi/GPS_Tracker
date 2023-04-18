CREATE TABLE [dbo].[UserInventory]
(
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]      NVARCHAR (MAX) NULL,
    [InventoryId] BIGINT         NOT NULL,
    CONSTRAINT [PK_UserInventory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserInventory_Inventory_InventoryId] FOREIGN KEY ([InventoryId]) REFERENCES [dbo].[Warehouse] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_UserInventory_InventoryId]
    ON [dbo].[UserInventory]([InventoryId] ASC);
