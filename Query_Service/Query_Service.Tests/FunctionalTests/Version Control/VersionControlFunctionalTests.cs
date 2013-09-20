using CWF.BAL;
using CWF.WorkflowQueryService;
using CWF.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query_Service.Tests.Common;
using Query_Service.Tests.FunctionalTests.Version_Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Query_Service.UnitTests;


namespace Query_Service.Tests.FunctionalTests.Version_Control
{
    /// <summary>
    /// These are the functional tests for version rule check in query service.
    /// </summary>
    [TestClass]
    public class VersionControlFunctionalTests
    {
        public TestContext TestContext { get; set; }
        const string privatestate = "Private";
        const string publicstate = "Public";
        const string retiredstate = "Retired";
        const int privatestatusid = 1000;
        const int publicstatusid = 1010;
        const int retiredstatusid = 1020;
        const string incallerversion = "1.2.3.7test";
        const string OWNER = "shkapoor";
        [WorkItem(139148)]
        [Owner(OWNER)]
        [Description("Save project as Private Status" +
                     "Verify version and status of AL and SA")]
        [TestCategory(TestCategory.Func)]
        [TestCategory(QualityGates.Functionality)]
        [TestMethod()]
        public void SaveProjectAsPrivate()
        {
            //Create WF with private AL, private SA
            var workflow = new StoreLibraryAndActivitiesRequestDC();
            workflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDC(privatestate, privatestatusid);
            var StoreActList = new System.Collections.Generic.List<StoreActivitiesDC>();
            var storeAct = VersionControlHelper.GetStoreActivitiesDC(workflow.ActivityLibrary, 1);
            StoreActList.Add(storeAct);
            workflow.StoreActivitiesList = StoreActList;
            var result = VersionControlHelper.CreateAndUploadWorkFlow(workflow);
            Assert.AreEqual(0, result.Errorcode);
            //get AL and SA info
            var reply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //assert on all private status and version of WF is 0.0.0.1, version of both AL and stored activity should be same.
            VersionControlHelper.CheckOutput(reply, privatestate, "0.0.0.1", privatestatusid);
            //Now change status to private
            workflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDC(privatestate, privatestatusid);
            workflow.StoreActivitiesList[0] = VersionControlHelper.GetStoreActivitiesDC(workflow.ActivityLibrary, 1);
            //Save
            string version = CWF.BAL.Versioning.VersionHelper.GetNextVersion(storeAct).ToString();
            workflow.ActivityLibrary.VersionNumber = version;
            workflow.StoreActivitiesList[0].Version = version;
            result = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(workflow)[0].StatusReply;
            Assert.AreEqual(0, result.Errorcode);
            //Get
            reply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //assert on status and version 0.1.0.1
            VersionControlHelper.CheckOutput(reply, privatestate, "0.0.0.2", privatestatusid);
        }

        [WorkItem(139149)]
        [Owner(OWNER)]
        [Description("Save project as Public Status" +
                     "Verify version and status of AL and SA")]
        [TestCategory(TestCategory.Func)]
        [TestCategory(QualityGates.Functionality)]
        [TestMethod()]
        public void SaveProjectAsPublic()
        {
            //Create WF with public status, public AL, public SA
            string expectedVersion = "0.1.0.0";
            var workflow = new StoreLibraryAndActivitiesRequestDC();
            workflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDC(publicstate, publicstatusid);
            workflow.ActivityLibrary.VersionNumber = expectedVersion;
            var StoreActList = new System.Collections.Generic.List<StoreActivitiesDC>();
            var storeAct = VersionControlHelper.GetStoreActivitiesDC(workflow.ActivityLibrary, 1);
            StoreActList.Add(storeAct);
            workflow.StoreActivitiesList = StoreActList;
            var result = VersionControlHelper.CreateAndUploadWorkFlow(workflow);
            Assert.AreEqual(0, result.Errorcode);
            //get  AL and SA info
            var reply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //assert on all public status and version of WF is 0.1.0.0
            VersionControlHelper.CheckOutput(reply, publicstate, expectedVersion, publicstatusid);
            //Now change status to public
            workflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDC(publicstate, publicstatusid);
            workflow.StoreActivitiesList[0] = VersionControlHelper.GetStoreActivitiesDC(workflow.ActivityLibrary, 1);
            //Save
            string version = CWF.BAL.Versioning.VersionHelper.GetNextVersion(storeAct).ToString();
            workflow.ActivityLibrary.VersionNumber = version;
            workflow.StoreActivitiesList[0].Version = version;
            result = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(workflow)[0].StatusReply;
            Assert.AreEqual(0, result.Errorcode);
            //Get
            reply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //assert on status and version 0.1.0.0..zeroing out revision number when going public
            VersionControlHelper.CheckOutput(reply, publicstate, "0.2.0.0", publicstatusid);
        }

        [WorkItem(139150)]
        [Owner(OWNER)]
        [Description("Save project as Private Status -> change the status to public" +
                     "Verify version and status of AL and SA")]
        [TestCategory(TestCategory.Func)]
        [TestCategory(QualityGates.Functionality)]
        [TestMethod()]
        public void SavePrivateProjectAndThenMakeItPublic()
        {
            //Create WF with private SA, and AL
            var workflow = new StoreLibraryAndActivitiesRequestDC();
            workflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDC(privatestate, privatestatusid);
            var StoreActList = new System.Collections.Generic.List<StoreActivitiesDC>();
            var storeAct = VersionControlHelper.GetStoreActivitiesDC(workflow.ActivityLibrary, 1);
            StoreActList.Add(storeAct);
            workflow.StoreActivitiesList = StoreActList;

            var result = VersionControlHelper.CreateAndUploadWorkFlow(workflow);
            Assert.AreEqual(0, result.Errorcode);
            //get WF, check version for both SA and AL
            var reply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //Check status and version
            VersionControlHelper.CheckOutput(reply, privatestate, "0.0.0.1", privatestatusid);
            //Now change status to public
            workflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDC(publicstate, publicstatusid);
            workflow.StoreActivitiesList[0] = VersionControlHelper.GetStoreActivitiesDC(workflow.ActivityLibrary, 1);
            //Save
            string version = CWF.BAL.Versioning.VersionHelper.GetNextVersion(storeAct).ToString();
            workflow.ActivityLibrary.VersionNumber = version;
            workflow.StoreActivitiesList[0].Version = version;
            result = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(workflow)[0].StatusReply;
            Assert.AreEqual(0, result.Errorcode);
            //Get
            reply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //assert on status and version 0.1.0.0..zeroing out revision number when going public
            VersionControlHelper.CheckOutput(reply, publicstate, "0.0.0.2", publicstatusid);
        }

        [WorkItem(139151)]
        [Owner(OWNER)]
        [Description("Save project as Public Status -> change the status to public" +
                     "Verify version and status of AL and SA")]
        [TestCategory(TestCategory.Func)]
        [TestCategory(QualityGates.Functionality)]
        [TestMethod()]
        public void SavePublicProjectAndThenMakeItPrivate()
        {
            //Create WF with private SA, and AL
            string expectedVersion = "0.1.0.0";
            var workflow = new StoreLibraryAndActivitiesRequestDC();
            workflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDC(publicstate, publicstatusid);
            workflow.ActivityLibrary.VersionNumber = expectedVersion;
            var StoreActList = new System.Collections.Generic.List<StoreActivitiesDC>();
            var storeAct = VersionControlHelper.GetStoreActivitiesDC(workflow.ActivityLibrary, 1);
            StoreActList.Add(storeAct);
            workflow.StoreActivitiesList = StoreActList;

            var result = VersionControlHelper.CreateAndUploadWorkFlow(workflow);
            Assert.AreEqual(0, result.Errorcode);
            //get WF, check version for both SA and AL
            var reply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //Check status and version
            VersionControlHelper.CheckOutput(reply, publicstate, "0.1.0.0", publicstatusid);
            //Now change status to private
            workflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDC(privatestate, privatestatusid);
            workflow.StoreActivitiesList[0] = VersionControlHelper.GetStoreActivitiesDC(workflow.ActivityLibrary, 1);
            //Save
            string version = CWF.BAL.Versioning.VersionHelper.GetNextVersion(storeAct).ToString();
            workflow.ActivityLibrary.VersionNumber = version;
            workflow.StoreActivitiesList[0].Version = version;
            result = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(workflow)[0].StatusReply;
            Assert.AreEqual(0, result.Errorcode);
            //Get
            reply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //assert on status and version 0.1.0.1
            VersionControlHelper.CheckOutput(reply, privatestate, "0.2.0.0", privatestatusid);
        }

        [WorkItem(139152)]
        [Owner(OWNER)]
        [Description("Create New project -> save as private -> Save new project as Public Status with name same as previous step" +
                     "Verify name collision error is thrown")]
        [TestCategory(TestCategory.Func)]
        [TestCategory(QualityGates.Functionality)]
        [TestMethod()]
        public void CreateNewProjectWithExistingName()
        {
            //Create WF with private AL, private SA
            var workflow = new StoreLibraryAndActivitiesRequestDC();
            workflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDC(privatestate, privatestatusid);
            var StoreActList = new System.Collections.Generic.List<StoreActivitiesDC>();
            var storeAct = VersionControlHelper.GetStoreActivitiesDC(workflow.ActivityLibrary, 1);
            StoreActList.Add(storeAct);
            workflow.StoreActivitiesList = StoreActList;
            workflow.InAuthGroupNames = new string[] { "pqocwfadmin" };

            var result = VersionControlHelper.CreateAndUploadWorkFlow(workflow);
            Assert.AreEqual(0, result.Errorcode);
            //get WF, check version for both SA and AL
            var reply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //Check status and version
            VersionControlHelper.CheckOutput(reply, privatestate, "0.0.0.1", privatestatusid);
            //Create again with similar name as previous one
            //Create WF with private AL, private SA
            var newworkflow = new StoreLibraryAndActivitiesRequestDC();
            newworkflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDCWithName(privatestate, privatestatusid, workflow.ActivityLibrary.Name);
            List<StoreActivitiesDC> NewStoreActList = new System.Collections.Generic.List<StoreActivitiesDC>();
            StoreActivitiesDC NewstoreAct = VersionControlHelper.GetStoreActivitiesDC(newworkflow.ActivityLibrary, 1);
            StoreActList.Add(NewstoreAct);
            newworkflow.StoreActivitiesList = NewStoreActList;

            newworkflow.Incaller = Environment.UserName;
            newworkflow.IncallerVersion = incallerversion;
            newworkflow.InInsertedByUserAlias = Environment.UserName;
            newworkflow.InUpdatedByUserAlias = Environment.UserName;
            StatusReplyDC Newresult = new StatusReplyDC();
            //Save WF
            string version = CWF.BAL.Versioning.VersionHelper.GetNextVersion(NewstoreAct).ToString();
            newworkflow.ActivityLibrary.VersionNumber = version;
            Newresult = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(newworkflow)[0].StatusReply;
            //Expect error
            Assert.AreEqual(0, Newresult.Errorcode);
        }

