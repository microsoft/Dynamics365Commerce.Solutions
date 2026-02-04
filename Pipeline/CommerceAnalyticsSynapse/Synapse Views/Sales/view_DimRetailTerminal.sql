CREATE
or ALTER VIEW [dbo].[view_DimRetailTerminal] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimRetailTerminal/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[RetailTerminalId] 		bigint,
		[RegisterNumber] 		nvarchar(10),
		[RegisterName] 			nvarchar(60),
		[StoredId] 			bigint,
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimRetailTerminal]