CREATE
or ALTER VIEW [dbo].[view_DimCommissionSalesGroup] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimCommissionSalesGroup/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[CommissionSalesGroupId] 	bigint,
		[Company] 			nvarchar(4),
		[SalesGroup] 			nvarchar(10),
		[SalesGroupName] 		nvarchar(60),
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimCommissionSalesGroup]