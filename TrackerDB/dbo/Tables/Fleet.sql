CREATE TABLE [dbo].[Fleet] (
    [Id]                           BIGINT         IDENTITY (1, 1) NOT NULL,
    [AgentId]                      INT            NOT NULL,
    [Name]                         NVARCHAR (MAX) NULL,
    [NameEn]                       NVARCHAR (MAX) NULL,
    [ManagerEmail]                 NVARCHAR (MAX) NULL,
    [ManagerMobile]                NVARCHAR (MAX) NULL,
    [SupervisorEmail]              NVARCHAR (MAX) NULL,
    [SupervisorMobile]             NVARCHAR (MAX) NULL,
    [IsDeleted]                    BIT            NOT NULL,
    [CreatedDate]                  DATETIME2 (7)  NULL,
    [CreatedBy]                    NVARCHAR (MAX) NULL,
    [UpdatedDate]                  DATETIME2 (7)  NULL,
    [UpdatedBy]                    NVARCHAR (MAX) NULL,
    [TaxRegistrationNumber]        NVARCHAR (MAX) NULL,
    [CommercialRegistrationNumber] NVARCHAR (MAX) NULL,
    [LogoPhotoByte] [varbinary](max) NULL,
    [LogoPhotoExtention]                         NVARCHAR (50) NULL,
    CONSTRAINT [PK_Fleet] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Fleet_Agent_AgentId] FOREIGN KEY ([AgentId]) REFERENCES [dbo].[Agent] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Fleet_AgentId]
    ON [dbo].[Fleet]([AgentId] ASC);

