using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Support.Workflow.Catalog;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Support.Workflow.CWFHelpers;
using System.Security.Principal;

namespace Microsoft.Support.Workflow
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Workflows" in code, svc and config file together.
    public class Workflows : IWorkflows
    {

        #region OldVersionOfServices
        /// <summary>
        /// for some reason the first service with an ilist is having problems
        /// </summary>
        /// <returns></returns>
        public IList<WFActivities> garbage()
        {
            return null;
        }

        public IList<xxContextItem> GetContextList(string Category, String Tags)
        {
            IList<Context> ContextList = new List<Context>();

            using (Workflow.Catalog.CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                if (Category.Trim().Length > 0)
                {
                    ContextList = (from c in proxy.Contexts
                                   where c.ContextCategory.Name.ToUpper() == Category.ToUpper()
                                   select c).ToList<Context>();

                    if (Tags.Trim().Length > 0)
                    {
                        // check tags once they are added back to the db.


                    }
                }
                else if (Tags.Trim().Length > 0)
                {

                }
                else
                {
                    ContextList = (from c in proxy.Contexts
                                   select c).ToList<Context>();
                }


                IList<xxContextItem> ContextItemList = new List<xxContextItem>();
                foreach (var item in ContextList)
                {
                    ContextItemList.Add(item.ConvertToContextItem());
                }

                return ContextItemList;

            }


            
            
        }

        public IList<xxContextCategory> GetContextCategoryList()
        {

            using (Workflow.Catalog.CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                List<Workflow.Catalog.ContextCategory> myList =
                    (from c in proxy.ContextCategories
                     select c).ToList<Workflow.Catalog.ContextCategory>();
                IList<Workflow.xxContextCategory> outList = new List<Workflow.xxContextCategory>();

                foreach (var item in myList)
                {
                    outList.Add(item.ConvertToContextCategory());

                }
                return outList;
            }

        }

        //public IWorkflows.WorkflowStoreClass GetWorkflow1()
        //{
        //    throw new NotImplementedException();
        //}

        //public void SaveWorkflow1(IWorkflows.WorkflowStoreClass aWorkflow)
        //{
        //    throw new NotImplementedException();
        //}
        //System.Configuration.Configuration rootWebConfig =
        //        System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(@"C:\Users\v-evsmit\Documents\Projects\Msft CSS CWF\Visual Studio\WF_Svc_Context\WorkflowService\Web.config");
        //System.Configuration.ConnectionStringSettings connString;
        public Store_Workflow GetWorkflowFromGuid(Guid WorkflowID)
        {
            Store_Workflow aWorkflow = new Store_Workflow();
            using (Workflow.Catalog.CWFDummyDataSourceEntities context = new CWFDummyDataSourceEntities())
            {
                var item = (from workflow in context.Workflows
                            where workflow.id == WorkflowID
                            select workflow).FirstOrDefault();

                if ((item == null))
                {
                    aWorkflow = null;
                }
                else
                {
                    aWorkflow = ConvertFrom(item);

                }

            }

            return aWorkflow;
        }

        private Store_Workflow ConvertFrom(Workflow.Catalog.Workflow item)
        {
            Store_Workflow aWorkflow = new Store_Workflow()
            {
                ID = item.id,
                Name = item.Name,
                //VersionString = item.Version ,
                Version = new Version(item.Version),
                IsNonDialog = item.IsNonDialog,
                IsService = item.IsService,
                WorkFlowText = item.Xaml,
                Description = item.Description,
                //StatusNo = item.Status ,
                Status = (Workflow.Status)item.Status,
                Locked = (bool)item.Locked,
                Applications = new List<WFApplication>(),
                LockedBy = item.LockedBy,
                   
            };
          
            aWorkflow.WorkFlowType = (from wt in GetWFTypes()
                                      where wt.id == item.WorkFlowType
                                      select wt).FirstOrDefault();

            return aWorkflow;
        }

        public Store_Workflow GetWorkflowFromNameAndVersion(string WorkflowName, string WorkflowVersion)
        {
            Store_Workflow aWorkflow = new Store_Workflow();
            using (Workflow.Catalog.CWFDummyDataSourceEntities context = new CWFDummyDataSourceEntities())
            {
                var item = (from workflow in context.Workflows
                            where workflow.Name == WorkflowName && 
                            workflow.Version == WorkflowVersion
                            select workflow).FirstOrDefault();

                if ((item == null))
                {
                    aWorkflow = null;
                }
                else
                {
                    aWorkflow = ConvertFrom(item);
                
                }

            }

            return aWorkflow;
        }

        /// <summary>
        /// New version of get the workflow which is now stored in StoreActivity
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public Store_Workflow WorkflowGetFromNameAndVersion(string name, string version)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(version))
            {
                string msgsuffix = "";
                if (string.IsNullOrEmpty(name))
                    msgsuffix = "Name was null or empty. ";
                if (string.IsNullOrEmpty(version))
                    msgsuffix = msgsuffix + "Version was null or empty.";
            
                string msg = "Null or empty string passed as parameter to WorkflowGetFromNameAndVersion :  "+ msgsuffix;
                LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                return null;
            }



            Version _version = VersionConvertFromString(version);
            if (_version == null)
            {
                string msg = "Invalid version number passed to WorkflowGetFromNameAndVersion : " + version;
                LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                return null;
            }
            string versionString = _version.ToString();
            using (Workflow.Catalog.CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                StoreActivity item = (from s in proxy.StoreActivities
                                     where s.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                     s.Version == versionString
                                     select s).FirstOrDefault();
                if (item == null)
                    return null;
                else
                {
                    Store_Workflow wst = new Store_Workflow()
                    {
                        ID = item.Id,
                        Name = item.Name,
                        Version = item.VersionNo,
                        IsNonDialog = !item.IsUxActivity,
                        IsService = item.IsService,
                        Description = item.Description,
                        Status = (Status)item.Status,
                        Tags = item.MetaTags.Split(';').ToList<string>(),
                        WorkFlowText = item.XAML,
                        Locked = (bool)item.Locked,
                        LockedBy = item.LockedBy,
                        // convert db entitytype for WorkFlowType to 
                        // Workflow_Activity_Type.
                        // Return nulls for templates and related 
                        // workflows to reduce size of returned array
                        WorkFlowType = new Workflow_Activity_Type()
                        {
                            id = item.WorkflowType1.id,
                            WorkflowTemplate = item.WorkflowType1.WorkflowTemplate,
                            PublishingWorkflow = item.WorkflowType1.PublishingWorkflow,
                            SelectionWorkflow = null, // this field does not make sense based on workflowtype
                                                      // should be based on application
                            ContextVariable = item.WorkflowType1.ContextVariable,
                            PageViewVariable = item.WorkflowType1.PageViewVariable,
                            HandleVariable = item.WorkflowType1.HandleVariable,
                            Name = item.WorkflowType1.Name,
                            AuthorityGroup = new AuthorityGroup()
                            {
                                Name = item.WorkflowType1.AuthGroup.AuthGroup1,
                            }
                        }

                    };


                    wst.Applications = new List<WFApplication>();
                    foreach (var app in item.ActivityApplicationXRefs)
                    {
                        wst.Applications.Add(
                            new WFApplication()
                            {
                                Id = app.Application,
                                Name = app.Application1.Name,

                            });
                    }

                    return wst;
                }
                // convert to Store_Activity
                
            }
        }

        public Byte[] GetActivityAssembly(string AssemblyName)
        {

            return GetActivityAssemblyFromNameAndVersion(AssemblyName, "");

            //string fileName = String.Concat(Microsoft.Support.Workflow.Catalog.Properties.Settings.Default.DllPath,
            //    AssemblyName);

            //if (!fileName.ToUpper().Contains(".DLL"))
            //    fileName = string.Concat(fileName, ".dll");


            //byte[] buff = null;
            //try
            //{
            //    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            //    BinaryReader br = new BinaryReader(fs);
            //    long numBytes = new FileInfo(fileName).Length;
            //    buff = br.ReadBytes((int)numBytes);

            //    //temp code
            //    System.Reflection.Assembly temp = System.Reflection.Assembly.Load(buff);
                
            //    Library aLib = new Library();
            //    System.Reflection.AssemblyName tname = temp.GetName();

            //    //aLib.Name = tname.Name;
            //    //aLib.Version = tname.Version.ToString();
            //    //aLib.Executable = buff;
            //    //aLib.AuthGroup = "";
            //    //aLib.ID = Guid.NewGuid();
                
            //    //aLib.Category = Guid.Parse("a94878ad-77c9-4b46-b785-c93deb342e4c");
            //    CWFHelpers.LogEventLog.Log2EventLog("Starting temp library save", System.Diagnostics.EventLogEntryType.Information);
            //    CWFHelpers.LogEventLog.Log2EventLog(aLib.Name + " Version=" + aLib.Version,System.Diagnostics.EventLogEntryType.Information);
            //    //SaveNewActivityAssembly(aLib);

                

            //    return buff;
            //}
            //catch (Exception ex)
            //{

            //    return null;
            //}


        }


        public EditReturnValue LibrarySave(Library library, byte[] assembly)
        {
            EditReturnValue retValue = new EditReturnValue();

            if (assembly == null)
            {
                string msg = "";
                if (string.IsNullOrEmpty(library.Name))
                    msg = "Assembly required to perform LibrarySave";
                else
                    msg = "Assembly required to perform LibrarySave for library name " + library.Name;

                LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                    
                return new EditReturnValue(false,msg);

            }
            try
            {
                using (Workflow.Catalog.CWFDummyDataSourceEntities context = new CWFDummyDataSourceEntities())
                {
                    // First check to see if the db record exists
                    ActivityLibrary aLib = (from a in context.ActivityLibraries
                                            where a.Name == library.Name &&
                                            a.VersionNumber == library.VersionNumber
                                            select a).FirstOrDefault<ActivityLibrary>();


                    if (aLib == null)
                    {
                        CWFHelpers.LogEventLog.Log2EventLog("Adding new library " 
                            + library.Name + " Version=" 
                            + library.VersionNumber, 
                            System.Diagnostics.EventLogEntryType.Information);

                        //
                        // Edit the Library, if it fails the edits return the 
                        // value to the calling program
                        retValue = library.Edit();
                        if (retValue.IsValid == false)
                        {
                            foreach (var item in retValue.Messages)
                            {
                                LogEventLog.Log2EventLog(item, EventLogEntryType.Error);

                            }
                            return retValue;
                        }

                        //
                        //Create the new activity library record
                        aLib = new ActivityLibrary()
                        {
                            Id = library.Id,
                            Name = library.Name,
                            VersionNumber = library.VersionNumber,
                            Executable = assembly,
                            Category = library.Category,
                            HasActivities = library.HasActivities,
                            Description = library.Description,
                            ImportedBy = library.ImportedBy,
                            Status = library.Status,
                            AuthGroup = "" //todo review use of this field
                        };

                        //
                        //Write a log entry to show that the library has been added and 
                        //call method to add library to entity object.
                        CWFHelpers.LogEventLog.Log2EventLog("Library object created for " 
                            + aLib.Name, System.Diagnostics.EventLogEntryType.Information);
                        
                        context.ActivityLibraries.AddObject(aLib);

                        //
                        //add each activity for the library to the activity entity
                        CWFHelpers.LogEventLog.Log2EventLog("Adding activities", System.Diagnostics.EventLogEntryType.Information);
                        foreach (var item in library.Activities)
                        {
                            CWFHelpers.LogEventLog.Log2EventLog("Adding activity "+item.Name , System.Diagnostics.EventLogEntryType.Information);

                            // edit the activity
                            retValue = item.Edit();
                            if (retValue.IsValid == false)
                            {
                                string temp = "";
                                foreach (var msg in retValue.Messages)
                                {
                                    temp = string.Concat(temp, msg, "\r\n");
                                }
                                LogEventLog.Log2EventLog(temp, EventLogEntryType.Error);
                                return retValue;
                            }
                            else if (retValue.IsWarning)
                            {
                                string temp = "";
                                foreach (var msg in retValue.Messages)
                                {
                                    temp = string.Concat(temp, msg, "\r\n");
                                }
                                LogEventLog.Log2EventLog(temp, EventLogEntryType.Warning);

                            }

                            try
                            {
                                context.StoreActivities.AddObject(StoreActivity.ConvertTo(item));
                            }
                            catch (Exception ex)
                            {
                                retValue.AddMessage(false, "Unhandled exception saving activity " + item.Name);
                                retValue.AddMessage(ex.Message);
                                if (!(ex.StackTrace == null))
                                {
                                    retValue.AddMessage("Stack Trace");
                                    retValue.AddMessage(ex.StackTrace);
                                }
                                if (!(ex.InnerException == null ))
                                {
                                    retValue.AddMessage("Inner Exception");
                                    retValue.AddMessage(ex.InnerException.Message);

                                }
                                string temp2;
                                var serializer = new DataContractSerializer(item.GetType());

                                using (var backing = new System.IO.StringWriter())

                                using (var writer = new System.Xml.XmlTextWriter(backing))
                                {

                                    serializer.WriteObject(writer, item);

                                    temp2 = backing.ToString();

                                }

                                LogEventLog.Log2EventLog(retValue.DisplayString()+"\r\n"+temp2, EventLogEntryType.Error);
                                return retValue;
                            }
                        }
                        CWFHelpers.LogEventLog.Log2EventLog("Saving Library changes", System.Diagnostics.EventLogEntryType.Information);
                        context.SaveChanges();
                        CWFHelpers.LogEventLog.Log2EventLog("Library changes saved", System.Diagnostics.EventLogEntryType.Information);
                        return new EditReturnValue(true, "Library changes saved");
                    }
                    else
                    {
                        return new EditReturnValue(false,"Saving existing libraries not implemented yet");
                        //todo add update capability
                    }
                }
            }
            catch (Exception ex)
            {
                IList<string> msgs = new List<string>() {"Unhandled exception saving dll " + ex.Message };
                if (ex.InnerException == null)
                {

                    msgs.Add(ex.StackTrace);

                }
                else 
                {
                    
                    msgs.Add("Inner Exception :" + ex.InnerException.Message);
                    msgs.Add(ex.StackTrace);

                
                }

                LogEventLog.Log2EventLog(retValue.DisplayString(), EventLogEntryType.Error);
                return new EditReturnValue(false,msgs);
            }
        }
        public Guid SaveNewActivityAssembly(LibraryWithExecutable aLibrary)
        {

            Guid id = Guid.NewGuid();
            try
            {
                using (Workflow.Catalog.CWFDummyDataSourceEntities context = new CWFDummyDataSourceEntities())
                {
                    //
                    // First check to see if the db record exists
                    ActivityLibrary aLib = (from a in context.ActivityLibraries
                                            where a.Name == aLibrary.Name && 
                                            a.VersionNumber == aLibrary.Version
                                            select a).FirstOrDefault<ActivityLibrary>();
                    
                    if (aLib == null)
                    {
                        aLib = new ActivityLibrary() { Id = id, 
                            Name = aLibrary.Name, 
                            VersionNumber = aLibrary.Version.ToString(), 
                            AuthGroup = aLibrary.AuthGroup,
                            Executable = aLibrary.Executable };
                        CWFHelpers.LogEventLog.Log2EventLog("Adding new library", System.Diagnostics.EventLogEntryType.Information);
                        CWFHelpers.LogEventLog.Log2EventLog(aLib.Name + " Version=" + aLib.VersionNumber, System.Diagnostics.EventLogEntryType.Information);
                        context.ActivityLibraries.AddObject(aLib);
                    }
                    else
                    {
                        CWFHelpers.LogEventLog.Log2EventLog("Updating Library", System.Diagnostics.EventLogEntryType.Information);
                        CWFHelpers.LogEventLog.Log2EventLog(aLib.Name + " Version=" + aLib.VersionNumber, System.Diagnostics.EventLogEntryType.Information);
                        aLib.Executable = aLibrary.Executable;
                        id = aLib.Id;
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                CWFHelpers.LogEventLog.Log2EventLog("Unhandled exception saving dll " + ex.Message, EventLogEntryType.Error);
                //
                // Todo Should we return an empty guid or throw exception
                id = Guid.Empty;
                
            }



            return id;
        }

       

        public void SaveWorkFlow(Microsoft.Support.Workflow.Store_Workflow aWorkflow)
        {
            bool NewWorkflow = true;
            //
            //First check to see if this workflow already exists?
            Store_Workflow chkWF = GetWorkflowFromGuid(aWorkflow.ID);

            if (chkWF == null)
            {
                // Check to make sure the workflow name and version number do not exist with a 
                // different guid.
                Store_Workflow chkWF2 = GetWorkflowFromNameAndVersion(aWorkflow.Name,aWorkflow.Version.ToString());
                if (chkWF2 == null)
                    NewWorkflow = true;
                else
                    throw new InvalidDataException(
                        string.Concat("Workflow ",aWorkflow.Name," Version ",chkWF.Version," with the supplied guid of ",aWorkflow.ID.ToString(),
                        " already exists with the guid number of ",chkWF2.ID.ToString()));

            }
            else
            {
                if ((Status)chkWF.Status == Status.Production &&
                    (!(((Status)aWorkflow.Status == Status.Depreciated || 
                    (Status)aWorkflow.Status == Status.Deleted))))
                {
                    throw new InvalidDataException(
                        string.Concat("Workflow ", aWorkflow.Name, " Version ", chkWF.Version, " with the supplied guid of ", chkWF.ID.ToString(),
                        " exists with a status of in production.  Existing workflows can only be Deleted or Depreciatied"));
                }
                else 
                    NewWorkflow = false;
            }

            using (Workflow.Catalog.CWFDummyDataSourceEntities context = new CWFDummyDataSourceEntities())
            {
                if (NewWorkflow)
                {
                    Catalog.Workflow aWF = new Catalog.Workflow()
                    {
                        id = aWorkflow.ID,
                        Name = aWorkflow.Name,
                        Description = aWorkflow.Description,
                        IsNonDialog = aWorkflow.IsNonDialog,
                        IsService = aWorkflow.IsService,
                        Version = aWorkflow.Version.ToString(),
                        WorkFlowType = aWorkflow.WorkFlowType.id,
                        Xaml = aWorkflow.WorkFlowText,
                        //Status = (int)aWorkflow.Status,
                        Locked = false,
                        LockedBy = aWorkflow.LockedBy,
                        ManagementContext = Guid.NewGuid()

                    };
                    //aWF.Applications = "";
                    //foreach (var app in aWorkflow.Applications)
                    //{
                    //    aWF.Applications = String.Concat(aWF.Applications, app.Id, ',');

                    //}
                    aWF.MetaTags = "";
                    foreach (var tag in aWorkflow.Tags)
                    {
                        aWF.MetaTags = String.Concat(aWF.MetaTags, tag, ',');
                    }
                
                        context.Workflows.AddObject(aWF);
                }
                else
                {
                    Catalog.Workflow aWF = (from wf in context.Workflows 
                                           where wf.id == aWorkflow.ID
                                           select wf).FirstOrDefault();

                    
                    aWF.id = aWorkflow.ID;
                    aWF.Name = aWorkflow.Name;
                    aWF.Description = aWorkflow.Description;
                    aWF.IsNonDialog = aWorkflow.IsNonDialog;
                    aWF.IsService = aWorkflow.IsService;
                    aWF.Version = aWorkflow.Version.ToString();
                    aWF.WorkFlowType = aWorkflow.WorkFlowType.id;
                    aWF.Xaml = aWorkflow.WorkFlowText;
                    //aWF.Status = (int)aWorkflow.Status;
                    aWF.Locked = false;
                    aWF.LockedBy = aWorkflow.LockedBy;
                    aWF.ManagementContext = Guid.NewGuid();
                    
                    
                }
                context.SaveChanges();
            }
            
        }

        public IList<Store_Workflow> GetAllWorkFlows()
        {
            //
            //modified 2010/10/20 by v-evsmit to change from using db.Workflows to db.StoreActivities
            //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WorkFlowServiceData"].ConnectionString);

            //conn.Open();

            IList<Store_Workflow> aList = new List<Store_Workflow>();

            IList<Workflow_Activity_Type> wtList = GetWFTypes();

            Workflow.Catalog.Workflow awf = new Catalog.Workflow();

            Workflow.Catalog.CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities();

            //
            //todo consider changing database to have an explicit record type indicator
            //or flag to show that the record contains xaml and is a displayble workflow.
            //Also, we should indicate if this is a compiled workflow.
            var query = from workflow in proxy.StoreActivities
                        where !(String.IsNullOrEmpty(workflow.XAML))
                        select workflow;
            //context.Dispose();

            foreach (var item in query)
            {
                Store_Workflow wst = new Store_Workflow()
                    {
                        ID = item.Id,
                        Name = item.Name,
                        Version = new Version(item.Version),
                        IsNonDialog = !item.IsUxActivity,
                        IsService = item.IsService,
                        Description = item.Description,
                        Status = (Status)item.Status ,
                        Tags = item.MetaTags.Split(';').ToList<string>(),
                        WorkFlowText = null, // do not return xaml when returning all workflows
                        Locked = (bool) item.Locked,
                        LockedBy = item.LockedBy,
                        // convert db entitytype for WorkFlowType to 
                        // Workflow_Activity_Type.
                        // Return nulls for templates and related 
                        // workflows to reduce size of returned array
                        WorkFlowType = new Workflow_Activity_Type()
                        {
                            id = item.WorkflowType1.id,
                            WorkflowTemplate = null,
                            PublishingWorkflow = null,
                            SelectionWorkflow = null,
                            ContextVariable = item.WorkflowType1.ContextVariable,
                            PageViewVariable = item.WorkflowType1.PageViewVariable,
                            HandleVariable = item.WorkflowType1.HandleVariable,
                            Name = item.WorkflowType1.Name,
                            AuthorityGroup = new AuthorityGroup()
                            {
                                Name = item.WorkflowType1.AuthGroup.AuthGroup1,
                            }
                        }
                        
                    };


                wst.Applications = new List<WFApplication>();
                foreach (var app in item.ActivityApplicationXRefs)
                {
                    wst.Applications.Add(
                        new WFApplication()
                        {
                            Id = app.Application,
                            Name = app.Application1.Name,
                            
                        });
                }

                aList.Add(wst);
            }


            return aList;
        }

 

        public IList<Workflow_Activity_Type> GetWFTypes()
        {
            IList<Workflow_Activity_Type> wftList = new List<Workflow_Activity_Type>();
            using (Workflow.Catalog.CWFDummyDataSourceEntities context = new CWFDummyDataSourceEntities())
            {
                var query = from w in context.WorkflowTypedbs select w;
                foreach (var item in query)
                {
                    wftList.Add(new Workflow_Activity_Type()
                        {
                            id = item.id,
                            Name = item.Name,
                            Group = item.WorkflowGroup,
                            PublishingWorkflow = item.PublishingWorkflow
                        });
                }
            }
            return wftList;
        }

        public bool SaveActivity(LibraryWithExecutable aLibrary)
        {

            

            return false;
        }

        public IList<WFApplication> GetAllApplications()
        {
            IList<WFApplication> appList = new List<WFApplication>();
            using (Workflow.Catalog.CWFDummyDataSourceEntities context = new CWFDummyDataSourceEntities())
            {
                
                var query = from apps in context.ApplicationDBs select apps;

                foreach (var item in query)
                {
                    appList.Add(new WFApplication()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Description = item.Description
                        });

                }
            }

            return appList;

        }


        public byte[] GetActivityAssemblyFromID(Guid AssemblyID)
        {
            
            using (Workflow.Catalog.CWFDummyDataSourceEntities context = new CWFDummyDataSourceEntities())
            {
                return (from asm in context.ActivityLibraries
                        where asm.Id == AssemblyID
                        select asm.Executable).FirstOrDefault();
            }

        }
        #endregion




        public byte[] GetActivityAssemblyFromNameAndVersion(string AssemblyName, string AssemblyVersion)
        {
            CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities();

            AssemblyName = AssemblyName.Trim().Replace(".dll", "");

            byte[] retValue;


            if (AssemblyVersion.Trim().Length == 0)
            {
                LogEventLog.Log2EventLog("Retreiving activity library by name only: " + AssemblyName, EventLogEntryType.Information);
                // no version number supplied, get the latest version that has the same library name.

                //todo add checking of version
                //todo fix version sort to correctly sort the version numbers instead of the string
                //trying to inline change the value to a System.Version does not work.
                //Also System.Version does not serialize
                retValue= (from al in proxy.ActivityLibraries
                        where al.Name.Trim().ToUpper() == AssemblyName.Trim().ToUpper()
                        select al.Executable).FirstOrDefault();

            }
            else
            {
                LogEventLog.Log2EventLog("Retreiving activity: " + AssemblyName + " version: " + AssemblyVersion, EventLogEntryType.Information);

                retValue = (from al in proxy.ActivityLibraries
                        where al.Name.Trim().ToUpper() == AssemblyName.Trim().ToUpper() &&
                         al.VersionNumber == AssemblyVersion
                        select al.Executable).FirstOrDefault();

                
            }

            if (retValue == null)
                LogEventLog.Log2EventLog("Activity library: " + AssemblyName + " version: " + AssemblyVersion +"not found", EventLogEntryType.Error);

            return retValue;
        }

        public xLibrary GetLibraryByLatestBuild(string AssemblyName, Version AssemblyVersion)
        {
            xLibrary aLib = new xLibrary();
            return aLib;
        }


        public bool WriteToServerEventLog(string message, EventLogEntryType entryType)
        {
            throw new NotImplementedException();
        }

        public IList<LibraryWithActivities> GetLibraryListWithOutExecutable()
        {

            LibraryWithActivities test = new LibraryWithActivities();
            
            try
            {
                IList<LibraryWithActivities> retList = new List<LibraryWithActivities>();
                using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
                {
                    List<viewActivityLibraryWoExecutable> aList =
                        (from a in proxy.viewActivityLibraryWoExecutables
                         where (bool)a.HasActivities 
                         select a).ToList<viewActivityLibraryWoExecutable>();

                    
                    foreach (var item in aList)
                    {

                        if (item.Category == null)
                            item.Category = Guid.Empty;

                        if (item.Description == null)
                            item.Description = "";




                        retList.Add(new LibraryWithActivities()
                            {
                                ID = item.Id,
                                Name = item.Name,
                                Version = item.VersionNumber,
                                StatusNo = (int)item.Status,
                                AuthGroup = item.AuthGroup,
                                Category = (Guid)item.Category,
                                Description = item.Description.ToString(),
                                ImportedBy = item.ImportedBy.ToString(),
                                isSelected = false,
                                //test = new WFActivities()
                                //ActivityList =
                                //( from s in proxy.StoreActivities
                                //      where s.ActivityLibrary == item.Id
                                //      select s).ToList<StoreActivity>()
                                 
                                //MetaTags = new List<Tag>()
                                //Executable = new byte[] { (byte)' ' }
                            }
                        );
                    }
                }
                return retList;
            }
            catch (Exception ex)
            {
                LogEventLog.Log2EventLog("GetLibraryListWithOutExecutable db access error: " + 
                    ex.Message, EventLogEntryType.Error);
                return null;
            }

        }

        public bool CheckForService(string UserId)
        {
            LogEventLog.Log2EventLog("User connected: " + UserId, EventLogEntryType.SuccessAudit);
            try
            {
                CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities();
                string temp = (from w in proxy.WorkflowTypedbs select w.Name).FirstOrDefault();
                return true;
            }
            catch (Exception ex)
            {
                LogEventLog.Log2EventLog("CheckForService db access error: " + ex.Message, EventLogEntryType.Error);
                return false;
            }

        }




        public WFContent GetContentByID(Guid id)
        {
            throw new NotImplementedException();
        }

        public WFTaxonomy GetTaxonomyById(Guid id)
        {
            throw new NotImplementedException();
        }




        public LibraryWithExecutable GetLibraryFromId(Guid id)
        {
            LibraryWithExecutable aLib = new LibraryWithExecutable();
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                ActivityLibrary tlib = (from t in proxy.ActivityLibraries
                                        where t.Id == id
                                        select t).FirstOrDefault();

                if (!(tlib == null))
                {
                    aLib = new LibraryWithExecutable()
                    {
                        ID = tlib.Id,
                        Name = tlib.Name,
                        Version = tlib.VersionNumber,
                        Description = tlib.Description,
                        AuthGroup = tlib.AuthGroup,
                        Category = (Guid)tlib.Category,
                        ImportedBy = tlib.ImportedBy,
                        StatusNo = (int)tlib.Status,
                        isSelected = false,
                        Executable = tlib.Executable
                    };

                }
            }
            return aLib;
        }


        public LibraryWithExecutable GetLibrary_LatestBuild(string Name, string Version)
        {
            throw new NotImplementedException();
        }


        public IList<ToolboxItem> ToolboxItemGetList(VersionFlag versionFlag)
        {
            return ToolboxItem.GetList( versionFlag);
        }
        //
        // 2010/10/27 - service added by v-evsmit
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Library LibraryGetById(Guid id)
        {
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                return (from l in proxy.Libraries
                        where l.Id == id
                        select l).FirstOrDefault();

            }
        }
        
        public IList<Library> LibraryGetAll()
        {
            return Library.GetList(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category">Name of a category, if blank all categories</param>
        /// <param name="SearchString">Search string used against Libary and Acitivy Metatags and names</param>
        /// <param name="versionFlag"></param>
        /// <param name="HasActivitiesOnly">Only retrieve libraries which have activities</param>
        /// <returns></returns>
        public IList<Library> LibraryGetList(string category, string SearchString, VersionFlag versionFlag, bool HasActivitiesOnly)
        {
            IList<Library> aList = Library.GetList(versionFlag, HasActivitiesOnly);

            if (aList.Count == 0)
                return aList;

            if (category == null)
                category = "";
            if (SearchString == null)
                SearchString = "";

            IList<Library> cList = new List<Library>();
            if (category.Trim().Length > 0)
            {
                ActivityCategory ac;
                using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
                {
                    ac = (from c in proxy.ActivityCategories
                          where c.Name.Equals(category, StringComparison.OrdinalIgnoreCase)
                          select c).FirstOrDefault();

                }

                if (ac == null)
                {
                    return new List<Library>();
                }
                else
                {
                    cList = (from acl in aList
                             where (Guid)acl.Category == ac.Id
                             select acl).ToList();

                    if (cList == null)
                        return new List<Library>();
                }
            }
            else
            {
                cList = aList;
            }

            if (SearchString.Trim().Length == 0)
            {
                return cList;
            }
            else
            {
                SearchString = SearchString.ToUpper();
                return (from sl in cList
                        where (sl.Name.ToUpper().Contains(SearchString) ||
                        sl.MetaTags.ToUpper().Contains(SearchString) ||
                        sl.Description.ToUpper().Contains(SearchString))
                        select sl).ToList();
            }

        }




        public Store_Activity StoreActivityGetByID(Guid Id)
        {
            throw new NotImplementedException();

            // return null if not found
        }

        internal System.Version VersionConvertFromString(string version)
        {
            Version testVersion = new Version();
            if (String.IsNullOrEmpty(version))
                return null;
            else
            {
                if (!System.Version.TryParse(version, out testVersion))
                {
                    string st = "";
                    //parse does not work if they only passed in the major version 
                    //number, try adding .0 and trying again
                    if (version.LastIndexOf('.') == (version.Length - 1))
                    {
                        st = version + "0";
                    }
                    else
                    {
                        st = version + ".0";
                    }
                    if (!System.Version.TryParse(st, out testVersion))
                    {
                        return null;
                    }
                }
            }

            //
            // make sure that the build and revision numbers are there
            if (testVersion.Build < 0)
                testVersion = new Version(testVersion.Major, testVersion.Minor, 0, 0);
            else if (testVersion.Revision < 0)
                testVersion = new Version(testVersion.Major, testVersion.Minor, testVersion.Build, 0);

            return testVersion;
        }

        public Store_Activity StoreActivityGetByVersion(string Name, string version, VersionFlag versionFlag)
        {
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                IList<StoreActivity> activities = (from sas in proxy.StoreActivities
                                                   where sas.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)
                                                   select sas).ToList();
                if (String.IsNullOrEmpty(version))
                    versionFlag = VersionFlag.LastestVersion;
                Version testVersion = new Version();
                StoreActivity retValue;
                switch (versionFlag)
                {
                    case VersionFlag.LastestVersion:
                        retValue = (from a in activities
                                orderby a.VersionNo descending
                                select a).FirstOrDefault();
                        break;
                    case VersionFlag.LatestBuild:
                        testVersion = VersionConvertFromString(version);
                        retValue = (from a in activities
                                where a.VersionNo.Major == testVersion.Major &&
                                a.VersionNo.Minor == testVersion.Minor
                                orderby a.VersionNo descending
                                select a).FirstOrDefault();
                        break;

                    //case VersionFlag.AllBuilds:
                    //    break;
                    //case VersionFlag.SpecificVersionAndBuild:
                    //    break;
                    //
                    // For all other cases, return only the specific build
                    default:
                        testVersion = VersionConvertFromString(version);
                        retValue = (from a in activities
                                where a.VersionNo.Equals(testVersion)
                                select a).FirstOrDefault();
                        break;

                }
                if (retValue == null)
                {
                    return null;
                }
                else
                    return retValue.ConvertFrom();
            }
        }

        public bool StoreActivitySaveWithXaml(Store_Activity storeActivity, string ActivityXaml)
        {
            StoreActivity a1 = StoreActivity.ConvertTo(storeActivity);
            a1.XAML = ActivityXaml;

            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {

                //
                // check to see if object exists alread
                StoreActivity a2 = (from a in proxy.StoreActivities
                                            where a.Id == a1.Id
                                            select a).FirstOrDefault();
                if (a2 == null)
                {
                    proxy.StoreActivities.AddObject(a1);
                }
                else
                {
                    a2.Id = a1.Id; // Should be the same
                    a2.Name = a1.Name;
                    a2.Namespace = a1.Namespace;
                    

                }
                try
                {
                    proxy.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    LogEventLog.Log2EventLog("Unhandled error saving activity : " + a1.Name + " version " + a1.VersionNo.ToString() + "\r\n" + ex.Message, EventLogEntryType.Error);
                    return false;
                }
            }

        }

        public string WorkflowGetXamlById(Guid Id)
        {
            //temporary code
            //todo change to using store activity once the 
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                return (from wf in proxy.StoreActivities
                        where wf.Id == Id
                        select wf.XAML).FirstOrDefault();
            }
        }

        public string WorkflowGetXamlByVersion(string Name, string Version, VersionFlag versionFlag)
        {
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                IList<StoreActivity> activities = (from sas in proxy.StoreActivities
                                                   where sas.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)
                                                   select sas).ToList();

                Version testVersion = new Version(Version);

                switch (versionFlag)
                {
                    case VersionFlag.LastestVersion:
                        return (from a in activities
                                orderby a.VersionNo descending
                                select a.XAML).FirstOrDefault();
                    case VersionFlag.LatestBuild:
                        return (from a in activities
                                where a.VersionNo.Major == testVersion.Major &&
                                a.VersionNo.Minor == testVersion.Minor
                                orderby a.VersionNo descending
                                select a.XAML).FirstOrDefault();
                        
                    //case VersionFlag.AllBuilds:
                    //    break;
                    //case VersionFlag.SpecificVersionAndBuild:
                    //    break;
                    //
                    // For all other cases, return only the specific build
                    default:
                        return (from a in activities
                                where a.VersionNo.Equals(testVersion)
                                select a.XAML).FirstOrDefault();
                        
                }
            }
        }

        public IList<Store_Workflow> WorkflowsGetAll(VersionFlag versionFlag)
        {
            //todo do something with the versionflag
            //modifed 20/10/20 by v-evsmit to copy code from GetAllWorkflows()
            //
            //modified 2010/10/20 by v-evsmit to change from using db.Workflows to db.StoreActivities
            //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WorkFlowServiceData"].ConnectionString);

            //conn.Open();

            IList<Store_Workflow> aList = new List<Store_Workflow>();

            IList<Workflow_Activity_Type> wtList = GetWFTypes();

            Workflow.Catalog.Workflow awf = new Catalog.Workflow();

            Workflow.Catalog.CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities();

            //
            //todo consider changing database to have an explicit record type indicator
            //or flag to show that the record contains xaml and is a displayble workflow.
            //Also, we should indicate if this is a compiled workflow.
            var query = from workflow in proxy.StoreActivities
                        where !(String.IsNullOrEmpty(workflow.XAML))
                        select workflow;
            //context.Dispose();

            foreach (var item in query)
            {
                Store_Workflow wst = new Store_Workflow()
                {
                    ID = item.Id,
                    Name = item.Name,
                    Version = new Version(item.Version),
                    IsNonDialog = !item.IsUxActivity,
                    IsService = item.IsService,
                    Description = item.Description,
                    Status = (Status)item.Status,
                    Tags = item.MetaTags.Split(';').ToList<string>(),
                    WorkFlowText = null, // do not return xaml when returning all workflows
                    Locked = (bool)item.Locked,
                    LockedBy = item.LockedBy,
                    // convert db entitytype for WorkFlowType to 
                    // Workflow_Activity_Type.
                    // Return nulls for templates and related 
                    // workflows to reduce size of returned array
                    WorkFlowType = new Workflow_Activity_Type()
                    {
                        id = item.WorkflowType1.id,
                        WorkflowTemplate = null,
                        PublishingWorkflow = null,
                        SelectionWorkflow = null,
                        ContextVariable = item.WorkflowType1.ContextVariable,
                        PageViewVariable = item.WorkflowType1.PageViewVariable,
                        HandleVariable = item.WorkflowType1.HandleVariable,
                        Name = item.WorkflowType1.Name,
                        AuthorityGroup = new AuthorityGroup()
                        {
                            Name = item.WorkflowType1.AuthGroup.AuthGroup1,
                        }
                    }

                };


                wst.Applications = new List<WFApplication>();
                foreach (var app in item.ActivityApplicationXRefs)
                {
                    wst.Applications.Add(
                        new WFApplication()
                        {
                            Id = app.Application,
                            Name = app.Application1.Name,

                        });
                }

                aList.Add(wst);
            }


            return aList;
        }

        public IList<Store_Workflow> WorkflowsGetList(ActivityCategory category, string keywords, VersionFlag versionFlag)
        {
            throw new NotImplementedException();
        }


        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflow"></param>
        /// <returns>A return value with a Saved flag of true and false
        /// and a list of messages, the first message formatted for user display</returns>
        public EditReturnValue WorkflowSave(Store_Workflow workflow)
        {
            if (workflow == null)
            {
                string msg = "WorkflowSave called with a workflow value of null";
                LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                return new EditReturnValue(false, msg);
            }
            
            EditReturnValue editRetValue = new EditReturnValue();

            //
            // perform pre edits, ensures we have default values
            // instead of null values where appropriate, and 
            // returns a EditReturnValue.IsValid of false if 
            // invalid values were encountered.
            editRetValue = workflow.Edit();

            // if there were errors or warnings, write to the event log
            if ((!editRetValue.IsValid) || editRetValue.IsValid)
            {
                EventLogEntryType thisType;
                if (!editRetValue.IsValid)
                    thisType = EventLogEntryType.Error;
                else
                    thisType = EventLogEntryType.Warning;

                foreach (var item in editRetValue.Messages)
                {
                    LogEventLog.Log2EventLog(item, thisType);
                }

                //
                // If this was an error, exit without saving
                if (!editRetValue.IsValid)
                    return editRetValue;
            }
            
            // Convert the version to a string.
            string versionString = workflow.Version.ToString();

            // Convert the list of tags into one string.
            string tagString = "";
            foreach (var item in workflow.Tags)
            {
                tagString = tagString + item + ";";
            }
            

            //
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                //
                // First check to see if the version number is
                StoreActivity sa = (from s in proxy.StoreActivities
                                        where s.Name.Equals(workflow.Name, StringComparison.OrdinalIgnoreCase) &&
                                        s.Version.Equals(versionString, StringComparison.OrdinalIgnoreCase)
                                        select s).FirstOrDefault();

                if (sa == null)
                {
                    //new record, use new StoreActivity to get automatic
                    //list of properties
                    sa = new StoreActivity()
                    {
                        Id = workflow.ID,
                        Name = workflow.Name,
                        Version = versionString,
                        AuthGroup = "", // authgroup currently not used, but read workflow fails when this is null
                        Description = workflow.Description,
                        CategoryId = workflow.Category.Id,
                        WorkflowType = workflow.WorkFlowType.id,
                        ActivityLibrary = null, // activity library  not valid for a new workflow
                        AuthGroupId = null,
                        BaseType = "Activity",
                        IsCodeBeside = false,   // new workflows should not have code beside, developer
                                                // xaml with code beside should be saved using Store_Activity_Save()
                        DefaultRender = "",     //Not valid for workflows (yet?)
                        DeveloperNotes = "",    //Not valid for workflows
                        IconName = null,        //Not valid for workflows
                        XAML = workflow.WorkFlowText,
                        IsService = workflow.IsService,
                        IsSwitch = false,       //Not valid for workflows
                        IsToolBoxActivity = false,//Not valid for workflows, use Store_Activity_Save
                        IsUxActivity = !workflow.IsNonDialog,
                        Locked = false,
                        LockedBy = "",
                        MetaTags = tagString,
                        Status = (int)workflow.Status,
                        Namespace = "",         //Not valid for workflows, use Store_Activity_Save to change
                        ToolBoxName = workflow.Name, //not really valid for workflows, but default to name
                        ToolboxTab = -1,
                        
                    
                    };
                    try
                    {
                        proxy.StoreActivities.AddObject(sa);
                        if (proxy.SaveChanges() > 0)
                            return new EditReturnValue(true, "Workflow added");
                        else
                        {
                            string msg = "Unknown error encountered saving workflow to database, Workflow " +
                                workflow.Name + " " + workflow.Version.ToString();
                            LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                            return new EditReturnValue(false, msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        editRetValue = new EditReturnValue();
                        editRetValue.IsValid = false;
                        string msg = "Unhandled exception encountered saving workflow to database, Workflow " +
                                workflow.Name + " " + workflow.Version.ToString();
                        editRetValue.AddMessage(msg);
                        string msg2 = ex.Message;
                        if (ex.InnerException == null)
                            LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                        else
                        {
                            editRetValue.AddMessage(ex.InnerException.Message);
                            msg2 = msg2 + "\r\nInnerException\r\n" + ex.InnerException.Message;
                            LogEventLog.Log2EventLog(msg + msg2, EventLogEntryType.Error);
                        }
                        
                        return  editRetValue;
                    }
                }
                else
                {
                    //Check to see if the guid is the same, if not reject the record
                    if (sa.Id.Equals(workflow.ID))
                    {
                        //
                        // Production workflows can only be marked depreciated or deleted
                        if ((Status)sa.Status == Status.Production)
                        {
                            //If the workflow is in product, the only change
                            //allowed is to change the status to a higher
                            //value status code.
                            if ((int)workflow.Status > sa.Status)
                            {
                                sa.Status = (int)workflow.Status;
                                try
                                {
                                    if (proxy.SaveChanges() > 0)
                                    {
                                        return new EditReturnValue(true, "Workflow status (only) updated from Production to "
                                            + workflow.Status.ToString());
                                    }
                                    else
                                    {
                                        string msg = "Unknown error encountered saving workflow to database, Workflow " +
                                            workflow.Name + " " + workflow.Version.ToString();
                                        LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                                        return new EditReturnValue(false, msg);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    editRetValue = new EditReturnValue();
                                    editRetValue.IsValid = false;
                                    string msg = "Unhandled exception encountered saving workflow to database, Workflow " +
                                            workflow.Name + " " + workflow.Version.ToString();
                                    editRetValue.AddMessage(msg);
                                    string msg2 = ex.Message;
                                    if (ex.InnerException == null)
                                        LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                                    else
                                    {
                                        editRetValue.AddMessage(ex.InnerException.Message);
                                        msg2 = msg2 + "\r\nInnerException\r\n" + ex.InnerException.Message;
                                        LogEventLog.Log2EventLog(msg + msg2, EventLogEntryType.Error);
                                    }

                                    return editRetValue;
                                }
                            }
                            else
                            {
                                editRetValue = new EditReturnValue(false,"Workflows in production can only be changed to deleted or depreciated");
                                LogEventLog.Log2EventLog("Attempt to change a production workflow "
                                    + workflow.Name+workflow.Version, EventLogEntryType.Information);
                                return editRetValue;
                            }

                        }

                        if (!StatusFlags.XamlReadOnly((Status)sa.Status))
                        {
                            //
                            sa.XAML = workflow.WorkFlowText;
                        }

                        if (!StatusFlags.TagsReadOnly((Status)sa.Status))
                        {
                            sa.MetaTags = tagString;
                        }

                        if (!sa.WorkflowType.Equals(workflow.WorkFlowType.id))
                        {
                            //You can not change the workflow type
                            string msg = "Worflow Type can not be changed from " + sa.WorkflowType1.Name 
                                + " to " + workflow.WorkFlowType.Name +
                                    " in workflow " + 
                                    workflow.Name + " " +workflow.Version.ToString();
                            LogEventLog.Log2EventLog(msg, EventLogEntryType.Warning);
                            return new EditReturnValue(false, msg);

                        }
   
                        sa.Description = workflow.Description;
                        sa.CategoryId = workflow.Category.Id;

                        sa.DefaultRender = "";     //Not valid for workflows (yet?)

                        sa.IsService = workflow.IsService;

                        sa.IsUxActivity = !workflow.IsNonDialog;
                        sa.Locked = false;
                        sa.LockedBy = "";
                        sa.Status = (int)workflow.Status;

                        try
                        {
                            proxy.SaveChanges();

                        }
                        catch (Exception ex)
                        {

                            editRetValue = new EditReturnValue();
                            editRetValue.IsValid = false;
                            string msg = "Unhandled exception encountered saving workflow to database, Workflow " +
                                    workflow.Name + " " + workflow.Version.ToString();
                            editRetValue.AddMessage(msg);
                            string msg2 = ex.Message;
                            if (ex.InnerException == null)
                            {
                                LogEventLog.Log2EventLog(msg, EventLogEntryType.Error);
                            }
                            else
                            {
                                editRetValue.AddMessage(ex.InnerException.Message);
                                msg2 = msg2 + "\r\nInnerException\r\n" + ex.InnerException.Message;
                                LogEventLog.Log2EventLog(msg + msg2, EventLogEntryType.Error);
                            }

                            return editRetValue;
                        }
     
                        //save the changed values
                        return new EditReturnValue(true, "Workflow updated");
                    }
                    else
                    {
                        //Different Guid, reject values
                        string msg = "Workflow already exists with different ID, Name: " + workflow.Name
                            + " Version " + versionString + "\r\n Old ID:" + sa.Id.ToString()
                            + "\r\n New ID:" + workflow.ID.ToString();
                        LogEventLog.Log2EventLog("Error with WorkflowSave \r\n" + msg, EventLogEntryType.Error);
                        return new EditReturnValue(false, msg);

                    }
                }
                                        
            }
        }

        public IList<Library> LibraryGetListByName(string Name)
        {
            return Library.GetList(Name, VersionFlag.AllBuilds, false);
        }

        public Store_Activity test()
        {
            return new Store_Activity();
        }

        public IList<Activity_Category> ActivityCategoryGetList()
        {
            IList<Activity_Category> catList = new List<Activity_Category>();
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {

                IList<ActivityCategory> dblist = (from ac in proxy.ActivityCategories
                                                  select ac).ToList();

                foreach (var item in dblist)
                {
                    catList.Add(new Activity_Category()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        Name = item.Name,
                        AuthGroup = new AuthorityGroup(),
                        Metatags = new MetaTags(item.MetaTags)
                    }
                        );
                }
                return catList;
            }
        }
    }
}
