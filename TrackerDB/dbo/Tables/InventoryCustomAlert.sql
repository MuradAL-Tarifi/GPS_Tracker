CREATE TABLE [dbo].[InventoryCustomAlert]
(
    [Id]            BIGINT  IDENTITY (1, 1) NOT NULL,
    [CustomAlertId] BIGINT NOT NULL, 
    [InventoryId] BIGINT NOT NULL, 
    CONSTRAINT [PK_InventoryCustomAlert] PRIMARY KEY CLUSTERED ([Id] ASC),
)
