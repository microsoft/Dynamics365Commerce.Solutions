CREATE
or ALTER VIEW [dbo].[view_DimCategoryHierarchy] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimCategoryHierarchy/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[CategoryId]				bigint,
		[CategoryName] 				nvarchar(254),
		[Language] 				nvarchar(7),
		[HierarchyId] 				bigint,
		[HierarchyName] 			nvarchar(128),
		[IsMainHierarchy] 			int,
		[IsRetailHierarchy] 			int,
		[IsRetailSpecialGroupHierarchy] 	int,
		[IsRetailVendorProductHierarchy] 	int,
		[IsRetailChannelNavigationHierarchy] 	int,
		[HierarchyLevel] 			bigint,
		[ProductId] 				bigint,
		[ProductNumber] 			nvarchar(70),
		[Level1] 				nvarchar(254),
		[Level2] 				nvarchar(254),
		[Level3] 				nvarchar(254),
		[Level4] 				nvarchar(254),
		[Level5] 				nvarchar(254),
		[Level6] 				nvarchar(254),
		[Level7] 				nvarchar(254),
		[Level8] 				nvarchar(254),
		[Level9] 				nvarchar(254),
		[Level10] 				nvarchar(254),
		[SysProcessingDateTime] 		datetime2,
		[SysIsDeleted] 				int
	) AS [DimCategoryHierarchy]