CREATE TABLE [dbo].[ScheduleTypeLookup] (
    [Id]     INT NOT NULL,
    [Name]   NVARCHAR (MAX) NULL,
    [NameEn] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ScheduleTypeLookup] PRIMARY KEY CLUSTERED ([Id] ASC)
);

