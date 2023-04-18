CREATE TABLE [dbo].[CustomAlert]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL,
    [Title]             NVARCHAR (MAX) NULL,
    [MinValueTemperature]          FLOAT (53)     NULL,
    [MaxValueTemperature]          FLOAT (53)     NULL,
    [MinValueHumidity]          FLOAT (53)     NULL,
    [MaxValueHumidity]          FLOAT (53)     NULL,
    [Interval]          INT            NOT NULL,
    [ToEmails]          NVARCHAR (MAX) NULL,
    [IsActive]          BIT            NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedDate]       DATETIME2 (7)  NULL,
    [CreatedBy]         NVARCHAR (MAX) NULL,
    [UpdatedDate]       DATETIME2 (7)  NULL,
    [UpdatedBy]         NVARCHAR (MAX) NULL,
    [LastAlertDate]     DATETIME2 (7)  NULL,
    [AlertTypeLookupId] INT            NOT NULL,
    [FleetId] BIGINT NOT NULL, 
    [UserIds] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_CustomAlert] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CustomAlert_AlertTypeLookup_AlertTypeLookupId] FOREIGN KEY ([AlertTypeLookupId]) REFERENCES [dbo].[AlertTypeLookup] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CustomAlert_Fleet_FleetId] FOREIGN KEY ([FleetId]) REFERENCES [dbo].[Fleet] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomAlert_AlertTypeLookupId]
    ON [dbo].[CustomAlert]([AlertTypeLookupId] ASC);
