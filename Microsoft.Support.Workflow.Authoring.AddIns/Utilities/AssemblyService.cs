using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
    public static class AssemblyService
    {
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
                    "wpfgfx_v0400",
                    //Task Activity
                    "Microsoft.Support.Workflow.Authoring.AddIns"
                };
            }
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
            var builtInTokens = new List<string> { "31bf3856ad364e35", "b77a5c561934e089", "b03f5f7f11d50a3a", "null"};

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
                result = LoadCachedAssembly(cache ?? AddInCaching.ActivityAssemblyItems, reqAssemblyName, out aai) ? Assembly.LoadFrom(aai.Location) : null;
            }

            // TODO: Conditionally load the assembly from the Repository

            return result;
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
