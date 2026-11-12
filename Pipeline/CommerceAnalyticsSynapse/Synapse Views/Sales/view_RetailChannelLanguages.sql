CREATE
or ALTER VIEW [dbo].[view_RetailChannelLanguages] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/RetailChannelLanguages/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[ChannelId] 			bigint 1,
		[RetailChannel] 		varchar(15) 2,
		[Language] 			varchar(10) 3,
		[IsDefault] 			int 4,
		[SysProcessingDateTime] 	datetime2 5,
		[SysIsDeleted] 			int 6
	) AS [DimChannel]