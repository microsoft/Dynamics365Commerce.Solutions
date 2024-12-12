CREATE
or ALTER VIEW [dbo].[view_DimOrganizationHierarchy] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimOrganizationHierarchy/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[OrganizationHierarchyId] 		bigint,
		[OrganizationId] 			bigint,
		[OrganizationName] 			nvarchar(100),
		[OperatingUnitNumber]			nvarchar(30),
		[OrganiaztionType] 			nvarchar(50),
		[OperatingUnitType] 			nvarchar(50),
		[LegalEntity] 				nvarchar(4),
		[PartyNumber] 				nvarchar(40),
		[HierarchyId] 				bigint,
		[HierarchyName] 			nvarchar(60),
		[IsMainHierarchy] 			int,
		[IsReportingHierarchy] 			int,
		[ChannelId] 				bigint,
		[RetailChannel] 			nvarchar(15),
		[ChannelType] 				nvarchar(50),
		[HierarchyLevel] 			bigint,
		[Level1] 				nvarchar(100),
		[Level2] 				nvarchar(100),
		[Level3] 				nvarchar(100),
		[Level4] 				nvarchar(100),
		[Level5] 				nvarchar(100),
		[Level6] 				nvarchar(100),
		[Level7] 				nvarchar(100),
		[Level8] 				nvarchar(100),
		[Level9] 				nvarchar(100),
		[Level10] 				nvarchar(100),
		[SysProcessingDateTime] 		datetime2,
		[SysIsDeleted] 				int
	) AS [DimOrganizationHierarchy]