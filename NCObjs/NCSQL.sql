USE [WSNOPInt];
GO
IF OBJECT_ID('[dbo].[CESP_NAVSET]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_NAVSET] 
END 
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
USE [NOP];
GO
IF COL_LENGTH('Address','CustId') IS NULL
BEGIN
    ALTER TABLE Address
    ADD CustID int
END
GO
IF COL_LENGTH('OrderItem','ExpiryDate') IS NULL
BEGIN
    ALTER TABLE OrderItem
    ADD ExpiryDate datetime NULL
END
GO
IF OBJECT_ID('[dbo].[CESP_QTYUPD]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_QTYUPD] 
END 
GO
CREATE PROC [dbo].[CESP_QTYUPD] 
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
    IF EXISTS ( SELECT 1 FROM [dbo].[Product] WHERE [Id] = @itno) BEGIN
        UPDATE [dbo].[Product]
        SET [StockQuantity] = @qty
        WHERE  [Id] = @itno
    END
    END

    COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_GETCUST]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_GETCUST] 
END 
GO
CREATE PROC [dbo].[CESP_GETCUST] 
@AddrId int,
@RetNo int out
AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

    BEGIN TRAN
    DECLARE @pos int = 0
    IF @AddrId <> 0 BEGIN
        IF EXISTS ( SELECT 1 FROM [dbo].[CustomerAddresses] WHERE [Address_Id] = @AddrId) BEGIN
        	SET @RetNo = (Select Customer_Id FROM [dbo].[CustomerAddresses] WHERE [Address_Id] = @AddrId)
        END ELSE IF EXISTS ( SELECT 1 FROM [dbo].[Order] WHERE [BillingAddressId] = @AddrId) BEGIN
        	SET @RetNo = (Select CustomerId FROM [dbo].[Order] WHERE [BillingAddressId] = @AddrId)
        END ELSE IF EXISTS ( SELECT 1 FROM [dbo].[Order] WHERE [ShippingAddressId] = @AddrId) BEGIN
        	SET @RetNo = (Select CustomerId FROM [dbo].[Order] WHERE [ShippingAddressId] = @AddrId)
        END
    END

    COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_CATSEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_CATSEL] 
END 
GO
CREATE PROC [dbo].[CESP_CATSEL] 
    @LastDate datetime
AS 
  SET NOCOUNT ON 
  SET XACT_ABORT ON  

  BEGIN TRAN

  SELECT [Id], Name, Description, CategoryTemplateId, MetaKeywords, MetaDescription, MetaTitle, ParentCategoryId, PictureId, PageSize, AllowCustomersToSelectPageSize, PageSizeOptions, PriceRanges, ShowOnHomePage, IncludeInTopMenu, HasDiscountsApplied, SubjectToAcl, LimitedToStores, Published, Deleted, DisplayOrder, CreatedOnUtc, UpdatedOnUtc
  FROM [dbo].[Category] 
  WHERE  ([UpdatedOnUtc] > @LastDate)

  COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_CATPROC]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_CATPROC] 
END 
GO
CREATE PROC [dbo].[CESP_CATPROC] 
    @Id int,
    @Name nvarchar(400),
    @Description nvarchar(MAX),
    @CategoryTemplateId int,
    @MetaKeywords nvarchar(400),
    @MetaDescription nvarchar(MAX),
    @MetaTitle nvarchar(400),
    @ParentCategoryId int,
    @PictureId int,
    @PageSize int,
    @AllowCustomersToSelectPageSize bit,
    @PageSizeOptions nvarchar(200),
    @PriceRanges nvarchar(400),
    @ShowOnHomePage bit,
    @IncludeInTopMenu bit,
    @HasDiscountsApplied bit,
    @SubjectToAcl bit,
    @LimitedToStores bit,
    @Published bit,
    @Deleted bit,
    @DisplayOrder int,
    @CreatedOnUtc datetime,
    @UpdatedOnUtc datetime,
@RetNo int out
AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

    BEGIN TRAN
    IF @Id IS NULL OR @Id = 0 BEGIN
        INSERT INTO [dbo].[Category] ([Name], [Description], [CategoryTemplateId], [MetaKeywords], [MetaDescription], [MetaTitle], [ParentCategoryId], [PictureId], [PageSize], [AllowCustomersToSelectPageSize], [PageSizeOptions], [PriceRanges], [ShowOnHomePage], [IncludeInTopMenu], [HasDiscountsApplied], [SubjectToAcl], [LimitedToStores], [Published], [Deleted], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc])
        SELECT @Name, @Description, @CategoryTemplateId, @MetaKeywords, @MetaDescription, @MetaTitle, @ParentCategoryId, @PictureId, @PageSize, @AllowCustomersToSelectPageSize, @PageSizeOptions, @PriceRanges, @ShowOnHomePage, @IncludeInTopMenu, @HasDiscountsApplied, @SubjectToAcl, @LimitedToStores, @Published, @Deleted, @DisplayOrder, @CreatedOnUtc, @UpdatedOnUtc
    SET @RetNo = SCOPE_IDENTITY()
    END ELSE BEGIN
        IF EXISTS ( SELECT 1 FROM [dbo].[Category]  WHERE [Id] = @Id ) BEGIN
            UPDATE [dbo].[Category]
            SET [Name] = @Name, [Description] = @Description, [CategoryTemplateId] = @CategoryTemplateId, [MetaKeywords] = @MetaKeywords, [MetaDescription] = @MetaDescription, [MetaTitle] = @MetaTitle, [ParentCategoryId] = @ParentCategoryId, [PictureId] = @PictureId, [PageSize] = @PageSize, [AllowCustomersToSelectPageSize] = @AllowCustomersToSelectPageSize, [PageSizeOptions] = @PageSizeOptions, [PriceRanges] = @PriceRanges, [ShowOnHomePage] = @ShowOnHomePage, [IncludeInTopMenu] = @IncludeInTopMenu, [HasDiscountsApplied] = @HasDiscountsApplied, [SubjectToAcl] = @SubjectToAcl, [LimitedToStores] = @LimitedToStores, [Published] = @Published, [Deleted] = @Deleted, [DisplayOrder] = @DisplayOrder, [CreatedOnUtc] = @CreatedOnUtc, [UpdatedOnUtc] = @UpdatedOnUtc
            WHERE  [Id] = @Id
            SET @RetNo = @Id
        END
    END
    IF @RetNo <> 0 BEGIN
        IF NOT EXISTS ( SELECT 1 FROM [dbo].[URLRecord]  WHERE [EntityId] = @RetNo AND [EntityName] = 'Category' ) BEGIN
        INSERT INTO [dbo].[URLRecord] ([EntityId], [EntityName], [Slug], [IsActive], [LanguageId])
        SELECT @RetNo, 'Category', Replace(Replace(Replace(@Name,'&',''),' ',''),'.',''), 1, 0 
        END
    END

    COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_CATDEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_CATDEL] 
END 
GO
CREATE PROC [dbo].[CESP_CATDEL] 

    @Id int

AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

BEGIN TRAN

DELETE
FROM   [dbo].[Category]
WHERE  [Id] = @Id

COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_ROLEPROC]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_ROLEPROC] 
END 
GO
CREATE PROC [dbo].[CESP_ROLEPROC] 
    @Id int,
    @Name nvarchar(255),
    @FreeShipping bit,
    @TaxExempt bit,
    @Active bit,
    @IsSystemRole bit,
    @SystemName nvarchar(255),
@RetNo int out
AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

    BEGIN TRAN
    IF @Id IS NULL OR @Id = 0 BEGIN
        INSERT INTO [dbo].[CustomerRole] ([Name], [FreeShipping], [TaxExempt], [Active], [IsSystemRole], [SystemName])
        SELECT @Name, @FreeShipping, @TaxExempt, @Active, @IsSystemRole, @SystemName
    SET @RetNo = SCOPE_IDENTITY()
    END ELSE BEGIN
        IF EXISTS ( SELECT 1 FROM [dbo].[CustomerRole]  WHERE [Id] = @Id ) BEGIN
            UPDATE [dbo].[CustomerRole]
            SET [Name] = @Name, [FreeShipping] = @FreeShipping, [TaxExempt] = @TaxExempt, [Active] = @Active, [IsSystemRole] = @IsSystemRole, [SystemName] = @SystemName
            WHERE  [Id] = @Id
            SET @RetNo = @Id
        END
    END

    COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_ROLEDEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_ROLEDEL] 
END 
GO
CREATE PROC [dbo].[CESP_ROLEDEL] 

    @Id int

AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

BEGIN TRAN

DELETE
FROM   [dbo].[CustomerRole]
WHERE  [Id] = @Id

COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_CUSTSEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_CUSTSEL] 
END 
GO
CREATE PROC [dbo].[CESP_CUSTSEL] 
    @LastDate datetime
AS 
  SET NOCOUNT ON 
  SET XACT_ABORT ON  

  BEGIN TRAN

  SELECT [dbo].[Customer].[Id], [dbo].[Customer].CustomerGuid, [dbo].[Customer].Username, [dbo].[Customer].Email, [dbo].[Customer].Password, [dbo].[Customer].PasswordFormatId, [dbo].[Customer].PasswordSalt, [dbo].[Customer].AdminComment, [dbo].[Customer].IsTaxExempt, [dbo].[Customer].AffiliateId, [dbo].[Customer].VendorId, [dbo].[Customer].Active, [dbo].[Customer].Deleted, [dbo].[Customer].IsSystemAccount, [dbo].[Customer].SystemName, [dbo].[Customer].LastIpAddress, [dbo].[Customer].CreatedOnUtc, [dbo].[Customer].LastLoginDateUtc, [dbo].[Customer].LastActivityDateUtc, [dbo].[Customer].BillingAddress_Id, [dbo].[Customer].ShippingAddress_Id,[dbo].[Address].FirstName, [dbo].[Address].LastName, [dbo].[Address].Email, [dbo].[Address].Company, [dbo].[Address].CountryId, [dbo].[Address].StateProvinceId, [dbo].[Address].City, [dbo].[Address].Address1, [dbo].[Address].Address2, [dbo].[Address].ZipPostalCode, [dbo].[Address].PhoneNumber, [dbo].[Address].FaxNumber, [dbo].[Address].CreatedOnUtc,[dbo].[Customer_CustomerRole_Mapping].CustomerRole_Id
  FROM [dbo].[Customer] 
  LEFT JOIN Address
  ON [Customer].BillingAddress_Id = Address.Id
  LEFT JOIN Customer_CustomerRole_Mapping
  ON [Customer].Id = [Customer_CustomerRole_Mapping].Customer_Id AND [Customer_CustomerRole_Mapping].CustomerRole_Id > 5
  WHERE ([dbo].[Customer].[CreatedOnUtc] > @LastDate)

  COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_CUSTPROC]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_CUSTPROC] 
END 
GO
CREATE PROC [dbo].[CESP_CUSTPROC] 
    @Id int,
    @CustomerGuid uniqueidentifier,
    @Username nvarchar(MAX),
    @Email nvarchar(MAX),
    @Password nvarchar(MAX),
    @PasswordFormatId int,
    @PasswordSalt nvarchar(MAX),
    @AdminComment nvarchar(MAX),
    @IsTaxExempt bit,
    @AffiliateId int,
    @VendorId int,
    @Active bit,
    @Deleted bit,
    @IsSystemAccount bit,
    @SystemName nvarchar(MAX),
    @LastIpAddress nvarchar(MAX),
    @CreatedOnUtc datetime,
    @LastLoginDateUtc datetime,
    @LastActivityDateUtc datetime,
    @BillingAddress_Id int,
    @ShippingAddress_Id int,
    @FirstName nvarchar(MAX),
    @LastName nvarchar(MAX),
    @Company nvarchar(MAX),
    @CountryId int,
    @StateProvinceId int,
    @City nvarchar(MAX),
    @Address1 nvarchar(MAX),
    @Address2 nvarchar(MAX),
    @ZipPostalCode nvarchar(MAX),
    @PhoneNumber nvarchar(MAX),
    @FaxNumber nvarchar(MAX),
