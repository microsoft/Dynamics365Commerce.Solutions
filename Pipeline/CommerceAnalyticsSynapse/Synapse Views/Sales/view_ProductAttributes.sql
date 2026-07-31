CREATE
or ALTER VIEW [dbo].[view_ProductAttributes] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/ProductAttributes/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[ProductId] 			bigint,
		[ProductNumber] 		nvarchar(100),
		[AttributeGroup] 		nvarchar(100),
		[AttributeName] 		nvarchar(100),
		[AttributeDataType] 		nvarchar(100),
		[AttributeValue] 		nvarchar(2000),
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimProductAttributes]