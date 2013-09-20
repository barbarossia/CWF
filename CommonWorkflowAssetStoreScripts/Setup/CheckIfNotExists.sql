declare @ExecSQL nvarchar(max)
set @ExecSQL = REPLACE('
-- the following is eliminated with a running version
IF DB_ID (N''@DBName'') IS NULL
BEGIN
   RAISERROR(N''@DBName Database Is Not Exists!'', 16, 127) WITH NOWAIT
END

', '@DBName', $(DBName))
Exec(@ExecSQL)
GO