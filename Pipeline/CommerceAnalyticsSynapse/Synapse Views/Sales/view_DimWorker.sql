CREATE
or ALTER VIEW [dbo].[view_DimWorker] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimWorker/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[WorkerId] 			bigint,
		[PersonnelNumber] 		nvarchar(25),
		[PersonId] 			bigint,
		[PersonName] 			nvarchar(100),
		[PartyNumber] 			nvarchar(40),
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimWorker]