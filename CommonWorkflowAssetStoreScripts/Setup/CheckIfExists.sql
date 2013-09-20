declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE('
-- the following is eliminated with a running version
IF DB_ID (N''@DBName'') IS NOT NULL
BEGIN
   RAISERROR(N''@DBName Database Is Exists!'', 16, 127) WITH NOWAIT
END

', '@DBName', $(DBName))
Exec(@ExecSQL)
GO