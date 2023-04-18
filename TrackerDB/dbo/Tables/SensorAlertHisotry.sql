CREATE TABLE [dbo].[SensorAlertHisotry]
(    
    [Id]                BIGINT          IDENTITY (1, 1) NOT NULL,
    [CustomerAlertId]   BIGINT          NOT NULL,
    [SensorId]          BIGINT          NULL,
    [LastAlertDate]     DATETIME        NOT NULL,
    CONSTRAINT [PK_SensorAlertHisotry] PRIMARY KEY CLUSTERED ([Id] ASC)
)
