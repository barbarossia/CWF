/****** Object:  StoredProcedure [dbo].[Error_Raise]    Script Date: 03/22/2012 11:08:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/* ********************************************************************
**    Name:   Error_Raise                                      *
**    Desc:   Inserts errorlog record and raises error.               *
**    Auth:   v-stska                                                 *
**    Date:   10/31/2010                                              *
**                                                                    *
***********************************************************************
**   sproc logic flow: <Optional if complex> 
**   Parameter definition if complex
***********************************************************************
**                  CHANGE HISTORY
***********************************************************************
**	Date:        Author:             Description:
** ____________    ________________    ______________________________
**  4/25/2008      v-tibyer             Original implementation
** 10/31/2010      v-stska				Modification
**  7/12/2011      v-stska              increase size of error msgs
**  11-Nov-2011	   v-richt              Bug#86713 - Change error codes to positive numbers
** 	22 Mar 2012	   v-richt			    tblErrorLog removed; changed to match our new error handling scheme
** ********************************************************************/
CREATE PROCEDURE [dbo].[Error_Raise]  @inCaller               [varchar](30) --Calling Object name
                                            ,@inCallerVersion        [varchar](30) --Calling Object version
                                            ,@ErrorGuid				 [uniqueidentifier] -- ties together client msg with error log
                                            ,@inErrorDate            [datetime]       = NULL --date error/msg occurred
                                            ,@inErrorNumber          [int]            = NULL --e.g. OnError @[System::ErrorCode] or @@error
                                            ,@inErrorMessage         [nvarchar](max) = NULL --e.g. OnInformation @[System::ErrorDescription] or @msg
                                            ,@inSeverityCode         [nvarchar](7)    = NULL --e.g. 'Error','Fatal','Kill','NoMSG','DEBUG'
                                            ,@inEventType            [nvarchar](20)   = NULL --e.g. 'OnPostExecute','OnError'
                                            ,@inTaskName             [nvarchar](50)   = NULL --e.g. @[System::SourceName]
                                            ,@inPackageName          [nvarchar](50)   = NULL --e.g. @[System::PackageName]
                                            ,@inHostName             [nvarchar](128)  = NULL --e.g. @[System::MachineName] or HOST_NAME()
                                            ,@inPackageDuration      [int]            = NULL --e.g. DATEDIFF("ss",@[System::StartTime],GETDATE())
                                            ,@inTaskDuration         [int]            = NULL --e.g. DATEDIFF("ss",@[System::ContainerStartTime],GETDATE())
                                            ,@inInsertCount          [int]            = NULL --e.g. @[User::InsertCount] or @@ROWCOUNT
                                            ,@inUpdateCount          [int]            = NULL --e.g. @[User::UpdateCount] or @@ROWCOUNT
                                            ,@inDeleteCount          [int]            = NULL --e.g. @[User::DeleteCount] or @@ROWCOUNT
                                            ,@inRowsAffected         [int]            = NULL --e.g. @@ROWCOUNT
                                            ,@inMethodName           [sysname]        = NULL --e.g. @ObjectName    --Current Object (e.g. stored proc name)
                                            ,@inMethodStep           [int]            = NULL --e.g. @step          --set within the stored proc
                                            ,@inTableName            [nvarchar](50)   = NULL --e.g. table name 
                                            ,@inSource               [nvarchar](512)  = NULL --e.g. the SQL command text
                                            ,@inMessageDisplayedFlag [bit]            = NULL --e.g. was error displayed to user
                                            ,@inTechMessage          [nvarchar](max)  = NULL --e.g. msg to tech for debugging
                                            ,@inUserMessage          [nvarchar](max)  = NULL --e.g. msg displayed to user that overrides original error msg

--WITH ENCRYPTION
AS
BEGIN
 
   SET NOCOUNT ON
   
   EXEC Error_Handle

 
   RETURN 0
 
END