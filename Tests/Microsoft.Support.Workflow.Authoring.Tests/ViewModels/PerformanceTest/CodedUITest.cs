using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace Microsoft.Support.Workflow.Authoring.Tests.ViewModels.PerformanceTest
{
    /// <summary>
    /// Summary description for CodedUITest
    /// </summary>
    [CodedUITest]
    public class CodedUITest
    {
        public CodedUITest()
        {
        }

        [TestMethod]
        public void OpenMarketPlace()
        {
            this.UIMap.OpenMarketPlace();
        }

        [TestMethod]
        public void CreateWorkflow()
        {
            this.UIMap.CreateWorkflow();
        }

        [TestMethod]
        public void OpenSmallWorkflow()
        {
            ApplicationUnderTest.Launch(@"C:\Users\v-allhe\Desktop\CWF_Doc\R2.5_local\Microsoft.Support.Workflow.Foundry.exe");
            //Assert.IsTrue(this.UIMap.UICommonWorkflowFoundrWindow.WaitForControlExist(6000),"CWF 启动失败!");
            DateTime start = DateTime.Now;
            this.UIMap.UICommonWorkflowFoundrWindow.UIMenuMenu.UIFileMenuItem.UIOpenProjectMenuItem.WaitForControlReady();
            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("CWF startup time is:" + span.TotalSeconds);
            //System.Threading.Thread.Sleep(1000);//wait for 1 seconds
            //Playback.Wait(1000);//wait for 1 seconds
            this.UIMap.OpenSmallWorkflow();
        }

        [TestMethod]
        public void OpenMiddleWorkflow()
        {
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
            this.UIMap.OpenMiddleWorkflow();
        }


        [TestMethod]
        public void OpenLargeWorkflow()
        {
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
            this.UIMap.OpenLargeWorkflow();
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        //}

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
