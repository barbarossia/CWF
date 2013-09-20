using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Collections.ObjectModel;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.DynamicImplementations;
using System.IO;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class ContentManagerUnitTest
    {
        [WorkItem(323624)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void ContentManager_TestGetContentFileItems()
        {
            ObservableCollection<ContentFileItem> items;
            using (var utility = new ImplementationOfType(typeof(Utility)))
            {
                utility.Register(() => Utility.GetContentDirectoryPath()).Return("Content\\BrokenContentDirectory.xml");
                items = ContentManager.GetContentFileItems();
                Assert.AreEqual(0, items.Count);

                utility.Register(() => Utility.GetContentDirectoryPath()).Return("Content\\ContentDirectory.xml");
                items = ContentManager.GetContentFileItems();
                Assert.AreEqual(3, items.Count);
            }
        }

        [WorkItem(323624)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-Dif")]
        public void ContentManager_TestGetContentItems()
        {
            using (var utility = new ImplementationOfType(typeof(Utility)))
            {
                utility.Register(() => Utility.GetContentDirectoryPath()).Return("Content\\ContentDirectory.xml");
                ObservableCollection<ContentFileItem> fileItems = ContentManager.GetContentFileItems();
                ObservableCollection<ContentItem> items = ContentManager.GetContentItems(fileItems);
                Assert.AreEqual(840, items.Count);
            }
        }
    }
}
