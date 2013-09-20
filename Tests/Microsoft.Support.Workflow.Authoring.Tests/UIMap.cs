namespace Microsoft.Support.Workflow.Authoring.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Input;
    using System.CodeDom.Compiler;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = System.Windows.Forms.MouseButtons;


    public partial class UIMap
    {
        /// <summary>
        /// RecordedMethod1 - Use 'RecordedMethod1Params' to pass parameters into this method.
        /// </summary>
        public void OpenSmallWorkflow()
        {
            #region Variable Declarations
            WpfMenuItem uIOpenProjectMenuItem = this.UICommonWorkflowFoundrWindow.UIMenuMenu.UIFileMenuItem.UIOpenProjectMenuItem;
            WpfEdit uIItemEdit = this.UIOpenProjectfromServeWindow.UIItemEdit;
            WpfButton uISearchButton = this.UIOpenProjectfromServeWindow.UISearchButton;
            WpfButton uIOpenButton = this.UIOpenProjectfromServeWindow.UIOpenButton;
            WpfButton uICloseTabButtonButton = this.UICommonWorkflowFoundrWindow.UIRadDockingCustom.UITabWorkflowTabList.UISmallTabPage.UICloseTabButtonButton;
            WpfButton uIOpenEditButton = this.UICommonWorkflowFoundrWindow1.UIOpenForEditingButton;
            WpfButton uIYesButton = this.UICommonWorkflowFoundrWindow1.UIYesButton;
            #endregion

            // Click 'File' -> 'Open Project...' menu item
            Mouse.Click(uIOpenProjectMenuItem, new Point(47, 9));

            // Type 'small' in 'Unknown Name' text box
            uIItemEdit.Text = "small";

            // Click 'Search' button
            Mouse.Click(uISearchButton, new Point(25, 7));

            // Last action on Row was not recorded because the control does not have any good identification property.

            // Click 'Open' button
            Mouse.Click(uIOpenButton, new Point(35, 13));


            DateTime start = DateTime.Now;
            // Click 'Yes' button
            Mouse.Click(uIOpenEditButton, new Point(36, 9));
            Keyboard.SendKeys("{ENTER}");
            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("Open the small workflow time is:" + span.TotalSeconds);

            // Click 'CloseTabButton' button
            Mouse.Click(uICloseTabButtonButton, new Point(2, 7));
            Mouse.Click(uIYesButton, new Point(36, 9));
        }

        public void OpenMiddleWorkflow()
        {
            #region Variable Declarations
            WpfMenuItem uIOpenProjectMenuItem = this.UICommonWorkflowFoundrWindow.UIMenuMenu.UIFileMenuItem.UIOpenProjectMenuItem;
            WpfEdit uIItemEdit = this.UIOpenProjectfromServeWindow.UIItemEdit;
            WpfButton uISearchButton = this.UIOpenProjectfromServeWindow.UISearchButton;
            WpfButton uIOpenButton = this.UIOpenProjectfromServeWindow.UIOpenButton;
            WpfButton uICloseTabButtonButton = this.UICommonWorkflowFoundrWindow.UIRadDockingCustom.UITabWorkflowTabList.UIMiddleTabPage.UICloseTabButtonButton;
            WpfButton uIOpenEditButton = this.UICommonWorkflowFoundrWindow1.UIOpenForEditingButton;
            WpfButton uIYesButton = this.UICommonWorkflowFoundrWindow1.UIYesButton;
            #endregion

            // Click 'File' -> 'Open Project...' menu item
            Mouse.Click(uIOpenProjectMenuItem, new Point(47, 9));

            // Type 'small' in 'Unknown Name' text box
            uIItemEdit.Text = "Middle";

            // Click 'Search' button
            Mouse.Click(uISearchButton, new Point(25, 7));

            // Last action on Row was not recorded because the control does not have any good identification property.

            // Click 'Open' button
            Mouse.Click(uIOpenButton, new Point(35, 13));


            DateTime start = DateTime.Now;
            // Click 'Yes' button
            Mouse.Click(uIOpenEditButton, new Point(36, 9));
            Keyboard.SendKeys("{ENTER}");
            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("Open the middle workflow time is:" + span.TotalSeconds);

            // Click 'CloseTabButton' button
            Mouse.Click(uICloseTabButtonButton, new Point(2, 7));
            Mouse.Click(uIYesButton, new Point(36, 9));
        }

        public void OpenLargeWorkflow()
        {
            #region Variable Declarations
            WpfMenuItem uIOpenProjectMenuItem = this.UICommonWorkflowFoundrWindow.UIMenuMenu.UIFileMenuItem.UIOpenProjectMenuItem;
            WpfEdit uIItemEdit = this.UIOpenProjectfromServeWindow.UIItemEdit;
            WpfButton uISearchButton = this.UIOpenProjectfromServeWindow.UISearchButton;
            WpfButton uIOpenButton = this.UIOpenProjectfromServeWindow.UIOpenButton;
            WpfButton uICloseTabButtonButton = this.UICommonWorkflowFoundrWindow.UIRadDockingCustom.UITabWorkflowTabList.UILargeTabPage.UICloseTabButtonButton;
            WpfButton uIOpenEditButton = this.UICommonWorkflowFoundrWindow1.UIOpenForEditingButton;
            WpfButton uIYesButton = this.UICommonWorkflowFoundrWindow1.UIYesButton;
            #endregion

            // Click 'File' -> 'Open Project...' menu item
            Mouse.Click(uIOpenProjectMenuItem, new Point(47, 9));

            // Type 'small' in 'Unknown Name' text box
            uIItemEdit.Text = "Large";

            // Click 'Search' button
            Mouse.Click(uISearchButton, new Point(25, 7));

            // Last action on Row was not recorded because the control does not have any good identification property.

            // Click 'Open' button
            Mouse.Click(uIOpenButton, new Point(35, 13));


            DateTime start = DateTime.Now;
            // Click 'Yes' button
            Mouse.Click(uIOpenEditButton, new Point(36, 9));
            Keyboard.SendKeys("{ENTER}");
            DateTime end = DateTime.Now;
            TimeSpan span = end - start;
            Console.WriteLine("Open the large workflow time is:" + span.TotalSeconds);

            // Click 'CloseTabButton' button
            Mouse.Click(uICloseTabButtonButton, new Point(2, 7));
            Mouse.Click(uIYesButton, new Point(36, 9));
        }

        /// <summary>
        /// RecordedMethod1 - Use 'RecordedMethod1Params' to pass parameters into this method.
        /// </summary>
        public void OpenMarketPlace()
        {
            #region Variable Declarations
            WpfMenuItem uIBrowsetheMarketplaceMenuItem = this.UICommonWorkflowFoundrWindow.UIMenuMenu.UIMarketplaceMenuItem.UIBrowsetheMarketplaceMenuItem;
            WpfProgressBar uILoadingmarketplaceasProgressBar = this.UIMarketplaceHomeWindow.UILoadingmarketplaceasProgressBar;
            WpfEdit uITxtSearchEdit = this.UIMarketplaceHomeWindow.UITxtSearchEdit;
            #endregion

            // Click 'Marketplace' -> 'Browse the Marketplace' menu item
            Mouse.Click(uIBrowsetheMarketplaceMenuItem, new Point(75, 4));

            // Click 'Loading marketplace assets...' progress bar
            Mouse.Click(uILoadingmarketplaceasProgressBar, new Point(836, 61));

            // Type 'small' in 'txtSearch' text box
            //uITxtSearchEdit.Text = this.RecordedMethod1Params.UITxtSearchEditText;
            uITxtSearchEdit.SetFocus();
            Keyboard.SendKeys("small");
            // Click 'Loading marketplace assets...' progress bar
            Mouse.Click(uILoadingmarketplaceasProgressBar, new Point(488, 181));

            // Click 'Loading marketplace assets...' progress bar
            Mouse.Click(uILoadingmarketplaceasProgressBar, new Point(874, 547));
        }

        /// <summary>
        /// CreateWorkflow - Use 'CreateWorkflowParams' to pass parameters into this method.
        /// </summary>
        public void CreateWorkflow()
        {
            #region Variable Declarations
            WpfMenuItem uINewProjectMenuItem = this.UICommonWorkflowFoundrWindow.UIMenuMenu.UIFileMenuItem.UINewProjectMenuItem;
            WpfProgressBar uIBusyIndicatorProgressBar = this.UINewProjectWindow.UIBusyIndicatorProgressBar;
            WpfEdit uIEnterthenameoftheworEdit = this.UINewProjectWindow.UIEnterthenameoftheworEdit;
            WpfTreeItem uISystemActivitiesStatTreeItem = this.UICommonWorkflowFoundrWindow.UIItemCustom.UIPART_ToolsTree.UIBasicLogicTreeItem.UISystemActivitiesStatTreeItem;
            WpfText uIDropactivityhereText = this.UICommonWorkflowFoundrWindow.UIActivityBuilderCustom.UIDropactivityhereCustom.UIDropactivityhereText;
            WpfTreeItem uISystemActivitiesStatTreeItem1 = this.UICommonWorkflowFoundrWindow.UIItemCustom.UIPART_ToolsTree.UIBasicLogicTreeItem.UISystemActivitiesStatTreeItem1;
            WpfCustom uISacdVerticalConnectoCustom = this.UICommonWorkflowFoundrWindow.UISequenceCustom.UIDropactivityhereCustom.UISacdVerticalConnectoCustom;
            WpfButton uICloseTabButtonButton = this.UICommonWorkflowFoundrWindow.UIRadDockingCustom.UITabWorkflowTabList.UITest0228001TabPage.UICloseTabButtonButton;
            WpfButton uIDontSaveButton = this.UICommonWorkflowFoundrWindow1.UIDontSaveButton;
            #endregion

            // Click 'File' -> 'New Project...' menu item
            Mouse.Click(uINewProjectMenuItem, new Point(52, 16));

            // Click 'busyIndicator' progress bar
            Mouse.Click(uIBusyIndicatorProgressBar, new Point(109, 315));

            // Click 'busyIndicator' progress bar
            Mouse.Click(uIBusyIndicatorProgressBar, new Point(161, 341));

            // Type 'test0228_001' in 'Enter the name of the workflow you wish to create' text box
            //uIEnterthenameoftheworEdit.Text = this.CreateWorkflowParams.UIEnterthenameoftheworEditText;
            uIEnterthenameoftheworEdit.SetFocus();
            Keyboard.SendKeys("test0228_001");

            // Click 'busyIndicator' progress bar
            Mouse.Click(uIBusyIndicatorProgressBar, new Point(264, 405));

            // Move 'Basic Logic' -> 'System.Activities.Statements.Sequence' tree item from (52, 14) to 'Drop activity here' label (48, 8)
            uIDropactivityhereText.EnsureClickable(new Point(48, 8));
            Mouse.StartDragging(uISystemActivitiesStatTreeItem, new Point(52, 14));
            Mouse.StopDragging(uIDropactivityhereText, new Point(48, 8));

            // Move 'Basic Logic' -> 'System.Activities.Statements.WriteLine' tree item from (36, 8) to 'sacd:VerticalConnector_1' custom control (71, 46)
            uISacdVerticalConnectoCustom.EnsureClickable(new Point(71, 46));
            Mouse.StartDragging(uISystemActivitiesStatTreeItem1, new Point(36, 8));
            Mouse.StopDragging(uISacdVerticalConnectoCustom, new Point(71, 46));

            // Click 'CloseTabButton' button
            Mouse.Click(uICloseTabButtonButton, new Point(9, 9));

            // Click 'Don't Save' button
            Mouse.Click(uIDontSaveButton, new Point(58, 11));
        }

    }
}
