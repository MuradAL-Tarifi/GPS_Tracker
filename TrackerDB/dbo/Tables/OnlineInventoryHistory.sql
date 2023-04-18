CREATE TABLE [dbo].[OnlineInventoryHistory] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [GatewayIMEI]  NVARCHAR(500)          NOT NULL,
    [Serial]       NVARCHAR(50) NOT NULL, 
    [Temperature]  DECIMAL (18, 2) NULL,
    [Humidity]     DECIMAL (18, 2) NULL,
    [IsLowVoltage] BIT             NULL,
    [GpsDate]      DATETIME2 (7)   NOT NULL,
    [Alram]        NVARCHAR (MAX)  NULL,
    [GSMStatus]    NVARCHAR (MAX)  NULL,
    CONSTRAINT [PK_OnlineInventoryHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);