@CustomerRole_Id int,
@RetNo int out
AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

    BEGIN TRAN
        IF NOT EXISTS ( SELECT 1 FROM [dbo].[Address]  WHERE [Address1] = @Address1 ) BEGIN
            INSERT INTO [dbo].[Address] ([FirstName], [LastName], [Email], [Company], [CountryId], [StateProvinceId], [City], [Address1], [Address2], [ZipPostalCode], [PhoneNumber], [FaxNumber], [CreatedOnUtc])
            SELECT @FirstName, @LastName, @Email, @Company, @CountryId, @StateProvinceId, @City, @Address1, @Address2, @ZipPostalCode, @PhoneNumber, @FaxNumber, @CreatedOnUtc
            SET @BillingAddress_Id = SCOPE_IDENTITY()
        END ELSE BEGIN
            SELECT @BillingAddress_Id = [dbo].[Address].Id
            FROM Address
            WHERE [Address1] = @Address1
        END
            SET @Username = @Email
    IF @Id IS NULL OR @Id = 0 BEGIN
        INSERT INTO [dbo].[Customer] ([CustomerGuid], [Username], [Email], [Password], [PasswordFormatId], [PasswordSalt], [AdminComment], [IsTaxExempt], [AffiliateId], [VendorId], [Active], [Deleted], [IsSystemAccount], [SystemName], [LastIpAddress], [CreatedOnUtc], [LastLoginDateUtc], [LastActivityDateUtc], [BillingAddress_Id], [ShippingAddress_Id])
        SELECT @CustomerGuid, @Username, @Email, @Password, @PasswordFormatId, @PasswordSalt, @AdminComment, @IsTaxExempt, @AffiliateId, @VendorId, @Active, @Deleted, @IsSystemAccount, @SystemName, @LastIpAddress, @CreatedOnUtc, @LastLoginDateUtc, @LastActivityDateUtc, @BillingAddress_Id, @ShippingAddress_Id
    SET @RetNo = SCOPE_IDENTITY()
    END ELSE BEGIN
        IF EXISTS ( SELECT 1 FROM [dbo].[Customer]  WHERE [Id] = @Id ) BEGIN
            UPDATE [dbo].[Customer]
            SET [CustomerGuid] = @CustomerGuid, [Username] = @Username, [Email] = @Email, [Password] = @Password, [PasswordFormatId] = @PasswordFormatId, [PasswordSalt] = @PasswordSalt, [AdminComment] = @AdminComment, [IsTaxExempt] = @IsTaxExempt, [AffiliateId] = @AffiliateId, [VendorId] = @VendorId, [Active] = @Active, [Deleted] = @Deleted, [IsSystemAccount] = @IsSystemAccount, [SystemName] = @SystemName, [LastIpAddress] = @LastIpAddress, [CreatedOnUtc] = @CreatedOnUtc, [LastLoginDateUtc] = @LastLoginDateUtc, [LastActivityDateUtc] = @LastActivityDateUtc, [BillingAddress_Id] = @BillingAddress_Id, [ShippingAddress_Id] = @ShippingAddress_Id
            WHERE  [Id] = @Id
            SET @RetNo = @Id
        END
    END
    IF @RetNo <> 0 BEGIN
        IF NOT EXISTS ( SELECT 1 FROM [dbo].[CustomerAddresses]  WHERE [Customer_Id] = @RetNo AND [Address_Id] = @BillingAddress_Id ) BEGIN
            INSERT INTO [dbo].[CustomerAddresses] ([Customer_Id], [Address_Id])
            SELECT @RetNo, @BillingAddress_Id
        END
        IF @CustomerRole_Id <> 0 BEGIN
            IF NOT EXISTS ( SELECT 1 FROM [dbo].[Customer_CustomerRole_Mapping]  WHERE [CustomerRole_Id] = @CustomerRole_Id AND [Customer_Id] = @RetNo ) BEGIN
                INSERT INTO [dbo].[Customer_CustomerRole_Mapping] ([Customer_Id], [CustomerRole_Id])
                SELECT @RetNo, @CustomerRole_Id
            END
            IF NOT EXISTS ( SELECT 1 FROM [dbo].[Customer_CustomerRole_Mapping]  WHERE [CustomerRole_Id] = 3 AND [Customer_Id] = @RetNo ) BEGIN
                INSERT INTO [dbo].[Customer_CustomerRole_Mapping] ([Customer_Id], [CustomerRole_Id])
                SELECT @RetNo, 3
            END
        END
    END

    COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_CUSTDEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_CUSTDEL] 
END 
GO
CREATE PROC [dbo].[CESP_CUSTDEL] 

    @Id int

AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

BEGIN TRAN

DELETE
FROM   [dbo].[Customer]
WHERE  [Id] = @Id

COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_ITEMSEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_ITEMSEL] 
END 
GO
CREATE PROC [dbo].[CESP_ITEMSEL] 
    @LastDate datetime
AS 
  SET NOCOUNT ON 
  SET XACT_ABORT ON  

  BEGIN TRAN

  SELECT [Id], ProductTypeId, ParentGroupedProductId, VisibleIndividually, Name, ShortDescription, FullDescription, AdminComment, ProductTemplateId, VendorId, ShowOnHomePage, MetaKeywords, MetaDescription, MetaTitle, AllowCustomerReviews, ApprovedRatingSum, NotApprovedRatingSum, ApprovedTotalReviews, NotApprovedTotalReviews, SubjectToAcl, LimitedToStores, Sku, ManufacturerPartNumber, Gtin, IsGiftCard, GiftCardTypeId, RequireOtherProducts, RequiredProductIds, AutomaticallyAddRequiredProducts, IsDownload, DownloadId, UnlimitedDownloads, MaxNumberOfDownloads, DownloadExpirationDays, DownloadActivationTypeId, HasSampleDownload, SampleDownloadId, HasUserAgreement, UserAgreementText, IsRecurring, RecurringCycleLength, RecurringCyclePeriodId, RecurringTotalCycles, IsShipEnabled, IsFreeShipping, AdditionalShippingCharge, DeliveryDateId, WarehouseId, IsTaxExempt, TaxCategoryId, ManageInventoryMethodId, StockQuantity, DisplayStockAvailability, DisplayStockQuantity, MinStockQuantity, LowStockActivityId, NotifyAdminForQuantityBelow, BackorderModeId, AllowBackInStockSubscriptions, OrderMinimumQuantity, OrderMaximumQuantity, AllowedQuantities, DisableBuyButton, DisableWishlistButton, AvailableForPreOrder, CallForPrice, Price, OldPrice, ProductCost, SpecialPrice, SpecialPriceStartDateTimeUtc, SpecialPriceEndDateTimeUtc, CustomerEntersPrice, MinimumCustomerEnteredPrice, MaximumCustomerEnteredPrice, HasTierPrices, HasDiscountsApplied, Weight, Length, Width, Height, AvailableStartDateTimeUtc, AvailableEndDateTimeUtc, DisplayOrder, Published, Deleted, CreatedOnUtc, UpdatedOnUtc
  FROM [dbo].[Product] 
  WHERE  ([UpdatedOnUtc] > @LastDate)

  COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_ITEMPROC]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_ITEMPROC] 
END 
GO
CREATE PROC [dbo].[CESP_ITEMPROC] 
    @Id int,
    @ProductTypeId int,
    @ParentGroupedProductId int,
    @VisibleIndividually bit,
    @Name nvarchar(400),
    @ShortDescription nvarchar(MAX),
    @FullDescription nvarchar(MAX),
    @AdminComment nvarchar(MAX),
    @ProductTemplateId int,
    @VendorId int,
    @ShowOnHomePage bit,
    @MetaKeywords nvarchar(400),
    @MetaDescription nvarchar(MAX),
    @MetaTitle nvarchar(400),
    @AllowCustomerReviews bit,
    @ApprovedRatingSum int,
    @NotApprovedRatingSum int,
    @ApprovedTotalReviews int,
    @NotApprovedTotalReviews int,
    @SubjectToAcl bit,
    @LimitedToStores bit,
    @Sku nvarchar(400),
    @ManufacturerPartNumber nvarchar(400),
    @Gtin nvarchar(400),
    @IsGiftCard bit,
    @GiftCardTypeId int,
    @RequireOtherProducts bit,
    @RequiredProductIds NVarChar,
    @AutomaticallyAddRequiredProducts bit,
    @IsDownload bit,
    @DownloadId int,
    @UnlimitedDownloads bit,
    @MaxNumberOfDownloads int,
    @DownloadExpirationDays int,
    @DownloadActivationTypeId int,
    @HasSampleDownload bit,
    @SampleDownloadId int,
    @HasUserAgreement bit,
    @UserAgreementText nvarchar(MAX),
    @IsRecurring bit,
    @RecurringCycleLength int,
    @RecurringCyclePeriodId int,
    @RecurringTotalCycles int,
    @IsShipEnabled bit,
    @IsFreeShipping bit,
    @AdditionalShippingCharge decimal(18, 4),
    @DeliveryDateId int,
    @WarehouseId int,
    @IsTaxExempt bit,
    @TaxCategoryId int,
    @ManageInventoryMethodId int,
    @StockQuantity int,
    @DisplayStockAvailability bit,
    @DisplayStockQuantity bit,
    @MinStockQuantity int,
    @LowStockActivityId int,
    @NotifyAdminForQuantityBelow int,
    @BackorderModeId int,
    @AllowBackInStockSubscriptions bit,
    @OrderMinimumQuantity int,
    @OrderMaximumQuantity int,
    @AllowedQuantities NVarChar,
    @DisableBuyButton bit,
    @DisableWishlistButton bit,
    @AvailableForPreOrder bit,
    @CallForPrice bit,
    @Price decimal(18, 4),
    @OldPrice decimal(18, 4),
    @ProductCost decimal(18, 4),
    @SpecialPrice decimal(18, 4),
    @SpecialPriceStartDateTimeUtc datetime,
    @SpecialPriceEndDateTimeUtc datetime,
    @CustomerEntersPrice bit,
    @MinimumCustomerEnteredPrice decimal(18, 4),
    @MaximumCustomerEnteredPrice decimal(18, 4),
    @HasTierPrices bit,
    @HasDiscountsApplied bit,
    @Weight decimal(18, 4),
    @Length decimal(18, 4),
    @Width decimal(18, 4),
    @Height decimal(18, 4),
    @AvailableStartDateTimeUtc datetime,
    @AvailableEndDateTimeUtc datetime,
    @DisplayOrder int,
    @Published bit,
    @Deleted bit,
    @CreatedOnUtc datetime,
    @UpdatedOnUtc datetime,
    @CategoryId int,
    @IsFeaturedProduct bit,
