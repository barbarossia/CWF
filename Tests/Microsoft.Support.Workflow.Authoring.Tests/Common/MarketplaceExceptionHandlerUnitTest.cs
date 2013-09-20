using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.Views;
using Microsoft.DynamicImplementations;
using System.Net;

namespace Microsoft.Support.Workflow.Authoring.Tests.Common
{
    [TestClass]
    public class MarketplaceExceptionHandlerUnitTest
    {
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void MarketplaceException_TestHandleSearchException()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                MessageBoxService.ShowFunc = (msg, title, button, image, result) =>
                {
                    Assert.AreEqual("Network issues have interrupted your downloads from Marketplace.  Please contact your network administrator.", msg);
                    return MessageBoxResult.OK;
                };
                MarketplaceExceptionHandler.HandleSearchException(new Exception("test message"));
            });
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void MarketplaceException_TestHandleDownloadException()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var errorDialog = new ImplementationOfType(typeof(ErrorMessageDialog)))
                {
                    try
                    {
                        var ex = new Exception("test message");
                        errorDialog.Register(() => ErrorMessageDialog.Show(Argument<string>.Any, Argument<string>.Any, Argument<Window>.Any))
                            .Execute((string message, string details, Window w) =>
                            {
                                Assert.AreEqual(ex.Message, details);
                                Assert.AreEqual("Failed to download projects and activities to local machine.", message);
                            });
                        MarketplaceExceptionHandler.HandleDownloadException(ex);

                        ex = new Exception("", new WebException("inner"));
                        errorDialog.Register(() => ErrorMessageDialog.Show(Argument<string>.Any, Argument<string>.Any, Argument<Window>.Any))
                            .Execute((string message, string details, Window w) =>
                            {
                                Assert.AreEqual(ex.InnerException.ToString(), details);
                                Assert.AreEqual("Network issues have interrupted your downloads from Marketplace.  Please contact your network administrator.", message);
                            });
                        MarketplaceExceptionHandler.HandleDownloadException(ex);
                    }
                    catch (Exception ex)
                    {
                        Assert.IsTrue(ex is InvalidOperationException);
                    }
                }
            });
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void MarketplaceException_TestHandleSaveProjectsException()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var errorDialog = new ImplementationOfType(typeof(ErrorMessageDialog)))
                {
                    try
                    {
                        var ex = new Exception("test message");
                        errorDialog.Register(() => ErrorMessageDialog.Show(Argument<string>.Any, Argument<string>.Any, Argument<Window>.Any))
                            .Execute((string message, string details, Window w) =>
                            {
                                Assert.AreEqual(ex.Message, details);
                                Assert.AreEqual("Failed to download projects and activities to local machine.", message);
                            });
                        MarketplaceExceptionHandler.HandleSaveProjectsException(ex);

                        ex = new Exception("", new WebException("inner"));
                        errorDialog.Register(() => ErrorMessageDialog.Show(Argument<string>.Any, Argument<string>.Any, Argument<Window>.Any))
                            .Execute((string message, string details, Window w) =>
                            {
                                Assert.AreEqual(ex.InnerException.ToString(), details);
                                Assert.AreEqual("Failed to download projects and activities to local machine.", message);
                            });
                        MarketplaceExceptionHandler.HandleSaveProjectsException(ex);
                    }
                    catch (Exception ex)
                    {
                        Assert.IsTrue(ex is InvalidOperationException);
                    }
                }
            });
        }

        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void MarketplaceException_TestHandleCancelDownloadExcption()
        {
            TestUtilities.MockUtilityGetCurrentWindowReturnNull(() =>
            {
                using (var errorDialog = new ImplementationOfType(typeof(ErrorMessageDialog)))
                {
                    try
                    {
                        var ex = new Exception();
                        errorDialog.Register(() => ErrorMessageDialog.Show(Argument<string>.Any, Argument<string>.Any, Argument<Window>.Any))
                            .Execute((string message, string details, Window w) =>
                            {
                                Assert.AreEqual("Projects and activities have been downloaded to local machine, system is caching them.", details);
                                Assert.AreEqual("Failed to cancel downloading, please try again.", message);
                            });
                        MarketplaceExceptionHandler.HandleCancelDownloadExcption(ex);
                    }
                    catch (Exception e)
                    {
                        Assert.IsTrue(e is InvalidOperationException);
                    }
                }
            });
        }
    }
}
