using CWF.DAL;
using Microsoft.DynamicImplementations;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Support.Workflow.Service.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Microsoft.Support.Workflow.Service.DataAccessServices.Tests.UnitTests
{
    [TestClass]
    public class ActivitesShouldDo
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

        [Description("")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        [ExpectedException(typeof(DataAccessException))]
        public void ReturnDatabaseExceptionIfSqlExceptionOccursWhenStoreActivitiesCreateOrUpdateIsCalled()
        {
            using (LogSettingConfigIsolator.GetValidLogSettingConfigurationMock()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
            using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                int databaseErrorCode = 50001;
                DataTable table = CreateValidActivityResponseMock(1);
                DataReaderMock mockReader = new DataReaderMock(table);
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Execute(
                    delegate
                    {
                        SqlHelper.CauseSqlException(databaseErrorCode);
                        return null;
                    });

                CWF.DataContracts.StoreActivitiesDC request = CreateStoreActivitiesDC();
                CWF.DataContracts.StoreActivitiesDC reply = Activities.StoreActivitiesCreateOrUpdate(request);
            }
        }

        [Description("")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        [ExpectedException(typeof(DataAccessException))]
        public void ReturnDatabaseExceptionIfSqlExceptionOccursWhenStoreActivitiesGetIsCalled()
        {
            using (LogSettingConfigIsolator.GetValidLogSettingConfigurationMock()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
            using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                int databaseErrorCode = 50001;
                DataTable table = CreateValidActivityResponseMock(1);
                DataReaderMock mockReader = new DataReaderMock(table);
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Execute(
                    delegate
                    {
                        SqlHelper.CauseSqlException(databaseErrorCode);
                        return null;
                    });

                CWF.DataContracts.StoreActivitiesDC request = CreateStoreActivitiesDC();
                List<CWF.DataContracts.StoreActivitiesDC> reply = Activities.StoreActivitiesGet(request);
            }
        }

        [Description("")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        [ExpectedException(typeof(DataAccessException))]
        public void ReturnDatabaseExceptionIfSqlExceptionOccursWhenStoreActivitiesGetByNameIsCalled()
        {
            using (LogSettingConfigIsolator.GetValidLogSettingConfigurationMock()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
            using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                int databaseErrorCode = 50001;
                DataTable table = CreateValidActivityResponseMock(1);
                DataReaderMock mockReader = new DataReaderMock(table);
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Execute(
                    delegate
                    {
                        SqlHelper.CauseSqlException(databaseErrorCode);
                        return null;
                    });

                List<CWF.DataContracts.StoreActivitiesDC> reply = Activities.StoreActivitiesGetByName("Test", "Test");
            }
        }
       
        private CWF.DataContracts.StoreActivitiesDC CreateStoreActivitiesDC()
        {
            return new CWF.DataContracts.StoreActivitiesDC()
            {
                Incaller = "v-sanja",
                IncallerVersion = "1.0.0.0",
                InAuthGroupNames = new string[] { "pqocwfadmin" },
                Environment = "Test",
                Name = "Test",
                Version = "1.0.0.0",
                Locked = true,
                LockedBy = "v-bobzh",
                InInsertedByUserAlias ="v-bobzh",
                InUpdatedByUserAlias = "v-bobzh",
            };
        }

        private CWF.DataContracts.ChangeAuthorRequest CreateChangeAuthorRequest()
        {
            return new CWF.DataContracts.ChangeAuthorRequest()
            {
                Incaller = "v-sanja",
                IncallerVersion = "1.0.0.0",
                InAuthGroupNames = new string[] { "pqocwfadmin" },
                Environment = "Test",
                Name = "Test",
                Version = "1.0.0.0",
                AuthorAlias = "test",
                InInsertedByUserAlias = "v-bobzh",
                InUpdatedByUserAlias = "v-bobzh",
            };
        }

        private DataTable CreateValidActivityResponseMock(int itemCount)
        {
            DataRow row;
            DataTable table = CreateActivityDataTable();

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

        private static DataTable CreateActivityDataTable()
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
