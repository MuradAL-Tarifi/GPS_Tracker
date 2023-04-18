CREATE TABLE [dbo].[UserRole] (
    [Id]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId] NVARCHAR (MAX) NULL,
    [RoleId] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([Id] ASC)
);

