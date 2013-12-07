using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Reflection;
using Microsoft.Support.Workflow.CWFHelpers;
using System.Diagnostics;
using System.Security.Principal;
using Microsoft.Support.Workflow.Catalog;

namespace Microsoft.Support.Workflow
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWorkflows" in both code and config file together.
    [ServiceContract]
    public interface IWorkflows
    {
        [OperationContract]
        IList<WFActivities> garbage();
        //[OperationContract]
        //WorkflowStoreClass GetWorkflow1();
        //[OperationContract]
        //void SaveWorkflow1(WorkflowStoreClass aWorkflow);
        [OperationContract]
        Store_Workflow GetWorkflowFromGuid(Guid WorkflowID);
        [OperationContract]
        Store_Workflow GetWorkflowFromNameAndVersion(string WorkflowName, string WorkflowVersion);
        [OperationContract]
        void SaveWorkFlow(Microsoft.Support.Workflow.Store_Workflow aWorkflow);
        [OperationContract]
        IList<Store_Workflow> GetAllWorkFlows();
        [OperationContract]
        IList<WFApplication> GetAllApplications();
        [OperationContract]
        IList<Workflow_Activity_Type> GetWFTypes();
        [OperationContract]
        Byte[] GetActivityAssembly(string AssemblyName);
        [OperationContract]
        Byte[] GetActivityAssemblyFromNameAndVersion(string AssemblyName, string AssemblyVersion);
        [OperationContract]
        Byte[] GetActivityAssemblyFromID(Guid AssemblyID);
        [OperationContract]
        Guid SaveNewActivityAssembly(LibraryWithExecutable aLibrary);
        [OperationContract]
        bool SaveActivity(LibraryWithExecutable aLibrary);
        [OperationContract]
        IList<xxContextItem> GetContextList(string Category, string Tags);
        [OperationContract]
        IList<xxContextCategory> GetContextCategoryList();
        //[OperationContract]
        //Store_Activity test();

        [OperationContract]
        IList<LibraryWithActivities> GetLibraryListWithOutExecutable();

        [OperationContract]
        LibraryWithExecutable GetLibraryFromId(Guid id);
        //
        // Get the library that has the latest build number for the 
        // requested library name and major + minor version number
        [OperationContract]
        LibraryWithExecutable GetLibrary_LatestBuild(string Name, String Version);

        [OperationContract]
        WFContent GetContentByID(Guid id);

        [OperationContract]
        WFTaxonomy GetTaxonomyById(Guid id);

        #region New version
        [OperationContract]
        bool WriteToServerEventLog(string message, EventLogEntryType entryType);

        [OperationContract]
        IList<Activity_Category> ActivityCategoryGetList();

        [OperationContract]
        bool CheckForService(string UserId);

        [OperationContract]
        IList<ToolboxItem> ToolboxItemGetList(VersionFlag versionFlag);

        [OperationContract]
        IList<Library> LibraryGetAll();

        [OperationContract]
        IList<Library> LibraryGetList(string category, string keywords, VersionFlag versionFlag, bool HasActivitiesOnly);

        [OperationContract]
        IList<Library> LibraryGetListByName(string Name);
        //201025 - v-evsmit, changed return value to EditReturnValue
        /// <summary>
        /// 
        /// </summary>
        /// <param name="library"></param>
        /// <returns></returns>
        [OperationContract]
        EditReturnValue LibrarySave(Library library, byte[] assembly);


        [OperationContract]
        Store_Activity StoreActivityGetByID(Guid Id);

        [OperationContract]
        Store_Activity StoreActivityGetByVersion(string Name, string Version, VersionFlag versionFlag);

        [OperationContract]
        bool StoreActivitySaveWithXaml(Store_Activity storeActivity, string ActivityXaml);

        [OperationContract]
        string WorkflowGetXamlById(Guid Id);

        [OperationContract]
        string WorkflowGetXamlByVersion(string Name, string Version, VersionFlag versionFlag);

        [OperationContract]
        IList<Store_Workflow> WorkflowsGetAll(VersionFlag versionFlag);

        [OperationContract]
        IList<Store_Workflow> WorkflowsGetList(ActivityCategory category, string keywords, VersionFlag versionFlag);

        [OperationContract]
        EditReturnValue WorkflowSave(Store_Workflow workflowHeader);

        [OperationContract]
        Store_Workflow WorkflowGetFromNameAndVersion(string name, string version);
        //2010/10/27 added by v-evsmit
        [OperationContract]
        Library LibraryGetById(Guid id);

        #endregion

        

    }

    #region Datacontracts
    


    [DataContract]
    public class xLibrary
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid Category { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string AuthGroup { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string ImportedBy { get; set; }
        [DataMember]
        public int StatusNo { get; set; }
        //[DataMember]
        //public IList<Tag> MetaTags { get; set; }
        //[DataMember]
        //public byte[] Executable { get; set; }
        [DataMember]
        public bool isSelected { get; set; }

    }

    [DataContract]
    public class LibraryWithActivities : xLibrary
    {
        [DataMember]
        public IList<StoreActivity> ActivityList { get; set; }
        //[DataMember]
        //public WFActivities test { get; set; }
        //[DataMember]
        //public IList<WFActivities> ActivityList { get; set; }
        //[DataMember]
        //public string Dummy { get; set; }
    }

    [DataContract]
    public class LibraryWithExecutable : xLibrary
    {
        [DataMember]
        public byte[] Executable { get; set; }
    }

    [DataContract]
    public class WFActivities : Catalog.StoreActivity
    {

    }

    [DataContract]
    public class WFTaxonomy
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public bool HasValueList { get; set; }

        //public CategoryTypes CategoryType { get; set; }
        [DataMember]
        public string CategoryKey { get; set; }
        [DataMember]
        public string ValueType { get; set; }
        [DataMember]
        public string RegularExpression { get; set; }   
        [DataMember]
        public int Volaltility { get; set; }
        [DataMember]
        public PrivacyTypes Privacy { get; set; }
    }

    [DataContract]
    public partial class WFContent
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public IList<string> Keywords { get; set; }
    }

    [DataContract]
    public class Tag
    {
        [DataMember]
        public WFTaxonomy Taxonomy { get; set; }
        [DataMember]
        public WFContent Content { get; set; }
    }
}
    #endregion
