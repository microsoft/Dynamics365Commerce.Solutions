CREATE
or ALTER VIEW [dbo].[view_DimLegalEntity] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimLegalEntity/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[CompanyId] 			bigint,
		[Company] 			nvarchar(4),
		[CompanyName] 			nvarchar(100),
		[AccountingCurrency] 		nvarchar(3),
		[ReportingCurrency] 		nvarchar(3),
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimLegalEntity]