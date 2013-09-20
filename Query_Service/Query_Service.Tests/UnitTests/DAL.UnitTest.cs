//-----------------------------------------------------------------------
// <copyright file="DAL.UnitTest.cs" company="Microsoft">
// Copyright
// DAL Unit Test
// </copyright>
//-----------------------------------------------------------------------

namespace Query_Service.UnitTests
{
    using CWF.DataContracts;
    using Microsoft.Support.Workflow.Service.DataAccessServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Unit tests for QueryService BAl and DAL layer
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "This not required")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Not required fot const/unit tests")]
    [TestClass]
    public class DALUnitTest
    {
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
            storeActivitiesDC.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            storeActivitiesDC.Environment = UnitTestConstant.TOENVIRONMENT;
            storeActivitiesDC.Incaller = UnitTestConstant.INCALLER;
            storeActivitiesDC.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            storeActivitiesDC.InInsertedByUserAlias = UnitTestConstant.INSERTEDBYUSERALIAS;
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
            storeActivitiesDC.InUpdatedByUserAlias = UnitTestConstant.UPDATEDBYUSERALIAS;
            storeActivitiesDC.Version = version;
            storeActivitiesDC.OldVersion = version;
            //// saDC.Version = "1.0.0.0";
            storeActivitiesDC.WorkflowTypeName = "Workflow";
            storeActivitiesDC.Xaml = "<XamlBeginTag></XamlBeginTag>";

            storeActivitiesDC.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            storeActivitiesDC.Environment = UnitTestConstant.TOENVIRONMENT;
            return storeActivitiesDC;
        }

        #region [ CreateActivityLibraryAndStoreActivities  ]

        public static void CreateActivityLibraryAndStoreActivities(out CWF.DataContracts.ActivityLibraryDC activityLibraryDC, out List<CWF.DataContracts.StoreActivitiesDC> storeActivityDC)
        {
            //// Setup the base request objects in request
            List<CWF.DataContracts.StoreActivitiesDC> storeActivityRequest = new List<CWF.DataContracts.StoreActivitiesDC>();

            CWF.DataContracts.ActivityLibraryDC alDC = new CWF.DataContracts.ActivityLibraryDC();
            CWF.DataContracts.StoreActivitiesDC saDC = new CWF.DataContracts.StoreActivitiesDC();

            //// population the request object
            //// create activity library entry

            alDC.Incaller = UnitTestConstant.INCALLER;
            alDC.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            alDC.Guid = Guid.NewGuid();
            alDC.Name = "TEST#421A";
            alDC.AuthGroupName = "pqocwfauthors";
            alDC.CategoryName = "OAS Basic Controls";
            alDC.Category = Guid.Empty;
            alDC.Executable = new byte[4];
            alDC.HasActivities = true;
            alDC.ImportedBy = "REDMOND\v-stska";
            alDC.Description = "TEST#421A DESCRIPTION";
            alDC.InInsertedByUserAlias = "v-stska";
            alDC.VersionNumber = "1.0.0.8";
            alDC.Status = 1;
            alDC.StatusName = "Private";
            alDC.InUpdatedByUserAlias = "REDMOND\v-stska1";
            alDC.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
            alDC.Environment = UnitTestConstant.TOENVIRONMENT;

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

            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Incaller = UnitTestConstant.INCALLER;
            request.InInsertedByUserAlias = UnitTestConstant.INCALLER;
            request.InUpdatedByUserAlias = UnitTestConstant.INCALLER;
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
                        IncallerVersion = UnitTestConstant.INCALLERVERSION,
                        Incaller = UnitTestConstant.INCALLER,
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

            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Incaller = UnitTestConstant.INCALLER;
            request.InInsertedByUserAlias = UnitTestConstant.INCALLER;
            request.InUpdatedByUserAlias = UnitTestConstant.INCALLER;
            request.EnforceVersionRules = true;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };
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
                        IncallerVersion = UnitTestConstant.INCALLERVERSION,
                        Incaller = UnitTestConstant.INCALLER,
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
            }
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

            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Incaller = UnitTestConstant.INCALLER;
            request.InInsertedByUserAlias = UnitTestConstant.INCALLER;
            request.InUpdatedByUserAlias = UnitTestConstant.INCALLER;
            request.EnforceVersionRules = true;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };

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
                        IncallerVersion = UnitTestConstant.INCALLERVERSION,
                        Incaller = UnitTestConstant.INCALLER,
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

            request.IncallerVersion = UnitTestConstant.INCALLERVERSION;
            request.Incaller = UnitTestConstant.INCALLER;
            request.InInsertedByUserAlias = UnitTestConstant.INCALLER;
            request.InUpdatedByUserAlias = UnitTestConstant.INCALLER;
            request.EnforceVersionRules = false;
            request.InAuthGroupNames = new string[] { UnitTestConstant.AUTHORGROUPNAME };

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
                        IncallerVersion = UnitTestConstant.INCALLERVERSION,
                        Incaller = UnitTestConstant.INCALLER,
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

            }
        }
        #endregion

    }
}
