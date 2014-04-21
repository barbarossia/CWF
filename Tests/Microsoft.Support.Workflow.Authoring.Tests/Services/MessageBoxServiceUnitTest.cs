using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Views;
using Microsoft.Support.Workflow.Authoring.Common;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class MessageBoxServiceUnitTest
    {
        [WorkItem(323370)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void MessageBox_TestBasicShowMethods()
        {
            MessageBoxImage i;
            MessageBoxService.ShowFunc = (msg, caption, button, image, defaultResult) =>
            {
                i = image;
                return MessageBoxResult.OK;
            };

            MessageBoxService.ShowInfo("");
            i = MessageBoxImage.Information;

            MessageBoxService.ShowError("");
            i = MessageBoxImage.Error;
        }

        [WorkItem(323370)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void MessageBox_TestOpenSaveMessages()
        {
            string message = null;
            MessageBoxService.ShowFunc = (msg, caption, button, image, defaultResult) =>
            {
                message = msg;
                return MessageBoxResult.OK;
            };
            MessageBoxService.ShowOpenActivityConfirmationFunc = (msg, caption) =>
            {
                message = msg;
                return MessageBoxResult.OK;
            };
 
            MessageBoxService.CannotSaveDuplicatedNameWorkflow("activity1");
            Assert.AreEqual("The name \"activity1\" already exists in server, please use another one.", message);

            MessageBoxService.CreateNewActivityOnSaving();
            Assert.AreEqual("A newer version of this project exists in the Marketplace.  Would you like to create a newer version?", message);

            MessageBoxService.DownloadNewActivityOnSaving();
            Assert.AreEqual("A new version of this project exists in the Marketplace, you are not allowed to save your changes to the server. It is suggested that you save your work locally. Do you want to download the newer version?", message);
       }

        [WorkItem(323370)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void MessageBox_TestLockMessages()
        {
            string message = null;
            MessageBoxService.ShowFunc = (msg, caption, button, image, defaultResult) =>
            {
                message = msg;
                return MessageBoxResult.OK;
            };

            MessageBoxService.LockChangedWhenAdminUnlocking("lockedBy");
            Assert.AreEqual("The workflow was locked by lockedBy. Would you like to unlock it?", message);

            MessageBoxService.LockChangedWhenAuthorUnlocking("lockedBy");
            Assert.AreEqual("The workflow was locked by administrator lockedBy. You cannot unlock it now.", message);

            MessageBoxService.OpenLockedActivityByNonAdmin("lockedBy");
            Assert.AreEqual("This workflow was locked by lockedBy. You will open it in read-only mode.", message);
 
            MessageBoxService.CannotSaveLockedActivity();
            Assert.AreEqual("This project was locked by an administrator.  You are not allowed to save your changes to the server.", message);
       }

        [WorkItem(323370)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void MessageBox_TestMarketplaceMessages()
        {
            string message = null;
            MessageBoxService.ShowFunc = (msg, caption, button, image, defaultResult) =>
            {
                message = msg;
                return MessageBoxResult.OK;
            };

            MessageBoxService.ShoudExitWithMarketplaceOpened();
            Assert.AreEqual("Do you want to close while the Marketplace View is open?", message);

            MessageBoxService.ShoudExitWithMarketplaceDownloading();
            Assert.AreEqual("Do you want to close while the Marketplace is processing download?", message);

            MessageBoxService.NotifyUploadResult("message", true);
            Assert.AreEqual("message", message);

            MessageBoxService.NotifyUploadResult("message", false);
            Assert.AreEqual("message", message);
        }

        [WorkItem(323370)]
        [TestMethod]
        [Owner("v-ery")]
        [TestCategory("Unit-NoDif")]
        public void MessageBox_TestSavingConfirmation()
        {
            string message = null;
            MessageBoxService.ShowSavingConfirmationFunc = (msg, caption, canKeepLock, shouldUnlock, visible) =>
            {
                message = msg;
                return null;
            };

            MessageBoxService.ShowUnlockConfirmation("activity1");
            Assert.AreEqual("Do you want to save changes to activity1 before unlock? The workflow will be read-only after unlocked.", message);

            MessageBoxService.ShowLocalSavingConfirmation("activity1");
            Assert.AreEqual("Do you want to save changes to activity1 before closing?", message);

            MessageBoxService.ShowKeepLockedConfirmation("activity1");
            Assert.AreEqual("activity1 is locked by yourself, do you want to keep the lock?", message);
        }
    }
}
