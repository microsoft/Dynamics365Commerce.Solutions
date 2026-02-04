CREATE
or ALTER VIEW [dbo].[view_DimRetailTenderType] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimRetailTenderType/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[RetailTenderTypeId] 		bigint,
		[PaymentMethod] 		nvarchar(10),
		[PaymentMethodName] 		nvarchar(60),
		[DefaultFunction] 		nvarchar(50),
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimTenderType]