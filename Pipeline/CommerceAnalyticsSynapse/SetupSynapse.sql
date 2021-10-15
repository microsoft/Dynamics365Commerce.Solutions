USE master
-- CREATE DATABASES
CREATE DATABASE [CommerceAnalytics]
GO
		 
-- CREATE CREDENTIAL TO ESTABLISH LINK TO STORAGE
-- SAS TOKEN WITHOUT '?' IN THE BEGINNING
CREATE CREDENTIAL [https://placeholder_storageaccount.dfs.core.windows.net/placeholder_container]
WITH IDENTITY='SHARED ACCESS SIGNATURE',
SECRET = 'placeholder_sastoken'
Go

-- CREATE SQL LOGIN FOR REPORT
CREATE LOGIN [reportreadonlyuser] WITH PASSWORD = 'placeholder_password'
GO

-- GRAND CREDENTIAL TO LOGIN
GRANT ALTER ANY CREDENTIAL TO [reportreadonlyuser]
GO

USE [CommerceAnalytics]
-- CREATE USER FROM LOGIN
CREATE USER [reportreadonlyuser] FROM LOGIN [reportreadonlyuser]
GO

-- ADD READER ROLE TO USER
ALTER ROLE db_dataReader ADD MEMBER [reportreadonlyuser]
GO