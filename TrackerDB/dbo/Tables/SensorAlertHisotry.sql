CREATE TABLE [dbo].[SensorAlertHisotry] (
    [Id]              BIGINT        IDENTITY (1, 1) NOT NULL,
    [CustomerAlertId] BIGINT        NOT NULL,
    [SensorId]        BIGINT        NOT NULL,
    [LastAlertDate]   DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_SensorsAlertHisotry] PRIMARY KEY CLUSTERED ([Id] ASC)
);


