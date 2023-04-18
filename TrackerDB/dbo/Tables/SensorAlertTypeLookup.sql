CREATE TABLE [dbo].[SensorAlertTypeLookup] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (MAX) NULL,
    [NameEn]    NVARCHAR (MAX) NULL,
    [RowOrder]  INT            NULL,
    [IsRange]   BIT            NOT NULL,
    [DataType]  NVARCHAR (MAX) NULL,
    [IsDeleted] BIT            NOT NULL,
    CONSTRAINT [PK_SensorAlertTypeLookup] PRIMARY KEY CLUSTERED ([Id] ASC)
);

