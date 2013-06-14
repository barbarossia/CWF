namespace Microsoft.Support.Workflow.Authoring.AddIns.Models
{
    using System;
    using System.Activities.Presentation.Services;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Project for compiling workflows into assemblies for the application.
    /// </summary>
    [Serializable]
    public class CompileProject
    {        
        /// <summary>
        /// Assemblies referenced in the project.
        /// </summary>
        public HashSet<Assembly> ReferencedAssemblies { get; set; }
        /// <summary>
        /// Types referenced in the project.
        /// </summary>
        public HashSet<Type> ReferencedTypes { get; set; }
        /// <summary>
        /// Name of the project to compile (Typically a class name)
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// Version of the project.
        /// </summary>
        public string ProjectVersion { get; set; }
        /// <summary>
        /// Xaml of the workflow to be compiled.
        /// </summary>
        public string ProjectXaml { get; set; }
        /// <summary>
        /// Allows interaction with the model of the workflow.
        /// </summary>
        //public ModelService ProjectModelService { get; set; }


        public CompileProject()
        {
        }
        /// <summary>
        /// Default constructor.
        /// </summary>
        ///<param name="name">Name of the project</param>
        /// <param name="version">Version of the project</param>
        /// <param name="xaml">Compilable xaml of the project</param>
        /// <param name="modelService">ModelService of the workflow.</param>
        public CompileProject(string name, string version, string xaml) 
            :base()
        {
            Version projectVersion;

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(CompileMessages.ProjectNameNull);
            }

            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException(CompileMessages.ProjectVersionNull);
            }

            if (string.IsNullOrEmpty(xaml))
            {
                throw new ArgumentNullException(CompileMessages.ProjectXamlNull);
            }
            if (!Version.TryParse(version, out projectVersion))
            {
                throw new ArgumentException(CompileMessages.ProjectVersionInvalid);
            }

            ProjectName = name;
            ProjectVersion = version;
            ProjectXaml = xaml;
            ReferencedAssemblies = new HashSet<Assembly>();
            ReferencedTypes = new HashSet<Type>();
        }
    }
}

