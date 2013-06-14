
SET ANSI_DEFAULTS ON
SET CURSOR_CLOSE_ON_COMMIT OFF
SET IMPLICIT_TRANSACTIONS OFF
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET NUMERIC_ROUNDABORT OFF
SET QUOTED_IDENTIFIER ON

SET DATEFORMAT ymd
SET LOCK_TIMEOUT -1
SET NOCOUNT ON
SET ROWCOUNT 0
SET TEXTSIZE 0
GO

/****************************************************************************
** File: ArgusUserRoleDev.sql
**
** Desc: This script creates the [ArgusUserRoleDev] database role.
**
** Used Where/By:
**    Database installation processes.
**
** History:
**
** Date     Name              Description
** -------- ----------------- -----------------------------------------------
** 
*****************************************************************************/


/*
** Declare and initialize SQL script "parameters".
*/

DECLARE
    @DebugBitmask                   [integer]

SET @DebugBitmask                   = 0x00000000   -- 0xFFFFFFFF = All debug bits enabled


/*
** Declare variables and initialize as needed.
*/

-- Generic variables

DECLARE
    @Msg                            [nvarchar](4000)
   ,@Error                          [integer]
   ,@RowCount                       [integer]
   ,@ReturnCode                     [integer]
   ,@LocalTransaction               [bit]
   ,@MaxStringLength                [integer]

DECLARE
    @ModuleName                     [sysname]
   ,@ModuleStartTime                [datetime]
   ,@ModuleFinishTime               [datetime]
   ,@ModuleElapsedMinutes           [integer]
   ,@ModuleElapsedSeconds           [integer]

-- Application variables

DECLARE
    @DBRoleName                     [sysname]
   ,@DBUserName                     [varchar] (255)
   ,@DBLogin                        [varchar] (255)
   ,@DBSchema                       [varchar] (255)
   ,@DBNAME                         [nvarchar](255)


/*
** Parameter and variable initialization.
*/

-- Generic variables

SET @Msg                = N''
SET @Error              = 0
SET @RowCount           = 0
SET @ReturnCode         = 0
SET @LocalTransaction   = 0
SET @DebugBitmask       = ISNULL(@DebugBitmask, 0x00000000) -- 0xFFFFFFFF = All debug bits enabled
SET @MaxStringLength    = 4000
SET @ModuleName         = N'ArgusUserRoleDev.sql'
SET @ModuleStartTime    = GETDATE()

-- Application variables

-- Display initial start messages

IF ( @DebugBitmask & 0x00000002 <> 0x00000000 )
BEGIN
   SET @Msg = CONVERT([nvarchar](40), @ModuleStartTime, 109)
   RAISERROR(N'%s [%s] Beginning execution...', 0, 1, @Msg, @ModuleName) WITH NOWAIT
END

IF ( @DebugBitmask & 0x00000004 <> 0x00000000 )
BEGIN
   RAISERROR(N'%s [%s] Parameter information:', 0, 1, @Msg, @ModuleName) WITH NOWAIT

   RAISERROR(N'  "@DebugBitmask" parameter value:       0x%X', 0, 1, @DebugBitmask) WITH NOWAIT
END

/*
** Parameter validation.
*/

/* 
** USER setup section
** 1 - add SQL Server login
** 2 - add Database Login
** 3 - add login to Database Role
*/

/*
** Redmond\argtsvc --- SECTION
*/


/*
** Add SQL Server login Redmond\argtsvc windows account.
*/

SET @DBUserName = 'argtscv'
SET @DBLogin    = 'Redmond\argtscv'
SET @DBSchema   = '[dbo]'
SET @DBRoleName = ''
SET @DBNAME     = DB_NAME()

RAISERROR(N'', 0, 1) WITH NOWAIT

IF ( EXISTS (  SELECT TOP 1 1
               FROM master.sys.syslogins
               WHERE [loginname] = @DBLogin) )
BEGIN
   RAISERROR(N'SQL Server Login [%s] already exists.', 0, 1, @DBLogin)
END
ELSE
BEGIN
   RAISERROR(N'Creating SQL Server Login [%s]...', 0, 1, @DBLogin);

   EXECUTE @ReturnCode = sp_grantlogin @DBLogin

   IF ( @ReturnCode <> 0 )
   BEGIN
      RAISERROR
         (N'%s: Could not add SQL Server Login [%s] (because [%s] returned an error).'
         ,10, 127, @ModuleName, @DBLogin, N'sp_grantlogin'
         ) WITH NOWAIT
      GOTO ErrorExitFromModule
   END
