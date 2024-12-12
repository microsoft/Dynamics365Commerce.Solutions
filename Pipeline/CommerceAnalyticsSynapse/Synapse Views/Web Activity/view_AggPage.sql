CREATE OR ALTER VIEW [dbo].[view_AggPage] AS
	SELECT * 
	FROM
		OPENROWSET
		( 
			BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/AggPage/*.parquet',
			FORMAT='parquet'
		)
		WITH
		(
			[dateId]			int,
			[clientUtcDate]		datetime2(7),
			[clientUtcHour]		int,
			[clientUtcMinute]	int,
			[channelId]			bigint,
			[environmentId]		varchar(200),
			[clientVersion]		varchar(200),
			[eventType]			varchar(200),
			[pageDomain]		varchar(200),
			[pageUriStem]		varchar(500),
			[pageType]			varchar(200),
			[pageName]			varchar(200),
			[pageLocale]		varchar(200),
			[browserLocale]		varchar(200),
			[utmSource]			varchar(200),
			[utmMedium]			varchar(200),
			[utmCampaign]		varchar(200),
			[utmTerm]			varchar(200),
			[utmContent]		varchar(200),
			[origin]			varchar(200),
			[isSignedIn]		bit,
			[isCookieConsent]	bit,
			[isNewUser]			bit,
			[uaPlatform]		varchar(200),
			[uaManufacturer]	varchar(200),
			[uaDevice]			varchar(200),
			[uaOsName]			varchar(200),
			[uaBrowserName]		varchar(200),
			[ripContinent]		varchar(200),
			[ripCountry]		varchar(200),
			[ripStateProvince]	varchar(200),
			[ripCity]			varchar(200),
			[clientSize]		varchar(200),
			[screenSize]		varchar(200),
			[referrerType]		varchar(200),
			[referrerDomain]	varchar(200),
			[referrerUriStem]	varchar(500),
			[targetType]		varchar(200),
			[targetDomain]		varchar(200),
			[targetUriStem]		varchar(500),
			[productId]			varchar(200),
			[productName]		varchar(500),
			[pageViewCount]		bigint,
			[pageActionCount]	bigint
		) AS AggPage