@RetNo int out
AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

    BEGIN TRAN
    IF @Id IS NULL OR @Id = 0 BEGIN
        INSERT INTO [dbo].[Product] ([ProductTypeId], [ParentGroupedProductId], [VisibleIndividually], [Name], [ShortDescription], [FullDescription], [AdminComment], [ProductTemplateId], [VendorId], [ShowOnHomePage], [MetaKeywords], [MetaDescription], [MetaTitle], [AllowCustomerReviews], [ApprovedRatingSum], [NotApprovedRatingSum], [ApprovedTotalReviews], [NotApprovedTotalReviews], [SubjectToAcl], [LimitedToStores], [Sku], [ManufacturerPartNumber], [Gtin], [IsGiftCard], [GiftCardTypeId], [RequireOtherProducts], [RequiredProductIds], [AutomaticallyAddRequiredProducts], [IsDownload], [DownloadId], [UnlimitedDownloads], [MaxNumberOfDownloads], [DownloadExpirationDays], [DownloadActivationTypeId], [HasSampleDownload], [SampleDownloadId], [HasUserAgreement], [UserAgreementText], [IsRecurring], [RecurringCycleLength], [RecurringCyclePeriodId], [RecurringTotalCycles], [IsShipEnabled], [IsFreeShipping], [AdditionalShippingCharge], [DeliveryDateId], [WarehouseId], [IsTaxExempt], [TaxCategoryId], [ManageInventoryMethodId], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity], [MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [BackorderModeId], [AllowBackInStockSubscriptions], [OrderMinimumQuantity], [OrderMaximumQuantity], [AllowedQuantities], [DisableBuyButton], [DisableWishlistButton], [AvailableForPreOrder], [CallForPrice], [Price], [OldPrice], [ProductCost], [SpecialPrice], [SpecialPriceStartDateTimeUtc], [SpecialPriceEndDateTimeUtc], [CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [HasTierPrices], [HasDiscountsApplied], [Weight], [Length], [Width], [Height], [AvailableStartDateTimeUtc], [AvailableEndDateTimeUtc], [DisplayOrder], [Published], [Deleted], [CreatedOnUtc], [UpdatedOnUtc])
        SELECT @ProductTypeId, @ParentGroupedProductId, @VisibleIndividually, @Name, @ShortDescription, @FullDescription, @AdminComment, @ProductTemplateId, @VendorId, @ShowOnHomePage, @MetaKeywords, @MetaDescription, @MetaTitle, @AllowCustomerReviews, @ApprovedRatingSum, @NotApprovedRatingSum, @ApprovedTotalReviews, @NotApprovedTotalReviews, @SubjectToAcl, @LimitedToStores, @Sku, @ManufacturerPartNumber, @Gtin, @IsGiftCard, @GiftCardTypeId, @RequireOtherProducts, @RequiredProductIds, @AutomaticallyAddRequiredProducts, @IsDownload, @DownloadId, @UnlimitedDownloads, @MaxNumberOfDownloads, @DownloadExpirationDays, @DownloadActivationTypeId, @HasSampleDownload, @SampleDownloadId, @HasUserAgreement, @UserAgreementText, @IsRecurring, @RecurringCycleLength, @RecurringCyclePeriodId, @RecurringTotalCycles, @IsShipEnabled, @IsFreeShipping, @AdditionalShippingCharge, @DeliveryDateId, @WarehouseId, @IsTaxExempt, @TaxCategoryId, @ManageInventoryMethodId, @StockQuantity, @DisplayStockAvailability, @DisplayStockQuantity, @MinStockQuantity, @LowStockActivityId, @NotifyAdminForQuantityBelow, @BackorderModeId, @AllowBackInStockSubscriptions, @OrderMinimumQuantity, @OrderMaximumQuantity, @AllowedQuantities, @DisableBuyButton, @DisableWishlistButton, @AvailableForPreOrder, @CallForPrice, @Price, @OldPrice, @ProductCost, @SpecialPrice, @SpecialPriceStartDateTimeUtc, @SpecialPriceEndDateTimeUtc, @CustomerEntersPrice, @MinimumCustomerEnteredPrice, @MaximumCustomerEnteredPrice, @HasTierPrices, @HasDiscountsApplied, @Weight, @Length, @Width, @Height, @AvailableStartDateTimeUtc, @AvailableEndDateTimeUtc, @DisplayOrder, @Published, @Deleted, @CreatedOnUtc, @UpdatedOnUtc
    SET @RetNo = SCOPE_IDENTITY()
    END ELSE BEGIN
        IF EXISTS ( SELECT 1 FROM [dbo].[Product]  WHERE [Id] = @Id ) BEGIN
            UPDATE [dbo].[Product]
            SET [ProductTypeId] = @ProductTypeId, [ParentGroupedProductId] = @ParentGroupedProductId, [VisibleIndividually] = @VisibleIndividually, [Name] = @Name, [ShortDescription] = @ShortDescription, [FullDescription] = @FullDescription, [AdminComment] = @AdminComment, [ProductTemplateId] = @ProductTemplateId, [VendorId] = @VendorId, [ShowOnHomePage] = @ShowOnHomePage, [MetaKeywords] = @MetaKeywords, [MetaDescription] = @MetaDescription, [MetaTitle] = @MetaTitle, [AllowCustomerReviews] = @AllowCustomerReviews, [ApprovedRatingSum] = @ApprovedRatingSum, [NotApprovedRatingSum] = @NotApprovedRatingSum, [ApprovedTotalReviews] = @ApprovedTotalReviews, [NotApprovedTotalReviews] = @NotApprovedTotalReviews, [SubjectToAcl] = @SubjectToAcl, [LimitedToStores] = @LimitedToStores, [Sku] = @Sku, [ManufacturerPartNumber] = @ManufacturerPartNumber, [Gtin] = @Gtin, [IsGiftCard] = @IsGiftCard, [GiftCardTypeId] = @GiftCardTypeId, [RequireOtherProducts] = @RequireOtherProducts, [RequiredProductIds] = @RequiredProductIds, [AutomaticallyAddRequiredProducts] = @AutomaticallyAddRequiredProducts, [IsDownload] = @IsDownload, [DownloadId] = @DownloadId, [UnlimitedDownloads] = @UnlimitedDownloads, [MaxNumberOfDownloads] = @MaxNumberOfDownloads, [DownloadExpirationDays] = @DownloadExpirationDays, [DownloadActivationTypeId] = @DownloadActivationTypeId, [HasSampleDownload] = @HasSampleDownload, [SampleDownloadId] = @SampleDownloadId, [HasUserAgreement] = @HasUserAgreement, [UserAgreementText] = @UserAgreementText, [IsRecurring] = @IsRecurring, [RecurringCycleLength] = @RecurringCycleLength, [RecurringCyclePeriodId] = @RecurringCyclePeriodId, [RecurringTotalCycles] = @RecurringTotalCycles, [IsShipEnabled] = @IsShipEnabled, [IsFreeShipping] = @IsFreeShipping, [AdditionalShippingCharge] = @AdditionalShippingCharge, [DeliveryDateId] = @DeliveryDateId, [WarehouseId] = @WarehouseId, [IsTaxExempt] = @IsTaxExempt, [TaxCategoryId] = @TaxCategoryId, [ManageInventoryMethodId] = @ManageInventoryMethodId, [StockQuantity] = @StockQuantity, [DisplayStockAvailability] = @DisplayStockAvailability, [DisplayStockQuantity] = @DisplayStockQuantity, [MinStockQuantity] = @MinStockQuantity, [LowStockActivityId] = @LowStockActivityId, [NotifyAdminForQuantityBelow] = @NotifyAdminForQuantityBelow, [BackorderModeId] = @BackorderModeId, [AllowBackInStockSubscriptions] = @AllowBackInStockSubscriptions, [OrderMinimumQuantity] = @OrderMinimumQuantity, [OrderMaximumQuantity] = @OrderMaximumQuantity, [AllowedQuantities] = @AllowedQuantities, [DisableBuyButton] = @DisableBuyButton, [DisableWishlistButton] = @DisableWishlistButton, [AvailableForPreOrder] = @AvailableForPreOrder, [CallForPrice] = @CallForPrice, [Price] = @Price, [OldPrice] = @OldPrice, [ProductCost] = @ProductCost, [SpecialPrice] = @SpecialPrice, [SpecialPriceStartDateTimeUtc] = @SpecialPriceStartDateTimeUtc, [SpecialPriceEndDateTimeUtc] = @SpecialPriceEndDateTimeUtc, [CustomerEntersPrice] = @CustomerEntersPrice, [MinimumCustomerEnteredPrice] = @MinimumCustomerEnteredPrice, [MaximumCustomerEnteredPrice] = @MaximumCustomerEnteredPrice, [HasTierPrices] = @HasTierPrices, [HasDiscountsApplied] = @HasDiscountsApplied, [Weight] = @Weight, [Length] = @Length, [Width] = @Width, [Height] = @Height, [AvailableStartDateTimeUtc] = @AvailableStartDateTimeUtc, [AvailableEndDateTimeUtc] = @AvailableEndDateTimeUtc, [DisplayOrder] = @DisplayOrder, [Published] = @Published, [Deleted] = @Deleted, [CreatedOnUtc] = @CreatedOnUtc, [UpdatedOnUtc] = @UpdatedOnUtc
            WHERE  [Id] = @Id
            SET @RetNo = @Id
        END
    END
    IF @CategoryId <> 0 BEGIN
        IF NOT EXISTS ( SELECT 1 FROM [dbo].[Product_Category_Mapping]  WHERE [CategoryId] = @CategoryId AND [ProductId] = @RetNo ) BEGIN
        INSERT INTO [dbo].[Product_Category_Mapping] ([ProductId], [CategoryId], [IsFeaturedProduct], [DisplayOrder])
        SELECT @RetNo, @CategoryId, @IsFeaturedProduct, 1
        END
    END
    IF @RetNo <> 0 BEGIN
        IF NOT EXISTS ( SELECT 1 FROM [dbo].[URLRecord]  WHERE [EntityId] = @RetNo AND [EntityName] = 'Product' ) BEGIN
        INSERT INTO [dbo].[URLRecord] ([EntityId], [EntityName], [Slug], [IsActive], [LanguageId])
        SELECT @RetNo, 'Product', Replace(Replace(Replace(@Name,'&',''),' ',''),'.',''), 1, 0 
        END
    END

    COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_ITEMDEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_ITEMDEL] 
