CREATE TABLE [dbo].[RegisterType] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (MAX) NULL,
    [NameEn]    NVARCHAR (MAX) NULL,
    [IsDeleted] BIT            NOT NULL,
    CONSTRAINT [PK_RegisterType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

