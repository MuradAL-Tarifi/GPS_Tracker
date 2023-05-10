CREATE TABLE [dbo].[CustomAlertWatcher] (
    [Id]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [FromDate]    DATETIME2 (7) NOT NULL,
    [ToDate]      DATETIME2 (7) NOT NULL,
    [CreatedDate] DATETIME2 (7) NULL,
    CONSTRAINT [PK_InventoryCustomAlertsWatcher] PRIMARY KEY CLUSTERED ([Id] ASC)
);


