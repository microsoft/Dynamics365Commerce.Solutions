CREATE
or ALTER VIEW [dbo].[view_DimPeriodicDiscount] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimPeriodicDiscount/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[PeriodicDiscountId] 		bigint,
		[Company] 			nvarchar(4),
		[Discount] 			nvarchar(20),
		[DiscountName] 			nvarchar(120),
		[EffectiveDate] 		date,
		[ExpirationDate] 		date,
		[DiscountType] 			nvarchar(50),
		[Status] 			nvarchar(50),
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimPeriodicDiscount]