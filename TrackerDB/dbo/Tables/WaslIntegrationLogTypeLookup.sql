CREATE TABLE [dbo].[WaslIntegrationLogTypeLookup] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_WaslIntegrationLogTypeLookup] PRIMARY KEY CLUSTERED ([Id] ASC)
);

