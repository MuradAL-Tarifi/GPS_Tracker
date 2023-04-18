CREATE TABLE [dbo].[User] (
    [Id]                 NVARCHAR (450) NOT NULL,
    [UserName]           NVARCHAR (MAX) NULL,
    [Password]           NVARCHAR (MAX) NULL,
    [Name]               NVARCHAR (MAX) NULL,
    [NameEn]             NVARCHAR (MAX) NULL,
    [Email]              NVARCHAR (MAX) NULL,
    [IsActive]           BIT            NOT NULL,
    [ExpirationDate]     DATETIME2 (7)  NULL,
    [IsAdmin]            BIT            NOT NULL,
    [IsDeleted]          BIT            NOT NULL,
    [CreatedDate]        DATETIME2 (7)  NULL,
    [CreatedBy]          NVARCHAR (MAX) NULL,
    [UpdatedDate]        DATETIME2 (7)  NULL,
    [UpdatedBy]          NVARCHAR (MAX) NULL,
    [AgentId]            INT            NULL,
    [FleetId]            BIGINT         NULL,
    [AppId]              NVARCHAR (MAX) NULL,
    [EnableMobileAlerts] BIT            NOT NULL,
    [IsSubAdminAgent] BIT NOT NULL,
    [IsSuperAdmin] BIT NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_Agent_AgentId] FOREIGN KEY ([AgentId]) REFERENCES [dbo].[Agent] ([Id]),
    CONSTRAINT [FK_User_Fleet_FleetId] FOREIGN KEY ([FleetId]) REFERENCES [dbo].[Fleet] ([Id]),
);


GO
CREATE NONCLUSTERED INDEX [IX_User_AgentId]
    ON [dbo].[User]([AgentId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_User_FleetId]
    ON [dbo].[User]([FleetId] ASC);



