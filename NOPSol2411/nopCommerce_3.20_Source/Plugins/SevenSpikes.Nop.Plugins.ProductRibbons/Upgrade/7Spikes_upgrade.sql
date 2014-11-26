IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Product_Tab_Mapping]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'Product_Tab_Mapping', 'SS_QT_Product_Tab_Mapping'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Carousel3DSettings]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'Carousel3DSettings', 'SS_AS_Carousel3DSettings'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CarouselSettings]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'CarouselSettings', 'SS_AS_CarouselSettings'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[NivoSettings]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'NivoSettings', 'SS_AS_NivoSettings'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SliderImage]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'SliderImage', 'SS_AS_SliderImage'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SliderWidget]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'SliderWidget', 'SS_AS_SliderWidget'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SliderCategory]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'SliderCategory', 'SS_AS_SliderCategory'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SliderManufacturer]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'SliderManufacturer', 'SS_AS_SliderManufacturer'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[AnywhereSlider]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'AnywhereSlider', 'SS_AS_AnywhereSlider'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CustomerReminderMessageRecord]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'CustomerReminderMessageRecord', 'SS_CR_CustomerReminderMessageRecord'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ReminderExcludedCustomer]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'ReminderExcludedCustomer', 'SS_CR_ReminderExcludedCustomer'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Reminder]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'Reminder', 'SS_CR_Reminder'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[JCarouselWidget]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'JCarouselWidget', 'SS_JC_JCarouselWidget'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[JCarouselProduct]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'JCarouselProduct', 'SS_JC_JCarouselProduct'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[JCarouselCategory]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'JCarouselCategory', 'SS_JC_JCarouselCategory'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[JCarouselManufacturer]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'JCarouselManufacturer', 'SS_JC_JCarouselManufacturer'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[JCarousel]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'JCarousel', 'SS_JC_JCarousel'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Product_Tab_Mapping]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'Product_Tab_Mapping', 'SS_QT_Product_Tab_Mapping'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Category_Tab_Mapping]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'Category_Tab_Mapping', 'SS_QT_Category_Tab_Mapping'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Manufacturer_Tab_Mapping]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'Manufacturer_Tab_Mapping', 'SS_QT_Manufacturer_Tab_Mapping'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Tab]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'Tab', 'SS_QT_Tab'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CategoryPageRibbon]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'CategoryPageRibbon', 'SS_PR_CategoryPageRibbon'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductPageRibbon]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'ProductPageRibbon', 'SS_PR_ProductPageRibbon'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductRibbon]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'ProductRibbon', 'SS_PR_ProductRibbon'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[RibbonPicture]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'RibbonPicture', 'SS_PR_RibbonPicture'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ConditionStatement]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'ConditionStatement', 'SS_PR_ConditionStatement'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ConditionGroup]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'ConditionGroup', 'SS_PR_ConditionGroup'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ProductOverride]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'ProductOverride', 'SS_PR_ProductOverride'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Condition]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'Condition', 'SS_PR_Condition'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SEOCategory]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'SEOCategory', 'SS_SS_SEOCategory'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SEOManufacturer]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'SEOManufacturer', 'SS_SS_SEOManufacturer'

	END
	
GO	

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[SeoTemplate]') AND type in (N'U'))

	BEGIN
	
		exec sp_rename 'SeoTemplate', 'SS_SS_SeoTemplate'

	END
	
GO