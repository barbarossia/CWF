using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Microsoft.Support.Workflow.Service.Test.Common;
using CWF.DAL;
using Microsoft.DynamicImplementations;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using CWF.DataContracts.Marketplace;
using Microsoft.Support.Workflow.Service.Test.Common;

namespace Microsoft.Support.Workflow.Service.DataAccessServices.Tests.UnitTests
{
    /// <summary>
    /// Summary description for MarketplaceRepositoryServiceShouldDo
    /// </summary>
    [TestClass]
    public class MarketplaceRepositoryServiceShouldDo
    {
        public MarketplaceRepositoryServiceShouldDo()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        /// <summary>
        /// Used to initalize Variation test excution object
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Utility.CopyTestConfigs(Utility.TempLocation);
            Utility.CopyTestConfigs(testContextInstance.TestDeploymentDir);
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [Description("")]
        [Owner("DiffReqTest")]
        [TestCategory("Unit")]
        [TestMethod]
        [ExpectedException(typeof(DataAccessException))]
        public void ReturnDatabaseExceptionIfSqlExceptionOccursWhenIsSearchMarketplaceCalled()
        {
            using (LogSettingConfigIsolator.GetValidLogSettingConfigurationMock()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
            using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                int databaseErrorCode = 50001;
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Execute(
                    delegate
                    {
                        SqlHelper.CauseSqlException(databaseErrorCode);
                        return null;
                    });

                MarketplaceSearchQuery request = CreateMarketplaceGetRequest();
                MarketplaceSearchResult reply = MarketplaceRepositoryService.SearchMarketplace(request);
            }
        }

        [Description("")]
        [Owner("DiffReqTest")]
        [TestCategory("Unit")]
        [TestMethod]
        [ExpectedException(typeof(DataAccessException))]
        public void ReturnDatabaseExceptionIfSqlExceptionOccursWhenIsGetMarketplaceAssetDetailsCalled()
        {
            using (LogSettingConfigIsolator.GetValidLogSettingConfigurationMock()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
            using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
            using (ImplementationOfType impl = new ImplementationOfType(typeof(Database)))
            {
                int databaseErrorCode = 50001;
                impl.Register<Database>(inst => inst.ExecuteReader(Argument<DbCommand>.Any)).Execute(
                    delegate
                    {
                        SqlHelper.CauseSqlException(databaseErrorCode);
                        return null;
                    });

                MarketplaceSearchDetail request = CreateGetMarketplaceAssetDetailsGetRequest();
                MarketplaceAssetDetails reply = MarketplaceRepositoryService.GetMarketplaceAssetDetails(request);
            }
        }

        private static MarketplaceSearchDetail CreateGetMarketplaceAssetDetailsGetRequest()
        {
            MarketplaceSearchDetail request = new MarketplaceSearchDetail();
            request.Id = 0;
            request.AssetType = AssetType.Project;
            return request;
        }

        private static MarketplaceSearchQuery CreateMarketplaceGetRequest()
        {
            MarketplaceSearchQuery request=new MarketplaceSearchQuery();
            request.SearchText = "microsoft";
            request.FilterType = CWF.DataContracts.Marketplace.MarketplaceFilter.Activities;
            request.PageSize = 15;
            request.PageNumber = 1;
            request.UserRole = "Admin";
            request.SortCriteria = new List<CWF.DataContracts.Marketplace.SortCriterion>
                                    {
                                        new CWF.DataContracts.Marketplace.SortCriterion()
                                        {
                                            FieldName="Name",
                                            IsAscending=true,
                                    }};
            request.InAuthGroupNames = new string[] { "pqocwfadmin" };
            return request;
        }

        private static DataTable CreateValidResponseMarketplaceMock(int itemCount)
        {
            DataRow row;
            DataTable table = CreateMarketplaceDataTable();

            for (int i = 0; i < itemCount; i++)
            {
                row = table.NewRow();
                row["Id"] = i;
                row["Name"] = "Name" + i;
                row["InsertedByUserAlias"] = "v-bobzh";
                row["UpdatedByUserAlias"] = "v-bobzh";
                row["UpdatedDateTime"] = DateTime.Now;
                row["Version"] = "1.0.0.0";
                row["AssetType"] = "XAML";
                row["IsTemplate"] = null;
                row["IsPublishingWorkflow"] = null;
                table.Rows.Add(row);
            }
            return table;
        }

        private static DataTable CreateMarketplaceDataTable()
        {
            DataTable table = new DataTable("MarketplaceAsset");
            DataColumn column;

            column = new DataColumn("Id", typeof(Int64));
            table.Columns.Add(column);

            column = new DataColumn("Name", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("InsertedByUserAlias", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("UpdatedByUserAlias", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("UpdatedDateTime", typeof(DateTime));
            table.Columns.Add(column);

            column = new DataColumn("Version", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("AssetType", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("IsTemplate", typeof(String));
            table.Columns.Add(column);

            column = new DataColumn("IsPublishingWorkflow", typeof(String));
            table.Columns.Add(column);

            return table;
        }
    }
}
