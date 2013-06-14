-- =============================================
-- Author: Luis Valencia (v-luval)
-- Description: Type for sending a table-valued parameter for the 
-- function that gets the missing assemblies in 
-- the asset store based on an initial list to search for.
-- Create Date: Mar-12, 2012
-- =============================================
CREATE TYPE [dbo].[ActivityLibraryTableType] AS TABLE(
	[Name] [nvarchar](255) NOT NULL,
	[VersionNumber] [nvarchar](50) NOT NULL
)
GO