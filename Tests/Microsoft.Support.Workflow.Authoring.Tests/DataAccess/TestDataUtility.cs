
using System.Collections.Generic;
using System.Data;
using System;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
namespace Microsoft.Support.Workflow.Authoring.Tests.DataAccess
{
    /// <summary>
    /// Utility methods that performs data read/write/delete operations 
    /// in the database should be added in this class.
    /// </summary>
    public class TestDataUtility
    {
        //Sql statements are here since it's a test code we really didn't wan't to get overloaded by putting the this into stored procs. 
        private static string DeleteTestInpurLibrarySql =
            "delete from dbo.Activity " +
            "where ActivityLibraryID in " +
                "(select Id " +
                "from dbo.ActivityLibrary " +
                "where Name = @name) " +

            "delete from dbo.ActivityLibraryDependency " +
            "where ActivityLibraryID in " +
                "(select Id " +
                "from dbo.ActivityLibrary " +
                "where Name = @name) " +

            "delete from dbo.ActivityLibrary " +
            "where Name = @name ";

        private static string SelectTestInputLibrarySql =
            "select Name " +
            "from dbo.ActivityLibrary " +
            "where Name = @name ";



        /// <summary>
        /// Delete's TestInput_Library1 from tables 
        /// (etblStoreActivities, mtblActivityLibraryDependencies, etblActivityLibraries)
        /// </summary>
        public static void DeleteTestInputLibrary1DataFromDB()
        {
            TestDataProvider.ExecuteNonQuery(DeleteTestInpurLibrarySql, "@name", "TestInput_Lib1");
        }
        /// <summary>
        /// Delete test library from tables (etblStoreActivities, mtblActivityLibraryDependencies, etblActivityLibraries)
        /// </summary>
        /// <param name="workflowName"></param>
        public static void ClearTestWorkflowFromDB(string workflowName)
        {
            TestDataProvider.ExecuteNonQuery(DeleteTestInpurLibrarySql, "@name", workflowName);
        }
        /// <summary>
        /// Delete's TestInput_Library2 from tables 
        /// (etblStoreActivities, mtblActivityLibraryDependencies, etblActivityLibraries)
        /// </summary>
        public static void DeleteTestInputLibrary2DataFromDB()
        {
            TestDataProvider.ExecuteNonQuery(DeleteTestInpurLibrarySql, "@name", "TestInput_Lib2");
        }

        /// <summary>
        /// Delete's TestInput_Library3 from tables 
        /// (etblStoreActivities, mtblActivityLibraryDependencies, etblActivityLibraries)
        /// </summary>
        public static void DeleteTestInputLibrary3DataFromDB()
        {
            TestDataProvider.ExecuteNonQuery(DeleteTestInpurLibrarySql, "@name", "TestInput_Lib3");
        }

        /// <summary>
        /// Delete's TestInput_Library1, TestInput_Library2, TestInput_Library3 from 
        /// tables (etblStoreActivities, mtblActivityLibraryDependencies, etblActivityLibraries)
        /// </summary>
        public static void DeleteTestInputLibrariesDataFromDB()
        {
            string sql =
               "delete from dbo.Activity " +
               "where ActivityLibraryID in " +
                   "(select Id " +
                   "from dbo.ActivityLibrary " +
                   "where Name in (@name1, @name2, @name3)) " +

               "delete from dbo.ActivityLibraryDependency " +
               "where (ActivityLibraryID in " +
                   "(select Id " +
                   "from dbo.ActivityLibrary " +
                   "where Name in (@name1, @name2, @name3))) OR  " +
                   "(DependentActivityLibraryId in " +
                   "(select Id " +
                   "from dbo.ActivityLibrary " +
                   "where Name in (@name1, @name2, @name3))) " +

               "delete from dbo.ActivityLibrary " +
               "where Name in (@name1, @name2, @name3) ";

            IDictionary<string, object> commandParams = new Dictionary<string, object>();

            commandParams.Add("@name1", "TestInput_Lib1");
            commandParams.Add("@name2", "TestInput_Lib2");
            commandParams.Add("@name3", "TestInput_Lib3");

            TestDataProvider.ExecuteNonQuery(sql, commandParams);
        }

        /// <summary>
        /// Verify that the TestInput_Library1 is added to etblActivityLibraries table.
        /// </summary>
        public static bool VerifyTestInputLibrary1IsAvailableInDB()
        {
            object result = TestDataProvider.ExecuteScalar(SelectTestInputLibrarySql, "@name", "TestInput_Lib1");
            return result != null;
        }

