CREATE
or ALTER VIEW [dbo].[view_DimRetailLoyaltyCard] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimRetailLoyaltyCard/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[RetailLoyaltyCardId] 		bigint,
		[CardNumber] 			nvarchar(30),
		[CardType] 			nvarchar(50),
		[Reference] 			bigint,
		[PartyId] 			bigint,
		[PartyNumber] 			nvarchar(40),
		[PartyName] 			nvarchar(100),
		[City] 				nvarchar(60),
		[State] 			nvarchar(10),
		[ZipOrPostalCode] 		nvarchar(10),
		[CountryOrRegion] 		nvarchar(10),
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimRetailLoyaltyCard]