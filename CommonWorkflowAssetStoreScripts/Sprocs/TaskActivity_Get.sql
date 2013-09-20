/**************************************************************************
// Product:  CommonWF
// FileName:TaskActivity_Get.sql
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   TaskActivity_Get                          *
**    Desc:   Get TaskActivity rows.                             *
**    Auth:   v-kason                                                     *
**    Date:   03/07/2013                                                   *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional IF complex> 
**   Parameter definition IF complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  03/06/2013     v-kason            Original implementation
** *************************************************************************/

/****** Object:  StoredProcedure [dbo].[Activity_Get]    Script Date: 08/03/2012 16:22:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskActivity_Get]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TaskActivity_Get]
GO


CREATE PROCEDURE [dbo].[TaskActivity_Get]
        @inCaller nvarchar(50),
        @inCallerversion nvarchar (50),
		@InId bigint,
        @InActivityId bigint,
        @InTaskActivityGUID varchar (50),
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
	    DECLARE @id bigint
		DECLARE @activityId bigint
		DECLARE @softDelete bit

		--Get TaskActivity by Id
	 IF (@InId > 0)
	 BEGIN
			SELECT @id = Id,@activityId = ActivityId
			FROM [dbo].[TaskActivity]
			WHERE Id = @InId

			SELECT @softDelete = SoftDelete
			FROM [dbo].[Activity]
			WHERE Id = @activityId

			IF ( @id IS NULL OR @activityId IS NULL)
				BEGIN
					SET @outErrorString = 'Invalid @InId attempting to perform a GET on table'
					RETURN 55040
				END

			IF (@softDelete = 1)
				BEGIN
					SET @outErrorString = 'Invalid @InId attempting to perform a GET on table that is marked soft delete'
					RETURN 55041
				END
	 END
	 ELSE IF(@InTaskActivityGUID <> '00000000-0000-0000-0000-000000000000')
	 BEGIN
			SELECT @id = Id,@activityId = Id
			FROM [dbo].[TaskActivity]
			WHERE [Guid] = @InTaskActivityGUID
			AND Id IN (SELECT MAX(Id) AS Id From TaskActivity GROUP BY [Guid])

			SELECT @softDelete = SoftDelete
			FROM [dbo].[Activity]
			WHERE Id = @activityId

			IF (@id = 0 OR @id Is NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InTaskActivityGUID attempting to perform a GET on table'
				RETURN 55042
			END
			IF (@softDelete = 1)
			BEGIN
				SET @outErrorString = 'Invalid @InTaskActivityGUID attempting to perform a GET on table that is marked soft delete'
				RETURN 55043
			END
	 END
	 ELSE IF (@InActivityId > 0)
	 BEGIN
			SELECT @id = Id,@activityId = ActivityId
			FROM [dbo].[TaskActivity]
			WHERE ActivityId = @InActivityId

			SELECT @softDelete = SoftDelete
			FROM [dbo].[Activity]
			WHERE Id = @activityId

			IF (@id = 0 OR @id Is NULL)
			BEGIN
				SET @outErrorString = 'Invalid @InActivityId attempting to perform a GET on table'
				RETURN 55044
			END
			IF (@softDelete = 1)
			BEGIN
				SET @outErrorString = 'Invalid @InActivityId attempting to perform a GET on table that is marked soft delete'
				RETURN 55045
			END
	 END
	 IF (@id IS NULL OR @id = 0)
	 BEGIN
		SET @outErrorString = 'Invalid @InId attempting to perform a GET on table'
		RETURN 55045
	 END
	 SELECT sa.Id, 
			sa.[GUID], 
			sa.Name,
			sa.ShortName, 
			sa.[Description], 
			sa.MetaTags, 
			sa.IconsId, 
			ic.[Name] AS iconsName,
			sa.IsSwitch, 
			sa.IsService, 
			sa.ActivityLibraryId, 
			al.Name AS ActivityLibraryName,
			al.VersionNumber AS ActivityLibraryVersion,
			al.AuthGroupID,
			ag.Name AS AuthGroupName, 
			sa.IsUxActivity, 
			sa.CategoryId, 
			ac.Name as ActivityCategoryName, 
			tbtn.Name as ToolBoxtabName, 
			sa.ToolBoxtab, 
			sa.IsToolBoxActivity, 
			sa.[Version], 
			sa.StatusId, 
			sc.Name AS StatusCodeName, 
			sa.WorkflowTypeId,
			wft.Name as WorkFlowTypeName, 
			sa.Locked, 
			sa.LockedBy, 
			sa.IsCodeBeside, 
			sa.DeveloperNotes, 
			sa.BaseType, 
			sa.[Namespace],
			sa.InsertedByUserAlias,
			sa.InsertedDateTime,
			sa.UpdatedByUserAlias,
			sa.UpdatedDateTime,
			sa.XAML,
			E.Name as [Environment],
			ta.Id as TaskActivityId,
			ta.ActivityId,
			ta.AssignedTo,
			ta.[Guid] as TaskActivityGuid,
			ta.[Status]
		FROM [dbo].[TaskActivity] ta 
		LEFT JOIN [dbo].[Activity] sa on ta.ActivityId = sa.Id
		LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
		JOIN ActivityCategory ac ON sa.CategoryId = ac.Id
		LEFT JOIN ToolBoxTabName tbtn ON sa.ToolBoxTab = tbtn.Id
		JOIN StatusCode sc ON sa.StatusId = sc.Code
		JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
		LEFT JOIN Icon ic ON sa.IconsId = ic.Id 
		LEFT JOIN AuthorizationGroup ag ON ag.Id = al.AuthGroupId
		JOIN Environment E ON sa.Environment = E.Id 
		WHERE ta.Id = @id

   END TRY
   BEGIN CATCH
		/*
           Available error values FROM CATCH
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
