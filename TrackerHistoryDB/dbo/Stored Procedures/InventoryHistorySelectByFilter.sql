-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InventoryHistorySelectByFilter]
	-- Add the parameters for the stored procedure here
	@inventoryId bigint, 
	@fromDate datetime,
	@toDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


SELECT *
FROM  InventoryHistory where InventoryId=@inventoryId and GPSDate >= @FromDate  and GPSDate <= @ToDate
order by  GPSDate desc

END
