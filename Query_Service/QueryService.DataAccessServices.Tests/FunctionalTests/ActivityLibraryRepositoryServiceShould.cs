using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Service.DataAccessServices;

namespace Microsoft.Support.Workflow.Service.DataAccessServices.Tests.FunctionalTests
{
    [TestClass]
    public class ActivityLibraryRepositoryServiceShould
    {
        private const string InCaller = "v-stska";
        private const string InCallerVersion = "1.0.0.0";

        [Description("")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnAllActivityLibrariesForCallerWhenGetActivityLibrariesIsCalledWithoutIDOrGuid()
        {
            // TODO: Instead of relying on the initial test data, populate data using the existing API and then delete after the test.
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            List<CWF.DataContracts.ActivityLibraryDC> reply = null;
            request.Incaller = InCaller;
            request.IncallerVersion = InCallerVersion;

            reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
            Assert.IsNotNull(reply);

            // TODO: Assert exact count, after implementing dynamic data initialization.
        }

        [Description("")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnMachingActivityLibraryAssociatedWithIdWhenGetActivityLibrariesIsCalled()
        {
            // TODO: Instead of relying on the initial test data, populate data using the existing API and then delete after the test.
            Guid activityLibraryGuid = Guid.Parse("cb46bc99-84db-4e27-aaac-b62fe801b392");
            int activityLibraryId = 1;
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            List<CWF.DataContracts.ActivityLibraryDC> reply = null;
            request.Incaller = InCaller;
            request.IncallerVersion = InCallerVersion;
            request.Id = activityLibraryId;

            reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
            Assert.IsNotNull(reply);
            Assert.AreEqual(1, reply.Count);
            Assert.AreEqual(activityLibraryGuid, reply[0].Guid);
        }

        [Description("")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnMachingActivityLibraryAssociatedWithGuidWhenGetActivityLibrariesIsCalled()
        {
            // TODO: Instead of relying on the initial test data, populate data using the existing API and then delete after the test.
            Guid activityLibraryGuid = Guid.Parse("c08fd4ae-5d98-413f-8716-a80b86b2b5db");
            int activityLibraryId = 2;
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            List<CWF.DataContracts.ActivityLibraryDC> reply = null;
            request.Incaller = InCaller;
            request.IncallerVersion = InCallerVersion;
            request.Guid = activityLibraryGuid;

            reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
            Assert.IsNotNull(reply);
            Assert.AreEqual(1, reply.Count);
            Assert.AreEqual(activityLibraryId, reply[0].Id);
        }

        [Description("")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnAllActivityLibrariesIfIdIsNegativeWhenGetActivityLibrariesIsCalled()
        {
            // TODO: Instead of relying on the initial test data, populate data using the existing API and then delete after the test.
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            List<CWF.DataContracts.ActivityLibraryDC> reply = null;
            request.Incaller = InCaller;
            request.IncallerVersion = InCallerVersion;
            request.Id = -1;

            reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
            Assert.IsNotNull(reply);
            
            // TODO: Assert exact count, after implementing dynamic data initialization.
        }

        [Description("")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnNoItemsIfIdIsPositiveYetNonExistingWhenGetActivityLibrariesIsCalled()
        {
            // TODO: Instead of relying on the initial test data, populate data using the existing API and then delete after the test.
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            List<CWF.DataContracts.ActivityLibraryDC> reply = null;
            request.Incaller = InCaller;
            request.IncallerVersion = InCallerVersion;
            request.Id = 100000; // TODO: Refine this with dynamic data initialization.

            reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
            Assert.IsNotNull(reply);
            Assert.AreEqual(0, reply.Count);
        }

        [Description("")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnNoItemsIfGuidIsValidYetNonExistingWhenGetActivityLibrariesIsCalled()
        {
            // TODO: Instead of relying on the initial test data, populate data using the existing API and then delete after the test.
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            List<CWF.DataContracts.ActivityLibraryDC> reply = null;
            request.Incaller = InCaller;
            request.IncallerVersion = InCallerVersion;
            request.Guid = Guid.NewGuid();

            reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
            Assert.IsNotNull(reply);
            Assert.AreEqual(0, reply.Count);
        }
    }
}
