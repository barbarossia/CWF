
/**************************************************************************
// Product:  CommonWF
// FileName:ps_etblWorkflowTypeCreateOrUpdate.sql
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   WorkflowType_CreateOrUpdate                            *
**    Desc:   Create/Delete WorkflowType rows.                             *
**    Auth:   v-stska                                                      *
**    Date:   10/27/2010                                                   *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional IF complex> 
**   Parameter definition IF complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  11/10/2010     v-stska             Original implementation
**  11/21/2010     v-stska             Add inserted, updated, alias & dt
**  1/5/2011       v-stska             Change Insert to Update IF row exists	
**  2/13/2011      v-stska             Add @OutError logic
**  7/22/2011      v-stska             Change names to 255
**  11-Nov-2011	   v-richt             Bug#86713 - Change error codes to positive numbers
** *************************************************************************/
CREATE PROCEDURE [dbo].[WorkflowType_CreateOrUpdate]
        @inCaller nvarchar(50),
        @inCallerversion nvarchar (50),
        @InId bigint,
        @InGUID varchar (50),
        @Inname nvarchar (255),
        @InPublishingWorkflowId bigint,
        @InWorkflowTemplateId bigint,
        @InSoftDelete bit,
        @InHandleVariable nvarchar (255),
        @InPageViewVariable nvarchar (255),
        @InAuthGroupId bigint,
        @InSelectionWorkflowId bigint,
        @InInsertedByUserAlias nvarchar(50),
        @InUpdatedByUserAlias nvarchar(50),
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
    DECLARE @InInsertedDateTime datetime
    SET @InInsertedDateTime = GETDATE()
    DECLARE @inUpdatedDateTime datetime
    SET @inUpdatedDateTime = GETDATE()
    BEGIN TRY
        /* check to see IF this needs to be a real insert or an update
           It could have been soft deleted and an insert will fail on
           a unique Name/versionNumber constraint */
        DECLARE @CHECKId bigint
        SELECT @CHECKId = ID
        FROM [dbo].[WorkflowType]
        WHERE @InId = Id
        -- If found, change this FROM an insert to an update
        If (@CHECKId > 0)
            SET @InId = @CHECKId
        IF (@InId = 0 OR @InId IS null)
        BEGIN
            -- insert
            IF (@InGUID IS NULL OR @InGUID = '00000000-0000-0000-0000-000000000000')
            BEGIN
                SET @outErrorString = 'Invalid Parameter Value (@InGUID)'
                RETURN 55105
            END
            IF (@InName IS NULL OR @InName = '')
            BEGIN
                SET @outErrorString = 'Invalid Parameter Value (@@InName)'
                RETURN 55106
            END
            IF (@InAuthGroupId = 0  OR @InAuthGroupId is null)
            BEGIN
                SET @outErrorString = 'Invalid Parameter Value (@InAuthGroupId)'
                RETURN 55118
            END		
            IF (@InInsertedByUserAlias IS NULL OR @InInsertedByUserAlias = '')
            BEGIN
                SET @outErrorString = 'Invalid Parameter Value (@InInsertedByUserAlias)'
                RETURN 55102
            END
            
            --check same name
            DECLARE @tempName1 nvarchar(255)
            BEGIN
				SELECT @tempName1 = [Name]
				from [dbo].WorkflowType
				where @Inname = Name
				if(@tempName1 IS NOT NULL)
				BEGIN
					SET @outErrorString = 'Invalid Parameter Value (@InName), The same workflow type Name already existed.'
					RETURN 55107
                END
            END
            
            SET @InInsertedDateTime = GETDATE()
            IF (@InUpdatedByUserAlias IS NULL OR @InUpdatedByUserAlias = '')
                SET @InUpdatedByUserAlias = @InInsertedByUserAlias
            SET @InUpdatedDateTime = GETDATE()
            
            
            --PublishingWorkflowID
            DECLARE @PublishingWorkFlowId bigint
            SELECT @PublishingWorkFlowId = Id
            FROM [dbo].[Activity]
            WHERE Id = @InPublishingWorkflowId
            
            --SelectionWorkflowID
            DECLARE @SelectionWorkflowId bigint
            SELECT @SelectionWorkflowID = [ID]
            FROM [dbo].[Activity]
            WHERE Id = @InSelectionWorkflowId
            
            --TemplateWorkflowId
            DECLARE @TemplateId bigint
            SELECT @templateId = Id
            FROM [dbo].[Activity] 
            WHERE Id = @InWorkflowTemplateId
            
            INSERT INTO [dbo].[WorkflowType]
                ( [GUID], 
                  Name, 
                  PublishingWorkflowID, 
                  WorkflowTemplateID,
                  HandleVariableID,
                  SoftDelete,
                  PageViewVariableID,
                  AuthGroupId, 
                  SelectionWorkflowId,
                  InsertedByUserAlias, 
                  InsertedDateTime, 
                  UpdatedByUserAlias, 
                  UpdatedDateTime)
                VALUES
                (@InGUID,
                 @Inname,
                 @PublishingWorkflowID,
                 @TemplateId,
                 null,
                 0,
                 null,
                 @InAuthGroupId,
                 @SelectionWorkflowID,
                 @InInsertedByUserAlias,
                 @InInsertedDateTime,
                 @InUpdatedByUserAlias, 
                 @InUpdatedDateTime)
        
         --COMMIT TRANSACTION
         END
         ELSE
         BEGIN
            DECLARE @AuthGroupID1 bigint
            IF (@InAuthGroupId = 0 OR @InAuthGroupId IS NULL)
            BEGIN
                SELECT @AuthGroupID1 = [ID]
                FROM [dbo].[AuthorizationGroup]
                WHERE	Id = @InAuthGroupId
                IF (@AuthGroupID1 = 0 OR @AuthGroupID1 IS NULL)
                BEGIN
                    SET @outErrorString = 'Invalid Parameter Value (@InAuthGroupId)'
                    RETURN 56099
                END
            END
            
            --check the same name
            DECLARE @tempName2 nvarchar(255)
            BEGIN
				SELECT @tempName2 = [Name]
				from [dbo].WorkflowType
				where @Inname = Name and @InId <> Id
				if(@tempName2 IS NOT NULL)
				BEGIN
					SET @outErrorString = 'Invalid Parameter Value (@InName), The same workflow type Name already existed.'
					RETURN 55108
                END
            END
            
            --PublishingWorkflowID
            SELECT @PublishingWorkFlowID = [ID]
            FROM [dbo].[Activity]
            WHERE Id = @InPublishingWorkflowId            
                      
            --SelectionWorkflowID
            SELECT @SelectionWorkflowID = [ID]
            FROM [dbo].[Activity]
            WHERE Id = @InSelectionWorkflowId
            
             --TemplateWorkflowId
            SELECT @templateId = Id
            FROM [dbo].[Activity] 
            WHERE Id = @InWorkflowTemplateId
            
            -- Test for valid @InId
            DECLARE @TEMPID bigint
            SELECT @TEMPID = [Id]
            FROM [dbo].[WorkflowType]
            WHERE Id = @InId
            IF (@TEMPID IS NULL)
            BEGIN
                SET @outErrorString = 'Invalid Parameter Value (@InId)'
                RETURN 55061
            END
             
             --Check IF delete workflowtype
             IF (@InSoftDelete = 1)
				UPDATE [dbo].[WorkflowType] SET SoftDelete = @InSoftDelete WHERE Id = @InId
             ELSE
             BEGIN   --update workflow
				UPDATE [dbo].[WorkflowType]
					SET [GUID] = Coalesce(@InGUID, [GUID]),
						Name = Coalesce(@InName, Name),
						PublishingWorkflowID = Coalesce(@PublishingWorkflowID, PublishingWorkflowID),
						WorkflowTemplateID = Coalesce(@templateId, WorkflowTemplateID),
						HandleVariableID = Coalesce(null, HandleVariableID),
						PageViewVariableID = Coalesce(null, PageViewVariableID),
						AuthGroupId = Coalesce(@AuthGroupID1, AuthGroupId),
						SelectionWorkflowID = Coalesce(@SelectionWorkflowID, SelectionWorkflowID),
						SoftDelete = @InSoftDelete,
						UpdatedByUserAlias = Coalesce(@InUpdatedByUserAlias, UpdatedByUserAlias),
						UpdatedDateTime = Coalesce(@InUpdatedDateTime, GETDATE())
				WHERE Id = @InId
            END
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


