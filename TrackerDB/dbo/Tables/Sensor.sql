CREATE TABLE [dbo].[Sensor]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Serial] NVARCHAR(50) NOT NULL,
    [BrandId] INT NOT NULL,
    [CalibrationDate] DATETIME NULL,
    [IsDeleted]   BIT            NOT NULL,
    [CreatedDate] DATETIME2 (7)  NULL,
    [CreatedBy]   NVARCHAR (MAX) NULL,
    [UpdatedDate] DATETIME2 (7)  NULL,
    [UpdatedBy]   NVARCHAR (MAX) NULL, 
    CONSTRAINT [FK_Sensor_Brand] FOREIGN KEY ([BrandId]) REFERENCES [Brand]([Id]), 

)
