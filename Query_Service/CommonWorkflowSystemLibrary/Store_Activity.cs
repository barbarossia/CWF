using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Microsoft.Support.Workflow.Activity;
using System.ComponentModel;

namespace Microsoft.Support.Workflow
{
    [Serializable]
    [DataContract]
    public class Store_Activity : IWorkflowStoreItem, INotifyPropertyChanged
    {
        //// private bool _HasXaml = false;
        System.Version _Version = new Version();

        /// <summary>
        /// Added due to EDm to ER model conversion
        /// </summary>
        [DataMember]
        public Int32 NEWId { get; set; }
        [DataMember]
        public Guid NEWStoreActivitiesId { get; set; }

        /// <summary>
        /// Old EDm model only
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; }
        //TODO this needs to be addressed as it causes an error in the build for the new QueryServices
        //[DataMember]
        //public ToolTabNames ToolBoxTab { get; set; }
        [DataMember]
        public string ToolBoxName { get; set; }
        [DataMember]
        public string VersionString 
        {
            get { return _Version.ToString(); }
            set { _Version = new Version(value); } 
        }
        [IgnoreDataMember]
        public Version Version
        {
            get { return _Version; }
            set { _Version = value; }
        }
        //[DataMember]
        //public string AuthGroup { get; set; }
        [DataMember]
        public string Namespace { get; set; }
        [DataMember]
        public string Description { get; set; } 
        [DataMember]
        public string DeveloperNotes { get; set; }
        [DataMember]
        public Icon Icon { get; set; }
        [DataMember]
        public bool IsSwitch { get; set; }
        [DataMember]
        public bool IsService { get; set; }
        //[DataMember]
        //public bool IsPrimative { get; set; }
        [DataMember]
        public bool IsToolBoxActivity { get; set; }
        [DataMember]
        public bool IsUxActivity { get; set; }
        [DataMember]
        public bool IsCodeBeside { get; set; }
        //[DataMember]
        //public bool AllowTestInProduction { get; set; }
        [DataMember]
        public Activity_Category Category { get; set; }
        [DataMember]
        public Guid ActivityLibraryID { get; set; }
        [DataMember]
        public Status Status { get; set; }
        [DataMember]
        public string BaseType { get; set; }
        [DataMember]
        public bool HasXaml { get; set; }
        //[DataMember]
        //public MetaTags MetaTags
        //{
        ////    get;
        ////    set;
        //}
        private IList<string> _tags = new List<string>();
        /// <summary>
        /// A list of tags used to search for the workflow.  A tag can 
        /// be a single value, or a Taxonomy Key seperated from a corresponding value 
        /// by an equal sign.
        /// </summary>
        [DataMember]
        public IList<string> Tags { get { return _tags; } set { _tags = value; } }
        [DataMember]
        public Workflow_Activity_Type WorkFlowType
        {
            get;
            set;

        }

        [DataMember]
        public ObservableCollection<ActivityApplication> Applications { get; set; }
        [DataMember]
        public IList<ContextRequirement> ContextRequirements { get; set; }

        public Store_Activity()
        {
            ID = Guid.NewGuid();
            //Applications = new ObservableCollection<ActivityApplication>();
            //Type = new Workflow_Activity_Type();
            //Status = Workflow.Status.Draft;
            //Icon = new Icon();
            //Category = new Activity_Category();
            ActivityLibraryID = Guid.Empty;
            
        }

        public EditReturnValue Edit()
        {
            EditReturnValue retVal = EditName();

            if (!retVal.IsValid)
            {
                return retVal;
            }

            retVal = EditDescription();
            if (retVal.IsError)
                return retVal;

            if (this.Category == null)
            {
                //
                //// Assigns Guid.Emtpy to the Category Id
                //// does not trigger an error, but does add
                //// a message to the return value.
                this.Category = Activity_Category.Unassigned();
                retVal.AddMessage(true, "Category group defaulted to Unassigned");
                retVal.IsWarning = true;
            }
            //
            //// This should not occur as we initialize to an empty value
            if (this.WorkFlowType == null)
            {
                string msg = "Workflow Type is null for workflow " + Name + Version.ToString();
                retVal.AddMessage(false, msg);
                OnInvalidWorkFlowType(new EditEventArgs(new EditReturnValue(false, msg)));
                return retVal;
            }

            if (string.IsNullOrEmpty(this.ToolBoxName) || string.IsNullOrEmpty(this.ToolBoxName.Trim()))
            {
                this.ToolBoxName = this.Name;
            }

            if (this.Tags == null)
            {
                //
                //// if the tags are null, change it to an emtpy list
                //// this should not occure as we initialize to an empty
                //// list.
                this.Tags = new List<string>();
                retVal.AddMessage(true, "Tags list was null, set to empty");
                retVal.IsWarning = true;
            }

            if (this.Applications == null)
            {
                this.Applications = new ObservableCollection<ActivityApplication>();
            }

            if (this.Icon == null)
            {
                retVal.IsWarning = true;
                retVal.AddMessage("Defaulting the default.ico");
                this.Icon = new Icon() { Name = "default.ico" };

            }

      

            return retVal;
        }