END

/*
** Add the database userlogin Redmond\argtscv service account.
*/

SET @DBUserName = 'argtscv'
SET @DBLogin    = 'Redmond\argtscv'
SET @DBSchema   = '[dbo]'
SET @DBRoleName = ''
SET @DBNAME     = DB_NAME()

RAISERROR(N'', 0, 1) WITH NOWAIT

IF ( EXISTS (  SELECT TOP 1 1
               FROM   sys.database_principals  as u
               JOIN   master.sys.syslogins as sl
               ON     sl.sid       = u.sid
               WHERE  sl.loginname = @DBLogin
             ) )
BEGIN
   RAISERROR(N'Database UserLogin [%s] already exists.', 0, 1, @DBUserName)
END
ELSE
BEGIN
   RAISERROR(N'Creating Database UserLogin [%s]...', 0, 1, @DBUserName);

   CREATE USER [argtsvc] FOR LOGIN [Redmond\argtsvc] WITH DEFAULT_SCHEMA = dbo

   IF ( @@ERROR <> 0 )
   BEGIN
      RAISERROR
         (N'%s: Could not add database UserLogin [%s] (because [%s] returned an error).'
         ,10, 127, @ModuleName, @DBUserName, N'CREATE USER'
         ) WITH NOWAIT
      GOTO ErrorExitFromModule
   END
END


/*
** Add the database userlogin to role.
*/

SET @DBUserName = 'argtscv'
SET @DBLogin    = 'Redmond\argtscv'
SET @DBSchema   = '[dbo]'
SET @DBRoleName = N'ArgusAdministrator'
SET @DBNAME     = DB_NAME()

RAISERROR(N'', 0, 1) WITH NOWAIT

IF ( EXISTS (  SELECT TOP 1 1
               FROM   sys.database_principals u
               JOIN   master.sys.syslogins as sl
               ON     sl.sid                = u.sid
               JOIN   sys.database_role_members m
               ON     m.member_principal_id = u.principal_id
               JOIN   sys.database_principals g
               ON     g.name                = @DBRoleName
               AND    g.principal_id        = m.role_principal_id
               WHERE  sl.loginname          = @DBLogin
) )
BEGIN
   RAISERROR(N'Database UserLogin [%s] already in role [%s].', 0, 1, @DBUserName, @DBRoleName)
END
ELSE
BEGIN
   -- this is a precaution in-case someone else created the user and named it differently then this script
   SELECT TOP 1 
          @DBUserName = u.name
   FROM   sys.database_principals u
   JOIN   master.sys.syslogins as sl
   ON     sl.sid                = u.sid
   WHERE  sl.loginname          = @DBLogin
               
   RAISERROR(N'Adding UserLogin [%s] to database role [%s] .', 0, 1, @DBUserName, @DBRoleName)

   EXECUTE @ReturnCode = [dbo].[sp_addrolemember]
                                        @rolename  =  @DBRoleName
                                       ,@membername =  @DBUserName

   IF ( @ReturnCode <> 0 )
   BEGIN
      RAISERROR
        (N'Could not add UserLogin [%s] to database role [%s] (because [%s] returned an error).'
         ,10, 127, @DBUserName, @DBRoleName, N'sp_addrolemember'
         ) WITH NOWAIT
      GOTO ErrorExitFromModule
   END
END

/*
** Add the database userlogin Redmond\argtsvc service account.
*/

SET @DBUserName = 'argtscv'
SET @DBLogin    = 'Redmond\argtscv'
SET @DBSchema   = '[dbo]'
SET @DBRoleName = N'ArgusAdministrator'
SET @DBNAME     = DB_NAME()

RAISERROR(N'', 0, 1) WITH NOWAIT

IF ( EXISTS (  SELECT TOP 1 1
               FROM   msdb.sys.database_principals  as u
               JOIN   master.sys.syslogins as sl
               ON     sl.sid       = u.sid
               WHERE  sl.loginname = @DBLogin
             ) )
               
BEGIN
   RAISERROR(N'Database UserLogin [%s] already exists.', 0, 1, @DBUserName)
END
ELSE
BEGIN
   RAISERROR(N'Creating Database UserLogin [%s]...', 0, 1, @DBUserName);

   USE [msdb]
   
   CREATE USER [argtsvc] FOR LOGIN [Redmond\argtsvc] WITH DEFAULT_SCHEMA = dbo

   IF ( @@ERROR <> 0 )
   BEGIN
      RAISERROR
         (N'%s: Could not add database UserLogin [%s] (because [%s] returned an error).'
         ,10, 127, @ModuleName, @DBUserName, N'CREATE USER'
         ) WITH NOWAIT
      GOTO ErrorExitFromModule
   END
   
   USE [Argus]
   
