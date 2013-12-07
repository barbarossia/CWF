using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Microsoft.Support.Workflow
{

    [Serializable]
    [DataContract]
    public class Store_Workflow : IWorkflowStoreItem , INotifyPropertyChanged
    {
        private Activity_Category _Category = Activity_Category.Unassigned();

        [DataMember]
        public Activity_Category Category 
        {
            get { return _Category; }
            set { _Category = value; }
        }

        /// <summary>
        /// Two new datamembers from EDM to ER model conversion
        /// </summary>
        [DataMember]
        public Guid NEWStoreActivitiesId { get; set; }
        [DataMember]
        public Int32 NEWId { get; set; }

        /// <summary>
        /// Only valid in EDm model
        /// </summary>
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        internal string VersionString { get; set; }
        //
        //System.Version can not be serialized (creates a 
        //Fault error) in WCF 
        //private System.Version _version;
        [IgnoreDataMember]
        public Version Version 
        { 
            get
            {
                return new Version(VersionString);
            }
            set
            {
                //Version = value;
                VersionString = value.ToString();
            }
        }
        [DataMember]
        public bool IsService { get; set; }
        [DataMember]
        public bool IsNonDialog { get; set; }
        [DataMember]

        private Workflow_Activity_Type _workFlowType = new Workflow_Activity_Type();
        public Workflow_Activity_Type WorkFlowType 
        {
            get { return _workFlowType; }
            set { _workFlowType = value; }
        }

        [DataMember]
        public string Description { get; set; }
        //
        private IList<WFApplication> _applications = new List<WFApplication>();
        /// <summary>
        /// A list of applications the workflow is setup to support.
        /// </summary>
        [DataMember]
        public IList<WFApplication> Applications 
        {
            get { return _applications; }
            set { _applications = value; }
        }

        private IList<string> _tags = new List<string>();
        /// <summary>
        /// A list of tags used to search for the workflow.  A tag can 
        /// be a single value, or a Taxonomy Key seperated from a corresponding value 
        /// by an equal sign.
        /// </summary>
        [DataMember]
        public IList<string> Tags { get { return _tags; } set { _tags = value; } }
        [DataMember]
        public string WorkFlowText { get; set; }
        //[DataMember]
        //public int StatusNo { get; set; }
        [DataMember]
        public bool Locked { get; set; }
        [DataMember]
        public string LockedBy { get; set; }
        [DataMember]
        //public MetaTags MetaTags
        //{
        ////    get; set;
        //}
        private Status _status = Status.Draft;
        [DataMember]
        public Status Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
                OnStatusChanged(EventArgs.Empty);
            }
        }

        [IgnoreDataMember]
        public bool XamlIsReadOnly
        {
            get
            {
                return StatusFlags.XamlReadOnly(_status);
            }

        }

        [IgnoreDataMember]
        public bool TagsAreReadOnly
        {
            get
            {
                return StatusFlags.TagsReadOnly(_status);
            }
        }



        public Store_Workflow()
        {
            

            ID = Guid.NewGuid();
            WorkFlowType = new Workflow_Activity_Type();
            Applications = new List<WFApplication>();
            Tags = new List<string>();
            //StatusNo = (int)Workflow.Status.Draft;
            Status = Workflow.Status.Draft;
            Locked = false;
            LockedBy = "";


        }

        public EditReturnValue Edit()
        {
            EditReturnValue retVal = EditName();

            if (!retVal.IsValid)
                return retVal;

            retVal = EditDescription();

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

            

            return retVal;


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
                retvalue.AddMessage(true,"Description was null for workflow "+this.Name +", "+this.Version );
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
}
