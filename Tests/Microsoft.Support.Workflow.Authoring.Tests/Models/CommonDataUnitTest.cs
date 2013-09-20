using System.Activities.Statements;
using System.IO;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Activities.Presentation.Services;
using Microsoft.DynamicImplementations;
using CWF.DataContracts;
using System.Collections.Generic;

namespace Microsoft.Support.Workflow.Authoring.Tests.Models
{
    [TestClass]
    public class CommonDataUnitTest
    {
        [WorkItem(322330)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason")]
        [TestMethod]
        public void CommonData_VerifyCommonData()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
               {
                   using (var impClient = new Implementation<WorkflowsQueryServiceClient>())
                   {
                       impClient.Register(inst => inst.WorkflowTypeGet(Argument<WorkflowTypesGetRequestDC>.Any)).Execute(() =>
                       {
                           var reply = new WorkflowTypeGetReplyDC();
                           reply.WorkflowActivityType = new List<WorkflowTypesGetBase>() 
                            {
                                new WorkflowTypesGetBase(){Name="MetaData"},
                                new WorkflowTypesGetBase(){Name="Page"},
                                new WorkflowTypesGetBase(){Name="Workflow"},
                            };
                           return reply;
                       });
                       impClient.Register(inst => inst.ActivityCategoryGet(Argument<ActivityCategoryByNameGetRequestDC>.Any)).Execute(() =>
                       {

                           var reply = new List<ActivityCategoryByNameGetReplyDC>();
                           reply.Add(new ActivityCategoryByNameGetReplyDC { Name = "Admin" });
                           return reply;
                       });
                       impClient.Register(inst => inst.StatusCodeGet(Argument<StatusCodeGetRequestDC>.Any)).Execute(() =>
                       {
                           var reply = new StatusCodeGetReplyDC();
                           reply.List = new List<StatusCodeAttributes>()
                    { 
                        new StatusCodeAttributes(){Name = "Public"},
                        new StatusCodeAttributes(){Name = "Private"},
                    };
                           return reply;
                       });
                       WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => impClient.Instance;
                       var cd = CommonData.Instance;
                       cd.Initialize();
                       Assert.AreEqual(cd.IsInitialized, true);
                       Assert.AreEqual(cd.StatusCodes.Count, 2);
                       Assert.AreEqual(cd.WorkflowTypes.Count, 3);
                       Assert.AreEqual(cd.ActivityCategories.Count, 1);
                       TestUtilities.ResetWorkflowsQueryServiceClientAfterMock();
                   }
               });
        }
    }
}
