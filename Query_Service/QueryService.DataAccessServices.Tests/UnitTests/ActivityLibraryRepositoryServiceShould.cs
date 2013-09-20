using System;
using System.Text;
using System.Collections.Generic;
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

namespace Microsoft.Support.Workflow.Service.DataAccessServices.Tests.UnitTests
{
    [TestClass]
    public class ActivityLibraryRepositoryServiceShould
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
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnEmptyItemsListIfDatabaseDoesNotReturnAnyItemsWhenGetActivityLibrariesIsCalled()
        {
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                DataTable table = new DataTable();
                DataReaderMock mockReader = new DataReaderMock(table);
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Return(mockReader);

                CWF.DataContracts.ActivityLibraryDC request = CreateActivityLibraryGetRequest();
                List<CWF.DataContracts.ActivityLibraryDC> reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
                Assert.IsNotNull(reply);
                Assert.AreEqual(0, reply.Count);
            }
        }

        [Description("Verifies whether a single item returned from the database is included in the reply items list.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnActivityLibraryReturnedByDatabaseWhenGetActivityLibrariesIsCalled()
        {
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                DataTable table = CreateValidActivityLibraryResponseMock(1);

                DataReaderMock mockReader = new DataReaderMock(table);
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Return(mockReader);

                CWF.DataContracts.ActivityLibraryDC request = CreateActivityLibraryGetRequest();
                List<CWF.DataContracts.ActivityLibraryDC> reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
                Assert.IsNotNull(reply);
                Assert.AreEqual(1, reply.Count);
                Assert.AreEqual(table.Rows[0][DataFieldName.ActivityLibrary.AuthGroupName], reply[0].AuthGroupName);
                Assert.IsNotNull(reply[0].Executable);
                Assert.AreEqual(((Byte[])table.Rows[0][DataFieldName.ActivityLibrary.Executable]).Length, reply[0].Executable.Length);
            }
        }

        [Description("Verifies whether a response with a number of items is returned corresponding to the number of items found by the database.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnMultipleActivityLibrariesIfDatabaseFindsMultipleResultsWhenGetActivityLibrariesIsCalled()
        {
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                // Simulate 10 items being returned by the database.
                int numberOfMatches = 10;
                DataTable table = CreateValidActivityLibraryResponseMock(numberOfMatches);

                DataReaderMock mockReader = new DataReaderMock(table);
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Return(mockReader);

                CWF.DataContracts.ActivityLibraryDC request = CreateActivityLibraryGetRequest();
                List<CWF.DataContracts.ActivityLibraryDC> reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
                Assert.IsNotNull(reply);
                Assert.AreEqual(numberOfMatches, reply.Count);

                for (int i = 0; i < numberOfMatches; i++)
                {
                    Assert.AreEqual(table.Rows[i][DataFieldName.ActivityLibrary.AuthGroupName], reply[i].AuthGroupName);
                    Assert.IsNotNull(reply[i].Executable);
                    Assert.AreEqual(((Byte[])table.Rows[i][DataFieldName.ActivityLibrary.Executable]).Length, reply[i].Executable.Length);
                }
            }
        }

        [Description("Verifies whether the executable binary is not returned if not requested.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void DoNotReturnExecutableIfNotRequestedWhenGetActivityLibrariesIsCalled()
        {
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                // Simulate 10 items being returned by the database.
                int numberOfMatches = 10;
                DataTable table = CreateValidActivityLibraryResponseMock(numberOfMatches);

                DataReaderMock mockReader = new DataReaderMock(table);
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Return(mockReader);

                CWF.DataContracts.ActivityLibraryDC request = CreateActivityLibraryGetRequest();
                List<CWF.DataContracts.ActivityLibraryDC> reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, false);
                Assert.IsNotNull(reply);
                Assert.AreEqual(numberOfMatches, reply.Count);

                for (int i = 0; i < numberOfMatches; i++)
                {
                    Assert.AreEqual(table.Rows[i][DataFieldName.ActivityLibrary.AuthGroupName], reply[i].AuthGroupName);
                    Assert.IsNull(reply[i].Executable);
                }
            }
        }

        [Description("")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        [ExpectedException(typeof(DataAccessException))]
        public void ReturnDatabaseExceptionIfSqlExceptionOccursWhenGetActivityLibrariesIsCalled()
        {
            using (LogSettingConfigIsolator.GetValidLogSettingConfigurationMock()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
            using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                int databaseErrorCode = 50001;
                DataTable table = CreateValidActivityLibraryResponseMock(3);
                DataReaderMock mockReader = new DataReaderMock(table);
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Execute(
                    delegate
                    {
                        CauseSqlException(databaseErrorCode);
                        return null;
                    });

                CWF.DataContracts.ActivityLibraryDC request = CreateActivityLibraryGetRequest();
                List<CWF.DataContracts.ActivityLibraryDC> reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
            }
        }

        [Description("")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnDatabaseExceptionWithMatchingErrorCodeReturnedWithSqlExceptionWhenGetActivityLibrariesIsCalled()
        {
            using (LogSettingConfigIsolator.GetValidLogSettingConfigurationMock()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
            using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                int databaseErrorCode = 50001;
                DataTable table = CreateValidActivityLibraryResponseMock(3);
                DataReaderMock mockReader = new DataReaderMock(table);
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Execute(
                    delegate
                    {
                        CauseSqlException(databaseErrorCode);
                        return null;
                    });


                try
                {
                    CWF.DataContracts.ActivityLibraryDC request = CreateActivityLibraryGetRequest();
                    List<CWF.DataContracts.ActivityLibraryDC> reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
                }
                catch (DataAccessException e)
                {
                    Assert.IsTrue(e.ErrorCode > 0);
                }
            }
        }

        private static CWF.DataContracts.ActivityLibraryDC CreateActivityLibraryGetRequest()
        {
            CWF.DataContracts.ActivityLibraryDC request;
            request = new CWF.DataContracts.ActivityLibraryDC();
            request.Incaller = "v-sanja";
            request.IncallerVersion = "1.0.0.0";
            request.InAuthGroupNames = new string[] { "pqocwfadmin" };
            request.Environment = "Test";
            return request;
        }

        private static void CauseSqlException(int errorNumber)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DbCommand cmd = db.GetSqlStringCommand(String.Format("RAISERROR('<ErrorNumber>{0}</ErrorNumber>Manual SQL exception', 16, 1)", errorNumber));
            db.ExecuteNonQuery(cmd);
        }

        private static DataTable CreateValidActivityLibraryResponseMock(int itemCount)
        {
            DataRow row;
            DataTable table = CreateActivityLibraryDataTable();

            for (int i = 0; i < itemCount; i++)
            {
                row = table.NewRow();
                row[DataFieldName.ActivityLibrary.AuthGroupId] = 1;
                row[DataFieldName.ActivityLibrary.AuthGroupName] = "AuthGroupName" + i;
                row[DataFieldName.ActivityLibrary.Category] = Guid.NewGuid();
                row[DataFieldName.ActivityLibrary.CategoryId] = 2;
                row[DataFieldName.ActivityLibrary.Description] = "Activity library description";
                row[DataFieldName.ActivityLibrary.Guid] = Guid.NewGuid();
                row[DataFieldName.ActivityLibrary.HasActivities] = true;
                row[DataFieldName.ActivityLibrary.Id] = 10;
                row[DataFieldName.ActivityLibrary.ImportedBy] = "v-sanja";
                row[DataFieldName.ActivityLibrary.MetaTags] = "ActivityLibraryMetaTags" + 1;
                row[DataFieldName.ActivityLibrary.Name] = "ActivityLibraryName";
                row[DataFieldName.ActivityLibrary.Status] = 2;
                row[DataFieldName.ActivityLibrary.VersionNumber] = "1.0.0.1";
                row[DataFieldName.ActivityLibrary.Executable] = new byte[10];
                row[DataFieldName.ActivityLibrary.Environment] = "TEST";
                table.Rows.Add(row);
            }
            return table;
        }

        private static DataTable CreateActivityLibraryDataTable()
        {
            DataTable table = new DataTable("ActivityLibraries");
            DataColumn column;

            column = new DataColumn(DataFieldName.ActivityLibrary.AuthGroupId, typeof(Int32));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.AuthGroupName, typeof(String));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.Category, typeof(Guid));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.CategoryId, typeof(Int32));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.Description, typeof(String));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.Guid, typeof(Guid));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.HasActivities, typeof(Boolean));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.Id, typeof(Int32));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.ImportedBy, typeof(String));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.MetaTags, typeof(String));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.Name, typeof(String));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.Status, typeof(Int32));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.StatusName, typeof(String));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.VersionNumber, typeof(String));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.Executable, typeof(Byte[]));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.FriendlyName, typeof(String[]));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.ReleaseNotes, typeof(String[]));
            table.Columns.Add(column);

            column = new DataColumn(DataFieldName.ActivityLibrary.Environment, typeof(String));
            table.Columns.Add(column);

            return table;
        }
    }
}
