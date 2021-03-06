/****** Object:  StoredProcedure [dbo].[ActivityLibrary_GetMissing]    Script Date: 07/31/2012 13:32:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
-- =============================================
-- Author: Luis Valencia (v-luval)
-- Description: Gets the missing assemblies in 
-- the asset store based on an initial list to search for.
-- Create Date: Mar-12, 2012
-- =============================================                                   
create PROCEDURE [dbo].[ActivityLibrary_GetMissing]
		@inActivityLibraries [dbo].[ActivityLibraryTableType] READONLY		
AS
BEGIN
	SET NOCOUNT ON
	
	BEGIN TRY
		
	SELECT al.Name, al.VersionNumber
    FROM @inActivityLibraries as al
	WHERE NOT EXISTS
		(
			SELECT 1 FROM [dbo].[ActivityLibrary] WITH (NOLOCK)
			JOIN [dbo].[StatusCode] ON [dbo].[ActivityLibrary].[Status]=[dbo].[StatusCode].Code
			WHERE [dbo].[ActivityLibrary].Name = al.Name AND
				[dbo].[ActivityLibrary].VersionNumber = al.VersionNumber AND	
				[dbo].[StatusCode].Name='Public'
		)
    

	END TRY
	BEGIN CATCH
		EXECUTE [dbo].Error_Handle 
	END CATCH
END
