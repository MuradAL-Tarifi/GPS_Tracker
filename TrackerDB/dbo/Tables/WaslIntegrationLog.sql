CREATE TABLE [dbo].[WaslIntegrationLog] (
    [Id]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [HttpCode] INT            NOT NULL,
    [Request]  NVARCHAR (MAX) NULL,
    [Response] NVARCHAR (MAX) NULL,
    [LogDate]  DATETIME2 (7)  NOT NULL,
    [WaslLogTypeLookupId] INT NOT NULL, 
    CONSTRAINT [PK_WaslIntegrationLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

