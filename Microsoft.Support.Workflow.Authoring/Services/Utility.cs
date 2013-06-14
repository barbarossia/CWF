// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;

namespace Microsoft.Support.Workflow.Authoring.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using CWF.DataContracts;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns.ViewModels;

    /// <summary>
    /// The utility.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// The name of the key that causes the Assemblies folder to be wiped out on the next application startup
        /// </summary>
        private const string DeleteAssembliesOnStartup = "DeleteCacheOnStartup";

        /// <summary>
        /// Format for name.dll
        /// </summary>
        public const string DllNameFormat = "{0}.dll";

        public static string[] BuiltinAssemblies
        {
            get
            {
                return new string[] 
                {
                    "System",
                    "System.Configuration",
                    "Gsfx", 
                    "Microsoft.Practices",
                    "Accessibility",
                    "AdoNetDiag",
                    "alink",
                    "AspNetMMCExt",
                    "aspnet_filter",
                    "aspnet_isapi",
                    "Aspnet_perf",
                    "aspnet_rc",
                    "clr",
                    "clretwrc",
                    "clrjit",
                    "CORPerfMonExt",
                    "Culture",
                    "CustomMarshalers",
                    "dfdll",
                    "diasymreader",
                    "EventLogMessages",
                    "FileTracker",
                    "fusion",
                    "InstallUtilLib",
                    "ISymWrapper",
                    "Microsoft.Build.Conversion.v4.0",
                    "Microsoft.Build",
                    "Microsoft.Build.Engine",
                    "Microsoft.Build.Framework",
                    "Microsoft.Build.Tasks.v4.0",
                    "Microsoft.Build.Utilities.v4.0",
                    "Microsoft.CSharp",
                    "Microsoft.Data.Entity.Build.Tasks",
                    "Microsoft.JScript",
                    "Microsoft.Transactions.Bridge",
                    "Microsoft.Transactions.Bridge.Dtc",
                    "Microsoft.VisualBasic.Activities.Compiler",
                    "Microsoft.VisualBasic.Compatibility.Data",
                    "Microsoft.VisualBasic.Compatibility",
                    "Microsoft.VisualBasic",
                    "Microsoft.VisualBasic.Vsa",
                    "Microsoft.VisualC",
                    "Microsoft.VisualC.STLCLR",
                    "Microsoft.Vsa",
                    "Microsoft.Windows.ApplicationServer.Applications",
                    "Microsoft_VsaVb",
                    "MmcAspExt",
                    "mscordacwks",
                    "mscordbi",
                    "mscoreei",
                    "mscoreeis",
                    "mscorlib",
                    "mscorpe",
                    "mscorpehost",
                    "mscorrc",
                    "mscorsecimpl",
                    "mscorsn",
                    "mscorsvc",
                    "nlssorting",
                    "normalization",
                    "PerfCounter",
                    "peverify",
                    "SbsNclPerf",
                    "ServiceModelEvents",
                    "ServiceModelInstallRC",
                    "ServiceModelPerformanceCounters",
                    "ServiceModelRegUI",
                    "ServiceMonikerSupport",
                    "SMDiagnostics",
                    "SOS",
                    "sysglobl",
                    "System.Activities.Core.Presentation",
                    "System.Activities",
                    "System.Activities.DurableInstancing",
                    "System.Activities.Presentation",
                    "System.AddIn.Contract",
                    "System.AddIn",
                    "System.ComponentModel.Composition",
                    "System.ComponentModel.DataAnnotations",
                    "System.configuration",
                    "System.Configuration.Install",
                    "System.Core",
                    "System.Data.DataSetExtensions",
                    "System.Data",
                    "System.Data.Entity.Design",
                    "System.Data.Entity",
                    "System.Data.Linq",
                    "System.Data.OracleClient",
                    "System.Data.Services.Client",
                    "System.Data.Services.Design",
                    "System.Data.Services",
                    "System.Data.SqlXml",
                    "System.Deployment",
                    "System.Design",
                    "System.Device",
                    "System.DirectoryServices.AccountManagement",
                    "System.DirectoryServices",
                    "System.DirectoryServices.Protocols",
                    "System",
                    "System.Drawing.Design",
                    "System.Drawing",
                    "System.Dynamic",
                    "System.EnterpriseServices",
                    "System.EnterpriseServices.Thunk",
                    "System.EnterpriseServices.Wrapper",
                    "System.IdentityModel",
                    "System.IdentityModel.Selectors",
                    "System.IO.Log",
                    "System.Management",
                    "System.Management.Instrumentation",
                    "System.Messaging",
                    "System.Net",
                    "System.Net.Http",
                    "System.Numerics",
                    "System.Runtime.Caching",
                    "System.Runtime.DurableInstancing",
                    "System.Runtime.Remoting",
                    "System.Runtime.Serialization",
                    "System.Runtime.Serialization.Formatters.Soap",
                    "System.Security",
                    "System.ServiceModel.Activation",
                    "System.ServiceModel.Activities",
                    "System.ServiceModel.Channels",
                    "System.ServiceModel.Discovery",
                    "System.ServiceModel.Internals",
                    "System.ServiceModel",
                    "System.ServiceModel.Routing",
                    "System.ServiceModel.ServiceMoniker40",
                    "System.ServiceModel.WasHosting",
                    "System.ServiceModel.Web",
                    "System.ServiceProcess",
                    "System.Transactions",
                    "System.Web.Abstractions",
                    "System.Web.ApplicationServices",
                    "System.Web.DataVisualization.Design",
                    "System.Web.DataVisualization",
                    "System.Web",
                    "System.Web.DynamicData.Design",
                    "System.Web.DynamicData",
                    "System.Web.Entity.Design",
                    "System.Web.Entity",
                    "System.Web.Extensions.Design",
                    "System.Web.Extensions",
                    "System.Web.Mobile",
                    "System.Web.RegularExpressions",
                    "System.Web.Routing",
                    "System.Web.Services",
                    "System.Windows.Forms.DataVisualization.Design",
                    "System.Windows.Forms.DataVisualization",
                    "System.Windows.Forms",
                    "System.Workflow.Activities",
                    "System.Workflow.ComponentModel",
                    "System.Workflow.Runtime",
                    "System.WorkflowServices",
                    "System.Xaml",
                    "System.Xaml.Hosting",
                    "System.Xml",
                    "System.Xml.Linq",
                    "System.Xml.Serialization",
                    "TLBREF",
                    "webengine",
                    "webengine4",
                    "WMINet_Utils",
                    "XamlBuildTask",
                    // WPF
                    "NaturalLanguage6",
                    "NlsData0009",
                    "NlsLexicons0009",
                    "PenIMC",
                    "PresentationBuildTasks",
                    "PresentationCore",
                    "PresentationFramework.Aero",
                    "PresentationFramework.Classic",
                    "PresentationFramework",
                    "PresentationFramework.Luna",
                    "PresentationFramework.Royale",
                    "PresentationHost_v0400",
                    "PresentationNative_v0400",
                    "PresentationUI",
                    "ReachFramework",
                    "System.Printing",
                    "System.Speech",
                    "System.Windows.Input.Manipulations",
                    "System.Windows.Presentation",
                    "UIAutomationClient",
                    "UIAutomationClientsideProviders",
                    "UIAutomationProvider",
                    "UIAutomationTypes",
                    "WindowsBase",
                    "WindowsFormsIntegration",
                    "wpfgfx_v0400"
                };
            }
        }

        /// <summary>
        /// The assembly is built in.
        /// </summary>
        /// <param name="assemblyName">
        /// The assembly name.
        /// </param>
        /// <returns>
        /// The bool value indicates if assembly is built in.
        /// </returns>
        public static bool AssemblyIsBuiltIn(AssemblyName assemblyName)
        {
            return AssemblyIsBuiltIn(assemblyName.FullName);
        }

        /// <summary>
        /// The assembly is built in.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The assembly full name.
        /// </param>
        /// <returns>
        /// The value indicate if a assembly is .NET built-in.
        /// </returns>
        public static bool AssemblyIsBuiltIn(string assemblyFullName)
        {
            // This list contains public tokens used by .NET Framework
            var builtInTokens = new List<string> { "31bf3856ad364e35", "b77a5c561934e089", "b03f5f7f11d50a3a", };

            string token = GetPublicTokenFromAssemblyFullName(assemblyFullName);
            string name = GetNameFromAssemblyFullName(assemblyFullName);
            return builtInTokens.Contains(token) && BuiltinAssemblies.Contains(name);
        }

        /// <summary>
        /// The rehosted designer will trigger unresolved assembly exceptions when activities are dropped on 
        /// the design surface even if the assembly has previously been loaded.  This code is intended to 
        /// handle the exception and return the assembly from memory if it is already in memory, or to get the
        /// assembly from either the cache or the asset store if it is not in memory.
        /// </summary>
        /// <param name="requiredAssemblyFullName">
        /// The required assembly full name.
        /// </param>
        /// <returns>
        /// Resolved assembly
        /// </returns>
        public static Assembly Resolve(string requiredAssemblyFullName, IEnumerable<ActivityAssemblyItem> cache = null)
        {
            var reqAssemblyName = new AssemblyName(requiredAssemblyFullName);
            string assemblyName = reqAssemblyName.Name;
            // Resource files do not need to be resolved for the reshosted designer to work
            if (assemblyName.ToLower().EndsWith("resources"))
            {
                return null;
            }

            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            Assembly result = loadedAssemblies.FirstOrDefault(a => a.FullName == requiredAssemblyFullName);
            // If the Assembly is NOT loaded in the requested Domain
            if (result == null)
            {
                // We first see if it an ActivityAssemblyItem in our cache
                ActivityAssemblyItem aai;
                result = LoadCachedAssembly(cache ?? Caching.ActivityAssemblyItems, reqAssemblyName, out aai) ? Assembly.LoadFrom(aai.Location) : null;
            }

            // TODO: Conditionally load the assembly from the Repository

            return result;
        }

        /// <summary>
        /// The copy assembly.
        /// </summary>
        /// <param name="assemblyName">
        /// The assembly.
        /// </param>
        /// <param name="sourceFileName">
        /// The source file name.
        /// </param>
        /// <param name="destFileName">
        /// The dest file name.
        /// </param>
        /// <param name="removeSourceFile">
        /// The remove source file.
        /// </param>
        /// <param name="forceOverride">
        /// The force override.
        /// </param>
        /// <returns>
        /// The bool value indicates if assembly was copied.
        /// </returns>
        private static bool CopyAssembly(
            AssemblyName assemblyName,
            string sourceFileName,
            out string destFileName,
            bool removeSourceFile,
            bool forceOverride)
        {
            string assembliesDirPath = GetAssembliesDirectoryPath();
            string level1 = string.Format(@"{0}\{1}", assembliesDirPath, assemblyName.Name);

            if (!Directory.Exists(level1))
            {
                Directory.CreateDirectory(level1);
            }

            string level2 = string.Format(@"{0}\{1}", level1, assemblyName.Version.IfNotNull(v => v.ToString()) ?? "None");
            destFileName = string.Format(@"{0}\{1}.dll", level2, assemblyName.Name);

            if (Directory.Exists(level2))
            {
                if (forceOverride == false)
                {
                    return false; // Activity assembly already be there. And don't override.
                }
                else
                {
                    FileService.DeleteDirectory(level2);
                }
            }

            Directory.CreateDirectory(level2);

            if (removeSourceFile)
            {
                File.Move(sourceFileName, destFileName);
            }
            else
            {
                File.Copy(sourceFileName, destFileName);
            }

            return true;
        }

        /// <summary>
        /// The copy assembly to local caching directory.
        /// </summary>
        /// <param name="assemblyName">
        /// The assembly name.
        /// </param>
        /// <param name="sourceFileName">
        /// The source file name.
        /// </param>
        /// <param name="forceOverride">
        /// The force override.
        /// </param>
        /// <returns>The destination path</returns>
        public static string CopyAssemblyToLocalCachingDirectory(
            AssemblyName assemblyName, string sourceFileName, bool forceOverride)
        {
            string destFileName = string.Empty;
            CopyAssembly(assemblyName, sourceFileName, out destFileName, false, forceOverride);
            return destFileName;
        }

        /// <summary>
        /// The deserialize saved content.
        /// </summary>
        /// <param name="sourceFileName">
        /// The source file name.
        /// </param>
        /// <returns>
        /// The object was deserialized from saved file..
        /// </returns>
        public static object DeserializeSavedContent(string sourceFileName)
        {
            object result = null;
            if (File.Exists(sourceFileName))
            {
                using (Stream stream = File.Open(sourceFileName, FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    result = formatter.Deserialize(stream);
                }
                return result;
            }

            throw new SerializationException(string.Format("File not found: {0}", sourceFileName));
        }

        /// <summary>
        /// The get activity assembly catalog file name.
        /// </summary>
        /// <returns>
        /// The full path of activity assembly catalog file.
        /// </returns>
        public static string GetActivityAssemblyCatalogFileName()
        {
            string assembliesDirPath = GetAssembliesDirectoryPath();
            string catalogFilePath = string.Format(@"{0}\{1}.txt", assembliesDirPath, "ActivityAssemblyCatalog");
            return catalogFilePath;
        }


        /// <summary>
        /// Get the local assemblies directory path. If not exists, create it.
        /// </summary>
        /// <returns>
        /// The assemblies directory path.
        /// </returns>
        public static string GetAssembliesDirectoryPath()
        {
            string assembliesDirPath = GetLocalDirectory("Assemblies");
            return assembliesDirPath;
        }

        /// <summary>
        /// Get the local projects directory path. If not exists, create it.
        /// </summary>
        /// <returns>
        /// The projects directory path.
        /// </returns>
        public static string GetProjectsDirectoryPath()
        {
            string projectsDirPath = GetLocalDirectory("Projects");
            return projectsDirPath;
        }

        /// <summary>
        /// Get the local content (XML files) directory path. If not exists, create it.
        /// </summary>
        /// <returns>
        /// The content directory path.
        /// </returns>
        public static string GetContentDirectoryPath()
        {

            string contentDirPath = string.Empty;
            if (ConfigurationManager.AppSettings["ContentDirectoryPath"] != null)
            {
                contentDirPath = ConfigurationManager.AppSettings["ContentDirectoryPath"].ToString(CultureInfo.InvariantCulture);
            }

            return contentDirPath;
        }

        /// <summary>
        /// Gets the location of the current running assembly
        /// </summary>
        /// <returns>
        /// The path of the current executing assembly.
        /// </returns>
        public static string GetExecutingAssemblyPath()
        {
            ////string path = Environment.CurrentDirectory;
            ////string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            const string path = ".";
            return path;
        }

        /// <summary>
        /// The get name from assembly full name.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The assembly full name.
        /// </param>
        /// <returns>
        /// The name (short name) of a assembly.
        /// </returns>
        public static string GetNameFromAssemblyFullName(string assemblyFullName)
        {
            if (null == assemblyFullName)
                return string.Empty;

            //// Example -- "System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            string[] parts = assemblyFullName.Split(',');
            return parts[0];
        }

        /// <summary>
        /// To get public token from assemlby full name
        /// </summary>
        /// <param name="assemblyFullName">
        /// The full name of assembly
        /// </param>
        /// <returns>
        /// The public token of assmably
        /// </returns>
        public static string GetPublicTokenFromAssemblyFullName(string assemblyFullName)
        {
            string token = assemblyFullName.Substring(assemblyFullName.LastIndexOf('=') + 1).ToLower();
            return token;
        }

        /// <summary>
        /// Translate generic activity class name to readable name. For example from Add `3 to Add T1,T2,T3
        /// </summary>
        /// <param name="activityTypeName">
        /// Activity type name
        /// </param>
        /// <returns>
        /// The translate activity type name.
        /// </returns>
        public static string TranslateActivityTypeName(string activityTypeName)
        {
            int position = activityTypeName.IndexOf('`');

            if (position < 0)
            {
                return activityTypeName;
            }

            string className = activityTypeName.Substring(0, position);
            string number = activityTypeName.Substring(position + 1);
            int x = int.Parse(number);
            var strArr = new string[x];

            for (int i = 0; i < x; i++)
            {
                strArr[i] = string.Format("T{0}", i + 1);
            }

            return string.Format("{0} <{1}>", className, string.Join(",", strArr));
        }

        /// <summary>
        /// Checks for the Deletion of the Cache, deletes the cache if required and resets the application flag
        /// </summary>
        public static void CheckForDeleteCache()
        {
            if (GetDeleteAssemblyFlagFromConfiguration())
            {
                // Delete the Assemblies folder
                string assemblyFolder = GetAssembliesDirectoryPath();

                FileService.DeleteDirectory(assemblyFolder);
                bool isPathCleared = !Directory.Exists(assemblyFolder);

                // Reset the Flag in accordance with successful deletion of the path
                SetDeleteAssemblyFlagInConfiguration(!isPathCleared);
            }
        }

        /// <summary>
        /// Gets the Delete Flag setting from the application configuration
        /// </summary>
        /// <returns>True if the flag is set</returns>
        public static bool GetDeleteAssemblyFlagFromConfiguration()
        {
            // Get the flag
            string deleteFlagSetting = ConfigurationManager.AppSettings[DeleteAssembliesOnStartup];
            return !string.IsNullOrEmpty(deleteFlagSetting) && deleteFlagSetting.Equals("1", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Sets the Value of the Delete Assembly Flag
        /// </summary>
        /// <param name="deleteFlag">If true then the value is set to 1 otherwise it is set to 0, the default is true</param>
        public static void SetDeleteAssemblyFlagInConfiguration(bool deleteFlag = true)
        {
            // We need the full path to the assembly to open the configuration file so we can write to it
            string configurationFileName = Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName;
            // Open the configuration
            Configuration thisConfig = ConfigurationManager.OpenExeConfiguration(configurationFileName);
            // CHeck for the flag in the configuration
            var deleteFlagSetting = thisConfig.AppSettings.Settings[DeleteAssembliesOnStartup];
            // If the flag was not present or appSettings is not present 
            if (deleteFlagSetting == null)
            {
                // Add it
                thisConfig.AppSettings.Settings.Add(DeleteAssembliesOnStartup, deleteFlag ? "1" : "0");
            }
            else
            {
                // otherwise change the value
                deleteFlagSetting.Value = deleteFlag ? "1" : "0";
            }
            // Only save the changes we made
            thisConfig.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// Get a directory path under current assembly. If not exists, create it.
        /// </summary>
        /// <param name="directoryName">Local directory name</param>
        /// <returns>The get local directory.</returns>
        public static string GetLocalDirectory(string directoryName)
        {
            string currentAssemblyPath = GetExecutingAssemblyPath();
            string result = Path.Combine(currentAssemblyPath, directoryName);

            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }

            return result;
        }

        public static void Assign<T>(this ObservableCollection<T> target, IEnumerable<T> values)
        {
            target.Clear();
            foreach (var val in values)
            {
                target.Add(val);
            }
        }

        public static Func<AssemblyInspectionService> GetAssemblyInspectionService = () => new AssemblyInspectionService();

        public static Func<Application, Window> FuncGetCurrentActiveWindow = ((app) =>
        {
            if (app == null)
                return null;

            Window main = null;
            if (app != null)
                main = app.IfNotNull(w => w.Windows).Cast<Window>().ToList().SingleOrDefault(w => w.IsActive == true);
            if (app != null && main == null)
                main = app.MainWindow;
            return main;
        });

        /// <summary>
        /// Start a task and show a busy dialog (on the current thread) until it finishes
        /// </summary>
        /// <param name="busyCaption">Caption for dialog</param>
        /// <param name="taskSpecification">What the task should do/return</param>
        /// <param name="runInBackground"></param>
        public static void DoTaskWithBusyCaption(string busyCaption, Action taskSpecification, bool runInBackground = true)
        {
            DoTaskWithBusyCaption(busyCaption, () => { taskSpecification(); return 0; }, runInBackground);
        }

        /// <summary>
        /// Start a task and show a busy dialog (on the current thread) until it finishes
        /// </summary>
        /// <param name="busyCaption">Caption for dialog</param>
        /// <param name="taskSpecification">What the task should do/return</param>
        /// <returns>Task representing completiong, i.e. taskSpecification is done and dialog is closed</returns>
        [SuppressMessage("Microsoft.Security", "CA2122", Justification = "Method is internal and has been reviewed. Users "
            + "will not be able to use this method to Dispatcher.PushFrame().")]
        public static T DoTaskWithBusyCaption<T>(string busyCaption, Func<T> taskSpecification, bool runInBackground = true)
        {
            // set busy and return a function for doing unbusy
            var unbusy = SetBusyAndGetUnbusy(Application.Current, busyCaption);
            if (runInBackground)
            {
                if (SynchronizationContext.Current == null)
                {
                    Task<T> taskToWaitFor = Task.Factory.StartNew(taskSpecification);
                    return taskToWaitFor.ResultOrException(); // Unwrap AggregateException if any
                }
                TaskScheduler ts = TaskScheduler.FromCurrentSynchronizationContext();
                DispatcherFrame frame = new DispatcherFrame();
                // Run this task on synchronizationContext if possible, because current thread may be an STA thread already
                // and caller may be expecting to be allowed to create UI objects. If there is no sync context then we have 
                // to block this thread, which means we have to run the task on the thread pool instead, but in that case
                // we can't be on a UI thread.
                Task<T> cleanupTask = Task.Factory.StartNew(taskSpecification).ContinueWith(t =>
                {
                    if (unbusy != null)
                        unbusy();
                    frame.Continue = false;
                    return t.ResultOrException();
                }, ts);
                Dispatcher.PushFrame(frame);
                return cleanupTask.ResultOrException();
            }
            else
            {
                Dispatcher uiDispatcher = Dispatcher.CurrentDispatcher;
                T result = default(T);
                DispatcherFrame frame = new DispatcherFrame();
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(500); // to make busy indicator shown
                    uiDispatcher.BeginInvoke(DispatcherPriority.Background,
                        new DispatcherOperationCallback((f) =>
                        {
                            try
                            {
                                result = taskSpecification();
                            }
                            finally
                            {
                                if (unbusy != null)
                                    unbusy();
                                ((DispatcherFrame)f).Continue = false;
                            }
                            return null;
                        }), frame);
                });
                Dispatcher.PushFrame(frame);
                return result;
            }
        }

        /// <summary>
        /// Helper method for DoTaskWithBusyCaption, for Don't Repeat Yourself (DRY) principle
        /// </summary>
        /// <param name="app"></param>
        /// <param name="busyCaption"></param>
        /// <returns></returns>
        private static Action SetBusyAndGetUnbusy(Application app, string busyCaption)
        {
            if (app == null)
            {
                return null; // we're just running in a test, not the real app. There is no main window to change.
            }

            Window main = FuncGetCurrentActiveWindow(app);
            if (main == null)
                return null;
            Action setup = () =>
                           main
                               .IfNotNull(w => w.DataContext as ViewModelBase)
                               .IfNotNull(vm =>
                                              {
                                                  vm.IsBusy = true;
                                                  vm.BusyCaption = busyCaption;
                                              });
            Action cleanup = () => main
                                       .IfNotNull(w => w.DataContext as ViewModelBase)
                                       .IfNotNull(vm =>
                                                      {
                                                          vm.IsBusy = false;
                                                      });
            if (app.Dispatcher.Thread == Thread.CurrentThread)
            {
                app.Dispatcher.Invoke(setup, DispatcherPriority.Send);
                return cleanup;
            }

            app.Dispatcher.Invoke(setup, DispatcherPriority.Send);
            return () => app.Dispatcher.Invoke(cleanup);
        }

        /// <summary>
        /// Used for atomic server operations which cancel cleanly, like uploading or downloading data.
        /// Displays a "Contacting server..." message while things are happening, 
        /// throws a UserFacingException "server is not available" if the server is down. Does not necessarily
        /// encapsulate a single atomic transaction or create a client--you will probably still 
        /// have a WorkflowsQueryServiceUtility.UsingClient() somewhere inside the spec. Think of this method as being
        /// part of the View layer (displaying UI for the user), and UsingClient is part of the 
        /// Model layer (programmatic correctness).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static T WithContactServerUI<T>(Func<T> taskSpecification, bool runInBackground = true)
        {
            string busyCaption = "Contacting server...";
            try
            {
                return DoTaskWithBusyCaption(busyCaption, taskSpecification, runInBackground);
            }
            catch (CommunicationException e)
            {
                throw new UserFacingException("The server is not available right now.", e);
            }
            catch (TimeoutException e)
            {
                throw new UserFacingException("The server is not available right now (request timed out).", e);
            }
        }

        /// <summary>
        /// Convenience overload for things like Upload that don't expect a return value.
        /// </summary>
        public static void WithContactServerUI(Action taskSpecification, bool runInBackground = true)
        {
            WithContactServerUI(() => { taskSpecification(); return 0; }, runInBackground);
        }

        /// <summary>
        /// Gets the name of the user who is performing actions with this tool.
        /// </summary>
        /// <returns>Current user name.</returns>
        public static string GetCurrentUserName()
        {
            return Environment.UserName;
        }

        /// <summary>
        /// Gets the current caller name.
        /// </summary>
        /// <returns>Caller name.</returns>
        public static string GetCallerName()
        {
            return Assembly.GetExecutingAssembly().FullName;
        }

        /// <summary>
        /// Gets the current caller version.
        /// </summary>
        /// <returns>Caller version.</returns>
        public static string GetCallerVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Try to retrieve a cached assembly
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="cachedAssembly"></param>
        /// <returns></returns>
        public static bool LoadCachedAssembly(
            IEnumerable<ActivityAssemblyItem> activityAssemblyItems,
            AssemblyName assemblyName,
            out ActivityAssemblyItem cachedAssembly)
        {
            cachedAssembly = activityAssemblyItems.FirstOrDefault(assembly => assembly.Matches(assemblyName));
            return cachedAssembly != null;
        }
    }
}
