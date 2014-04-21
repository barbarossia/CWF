using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CWF.DataContracts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Activities.XamlIntegration;
using System.Xaml;
using System.Collections;
using System.Security.Principal;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Security;
using Microsoft.Support.Workflow.Authoring.ExpressionEditor;
using Microsoft.Support.Workflow.Authoring.Common;
using System.Activities.Presentation.Model;
using System.Activities.Presentation;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.Support.Workflow.Authoring.AddIns;
using Microsoft.Support.Workflow.Authoring.AddIns.Data;
using Microsoft.Support.Workflow.Authoring.AssetStore;
using System.Windows.Data;

namespace Microsoft.Support.Workflow.Authoring.Tests
{
    /// <summary>
    /// Enumeration of locations to save workflows or workflowitems
    /// </summary>
    public enum Locations
    {
        ToLocal,//ToLocal Location
        ToServer, // ToServer Location  
        ToMarketplace, //ToMarketPlace Location
        ToImage,//To Image format
        FromLocal, //open from local location 
        FromServer, // ToServer Location
    }

    static class TestUtilities
    {
        public const string ListOfTemplateErrorMessage = "GetListOfTemplates returned an empty list";

        /// <summary>
        /// Generate random status and this can be "Private, Public or Retired"
        /// </summary>
        public static string GetRandomStatus
        {
            get
            {
                string result = "Private";
                var random = new Random();
                int switchNum = random.Next(0, 2);

                switch (switchNum)
                {
                    case 0:
                        result = "Private";
                        break;
                    case 1:
                        result = "Public";
                        break;
                    case 2:
                        result = "Retire";
                        break;
                }
                return result;
            }
        }

        /// <summary>
        /// Generate random boolean value and can be true or false
        /// </summary>
        public static bool GetRandomBoolean
        {
            get
            {
                bool result = false;
                var random = new Random();
                //int switchNum = random.Next(0, 100);
                int switchNum = (random.Next(0, 100) % 2);

                switch (switchNum)
                {
                    case 0:
                        result = false;
                        break;
                    case 1:
                        result = true;
                        break;
                }
                return result;
            }
        }

        /// <summary>
        /// Generate random status and this can be "Private, Public or Retired"
        /// </summary>
        public static string GetRandomCategory
        {
            get
            {
                string result = "Administration";
                var random = new Random();
                int switchNum = random.Next(0, 6);

                switch (switchNum)
                {
                    case 0:
                        result = "OAS Basic Controls";
                        break;
                    case 1:
                        result = "Developer Toolbox";
                        break;
                    case 2:
                        result = "Pages";
                        break;
                    case 3:
                        result = "Specific";
                        break;
                    case 4:
                        result = "Generic Activities";
                        break;
                    case 5:
                        result = "Publishing Workflow";
                        break;
                    case 6:
                        result = "Business";
                        break;
                }
                return result;
            }
        }

        /// <summary>
        /// Generate random string
        /// </summary>
        /// <param name="size">length of the random value to be generated</param>
        /// <returns></returns>
        public static string GenerateRandomString(int size)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Get the xaml code from a activity instance.
        /// </summary>
        /// <param name="activity">
        /// The activity.
        /// </param>
        /// <returns>
        /// The xaml of a activity.
        /// </returns>
        public static string ToXaml(ActivityBuilder<int> ab, string name)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            XamlWriter xw = ActivityXamlServices.CreateBuilderWriter(new XamlXmlWriter(tw, new XamlSchemaContext()));
            XamlServices.Save(xw, ab);
            return sb.ToString();
        }

        /// <summary>
        /// Calculates the next expected version number
        /// </summary>
        /// <param name="currentVersion">string of current version</param>
        /// <returns>string of next expected version</returns>
        public static string CalculateNextExpectedVersion(string currentVersion)
        {
            Version currVersion = new Version(currentVersion);
            return new Version(currVersion.Major, currVersion.Minor, currVersion.Build + 1, currVersion.Revision).ToString();
        }

        public static void MockUtilityGetCurrentWindowReturnNull(Action action)
        {
            var getCurrentWindow = Utility.FuncGetCurrentActiveWindow;
            Utility.FuncGetCurrentActiveWindow = ((app) => { return null; });
            action();
            Utility.FuncGetCurrentActiveWindow = getCurrentWindow;
        }

        public static void RegistUtilityGetCurrentWindow()
        {
            Utility.FuncGetCurrentActiveWindow = (app) =>
            {
                return null;
            };
        }

        /// <summary>
        /// Uses Query Service to get all items in the StoreActivities table 
        /// </summary>
        /// <returns>Returns a collection of all items in the StoreActivities table</returns>
        public static IEnumerable<StoreActivitiesDC> GetEntireCollectionFromStoreActivitiesTable()
        {
            IEnumerable<StoreActivitiesDC> workflows = null;

            WorkflowsQueryServiceUtility.UsingClient(client =>
            {
                // Now fill in the actual workflow list
                workflows = from activity in
                                client.StoreActivitiesGet(
                                    new StoreActivitiesDC
                                    {
                                        Incaller = Environment.UserName,
                                        IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                                    })
                            select activity;
            });

            return workflows;
        }

        /// <summary>
        /// Checks workflow version against expected version in the Activity Libraries and Store Activities tables
        /// </summary>
        /// <param name="expectedVersion">Expected Version</param>
        /// <param name="wfi">WorkflowItem to verify</param>
        public static void VerifyWorkflowNameAndVersionInDB(string expectedVersion, WorkflowItem wfi)
        {
            Assert.AreEqual(expectedVersion, wfi.Version);

            Assert.IsNotNull(TestUtilities.GetCollectionFromStoreActivitiesTable().FirstOrDefault(saDC => saDC.ShortName == wfi.Name && saDC.Version == wfi.Version),
                                                        "Did not find workflow with that name and version in Store Activities table.");

            Assert.IsNotNull(TestUtilities.GetCollectionFromActivityLibrariesTable().FirstOrDefault(saDC => saDC.Name.Contains(wfi.Name) && saDC.Version == Version.Parse(wfi.Version)),
                                                        "Did not find workflow containing that name and version in Activity Libraries table.");
        }

