CREATE TABLE [dbo].[PrivilegeType] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (MAX) NULL,
    [NameEn]    NVARCHAR (MAX) NULL,
    [IsDeleted] BIT            NOT NULL,
    [Order]     INT            NULL,
    [RoleId]    NVARCHAR (MAX) NULL,
    [Editable]  BIT            NOT NULL,
    CONSTRAINT [PK_PrivilegeType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

