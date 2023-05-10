CREATE TABLE [dbo].[Alert] (
    [Id]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [AlertTypeLookupId] INT            NOT NULL,
    [FleetId]           BIGINT         NOT NULL,
    [WarehouseId]       BIGINT         NULL,
    [InventoryId]       BIGINT         NULL,
    [SensorId]          BIGINT         NULL,
    [AlertTextAr]       NVARCHAR (MAX) NULL,
    [AlertTextEn]       NVARCHAR (MAX) NULL,
    [AlertForValueAr]   NVARCHAR (MAX) NULL,
    [AlertForValueEn]   NVARCHAR (MAX) NULL,
    [AlertDateTime]     DATETIME2 (7)  NOT NULL,
    [IsDismissed]       BIT            NOT NULL,
    [CustomAlertId]     BIGINT         NOT NULL,
    [Temperature]       DECIMAL (8, 2) NULL,
    [Humidity]          DECIMAL (8, 2) NULL,
    [IsDeleted]         BIT            NOT NULL,
    [CreatedDate]       DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_Alert] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Alert_CustomAlert] FOREIGN KEY ([CustomAlertId]) REFERENCES [dbo].[CustomAlert] ([Id])
);




GO
Create TRIGGER [dbo].[SMTPAlert]
ON [dbo].[Alert]
AFTER INSERT
AS
DECLARE @RecId INT;

DECLARE @UserName nvarchar(Max);
DECLARE @AlertDateTime datetime;
DECLARE @AlertType nvarchar(Max);
DECLARE @MonitoredUnit nvarchar(Max);
DECLARE @MessageForValue nvarchar(Max);
DECLARE @Serial nvarchar(Max);
DECLARE @WarehouseName nvarchar(Max);
DECLARE @Zone nvarchar(Max);

DECLARE @SendTo nvarchar(100);


BEGIN
    SET NOCOUNT ON;

	select @RecId=MAX(Id) FROM Alert ; --last inserted record id

	select @UserName        = u.UserName FROM Alert a , CustomAlert ca , [dbo].[User] u WHERE a.CustomAlertId=ca.Id and ca.UserIds=u.Id and a.Id = @RecId;
	select @AlertDateTime   = AlertDateTime FROM Alert WHERE Id = @RecId;
	select @AlertType	    = NameEn FROM AlertTypeLookup WHERE Id = (select AlertTypeLookupId FROM Alert WHERE Id = @RecId );	
	select @MonitoredUnit = s.Name +' ('+s.Serial+')' FROM Alert a , Sensor s WHERE a.SensorId=s.Id and a.Id = @RecId;
	select @MessageForValue = AlertForValueEn FROM Alert WHERE Id = @RecId;
	select @Serial = s.Serial FROM Alert a , Sensor s WHERE a.SensorId=s.Id and a.Id = @RecId;
	select @Zone		    = f.Name FROM Alert a , Fleet f WHERE a.FleetId=f.Id and a.Id = @RecId;
	select @WarehouseName   = w.Name FROM Alert a , Warehouse w WHERE a.WarehouseId=w.Id and a.Id = @RecId;
	select @SendTo = ca.ToEmails FROM Alert a , CustomAlert ca WHERE a.CustomAlertId=ca.Id and a.Id = @RecId;


    INSERT INTO [SMTPTracker].[dbo].[AlertTracker] VALUES 
	(@UserName,@AlertDateTime,@AlertType,@MonitoredUnit,@MessageForValue,@Serial,@Zone,@WarehouseName,@SendTo,0,@RecId)
END