END 
GO
CREATE PROC [dbo].[CESP_ITEMDEL] 

    @Id int

AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

BEGIN TRAN

DELETE
FROM   [dbo].[Product]
WHERE  [Id] = @Id

COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_PRICEPROC]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_PRICEPROC] 
END 
GO
CREATE PROC [dbo].[CESP_PRICEPROC] 
    @Id int,
    @ProductId int,
    @StoreId int,
    @CustomerRoleId int,
    @Quantity int,
    @Price decimal(18, 4),
@RetNo int out
AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

    BEGIN TRAN
    IF @Id IS NULL OR @Id = 0 BEGIN
        INSERT INTO [dbo].[TierPrice] ([ProductId], [StoreId], [CustomerRoleId], [Quantity], [Price])
        SELECT @ProductId, @StoreId, @CustomerRoleId, @Quantity, @Price
    SET @RetNo = SCOPE_IDENTITY()
    END ELSE BEGIN
        IF EXISTS ( SELECT 1 FROM [dbo].[TierPrice]  WHERE [Id] = @Id ) BEGIN
            UPDATE [dbo].[TierPrice]
            SET [ProductId] = @ProductId, [StoreId] = @StoreId, [CustomerRoleId] = @CustomerRoleId, [Quantity] = @Quantity, [Price] = @Price
            WHERE  [Id] = @Id
            SET @RetNo = @Id
        END
    END

    COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_PRICEDEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_PRICEDEL] 
END 
GO
CREATE PROC [dbo].[CESP_PRICEDEL] 

    @Id int

AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

BEGIN TRAN

DELETE
FROM   [dbo].[TierPrice]
WHERE  [Id] = @Id

COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_SADDRSEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_SADDRSEL] 
END 
GO
CREATE PROC [dbo].[CESP_SADDRSEL] 
    @DocNo integer
AS 
  SET NOCOUNT ON 
  SET XACT_ABORT ON  

  BEGIN TRAN

  SELECT [Id], FirstName, LastName, Email, Company, CountryId, StateProvinceId, City, Address1, Address2, ZipPostalCode, PhoneNumber, FaxNumber, CreatedOnUtc
  FROM [dbo].[Address] 
  WHERE  ([Id] = @DocNo)

  COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_SADDRPROC]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_SADDRPROC] 
