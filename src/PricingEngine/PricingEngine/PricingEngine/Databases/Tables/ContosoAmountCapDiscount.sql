/**
 * SAMPLE CODE NOTICE
 * 
 * THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED,
 * OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.
 * THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.
 * NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
 */
IF (SELECT OBJECT_ID('[ext].[CONTOSOAMOUNTCAPDISCOUNT]')) IS NULL 
BEGIN
    CREATE TABLE [ext].[CONTOSOAMOUNTCAPDISCOUNT](
        [AMOUNTCAP] [numeric](32, 16) NULL,
        [APPLYBASEREDUCTION] [int] NOT NULL,
        [DATAAREAID] [nvarchar](4) NOT NULL,
        [RECID] [bigint] NOT NULL,
        CONSTRAINT [I_CONTOSODISCOUNTAMOUNTCAP_RECID] PRIMARY KEY CLUSTERED ([RECID] ASC)
    );
END
GO

GRANT INSERT, DELETE, UPDATE, SELECT ON OBJECT::[ext].[CONTOSOAMOUNTCAPDISCOUNT] TO [DataSyncUsersRole];
GO