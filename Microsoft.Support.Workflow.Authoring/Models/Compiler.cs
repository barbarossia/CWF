

namespace Microsoft.Support.Workflow.Authoring.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Build.Evaluation;
    using Build.Execution;
    using Microsoft.Support.Workflow.Authoring.AddIns.Models;
    using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
    using Services;
    using TextResources = Microsoft.Support.Workflow.Authoring.AddIns.Properties.Resources;
    /// <summary>
    /// Compiles projects producing assembly files based on a sepecified workflow.
    /// </summary>
    public static class Compiler
    {
        /// <summary>
        /// Name of the key file for signing the assembly
        /// </summary>
        private const string PrivateKeyName = "PrivateKeyForCompiledAssemblySigning.snk";
        /// <summary>
        /// Full name of the key file for signing the assembly
        /// </summary>
        private const string PrivateKeyFullName =
            "Microsoft.Support.Workflow.Authoring.Resources.PrivateKeyForCompiledAssemblySigning.snk";

        /// <summary>
        /// Compiles the project and produces an output assembly
        /// </summary>
        /// <returns>
        /// Full name of the output assembly file.
        /// </returns>
        public static CompileResult Compile(CompileProject project)
        {
            string outputDirectory = string.Empty;
            if (project == null)
            {
                throw new ArgumentNullException(CompileMessages.ProjectNull);
            }

            try
            {
                //Create output directory for the compile project necessary files
                outputDirectory = CreateBuildDirectory();

                //Create necessary files for MSBuild
                CreateProjectFiles(project, outputDirectory);

                // Once all the temporary files are created, we can call MsBuild to compile.
                // NOTE: If you want to dynamic build, the Targect Framework of this project MUST be .NET Framework 4. 
                // MUST NOT be the Client Profile, or you cannot reference Microsoft.Build.dll
                using (var projectPropertiesCollection = new ProjectCollection())
                {
                    var globalProperty = new Dictionary<string, string>();
                    var buildRequest = new BuildRequestData(
                        Path.Combine(outputDirectory, project.ProjectName) + ".csproj", globalProperty, null, new[] { "Build" }, null);

                    var buildResult =
                        BuildManager.DefaultBuildManager.Build(new BuildParameters(projectPropertiesCollection),
                                                               buildRequest);

                    if (buildResult == null)
                    {
                        return new CompileResult(BuildResultCode.Failure, outputDirectory, null);
                    }
                    switch (buildResult.OverallResult)
                    {
                        case BuildResultCode.Success:
                            var filePath = Path.Combine(outputDirectory, project.ProjectName) + ".dll";
                            var result = new CompileResult(buildResult.OverallResult, filePath, null);

                            try
                            {
                                File.Delete(Path.Combine(outputDirectory, PrivateKeyName));
                            }
                            catch (Exception) //Failure to delete key doesn't affect the result of the operation
                            {
                            }
                            return result;

                        default:
                            throw new CompileException(CompileMessages.ErrorCompiling, buildResult.Exception);
                    }
                }
            }
            catch (CompileException ex)
            {
                var result = new CompileResult(BuildResultCode.Failure, outputDirectory, ex);
                return result;
            }
            catch (AggregateException ex)
            {
                //More than one exception occured in one of the compile steps
                var result = new CompileResult(BuildResultCode.Failure, outputDirectory, ex);
                return result;
            }
            catch (ArgumentException ex)
            {
                var result = new CompileResult(BuildResultCode.Failure, outputDirectory, ex);
                return result;
            }
            catch (InvalidOperationException ex)
            {
                var result = new CompileResult(BuildResultCode.Failure, outputDirectory, ex);
                return result;
            }
        }

        public static void AddToCaching(string assemblyFilePath)
        {
            AssemblyInspectionService.CheckAssemblyPath(assemblyFilePath);
            var assemblyItem = new ActivityAssemblyItem(AssemblyName.GetAssemblyName(assemblyFilePath)) 
            { 
                Location = assemblyFilePath,
                Category = TextResources.Unassigned,
            };

            var inspection = Utility.GetAssemblyInspectionService();
            if (!inspection.Inspect(assemblyFilePath))
            {
                throw new UserFacingException(inspection.OperationException.Message, inspection.OperationException);
            }

            assemblyItem.ReferencedAssemblies = new ObservableCollection<AssemblyName>(inspection.ReferencedAssemblies.Select(r => r.AssemblyName));
            assemblyItem.ActivityItems = inspection.SourceAssembly.ActivityItems;
            assemblyItem.UserSelected = true;
            assemblyItem.ActivityItems.ToList().ForEach(i =>
            {
                i.UserSelected = true;
                i.Category = TextResources.Unassigned;
            });

            Caching.CacheAssembly(new [] { assemblyItem }.ToList());
            Caching.Refresh();
        }

        /// <summary>
        /// Create a temporary directory for the files generated for compiling
        /// </summary>
        ///  <exception cref="CompileException"></exception>
        private static string CreateBuildDirectory()
        {
            try
            {
                //Get the directory the authoring tool is running from, and then create a temporary directory in "Output" for the compile.
                string buildPath = Path.Combine(@".", "Output");
                buildPath = Path.GetFullPath(Path.Combine(buildPath, Guid.NewGuid().ToString()));

                Directory.CreateDirectory(buildPath);
                return buildPath;
            }
            catch (PathTooLongException ex)
            {
                //The file path is too long for the OS.
                throw new CompileException(CompileMessages.ProjectFileNameTooLong, ex);
            }
            catch (IOException ex)
            {
                //The directory specified by path is a file or the network name is not known or invalid.
                throw new CompileException(CompileMessages.ErrorCreatingOutputDirectory, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                //The caller does not have the required permission.
                throw new CompileException(CompileMessages.ErrorCreatingOutputDirectory, ex);
            }
            catch (NotSupportedException ex)
            {
                //path contains a colon character (:) that is not part of a drive label ("C:\").
                throw new CompileException(CompileMessages.ErrorCreatingOutputDirectory, ex.InnerException);
            }
            catch (ArgumentException ex)
            {
                //Path is null
                throw new CompileException(CompileMessages.ErrorCreatingOutputDirectory, ex);

            }
            catch (SecurityException ex)
            {
                //The caller does not have the required permission.
                throw new CompileException(CompileMessages.ErrorCreatingOutputDirectory, ex);
            }
        }


        /// <summary>
        /// Creates all the necessary files for building an assembly based on a workflow.
        /// </summary>
        /// <param name="project">Project for the compile operation</param>
        /// <param name="path">Path of the output directory</param>
        private static void CreateProjectFiles(CompileProject project, string path)
        {
            if (null == project)
            {
                throw new ArgumentNullException(CompileMessages.ProjectNull);
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(CompileMessages.OutputDirectoryNull);
            }

            try
            {
                //Generate project files
                Parallel.Invoke(() => GenerateVisualStudioProjectFile(project, path),
                                () => GenerateAssemblyInfoFile(project.ProjectName, path, project.ProjectVersion),
                                () => GeneratePrivateKeyFile(path),
                                () => GenerateXamlFile(project.ProjectName, path, project.ProjectXaml));

            }
            catch (PathTooLongException ex)
            {
                //The file path is too long for the OS.
                throw new CompileException(CompileMessages.ProjectFileNameTooLong, ex);
            }
            catch (IOException ex)
            {
                //An I/O error occurred while opening the file.
                throw new CompileException(CompileMessages.ErrorCreatingFiles, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                //path specified a file that is read-only or this operation is not supported on the current platform or
                //path specified a directory or the caller does not have the required permission.
                throw new CompileException(CompileMessages.ErrorCreatingFiles, ex);
            }
            catch (NotSupportedException ex)
            {
                //path is in an invalid format.
                throw new CompileException(CompileMessages.ErrorCreatingFiles, ex);
            }
            catch (SecurityException ex)
            {
                //The caller does not have the required permission.
                throw new CompileException(CompileMessages.ErrorCreatingFiles, ex);
            }
            catch (AggregateException ex)
            {
                //The caller does not have the required permission.
                throw new CompileException(CompileMessages.ErrorCreatingFiles, ex);
            }
            catch (OutOfMemoryException ex)
            {
                //Not enough memory for creating files or directories
                throw new CompileException(CompileMessages.ErrorCreatingFiles, ex);
            }
        }


        /// <summary>
        /// Creates a Visual Studio project file (.csproj) based on a workflow, including its dependencies,
        /// to use later in MSBuild
        /// </summary>
        /// <param name="project">Project for the compile operation</param>
        /// <param name="path">Path of the output directory</param>
        private static void GenerateVisualStudioProjectFile(CompileProject project, string path)
        {
            if (project == null)
            {
                throw new ArgumentNullException(CompileMessages.ProjectNull);
            }

            if (path == null)
            {
                throw new ArgumentNullException(CompileMessages.OutputDirectoryNull);
            }

            //We will concatenate several strings for all the content of the project, so we use StringBuilder.
            var projectFileStringBuilder = new StringBuilder();
            var referencesStringBuilder = new StringBuilder();
            const string referenceFormat = "<Reference Include=\"{0}\"><HintPath>{1}</HintPath></Reference>{2}";

            //Get referenced assemblies. We filter dynamic references because they will be resolved automatically by the compiler using reflection,
            //so there is no need to include them in the project file.
            GetReferencedAssemblies(project);

            foreach (Assembly asm in project.ReferencedAssemblies.Where(assembly => !assembly.IsDynamic))
            {
                // Build a Reference Include line for each referenced assembly
                referencesStringBuilder.Append(string.Format(referenceFormat, asm.FullName, asm.Location, Environment.NewLine));
            }

            projectFileStringBuilder.Append(Properties.Resources.WorkflowProjectTemplate);
            projectFileStringBuilder.Replace("[ASSEMBLYNAME]", project.ProjectName);
            projectFileStringBuilder.Replace("[REFERENCE]", referencesStringBuilder.ToString());
            projectFileStringBuilder.Replace("[XAMLFILE]", project.ProjectName + ".xaml");
            projectFileStringBuilder.Replace("[PRODUCTVERSION]", project.ProjectVersion);

            //Create Visual Studio Project File
            File.WriteAllText(path + Path.DirectorySeparatorChar + project.ProjectName + ".csproj", projectFileStringBuilder.ToString());
        }

        /// <summary>
        /// Generates AssemblyInfo.cs file
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="path">Path of the output directory</param>
        /// <param name="version">Version of the project</param>
        private static void GenerateAssemblyInfoFile(string projectName, string path, string version)
        {
            if (String.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException(CompileMessages.ProjectNameNull);
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(CompileMessages.OutputDirectoryNull);
            }

            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException(CompileMessages.ProjectVersionNull);
            }

            Version projectVersion;
            if (!Version.TryParse(version, out projectVersion))
            {
                throw new ArgumentException(CompileMessages.ProjectVersionInvalid);
            }

            var assemblyInfoBuilder = new StringBuilder();
            assemblyInfoBuilder.Append(Properties.Resources.AssemblyInfoTemplate);
            assemblyInfoBuilder.Replace("[ASSEMBLYNAME]", projectName);
            assemblyInfoBuilder.Replace("[VERSION]", version);

            //Create AssemblyInfo file
            //Any exception here will be handled by the CreateProjectFiles method
            File.WriteAllText(path + Path.DirectorySeparatorChar + "AssemblyInfo.cs", assemblyInfoBuilder.ToString());
        }

        /// <summary>
        /// Create a temporary private key for signing the assembly
        /// </summary>
        /// <param name="path">Path of the output directory</param>
        private static void GeneratePrivateKeyFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(CompileMessages.OutputDirectoryNull);
            }

            // Create a temporary private key for signing the assembly
            string privateKeyPath = Path.Combine(path, PrivateKeyName);
            using (Stream privateKeyStream =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(PrivateKeyFullName))
            {
                if (privateKeyStream != null)
                {
                    var privateKeyBytes = new byte[privateKeyStream.Length];
                    privateKeyStream.Read(privateKeyBytes, 0, privateKeyBytes.Length);

                    File.WriteAllBytes(privateKeyPath, privateKeyBytes);
                }
            }
        }

        /// <summary>
        /// Creates Xaml file that contains the workflow xaml for compiling.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="path">Path of the output directory</param>
        /// <param name="xaml">Compilable Xaml code of the project</param>
        private static void GenerateXamlFile(string projectName, string path, string xaml)
        {
            if (String.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException(CompileMessages.ProjectNameNull);
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(CompileMessages.OutputDirectoryNull);
            }

            if (string.IsNullOrEmpty(xaml))
            {
                throw new ArgumentNullException(CompileMessages.ProjectXamlNull);
            }

            //Create Xaml File
            File.WriteAllText(path + Path.DirectorySeparatorChar + projectName + ".xaml", xaml);
        }

        /// <summary>
        /// Attempt to warn the user if he is relying on unsigned assemblies in his workflow (an unsupported scenario).
        /// There are cases we won't catch though that the C# compiler will, involving generics like 
        /// Variable of List of List of TypeFromUnsignedAssembly
        /// </summary>
        /// <param name="project">Project for the compile operation</param>
        private static void GetReferencedAssemblies(CompileProject project)
        {
            if (project == null)
            {
                throw new ArgumentNullException(CompileMessages.ProjectNull);
            }

            //Check for conflicts, e.g. references to the same type with two different versions.
            CheckForConflictingVersions(project.ReferencedTypes, project.ReferencedAssemblies, project.ProjectName);

            // Get the references that are not signed. For some purposes like security the tool only acepts signed assemblies.
            var unsigned = (from assemblyItem in project.ReferencedAssemblies
                            where (assemblyItem.GetName().GetPublicKey().IfNotNull(key => key.Length) == 0)
                            select assemblyItem.FullName).ToList();

            if (unsigned.Any())
            {
                string message = string.Format(CompileMessages.UnsignedAssemblies, project.ProjectName,
                                               string.Join(Environment.NewLine, unsigned));

                throw new CompileException(message);
            }
        }

        /// <summary>
        /// Check for conflicts of versions in the referenced assemblies of the workflow.
        /// </summary>
        /// <param name="referencedTypes">Referenced types in the project</param>
        /// <param name="referencedAssemblies">Referenced assemblies in the project</param>
        /// <param name="projectName">Name of the main type (workflow) of the project</param>
        private static void CheckForConflictingVersions(HashSet<Type> referencedTypes, HashSet<Assembly> referencedAssemblies, string projectName)
        {
            // XamlBuildTask cannot support two different versions of the same dependency in XAML. 
            // As a workaround, we raise an error here if the workflow contains activities/variables/etc.
            // from different versions of the same assembly.
            var conflicts =
                referencedAssemblies.GroupBy(asm => asm.GetName().Name).Where(grp => grp.Count() > 1).ToList();


            if (conflicts.Any())
            {
                var conflict = conflicts.First();
                Assembly asm1 = referencedAssemblies.First(item => item.GetName().Name == conflict.Key);
                Assembly asm2 = referencedAssemblies.Last(item => item.GetName().Name == conflict.Key);


                var type1 = referencedTypes.First(item => item.Assembly.GetName().Name == asm1.GetName().Name &&
                    item.Assembly.GetName().Version == asm1.GetName().Version);
                var type2 = referencedTypes.First(item => item.Assembly.GetName().Name == asm2.GetName().Name &&
                    item.Assembly.GetName().Version == asm2.GetName().Version);


                string message = string.Format(CompileMessages.MultipleVersionsUsed,
                        type1.Name, asm1.FullName, type2.Name, asm2.FullName, conflict.Key);
                throw new CompileException(message);
            }

            // Check if the workflow contains a previous version of itself
            var referencesToItself = new List<Assembly>(
                                      from assemblies in referencedAssemblies
                                      where assemblies.GetName().Name == projectName
                                      select assemblies);

            if (referencesToItself.Any())
            {
                string message = string.Format(CompileMessages.PreviousSelfVersion,
                                               referencesToItself.First().GetName().Name,
                                               referencesToItself.First().GetName().FullName);
                throw new CompileException(message);
            }
        }
    }
}
