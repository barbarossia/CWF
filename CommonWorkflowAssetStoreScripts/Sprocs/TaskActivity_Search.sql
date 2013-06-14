/**************************************************************************
// Product:  CommonWF
// FileName:[TaskActivity_Search]
//
// Copyright 2010 Microsoft Corporation. All rights reserved.
// Microsoft Confidential
***************************************************************************/
/* *************************************************************************
**    Name:   TaskActivity_Get                          *
**    Desc:   Get TaskActivity rows.                             *
**    Auth:   v-kason                                                     *
**    Date:   03/10/2013                                                   *
**                                                                         *
****************************************************************************
**   sproc logic flow: <Optional IF complex> 
**   Parameter definition IF complex
****************************************************************************
**                  CHANGE HISTORY
****************************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ____________________________________
**  03/10/2013     v-kason            Original implementation
** *************************************************************************/

/****** Object:  StoredProcedure [dbo].[TaskActivity_GetLatestVersion]   Script Date: 08/03/2012 16:22:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[TaskActivity_Search]
        @inCaller nvarchar(50),
        @inCallerversion nvarchar (50),
        @InTaskActivityGUID varchar (50),
        @InAssignedTo nvarchar (100),
		@InFilterOlder bit,
		@SearchText nvarchar(250),
		@SortColumn varchar(50),
		@SortAscending bit,
		@PageSize int,
		@PageNumber int,
		@HideUnassignedTasks bit,
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
		DECLARE @InAssignedToExists bit
		DECLARE @NeedSearch bit

		IF (@SearchText IS NOT NULL AND @SearchText != '')	
		SET @NeedSearch = 1
		ELSE
		SET @NeedSearch = 0	 
		 
	    IF (@SortColumn IS NULL OR @SortColumn = '')
		SET @SortColumn = 'Name'

		IF (@PageSize IS NULL OR @PageSize < 1)
		SET @PageSize = 10

		IF (@PageNumber IS NULL OR @PageNumber < 1)
		SET @PageNumber = 1

		DECLARE @StartIndex int
		IF @PageNumber > 1 
			SET @StartIndex = @PageSize * (@PageNumber - 1)+1
		ELSE
			SET @StartIndex = 1

		IF (@SearchText IS NOT NULL AND @SearchText != '')
			SET @SearchText = '%' + @SearchText + '%'

	CREATE TABLE #SearchTasksResults 
	( 
		RowNumber bigint, 
		Id bigint,
		[GUID] UniqueIdentifier, 
		Name nvarchar(255),
		ShortName nvarchar(50), 
		[Description] nvarchar(250), 
		MetaTags nvarchar(max), 
		IconsId bigint, 
		iconsName nvarchar(50),
		IsSwitch bit, 
		IsService bit, 
		ActivityLibraryId bigint, 
		ActivityLibraryName nvarchar(255),
		ActivityLibraryVersion nvarchar(25),
		AuthGroupID bigint,
		AuthGroupName nvarchar(255), 
		IsUxActivity bit, 
		CategoryId bigint, 
		ActivityCategoryName nvarchar(255), 
		ToolBoxtabName nvarchar(30), 
		ToolBoxtab bigint, 
		IsToolBoxActivity bit, 
		[Version] nvarchar(25), 
		StatusId bigint, 
		StatusCodeName nvarchar(50), 
		WorkflowTypeId bigint,
		WorkFlowTypeName nvarchar(50), 
		Locked bit, 
		LockedBy nvarchar(50), 
		IsCodeBeside bit, 
		XAML nvarchar(max), 
		DeveloperNotes nvarchar(250), 
		BaseType nvarchar(50), 
		[Namespace] nvarchar(250),
		InsertedByUserAlias nvarchar(50),
		InsertedDateTime datetime,
		UpdatedByUserAlias nvarchar(50),
		UpdatedDateTime datetime,
		TaskActivityId bigint,
		ActivityId bigint,
		AssignedTo nvarchar(100),
		TaskActivityGuid UniqueIdentifier,
		[Status] nvarchar(50)
	)
      
	BEGIN

    IF (@InAssignedTo is null or @InAssignedTo = '')
		SET @InAssignedToExists = 0
	ELSE
		SET @InAssignedToExists = 1

    IF (@InTaskActivityGuid <> '00000000-0000-0000-0000-000000000000')	
		BEGIN
			 INSERT INTO #SearchTasksResults
			 SELECT ROW_NUMBER() OVER 
			(ORDER BY							
						CASE WHEN @SortColumn = 'Name' AND @SortAscending = 1 THEN sa.Name END ASC,
						CASE WHEN @SortColumn = 'Name' THEN sa.Name END DESC,
						CASE WHEN @SortColumn = 'Version'  AND @SortAscending = 1  THEN sa.[Version] END ASC,
						CASE WHEN @SortColumn = 'Version' THEN sa.[Version] END DESC,
						CASE WHEN @SortColumn = 'CreatedBy'  AND @SortAscending = 1  THEN sa.[InsertedByUserAlias] END ASC,
						CASE WHEN @SortColumn = 'CreatedBy' THEN sa.[InsertedByUserAlias] END DESC,
						CASE WHEN @SortColumn = 'CreatedTime'  AND @SortAscending = 1 THEN sa.InsertedDateTime END ASC,
						CASE WHEN @SortColumn = 'CreatedTime' THEN sa.InsertedDateTime END DESC
			            ) AS [RowNumber],
				sa.Id, 
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
			LEFT JOIN [dbo].[Activity] sa on ta.ActivityId = sa.Id
			LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
			JOIN ActivityCategory ac ON sa.CategoryId = ac.Id
			LEFT JOIN ToolBoxTabName tbtn ON sa.ToolBoxTab = tbtn.Id
			JOIN StatusCode sc ON sa.StatusId = sc.Code
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
			LEFT JOIN Icon ic ON sa.IconsId = ic.Id 
			LEFT JOIN AuthorizationGroup ag ON ag.Id = al.AuthGroupId
			WHERE 
			ta.[GUID] = @InTaskActivityGuid AND sa.SoftDelete = 0 
			AND (@NeedSearch = 0 OR sa.Name like @SearchText)
			AND (@InFilterOlder = 0 OR
				 ta.Id IN (SELECT MAX(Id) as Id From TaskActivity GROUP BY [Guid]))
			AND (@HideUnassignedTasks = 0 OR ta.[Status] <> 'Unassigned')
		END   
    ELSE
	   BEGIN 
			 INSERT INTO #SearchTasksResults
			 SELECT ROW_NUMBER() OVER 
			           (ORDER BY							
						CASE WHEN @SortColumn = 'Name' AND @SortAscending = 1 THEN sa.Name END ASC,
						CASE WHEN @SortColumn = 'Name' THEN sa.Name END DESC,
						CASE WHEN @SortColumn = 'Version'  AND @SortAscending = 1  THEN sa.[Version] END ASC,
						CASE WHEN @SortColumn = 'Version' THEN sa.[Version] END DESC,
						CASE WHEN @SortColumn = 'CreatedBy'  AND @SortAscending = 1  THEN sa.[InsertedByUserAlias] END ASC,
						CASE WHEN @SortColumn = 'CreatedBy' THEN sa.[InsertedByUserAlias] END DESC,
						CASE WHEN @SortColumn = 'CreatedTime'  AND @SortAscending = 1 THEN sa.InsertedDateTime END ASC,
						CASE WHEN @SortColumn = 'CreatedTime' THEN sa.InsertedDateTime END DESC
			            ) AS [RowNumber],
				sa.Id, 
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
			LEFT JOIN [dbo].[Activity] sa on ta.ActivityId = sa.Id
			LEFT JOIN ActivityLibrary al ON sa.ActivityLibraryId = al.Id
			JOIN ActivityCategory ac ON sa.CategoryId = ac.Id
			LEFT JOIN ToolBoxTabName tbtn ON sa.ToolBoxTab = tbtn.Id
			JOIN StatusCode sc ON sa.StatusId = sc.Code
			JOIN WorkflowType wft ON sa.WorkflowTypeId = wft.Id
			LEFT JOIN Icon ic ON sa.IconsId = ic.Id 
			LEFT JOIN AuthorizationGroup ag ON ag.Id = al.AuthGroupId
			WHERE 
			(@InAssignedTo = ta.AssignedTo OR @InAssignedToExists <> 1)
			AND (@NeedSearch = 0 OR sa.Name like @SearchText)
			AND sa.SoftDelete = 0 
			AND (@InFilterOlder = 0 OR
				 ta.Id IN (SELECT MAX(Id) as Id From TaskActivity GROUP BY [Guid]))
			AND (@HideUnassignedTasks = 0 OR ta.[Status] <> 'Unassigned')
	   END	
   END
   
   SELECT * FROM   #SearchTasksResults
   WHERE  RowNumber BETWEEN @StartIndex AND ((@StartIndex + @PageSize) -1)
   ORDER BY rownumber	
   SELECT count(*) AS Total from #SearchTasksResults
   DROP TABLE  #SearchTasksResults

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