END 
GO
CREATE PROC [dbo].[CESP_SADDRPROC] 
    @Id int,
    @FirstName nvarchar(MAX),
    @LastName nvarchar(MAX),
    @Email nvarchar(MAX),
    @Company nvarchar(MAX),
    @CountryId int,
    @StateProvinceId int,
    @City nvarchar(MAX),
    @Address1 nvarchar(MAX),
    @Address2 nvarchar(MAX),
    @ZipPostalCode nvarchar(MAX),
    @PhoneNumber nvarchar(MAX),
    @FaxNumber nvarchar(MAX),
    @CreatedOnUtc datetime,
    @Customer_Id int,
@RetNo int out
AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

    BEGIN TRAN
    IF @Id IS NULL OR @Id = 0 BEGIN
        INSERT INTO [dbo].[Address] ([FirstName], [LastName], [Email], [Company], [CountryId], [StateProvinceId], [City], [Address1], [Address2], [ZipPostalCode], [PhoneNumber], [FaxNumber], [CreatedOnUtc])
        SELECT @FirstName, @LastName, @Email, @Company, @CountryId, @StateProvinceId, @City, @Address1, @Address2, @ZipPostalCode, @PhoneNumber, @FaxNumber, @CreatedOnUtc
    SET @RetNo = SCOPE_IDENTITY()
    END ELSE BEGIN
        IF EXISTS ( SELECT 1 FROM [dbo].[Address]  WHERE [Id] = @Id ) BEGIN
            UPDATE [dbo].[Address]
            SET [FirstName] = @FirstName, [LastName] = @LastName, [Email] = @Email, [Company] = @Company, [CountryId] = @CountryId, [StateProvinceId] = @StateProvinceId, [City] = @City, [Address1] = @Address1, [Address2] = @Address2, [ZipPostalCode] = @ZipPostalCode, [PhoneNumber] = @PhoneNumber, [FaxNumber] = @FaxNumber, [CreatedOnUtc] = @CreatedOnUtc
            WHERE  [Id] = @Id
            SET @RetNo = @Id
        END
    END
    IF @Customer_Id <> 0 AND @RetNo <> 0 BEGIN
        IF NOT EXISTS ( SELECT 1 FROM [dbo].[CustomerAddresses]  WHERE [Customer_Id] = @Customer_Id AND [Address_Id] = @RetNo ) BEGIN
        INSERT INTO [dbo].[CustomerAddresses] ([Customer_Id], [Address_Id])
        SELECT @Customer_Id, @RetNo
        END
    END

    COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_SADDRDEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_SADDRDEL] 
END 
GO
CREATE PROC [dbo].[CESP_SADDRDEL] 

    @Id int

AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

BEGIN TRAN

DELETE
FROM   [dbo].[Address]
WHERE  [Id] = @Id

COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_SHSEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_SHSEL] 
END 
GO
CREATE PROC [dbo].[CESP_SHSEL] 
    @LastDate datetime
AS 
  SET NOCOUNT ON 
  SET XACT_ABORT ON  

  BEGIN TRAN

  SELECT [dbo].[Order].[Id], [dbo].[Order].OrderGuid, [dbo].[Order].StoreId, [dbo].[Order].CustomerId, [dbo].[Order].BillingAddressId, [dbo].[Order].ShippingAddressId, [dbo].[Order].OrderStatusId, [dbo].[Order].ShippingStatusId, [dbo].[Order].PaymentStatusId, [dbo].[Order].PaymentMethodSystemName, [dbo].[Order].CustomerCurrencyCode, [dbo].[Order].CurrencyRate, [dbo].[Order].CustomerTaxDisplayTypeId, [dbo].[Order].VatNumber, [dbo].[Order].OrderSubtotalInclTax, [dbo].[Order].OrderSubtotalExclTax, [dbo].[Order].OrderSubTotalDiscountInclTax, [dbo].[Order].OrderSubTotalDiscountExclTax, [dbo].[Order].OrderShippingInclTax, [dbo].[Order].OrderShippingExclTax, [dbo].[Order].PaymentMethodAdditionalFeeInclTax, [dbo].[Order].PaymentMethodAdditionalFeeExclTax, [dbo].[Order].TaxRates, [dbo].[Order].OrderTax, [dbo].[Order].OrderDiscount, [dbo].[Order].OrderTotal, [dbo].[Order].RefundedAmount, [dbo].[Order].RewardPointsWereAdded, [dbo].[Order].CheckoutAttributeDescription, [dbo].[Order].CheckoutAttributesXml, [dbo].[Order].CustomerLanguageId, [dbo].[Order].AffiliateId, [dbo].[Order].CustomerIp, [dbo].[Order].AllowStoringCreditCardNumber, [dbo].[Order].CardType, [dbo].[Order].CardName, [dbo].[Order].CardNumber, [dbo].[Order].MaskedCreditCardNumber, [dbo].[Order].CardCvv2, [dbo].[Order].CardExpirationMonth, [dbo].[Order].CardExpirationYear, [dbo].[Order].AuthorizationTransactionId, [dbo].[Order].AuthorizationTransactionCode, [dbo].[Order].AuthorizationTransactionResult, [dbo].[Order].CaptureTransactionId, [dbo].[Order].CaptureTransactionResult, [dbo].[Order].SubscriptionTransactionId, [dbo].[Order].PurchaseOrderNumber, [dbo].[Order].PaidDateUtc, [dbo].[Order].ShippingMethod, [dbo].[Order].ShippingRateComputationMethodSystemName, [dbo].[Order].Deleted, [dbo].[Order].CreatedOnUtc,[dbo].[Address].FirstName, [dbo].[Address].LastName, [dbo].[Address].Email, [dbo].[Address].Company, [dbo].[Address].CountryId, [dbo].[Address].StateProvinceId, [dbo].[Address].City, [dbo].[Address].Address1, [dbo].[Address].Address2, [dbo].[Address].ZipPostalCode, [dbo].[Address].PhoneNumber, [dbo].[Address].FaxNumber, [dbo].[Address].CreatedOnUtc
  FROM [dbo].[Order] 
  LEFT JOIN Address
  ON [Order].BillingAddressId = Address.Id
  WHERE ([dbo].[Order].[CreatedOnUtc] > @LastDate)

  COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_SHPROC]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_SHPROC] 
