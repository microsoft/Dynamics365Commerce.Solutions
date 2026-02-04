CREATE
or ALTER VIEW [dbo].[view_DimCustomerRFML] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimCustomerRFML/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[CustomerId] 				bigint,
		[Company] 				nvarchar(4),
		[CustomerAccount] 			nvarchar(20),
		[PartyId] 				bigint,
		[PartyNumber] 				nvarchar(40),
		[PartyName] 				nvarchar(100),
		[CreatedDate] 				date,
		[DaysSinceCreated] 			bigint,
		[RetailCustomerId] 			bigint,
		[IsRetailCustomer] 			bigint,
		[CompanyId] 				bigint,
		[FirstSaleDate] 			date,
		[DaysSinceFirstSale] 			bigint,
		[LastSaleDate] 				date,
		[DaysSinceLastSale] 			bigint,
		[LifetimeValue] 			numeric(32, 6),
		[AverageTransactionValue] 		numeric(32, 6),
		[SalesTransactionCount] 		bigint,
		[SalesLineCount] 			bigint,
		[RecencyBand] 				int,
		[RecencyString] 			nvarchar(100),
		[LifetimeValueBand] 			int,
		[LifetimeValueString] 			nvarchar(100),
		[RelationshipLengthBand] 		int,
		[RelationshipLengthString] 		nvarchar(100),
		[FrequencyBand] 			int,
		[FrequencyString] 			nvarchar(100),
		[SysProcessingDateTime] 		datetime2,
		[SysIsDeleted] 				int
	) AS [DimCustomerRFML]