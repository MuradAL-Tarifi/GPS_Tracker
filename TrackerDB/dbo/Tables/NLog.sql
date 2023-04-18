
CREATE TABLE [dbo].[NLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[MachineName] [nvarchar](200) NOT NULL,
	[SiteName] [nvarchar](300) NULL,
	[Logged] [datetime] NOT NULL,
	[Level] [nvarchar](5) NOT NULL,
	[Username] [nvarchar](100) NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Logger] [nvarchar](300) NULL,
	[Properties] [nvarchar](max) NULL,
	[ServerName] [nvarchar](200) NULL,
	[Port] [nvarchar](10) NULL,
	[Url] [nvarchar](2256) NULL,
	[Https] [nvarchar](5) NULL,
	[ServerAddress] [nvarchar](300) NULL,
	[RemoteAddress] [nvarchar](300) NULL,
	[Callsite] [nvarchar](300) NULL,
	[Exception] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


