using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.Views;
using System.Net;
using System.Windows;
using Microsoft.Support.Workflow.Authoring.Services;

namespace Microsoft.Support.Workflow.Authoring.Common.ExceptionHandling
{
    public class MarketplaceExceptionHandler
    {
        public static void HandleSearchException(Exception ex)
        {
            MessageBoxService.Show(
                "Network issues have interrupted your downloads from Marketplace.  Please contact your network administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void HandleDownloadException(Exception ex)
        {
            string details = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
            if (ex.InnerException != null && ex.InnerException is WebException)
            {
                ErrorMessageDialog.Show("Network issues have interrupted your downloads from Marketplace.  Please contact your network administrator."
                    , details,
                    Utility.FuncGetCurrentActiveWindow(Application.Current));
            }
            else
            {
                ErrorMessageDialog.Show("Failed to download projects and activities to local machine."
                                  , details,
                                Utility.FuncGetCurrentActiveWindow(Application.Current));
            }
        }

        public static void HandleSaveProjectsException(Exception ex)
        {
            string details = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message;
            ErrorMessageDialog.Show("Failed to download projects and activities to local machine."
                                  , details,
                                 Utility.FuncGetCurrentActiveWindow(Application.Current));

        }

        public static void HandleCancelDownloadExcption(Exception ex)
        {
            ErrorMessageDialog.Show("Failed to cancel downloading, please try again."
                                  , "Projects and activities have been downloaded to local machine, system is caching them.",
                                  Utility.FuncGetCurrentActiveWindow(Application.Current));
        }
    }
}
