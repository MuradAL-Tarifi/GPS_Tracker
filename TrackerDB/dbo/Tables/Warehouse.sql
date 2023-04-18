CREATE TABLE [dbo].[Warehouse] (
    [Id]                    BIGINT          IDENTITY (1, 1) NOT NULL,
    [FleetId]               BIGINT          NOT NULL,
    [Name]                  NVARCHAR (MAX)  NULL,
    [Phone]                 NVARCHAR (MAX)  NULL,
    [Address]               NVARCHAR (MAX)  NULL,
    [City]                  NVARCHAR (MAX)  NULL,
    [Latitude]              DECIMAL (9, 7) NULL,
    [Longitude]             DECIMAL (9, 7) NULL,
    [LandCoordinates]       NVARCHAR (MAX)  NULL,
    [LandAreaInSquareMeter] FLOAT (53)      NOT NULL,
    [LicenseNumber]         NVARCHAR (MAX)  NULL,
    [LicenseIssueDate]      NVARCHAR (MAX)  NULL,
    [LicenseExpiryDate]     NVARCHAR (MAX)  NULL,
    [ManagerMobile]         NVARCHAR (MAX)  NULL,
    [Email]                 NVARCHAR (MAX)  NULL,
    [IsActive]              BIT             NOT NULL,
    [ReferenceKey]          NVARCHAR (MAX)  NULL,
    [WaslActivityType]      NVARCHAR (MAX)  NULL,
    [IsLinkedWithWasl]      BIT             NOT NULL,
    [IsDeleted]             BIT             NOT NULL,
    [CreatedDate]           DATETIME2 (7)   NULL,
    [CreatedBy]             NVARCHAR (MAX)  NULL,
    [UpdatedDate]           DATETIME2 (7)   NULL,
    [UpdatedBy]             NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_Warehouse] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Warehouse_Fleet_FleetId] FOREIGN KEY ([FleetId]) REFERENCES [dbo].[Fleet] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Warehouse_FleetId]
    ON [dbo].[Warehouse]([FleetId] ASC);

