USE watershop
GO

/****** Object:  StoredProcedure [dbo].[CESP_MAPUPD]    Script Date: 28/11/2014 3:21:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[CESP_MAPUPD] 
@csv nvarchar(MAX)
AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

BEGIN TRAN
    DECLARE @pos int = 0
    DECLARE @pos2 int = 0
    DECLARE @pos3 int = 0
    DECLARE @itno int
    DECLARE @qty int
    while @pos3 <> LEN(@csv)
    BEGIN
		SET @pos = @pos3
		SET @pos2 = CHARINDEX(',', @csv, @pos+1)
		SET @pos3 = CHARINDEX(',', @csv, @pos2+1)
		SET @itno = SUBSTRING(@csv, (@pos+1), @pos2 - (@pos+1))
		SET @qty = SUBSTRING(@csv, (@pos2+1), @pos3 - (@pos2+1))
		IF EXISTS ( SELECT 1 FROM [dbo].[The Water Shop$Integration Mapping Table] WHERE [LoadingIndex] = @itno) BEGIN
			UPDATE [dbo].[The Water Shop$Integration Mapping Table]
			SET [NCID] = @qty
			WHERE  [LoadingIndex] = @itno
		END
    END

COMMIT

GO


