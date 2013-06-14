//-----------------------------------------------------------------------
// <copyright file="DAL.UnitTest.cs" company="Microsoft">
// Copyright
// DAL Unit Test
// </copyright>
//-----------------------------------------------------------------------

namespace Query_Service.Testsproject
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using CWF.BAL.Versioning;
    using CWF.DataContracts;
    using Microsoft.Support.Workflow.Service.BusinessServices;
    using Microsoft.Support.Workflow.Service.DataAccessServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Support.Workflow.Service.Test.Common;
    using CWF.BAL;

    /// <summary>
    /// Unit tests for QueryService BAl and DAL layer
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "This not required")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Not required fot const/unit tests")]
    [TestClass]
    public class DALUnitTest
    {
        private const string INCALLER = "v-stska";
        private const string INCALLERVERSION = "1.0.0.0";
        private const string OWNER = "v-stska";
        private const string UPDATEDBYUSERALIAS = "v-stska";
        private const string INSERTEDBYUSERALIAS = "v-stska";

        [Description("Get All Activity Libraries")]
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void TESTBalGetAllActivityLibraries()
        {
            CWF.DataContracts.GetAllActivityLibrariesRequestDC request = new CWF.DataContracts.GetAllActivityLibrariesRequestDC();
            CWF.DataContracts.GetAllActivityLibrariesReplyDC reply = null;
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
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
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void UnitTestBalGetActivitiesByActivityLibraryNameAndVersion()
        {
            var request = new CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionRequestDC();
            var reply = new CWF.DataContracts.GetActivitiesByActivityLibraryNameAndVersionReplyDC();

            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            request.Name = "OASP.Activities";
            request.VersionNumber = "2.2.108.0";

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
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetMissingActivityLibraries()
        {
            var request = new CWF.DataContracts.GetMissingActivityLibrariesRequest();
            var reply = new List<CWF.DataContracts.ActivityLibraryDC>();

            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();
            activityLibraryDC.IncallerVersion = INCALLERVERSION;
            activityLibraryDC.Incaller = INCALLER;
            activityLibraryDC.Name = "OASP.Core2";
            activityLibraryDC.VersionNumber = "1.0.0.0";
            activityLibraryDC.Id = 0;

            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
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
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALGetMissingActivityLibraries()
        {
            var request = new CWF.DataContracts.GetMissingActivityLibrariesRequest();
            var reply = new GetMissingActivityLibrariesReply();

            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();
            activityLibraryDC.IncallerVersion = INCALLERVERSION;
            activityLibraryDC.Incaller = INCALLER;
            activityLibraryDC.Name = "OASP.Core2";
            activityLibraryDC.VersionNumber = "1.0.0.0";
            activityLibraryDC.Id = 0;

            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
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
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetActivityLibraryAndStoreActivities()
        {
            List<CWF.DataContracts.GetLibraryAndActivitiesDC> reply = null;
            CWF.DataContracts.GetLibraryAndActivitiesDC request = new CWF.DataContracts.GetLibraryAndActivitiesDC();
            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();
            activityLibraryDC.IncallerVersion = INCALLERVERSION;
            activityLibraryDC.Incaller = INCALLER;
            activityLibraryDC.Name = "OASP.Core2";
            activityLibraryDC.VersionNumber = "1.0.0.0";
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
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void StoreActivityLibraryDependencyListBADdependencyActivityLibrary()
        {
            CWF.DataContracts.StoreActivityLibrariesDependenciesDC request = new CWF.DataContracts.StoreActivityLibrariesDependenciesDC();
            CWF.DataContracts.StoreDependenciesRootActiveLibrary rdl = new CWF.DataContracts.StoreDependenciesRootActiveLibrary();
            CWF.DataContracts.StoreActivityLibrariesDependenciesDC reply = null;
            List<CWF.DataContracts.StoreDependenciesDependentActiveLibrary> dalList = new List<CWF.DataContracts.StoreDependenciesDependentActiveLibrary>();

            request.InsertedByUserAlias = INSERTEDBYUSERALIAS;
            request.UpdatedByUserAlias = UPDATEDBYUSERALIAS;
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
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
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetActivitiesByActivityLibrary()
        {
            List<CWF.DataContracts.GetLibraryAndActivitiesDC> reply = null;
            var request = new CWF.DataContracts.GetLibraryAndActivitiesDC();
            var activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();

            activityLibraryDC.IncallerVersion = INCALLERVERSION;
            activityLibraryDC.Incaller = INCALLER;
            activityLibraryDC.Name = "OASP.Core";
            activityLibraryDC.VersionNumber = "2.2.108.0";
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
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void GetActivitiesByActivityLibraryBadLibraryName()
        {
            List<CWF.DataContracts.GetLibraryAndActivitiesDC> reply = null;
            CWF.DataContracts.GetLibraryAndActivitiesDC request = new CWF.DataContracts.GetLibraryAndActivitiesDC();
            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();
            activityLibraryDC.IncallerVersion = INCALLERVERSION;
            activityLibraryDC.Incaller = INCALLER;
            activityLibraryDC.Name = "OASP.Corexx";
            activityLibraryDC.VersionNumber = "1.0.0.0";
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

        /// <summary>
        /// Creates a store activity for TEST
        /// </summary>
        /// <param name="name">store activity name</param>
        /// <param name="version">version string</param>
        /// <param name="guid">GUID identifier</param>
        /// <param name="activityLibraryName">activityLibraryName identifier</param>
        /// <param name="activityLibraryVersion">activityLibraryVersion identifier</param>
        /// <returns>StoreActivitiesDC object</returns>
        public static CWF.DataContracts.StoreActivitiesDC CreateSA(string name, string version, Guid guid, string activityLibraryName, string activityLibraryVersion)
        {
            CWF.DataContracts.StoreActivitiesDC storeActivitiesDC = new CWF.DataContracts.StoreActivitiesDC();
            storeActivitiesDC.ActivityLibraryName = activityLibraryName;
            storeActivitiesDC.ActivityLibraryVersion = activityLibraryVersion;
            storeActivitiesDC.ActivityCategoryName = "OAS Basic Controls";
            storeActivitiesDC.AuthGroupName = "pqocwfadmin";
            storeActivitiesDC.Description = "TEST type";
            storeActivitiesDC.Incaller = INCALLER;
            storeActivitiesDC.IncallerVersion = INCALLERVERSION;
            storeActivitiesDC.InsertedByUserAlias = INSERTEDBYUSERALIAS;
            storeActivitiesDC.IsCodeBeside = true;
            storeActivitiesDC.IsService = true;
            storeActivitiesDC.Locked = true;
            storeActivitiesDC.LockedBy = "v-stska";
            storeActivitiesDC.MetaTags = "Meta, Tags, TEST";
            storeActivitiesDC.Name = name;
            storeActivitiesDC.ShortName = name;
            storeActivitiesDC.Namespace = "Namespace1";
            storeActivitiesDC.StatusCodeName = "Private";
            storeActivitiesDC.Guid = Guid.NewGuid();
            //// saDC.Guid = new Guid("AAAAAAAA-1A29-44D1-B783-0A3659F1CDB2");
            storeActivitiesDC.ToolBoxtab = 1;
            storeActivitiesDC.UpdatedByUserAlias = UPDATEDBYUSERALIAS;
            storeActivitiesDC.Version = version;
            storeActivitiesDC.OldVersion = version;
            //// saDC.Version = "1.0.0.0";
            storeActivitiesDC.WorkflowTypeName = "Workflow";
            storeActivitiesDC.Xaml = "<XamlBeginTag></XamlBeginTag>";
            return storeActivitiesDC;
        }

        [Description("Get The entire StoreActivities table")]
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void StoreActivitiesGet()
        {
            CWF.DataContracts.StoreActivitiesDC request = new CWF.DataContracts.StoreActivitiesDC();
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            //// request.Id = 53;
            request.Name = "PublishingWorkflow";
            request.Version = "1.0.1.0";
            List<CWF.DataContracts.StoreActivitiesDC> reply = null;

            try
            {
                reply = Activities.StoreActivitiesGet(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.StoreActivitiesGet(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Get all records for a workflow name")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void StoreActivitiesGetByName()
        {
            CWF.DataContracts.StoreActivitiesDC request = new CWF.DataContracts.StoreActivitiesDC();
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            request.Name = "PublishingWorkflow";
            List<CWF.DataContracts.StoreActivitiesDC> reply = null;

            try
            {
                reply = Activities.StoreActivitiesGetByName(request.Name, string.Empty);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.StoreActivitiesGetByName('PublishingWorkflow','');");
            }

            Assert.IsNotNull(reply);
            Assert.IsTrue(reply.Any());
            Assert.AreEqual("setup", reply[0].InsertedByUserAlias);
            Assert.IsNotNull(reply[0].InsertedDateTime);

        }

        #region [ Test StoreActivitiesSetLock ]

        [Description("Set lock for the activity.")]
        [Owner("v-ery")]
        [TestCategory("Unit")]
        [TestMethod]
        public void TestBalStoreActivitiesSetLock()
        {
            CWF.DataContracts.StoreActivitiesDC request = new CWF.DataContracts.StoreActivitiesDC();
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            request.Name = "PublishingWorkflow";
            request.Version = "1.0.1.0";
            request.Locked = true;
            request.LockedBy = "v-ery";
            DateTime lockedTime = DateTime.Now;
            CWF.DataContracts.StatusReplyDC result = null;

            try
            {
                result = Services.StoreActivitiesSetLock(request, lockedTime);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.StoreActivitiesGet(request);");
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(SprocValues.REPLY_ERRORCODE_VALUE_OK, result.Errorcode);
        }

        #endregion

        #region [ CreateActivityLibraryAndStoreActivities  ]

        public static void CreateActivityLibraryAndStoreActivities(out CWF.DataContracts.ActivityLibraryDC activityLibraryDC, out List<CWF.DataContracts.StoreActivitiesDC> storeActivityDC)
        {
            //// Setup the base request objects in request
            List<CWF.DataContracts.StoreActivitiesDC> storeActivityRequest = new List<CWF.DataContracts.StoreActivitiesDC>();

            CWF.DataContracts.ActivityLibraryDC alDC = new CWF.DataContracts.ActivityLibraryDC();
            CWF.DataContracts.StoreActivitiesDC saDC = new CWF.DataContracts.StoreActivitiesDC();

            //// population the request object
            //// create activity library entry

            alDC.Incaller = INCALLER;
            alDC.IncallerVersion = INCALLERVERSION;
            alDC.Guid = Guid.NewGuid();
            alDC.Name = "TEST#421A";
            alDC.AuthGroupName = "pqocwfauthors";
            alDC.CategoryName = "OAS Basic Controls";
            alDC.Category = Guid.Empty;
            alDC.Executable = new byte[4];
            alDC.HasActivities = true;
            alDC.ImportedBy = "REDMOND\v-stska";
            alDC.Description = "TEST#421A DESCRIPTION";
            alDC.InsertedByUserAlias = "v-stska";
            alDC.VersionNumber = "1.0.0.8";
            alDC.Status = 1;
            alDC.StatusName = "Private";
            alDC.UpdatedByUserAlias = "REDMOND\v-stska1";

            //// Create store activity entries
            storeActivityRequest.Add(CreateSA("TEST#300", "1.1.0.0", new Guid(), alDC.Name, alDC.VersionNumber));

            activityLibraryDC = alDC;
            storeActivityDC = storeActivityRequest;
        }
        #endregion

        #region [ TESTBalUploadActivityLibraryAndDependentActivities ]
        /// <summary>
        /// Test of the Bal UploadActivityLibraryAndDependentActivities
        /// </summary>
        [Description("Test of the Bal UploadActivityLibraryAndDependentActivities")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void TESTBalUploadActivityLibraryAndDependentActivities()
        {
            var nameModifier = Guid.NewGuid().ToString();
            CWF.DataContracts.StoreLibraryAndActivitiesRequestDC request = new CWF.DataContracts.StoreLibraryAndActivitiesRequestDC();
            List<CWF.DataContracts.StoreActivitiesDC> reply = null;

            request.IncallerVersion = INCALLERVERSION;
            request.Incaller = INCALLER;
            request.InInsertedByUserAlias = INCALLER;
            request.InUpdatedByUserAlias = INCALLER;
            request.EnforceVersionRules = true;


            // Create ActivityLibrary object and add to request object
            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();

            // create storeActivitiesDC list and individual objects and add to request
            List<CWF.DataContracts.StoreActivitiesDC> storeActivitiesDCList = new List<CWF.DataContracts.StoreActivitiesDC>();
            CWF.DataContracts.StoreActivitiesDC storeActivitiesDC = new CWF.DataContracts.StoreActivitiesDC();
            CreateActivityLibraryAndStoreActivities(out activityLibraryDC, out storeActivitiesDCList);
            request.ActivityLibrary = activityLibraryDC;
            request.StoreActivitiesList = storeActivitiesDCList;
            request.StoreActivitiesList.ForEach(record =>
            {
                record.Name += nameModifier;
                record.ActivityLibraryName += nameModifier;
                record.ShortName += nameModifier;

            });

            activityLibraryDC.Name += nameModifier;

            request.StoreActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC()
            {
                Name = activityLibraryDC.Name,
                Version = activityLibraryDC.VersionNumber,
                List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>
                {
                    new StoreActivityLibraryDependenciesGroupsRequestDC
                    {
                        IncallerVersion = INCALLERVERSION,
                        Incaller = INCALLER,
                        Name = "PublishingInfo",
                        Version = "1.0.0.1"
                    },
                }
            };

            try
            {
                reply = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(request);");
            }
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
            Assert.AreEqual(reply.Count, storeActivitiesDCList.Count);
        }
        #endregion

        #region [ TESTBalUploadActivityLibraryAndDependentActivities ]
        /// <summary>
        /// Test of the Bal UploadActivityLibraryAndDependentActivities
        /// </summary>
        [Description("Test of the Bal UploadActivityLibraryAndDependentActivities")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void TESTBalUploadActivityLibraryAndDependentActivitiesRepeat()
        {
            var nameModifier = Guid.NewGuid().ToString();
            CWF.DataContracts.StoreLibraryAndActivitiesRequestDC request = new CWF.DataContracts.StoreLibraryAndActivitiesRequestDC();
            List<CWF.DataContracts.StoreActivitiesDC> reply = null;

            request.IncallerVersion = INCALLERVERSION;
            request.Incaller = INCALLER;
            request.InInsertedByUserAlias = INCALLER;
            request.InUpdatedByUserAlias = INCALLER;
            request.EnforceVersionRules = true;


            // Create ActivityLibrary object and add to request object
            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();

            // create storeActivitiesDC list and individual objects and add to request
            List<CWF.DataContracts.StoreActivitiesDC> storeActivitiesDCList = new List<CWF.DataContracts.StoreActivitiesDC>();
            CWF.DataContracts.StoreActivitiesDC storeActivitiesDC = new CWF.DataContracts.StoreActivitiesDC();
            CreateActivityLibraryAndStoreActivities(out activityLibraryDC, out storeActivitiesDCList);
            request.ActivityLibrary = activityLibraryDC;
            request.StoreActivitiesList = storeActivitiesDCList;
            request.StoreActivitiesList.ForEach(record =>
            {
                record.Name += nameModifier;
                record.ActivityLibraryName += nameModifier;
                record.ShortName += nameModifier;

            });

            activityLibraryDC.Name = "PublishingInfo";
            activityLibraryDC.VersionNumber = "1.0.0.1";

            request.StoreActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC()
            {
                Name = activityLibraryDC.Name,
                Version = activityLibraryDC.VersionNumber,
                List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>
                {
                    new StoreActivityLibraryDependenciesGroupsRequestDC
                    {
                        IncallerVersion = INCALLERVERSION,
                        Incaller = INCALLER,
                        Name = "PublishingInfo",
                        Version = "1.0.0.1"
                    },
                }
            };

            try
            {
                reply = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(request);");
            }
            Assert.IsNotNull(reply);
        }
        #endregion

        #region [ TESTBalUploadActivityLibraryAndDependentActivities ]
        /// <summary>
        /// Test of the Bal UploadActivityLibraryAndDependentActivities
        /// </summary>
        [Description("Test of the Bal UploadActivityLibraryAndDependentActivities")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void TESTBalUploadActivityLibraryAndDependentActivitiesAndUpdateVersion()
        {
            //var nameModifier = Guid.NewGuid().ToString();
            CWF.DataContracts.StoreLibraryAndActivitiesRequestDC request = new CWF.DataContracts.StoreLibraryAndActivitiesRequestDC();
            List<CWF.DataContracts.StoreActivitiesDC> reply = null;

            request.IncallerVersion = INCALLERVERSION;
            request.Incaller = INCALLER;
            request.InInsertedByUserAlias = INCALLER;
            request.InUpdatedByUserAlias = INCALLER;
            request.EnforceVersionRules = true;


            // Create ActivityLibrary object and add to request object
            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();

            // create storeActivitiesDC list and individual objects and add to request
            List<CWF.DataContracts.StoreActivitiesDC> storeActivitiesDCList = new List<CWF.DataContracts.StoreActivitiesDC>();
            CWF.DataContracts.StoreActivitiesDC storeActivitiesDC = new CWF.DataContracts.StoreActivitiesDC();
            CreateActivityLibraryAndStoreActivities(out activityLibraryDC, out storeActivitiesDCList);
            request.ActivityLibrary = activityLibraryDC;
            request.StoreActivitiesList = storeActivitiesDCList;
            //request.StoreActivitiesList.ForEach(record =>
            //{
            //    record.Name += nameModifier;
            //    record.ActivityLibraryName += nameModifier;
            //    record.ShortName += nameModifier;

            //});

            //activityLibraryDC.Name += nameModifier;

            request.StoreActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC()
            {
                Name = activityLibraryDC.Name,
                Version = activityLibraryDC.VersionNumber,
                List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>
                {
                    new StoreActivityLibraryDependenciesGroupsRequestDC
                    {
                        IncallerVersion = INCALLERVERSION,
                        Incaller = INCALLER,
                        Name = "PublishingInfo",
                        Version = "1.0.0.1"
                    },
                }
            };

            try
            {
                reply = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(request);");
            }
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
            Assert.AreEqual(reply.Count, storeActivitiesDCList.Count);
        }
        #endregion

        #region [ TESTBalUploadActivityLibraryAndDependentActivities ]
        /// <summary>
        /// Test of the Bal UploadActivityLibraryAndDependentActivities
        /// </summary>
        [Description("Test of the Bal UploadActivityLibraryAndDependentActivities")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void TESTBalUploadActivityLibraryAndDependentActivitiesAndUpdateVersionAndSaveStoreActivitiesFailed()
        {
            //var nameModifier = Guid.NewGuid().ToString();
            CWF.DataContracts.StoreLibraryAndActivitiesRequestDC request = new CWF.DataContracts.StoreLibraryAndActivitiesRequestDC();
            List<CWF.DataContracts.StoreActivitiesDC> reply = null;

            request.IncallerVersion = INCALLERVERSION;
            request.Incaller = INCALLER;
            request.InInsertedByUserAlias = INCALLER;
            request.InUpdatedByUserAlias = INCALLER;
            request.EnforceVersionRules = false;


            // Create ActivityLibrary object and add to request object
            CWF.DataContracts.ActivityLibraryDC activityLibraryDC = new CWF.DataContracts.ActivityLibraryDC();

            // create storeActivitiesDC list and individual objects and add to request
            List<CWF.DataContracts.StoreActivitiesDC> storeActivitiesDCList = new List<CWF.DataContracts.StoreActivitiesDC>();
            CWF.DataContracts.StoreActivitiesDC storeActivitiesDC = new CWF.DataContracts.StoreActivitiesDC();
            CreateActivityLibraryAndStoreActivities(out activityLibraryDC, out storeActivitiesDCList);
            request.ActivityLibrary = activityLibraryDC;
            request.StoreActivitiesList = new List<StoreActivitiesDC> { CreateSA(string.Empty, string.Empty, new Guid(), activityLibraryDC.Name, activityLibraryDC.VersionNumber) };

            request.StoreActivityLibraryDependenciesGroupsRequestDC = new StoreActivityLibraryDependenciesGroupsRequestDC()
            {
                Name = activityLibraryDC.Name,
                Version = activityLibraryDC.VersionNumber,
                List = new List<StoreActivityLibraryDependenciesGroupsRequestDC>
                {
                    new StoreActivityLibraryDependenciesGroupsRequestDC
                    {
                        IncallerVersion = INCALLERVERSION,
                        Incaller = INCALLER,
                        Name = "PublishingInfo",
                        Version = "1.0.0.1"
                    },
                }
            };

            try
            {
                reply = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(request);");
            }
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, 55106);
        }
        #endregion

        [Description("Set StoreActivity locked")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void StoreActivitiesUpdateLock()
        {
            CWF.DataContracts.StoreActivitiesDC request = new CWF.DataContracts.StoreActivitiesDC();
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            request.Name = "PublishingWorkflow";
            request.Version = "1.0.1.0";
            request.Locked = true;
            request.LockedBy = OWNER;
            CWF.DataContracts.StoreActivitiesDC reply = null;

            try
            {
                reply = Activities.StoreActivitiesUpdateLock(request, DateTime.Now);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Activities.StoreActivityUpdateLock(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Get Applications")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void ApplicationsGet()
        {
            CWF.DataContracts.ApplicationsGetRequestDC request = new CWF.DataContracts.ApplicationsGetRequestDC();
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            request.InId = 0;
            request.InName = string.Empty;

            CWF.DataContracts.ApplicationsGetReplyDC reply = null;

            try
            {
                reply = Applications.ApplicationsGet(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.Applications.ApplicationsGet(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Get StatusCode")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void StatusCodeGet()
        {
            CWF.DataContracts.StatusCodeGetRequestDC request = new CWF.DataContracts.StatusCodeGetRequestDC();
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;

            CWF.DataContracts.StatusCodeGetReplyDC reply = null;

            try
            {
                reply = StatusCode.StatusCodeGet(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply = CWF.DAL.StatusCode.StatusCodeGet(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Searches for activities in the StoreActivities table")]
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void SearchActivities()
        {
            ActivitySearchRequestDC request = new ActivitySearchRequestDC();
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;

            ActivitySearchReplyDC reply = null;

            try
            {
                reply = ActivityRepositoryService.SearchActivities(request);

            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply =  ActivityRepositoryService.SearchActivities(request);");
            }
        }

        [Description("Searches for activities in the StoreActivities table")]
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALSearchActivities()
        {
            ActivitySearchRequestDC request = new ActivitySearchRequestDC();
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;

            ActivitySearchReplyDC reply = null;

            try
            {
                reply = ActivityBusinessService.SearchActivities(request);

            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) on reply =  ActivityRepositoryService.SearchActivities(request);");
            }
        }

        [Description("Get the entire activityLibrary table")]
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityLibraryGet()
        {
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            List<CWF.DataContracts.ActivityLibraryDC> reply = null;
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;

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
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALActivityLibraryGet()
        {
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            List<CWF.DataContracts.ActivityLibraryDC> reply = null;
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;

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
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityLibraryCreateANDUpdate()
        {
            CWF.DataContracts.ActivityLibraryDC reply = null;
            CWF.DataContracts.ActivityLibraryDC request = new CWF.DataContracts.ActivityLibraryDC();
            //// UPDATE
            try
            {
                request.Incaller = INCALLER;
                request.IncallerVersion = INCALLERVERSION;
                request.Description = "In NEW Description";
                request.Id = 100000;
                request.InsertedByUserAlias = INSERTEDBYUSERALIAS;
                request.UpdatedByUserAlias = UPDATEDBYUSERALIAS;
                try
                {
                    reply = ActivityLibrary.ActivityLibraryCreateOrUpdate(request);
                }
                catch (Exception deleteEx)
                {
                    string faultMessage = deleteEx.Message;
                    Assert.Fail(faultMessage + "-catch (Exception deleteEx) in reply = CWF.DAL.Activities.ActivityLibraryCreateOrUpdate(request);");
                }
                // TODO Unit testing fails(too many parameters) while TEST Harness works - Assert.AreEqual(reply.StatusReply.Errorcode, -60);
                //// update
                request.Guid = new Guid("ACE00000-0000-0000-0000-000000000000");
                request.Name = "TEST#200";
                request.AuthGroupName = "pqocwfauthors";
                request.CategoryName = "OAS Basic Controls";
                request.Category = Guid.Empty;
                request.Executable = new byte[4];
                request.HasActivities = true;
                request.ImportedBy = "REDMOND\v-stska";
                request.Description = "TEST DESCRIPTION";
                request.InsertedByUserAlias = INCALLER;
                request.VersionNumber = "1.0.0.1";
                request.Status = 990;
                request.UpdatedByUserAlias = "REDMOND\v-stska";

                //// update
                try
                {
                    reply = ActivityLibrary.ActivityLibraryCreateOrUpdate(request);
                }
                catch (Exception ex)
                {
                    string faultMessage = ex.Message;
                    Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.ActivityLibraryCreateOrUpdate(request);");
                }

                Assert.IsNotNull(reply);
                // TODO Unit testing fails(too many parameters) while TEST Harness works - Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.UPDATE_INVALID_ID);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                // TODO Unit testing fails(too many parameters) while TEST Harness works - Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.ActivityLibraryCreateOrUpdate(request);");
            }

            Assert.IsNotNull(reply);
            // TODO Unit testing fails(too many parameters) while TEST Harness works - Assert.AreEqual(reply.StatusReply.Errorcode, SprocValues.UPDATE_INVALID_ID);
        }

        [Description("Get a row from the activityCategory table")]
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityCategoryGet()
        {
            List<CWF.DataContracts.ActivityCategoryByNameGetReplyDC> reply = null;
            try
            {
                CWF.DataContracts.ActivityCategoryByNameGetRequestDC request = new CWF.DataContracts.ActivityCategoryByNameGetRequestDC();
                request.Incaller = INCALLER;
                request.IncallerVersion = "1.0.0.0";
                request.InName = "OAS Basic Controls";

                try
                {
                    reply = ActivityCategoryRepositoryService.GetActivityCategories(request);
                }
                catch (Exception ex)
                {
                    string faultMessage = ex.Message;
                    Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.ActivityCategoryGet(request);");
                }
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }


        [Description("Get a row from the activityCategory table")]
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALActivityCategoryGet()
        {
            List<CWF.DataContracts.ActivityCategoryByNameGetReplyDC> reply = null;
            try
            {
                CWF.DataContracts.ActivityCategoryByNameGetRequestDC request = new CWF.DataContracts.ActivityCategoryByNameGetRequestDC();
                request.Incaller = INCALLER;
                request.IncallerVersion = "1.0.0.0";
                request.InName = "OAS Basic Controls";

                try
                {
                    reply = ActivityCategoryBusinessService.GetActivityCategories(request);
                }
                catch (Exception ex)
                {
                    string faultMessage = ex.Message;
                    Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.ActivityCategoryGet(request);");
                }
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].StatusReply.Errorcode, SprocValues.REPLY_ERRORCODE_VALUE_OK);
        }

        [Description("Create or update a row in the activityCategory table")]
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityCategoryCreateOrUpdate()
        {
            CWF.DataContracts.ActivityCategoryCreateOrUpdateRequestDC request = new CWF.DataContracts.ActivityCategoryCreateOrUpdateRequestDC();
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            request.InDescription = Utility.GenerateRandomString(250);
            request.InGuid = Guid.NewGuid();
            request.InId = 0;
            request.InMetaTags = "Meta Data";
            request.InAuthGroupName = "pqocwfadmin";
            request.InName = "TESTHarness100" + Guid.NewGuid();
            request.InUpdatedByUserAlias = UPDATEDBYUSERALIAS;
            request.InInsertedByUserAlias = INSERTEDBYUSERALIAS;
            CWF.DataContracts.ActivityCategoryCreateOrUpdateReplyDC reply = null;

            try
            {
                reply = ActivityCategory.ActivityCategoryCreateOrUpdate(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.ActivityCategoryCreateOrUpdate(request);");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(SprocValues.REPLY_ERRORCODE_VALUE_OK, reply.StatusReply.Errorcode);
        }

        [Description("get the entire activityLibrary tree of dependency")]
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void ActivityLibraryDependenciesTreeGet()
        {
            var request = new CWF.DataContracts.StoreActivityLibrariesDependenciesDC();
            var rdl = new CWF.DataContracts.StoreDependenciesRootActiveLibrary();
            List<CWF.DataContracts.StoreActivityLibrariesDependenciesDC> reply = null;

            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
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
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALActivityLibraryDependenciesTreeGet()
        {
            var request = new CWF.DataContracts.StoreActivityLibrariesDependenciesDC();
            var rdl = new CWF.DataContracts.StoreDependenciesRootActiveLibrary();
            List<CWF.DataContracts.StoreActivityLibrariesDependenciesDC> reply = null;

            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
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


        [Description("Search workflow type")]
        [Owner("v-kason")]
        [TestCategory("Unit")]
        [TestMethod]
        public void WorkflowTypeSearch()
        {
            //Test create a new workflow type
            WorkFlowTypeCreateOrUpdateRequestDC request = new WorkFlowTypeCreateOrUpdateRequestDC();
            WorkFlowTypeCreateOrUpdateReplyDC reply = null;
            request.Incaller = INCALLER;
            request.IncallerVersion = INCALLERVERSION;
            request.InGuid = Guid.NewGuid();
            request.InName = "TestType_" + request.InGuid.ToString();
            request.InInsertedByUserAlias = INSERTEDBYUSERALIAS;
            request.InAuthGroupId = 2;
            try
            {
                reply = WorkflowTypeBusinessService.WorkflowTypeCreateOrUpdate(request);
                Assert.IsNotNull(reply);
                Assert.AreEqual(reply.StatusReply.Errorcode, 0);

                WorkflowTypeSearchRequest searchRequest = new WorkflowTypeSearchRequest();
                searchRequest.Incaller = INCALLER;
                searchRequest.IncallerVersion = INCALLERVERSION;
                searchRequest.SearchText = "TestType";
                searchRequest.PageSize = 10;
                searchRequest.PageNumber = 1;
                WorkflowTypeSearchReply searchReply = WorkflowTypeBusinessService.SearchWorkflowTypes(searchRequest);

                Assert.IsNotNull(searchReply);
                Assert.AreEqual(searchReply.StatusReply.Errorcode, 0);
                Assert.IsTrue(searchReply.ServerResultsLength >= 1);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.BAL.WorkflowTypeBusinessService.WorkflowTypeCreateOrUpdate();");
            }

        }

        [Description("Get all workflow types")]
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void WorkFlowTypesGet()
        {
            CWF.DataContracts.WorkflowTypeGetReplyDC reply = null;

            try
            {
                reply = WorkflowTypeRepositoryService.GetWorkflowTypes();
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.WorkflowTypesGet();");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, 0);
        }

        [Description("Get all workflow types")]
        [Owner(OWNER)]
        [TestCategory("Unit")]
        [TestMethod]
        public void BALWorkFlowTypesGet()
        {
            CWF.DataContracts.WorkflowTypeGetReplyDC reply = null;

            try
            {
                reply = WorkflowTypeBusinessService.GetWorkflowTypes();
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.Activities.WorkflowTypesGet();");
            }

            Assert.IsNotNull(reply);
            Assert.AreEqual(reply.StatusReply.Errorcode, 0);
        }

        [Description("Get the marketplace details")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void MarketplaceAssetDetailsGet()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchDetail();

            CWF.DataContracts.Marketplace.MarketplaceAssetDetails reply = null;

            request.Id = 0;
            request.AssetType = CWF.DataContracts.Marketplace.AssetType.Project;

            //// Get
            try
            {
                reply = MarketplaceRepositoryService.GetMarketplaceAssetDetails(request);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.MarketplaceRepositoryService.GetMarketplaceAssetDetails(request, get)");
            }

            Assert.IsNull(reply);
        }

        [Description("Get the marketplace result")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void SearchMarketplaceWithFilterNone()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchQuery();

            CWF.DataContracts.Marketplace.MarketplaceSearchResult reply = null;

            request.SearchText = "microsoft";
            request.FilterType = CWF.DataContracts.Marketplace.MarketplaceFilter.None;
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

            //// Get
            try
            {
                reply = MarketplaceRepositoryService.SearchMarketplace(request);
                var detailRequest = new CWF.DataContracts.Marketplace.MarketplaceSearchDetail();
                CWF.DataContracts.Marketplace.MarketplaceAssetDetails detailReply = null;
                Assert.IsNotNull(reply);
                foreach (var item in reply.Items)
                {
                    detailRequest.Id = item.Id;
                    detailRequest.AssetType = item.AssetType;
                    detailReply = MarketplaceRepositoryService.GetMarketplaceAssetDetails(detailRequest);
                    Assert.IsNotNull(detailReply);
                }
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.MarketplaceRepositoryService.GetMarketplaceAssetDetails(request, get)");
            }

        }

        [Description("Get the marketplace result")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void SearchMarketplaceWithFilterActivities()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchQuery();

            CWF.DataContracts.Marketplace.MarketplaceSearchResult reply = null;

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

            //// Get
            try
            {
                reply = MarketplaceRepositoryService.SearchMarketplace(request);
                Assert.IsNotNull(reply);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.MarketplaceRepositoryService.GetMarketplaceAssetDetails(request, get)");
            }

        }

        [Description("Get the marketplace result")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void SearchMarketplaceWithFilterTemplates()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchQuery();

            CWF.DataContracts.Marketplace.MarketplaceSearchResult reply = null;

            request.SearchText = "microsoft";
            request.FilterType = CWF.DataContracts.Marketplace.MarketplaceFilter.Templates;
            request.PageSize = 15;
            request.PageNumber = 1;
            request.UserRole = "Author";
            request.SortCriteria = new List<CWF.DataContracts.Marketplace.SortCriterion>
                                    {
                                        new CWF.DataContracts.Marketplace.SortCriterion()
                                        {
                                            FieldName="Name",
                                            IsAscending=true,
                                    }};

            //// Get
            try
            {
                reply = MarketplaceRepositoryService.SearchMarketplace(request);
                Assert.IsNull(reply);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.MarketplaceRepositoryService.GetMarketplaceAssetDetails(request, get)");
            }

        }

        [Description("Get the marketplace result")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        public void SearchMarketplaceWithFilterPublish()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchQuery();

            CWF.DataContracts.Marketplace.MarketplaceSearchResult reply = null;

            request.SearchText = "microsoft";
            request.FilterType = CWF.DataContracts.Marketplace.MarketplaceFilter.PublishingWorkflows;
            request.PageSize = 15;
            request.PageNumber = 1;
            request.UserRole = "Author";
            request.SortCriteria = new List<CWF.DataContracts.Marketplace.SortCriterion>
                                    {
                                        new CWF.DataContracts.Marketplace.SortCriterion()
                                        {
                                            FieldName="Name",
                                            IsAscending=true,
                                    }};

            //// Get
            try
            {
                reply = MarketplaceRepositoryService.SearchMarketplace(request);
                Assert.IsNull(reply);
            }
            catch (Exception ex)
            {
                string faultMessage = ex.Message;
                Assert.Fail(faultMessage + "-catch (Exception ex) in reply = CWF.DAL.MarketplaceRepositoryService.GetMarketplaceAssetDetails(request, get)");
            }
        }


        [Description("Get the marketplace result")]
        [Owner("v-bobzh")]
        [TestCategory("Unit")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SearchMarketplaceWithFilterWrongCritieria()
        {
            var request = new CWF.DataContracts.Marketplace.MarketplaceSearchQuery();

            CWF.DataContracts.Marketplace.MarketplaceSearchResult reply = null;

            request.SearchText = "microsoft";
            request.FilterType = CWF.DataContracts.Marketplace.MarketplaceFilter.PublishingWorkflows;
            request.PageSize = 15;
            request.PageNumber = 1;
            request.UserRole = "";
            request.SortCriteria = new List<CWF.DataContracts.Marketplace.SortCriterion>
                                    {
                                        new CWF.DataContracts.Marketplace.SortCriterion()
                                        {
                                            FieldName="Name",
                                            IsAscending=true,
                                    }};

            //// Get
            reply = MarketplaceRepositoryService.SearchMarketplace(request);
        }
    }


}
