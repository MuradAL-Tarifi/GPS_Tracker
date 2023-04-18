CREATE TABLE [dbo].[DeviceType] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (MAX) NULL,
    [IsDeleted] BIT            NOT NULL,
    CONSTRAINT [PK_DeviceType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