        public string TagsAsString()
        {
            string retval = "";
            foreach (var item in this.Tags)
            {
                retval = String.Concat(retval, item, ';');
            }
            return retval;
        }

        private EditReturnValue EditName()
        {
            EditReturnValue retValue = new EditReturnValue();
            if (Name == null)
            {
                retValue.AddMessage(false, "Null workflow name");
            }
            Name = Name.Trim();
            if (String.IsNullOrEmpty(Name))
            {
                retValue.AddMessage(false, "Empty workflow name");
            }
            if (Name.Length > 50)
            {
                retValue.AddMessage(false, "Workflow name longer than 50 characters");
                retValue.AddMessage("Name: " + Name + " is " + Name.Length.ToString() + " characters long");

            }
            if (!retValue.IsValid)
            {
                OnInvalidName(new EditEventArgs(retValue));

            }

            return retValue;
        }

        public EditReturnValue EditDescription()
        {
            EditReturnValue retvalue = new EditReturnValue();
            //
            if (this.Description == null)
            {
                //
                //// If the description is null, set it to an emtpy string and return a warning
                retvalue.AddMessage(true, "Description was null for workflow " + this.Name + ", " + this.Version);
                Description = "";
                retvalue.IsWarning = true;
                OnInvalidDescription(new EditEventArgs(retvalue));
                return retvalue;
            }
            //Make sure the description is not only white spaces
            this.Description = this.Description.Trim();
            if (string.IsNullOrEmpty(this.Description) || this.Description.Length == 0)
            {
                //
                //// If the description is empty,  return a warning
                retvalue.AddMessage(true, "Description is empty for workflow " + this.Name + ", " + this.Version);
                retvalue.IsWarning = true;
                OnInvalidDescription(new EditEventArgs(retvalue));
                return retvalue;
            }
            //
            //// Check to make sure the description is not too long
            if (this.Description.Length > 250)
            {
                //If the string is too long, return an error.
                retvalue.AddMessage(false, "Description is longer than 250 characters for workflow " + this.Name + ", " + this.Version);
                retvalue.AddMessage(this.Description + "is truncated to");
                this.Description = this.Description.Substring(0, 250);
                retvalue.AddMessage(this.Description);
                OnInvalidDescription(new EditEventArgs(retvalue));

                return retvalue;
            }
            return retvalue;
        }

        #region Class Events

        protected void OnInvalidWorkFlowType(EventArgs e)
        {
            EventHandler handler = InvalidWorkFlowType;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raised if the the workflowtype is null or 
        /// otherwise invalid
        /// </summary>
        public event EventHandler InvalidWorkFlowType;

        protected void OnInvalidDescription(EventArgs e)
        {
            EventHandler handler = InvalidDescription;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raised if the description is empty, longer than the allowed
        /// length, or contains invalid characters.
        /// </summary>
        public event EventHandler InvalidDescription;

        protected void OnInvalidVersion(EventArgs e)
        {
            EventHandler handler = InvalidVersion;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler InvalidVersion;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }


        protected void OnInvalidName(EventArgs e)
        {
            EventHandler handler = InvalidName;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler InvalidName;

        protected void OnStatusChanged(EventArgs e)
        {
            EventHandler handler = StatusChanged;
            if (handler != null)
                handler(this, e);
        }



        /// <summary>
        /// Event raised whenever the Status Code has changed
        /// on the workflow.
        /// </summary>
        public event EventHandler StatusChanged;


        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }

    [DataContract]
    public class ActivityApplication : WFApplication
    {
        [DataMember]
        public Guid ActivityAppID { get; set; }
        [DataMember]
        public Status Status { get; set; }
        [DataMember]
        public bool IsSupportedInProduction { get; set; }

        public ActivityApplication()
            : base()
        {
            ActivityAppID = Guid.NewGuid();
            Status = Workflow.Status.Draft;
        }
    }
}
