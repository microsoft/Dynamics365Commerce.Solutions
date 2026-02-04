CREATE
or ALTER VIEW [dbo].[view_DimChannel] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimChannel/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[ChannelId] 			bigint,
		[RetailChannel] 		nvarchar(15),
		[RetailChannelName] 		nvarchar(100),
		[ChannelType] 			nvarchar(50),
		[StoreNumber] 			nvarchar(10),
		[StoreArea] 			numeric(32, 6),
		[TimeZoneId] 			bigint,
		[TimeZone] 			nvarchar(60),
		[WarehouseCompany] 		nvarchar(4),
		[Warehouse] 			nvarchar(10),
		[DefaultCustomerCompany] 	nvarchar(4),
		[DefaultCustomer] 		nvarchar(20),
		[Currency] 			nvarchar(3),
		[DefaultDimensionId] 		bigint,
		[ReportingCompany] 		nvarchar(4),
		[CategoryHierarchyId] 		bigint,
		[CategoryHierarchyName] 	nvarchar(128),
		[OrganizationId] 		bigint,
		[OperatingUnitNumber] 		nvarchar(30),
		[AddressId] 			bigint,
		[City] 				nvarchar(100),
		[State] 			nvarchar(100),
		[ZipOrPostalCode] 		nvarchar(100),
		[CountryOrRegion] 		nvarchar(100),
		[CustomerType] 			nvarchar(50),
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimChannel]