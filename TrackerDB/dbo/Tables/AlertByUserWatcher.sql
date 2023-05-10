CREATE TABLE [dbo].[AlertByUserWatcher] (
    [Id]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]   NVARCHAR (MAX) NOT NULL,
    [AlertId]  BIGINT         NOT NULL,
    [ViewDate] DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_AlertByUserWatcher] PRIMARY KEY CLUSTERED ([Id] ASC)
);