        /// <summary>
        /// Verify that the TestInput_Library2 is added to etblActivityLibraries table.
        /// </summary>
        public static bool VerifyTestInputLibrary2IsAvailableInDB()
        {
            object result = TestDataProvider.ExecuteScalar(SelectTestInputLibrarySql, "@name", "TestInput_Lib2");
            return result != null;
        }

        /// <summary>
        /// Verify that the TestInput_Library3 is added to etblActivityLibraries table.
        /// </summary>
        public static bool VerifyTestInputLibrary3IsAvailableInDB()
        {
            object result = TestDataProvider.ExecuteScalar(SelectTestInputLibrarySql, "@name", "TestInput_Lib3");
            return result != null;
        }

        /// <summary>
        /// Verify that the TestInput_Library1, TestInput_Library2, TestInput_Library3
        /// are added to etblActivityLibraries table.
        /// </summary>
        public static bool VerifyTestInputLibrariesAreAvailableInDB()
        {
            string sql =
                "select * " +
                "from dbo.ActivityLibrary " +
                "where Name in ('TestInput_Lib1', 'TestInput_Lib2', 'TestInput_Lib3') ";

            DataTable result = TestDataProvider.ExecuteSelect(sql);
            return TestDataProvider.VerifyValuesInResult(result, "Name", "TestInput_Lib1", "TestInput_Lib2", "TestInput_Lib3");
        }

        public static string CreateWorkFlowItemTestData(bool isEmptyWorkFlow = true, string workFlowName = null, string version = "1.0.0.0", string status = "Private", Env environment = Env.Dev)
        {           
            if (workFlowName == null)
                workFlowName = TestUtilities.GenerateRandomString(20);
            string xmalCode = string.Empty;
            if (isEmptyWorkFlow)
                xmalCode = TestUtilities.GetEmptyWorkFlowTemplateXamlCode().Replace("[EmptyWorkflow]", workFlowName);
            else
                xmalCode = TestUtilities.GenerateComplexXmalCode();
            string statusCode="1000";
            if(status=="Private")
                statusCode="1000";
            else if(status=="Public")
                statusCode="1010";
            CreateWorkFlowItemTestDataInDB(workFlowName, version, xmalCode,statusCode,(Int32)environment);
            return workFlowName;
        }


        private static void CreateWorkFlowItemTestDataInDB(string workFlowName, string version, string xmalCode,string statusCode,int environment)
        {
            string ActivitySql =
         " INSERT INTO Activity ([GUID], Name, ShortName, [Description],[Environment], " +
         " MetaTags, IconsId, IsSwitch, IsService, ActivityLibraryId, " +
         " IsUxActivity, CategoryId, ToolboxTab, IsToolBoxActivity, [Version], StatusId, " +
         " WorkflowTypeId, Locked, LockedBy, IsCodeBeside, XAML, DeveloperNotes, BaseType, [Namespace], SoftDelete," +
         " InsertedByUserAlias, InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime) " +
         " VALUES('" + System.Guid.NewGuid() + "', '" + workFlowName + "', '" + workFlowName + "',  'NULL','"+environment+"', 'NULL', 1, 1, 0, @@identity, " +
              "   1, 3, 1, 1, '" + version + "', '"+statusCode+"', " +
                " 2, 0, NULL, 1, '" + xmalCode + "', NULL, '', NULL, 0, " +
                " '" + Utility.GetCurrentUserName() + "', '" + DateTime.Now + "', '" + Utility.GetCurrentUserName() + "', '" + DateTime.Now + "') ";

            string LibrarySql = "  INSERT INTO ActivityLibrary " +
                " ([GUID],[Environment], Name, AuthGroupId, Category, CategoryId, [Executable], HasActivities," +
                " [Description], ImportedBy, VersionNumber, [Status], MetaTags, SoftDelete, InsertedByUserAlias, " +
                " InsertedDateTime, UpdatedByUserAlias, UpdatedDateTime, FriendlyName,ReleaseNotes) " +
                " VALUES('" + System.Guid.NewGuid() + "','"+environment+"', '" + workFlowName + "', 3, '00000000-0000-0000-0000-000000000000', NULL, NULL, 0," +
                " NULL, '" + Utility.GetCurrentUserName() + "', '" + version + "', '"+statusCode+"', NULL, 0, '" + Utility.GetCurrentUserName() + "', '" + System.DateTime.Now + "'," +
                " '" + Utility.GetCurrentUserName() + "', '" + DateTime.Now + "',NULL,NULL)  ";
            try
            {
                TestDataProvider.ExecuteNonQuery(LibrarySql + ActivitySql);
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }

        }


    }
}
