// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentSelectorViewModelUnitTest.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Tests
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Microsoft.Support.Workflow.Authoring.Models;
    using Microsoft.Support.Workflow.Authoring.ViewModels;
    using System.Windows.Data;
    using Microsoft.Support.Workflow.Authoring.Services;
    using System.Collections;
    using Microsoft.DynamicImplementations;

    #endregion References

    #region Enumerations
    /// <summary>
    /// Enumeration for whether to use Key or Value to Search
    /// </summary>
    internal enum ContentItemFilter
    {
        /// <summary>
        /// Use the Key in the KeyValuePair 
        /// </summary>
        Key,
        /// <summary>
        /// Use the Value in the KeyValuePair
        /// </summary>
        Value
    } 
    #endregion Enumerations

    /// <summary>
    /// Unit tests for the ContentSelectorViewModel class
    ///</summary>
    [TestClass()]
    public class ContentSelectorViewModelUnitTest
    {
        #region Fields and constants
        /// <summary>
        /// Dictionary of key value pairs to use for testing
        /// </summary>
        private Dictionary<string, string> testDictionary1 = new Dictionary<string, string>() { { "foo", "1" }, { "bar", "1" }, { "foobar", "2" }, { "foobarfoo", "2" } };
        private Dictionary<string, string> testDictionary2 = new Dictionary<string, string>() { { "foo1", "11" }, { "bar1", "11" }, { "foobar1", "21" }, { "foobarfoo1", "21" } };
        private Dictionary<string, string> testDictionary3 = new Dictionary<string, string>() { { "foo2", "12" }, { "bar2", "12" }, { "foobar2", "21" }, { "foobarfoo2", "21" } };

        /// <summary>
        /// String for not_found
        /// </summary>
        private const string NOT_FOUND = "not_found";
        /// <summary>
        /// String for search key 
        /// </summary>
        private const string SEARCH_KEY = "foo";
        /// <summary>
        /// String for search value
        /// </summary>
        private const string SEARCH_VALUE = "2";

        /// <summary>
        /// List of content items
        /// </summary>
        private ObservableCollection<ContentFileItem> contentItemList = null;

        #endregion Fields and constants

        [WorkItem(32325763)]
        [TestCategory("Unit-Dif")]
        [Owner("v-kason1")]
        [TestMethod()]
        public void ContentSelector_VerifyPropertyChanged() 
        {
            using (var manager = new ImplementationOfType(typeof(ContentManager)))
            {
                var list = this.IntializeTestList(true);
                manager.Register(() => ContentManager.GetContentFileItems()).Execute(() =>
                {
                    return list;
                });

                using (var viewModel = new Implementation<ContentSelectorViewModel>())
                {
                    bool isInvoked = false;
                    viewModel.Register(inst => inst.RefreshContentItems()).Execute(() =>
                    {
                        isInvoked = true;
                    });

                    ContentSelectorViewModel vm = viewModel.Instance;
                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedContentFileItem", () => vm.SelectedContentFileItem = list[0]);
                    Assert.AreEqual(vm.SelectedContentFileItem, list[0]);

                    TestUtilities.Assert_ShouldRaiseINPCNotification(vm, "SelectedGroup", () => vm.SelectedGroup = null);
                    Assert.AreEqual(vm.SelectedGroup, null);

                    //verify ContentFileItem_PropertyChanged
                    isInvoked = false;
                    vm.SelectedContentFileItem.FileName = "changed";
                    Assert.IsFalse(isInvoked);
                }
            }
        }

        #region Test Methods

        #region Tests for found Keys and Values
        /// <summary>
        ///A test for RefreshContentItems where the items are filtered by key
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        [Description("Unit test for the RefreshContentItems call when the content items are filtered by key")]
        [Owner("v-mimcin")]
        [TestCategory("Unit-NoDif")]
        public void ContentSelector_RefreshContentItemsFilteredByKeyTest()
        {
            ContentSelectorViewModel_Accessor target = new ContentSelectorViewModel_Accessor();
            // Create a Test List
            target.ContentFileItems = IntializeTestList(true);
            target.ContentItems = ContentManager.GetContentItems(target.ContentFileItems);
            target.ItemsView = new CollectionViewSource();
            target.ItemsView.Source = target.ContentItems;           
            // Search for items by key
            target.SearchFilter = SEARCH_KEY;
            // Refresh the items
            target.RefreshContentItems();
            // ASSERT
            Assert.AreEqual(GetContentItemCountByFieldUsingContains(ContentItemFilter.Key, SEARCH_KEY), target.ItemsView.View.Cast<ContentItem>().Count());
        }

        /// <summary>
        ///A test for RefreshContentItems
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        [Description("Unit test for the RefreshContentItems call when the content items are filtered by value")]
        [Owner("v-mimcin")]
        [TestCategory("Unit-NoDif")]
        public void ContentSelector_RefreshContentItemsFilteredByValueTest()
        {
            ContentSelectorViewModel_Accessor target = new ContentSelectorViewModel_Accessor();
            // Create a Test List
            target.ContentFileItems = IntializeTestList(true);
            target.ContentItems = ContentManager.GetContentItems(target.ContentFileItems);
            target.ItemsView = new CollectionViewSource();
            target.ItemsView.Source = target.ContentItems; 
            // Search for Items by Value
            target.SearchFilter = SEARCH_VALUE;
            target.RefreshContentItems();
            // ASSERT
            Assert.AreEqual(GetContentItemCountByFieldUsingContains(ContentItemFilter.Value, SEARCH_VALUE), target.ItemsView.View.Cast<ContentItem>().Count());
        }

        #endregion Tests for found Keys and Values

        #region Tests for no items selected
        /// <summary>
        ///A test for RefreshContentItems where the items are filtered by key but there are no items selected
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        [Description("Unit test for the RefreshContentItems call when the content items are filtered by key but there are no content items selected")]
        [Owner("v-mimcin")]
        [TestCategory("Unit-NoDif")]
        public void ContentSelector_RefreshContentItemsFilteredByKeyWhenContentItemsNotSelectedTest()
        {
            ContentSelectorViewModel_Accessor target = new ContentSelectorViewModel_Accessor();
            // Create a Test List that is empty
            target.ContentFileItems = IntializeTestList(false);
            target.ItemsView = new CollectionViewSource();
            target.ItemsView.Source = target.ContentItems; 
            // Search for items by key
            target.SearchFilter = SEARCH_KEY;
            // Refresh the items
            target.RefreshContentItems();
            // Assert that the count is 0 items
            Assert.AreEqual(0, target.ItemsView.View.Cast<ContentItem>().Count());
        }

        /// <summary>
        ///A test for RefreshContentItems where the items are filtered by value but there are no items selected
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        [Description("Unit test for the RefreshContentItems call when the content items are filtered by value but there are no content items selected")]
        [Owner("v-mimcin")]
        [TestCategory("Unit-NoDif")]
        public void ContentSelector_RefreshContentItemsFilteredByValueWhenContentItemsNotSelectedTest()
        {
            ContentSelectorViewModel_Accessor target = new ContentSelectorViewModel_Accessor();
            // Create a Test List that is empty
            target.ContentFileItems = IntializeTestList(false);
            target.ItemsView = new CollectionViewSource();
            target.ItemsView.Source = target.ContentItems; 
            // Search for items by key
            target.SearchFilter = SEARCH_VALUE;
            // Refresh the items
            target.RefreshContentItems();
            // Assert that the count is 0 items           
            Assert.AreEqual(0, target.ItemsView.View.Cast<ContentItem>().Count());
        }

        #endregion Tests for no items selected

        #region Tests for Keys and Values not found in selected list
        /// <summary>
        ///A test for RefreshContentItems where the items are filtered by key
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        [Description("Unit test for the RefreshContentItems call when the content items are filtered by key but the key is not found")]
        [Owner("v-mimcin")]
        [TestCategory("Unit-NoDif")]
        public void ContentSelector_RefreshContentItemsFilteredByKeyWhereKeyNotFoundTest()
        {
            ContentSelectorViewModel_Accessor target = new ContentSelectorViewModel_Accessor();
            // Create a Test List
            target.ContentFileItems = IntializeTestList(true);
            target.ItemsView = new CollectionViewSource();
            target.ItemsView.Source = target.ContentItems; 
            // Search for items by key
            target.SearchFilter = NOT_FOUND;
            // Refresh the items
            target.RefreshContentItems();
            // ASSERT
            Assert.AreEqual(GetContentItemCountByFieldUsingContains(ContentItemFilter.Key, NOT_FOUND), target.ItemsView.View.Cast<ContentItem>().Count());
        }

        /// <summary>
        ///A test for RefreshContentItems
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Microsoft.Support.Workflow.Authoring.exe")]
        [Description("Unit test for the RefreshContentItems call when the content items are filtered by value but the value is not found")]
        [Owner("v-mimcin")]
        [TestCategory("Unit-NoDif")]
        public void ContentSelector_RefreshContentItemsFilteredByValueWhereValueNotFoundTest()
        {
            ContentSelectorViewModel_Accessor target = new ContentSelectorViewModel_Accessor();
            // Create a Test List
            target.ContentFileItems = IntializeTestList(true);
            target.ItemsView = new CollectionViewSource();
            target.ItemsView.Source = target.ContentItems;
            // Search for Items by Value
            target.SearchFilter = NOT_FOUND;
            target.RefreshContentItems();
            // ASSERT            
            Assert.AreEqual(GetContentItemCountByFieldUsingContains(ContentItemFilter.Value, NOT_FOUND), target.ItemsView.View.Cast<ContentItem>().Count());
        }

        #endregion Tests for Keys and Values not found in selected list

        #endregion Test Methods

        #region Private helpers

        /// <summary>
        /// Uses Linq to return the number of ContentItems qualify the search criteria
        /// </summary>
        /// <param name="filterToUse">The type of filter to use</param>
        /// <param name="searchValue">String to search for</param>
        /// <returns>Count of items that meet criteria based on Contains syntax</returns>
        private int GetContentItemCountByFieldUsingContains(ContentItemFilter filterToUse, string searchValue)
        {
            int resultCount = 0;
            // Concat all three sequences
            IEnumerable<KeyValuePair<string, string>> testDictionary = testDictionary1.Concat(testDictionary2).Concat(testDictionary3);

            switch(filterToUse)
            {
                case ContentItemFilter.Key:
                    resultCount += testDictionary.Count(td => td.Key.Contains(searchValue));
                    break;

                case ContentItemFilter.Value:
                    resultCount += testDictionary.Count(td => td.Value.Contains(searchValue));
                    break;
            }

            return resultCount;
        }

        /// <summary>
        /// Creates a Contents XDocument that has the specified keyvalue pairs
        /// </summary>
        /// <param name="testDictionary">IDictionary</param>
        /// <returns>XDocument</returns>
        private XDocument CreateDocumentWithTestKeysAndValues(IDictionary<string, string> testDictionary)
        {
            // <Contents name='UI'>
            //  <Section>
            //      <Key name="format" value="{0},{1}"/>
            //  </Section>
            // </Contents>
            // Create the XML Document
            XDocument targetDocument = new XDocument();
            // Create the top level Contents with UI as the name
            XElement uiElement = new XElement("configuration");
            uiElement.SetAttributeValue("name", "UI");
            XElement section = new XElement("section");

            uiElement.Add(section);

            foreach (string currentKey in testDictionary.Keys)
            {
                XElement keyValuePair = new XElement("key");
                keyValuePair.SetAttributeValue("name", currentKey);
                keyValuePair.SetAttributeValue("value", testDictionary[currentKey]);
                section.Add(keyValuePair);
            }

            targetDocument.Add(uiElement);

            return targetDocument;
        }

        /// <summary>
        /// Initialize the content that we are using
        /// </summary>
        /// <param name="selectItems">If true then all the content items are selected otherwise none of them are selected</param>
        private ObservableCollection<ContentFileItem> IntializeTestList(bool selectItems)
        {
            contentItemList = new ObservableCollection<ContentFileItem>();

            // Add known items to the list with different Keys and Values
            ContentFileItem firstItem = new ContentFileItem();
            ContentFileItem secondItem = new ContentFileItem();
            ContentFileItem thirdItem = new ContentFileItem();
            // Create the Content and select or deselect all of them
            firstItem.Content = CreateDocumentWithTestKeysAndValues(testDictionary1); 
           
            secondItem.Content = CreateDocumentWithTestKeysAndValues(testDictionary2);            
            thirdItem.Content = CreateDocumentWithTestKeysAndValues(testDictionary3);            
            contentItemList.Add(firstItem);
            contentItemList.Add(secondItem);
            contentItemList.Add(thirdItem);
            return contentItemList;
        }

        #endregion Private helpers

    }
}
