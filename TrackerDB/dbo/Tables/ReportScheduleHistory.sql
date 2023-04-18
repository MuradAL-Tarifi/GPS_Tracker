CREATE TABLE [dbo].[ReportScheduleHistory] (
    [Id]               BIGINT        IDENTITY (1, 1) NOT NULL,
    [ReportScheduleId] BIGINT        NOT NULL,
    [ScheduleTypeId]   INT           NOT NULL,
    [DueDateTime]      DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_ReportScheduleHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);

