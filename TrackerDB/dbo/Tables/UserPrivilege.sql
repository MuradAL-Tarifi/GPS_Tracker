CREATE TABLE [dbo].[UserPrivilege] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]          NVARCHAR (MAX) NULL,
    [PrivilegeTypeId] INT            NOT NULL,
    [IsActive]        BIT            NOT NULL,
    CONSTRAINT [PK_UserPrivilege] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserPrivilege_PrivilegeType_PrivilegeTypeId] FOREIGN KEY ([PrivilegeTypeId]) REFERENCES [dbo].[PrivilegeType] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_UserPrivilege_PrivilegeTypeId]
    ON [dbo].[UserPrivilege]([PrivilegeTypeId] ASC);