        /// <summary>
        /// Test whether assembly can be imported, pop up error message if disallowed
        /// </summary>
        /// <param name="assemblyFileName"></param>
        /// <returns>True if okay to import, false if already imported</returns>
        public static bool PreviewImportAssembly(string assemblyFileName)
        {
            AssemblyName assemblyName;
            try
            {
                assemblyName = AssemblyName.GetAssemblyName(assemblyFileName);
            }
            catch (Exception)
            {
                return false;
            }

            ActivityAssemblyItem ignore;
            if (Caching.LoadCachedAssembly(assemblyName, out ignore))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks workflow version against expected version in the Activity Libraries and Store Activities tables
        /// </summary>
        /// <param name="expectedVersion">Expected Version</param>
        /// <param name="workflowName">Workflow Name</param>
        public static void VerifyWorkflowNameAndVersionInDB(string expectedVersion, string workflowName)
        {
            List<ActivityLibraryDC> activityLibraryDCs = GetActivityLibrariesByName(workflowName);
            Assert.AreNotEqual(null, activityLibraryDCs);
            Assert.AreNotEqual(null, activityLibraryDCs.Where(dc => dc.VersionNumber == expectedVersion).FirstOrDefault());
        }

        /// <summary>
        /// Gets the collection of ActivityLibraryDC in the ActivityLibraries table
        /// which match the workflow name
        /// </summary>
        /// <param name="workflowName">Workflow Item Name</param>
        /// <returns>The collection of ActivityLibraryDC match the name</returns>
        public static List<ActivityLibraryDC> GetActivityLibrariesByName(string workflowName)
        {
            List<ActivityLibraryDC> activityAssemblyItems = WorkflowsQueryServiceUtility.UsingClientReturn<List<ActivityLibraryDC>>(client => client.ActivityLibraryGet(new ActivityLibraryDC
            {
                Incaller = Environment.UserName,
                IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                Name = workflowName
            }));

            return activityAssemblyItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public static List<StoreActivitiesDC> GetStoreActivitiesFromeServerByName(string activityName, Env env = Env.Dev)
        {
            IList<StoreActivitiesDC> storeActivities = WorkflowsQueryServiceUtility.UsingClientReturn<IList<StoreActivitiesDC>>(client => client.StoreActivitiesGetByName(new StoreActivitiesDC
            {
                Name = activityName,
                Incaller = Environment.UserName,
                IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                Environment = env.ToString()
            }));

            //Assert.AreNotEqual(0, storeActivities.Count, String.Format("StoreActivities.Count = {0}", storeActivities.Count));
            return storeActivities.ToList();
        }

        /// <summary>
        /// Gets the collection of StoreActivitiesDC from the Store Activities table *Creaded by v-jerzha*
        /// </summary>
        /// <returns></returns>
        public static List<StoreActivitiesDC> GetStoreActivitiesFromServer()
        {
            List<StoreActivitiesDC> storeActivities = WorkflowsQueryServiceUtility.UsingClientReturn<List<StoreActivitiesDC>>(client => client.StoreActivitiesGet(new StoreActivitiesDC
            {
                Incaller = Environment.UserName,
                IncallerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            }));

            Assert.AreNotEqual(0, storeActivities.Count, String.Format("StoreActivities.Count = {0}", storeActivities.Count));
            Console.WriteLine("StoreActivities.Count = {0}", storeActivities.Count);
            return storeActivities;
        }

        /// <summary>
        /// Checks workflow expected properties in the Activity Libraries and Store Activities tables
        /// </summary>
        /// <param name="properties"> </param>
        /// <param name="wfi">WorkflowItem to verify</param>
        public static void VerifyWorkflowPropertiesInDB(WorkFlowProperties properties, WorkflowItem wfi)
        {
            //Verification In Store Activities table
            Assert.IsNotNull(TestUtilities.GetCollectionFromStoreActivitiesTable().FirstOrDefault(saDC => saDC.ShortName == properties.Name),
                                      string.Format("WorkFlow Name Expected:{0}  but not found in Store Activities Table.", properties.Name));
            Assert.IsNotNull(TestUtilities.GetCollectionFromStoreActivitiesTable().FirstOrDefault(saDC => saDC.Version == properties.Version),
                                      string.Format("WorkFlow Version Expected:{0}  but not found in Store Activities Table.", properties.Version));
            Assert.IsNotNull(TestUtilities.GetCollectionFromStoreActivitiesTable().FirstOrDefault(saDC => saDC.StatusCodeName == properties.Status),
                                      string.Format("WorkFlow Status Expected:{0}  but not found in Store Activities Table.", properties.Status));

            //Verification In Activity Libraries table
            Assert.IsNotNull(TestUtilities.GetCollectionFromActivityLibrariesTable().FirstOrDefault(saDC => saDC.Name.Contains(properties.Name)),
                                    string.Format("WorkFlow Name Expected:{0}  but not found in Activity Libraries Table.", properties.Name));
            Assert.IsNotNull(TestUtilities.GetCollectionFromActivityLibrariesTable().FirstOrDefault(saDC => saDC.Version.ToString().Contains(properties.Version)),
                                    string.Format("WorkFlow Version Expected:{0}  but not found in Activity Libraries Table.", properties.Version));
            Assert.IsNotNull(TestUtilities.GetCollectionFromActivityLibrariesTable().FirstOrDefault(saDC => saDC.Name.Contains(properties.Status)),
                                    string.Format("WorkFlow Status Expected:{0}  but not found in Activity Libraries Table.", properties.Status));
        }

        /// <summary>
        /// Workaround for runtime exception that System.Activities are not loaded into the Test AppDomain
        /// </summary>
        public static void LoadSystemActivitesIntoCurrentAppDomain()
        {
            AppDomain.CurrentDomain.Load(new AssemblyName("System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"));
        }

        /// <summary>
        /// Tests existence by name of a workflowitem in the ActivityLibraries table
        /// </summary>
        /// <param name="workflowItem">WorkflowItem to check</param>
        /// <returns>true, if exists in table, false otherwise</returns>
        public static bool WorkflowItemExistsInActivityLibrariesTable(WorkflowItem workflowItem)
        {
            bool bExists = false;

            ObservableCollection<ActivityAssemblyItem> aaiColl = GetCollectionFromActivityLibrariesTable();

            // There should be one and only one
            Assert.AreEqual(1, aaiColl.Count(aai => aai.Name.Contains(workflowItem.Name)));
            bExists = true;

            return bExists;
        }

        /// <summary>
        /// Gets the collection of ActivityAssemblyItem in the ActivityLibraries table
        /// </summary>
        /// <returns>the ObservableCollection of ActivityAssemblyItem</returns>
        public static ObservableCollection<ActivityAssemblyItem> GetCollectionFromActivityLibrariesTable()
        {
            var vmSelectWorkflow = new SelectAssemblyAndActivityViewModel();

            WorkflowsQueryServiceUtility.UsingClient(client =>
                ((SelectAssemblyAndActivityViewModel)vmSelectWorkflow).LoadLiveData(client));

            ObservableCollection<ActivityAssemblyItem> aaiColl = vmSelectWorkflow.ActivityAssemblyItemCollection; // items in the ActivityLibraries table in the database
            Assert.AreNotEqual(0, aaiColl.Count, String.Format("ActivityLibraries.Count = {0}", aaiColl.Count));

            return aaiColl;
        }

        /// <summary>
        /// Tests existence by name and xaml of a workflowitem in the StoreActivities table
        /// </summary>
        /// <param name="workflowItem">workflowitem to check</param>
        /// <returns>true, if exists in table, false otherwise</returns>
        public static bool WorkflowItemExistsInStoreActivitiesTable(WorkflowItem workflowItem)
        {
            bool bExists = false;

            Contract.Requires(null != workflowItem);
            if (null == workflowItem)
            {
                return false;
            }

            ObservableCollection<StoreActivitiesDC> storeActivitiesDC = GetCollectionFromStoreActivitiesTable();

            foreach (StoreActivitiesDC sa in storeActivitiesDC)
            {
                if (String.Equals(sa.Name, workflowItem.Name))
                {
                    // XAML only exists in the StoreActivites table
                    Assert.AreEqual(sa.Xaml, workflowItem.XamlCode, "Workflow xaml does not match the database xaml for the corresponding workflow");
                    bExists = true;
                    break;
                }
            }

            return bExists;
        }

        /// <summary>
        /// Gets the collection of StoreActivitiesDC from the Store Activities table
        /// </summary>
        /// <returns>an ObservableCollection of StoreActivitiesDC</returns>
        public static ObservableCollection<StoreActivitiesDC> GetCollectionFromStoreActivitiesTable()
        {
            var vmOpenWorkflow = new OpenWorkflowFromServerViewModel();
            vmOpenWorkflow.EnvFilters.Where(e => e.Env == Env.Dev).FirstOrDefault().IsFilted = true;
            // loads data from the database
            WorkflowsQueryServiceUtility.UsingClient(client => vmOpenWorkflow.LoadLiveData(client));

            ObservableCollection<StoreActivitiesDC> storeActivitiesDC = vmOpenWorkflow.ExistingWorkflows; // workflows from the database
            Assert.AreNotEqual(0, storeActivitiesDC.Count, String.Format("StoreActivities.Count = {0}", storeActivitiesDC.Count));

            return storeActivitiesDC;
        }

        /// <summary>
        /// Opens a given workflow from the server
        /// </summary>
        /// <param name="saDC">StoreActivitiesDC of the workflow to open</param>
        /// <param name="mw">MainWindowViewModel the opened workflow is added to</param>
        /// <returns>the MainWindowViewModel</returns>
        public static MainWindowViewModel OpenWorkflowFromServer(StoreActivitiesDC saDC, MainWindowViewModel mw)
        {
            OpenWorkflowFromServerViewModel vmOpenWorkflow = new OpenWorkflowFromServerViewModel();

            vmOpenWorkflow.SelectedWorkflow = saDC;

            WorkflowsQueryServiceUtility.UsingClient(client =>
            {
                var assembly = new ActivityAssemblyItem { Name = vmOpenWorkflow.SelectedWorkflow.ActivityLibraryName, Version = Version.Parse(vmOpenWorkflow.SelectedWorkflow.ActivityLibraryVersion), Env = saDC.Environment.ToEnv() };
                //Caching.DownloadAndCacheAssembly(client, Caching.ComputeDependencies(client, assembly));
                //WorkflowItem itemOpened = DataContractTranslator.StoreActivitiyDCToWorkflowItem(vmOpenWorkflow.SelectedWorkflow, assembly);
                // mw.CheckIsAlreadyInListOrAdd(itemOpened);
                mw.OpenStoreActivitiesDC(saDC, assembly);
            });

            return mw;
        }

        /// <summary>
        /// Saves a workflow to location specified
        /// </summary>
        /// <param name="workflowItem">WorkflowItem to save</param>
        /// <param name="saveLocation">Location to save, i.e. "ToServer"</param>
        public static void SaveWorkflow(WorkflowItem workflowItem, string saveLocation)
        {
            //MainWindowViewModel mw = new MainWindowViewModel();
            //mw.FocusedWorkflowItem = workflowItem;
            //mw.SaveFocusedWorkflowCommand.Execute(saveLocation);
            SaveWorkflow(workflowItem, saveLocation, string.Empty);
        }

        /// <summary>
        /// Saves a workflow to location specified
        /// </summary>
        /// <param name="workflowItem">WorkflowItem to save</param>
        /// <param name="saveLocation">Location to save, i.e. "ToServer"</param>
        public static void SaveWorkflow(WorkflowItem workflowItem, string saveLocation, string localPath)
        {
            MainWindowViewModel mw = new MainWindowViewModel();
            mw.FocusedWorkflowItem = workflowItem;
            mw.SaveFocusedWorkflowCommand.Execute(saveLocation);
        }

        public static MainWindowViewModel OpenWorkflowFromLocal(string localFileName)
        {
            MainWindowViewModel mw = new MainWindowViewModel();
            //  mw.ShowOpenFileDialogAndReturnResult = new Func<string, string, string>((defaultFileName, filter) => localFileName);       
            mw.OpenWorkflowCommand.Execute("FromLocal");

            return mw;
        }


        /// <summary>
        /// 
        /// </summary>
        public static SelectAssemblyAndActivityViewModel SelectAssemblyAndActivity
        {
            get
            {
                var selectviewModel = new SelectAssemblyAndActivityViewModel();
                return selectviewModel;
            }
        }

        /// <summary>
        /// Saves a workflow to location specified
        /// </summary>
        /// <param name="workflowItem">WorkflowItem to save</param>
        /// <param name="saveLocation">Location to save, i.e. "ToServer"</param>
        /// <param name="viewModel"></param>
        public static void SaveWorkflow(WorkflowItem workflowItem, string saveLocation,
                                                       out MainWindowViewModel viewModel)
        {
            viewModel = new MainWindowViewModel();
            viewModel.FocusedWorkflowItem = workflowItem;
            viewModel.SaveFocusedWorkflowCommand.Execute(saveLocation);
        }

        /// <summary>
        /// Creates a unique name based on current date and time
        /// </summary>
        /// <param name="workflowName">Base name of workflow</param>
        /// <returns>Unique workflow name</returns>
        public static string CreateUniqueWorkflowName(string workflowName)
        {
            DateTime dt = DateTime.Now;
            string amorpm = dt.ToShortTimeString().Contains("PM") ? "P" : "A";
            return String.Format("{0}_{1}{2}{3}{4}", workflowName, dt.Hour, dt.Minute, amorpm, dt.Second);
        }

        /// <summary>
        /// Gets path of the executing assembly
        /// </summary>
        /// <returns>Path of Executing assembly</returns>
        public static string GetExecutingAssemblyPath()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        /// <summary>
        /// Extension to Assert: for unit-testing INPC notifications
        /// </summary>
        /// <param name="objectWhichWillNotify">objectWhichWillNotify</param>
        /// <param name="propertyName">propertyName</param>
        /// <param name="action">action</param>
        public static void Assert_ShouldRaiseINPCNotification(INotifyPropertyChanged objectWhichWillNotify, string propertyName, Action action)
        {
            bool notified = false;
            PropertyChangedEventHandler tracker = (s, e) =>
            {
                if (string.Equals(propertyName, e.PropertyName))
                {
                    notified = true;
                }
            };
            try
            {
                objectWhichWillNotify.PropertyChanged += tracker;
                action();
                Assert.IsTrue(notified, string.Format("PropertyChanged(\"{0}\") should have been raised", propertyName));
            }
            finally
            {
                objectWhichWillNotify.PropertyChanged -= tracker;
            }
        }

        /// <summary>
        /// Extension to Assert: for unit-testing CanExecuteChanged notifications
        /// </summary>
        public static void Assert_EventuallyShouldRaiseCanExecuteChanged(Action action, params DelegateCommandBase[] commands)
        {
            HashSet<DelegateCommandBase> notified = new HashSet<DelegateCommandBase>();
            EventHandler tracker = (s, e) =>
            {
                notified.Add((DelegateCommandBase)s);
            };
            foreach (var command in commands)
                command.CanExecuteChanged += tracker;
            Execute_Sequentially_On_Dispatcher(
                action,
                () => Assert.IsTrue(
                    commands.All(c => notified.Contains(c)),
                    string.Format("CanExecuteChanged should have been raised for {0} commands but was raised for only {1}",
                        commands.Length,
                        notified.Count))
            );
        }

        /// <summary>
        /// Like Assert.Throws, if MSTest had Assert.Throws in its framework. Will fail unless a T is thrown, and returns the T so you can
        /// do further checking on it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public static T Assert_ShouldThrow<T>(Action a) where T : Exception
        {
            try
            {
                a();
                throw new AssertFailedException(string.Format("Expected '{0}'. Actual: no exception thrown.", typeof(T).Name));
            }
            catch (T exception)
            {
                return exception;
            }
        }

        /// <summary>
        /// For unit tests that process events asynchronously on the dispatcher thread--make it easy to assert that something will eventually happen
        /// </summary>
        /// <param name="arrangeAct">The first action to execute</param>
        /// <param name="assert">The action to post to the dispatcher thread after the first one executes</param>
        public static void Execute_Sequentially_On_Dispatcher(Action arrangeAct, Action assert)
        {
            // We have to do some complicated setup to test the asynchronous behavior on an STA 
            // thread with a Dispatcher event loop running. Could be worth refactoring into an 
            // Assert extension at some point.

            Exception exn = null;

            var thread = new Thread(() =>
            {
                var disp = Application.Current.IfNotNull(app => app.Dispatcher) ?? Dispatcher.CurrentDispatcher;
                disp.UnhandledException += (s, e) =>
                {
                    exn = e.Exception;
                    e.Handled = true;
                    disp.InvokeShutdown();
                };

                // The first thing the dispatcher should do when it starts is kick off the test
                new DispatcherSynchronizationContext(disp).Post((_) =>
                {
                    // Now the async infrastructure is all set up and we can actually do the test

                    // Arrange & Act
                    arrangeAct();

                    // This will have added a work item to the dispatcher queue which will eventually update controls.
                    // So we queue yet another work item after that which will do the Assert.

                    // Assert
                    SynchronizationContext.Current.Post((_1) =>
                    {
                        // There should be two User controls after the update
                        assert();
                        disp.InvokeShutdown();
                    }, null);
                }, null);

                // Actually start the dispatcher so it can do the work we just queued
                Dispatcher.Run();
            });
            thread.SetApartmentState(ApartmentState.STA); // required to create the ToolboxControls
            thread.Start();
            thread.Join(5000); // Plenty of time. We should never hit this limit if exception handling 
            // etc. is set up right, but if we do the test will fail instead of hang.

            // This is just a way of retrieving any exception thrown on the test thread. There may be a better way.
            if (exn != null)
            {
                throw new AssertFailedException("Error!" + exn.ToString(), exn);
            }
        }

        /// <summary>
        /// Verify the Compiled Assembly is the same as the expectd dll
        /// </summary>
        /// <param name="assemblyPath">The assembly path which you just compiled (as returned by WorkflowCompiler.Compile) </param>
        /// <param name="expectedAssemblyName">the classname of the workflow you created</param>
        /// <param name="expectedActivity">this expectedActivity will be used to compare the activity retrieved through reflection from the compiled dll</param>
        public static void VerifyTheCompiledAssemblyHelper(string assemblyPath, string expectedAssemblyName, Activity expectedActivity, string expectedVersion = "1.0.0.0", string expectedClassName = null)
        {
            if (expectedClassName == null)
                expectedClassName = expectedAssemblyName;
            Assert.AreNotEqual(string.Empty, assemblyPath);
            Assert.AreEqual(expectedAssemblyName, Path.GetFileNameWithoutExtension(assemblyPath));

            // Assembly should be signed so that versioning works
            Assembly loadedAssembly = Assembly.LoadFrom(assemblyPath);
            Assert.IsTrue(loadedAssembly != null);
            Assert.IsTrue(loadedAssembly.GetName().GetPublicKey() != null && loadedAssembly.GetName().GetPublicKey().Length > 0, "Compiled assembly should have been signed");

            // Verify the classname            
            Assert.IsTrue(loadedAssembly.FullName.Contains(expectedAssemblyName));
            Assert.IsTrue(loadedAssembly.FullName.Contains(expectedVersion));

            // get type of class from the just loaded assembly
            Type typeName = loadedAssembly.GetType(expectedClassName);
            Assert.IsTrue(typeName != null);

            // create an instance of the activity thru Activator
            object activityObj = Activator.CreateInstance(typeName);

            // now use PrivateObject to get the the Protected property, Implementation, to check the details.
            PrivateObject po = new PrivateObject(activityObj);
            object dele = po.GetFieldOrProperty("Implementation");
            Activity retrievedActivity = ((Func<Activity>)dele).Invoke();

            Assert.IsTrue(AreSameSequenceInActivity(expectedActivity, retrievedActivity));
        }

        /// <summary>
        /// Compare the activites and if it has a sequence, it will recursively compare. Non-sequence activity will
        /// still be checked but not recursively.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static bool AreSameSequenceInActivity(Activity expected, Activity actual)
        {
            if (actual == null && expected == null) return true;
            if ((actual == null && expected != null) || (actual != null && expected == null)) return false;
            if (expected.GetType().FullName != actual.GetType().FullName) return false;
            if (expected.DisplayName != actual.DisplayName) return false;

            if (actual is Sequence)
            {
                var aSequence = (Sequence)actual;
                var eSequence = (Sequence)expected;

                if (aSequence.Activities.Count != eSequence.Activities.Count) return false;

                for (int i = 0; i < aSequence.Activities.Count; i++)
                {
                    Activity a = aSequence.Activities[i];
                    Activity e = eSequence.Activities[i];
                    if (!AreSameSequenceInActivity(a, e))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Create a new sequence which contains mutiple activities
        /// </summary>
        /// <returns>a new Sequence object</returns>
        public static Sequence GenerateASequenceWithMutipleActivities()
        {
            Sequence complexSequence = new Sequence
            {
                DisplayName = GetRandomStringOfLength(30),
                Activities = {  
                                new WriteLine { Text = "Hello1" }, 
                                new WriteLine { Text = "Hello2" }, 
                                new Delay { Duration = TimeSpan.FromSeconds(2) }, 

                                new Sequence // another sequence
                                {                                            
                                    Activities = 
                                    {   
                                        new WriteLine { Text = "Hello5" },                                           
                                    }     
                                }
                             }
            };

            return complexSequence;
        }

        /// <summary>
        /// Create a new sequence which contains only one activity
        /// </summary>
        /// <returns>a new Sequence object</returns>
        public static Sequence GenerateASequenceWithOneActivity()
        {
            Sequence sequenceWithoneActivity = new Sequence
            {
                DisplayName = GetRandomStringOfLength(30),
                Activities = {  
                                new WriteLine { Text = "Hello1" }                            
                             }
            };
            return sequenceWithoneActivity;
        }

        /// <summary>
        /// Create a new sequence which does not contains any activity
        /// </summary>
        /// <returns>a new Sequence object</returns>
        public static Sequence GenerateASequenceWithoutActivity()
        {
            Sequence sequenceWithoutActivity = new Sequence
            {
                DisplayName = GetRandomStringOfLength(30)
            };

            return sequenceWithoutActivity;
        }

        public static string GenerateASequenceXmalCodeWithoutActivity()
        {
            string filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Data", "Content", "SequenceTemplate.txt");
            if (!File.Exists(filePath))
                filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Content", "SequenceTemplate.txt");
            string sequenceXaml = ReadTxt(filePath);//ReadTxt(@"Content\SequenceTemplate.txt");
            sequenceXaml = sequenceXaml.Replace("RandomName", GetRandomStringOfLength(30));
            return sequenceXaml;
        }

        public static string GenerateComplexXmalCode()
        {
            string filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Data", "Content", "ComplexTemplate.txt");
            if (!File.Exists(filePath))
                filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Content", "ComplexTemplate.txt");
            string sequenceXaml = ReadTxt(filePath);
            sequenceXaml = sequenceXaml.Replace("[ClassName]", GetRandomStringOfLength(20));
            return sequenceXaml;
        }
        /// <summary>
        /// Create a string whose length is determined by input value
        /// </summary>
        /// <param name="length">the length of the new string</param>
        /// <returns>a new Sequence object</returns>
        public static string GetRandomStringOfLength(int length)
        {
            Random random = new Random();

            return new string(
                                Enumerable.Range(0, length).Select
                                (
                                    i => (char)(((int)'a') + random.Next(26))
                                ).ToArray()
                             );
        }

        public static string GetEmptyWorkFlowTemplateXamlCode()
        {
            string filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Data", "Content", "EmptyWorkflowTemplate.txt");
            if (!File.Exists(filePath))
                filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Content", "EmptyWorkflowTemplate.txt");
            return ReadTxt(filePath);//ReadTxt(@"Content\EmptyWorkflowTemplate.txt");
        }

        public static string EmptyWorkFlowTemplateXamlCode
        {
            get
            {
                return ReadTxt(@"Content\EmptyWorkflowTemplate.txt");
            }
        }

        private static string ReadTxt(string filePath)
        {

            FileStream sFile = new FileStream(filePath, FileMode.Open);
            StreamReader reader = new StreamReader(sFile);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            string xamlCode = reader.ReadToEnd();
            reader.Close();
            return xamlCode;
        }


        public static void MockWinPrincipal(string authorType, Action action)
        {
            using (var principal = new Implementation<WindowsPrincipal>())
            {
                //if (authorType == AuthorizationService.AdminAuthorizationGroupName)
                //{
                //    principal.Register(p => p.IsInRole(AuthorizationService.AdminAuthorizationGroupName))
                //                    .Return(true);
                //    principal.Register(p => p.IsInRole(AuthorizationService.AuthorAuthorizationGroupName))
                //        .Return(false);
                //}
                //else
                //{
                //    principal.Register(p => p.IsInRole(AuthorizationService.AdminAuthorizationGroupName))
                //                     .Return(false);
                //    principal.Register(p => p.IsInRole(AuthorizationService.AuthorAuthorizationGroupName))
                //        .Return(true);
                //}
                Thread.CurrentPrincipal = principal.Instance;
                action();
            }
        }

        public static void RegistWinPrincipalFunc(string authorType)
        {
            //AuthorizationService.PrincipalIsInRoleFunc = (authGroupName, currentPrincipal) =>
            //{
            //    if (authGroupName == authorType)
            //        return true;
            //    else
            //        return false;
            //};
            //AuthorizationService.CurrentPrincipalFunc = () =>
            //{
            //    //return Thread.CurrentPrincipal;
            //    return new WindowsPrincipal(WindowsIdentity.GetCurrent());
            //};
        }

        public static void RegistLoginUserRole(Role role)
        {
            Dictionary<Env, Permission> envPermissionMaps = new Dictionary<Env, Permission>();
            switch (role)
            {
                case Role.Viewer:
                    envPermissionMaps = RegistViewerPermission();
                    break;
                case Role.Author:
                    envPermissionMaps = RegistAuthorPermission();
                    break;
                case Role.Admin:
                    envPermissionMaps = RegistAdminPermission();
                    break;
                case Role.CWFEnvAdmin:
                    envPermissionMaps = RegistEnAdminPermission();
                    break;
                case Role.CWFAdmin:
                    envPermissionMaps = RegistCWFAdminPermission();
                    break;
                default:
                    break;
            }
            System.Reflection.FieldInfo fi = typeof(AuthorizationService).GetField("envPermissionMaps", BindingFlags.NonPublic | BindingFlags.Static);
            fi.SetValue(null, envPermissionMaps);

            List<PermissionGetReplyDC> permissionList = GetPermissionList();
            FieldInfo fi2 = typeof(AuthorizationService).GetField("permissionList", BindingFlags.NonPublic | BindingFlags.Static);
            fi2.SetValue(null, permissionList);
        }

        private static List<PermissionGetReplyDC> GetPermissionList()
        {
            return new List<PermissionGetReplyDC>() {
                new PermissionGetReplyDC() { 
                    AuthorGroupName = "cwf_eng_wsp", 
                    EnvironmentName = Env.Dev.ToString(), 
                    Permission = (long)(Permission.OpenWorkflow | Permission.SaveWorkflow)
                },
                new PermissionGetReplyDC() { 
                    AuthorGroupName = "cwf_eng_wsp", 
                    EnvironmentName = Env.Test.ToString(), 
                    Permission = (long)(Permission.OpenWorkflow | Permission.SaveWorkflow)
                },
                new PermissionGetReplyDC() { 
                    AuthorGroupName = "cwf_eng_wsp", 
                    EnvironmentName = Env.Stage.ToString(), 
                    Permission = (long)(Permission.OpenWorkflow)
                },
                new PermissionGetReplyDC() { 
                    AuthorGroupName = "cwf_eng_wsp", 
                    EnvironmentName = Env.Prod.ToString(), 
                    Permission = (long)(Permission.OpenWorkflow)
                },
            };
        }

        private static Dictionary<Env, Permission> RegistViewerPermission()
        {
            Dictionary<Env, Permission> envPermissionMaps = new Dictionary<Env, Permission>();
            envPermissionMaps.Add(Env.Dev, Permission.OpenWorkflow | Permission.ViewMarketplace);
            envPermissionMaps.Add(Env.Test, Permission.OpenWorkflow | Permission.ViewMarketplace);
            envPermissionMaps.Add(Env.Stage, Permission.OpenWorkflow | Permission.ViewMarketplace);
            envPermissionMaps.Add(Env.Prod, Permission.OpenWorkflow | Permission.ViewMarketplace);
            return envPermissionMaps;
        }

        public static Dictionary<Env, Permission> RegistAuthorPermission()
        {
            Dictionary<Env, Permission> envPermissionMaps = new Dictionary<Env, Permission>();
            envPermissionMaps.Add(Env.Dev, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.SaveWorkflow | Permission.CopyWorkflow |
                            Permission.UploadAssemblyToMarketplace | Permission.UploadProjectToMarketplace | Permission.CompileWorkflow | Permission.PublishWorkflow | Permission.CreateTask);
            envPermissionMaps.Add(Env.Test, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.SaveWorkflow | Permission.CopyWorkflow |
                Permission.UploadAssemblyToMarketplace | Permission.UploadProjectToMarketplace | Permission.CompileWorkflow | Permission.PublishWorkflow | Permission.CreateTask);
            envPermissionMaps.Add(Env.Stage, Permission.OpenWorkflow | Permission.ViewMarketplace);
            envPermissionMaps.Add(Env.Prod, Permission.OpenWorkflow | Permission.ViewMarketplace);
            return envPermissionMaps;
        }


        private static Dictionary<Env, Permission> RegistAdminPermission()
        {
            Dictionary<Env, Permission> envPermissionMaps = new Dictionary<Env, Permission>();
            envPermissionMaps.Add(Env.Dev, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.SaveWorkflow | Permission.OverrideLock | Permission.CopyWorkflow |
                             Permission.UploadProjectToMarketplace | Permission.UploadAssemblyToMarketplace | Permission.CompileWorkflow | Permission.PublishWorkflow | Permission.CreateTask
                             | Permission.ManageWorkflowType | Permission.DeleteWorkflow | Permission.ChangeWorkflowAuthor | Permission.MoveWorkflow);

            envPermissionMaps.Add(Env.Test, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.SaveWorkflow | Permission.OverrideLock | Permission.CopyWorkflow |
            Permission.UploadProjectToMarketplace | Permission.UploadAssemblyToMarketplace | Permission.CompileWorkflow | Permission.PublishWorkflow | Permission.CreateTask
            | Permission.ManageWorkflowType | Permission.DeleteWorkflow | Permission.ChangeWorkflowAuthor | Permission.MoveWorkflow);

            envPermissionMaps.Add(Env.Stage, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.CopyWorkflow |
             Permission.CompileWorkflow | Permission.PublishWorkflow
            | Permission.ManageWorkflowType | Permission.DeleteWorkflow | Permission.MoveWorkflow);

            envPermissionMaps.Add(Env.Prod, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.CopyWorkflow |
             Permission.CompileWorkflow | Permission.PublishWorkflow |
             Permission.ManageWorkflowType | Permission.MoveWorkflow);
            return envPermissionMaps;
        }

        private static Dictionary<Env, Permission> RegistEnAdminPermission()
        {
            Dictionary<Env, Permission> envPermissionMaps = new Dictionary<Env, Permission>();
            envPermissionMaps.Add(Env.Dev, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.MoveWorkflow | Permission.ManageRoles);
            envPermissionMaps.Add(Env.Test, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.MoveWorkflow | Permission.ManageRoles);
            envPermissionMaps.Add(Env.Stage, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.MoveWorkflow | Permission.ManageRoles);
            envPermissionMaps.Add(Env.Prod, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.MoveWorkflow | Permission.ManageRoles);
            return envPermissionMaps;
        }

        private static Dictionary<Env, Permission> RegistCWFAdminPermission()
        {
            Dictionary<Env, Permission> envPermissionMaps = new Dictionary<Env, Permission>();
            envPermissionMaps.Add(Env.Dev, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.DeleteWorkflow | Permission.ManageEnvAdmin);
            envPermissionMaps.Add(Env.Test, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.DeleteWorkflow | Permission.ManageEnvAdmin);
            envPermissionMaps.Add(Env.Stage, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.DeleteWorkflow | Permission.ManageEnvAdmin);
            envPermissionMaps.Add(Env.Prod, Permission.OpenWorkflow | Permission.ViewMarketplace | Permission.DeleteWorkflow | Permission.ManageEnvAdmin);
            return envPermissionMaps;
        }

        public static void MockMessageBoxServiceOfCommonOperate(Action action)
        {
            using (var messageBox = new ImplementationOfType(typeof(MessageBoxService)))
            {
                messageBox.Register(() => MessageBoxService.DownloadNewActivityOnSaving())
         .Execute<MessageBoxResult>(() => { return MessageBoxResult.Yes; });
                messageBox.Register(() => MessageBoxService.CreateNewActivityOnSaving())
                 .Execute<MessageBoxResult>(() => { return MessageBoxResult.Yes; });
                messageBox.Register(() => MessageBoxService.ShowClosingConfirmation(Argument<string>.Any)).Return(SavingResult.DoNothing);
                messageBox.Register(() => MessageBoxService.ShowKeepLockedConfirmation(Argument<string>.Any)).Return(SavingResult.DoNothing);
                messageBox.Register(() => MessageBoxService.Show(Argument<string>.Any,
                           Argument<string>.Any,
                           MessageBoxButton.YesNo,
                           MessageBoxImage.Question)).Return(MessageBoxResult.Yes);
                action();
            }
        }

        public static void RegistMessageBoxServiceOfCommonOperate()
        {
            string message;
            //MessageBoxService.OpenActivity,Show
            MessageBoxService.ShowOpenActivityConfirmationFunc = (msg, caption) =>
            {
                message = msg;
                return MessageBoxResult.Yes;
            };

            //MessageBoxService.CreateNewActivityOnSaving,DownloadNewActivityOnSaving
            MessageBoxService.ShowFunc = ((msg, caption, button, image, result) =>
            {
                message = msg;
                return MessageBoxResult.Yes;
            });
            //ShowClosingConfirmation
            MessageBoxService.ShowSavingConfirmationFunc = ((msg, caption, canKeepLocked, shouldUnlock, unlockVisibility) =>
                {
                    message = msg;
                    return SavingResult.DoNothing;
                });
            //ShowKeepLockedConfirmation
            //MessageBoxService.ShowSavingConfirmationFunc = ((msg, caption, canKeepLocked, shouldUnlock, unlockVisibility) =>
            //{
            //    message = msg;
            //    return SavingResult.DoNothing;
            //});

            // MessageBoxService.ShowFunc = ((msg, caption, button, icon, defaultResult) => MessageBox.Show(msg, caption, button, icon, defaultResult));
        }

        public static void MockAssetStoreProxy(ref ImplementationOfType imp, Action action)
        {
            using (imp = new ImplementationOfType(typeof(AssetStoreProxy)))
            {
                imp.Register(() => AssetStoreProxy.GetActivityCategories()).Return(true);
                imp.Register(() => AssetStoreProxy.GetWorkflowTypes(Argument<Env>.Any))
                    .Return(true);
                imp.Register(() => AssetStoreProxy.ActivityCategoryCreateOrUpdate(Argument<string>.Any)).Return(true);
                imp.Register(() => AssetStoreProxy.ActivityCategories).Return(new CollectionViewSource());
                imp.Register(() => AssetStoreProxy.GetTenantName()).Execute(() => { });
                imp.Register(() => AssetStoreProxy.GetClientEndpoint()).Execute(() => { });
                action();
            }
        }

        public static void RegistCreateIntellisenseList()
        {
            ExpressionEditorHelper.GetReferencesFunc = (() =>
            {
                return null;
            });
        }

        public static void MockDialogService(string workFlowItemName, Action action)
        {
            using (var prin = new ImplementationOfType(typeof(DialogService)))
            {
                prin.Register(() => DialogService.ShowSaveDialogAndReturnResult(Argument<string>.Any, Argument<string>.Any))
                    .Execute<string>(() =>
                    {
                        return workFlowItemName + ".wf";
                    });
                prin.Register(() => DialogService.ShowOpenFileDialogAndReturnResult(Argument<string>.Any, Argument<string>.Any)).Execute<string>(() =>
               {
                   return workFlowItemName + ".wf";
               });
                action();
            }
        }


        //mock assembly reflection from assembly 
        public static void MockAssemblyReflection(Assembly assembly, Action action)
        {
            using (var mock = new ImplementationOfType(typeof(Assembly)))
            {
                mock.Register(() => Assembly.ReflectionOnlyLoadFrom(assembly.Location)).Execute<string, Assembly>((s) =>
                {
                    return assembly;
                });
                action();
            }
        }

        public static CompileProject CreateCompileProject(string wfName, Activity activity)
        {
            ActivityBuilder builder = new ActivityBuilder();
            builder.Name = wfName;
            builder.Implementation = activity;

            WorkflowDesigner designer = new WorkflowDesigner();
            designer.Load(builder);

            HashSet<Type> referencedTypes = DependencyAnalysisService.GetReferencedTypes(designer);
            HashSet<Assembly> referencedAssemblies = DependencyAnalysisService.GetReferencedAssemblies(designer, referencedTypes);
            CompileProject compileProject = new CompileProject();

            compileProject.ReferencedAssemblies = referencedAssemblies;
            compileProject.ReferencedTypes = referencedTypes;

            compileProject.ProjectName = wfName;
            compileProject.ProjectVersion = "1.0.0.0";
            compileProject.ProjectXaml = designer.CompilableXaml();
            return compileProject;
        }

        public static CompileProject CreateCompileProject(string wfName, string xamlCode)
        {
            //ActivityBuilder builder = new ActivityBuilder();
            //builder.Name = wfName;
            //builder.Implementation = activity;

            WorkflowDesigner designer = new WorkflowDesigner();
            designer.Load(xamlCode);

            HashSet<Type> referencedTypes = DependencyAnalysisService.GetReferencedTypes(designer);
            HashSet<Assembly> referencedAssemblies = DependencyAnalysisService.GetReferencedAssemblies(designer, referencedTypes);
            CompileProject compileProject = new CompileProject();

            compileProject.ReferencedAssemblies = referencedAssemblies;
            compileProject.ReferencedTypes = referencedTypes;

            compileProject.ProjectName = wfName;
            compileProject.ProjectVersion = "1.0.0.0";
            compileProject.ProjectXaml = designer.CompilableXaml();
            return compileProject;
        }

        public static string CreateCompositeWorkFlowTemplateXamlCode(string workflowName)
        {
            string xmalCode = @"<Activity x:Class='[ClassName]' sap:VirtualizedContainerService.HintSize='240,240' 
                    mva:VisualBasic.Settings='Assembly references and imported namespaces for internal implementation' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/activities' xmlns:local='clr-namespace:;assembly=test001' xmlns:mswac='clr-namespace:Microsoft.Support.Workflow.Authoring.CompositeActivity;assembly=Microsoft.Support.Workflow.Authoring' xmlns:mv='clr-namespace:Microsoft.VisualBasic;assembly=System' xmlns:mva='clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities' xmlns:s='clr-namespace:System;assembly=mscorlib' xmlns:s1='clr-namespace:System;assembly=System' xmlns:s2='clr-namespace:System;assembly=System.Xml' xmlns:s3='clr-namespace:System;assembly=System.Core' xmlns:sad='clr-namespace:System.Activities.Debugger;assembly=System.Activities' xmlns:sap='http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation' xmlns:scg='clr-namespace:System.Collections.Generic;assembly=System' xmlns:scg1='clr-namespace:System.Collections.Generic;assembly=System.ServiceModel' xmlns:scg2='clr-namespace:System.Collections.Generic;assembly=System.Core' xmlns:scg3='clr-namespace:System.Collections.Generic;assembly=mscorlib' xmlns:sd='clr-namespace:System.Data;assembly=System.Data' xmlns:sl='clr-namespace:System.Linq;assembly=System.Core' xmlns:st='clr-namespace:System.Text;assembly=mscorlib' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                      <Sequence sap:VirtualizedContainerService.HintSize='200,200'>
                        <sap:WorkflowViewStateService.ViewState>
                          <scg3:Dictionary x:TypeArguments='x:String, x:Object'>
                            <x:Boolean x:Key='IsExpanded'>True</x:Boolean>
                          </scg3:Dictionary>
                        </sap:WorkflowViewStateService.ViewState>
                        <local:test001 />
                      </Sequence>
                    </Activity>";
            xmalCode = xmalCode.Replace("[ClassName]", workflowName);
            return xmalCode;
        }

        public static string CreateCompositeWorkFlowWithMultiActivityXamlCode(string workflowName)
        {
            string xmalCode = @"<Activity x:Class='[ClassName]' sap:VirtualizedContainerService.HintSize='240,240' 
                    mva:VisualBasic.Settings='Assembly references and imported namespaces for internal implementation' xmlns='http://schemas.microsoft.com/netfx/2009/xaml/activities' xmlns:local='clr-namespace:;assembly=test001' xmlns:mswac='clr-namespace:Microsoft.Support.Workflow.Authoring.CompositeActivity;assembly=Microsoft.Support.Workflow.Authoring' xmlns:mv='clr-namespace:Microsoft.VisualBasic;assembly=System' xmlns:mva='clr-namespace:Microsoft.VisualBasic.Activities;assembly=System.Activities' xmlns:s='clr-namespace:System;assembly=mscorlib' xmlns:s1='clr-namespace:System;assembly=System' xmlns:s2='clr-namespace:System;assembly=System.Xml' xmlns:s3='clr-namespace:System;assembly=System.Core' xmlns:sad='clr-namespace:System.Activities.Debugger;assembly=System.Activities' xmlns:sap='http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation' xmlns:scg='clr-namespace:System.Collections.Generic;assembly=System' xmlns:scg1='clr-namespace:System.Collections.Generic;assembly=System.ServiceModel' xmlns:scg2='clr-namespace:System.Collections.Generic;assembly=System.Core' xmlns:scg3='clr-namespace:System.Collections.Generic;assembly=mscorlib' xmlns:sd='clr-namespace:System.Data;assembly=System.Data' xmlns:sl='clr-namespace:System.Linq;assembly=System.Core' xmlns:st='clr-namespace:System.Text;assembly=mscorlib' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                      <Sequence sap:VirtualizedContainerService.HintSize='200,200'>
                        <sap:WorkflowViewStateService.ViewState>
                          <scg3:Dictionary x:TypeArguments='x:String, x:Object'>
                            <x:Boolean x:Key='IsExpanded'>True</x:Boolean>
                          </scg3:Dictionary>
                        </sap:WorkflowViewStateService.ViewState>
                        <local:test001 />
                        <local:test001 />
                      </Sequence>
                    </Activity>";
            xmalCode = xmalCode.Replace("[ClassName]", workflowName);
            return xmalCode;
        }

        public static void Loadtest001Assembly()
        {
            string filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "testData", "test001.dll");
            //Assembly test001_assembly = Assembly.LoadFrom(@"testData\test001.dll");
            Assembly test001_assembly = Assembly.LoadFrom(filePath);
        }

        public static int CountStringAContainsStringB(string a, string b)
        {
            int count = 0;
            int i = 0;
            while (a.IndexOf(b, i) >= 0)
            {
                i = a.IndexOf(b, i) + 1;
                count++;
            }
            return count;
        }

        public static string ReplacefirstMatch(string a, string b, string c)
        {
            string anew = string.Empty;
            if (a.Contains(b))
            {
                int index = a.IndexOf(b);
                anew = a.Substring(0, index) + c + a.Substring(index + b.Length);
            }
            return anew;
        }

        public static void ResetWorkflowsQueryServiceClientAfterMock()
        {
            WorkflowsQueryServiceUtility.GetWorkflowQueryServiceClient = () => { return new WorkflowsQueryServiceClient(); };
        }

        public static string RemovefirstMatch(string a, string b)
        {
            string anew = string.Empty;
            if (a.Contains(b))
            {
                int index = a.IndexOf(b);
                anew = a.Substring(0, index) + a.Substring(index + b.Length);
            }
            return anew;
        }

        public static WorkflowDesigner CreateWorkflowDesigner(Activity activity)
        {
            WorkflowDesigner wfDesigner = new WorkflowDesigner();

            ActivityBuilder activityBuilder = new ActivityBuilder();
            activityBuilder.Implementation = activity;

            wfDesigner.Load(activityBuilder);

            return wfDesigner;
        }

    }
}
