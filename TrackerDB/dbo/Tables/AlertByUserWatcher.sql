CREATE TABLE [dbo].[AlertByUserWatcher]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
	[UserId]      NVARCHAR (MAX) NOT NULL,
    [AlertId] BIGINT         NOT NULL,
)
