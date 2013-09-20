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
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Support.Workflow.Authoring.Common.Converters;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using System.Security;
using System.Windows.Threading;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class FileServiceUnitTest
    {
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void FileService_TestSaveImageToDisk()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp.jpg");
            BitmapSource bitmap = BitmapFrame.Create(new Uri("http://localhost")) as BitmapSource;

            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() =>
            {
                FileService.SaveImageToDisk(null, bitmap);
            });

            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() =>
            {
                FileService.SaveImageToDisk(path, null);
            });

            Assert.IsTrue(FileService.SaveImageToDisk(path, bitmap));
            Assert.IsTrue(File.Exists(path));
            File.Delete(path);
            using (var frame = new ImplementationOfType(typeof(BitmapFrame)))
            {
                frame.Register(() => BitmapFrame.Create(Argument<BitmapSource>.Any)).Throw(new IOException());
                Assert.IsFalse(FileService.SaveImageToDisk(path, bitmap));

                frame.Register(() => BitmapFrame.Create(Argument<BitmapSource>.Any)).Throw(new SecurityException());
                Assert.IsFalse(FileService.SaveImageToDisk(path, bitmap));

                frame.Register(() => BitmapFrame.Create(Argument<BitmapSource>.Any)).Throw(new Exception());
                TestUtilities.Assert_ShouldThrow<Exception>(() =>
                {
                    FileService.SaveImageToDisk(path, bitmap);
                });
            }
        }
        
        [WorkItem(322323)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod()]
        public void FileService_VerifyCleanOutputDirectory()
        { 
            using (var mock = new ImplementationOfType(typeof(Directory)))
            {
                int deleteCount = 0;
                mock.Register(() => Directory.GetDirectories(Argument<string>.Any)).Execute(() =>
                {
                    return new string[] { "1", "2", "3" };
                });
                using (var service = new ImplementationOfType(typeof(FileService)))
                {
                    service.Register(() => FileService.DeleteDirectory(Argument<string>.Any)).Execute(() =>
                    {
                        deleteCount++;
                    });
                    FileService.ClearOutputDirectory();
                    Assert.AreEqual(deleteCount, 3);
                }
            }
        }

        [WorkItem(322324)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod()]
        public void FileService_VerifyCleanTempDirectory()
        {
            bool isClear = false;
            using (var service = new ImplementationOfType(typeof(FileService)))
            {
                service.Register(() => FileService.ClearDirectory(Argument<string>.Any)).Execute(() =>
                {
                    isClear = true;
                });
                FileService.ClearTempDirectory();
                Assert.IsTrue(isClear);
            }
        }

        [WorkItem(322336)]
        [Owner("v-kason")]
        [TestCategory("Unit-Dif")]
        [TestMethod()]
        public void FileService_VerifyDeleteDirectory()
        {
            string testDirToDelete = @".\testdir";
            using (var mock = new ImplementationOfType(typeof(Directory)))
            {
                int deleteCount = 0;
                mock.Register(() => Directory.GetDirectories(Argument<string>.Any)).Execute(() =>
                {
                    deleteCount++;
                    return new string[] {};
                });

                bool isDelete = false;
                mock.Register(() => Directory.Delete(Argument<string>.Any)).Execute(() =>
                {
                    isDelete = true;
                });

                bool isClear = false;
                using (var service = new ImplementationOfType(typeof(FileService)))
                {
                    service.Register(() => FileService.ClearDirectory(Argument<string>.Any)).Execute(() =>
                    {
                        isClear = true;
                    });
                    FileService.DeleteDirectory(testDirToDelete);
                    Assert.AreEqual(deleteCount, 1);
                    Assert.IsTrue(isDelete);
                    Assert.IsTrue(isClear);
                }
            }
        }

        [WorkItem(322325)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod()]
        public void FileService_VerifyClearDirectory()
        {
            using (var dir = new ImplementationOfType(typeof(Directory)))
            {
                int deleteCount = 0;
                dir.Register(() => Directory.GetFiles(Argument<string>.Any)).Execute(() =>
                {
                    return new string[] { "1", "2", "3" };
                });
                using (var file = new ImplementationOfType(typeof(File)))
                {
                    file.Register(() => File.Exists(Argument<string>.Any)).Return(true);
                    file.Register(() => File.Delete(Argument<string>.Any)).Execute(() =>
                    {
                        deleteCount++;
                    });
                    FileService.ClearDirectory("");
                    Assert.AreEqual(deleteCount, 3);
                }
            }
        }

    }
}
