CREATE TABLE [dbo].[Role] (
    [Id]            NVARCHAR (450) NOT NULL,
    [Name]          NVARCHAR (MAX) NULL,
    [DisplayName]   NVARCHAR (MAX) NULL,
    [DisplayNameEn] NVARCHAR (MAX) NULL,
    [Order]         INT            NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC)
);

