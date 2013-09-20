using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using System.Reflection;
//using Microsoft.Support.Workflow.Service.Test.Common;
using CWF.DAL;
using Microsoft.Support.Workflow.Service.Test.Common;

namespace Microsoft.Support.Workflow.Service.DataAccessServices.Tests.FunctionalTests
{
    [TestClass]
    public class StoreActivityRepositoryServiceShould
    {
        [Description("")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void AllowNullDescriptionWhenStoreActivitiesCreateOrUpdateIsCalled()
        {
            StoreActivitiesDC activity = CreateStoreActivity();
            activity.Description = null;

            var reply = Activities.StoreActivitiesCreateOrUpdate(activity);
            Assert.IsNotNull(reply);
            Assert.AreEqual(0, reply.StatusReply.Errorcode);

            var actual = Activities.StoreActivitiesGetByName(activity.Name, "dev");
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(activity.Guid, actual[0].Guid);
            Assert.AreEqual(activity.ShortName, actual[0].ShortName);
            Assert.AreEqual(activity.Namespace, actual[0].Namespace);

            activity.Id = actual[0].Id;
            Activities.StoreActivitiesDelete(activity);
        }

        private static StoreActivitiesDC CreateStoreActivity()
        {
            return new StoreActivitiesDC 
            {
                Incaller = Assembly.GetExecutingAssembly().FullName,
                IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                Guid = Guid.NewGuid(),
                Name = Utility.GenerateRandomString(100),
                ShortName = Utility.GenerateRandomString(50),
                Description = Utility.GenerateRandomString(200),
                AuthGroupId = 1,
                AuthGroupName = "pqocwfauthors",
                MetaTags = Utility.GenerateRandomString(50),
                IsService = false,
                ActivityLibraryName = Utility.GenerateRandomString(100),
                ActivityLibraryVersion = new Version(Utility.GenerateRandomNumber(3), Utility.GenerateRandomNumber(100), Utility.GenerateRandomNumber(3000)).ToString(),
                ActivityCategoryId = 1,
                Version = new Version(Utility.GenerateRandomNumber(3), Utility.GenerateRandomNumber(100), Utility.GenerateRandomNumber(3000)).ToString(),
                WorkflowTypeID = 1,
                WorkflowTypeName = "Page",
                Locked = false,
                LockedBy = string.Empty,
                IsCodeBeside = false,
                Xaml = null,
                Namespace = Utility.GenerateRandomString(20),
                InInsertedByUserAlias = "v-sanja",
                InUpdatedByUserAlias = "v-sanja",
                StatusCodeName = "Private",
                StatusId = 1000,
                InAuthGroupNames = new string[] { "pqocwfauthors" },
                Environment="Dev",

            };
        }

    }
}
