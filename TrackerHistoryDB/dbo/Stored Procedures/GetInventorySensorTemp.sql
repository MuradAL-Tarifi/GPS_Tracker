-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetInventorySensorTemp]
	-- Add the parameters for the stored procedure here
	@InventoryId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select [Id], [InventoryId], [GatewayIMEI], [Serial], 
	[Temperature], [Humidity], [IsLowVoltage], [GpsDate], [Alram], [GSMStatus], RowID
From 
( 
 
Select  [Id], [InventoryId], [GatewayIMEI], [Serial], [Temperature], [Humidity], 
[IsLowVoltage], [GpsDate], [Alram], [GSMStatus], 
Row_Number() OVER(Partition By Serial Order By GPsDate DESC) AS RowID 
  From [TrackerHistoryDB].[dbo].[InventoryHistory] IHI ) AS Data
  Where RowID = 1
  AND InventoryId = @InventoryId
  Order by GpsDate DESC
END
