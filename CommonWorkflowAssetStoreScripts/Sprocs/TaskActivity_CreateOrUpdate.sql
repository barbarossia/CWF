/**************************************************************************
// Product:  CommonWF
// FileName:TaskActivity_CreateOrUpdate.sql
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   TaskActivity_CreateOrUpdate                          *
**    Desc:   Create/Delete TaskActivity rows.                             *
**    Auth:   v-kason                                                      *
**    Date:   03/07/2013                                                  *
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
CREATE PROCEDURE [dbo].[TaskActivity_CreateOrUpdate]
        @inCaller nvarchar(50),
        @inCallerversion nvarchar (50),
        @InId BIGINT,
        @InGUID varchar (50),
        @InAssignedTo nvarchar (100),
        @InActivityId BIGINT,
        @InStatus nvarchar(50),
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
          ,@Guid              [nvarchar] (36)
 
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
	IF (@InGUID IS NULL OR @InGUID = '00000000-0000-0000-0000-000000000000')
	BEGIN
		SET @outErrorString = 'Invalid Parameter Value (@InGUID)'
		RETURN 55105
	END
	
	IF (@InActivityId = 0 or @InActivityId is null)
	BEGIN
		SET @outErrorString = 'Invalid Parameter Value (@@InActivityId)'
		RETURN 55107
	END
	
	DECLARE @ChildActivityId BIGINT
	SELECT @ChildActivityId = Id
	FROM [dbo].[Activity]
	WHERE Id =@InActivityID

	DECLARE @Id bigint

	BEGIN TRY
	DECLARE @CHECKId BIGINT
    
	SELECT @CHECKId = Id
    FROM [dbo].[TaskActivity]
    WHERE @InId = Id

	IF(@CHECKID <= 0)
	BEGIN
		SELECT @CHECKID = Id
		FROM [dbo].[TaskActivity]
		WHERE ActivityId = @InActivityID
	END

	IF(@CHECKID > 0)
	SET @InId = @CHECKID

    IF (@InId = 0 or @InId is null)
		BEGIN
			--insert taskactivity new record 
			INSERT INTO [dbo].[TaskActivity]
			(   [GUID],
				ActivityId,
				AssignedTo,
				[Status]
			)
			VALUES
			(@InGUID,
			 @ChildActivityId,
			 @InAssignedTo,
			 @InStatus
			)

			SELECT @Id = SCOPE_IDENTITY(); 
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
			sa.XAML, 
			sa.DeveloperNotes, 
			sa.BaseType, 
			sa.[Namespace],
			sa.InsertedByUserAlias,
			sa.InsertedDateTime,
			sa.UpdatedByUserAlias,
			sa.UpdatedDateTime,
			ta.Id as TaskActivityId,
			ta.ActivityId,
			ta.AssignedTo,
			ta.[Guid] as TaskActivityGuid,
			ta.[Status]
			FROM [dbo].[TaskActivity] ta
			left join Activity sa on ta.ActivityId = sa.Id
			LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
			JOIN ActivityCategory ac ON sa.CategoryId = ac.Id
			LEFT JOIN ToolBoxTabName tbtn ON sa.ToolBoxTab = tbtn.Id
			JOIN StatusCode sc ON sa.StatusId = sc.Code
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
			LEFT JOIN Icon ic ON sa.IconsId = ic.Id 
			LEFT JOIN AuthorizationGroup ag ON ag.Id = al.AuthGroupId
			WHERE ta.Id = @Id 
			AND sa.SoftDelete = 0
		END
    ELSE
	BEGIN

	 UPDATE [dbo].[TaskActivity]
	 SET [GUID] = coalesce(@InGUID,[GUID]),
		 ActivityId = coalesce(@InActivityId,@ChildActivityId),
		 AssignedTo = @InAssignedTo,
		 [Status] = @InStatus
	 WHERE Id = @InId

	 SELECT @Id = @InID; 

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
			sa.XAML, 
			sa.DeveloperNotes, 
			sa.BaseType, 
			sa.[Namespace],
			sa.InsertedByUserAlias,
			sa.InsertedDateTime,
			sa.UpdatedByUserAlias,
			sa.UpdatedDateTime,
			ta.Id as TaskActivityId,
			ta.ActivityId,
			ta.AssignedTo,
			ta.[Guid] as TaskActivityGuid,
			ta.[Status]
			FROM [dbo].[TaskActivity] ta
			left join Activity sa on ta.ActivityId = sa.Id
			LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
			JOIN ActivityCategory ac ON sa.CategoryId = ac.Id
			LEFT JOIN ToolBoxTabName tbtn ON sa.ToolBoxTab = tbtn.Id
			JOIN StatusCode sc ON sa.StatusId = sc.Code
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
			LEFT JOIN Icon ic ON sa.IconsId = ic.Id 
			LEFT JOIN AuthorizationGroup ag ON ag.Id = al.AuthGroupId
			WHERE ta.Id = @Id
			AND sa.SoftDelete = 0
	END

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
        
          -- error - could not Select FROM etblActivityLibraries
          SET @Guid         = NEWID();
          SET @rc           = 56099
          SET @step         = ERROR_LINE()
          SET @ErrorMessage = ERROR_MESSAGE()
          SET @SeverityCode = ERROR_SEVERITY()
          SET @Error         = ERROR_NUMBER()

          EXECUTE @rc2 = [dbo].[Error_Raise]
                 @inCaller           = @inCaller        --calling object
                ,@inCallerVersion    = @inCallerVersion --calling object version
                ,@ErrorGuid          = @Guid
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
