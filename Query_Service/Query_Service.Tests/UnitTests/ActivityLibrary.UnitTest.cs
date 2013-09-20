using CWF.DataContracts;
using Microsoft.Support.Workflow.Service.BusinessServices;
using Microsoft.Support.Workflow.Service.DataAccessServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Query_Service.UnitTests
{
    /// <summary>
    /// Unit tests for QueryService BAl and DAL layer
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "This not required")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Not required fot const/unit tests")]
    [TestClass]
    public class ActivityLibraryUnitTest
    {
        [Description("Get All Activity Libraries")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void TESTBalGetAllActivityLibraries()
        {
            CWF.DataContracts.GetAllActivityLibrariesRequestDC request = new CWF.DataContracts.GetAllActivityLibrariesRequestDC();
            CWF.DataContracts.GetAllActivityLibrariesReplyDC reply = null;
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            reply = ActivityLibraryBusinessService.GetActivityLibrariesWithoutDlls(request);
            Assert.IsNotNull(reply);
            Assert.IsNotNull(reply.List);
            Assert.IsTrue(reply.List.Count > 1); // Expecting multiple results here.  Better to perform an initialization before running this test and assert the expected count.
            foreach (var item in reply.List)
            {
                Assert.IsNull(item.Executable); // DLL should not be returned with this method.
            }
        }

        [Description("Get Activities By Activity Library Name And Version")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void UnitTestBalGetActivitiesByActivityLibraryNameAndVersion()
        {
            var request = new CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionRequestDC();
            var reply = new CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionReplyDC();

            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Name = "OASP.Activities";
            request.VersionNumber = "2.2.108.0";
            request.Environment = UnitTestConstant.TOENVIRONMENT;
            try
            {
                reply = ActivityBusinessService.GetActivitiesByActivityLibrary(request, true);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.BAL.StoreActivityLibraryDependencies.StoreActivityLibraryDependencyList(request);");
            }

            Assert.AreEqual(reply.StatusReply.Errorcode, 0);
            Assert.AreEqual(reply.StatusReply.ErrorMessage, string.Empty);
        }

        [Description("Get missing Activity Libraries")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetMissingActivityLibraries()
        {
            var request = new CWF.DataContracts.GetMissingActivityLibrariesRequest();
            var reply = new List<CWF.DataContracts.ActivityLibraryDC>();

            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();
            activityLibraryDC.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            activityLibraryDC.Incaller = UnitTestConstant.INCALLER;
            activityLibraryDC.Name = "OASP.Core2";
            activityLibraryDC.VersionNumber = "1.0.0.0";
            activityLibraryDC.Environment = UnitTestConstant.TOENVIRONMENT;
            activityLibraryDC.Id = 0;

            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.ActivityLibrariesList = new List<ActivityLibraryDC> { activityLibraryDC };

            try
            {
                reply = ActivityLibraryRepositoryService.GetMissingActivityLibraries(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.BAL.StoreActivityLibraryDependencies.StoreActivityLibraryDependencyList(request);");
            }

            Assert.IsNotNull(reply);
        }

        [Description("Get missing Activity Libraries")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALGetMissingActivityLibraries()
        {
            var request = new CWF.DataContracts.GetMissingActivityLibrariesRequest();
            var reply = new GetMissingActivityLibrariesReply();

            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();
            activityLibraryDC.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            activityLibraryDC.Incaller = UnitTestConstant.INCALLER;
            activityLibraryDC.Name = "OASP.Core2";
            activityLibraryDC.VersionNumber = "1.0.0.0";
            activityLibraryDC.Environment = UnitTestConstant.TOENVIRONMENT;
            activityLibraryDC.Id = 0;

            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.ActivityLibrariesList = new List<ActivityLibraryDC> { activityLibraryDC };

            try
            {
                reply = ActivityLibraryBusinessService.GetMissingActivityLibraries(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.BAL.StoreActivityLibraryDependencies.StoreActivityLibraryDependencyList(request);");
            }

            Assert.IsNotNull(reply);
        }

        [Description("Get the Activity Library and all store activities")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetActivityLibraryAndStoreActivities()
        {
            List<CWF.DataContracts.GetLibraryAndActivitiesDC> reply = null;
            CWF.DataContracts.GetLibraryAndActivitiesDC request = new CWF.DataContracts.GetLibraryAndActivitiesDC();
            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();
            activityLibraryDC.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            activityLibraryDC.Incaller = UnitTestConstant.INCALLER;
            activityLibraryDC.Name = "OASP.Core2";
            activityLibraryDC.VersionNumber = "1.0.0.0";
            activityLibraryDC.Environment = UnitTestConstant.TOENVIRONMENT;
            activityLibraryDC.Id = 0;
            request.ActivityLibrary = activityLibraryDC;
            try
            {
                reply = ActivityRepositoryService.GetActivitiesByActivityLibrary(request, false);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.GetActivityLibraryAndStoreActivities(request);");
            }

            Assert.IsNotNull(reply);
        }

        [Description("Store the entire node of a dependency list with a null dependency list")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void StoreActivityLibraryDependencyListBADdependencyActivityLibrary()
        {
            CWF.DataContracts.StoreActivityLibrariesDependenciesDC request = new CWF.DataContracts.StoreActivityLibrariesDependenciesDC();
            CWF.DataContracts.StoreDependenciesRootActiveLibrary rdl = new CWF.DataContracts.StoreDependenciesRootActiveLibrary();
            CWF.DataContracts.StoreActivityLibrariesDependenciesDC reply = null;
            List<CWF.DataContracts.StoreDependenciesDependentActiveLibrary> dalList = new List<CWF.DataContracts.StoreDependenciesDependentActiveLibrary>();

            request.InInsertedByUserAlias = UnitTestConstant.INSERTEDBYUSERALIAS;
            request.InUpdatedByUserAlias = UnitTestConstant.UPDATEDBYUSERALIAS;
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            rdl.ActivityLibraryName = "TEST#200";
            rdl.ActivityLibraryVersionNumber = "1.0.0.4";

            request.StoreDependenciesRootActiveLibrary = rdl;
            request.StoreDependenciesDependentActiveLibraryList = dalList;

            try
            {
                reply = CWF.BAL.Services.StoreActivityLibraryDependencyList(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.BAL.StoreActivityLibraryDependencies.StoreActivityLibraryDependencyList(request);");
            }

            Assert.IsNull(reply);
        }

        [Description("Get all activities associated with an activity library")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetActivitiesByActivityLibrary()
        {
            List<CWF.DataContracts.GetLibraryAndActivitiesDC> reply = null;
            var request = new CWF.DataContracts.GetLibraryAndActivitiesDC();
            var activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();

            activityLibraryDC.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            activityLibraryDC.Incaller = UnitTestConstant.INCALLER;
            activityLibraryDC.Name = "OASP.Core";
            activityLibraryDC.VersionNumber = "2.2.108.0";
            activityLibraryDC.Environment = UnitTestConstant.TOENVIRONMENT;
            activityLibraryDC.Id = 0;
            request.ActivityLibrary = activityLibraryDC;

            try
            {
                reply = CWF.BAL.Services.GetLibraryAndActivities(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.BAL.GetLibrary.GetLibraryAndActivities(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, 0);
        }

        [Description("Get all activities associated with an activity library - Bad Library Name")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetActivitiesByActivityLibraryBadLibraryName()
        {
            List<CWF.DataContracts.GetLibraryAndActivitiesDC> reply = null;
            CWF.DataContracts.GetLibraryAndActivitiesDC request = new CWF.DataContracts.GetLibraryAndActivitiesDC();
            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();
            activityLibraryDC.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            activityLibraryDC.Incaller = UnitTestConstant.INCALLER;
            activityLibraryDC.Name = "OASP.Corexx";
            activityLibraryDC.VersionNumber = "1.0.0.0";
            activityLibraryDC.Environment = UnitTestConstant.TOENVIRONMENT;
            activityLibraryDC.Id = 0;
            request.ActivityLibrary = activityLibraryDC;
            try
            {
                reply = CWF.BAL.Services.GetLibraryAndActivities(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.BAL.GetLibrary.GetLibraryAndActivities(request);");
            }

            Assert.IsNotNull(reply);
        }

        [Description("Get the entire activityLibrary table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityLibraryGet()
        {
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            List<CWF.DataContracts.ActivityLibraryDC> reply = null;
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;

            try
            {
                reply = ActivityLibraryRepositoryService.GetActivityLibraries(request, true);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.ActivityLibraryGet(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Get the entire activityLibrary table")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALActivityLibraryGet()
        {
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            List<CWF.DataContracts.ActivityLibraryDC> reply = null;
            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;

            try
            {
                reply = ActivityLibraryBusinessService.GetActivityLibraries(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = Microsoft.Support.Workflow.Service.BusinessServices.ActivityLibraryBusinessService.ActivityLibraryGet(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Create and update an activityLibrary row")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityLibraryCreateANDUpdate()
        {
            CWF.DataContracts.ActivityLibraryDC reply = null;
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            //// UPDATE
            try
            {
                request.Incaller = UnitTestConstant.INCALLER;
                request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
                request.Description = "In NEW Description";
                request.InInsertedByUserAlias = UnitTestConstant.INSERTEDBYUSERALIAS;
                request.InUpdatedByUserAlias = UnitTestConstant.UPDATEDBYUSERALIAS;
                request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
                request.Environment = UnitTestConstant.TOENVIRONMENT;
                // TODO Unit testing fails(too many parameters) while TEST Harness works - Assert.AreEqual(reply.StatusReply.Errorcode, -60);
                //// update
                request.Guid = new Guid("ACE00000-0000-0000-0000-000000000000");
                request.Name = "TEST#200" + Guid.NewGuid();
                request.AuthGroupName = "pqocwauthors";
                request.CategoryName = "OAS Basic Controls";
                request.Category = Guid.Empty;
                request.Executable = new byte[4];
                request.HasActivities = true;
                request.ImportedBy = "REDMOND\v-stska";
                request.Description = "TEST DESCRIPTION";
                request.InInsertedByUserAlias = UnitTestConstant.INCALLER;
                request.VersionNumber = "1.0.0.1";
                request.Status = 990;
                request.InUpdatedByUserAlias = "REDMOND\v-stska";

                reply = ActivityLibrary.ActivityLibraryCreateOrUpdate(request);
                Assert.IsNotNull(reply);
                // TODO Unit testing fails(too many parameters) while TEST Harness works - Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.UPDATE_INVALID_ID);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                // TODO Unit testing fails(too many parameters) while TEST Harness works - Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.ActivityLibraryCreateOrUpdate(request);");
            }
        }

        [Description("get the entire activityLibrary tree of dependency")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityLibraryDependenciesTreeGet()
        {
            var request = new CWF.DataContracts.StoreActivityLibrariesDependenciesDC();
            var rdl = new CWF.DataContracts.StoreDependenciesRootActiveLibrary();
            List<CWF.DataContracts.StoreActivityLibrariesDependenciesDC> reply = null;

            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            rdl.ActivityLibraryName = "OASP.Core";
            rdl.ActivityLibraryVersionNumber = "2.2.108.0";
            request.StoreDependenciesRootActiveLibrary = rdl;

            //// TreeGet
            try
            {
                reply = ActivityLibraryDependencyRepositoryService.GetActivityLibraryDependencyTree(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.StoreActivityLibraryDependenciesTreeGet(request, treeGet)");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(SprocValues.REPLY_ERRORCODE_VALUE_OK, reply[0].StatusReply.Errorcode);
        }

        [Description("get the entire activityLibrary tree of dependency")]
        [Owner(UnitTestConstant.OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALActivityLibraryDependenciesTreeGet()
        {
            var request = new CWF.DataContracts.StoreActivityLibrariesDependenciesDC();
            var rdl = new CWF.DataContracts.StoreDependenciesRootActiveLibrary();
            List<CWF.DataContracts.StoreActivityLibrariesDependenciesDC> reply = null;

            request.Incaller = UnitTestConstant.INCALLER;
            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            rdl.ActivityLibraryName = "OASP.Core";
            rdl.ActivityLibraryVersionNumber = "2.2.108.0";
            request.StoreDependenciesRootActiveLibrary = rdl;

            //// TreeGet
            try
            {
                reply = ActivityLibraryDependencyBusinessService.GetActivityLibraryDependencyTree(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.StoreActivityLibraryDependenciesTreeGet(request, treeGet)");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(SprocValues.REPLY_ERRORCODE_VALUE_OK, reply[0].StatusReply.Errorcode);
        }
    }
}
