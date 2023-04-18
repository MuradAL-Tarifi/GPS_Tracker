CREATE TABLE [dbo].[InventoryHistory] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [InventoryId]  BIGINT         NOT NULL,
    [GatewayIMEI]  NVARCHAR(500)         NOT NULL,
    [Serial]       NVARCHAR(50)         NOT NULL,
    [Temperature]  DECIMAL (8, 2) NULL,
    [Humidity]     DECIMAL (8, 2) NULL,
    [IsLowVoltage] BIT            NULL,
    [GpsDate]      DATETIME       NOT NULL,
    [Alram]        VARCHAR (100)  NULL,
    [GSMStatus]    VARCHAR (50)   NULL,
    CONSTRAINT [PK_GatewayHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);