END

/*
** Add the database userlogin to role.
*/

SET @DBUserName = 'argtscv'
SET @DBLogin    = 'Redmond\argtscv'
SET @DBSchema   = '[dbo]'
SET @DBRoleName = N'db_ssisoperator'
SET @DBNAME     = DB_NAME()

RAISERROR(N'', 0, 1) WITH NOWAIT

IF ( EXISTS (  SELECT TOP 1 1
               FROM   msdb.sys.database_principals u
               JOIN   master.sys.syslogins as sl
               ON     sl.sid                = u.sid
               JOIN   msdb.sys.database_role_members m
               ON     m.member_principal_id = u.principal_id
               JOIN   msdb.sys.database_principals g
               ON     g.name                = @DBRoleName
               AND    g.principal_id        = m.role_principal_id
               WHERE  sl.loginname          = @DBLogin
               
) )
BEGIN
   RAISERROR(N'Database UserLogin [%s] already in role [%s].', 0, 1, @DBUserName, @DBRoleName)
END
ELSE
BEGIN
   -- this is a precaution in-case someone else created the user and named it differently then this script
   SELECT TOP 1 
          @DBUserName = u.name
   FROM   msdb.sys.database_principals u
   JOIN   master.sys.syslogins as sl
   ON     sl.sid                = u.sid
   WHERE  sl.loginname          = @DBLogin
               
   RAISERROR(N'Adding UserLogin [%s] to database role [%s] .', 0, 1, @DBUserName, @DBRoleName)

   EXECUTE @ReturnCode = [msdb].[dbo].[sp_addrolemember]
                                                 @rolename  =  @DBRoleName
                                                ,@membername =  @DBUserName

   IF ( @ReturnCode <> 0 )
   BEGIN
      RAISERROR
        (N'Could not add UserLogin [%s] to database role [%s] (because [%s] returned an error).'
         ,10, 127, @DBUserName, @DBRoleName, N'sp_addrolemember'
         ) WITH NOWAIT
      GOTO ErrorExitFromModule
   END
END

/*
** Add the database userlogin to role.
*/

SET @DBUserName = 'argtscv'
SET @DBLogin    = 'Redmond\argtscv'
SET @DBSchema   = '[dbo]'
SET @DBRoleName = N'SQLAgentOperatorRole'
SET @DBNAME     = DB_NAME()

RAISERROR(N'', 0, 1) WITH NOWAIT

IF ( EXISTS (  SELECT TOP 1 1
               FROM   msdb.sys.database_principals u
               JOIN   master.sys.syslogins as sl
               ON     sl.sid                = u.sid
               JOIN   msdb.sys.database_role_members m
               ON     m.member_principal_id = u.principal_id
               JOIN   msdb.sys.database_principals g
               ON     g.name                = @DBRoleName
               AND    g.principal_id        = m.role_principal_id
               WHERE  sl.loginname          = @DBLogin
) )
BEGIN
   RAISERROR(N'Database UserLogin [%s] already in role [%s].', 0, 1, @DBUserName, @DBRoleName)
END
ELSE
BEGIN
   -- this is a precaution in-case someone else created the user and named it differently then this script
   SELECT TOP 1 
          @DBUserName = u.name
   FROM   msdb.sys.database_principals u
   JOIN   master.sys.syslogins as sl
   ON     sl.sid                = u.sid
   WHERE  sl.loginname          = @DBLogin
               
   RAISERROR(N'Adding UserLogin [%s] to database role [%s] .', 0, 1, @DBUserName, @DBRoleName)

   EXECUTE @ReturnCode = [msdb].[dbo].[sp_addrolemember]
                                                 @rolename  =  @DBRoleName
                                                ,@membername =  @DBUserName

   IF ( @ReturnCode <> 0 )
   BEGIN
      RAISERROR
        (N'Could not add UserLogin [%s] to database role [%s] (because [%s] returned an error).'
         ,10, 127, @DBUserName, @DBRoleName, N'sp_addrolemember'
         ) WITH NOWAIT
      GOTO ErrorExitFromModule
   END
END

/*
** Add the database userlogin to role.
*/

SET @DBUserName = 'argtscv'
SET @DBLogin    = 'Redmond\argtscv'
SET @DBSchema   = '[dbo]'
SET @DBRoleName = N'db_datareader'
SET @DBNAME     = DB_NAME()

