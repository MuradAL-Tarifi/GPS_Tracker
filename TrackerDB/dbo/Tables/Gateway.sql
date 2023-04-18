CREATE TABLE [dbo].[Gateway] (
    [Id]             BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (MAX) NULL,
    [IMEI]           NVARCHAR(500)         NOT NULL,
    [SIMNumber]      NVARCHAR (MAX) NULL,
    [ExpirationDate] DATETIME2 (7)  NOT NULL,
    [BrandId] INT NOT NULL,
    [IsActive]       BIT            NOT NULL,
    [IsDeleted]      BIT            NOT NULL,
    [CreatedDate]    DATETIME2 (7)  NULL,
    [CreatedBy]      NVARCHAR (MAX) NULL,
    [UpdatedDate]    DATETIME2 (7)  NULL,
    [UpdatedBy]      NVARCHAR (MAX) NULL,
    [ActivationDate] DATETIME2 NULL, 
    [SIMCardExpirationDate] DATETIME2 NULL, 
    [NumberOfMonths] INT NULL, 
    [Notes] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_Gateway] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Gateway_Brand] FOREIGN KEY ([BrandId]) REFERENCES [Brand]([Id])
);

