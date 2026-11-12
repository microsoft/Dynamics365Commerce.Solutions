CREATE
or ALTER VIEW [dbo].[view_DimProductVariant] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimProductVariant/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[ProductVariantId] 		bigint,
		[ProductVariantNumber] 		nvarchar(70),
		[CwProduct] 			bigint,
		[TotalWeight] 			bigint,
		[SearchName] 			nvarchar(40),
		[RecordVersion] 		bigint,
		[Partition] 			bigint,
		[ProductVariantName] 		nvarchar(120),
		[ProductId] 			bigint,
		[ProductNumber] 		nvarchar(70),
		[Color] 			nvarchar(10),
		[Configuration] 		nvarchar(50),
		[Size] 				nvarchar(10),
		[Style] 			nvarchar(10),
		[Language] 			nvarchar(7),
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimProductVariant]