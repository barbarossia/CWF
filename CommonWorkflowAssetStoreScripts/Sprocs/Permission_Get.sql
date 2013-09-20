/****** Object:  StoredProcedure [dbo].[Permission_Get]    Script Date: 05/16/2013 22:48:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permission_Get]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Permission_Get]
GO


Create PROCEDURE [dbo].[Permission_Get]
		@inCaller			nvarchar(50),
		@inCallerversion	nvarchar (50),
		@outErrorString nvarchar (300)OUTPUT
--WITH ENCRYPTION
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
          ,@Guid1              [nvarchar] (36)
 
   	SELECT   @rc                = 0
          	,@error             = 0
          	,@rowcount          = 0
          	,@step              = 0
          	,@cObjectName       = OBJECT_NAME(@@PROCID)
	--BEGIN TRANSACTION
	SET @outErrorString = ''
	-- Check the input variables
	IF (@inCaller IS NULL OR @inCaller = '')
	BEGIN
		SET @outErrorString = 'Invalid Parameter Value (@inCaller)'
		RETURN 55100
	END
	IF (@inCallerversion IS NULL OR @inCallerversion = '')
	BEGIN
		SET @outErrorString = 'Invalid Parameter Value (@inCallerversion)'
		RETURN 55101
	END
	
	BEGIN TRY
		select s.AuthorGroupName, s.Permission, e.name as EnvironmentName from
		(
			select ag.Name as AuthorGroupName, p.Permission, p.EnvId from AuthorizationGroup ag join Role r
			on ag.RoleId = r.Id and ag.SoftDelete = 0 AND ag.Enabled = 1
			join RoleEnvPermission p 
			on r.Id = p.RoleId 
		) s join Environment e on s.EnvId = e.Id and e.SoftDelete = 0
	END TRY
	BEGIN CATCH
		/*
           Available error values from CATCH
           ERROR_NUMBER() ,ERROR_SEVERITY() ,ERROR_STATE() ,ERROR_PROCEDURE() ,ERROR_LINE() ,ERROR_MESSAGE()
        */
		SELECT @error    = @@ERROR
			 ,@rowcount = @@ROWCOUNT
		IF @error <> 0
		BEGIN
		  SET @Guid1         = NEWID();
		  SET @rc           = 56099
		  SET @step         = ERROR_LINE()
		  SET @ErrorMessage = ERROR_MESSAGE()
		  SET @SeverityCode = ERROR_SEVERITY()
		  SET @Error         = ERROR_NUMBER()
		  --IF @@TRANCOUNT <> 0
		  --BEGIN
			 --ROLLBACK TRAN
		  --END
		  EXECUTE @rc2 = [dbo].[Error_Raise]
				 @inCaller           = @inCaller        --calling object
				,@inCallerVersion    = @inCallerVersion --calling object version
				,@ErrorGuid          = @Guid1
				,@inMethodName       = @cObjectName     --current object
				,@inMethodStep       = @step
				,@inErrorNumber      = @Error
				,@inRowsAffected     = @rowcount
				,@inSeverityCode     = @SeverityCode
				,@inErrorMessage     = @ErrorMessage
		  SET @outErrorString = @ErrorMessage
		  RETURN @Error
		 END
	END CATCH
   RETURN @rc

END
GO


