/****** Object:  StoredProcedure [dbo].[ValidateSPPermission]    Script Date: 05/19/2013 22:34:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ValidateSPPermission]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ValidateSPPermission]
GO


CREATE PROCEDURE [dbo].[ValidateSPPermission]
@InSPName   [nvarchar](100),
@InAuthGroupName [dbo].[AuthGroupNameTableType] READONLY ,
@InEnvironments	[dbo].[EnvironmentTableType] READONLY,
@OutErrorString nvarchar (300)OUTPUT
AS
BEGIN
    SET NOCOUNT ON

    DECLARE 
           @rc                [int]
          ,@rc2               [int]
          ,@error             [int]
          ,@rowcount          [int]
          ,@step              [int]
          ,@cObjectName       [sysname]
          ,@ErrorMessage      [nvarchar](2048)
          ,@SeverityCode      [nvarchar] (50)
          ,@Guid              [nvarchar] (36)
          
	SELECT   @rc                = 0
	,@error             = 0
	,@rowcount          = 0
	,@step              = 0
	,@cObjectName       = OBJECT_NAME(@@PROCID)
            
    SET @outErrorString = ''

DECLARE @COUNT int
SELECT @COUNT = COUNT(1)
FROM [dbo].[AuthorizationGroup] ag
JOIN @InAuthGroupName a 
ON ag.Name = a.Name
WHERE ag.SoftDelete = 0 AND ag.Enabled = 1

IF (@count = 0)
BEGIN
    SET @outErrorString = 'Invalid Parameter Value (@InAuthGroup) - not in [dbo].[AuthorizationGroup]'
    RETURN 55118
END

DECLARE @Permission bigint
SELECT @Permission = [value]
FROM [dbo].[Permission]
WHERE [SPName]= @InSPName

IF (@Permission IS NULL)
BEGIN
    SET @outErrorString = 'Invalid Parameter Value (@InSPName) - not in [dbo].[Permission]'
    RETURN 55221
END

SELECT @COUNT = COUNT(1)
FROM [dbo].[RoleEnvPermission] rep 
JOIN [dbo].[AuthorizationGroup] ag ON rep.[RoleId] = ag.RoleId
JOIN @InAuthGroupName a ON ag.Name = a.Name AND ag.SoftDelete = 0 AND ag.Enabled = 1
JOIN Environment E ON rep.EnvId = E.Id
JOIN @InEnvironments IE ON E.[Name] = IE.[Name]
WHERE rep.Permission & @Permission != 0

IF (@COUNT = 0)
BEGIN
    SET @outErrorString = '(@InSPName = ' + @InSPName + ') Has No Permission To Be Executed'
    RETURN 55150
END

return @rc
end



GO


