using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.ServiceModel;
using System.Text;
using AuthoringToolTests.Services;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests.Services;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Tests;
using System.Threading.Tasks;
using Microsoft.Support.Workflow.Authoring;
using System.Configuration;
using System.Activities.Statements;
using System.Runtime.Serialization;
using CWF.DataContracts;
using System.Activities.Presentation;

namespace Authoring.Tests.Unit
{
    [TestClass]
    public class UtilityUnitTests
    {
        [WorkItem(322322)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod()]
        public void Utility_VerifyCheckForDeleteCache()
        {
            using (var mock = new ImplementationOfType(typeof(Utility)))
            {
                bool isDeleted = false;
                mock.Register(() => Utility.SetDeleteAssemblyFlagInConfiguration(Argument<bool>.Any)).Execute(() =>
                {
                    isDeleted = true;
                });
                Utility.CheckForDeleteCache();
                Assert.IsTrue(isDeleted);
            }
        }

        [WorkItem(322340)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod()]
        public void Utility_VerifyGetDeleteAssemblyFlagFromConfiguration()
        {
            bool result = false;
            bool configValue = true;

            string config = ConfigurationManager.AppSettings["DeleteCacheOnStartup"];
            if (!string.IsNullOrEmpty(config))
                configValue = config == "1" ? true : false;
            result = Utility.GetDeleteAssemblyFlagFromConfiguration();
            Assert.AreEqual(result, configValue);
        }

        [WorkItem(322363)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod()]
        public void Utility_VerifySetDeleteAssemblyFlagInConfiguration()
        {
            Utility.SetDeleteAssemblyFlagInConfiguration(true);
            Assert.IsTrue(Utility.GetDeleteAssemblyFlagFromConfiguration());
        }



        [Description("Test that UsingClient() aborts iff an exception is thrown, otherwise closes")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod()]
        public void Utility_UsingClient_AbortsOrClosesAppropriately()
        {
            using (var mock = new Mock<ICommunicationObject>())
            {
                mock.Expect(inst => inst.Abort()).Return();
                mock.Expect(inst => inst.Close()).Return();
                TestUtilities.Assert_ShouldThrow<InvalidOperationException>(() =>
                    WorkflowsQueryServiceUtility.UsingClient(mock.Instance, client => { throw new InvalidOperationException(); }));
                WorkflowsQueryServiceUtility.UsingClient(mock.Instance, client => { });
            }
        }

        [Description("Test that UsingClient/Return abstraction doesn't leak (exceptions and return values appear synchronous)")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod()]
        public void Utility_UsingClient_TransparentlyHandlesReturnValuesAndExceptions()
        {
            WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => new WorkflowsQueryServiceClient();
            TestUtilities.Assert_ShouldThrow<InvalidOperationException>(() =>
                WorkflowsQueryServiceUtility.UsingClient(client => { throw new InvalidOperationException(); }));
            Assert.AreEqual(608, WorkflowsQueryServiceUtility.UsingClientReturn(client => 608));
        }

        [Description("Test that Task.ResultOrException() unwraps iff only one exception exists")]
        [Owner("v-maxw")]
        [TestCategory("Unit-NoDif")]
        [TestMethod()]
        public void Utility_ResultOrException_UnwrapsSingleExceptions()
        {
            TestUtilities.Assert_ShouldThrow<InvalidOperationException>(() =>
                Task.Factory.StartNew<int>(() => { throw new InvalidOperationException(); }).ResultOrException());
            var exn = TestUtilities.Assert_ShouldThrow<AggregateException>(() =>
                Task.Factory.StartNew<int>(() =>
                {
                    Task.Factory.StartNew<int>(() => { throw new InvalidOperationException(); }, TaskCreationOptions.AttachedToParent);
                    Task.Factory.StartNew<int>(() => { throw new InvalidCastException(); }, TaskCreationOptions.AttachedToParent);
                    return 0;
                }).ResultOrException());
            Assert.AreEqual(2, exn.InnerExceptions.Count);

            Assert.IsTrue(exn.InnerExceptions.Any(e => e.InnerException is InvalidCastException));
            Assert.IsTrue(exn.InnerExceptions.Any(e => e.InnerException is InvalidOperationException));

            Assert.AreEqual(608, Task.Factory.StartNew<int>(() => 608).ResultOrException());
        }

        [Description("Test that WithContactServerUI converts (only) WCF exceptions to user exceptions appropriately")]
        [Owner("v-maxw")]
        [TestCategory("Unit-Dif")]
        [TestMethod()]
        public void Utility_WithContactServerUI_ConvertsTimeoutsAndCommunicationExceptionsToUserException()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
               {
                   TestUtilities.Assert_ShouldThrow<InvalidOperationException>(() =>
                       Utility.WithContactServerUI(() => { throw new InvalidOperationException(); }));
                   Assert.AreEqual("The server is not available right now.",
                       TestUtilities.Assert_ShouldThrow<UserFacingException>(() =>
                           Utility.WithContactServerUI(() => { throw new CommunicationException(); }))
                       .Message);
                   Assert.AreEqual("The server is not available right now (request timed out).",
                       TestUtilities.Assert_ShouldThrow<UserFacingException>(() =>
                           Utility.WithContactServerUI(() => { throw new TimeoutException(); }))
                       .Message);
                   Assert.AreEqual(608, Utility.WithContactServerUI(() => 608));
               });
        }
    }
}
