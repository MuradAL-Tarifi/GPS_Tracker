CREATE TABLE [dbo].[CustomAlertWatcher]
(
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [FromDate]          DATETIME2 (7)   NULL, 
    [ToDate]            DATETIME2 (7)   NULL, 
    [CreatedDate]       DATETIME2 (7)   NULL,
    CONSTRAINT [PK_CustomAlertWatcher] PRIMARY KEY CLUSTERED ([Id] ASC)
)
