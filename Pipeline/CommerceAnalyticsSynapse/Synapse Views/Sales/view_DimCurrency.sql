CREATE
or ALTER VIEW [dbo].[view_DimCurrency] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimCurrency/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[CurrencyId] 			bigint,
		[Currency] 			nvarchar(3),
		[CurrencyName] 			nvarchar(60),
		[IsoCurrencyCode] 		nvarchar(3),
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimCurrency]