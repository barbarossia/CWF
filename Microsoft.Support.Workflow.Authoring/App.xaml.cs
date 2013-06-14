// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.Common.Messages;
using Microsoft.Support.Workflow.Authoring.Security;
namespace Microsoft.Support.Workflow.Authoring
{
    using System;
    using System.Reflection;
    using System.Security.Principal;
    using System.Windows;
    using Services;
    using ViewModels;
    using Views;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public App()
        //{
        //    AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        //    splashScreen = new SplashScreenView();
        //    DispatcherUnhandledException += UnhandledException;
        //    Startup += ApplicationStartup;
        //}

        //private readonly SplashScreenView splashScreen;

        //private void InitializationFinished()
        //{
        //    var window = new MainWindow();
        //    var viewModel = new MainWindowViewModel();
        //    window.DataContext = viewModel;
        //    Current.MainWindow = window;
        //    window.Show();

        //    // From this point on, if there is an unhandled exception it won't leave a phantom app running
        //    // because MainWindow has been shown, so we can change our error handling to not shut down the app.
        //    splashScreen.Close();
        //    // Work around WPF limitation: there is no event to tell you when frame at StartUri has loaded successfully
        //    // (Application.LoadCompleted does not do what you think it does) by Manually creating the MainWindow during
        //    // startup.

        //    IsInitialized = true;
        //}

        ///// <summary>
        ///// The event handler for current app domain's AssemblyResolve event.
        ///// </summary>
        ///// <param name="sender">
        ///// The sender.
        ///// </param>
        ///// <param name="args">
        ///// The event args.
        ///// </param>
        ///// <returns>
        ///// The resolved assembly.
        ///// </returns>
        //private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    return Utility.Resolve(args.Name);
        //}

        //private void UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs unhandledExceptionEvent)
        //{
        //    // In the long-run, only UserFacingExceptions should be surfaced here.
        //    var e = unhandledExceptionEvent.Exception;
        //    // For UserFacingException, show Message and InnerException in details.
        //    // For anything else, show a generic message and the actual exception in details. 
        //    // If this happens it always means there's a bug in the tool.
        //    string msg = (e is UserFacingException) ? e.Message : "An unexpected error occurred. You have found a bug.";
        //    string details = (e is UserFacingException)
        //        ? e.InnerException.IfNotNull(inner => inner.ToString())
        //        : e.ToString();
        //    ErrorMessageDialog.Show(msg, details, owner: IsInitialized ? Utility.FuncGetCurrentActiveWindow(Application.Current) : null);

        //    if (!IsInitialized) // exception during startup should kill app instead of leaving "phantom" app running
        //    {
        //        // Mark the application configuration with the flag to cause the Assemblies references to be cleared 
        //        // out when the application is started next
        //        Utility.SetDeleteAssemblyFlagInConfiguration(deleteFlag: true);

        //        Application.Current.Shutdown(); // will close the WpfBugWindow automatically
        //    }
        //    unhandledExceptionEvent.Handled = true;
        //}

        //private void ApplicationStartup(object sender, StartupEventArgs e)
        //{
        //    //Check User Level to grant/deny access to app
        //    AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

        //    var currentPrincipal = AuthorizationService.CurrentPrincipalFunc() as WindowsPrincipal;
        //    switch (AuthorizationService.GetSecurityLevel(currentPrincipal))
        //    {
        //        case SecurityLevel.Author:
        //            break;
        //        case SecurityLevel.Administrator:
        //            break;
        //        case SecurityLevel.Offline:
        //            MessageBoxService.Show(AuthorizationMessages.Offline,
        //                Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK,
        //                MessageBoxImage.Warning);
        //            Current.Shutdown();
        //            return;
        //        default:
        //            MessageBoxService.Show(AuthorizationMessages.Unauthorized, Assembly.GetExecutingAssembly().GetName().Name, MessageBoxButton.OK,
        //                MessageBoxImage.Warning);
        //            Current.Shutdown();
        //            return;
        //    }

        //    splashScreen.Show();
        //    Task.Factory.StartNew(() =>
        //    {
        //        // Clean temp directory
        //        FileService.ClearTempDirectory();
        //        // Clean the compile output directory
        //        FileService.ClearOutputDirectory();

        //        Utility.CheckForDeleteCache();

        //        Caching.LoadFromLocal();

        //        Dispatcher.BeginInvoke(new Action(InitializationFinished), DispatcherPriority.Background);
        //    });
        //}

        ///// <summary>
        ///// So we can handle errors differently during application startup
        ///// </summary>
        //public static bool IsInitialized { get; set; }
    }
}