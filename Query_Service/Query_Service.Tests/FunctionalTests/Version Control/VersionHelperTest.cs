using CWF.BAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CWF.DataContracts;
using System.Collections.Generic;
using CWF.BAL.Versioning;
using CWF.DAL;
using CWF.DataContracts.Versioning;
using System.Linq;
using Query_Service.Tests.Common;
using Microsoft.DynamicImplementations;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using Microsoft.Support.Workflow.Service.DataAccessServices;

namespace Query_Service.Tests
{
    /// <summary>
    ///This is a test class for VersionHelperTest and is intended
    ///to contain all VersionHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VersionHelperTest
    {
        private const string TestVersionString = "1.2.3.4";
        private const string Incaller = "testuser";
        private const string InCallerVersion = "1.0.0.0";
        private const string Owner = "testuser";
        private const string UPDATEDBYUSERALIAS = "testuser";
        private const string InsertedByUserAlias = "testuser";
        private const int DefaultTestStringLength = 10;
        private const string DefaultStatusCodeName = "Private";
        private const string DefaultAuthgroupName = "pqocwfadmin";
        private const string DefaultIconsName = "TEST";
        private const string DefaultWorkFlowTypeName = "Workflow";
        private const string DefaultActivityCategoryName = "Unassigned";
        private const string DummyWorkflowXaml = "<root/>";
        private const int TestDependenciesCount = 10; // number of dummy dependencies to create when a test needs them
        private const string TestOwner = "v-richt";

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///A test for GetVersionSection
        ///</summary>
        [Description("Verify GetVersionSection returns the correct section value for a version object.")]
        [Owner(TestOwner)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void GetVersionSectionTest()
        {
            var versionStringSections = TestVersionString.Split('.');

            Assert.AreEqual(VersionHelper_Accessor.GetVersionSection(TestVersionString, Section.Major).ToString(), versionStringSections[0]);
            Assert.AreEqual(VersionHelper_Accessor.GetVersionSection(TestVersionString, Section.Minor).ToString(), versionStringSections[1]);
            Assert.AreEqual(VersionHelper_Accessor.GetVersionSection(TestVersionString, Section.Build).ToString(), versionStringSections[2]);
            Assert.AreEqual(VersionHelper_Accessor.GetVersionSection(TestVersionString, Section.Revision).ToString(), versionStringSections[3]);
        }

        /// <summary>
        ///A test for GetWorkflowRecordState
        ///</summary>
        [Description("Verify GetWorkflowRecordState returns the correct enum value for the string passed.")]
        [Owner(TestOwner)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void GetWorkflowRecordStateTest()
        {
            // when passed various strings, GetWorkflowRecordState should return the appropriate WorkflowRecordState 
            Assert.AreEqual(VersionHelper.GetWorkflowRecordState("Private"), WorkflowRecordState.Private);
            Assert.AreEqual(VersionHelper.GetWorkflowRecordState("private"), WorkflowRecordState.Private);
            Assert.AreEqual(VersionHelper.GetWorkflowRecordState("anything else not recognized"), WorkflowRecordState.Public);

            Assert.AreEqual(VersionHelper.GetWorkflowRecordState("Public"), WorkflowRecordState.Public);
            Assert.AreEqual(VersionHelper.GetWorkflowRecordState("Retired"), WorkflowRecordState.Retired);
        }


        /// <summary>
        /// Creates a record that we can then check versioning against
        /// </summary>
        /// <param name="name">Name of the workflow to be saved.</param>
        /// <param name="version">The version number of the workflow.</param>
        /// <returns>The workflow saved.</returns>
        private StoreActivitiesDC CreateTestStoreActivity(string name, string version, bool saveToDatabase)
        {
            var newRecord = new StoreActivitiesDC
           {
               Incaller = Incaller,
               IncallerVersion = InCallerVersion,
               InInsertedByUserAlias = InsertedByUserAlias,
               Version = version,
               InUpdatedByUserAlias = UPDATEDBYUSERALIAS,
               Guid = Guid.NewGuid(),
               Name = name,
               Description = name + " DESCRIPTION",
               IsService = false,
               WorkflowTypeID = 1,
               Xaml = DummyWorkflowXaml,
               UpdatedDateTime = DateTime.Now,
               ShortName = name,

               MetaTags = new String('A', DefaultTestStringLength),
               ActivityCategoryName = DefaultActivityCategoryName,
               WorkflowTypeName = DefaultWorkFlowTypeName,
               AuthGroupName = DefaultAuthgroupName,
               StatusCodeName = DefaultStatusCodeName,

               WorkflowRecordState = WorkflowRecordState.Private,         
               InAuthGroupNames = new string[] { DefaultAuthgroupName },
               Environment = "Dev"
           };

            if (saveToDatabase)
                Activities.StoreActivitiesCreateOrUpdate(newRecord);

            return newRecord;
        }

        private void DeleteAllTestWorkflows(string workflowName)
        {
            Activities.StoreActivitiesGetByName(workflowName, Incaller)
                .ForEach(workflow =>
                {
                    var result = Activities.StoreActivitiesDelete(workflow);
                });
        }

        ///<summary>
        ///A test for GetNextVersion
        ///</summary>
        [Description("Verify GetNextVersion() returns the correct next version number.")]
        [Owner(TestOwner)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void GetNextVersionTest()
        {
            var workflowName = "Test " + Guid.NewGuid().ToString();
            var workflow = CreateTestStoreActivity(workflowName, InCallerVersion, false);
            Version result;

            //there is no record in the database yet, and no version is specified
            result = VersionHelper.GetNextVersion(workflow);
            Assert.AreEqual(new Version(0, 0, 0, 1), result);

            workflow = CreateTestStoreActivity(workflowName, "1.0.0.0", true);
            workflow.WorkflowRecordState = WorkflowRecordState.Private;
            result = VersionHelper.GetNextVersion(workflow);

            Assert.AreEqual(new Version(1, 0, 0, 1), result);                     // we're saving as private, so the revision should update
            workflow.Version = result.ToString();                                 // update the workflow we are testing with 
            workflow = CreateTestStoreActivity(workflowName, result.ToString(), true);    // save it - save should work fine

            // once more -- the result should have a revision of 2 this time
            result = VersionHelper.GetNextVersion(workflow);
            Assert.AreEqual(new Version(1, 0, 0, 2), result);
            

            workflow = CreateTestStoreActivity(workflowName, new Version(1, 0, 1, 2).ToString(), true);
            Assert.AreEqual(new Version(1, 0, 1, 2).ToString(), workflow.Version);

            // pretend we compiled
            result = VersionHelper.GetNextVersion(workflow);
            Assert.AreEqual(new Version(1, 0, 1, 3), result);

            workflow = CreateTestStoreActivity(workflowName, new Version(2, 0, 0, 0).ToString(), true);
            Assert.AreEqual(new Version(2, 0, 0, 0).ToString(), workflow.Version);

            // changing the major version should work OK, as well
            result = VersionHelper.GetNextVersion(workflow);
            Assert.AreEqual(new Version(2, 0, 0, 1), result);

            DeleteAllTestWorkflows(workflowName); // note - this only does a soft delete. We don't have a stored proc that actually removes these records
        }


        /// <summary>
        ///A test for CheckVersioningRules
        ///</summary>
        [Description("Verify CheckVersioningRules() returns the correct pass/fail indicaitons for the conditions supplied.")]
        [Owner(TestOwner)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void CheckVersioningRulesTest()
        {
            var workflowName = "Test " + Guid.NewGuid().ToString();
            Tuple<bool, string, Rule> result;
            var workflow = CreateTestStoreActivity(workflowName, "1.2.3.4", true);


            // check changing the revision
            workflow.Version = "1.2.3.5";
            workflow.WorkflowRecordState = WorkflowRecordState.Private;
            result = VersionHelper.CheckVersioningRules(workflow, RequestedOperation.Update, Incaller);
            Assert.IsTrue(result.Item1);

            // check changing the build (revision must also be updated)
            workflow.Version = "1.2.4.5";
            workflow.WorkflowRecordState = WorkflowRecordState.Private;
            result = VersionHelper.CheckVersioningRules(workflow, RequestedOperation.Update, Incaller);
            Assert.IsTrue(result.Item1);

            // check changing the minor (revision must also be updated)
            workflow.Version = "1.3.3.5";
            workflow.WorkflowRecordState = WorkflowRecordState.Private;
            result = VersionHelper.CheckVersioningRules(workflow, RequestedOperation.Update, Incaller);
            Assert.IsTrue(result.Item1);

            // check changing the major (revision must also be updated)
            workflow.Version = "2.0.0.0";
            workflow.WorkflowRecordState = WorkflowRecordState.Private;
            result = VersionHelper.CheckVersioningRules(workflow, RequestedOperation.Update, Incaller);
            Assert.IsTrue(result.Item1);

            DeleteAllTestWorkflows(workflowName); // note - this only does a soft delete. We don't have a stored proc that actually removes these records
        }

        /// <summary>
        /// Tests to make sure the rules for dependent activities are correct.
        /// </summary>
        [Description("Verify GetVersionSection returns the correct section value for a version object.")]
        [Owner(TestOwner)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void TestCheckDependencyRules()
        {
            var workflowName = "Test " + Guid.NewGuid().ToString();
            var testWorkflow = CreateTestStoreActivity(workflowName, TestVersionString, false);

            var request = new StoreLibraryAndActivitiesRequestDC();

            request.StoreActivitiesList = new List<StoreActivitiesDC>();
            request.StoreActivitiesList.Add(testWorkflow);

            request.StoreActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC();

            Action createTree = () =>
            {
                // create a dependency tree 3 levels deep, 10 items wide     
                request.StoreActivityLibraryDependenciesGroupsRequestDC.List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>();
                CreateTestDependencies(request.StoreActivityLibraryDependenciesGroupsRequestDC.List);

                request
                    .StoreActivityLibraryDependenciesGroupsRequestDC
                    .List
                    .ForEach(dependency =>
                    {
                        dependency.List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>();
                        CreateTestDependencies(dependency.List);

                        dependency
                           .List
                           .ForEach(dependency2 =>
                           {
                               dependency2.List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>();
                               CreateTestDependencies(dependency2.List);
                           });
                    });
            };

            // everything should be Public for a new tree. The test should pass.
            createTree();
            testWorkflow.StatusCodeName = "Public";
            Assert.IsTrue(VersionHelper.CheckDependencyRules(request).Item1, "Expected all Public to pass the test");

            // with one item in the tree as private, the rule should fail.
            testWorkflow.StatusCodeName = "Public";
            createTree();
            request
                .StoreActivityLibraryDependenciesGroupsRequestDC
                .List[5]
                .List[7].Status = "Private";   // set an arbitrary item to Private
            Assert.IsFalse(VersionHelper.CheckDependencyRules(request).Item1, "Expected all Public but with one set to Private to fail the test.");

            // with one item in the tree as private, the rule should fail.
            testWorkflow.StatusCodeName = "Public";
            createTree();
            request
                .StoreActivityLibraryDependenciesGroupsRequestDC
                .List[2]
                .List[7].Status = "Retired";   // set an arbitrary item to Retired
            Assert.IsFalse(VersionHelper.CheckDependencyRules(request).Item1, "Expected all Public but with one set to Retired to fail the test.");
        }

        private static void CreateTestDependencies(List<StoreActivityLibraryDependenciesGroupsRequestDC> list)
        {
            for (int i = 0; i < TestDependenciesCount; i++)
                list.Add(new StoreActivityLibraryDependenciesGroupsRequestDC
                {
                    Status = WorkflowRecordState.Public.ToString(),

                    // make sure there are some name collisions -- the VersionHelper routine should
                    // not have any problems with this.
                    Name = i % 3 == 0 ? Guid.Empty.ToString() : Guid.NewGuid().ToString(),
                });
        }

        [Description("Verify GetMissingDependencyStates() gets the correct values from the database.")]
        [Owner("DiffReqTest")]//[Owner(TestOwner)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void TestFillInMissingStatuses()
        {
            var workflowName = "Test " + Guid.NewGuid().ToString();
            var testWorkflow = CreateTestStoreActivity(workflowName, TestVersionString, false);
            var testDependencyName = "Test Dependency " + Guid.NewGuid().ToString();
            //var testDependency = CreateTestStoreActivity(testDependencyName, TestVersionString, true);

            var request = new StoreLibraryAndActivitiesRequestDC();

            request.StoreActivitiesList = new List<StoreActivitiesDC>();
            request.StoreActivitiesList.Add(testWorkflow);

            request.StoreActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC();

            request.StoreActivityLibraryDependenciesGroupsRequestDC.List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>();
            CreateTestDependencies(request.StoreActivityLibraryDependenciesGroupsRequestDC.List);

            var item = request.StoreActivityLibraryDependenciesGroupsRequestDC.List[1];

            item.Status = String.Empty;
            item.Name = testDependencyName;
            item.Version = "1.0.0.0";

            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                var table = new System.Data.DataTable();
                DataReaderMock mockReader;

                table.Columns.Add("AuthGroupId", typeof(int));
                table.Columns.Add("AuthGroupName", typeof(string));
                table.Columns.Add("Category", typeof(Guid));
                table.Columns.Add("CategoryId", typeof(int));
                table.Columns.Add("Description", typeof(string));
                table.Columns.Add("Executable", typeof(byte[]));
                table.Columns.Add("Guid", typeof(Guid));
                table.Columns.Add("HasActivities", typeof(bool));
                table.Columns.Add("Id", typeof(string));
                table.Columns.Add("ImportedBy", typeof(string));
                table.Columns.Add("MetaTags", typeof(string));
                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("Status", typeof(int));
                table.Columns.Add("StatusName", typeof(string));
                table.Columns.Add("VersionNumber", typeof(string));
                table.Columns.Add("FriendlyName", typeof(string));
                table.Columns.Add("ReleaseNotes", typeof(string));
                table.Columns.Add("Environment", typeof(string));
                table.Rows.Add(  
                                0,
                                String.Empty,
                                Guid.Empty,
                                0,
                                String.Empty,
                                DBNull.Value,
                                Guid.Empty,
                                false,
                                0,
                                String.Empty,
                                String.Empty,
                                testDependencyName,
                                0,
                                DefaultStatusCodeName,
                                String.Empty,
                                String.Empty,
                                String.Empty,
                                "Dev"
                              );

                mockReader = new DataReaderMock(table);

                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Return(mockReader);

                VersionHelper.GetMissingDependencyStates(request.StoreActivityLibraryDependenciesGroupsRequestDC.List);

                Assert.IsFalse(request
                                 .StoreActivityLibraryDependenciesGroupsRequestDC
                                 .List
                                 .Any(resultItem => string.IsNullOrEmpty(item.Status)),
                                 "All items should have their status property filled in.");

            }

        }


        [Description("Some valid version numbers (as defined by passing Version.Parse) are not valid for our purposes Test some valid versions that are not valid Marketplace versions.")]
        [Owner(TestOwner)]
        [TestCategory(TestCategory.Func)]
        [TestMethod]
        public void TestValidVersionsButInvalidMarketplaceVersions()
        {
            Assert.IsFalse(VersionHelper.IsValidMarketplaceVersion("0.0.0"));
            Assert.IsFalse(VersionHelper.IsValidMarketplaceVersion("9999.99.99.99"));
            Assert.IsFalse(VersionHelper.IsValidMarketplaceVersion("1.0"));
            Assert.IsFalse(VersionHelper.IsValidMarketplaceVersion("1"));
            Assert.IsFalse(VersionHelper.IsValidMarketplaceVersion("1.1.1.1.1.1"));

            // Test some valid ones...
            Assert.IsTrue(VersionHelper.IsValidMarketplaceVersion("1.1.1.1"));
            Assert.IsTrue(VersionHelper.IsValidMarketplaceVersion("1.0.0.0"));
            Assert.IsTrue(VersionHelper.IsValidMarketplaceVersion("0.1.0.0"));
            Assert.IsTrue(VersionHelper.IsValidMarketplaceVersion("0.1.0.99"));
        }
    }
}
