CREATE TABLE [dbo].[UserInventory] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]      NVARCHAR (MAX) NULL,
    [InventoryId] BIGINT         NOT NULL,
    CONSTRAINT [PK_UserInventory] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserInventory_Inventory] FOREIGN KEY ([InventoryId]) REFERENCES [dbo].[Inventory] ([Id])
);




GO

