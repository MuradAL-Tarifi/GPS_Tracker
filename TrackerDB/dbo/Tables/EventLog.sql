CREATE TABLE [dbo].[EventLog] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [Type]         NVARCHAR (MAX) NULL,
    [ObjectId]     NVARCHAR (MAX) NULL,
    [ObjectType]   NVARCHAR (MAX) NULL,
    [Data]         NVARCHAR (MAX) NULL,
    [UserId]       NVARCHAR (450) NULL,
    [CreationDate] DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_EventLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EventLog_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_EventLog_UserId]
    ON [dbo].[EventLog]([UserId] ASC);

