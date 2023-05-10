CREATE TABLE [dbo].[Sensor] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (50)  NOT NULL,
    [Serial]          NVARCHAR (50)  NOT NULL,
    [BrandId]         INT            NOT NULL,
    [CalibrationDate] DATETIME       NULL,
    [IsDeleted]       BIT            NOT NULL,
    [CreatedDate]     DATETIME2 (7)  NULL,
    [CreatedBy]       NVARCHAR (MAX) NULL,
    [UpdatedDate]     DATETIME2 (7)  NULL,
    [UpdatedBy]       NVARCHAR (MAX) NULL,
    [DueDate]         DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Sensor_Brand] FOREIGN KEY ([BrandId]) REFERENCES [dbo].[Brand] ([Id])
);


