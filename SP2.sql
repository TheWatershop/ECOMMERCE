USE [watershop]
GO

/****** Object:  StoredProcedure [dbo].[CESP_NAVSET]    Script Date: 28/11/2014 3:24:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[CESP_NAVSET] 
    @RecType VarChar(max),
    @NCNo Int,
    @NCDoc Int,
    @f1 VarChar(max),
    @f2 Int,
    @f3 VarChar(max),
    @f4 Decimal,
    @f5 Decimal,
    @f6 Decimal,
    @f7 Decimal,
    @f8 Bit,
    @f9 VarChar(max),
    @f10 Decimal,
    @f11 VarChar(max),
    @f12 VarChar(max),
    @f13 VarChar(max),
    @f14 VarChar(max),
    @f15 Int,
    @f16 VarChar(max),
    @f17 VarChar(max),
    @f18 VarChar(max),
    @f19 Bit,
    @f20 DateTime,
 @CompName nvarchar(50)
AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

    BEGIN TRAN
    IF @f20 is null
      set @f20 = '1753-01-01 00:00:00.000'
    IF EXISTS ( SELECT 1 FROM [dbo].[The Water Shop$Integration Table]  WHERE [NCNo] = @NCNo AND [RecType] = @RecType) BEGIN
        UPDATE [dbo].[The Water Shop$Integration Table]
        SET [RecType] = @RecType, [NCNo] = @NCNo,[NCDoc] = @NCDoc,[Status] = '2',[ErrorText] = '',[f1] = @f1,[f2] = @f2,[f3] = @f3,[f4] = @f4,[f5] = @f5,[f6] = @f6,[f7] = @f7,[f8] = @f8,[f9] = @f9,[f10] = @f10,[f11] = @f11,[f12] = @f12,[f13] = @f13,[f14] = @f14,[f15] = @f15,[f16] = @f16,[f17] = @f17,[f18] = @f18,[f19] = @f19,[f20] = @f20
        WHERE  [NCNo] = @NCNo AND [RecType] = @RecType
    END ELSE BEGIN
        INSERT INTO [dbo].[The Water Shop$Integration Table] ([RecType],[NCNo],[NCDoc],[Status],[ErrorText],[f1],[f2],[f3],[f4],[f5],[f6],[f7],[f8],[f9],[f10],[f11],[f12],[f13],[f14],[f15],[f16],[f17],[f18],[f19],[f20])
        SELECT @RecType,@NCNo,@NCDoc,'2','',@f1,@f2,@f3,@f4,@f5,@f6,@f7,@f8,@f9,@f10,@f11,@f12,@f13,@f14,@f15,@f16,@f17,@f18,@f19,@f20
    END

    COMMIT

GO


