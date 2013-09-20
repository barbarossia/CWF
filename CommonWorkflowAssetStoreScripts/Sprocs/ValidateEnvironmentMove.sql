/****** Object:  StoredProcedure [dbo].[ValidateEnvironmentMove]    Script Date: 05/19/2013 22:34:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ValidateEnvironmentMove]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ValidateEnvironmentMove]
GO


CREATE PROCEDURE [dbo].[ValidateEnvironmentMove]
@CurrentEnvironmentName	nvarchar(50),
@ToEnvironmentName	nvarchar(50),
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

	IF (@CurrentEnvironmentName = 'dev' and @ToEnvironmentName != 'test')
	BEGIN
    		SET @outErrorString = 'Invalid Move Sequence (@CurrentEnvironmentName) and (@ToEnvironmentName)!'
    		RETURN 55120
	END
	IF (@CurrentEnvironmentName = 'test' and @ToEnvironmentName != 'stage' and @ToEnvironmentName != 'dev')
	BEGIN
    		SET @outErrorString = 'Invalid Move Sequence (@CurrentEnvironmentName) and (@ToEnvironmentName)!'
    		RETURN 55120
	END
	IF (@CurrentEnvironmentName = 'stage' and @ToEnvironmentName != 'prod' and @ToEnvironmentName != 'test')
	BEGIN
    		SET @outErrorString = 'Invalid Move Sequence (@CurrentEnvironmentName) and (@ToEnvironmentName)!'
    		RETURN 55120
	END
	IF (@CurrentEnvironmentName = 'prod' and @ToEnvironmentName != 'stage')
	BEGIN
    		SET @outErrorString = 'Invalid Move Sequence (@CurrentEnvironmentName) and (@ToEnvironmentName)!'
    		RETURN 55120
	END

return @rc
end



GO


