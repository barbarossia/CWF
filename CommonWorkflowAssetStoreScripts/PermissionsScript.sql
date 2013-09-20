-- ==========================================================================================
-- This script assigns the network service accounts of the front end web servers to access
-- cwf marketplace service database and perform data access operations needed by the cwf 
-- marketplace web service API.
-- IMPORTANT: Please follow instructions given in VARIABLE INITIALIATION section below.
-- ==========================================================================================
USE CWFMarketplaceService;
DECLARE @WebServerNames VARCHAR(MAX), @MarketplaceServiceDBName VARCHAR(100), @DomainName VARCHAR(100);
SET NOCOUNT ON;

 
-- ================VARIABLE INITIALIZATION [BEGIN]==========================================
-- Replace the comma separated list of web server names with real web server names to which
-- you intend to grant permission. Do not leave any spaces around the commas.
-- These web servers are where you host the cwf marketplace service.
SET @WebServerNames = 'WebServerName1,WebServerName2';

-- The cwf marketplace service database is not expected to change.  If you have selected a different 
-- name than CWFMarketplaceService, provide that value here.
SET @MarketplaceServiceDBName = 'CWFMarketplaceService';

-- Set the appropriate domain name where the web servers are hosted.
SET @DomainName = 'REDMOND';
-- ================VARIABLE INITIALIZATION [END] ============================================

 
 
-- ================CREATE WEB SERVICE DATABASE ROLE [BEGIN]==================================
PRINT 'MarketplaceService role - dropping'
DECLARE @RoleName sysname
SET @RoleName = N'MarketplaceService'
IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = @RoleName AND type = 'R')
Begin
    DECLARE @RoleMemberName sysname
    DECLARE Member_Cursor CURSOR FOR
    select [name]
    from sys.database_principals 
    where principal_id in ( 
        select member_principal_id 
        from sys.database_role_members 
        where role_principal_id in (
            select principal_id
            FROM sys.database_principals where [name] = @RoleName  AND type = 'R' ))

    OPEN Member_Cursor;

    FETCH NEXT FROM Member_Cursor
    into @RoleMemberName

    WHILE @@FETCH_STATUS = 0
    BEGIN

        exec sp_droprolemember @rolename=@RoleName, @membername= @RoleMemberName

        FETCH NEXT FROM Member_Cursor
        into @RoleMemberName
    END;

    CLOSE Member_Cursor;
    DEALLOCATE Member_Cursor;
End

IF  EXISTS (SELECT * FROM sys.database_principals WHERE name = N'MarketplaceService' AND type = 'R')
DROP ROLE [MarketplaceService]
PRINT 'MarketplaceService role - dropped'
PRINT '--------------------------------------'

PRINT 'MarketplaceService role - creating'
CREATE ROLE [MarketplaceService] AUTHORIZATION [dbo]
PRINT 'MarketplaceService role - created'
PRINT '--------------------------------------'
-- ================CREATE WEB SERVICE DATABASE ROLE [END]====================================

 
 
-- ================GRANT PERMISSION TO WEB SERVICE ROLE [BEGIN]==============================
PRINT 'Permission to [MarketplaceService] role - granting'

