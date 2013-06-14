using CWF.DataContracts;
using CWF.BAL.Versioning;
using CWF.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Query_Service.Testsproject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Query_Service.Tests.FunctionalTests.Version_Control
{

    /// <summary>
    /// This class provides helper methods to be used in the versioncontrolfunctional test class
    /// </summary>
    public class VersionControlHelper
    {
        
        const string incallerversion = "1.2.3.7test";
        private static string GenerateRandomString(int size)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Creates Activity Library Object given actilitylibrary(i.e public/private/retired status of the activity) and status id(the id corresponding
        /// to each status)
        /// </summary>
        public static ActivityLibraryDC GetActivityLibraryDC(string activityLibraryStatus, int activityLibraryStatusID)
        {  
            var activityLibraryDC = new ActivityLibraryDC()
                {
                    Name = "test"+GenerateRandomString(4),
                    VersionNumber = "0.0.0.1",
                    Executable = new byte[10],
                    Description = GenerateRandomString(50),
                    AuthGroupName = @"pqocwfauthors",
                    CategoryName = "OAS Basic Controls",
                    ImportedBy = Environment.UserName,
                    Guid = Guid.NewGuid(),
                    StatusName = activityLibraryStatus,
                    Status = activityLibraryStatusID,
                    MetaTags = null,
                    HasActivities = true,
                    AuthGroupId = 2,
                    Incaller = Environment.UserName,
                    IncallerVersion = incallerversion,
                    InsertedByUserAlias = Environment.UserName,
                    UpdatedByUserAlias = Environment.UserName,
                    Category = Guid.NewGuid(),
                    CategoryId = 3
                };
                return activityLibraryDC;
        }

        /// <summary>
        /// Creates Activity Library Object. Same as the method above but additional parameter as name
        /// This method is used when trying to test for name collosion
        /// </summary>
        public static ActivityLibraryDC GetActivityLibraryDCWithName(string activityLibraryStatus, int activityLibraryStatusID, string name)
        {
            var activityLibraryDC = new ActivityLibraryDC()
            {
                Name = name,
                VersionNumber = "0.0.0.1",
                Executable = new byte[10],
                Description = GenerateRandomString(50),
                AuthGroupName = @"pqocwfauthors",
                CategoryName = "OAS Basic Controls",
                ImportedBy = Environment.UserName,
                Guid = Guid.NewGuid(),
                StatusName = activityLibraryStatus,
                Status = activityLibraryStatusID,
                MetaTags = null,
                HasActivities = true,
                AuthGroupId = 2,
                Incaller = Environment.UserName,
                IncallerVersion = incallerversion,
                InsertedByUserAlias = Environment.UserName,
                UpdatedByUserAlias = Environment.UserName,
                Category = Guid.NewGuid(),
                CategoryId = 3
            };
            return activityLibraryDC;
        }

        /// <summary>
        /// This method creates store activity. Since there is a DB dependency of store activities on Activity library, we pass activity library as input
        /// </summary>
        /// <param name="activityItem">
        /// The activity item corresponding to the store activity
        /// </param>
        /// <param name="workflowtypeid">
        /// This parameter allows us to pass different type of workflows to test versioning
        /// </param>
        /// <returns>
        /// store activity
        /// </returns>
        public static StoreActivitiesDC GetStoreActivitiesDC(ActivityLibraryDC activityLibrary, int workflowtypeid)
        {
            var storeActivity = new StoreActivitiesDC
            {
                ActivityCategoryName = activityLibrary.CategoryName,
                Description = GenerateRandomString(5),
                Name = activityLibrary.Name.Substring(0,8),
                ShortName = activityLibrary.Name.Substring(0, 8),
                IsCodeBeside = true,
                Version = activityLibrary.VersionNumber,
                MetaTags = "Meta Data",
            };
            storeActivity.ActivityCategoryId = activityLibrary.CategoryId;
            storeActivity.Locked = false;
            storeActivity.IsCodeBeside = true;

            storeActivity.IconsId = 1;
            storeActivity.IsService = false;
            storeActivity.StatusCodeName = activityLibrary.StatusName;

            storeActivity.ToolBoxtab = 1;
            storeActivity.Guid = Guid.NewGuid();
            storeActivity.Incaller = Environment.UserName;
            storeActivity.LockedBy = Environment.UserName;
            storeActivity.UpdatedByUserAlias = Environment.UserName;
            storeActivity.InsertedByUserAlias = Environment.UserName;
            storeActivity.StatusId = activityLibrary.Status;

            storeActivity.IncallerVersion = activityLibrary.IncallerVersion;
            storeActivity.WorkflowTypeID = workflowtypeid;
            storeActivity.ActivityLibraryId = activityLibrary.Id;
            storeActivity.ActivityLibraryName = activityLibrary.Name;
            storeActivity.ActivityLibraryVersion = activityLibrary.VersionNumber;
            storeActivity.WorkflowTypeName = "Page";
            storeActivity.AuthGroupId = activityLibrary.AuthGroupId;
            storeActivity.AuthGroupName = activityLibrary.AuthGroupName;
            storeActivity.Xaml = "<XamlBeginTag></XamlBeginTag>";
            
            return storeActivity;
        }
        
        /// <summary>
        /// Thhis method gets information of the store activity and activity library. These are passed as input params
        /// </summary>
        /// <param name="activityLibrary"></param>
        /// Activity libraray for which information is required from the DB
        /// <param name="storeActivitiesList"></param>
        /// Store Activity for which information is required from the DB
        /// <returns></returns>
        public static List<GetLibraryAndActivitiesDC> GetProjectInfo(ActivityLibraryDC activityLibrary, List<StoreActivitiesDC> storeActivitiesList)
        {
            List<GetLibraryAndActivitiesDC> reply = null;
            GetLibraryAndActivitiesDC request = new GetLibraryAndActivitiesDC();
            request.ActivityLibrary = activityLibrary;
            request.StoreActivitiesList = storeActivitiesList;
            reply = CWF.BAL.Services.GetLibraryAndActivities(request);
            return reply;
        }

        /// <summary>
        /// This method checks for the o/p returned from the DB
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="statusName"></param>
        /// <param name="version"></param>
        /// <param name="statusid"></param>
        public static void CheckOutput(List<GetLibraryAndActivitiesDC> reply, string statusName, string version, int statusid)
        {
            Assert.IsNotNull(reply);
            Assert.AreEqual(reply[0].ActivityLibrary.VersionNumber, version);
            Assert.AreEqual(reply[0].ActivityLibrary.Status, statusid);
            Assert.AreEqual(reply[0].ActivityLibrary.StatusName, statusName);
            Assert.AreEqual(reply[0].StoreActivitiesList[0].ActivityLibraryName,reply[0].StoreActivitiesList[0].Name);
            Assert.AreEqual(reply[0].StoreActivitiesList[0].Version, version);
        }

        /// <summary>
        /// This method attaches additional properties to the workflow and saves it to Database
        /// </summary>
        /// <param name="WorkFlow"></param>
        /// <returns></returns>
        public static StatusReplyDC CreateAndUploadWorkFlow(StoreLibraryAndActivitiesRequestDC workflow)
        {
            workflow.Incaller = Environment.UserName;
            workflow.IncallerVersion = incallerversion;
            workflow.InInsertedByUserAlias = Environment.UserName;
            workflow.InUpdatedByUserAlias = Environment.UserName;
            var result = new StatusReplyDC();
            //Save WF
            result = CWF.BAL.Services.UploadActivityLibraryAndDependentActivities(workflow)[0].StatusReply;
            return result;
        }
        
    }
}
