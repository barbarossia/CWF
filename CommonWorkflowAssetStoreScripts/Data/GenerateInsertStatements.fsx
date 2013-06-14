// Use this script to re-generate InsertOASPActivitiesAndTemplates.dat for a new version of OASP dlls.

// You'll need to do some post-processing on etblWorkflowTypes to break the foreign key dependency cycle (WorkflowType -> StoreActivity -> WorkflowType) 
// First create workflow types with no publishing workflow or template. (Edit the insert statement to insert nulls instead of the generated workflow IDs.)
// Then when the workflows are all created, update etblWorkflowTypes to point to the appropriate workflows. (Set the workflow IDs that you would have originally inserted.)
// Finally, edit the publishing workflow so it respects the RemoteDirectory and PreviewLocation parameters.

#r "System.Data"
#r "System.Transactions"
open System
open System.Data.SqlClient
module Array =
    let contains x = Array.exists ((=) x)

let formatValueForInsertStatement(fieldType, value : obj) : string =
    // null value is like a literal: null
    if value = null || value = upcast System.DBNull.Value then
        "null"
    // literals needing no quotes: 223
    elif [|typeof<int> ; typeof<int64> ; typeof<float>|] |> Array.contains fieldType then
        sprintf "%s" (value.ToString())
    // bit: 0 or 1
    elif typeof<bool> = fieldType then
        if (value :?> bool) then "1" else "0"
    // string-type literals: 'WorkflowTemplate'
    elif [|typeof<string> ; typeof<Guid> ; typeof<DateTime>|] |> Array.contains fieldType then
        sprintf "'%s'" (value.ToString())
    // byte array: 0xA7B2992C etc.
    elif fieldType = typeof<byte[]> then
        sprintf "0x%s" (System.String.Join("", (value :?> byte[]) |> Array.map (fun byte -> System.String.Format("{0:X2}", byte))))
    // unknown: error. Need to add new type to the script.
    else
        failwithf "Unknown type: '%s'" fieldType.Name

let readValues insertTableName selectCmd =
    let c = new SqlConnection("Data Source=localhost;Initial Catalog=CommonWorkflowAssetStorePPE;Integrated Security=true")
    c.Open()
    let cmd = new SqlCommand(selectCmd, c)
    let r = cmd.ExecuteReader()
    let header = sprintf "insert %s(%s)" insertTableName <| String.Join(", ", [0..(r.FieldCount - 1)] |> List.map (r.GetName))
    let v = Array.create r.FieldCount (null :> obj)
    let formatRowForInsertStatement (r : SqlDataReader) =
        r.GetValues(v) |> ignore
        sprintf "%s\n\tvalues(%s)" header (System.String.Join(", ", (v |> Array.mapi (fun i x -> formatValueForInsertStatement (r.GetFieldType(i), x)))))    
    let inserts = System.String.Join(Environment.NewLine, seq {for row in r -> row} |> Seq.map (fun x -> formatRowForInsertStatement(r)))
    System.String.Format("set identity_insert {0} on\n{1}\nset identity_insert {0} off\n\n", insertTableName, inserts)

// omit Timestamp column    
let insertWorkflowTypes = readValues "etblWorkflowType" "select Id, GUID, Name, PublishingWorkflowId, WorkflowTemplateId, ContextVariableId, HandleVariableId, PageViewVariableId, AuthGroupId, SelectionWorkflowId, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime from etblWorkflowType"
let insertLibraries = readValues "etblActivityLibraries" "select Id, GUID, Name, AuthGroupId, Category, Executable, HasActivities, Description, ImportedBy, VersionNumber, Status, MetaTags, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, CategoryId from etblActivityLibraries"
let insertDependencies = readValues "mtblActivityLibraryDependencies" "select Id, ActivityLibraryID, DependentActivityLibraryId, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, UsageCount from mtblActivityLibraryDependencies"
let insertStoreActivities = readValues "etblStoreActivities" "select Id, GUID, Name, Description, MetaTags, IconsId, IsSwitch, IsService, ActivityLibraryId, IsUxActivity, DefaultRender, CategoryId, ToolBoxTab, ToolBoxName, IsToolBoxActivity, Version, StatusId, WorkflowTypeId, Locked, LockedBy, IsCodeBeside, XAML, DeveloperNotes, BaseType, Namespace, SoftDelete, InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, Url, ShortName from etblstoreactivities"
// copy "insert" statements to clipboard so you can paste them into the script
System.Windows.Forms.Clipboard.SetText(sprintf "%s\n%s\n%s\n%s" insertWorkflowTypes insertLibraries insertDependencies insertStoreActivities)
