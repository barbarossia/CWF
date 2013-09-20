using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Tests;
using Microsoft.Support.Workflow.Authoring.UIControls;
using System.Windows;
using System.Threading;
using Microsoft.DynamicImplementations;

namespace Microsoft.Support.Workflow.Authoring.Tests.UIControl
{
    [TestClass]
    public class SearchTextBoxUnitTest
    {
        [WorkItem(322352)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void SearchTextBox_VerifyPropertyChanged() 
        {
            SearchTextBox searchTextBox = new SearchTextBox();
            searchTextBox.LabelText = "search";
            Assert.AreEqual(searchTextBox.LabelText,"search");

            searchTextBox.LabelTextColor = null;
            Assert.AreEqual(searchTextBox.LabelTextColor, null);

            searchTextBox.SearchMode = SearchMode.Delayed;
            Assert.AreEqual(searchTextBox.SearchMode, SearchMode.Delayed);

            searchTextBox.HasText = true;
            Assert.IsTrue(searchTextBox.HasText);

            Duration duration = new Duration(new TimeSpan(0,0,1));
            searchTextBox.SearchEventTimeDelay = duration;
            Assert.AreEqual(searchTextBox.SearchEventTimeDelay, duration);

            searchTextBox.IsMouseLeftButtonDown = true;
            Assert.AreEqual(searchTextBox.IsMouseLeftButtonDown, true);

        }

        [WorkItem(322357)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [TestMethod]
        public void SearchTextBox_VerifySearchExecuteWhenTextChanged() 
        {
            
            SearchTextBox searchTextBox = new SearchTextBox();
            bool isRaised = false;
            searchTextBox.Search += (s, e) =>
            {
                isRaised = true;
            };
            searchTextBox.SearchEventTimeDelay = new Duration(new TimeSpan(0, 0, 0, 0, 1));
            searchTextBox.SearchMode = SearchMode.Instant;
            searchTextBox.Text = "search";
            searchTextBox.HasText = true;
            searchTextBox.OnApplyTemplate();
            searchTextBox.OnSeachEventDelayTimerTick(null, null);
            Assert.IsTrue(isRaised);
        }
    }
}
