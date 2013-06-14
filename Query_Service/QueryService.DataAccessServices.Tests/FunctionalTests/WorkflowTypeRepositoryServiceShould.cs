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
    public class WorkflowTypeRepositoryServiceShould
    {
        [Description("Verifies whether GetWorkflowTypes method returns the matching item when called with Name.")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnWorkflowTypeByNameWhenGetWorkflowTypesIsCalledWithValidName()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);
            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(0, expected.InName); // Valid name.
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.WorkflowActivityType);
            Assert.IsTrue(actual.WorkflowActivityType.Count == 1);
            Assert.AreEqual(expected.InName, actual.WorkflowActivityType[0].Name);
            
            // Cleanup
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actual.WorkflowActivityType[0].Id);
        }

        [Description("Verifies whether GetWorkflowTypes method does not return any items when called with invalid name.")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnNoneWhenGetWorkflowTypesIsCalledWithInvalidName()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);
            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(0, Guid.NewGuid().ToString()); // Invalid name.
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.WorkflowActivityType);
            Assert.IsTrue(actual.WorkflowActivityType.Count == 0);

            // Cleanup
            var actualId = WorkflowTypeRepositoryService.GetWorkflowTypes(0, expected.InName).WorkflowActivityType[0].Id;
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actualId);
        }

        [Description("Verifies whether GetWorkflowTypes method returns the matching item when called with Id.")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnWorkflowTypeByIdWhenGetWorkflowTypesIsCalledWithValidId()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);

            // Since we don't have the ID yet, first get by name and then use that Id to call get by Id.
            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(0, expected.InName);           

            var actualById = WorkflowTypeRepositoryService.GetWorkflowTypes(actual.WorkflowActivityType[0].Id);  // Get by valid Id.
            Assert.IsNotNull(actualById);
            Assert.IsNotNull(actualById.WorkflowActivityType);
            Assert.IsTrue(actualById.WorkflowActivityType.Count == 1);
            Assert.AreEqual(expected.InName, actualById.WorkflowActivityType[0].Name);

            // Cleanup
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actual.WorkflowActivityType[0].Id);
        }

        [Description("Verifies whether GetWorkflowTypes method does not return any items when called with invalid Id.")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnNoneWhenGetWorkflowTypesIsCalledWithInvalidId()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);
            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(-1); // Invalid Id.
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.WorkflowActivityType);
            Assert.IsTrue(actual.WorkflowActivityType.Count == 0);

            // Cleanup
            var actualId = WorkflowTypeRepositoryService.GetWorkflowTypes(0, expected.InName).WorkflowActivityType[0].Id;
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actualId);
        }

        [Description("Verifies whether GetWorkflowTypes method returns the matching item by Id when called with valid Id and invalid name.")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnWorkflowTypeWhenGetWorkflowTypesIsCalledWithValidIdAndInvalidName()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);
            
            // Since we don't have the ID yet, first get by name and then use that Id to call get by Id.
            var actualTemp = WorkflowTypeRepositoryService.GetWorkflowTypes(0, expected.InName);

            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(actualTemp.WorkflowActivityType[0].Id, Guid.NewGuid().ToString());  // Valid Id and invalid Name.
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.WorkflowActivityType);
            Assert.IsTrue(actual.WorkflowActivityType.Count == 1);
            Assert.AreEqual(expected.InName, actual.WorkflowActivityType[0].Name);

            // Cleanup
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actual.WorkflowActivityType[0].Id);
        }

        [Description("Verifies whether GetWorkflowTypes method does not return any items when called with invalid Id and valid name.")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnNoneWhenGetWorkflowTypesIsCalledWithInvalidIdAndValidName()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);
            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(-1, expected.InName); // Invalid Id and valid Name.
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.WorkflowActivityType);
            Assert.IsTrue(actual.WorkflowActivityType.Count == 0);

            // Cleanup
            var actualId = WorkflowTypeRepositoryService.GetWorkflowTypes(0, expected.InName).WorkflowActivityType[0].Id;
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actualId);
        }

        [Description("Verifies whether GetWorkflowTypes method returns all the items when called without name or ID.")]
        [Owner("v-sanja")]
        [TestCategory("Full")]
        [TestMethod]
        public void ReturnAllWorkflowTypesWhenGetWorkflowTypesIsCalledWithoutIdOrName()
        {
            var initialList = WorkflowTypeRepositoryService.GetWorkflowTypes();
            Assert.IsTrue(initialList.WorkflowActivityType.Count > 0);

            WorkFlowTypeCreateOrUpdateRequestDC newType = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(newType);

            var finalList = WorkflowTypeRepositoryService.GetWorkflowTypes();
            Assert.AreEqual(initialList.WorkflowActivityType.Count + 1, finalList.WorkflowActivityType.Count);
            
            // Cleanup
            int newTypeId = WorkflowTypeRepositoryService.GetWorkflowTypes(0, newType.InName).WorkflowActivityType[0].Id;
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(newTypeId);
        }

        private static WorkFlowTypeCreateOrUpdateRequestDC CreateWorkflowTypeObject()
        {
            return new WorkFlowTypeCreateOrUpdateRequestDC 
            { 
                InGuid = Guid.NewGuid(), 
                InName = Guid.NewGuid().ToString(), 
                InAuthGroupId = 2,
                Incaller = Guid.NewGuid().ToString(),
                IncallerVersion = "1.0.0.0",
                InInsertedByUserAlias = Environment.UserName,
                InUpdatedByUserAlias = Environment.UserName
            };
        }
    }
}
