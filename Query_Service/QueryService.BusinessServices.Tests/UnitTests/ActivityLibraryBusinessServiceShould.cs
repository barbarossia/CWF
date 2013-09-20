using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Service.BusinessServices;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using CWF.DataContracts;
using Microsoft.Support.Workflow.Service.Test.Common;
using CWF.DAL;
using Microsoft.Support.Workflow.QueryService.Common;
using Microsoft.Support.Workflow.Service.Common.Logging.Config;
using Microsoft.Support.Workflow.Service.Common.Logging;
using System.Configuration;
using System.Diagnostics;

namespace Microsoft.Support.Workflow.Service.BusinessServices.Tests.UnitTests
{
    [TestClass]
    public class ActivityLibraryBusinessServiceShould
    {
        [Description("Verifies whether an empty item list  returned from DAL is also returned from BAL.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnEmptyItemsListIfDalReturnedEmptyItemsWhenGetActivityLibrariesIsCalled()
        {
            List<CWF.DataContracts.ActivityLibraryDC> list = CreateActivityLibraryGetReturnItems(0);
            using (ImplementationOfType impl = ActivityLibraryRepositoryServiceIsolator.GetActivityLibrariesSuccessResponseMock(list))
            {
                CWF.DataContracts.ActivityLibraryDC request = CreateActivityLibraryGetRequest();
                List<CWF.DataContracts.ActivityLibraryDC> reply = ActivityLibraryBusinessService.GetActivityLibraries(request);
                Assert.IsNotNull(reply);
                Assert.AreEqual(0, reply.Count);
            }
        }

        [Description("Verifies whether a single item returned from the DAL is also returned from BAL.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnSingleActivityLibraryIfDalReturnedSingleItemWhenGetActivityLibrariesIsCalled()
        {
            int itemCount = 1;
            List<CWF.DataContracts.ActivityLibraryDC> list = CreateActivityLibraryGetReturnItems(itemCount);
            using (ImplementationOfType impl = ActivityLibraryRepositoryServiceIsolator.GetActivityLibrariesSuccessResponseMock(list))
            {
                CWF.DataContracts.ActivityLibraryDC request = CreateActivityLibraryGetRequest();
                List<CWF.DataContracts.ActivityLibraryDC> reply = ActivityLibraryBusinessService.GetActivityLibraries(request);

                Assert.IsNotNull(reply);
                Assert.AreEqual(itemCount, reply.Count);

                for (int i = 0; i < itemCount; i++)
                {
                    Assert.AreEqual(list[i].AuthGroupName, reply[i].AuthGroupName);
                    Assert.IsNotNull(reply[i].Executable);
                    Assert.AreEqual((list[i].Executable).Length, reply[i].Executable.Length);
                }
            }
        }

        [Description("Verifies whether a single item returned from the DAL is also returned from BAL.")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnMultipleActivityLibrariesIfDalReturnedMultipleItemsWhenGetActivityLibrariesIsCalled()
        {
            int itemCount = 10;
            List<CWF.DataContracts.ActivityLibraryDC> list = CreateActivityLibraryGetReturnItems(itemCount);
            using (ImplementationOfType impl = ActivityLibraryRepositoryServiceIsolator.GetActivityLibrariesSuccessResponseMock(list))
            {
                CWF.DataContracts.ActivityLibraryDC request = CreateActivityLibraryGetRequest();
                List<CWF.DataContracts.ActivityLibraryDC> reply = ActivityLibraryBusinessService.GetActivityLibraries(request);

                Assert.IsNotNull(reply);
                Assert.AreEqual(itemCount, reply.Count);

                for (int i = 0; i < itemCount; i++)
                {
                    Assert.AreEqual(list[i].AuthGroupName, reply[i].AuthGroupName);
                    Assert.IsNotNull(reply[i].Executable);
                    Assert.AreEqual((list[i].Executable).Length, reply[i].Executable.Length);
                }
            }
        }

        [Description("")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        [ExpectedException(typeof(BusinessException))]
        public void ThrowBusinessExceptionIfDatabaseExceptionOccursWhenGetActivityLibrariesIsCalled()
        {
            int eventCode = EventCode.DatabaseEvent.Validation.ActivityLibraryNotFound;
            DataAccessException exception = new DataAccessException(eventCode);
            using (LogSettingConfigIsolator.GetValidLogSettingConfigurationMock()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
            using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
            using (ImplementationOfType impl = ActivityLibraryRepositoryServiceIsolator.GetActivityLibrariesExceptionResponseMock(exception))
            {
                CWF.DataContracts.ActivityLibraryDC request = CreateActivityLibraryGetRequest();
                List<CWF.DataContracts.ActivityLibraryDC> reply = ActivityLibraryBusinessService.GetActivityLibraries(request);
            }
        }

        [Description("")]
        [Owner("DiffReqTest")]//[Owner("v-sanja")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ReturnBusinessExceptionWithMatchingErrorCodeReturnedWithDatabaseExceptionWhenGetActivityLibrariesIsCalled()
        {
            int eventCode = EventCode.DatabaseEvent.Validation.ActivityLibraryNotFound;
            DataAccessException exception = new DataAccessException(eventCode);
            using (LogSettingConfigIsolator.GetValidLogSettingConfigurationMock()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
            using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
            using (ImplementationOfType impl = ActivityLibraryRepositoryServiceIsolator.GetActivityLibrariesExceptionResponseMock(exception))
            {
                CWF.DataContracts.ActivityLibraryDC request = CreateActivityLibraryGetRequest();
                try
                {
                    List<CWF.DataContracts.ActivityLibraryDC> reply = ActivityLibraryBusinessService.GetActivityLibraries(request);
                }
                catch (BusinessException e)
                {
                    Assert.AreEqual(eventCode, e.ErrorCode);
                }
            }
        }

        [Description("")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void InValidateRequestActivityLibraryBusinessServiceGetActivityLibrariesWhichActivityLibraryIsNull()
        {
            int eventCode = EventCode.BusinessLayerEvent.Validation.RequestIsNull;
            try
            {
                using (LogSettingConfigIsolator.GetValidLogSettingConfigurationInstance()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
                using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
                {
                    var reply = ActivityLibraryBusinessService.GetActivityLibraries(null);
                }
            }
            catch (BusinessException e)
            {
                Assert.AreEqual(eventCode, e.ErrorCode);
            }
        }

        [Description("")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void InValidateRequestActivityLibraryBusinessServiceGetActivityLibrariesWhichActivityLibraryInValidateInCaller()
        {
            int eventCode = EventCode.BusinessLayerEvent.Validation.CallerNameRequired;
            try
            {
                using (LogSettingConfigIsolator.GetValidLogSettingConfigurationInstance()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
                using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
                {
                    var request = CreateActivityLibraryGetRequest();
                    request.Incaller = null;
                    var reply = ActivityLibraryBusinessService.GetActivityLibraries(request);
                }
            }
            catch (BusinessException e)
            {
                Assert.AreEqual(eventCode, e.ErrorCode);
            }
        }

        [Description("")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void InValidateRequestActivityLibraryBusinessServiceGetActivityLibrariesWhichActivityLibraryInValidateInCallerVersion()
        {
            int eventCode = EventCode.BusinessLayerEvent.Validation.CallerVersionRequired;
            try
            {
                using (LogSettingConfigIsolator.GetValidLogSettingConfigurationInstance()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
                using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
                {
                    var request = CreateActivityLibraryGetRequest();
                    request.IncallerVersion = null;
                    var reply = ActivityLibraryBusinessService.GetActivityLibraries(request);
                }
            }
            catch (BusinessException e)
            {
                Assert.AreEqual(eventCode, e.ErrorCode);
            }
        }

        private static CWF.DataContracts.ActivityLibraryDC CreateActivityLibraryGetRequest()
        {
            CWF.DataContracts.ActivityLibraryDC request;
            request = new CWF.DataContracts.ActivityLibraryDC();
            request.Incaller = "v-sanja";
            request.IncallerVersion = "1.0.0.0";
            return request;
        }

        private static List<ActivityLibraryDC> CreateActivityLibraryGetReturnItems(int itemCount)
        {
            List<ActivityLibraryDC> items = new List<ActivityLibraryDC>();
            for (int i = 0; i < itemCount; i++)
            {
                items.Add(new ActivityLibraryDC
                {
                    AuthGroupId = i,
                    AuthGroupName = "AuthGroupName" + i,
                    Category = Guid.NewGuid(),
                    CategoryId = i,
                    CategoryName = "CategoryName" + i,
                    Description = "Description" + i,
                    Executable = new byte[10],
                    Guid = Guid.NewGuid(),
                    HasActivities = true,
                    Id = i,
                    ImportedBy = "v-sanja",
                    InInsertedByUserAlias = "v-sanja",
                    MetaTags = "MetaTags" + i,
                    Name = "Name" + i,
                    Status = i,
                    StatusName = "StatusName" + i,
                    VersionNumber = "1.0.0." + i,
                    InUpdatedByUserAlias = "v-sanja"
                });
            }
            return items;
        }
    }
}
