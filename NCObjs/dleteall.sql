USE [NOP]
GO

DELETE FROM [ComponentItem];
DELETE FROM [Order];
DELETE FROM OrderItem;

DELETE FROM Category;
DBCC CHECKIDENT('dbo.Category', RESEED,0)
DELETE FROM Product;
DBCC CHECKIDENT('dbo.Product', RESEED,0)
DELETE FROM [TaxCategory] where Id <> 1;
UPDATE [dbo].[TaxCategory]
SET  [Name] = 'AllProducts'
WHERE  [Id] = 1
INSERT INTO [dbo].[TaxRate] ([StoreId], [TaxCategoryId], [CountryId], [StateProvinceId], [Zip], [Percentage])
SELECT 1, 1, 6, 1, null, 10;

DELETE FROM Customer_CustomerRole_Mapping where Customer_Id <> 1;
DELETE FROM CustomerRole where IsSystemRole = 0;
DELETE FROM CustomerAddresses where Customer_Id <> 1;
DELETE FROM Customer where Id <> 1;
DELETE FROM [Address] where Id <> 1;
DELETE FROM Product_Category_Mapping;
DELETE FROM Product_Manufacturer_Mapping;
DELETE FROM Product_Picture_Mapping;
DELETE FROM TierPrice;
DELETE FROM UrlRecord;

DELETE FROM [StateProvince] where Id <> 1;
UPDATE [dbo].[StateProvince]
SET  [CountryId] = 6, [Name] = 'Australian Capital Territory', [Abbreviation] = 'ACT', [Published] = 'true', [DisplayOrder] = 1
WHERE  [Id] = 1;
DBCC CHECKIDENT('dbo.StateProvince', RESEED,1)
INSERT INTO [dbo].[StateProvince] ([CountryId], [Name], [Abbreviation], [Published], [DisplayOrder])
SELECT 6, 'New South Wales', 'NSW', 'true', 1;
INSERT INTO [dbo].[StateProvince] ([CountryId], [Name], [Abbreviation], [Published], [DisplayOrder])
SELECT 6, 'Queensland', 'QLD', 'true', 1;
INSERT INTO [dbo].[StateProvince] ([CountryId], [Name], [Abbreviation], [Published], [DisplayOrder])
SELECT 6, 'Victoria', 'VIC', 'true', 1;
INSERT INTO [dbo].[StateProvince] ([CountryId], [Name], [Abbreviation], [Published], [DisplayOrder])
SELECT 6, 'Tasmania', 'TAS', 'true', 1;
INSERT INTO [dbo].[StateProvince] ([CountryId], [Name], [Abbreviation], [Published], [DisplayOrder])
SELECT 6, 'Northern Territory', 'NT', 'true', 1;
INSERT INTO [dbo].[StateProvince] ([CountryId], [Name], [Abbreviation], [Published], [DisplayOrder])
SELECT 6, 'South Australia', 'SA', 'true', 1;
INSERT INTO [dbo].[StateProvince] ([CountryId], [Name], [Abbreviation], [Published], [DisplayOrder])
SELECT 6, 'Western Australia', 'WA', 'true', 1;