using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.Views;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Windows.Threading;
using System.Threading.Tasks;
using Microsoft.Support.Workflow.Authoring.Security;
using System.Security.Principal;
using System.Windows;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.Common.Messages;
using Microsoft.Support.Workflow.Authoring.AddIns.Converters;
using Microsoft.Support.Workflow.Authoring.AddIns.Logger;
using System.Collections.ObjectModel;

namespace Microsoft.Support.Workflow.Authoring
{
    public class Startup
    {
        private static App app = new App();
        private static SplashScreenView splashScreen = new SplashScreenView();
        private static ILogWriter logger = new EventLogWriter();
        /// <summary>
        /// So we can handle errors differently during application startup
        /// </summary>
        public static bool IsInitialized { get; set; }

        [STAThread]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            app.DispatcherUnhandledException += UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            ApplicationStartup(null, null);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.WriteEvent(EventIdValues.UNHANDELED_EXCEPTION, e.ToString());
        }

        private static void InitializationFinished()
        {
            var window = new MainWindow();
            var viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;
            app.MainWindow = window;
            app.MainWindow.Show();
            // From this point on, if there is an unhandled exception it won't leave a phantom app running
            // because MainWindow has been shown, so we can change our error handling to not shut down the app.
            splashScreen.Close();
            // Work around WPF limitation: there is no event to tell you when frame at StartUri has loaded successfully
            // (Application.LoadCompleted does not do what you think it does) by Manually creating the MainWindow during
            // startup.
            IsInitialized = true;
        }

        /// <summary>
        /// The event handler for current app domain's AssemblyResolve event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The event args.
        /// </param>
        /// <returns>
        /// The resolved assembly.
        /// </returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Utility.Resolve(args.Name);
        }

        private static void UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs unhandledExceptionEvent)
        {
            // In the long-run, only UserFacingExceptions should be surfaced here.
            var e = unhandledExceptionEvent.Exception;
            // For UserFacingException, show Message and InnerException in details.
            // For anything else, show a generic message and the actual exception in details. 
            // If this happens it always means there's a bug in the tool.
            string msg = (e is UserFacingException) ? e.Message : "An unexpected error occurred. You have found a bug.";
            string details = (e is UserFacingException)
                ? e.InnerException.IfNotNull(inner => inner.ToString())
                : e.ToString();
            logger.WriteEvent(EventIdValues.HANDELED_EXCEPTION, e.ToString());
            ErrorMessageDialog.Show(msg, details, owner: IsInitialized ? Utility.FuncGetCurrentActiveWindow(Application.Current) : null);

            if (!IsInitialized) // exception during startup should kill app instead of leaving "phantom" app running
            {
                // Mark the application configuration with the flag to cause the Assemblies references to be cleared 
                // out when the application is started next
                Utility.SetDeleteAssemblyFlagInConfiguration(deleteFlag: true);

                Application.Current.Shutdown(); // will close the WpfBugWindow automatically
            }
            unhandledExceptionEvent.Handled = true;
        }

        private static void ApplicationStartup(object sender, StartupEventArgs e)
        {
            //Check User Level to grant/deny access to app
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            Task.Factory.StartNew(() => { var m = AuthorizationService.EnvPermissionMaps; });
            app.Resources.Source = new Uri("pack://application:,,,/Resources/MergedDictionary.Xaml");
            splashScreen.Show();
            Task.Factory.StartNew(() =>
            {
                // Clean temp directory
                FileService.ClearTempDirectory();
                // Clean the compile output directory
                FileService.ClearOutputDirectory();
                Utility.CheckForDeleteCache();
                Caching.LoadFromLocal();
                app.Dispatcher.BeginInvoke(new Action(InitializationFinished), DispatcherPriority.Background);
                ObservableCollection<string> categories = AssetStore.AssetStoreProxy.Categories;
                if (categories != null && (string.IsNullOrWhiteSpace(DefaultValueSettings.DefaultCategory) || !categories.Contains(DefaultValueSettings.DefaultCategory)))
                {
                    DefaultValueSettings.SetConfigValue(DefaultValueSettings.DefaultCategoryKey, categories.FirstOrDefault());
                    DefaultValueSettings.RefreshConfigValues();
                }

            });
            app.Run();
        }

    }
}
