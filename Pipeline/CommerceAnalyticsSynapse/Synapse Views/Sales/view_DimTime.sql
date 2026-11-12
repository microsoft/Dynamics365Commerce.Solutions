CREATE
or ALTER VIEW [dbo].[view_DimTime] AS
SELECT *
FROM OPENROWSET (
		BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/DimTime/*.parquet',
		FORMAT = 'parquet'
	) WITH (
		[TimeId] 			bigint,
		[TimeOfDay] 			datetime2,
		[MinuteOfHour] 			nvarchar(2),
		[MinuteOfHourOrder] 		bigint,
		[MinuteOfDay] 			nvarchar(5),
		[MinuteOfDayOrder] 		bigint,
		[Interval] 			nvarchar(5),
		[IntervalOrder] 		bigint,
		[Hour] 				nvarchar(5),
		[HourOrder] 			bigint,
		[SysProcessingDateTime] 	datetime2,
		[SysIsDeleted] 			int
	) AS [DimTime]