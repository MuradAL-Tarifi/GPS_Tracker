CREATE TABLE [dbo].[SMTPSetting] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [UserName]           VARCHAR (MAX) NULL,
    [Password]           VARCHAR (MAX) NULL,
    [MailAddress]        VARCHAR (MAX) NULL,
    [MaxEmailNumber]     INT           NULL,
    [CurrentEmailNumber] INT           NULL,
    CONSTRAINT [PK_SMTPSetting] PRIMARY KEY CLUSTERED ([Id] ASC)
);

