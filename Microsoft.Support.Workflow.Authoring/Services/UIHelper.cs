using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Microsoft.Support.Workflow.Authoring.Services
{
    public static class UIHelper
    {
        public static bool IsInTesting { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        internal static void AwaitRunInBackground(Action action)
        {
            if (IsInTesting)
            {
                action();
            }
            else
            {
                DispatcherFrame frame = new DispatcherFrame();
                Task.Factory.StartNew(() =>
                {
                    action();
                    frame.Continue = false;
                });
                Dispatcher.PushFrame(frame);
            }
        }
    }
}
