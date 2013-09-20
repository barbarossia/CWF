using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AuthoringToolTests.Services;
using CWF.DataContracts;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using Microsoft.Support.Workflow.Service.Contracts.FaultContracts;

namespace Authoring.Tests.Functional
{
    [TestClass]
    public class ActivityCategoryGetExceptionHandlingFunctionalTest
    {

        private int NULL_INCALLERNAME_ERROR_CODE = 52501;
        private int NULL_INCALLERVERSION_ERROR_CODE = 52502;
        private int NULL_REQUEST_ERROR_CODE = 52500;
        private string VERY_LARGE_INCALLERNAME_ERROR_MESSAGE = "An error occurred.  Please contact workflow marketplace service system administrator for assistance.";
        private int VERY_LARGE_INCALLERNAME_ERROR_CODE = 53000;
        private string INCALLERNAME = "v-beriva";
        private string INCALLERVERSION = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private string NULL_REQUEST_ERROR_MESSAGE = "Request object reference is not set to an instance of an object.";
        private string NULL_INCALLERVERSION_ERROR_MESSAGE = "Caller version is required.";
        private string NULL_INCALLERNAME_ERROR_MESSAGE = "Caller name is required.";
        private string VERY_LARGE_INCALLERNAME = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec quis urna arcu. Mauris diam nisi, aliquet posuere consectetur sed, rutrum sit amet libero. Sed non nunc bibendum ligula aliquet tempus quis in sapien. Aliquam ante magna, fringilla sed sollicitudin eu, mollis non nunc. Quisque sit amet augue est, sit amet fringilla sem. Donec auctor dapibus elementum. Nunc tellus quam, convallis nec scelerisque nec, eleifend nec ante. Quisque tristique porta metus sed vestibulum. Pellentesque et lobortis urna. Sed pharetra erat a metus ullamcorper eleifend.Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Nunc ut purus non dolor porttitor hendrerit in in tellus. Suspendisse potenti. Integer est tortor, ornare a imperdiet eget, porttitor nec mi. Aenean justo velit, hendrerit et sollicitudin vel, vulputate at urna. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed varius nisl augue. In eu est quis massa tristique consectetur sit amet mollis diam. Fusce porttitor dignissim tellus, sit amet accumsan sapien pellentesque tincidunt. Curabitur porta consectetur odio, nec elementum dolor consequat in. Aliquam et metus non arcu congue sollicitudin.Fusce dictum, lorem sit amet facilisis euismod, eros diam pretium quam, eu posuere arcu sapien eu orci. Maecenas sed nunc massa. Vivamus facilisis justo id turpis ultricies scelerisque. Curabitur mi nisi, volutpat et dignissim vel, tempor egestas lorem. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Morbi ut justo sit amet leo vehicula euismod sit amet id eros. Proin laoreet iaculis faucibus. Nunc ac diam nisl. Morbi commodo mauris vitae metus laoreet porta id in arcu.Sed tincidunt velit vitae justo aliquet faucibus. Vestibulum non rhoncus risus. Curabitur porttitor adipiscing lorem at sollicitudin. In bibendum elit non magna faucibus eleifend. Vestibulum ut velit magna. Morbi a dolor sed nibh tempor dictum consectetur sed eros. Proin molestie est quis diam accumsan consectetur. Maecenas facilisis leo a purus malesuada rutrum. Sed risus nulla, vestibulum id tempus eleifend, aliquam non leo. Donec luctus adipiscing nisi, vitae faucibus ipsum posuere eu. Suspendisse potenti. Etiam nec justo eget justo dictum ornare.";