END 
GO
CREATE PROC [dbo].[CESP_SHPROC] 
    @Id int,
    @OrderGuid uniqueidentifier,
    @StoreId int,
    @CustomerId int,
    @BillingAddressId int,
    @ShippingAddressId int,
    @OrderStatusId int,
    @ShippingStatusId int,
    @PaymentStatusId int,
    @PaymentMethodSystemName nvarchar(MAX),
    @CustomerCurrencyCode nvarchar(MAX),
    @CurrencyRate decimal(18, 8),
    @CustomerTaxDisplayTypeId int,
    @VatNumber nvarchar(MAX),
    @OrderSubtotalInclTax decimal(18, 4),
    @OrderSubtotalExclTax decimal(18, 4),
    @OrderSubTotalDiscountInclTax decimal(18, 4),
    @OrderSubTotalDiscountExclTax decimal(18, 4),
    @OrderShippingInclTax decimal(18, 4),
    @OrderShippingExclTax decimal(18, 4),
    @PaymentMethodAdditionalFeeInclTax decimal(18, 4),
    @PaymentMethodAdditionalFeeExclTax decimal(18, 4),
    @TaxRates nvarchar(MAX),
    @OrderTax decimal(18, 4),
    @OrderDiscount decimal(18, 4),
    @OrderTotal decimal(18, 4),
    @RefundedAmount decimal(18, 4),
    @RewardPointsWereAdded bit,
    @CheckoutAttributeDescription nvarchar(MAX),
    @CheckoutAttributesXml nvarchar(MAX),
    @CustomerLanguageId int,
    @AffiliateId int,
    @CustomerIp nvarchar(MAX),
    @AllowStoringCreditCardNumber bit,
    @CardType nvarchar(MAX),
    @CardName nvarchar(MAX),
    @CardNumber nvarchar(MAX),
    @MaskedCreditCardNumber nvarchar(MAX),
    @CardCvv2 nvarchar(MAX),
    @CardExpirationMonth nvarchar(MAX),
    @CardExpirationYear nvarchar(MAX),
    @AuthorizationTransactionId nvarchar(MAX),
    @AuthorizationTransactionCode nvarchar(MAX),
    @AuthorizationTransactionResult nvarchar(MAX),
    @CaptureTransactionId nvarchar(MAX),
    @CaptureTransactionResult nvarchar(MAX),
    @SubscriptionTransactionId nvarchar(MAX),
    @PurchaseOrderNumber nvarchar(MAX),
    @PaidDateUtc datetime,
    @ShippingMethod nvarchar(MAX),
    @ShippingRateComputationMethodSystemName nvarchar(MAX),
    @Deleted bit,
    @CreatedOnUtc datetime,
    @FirstName nvarchar(MAX),
    @LastName nvarchar(MAX),
    @Email nvarchar(MAX),
    @Company nvarchar(MAX),
    @CountryId int,
    @StateProvinceId int,
    @City nvarchar(MAX),
    @Address1 nvarchar(MAX),
    @Address2 nvarchar(MAX),
    @ZipPostalCode nvarchar(MAX),
    @PhoneNumber nvarchar(MAX),
    @FaxNumber nvarchar(MAX),
@RetNo int out
AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

    BEGIN TRAN
        IF NOT EXISTS ( SELECT 1 FROM [dbo].[Address]  WHERE [Address1] = @Address1 ) BEGIN
            INSERT INTO [dbo].[Address] ([FirstName], [LastName], [Email], [Company], [CountryId], [StateProvinceId], [City], [Address1], [Address2], [ZipPostalCode], [PhoneNumber], [FaxNumber], [CreatedOnUtc])
            SELECT @FirstName, @LastName, @Email, @Company, @CountryId, @StateProvinceId, @City, @Address1, @Address2, @ZipPostalCode, @PhoneNumber, @FaxNumber, @CreatedOnUtc
            SET @BillingAddressId = SCOPE_IDENTITY()
        END ELSE BEGIN
            SELECT @BillingAddressId = [dbo].[Address].Id
            FROM Address
            WHERE [Address1] = @Address1
        END
    IF @Id IS NULL OR @Id = 0 BEGIN
        INSERT INTO [dbo].[Order] ([OrderGuid], [StoreId], [CustomerId], [BillingAddressId], [ShippingAddressId], [OrderStatusId], [ShippingStatusId], [PaymentStatusId], [PaymentMethodSystemName], [CustomerCurrencyCode], [CurrencyRate], [CustomerTaxDisplayTypeId], [VatNumber], [OrderSubtotalInclTax], [OrderSubtotalExclTax], [OrderSubTotalDiscountInclTax], [OrderSubTotalDiscountExclTax], [OrderShippingInclTax], [OrderShippingExclTax], [PaymentMethodAdditionalFeeInclTax], [PaymentMethodAdditionalFeeExclTax], [TaxRates], [OrderTax], [OrderDiscount], [OrderTotal], [RefundedAmount], [RewardPointsWereAdded], [CheckoutAttributeDescription], [CheckoutAttributesXml], [CustomerLanguageId], [AffiliateId], [CustomerIp], [AllowStoringCreditCardNumber], [CardType], [CardName], [CardNumber], [MaskedCreditCardNumber], [CardCvv2], [CardExpirationMonth], [CardExpirationYear], [AuthorizationTransactionId], [AuthorizationTransactionCode], [AuthorizationTransactionResult], [CaptureTransactionId], [CaptureTransactionResult], [SubscriptionTransactionId], [PurchaseOrderNumber], [PaidDateUtc], [ShippingMethod], [ShippingRateComputationMethodSystemName], [Deleted], [CreatedOnUtc])
        SELECT @OrderGuid, @StoreId, @CustomerId, @BillingAddressId, @ShippingAddressId, @OrderStatusId, @ShippingStatusId, @PaymentStatusId, @PaymentMethodSystemName, @CustomerCurrencyCode, @CurrencyRate, @CustomerTaxDisplayTypeId, @VatNumber, @OrderSubtotalInclTax - @OrderShippingInclTax, @OrderSubtotalExclTax - @OrderShippingInclTax, @OrderSubTotalDiscountInclTax, @OrderSubTotalDiscountExclTax, @OrderShippingInclTax, @OrderShippingInclTax, @PaymentMethodAdditionalFeeInclTax, @PaymentMethodAdditionalFeeExclTax, @TaxRates, @OrderSubtotalInclTax - @OrderSubtotalExclTax, @OrderDiscount, @OrderSubtotalInclTax, @RefundedAmount, @RewardPointsWereAdded, @CheckoutAttributeDescription, @CheckoutAttributesXml, @CustomerLanguageId, @AffiliateId, @CustomerIp, @AllowStoringCreditCardNumber, @CardType, @CardName, @CardNumber, @MaskedCreditCardNumber, @CardCvv2, @CardExpirationMonth, @CardExpirationYear, @AuthorizationTransactionId, @AuthorizationTransactionCode, @AuthorizationTransactionResult, @CaptureTransactionId, @CaptureTransactionResult, @SubscriptionTransactionId, @PurchaseOrderNumber, @PaidDateUtc, @ShippingMethod, @ShippingRateComputationMethodSystemName, @Deleted, @CreatedOnUtc
    SET @RetNo = SCOPE_IDENTITY()
    END ELSE BEGIN
        IF EXISTS ( SELECT 1 FROM [dbo].[Order]  WHERE [Id] = @Id ) BEGIN
            UPDATE [dbo].[Order]
            SET  [OrderGuid] = @OrderGuid, [StoreId] = @StoreId, [CustomerId] = @CustomerId, [BillingAddressId] = @BillingAddressId, [ShippingAddressId] = @ShippingAddressId, [OrderStatusId] = @OrderStatusId, [ShippingStatusId] = @ShippingStatusId, [PaymentStatusId] = @PaymentStatusId, [PaymentMethodSystemName] = @PaymentMethodSystemName, [CustomerCurrencyCode] = @CustomerCurrencyCode, [CurrencyRate] = @CurrencyRate, [CustomerTaxDisplayTypeId] = @CustomerTaxDisplayTypeId, [VatNumber] = @VatNumber, [OrderSubtotalInclTax] = @OrderSubtotalInclTax - OrderShippingInclTax, [OrderSubtotalExclTax] = @OrderSubtotalExclTax - OrderShippingInclTax, [OrderSubTotalDiscountInclTax] = @OrderSubTotalDiscountInclTax, [OrderSubTotalDiscountExclTax] = @OrderSubTotalDiscountExclTax, [OrderShippingInclTax] = @OrderShippingInclTax, [OrderShippingExclTax] = @OrderShippingInclTax, [PaymentMethodAdditionalFeeInclTax] = @PaymentMethodAdditionalFeeInclTax, [PaymentMethodAdditionalFeeExclTax] = @PaymentMethodAdditionalFeeExclTax, [TaxRates] = @TaxRates, [OrderTax] = @OrderSubtotalInclTax - OrderSubtotalExclTax, [OrderDiscount] = @OrderDiscount, [OrderTotal] = @OrderSubtotalInclTax, [RefundedAmount] = @RefundedAmount, [RewardPointsWereAdded] = @RewardPointsWereAdded, [CheckoutAttributeDescription] = @CheckoutAttributeDescription, [CheckoutAttributesXml] = @CheckoutAttributesXml, [CustomerLanguageId] = @CustomerLanguageId, [AffiliateId] = @AffiliateId, [CustomerIp] = @CustomerIp, [AllowStoringCreditCardNumber] = @AllowStoringCreditCardNumber, [CardType] = @CardType, [CardName] = @CardName, [CardNumber] = @CardNumber, [MaskedCreditCardNumber] = @MaskedCreditCardNumber, [CardCvv2] = @CardCvv2, [CardExpirationMonth] = @CardExpirationMonth, [CardExpirationYear] = @CardExpirationYear, [AuthorizationTransactionId] = @AuthorizationTransactionId, [AuthorizationTransactionCode] = @AuthorizationTransactionCode, [AuthorizationTransactionResult] = @AuthorizationTransactionResult, [CaptureTransactionId] = @CaptureTransactionId, [CaptureTransactionResult] = @CaptureTransactionResult, [SubscriptionTransactionId] = @SubscriptionTransactionId, [PurchaseOrderNumber] = @PurchaseOrderNumber, [PaidDateUtc] = @PaidDateUtc, [ShippingMethod] = @ShippingMethod, [ShippingRateComputationMethodSystemName] = @ShippingRateComputationMethodSystemName, [Deleted] = @Deleted, [CreatedOnUtc] = @CreatedOnUtc
            WHERE  [Id] = @Id
            SET @RetNo = @Id
        END
    END

    COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_SHDEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_SHDEL] 
