CREATE OR ALTER VIEW [dbo].[view_AggSession] AS
	SELECT * 
	FROM
		OPENROWSET
		( 
			BULK N'https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container/placeholder_datarootpath/Ontologies/Commerce/AggSession/*.parquet',
			FORMAT='parquet'
		)
		WITH
		(
			[dateId]							int,
			[clientUtcDate]						datetime2(7),
			[channelId]							bigint,
			[environmentId]						varchar(200),
			[pageDomain]						varchar(200),
			[pageType]							varchar(200),
			[pageLocale]						varchar(200),
			[browserLocale]						varchar(200),
			[utmSource]							varchar(200),
			[utmMedium]							varchar(200),
			[utmCampaign]						varchar(200),
			[utmTerm]							varchar(200),
			[utmContent]						varchar(200),
			[origin]							varchar(200),
			[isSignedIn]						bit,
			[isCookieConsent]					bit,
			[isNewUser]							bit,
			[uaPlatform]						varchar(200),
			[uaManufacturer]					varchar(200),
			[uaDevice]							varchar(200),
			[uaOsName]							varchar(200),
			[uaBrowserName]						varchar(200),
			[ripContinent]						varchar(200),
			[ripCountry]						varchar(200),
			[ripStateProvince]					varchar(200),
			[ripCity]							varchar(200),
			[screenSize]						varchar(200),
			[referrerType]						varchar(200),
			[referrerDomain]					varchar(200),
			[referrerUriStem]					varchar(200),
			[entryPageType]						varchar(200),
			[exitPageType]						varchar(200),
			[exitProductId]						varchar(200),
			[exitProductName]					varchar(200),
			[sessionCount]						bigint,
			[convertedSessionCount]				bigint,
			[bounceVisitsCount]					bigint,
			[sessionDuration]					float,
			[totalEventCount]					bigint,
			[pageViewCount]						bigint,
			[pageContentUpdateCount]			bigint,
			[pageActionCount]					bigint,
			[acquisitionEventCount]				bigint,
			[customEventCount]					bigint,
			[impressionCount]					bigint,
			[orderCount]						bigint,
			[sessionOnlineRevenue]				float
		) AS AggSession