using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.UIControls;

namespace Microsoft.Support.Workflow.Authoring.Tests.UIControl
{
    [TestClass]
    public class DataPagingUnitTest
    {
        /// <summary>
        /// datapaging control need to be tested
        /// </summary>
        private DataPaging dp;
        [WorkItem(322318)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [Description("Verify the datapaging buttons IsEnabled property according PageNumber")]
        [TestMethod]
        public void DataPaging_VerifyButtonsIsEnabledProperty()
        {
            dp = new DataPaging();
            dp.PageCount = 5;
            dp.PageNumber = 1;
            Assert.IsFalse(dp.IsPreviousEnable,
               string.Format("DataPaging's previous button's IsEnabled is not set to false when PageNumber is 1."));

            dp.PageNumber = 5;
            Assert.IsFalse(dp.IsNextEnable, string.Format("DataPaging's previous button's IsEnabled is not set to false when PageNumber is equal to PageCount."));

            dp.PageNumber = 2;
            Assert.IsTrue((dp.IsNextEnable && dp.IsPreviousEnable), "DataPaging's previous and next button's IsEnabled is not set to True when PageNumber is between 1 to PageCount.");
        }

        [WorkItem(322317)]
        [Owner("v-kason")]
        [TestCategory("Unit-NoDif")]
        [Description("Verify the datapaging AvailablePages")]
        [TestMethod]
        public void DataPaging_VerifyAvailablePageWhenButtonClick()
        {
            string msgNext = "DataPaging's AvailablePages is incorrect when next button click, Expected:'{0}',Actual:'{1}'";
            string msgPrevious = "DataPaging's AvailablePages is incorrect when previous button click, Expected:'{0}',Actual:'{1}'";

            List<int> expectedList = new List<int>();
            string expectedPages = string.Empty;
            string actualPages = string.Empty;


            dp = new DataPaging();
            dp.PageCount = 7;

            //next page button click
            dp.PageNumber = 5;
            dp.GotoNextPage(dp, null);

            expectedList.Add(6);
            expectedList.Add(7);

            expectedList.ForEach(i =>
            {
                expectedPages += i.ToString() + ",";
            });

            dp.AvailablePages.ForEach(i =>
            {
                actualPages += i.ToString() + ",";
            });

            CollectionAssert.AreEqual(expectedList, dp.AvailablePages, string.Format(msgNext, expectedPages, actualPages));

            //previous page button click
            dp.PageCount = 7;
            dp.PageNumber = 6;
            dp.GotoPreviousPage(dp, null);
            
            expectedList.Clear();
            expectedPages = string.Empty;
            actualPages = string.Empty;

            expectedList.AddRange(new List<int> { 1, 2, 3, 4, 5 });
            expectedList.ForEach(i =>
            {
                expectedPages += i.ToString() + ",";
            });

            dp.AvailablePages.ForEach(i =>
            {
                actualPages += i.ToString() + ",";
            });
            CollectionAssert.AreEqual(expectedList, dp.AvailablePages, string.Format(msgPrevious, expectedPages, actualPages));
        }
    }
}
