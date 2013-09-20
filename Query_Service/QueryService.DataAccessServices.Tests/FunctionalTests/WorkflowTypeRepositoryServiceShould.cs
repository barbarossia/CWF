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
        private const string DefaultEnv = "Dev";

        [Description("Verifies whether GetWorkflowTypes method returns the matching item when called with Name.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnWorkflowTypeByNameWhenGetWorkflowTypesIsCalledWithValidName()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);
            WorkflowTypesGetRequestDC request = new WorkflowTypesGetRequestDC()
                {
                    Id= 0,
                    Name = expected.InName,
                    Environment=DefaultEnv
                };
            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(request); // Valid name.
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.WorkflowActivityType);
            Assert.IsTrue(actual.WorkflowActivityType.Count == 1);
            Assert.AreEqual(expected.InName, actual.WorkflowActivityType[0].Name);
            
            // Cleanup
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actual.WorkflowActivityType[0].Id);
        }

        [Description("Verifies whether GetWorkflowTypes method does not return any items when called with invalid name.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnNoneWhenGetWorkflowTypesIsCalledWithInvalidName()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);
            WorkflowTypesGetRequestDC request = new WorkflowTypesGetRequestDC()
            {
                Id = 0,
                Name = Guid.NewGuid().ToString(),
                Environment=DefaultEnv
            };
            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(request); // Invalid name.
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.WorkflowActivityType);
            Assert.IsTrue(actual.WorkflowActivityType.Count == 0);

            // Cleanup
            request = new WorkflowTypesGetRequestDC()
            {
                Id = 0,
                Name=expected.InName,
                Environment=DefaultEnv
            };
            var actualId = WorkflowTypeRepositoryService.GetWorkflowTypes(request).WorkflowActivityType[0].Id;
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actualId);
        }

        [Description("Verifies whether GetWorkflowTypes method returns the matching item when called with Id.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnWorkflowTypeByIdWhenGetWorkflowTypesIsCalledWithValidId()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);

            // Since we don't have the ID yet, first get by name and then use that Id to call get by Id.
            WorkflowTypesGetRequestDC request = new WorkflowTypesGetRequestDC()
            {
                Id = 0,
                Name = expected.InName,
                Environment=DefaultEnv
            };
            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(request);

            request = new WorkflowTypesGetRequestDC()
            {
                Id = actual.WorkflowActivityType[0].Id,
                Environment=DefaultEnv
            };
            var actualById = WorkflowTypeRepositoryService.GetWorkflowTypes(request);  // Get by valid Id.
            Assert.IsNotNull(actualById);
            Assert.IsNotNull(actualById.WorkflowActivityType);
            Assert.IsTrue(actualById.WorkflowActivityType.Count == 1);
            Assert.AreEqual(expected.InName, actualById.WorkflowActivityType[0].Name);

            // Cleanup
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actual.WorkflowActivityType[0].Id);
        }

        [Description("Verifies whether GetWorkflowTypes method does not return any items when called with invalid Id.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnNoneWhenGetWorkflowTypesIsCalledWithInvalidId()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);
            WorkflowTypesGetRequestDC request = new WorkflowTypesGetRequestDC()
            {
                Id = -1,
                Environment="Dev"
            };
            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(request); // Invalid Id.
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.WorkflowActivityType);
            Assert.IsTrue(actual.WorkflowActivityType.Count == 0);

            // Cleanup
            request = new WorkflowTypesGetRequestDC()
            {
                Id = expected.InId,
                Name = expected.InName,
                Environment="Dev"
            };
            var actualId = WorkflowTypeRepositoryService.GetWorkflowTypes(request).WorkflowActivityType[0].Id;
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actualId);
        }

        [Description("Verifies whether GetWorkflowTypes method returns the matching item by Id when called with valid Id and invalid name.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnWorkflowTypeWhenGetWorkflowTypesIsCalledWithValidIdAndInvalidName()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);
            
            // Since we don't have the ID yet, first get by name and then use that Id to call get by Id.
            WorkflowTypesGetRequestDC request = new WorkflowTypesGetRequestDC()
            {
                Id = 0,
                Name = expected.InName,
                Environment=DefaultEnv
            };
            var actualTemp = WorkflowTypeRepositoryService.GetWorkflowTypes(request);

            request = new WorkflowTypesGetRequestDC()
            {
                Id = actualTemp.WorkflowActivityType[0].Id,
                Name = Guid.NewGuid().ToString(),
                Environment=DefaultEnv
            };
            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(request);  // Valid Id and invalid Name.
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.WorkflowActivityType);
            Assert.IsTrue(actual.WorkflowActivityType.Count == 1);
            Assert.AreEqual(expected.InName, actual.WorkflowActivityType[0].Name);

            // Cleanup
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actual.WorkflowActivityType[0].Id);
        }

        [Description("Verifies whether GetWorkflowTypes method does not return any items when called with invalid Id and valid name.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnNoneWhenGetWorkflowTypesIsCalledWithInvalidIdAndValidName()
        {
            WorkFlowTypeCreateOrUpdateRequestDC expected = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(expected);
            WorkflowTypesGetRequestDC request = new WorkflowTypesGetRequestDC()
            {
                Id = -1,
                Name = expected.InName,
                Environment="Dev"
            };
            var actual = WorkflowTypeRepositoryService.GetWorkflowTypes(request); // Invalid Id and valid Name.
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.WorkflowActivityType);
            Assert.IsTrue(actual.WorkflowActivityType.Count == 0);

            // Cleanup
            request = new WorkflowTypesGetRequestDC()
            {
                Id = 0,
                Name = expected.InName,
                Environment="Dev"
            };
            var actualId = WorkflowTypeRepositoryService.GetWorkflowTypes(request).WorkflowActivityType[0].Id;
            WorkflowTypeTestDataAccessUtility.DeleteWorkflowType(actualId);
        }

        [Description("Verifies whether GetWorkflowTypes method returns all the items when called without name or ID.")]
        [Owner("v-sanja")]
        [TestCategory("Func")]
        [TestMethod]
        public void ReturnAllWorkflowTypesWhenGetWorkflowTypesIsCalledWithoutIdOrName()
        {
            WorkflowTypesGetRequestDC request = new WorkflowTypesGetRequestDC()
            {
                Environment = "Dev"
            };
            var initialList = WorkflowTypeRepositoryService.GetWorkflowTypes(request);
            Assert.IsTrue(initialList.WorkflowActivityType.Count > 0);

            WorkFlowTypeCreateOrUpdateRequestDC newType = CreateWorkflowTypeObject();
            WorkflowTypeRepositoryService.WorkflowTypeCreateOrUpdate(newType);

            var finalList = WorkflowTypeRepositoryService.GetWorkflowTypes(request);
            Assert.AreEqual(initialList.WorkflowActivityType.Count + 1, finalList.WorkflowActivityType.Count);
            
            // Cleanup
            WorkflowTypesGetRequestDC newrequest = new WorkflowTypesGetRequestDC()
            {
                Id = 0,
                Name = newType.InName,
                Environment=DefaultEnv
            };
            int newTypeId = WorkflowTypeRepositoryService.GetWorkflowTypes(newrequest).WorkflowActivityType[0].Id;
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
                InUpdatedByUserAlias = Environment.UserName,
                InAuthGroupNames = new string[] { "pqocwfadmin" },
                Environment="Dev"
            };
        }
    }
}
