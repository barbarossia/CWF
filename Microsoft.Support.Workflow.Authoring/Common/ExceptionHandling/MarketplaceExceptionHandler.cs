using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.Views;
using System.Net;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.Services;
using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;

namespace Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling
{
    public class MarketplaceExceptionHandler
    {
        public static void HandleSearchException(Exception ex)
        {
            MessageBoxService.Show(
                TextResources.MarketplaceNetworkIssueMsg, TextResources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void HandleDownloadException(Exception ex)
        {
            string details = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
            if (ex.InnerException != null && ex.InnerException is WebException)
            {
                ErrorMessageDialog.Show(TextResources.MarketplaceNetworkIssueMsg
                    , details,
                    Utility.FuncGetCurrentActiveWindow(Application.Current));
            }
            else
            {
                ErrorMessageDialog.Show(TextResources.MarketplaceDownloadFailureMsg
                                  , details,
                                Utility.FuncGetCurrentActiveWindow(Application.Current));
            }
        }

        public static void HandleSaveProjectsException(Exception ex)
        {
            string details = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
            ErrorMessageDialog.Show(TextResources.MarketplaceDownloadFailureMsg
                                  , details,
                                 Utility.FuncGetCurrentActiveWindow(Application.Current));

        }

        public static void HandleCancelDownloadExcption(Exception ex)
        {
            ErrorMessageDialog.Show(TextResources.MarketplaceCancelFailureMsg
                                  , TextResources.MarketplaceCachingMsg,
                                  Utility.FuncGetCurrentActiveWindow(Application.Current));
        }
    }
}
