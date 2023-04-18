CREATE TABLE [dbo].[SystemSetting] (
    [Id]             BIGINT         IDENTITY (1, 1) NOT NULL,
	[LogoPhotoByte] [varbinary](max) NULL,
	[CompanyName] [nvarchar](200) NULL,
	[GoogleApiKey] [nvarchar](1000) NULL,
	[WaslApiKey] [nvarchar](1000) NULL,
	[EnableSMTP] [bit] NOT NULL,
	[SMTP_HOST] [nvarchar](500) NULL,
	[SMTP_PORT] [int] NULL,
	[SMTP_IsSslEnabled] [bit] NOT NULL,
	[SMTP_Address] [nvarchar](500) NULL,
	[SMTP_DisplayName] [nvarchar](500) NULL,
	[SMTP_Password] [nvarchar](100) NULL,
	[EnableSMS] [bit] NOT NULL,
	[SMS_GatewayURL] [nvarchar](500) NULL,
	[SMS_Password] [nvarchar](100) NULL,
	[SMS_Username] [nvarchar](100) NULL,
   	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](max) NULL,
    CONSTRAINT [PK_SystemSetting] PRIMARY KEY CLUSTERED ([Id] ASC)
);

