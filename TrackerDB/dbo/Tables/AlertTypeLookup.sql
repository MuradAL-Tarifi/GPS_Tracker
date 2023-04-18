CREATE TABLE [dbo].[AlertTypeLookup] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (MAX) NULL,
    [NameEn]             NVARCHAR (MAX) NULL,
    [RowOrder]           INT            NULL,
    [IsRange]            BIT            NOT NULL,
    [HasMinValue]        BIT            NOT NULL,
    [HasMaxValue]        BIT            NOT NULL,
    [DataType]           NVARCHAR (MAX) NULL,
    [Unit]               NVARCHAR (MAX) NULL,
    [UnitEn]             NVARCHAR (MAX) NULL,
    [IsDeleted]          BIT            NOT NULL,
    CONSTRAINT [PK_AlertTypeLookup] PRIMARY KEY CLUSTERED ([Id] ASC)
);

