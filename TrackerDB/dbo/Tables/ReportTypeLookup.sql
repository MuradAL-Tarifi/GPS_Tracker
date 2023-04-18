CREATE TABLE [dbo].[ReportTypeLookup] (
    [Id]        INT NOT NULL,
    [Name]      NVARCHAR (MAX) NULL,
    [NameEn]    NVARCHAR (MAX) NULL,
    [IsDeleted] BIT            NOT NULL,
    CONSTRAINT [PK_ReportTypeLookup] PRIMARY KEY CLUSTERED ([Id] ASC)
);

