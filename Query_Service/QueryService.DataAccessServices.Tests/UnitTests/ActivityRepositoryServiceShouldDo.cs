using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
//using Microsoft.Support.Workflow.Service.Test.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.DynamicImplementations;
using System.Data.Common;
using Microsoft.Support.Workflow.QueryService.Common;
using CWF.DAL;
using Microsoft.Support.Workflow.Service.Common.Logging.Config;
using Microsoft.Support.Workflow.Service.Common.Logging;
using Microsoft.Support.Workflow.Service.Test.Common;
using System.Collections.Generic;
using System;

namespace Microsoft.Support.Workflow.Service.DataAccessServices.Tests.UnitTests
{
        [TestClass]
    public class ActivityRepositoryServiceShouldDo
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// Used to initalize Variation test excution object
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Utility.CopyTestConfigs(Utility.TempLocation);
            Utility.CopyTestConfigs(testContextInstance.TestDeploymentDir);
        }

        [Description("Verifies whether an empty item list reply is returned when the database does not find any items.")]
        [Owner("DiffReqTest")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnEmptyItemsListIfDatabaseDoesNotReturnAnyItemsWhenGetActivitiesByActivityLibraryIsCalled()
        {
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                DataTable table = new DataTable();
                DataReaderMock mockReader = new DataReaderMock(table);
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Return(mockReader);

                CWF.DataContracts.GetLibraryAndActivitiesDC request = CreateActivityLibraryGetRequest();
                List<CWF.DataContracts.GetLibraryAndActivitiesDC> reply = ActivityRepositoryService.GetActivitiesByActivityLibrary(request, true);
                Assert.IsNotNull(reply);
            }
        }

        [Description("Verifies whether a single item returned from the database is included in the reply items list.")]
        [Owner("DiffReqTest")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnActivityLibraryReturnedByDatabaseWhenGetActivityLibrariesIsCalled()
        {
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                DataTable table = CreateValidActivityResponseMock(1);

                DataReaderMock mockReader = new DataReaderMock(table);
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Return(mockReader);

                CWF.DataContracts.GetLibraryAndActivitiesDC request = CreateActivityLibraryGetRequest();
                List<CWF.DataContracts.GetLibraryAndActivitiesDC> reply = ActivityRepositoryService.GetActivitiesByActivityLibrary(request, true);
                Assert.IsNotNull(reply);
            }
        }

        private DataTable CreateValidActivityResponseMock(int itemCount)
        {
            DataRow row;
            DataTable table = CreateActivityLibraryDataTable();

            for (int i = 0; i < itemCount; i++)
            {
                row = table.NewRow();
                row["ActivityCategoryName"] = "ActivityCategoryName";
                row["ActivityLibraryName"] = "ActivityLibraryName";
                row["Name"] = "Name" + i.ToString();
                row["Description"] = "Description";
                row["DeveloperNotes"] = "Activity DeveloperNotes";
                row["Id"] = i;
                row["IsCodeBeside"] = true;
                row["IsService"] = true;
                row["Locked"] = true;
                row["LockedBy"] = "v-sanja";
                row["MetaTags"] = "ActivityLibraryName";
                row["Namespace"] = "ActivityLibraryName";
                row["Guid"] = Guid.NewGuid();
                row["ToolBoxtab"] = 1;
                row["Version"] = "1.0.0.0";
                row["WorkflowTypeName"] = "WorkflowTypeName";
                row["Environment"] = "TEST";
                row["Xaml"] = "XAML";
                table.Rows.Add(row);
            }
            return table;
        }

        private CWF.DataContracts.GetLibraryAndActivitiesDC CreateActivityLibraryGetRequest()
        {
            return new CWF.DataContracts.GetLibraryAndActivitiesDC()
            {
                ActivityLibrary = new CWF.DataContracts.ActivityLibraryDC() 
                {
                    Id = 0,
                    Name = "Test",
                    VersionNumber = "1.0.0.0",
                    Environment = "Test",
                },
            };
        }

        private static DataTable CreateActivityLibraryDataTable()
        {
            DataTable table = new DataTable("Activity");
            DataColumn column;

            column = new DataColumn("ActivityCategoryName", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("ActivityLibraryName", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("Name", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("Description", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("DeveloperNotes", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("Id", typeof(Int32));
            table.Columns.Add(column);

            column = new DataColumn("IsCodeBeside", typeof(Boolean));
            table.Columns.Add(column);

            column = new DataColumn("IsService", typeof(Boolean));
            table.Columns.Add(column);

            column = new DataColumn("Locked", typeof(Boolean));
            table.Columns.Add(column);

            column = new DataColumn("LockedBy", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("MetaTags", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("Namespace", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("Guid", typeof(Guid));
            table.Columns.Add(column);

            column = new DataColumn("ToolBoxtab", typeof(Int32));
            table.Columns.Add(column);

            column = new DataColumn("Version", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("WorkflowTypeName", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("Environment", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("Xaml", typeof(String));
            table.Columns.Add(column);

            return table;
        }
    }
}
