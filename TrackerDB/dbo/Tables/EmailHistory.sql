CREATE TABLE [dbo].[EmailHistory] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [AlertId]     BIGINT         NULL,
    [Title]       NVARCHAR (MAX) NULL,
    [Body]        NVARCHAR (MAX) NULL,
    [ToEmails]    NVARCHAR (MAX) NULL,
    [CreatedDate] DATETIME2 (7)  NOT NULL,
    [IsSent]      BIT            NOT NULL,
    [SentDate]    DATETIME2 (7)  NULL,
    CONSTRAINT [PK_EmailHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);

