using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data.Objects.DataClasses;

namespace Microsoft.Support.Workflow.Catalog
{
    public partial class WorkflowHeader
    {
        public static explicit operator StoreActivity(WorkflowHeader wh)
        {
            if (wh.ActivityLibrary == null)
                wh.ActivityLibrary = Guid.Empty;

            StoreActivity retValue = new StoreActivity()
            {
                Id = wh.Id,
                Name = wh.Name,
                VersionNo = wh.VersionNo,
                MetaTags = wh.MetaTags,
                IsSwitch = wh.IsSwitch,
                IsService = wh.IsSwitch,
                ActivityLibrary = wh.ActivityLibrary,
                IsUxActivity = wh.IsUxActivity,
                DefaultRender = wh.DefaultRender,
                CategoryId = wh.Category.Id,
                ActivityCategory = wh.Category,
                Status = wh.Status,
                WorkflowType = wh.WorkflowType,
                WorkflowType1 = wh.Type,
                ActivityContexts = wh.Contexts,
                ActivityApplicationXRefs = wh.Applications,
                Description = wh.Description,
                IsCodeBeside = false,
                Locked = wh.Locked,
                LockedBy = wh.LockedBy,
                XAML = wh.XAML,
                //AllowTestInProduction = wh.AllowTestInProduction,
                //Icon = null,
                AuthGroup = "",
                //IsPrimative = false,
                StatusCode = new StatusCode() {Code = wh.Status},
                IsToolBoxActivity = false,
                

            };

            return retValue;
        }

        [IgnoreDataMember]
        public Version VersionNo
        {
            get { return new Version(Version); }
            set { Version = value.ToString(); }
        }

        [DataMember]
        public WorkflowTypedb Type
        {
            get;
            set;
        }

        [DataMember]
        public EntityCollection<ActivityContext> Contexts { get; set; }

        [DataMember]
        public EntityCollection<ActivityApplicationXRef> Applications { get; set; }

        [DataMember]
        public ActivityCategory Category
        {

            get;
            set;

        }

        [DataMember]
        public bool IsCompiled
        {
            get
            {
                if (this.ActivityLibrary == null)
                    return false;
                else if (this.ActivityLibrary == Guid.Empty)
                    return false;
                else
                    return true;
            }
            set { }
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
                    string msg = "StoreActivity not found for " + Name + "," + version.ToString() + " in StoreActivity.GetXAML";
                    CWFHelpers.LogEventLog.Log2EventLog(msg, System.Diagnostics.EventLogEntryType.Error);
                    throw new ArgumentOutOfRangeException(msg);
                }
                else
                {
                    return retValue.GetXAML();
                }
            }
        }
    }


}