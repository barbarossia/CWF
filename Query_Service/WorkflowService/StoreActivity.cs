using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow.Catalog
{
    public partial class StoreActivity
    {
        [IgnoreDataMember]
        public Version VersionNo
        {
            get { return new Version(Version); }
            set { Version = value.ToString(); }
        }

        public StoreActivity()
        {
            Id = Guid.Empty;
        }
        internal string GetXAML()
        {
            if (String.IsNullOrEmpty(this.XAML))
                return "";
            else
                return XAML;
        }

        internal static string GetXAML(Guid Id)
        {
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                StoreActivity retValue = (from sa in proxy.StoreActivities
                                          where sa.Id == Id
                                          select sa).FirstOrDefault();

                if (retValue == null)
                {
                    string msg = "StoreActivity not found for GUID: " + Id.ToString() + " in StoreActivity.GetXAML";
                    CWFHelpers.LogEventLog.Log2EventLog(msg, System.Diagnostics.EventLogEntryType.Error);
                    throw new ArgumentOutOfRangeException(msg);
                }
                else
                {
                    return retValue.GetXAML();
                }
            }
        }

        internal static string GetXAML(string Name, Version version)
        {
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                StoreActivity retValue = (from sa in proxy.StoreActivities
                                          where sa.Name.Equals(Name, StringComparison.OrdinalIgnoreCase) &&
                                          sa.VersionNo.Equals(version)
                                          select sa).FirstOrDefault();

                if (retValue == null)
                {
                    string msg = "StoreActivity not found for " + Name +","+version.ToString() + " in StoreActivity.GetXAML";
                    CWFHelpers.LogEventLog.Log2EventLog(msg, System.Diagnostics.EventLogEntryType.Error);
                    throw new ArgumentOutOfRangeException(msg);
                }
                else
                {
                    return retValue.GetXAML();
                }
            }
        }

        public StoreActivity Copy(string NewName, Version NewVersion)
        {

            StoreActivity newCopy = new StoreActivity();
            newCopy = new StoreActivity()
            {
                Id = Guid.NewGuid(),
                Name = NewName,
                VersionNo = NewVersion,
                WorkflowType = this._WorkflowType,
                WorkflowType1 = this.WorkflowType1,
                Status = (int)Microsoft.Support.Workflow.Status.Draft,
                ActivityCategory = this.ActivityCategory,
                Description = "Copy of " + this.Name +
                " " + this.VersionNo.ToString() + " " +
                this.Description,
                ActivityContexts = this.ActivityContexts,
                ActivityLibrary = Guid.Empty,
                IsCodeBeside = false,
                //AllowTestInProduction = this.AllowTestInProduction,
                DefaultRender = this.DefaultRender,
                //Icon = this.Icon,
            };

            return newCopy;
        }

        public static StoreActivity ConvertTo(Store_Activity storeactivity)
        {
            if (storeactivity == null)
                return null;

            CWFHelpers.LogEventLog.Log2EventLog("calling activity edit", System.Diagnostics.EventLogEntryType.Information);
            EditReturnValue retval = storeactivity.Edit();
            if (!retval.IsValid)
            {

                    CWFHelpers.LogEventLog.Log2EventLog(retval.DisplayString(), System.Diagnostics.EventLogEntryType.Error);
                
            }
            else if (retval.IsWarning)
            {

                CWFHelpers.LogEventLog.Log2EventLog(retval.DisplayString(), System.Diagnostics.EventLogEntryType.Warning);
                
            }

            //
            // write the incoming activity to the log
            CWFHelpers.LogEventLog.Log2EventLog("Converting store activity"+ storeactivity.Name, System.Diagnostics.EventLogEntryType.Information);
            
            StoreActivity anActivity = new StoreActivity();

            anActivity.Id = storeactivity.ID;
            anActivity.Name = storeactivity.Name;
            anActivity.Version = storeactivity.VersionString;
            anActivity.Description = storeactivity.Description;
            //MetaTags = storeactivity.MetaTags.ToString(),
            anActivity.MetaTags = storeactivity.TagsAsString();
            anActivity.IconName = storeactivity.Icon.Name;
            anActivity.IsSwitch = storeactivity.IsSwitch;
            anActivity.IsService = storeactivity.IsService;
            //IsPrimative = storeactivity.IsPrimative,
            CWFHelpers.LogEventLog.Log2EventLog("1/2 way Converting store activity" + storeactivity.Name, System.Diagnostics.EventLogEntryType.Information);

            anActivity.IsUxActivity = storeactivity.IsUxActivity;
            anActivity.DefaultRender = "";
            anActivity.CategoryId = storeactivity.Category.Id;
            anActivity.IsToolBoxActivity = storeactivity.IsToolBoxActivity;
            anActivity.VersionNo = storeactivity.Version;
            anActivity.Status = (int)storeactivity.Status;
            anActivity.WorkflowType = storeactivity.WorkFlowType.id;
            anActivity.Locked = false;
            anActivity.LockedBy = "";
            anActivity.IsCodeBeside = storeactivity.IsCodeBeside;
            //AllowTestInProduction = storeactivity.AllowTestInProduction,
            anActivity.DeveloperNotes = storeactivity.DeveloperNotes;
            anActivity.BaseType = storeactivity.BaseType;
            anActivity.Namespace = storeactivity.Namespace;
            anActivity.AuthGroup = ""; //todo review use of this field.
                
                

            
            CWFHelpers.LogEventLog.Log2EventLog("Debug Point, getting library id", System.Diagnostics.EventLogEntryType.Information);
            anActivity.ActivityLibrary = storeactivity.ActivityLibraryID; 
            
            //anActivity.ActivityApplicationXRefs = new System.Data.Objects.DataClasses.EntityCollection<ActivityApplicationXRef>();

            if (storeactivity.IsToolBoxActivity)
            {
                anActivity.ToolBoxName = storeactivity.ToolBoxName;
                anActivity.ToolboxTab = (int)storeactivity.ToolBoxTab;
            }
            else
            {
                anActivity.ToolBoxName = "";
                anActivity.ToolboxTab = 0;

            }
            CWFHelpers.LogEventLog.Log2EventLog("Debug Point, about to write apps", System.Diagnostics.EventLogEntryType.Information);
            foreach (var item in storeactivity.Applications)
            {
                anActivity.ActivityApplicationXRefs.Add
                    (
                    new ActivityApplicationXRef()
                    {
                        Id = item.ActivityAppID,
                        Activity = anActivity.Id,
                        Application = item.Id,
                        IsSupportedInProduction = item.IsSupportedInProduction,
                        Status = (int)item.Status,

                    }
                    );

            }
            CWFHelpers.LogEventLog.Log2EventLog("Debug Point, about to write context", System.Diagnostics.EventLogEntryType.Information);
            foreach (var item in storeactivity.ContextRequirements)
            {
                if (string.IsNullOrEmpty(item.Notes))
                    item.Notes = "";

                anActivity.ActivityContexts.Add(new ActivityContext()
                {
                    Id = item.Id,
                    ContextId = item.Context.Id,
                    Activity = anActivity.Id,
                    IsRequired = item.Required,
                    Notes = item.Notes,
                    ContextDirection = (int)item.Direction
                });
            }
            return anActivity;

        }

        public Store_Activity ConvertFrom()
        {
            //
            // Make sure that we pass on getting back a null value.
            if (this.Id == null)
                return null;
            else if (this.Id == Guid.Empty)
                return null;

            

            Store_Activity acwfa = new Store_Activity()
            {
                ID = this.Id,
                Name = this.Name,
                Version = this.VersionNo,
                Description = this.Description,
                //MetaTags = new Microsoft.Support.Workflow.MetaTags(this.MetaTags),//
                Tags = this.MetaTags.Split(';'),
                Status = (Status)this.Status,
                IsToolBoxActivity = (bool)this.IsToolBoxActivity,
                ActivityLibraryID = (Guid)this.ActivityLibrary,
                BaseType = this.BaseType,
                Applications = new System.Collections.ObjectModel.ObservableCollection<ActivityApplication>(),//
                Category = new Activity_Category()
                {
                    Id = this.CategoryId,
                    Name = this.ActivityCategory.Name,
                    Description = this.ActivityCategory.Description,
                    Metatags = new MetaTags(this.ActivityCategory.MetaTags),
                    AuthGroup = new AuthorityGroup()
                },
                ContextRequirements = new List<ContextRequirement>(),//
                //IsPrimative = this.IsPrimative,
                IsService = this.IsService,
                IsSwitch = this.IsSwitch,
                IsUxActivity = this.IsUxActivity,
                IsCodeBeside = (bool)this.IsCodeBeside,
                DeveloperNotes = this.DeveloperNotes,
                ToolBoxName = this.ToolBoxName,
                ToolBoxTab = (Activity.ToolTabNames)  this.ToolboxTab,
                WorkFlowType = new Workflow_Activity_Type()
                {
                    id = this.WorkflowType,
                    Name = this.WorkflowType1.Name,
                    Group = this.WorkflowType1.WorkflowGroup,
                    WorkflowTemplate = this.WorkflowType1.WorkflowTemplate,
                    PublishingWorkflow = this.WorkflowType1.PublishingWorkflow,
                    AuthorityGroup = new AuthorityGroup()
                    {
                        Name = this.WorkflowType1.AuthGroup.AuthGroup1
                    },
                    SelectionWorkflow = this.WorkflowType1.SelectionWorkflow,
                    ContextVariable = this.WorkflowType1.ContextVariable,
                    HandleVariable = this.WorkflowType1.HandleVariable,
                    PageViewVariable = this.WorkflowType1.PageViewVariable
                },
                Icon = new Microsoft.Support.Workflow.Icon(),
                

            };

            acwfa.HasXaml = (this.XAML.Trim().Length > 0);
             
            foreach (var item in this.ActivityApplicationXRefs)
            {
                acwfa.Applications.Add(
                    new Microsoft.Support.Workflow.ActivityApplication()
                    {
                        ActivityAppID = item.Id,
                        Id = item.Application1.Id,
                        Name = item.Application1.Name,
                        ApplicationType = ApplicationTypes.Other,
                        Description = item.Application1.Description,
                        Status = (Status)item.Status,
                        IsSupportedInProduction = item.IsSupportedInProduction

                    });

            }

            foreach (var item in this.ActivityContexts)
            {
                acwfa.ContextRequirements.Add(
                    new ContextRequirement()
                    {
                        Id = item.Id,
                        Direction = (ContextDirection)item.ContextDirection,
                        Notes = item.Notes,
                        Required = (bool)item.IsRequired,
                        Context = new xxContextItem()
                        {
                            Id = item.ContextId,
                            Name = item.Context_1.Name,
                            BusinessImpact = (BusinessImpact)Enum.Parse(typeof(BusinessImpact), item.Context_1.BusinessImpactClassification),
                            Category = new xxContextCategory()
                            {
                                Id = item.Context_1.ContextCategory.Id,
                                Name = item.Context_1.ContextCategory.Name,
                                Description = item.Context_1.ContextCategory.Description,
                                PersistencePeriod = new TimeSpan(0), //todo add to database
                                MetaTags = "" //todo add to database
                            }

                        }
                    });
                //todo lookup context detail

            }




            return acwfa;
        }
    }

    public class StoreActivityNameEqual : IEqualityComparer<StoreActivity>
    {

        public bool Equals(StoreActivity x, StoreActivity y)
        {
            return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(StoreActivity obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public class StoreActivityVersionEqual : IEqualityComparer<StoreActivity>
    {

        public bool Equals(StoreActivity x, StoreActivity y)
        {
            return (x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) &&
                x.VersionNo.Major == y.VersionNo.Major &&
                x.VersionNo.Minor == y.VersionNo.Minor);
        }

        public int GetHashCode(StoreActivity obj)
        {
            return obj.Name.GetHashCode() ^ obj.VersionNo.Major ^ obj.VersionNo.Minor;
        }
    }

    public class StoreActivityRevisionEqual : IEqualityComparer<StoreActivity>
    {

        public bool Equals(StoreActivity x, StoreActivity y)
        {
            return (x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) &&
                 x.VersionNo.Equals(y.VersionNo));
        }

        public int GetHashCode(StoreActivity obj)
        {
            return obj.Name.GetHashCode() ^ obj.VersionNo.GetHashCode();
        }
    }
}