GRANT EXECUTE ON [dbo].[Activity_ChangeAuthor] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Copy] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Delete] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Exists] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_GetByActivityLibrary] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_GetByName] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Move] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_OverrideLock] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_Search] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_SetLock] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Activity_UpdateLock] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityCategory_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityCategory_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibrary_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibrary_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibrary_GetMissing] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibrary_RecursionCheck] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibraryDependency_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibraryDependency_CreateOrUpdateListHead] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ActivityLibraryDependency_GetTree] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Application_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[AuthorizationGroup_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[AuthorizationGroup_EnableOrDisable] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[AuthorizationGroup_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Error_Handle] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Error_Raise] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[Marketplace_Search] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[Marketplace_SearchAllItems] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[Marketplace_SearchExecutableItems] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[Marketplace_SearchXamlItems] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[Marketplace_GetAssetDetails] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[Permission_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[StatusCode_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[TableRow_DeleteById] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[TableRow_UpdateSoftDelete] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[TaskActivity_CreateOrUpdate] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[TaskActivity_Get] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[TaskActivity_Search] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[TaskActivity_UpdateStatus] to [MarketplaceService];
GRANT EXECUTE ON [dbo].[ValidateEnvironmentMove] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[ValidateSPPermission] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[WorkflowType_CreateOrUpdate] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[WorkflowType_Get] TO [MarketplaceService];
GRANT EXECUTE ON [dbo].[WorkflowType_Search] TO [MarketplaceService];


GRANT EXECUTE ON TYPE::[dbo].[ActivityLibraryTableType] to [MarketplaceService];
GRANT EXECUTE ON TYPE::[dbo].[AuthGroupNameTableType] to [MarketplaceService];
GRANT EXECUTE ON TYPE::[dbo].[EnvironmentTableType] to [MarketplaceService];





-- GRANT CONNECT  TO [MarketplaceService];
PRINT 'Permission to [MarketplaceService] role - granted'
PRINT '--------------------------------------'
-- ================GRANT PERMISSION TO WEB SERVICE ROLE [END]================================

 
 
-- ================ADD NETWORK SERVICE ACCOUNTS TO WEB SERVICE ROLE [BEGIN]==================
DECLARE @WebServerNamesXml XML;
SET @WebServerNamesXml = CAST('<Servers><Server>' + REPLACE(@WebServerNames,  ',' , '</Server><Server>') + '</Server></Servers>' AS XML);

CREATE TABLE #WebServers
(
    ID INT IDENTITY (1,1),
    ServerName VARCHAR(100)
)

;WITH Temp AS
( 
    SELECT 
        @WebServerNamesXml AS Names 
) 
INSERT INTO #WebServers
SELECT 
    Split.a.value('.', 'VARCHAR(100)') AS Names 
FROM Temp 
CROSS APPLY Names.nodes('/Servers/Server') Split(a)

DECLARE @Count INT, @Index INT;
SELECT @Count = COUNT(ID) FROM #WebServers;

SET @Index = 1;
DECLARE @Command VARCHAR(MAX);
WHILE @Index <= @Count
BEGIN
    DECLARE @MachineAccount VARCHAR(MAX);
    DECLARE @ServerName VARCHAR(100);
    SELECT @ServerName = ServerName FROM #WebServers WHERE ID = @Index
    SET @MachineAccount = @DomainName + '\' + @ServerName + '$';
    
    IF EXISTS (SELECT * FROM sys.server_principals WHERE name = @MachineAccount)
    BEGIN
        PRINT 'Login for ' + @MachineAccount + ' - dropping';
        SET @Command = 'DROP LOGIN [' + @MachineAccount + ']';
        EXEC (@Command);
        PRINT 'Login for ' + @MachineAccount + ' - dropped';
        PRINT '--------------------------------------'
    END

    PRINT 'Login for ' + @MachineAccount + ' - creating';
    SET @Command = 'CREATE LOGIN [' + @MachineAccount + '] FROM WINDOWS WITH DEFAULT_DATABASE=[' + @MarketplaceServiceDBName + '], DEFAULT_LANGUAGE=[us_english]';
    EXEC (@Command);
    PRINT 'Login for ' + @MachineAccount + ' - created';
    PRINT '--------------------------------------'

    IF EXISTS (SELECT * FROM sys.database_principals WHERE name = @MachineAccount)
    BEGIN
        PRINT 'User for login ' + @MachineAccount + ' - dropping';
        SET @Command = 'DROP USER [' + @MachineAccount + ']';
        EXEC (@Command);
        PRINT 'User for login ' + @MachineAccount + ' - dropped';
        PRINT '--------------------------------------'
    END

    PRINT 'User for login ' + @MachineAccount + ' - creating';
    SET @Command = 'CREATE USER [' + @MachineAccount + '] FOR LOGIN [' + @MachineAccount + '] WITH DEFAULT_SCHEMA=[dbo]';
    EXEC (@Command);
    PRINT 'User for login ' + @MachineAccount + ' - created';
    PRINT '--------------------------------------'  
    
    PRINT 'Login ' + @MachineAccount + ' - adding to MarketplaceService role';
    EXEC sp_addrolemember @rolename = 'MarketplaceService', @membername = @MachineAccount    
    PRINT 'Login ' + @MachineAccount + ' - added to MarketplaceService role';
    PRINT '--------------------------------------'
    SET @Index = @Index + 1;
END
DROP TABLE #WebServers;
-- ================ADD NETWORK SERVICE ACCOUNTS TO WEB SERVICE ROLE [END]====================
SET NOCOUNT OFF;
PRINT 'Add done.';