        [WorkItem(139153)]
        [Owner(OWNER)]
        [Description("Create New project -> save as retired -> Save new project as Public Status with name same as previous step" +
                     "Verify name collision error is thrown")]
        [TestCategory(TestCategory.Func)]
        [TestCategory(QualityGates.Functionality)]
        [TestMethod()]
        public void CreateNewProjectWithExistingNameAsRetired()
        {
            //Create WF with private AL, private SA
            var workflow = new StoreLibraryAndActivitiesRequestDC();
            workflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDC(retiredstate, retiredstatusid);
            var StoreActList = new System.Collections.Generic.List<StoreActivitiesDC>();
            var storeAct = VersionControlHelper.GetStoreActivitiesDC(workflow.ActivityLibrary, 1);
            StoreActList.Add(storeAct);
            workflow.StoreActivitiesList = StoreActList;

            var result = VersionControlHelper.CreateAndUploadWorkFlow(workflow);
            Assert.AreEqual(0, result.Errorcode);
            //get WF, check version for both SA and AL
            var reply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //Check status and version
            VersionControlHelper.CheckOutput(reply, retiredstate, "0.0.0.1", retiredstatusid);
            //Create again with similar name as previous one
            //Create WF with private AL, private SA
            var newworkflow = new StoreLibraryAndActivitiesRequestDC();
            newworkflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDCWithName(privatestate, privatestatusid, workflow.ActivityLibrary.Name);
            var NewStoreActList = new System.Collections.Generic.List<StoreActivitiesDC>();
            var NewstoreAct = VersionControlHelper.GetStoreActivitiesDC(newworkflow.ActivityLibrary, 1);
            StoreActList.Add(NewstoreAct);
            newworkflow.StoreActivitiesList = NewStoreActList;

            newworkflow.Incaller = Environment.UserName;
            newworkflow.IncallerVersion = incallerversion;
            newworkflow.InInsertedByUserAlias = Environment.UserName;
            newworkflow.InUpdatedByUserAlias = Environment.UserName;
            StatusReplyDC Newresult = new StatusReplyDC();
            //Save WF
            string version = CWF.BAL.Versioning.VersionHelper.GetNextVersion(NewstoreAct).ToString();
            newworkflow.ActivityLibrary.VersionNumber = version;
            Newresult = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(newworkflow)[0].StatusReply;
            //Expect error
            Assert.AreEqual(0, Newresult.Errorcode);
        }

        [WorkItem(139154)]
        [Owner(OWNER)]
        [Description("Create New project -> save as private->Publish Project" +
                     "Verify version and status of AL and SA should be public")]
        [TestCategory(TestCategory.Func)]
        [TestCategory(QualityGates.Functionality)]
        [TestMethod()]
        [Ignore] //ignore since publish need specific environment
        public void PublishProject()
        {
            //Create WF with private AL, private SA
            var workflow = new StoreLibraryAndActivitiesRequestDC();
            workflow.ActivityLibrary = VersionControlHelper.GetActivityLibraryDC(privatestate, privatestatusid);
            var StoreActList = new System.Collections.Generic.List<StoreActivitiesDC>();
            var storeAct = VersionControlHelper.GetStoreActivitiesDC(workflow.ActivityLibrary, 1);
            StoreActList.Add(storeAct);
            workflow.StoreActivitiesList = StoreActList;

            var result = VersionControlHelper.CreateAndUploadWorkFlow(workflow);
            Assert.AreEqual(0, result.Errorcode);
            //get WF, check version for both SA and AL
            var reply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //Check status and version
            VersionControlHelper.CheckOutput(reply, privatestate, "0.0.0.1", privatestatusid);
            //Publish Project

            //assert the status is now public in DB with status revised as 0.1.0.0
            var Newreply = VersionControlHelper.GetProjectInfo(workflow.ActivityLibrary, workflow.StoreActivitiesList);
            //Check status and version
            VersionControlHelper.CheckOutput(Newreply, publicstate, "0.1.0.1", publicstatusid);
        }
    }
}
