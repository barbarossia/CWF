IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Error_Handle]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Error_Handle]
GO

-- =============================================
-- Author: Sanjeewa Jayasinghe (v-sanja)
-- Description: Handles errors and raises to be caught in DAL.
-- Create Date: Feb-08, 2012
-- =============================================
CREATE PROCEDURE [dbo].[Error_Handle]
AS
BEGIN
    SET NOCOUNT OFF;
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorNumber INT, @ErrorSeverity INT, @ErrorState INT;
    
    SET @ErrorNumber = ERROR_NUMBER();
    SET @ErrorSeverity = ERROR_SEVERITY();
    SET @ErrorState = ERROR_STATE();
    
    -- Make the error number a part of the message.  
    -- When we RAISERROR with a message, it sends error number 50000 to calling app.
    -- We could raise errors with application specific error number, however then we 
    -- have to reformat the error message template of those error numbers to replace the 
    -- string place holders defined in them custom error messages.  However, when this 
    -- stored proc handles an error, the message received here is already formatted
    -- with appropriate contextual values.
    -- To preserve such contextual error messages, we are raising error with error message.  
    -- To convey the actual error number, we are pre-pending it to the message.
    SET @ErrorMessage = N'<ErrorNumber>' + CAST(ERROR_NUMBER() AS NVARCHAR(20)) + '</ErrorNumber> ' + ERROR_MESSAGE();    
    RAISERROR(@ErrorMessage, 
              @ErrorSeverity,
              @ErrorState
              ) WITH LOG;
END
GO