        [WorkItem(205017)]
        [Description("Exception Handling: Verifies that the proper Validation Exception is thrown when a Null Request is sent")]
        [Owner("v-beriva")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void ExceptionHandling_ActivityCategoryGet_NullRequest()
        {
            try
            {
                WorkflowsQueryServiceUtility.UsingClient(client => client.ActivityCategoryGet(null));
                Assert.Fail("Did not generate an Exception");
            }
            catch (FaultException<ValidationFault> ex)
            {
                Assert.AreEqual(NULL_REQUEST_ERROR_CODE, ex.Detail.ErrorCode,
                    String.Format("Did not get the expected error code. Expected:{0} Actual:{1}", NULL_REQUEST_ERROR_CODE, ex.Detail.ErrorCode));
                Assert.AreEqual(NULL_REQUEST_ERROR_MESSAGE, ex.Detail.ErrorMessage.ToString(),
                    String.Format("Did not get the expected Error Message. Expected:{0} Actual{1}", NULL_REQUEST_ERROR_MESSAGE, ex.Detail.ErrorMessage));
            }
            catch (FaultException<Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault> ex)
            {
                Assert.Fail("Did not get the expected Service Fault from the QS. Error Message: " + ex.Detail.ErrorMessage + " Error Code: " + ex.Detail.ErrorCode);
            }
            catch (Exception ex)
            {
                Assert.Fail("Did not get a ValidationFault from the QS. Error Message: " + ex.Message.ToString());
            }
        }

        [WorkItem(205027)]
        [Description("Exception Handling: Verifies that the proper Validation Exception is thrown when a Null Incaller value is sent")]
        [Owner("v-beriva")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void ExceptionHandling_ActivityCategoryGet_NullIncaller()
        {
            var request = new ActivityCategoryByNameGetRequestDC
               {
                   Incaller = null,
                   IncallerVersion = INCALLERVERSION
               };


            try
            {
                WorkflowsQueryServiceUtility.UsingClient(client => client.ActivityCategoryGet(request));
                Assert.Fail("Did not generate an Exception");
            }
            catch (FaultException<ValidationFault> ex)
            {
                Assert.AreEqual(NULL_INCALLERNAME_ERROR_CODE, ex.Detail.ErrorCode,
                    String.Format("Did not get the expected error code. Expected:{0} Actual:{1}", NULL_INCALLERNAME_ERROR_CODE, ex.Detail.ErrorCode));
                Assert.AreEqual(NULL_INCALLERNAME_ERROR_MESSAGE, ex.Detail.ErrorMessage.ToString(),
                    String.Format("Did not get the expected Error Message. Expected:{0} Actual{1}", NULL_INCALLERNAME_ERROR_MESSAGE, ex.Detail.ErrorMessage));
            }
            catch (FaultException<Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault> ex)
            {
                Assert.Fail("Did not get the expected Service Fault from the QS. Error Message: " + ex.Detail.ErrorMessage + " Error Code: " + ex.Detail.ErrorCode);
            }
            catch (Exception ex)
            {
                Assert.Fail("Did not get a ValidationFault from the QS. Error Message: " + ex.Message.ToString());
            }
        }

        [WorkItem(205028)]
        [Description("Exception Handling: Verifies that the proper Validation Exception is thrown when a Null Incaller Version value is sent")]
        [Owner("v-beriva")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void ExceptionHandling_ActivityCategoryGet_NullInCallerVersion()
        {
            var request = new ActivityCategoryByNameGetRequestDC
            {
                Incaller = INCALLERNAME,
                IncallerVersion = null
            };


            try
            {
                WorkflowsQueryServiceUtility.UsingClient(client => client.ActivityCategoryGet(request));
                Assert.Fail("Did not generate an Exception");
            }
            catch (FaultException<ValidationFault> ex)
            {
                Assert.AreEqual(NULL_INCALLERVERSION_ERROR_CODE, ex.Detail.ErrorCode,
                    String.Format("Did not get the expected error code. Expected:{0} Actual:{1}", NULL_INCALLERVERSION_ERROR_CODE, ex.Detail.ErrorCode));
                Assert.AreEqual(NULL_INCALLERVERSION_ERROR_MESSAGE, ex.Detail.ErrorMessage.ToString(),
                    String.Format("Did not get the expected Error Message. Expected:{0} Actual{1}", NULL_INCALLERVERSION_ERROR_MESSAGE, ex.Detail.ErrorMessage));
            }
            catch (FaultException<Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault> ex)
            {
                Assert.Fail("Did not get the expected Service Fault from the QS. Error Message: " + ex.Detail.ErrorMessage + " Error Code: " + ex.Detail.ErrorCode);
            }
            catch (Exception ex)
            {
                Assert.Fail("Did not get a ValidationFault from the QS. Error Message: " + ex.Message.ToString());
            }
        }

        [WorkItem(205029)]
        [Description("Exception Handling: Verifies that the proper Validation Exception is thrown when a Empty Incaller value is sent")]
        [Owner("v-beriva")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void ExceptionHandling_ActivityCategoryGet_EmptyIncaller()
        {

            var request = new ActivityCategoryByNameGetRequestDC
            {
                Incaller = String.Empty,
                IncallerVersion = INCALLERVERSION
            };


            try
            {
                WorkflowsQueryServiceUtility.UsingClient(client => client.ActivityCategoryGet(request));
                Assert.Fail("Did not generate an Exception");
            }
            catch (FaultException<ValidationFault> ex)
            {
                Assert.AreEqual(NULL_INCALLERNAME_ERROR_CODE, ex.Detail.ErrorCode,
                    String.Format("Did not get the expected error code. Expected:{0} Actual:{1}", NULL_INCALLERNAME_ERROR_CODE, ex.Detail.ErrorCode));
                Assert.AreEqual(NULL_INCALLERNAME_ERROR_MESSAGE, ex.Detail.ErrorMessage.ToString(),
                    String.Format("Did not get the expected Error Message. Expected:{0} Actual{1}", NULL_INCALLERNAME_ERROR_MESSAGE, ex.Detail.ErrorMessage));
            }
            catch (FaultException<Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault> ex)
            {
                Assert.Fail("Did not get the expected Service Fault from the QS. Error Message: " + ex.Detail.ErrorMessage + " Error Code: " + ex.Detail.ErrorCode);
            }
            catch (Exception ex)
            {
                Assert.Fail("Did not get a ValidationFault from the QS. Error Message: " + ex.Message.ToString());
            }
        }

        [WorkItem(205031)]
        [Description("Exception Handling: Verifies that the proper Validation Exception is thrown when a Empty Incaller Version value is sent")]
        [Owner("v-beriva")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void ExceptionHandling_ActivityCategoryGet_EmptyInCallerVersion()
        {



            var request = new ActivityCategoryByNameGetRequestDC
            {
                Incaller = INCALLERNAME,
                IncallerVersion = String.Empty
            };


            try
            {
                WorkflowsQueryServiceUtility.UsingClient(client => client.ActivityCategoryGet(request));
                Assert.Fail("Did not generate an Exception");
            }
            catch (FaultException<ValidationFault> ex)
            {
                Assert.AreEqual(NULL_INCALLERVERSION_ERROR_CODE, ex.Detail.ErrorCode,
                    String.Format("Did not get the expected error code. Expected:{0} Actual:{1}", NULL_INCALLERVERSION_ERROR_CODE, ex.Detail.ErrorCode));
                Assert.AreEqual(NULL_INCALLERVERSION_ERROR_MESSAGE, ex.Detail.ErrorMessage.ToString(),
                    String.Format("Did not get the expected Error Message. Expected:{0} Actual{1}", NULL_INCALLERVERSION_ERROR_MESSAGE, ex.Detail.ErrorMessage));
            }
            catch (FaultException<Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault> ex)
            {
                Assert.Fail("Did not get the expected Service Fault from the QS. Error Message: " + ex.Detail.ErrorMessage + " Error Code: " + ex.Detail.ErrorCode);
            }
            catch (Exception ex)
            {
                Assert.Fail("Did not get a ValidationFault from the QS. Error Message: " + ex.Message.ToString());
            }
        }

        [WorkItem(205032)]
        [Description("Exception Handling: Verifies that the proper Validation Exception is thrown when a Very Large Incaller Version value is sent")]
        [Owner("v-beriva")]
        [TestCategory("Func-NoDif1-Full")]
        [TestMethod()]
        public void ExceptionHandling_ActivityLibrariesGet_VeryLargeIncaller()
        {
            var request = new ActivityCategoryByNameGetRequestDC
            {
                Incaller = VERY_LARGE_INCALLERNAME,
                IncallerVersion = INCALLERVERSION
            };


            try
            {
                WorkflowsQueryServiceUtility.UsingClient(client => client.ActivityCategoryGet(request));

            }
            catch (FaultException<ValidationFault> ex)
            {
                Assert.Fail("Did not get the expected Service Fault from the QS. Error Message: " + ex.Detail.ErrorMessage + " Error Code: " + ex.Detail.ErrorCode);

            }
            catch (FaultException<Microsoft.Support.Workflow.Service.Contracts.FaultContracts.ServiceFault> ex)
            {
                Assert.AreEqual(VERY_LARGE_INCALLERNAME_ERROR_CODE, ex.Detail.ErrorCode,
                    String.Format("Did not get the expected error code. Expected:{0} Actual:{1}", VERY_LARGE_INCALLERNAME_ERROR_CODE, ex.Detail.ErrorCode));
                Assert.AreEqual(VERY_LARGE_INCALLERNAME_ERROR_MESSAGE, ex.Detail.ErrorMessage.ToString(),
                    String.Format("Did not get the expected Error Message. Expected:{0} Actual{1}", VERY_LARGE_INCALLERNAME_ERROR_MESSAGE, ex.Detail.ErrorMessage));
            }
            catch (Exception ex)
            {
                Assert.Fail("Did not get a ValidationFault from the QS. Error Message: " + ex.Message.ToString());
            }
        }
    }
}

