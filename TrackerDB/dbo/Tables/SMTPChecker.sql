CREATE TABLE [dbo].[SMTPChecker] (
    [Id]                      BIGINT         IDENTITY (1, 1) NOT NULL,
    [Serial]                  NVARCHAR (MAX) NULL,
    [IsSendTemperature]       BIT            NULL,
    [IsSendHumidity]          BIT            NULL,
    [IsSendTemperatureSecond] BIT            NULL,
    [IsSendHumiditySecond]    BIT            NULL,
    [UpdatedDateTemperature]  DATETIME       NULL,
    [UpdatedDateHumidity]     DATETIME       NULL,
    CONSTRAINT [PK_SMTPChecker] PRIMARY KEY CLUSTERED ([Id] ASC)
);

