CREATE
or ALTER VIEW [dbo].[view_DimRetailTenderTypeCard] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimRetailTenderTypeCard/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[RetailTenderTypeCardId] 	bigint,
		[CardIssuer] 			nvarchar(60),
		[CardId] 			nvarchar(10),
		[CardType] 			nvarchar(50),
		[CardTypeName] 			nvarchar(60),
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimTenderTypeCard]