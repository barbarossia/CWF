using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CWF.DataContracts;
using CWF.DAL;
using Microsoft.Support.Workflow.Service.DataAccessServices.Tests.Utilities;

namespace Microsoft.Support.Workflow.Service.DataAccessServices.Tests.FunctionalTests
{
    [TestClass]
    public class ActivityCategoryRepositoryServiceShould
    {
        [Description("Verifies whether GetActivityCategories method returns the matching item when called with Name.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnActivityCategoryWhenGetActivityCategoriesIsCalledWithValidName()
        {
            var expected = CreateActivityCategoryObject();
            ActivityCategory.ActivityCategoryCreateOrUpdate(expected);
            var actual = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = 0, InName = expected.InName }); // Valid name.  Id is unspecified.
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count == 1);
            Assert.AreEqual(expected.InName, actual[0].Name);

            // Cleanup
            ActivityCategoryTestDataAccessUtility.DeleteActivityCategory(actual[0].Id);
        }

        [Description("Verifies whether GetActivityCategories method returns the matching item when called with Id.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnActivityCategoryWhenGetActivityCategoriesIsCalledWithValidId()
        {
            var expected = CreateActivityCategoryObject();
            ActivityCategory.ActivityCategoryCreateOrUpdate(expected);

            // Since we don't have the ID yet, first get by name and then use that Id to call get by Id.
            var actual = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = 0, InName = expected.InName });

            var actualById = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = actual[0].Id, InName = string.Empty });  // Get by valid Id.  Name is unspecified.
            Assert.IsNotNull(actualById);
            Assert.IsNotNull(actualById);
            Assert.IsTrue(actualById.Count == 1);
            Assert.AreEqual(expected.InName, actualById[0].Name);
            
            // Cleanup
            ActivityCategoryTestDataAccessUtility.DeleteActivityCategory(actual[0].Id);
        }

        [Description("Verifies whether GetActivityCategories method returns the matching item when called with Id and invalid name.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnActivityCategoryWhenGetActivityCategoriesIsCalledWithValidIdAndInvalidName()
        {
            var expected = CreateActivityCategoryObject();
            ActivityCategory.ActivityCategoryCreateOrUpdate(expected);

            // Since we don't have the ID yet, first get by name and then use that Id to call get by Id.
            var actual = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = 0, InName = expected.InName });

            var actualById = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = actual[0].Id, InName = Guid.NewGuid().ToString() });  // Get by valid Id.  Invalid name.
            Assert.IsNotNull(actualById);
            Assert.IsNotNull(actualById);
            Assert.IsTrue(actualById.Count == 1);
            Assert.AreEqual(expected.InName, actualById[0].Name);

            // Cleanup
            ActivityCategoryTestDataAccessUtility.DeleteActivityCategory(actual[0].Id);
        }

        [Description("Verifies whether GetActivityCategories method returns the matching item when called with Id and valid name.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnActivityCategoryWhenGetActivityCategoriesIsCalledWithValidIdAndValidName()
        {
            var expected = CreateActivityCategoryObject();
            ActivityCategory.ActivityCategoryCreateOrUpdate(expected);

            // Since we don't have the ID yet, first get by name and then use that Id to call get by Id.
            var actual = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = 0, InName = expected.InName });

            var actualById = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = actual[0].Id, InName = expected.InName });  // Get by valid Id and valid name.
            Assert.IsNotNull(actualById);
            Assert.IsNotNull(actualById);
            Assert.IsTrue(actualById.Count == 1);
            Assert.AreEqual(expected.InName, actualById[0].Name);

            // Cleanup
            ActivityCategoryTestDataAccessUtility.DeleteActivityCategory(actual[0].Id);
        }

        [Description("Verifies whether GetActivityCategories method returns all items when called with invalid Id and invalid Name.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnAllItemsWhenGetActivityCategoriesIsCalledWithInvalidIdAndInvalidName()
        {
            var initialList = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = -1, InName = Guid.NewGuid().ToString() }); // Invalid id, name.            
            Assert.IsNotNull(initialList);
            Assert.IsTrue(initialList.Count > 0);

            var newCategory = CreateActivityCategoryObject();
            ActivityCategory.ActivityCategoryCreateOrUpdate(newCategory);

            var finalList = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = -1, InName = Guid.NewGuid().ToString() }); // Invalid id, name.            
            Assert.IsNotNull(finalList);
            Assert.AreEqual(initialList.Count + 1, finalList.Count);

            // Cleanup
            var actualId = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = 0, InName = newCategory.InName })[0].Id;
            ActivityCategoryTestDataAccessUtility.DeleteActivityCategory(actualId);
        }

        [Description("Verifies whether GetActivityCategories method returns all items when called with invalid Id and valid Name.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnAllItemsWhenGetActivityCategoriesIsCalledWithInvalidIdAndValidName()
        {
            var initialList = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = -1, InName = Guid.NewGuid().ToString() }); // Invalid id, invalid name.            
            Assert.IsNotNull(initialList);
            Assert.IsTrue(initialList.Count > 0);

            var newCategory = CreateActivityCategoryObject();
            ActivityCategory.ActivityCategoryCreateOrUpdate(newCategory);

            var finalList = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = -1, InName = newCategory.InName }); // Invalid id, valid name.            
            Assert.IsNotNull(finalList);
            Assert.AreEqual(initialList.Count + 1, finalList.Count);

            // Cleanup
            var actualId = ActivityCategoryRepositoryService.GetActivityCategories(new ActivityCategoryByNameGetRequestDC { InId = 0, InName = newCategory.InName })[0].Id;
            ActivityCategoryTestDataAccessUtility.DeleteActivityCategory(actualId);
        }

        private static ActivityCategoryCreateOrUpdateRequestDC CreateActivityCategoryObject()
        {
            return new ActivityCategoryCreateOrUpdateRequestDC
            {
                InGuid = Guid.NewGuid(),
                InName = Guid.NewGuid().ToString(),
                InDescription = Guid.NewGuid().ToString(),
                InMetaTags = Guid.NewGuid().ToString(),
                InAuthGroupName = "pqocwfauthors",
                Incaller = Guid.NewGuid().ToString(),
                IncallerVersion = "1.0.0.0",
                InInsertedByUserAlias = "v-sanja",
                InUpdatedByUserAlias = "v-sanja"
            };
        }
    }
}
