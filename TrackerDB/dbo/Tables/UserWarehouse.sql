CREATE TABLE [dbo].[UserWarehouse] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]      NVARCHAR (MAX) NULL,
    [WarehouseId] BIGINT         NOT NULL,
    CONSTRAINT [PK_UserWarehouse] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserWarehouse_Warehouse_WarehouseId] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouse] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_UserWarehouse_WarehouseId]
    ON [dbo].[UserWarehouse]([WarehouseId] ASC);

