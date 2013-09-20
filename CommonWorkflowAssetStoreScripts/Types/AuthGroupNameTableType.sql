-- =============================================
-- Author: Luis Valencia (v-luval)
-- Description: Type for sending a table-valued parameter for the 
-- function that gets the missing assemblies in 
-- the asset store based on an initial list to search for.
-- Create Date: Mar-12, 2012
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'AuthGroupNameTableType' AND ss.name = N'dbo')
CREATE TYPE [dbo].[AuthGroupNameTableType] AS TABLE(
	[Name] [nvarchar](255) NOT NULL
)
GO