RAISERROR(N'', 0, 1) WITH NOWAIT

IF ( EXISTS (  SELECT TOP 1 1
               FROM   msdb.sys.database_principals u
               JOIN   master.sys.syslogins as sl
               ON     sl.sid                = u.sid
               JOIN   msdb.sys.database_role_members m
               ON     m.member_principal_id = u.principal_id
               JOIN   msdb.sys.database_principals g
               ON     g.name                = @DBRoleName
               AND    g.principal_id        = m.role_principal_id
               WHERE  sl.loginname          = @DBLogin
) )
BEGIN
   RAISERROR(N'Database UserLogin [%s] already in role [%s].', 0, 1, @DBUserName, @DBRoleName)
END
ELSE
BEGIN
   -- this is a precaution in-case someone else created the user and named it differently then this script
   SELECT TOP 1 
          @DBUserName = u.name
   FROM   msdb.sys.database_principals u
   JOIN   master.sys.syslogins as sl
   ON     sl.sid                = u.sid
   WHERE  sl.loginname          = @DBLogin
               
   RAISERROR(N'Adding UserLogin [%s] to database role [%s] .', 0, 1, @DBUserName, @DBRoleName)

   EXECUTE @ReturnCode = [msdb].[dbo].[sp_addrolemember]
                                                 @rolename  =  @DBRoleName
                                                ,@membername =  @DBUserName

   IF ( @ReturnCode <> 0 )
   BEGIN
      RAISERROR
        (N'Could not add UserLogin [%s] to database role [%s] (because [%s] returned an error).'
         ,10, 127, @DBUserName, @DBRoleName, N'sp_addrolemember'
         ) WITH NOWAIT
      GOTO ErrorExitFromModule
   END
END

/*
** Changing userlogin default database.
*/

SET @DBUserName = 'argtscv'
SET @DBLogin    = 'Redmond\argtscv'
SET @DBSchema   = '[dbo]'
SET @DBRoleName = N'Argus'
SET @DBNAME     = DB_NAME()


RAISERROR(N'', 0, 1) WITH NOWAIT

RAISERROR(N'Changing UserLogin [%s] to default database of [%s] .', 0, 1, @DBLogin, @DBRoleName)

EXECUTE @ReturnCode = [dbo].[sp_defaultdb]
    @loginame  =  @DBLogin
   ,@defdb =  @DBRoleName

IF ( @ReturnCode <> 0 )
BEGIN
   RAISERROR
     (N'Could not change UserLogin [%s] default database to [%s] (because [%s] returned an error).'
      ,10, 127, @DBLogin, @DBRoleName, N'sp_defaultdb'
      ) WITH NOWAIT
   GOTO ErrorExitFromModule
END


GOTO WrapUpModule


/*
** End of module execution.
*/
WrapUpModule:

/* Uncomment if a transaction is needed.
-- Commit the entire named transaction or to the named savepoint
IF ( @LocalTransaction = 1 AND @@TRANCOUNT > 0 )
BEGIN
   COMMIT TRANSACTION @ModuleName
   SET @LocalTransaction = 0
END
*/

-- Display final finish messages

SET @ModuleFinishTime = GETDATE()

IF ( @DebugBitmask & 0x00000001 <> 0x00000000 )
BEGIN
   SET @ModuleElapsedMinutes = DATEDIFF(minute, @ModuleStartTime, @ModuleFinishTime)
   SET @ModuleElapsedSeconds = DATEDIFF(second, @ModuleStartTime, @ModuleFinishTime)

   SET @Msg = CONVERT([nvarchar](40), GETDATE(), 109)
   RAISERROR
      (N'%s [%s] Elapsed Time:  %d minutes (or %d in seconds)'
      ,0, 1, @Msg, @ModuleName, @ModuleElapsedMinutes, @ModuleElapsedSeconds
      ) WITH NOWAIT
END

IF ( @DebugBitmask & 0x00000002 <> 0x00000000 )
BEGIN
   SET @Msg = CONVERT([nvarchar](40), @ModuleFinishTime, 109)
   RAISERROR(N'%s [%s] Finished execution.', 0, 1, @Msg, @ModuleName) WITH NOWAIT
END


GOTO ExitFromModule


ErrorExitFromModule:

/* Uncomment if a transaction is needed.
-- Roll back the entire named transaction or to the named savepoint
IF ( @@TRANCOUNT > 0 )
BEGIN
   ROLLBACK TRANSACTION @ModuleName
END
*/

GOTO ExitFromModule


ExitFromModule:

SELECT ReturnCode = @ReturnCode
GO

