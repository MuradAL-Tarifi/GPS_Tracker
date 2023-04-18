

Create PROCEDURE [dbo].[AddNLog] (

  @machineName nvarchar(200),
  @siteName nvarchar(300),
  @logged datetime,
  @level varchar(5),
  @username nvarchar(100),
  @message nvarchar(max),
  @logger nvarchar(300),
  @properties nvarchar(max),
  @serverName nvarchar (200),
  @port nvarchar(10),
  @url nvarchar(2256),
  @https nvarchar (5),
  @serverAddress nvarchar(300),
  @remoteAddress nvarchar(300),
  @callsite nvarchar(300),
  @exception nvarchar(max)

) AS

BEGIN

  INSERT INTO [dbo].[NLog] (

    [MachineName],

       [SiteName],

    [Logged],

    [Level],

       [Username],

    [Message],

    [Logger],

    [Properties],

       [ServerName],

       [Port],

       [Url],

       [Https],

       [ServerAddress],

       [RemoteAddress] ,

    [Callsite],

    [Exception]

  ) VALUES (

    @machineName,

       @siteName,

    @logged,

    @level,

       @username,

    @message,

    @logger,

    @properties,

       @serverName,

       @port,

       @url,

       @https,

       @serverAddress,

       @remoteAddress,

    @callsite,

    @exception

  );

END