END 
GO
CREATE PROC [dbo].[CESP_SHDEL] 

    @Id int

AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

BEGIN TRAN

DELETE
FROM   [dbo].[Order]
WHERE  [Id] = @Id

COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_SHLSEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_SHLSEL] 
END 
GO
CREATE PROC [dbo].[CESP_SHLSEL] 
    @DocNo integer
AS 
  SET NOCOUNT ON 
  SET XACT_ABORT ON  

  BEGIN TRAN

  SELECT [Id], OrderItemGuid, OrderId, ProductId, Quantity, UnitPriceInclTax, UnitPriceExclTax, PriceInclTax, PriceExclTax, DiscountAmountInclTax, DiscountAmountExclTax, OriginalProductCost, AttributeDescription, AttributesXml, DownloadCount, IsDownloadActivated, LicenseDownloadId, ItemWeight, ExpiryDate
  FROM [dbo].[OrderItem] 
  WHERE  ([OrderId] = @DocNo)

  COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_SHLPROC]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_SHLPROC] 
END 
GO
CREATE PROC [dbo].[CESP_SHLPROC] 
    @Id int,
    @OrderItemGuid uniqueidentifier,
    @OrderId int,
    @ProductId int,
    @Quantity int,
    @UnitPriceInclTax decimal(18, 4),
    @UnitPriceExclTax decimal(18, 4),
    @PriceInclTax decimal(18, 4),
    @PriceExclTax decimal(18, 4),
    @DiscountAmountInclTax decimal(18, 4),
    @DiscountAmountExclTax decimal(18, 4),
    @OriginalProductCost decimal(18, 4),
    @AttributeDescription nvarchar(MAX),
    @AttributesXml nvarchar(MAX),
    @DownloadCount int,
    @IsDownloadActivated bit,
    @LicenseDownloadId int,
    @ItemWeight decimal(18, 4),
    @ExpiryDate datetime,
@RetNo int out
AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

    BEGIN TRAN
    IF @Id IS NULL OR @Id = 0 BEGIN
        INSERT INTO [dbo].[OrderItem] ([OrderItemGuid], [OrderId], [ProductId], [Quantity], [UnitPriceInclTax], [UnitPriceExclTax], [PriceInclTax], [PriceExclTax], [DiscountAmountInclTax], [DiscountAmountExclTax], [OriginalProductCost], [AttributeDescription], [AttributesXml], [DownloadCount], [IsDownloadActivated], [LicenseDownloadId], [ItemWeight], [ExpiryDate])
        SELECT @OrderItemGuid, @OrderId, @ProductId, @Quantity, @UnitPriceInclTax, @UnitPriceExclTax, @PriceInclTax, @PriceExclTax, @DiscountAmountInclTax, @DiscountAmountExclTax, @OriginalProductCost, @AttributeDescription, @AttributesXml, @DownloadCount, @IsDownloadActivated, @LicenseDownloadId, @ItemWeight, @ExpiryDate
    SET @RetNo = SCOPE_IDENTITY()
    END ELSE BEGIN
        IF EXISTS ( SELECT 1 FROM [dbo].[OrderItem]  WHERE [Id] = @Id ) BEGIN
            UPDATE [dbo].[OrderItem]
            SET  [OrderItemGuid] = @OrderItemGuid, [OrderId] = @OrderId, [ProductId] = @ProductId, [Quantity] = @Quantity, [UnitPriceInclTax] = @UnitPriceInclTax, [UnitPriceExclTax] = @UnitPriceExclTax, [PriceInclTax] = @PriceInclTax, [PriceExclTax] = @PriceExclTax, [DiscountAmountInclTax] = @DiscountAmountInclTax, [DiscountAmountExclTax] = @DiscountAmountExclTax, [OriginalProductCost] = @OriginalProductCost, [AttributeDescription] = @AttributeDescription, [AttributesXml] = @AttributesXml, [DownloadCount] = @DownloadCount, [IsDownloadActivated] = @IsDownloadActivated, [LicenseDownloadId] = @LicenseDownloadId, [ItemWeight] = @ItemWeight, [ExpiryDate] = @ExpiryDate
            WHERE  [Id] = @Id
            SET @RetNo = @Id
        END
    END

    COMMIT
GO
IF OBJECT_ID('[dbo].[CESP_SHLDEL]') IS NOT NULL
BEGIN 
    DROP PROC [dbo].[CESP_SHLDEL] 
END 
GO
CREATE PROC [dbo].[CESP_SHLDEL] 

    @Id int

AS 
SET NOCOUNT ON 
SET XACT_ABORT ON  

BEGIN TRAN

DELETE
FROM   [dbo].[OrderItem]
WHERE  [Id] = @Id

COMMIT
GO
