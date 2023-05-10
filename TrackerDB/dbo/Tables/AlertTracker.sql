CREATE TABLE [dbo].[AlertTracker] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [UserName]        NVARCHAR (100) NULL,
    [AlertDateTime]   DATETIME       NULL,
    [AlertType]       NVARCHAR (MAX) NULL,
    [MonitoredUnit]   NVARCHAR (MAX) NULL,
    [MessageForValue] NVARCHAR (MAX) NULL,
    [Serial]          NVARCHAR (MAX) NULL,
    [Zone]            NVARCHAR (MAX) NULL,
    [WarehouseName]   NVARCHAR (MAX) NULL,
    [SendTo]          NVARCHAR (MAX) NULL,
    [IsSend]          BIT            NULL,
    [AlertId]         INT            NULL,
    [Interval]        INT            NULL,
    CONSTRAINT [PK_AlertTracker] PRIMARY KEY CLUSTERED ([Id] ASC)
);

