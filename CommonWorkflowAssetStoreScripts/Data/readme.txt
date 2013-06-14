The InsertOASPActivitiesAndTemplates.data script is used for generating databases
pre-populated with a particular version of OASP activities. It will need to be updated
when OASP releases a new version of OASP.Core/etc. that you want to standardize on.

The script is generated mostly by using an F# script which clones an existing database
into a set of insert scripts. So, the update procedure for making a new
InsertOASPActivititiesAndTemplates.dat file is:

1.) Overwrite InsertOASPActivitiesAndTemplates.dat with InsertInitialData
2.) Run GenerateCWFAssetStore.bat. Because of step #1 this will create a mostly-blank
    database.
3.) Use the authoring tool to Import/Upload the appropriate versions of OASP.
4.) Use the authoring tool to tweak PageTemplate and WorkflowTemplate to use the
    new activities from the new version of OASP.
5.) Use GenerateInsertStatements.fsx to generate the new InsertOASPActivitiesAndTemplates.dat
    a.) You can run it either from inside VisualStudio or via fsi from the command line
    b.) You may need to tweak the ConnectionString to point to wherever you've got your database
    c.) If the DB schema has changed you may need to update the SQL statements to include new fields or tables.
    d.) Because foreign key constraints have already been created by the time this script runs,
        etblWorkflowTypes has to be created in two passes. You will need to do some post-processing
        on the generated SQL to break etblWorkflowTypes apart into insert/update statements. See
        InsertOASPActivitiesAndTemplates.dat for example.