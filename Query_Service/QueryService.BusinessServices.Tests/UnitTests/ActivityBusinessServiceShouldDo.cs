using Microsoft.Support.Workflow.QueryService.Common;
using Microsoft.Support.Workflow.Service.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DAL;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using CWF.DataContracts;
using System.Collections.Generic;

namespace Microsoft.Support.Workflow.Service.BusinessServices.Tests.UnitTests
{
    [TestClass]
    public class ActivityBusinessServiceShouldDo
    {
        private const string INCALLER = "v-bobzh";
        private const string INCALLERVERSION = "1.0.0.0";

        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void InValidateRequestActivityBusinessServiceGetActivityLibrariesWhichActivityLibraryIsNull()
        {
            int eventCode = EventCode.BusinessLayerEvent.Validation.RequestIsNull;
            try
            {
                using (LogSettingConfigIsolator.GetValidLogSettingConfigurationInstance()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
                using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
                {
                    var reply = ActivityBusinessService.GetActivitiesByActivityLibrary(null, false);
                }
            }
            catch (BusinessException e)
            {
                Assert.AreEqual(eventCode, e.ErrorCode);
            }
        }

        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityBusinessServiceGetActivityLibrariesWhichActivityLibraryThrowDataAccessException()
        {
            int eventCode = EventCode.DatabaseEvent.Validation.ActivityLibraryNotFound;
            DataAccessException exception = new DataAccessException(eventCode);
            try
            {
                using (ImplementationOfType impl = new ImplementationOfType(typeof(ActivityRepositoryService)))
                {
                    impl.Register(() => ActivityRepositoryService.GetActivitiesByActivityLibrary(Argument<GetLibraryAndActivitiesDC>.Any, false))
                        .Execute(delegate { throw exception; return null; });
                    CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionRequestDC request = CreateActivityLibraryGetRequest();
                    var reply = ActivityBusinessService.GetActivitiesByActivityLibrary(request, false);
                }
            }
            catch (BusinessException e)
            {
                Assert.AreEqual(eventCode, e.ErrorCode);
            }
        }

        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityBusinessServiceGetActivityLibrariesWhichActivityLibraryReturnReply()
        {
            List<GetLibraryAndActivitiesDC> result = new List<GetLibraryAndActivitiesDC>(){
                new GetLibraryAndActivitiesDC()
                {
                    StoreActivitiesList=new List<StoreActivitiesDC>(),
                },
            };
            using (ImplementationOfType impl = new ImplementationOfType(typeof(ActivityRepositoryService)))
            {
                impl.Register(() => ActivityRepositoryService.GetActivitiesByActivityLibrary(Argument<GetLibraryAndActivitiesDC>.Any, false))
                    .Execute(delegate { return result; });
                CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionRequestDC request = CreateActivityLibraryGetRequest();
                var reply = ActivityBusinessService.GetActivitiesByActivityLibrary(request, false);
                Assert.AreEqual(result[0].StoreActivitiesList, reply.List);
            }
        }

        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityBusinessServiceGetActivityLibrariesWhichActivityLibraryReturnEmptyReply()
        {
            using (ImplementationOfType impl = new ImplementationOfType(typeof(ActivityRepositoryService)))
            {
                impl.Register(() => ActivityRepositoryService.GetActivitiesByActivityLibrary(Argument<GetLibraryAndActivitiesDC>.Any, false))
                    .Execute(delegate { return null; });
                CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionRequestDC request = CreateActivityLibraryGetRequest();
                var reply = ActivityBusinessService.GetActivitiesByActivityLibrary(request, false);
            }
        }

        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityBusinessServiceSearchActivitiesWhichReturnEmptyReply()
        {
            using (ImplementationOfType impl = new ImplementationOfType(typeof(ActivityRepositoryService)))
            {
                impl.Register(() => ActivityRepositoryService.SearchActivities(Argument<ActivitySearchRequestDC>.Any))
                    .Execute(delegate { return null; });
                CWF.DataContracts.ActivitySearchRequestDC request = new ActivitySearchRequestDC();
                var reply = ActivityBusinessService.SearchActivities(request);
            }
        }

        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void InValidateRequestActivityBusinessServiceSearchActivities()
        {
            int eventCode = EventCode.BusinessLayerEvent.Validation.RequestIsNull;
            try
            {
                using (LogSettingConfigIsolator.GetValidLogSettingConfigurationInstance()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
                using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
                {
                    var reply = ActivityBusinessService.SearchActivities(null);
                }
            }
            catch (BusinessException e)
            {
                Assert.AreEqual(eventCode, e.ErrorCode);
            }
        }

        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityBusinessServiceSearchActivitiesThrowDataAccessException()
        {
            int eventCode = EventCode.DatabaseEvent.Validation.ActivityLibraryNotFound;
            DataAccessException exception = new DataAccessException(eventCode);
            try
            {
                using (ImplementationOfType impl = new ImplementationOfType(typeof(ActivityRepositoryService)))
                {
                    impl.Register(() => ActivityRepositoryService.SearchActivities(Argument<ActivitySearchRequestDC>.Any))
                        .Execute(delegate { throw exception; return null; });
                    CWF.DataContracts.ActivitySearchRequestDC request = new ActivitySearchRequestDC();
                    var reply = ActivityBusinessService.SearchActivities(request);
                }
            }
            catch (BusinessException e)
            {
                Assert.AreEqual(eventCode, e.ErrorCode);
            }
        }

        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityBusinessServiceCheckActivityExistsWhichReturnEmptyReply()
        {
            using (ImplementationOfType impl = new ImplementationOfType(typeof(ActivityRepositoryService)))
            {
                impl.Register(() => ActivityRepositoryService.CheckActivityExists(Argument<StoreActivitiesDC>.Any))
                    .Execute(delegate { return new StatusReplyDC(); });
                CWF.DataContracts.StoreActivitiesDC request = CreateStoreActivitiesDC();
                var reply = ActivityBusinessService.CheckActivityExists(request);
            }
        }

        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void InValidateRequestActivityBusinessServiceCheckActivityExists()
        {
            int eventCode = EventCode.BusinessLayerEvent.Validation.RequestIsNull;
            try
            {
                using (LogSettingConfigIsolator.GetValidLogSettingConfigurationInstance()) // Simulate valid log setting config in order to let the LogWriterFactory work as expected.
                using (EventLogWriterIsolator.GetNoLoggingEventLogWriterMock()) // Mock event log writer not to write events.
                {
                    var reply = ActivityBusinessService.CheckActivityExists(null);
                }
            }
            catch (BusinessException e)
            {
                Assert.AreEqual(eventCode, e.ErrorCode);
            }
        }

        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityBusinessServiceCheckActivityExistsThrowDataAccessException()
        {
            int eventCode = EventCode.DatabaseEvent.Validation.ActivityLibraryNotFound;
            DataAccessException exception = new DataAccessException(eventCode);
            try
            {
                using (ImplementationOfType impl = new ImplementationOfType(typeof(ActivityRepositoryService)))
                {
                    impl.Register(() => ActivityRepositoryService.CheckActivityExists(Argument<StoreActivitiesDC>.Any))
                        .Execute(delegate { throw exception; return null; });
                    CWF.DataContracts.StoreActivitiesDC request = CreateStoreActivitiesDC();
                    var reply = ActivityBusinessService.CheckActivityExists(request);
                }
            }
            catch (BusinessException e)
            {
                Assert.AreEqual(eventCode, e.ErrorCode);
            }
        }

        private static StoreActivitiesDC CreateStoreActivitiesDC()
        {
            return new StoreActivitiesDC()
            {
                Incaller = INCALLER,
                IncallerVersion = INCALLERVERSION,
            };
        }

        private static CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionRequestDC CreateActivityLibraryGetRequest()
        {
            CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionRequestDC request = new CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionRequestDC();
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            request.Name = "Test#001";
            request.VersionNumber = "1.0.0.0";
            return request;
        }
    }
}
