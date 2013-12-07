using AuthoringToolTests.Services;
using Microsoft.Support.Workflow.Authoring.CDS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Microsoft.Support.Workflow.Authoring.Tests.CDS
{
    [TestClass]
    public class CWFFileSystemUnitTest
    {
        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void ConstructorThrowsArgumentExceptionIfRootIsNull()
        {
            // Act and Assert
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                new CWFFileSystem(null));

        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void ConstructorThrowsArgumentExceptionIfRootIsTheEmptyString()
        {
            // Act and Assert
            TestUtilities.Assert_ShouldThrow<ArgumentException>(() =>
                new CWFFileSystem(string.Empty));
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void ConstructorInitializesInstance()
        {
            // Arrange
            var root = @"X:\MyRoot\MyDir";

            // Act
            var target = new CWFFileSystem(root);

            // Assert
            Assert.AreEqual(root, target.Root);
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void GetFullPathCombinesRootAndSpecifiedPath()
        {
            // Arrange
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var path = Path.GetRandomFileName();
            var target = new CWFFileSystem(root);

            // Act
            string result = target.GetFullPath(path);

            // Assert
            Assert.AreEqual(Path.Combine(root, path), result);
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void AddFileThrowsArgumentNullExceptionIfStreamIsNull()
        {
            // Arrange
            var root = Path.GetRandomFileName();
            var target = new CWFFileSystem(root);

            // Act and Assert
            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() =>
                target.AddFile(Path.GetRandomFileName(), stream: null));
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void AddFileThrowsArgumentNullExceptionIfWriteToStreamIsNull()
        {
            // Arrange
            var root = Path.GetRandomFileName();
            var target = new CWFFileSystem(root);

            // Act and Assert
            TestUtilities.Assert_ShouldThrow<ArgumentNullException>(() =>
                target.AddFile(Path.GetRandomFileName(), writeToStream: null));
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void GetFullPathReturnsRootIfPathIsNullOrEmpty()
        {
            // Arrange
            string path = string.Empty;
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var target = new CWFFileSystem(root);

            // Act
            var fullPath = target.GetFullPath(path);

            // Assert
            Assert.AreEqual(root, fullPath);
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void GetTempPath()
        {
            // Arrange
            string path = string.Empty;
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var target = new CWFFileSystem(root);

            // Act
            var fullPath = target.GetTempPath(path);
        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void AddFile()
        {
            // Arrange
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var target = new CWFFileSystem(root);

            var lib1 = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1;
            string path = target.GetFullPath(@"Assemblies");
            string fileName = Path.GetFileName(lib1.Location);
            EnsureDirectory(path);
            string destPath = Path.Combine(path, fileName);
            using (new CachingIsolator())
            {
                target.AddFile(destPath, File.OpenRead(lib1.Location));
            }

        }

        [TestMethod]
        [Owner("v-bobzh")]
        [TestCategory("Unit-NoDif")]
        public void AddFileToStream()
        {
            // Arrange
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var target = new CWFFileSystem(root);

            var lib1 = TestInputs.TestInputs.ActivityAssemblyItems.TestInput_Lib1;
            string path = target.GetFullPath(@"Assemblies");
            string fileName = Path.GetFileName(lib1.Location);
            EnsureDirectory(path);
            string destPath = Path.Combine(path, fileName);

            using (new CachingIsolator())
            {
                target.AddFile(destPath, targetStream => File.OpenRead(lib1.Location).CopyTo(targetStream));
            }

        }

        private void EnsureDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

    }
}
