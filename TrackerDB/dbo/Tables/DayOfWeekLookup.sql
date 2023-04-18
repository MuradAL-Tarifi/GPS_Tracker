CREATE TABLE [dbo].[DayOfWeekLookup] (
    [Id]       INT NOT NULL,
    [Name]     NVARCHAR (MAX) NULL,
    [NameEn]   NVARCHAR (MAX) NULL,
    [RowOrder] INT            NOT NULL,
    CONSTRAINT [PK_DayOfWeekLookup] PRIMARY KEY CLUSTERED ([Id] ASC)
);

